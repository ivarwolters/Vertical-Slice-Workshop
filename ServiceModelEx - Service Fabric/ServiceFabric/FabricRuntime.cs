// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

using ServiceModelEx.ServiceFabric;
using ServiceModelEx.ServiceFabric.Actors;
using ServiceModelEx.ServiceFabric.Actors.Runtime;
using ServiceModelEx.ServiceFabric.Services;
using ServiceModelEx.ServiceFabric.Services.Communication.Runtime;
using ServiceModelEx.ServiceFabric.Services.Communication.Wcf.Runtime;
using ServiceModelEx.ServiceFabric.Services.Runtime;

namespace ServiceModelEx.Fabric
{
   public sealed class FabricRuntime : IDisposable
   {
      internal static IEnumerable<Assembly> Assemblies
      {get;private set;}
      internal static Dictionary<string,Type[]> Services
      {get;set;}
      internal static Dictionary<string,Type[]> Actors
      {get;set;}
      internal static Dictionary<Type,Func<StatefulServiceContext,ActorTypeInformation,ActorService>> ActorServiceFactories
      {get;set;}

      static Dictionary<Type,ServiceHost> m_Hosts = new Dictionary<Type,ServiceHost>();
      static IEnumerable<Assembly> LoadAssemblies(string namespaceRoot)
      {
         List<Assembly> assemblies = new List<Assembly>();

         DirectoryInfo directories = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
         List<FileInfo> files = new List<FileInfo>(directories.GetFiles("*.dll"));
         files.AddRange(directories.GetFiles("*.exe"));
         files = new List<FileInfo>(files.Where(fileInfo=>!fileInfo.FullName.StartsWith("System") && (string.IsNullOrEmpty(namespaceRoot) ? true : fileInfo.Name.StartsWith(namespaceRoot))));

         foreach(FileInfo info in files)
         {
            try
            {
               assemblies.Add(Assembly.LoadFile(info.FullName));
            }
            catch
            {}
         }
         return assemblies.ToArray();
      }
      static readonly Type m_GenericServiceHostDefinition = null;

      public static FabricRuntime Create()
      {
         return Create(null);
      }
      public static FabricRuntime Create(string namespaceRoot = null)
      {
         FabricRuntime runtime = new FabricRuntime();
         if(Assemblies == null)
         {
            Assemblies = LoadAssemblies(namespaceRoot);
            Services = new Dictionary<string,Type[]>();
            Actors = new Dictionary<string,Type[]>();
            ActorServiceFactories = new Dictionary<Type, Func<StatefulServiceContext, ActorTypeInformation, ActorService>>();
            FabricThreadPoolHelper.ConfigureThreadPool();
         }
         return runtime;
      }
      public static void PurgeState()
      {
         ActorManager.PurgeState();
      }

      static FabricRuntime()
      {
         m_GenericServiceHostDefinition = typeof(ServiceHost<>).GetGenericTypeDefinition();
      }
      private FabricRuntime()
      {}
      public void Dispose()
      {}

