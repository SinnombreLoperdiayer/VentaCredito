using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public class ReglaMapeoFechaDescarga : IEstadoRAPS
    {
        /// <summary>
        /// Agrega parametrizacion FechaMotivoDevolucion
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="IdParametroAsociado"></param>
        /// <param name="parametrosParametrizacion"></param>
         public string EjecutarReglaParametros(RADatosFallaDC datos)
        {

            return datos.FechaDescarga.ToString();
            
        }
    }
}
