// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MethodModelEx.Microservices
{
    public static class Hosting
    {
      static readonly string NamspaceRoot = Settings.NamespaceRoot;
      const string ComponentIdentifier = "Service";
      const string MicroserviceIdentifier = "Microservice";
      const string ResourceIdentifier = "Resource";
      const string HostingClassName = "Hosting";
      static Assembly[] m_Assemblies = new Assembly[0];

      static void LoadAssemblies()
      {
         DirectoryInfo baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
         FileInfo[] files = baseDirectory.GetFiles("*.dll").Where(f => f.Name.StartsWith(NamspaceRoot)).ToArray();

         foreach(FileInfo info in files)
         {
            try
            {
               Assembly.LoadFile(info.FullName);
            }
            catch
            {}
         }

         List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
         IEnumerable<Assembly> scannableAssemblies = assemblies.Where(assembly=>assembly.FullName.Contains(MicroserviceIdentifier) ||
                                                                                assembly.FullName.Contains(ResourceIdentifier));
         foreach(Assembly assembly in scannableAssemblies)
         {
            IEnumerable<AssemblyName> names = assembly.GetReferencedAssemblies();
            foreach(AssemblyName name in names)
            {
               Assembly referenced = Assembly.Load(name);
               assemblies.Add(referenced);
            }
         }
         m_Assemblies = assemblies.ToArray();
      }
      static Hosting()
      {
         LoadAssemblies();
      }

      static IEnumerable<Type> FindHosts(Predicate<Assembly> filter)
      {
         List<Type> hostTypes = new List<Type>();
         IEnumerable<Assembly> assemblies = m_Assemblies.Where(assembly=>filter(assembly));
         foreach(Assembly assembly in assemblies)
         {
            hostTypes.AddRange(assembly.GetExportedTypes().Where(type => type.FullName.Equals(HostingClassName)));
         }
         return hostTypes.ToArray();
      }
      static IEnumerable<Type> FindComponents()
      {
         return FindHosts((assembly)=>assembly.FullName.StartsWith(NamspaceRoot) && assembly.FullName.EndsWith(ComponentIdentifier));
      }
      static IEnumerable<Type> FindMicroservices()
      {
         return FindHosts((assembly)=>assembly.FullName.StartsWith(NamspaceRoot + "." + MicroserviceIdentifier));
      }
      static IEnumerable<Type> FindResources()
      {
         return FindHosts((assembly)=>assembly.FullName.StartsWith(NamspaceRoot + "." + ResourceIdentifier));
      }
      static void Host(IEnumerable<Type> hosts)
      {
         foreach (Type host in hosts)
         {
            host.InvokeMember("Register",BindingFlags.Public | BindingFlags.Static,null,null,new object[0]);
         }
      }

      public static class Components
      {
         public static void Register()
         {
            Host(FindComponents());
         }
      }
      public static class Microservices
      {
         public static void Register()
         {
            Host(FindMicroservices());
         }
      }
      public static class Resources
      {
         public static void Register()
         {
            Host(FindResources());
         }
      }
   }
}
