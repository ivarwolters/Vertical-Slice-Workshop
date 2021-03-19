using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IDesign.Manager.Sales.Interface.Online
{
   [DataContract]
   public class FindItemResponse
   {
      [DataMember]
      public IEnumerable<MenuItem> MenuItems { get; set; }
   }
}