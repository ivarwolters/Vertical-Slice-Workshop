// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using ServiceModelEx.ServiceFabric.Services.Remoting;
using ServiceModelEx.ServiceFabric.Services.Remoting.Client;

using ServiceModelEx.ServiceFabric.Extensions.Services;

namespace ServiceModelEx.ServiceFabric.Extensions.Client
{
   public static partial class ProxyFactory
   {
      class InvokerComparer : IEqualityComparer<IProxyOperationInvoker>
      {
         public bool Equals(IProxyOperationInvoker x,IProxyOperationInvoker y)
         {
            return x.GetType().Equals(y);
         }
         public int GetHashCode(IProxyOperationInvoker obj)
         {
            return obj.GetHashCode();
         }
      }
      class InterceptorComparer : IEqualityComparer<IProxyHeaderInterceptor>
      {
         Type GetTypeToCompare(IProxyHeaderInterceptor interceptor)
         {
            if(interceptor is ProxyHeaderFlowInterceptor)
            {
               return (interceptor as ProxyHeaderFlowInterceptor).Manager.GetType();
            }
            else
            {
               return interceptor.GetType();
            }
         }

         public bool Equals(IProxyHeaderInterceptor x,IProxyHeaderInterceptor y)
         {
            return GetTypeToCompare(x).Equals(GetTypeToCompare(y));
         }
         public int GetHashCode(IProxyHeaderInterceptor obj)
         {
            return GetTypeToCompare(obj).GetHashCode();
         }
      }

      static List<IProxyHeaderInterceptor> Interceptors
      {get;set;}
      static List<IProxyOperationInvoker> Invokers
      {get;set;}
      static ProxyFactory()
      {
         Interceptors = new List<IProxyHeaderInterceptor>();
         Invokers = new List<IProxyOperationInvoker>();
         AddInterceptors(new ProxyHeaderFlowInterceptor(new FrameworkFlowPolicy()));
      }

      public static void AddInterceptors(params IProxyHeaderInterceptor[] inspectors)
      {
         Interceptors = Interceptors.Union(inspectors).Distinct(new InterceptorComparer()).ToList();
      }
      public static void AddInvokers(params IProxyOperationInvoker[] invokers)
      {
         Invokers = Invokers.Union(invokers).Distinct(new InvokerComparer()).ToList();
      }
      public static I Create<I>(Uri serviceAddress,string listenerName,IHeaderAccessor accessor) where I : class,IService
      {
         string applicationName = string.Empty,
                serviceName = string.Empty;

         Debug.Assert(listenerName.Equals(typeof(I).Name));

         AddressHelper.EvaluateAddress(serviceAddress,out applicationName,out serviceName);
         return ServiceProxy.Create<I>(applicationName,serviceName,new ProxyOperationInvoker(Invokers),new ProxyMessageInterceptor(Interceptors,accessor));
      }
      public static I Create<I>(Uri serviceAddress,string listenerName,Action<IHeaderAccessor> addHeaders,Action<IHeaderAccessor> getHeaders) where I : class,IService
      {
         string applicationName = string.Empty,
                serviceName = string.Empty;

         Debug.Assert(listenerName.Equals(typeof(I).Name));

         AddressHelper.EvaluateAddress(serviceAddress,out applicationName,out serviceName);
         return ServiceProxy.Create<I>(applicationName,serviceName,new ProxyOperationInvoker(Invokers),new ProxyMessageInterceptor(Interceptors,addHeaders,getHeaders));
      }
   }
}
