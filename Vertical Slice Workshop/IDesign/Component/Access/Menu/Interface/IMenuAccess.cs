using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Access.Menu.Interface
{
   [ServiceContract]
   public interface IMenuAccess : IService
   {
      [OperationContract]
      Task<FilterResponse> FilterAsync(FilterCriteria criteria);

      [OperationContract]
      Task<StoreResponse> StoreAsync(Menu menu);
   }
}
