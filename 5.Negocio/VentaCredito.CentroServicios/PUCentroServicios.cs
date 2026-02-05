using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Framework.Servidor.Servicios.ContratoDatos;
using Servicio.Entidades.Admisiones.Mensajeria;
using Servicio.Entidades.CentroServicios;
using VentaCredito.CentroServicios.Comun;
using VentaCredito.CentroServicios.Datos.Repositorio;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace VentaCredito.CentroServicios
{
    public class PUCentroServicios
    {

        private string conexionString = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private static readonly PUCentroServicios instancia = new PUCentroServicios();

        public static PUCentroServicios Instancia
        {
            get { return PUCentroServicios.instancia; }
        }

        /// <summary>
        /// Obtener la agencia a partir de la localida
        /// </summary>
        /// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad)
        {
            return CentroServicios.Datos.Repositorio.CentroServiciosRepositorio.Instancia.ObtenerAgenciaLocalidad(localidad);
        }

        /// <summary>
        /// Obtiene el centro de servicio.
        /// para el valor de la BaseInicial
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {
            return CentroServicios.Datos.Repositorio.CentroServiciosRepositorio.Instancia.ObtenerCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Actualiza la informació de validación de dos centros de servicios implicados en un trayecto
        /// </summary>
        /// <param name="localidadDestino">Localidad de destino del trayecto</param>
        /// <param name="idCentroServicio">Identificador del Centro de servicios que inicia la transacción</param>
        /// <param name="validacion">Contiene la información de las agencias implicadas en el proceso</param>
        public void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, long idCentroServicio, PALocalidadDC localidadOrigen = null)
        {
            CentroServiciosRepositorio.Instancia.ObtenerInformacionValidacionTrayectoAdo(localidadDestino, validacion, idCentroServicio, localidadOrigen);
        }

        /// <summary>
        /// Obtener información de validación del trayecto
        /// </summary>
        /// <param name="localidadOrigen"></param>
        /// <param name="idCentroServicioOrigen"></param>
        public void ObtenerInformacionValidacionTrayectoOrigen(PALocalidadDC localidadOrigen, ADValidacionServicioTrayectoDestino validacion)
        {
            CentroServicios.Datos.Repositorio.CentroServiciosRepositorio.Instancia.ObtenerInformacionValidacionTrayectoOrigen(localidadOrigen, validacion);
        }
    }
}
