using System.Threading.Tasks;
using ServiceModelEx.ServiceFabric;

using IDesign.iFX.Service;
using MethodModelEx.Microservices;

using IDesign.Engine.Validation.Interface;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.Fabric;
#else
using System.Fabric;
#endif

namespace IDesign.Engine.Validation.Service
{
   [ApplicationManifest("IDesign.Microservice.Sales","ValidationEngine")]
   public class ValidationEngine : ServiceBase, IValidationEngine
   {
      public ValidationEngine(StatelessServiceContext context) : base(context)
      {}

      async Task<ValidateResponse> IValidationEngine.ValidateAsync(ValidateCriteria criteria)
      {
         return new ValidateResponse();
      }
   }
}
