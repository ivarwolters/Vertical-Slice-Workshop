// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Threading;
using System.Threading.Tasks;

namespace ServiceModelEx.ServiceFabric.Services.Communication.Runtime
{
   public interface ICommunicationListener
   {
      void Abort();
      Task<string> OpenAsync(CancellationToken cancellationToken);
      Task CloseAsync(CancellationToken cancellationToken);
   }
}
