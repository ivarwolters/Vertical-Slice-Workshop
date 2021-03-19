// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Reflection;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   public interface IServiceOperationInvoker
   {
      void PreInvoke(ServiceContext context,IService instance,MethodInfo operation);
      void PostInvoke(ServiceContext context,IService instance,MethodInfo operation,Exception exception);
   }
}
