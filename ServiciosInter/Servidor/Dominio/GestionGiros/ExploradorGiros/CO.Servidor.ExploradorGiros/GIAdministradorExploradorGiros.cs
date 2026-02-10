using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.LogisticaInversa;
using CO.Servidor.Dominio.Comun.Middleware;
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
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;
using Giros.SharedTypes.BE;
using RestSharp;

namespace CO.Servidor.ExploradorGiros
{
    public enum DocumentosIdentidad : int
    {
        CEDULA_CIUDADANIA = 1,
        CEDULA_EXTRANJERIA = 2,
        NIT = 3,
        TARJETA_IDENTIDAD = 4
    };

    /// <summary>
    /// Clase que expone la fachada del explorador de giros
    /// </summary>
    public class GIAdministradorExploradorGiros : IGIAdministradorExploradorGiros
    {
        #region Campos

        static volatile GIAdministradorExploradorGiros instancia;
        IGIExploradorGiros exploradorGiros = GIExploradorGiros.Instancia;

        #endregion Campos

        #region Propiedades
        /// <summary>
        /// Atributo utilizado para evitar problemas con multithreading en el singleton.
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// Instancia de acceso ( Singleton con multithreading )
        /// </summary>
        public static GIAdministradorExploradorGiros Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (syncRoot)
                    {
                        if (instancia == null)
                        {
                            instancia = new GIAdministradorExploradorGiros();
                        }
                    }
                }
                return instancia;
            }
        }
        #endregion Propiedades

        #region Métodos Públicos

        /// <summary>
        /// Obtener colección giros
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="indicePagina">Indice Página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <returns>Colección giros</returns>
        public IEnumerable<GIAdmisionGirosDC> ObtenerGirosExplorador(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, bool incluyeFecha, out int totalRegistros)
        {
            return exploradorGiros.ObtenerGirosExplorador(filtro, indicePagina, registrosPorPagina, incluyeFecha, out totalRegistros);
        }

        /// <summary>
        /// Obtiene los tipos de giros
        /// </summary>
        /// <returns>Colección con los tipos de giros</returns>
        public IEnumerable<GITipoGiroDC> ObtenerTiposGiros()
        {
            return exploradorGiros.ObtenerTiposGiros();
        }

        /// <summary>
        /// Obtiene los valores adicionales de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección valores adicionales</returns>
        public IEnumerable<TAValorAdicional> ObtenerValoresAdicionalesGiro(long idGiro)
        {
            return exploradorGiros.ObtenerValoresAdicionalesGiro(idGiro);
        }

        /// <summary>
        /// Obtiene los impuestos de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección impuestos giros</returns>
        public IEnumerable<TAImpuestoDelServicio> ObtenerImpuestosGiros(long idGiro)
        {
            return exploradorGiros.ObtenerImpuestosGiros(idGiro);
        }

        /// <summary>
        /// Obtiene la información del pago
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión</param>
        /// <returns>Objeto pago</returns>
        public PGPagosGirosDC ObtenerInformacionPago(long idAdmisionGiro)
        {
            return exploradorGiros.ObtenerInformacionPago(idAdmisionGiro);
        }

        /// <summary>
        /// Retorna el comprobante de pago de un giro
        /// </summary>
        /// <param name="numeroGiro">Numero de giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerArchivosGiros(long numeroGiro)
        {
            return exploradorGiros.ObtenerArchivosGiros(numeroGiro);
        }

        /// <summary>
        /// Obtiene la informacíón dependiendo del tipo de giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <param name="tipoGiro">Tipo de giro</param>
        /// <returns>Objeto datos convenio</returns>
        public GIAdmisionGirosDC ObtenerInformacionTipoGiro(long idAdmisionGiro, string tipoGiro)
        {
            return exploradorGiros.ObtenerInformacionTipoGiro(idAdmisionGiro, tipoGiro);
        }

        /// <summary>
        /// Retorna o asigna las solicitudes de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador Admisión giro</param>
        /// <returns>Colección de solicitudes</returns>
        public IEnumerable<GISolicitudGiroDC> ObtenerSolicitudesGiros(long idAdmisionGiro)
        {
            return exploradorGiros.ObtenerSolicitudesGiros(idAdmisionGiro);
        }

        /// <summary>
        /// Retorna el archivo seleccionado de la solicitud
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerArchivoSolicitud(long idSolicitud, GIArchSolicitudDC archivoSeleccionado)
        {
            return exploradorGiros.ObtenerArchivoSolicitud(idSolicitud, archivoSeleccionado);
        }

        /// <summary>
        /// Obtiene la informacion de los intentos a transmitir de un giro
        /// </summary>
        /// <param name="idAdminGiro"></param>
        /// <returns></returns>
        public List<GIIntentosTransmisionGiroDC> ObtenerIntentosTransmitir(long idAdminGiro)
        {
            return exploradorGiros.ObtenerIntentosTransmitir(idAdminGiro);
        }

        /// <summary>
        /// Consulta la informacion basica de una factura por el numero de Operacion( Numero de giro)
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <returns></returns>
        public FAOperacionFacturadaDC ConsultarFacturaPorNumeroOperacion(long numeroOperacion)
        {
            return exploradorGiros.ConsultarFacturaPorNumeroOperacion(numeroOperacion);
        }

        /// <summary>
        /// Obtiene la informacion del almacen
        /// </summary>
        /// <param name="idOperacion">Numero del giro</param>
        /// <returns></returns>
        public List<CCAlmacenDC> ObtenerAlmacenControlCuentas(long idOperacion)
        {
            return exploradorGiros.ObtenerAlmacenControlCuentas(idOperacion);
        }

        /// <summary>
        /// Obtiene los estados de un giro a traves del tiempo
        /// </summary>
        /// <param name="idAdminGiro">Es el idAdmin del Giro</param>
        /// <returns>Lista de estados y Fecha</returns>
        public IList<GIEstadosGirosDC> ObtenerEstadosGiro(long idAdminGiro)
        {
            return exploradorGiros.ObtenerEstadosGiro(idAdminGiro);
        }

        /// <summary>
        /// Obtiene el archivo adjunto Solicitud.
        /// </summary>
        /// <param name="idArchivo">Valor del id del archivo adjunto.</param>
        /// <returns>la direccion en bd del archivo</returns>
        public string ObtenerArchivoAdjuntoSolicitud(long idArchivo)
        {
            return exploradorGiros.ObtenerArchivoAdjuntoSolicitud(idArchivo);
        }

        /// <summary>
        /// Obtiene la informacion de Telemercadeo de
        /// un giro especifico
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns>la info del telemercadeo de un giro</returns>
        public GITelemercadeoGiroDC ObtenerTelemercadeoDeGiro(long idAdmisionGiro)
        {
            return exploradorGiros.ObtenerTelemercadeoDeGiro(idAdmisionGiro);
        }

        /// <summary>
        /// Obtiene reporte para la uiaf de giros
        /// </summary>
        /// <param name="fechaGeneracion">Fecha para generar el reporte</param>
        /// <returns>Lista con los registros a reportar</returns>
        public List<string> ObtenerReporteUIAFGiros(DateTime fechaGeneracion)
        {
            return exploradorGiros.ObtenerReporteUIAFGiros(fechaGeneracion);
        }

        /// <summary>
        /// Obtiene la informacíón de un giro
        /// </summary>
        /// <param name="informacionGiro">Informacíon del giro</param>
        public GIExploradorGirosWebDC ObtenerDatosGiros(GIExploradorGirosWebDC informacionGiro)
        {
            if (informacionGiro == null) {
                throw new ArgumentNullException(nameof(informacionGiro));
            }

            //Conexión Server Financiera
            string hostFinanciera = ServerURIsResources.URI_Financiera_Server.ToString();
            if (String.IsNullOrEmpty(hostFinanciera)) {
                throw new Exception("URI servidor financiera no encontrado.");
            }

            var client = new RestClient(hostFinanciera);
            string uri = "api/giros/{IdTipoDocumento}/{NumeroGiro}";
            var request = new RestRequest(uri, Method.POST);
            request.AddUrlSegment("IdTipoDocumento", DocumentosIdentidad.CEDULA_CIUDADANIA.ToString());
            request.AddUrlSegment("NumeroGiro", informacionGiro.NumeroGiro.ToString());
            RestResponse<AdmisionGiro_GIR> response = (RestSharp.RestResponse<Giros.SharedTypes.BE.AdmisionGiro_GIR>)client.Execute<AdmisionGiro_GIR>(request);
            
            return exploradorGiros.ObtenerDatosGiros(informacionGiro);
        }


        /// <summary>
        /// obtiene los motivos de bloqueo
        /// </summary>
        /// <returns></returns>
        public List<GIMotivosInactivacion> ObtenerMotivosGiros()
        {
            return exploradorGiros.ObtenerMotivosGiros();
        }
        /// <summary>
        /// obtiene todas los usuarios internas
        /// </summary>
        /// <returns></returns>
        public List<Servicios.ContratoDatos.CentroServicios.PUArchivosPersonas> ObtenerTodasPersonasInternas()
        {
            return exploradorGiros.ObtenerTodasPersonasInternas();
        }
        /// <summary>
        /// agrega el motivo de bloqueo
        /// </summary>
        /// <param name="GIMotivo"></param>
        public void AgregarMotivoBloqueo(GIMotivosBloqueoDesbloqueo GIMotivo)
        {
            exploradorGiros.AgregarMotivoBloqueo(GIMotivo);
        }


        #endregion Métodos Públicos

    }
}