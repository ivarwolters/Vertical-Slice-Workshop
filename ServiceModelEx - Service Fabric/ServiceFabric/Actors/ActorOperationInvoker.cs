// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Actors.Runtime;
using ServiceModelEx.ServiceFabric.Services;
using ServiceModelEx.ServiceFabric.Services.Remoting;
using System.Runtime.Remoting.Messaging;

#pragma warning disable 618

namespace ServiceModelEx.ServiceFabric.Actors
{
   public class ActorOperationInvoker : IOperationInvoker
   {
      readonly IOperationInvoker m_OldInvoker;
      readonly MethodInfo m_MethodInfo;
      readonly IEnumerable<ServiceRemotingDispatcher> m_Dispatchers;

      //TEST ONLY...
      AsyncContextScope ContextScope = null;

      public ActorOperationInvoker(IOperationInvoker oldInvoker,OperationDescription operationDescription,IEnumerable<ServiceRemotingDispatcher> dispatchers)
      {
         Debug.Assert(oldInvoker != null);

         m_OldInvoker = oldInvoker;
         Debug.Assert(operationDescription.TaskMethod != null,"All actor operations must be asynchronous and have a return type of Task or Task<T>.");
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

      bool CompletesActorInstance(object instance)
      {
         if(instance is IStatefulActorManagement)
         {
            if(m_MethodInfo.GetCustomAttribute<CompletesActorInstanceAttribute>() != null)
            {
               return true;
            }
         }
         return false;
      }
      bool IsInfrastructureEndpoint(object instance)
      {
         return typeof(IActor).GetMembers().Any(info=>info.Name.Equals(m_MethodInfo.Name)) ||
                typeof(IStatefulActorManagement).GetMembers().Any(info=>info.Name.Equals(m_MethodInfo.Name)) ||
                CompletesActorInstance(instance);
      }
      void OnPreInvoke(object instance)
      {
         if(m_Dispatchers != null && instance != null)
         {
            ActorService service = (instance as ActorBase).ActorService;
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
         if(m_Dispatchers != null && instance != null)
         {
            ActorService service = (instance as ActorBase).ActorService;
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
         Debug.Assert(false,"All actor operations must be asynchronous and have a return type of Task or Task<T>.");
         outputs = new object[]{};
         return null;
      }
      public IAsyncResult InvokeBegin(object instance,object[] inputs,AsyncCallback callback,object state)
      {
         IAsyncResult result = null;
         OnPreInvoke(instance);
         try
         {
            if(IsInfrastructureEndpoint(instance))
            {
               //TEST ONLY...
               if(CompletesActorInstance(instance))
               {
                  CallContext.LogicalSetData("Context",new AsyncContextScope());
               }
               result = m_OldInvoker.InvokeBegin(instance,inputs,callback,state);
            }
            else
            {
               //Implicitly activate instance:
               //1. Upon every stateless actor request
               //2. Upon first application request after state collection
               if((m_MethodInfo.Name.Contains("Deactivate") == false)  && ((instance as ActorBase).Activated == false)) 
               {
                   (instance as ActorBase).ActivateAsync().Wait();
               }

               result = m_OldInvoker.InvokeBegin(instance,inputs,callback,state);
            }
         }
         catch(Exception exception)
         {
            Debug.Assert(true,"Invoker Begin exception: " + exception.Message);
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
      Exception CheckForActorNotFoundException(object instance,Exception source)
      {
         Exception result = source;
         if(source.InnerException != null && source.InnerException.Message.Equals("The service implementation object was not initialized or is not available."))
         {
            if(instance == null)
            {
               result = new InvalidOperationException("An actor could not be initialzed or was not found in the state store. To switch from StatePersistence.Persistent to StatePersistence.Volatile, first delete the PersistentActorIds.bin in the client working folder.");
            }
         }
         return result;
      }
      void CheckCompletesActorInstance(object instance)
      {
         if(CompletesActorInstance(instance))
         {
            (instance as IStatefulActorManagement).CompleteAsync().Wait();
         }
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
            //TEST ONLY...
            if(CompletesActorInstance(instance))
            {
               AsyncContextScope scope = CallContext.LogicalGetData("Context") as AsyncContextScope;
               if(scope != null)
               {
                  scope.Close();
               }
            }

            CheckCompletesActorInstance(instance);
            returnedValue = m_OldInvoker.InvokeEnd(instance,out outputs,result);
            outputs = outputParams;

            return returnedValue;
         }
         catch
         {
            Debug.Assert(true,"Invoker End exception: " + exception.Message);
            throw CheckForActorNotFoundException(instance,exception);
         }
         finally
         {
            OnPostInvoke(instance,exception);
         }
      }
   }
}

#pragma warning restore 618