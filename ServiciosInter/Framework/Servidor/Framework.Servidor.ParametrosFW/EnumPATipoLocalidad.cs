using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.ParametrosFW
{
  [Flags]
  public enum EnumPATipoLocalidad : short
  {
    PAIS = 1,
    DEPARTAMENTO = 2,
    MUNICIPIO = 3,
    CORREGIMIENTO = 4,
    VEREDA = 5,
    INSPECCION = 6,
    CASERIO = 7
  }
}