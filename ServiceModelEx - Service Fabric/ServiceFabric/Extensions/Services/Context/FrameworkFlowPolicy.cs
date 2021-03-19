// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   internal class FrameworkFlowPolicy : HeaderFlowManager
   {
      protected override void OnAddCommonHeaders(IHeaderAccessor accessor)
      {
         accessor.AddHeader(OriginationHelper.Create());
      }
      protected override void OnCopyCommonHeadersFrom(IHeaderAccessor accessor)
      {
         accessor.CopyHeaderFrom<OriginationContext>();
      }
      protected override void OnCopyCommonHeadersTo(IHeaderAccessor accessor)
      {
         accessor.CopyHeaderTo<OriginationContext>();
      }
   }
}
