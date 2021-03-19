using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Engine.Pricing.Interface
{
   [ServiceContract]
   public interface IPricingEngine : IService
   {
      [OperationContract]
      Task CalculateAsync();
   }
}