      Type[] GetContracts(Type serviceType)
      {
         Type[] interfaces = serviceType.GetInterfaces();
         List<Type> contracts = new List<Type>();

         foreach(Type type in interfaces)
         {
            if(type.GetCustomAttributes(typeof(ServiceContractAttribute),false).Any())
            {
               contracts.Add(type);
            }
         }

         return contracts.ToArray();
      }
      void ValidateContract(Type serviceType,Type interfaceType)
      {
         Type[] contracts = GetContracts(serviceType);
         if(!contracts.Any(contractType=>contractType.Equals(interfaceType)))
         {
            throw new InvalidOperationException("Validation failed. Service Type " + serviceType.FullName + " does not implement interface type " + interfaceType.FullName + ".");
         }
      }
      void ActivateWcfService(Type serviceType,WcfCommunicationListener listener)
      {
         if(listener != null)
         {
            if(listener.ImplementationType.Equals(serviceType) == false)
            {
               throw new InvalidOperationException("Validation failed. Service Type " + serviceType.FullName + " does not match listener implementation type " + listener.ImplementationType.FullName + ".");
            }
            ValidateContract(serviceType,listener.InterfaceType);

            Type concreteHostType = m_GenericServiceHostDefinition.MakeGenericType(serviceType);
            ServiceHost host = Activator.CreateInstance(concreteHostType) as ServiceHost;

            NetTcpBinding binding = listener.Binding;
            if(binding == null)
            {
               binding = BindingHelper.Service.Wcf.ServiceBinding();
            }

            host.Description.Behaviors.Add(new StatelessServiceBehavior());

            IEnumerable<ApplicationManifestAttribute> manifests = serviceType.GetCustomAttributes<ApplicationManifestAttribute>();
            foreach(ApplicationManifestAttribute manifest in manifests)
            {
               host.AddServiceEndpoint(listener.InterfaceType,binding,AddressHelper.Wcf.BuildAddress("localhost",manifest.ApplicationName,manifest.ServiceName,listener.InterfaceType));
            }
            host.Open();
            AppDomain.CurrentDomain.ProcessExit += delegate
                                                   {
                                                      try
                                                      {
                                                         host.Close();
                                                      }
                                                      catch
                                                      {}
                                                   };
         }
      }
      void ActivateServiceType(Type serviceType)
      {
         //DESIGN NOTE:
         //Use the first manifest for initialization. For all Fabric-based interactions, ServiceContext sent in message updates context.
         ApplicationManifestAttribute[] manifests = serviceType.GetCustomAttributes<ApplicationManifestAttribute>().ToArray();
         StatelessServiceContext serviceContext = new StatelessServiceContext { ServiceName = new Uri("fabric:/" + manifests[0].ApplicationName + "/" + manifests[0].ServiceName) };

         StatelessService service = Activator.CreateInstance(serviceType,serviceContext) as StatelessService;
         foreach(ServiceInstanceListener instanceListener in service.ServiceInstanceListeners)
         {
            ICommunicationListener listener = instanceListener.CreateCommunicationListener(serviceContext);
            if(listener is WcfCommunicationListener)
            {
               ActivateWcfService(serviceType,listener as WcfCommunicationListener);
            }
            else
            {
               listener.OpenAsync(new System.Threading.CancellationToken()).Wait();
               AppDomain.CurrentDomain.ProcessExit += delegate
                                                      {
                                                         try
                                                         {
                                                            listener.CloseAsync(new System.Threading.CancellationToken()).Wait();
                                                         }
                                                         catch
                                                         {}
                                                      };
            }
         }
      }

      void RegisterServiceType(Dictionary<string,Type[]> applications,Type serviceType)
      {
         IEnumerable<ApplicationManifestAttribute> manifests = serviceType.GetCustomAttributes<ApplicationManifestAttribute>();
         Debug.Assert(manifests.Any());

         foreach (ApplicationManifestAttribute manifest in manifests)
         {
            Debug.Assert(manifest != null);
            Debug.Assert(!string.IsNullOrEmpty(manifest.ApplicationName));
            Debug.Assert(!string.IsNullOrEmpty(manifest.ServiceName));
            Debug.Assert(manifests.Count(attribute=>attribute.ApplicationName == manifest.ApplicationName) == 1);

            if(applications.ContainsKey(manifest.ApplicationName) == false)
            {
               applications.Add(manifest.ApplicationName,new Type[] {serviceType});
            }
            else
            {
               if((applications[manifest.ApplicationName].Any(type=>type.Equals(serviceType)) == false) &&
                  (applications[manifest.ApplicationName].Any(type=>type.GetCustomAttributes<ApplicationManifestAttribute>().Any(m=>m.ServiceName.Equals(manifest.ServiceName))) == false))
               {
                  List<Type> serviceTypes = new List<Type>(applications[manifest.ApplicationName]);
                  Debug.Assert(serviceTypes.Contains(serviceType) == false);

                  serviceTypes.Add(serviceType);
                  applications[manifest.ApplicationName] = serviceTypes.ToArray();
               }
            }
         }
      }
      internal void RegisterServiceType(string serviceTypeName,Type serviceType,bool forTest = false)
      {
         if(serviceType.IsSubclassOf(typeof(StatelessService)))
         {
            RegisterServiceType(FabricRuntime.Services,serviceType);
            if(forTest == false)
            {
               ActivateServiceType(serviceType);
            }
         }
         else if(serviceType.IsSubclassOf(typeof(ActorBase)))
         {
            RegisterServiceType(FabricRuntime.Actors,serviceType);
         }
         else
         {
            throw new InvalidOperationException("Registration failed. Invalid service type" + serviceType.Name + " encountered during registration.");
         }
      }
   }
}
