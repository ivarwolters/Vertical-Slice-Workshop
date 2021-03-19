using System.ServiceModel;
using System.Threading.Tasks;
using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Manager.Sales.Interface.Restaurant
{
   [ServiceContract]
   public interface ISalesManager : IService
   {
       [OperationContract]
       Task RestaurantSalesRelatedMethod();
    }
}
