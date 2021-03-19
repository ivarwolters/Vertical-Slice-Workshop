// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services.Runtime;
using ServiceModelEx.ServiceFabric.Services.Communication.Runtime;
using ServiceModelEx.ServiceFabric.Services.Remoting.Runtime;

#pragma warning disable 618

namespace ServiceModelEx.ServiceFabric.Services
{
   internal class StatelessServiceBehavior : IServiceBehavior
   {
      public void AddBindingParameters(ServiceDescription serviceDescription,System.ServiceModel.ServiceHostBase serviceHostBase,System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints,System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
      {}
      public void ApplyDispatchBehavior(ServiceDescription serviceDescription,System.ServiceModel.ServiceHostBase serviceHostBase)
      {}
      public void Validate(ServiceDescription serviceDescription,System.ServiceModel.ServiceHostBase serviceHostBase)
      {
         EnforceStatelessServiceEndpointBehaviorPolicy(serviceDescription);
         EnforceStatelessServiceBehaviorPolicy(serviceDescription);
      }

      void EnforceStatelessServiceBehaviorPolicy(ServiceDescription serviceDescription)
      {
         ServiceBehaviorAttribute serviceBehavior = serviceDescription.Behaviors.FirstOrDefault(behavior => behavior is ServiceBehaviorAttribute) as ServiceBehaviorAttribute;
         if(serviceBehavior != null)
         {
            serviceBehavior.InstanceContextMode = InstanceContextMode.PerCall;
            serviceBehavior.ConcurrencyMode = ConcurrencyMode.Multiple;
            serviceBehavior.MaxItemsInObjectGraph = int.MaxValue;
            serviceBehavior.UseSynchronizationContext = false;
         }
         serviceDescription.SetThrottle();
      }

      IEnumerable<ServiceRemotingDispatcher> GetDispatchers(Type serviceType,ServiceEndpoint endpoint)
      {
         List<ServiceRemotingDispatcher> dispatchers = new List<ServiceRemotingDispatcher>();   

         StatelessServiceContext serviceContext = new StatelessServiceContext();
         StatelessService service = Activator.CreateInstance(serviceType,serviceContext) as StatelessService;
         foreach(ServiceInstanceListener instanceListener in service.ServiceInstanceListeners)
         {
            if(instanceListener.Name.Equals(endpoint.Contract.Name))
            {
               IServiceRemotingListener listener = instanceListener.CreateCommunicationListener(serviceContext) as IServiceRemotingListener;
               if(listener.Dispatcher != null)
               {
                  dispatchers.Add(listener.Dispatcher);
               }
            }
         }

         return dispatchers;
      }
      void EnforceStatelessServiceEndpointBehaviorPolicy(ServiceDescription serviceDescription)
      {
         foreach(ServiceEndpoint endpoint in serviceDescription.Endpoints)
         {
            endpoint.EndpointBehaviors.Add(new FabricThreadPoolBehavior());
            endpoint.EndpointBehaviors.Add(new StatelessServiceInstanceProvider());
            IEnumerable<ServiceRemotingDispatcher> remotingDispatchers = GetDispatchers(serviceDescription.ServiceType,endpoint);
            endpoint.EndpointBehaviors.Add(new ServiceHeaderInterceptor(remotingDispatchers));

            foreach(OperationDescription operation in endpoint.Contract.Operations)
            {
               if(operation.TaskMethod == null)
               {
                  throw new InvalidOperationException("Validation failed. Service operation '" + endpoint.Contract.ContractType.FullName + "." + operation.Name + "' does not return Task or Task<>. Service interface methods must be async and must return either Task or Task<>.");
               }
               OperationBehaviorAttribute operationBehavior = operation.OperationBehaviors.FirstOrDefault(behavior=>behavior is OperationBehaviorAttribute) as OperationBehaviorAttribute;

               Debug.Assert(operationBehavior.TransactionScopeRequired == false);
               operationBehavior.TransactionScopeRequired = false;

               operation.OperationBehaviors.Add(new ServiceOperationBehavior(remotingDispatchers));
            }
         }
      }
   }
}

#pragma warning restore 618