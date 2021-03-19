// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ServiceModelEx.ServiceFabric.Actors.Runtime
{
   [Serializable]
   public sealed class ActorTypeInformation
   {
      public Type ImplementationType
      {get; private set;}
      public IEnumerable<Type> InterfaceTypes
      {get; private set;}
      public bool IsAbstract
      {get; private set;}
      public string ServiceName
      {get; private set;}
      public StatePersistence StatePersistence
      {get; private set;}

      public static ActorTypeInformation Get(Type actorType)
      {
         Debug.Assert(actorType != null);
         Debug.Assert(actorType.BaseType == typeof(Actor), "Actor must inherit from Actor class.");
         Debug.Assert(actorType.GetInterfaces().Contains(typeof(IActor)));

         IEnumerable<ApplicationManifestAttribute> manifests = actorType.GetCustomAttributes<ApplicationManifestAttribute>();
         Debug.Assert(manifests.Count() > 0, "Actor must possess one or more ApplicationManifestAttributes.");
         foreach(ApplicationManifestAttribute manifest in manifests)
         {
            Debug.Assert(ActorNaming.ValidateActorNames(actorType.Name,manifest.ServiceName), "All actor service names must end with 'ActorService'.");
         }

         StatePersistenceAttribute persistenceAttribute = actorType.GetCustomAttributes(typeof(StatePersistenceAttribute), false).SingleOrDefault() as StatePersistenceAttribute;
         Debug.Assert(persistenceAttribute != null, "Actor must possess a StatePersistenceAttribute.");

         return new ActorTypeInformation
         {
            ImplementationType = actorType,
            InterfaceTypes = actorType.GetInterfaces().Where(type=> type.Equals(typeof(IActor)) == false).ToArray(),
            IsAbstract = actorType.IsAbstract,
            ServiceName = ActorNaming.BuildActorServiceName(actorType.Name),
            StatePersistence = persistenceAttribute.StatePersistence   
         };
      }
      public static bool TryGet(Type actorType,out ActorTypeInformation actorTypeInformation)
      {
         actorTypeInformation = Get(actorType);  
         return actorTypeInformation != null;
      }
   }
}
