// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using Test.Client.Sales;

namespace Test.Host.Sales
{
   static class Hosting
   {
      static void Main()
      {
         try
         {
#if ServiceModelEx_ServiceFabric
            IDesign.Microservice.Sales.Hosting.Register();
            Console.WriteLine("Hosted...");
#endif

            Test.Client.Sales.Client testClient = new Test.Client.Sales.Client();
            testClient.Test().Wait();
            Console.WriteLine("Enter to close");
            Console.ReadLine();
         }
         catch
         {
            throw;
         }
      }
   }
}
