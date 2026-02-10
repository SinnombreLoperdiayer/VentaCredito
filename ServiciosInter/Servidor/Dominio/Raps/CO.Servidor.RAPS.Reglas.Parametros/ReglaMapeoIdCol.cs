using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public class ReglaMapeoIdCol : IEstadoRAPS
    {
        /// <summary>
        ///  Agrega parametrizacion IdCentroLogistico
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="IdParametroAsociado"></param>
        /// <param name="parametrosParametrizacion"></param>
         public string EjecutarReglaParametros(RADatosFallaDC datos)
        {

            return datos.IdCentroServicioDestino.ToString();
        }
    }
}