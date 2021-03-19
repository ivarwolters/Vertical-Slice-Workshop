using System.Threading.Tasks;
using ServiceModelEx.ServiceFabric;

using IDesign.iFX.Service;
using MethodModelEx.Microservices;

using IDesign.Access.Restaurant.Interface;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.Fabric;
#else
using System.Fabric;
#endif

namespace IDesign.Access.Restaurant.Service
{
   [ApplicationManifest("IDesign.Microservice.Sales","RestaurantAccess")]
   public class RestaurantAccess : ServiceBase, IRestaurantAccess
   {
      public RestaurantAccess(StatelessServiceContext context) : base(context)
      {}

      async Task IRestaurantAccess.FilterAsync()
      
        {}
      async Task IRestaurantAccess.StoreAsync()
      {}
   }
}
