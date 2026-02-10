using CO.Servidor.Servicios.ContratoDatos.ServicioalCliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.ServicioalCliente
{
    public class SCAdministrador
    {
        private static readonly SCAdministrador instancia = new SCAdministrador();

        /// <summary>
        /// Retorna la instancia de la clase SCRepositorio
        /// </summary>
        public static SCAdministrador Instancia
        {
            get { return SCAdministrador.instancia; }
        }

        #region Consultas


        /// <summary>
        /// Método para obtener una lista con los tipos de solicitud y subtipos asociados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACTipoSolicitudDC> ObtenerTiposSolicitud()
        {
            return SCServicioalCliente.Instancia.ObtenerTiposSolicitud();
        }


        /// <summary>
        /// Método para obtener una lista con los posibles estados de una solicitud
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACEstadosSolicitudDC> ObtenerEstados()
        {
            return SCServicioalCliente.Instancia.ObtenerEstados();
        }

        /// <summary>
        /// Método para obtener una lista con los tipos de seguimiento
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACTipoSeguimientoDC> ObtenerTiposSeguimiento()
        {
            return SCServicioalCliente.Instancia.ObtenerTiposSeguimiento();
        }


        /// <summary>
        /// Método para obtener una lista con los medios de recepción
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACMedioRecepcionDC> ObtenerMediosRecepcion()
        {
            return SCServicioalCliente.Instancia.ObtenerMediosRecepcion();
        }

        /// <summary>
        /// Método para obtener una lista con los medios de recepción
        /// </summary>
        /// <returns></returns>
        public SACSolicitudDC ObtenerSolicitud(long numeroGuia)
        {
          return SCServicioalCliente.Instancia.ObtenerSolicitud(numeroGuia);
        }


        #endregion


        #region Adicionar

        /// <summary>
        /// Método para adicionar una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public SACSolicitudDC GuardarCambiosSolicitud(SACSolicitudDC solicitud)
        {
            return SCServicioalCliente.Instancia.GuardarCambiosSolicitud(solicitud);
        }

        /// <summary>
        /// Método para adicionar un estado de una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public SACSeguimientoSolicitudDC GuardarSeguimiento(SACSeguimientoSolicitudDC seguimiento)
        {
            return SCServicioalCliente.Instancia.GuardarSeguimiento(seguimiento);
        }


        #endregion
    }
}
