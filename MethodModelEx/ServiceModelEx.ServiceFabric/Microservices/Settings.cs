using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Web.Configuration;

namespace MethodModelEx.Microservices
{
   public static class Settings
   {
      static readonly NameValueCollection m_AppSettings;
      static readonly Process m_CurrentProcess;
      static Settings()
      {
         m_CurrentProcess = System.Diagnostics.Process.GetCurrentProcess();
         m_AppSettings = IsWebConfiguration() ? WebConfigurationManager.AppSettings : ConfigurationManager.AppSettings;
      }
      static NameValueCollection AppSettings
      {
         get
         {
            return m_AppSettings;
         }
      }
      static object GetSection(string sectionPath)
      {
         if(sectionPath == null)
         {
            throw new ArgumentNullException("sectionPath");
         }

         return IsWebConfiguration() ?
                WebConfigurationManager.GetSection(sectionPath) :
                ConfigurationManager.GetSection(sectionPath);
      }

      static string MachineName
      {
         get
         {
            return m_CurrentProcess.MachineName;
         }
      }
      static string ProcessName
      {
         get
         {
            return m_CurrentProcess.ProcessName;
         }
      }

      public static bool IsWebConfiguration()
      {
         return ProcessName.ToLower().Contains("w3wp") || ProcessName.ToLower().Contains("webdev.webserver");
      }
      public static Configuration GetConfiguration()
      {
         return IsWebConfiguration() ?
                WebConfigurationManager.OpenWebConfiguration(null) :
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      }

      public static string NamespaceRoot
      {
         get
         {
            string namespaceRoot = "Company";
            if(string.IsNullOrEmpty(AppSettings["NamespaceRoot"]) == false)
            {
               namespaceRoot = AppSettings["NamespaceRoot"];
            }
            return namespaceRoot;
         }
      }
   }
}