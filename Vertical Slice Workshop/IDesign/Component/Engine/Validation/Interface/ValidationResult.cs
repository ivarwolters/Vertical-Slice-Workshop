
using System.Runtime.Serialization;

namespace IDesign.Engine.Validation.Interface
{
   [DataContract]
   public class ValidationResult
   {
      [DataMember]
      public string Code { get; set; }

      [DataMember]
      public string Description { get; set; }
   }
}