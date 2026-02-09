using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Almacen;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.ExploradorGiros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;

namespace CO.Servidor.Servicios.Contratos
{
    public interface IGIRepositorioExploradorGiros
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
        ///  Metodo que obtiene el id de la admision a partir del numero del giro
        /// </summary>
        /// <param name="numeroGuia">Numero de la guía</param>
        /// <returns>Identificador de la admisión de la guía</returns>
        long ValidarGiro(long numeroGiro);

        /// <summary>
        ///    /// Metodo que obtiene el id de la admision a partir del pago
        /// </summary>
        /// <param name="numeroGuia">Numero de la guía</param>
        long ValidarPago(long numeroPago);

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
        /// <param name="numeroGiro">Numero de giro</param>
        /// <returns>objeto con datos de archivo</returns>
        AlmacenArchivoPagoGiro_LOI ObtenerArchivosGiros(long numeroGiro);

        /// <summary>
        /// Obtiene el archivo adjunto Solicitud.
        /// </summary>
        /// <param name="idArchivo">Valor del id del archivo adjunto.</param>
        /// <returns>la direccion en bd del archivo</returns>
        string ObtenerArchivoAdjuntoSolicitud(long idArchivo);

        /// <summary>
        /// Obtiene la información del convenio peatón peatón
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns></returns>
        GIGirosPeatonPeatonDC ObtenerConvenioPeatonPeaton(long idAdmisionGiro);

        /// <summary>
        /// Consultar la informacion de la tabla peaton Convenio
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        GIGirosPeatonConvenioDC ConsultarInformacionPeatonConvenio(long idAdmisionGiro);

        /// <summary>
        /// Obtiene los estados de un giro a traves del tiempo
        /// </summary>
        /// <param name="idAdminGiro">Es el idAdmin del Giro</param>
        /// <returns>Lista de estados y Fecha</returns>
        IList<GIEstadosGirosDC> ObtenerEstadosGiro(long numeroGiro);

        /// <summary>
        /// Obtiene reporte para la uiaf de giros
        /// </summary>
        /// <param name="fechaGeneracion">Fecha para generar el reporte</param>
        /// <returns>Lista con los registros a reportar</returns>
        List<string> ObtenerReporteUIAFGiros(DateTime fechaGeneracion);

        /// <summary>
        /// Metodo para Obtener información de un giro
        /// </summary>
        /// <param name="informacionGiro"></param>
        /// <returns></returns>
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
        List<PUArchivosPersonas> ObtenerTodasPersonasInternas();

        /// <summary>
        /// agrega el motivo de bloqueo
        /// </summary>
        /// <param name="GIMotivo"></param>
        void AgregarMotivoBloqueo(GIMotivosBloqueoDesbloqueo GIMotivo);


    }
}
