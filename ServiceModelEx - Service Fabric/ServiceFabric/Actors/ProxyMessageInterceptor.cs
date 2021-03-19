// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services;

namespace ServiceModelEx.ServiceFabric.Actors
{
   internal class ProxyMessageInterceptor : IEndpointBehavior,IClientMessageInspector
   {
      ActorId ActorId
      {get;set;}
      ServiceContext Context
      {get;set;}
      ProxyRemotingInterceptor RemotingInterceptor
      {get;set;}

      public ProxyMessageInterceptor(ActorId actorId,ServiceContext context,ProxyRemotingInterceptor remotingInterceptor)
      {
         ActorId = actorId;
         Context = context;
         RemotingInterceptor = remotingInterceptor;
      }

      void OnAddHeaders(MessageHeaders headers)
      {
         if(RemotingInterceptor != null)
         {
            RemotingInterceptor.OnAddHeaders(headers);
         }
      }
      void OnGetHeaders(MessageHeaders headers)
      {
         if(RemotingInterceptor != null)
         {
            RemotingInterceptor.OnGetHeaders(headers);
         }
      }
      void OnSending(MessageHeaders headers)
      {
         if(RemotingInterceptor != null)
         {
            RemotingInterceptor.OnSending(Context.ServiceName,headers);
         }
      }
      void OnReceiving(MessageHeaders headers)
      {
         if(RemotingInterceptor != null)
         {
            RemotingInterceptor.OnReceiving(Context.ServiceName,headers);
         }
      }
      public void AfterReceiveReply(ref Message reply,object correlationState)
      {
         OnReceiving(reply.Headers);
         OnGetHeaders(reply.Headers);
      }
      public object BeforeSendRequest(ref Message request,IClientChannel channel)
      {
         ActorIdHelper.Add(request.Headers,ActorId);
         ServiceContextHelper.Add(request.Headers,Context);
         OnAddHeaders(request.Headers);
         OnSending(request.Headers);
         return null;
      }

      #region IEndpointBehavior
      public void AddBindingParameters(ServiceEndpoint endpoint,BindingParameterCollection bindingParameters)
      {}
      public void ApplyClientBehavior(ServiceEndpoint endpoint,ClientRuntime clientRuntime)
      {
         clientRuntime.ClientMessageInspectors.Add(this);
      }
      public void ApplyDispatchBehavior(ServiceEndpoint endpoint,EndpointDispatcher endpointDispatcher)
      {}
      public void Validate(ServiceEndpoint endpoint)
      {}
      #endregion
   }
}