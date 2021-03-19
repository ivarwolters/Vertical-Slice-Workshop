namespace IDesign.Microservice.Sales
{
   public static class Hosting
   {
      public static void Register()
      {
         IDesign.Manager.Sales.Service.Hosting.Register();
         IDesign.Engine.Validation.Service.Hosting.Register();
         IDesign.Engine.Ordering.Service.Hosting.Register();
         IDesign.Engine.Pricing.Service.Hosting.Register();
         IDesign.Access.Restaurant.Service.Hosting.Register();
         IDesign.Access.Customer.Service.Hosting.Register();
         IDesign.Access.Menu.Service.Hosting.Register();
         IDesign.Access.Specials.Service.Hosting.Register();
      }
    }
}
