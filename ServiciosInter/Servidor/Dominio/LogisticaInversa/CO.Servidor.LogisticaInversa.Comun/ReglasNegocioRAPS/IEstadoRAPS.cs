using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun.ReglasNegocioRAPS
{
    public interface IEstadoRAPS
    {
        //  Dictionary<string, object> EjecutarRegla(OUGuiaIngresadaDC guia, List<LIParametrizacionIntegracionRAPSDC> lstParametros, string[] datosMensajero, PUAgenciaDeRacolDC racolDestino, out EnumTipoNovedadRaps motivoRaps,int IdParametroAsociado);

        /// <summary>
        /// Ejecuta reglas motivos raps
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="IdParametro"></param>
        /// <param name="parametrosParametrizacion"></param>
        void EjecutarReglaParametros(OUGuiaIngresadaDC guia, int IdParametro, ref Dictionary<string, object> parametrosParametrizacion);
    }
}
