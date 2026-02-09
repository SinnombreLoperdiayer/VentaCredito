using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public class ReglaMapeoObservaciones : IEstadoRAPS
    {
        /// <summary>
        ///  Agrega parametrizacion Observaciones
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="IdParametroAsociado"></param>
        /// <param name="parametrosParametrizacion"></param>
         public string EjecutarReglaParametros(RADatosFallaDC datos)
        {

              return datos.Observaciones;
        }
    }
}
