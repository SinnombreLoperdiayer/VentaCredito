using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Raps.Comun.Integraciones
{
    public enum EnumTipoNovedadRaps : int
    {
        Pordefecto = 0,
        NoAlcanzo = 1,
        NoDescargo = 2,
        FueraDeZona = 3,
        MotivoDevolucionFalsa = 4,
        FirmaFalsificada = 5,
        MalVestido = 6
    }
}
