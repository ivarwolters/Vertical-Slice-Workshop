using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Access.Menu.Interface
{
   [ServiceContract]
   public interface IMenuAccess : IService
   {
      [OperationContract]
      Task FilterAsync();
      [OperationContract]
      Task StoreAsync();
   }
}
