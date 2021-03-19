using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IDesign.Engine.Validation.Interface
{
   [DataContract]
   public class ValidateResponse
   {
      [DataMember]
      public bool IsValid { get; set; }

      [DataMember]
      public IEnumerable<ValidationResult> ValidationMessages { get; set; }
   }
}
