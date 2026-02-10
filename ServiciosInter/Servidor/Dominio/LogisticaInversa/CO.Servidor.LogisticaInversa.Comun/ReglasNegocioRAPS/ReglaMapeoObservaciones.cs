using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun.ReglasNegocioRAPS
{
    public class ReglaMapeoObservaciones : IEstadoRAPS
    {
        /// <summary>
        ///  Agrega parametrizacion Observaciones
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="IdParametroAsociado"></param>
        /// <param name="parametrosParametrizacion"></param>
        public void EjecutarReglaParametros(OUGuiaIngresadaDC guia, int IdParametroAsociado, ref Dictionary<string, object> parametrosParametrizacion)
        {

           // parametrosParametrizacion = new Dictionary<string, object>();

            parametrosParametrizacion.Add(IdParametroAsociado.ToString(), guia.Observaciones);

        }
    }
}
