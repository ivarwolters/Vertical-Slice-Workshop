// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Threading;
using System.Threading.Tasks;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services.Communication.Runtime;
using ServiceModelEx.ServiceFabric.Services.Remoting.Runtime;

namespace ServiceModelEx.ServiceFabric.Services.Remoting.FabricTransport.Runtime
{
   public class FabricTransportServiceRemotingListener : IServiceRemotingListener,ICommunicationListener
   {
      public ServiceRemotingDispatcher Dispatcher
      {get;set;}

      public FabricTransportServiceRemotingListener(ServiceContext serviceContext,IService serviceImplementation,FabricTransportRemotingListenerSettings listenerSettings)
      {}
      public FabricTransportServiceRemotingListener(ServiceContext serviceContext,ServiceRemotingDispatcher dispatcher,FabricTransportRemotingListenerSettings listenerSettings)
      {
         Dispatcher = dispatcher;
      }
      public Task<string> OpenAsync(CancellationToken cancellationToken)
      {
         return Task.FromResult(string.Empty);
      }
      public Task CloseAsync(CancellationToken cancellationToken)
      {
         return Task.FromResult(true);
      }
      public void Abort()
      {}
   }
}
