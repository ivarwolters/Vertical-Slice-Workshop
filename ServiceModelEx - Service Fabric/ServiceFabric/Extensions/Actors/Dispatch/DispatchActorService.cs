// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Collections.Generic;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Actors.Remoting.FabricTransport.Runtime;
using ServiceModelEx.ServiceFabric.Actors.Runtime;
using ServiceModelEx.ServiceFabric.Services.Communication.Runtime;
using ServiceModelEx.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

using ServiceModelEx.ServiceFabric.Extensions.Services;

namespace ServiceModelEx.ServiceFabric.Extensions.Actors
{
   public class DispatchActorService : ActorService
   {
      IEnumerable<IServiceOperationInvoker> Invokers
      {get;set;}
      IEnumerable<IServiceHeaderInterceptor> Interceptors
      {get;set;}

      public DispatchActorService(StatefulServiceContext context,ActorTypeInformation actorTypeInfo) : this(context,actorTypeInfo,null,null)
      {}
      public DispatchActorService(StatefulServiceContext context,ActorTypeInformation actorTypeInfo,IEnumerable<IServiceHeaderInterceptor> interceptors) : this(context,actorTypeInfo,interceptors,null)
      {}
      public DispatchActorService(StatefulServiceContext context,ActorTypeInformation actorTypeInfo,IEnumerable<IServiceOperationInvoker> invokers) : this(context,actorTypeInfo,null,invokers)
      {}
      public DispatchActorService(StatefulServiceContext context,ActorTypeInformation actorTypeInfo,IEnumerable<IServiceHeaderInterceptor> interceptors,IEnumerable<IServiceOperationInvoker> invokers) : base(context,actorTypeInfo)
      {
         Interceptors = interceptors;
         Invokers = invokers;
      }

      protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
      {
         return new []
         {
            new ServiceReplicaListener((context)=>new FabricTransportActorServiceRemotingListener(context,new ActorOperationDispatcher(context,this,Interceptors,Invokers),new FabricTransportRemotingListenerSettings()))
         };
      }
   }
}
