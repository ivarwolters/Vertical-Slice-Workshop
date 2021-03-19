// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using ServiceModelEx.ServiceFabric.Services.Communication.Runtime;

namespace ServiceModelEx.ServiceFabric.Services.Remoting.Runtime
{
   public interface IServiceRemotingListener : ICommunicationListener
   {
      ServiceRemotingDispatcher Dispatcher
      {get;set;}
   }
}
