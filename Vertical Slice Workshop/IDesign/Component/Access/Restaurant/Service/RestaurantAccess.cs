using System.Collections.Generic;
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
    [ApplicationManifest("IDesign.Microservice.Sales", "RestaurantAccess")]
    public class RestaurantAccess : ServiceBase, IRestaurantAccess
    {
        public RestaurantAccess(StatelessServiceContext context) : base(context)
        { }


        async Task<FilterRestaurantResponse> IRestaurantAccess.FilterAsync(RestaurantCriteria criteria)
        {
            await Task.Delay(50);
            return new FilterRestaurantResponse
            {
                Restaurants = new List<Interface.Restaurant>()
            };
        }

        Task<StoreRestaurantResponse> IRestaurantAccess.StoreAsync(Interface.Restaurant restaurant)
        {
            throw new System.NotImplementedException();
        }
    }
}
