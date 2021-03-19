using System.ServiceModel;
using System.Threading.Tasks;
using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Manager.Sales.Interface.Online
{
   [ServiceContract]
   public interface ISalesManager : IService
   {
      [OperationContract]
      Task<FindItemResponse> FindItemAsync(FindItemRequest request);
   }
}
