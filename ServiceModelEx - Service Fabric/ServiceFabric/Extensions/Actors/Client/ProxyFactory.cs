// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;

using ServiceModelEx.ServiceFabric.Actors;
using ServiceModelEx.ServiceFabric.Actors.Client;

using ServiceModelEx.ServiceFabric.Extensions.Services;

namespace ServiceModelEx.ServiceFabric.Extensions.Client
{
   public static partial class ProxyFactory
   {
      public static I Create<I>(Uri serviceAddress,ActorId id,IHeaderAccessor accessor) where I : class,IActor
      {
         string applicationName = string.Empty,
                serviceName = string.Empty;

         AddressHelper.EvaluateAddress(serviceAddress,out applicationName,out serviceName);
         return ActorProxy.Create<I>(id,applicationName,serviceName,new ProxyOperationInvoker(Invokers),new Extensions.Services.ProxyMessageInterceptor(Interceptors,accessor));
      }
      public static I Create<I>(Uri serviceAddress,ActorId id,Action<IHeaderAccessor> addHeaders,Action<IHeaderAccessor> getHeaders) where I : class,IActor
      {
         string applicationName = string.Empty,
                serviceName = string.Empty;

         AddressHelper.EvaluateAddress(serviceAddress,out applicationName,out serviceName);
         return ActorProxy.Create<I>(id,applicationName,serviceName,new ProxyOperationInvoker(Invokers),new Extensions.Services.ProxyMessageInterceptor(Interceptors,addHeaders,getHeaders));
      }
   }
}
