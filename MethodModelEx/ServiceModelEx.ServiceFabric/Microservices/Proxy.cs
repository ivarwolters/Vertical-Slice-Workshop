// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.ServiceFabric.Actors;
using ServiceModelEx.ServiceFabric.Actors.Client;
using ServiceModelEx.ServiceFabric.Actors.Runtime;
using ServiceModelEx.ServiceFabric.Services.Remoting;
using ServiceModelEx.ServiceFabric.Services.Runtime;
using ServiceModelEx.ServiceFabric.Extensions.Services;
using ServiceModelEx.ServiceFabric.Extensions.Client;
#else
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Runtime;
using ServiceFabricEx.Client;
using ServiceFabricEx.Services;
#endif

namespace MethodModelEx.Microservices
{
   public static class Proxy
   {
      public static void SetHeaderFlowPolicy(HeaderFlowManager policy)
      {
         ProxyFactory.AddInterceptors(new ProxyHeaderFlowInterceptor(policy));
      }

      static I Create<I>(Uri serviceAddress,IHeaderAccessor accessor) where I : class,IService
      {
         Debug.Assert(typeof(I).IsInterface);
         return ProxyFactory.Create<I>(serviceAddress,Naming.Listener<I>(),accessor);
      }
      static I Create<I>(Uri serviceAddress,Action<IHeaderAccessor> addHeaders,Action<IHeaderAccessor> getHeaders) where I : class,IService
      {
         Debug.Assert(typeof(I).IsInterface);
         return ProxyFactory.Create<I>(serviceAddress,Naming.Listener<I>(),addHeaders,getHeaders);
      }
      public static I ForMicroservice<I>() where I : class,IService
      {
         Debug.Assert(typeof(I).Namespace.Contains("Manager"), "Invalid microservice call. Use only the Manager interface to access a microservice.");
         return Create<I>(Addressing.Microservice<I>(),null);
      }
      public static I ForMicroservice<I>(IHeaderAccessor accessor) where I : class,IService
      {
         Debug.Assert(typeof(I).Namespace.Contains("Manager"), "Invalid microservice call. Use only the Manager interface to access a microservice.");
         return Create<I>(Addressing.Microservice<I>(),accessor);
      }
      public static I ForMicroservice<I>(Action<IHeaderAccessor> addHeaders,Action<IHeaderAccessor> getHeaders) where I : class,IService
      {
         Debug.Assert(typeof(I).Namespace.Contains("Manager"), "Invalid microservice call. Use only the Manager interface to access a microservice.");
         return Create<I>(Addressing.Microservice<I>(),addHeaders,getHeaders);
      }
      public static I ForComponent<I>(StatelessService caller) where I : class,IService
      {
         Debug.Assert(caller != null,"Invalid component call. Must supply stateless service caller.");
         return Create<I>(Addressing.Component<I>(caller),null);
      }

      static I Create<I>(Uri actorAddress,string instanceName,IHeaderAccessor accessor)  where I : class,IActor
      {
         Debug.Assert(typeof(I).IsInterface);
         return ProxyFactory.Create<I>(actorAddress,new ActorId(instanceName),accessor);
      }
      static I Create<I>(Uri actorAddress,string instanceName,Action<IHeaderAccessor> addHeaders,Action<IHeaderAccessor> getHeaders) where I : class,IActor
      {
         Debug.Assert(typeof(I).IsInterface);
         return ProxyFactory.Create<I>(actorAddress,new ActorId(instanceName),addHeaders,getHeaders);
      }
      public static I ForAccessor<I>(string instanceName) where I : class,IActor
      {
         Debug.Assert(typeof(I).Namespace.Contains("Accessor"), "Invalid resource call. Use only the accesor interface to access a resource.");
         return Create<I>(Addressing.Accessor<I>(),instanceName,null);
      }
      public static I ForAccessor<I>(string instanceName,IHeaderAccessor accessor) where I : class,IActor
      {
         Debug.Assert(typeof(I).Namespace.Contains("Accessor"), "Invalid resource call. Use only the accesor interface to access a resource.");
         return Create<I>(Addressing.Accessor<I>(),instanceName,accessor);
      }
      public static I ForAccessor<I>(string instanceName,Action<IHeaderAccessor> addHeaders,Action<IHeaderAccessor> getHeaders) where I : class,IActor
      {
         Debug.Assert(typeof(I).Namespace.Contains("Accessor"), "Invalid resource call. Use only the accesor interface to access a resource.");
         return Create<I>(Addressing.Accessor<I>(),instanceName,addHeaders,getHeaders);
      }
      public static I ForAccessor<I>(string actorName,string instanceName) where I : class,IActor
      {
         Debug.Assert(typeof(I).Namespace.Contains("Accessor"), "Invalid resource call. Use only the accesor interface to access a resource.");
         return Create<I>(Addressing.Accessor<I>(actorName),instanceName,null);
      }
      public static I ForAccessor<I>(string actorName,string instanceName,IHeaderAccessor accessor) where I : class,IActor
      {
         Debug.Assert(typeof(I).Namespace.Contains("Accessor"), "Invalid resource call. Use only the accesor interface to access a resource.");
         return Create<I>(Addressing.Accessor<I>(actorName),instanceName,accessor);
      }
      public static I ForAccessor<I>(string actorName,string instanceName,Action<IHeaderAccessor> addHeaders,Action<IHeaderAccessor> getHeaders) where I : class,IActor
      {
         Debug.Assert(typeof(I).Namespace.Contains("Accessor"), "Invalid resource call. Use only the accesor interface to access a resource.");
         return Create<I>(Addressing.Accessor<I>(actorName),instanceName,addHeaders,getHeaders);
      }
      public static I ForNode<I>(string instanceName,Actor caller) where I : class,IActor
      {
         Debug.Assert(caller != null,"Invalid node call. Must supply stateful actor caller.");
         return Create<I>(Addressing.Node<I>(caller),instanceName,null);
      }
      public static I ForNode<I>(string actorName,string instanceName,Actor caller) where I : class,IActor
      {
         Debug.Assert(caller != null,"Invalid node call. Must supply stateful actor caller.");
         return Create<I>(Addressing.Node<I>(caller,actorName),instanceName,null);
      }
   }
}
