using System.Threading.Tasks;
using ServiceModelEx.ServiceFabric;

using IDesign.iFX.Service;
using MethodModelEx.Microservices;

using IDesign.Access.Restaurant.Interface;
using IDesign.Engine.Validation.Interface;
using IDesign.Engine.Ordering.Interface;
using IDesign.Engine.Pricing.Interface;
using IDesign.Manager.Sales.Interface;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.Fabric;
#else
using System.Fabric;
#endif

namespace IDesign.Manager.Sales.Service
{
   [ApplicationManifest("IDesign.Microservice.Sales", "SalesManager")]
   public class SalesManager : ServiceBase, ISalesManager
   {
      public SalesManager(StatelessServiceContext context) : base(context)
      { }
      async Task ISalesManager.FindItemAsync()
      {
         var validationProxy = Proxy.ForComponent<IValidationEngine>(this);
         await validationProxy.ValidateAsync();

         IRestaurantAccess restaurantProxy = Proxy.ForComponent<IRestaurantAccess>(this);
         await restaurantProxy.FilterAsync();

         var orderingEngine = Proxy.ForComponent<IOrderingEngine>(this);
         await orderingEngine.SubmitAsync();

         var pricingProxy = Proxy.ForComponent<IPricingEngine>(this);
         await pricingProxy.CalculateAsync();
      }
   }
}
