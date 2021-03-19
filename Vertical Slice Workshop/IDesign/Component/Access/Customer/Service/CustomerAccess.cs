using System.Threading.Tasks;
using ServiceModelEx.ServiceFabric;

using IDesign.iFX.Service;
using MethodModelEx.Microservices;

using IDesign.Access.Customer.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using IDesign.Framework.Repository;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.Fabric;
#else
using System.Fabric;
#endif

namespace IDesign.Access.Customer.Service
{
   [ApplicationManifest("IDesign.Microservice.Sales","CustomerAccess")]
   public class CustomerAccess : ServiceBase, ICustomerAccess
   {
      public CustomerAccess(StatelessServiceContext context) : base(context)
      {}

      async Task<FilterResponse> ICustomerAccess.FilterAsync(CustomerCriteria criteria)
      {
         Entity.ICustomerRepository customerRepository = RepositoryFactory.Create<Entity.ICustomerRepository>();
         IEnumerable<Entity.Customer> entityCustomer = customerRepository.Get<Entity.Customer>(criteria.NameCriteria);
         return MapEntityToDTO(entityCustomer);
         return new FilterResponse();
      }

      async Task ICustomerAccess.StoreAsync()
      {}
   }
}

namespace IDesign.Framework.Repository
{
   [AttributeUsage(AttributeTargets.Class)]
   public class Table : Attribute
   { }
   [AttributeUsage(AttributeTargets.Property)]
   public class Column : Attribute
   { }
   public abstract class TableBase
   { }

   public interface IRepository
   {
      IEnumerable<T> Get<T>(string key) where T : TableBase, new();
   }
   public class Repository : IRepository
   {
      public IEnumerable<T> Get<T>(string key) where T : TableBase, new()
      {
         return new T[0];
      }
   }

   public static class RepositoryFactory
   {
      static Type Resolve<I>() where I : class
      {
         string typeName = string.Empty;

         string repositoryName = typeof(I).Name.Replace("I", "");
         typeName = typeof(I).Namespace + "." + repositoryName;

         Type implementationType = typeof(I).Assembly.GetType(typeName);
         Debug.Assert(implementationType != null, "You did not follow the rules...");

         return implementationType;
      }
      public static I Create<I>() where I : class, IRepository
      {
         return Activator.CreateInstance(Resolve<I>()) as I;
      }
   }
}

namespace IDesign.Access.Customer.Service.Entity
{
   using IDesign.Framework.Repository;

   //Our Entity...
   [Table]
   public class Customer : TableBase
   {
      [Column]
      public string Name
      { get; set; }
   }
   public interface ICustomerRepository : IRepository
   { }
   class CustomerRepository : Repository, ICustomerRepository
   { }
}
