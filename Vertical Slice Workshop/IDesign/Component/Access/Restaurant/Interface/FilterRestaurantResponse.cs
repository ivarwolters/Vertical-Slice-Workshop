using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IDesign.Access.Restaurant.Interface
{
    [DataContract]
    public class FilterRestaurantResponse
    {
        [DataMember]
        public IEnumerable<Restaurant> Restaurants { get; set; }
    }
}