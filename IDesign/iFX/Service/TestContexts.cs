// © 2016 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Runtime.Serialization;

namespace IDesign.iFX.Contract
{
   [DataContract]
   public class MyContext
   {
      [DataMember]
      public string Value
      {get;set;}
      [DataMember]
      public int CallCount
      {get;set;}
   }
   [DataContract]
   public class MyContext2
   {
      [DataMember]
      public string Value
      {get;set;}
   }
}
