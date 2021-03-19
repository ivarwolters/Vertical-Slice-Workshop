// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Reflection;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services;
using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   public class ServiceOperationDispatcher : ServiceRemotingDispatcher
   {
      protected IEnumerable<IServiceOperationInvoker> Invokers
      {get;set;}
      protected IEnumerable<IServiceHeaderInterceptor> Interceptors
      {get;set;}

      public ServiceOperationDispatcher(ServiceContext serviceContext,IService service,IEnumerable<IServiceHeaderInterceptor> interceptors) : this(serviceContext,service,interceptors,null)
      {}
      public ServiceOperationDispatcher(ServiceContext serviceContext,IService service,IEnumerable<IServiceOperationInvoker> invokers) : this(serviceContext,service,null,invokers)
      {}
      public ServiceOperationDispatcher(ServiceContext serviceContext,IService service,IEnumerable<IServiceHeaderInterceptor> interceptors,IEnumerable<IServiceOperationInvoker> invokers)
      {
         List<IServiceHeaderInterceptor> interceptorList = new List<IServiceHeaderInterceptor>(interceptors ?? new IServiceHeaderInterceptor[0]);
         interceptorList.Add(new ServiceHeaderFlowInterceptor(new FrameworkFlowPolicy()));
         Interceptors = interceptorList;
         Invokers = invokers;
      }

      internal override void OnPreInvoke(ServiceContext context,IService service,MethodInfo operation)
      {
         if(Invokers != null)
         {
            foreach(IServiceOperationInvoker invoker in Invokers)
            {
               invoker.PreInvoke(context,service,operation);
            }
         }
      }
      internal override void OnPostInvoke(ServiceContext context,IService service,MethodInfo operation,Exception exception)
      {
         if(Invokers != null)
         {
            foreach(IServiceOperationInvoker invoker in Invokers)
            {
               invoker.PostInvoke(context,service,operation,exception);
            }
         }
      }
      internal override void OnReceiving(ServiceContext context,MessageHeaders messageHeaders)
      {
         if(Interceptors != null)
         {
            foreach(IServiceHeaderInterceptor interceptor in Interceptors)
            {
               ServiceRemotingMessageHeaders headers = Header<ServiceRemotingMessageHeaders>.Get(messageHeaders);
               interceptor.Receiving(context,headers,messageHeaders.Action);
               Header<ServiceRemotingMessageHeaders>.Replace(headers,messageHeaders);
            }
         }
      }
      internal override void OnSending(ServiceContext context,MessageHeaders messageHeaders)
      {
         if(Interceptors != null)
         {
            foreach(IServiceHeaderInterceptor interceptor in Interceptors)
            {
               ServiceRemotingMessageHeaders headers = Header<ServiceRemotingMessageHeaders>.Get(messageHeaders);
               interceptor.Sending(context,headers,messageHeaders.Action);
               Header<ServiceRemotingMessageHeaders>.Replace(headers,messageHeaders);
            }
         }
      }
   }
}
