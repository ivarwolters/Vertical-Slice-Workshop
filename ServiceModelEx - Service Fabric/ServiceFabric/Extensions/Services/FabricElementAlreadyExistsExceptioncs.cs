// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;

namespace ServiceModelEx.ServiceFabric.Extensions
{
   public class FabricElementAlreadyExistsException : Exception
   {
      public FabricElementAlreadyExistsException(string message) : base(message)
      {}
   }
}
