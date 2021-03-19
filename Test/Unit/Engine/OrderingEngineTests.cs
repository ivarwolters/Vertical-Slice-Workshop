// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Test.Unit.Engine
{
   [TestClass]
   public class EngineUnitTests
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
