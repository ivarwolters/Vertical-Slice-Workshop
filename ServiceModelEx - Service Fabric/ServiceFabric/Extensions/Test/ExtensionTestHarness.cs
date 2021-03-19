using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;

using ServiceModelEx.ServiceFabric.Extensions.Services;
using ServiceModelEx.ServiceFabric.Services.Remoting;
using ServiceModelEx.ServiceFabric.Test;

namespace ServiceModelEx.ServiceFabric.Extensions.Test
{
   public abstract class ExtensionTestHarness : ServiceTestBase
   {
      static Type m_HeaderDefinition = null;
      static ExtensionTestHarness()
      {
         m_HeaderDefinition = typeof(Header<>).GetGenericTypeDefinition();
      }

      protected override void AddStandardContexts(MessageHeaders headers)
      {
         OriginationHelper.Add(headers);
         base.AddStandardContexts(headers);
      }
      protected override void AddCustomContexts(bool isService,params object[] mocks)
      {
         string directionQualifier = string.Empty;
         if(isService == true)
         {
            directionQualifier = "Outgoing";
         }
         else
         {
            directionQualifier = "Incoming";
            MockMessage();
         }

         IEnumerable<object> contexts = mocks.Where(m=>m.GetType().GetCustomAttribute(typeof(DataContractAttribute)) != null);
         if(contexts.Any())
         {
            foreach (object context in contexts)
            {
               Type header = m_HeaderDefinition.MakeGenericType(context.GetType());
               MessageHeaders messageHeaders = null;
               if(isService == true)
               {
                  messageHeaders = OperationContext.Current.OutgoingMessageHeaders;
               }
               else
               {
                  messageHeaders = OperationContext.Current.IncomingMessageHeaders;
               }
               ServiceRemotingMessageHeaders remotingHeaders = header.InvokeMember("Get",BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.InvokeMethod,null,null,new object[] {messageHeaders}) as ServiceRemotingMessageHeaders;

               byte[] contextHeader = null;
               if (remotingHeaders.TryGetHeaderValue(context.GetType().Name,out contextHeader) == false)
               {
                  header.InvokeMember("Add"+directionQualifier,BindingFlags.Public|BindingFlags.Static|BindingFlags.InvokeMethod,null,null,new object[] {context});
               }
               else
               {
                  header.InvokeMember("Replace"+directionQualifier,BindingFlags.Public|BindingFlags.Static|BindingFlags.InvokeMethod,null,null,new object[] {context});
               }
            }
         }
      }
   }
}
