// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Reflection;

using ServiceModelEx.ServiceFabric.Services.Communication.Client;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   public interface IProxyOperationInvoker
   {
      void PreInvoke(ICommunicationClient cient,MethodInfo operation);
      void PostInvoke(ICommunicationClient cient,MethodInfo operation,Exception exception);
   }
}
