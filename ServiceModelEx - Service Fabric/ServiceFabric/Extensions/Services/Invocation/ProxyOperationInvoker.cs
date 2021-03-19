// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.Reflection;

using ServiceModelEx.ServiceFabric.Services;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   internal class ProxyOperationInvoker : ProxyRemotingInvoker
   {
      IEnumerable<IProxyOperationInvoker> Invokers
      {get;set;}

      public ProxyOperationInvoker(IEnumerable<IProxyOperationInvoker> invokers)
      {
         Invokers = invokers;
      }

      internal override void OnPreInvoke(MethodInfo operation)
      {
         foreach(IProxyOperationInvoker invoker in Invokers)
         {
            invoker.PreInvoke(null,operation);
         }
      }
      internal override void OnPostInvoke(MethodInfo operation,Exception exception)
      {
         foreach(IProxyOperationInvoker invoker in Invokers)
         {
            invoker.PostInvoke(null,operation,exception);
         }
      }
   }
}
