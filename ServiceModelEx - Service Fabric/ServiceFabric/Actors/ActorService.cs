// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services.Remoting;
using ServiceModelEx.ServiceFabric.Services.Communication.Runtime;

namespace ServiceModelEx.ServiceFabric.Actors.Runtime
{
   [Serializable]
   public class ActorService : IService
   {
      public StatefulServiceContext Context
      {get;private set;}
      public ActorTypeInformation ActorTypeInformation
      {get;private set;}
      public IEnumerable<ServiceReplicaListener> ServiceReplicaListeners
      {get; private set;}

      public ActorService(StatefulServiceContext context,ActorTypeInformation actorTypeInfo)
      {
         Context = context;
         ActorTypeInformation = actorTypeInfo;
         ServiceReplicaListeners = CreateServiceReplicaListeners();
      }

      protected virtual IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
      {
         return null;
      }
   }
}
