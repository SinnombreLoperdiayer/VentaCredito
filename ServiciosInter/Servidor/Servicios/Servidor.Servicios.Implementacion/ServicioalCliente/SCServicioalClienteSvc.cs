using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CO.Servidor.GestionGiro.ClienteConvenio;
using CO.Servidor.ServicioalCliente;
using CO.Servidor.Servicios.ContratoDatos.ServicioalCliente;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.Implementacion.ServicioalCliente
{
    /// <summary>
    ///Implementacion de Solicitudes Giros
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SCServicioalClienteSvc : ISCServicioalClienteSvc
    {
        #region Consultas

        /// <summary>
        /// Método para obtener una lista con los tipos de solicitud y subtipos asociados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACTipoSolicitudDC> ObtenerTiposSolicitud()
        {
            return SCAdministrador.Instancia.ObtenerTiposSolicitud();
        }

        /// <summary>
        /// Método para obtener una lista con los posibles estados de una solicitud
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACEstadosSolicitudDC> ObtenerEstados()
        {
            return SCAdministrador.Instancia.ObtenerEstados();
        }

        /// <summary>
        /// Método para obtener una lista con los tipos de seguimiento
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACTipoSeguimientoDC> ObtenerTiposSeguimiento()
        {
            return SCAdministrador.Instancia.ObtenerTiposSeguimiento();
        }

        /// <summary>
        /// Método para obtener una lista con los medios de recepción
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACMedioRecepcionDC> ObtenerMediosRecepcion()
        {
            return SCAdministrador.Instancia.ObtenerMediosRecepcion();
        }

        /// <summary>
        /// Método para obtener una lista con los medios de recepción
        /// </summary>
        /// <returns></returns>
        public SACSolicitudDC ObtenerSolicitud(long numeroGuia)
        {
            return SCAdministrador.Instancia.ObtenerSolicitud(numeroGuia);
        }

        #endregion Consultas

        #region Adicionar

        /// <summary>
        /// Método para adicionar una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public SACSolicitudDC GuardarCambiosSolicitud(SACSolicitudDC solicitud)
        {
            return SCAdministrador.Instancia.GuardarCambiosSolicitud(solicitud);
        }

        /// <summary>
        /// Método para adicionar un estado de una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public SACSeguimientoSolicitudDC GuardarSeguimiento(SACSeguimientoSolicitudDC seguimiento)
        {
            return SCAdministrador.Instancia.GuardarSeguimiento(seguimiento);
        }

        #endregion Adicionar
    }
}