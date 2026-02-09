using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.GestionGiros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Solicitudes.Giros.Solicitudes;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;

namespace CO.Servidor.Solicitudes.Giros
{
    public class GIFachadaGestionGiros : IGIFachadaGestionGiros
    {
        /// <summary>
        /// Metodo que Adiciona una nueva Solicitud
        /// dependiendo del tipo de Motivo que tenga
        /// </summary>
        /// <param name="nvaSol"></param>
        public void AdicionarNvaSolicitud(GISolicitudGiroDC nvaSol)
        {
            GISolicitud.Instancia.AdicionarNvaSolicitud(nvaSol);
        }

        /// <summary>
        /// Crear una solicitud dependiendo el motivo de la solicitud
        /// </summary>
        /// <param name="solicitudPago"></param>
        public GISolicitudGiroDC AdicionarNvaSolicitud(GIEnumMotivoSolicitudDC motivo, GISolicitdPagoDC solicitudPago)
        {
            switch (motivo)
            {
                case GIEnumMotivoSolicitudDC.MOTIVO_SOLICITUD_CAMBIO_AGENCIA:

                    GISolicitudGiroDC sol = GISolicitud.Instancia.AdicionarNvaSolicitudCambioAgencia(solicitudPago);

                    //Se valida si la solicitud es de cambio entre la misma ciudad, para realizar el proceso automatico
                    if (sol.CentroServicioOrigen.CiudadUbicacion.IdLocalidad == solicitudPago.CentroServicioSolicita.CiudadUbicacion.IdLocalidad)
                    {
                        sol.CentroQueSolicita = sol.CambioAgenciaPorAgencia;

                        //proceso de Aprobacion de Solicitud
                        GISolicitud.Instancia.ActualizarSolicitud(sol);
                        return null;
                    }
                    else
                    {
                        return sol;
                    }

                case GIEnumMotivoSolicitudDC.MOTIVO_SOLICITUD_CAMBIO_ESTADO:
                    return GISolicitud.Instancia.AdicionarNvaSolicitudCambioEstado(solicitudPago);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Retorna o asigna las solicitudes de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador Admisión giro</param>
        /// <returns>Colección de solicitudes</returns>
        public IEnumerable<GISolicitudGiroDC> ObtenerSolicitudesGiros(long idAdmisionGiro)
        {
            return GISolicitud.Instancia.ObtenerSolicitudesGiros(idAdmisionGiro);
        }

        /// <summary>
        /// Retorna el archivo seleccionado de la solicitud
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerArchivoSolicitud(long idSolicitud, GIArchSolicitudDC archivoSeleccionado)
        {
            return GISolicitud.Instancia.ObtenerArchivoSolicitud(idSolicitud, archivoSeleccionado);
        }

        /// <summary>
        /// Metodo para ejecutar el proceso de anulación de un giro en caja
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        public PGPagoPorDevolucionDC DevolverGiroCaja(long idAdmisionGiro,int idCaja,long IdCentroServiciosPagador)
        {
            return GIAdministradorSolicitudes.Instancia.DevolverGiroCaja(idAdmisionGiro, idCaja, IdCentroServiciosPagador);
        }

        /// <summary>
        /// Convierte el estado del giro.
        /// </summary>
        /// <param name="estado">valor actual del estado.</param>
        /// <returns>la palabra del Estado</returns>
        public GIEnumEstadosGirosDC ConvertirEstGiro(string estado)
        {
            return GISolicitud.Instancia.ConvertirEstGiro(estado);
        }

       
        /// <summary>
        /// Inserta en la tabla de [AuditoriaIntegraciones_AUD] cada vez que sesa consumido el servicio por 472
        /// </summary>
        public void AuditarIntegracion472(string tipoIntegracion, string request, string response)
        {
            GISolicitud.Instancia.RegistrarAuditoria472(tipoIntegracion, request, response);
        }

        /// <summary>
        /// valida el usuario y contraseña para consumir el servicio por 472
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        public bool ValidarPassword472(credencialDTO credencial)
        {
            return GISolicitud.Instancia.ValidarPassword472(credencial);
        }


        /// <summary>
        /// obtiene el valor real de la caja en los puntos
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<puntoDeAtencion> consultaValorRealPorPuntosDeAtencion()
        {
            return GISolicitud.Instancia.consultaValorRealPorPuntosDeAtencion();
        }

        /// <summary>
        /// obtiene el valor real de la caja en el punto
        /// </summary>
        /// <returns></returns>
        public List<puntoDeAtencion> consultaValorRealPorPuntoDeAtencion(string idCentroServicio)
        {
            return GISolicitud.Instancia.consultaValorRealPorPuntoDeAtencion(idCentroServicio);
        }

        /// <summary>
        /// Consulta los giros vendidor y pagados por algunos centros de servicio
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<puntoDeAtencion> consultaIngresosEgresosPuntosDeAtencion(string fechaInicial, string fechaFinal)
        {
            return GISolicitud.Instancia.consultaIngresosEgresosPuntosDeAtencion(fechaInicial, fechaFinal);
        }
    }
}