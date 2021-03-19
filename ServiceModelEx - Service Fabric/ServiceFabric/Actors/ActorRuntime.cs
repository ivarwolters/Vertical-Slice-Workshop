// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using ServiceModelEx.Fabric;

namespace ServiceModelEx.ServiceFabric.Actors.Runtime
{
   public static class ActorRuntime
   {
      static void RegisterActor<T>() where T : ActorBase
      {
         Type actorType = typeof(T);
         FabricRuntime runtime = FabricRuntime.Create();

         string[] services = actorType.GetCustomAttributes<ApplicationManifestAttribute>().Select(manifest=>manifest.ServiceName).ToArray();
         if(services.Length > 0)
         {
            foreach(string serviceName in services)
            {
               if(serviceName.EndsWith("ActorService") == false)
               {
                  throw new InvalidOperationException("Validation failed. Invalid actor service name '" + serviceName + "'. Actor service name must end in 'ActorService'.");
               }
            }
         }
         if(FabricRuntime.Actors.Count > 0)
         {
            Type actorContract = actorType.GetInterfaces().Single(contract=>contract.Namespace.Contains("ServiceModelEx") == false); 

            string[] applications = actorType.GetCustomAttributes<ApplicationManifestAttribute>().Select(manifest=>manifest.ApplicationName).ToArray();
            foreach(string application in applications)
            {
               if(FabricRuntime.Actors.ContainsKey(application) &&
                  FabricRuntime.Actors[application].Any(type=>type.GetInterfaces().Any(contract=>contract.Equals(actorContract))))
               {
                  throw new InvalidOperationException("Validation failed. Actor interface " + actorContract.Name + " already exists within application " + application + " . An Actor interface must be unique within an application.");
               }
            }
         }
         runtime.RegisterServiceType(actorType.Name+"Type",actorType,Test.TestHelper.IsUnderTest());
      }
      static void RegisterActorServiceFactory<T>(Func<StatefulServiceContext,ActorTypeInformation,ActorService> actorServiceFactory) where T : ActorBase
      {
         Type actorType = typeof(T);
         if(FabricRuntime.ActorServiceFactories.ContainsKey(actorType) == false)
         {
            FabricRuntime.ActorServiceFactories.Add(actorType,actorServiceFactory);
         }
      }
      public static Task RegisterActorAsync<T>(TimeSpan timeout = default(TimeSpan),CancellationToken cancellationToken = default(CancellationToken)) where T : ActorBase
      {
         RegisterActor<T>();
         RegisterActorServiceFactory<T>(null);
         return Task.CompletedTask;
      }
      public static Task RegisterActorAsync<T>(Func<StatefulServiceContext,ActorTypeInformation,ActorService> actorServiceFactory,TimeSpan timeout = default(TimeSpan),CancellationToken cancellationToken = default(CancellationToken)) where T : ActorBase
      {
         RegisterActor<T>();
         RegisterActorServiceFactory<T>(actorServiceFactory);
         return Task.CompletedTask;
      }
   }
}
