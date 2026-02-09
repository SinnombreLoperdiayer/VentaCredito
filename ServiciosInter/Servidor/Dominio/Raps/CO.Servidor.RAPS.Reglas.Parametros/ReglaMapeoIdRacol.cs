using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public class ReglaMapeoIdRacol : IEstadoRAPS
    {
        IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        /// <summary>
        ///  Agrega parametrizacion racolDestino.IdCentroServicio
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="IdParametroAsociado"></param>
        /// <param name="parametrosParametrizacion"></param>
        public string EjecutarReglaParametros(RADatosFallaDC datos)
        {

            PUCentroServiciosDC racolDestino = new PUCentroServiciosDC();
            racolDestino = fachadaCentroServicios.ObtenerTipoYResponsableCentroServicio(datos.IdCentroServicioDestino);
            return racolDestino.IdCentroServicio.ToString();
        }
    }
}
