// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using IDesign.iFX.Contract;
using IDesign.Engine.Validation.Service;
using IDesign.Access.Restaurant.Interface;
using IDesign.Access.Restaurant.Service;
using IDesign.Engine.Ordering.Service;
using IDesign.Access.Menu.Service;
using IDesign.Access.Customer.Service;
using IDesign.Engine.Pricing.Service;
using IDesign.Access.Specials.Service;
using IDesign.Manager.Sales.Service;
using Online = IDesign.Manager.Sales.Interface.Online;

namespace Test.Unit.Manager
{
   [TestClass]
   public class SalesManagerTests
   {
      UnitTestHarness harness = null;

      [TestInitialize]
      public void Setup()
      {
         harness = new UnitTestHarness();
         //TODO: Add your components here...
         harness.Setup(harness.ActorServiceFactory,
                       typeof(SalesManager),
                       typeof(ValidationEngine),
                       typeof(RestaurantAccess),
                       typeof(OrderingEngine),
                       typeof(CustomerAccess),
                       typeof(MenuAccess),
                       typeof(PricingEngine),
                       typeof(SpecialsAccess));
      }
      [TestCleanup]
      public void Cleanup()
      {
         harness.Cleanup();
      }

      #region SalesManager.ISalesManager
      [TestMethod]
      [TestCategory("Manager.SalesManager.ISalesManager")]
      public void Test_FindItemAsync_With_RestaurantMock_As_Poco()
      {
         MyContext contextMock = new MyContext { Value = "Test" };

         var restaurantAccessMock = new Mock<IRestaurantAccess>();
         restaurantAccessMock.Setup(x=>x.FilterAsync(new RestaurantCriteria()));

         //var validationEngineMock = new Mock<IValidationEngine>();
         //validationEngineMock.Setup(x=>x.ConfirmRequestAsync(It.IsAny<string>())).Returns<string>(r=>Task.FromResult(r + " ValidationEngineMock.ConfirmRequestAsync"));

         Action<Online.ISalesManager> callerMock = (poco) =>
         {
            //TEST NOTE: Cannot use await in test menhods.
            poco.FindItemAsync(new Online.FindItemRequest()).Wait();
         };

         harness.TestServicePoco<Online.ISalesManager>(callerMock,restaurantAccessMock,contextMock);
      }
      [TestMethod]
      [TestCategory("Manager.SalesManager.ISalesManager")]
      public void Test_FindItemAsync_With_RestaurantMock_As_Service()
      {
         MyContext contextMock = new MyContext { Value = "Test" };

         var restaurantAccessMock = new Mock<IRestaurantAccess>();
         restaurantAccessMock.Setup(x=>x.FilterAsync(new RestaurantCriteria()));

         Action<Online.ISalesManager> callerMock = (proxy) =>
         {
            proxy.FindItemAsync(new Online.FindItemRequest()).Wait();
         };

         harness.TestService<Online.ISalesManager>(callerMock,restaurantAccessMock,contextMock);
      }
      [TestMethod]
      [TestCategory("Manager.SalesManager.ISalesManager")]
      public void Test_FindItemAsync_With_NoMocks_As_Poco()
      {
         MyContext contextMock = new MyContext { Value = "Test" };

         Action<Online.ISalesManager> callerMock = (poco) =>
         {
            poco.FindItemAsync(new Online.FindItemRequest()).Wait();
         };

         harness.TestServicePoco<Online.ISalesManager>(callerMock,contextMock);
      }
      [TestMethod]
      [TestCategory("Manager.SalesManager.ISalesManager")]
      public void Test_FindItemAsync_With_NoMocks_As_Service()
      {
         MyContext contextMock = new MyContext { Value = "Test" };

         Action<Online.ISalesManager> callerMock = (proxy) =>
         {
            proxy.FindItemAsync(new Online.FindItemRequest()).Wait();
         };

         harness.TestService<Online.ISalesManager>(callerMock,contextMock);
      }
      #endregion
   }
}
