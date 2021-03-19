using System;

namespace ServiceModelEx.Fabric
{
   [Serializable]
   public sealed class StatefulServiceContext : ServiceContext
   {
      internal StatefulServiceContext()
      {}
      internal StatefulServiceContext(Uri serviceName)
      {
         ServiceName = serviceName;
      }
   }
}
