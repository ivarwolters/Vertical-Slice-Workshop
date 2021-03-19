using System.Threading.Tasks;
using ServiceModelEx.ServiceFabric;

using IDesign.iFX.Service;
using MethodModelEx.Microservices;

using IDesign.Access.Specials.Interface;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.Fabric;
#else
using System.Fabric;
#endif

namespace IDesign.Access.Specials.Service
{
   [ApplicationManifest("IDesign.Microservice.Sales","SpecialsAccess")]
   public class SpecialsAccess : ServiceBase, ISpecialsAccess
   {
      public SpecialsAccess(StatelessServiceContext context) : base(context)
      {}

      async Task ISpecialsAccess.FilterAsync()
      {}
      async Task ISpecialsAccess.StoreAsync()
      {}
   }
}
