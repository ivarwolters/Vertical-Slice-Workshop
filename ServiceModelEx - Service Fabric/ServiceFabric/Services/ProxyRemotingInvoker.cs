// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Reflection;

namespace ServiceModelEx.ServiceFabric.Services
{
   internal abstract class ProxyRemotingInvoker
   {
      internal abstract void OnPreInvoke(MethodInfo operation);
      internal abstract void OnPostInvoke(MethodInfo operation,Exception exception);
   }
}
