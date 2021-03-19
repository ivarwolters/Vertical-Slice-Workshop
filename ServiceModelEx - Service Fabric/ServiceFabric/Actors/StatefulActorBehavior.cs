// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Actors.Runtime;
using ServiceModelEx.ServiceFabric.Services;
using ServiceModelEx.ServiceFabric.Services.Communication.Runtime;
using ServiceModelEx.ServiceFabric.Services.Remoting.Runtime;

#pragma warning disable 618

namespace ServiceModelEx.ServiceFabric.Actors
{
   internal class StatefulActorBehavior : IServiceBehavior
   {
      public void AddBindingParameters(ServiceDescription serviceDescription,System.ServiceModel.ServiceHostBase serviceHostBase,System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints,System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
      {}
      public void ApplyDispatchBehavior(ServiceDescription serviceDescription,System.ServiceModel.ServiceHostBase serviceHostBase)
      {}
      public void Validate(ServiceDescription serviceDescription,System.ServiceModel.ServiceHostBase serviceHostBase)
      {
         EnforceStatefulActorEndpointBehaviorPolicy(serviceDescription);
         EnforceStatefulActorBehaviorPolicy(serviceDescription);
      }

      void EnforceStatefulActorBehaviorPolicy(ServiceDescription serviceDescription)
      {
         ServiceBehaviorAttribute serviceBehavior = serviceDescription.Behaviors.Find<ServiceBehaviorAttribute>();
         if(serviceBehavior != null)
         {
            Debug.Assert(serviceBehavior.InstanceContextMode == InstanceContextMode.PerSession);
            Debug.Assert(serviceBehavior.ConcurrencyMode == ConcurrencyMode.Single);

            serviceBehavior.InstanceContextMode = InstanceContextMode.PerSession;
            serviceBehavior.ConcurrencyMode = ConcurrencyMode.Single;
            serviceBehavior.MaxItemsInObjectGraph = int.MaxValue;
            serviceBehavior.UseSynchronizationContext = false;
         }
         DurableServiceAttribute durableService = new DurableServiceAttribute();
         durableService.SaveStateInOperationTransaction = true;
         serviceDescription.Behaviors.Add(durableService);
         if(serviceDescription.Behaviors.Any(behavior=>behavior is ActorStateProviderAttribute) == false)
         {
            StatePersistenceAttribute persistenceMode = serviceDescription.ServiceType.GetCustomAttribute<StatePersistenceAttribute>();
            if (persistenceMode == null)
            {
               throw new InvalidOperationException("Validation failed. Actor " + serviceDescription.ServiceType.Name + " is missing a StatePersistenceAttribute." );
            }
            else
            {
               if(persistenceMode.StatePersistence != StatePersistence.Persisted)
               {
                  serviceDescription.Behaviors.Add(new VolatileActorStateProviderAttribute());
               }
               else
               {
                  serviceDescription.Behaviors.Add(new KvsActorStateProviderAttribute());
               }
            }
         }
         serviceDescription.SetThrottle();
         ServiceThrottlingBehavior throttle = serviceDescription.Behaviors.Find<ServiceThrottlingBehavior>();
         if(throttle != null)
         {
            throttle.MaxConcurrentInstances = int.MaxValue;
         }
      }

      IEnumerable<ServiceRemotingDispatcher> GetDispatchers(Type actorType)
      {
         List<ServiceRemotingDispatcher> dispatchers = new List<ServiceRemotingDispatcher>();   
         StatefulServiceContext context = new StatefulServiceContext();

         Func<StatefulServiceContext,ActorTypeInformation,ActorService> factory = FabricRuntime.ActorServiceFactories[actorType];
         if(factory != null)
         {
            ActorService service = factory(context,ActorTypeInformation.Get(actorType));
            foreach(ServiceReplicaListener replicaListener in service.ServiceReplicaListeners)
            {
               IServiceRemotingListener listener = replicaListener.CreateCommunicationListener(context) as IServiceRemotingListener;
               if(listener.Dispatcher != null)
               {
                  dispatchers.Add(listener.Dispatcher);
               }
            }
         }
         return dispatchers;
      }
      void EnforceStatefulActorEndpointBehaviorPolicy(ServiceDescription serviceDescription)
      {
         IEnumerable<ServiceRemotingDispatcher> remotingDispatchers = GetDispatchers(serviceDescription.ServiceType);
         ActorInstanceContextProvider contextProvider = new ActorInstanceContextProvider();
         foreach(ServiceEndpoint endpoint in serviceDescription.Endpoints)
         {
            //All endpoints for a given Actor must share the same ActorInstanceContextProvider.
            endpoint.EndpointBehaviors.Add(contextProvider);
            endpoint.EndpointBehaviors.Add(new FabricThreadPoolBehavior());
            endpoint.EndpointBehaviors.Add(new StatefulActorInstanceProvider());
            endpoint.EndpointBehaviors.Add(new ServiceHeaderInterceptor(remotingDispatchers));

            foreach(OperationDescription operation in endpoint.Contract.Operations)
            {
               if(operation.TaskMethod == null)
               {
                  throw new InvalidOperationException("Validation failed. Actor operation '" + endpoint.Contract.ContractType.FullName + "." + operation.Name + "' does not return Task or Task<>. Actor interface methods must be async and must return either Task or Task<>." );
               }

               OperationBehaviorAttribute operationBehavior = operation.OperationBehaviors.FirstOrDefault(behavior=>behavior is OperationBehaviorAttribute) as OperationBehaviorAttribute;
               operationBehavior.TransactionScopeRequired = true;
               operation.OperationBehaviors.Add(new ActorOperationBehavior(remotingDispatchers));
            }
         }
      }
   }
}

#pragma warning restore 618