// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ServiceModelEx.ServiceFabric.Actors.Runtime
{
   [Serializable]
   [ErrorHandlerBehavior]
   public class ActorBase : IActor
   {
      public ActorService ActorService
      {get; private set;}
      public string ApplicationName
      {get;private set;}
      public ActorId Id
      {get;private set;}
      public Uri ServiceUri
      {get;private set;}

      internal bool Activated
      {get;private set;}
      internal void Activate()
      {
         Activated = true;
         Id = GenericContext<ActorId>.Current.Value;

         Debug.Assert(!string.IsNullOrEmpty(Id.ApplicationName));
         ApplicationName = Id.ApplicationName;

         ApplicationManifestAttribute appManifest = this.GetType().GetCustomAttributes<ApplicationManifestAttribute>().SingleOrDefault(manifest=>manifest.ApplicationName.Equals(ApplicationName));
         Debug.Assert(appManifest != null);
         Debug.Assert(appManifest.ApplicationName.Equals(ApplicationName));
         ServiceUri = new Uri("fabric:/" + appManifest.ApplicationName + "/" + appManifest.ServiceName);

         ServiceContext context = GenericContext<ServiceContext>.Current.Value;
         ActorService = (context == null) ? new ActorService(null,ActorTypeInformation.Get(GetType())) : new ActorService(new StatefulServiceContext(context.ServiceName),ActorTypeInformation.Get(GetType()));
      }

      protected virtual Task OnActivateAsync()
      {
         return Task.FromResult(true);
      }
      protected virtual Task OnDeactivateAsync()
      {
         return Task.FromResult(true);
      }

      public async Task ActivateAsync()
      {  
         if(Activated == false)
         {
            Activate();
            await OnActivateAsync().FlowWcfContext();
         }
      }
      public async Task DeactivateAsync()
      {
         if(Activated)
         {
            await OnDeactivateAsync().FlowWcfContext();
            Activated = false;
         }
      }
   }
}
