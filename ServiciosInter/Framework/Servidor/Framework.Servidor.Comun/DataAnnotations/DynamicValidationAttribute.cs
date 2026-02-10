using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Framework.Servidor.Comun.DataAnnotations
{
  public class DynamicValidationAttribute : ValidationAttribute
  {
    private string[] involvedPropertyNames;
    private bool success;

    public DynamicValidationAttribute()
      : base()
    {
    }

    public DynamicValidationAttribute(string message)
      : base(message)
    {
    }

    protected void DeclareDependency(params string[] properties)
    {
      involvedPropertyNames = properties;
    }

    public override string FormatErrorMessage(string name)
    {
      if (success) return string.Empty;
      return base.FormatErrorMessage(name);
    }

    protected ValidationResult ValidationResultFor(string error, ValidationContext validationContext, bool success)
    {
      this.success = success;
      string memberName = validationContext.MemberName;
      List<string> members = new List<string>();
      members.Add(memberName);
      foreach (string m in involvedPropertyNames)
      {
        if (m != null) members.Add(m);
      }
      return new ValidationResult(error, members);
    }
  }

  //internal class PropertyLocation
  //{
  //  internal BindWrapper wrapper { get; set; }

  //  internal string propertyName { get; set; }
  //}

  //internal class ValidationDependency
  //{
  //  internal bool MyUpdaterInformed { get; set; }

  //  internal List<PropertyLocation> ToUpdate { get; set; }
  //}
}