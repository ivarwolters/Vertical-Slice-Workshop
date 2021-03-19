using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Engine.Validation.Interface
{
   [ServiceContract]
   public interface IValidationEngine : IService
   {
      [OperationContract]
      Task<ValidateResponse> ValidateAsync(ValidateCriteria criteria);
   }
}
