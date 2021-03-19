// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Reflection;
using System.ServiceModel.Channels;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Services
{
   public abstract class ServiceRemotingDispatcher
   {
      internal abstract void OnPreInvoke(ServiceContext context,IService service,MethodInfo operation);
      internal abstract void OnPostInvoke(ServiceContext context,IService service,MethodInfo operation,Exception exception);
      internal abstract void OnReceiving(ServiceContext context,MessageHeaders messageHeaders);
      internal abstract void OnSending(ServiceContext context,MessageHeaders messageHeaders);
   }
}
