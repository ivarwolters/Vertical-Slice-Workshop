// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Collections.Generic;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Actors.Runtime;

using ServiceModelEx.ServiceFabric.Extensions.Services;

namespace ServiceModelEx.ServiceFabric.Extensions.Actors
{
   public class ActorOperationDispatcher : ServiceOperationDispatcher
   {
      public ActorOperationDispatcher(ServiceContext serviceContext,ActorService service,IEnumerable<IServiceHeaderInterceptor> interceptors) : this(serviceContext,service,interceptors,null)
      {}
      public ActorOperationDispatcher(ServiceContext serviceContext,ActorService service,IEnumerable<IServiceOperationInvoker> invokers) : this(serviceContext,service,null,invokers)
      {}
      public ActorOperationDispatcher(ServiceContext serviceContext,ActorService service,IEnumerable<IServiceHeaderInterceptor> interceptors,IEnumerable<IServiceOperationInvoker> invokers) : base(serviceContext,service,interceptors,invokers)
      {}
   }
}
