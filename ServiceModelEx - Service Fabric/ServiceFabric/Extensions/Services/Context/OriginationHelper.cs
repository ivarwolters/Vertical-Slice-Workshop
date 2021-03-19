// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.Web;
using System.ServiceModel.Channels;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   internal class OriginationHelper
   {
      static Process m_CurrentProcess = Process.GetCurrentProcess();

      internal static string GenerateOriginationId
      {
         get
         {
            string baseOriginationId = Environment.MachineName + "." + m_CurrentProcess.ProcessName + "[" + m_CurrentProcess.Id + "]";
            if(HttpContext.Current != null)
            {
               return baseOriginationId + ".[" + HttpContext.Current.Session.SessionID + "]";
            }
            else
            {
               return baseOriginationId;
            }
         }
      }
      internal static OriginationContext Create()
      {
         return new OriginationContext
         {
            OriginationId = GenerateOriginationId
         };
      }
      internal static OriginationContext Get(ServiceRemotingMessageHeaders messageHeaders)
      {
         return Header<OriginationContext>.Get(messageHeaders);
      }
      internal static void Add(MessageHeaders messageHeaders)
      {
         ServiceRemotingMessageHeaders remotingHeaders = Header<OriginationContext>.Get(messageHeaders);
         Header<OriginationContext>.Add(Create(),remotingHeaders);
         Header<OriginationContext>.Replace(remotingHeaders,messageHeaders);
      }
   }
}
