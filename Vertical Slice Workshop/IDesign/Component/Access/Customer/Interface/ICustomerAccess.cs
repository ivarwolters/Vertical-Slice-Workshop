using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Access.Customer.Interface
{
   [ServiceContract]
   public interface ICustomerAccess : IService
   {
      [OperationContract]
      Task<FilterResponse> FilterAsync(CustomerCriteria criteria);

      [OperationContract]
      Task<StoreResponse> StoreAsync(Customer customer);
   }

   [DataContract]
   public class CustomerCriteria
   {
      [DataMember]
      public IEnumerable<Guid> CustomerIds { get; set; }

      [DataMember]
      public string NameCriteria { get; set; }
   }

   public class FilterResponse
   {
      [DataMember]
      public IEnumerable<Customer> Customers { get; set; }
   }

   [DataContract]
   public class Customer
   {
      [DataMember]
      public Guid Id { get; set; }
      [DataMember]
      public string Name { get; set; }
      [DataMember]
      public IEnumerable<PreferenceBase> Preferences { get; set; }
   }

   [DataContract]
   public abstract class PreferenceBase
   {
      [DataMember]
      public Guid Id { get; set; }
   }

   [DataContract]
   public class ItemPreference : PreferenceBase
   {
      public Guid ItemId { get; set; }
   }

   [DataContract]
   public class RestaurantPreference : PreferenceBase
   {
      public Guid RestaurantId { get; set; }
   }


}
