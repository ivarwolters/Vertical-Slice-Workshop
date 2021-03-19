using System.Threading.Tasks;
using ServiceModelEx.ServiceFabric;

using IDesign.iFX.Service;
using MethodModelEx.Microservices;

using IDesign.Access.Customer.Interface;

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

      async Task ICustomerAccess.FilterAsync()
      {}
      async Task ICustomerAccess.StoreAsync()
      {}
   }
}
