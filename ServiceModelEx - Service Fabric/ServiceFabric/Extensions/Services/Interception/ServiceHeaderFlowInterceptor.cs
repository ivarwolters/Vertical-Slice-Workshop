// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   public sealed class ServiceHeaderFlowInterceptor : IServiceHeaderInterceptor
   {
      HeaderFlowManager Manager
      {get;set;}

      public ServiceHeaderFlowInterceptor(HeaderFlowManager manager)
      {
         Manager = manager;
      }
      //Do nothing on receive. The headers are already in the IncomingMessageHeaders collection.
      public void Receiving(ServiceContext context,ServiceRemotingMessageHeaders headers,string action)
      {
         if(action.Contains("fault") == false)
         {
            if(OriginationHelper.Get(headers) == null)
            {
               string[] segments = action.Split(new char[] {'/'},StringSplitOptions.RemoveEmptyEntries);
               throw new InvalidOperationException("Invalid ServiceFabricEx extensibility sequence. Service " + context.ServiceName + " called without proper extensibility proxy for operation " + segments[3] + " on contract " + segments[2] + ".");
            }
         }
      }
      public void Sending(ServiceContext context,ServiceRemotingMessageHeaders headers,string action)
      {
         Manager.CopyCommonHeadersTo(headers);
      }
   }
}
