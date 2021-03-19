// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Runtime.Serialization;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   [DataContract]
   public class OriginationContext
   {
      [DataMember]
      public string OriginationId
      {get;internal set;}
   }
}
