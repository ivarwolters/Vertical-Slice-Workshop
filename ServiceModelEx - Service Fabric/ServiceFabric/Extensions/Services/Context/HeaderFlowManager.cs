// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.ServiceModel;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   public abstract class HeaderFlowManager
   {
      protected abstract void OnAddCommonHeaders(IHeaderAccessor accessor);
      protected abstract void OnCopyCommonHeadersTo(IHeaderAccessor accessor);
      protected abstract void OnCopyCommonHeadersFrom(IHeaderAccessor accessor);

      // Add common headers at the point of message origin to the given message headers collection.
      internal void AddCommonHeaders(ServiceRemotingMessageHeaders destination)
      {
         Debug.Assert(OperationContext.Current == null);
         if(OperationContext.Current != null)
         {
            throw new InvalidOperationException("Invalid attempt to add headers. Add headers only during the originating proxy call.");
         }
         else
         {
            OnAddCommonHeaders(new HeaderAccessor(destination));
         }
      }
      // Copy common headers from the service's header cache to the given message headers collection.
      internal void CopyCommonHeadersTo(ServiceRemotingMessageHeaders destination)
      {
         Debug.Assert(OperationContext.Current != null);
         if(OperationContext.Current == null)
         {
            throw new InvalidOperationException("Invalid attempt to copy headers. Only copy headers within a service operation.");
         }
         else
         {
            OnCopyCommonHeadersTo(new HeaderAccessor(destination));
         }
      }
      // Copy common headers from the given MessageHeaders collection to the service's header cache.
      internal void CopyCommonHeadersFrom(ServiceRemotingMessageHeaders source)
      {
         Debug.Assert(OperationContext.Current != null);
         if(OperationContext.Current == null)
         {
            throw new InvalidOperationException("Invalid attempt to copy headers. Only copy headers within a service operation.");
         }
         else
         {
            OnCopyCommonHeadersFrom(new HeaderAccessor(source));
         }
      }

      public bool CanCopyHeaders()
      {
         return OperationContext.Current != null && OperationContext.Current.IncomingMessageHeaders != null;
      }
   }
}
