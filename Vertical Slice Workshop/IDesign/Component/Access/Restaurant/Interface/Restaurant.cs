using System;
using System.Runtime.Serialization;

namespace IDesign.Access.Restaurant.Interface
{
    [DataContract]
    public class Restaurant
    {
        [DataMember]
        public Guid Id { get; set; }

    }
}