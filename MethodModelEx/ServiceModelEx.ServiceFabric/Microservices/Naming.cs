// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Diagnostics;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.ServiceFabric.Actors;
using ServiceModelEx.ServiceFabric.Actors.Runtime;
using ServiceModelEx.ServiceFabric.Services.Remoting;
using ServiceModelEx.ServiceFabric.Services.Runtime;
#else
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Runtime;
#endif

namespace MethodModelEx.Microservices
{
   public static class Naming
   {
      public static string Microservice<I>() where I : IService
      {
         Debug.Assert(typeof(I).Namespace.Contains("Manager"), "Invalid microservice interface. Use only the Manager interface to access a microservice.");
         string[] namespaceSegments = typeof(I).Namespace.Split('.');
         return namespaceSegments[0] + ".Microservice." + namespaceSegments[2];
      }
      public static string Resource<I>() where I : IActor
      {
         string[] namespaceSegments = typeof(I).Namespace.Split('.');
         return namespaceSegments[0] + ".Resource." + namespaceSegments[2];
      }
      public static string Component<I>() where I : IService
      {
         string[] namespaceSegments = typeof(I).Namespace.Split('.');
         return namespaceSegments[2] + namespaceSegments[1];
      }
      public static string Listener<I>() where I : IService
      {
         return typeof(I).Name;
      }

      public static string ServiceType<T>() where T : StatelessService
      {
         return typeof(T).FullName.Replace(".Service","");
      }
      public static string ActorType<T>() where T : Actor
      {
         return typeof(T).FullName.Replace(".Service","");
      }
   }
}
