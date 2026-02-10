using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public class ReglaMapeoFechaProgramada : IEstadoRAPS
    {
        /// <summary>
        /// agrega mapeo fecha programada
        /// </summary>
        /// <param name="datos"></param>
        /// <param name="IdParametroAsociado"></param>
        /// <param name="parametrosParametrizacion"></param>
        public string EjecutarReglaParametros(RADatosFallaDC datos)
        {
            return datos.FechaProgramacionRecogida.ToString();
        }
    }
}
