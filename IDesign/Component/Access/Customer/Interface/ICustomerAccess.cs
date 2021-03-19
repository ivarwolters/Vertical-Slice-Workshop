using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Access.Customer.Interface
{
   [ServiceContract]
   public interface ICustomerAccess : IService
   {
      [OperationContract]
      Task FilterAsync();
      [OperationContract]
      Task StoreAsync();
   }
}
