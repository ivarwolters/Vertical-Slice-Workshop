// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services;
using System.Collections;

namespace ServiceModelEx.ServiceFabric
{
   internal class ChannelInvokerBase<I> : RealProxy where I : class
   {
      const string ContextKey = "AsyncContext";

      readonly ChannelFactory<I> m_Factory;
      readonly ProxyRemotingInvoker m_Invoker;
      static readonly Assembly[] m_Assemblies;
      static MethodInfo m_InvokeAsyncGenDef = null;

      static ChannelInvokerBase()
      {
         m_Assemblies = AppDomain.CurrentDomain.GetAssemblies();
         MethodInfo[] infos = typeof(ChannelInvokerBase<I>).GetMethods(BindingFlags.NonPublic|BindingFlags.Instance);
         m_InvokeAsyncGenDef = infos.Single(info=>info.Name.Equals("InvokeAsync") && info.IsGenericMethod).GetGenericMethodDefinition();
      }
      public ChannelInvokerBase(Type classToProxy,ChannelFactory<I> factory,ProxyRemotingInvoker invoker) : base(classToProxy)
      {
         m_Factory = factory;
         m_Invoker = invoker;
      }

      Type FindExceptionType(string typeName)
      {
         Type type = null;
         try
         {
            IEnumerable<Assembly> assemblies = m_Assemblies.Where(assembly=>assembly.GetType(typeName) != null);
            type = assemblies.First().GetType(typeName);
            Debug.Assert(type != null,"Make sure this assembly (ServiceModelEx by default) contains the definition of the custom exception");
            Debug.Assert(type.IsSubclassOf(typeof(Exception)));
         }
         catch
         {
            type = typeof(Exception);
         }

         return type;
      }
      AggregateException ExtractException(ExceptionDetail detail)
      {
         AggregateException innerException = null;
         if(detail.InnerException != null)
         {
            innerException = ExtractException(detail.InnerException);
         }

         AggregateException exception = null;
         if(innerException is AggregateException)
         {
            exception = innerException;
         }
         else
         {
            Type type = FindExceptionType(detail.Type);
            Type[] parameters = { typeof(string),typeof(Exception) };
            ConstructorInfo info = type.GetConstructor(parameters);
            Debug.Assert(info != null,"Exception type " + detail.Type + " does not have suitable constructor");

            Exception temp = Activator.CreateInstance(type,detail.Message,innerException) as Exception;
            FieldInfo stackTrace = typeof(Exception).GetField("_stackTraceString",BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance);
            if(stackTrace != null)
            {
               stackTrace.SetValue(temp,detail.StackTrace);
            }
                                                  
            exception = new AggregateException(temp);
            Debug.Assert(exception != null);
         }
         return exception;
      }
      Exception EvaluateException(Exception rootException)
      {
         Exception exception = rootException;
         Exception innerException = rootException.InnerException;

         //Since we're invoking, the outer exception will always be an invocation exception.
         if(innerException == null)
         {
            exception = rootException;
         }
         else if(!(innerException is FaultException))
         {
            exception = innerException;
         }
         else
         {
            if(!(innerException is FaultException<ExceptionDetail>))
            {
               exception = innerException;
            }
            else
            {
               exception = ExtractException((innerException as FaultException<ExceptionDetail>).Detail);
            }
         }
         return exception;
      }

      void OnPreInvoke(MethodInfo method)
      {
         if(m_Invoker != null)
         {
            m_Invoker.OnPreInvoke(method);
         }
      }
      void OnPostInvoke(MethodInfo method,Exception exception)
      {
         if(m_Invoker != null)
         {
            m_Invoker.OnPostInvoke(method,exception);
         }
      }

      Task InvokeAsync(IClientChannel channel,MethodCallMessageWrapper methodCallWrapper)
      {
         return (methodCallWrapper.MethodBase.Invoke(channel,methodCallWrapper.Args) as Task).ContinueWith(result=>InvokeEnd(result,channel,methodCallWrapper));
      }
      Task<T> InvokeAsync<T>(IClientChannel channel,MethodCallMessageWrapper methodCallWrapper)
      {
         return (methodCallWrapper.MethodBase.Invoke(channel,methodCallWrapper.Args) as Task<T>).ContinueWith((result)=>InvokeEnd<T>(result,channel,methodCallWrapper));
      }
      IMessage InjectContinuation(IMessage message,IClientChannel channel,MethodCallMessageWrapper methodCallWrapper)
      {
         Task returnTask = null;
         if((methodCallWrapper.MethodBase as MethodInfo).ReturnType.IsGenericType == false)
         {
            returnTask = InvokeAsync(channel,methodCallWrapper);
         }
         else
         {
            Type returnArg = (methodCallWrapper.MethodBase as MethodInfo).ReturnType.GetGenericArguments()[0];
            MethodInfo strongInvokeAsnyc = m_InvokeAsyncGenDef.MakeGenericMethod(returnArg);
            returnTask = strongInvokeAsnyc.Invoke(this,new object[]{channel,methodCallWrapper}) as Task;
         }

         ReturnMessage taskMessage = new ReturnMessage(returnTask,null,0,methodCallWrapper.LogicalCallContext,methodCallWrapper);
         return taskMessage;
      }
      void FlowContextForTest(MethodCallMessageWrapper methodCallWrapper)
      {
         if(Test.TestHelper.IsUnderTest())
         {
            IEnumerable<AsyncContext> test = methodCallWrapper.Properties.Values.Cast<AsyncContext>();
            if(test.Any() == false)
            {
               methodCallWrapper.Properties.Add(ContextKey,new AsyncContext());
            }
            else
            {
               AsyncContext context = test.First();
               context.Restore();
            }
         }
      }
      IMessage InvokeBegin(IMessage message)
      {
         MethodCallMessageWrapper methodCallWrapper = new MethodCallMessageWrapper((IMethodCallMessage)message);

         FlowContextForTest(methodCallWrapper);

         IClientChannel channel = m_Factory.CreateChannel() as IClientChannel;
         channel.OperationTimeout = TimeSpan.MaxValue;
         channel.Open();

         OnPreInvoke(methodCallWrapper.MethodBase as MethodInfo);
         IMessage response = InjectContinuation(message,channel,methodCallWrapper);
         return response;
      }
      void InvokeEnd(Task response,IClientChannel channel,MethodCallMessageWrapper methodCallWrapper)
      {
         Exception exception = null;
         MethodInfo method = methodCallWrapper.MethodBase as MethodInfo;
         try
         {
            FlowContextForTest(methodCallWrapper);

            if(response.IsFaulted)
            {
               exception = EvaluateException(response.Exception);
               throw exception;
            }
         }
         finally
         {

            OnPostInvoke(method,exception);
            if(channel.State != CommunicationState.Closed && channel.State != CommunicationState.Faulted)
            {
               try
               {
                  channel.Close();
               }
               catch
               {
                  channel.Abort();
               }
            }
            channel = null;
         }
      }
      T InvokeEnd<T>(Task<T> response,IClientChannel channel,MethodCallMessageWrapper methodCallWrapper)
      {
         InvokeEnd(response as Task,channel,methodCallWrapper);
         return response.Result;
      }
      public override IMessage Invoke(IMessage message)
      {
         return InvokeBegin(message);
      }
   }
}
