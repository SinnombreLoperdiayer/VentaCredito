using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.ExploradorGiros;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.PagosManuales;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.Telemercadeo;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;

namespace CO.Servidor.Servicios.Contratos
{
    public interface IGIExploradorGiros
    {
        /// <summary>
        /// Obtetener colección giros
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="indicePagina">Indice Página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <returns>Colección giros</returns>
        IEnumerable<GIAdmisionGirosDC> ObtenerGirosExplorador(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, bool incluyeFecha, out int totalRegistros);

        /// <summary>
        /// Obtiene los tipos de giros
        /// </summary>
        /// <returns>Colección con los tipos de giros</returns>
        IEnumerable<GITipoGiroDC> ObtenerTiposGiros();

        /// <summary>
        /// Obtiene los valores adicionales de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección valores adicionales</returns>
        IEnumerable<TAValorAdicional> ObtenerValoresAdicionalesGiro(long idGiro);

        /// <summary>
        /// Obtiene los impuestos de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección impuestos giros</returns>
        IEnumerable<TAImpuestoDelServicio> ObtenerImpuestosGiros(long idGiro);

        /// <summary>
        /// Obtiene la información del pago
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión</param>
        /// <returns>Objeto pago</returns>
        PGPagosGirosDC ObtenerInformacionPago(long idAdmisionGiro);

        /// <summary>
        /// Retorna el comprobante de pago de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        string ObtenerArchivosGiros(long numeroGiro);

        /// <summary>
        /// Obtiene el archivo adjunto Solicitud.
        /// </summary>
        /// <param name="idArchivo">Valor del id del archivo adjunto.</param>
        /// <returns>la direccion en bd del archivo</returns>
        string ObtenerArchivoAdjuntoSolicitud(long idArchivo);

        /// <summary>
        /// Obtiene la informacíón dependiendo del tipo de giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <param name="tipoGiro">Tipo de giro</param>
        /// <returns>Objeto datos convenio</returns>
        GIAdmisionGirosDC ObtenerInformacionTipoGiro(long idAdmisionGiro, string tipoGiro);

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
        /// Obtiene la informacion de los intentos a transmitir de un giro
        /// </summary>
        /// <param name="idAdminGiro"></param>
        /// <returns></returns>
        List<GIIntentosTransmisionGiroDC> ObtenerIntentosTransmitir(long idAdminGiro);

        /// <summary>
        /// Consulta la informacion basica de una factura por el numero de Operacion( Numero de giro)
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <returns></returns>
        FAOperacionFacturadaDC ConsultarFacturaPorNumeroOperacion(long numeroOperacion);

        /// <summary>
        /// Obtiene la informacion del almacen
        /// </summary>
        /// <param name="idOperacion">Numero del giro</param>
        /// <returns></returns>
        List<CCAlmacenDC> ObtenerAlmacenControlCuentas(long idOperacion);

        /// <summary>
        /// Obtiene los estados de un giro a traves del tiempo
        /// </summary>
        /// <param name="idAdminGiro">Es el idAdmin del Giro</param>
        /// <returns>Lista de estados y Fecha</returns>
        IList<GIEstadosGirosDC> ObtenerEstadosGiro(long idAdminGiro);

        /// <summary>
        /// Obtiene la informacion de Telemercadeo de
        /// un giro especifico
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns>la info del telemercadeo de un giro</returns>
        GITelemercadeoGiroDC ObtenerTelemercadeoDeGiro(long idAdmisionGiro);

        /// <summary>
        /// Obtiene reporte para la uiaf de giros
        /// </summary>
        /// <param name="fechaGeneracion">Fecha para generar el reporte</param>
        /// <returns>Lista con los registros a reportar</returns>
        List<string> ObtenerReporteUIAFGiros(DateTime fechaGeneracion);

        /// <summary>
        /// Obtiene la informacíón básica de un giro
        /// </summary>
        /// <param name="numeroGiro">Número del giro</param>
        /// <param name="idRemitente">No. identificación del Remitente</param>
        /// <param name="idDestinatario">No. identificación del Destinatario</param>
        /// <returns>Objeto datos convenio</returns>
        GIExploradorGirosWebDC ObtenerDatosGiros(GIExploradorGirosWebDC informacionGiro);

        /// <summary>
        /// obtiene los motivos de bloqueo
        /// </summary>
        /// <returns></returns>
        List<GIMotivosInactivacion> ObtenerMotivosGiros();

        /// <summary>
        /// obtiene todas los usuarios internas
        /// </summary>
        /// <returns></returns>
        List<Servicios.ContratoDatos.CentroServicios.PUArchivosPersonas> ObtenerTodasPersonasInternas();

        /// <summary>
        /// agrega el motivo de bloqueo
        /// </summary>
        /// <param name="GIMotivo"></param>
        void AgregarMotivoBloqueo(GIMotivosBloqueoDesbloqueo GIMotivo);


    }
}
