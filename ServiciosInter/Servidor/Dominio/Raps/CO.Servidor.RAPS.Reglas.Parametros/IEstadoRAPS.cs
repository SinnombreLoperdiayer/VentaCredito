using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public interface IEstadoRAPS
    {

        /// <summary>
        /// Ejecuta reglas motivos raps
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="IdParametro"></param>
        /// <param name="parametrosParametrizacion"></param>
        string EjecutarReglaParametros(RADatosFallaDC datos);

    }
}
