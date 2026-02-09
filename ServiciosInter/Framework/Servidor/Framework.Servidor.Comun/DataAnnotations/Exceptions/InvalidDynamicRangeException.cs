using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun.DataAnnotations.Exceptions
{
  public class InvalidDynamicRangeException : Exception
  {
    public InvalidDynamicRangeException(string message)
      : base(message)
    {
    }
  }
}