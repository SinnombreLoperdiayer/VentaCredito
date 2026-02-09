using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.ParametrosFW
{
  public enum EnumPAEstadoEmpaque : short
  {
    CON_BOLSA_SEGURIDAD=1,
    SIN_BOLSA_SEGURIDAD=2,
    BIEN_EMBALADO=3,
    MAL_EMBALADO=4,
    SIN_SUMINISTRO=5
  }
}
