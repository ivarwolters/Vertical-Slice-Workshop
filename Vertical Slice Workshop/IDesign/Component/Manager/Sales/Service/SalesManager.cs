using System.Threading.Tasks;
using ServiceModelEx.ServiceFabric;

using IDesign.iFX.Service;
using MethodModelEx.Microservices;

using IDesign.Access.Restaurant.Interface;
using IDesign.Engine.Validation.Interface;
using IDesign.Engine.Ordering.Interface;
using IDesign.Engine.Pricing.Interface;
using Online = IDesign.Manager.Sales.Interface.Online;
using Restaurant = IDesign.Manager.Sales.Interface.Restaurant;
using IDesign.Manager.Sales.Interface.Online;
#if ServiceModelEx_ServiceFabric
using ServiceModelEx.Fabric;
#else
using System.Fabric;
#endif

namespace IDesign.Manager.Sales.Service
{
   [ApplicationManifest("IDesign.Microservice.Sales", "SalesManager")]
   public class SalesManager : ServiceBase, Online.ISalesManager, Restaurant.ISalesManager
   {
      public SalesManager(StatelessServiceContext context) : base(context)
      { }

      FindItemResponse MapInternalToPublic()
      {
         return new FindItemResponse();
      }

      async Task<FindItemResponse> Online.ISalesManager.FindItemAsync(FindItemRequest request)
      {
         var validationProxy = Proxy.ForComponent<IValidationEngine>(this);
         await validationProxy.ValidateAsync(new ValidateCriteria());

         var restaurantProxy = Proxy.ForComponent<IRestaurantAccess>(this);
         await restaurantProxy.FilterAsync(new RestaurantCriteria());

         var orderingEngine = Proxy.ForComponent<IOrderingEngine>(this);
         await orderingEngine.MatchAsync(new ItemCriteria());

         var pricingProxy = Proxy.ForComponent<IPricingEngine>(this);
         await pricingProxy.CalculateAsync();

         return MapInternalToPublic();
      }

        Task Restaurant.ISalesManager.RestaurantSalesRelatedMethod()
        {
            throw new System.NotImplementedException();
        }
    }
}
