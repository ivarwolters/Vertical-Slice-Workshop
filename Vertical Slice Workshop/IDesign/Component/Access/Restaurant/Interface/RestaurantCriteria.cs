using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IDesign.Access.Restaurant.Interface
{
    [DataContract]
    public class RestaurantCriteria
    {
        [DataMember]
        public IEnumerable<Guid> Ids { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}