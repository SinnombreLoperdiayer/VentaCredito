using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Enumerables
{
    public enum EnumEstadoRegistro
    {
        SIN_CAMBIOS = 0x1,
        ADICIONADO = 0x2,
        MODIFICADO = 0x4,
        BORRADO = 0x8
    }
}
