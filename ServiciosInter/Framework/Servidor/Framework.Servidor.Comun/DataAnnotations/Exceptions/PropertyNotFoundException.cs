using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAnnotationFramework;

namespace Framework.Servidor.Comun.DataAnnotations.Exceptions
{
  public class PropertyNotFoundException : Exception
  {
    public PropertyNotFoundException(string property, Type objectType) :
      base(string.Format(Resource.PropertyNotFound, objectType.GetType(), property))
    {
      _property = property;
      _type = objectType;
    }

    string _property;
    Type _type;

    public string PropertyName
    {
      get { return _property; }
    }

    public Type Type
    {
      get { return _type; }
    }
  }
}