using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Engine.Ordering.Interface
{
   [ServiceContract]
   public interface IOrderingEngine : IService
   {
      [OperationContract]
      Task SubmitAsync();
   }
}
