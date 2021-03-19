// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel.Channels;

namespace ServiceModelEx.ServiceFabric.Services
{
   internal abstract class ProxyRemotingInterceptor
   {
      internal abstract void OnAddHeaders(MessageHeaders messageHeaders);
      internal abstract void OnGetHeaders(MessageHeaders messageHeaders);
      internal abstract void OnReceiving(Uri serviceAddress,MessageHeaders messageHeaders);
      internal abstract void OnSending(Uri serviceAddress,MessageHeaders messageHeaders);
   }
}
