// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   internal static class HeaderExtensions
   {
      public static Dictionary<string,byte[]> Headers(this ServiceRemotingMessageHeaders instance)
      {
         Dictionary<string,byte[]> headers = null;
         headers = instance.GetType().InvokeMember("headers",
                                                   BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.GetField,
                                                   null,instance,new object[0]) as Dictionary<string,byte[]>;
         return headers;
      }
      public static bool RemoveHeader(this ServiceRemotingMessageHeaders instance,string headerName)
      {
         bool removed = false;

         Dictionary<string,byte[]> headers = instance.Headers();
         Debug.Assert(headers != null);
         if(headers != null)
         {
            removed = headers.Remove(headerName);
         }
         return removed;
      }
   }
}
