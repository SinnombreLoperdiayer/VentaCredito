using CO.Servidor.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun.ReglasNegocioRAPS
{
    public class ReglaMapeoIdRacol : IEstadoRAPS
    {
        /// <summary>
        ///  Agrega parametrizacion racolDestino.IdCentroServicio
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="IdParametroAsociado"></param>
        /// <param name="parametrosParametrizacion"></param>
        public void EjecutarReglaParametros(OUGuiaIngresadaDC guia, int IdParametroAsociado, ref Dictionary<string, object> parametrosParametrizacion)
        {
            PUAgenciaDeRacolDC racolDestino = new PUAgenciaDeRacolDC();
            racolDestino = PUCentroServicios.Instancia.ObtenerRacolResponsable(guia.IdCentroServicioDestino);


           // parametrosParametrizacion = new Dictionary<string, object>();

            parametrosParametrizacion.Add(IdParametroAsociado.ToString(), racolDestino.IdCentroServicio);

        }
    }
}
