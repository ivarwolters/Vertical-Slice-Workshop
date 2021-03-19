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

namespace MethodModelEx
{
   public class PolicyDispatcher : ServiceOperationDispatcher
   {
      public PolicyDispatcher(ServiceContext serviceContext,IService service,HeaderFlowManager headerFlowPolicy) : base(serviceContext,service,new [] {new ServiceHeaderFlowInterceptor(headerFlowPolicy)})
      {}
      public PolicyDispatcher(ServiceContext serviceContext,IService service,HeaderFlowManager headerFlowPolicy,IServiceOperationInvoker invokerPolicy) : base(serviceContext,service,new [] {new ServiceHeaderFlowInterceptor(headerFlowPolicy)},new [] {invokerPolicy})
      {}
   }
}
