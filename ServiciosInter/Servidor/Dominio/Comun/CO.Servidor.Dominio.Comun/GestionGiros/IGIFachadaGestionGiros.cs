using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;

namespace CO.Servidor.Dominio.Comun.GestionGiros
{
    public interface IGIFachadaGestionGiros
    {
        /// <summary>
        /// Metodo que Adiciona una nueva Solicitud
        /// dependiendo del tipo de Motivo que tenga
        /// </summary>
        /// <param name="nvaSol"></param>
        void AdicionarNvaSolicitud(GISolicitudGiroDC nvaSol);

        /// <summary>
        /// Crear una solicitud dependiendo el motivo de la solicitud
        /// </summary>
        /// <param name="solicitudPago"></param>
        GISolicitudGiroDC AdicionarNvaSolicitud(GIEnumMotivoSolicitudDC motivo, GISolicitdPagoDC solicitudPago);

        /// <summary>
        /// Retorna o asigna las solicitudes de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador Admisión giro</param>
        /// <returns>Colección de solicitudes</returns>
        IEnumerable<GISolicitudGiroDC> ObtenerSolicitudesGiros(long idAdmisionGiro);

        /// <summary>
        /// Retorna el archivo seleccionado de la solicitud
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        string ObtenerArchivoSolicitud(long idSolicitud, GIArchSolicitudDC archivoSeleccionado);

        /// <summary>
        /// Metodo para ejecutar el proceso de anulación de un giro en caja
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        PGPagoPorDevolucionDC DevolverGiroCaja(long idAdmisionGiro, int idCaja, long idCajaIdCentroServiciosPagador);

        /// <summary>
        /// Convierte el estado del giro.
        /// </summary>
        /// <param name="estado">valor actual del estado.</param>
        /// <returns>la palabra del Estado</returns>
        GIEnumEstadosGirosDC ConvertirEstGiro(string estado);

        /// <summary>
        /// obtiene los ingresos y egresos por punto que reporta giros
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        List<puntoDeAtencion> consultaIngresosEgresosPuntosDeAtencion(string fechaInicial, string fechaFinal);

        /// <summary>
        /// Inserta en la tabla de [AuditoriaIntegraciones_AUD] cada vez que sesa consumido el servicio por 472
        /// </summary>
        void AuditarIntegracion472(string tipoIntegracion, string request, string response);        

        /// <summary>
        /// valida el usuario y contraseña para consumir el servicio por 472
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        bool ValidarPassword472(credencialDTO credencial);

        /// <summary>
        /// obtiene el valor real de la caja en los puntos
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        List<puntoDeAtencion> consultaValorRealPorPuntosDeAtencion();

        /// <summary>
        /// obtiene el valor real de la caja en el punto
        /// </summary>
        /// <returns></returns>
        List<puntoDeAtencion> consultaValorRealPorPuntoDeAtencion(string idCentroServicio);
    }
}