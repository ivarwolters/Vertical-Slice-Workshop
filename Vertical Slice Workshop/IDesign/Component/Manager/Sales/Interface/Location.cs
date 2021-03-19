
using System.Runtime.Serialization;

namespace IDesign.Manager.Sales.Interface
{
   [DataContract]
   public class Location
   {
      [DataMember]
      public int Latitude { get; set; }

      [DataMember]
      public int Longitude { get; set; }

      [DataMember]
      public int Range { get; set; }
   }
}