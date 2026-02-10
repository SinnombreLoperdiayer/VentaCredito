using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.RAPS.Reglas.Parametros
{

    public class ReglaMapeoFotografia : IEstadoRAPS
    {
        /// <summary>
        /// agrega mapeo foto
        /// </summary>
        /// <param name="datos"></param>
        /// <returns></returns>
        public string EjecutarReglaParametros(RADatosFallaDC datos)
        {
            return datos.Foto;
        }
    }
}