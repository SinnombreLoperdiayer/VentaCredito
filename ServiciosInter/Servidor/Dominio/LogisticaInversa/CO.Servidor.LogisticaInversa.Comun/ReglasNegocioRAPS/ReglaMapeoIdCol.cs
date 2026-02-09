using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun.ReglasNegocioRAPS
{
    public class ReglaMapeoIdCol : IEstadoRAPS
    {
        /// <summary>
        ///  Agrega parametrizacion IdCentroLogistico
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="IdParametroAsociado"></param>
        /// <param name="parametrosParametrizacion"></param>
        public void EjecutarReglaParametros(OUGuiaIngresadaDC guia, int IdParametroAsociado, ref Dictionary<string, object> parametrosParametrizacion)
        {

               //parametrosParametrizacion = new Dictionary<string, object>();

            //motivoRaps=EnumTipoNovedadRaps.
            parametrosParametrizacion.Add(IdParametroAsociado.ToString(), guia.IdCentroLogistico);

        }
    }
}