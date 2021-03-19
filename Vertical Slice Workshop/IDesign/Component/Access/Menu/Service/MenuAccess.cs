using System.Threading.Tasks;
using ServiceModelEx.ServiceFabric;

using IDesign.iFX.Service;
using MethodModelEx.Microservices;

using IDesign.Access.Menu.Interface;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.Fabric;
#else
using System.Fabric;
#endif

namespace IDesign.Access.Menu.Service
{
   [ApplicationManifest("IDesign.Microservice.Sales","MenuAccess")]
   public class MenuAccess : ServiceBase, IMenuAccess
   {
      public MenuAccess(StatelessServiceContext context) : base(context)
      {}

      async Task<FilterResponse> IMenuAccess.FilterAsync(FilterCriteria criteria)
      {
         // TODO: Implement with CustomerAccess as example.
         return new Interface.FilterResponse();
      }

      async Task<StoreResponse> IMenuAccess.StoreAsync(Interface.Menu menu)
      {
         // TODO: Implement with CustomerAccess as example.
         return new StoreResponse();
      }
   }
}
