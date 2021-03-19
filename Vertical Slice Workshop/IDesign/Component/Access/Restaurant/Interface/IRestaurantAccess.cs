using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Access.Restaurant.Interface
{
   [ServiceContract]
   public interface IRestaurantAccess : IService
   {
      [OperationContract]
      Task FilterAsync();
      [OperationContract]
      Task StoreAsync();
   }
}
