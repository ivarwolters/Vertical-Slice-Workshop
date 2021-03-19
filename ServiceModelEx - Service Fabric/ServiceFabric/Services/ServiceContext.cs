using System;
using System.Runtime.Serialization;

namespace ServiceModelEx.Fabric
{
   [Serializable]
   [DataContract]
   public class ServiceContext
   {
      [DataMember]
      public Uri ServiceName
      {get; internal set;}
   }
}
