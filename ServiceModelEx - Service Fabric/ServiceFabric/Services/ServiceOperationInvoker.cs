// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services.Remoting;
using ServiceModelEx.ServiceFabric.Services.Runtime;

#pragma warning disable 618

namespace ServiceModelEx.ServiceFabric.Services
{
   public class ServiceOperationInvoker : IOperationInvoker
   {
      readonly IOperationInvoker m_OldInvoker;
      readonly MethodInfo m_MethodInfo;
      readonly IEnumerable<ServiceRemotingDispatcher> m_Dispatchers;

      public ServiceOperationInvoker(IOperationInvoker oldInvoker,OperationDescription operationDescription,IEnumerable<ServiceRemotingDispatcher> dispatchers)
      {
         Debug.Assert(oldInvoker != null);

         m_OldInvoker = oldInvoker;
         Debug.Assert(operationDescription.TaskMethod != null,"All service operations must be asynchronous and have a return type of Task or Task<T>.");
         m_MethodInfo = operationDescription.TaskMethod;
         m_Dispatchers = dispatchers;
      }
      public virtual object[] AllocateInputs()
      {
         return m_OldInvoker.AllocateInputs();
      }
      public bool IsSynchronous
      {
         get
         {
            return m_OldInvoker.IsSynchronous;
         }
      }

      void OnPreInvoke(object instance)
      {
         if(m_Dispatchers != null)
         {
            StatelessService service = instance as StatelessService;
            if(service != null)
            {
               foreach(ServiceRemotingDispatcher dispatcher in m_Dispatchers)
               {
                  dispatcher.OnPreInvoke(service.Context,(service as IService),m_MethodInfo);
               }
            }
         }
      }
      void OnPostInvoke(object instance,Exception exception)
      {
         if(m_Dispatchers != null)
         {
            StatelessService service = instance as StatelessService;
            if(service != null)
            {
               foreach(ServiceRemotingDispatcher dispatcher in m_Dispatchers)
               {
                  dispatcher.OnPostInvoke(service.Context,(service as IService),m_MethodInfo,exception);
               }
            }
         }
      }

      public object Invoke(object instance,object[] inputs,out object[] outputs)
      {
         Debug.Assert(false,"All service operations must be asynchronous and have a return type of Task or Task<T>.");
         outputs = new object[]{};
         return null;
      }
      public IAsyncResult InvokeBegin(object instance,object[] inputs,AsyncCallback callback,object state)
      {
         IAsyncResult result = null;

         OnPreInvoke(instance);
         try
         {
            result = m_OldInvoker.InvokeBegin(instance,inputs,callback,state);
         }
         catch(Exception exception)
         {
            Debug.Assert(true,"Invoker exception: " + exception.Message);
            throw exception;
         }
         return result;
      }

      Exception PreserveOriginalException(Exception source)
      {
         Exception result = null;
         MemoryStream m_Stream = new MemoryStream();
         BinaryFormatter formatter = new BinaryFormatter();

         m_Stream.Position = 0;
         using (m_Stream)
         {
               formatter.Serialize(m_Stream, source);
               m_Stream.Position = 0;
               result = formatter.Deserialize(m_Stream) as Exception;
         }
         return result;
      }
      public object InvokeEnd(object instance,out object[] outputs,IAsyncResult result)
      {
         object returnedValue = null;
         object[] outputParams = {};
         Exception exception = null;

         try
         {
            Task task = (result as Task);
            if(task.Status == TaskStatus.Faulted)
            {
               //Preserve original stack trace.
               exception = PreserveOriginalException(task.Exception);
            }
            returnedValue = m_OldInvoker.InvokeEnd(instance,out outputs,result);
            outputs = outputParams;

            return returnedValue;
         }
         catch
         {
            Debug.Assert(true,"Invoker End exception: " + exception.Message);
            throw exception;
         }
         finally
         {
            OnPostInvoke(instance,exception);
         }
      }
   }
}

#pragma warning restore 618
