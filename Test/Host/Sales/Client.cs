// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using MethodModelEx.Microservices;
using IDesign.Manager.Sales.Interface;

#if ServiceModelEx_ServiceFabric
using ServiceModelEx.ServiceFabric.Extensions.Client;
using ServiceModelEx.ServiceFabric.Extensions.Services;
#else
using ServiceFabricEx.Client;
using ServiceFabricEx.Services;
#endif

namespace Test.Client.Sales
{
   public class Client
   {
      async Task Test_ISalesManager_FindItemAsync()
      {
         //Put your test call here...
         ISalesManager manager = Proxy.ForMicroservice<ISalesManager>();
         await manager.FindItemAsync(new FindItemRequest());
      }
      public async Task Test()
      {
         try
         {
            Console.WriteLine("Test Started...");
            await Test_ISalesManager_FindItemAsync();
            Console.WriteLine("Test Finished...");
         }
         catch(Exception ex)
         {
            Console.WriteLine("Test Exception: " + ex.InnerException != null ? ex.Message : ex.InnerException.Message);
         }
      }
   }
}
