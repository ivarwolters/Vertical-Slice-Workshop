using System.ServiceModel;
using System.Threading.Tasks;

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace IDesign.Access.Specials.Interface
{
   [ServiceContract]
   public interface ISpecialsAccess : IService
   {
      [OperationContract]
      Task FilterAsync();
      [OperationContract]
      Task StoreAsync();
   }
}
