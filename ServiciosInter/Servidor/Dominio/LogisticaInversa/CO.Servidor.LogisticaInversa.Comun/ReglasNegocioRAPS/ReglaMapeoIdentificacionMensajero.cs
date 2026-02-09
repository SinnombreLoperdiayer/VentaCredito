using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun.ReglasNegocioRAPS
{
    public class ReglaMapeoIdentificacionMensajero : IEstadoRAPS
    {
        /// <summary>
        /// Agrega parametrizacion NombreCompleto
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="IdParametroAsociado"></param>
        /// <param name="parametrosParametrizacion"></param>
        public void EjecutarReglaParametros(OUGuiaIngresadaDC guia, int IdParametroAsociado, ref Dictionary<string, object> parametrosParametrizacion)
        {
            string[] datosMensajero = guia.NombreCompleto == null ? new string[1] : guia.NombreCompleto.Split('-');

            //parametrosParametrizacion = new Dictionary<string, object>();

            parametrosParametrizacion.Add(IdParametroAsociado.ToString(), datosMensajero[1]);

        }
    }
}
