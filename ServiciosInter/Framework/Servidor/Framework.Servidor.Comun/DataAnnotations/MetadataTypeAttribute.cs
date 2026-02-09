using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun.DataAnnotations
{
  [AttributeUsage(AttributeTargets.Class)]
  public class MetadataTypeAttribute : Attribute
  {
    public Type MetadataClassType { get; set; }

    public MetadataTypeAttribute(Type metadataClassType)
      : base()
    {
      MetadataClassType = metadataClassType;
    }
  }
}