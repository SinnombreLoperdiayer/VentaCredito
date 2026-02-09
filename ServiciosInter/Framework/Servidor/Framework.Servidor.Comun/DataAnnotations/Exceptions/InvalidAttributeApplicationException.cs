using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun.DataAnnotations.Exceptions
{
  public class InvalidAttributeApplicationException : Exception
  {
    public InvalidAttributeApplicationException(string message)
      : base(message)
    {
    }
  }
}