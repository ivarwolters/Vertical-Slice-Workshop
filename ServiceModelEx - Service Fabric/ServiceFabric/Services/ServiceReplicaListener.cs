// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using ServiceModelEx.Fabric;

namespace ServiceModelEx.ServiceFabric.Services.Communication.Runtime
{
   public sealed class ServiceReplicaListener
   {
      public const string DefaultName = "";
      public string Name
      {get; private set;}

      internal Func<StatefulServiceContext,ICommunicationListener> CreateCommunicationListener
      {get; private set;}

      public ServiceReplicaListener(Func<StatefulServiceContext,ICommunicationListener> createCommunicationListener,string name = "")
      {
         Name = name;
         CreateCommunicationListener = createCommunicationListener;
      }
   }
}