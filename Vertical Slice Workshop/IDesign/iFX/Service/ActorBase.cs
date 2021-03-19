using System;
using System.Linq;
using System.Collections.Generic;
using ServiceModelEx.ServiceFabric.Actors;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.ServiceFabric.Actors.Runtime;
#else
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime;
using ServiceFabricEx.Services;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
#endif

using MethodModelEx.Microservices;

namespace IDesign.iFX.Service
{
   public abstract partial class ActorBase : Actor
   {
      public ActorBase(ActorService service, ActorId id) : base(service,id)
      {
         //Proxy.SetHeaderFlowPolicy(new TestFlowPolicy());
      }

      //TODO: Add ambient context accessors here...
      //public TraceContext Trace
      //{
      //   get
      //   {return TraceHelper.Get();}
      //}
      //public SessionContext Session
      //{
      //   get
      //   {return SessionHelper.Get();}
        
      //}
      //public PartnerContext Partner
      //{
      //   get
      //   {return PartnerHelper.Get();}
      //}
   }
}