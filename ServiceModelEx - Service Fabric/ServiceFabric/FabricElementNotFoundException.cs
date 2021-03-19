// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;

namespace ServiceModelEx.Fabric
{
   public class FabricElementNotFoundException : Exception
   {
      public FabricElementNotFoundException() : base()
      {}
      public FabricElementNotFoundException(string message) : base(message)
      {}
      public FabricElementNotFoundException(string message,Exception innerException) : base(message,innerException)
      {}
   }
}
