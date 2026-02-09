using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using CO.Servidor.Admisiones.Giros.Comun;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.ControlCuentas;
using CO.Servidor.Dominio.Comun.Facturacion;
using CO.Servidor.Dominio.Comun.GestionGiros;
using CO.Servidor.Dominio.Comun.LogisticaInversa;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Telemercadeo;
using CO.Servidor.ExploradorGiros.Datos;
using CO.Servidor.PagosManuales;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Almacen;
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
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;

namespace CO.Servidor.ExploradorGiros
{
    /// <summary>
    /// Clase que contiene el negocio del explorador de giros
    /// </summary>
    internal class GIExploradorGiros : ControllerBase, IGIExploradorGiros
    {
        #region Campos
        /// <summary>
        /// Atributo utilizado para evitar problemas con multithreading en el singleton.
        /// </summary>
        static object syncRoot = new Object();

        static volatile GIExploradorGiros instancia;

        IGIRepositorioExploradorGiros repositorioExplorador = GIRepositorioExploradorGiros.Instancia;

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase
        /// </summary>
        /// 
        public static GIExploradorGiros Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (syncRoot)
                    {
                        if (instancia == null)
                        {
                            instancia = (GIExploradorGiros)FabricaInterceptores.GetProxy(new GIExploradorGiros(), COConstantesModulos.GIROS);
                        }
                    }
                }
                return instancia;
            }
        }

        #endregion Propiedades

        #region Métodos Públicos

        /// <summary>
        /// Obtetener colección giros
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="indicePagina">Indice Página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <returns>Colección giros</returns>
        public IEnumerable<GIAdmisionGirosDC> ObtenerGirosExplorador(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, bool incluyeFecha, out int totalRegistros)
        {
            return repositorioExplorador.ObtenerGirosExplorador(filtro, indicePagina, registrosPorPagina, incluyeFecha, out totalRegistros);
        }

        /// <summary>
        /// Obtiene los tipos de giros
        /// </summary>
        /// <returns>Colección con los tipos de giros</returns>
        public IEnumerable<GITipoGiroDC> ObtenerTiposGiros()
        {
            return repositorioExplorador.ObtenerTiposGiros();
        }

        /// <summary>
        /// Obtiene los valores adicionales de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección valores adicionales</returns>
        public IEnumerable<TAValorAdicional> ObtenerValoresAdicionalesGiro(long idGiro)
        {
            return repositorioExplorador.ObtenerValoresAdicionalesGiro(idGiro);
        }

        /// <summary>
        /// Obtiene los impuestos de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección impuestos giros</returns>
        public IEnumerable<TAImpuestoDelServicio> ObtenerImpuestosGiros(long idGiro)
        {
            return repositorioExplorador.ObtenerImpuestosGiros(idGiro);
        }

        /// <summary>
        /// Obtiene la información del pago
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión</param>
        /// <returns>Objeto pago</returns>
        public PGPagosGirosDC ObtenerInformacionPago(long idAdmisionGiro)
        {
            return repositorioExplorador.ObtenerInformacionPago(idAdmisionGiro);
        }

        /// <summary>
        /// Retorna el comprobante de pago de un giro
        /// </summary>
        /// <param name="numeroGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerArchivosGiros(long numeroGiro)
        {
            AlmacenArchivoPagoGiro_LOI archivoInfo =  repositorioExplorador.ObtenerArchivosGiros(numeroGiro);
            return Convert.ToBase64String(File.ReadAllBytes(archivoInfo.APG_RutaAdjunto));
        }

        /// <summary>
        /// Obtiene el archivo adjunto Solicitud.
        /// </summary>
        /// <param name="idArchivo">Valor del id del archivo adjunto.</param>
        /// <returns>la direccion en bd del archivo</returns>
        public string ObtenerArchivoAdjuntoSolicitud(long idArchivo)
        {
            return repositorioExplorador.ObtenerArchivoAdjuntoSolicitud(idArchivo);
        }

        /// <summary>
        /// Obtiene la informacíón dependiendo del tipo de giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <param name="tipoGiro">Tipo de giro</param>
        /// <returns>Objeto datos convenio</returns>
        public GIAdmisionGirosDC ObtenerInformacionTipoGiro(long idAdmisionGiro, string tipoGiro)
        {
            GIAdmisionGirosDC giro = new GIAdmisionGirosDC();
            GIGirosPeatonPeatonDC peatonPeaton = null;
            GIGirosPeatonConvenioDC peatonConvenio = null;

            if (tipoGiro == GIConstantesAdmisionesGiros.GIROPEATONAPEATON)
            {
                peatonPeaton = repositorioExplorador.ObtenerConvenioPeatonPeaton(idAdmisionGiro);
            }
            else
            {
                peatonConvenio = repositorioExplorador.ConsultarInformacionPeatonConvenio(idAdmisionGiro);
            }

            giro.GirosPeatonPeaton = peatonPeaton;
            giro.GirosPeatonConvenio = peatonConvenio;

            return giro;
        }

        /// <summary>
        /// Retorna o asigna las solicitudes de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador Admisión giro</param>
        /// <returns>Colección de solicitudes</returns>
        public IEnumerable<GISolicitudGiroDC> ObtenerSolicitudesGiros(long idAdmisionGiro)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IGIFachadaGestionGiros>().ObtenerSolicitudesGiros(idAdmisionGiro);
        }

        /// <summary>
        /// Retorna el archivo seleccionado de la solicitud
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerArchivoSolicitud(long idSolicitud, GIArchSolicitudDC archivoSeleccionado)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IGIFachadaGestionGiros>().ObtenerArchivoSolicitud(idSolicitud, archivoSeleccionado);
        }

        /// <summary>
        /// Obtiene la informacion de los intentos a transmitir de un giro
        /// </summary>
        /// <param name="idAdminGiro"></param>
        /// <returns></returns>
        public List<GIIntentosTransmisionGiroDC> ObtenerIntentosTransmitir(long idAdminGiro)
        {
            return GIPagosManuales.Instancia.ObtenerIntentosTransmitir(idAdminGiro);
        }

        /// <summary>
        /// Consulta la informacion basica de una factura por el numero de Operacion( Numero de giro)
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <returns></returns>
        public FAOperacionFacturadaDC ConsultarFacturaPorNumeroOperacion(long numeroOperacion)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IFAFachadaFacturacion>().ConsultarFacturaPorNumeroOperacion(numeroOperacion);
        }

        /// <summary>
        /// Obtiene la informacion del almacen
        /// </summary>
        /// <param name="idOperacion">Numero del giro</param>
        /// <returns></returns>
        public List<CCAlmacenDC> ObtenerAlmacenControlCuentas(long idOperacion)
        {
            List<CCAlmacenDC> infoControlCuentas = new List<CCAlmacenDC>();

            ICCFachadaControlCuentas fachadaControlCuentas = COFabricaDominio.Instancia.CrearInstancia<ICCFachadaControlCuentas>();
            IADFachadaAdmisionesGiros fachadaAdmisionGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();

            CCAlmacenDC infoAlmacenCtroCuentas = fachadaControlCuentas.ObtenerAlmacenControlCuentasGiros(idOperacion);

            if (infoAlmacenCtroCuentas != null)
                infoControlCuentas.Add(fachadaControlCuentas.ObtenerAlmacenControlCuentasGiros(idOperacion));

            //obtengo el idAdmision para consultar el numero del Comprobante de pago
            long? idAdmisionGiro = fachadaAdmisionGiros.ObtenerIdentificadorAdmisionGiro(idOperacion);

            CCAlmacenDC infoAlmacenCtroCuentasComprobante = fachadaControlCuentas.ObtenerAlmacenControlCuentasGiros(fachadaAdmisionGiros.ObtenerIdentificadorPagosGiro(idAdmisionGiro.Value));

            if (infoAlmacenCtroCuentasComprobante != null)
                infoControlCuentas.Add(infoAlmacenCtroCuentasComprobante);

            return infoControlCuentas;
        }

        /// <summary>
        /// Obtiene los estados de un giro a traves del tiempo
        /// </summary>
        /// <param name="idAdminGiro">Es el idAdmin del Giro</param>
        /// <returns>Lista de estados y Fecha</returns>
        public IList<GIEstadosGirosDC> ObtenerEstadosGiro(long numeroGiro)
        {
            IList<GIEstadosGirosDC> lstHistoricoEstadosGiro = repositorioExplorador.ObtenerEstadosGiro(numeroGiro);
            return lstHistoricoEstadosGiro;
        }

        /// <summary>
        /// Obtiene la informacion de Telemercadeo de
        /// un giro especifico
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns>la info del telemercadeo de un giro</returns>
        public GITelemercadeoGiroDC ObtenerTelemercadeoDeGiro(long idAdmisionGiro)
        {
            IGIFachadaTelemercadeo fachadaTelemercadeo = COFabricaDominio.Instancia.CrearInstancia<IGIFachadaTelemercadeo>();

            return fachadaTelemercadeo.ObtenerTelemercadeoDeGiro(idAdmisionGiro);
        }

        /// <summary>
        /// Obtiene reporte para la uiaf de giros
        /// </summary>
        /// <param name="fechaGeneracion">Fecha para generar el reporte</param>
        /// <returns>Lista con los registros a reportar</returns>
        public List<string> ObtenerReporteUIAFGiros(DateTime fechaGeneracion)
        {
            return repositorioExplorador.ObtenerReporteUIAFGiros(fechaGeneracion);
        }



        /// <summary>
        /// Obtiene la informacíón básica de un giro
        /// </summary>
        /// <param name="informacionGiro">Información del giro</param>
        /// <returns>Objeto datos convenio</returns>
        public GIExploradorGirosWebDC ObtenerDatosGiros(GIExploradorGirosWebDC informacionGiro)
        {

            return repositorioExplorador.ObtenerDatosGiros(informacionGiro);
        }

        /// <summary>
        /// obtiene los motivos de bloqueo
        /// </summary>
        /// <returns></returns>
        public List<GIMotivosInactivacion> ObtenerMotivosGiros()
        {
            return repositorioExplorador.ObtenerMotivosGiros();
        }
        /// <summary>
        /// obtiene todas los usuarios internas
        /// </summary>
        /// <returns></returns>
        public List<Servicios.ContratoDatos.CentroServicios.PUArchivosPersonas> ObtenerTodasPersonasInternas()
        {
            return repositorioExplorador.ObtenerTodasPersonasInternas();
        }
        /// <summary>
        /// agrega el motivo de bloqueo
        /// </summary>
        /// <param name="GIMotivo"></param>
        public void AgregarMotivoBloqueo(GIMotivosBloqueoDesbloqueo GIMotivo)
        {

            long idAdminGiro = 0;
            IADFachadaAdmisionesGiros fachadaAdmisionGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();
            idAdminGiro = fachadaAdmisionGiros.ObtenerIdentificadorAdmisionGiro(GIMotivo.IdAdmisionGiroIdent).Value;
            GIMotivo.IdAdmisionGiro = idAdminGiro;
            repositorioExplorador.AgregarMotivoBloqueo(GIMotivo);
        }

        #endregion Métodos Públicos

    }
}