// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;

using ServiceModelEx.ServiceFabric.Services;
using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   internal class ProxyMessageInterceptor : ProxyRemotingInterceptor
   {
      IEnumerable<IProxyHeaderInterceptor> Interceptors
      {get;set;}
      IHeaderAccessor Accessor
      {get;set;}
      Action<IHeaderAccessor> AddHeaders
      {get;set;}
      Action<IHeaderAccessor> GetHeaders
      {get;set;}

      public ProxyMessageInterceptor(IEnumerable<IProxyHeaderInterceptor> interceptors,IHeaderAccessor accessor)
      {
         Interceptors = interceptors;
         Accessor = accessor;
      }
      public ProxyMessageInterceptor(IEnumerable<IProxyHeaderInterceptor> interceptors,Action<IHeaderAccessor> addHeaders,Action<IHeaderAccessor> getHeaders)
      {
         Interceptors = interceptors;
         AddHeaders = addHeaders;
         GetHeaders = getHeaders;
      }

      internal override void OnAddHeaders(MessageHeaders messageHeaders)
      {
         if(AddHeaders != null)
         {
            ServiceRemotingMessageHeaders headers = Header<ServiceRemotingMessageHeaders>.Get(messageHeaders);
            AddHeaders(new HeaderAccessor(headers));
            Header<ServiceRemotingMessageHeaders>.Replace(headers,messageHeaders);
         }
         else
         {
            if(Accessor != null)
            {
               ServiceRemotingMessageHeaders headers = Header<ServiceRemotingMessageHeaders>.Get(messageHeaders);
               foreach(KeyValuePair<string,byte[]> header in (Accessor as HeaderAccessor).Headers.Headers())
               {
                  headers.AddHeader(header.Key,header.Value);
               }
               Header<ServiceRemotingMessageHeaders>.Replace(headers,messageHeaders);
            }
         }
      }
      internal override void OnGetHeaders(MessageHeaders messageHeaders)
      {
         if(GetHeaders != null)
         {
            GetHeaders(new HeaderAccessor(Header<ServiceRemotingMessageHeaders>.Get(messageHeaders)));
         }
      }
      internal override void OnReceiving(Uri serviceAddress,MessageHeaders messageHeaders)
      {
         if(messageHeaders.Action.Contains("fault") == false)
         {
            if(OriginationHelper.Get(Header<ServiceRemotingMessageHeaders>.Get(messageHeaders)) == null)
            {
               throw new InvalidOperationException("Invalid ServiceFabricEx extensibility sequence. Operation dispatcher not installed on service "  + serviceAddress + ".");
            }
         }
         foreach(IProxyHeaderInterceptor interceptor in Interceptors)
         {
            ServiceRemotingMessageHeaders headers = Header<ServiceRemotingMessageHeaders>.Get(messageHeaders);
            interceptor.Receiving(headers);
            Header<ServiceRemotingMessageHeaders>.Replace(headers,messageHeaders);
         }
      }
      internal override void OnSending(Uri serviceAddress,MessageHeaders messageHeaders)
      {
         foreach(IProxyHeaderInterceptor interceptor in Interceptors)
         {
            ServiceRemotingMessageHeaders headers = Header<ServiceRemotingMessageHeaders>.Get(messageHeaders);
            interceptor.Sending(headers);
            Header<ServiceRemotingMessageHeaders>.Replace(headers,messageHeaders);
         }
      }
   }
}
