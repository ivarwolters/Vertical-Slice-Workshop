// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Actors.Runtime;
using ServiceModelEx.ServiceFabric.Extensions.Actors;
using ServiceModelEx.ServiceFabric.Extensions.Services;
using ServiceModelEx.ServiceFabric.Extensions.Test;

namespace Test.Unit.Access
{
   public class UnitTestHarness : ExtensionTestHarness
   {
      public Func<StatefulServiceContext,ActorTypeInformation,ActorService> ActorServiceFactory = 
         (context,info) => new DispatchActorService(context,info,new IServiceHeaderInterceptor[]
         {
            //TODO: Add your custom concepts here...
            //new ServiceHeaderFlowInterceptor(new TestFlowPolicy()),
            //new CustomServiceHeaderInterceptor()
         });
   }
}
