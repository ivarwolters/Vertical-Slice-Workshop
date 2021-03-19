using System;
using System.Linq;
using System.Collections.Generic;
using IDesign.iFX.Contract;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Extensions.Services;
using ServiceModelEx.ServiceFabric.Services.Communication.Runtime;
using ServiceModelEx.ServiceFabric.Services.Remoting;
using ServiceModelEx.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using ServiceModelEx.ServiceFabric.Services.Runtime;
#else
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime;
using ServiceFabricEx.Services;
#endif

using MethodModelEx.Microservices;

namespace IDesign.iFX.Service
{
   public abstract partial class ServiceBase : StatelessService,IService
   {
      public ServiceBase(StatelessServiceContext context) : base(context)
      {
         //TODO: Install your custom header flow policy here...
         //Proxy.SetHeaderFlowPolicy(new TestFlowPolicy());
      }

      //TODO: Create policy for your context flow and install...
      CustomDispatcher EnforceRemotingDispatcherPolicy(StatelessServiceContext context)
      {
         return new CustomDispatcher(context,this);
      }
      //Tuple<IEnumerable<IServiceHeaderInterceptor>,IEnumerable<IServiceOperationInvoker>> EnforceEventingDispatcherPolicy()
      //{
      //   return Tuple.Create(IceServiceDispatcher.GetInterceptors(),IceServiceDispatcher.GetInvokers());
      //}
      IEnumerable<ServiceInstanceListener> CreateRemotingListeners(Func<Type,FabricTransportRemotingListenerSettings> createSettings)
      {
         List<ServiceInstanceListener> listeners = new List<ServiceInstanceListener>();
         foreach(Type contract in this.GetType().GetInterfaces().Where(i => i.GetInterface("IService") != null))
         {
            //listeners.Add(new ServiceInstanceListener(context) => new FabricTransportServiceRemotingListener(context,EnforceRemotingDispatcherPolicy(context),createSettings(contract)),contract.Name));
            listeners.Add(new ServiceInstanceListener((Context) => new FabricTransportServiceRemotingListener(Context,EnforceRemotingDispatcherPolicy(Context),createSettings(contract)),contract.Name));
         }
         return listeners;
      }
      IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners(Func<Type,FabricTransportRemotingListenerSettings> createRemotingSettings = null)
      {
         if(createRemotingSettings == null)
         {
            createRemotingSettings = (contract) => new FabricTransportRemotingListenerSettings {EndpointResourceName = contract.Name};
         }
         List<ServiceInstanceListener> listeners = new List<ServiceInstanceListener>(CreateRemotingListeners(createRemotingSettings));

         return listeners;
      }
      protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
      {
         return CreateServiceInstanceListeners();
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