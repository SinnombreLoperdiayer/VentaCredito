using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public class ReglaMapeoTipoObjeto : IEstadoRAPS
    {
        /// <summary>
        /// agrega mapeo tipobojeto
        /// </summary>
        /// <param name="datos"></param>
        /// <returns></returns>
        public string EjecutarReglaParametros(RADatosFallaDC datos)
        {
            return datos.TipoObjeto;
        }




    }

}