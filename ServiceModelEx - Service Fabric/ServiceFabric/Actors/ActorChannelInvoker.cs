// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;

using ServiceModelEx.ServiceFabric.Services;

namespace ServiceModelEx.ServiceFabric.Actors
{
   internal class ActorChannelInvoker<I> where I : class,IActor
   {
      public I Install(ChannelFactory<I> factory,ActorId actorid,ProxyRemotingInvoker remotingInvoker)
      {
         ChannelInvoker invoker = new ChannelInvoker(typeof(I),factory,actorid,remotingInvoker);
         return invoker.GetTransparentProxy() as I;
      }
      class ChannelInvoker : ChannelInvokerBase<I>
      {
         readonly ActorId m_ActorId;

         public ChannelInvoker(Type classToProxy,ChannelFactory<I> factory,ActorId actorId,ProxyRemotingInvoker remotingInvoker) : base(classToProxy,factory,remotingInvoker)
         {
            m_ActorId = actorId;
         }

         public override IMessage Invoke(IMessage message)
         {
            if((message as IMethodMessage).MethodName.Equals("get_Id"))
            {
               MethodCallMessageWrapper methodCallWrapper = new MethodCallMessageWrapper((IMethodCallMessage)message);
               return new ReturnMessage(m_ActorId,null,0,methodCallWrapper.LogicalCallContext,methodCallWrapper);
            }
            return base.Invoke(message);
         }
      }
   }
}
