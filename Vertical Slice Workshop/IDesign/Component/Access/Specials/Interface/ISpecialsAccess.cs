using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Access.Specials.Interface
{
   [ServiceContract]
   public interface ISpecialsAccess : IService
   {
      //TODO: Take Customer Access as example.
      [OperationContract]
      Task FilterAsync();
      [OperationContract]
      Task StoreAsync();
   }
}
