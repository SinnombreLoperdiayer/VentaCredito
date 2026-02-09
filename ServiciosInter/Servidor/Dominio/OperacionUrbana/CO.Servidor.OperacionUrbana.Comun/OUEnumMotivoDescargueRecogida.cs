using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.OperacionUrbana.Comun
{
    public enum OUEnumMotivoDescargueRecogida : int
    {

        CANCELADA = 1,
        LOCAL_CERRADO = 2 ,
        MERCANCIA_NO_LISTA=3,
        FLETE_COSTOSO=4,
        MAL_EMPACADO=5,
        NO_ALCANZO=6,
        RECOGIDA_REALIZADA=7,
        DIRECCION_ERRADA=8,
        MERCANCIA_NO_TRANSPORTABLE=9,
        NO_CONOCEN_AL_CLIENTE=10,
        DATOS_INCOMPLETOS=11,
        VISITA_A_DESTIEMPO=12,
        CANCELADA_POR_CLIENTE=13,
        CANCELADA_POR_MENSAJERO=14,
        VENCIDA=15

    }
}
