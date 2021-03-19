// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Collections.Generic;

using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace ServiceModelEx.ServiceFabric.Services
{
   public class ServiceOperationBehavior : IOperationBehavior
   {
      readonly IEnumerable<ServiceRemotingDispatcher> m_Dispatchers;

      public ServiceOperationBehavior(IEnumerable<ServiceRemotingDispatcher> dispatchers)
      {
         m_Dispatchers = dispatchers;
      }
      public void AddBindingParameters(OperationDescription operationDescription,BindingParameterCollection bindingParameters)
      {}
      public void ApplyClientBehavior(OperationDescription operationDescription,ClientOperation clientOperation)
      {}
      public void ApplyDispatchBehavior(OperationDescription operationDescription,DispatchOperation dispatchOperation)
      {
         dispatchOperation.Invoker = new ServiceOperationInvoker(dispatchOperation.Invoker,operationDescription,m_Dispatchers);
      }
      public void Validate(OperationDescription operationDescription)
      {}
   }
}
