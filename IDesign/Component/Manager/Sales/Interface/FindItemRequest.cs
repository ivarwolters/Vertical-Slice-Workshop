using System;
using System.Runtime.Serialization;

namespace IDesign.Manager.Sales.Interface
{
   [DataContract]
   public class FindItemRequest
   {
     [DataMember]
      public Location Location { get; set; }

      [DataMember]
      public Guid CustomerId { get; set; }

      [DataMember]
      public string SearchText { get; set; }
   }
}