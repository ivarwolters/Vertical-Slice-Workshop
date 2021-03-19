// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceModelEx.ServiceFabric.Services.Remoting
{
   [DataContract]
   public class ServiceRemotingMessageHeaders
   {
      [DataMember]
      Dictionary<string,byte[]> headers = new Dictionary<string, byte[]>();

      public ServiceRemotingMessageHeaders()
      {}

      public void AddHeader(string headerName,byte[] headerValue)
      {
         headers.Add(headerName,headerValue);
      }
      public bool TryGetHeaderValue(string headerName,out byte[] headerValue)
      {
         return headers.TryGetValue(headerName,out headerValue);
      }
   }
}