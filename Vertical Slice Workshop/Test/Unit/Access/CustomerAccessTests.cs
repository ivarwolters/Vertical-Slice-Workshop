// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Access
{
   [TestClass]
   public class CustomerAccessTests
   {
      UnitTestHarness harness = null;

      [TestInitialize]
      public void Setup()
      {
         harness = new UnitTestHarness();
         harness.Setup(harness.ActorServiceFactory);
      }
      [TestCleanup]
      public void Cleanup()
      {
         harness.Cleanup();
      }
   }
}
