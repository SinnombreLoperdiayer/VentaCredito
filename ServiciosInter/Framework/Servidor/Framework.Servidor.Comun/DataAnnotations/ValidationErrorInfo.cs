using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Comun.DataAnnotations
{
  [DataContract]
  public class ValidationErrorInfo
  {
    public ValidationErrorInfo()
    {
    }

    public ValidationErrorInfo(ValidationResult vr)
    {
      Members = vr.MemberNames;
      Error = vr.ErrorMessage;
    }

    [DataMember]
    public IEnumerable<string> Members { get; set; }

    [DataMember]
    public string Error { get; set; }
  }
}