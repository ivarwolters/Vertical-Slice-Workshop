// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.ServiceFabric.Extensions.Services;
#else
using ServiceFabricEx.Services;
#endif

namespace IDesign.iFX.Contract
{
   public class TestFlowPolicy : HeaderFlowManager
   {
      protected override void OnAddCommonHeaders(IHeaderAccessor accessor)
      {
         accessor.AddHeader<MyContext>(new MyContext { Value = "Call chain start" });
      }
      protected override void OnCopyCommonHeadersFrom(IHeaderAccessor accessor)
      {
         accessor.CopyHeaderFrom<MyContext>();
      }
      protected override void OnCopyCommonHeadersTo(IHeaderAccessor accessor)
      {
         accessor.CopyHeaderTo<MyContext>();
      }
   }
}
