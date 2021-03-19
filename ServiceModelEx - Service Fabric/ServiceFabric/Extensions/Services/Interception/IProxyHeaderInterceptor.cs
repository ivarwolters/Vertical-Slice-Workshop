// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   public interface IProxyHeaderInterceptor
   {
      void Receiving(ServiceRemotingMessageHeaders headers);
      void Sending(ServiceRemotingMessageHeaders headers);
   }
}
