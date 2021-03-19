using System.Diagnostics;
using System.Reflection;

using ServiceModelEx.ServiceFabric.Actors;
using ServiceModelEx.ServiceFabric.Actors.Runtime;

namespace ServiceModelEx.ServiceFabric.Test
{
   internal static class TestHelper
   {
      static readonly Process m_CurrentProcess = null;

      static TestHelper()
      {
         m_CurrentProcess = Process.GetCurrentProcess();
      }

      public static bool IsUnderTest()
      {
         string processName = m_CurrentProcess.ProcessName.ToLower();
         return processName.Contains("te.processhost.managed") || processName.Contains("vstest");
      }
      public static void ActivateActor<S>(IActor actor,S state)
      {
         if(actor != null)
         {
            actor.ActivateAsync().Wait();
            if(state != null)
            {
               IActorStateManager stateManager = actor.GetType().InvokeMember("StateManager",BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.GetProperty,null,actor,null) as IActorStateManager;
               if(stateManager != null)
               {
                  stateManager.SetStateAsync<S>(typeof(S).FullName,state);
               }
            }
         }
      }
   }
}
