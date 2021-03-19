// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services.Remoting;
using ServiceModelEx.ServiceFabric.Extensions.Services;
#else
using System.Fabric;
using Microsoft.ServiceFabric.Services.Remoting;
using ServiceFabricEx.Services;
#endif

namespace IDesign.iFX.Contract
{
   public class CustomDispatcher : ServiceOperationDispatcher
   {
      public CustomDispatcher(ServiceContext serviceContext,IService service) : base(serviceContext,service,new IServiceHeaderInterceptor[] 
      {
         new ServiceHeaderFlowInterceptor(new TestFlowPolicy()),
      })
      {}
   }
}
