// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   public interface IServiceHeaderInterceptor
   {
      void Receiving(ServiceContext context,ServiceRemotingMessageHeaders headers,string action);
      void Sending(ServiceContext context,ServiceRemotingMessageHeaders headers,string action);
   }
}
