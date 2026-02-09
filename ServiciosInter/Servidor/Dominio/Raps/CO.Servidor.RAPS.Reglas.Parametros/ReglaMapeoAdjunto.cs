using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public class ReglaMapeoAdjunto : IEstadoRAPS
    {
        /// <summary>
        /// agrega mapeo adjunto
        /// </summary>
        /// <param name="datos"></param>
        /// <returns></returns>
        public string EjecutarReglaParametros(RADatosFallaDC datos)
        {
            return datos.Adjunto;
        }
    }
}