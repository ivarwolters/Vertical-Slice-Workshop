// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

namespace ServiceModelEx.ServiceFabric.Actors
{
   public static class ActorNaming
   {
      public static bool ValidateActorNames(string actorName,string serviceName)
      {
         bool valid = false;
         if(actorName.EndsWith("Actor"))
         {
            valid = serviceName.Equals(actorName + "Service");
         }
         else if(actorName.EndsWith("ActorService"))
         {
            valid = serviceName.Equals(actorName);   
         }
         else
         {
            valid = serviceName.Equals(actorName + "ActorService");
         }
         return valid;
      }
      public static string BuildActorServiceName(string actorName)
      {
         string serviceName = string.Empty;
         if(actorName.EndsWith("Actor"))
         {
            serviceName = actorName + "Service";
         }
         else if(actorName.EndsWith("ActorService"))
         {
            serviceName = actorName;   
         }
         else
         {
            serviceName = actorName + "ActorService";
         }
         return serviceName;
      }
   }
}
