// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.ServiceModel;
using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   public sealed class ProxyHeaderFlowInterceptor : IProxyHeaderInterceptor
   {
      internal HeaderFlowManager Manager
      {get;set;}

      public ProxyHeaderFlowInterceptor(HeaderFlowManager manager)
      {
         Manager = manager;
      }
      public void Receiving(ServiceRemotingMessageHeaders headers)
      {
         if(Manager.CanCopyHeaders())
         {
            Manager.CopyCommonHeadersFrom(headers);
         }
      }
      public void Sending(ServiceRemotingMessageHeaders headers)
      {
         if(OperationContext.Current == null)
         {
            Manager.AddCommonHeaders(headers);
         }
         else
         {
            Manager.CopyCommonHeadersTo(headers);
         }
      }
   }
}
