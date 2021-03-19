// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Actors.Runtime;
using ServiceModelEx.ServiceFabric.Services;
using ServiceModelEx.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace ServiceModelEx.ServiceFabric.Actors.Remoting.FabricTransport.Runtime
{
   public class FabricTransportActorServiceRemotingListener : FabricTransportServiceRemotingListener
   {
      public FabricTransportActorServiceRemotingListener(ActorService actorService,FabricTransportRemotingListenerSettings listenerSettings = null) : base(actorService.Context,actorService,listenerSettings)
      {}
      public FabricTransportActorServiceRemotingListener(ServiceContext serviceContext,ServiceRemotingDispatcher dispatcher,FabricTransportRemotingListenerSettings listenerSettings = null) : base(serviceContext,dispatcher,listenerSettings)
      {}
   }
}
