// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

using ServiceModelEx.Fabric;

namespace ServiceModelEx.ServiceFabric.Services
{
   internal class ServiceHeaderInterceptor : IEndpointBehavior,IDispatchMessageInspector
   {
      readonly IEnumerable<ServiceRemotingDispatcher> m_Dispatchers;

      public ServiceHeaderInterceptor(IEnumerable<ServiceRemotingDispatcher> dispatchers)
      {
         m_Dispatchers = dispatchers;
      }

      void OnReceiving(ServiceContext context,MessageHeaders headers)
      {
         if(m_Dispatchers != null)
         {
            foreach(ServiceRemotingDispatcher dispatcher in m_Dispatchers)
            {
               dispatcher.OnReceiving(context,headers);
            }
         }
      }
      void OnSending(ServiceContext context,MessageHeaders headers)
      {
         if(m_Dispatchers != null)
         {
            foreach(ServiceRemotingDispatcher dispatcher in m_Dispatchers)
            {
               dispatcher.OnSending(context,headers);
            }
         }
      }
      public object AfterReceiveRequest(ref Message request,IClientChannel channel,InstanceContext instanceContext)
      {
         ServiceContext context = ServiceContextHelper.Get(request.Headers);
         OnReceiving(context,request.Headers);
         return context;
      }
      public void BeforeSendReply(ref Message reply,object correlationState)
      {
         //Reply is actually null for one-way calls...
         if(reply != null && correlationState != null)
         {
            ServiceContext context = correlationState as ServiceContext;
            OnSending(context,reply.Headers);
         }
      }

      #region IEndpointBehavior
      public void AddBindingParameters(ServiceEndpoint endpoint,BindingParameterCollection bindingParameters)
      {}
      public void ApplyClientBehavior(ServiceEndpoint endpoint,ClientRuntime clientRuntime)
      {}
      public void ApplyDispatchBehavior(ServiceEndpoint endpoint,EndpointDispatcher endpointDispatcher)
      {
         endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
      }
      public void Validate(ServiceEndpoint endpoint)
      {}
      #endregion
   }
}
