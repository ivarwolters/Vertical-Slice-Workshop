// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;

namespace ServiceModelEx.ServiceFabric.Services
{
   internal class ServiceChannelInvoker<I> where I : class
   {
      public I Install(ChannelFactory<I> factory,ProxyRemotingInvoker remotingInvoker)
      {
         ChannelInvoker invoker = new ChannelInvoker(typeof(I),factory,remotingInvoker);
         return invoker.GetTransparentProxy() as I;
      }
      class ChannelInvoker : ChannelInvokerBase<I>
      {
         public ChannelInvoker(Type classToProxy,ChannelFactory<I> factory,ProxyRemotingInvoker remotingInvoker) : base(classToProxy,factory,remotingInvoker)
         {}
      }
   }
}
