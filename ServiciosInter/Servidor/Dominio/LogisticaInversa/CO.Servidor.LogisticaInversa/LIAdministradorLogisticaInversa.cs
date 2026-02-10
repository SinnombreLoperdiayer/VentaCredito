using System.Collections.Generic;
using System.ServiceModel;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.DigitalizacionArchivo;
using CO.Servidor.LogisticaInversa.Notificaciones;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Transactions;
using CO.Servidor.LogisticaInversa.Datos;

namespace CO.Servidor.LogisticaInversa
{
    /// <summary>
    /// Clase fachada administrador para acceso a los controles visuales del modulo
    /// </summary>
    public class LIAdministradorLogisticaInversa
    {
        private static readonly LIAdministradorLogisticaInversa instancia = new LIAdministradorLogisticaInversa();

        public static LIAdministradorLogisticaInversa Instancia
        {
            get { return LIAdministradorLogisticaInversa.instancia; }
        }

        /// <summary>
        /// Válida si existe el giro y si tiene un archivo
        /// </summary>
        /// <param name="imagenes">Imágenes</param>
        /// <returns>Imágenes</returns>
        public List<LIArchivoGiroDC> ExisteGiroArchivo(List<LIArchivoGiroDC> imagenes)
        {
            return LIDigitalizacionArchivo.Instancia.ExisteGiroArchivo(imagenes);
        }

        /// <summary>
        /// Adiciona un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public LIArchivoGiroDC AdicionarArchivoGiro(LIArchivoGiroDC imagen)
        {
            return LIDigitalizacionArchivo.Instancia.AdicionarArchivoGiro(imagen);
        }

        /// <summary>
        /// Editar un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoGiro(LIArchivoGiroDC imagen)
        {
            LIDigitalizacionArchivo.Instancia.EditarArchivoGiro(imagen);
        }

        /// <summary>
        /// Adicionar archivos de giro
        /// </summary>
        /// <param name="imagenes">Objeto imagenes</param>
        public List<LIArchivoGiroDC> AdicionarArchivosGiro(List<LIArchivoGiroDC> imagenes)
        {
            return LIDigitalizacionArchivo.Instancia.AdicionarArchivosGiro(imagenes);
        }

        /// <summary>
        /// Valida si se puede digitalizar la guía de acuerdo al estado
        /// </summary>
        /// <param name="imagenes">Colección de imágenes</param>
        public void GuardarGuiasCorrectas(List<LIArchivoGuiaMensajeriaDC> imagenes, bool entregaExitosa)
        {
            LIDigitalizacionArchivo.Instancia.GuardarGuiasCorrectas(imagenes, entregaExitosa);
        }

        /// <summary>
        /// Adiciona un archivo guía mensajería
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagen"></param>
        public void AdicionarArchivoGuiaMensajeria(bool entregaExitosa, LIArchivoGuiaMensajeriaDC imagen)
        {
            LIDigitalizacionArchivo.Instancia.AdicionarArchivoGuiaMensajeria(entregaExitosa, imagen);
        }

        /// <summary>
        /// Edita un archivo guía de mensajería
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoGuiaMensajeria(LIArchivoGuiaMensajeriaDC imagen)
        {
            LIDigitalizacionArchivo.Instancia.EditarArchivoGuiaMensajeria(imagen);
        }

        /// <summary>
        /// Válida si existe el giro y si tiene un archivo
        /// </summary>
        /// <param name="imagenes">Imágenes</param>
        /// <returns>Imágenes</returns>
        public List<LIArchivoComprobantePagoDC> ExisteComprobanteGiroArchivo(List<LIArchivoComprobantePagoDC> imagenes)
        {
            return LIDigitalizacionArchivo.Instancia.ExisteComprobanteGiroArchivo(imagenes);
        }

        /// <summary>
        /// Adiciona un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public LIArchivoComprobantePagoDC AdicionarComprobanteArchivoGiro(LIArchivoComprobantePagoDC imagen)
        {
            return LIDigitalizacionArchivo.Instancia.AdicionarComprobanteArchivoGiro(imagen);
        }

        /// <summary>
        /// Editar un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarComprobanteArchivoGiro(LIArchivoComprobantePagoDC imagen)
        {
            LIDigitalizacionArchivo.Instancia.EditarComprobanteArchivoGiro(imagen);
        }

        /// <summary>
        /// Adicionar comprobantes de pago de gito
        /// </summary>
        /// <param name="imagenes">Objeto imagenes</param>
        public List<LIArchivoComprobantePagoDC> AdicionarComprobanteArchivosGiro(List<LIArchivoComprobantePagoDC> imagenes)
        {
            return LIDigitalizacionArchivo.Instancia.AdicionarComprobanteArchivosGiro(imagenes);
        }

        /// <summary>
        /// Archiva una guía
        /// </summary>
        /// <param name="guia">Objeto guía</param>
        public LIArchivoGuiaMensajeriaDC GuardarArchivoGuiaMensajeria(LIArchivoGuiaMensajeriaDC guia)
        {
            return LIDigitalizacionArchivo.Instancia.GuardarArchivoGuiaMensajeria(guia);
        }

        /// <summary>
        /// Método para obtener un archivo guia
        /// </summary>
        /// <param name="imagen"></param>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaFS(LIArchivoGuiaMensajeriaDC archivoGuia)
        {
            return LIDigitalizacionArchivo.Instancia.ObtenerArchivoGuiaFS(archivoGuia);
        }

        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaSispostal(LIArchivoGuiaMensajeriaDC archivoGuia)
        {
            return LIDigitalizacionArchivo.Instancia.ObtenerArchivoGuiaSispostal(archivoGuia);
        }

        /// <summary>
        /// Método para obtener imagen de la Fachada de una guia
        /// </summary>
        /// <param name="imagen"></param>
        public List<LIArchivoGuiaMensajeriaFachadaDC> ObtenerArchivoGuiaFachadaFS(long numeroGuia)
        {
            return LIDigitalizacionArchivo.Instancia.ObtenerArchivoGuiaFachadaFS(numeroGuia);
        }

       
        /// <summary>
        /// Obtiene las guías archivadas
        /// </summary>
        /// <returns>Colección guías archivadas</returns>
        public List<LIArchivoGuiaMensajeriaDC> ObtenerArchivoGuia(long idCol)
        {
            return LIDigitalizacionArchivo.Instancia.ObtenerArchivoGuia(idCol);
        }

        /// <summary>
        /// Retorna el archivo digitalizado de una guía
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADArchivoAlmacenGuia ObtenerArchivoAlmacenGuia(long numeroGuia)
        {
            ADArchivoAlmacenGuia guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerArchivoAlmacenGuia(numeroGuia);
            if (guia != null)
            {
                if (!LINotificaciones.Instancia.RecibidoRegistrado(guia.Guia.NumeroGuia))
                {
                    OUNombresMensajeroDC mensajero = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().ConsultarUltimoMensajeroGuia(guia.Guia.IdAdmision);
                    if (mensajero != null)
                    {
                        long idMensajero = 0;
                        if (long.TryParse(mensajero.Identificacion, out idMensajero))
                        {
                            guia.IdMensajero = idMensajero;
                        }
                        guia.NombreMensajero = mensajero.NombreApellido;
                    }
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.NOTIFICACIONES, LOIEnumTipoErrorLogisticaInversa.EX_ERROR_DATOS_RECIBIDO_YA_CAPTURADOS.ToString(), string.Format(LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_DATOS_RECIBIDO_YA_CAPTURADOS), numeroGuia));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
            return guia;
        }

        /// <summary>
        /// Retorna la información de una guía dada su forma de pago, en un rango de fechas de admisión, que pertenezcan al cliente dado y al RACOL dado, que 
        /// sean del servicio de notificaciones, tipo de envío certificación, que estén descargadas como entrega correcta, que no tengan capturado los datos de 
        /// recibido y estén digitalizadas 
        /// </summary>
        /// <param name="idFormaPago">Forma de pago</param>
        /// <param name="fechaInicio">Fecha Inicial</param>
        /// <param name="fechaFin">Fecha Final</param>
        /// <param name="idCliente">Id del Cliente</param>
        /// <param name="idRacol">Id del Racol</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasParaCapturaAutomatica(short idFormaPago, System.DateTime fechaInicio, System.DateTime fechaFin, int? idCliente, long idRacol)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiasParaCapturaAutomatica(idFormaPago, fechaInicio, fechaFin, idCliente, idRacol);
        }

        /// <summary>
        /// Retorna información de guía que para ser recibida en forma manual en la parte de notificaciones.
        /// La guía debe estar en estado "Devolución" o "Entrega" y la prueba de entrega o de devolución
        /// correspondiente debe estar digitalizada en la aplicación
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaParaRecibirManualNotificaciones(long numeroGuia)
        {
            IADFachadaAdmisionesMensajeria fachadaAdmisiones = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
            IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
            ADGuia guia = fachadaAdmisiones.ObtenerGuiaParaRecibirManualNotificaciones(numeroGuia);
            if (guia != null)
            {
                if (!LINotificaciones.Instancia.RecibidoRegistrado(guia.NumeroGuia))
                {
                    OUNombresMensajeroDC mensajero = fachadaOperacionUrbana.ConsultarUltimoMensajeroGuia(guia.IdAdmision);
                    if (mensajero != null)
                    {
                        long idMensajero = 0;
                        if (long.TryParse(mensajero.Identificacion, out idMensajero))
                        {
                            guia.IdMensajero = idMensajero;
                        }
                        guia.NombreMensajero = mensajero.NombreApellido;
                    }
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.NOTIFICACIONES, LOIEnumTipoErrorLogisticaInversa.EX_ERROR_DATOS_RECIBIDO_YA_CAPTURADOS.ToString(), string.Format(LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_DATOS_RECIBIDO_YA_CAPTURADOS), numeroGuia));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
            return guia;
        }

        /// <summary>
        /// Registra recibido de guía manual
        /// </summary>
        /// <param name="recibido"></param>
        public void RegistrarRecibidoGuiaManual(LIRecibidoGuia recibido)
        {
            LINotificaciones.Instancia.RegistrarRecibidoGuiaManual(recibido);
        }

        /// <summary>
        /// Método para validar archivos de guías, para clasificarlas de acuerdo a su posible resultado
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagenes"></param>
        /// <returns></returns>
        public List<LIArchivoGuiaMensajeriaDC> ValidarArchivosGuias(bool entregaExitosa, List<LIArchivoGuiaMensajeriaDC> imagenes)
        {
            return LIDigitalizacionArchivo.Instancia.ValidarArchivosGuias(entregaExitosa, imagenes);
        }

        /// <summary>
        /// Método para validar la digitalización de los volantes de devolución
        /// </summary>
        /// <param name="imagen"></param>
        /// <returns></returns>
        public List<LIEvidenciaDevolucionDC> ValidarArchivosVolantes(List<LIEvidenciaDevolucionDC> imagenes)
        {
            return LIDigitalizacionArchivo.Instancia.ValidarArchivosVolantes(imagenes);
        }

        public List<LIEvidenciaDevolucionDC> ValidarArchivosVolantesWPF(List<LIEvidenciaDevolucionDC> imagenes)
        {
            return LIDigitalizacionArchivo.Instancia.ValidarArchivosVolantesWPF(imagenes);
        }


        public LIEvidenciaDevolucionDC AsociarNumeroGuiaAVolante(LIEvidenciaDevolucionDC imagenVolante)
        {
            return LIDigitalizacionArchivo.Instancia.AsociarNumeroGuiaAVolante(imagenVolante);
        }

        public List<LIEvidenciaDevolucionDC> ObtenerEvidenciaDevolucionxGuia(long NumeroGuia)
        {
            return LIDigitalizacionArchivo.Instancia.ObtenerEvidenciaDevolucionxGuia(NumeroGuia);
        }


        /// <summary>
        ///Guarda un archivo de un volante
        /// </summary>
        /// <param name="imagenes">Colección de imágenes</param>
        public void GuardarVolantesCorrectos(List<LIEvidenciaDevolucionDC> imagenes)
        {
            LIDigitalizacionArchivo.Instancia.GuardarVolantesCorrectos(imagenes);
        }

        /// <summary>
        /// Retorna las guias internas de la planilla, o de las certificaciones de la planilla
        /// </summary>
        /// <param name="planillas"></param>
        /// <param name="EsPlanilla"></param>
        /// <returns></returns>
        public List<ADGuiaInternaDC> ObtenerGuiasInternasPlanilla(string planillas, bool EsPlanilla, long idCol)
        {
            return LINotificaciones.Instancia.ObtenerGuiasInternasImpresionPlanillas(planillas, EsPlanilla, idCol);
        }

        /// <summary>
        /// Edita un archivo de un volante
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoVolante(LIArchivosDC imagen)
        {
            LIDigitalizacionArchivo.Instancia.EditarArchivoVolante(imagen);
        }

        /// <summary>
        /// Método para obtener las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ADNotificacion> ObtenerNotificacionesRecibido(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return LINotificaciones.Instancia.ObtenerNotificacionesRecibido(filtro, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Método para generar las guías internas de las notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public int GenerarGuiasInternasNotificacion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return LINotificaciones.Instancia.GenerarGuiasInternasNotificacion(filtro, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Método para obtener los id de las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<long> ObtenerIdNotificaciones(IDictionary<string, string> filtro)
        {
            return LINotificaciones.Instancia.ObtenerIdNotificaciones(filtro);
        }



        #region Planilla Certificacion

        /// <summary>
        /// Obtener planillas de certificacion
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<LIPlanillaCertificacionesDC> ObtenerPlanillasCertificaciones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, bool esDevolucion, out int totalRegistros)
        {
            return LOINotificaciones.Instancia.ObtenerPlanillasCertificaciones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, esDevolucion, out totalRegistros);
        }

        /// <summary>
        /// Obtener planillas de certificacion con ADO
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="esDevolucion"></param>
        /// <param name="totalPaginas"></param>
        /// <returns></returns>
        public IEnumerable<LIPlanillaCertificacionesDC> ObtenerPlanillasCertificacionesAdo(IDictionary<String, String> filtro, String campoOrdenamiento, Int32 indicePagina, Int32 registrosPorPagina, Boolean ordenamientoAscendente, Boolean esDevolucion, out Int32 totalPaginas)
        {
            return LOINotificaciones.Instancia.ObtenerPlanillasCertificacionesAdo(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, esDevolucion, out totalPaginas);
        }


        /// <summary>
        /// Método para obtener las guias de una planilla de certificacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<ADNotificacion> ObtenerGuiasPlanillaCertificacion(long idPlanilla)
        {
            return LOINotificaciones.Instancia.ObtenerGuiasPlanillaCertificacion(idPlanilla);
        }


        public void GuardarGuiaPlanilla() { }

        /// <summary>
        /// Guarda los envios de la planilla de asignacion
        /// </summary>
        /// <param name="planilla"></param>
        public void GuardarLstEnvioPlanillaCertificacion(LIPlanillaCertificacionesDC planilla)
        {
            LINotificaciones.Instancia.GuardarLstEnvioPlanillaCertificacion(planilla);
        }

        /// <summary>
        /// Guarda guia en la planilla de certificacion
        /// </summary>
        /// <param name="planilla"></param>
        public LIPlanillaCertificacionesDC GuardarGuiaPlanillaCertificacion(LIPlanillaCertificacionesDC planilla)
        {
            return LINotificaciones.Instancia.GuardarGuiaPlanillaCertificacion(planilla);
        }

        /// <summary>
        /// Elimina una guia de la planilla de certificaciones
        /// </summary>
        /// <param name="guiaCertificacion"></param>
        /// <returns></returns>
        public ADNotificacion EliminarGuiaPlanillaCertificaciones(ADNotificacion guiaCertificacion)
        {
            return LINotificaciones.Instancia.EliminarGuiaPlanillaCertificaciones(guiaCertificacion);
        }

        /// <summary>
        /// Valida que la guia ingresada cumpla con las condiciones para la certificacion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADNotificacion ValidarGuiaCertificacionNotificacion(long numeroGuia)
        {
            return LINotificaciones.Instancia.ValidarGuiaCertificacionNotificacion(numeroGuia);
        }

        /// <summary>
        /// Obtiene las notificaciones del col, con los filtros seleccionados que esten sin planillar
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="idCol"></param>
        public List<ADNotificacion> ObtenerNotificacionesFiltroSinPla(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCol)
        {
            return LINotificaciones.Instancia.ObtenerNotificacionesFiltroSinPla(filtro, indicePagina, registrosPorPagina, idCol);
        }

        /// <summary>
        /// Descarga la planilla de certificacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void DescargarPlanillaCertificaciones(long idPlanilla)
        {
            LINotificaciones.Instancia.DescargarPlanillaCertificaciones(idPlanilla);
        }

        /// <summary>
        /// Método que adiciona una planilla de certificación
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public LIPlanillaCertificacionesDC AdicionarPlanilla(LIPlanillaCertificacionesDC planilla)
        {
            return LOINotificaciones.Instancia.AdicionarPlanilla(planilla);
        }


        /// <summary>
        /// Método para adicionar una guía a una planilla
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public ADNotificacion AdicionarGuiaPlanilla(ADNotificacion guia)
        {
            return LOINotificaciones.Instancia.AdicionarGuiaPlanilla(guia);
        }


        /// <summary>
        /// Método para cerrar una planilla de notificaciones
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        public void CerrarPlanillaNotificaciones(long numeroPlanilla)
        {
            LOINotificaciones.Instancia.CerrarPlanillaNotificaciones(numeroPlanilla);
        }


        /// <summary>
        /// Método para generar las guias de la planilla de entrega
        /// </summary>
        /// <param name="planilla"></param>
        public void AdicionarGuiasPlanillaEntrega(LIPlanillaCertificacionesDC planilla)
        {
            LOINotificaciones.Instancia.AdicionarGuiasPlanillaEntrega(planilla);
        }

        #endregion Planilla Certificacion


        #region GeneracionImagenes
        /// <summary>
        /// Obtiene los numeros de guias y la ruta
        /// </summary>
        /// <returns>Colección numeros de guia y rutas</returns>
        public List<LIArchivoGuiaMensajeriaDC> ObtenerGuiasRuta(string imagenesGenerar, int idCliente, string idCiudad, int idSucursal, DateTime fechaAdmisionInical, DateTime fechaAdmisionFinal, long guiaFacturaInicial, long guiaFacturaFinal, long ordenCompraInicial, long ordenCompraFinal)
        {
            return LIDigitalizacionArchivo.Instancia.ObtenerGuiasRuta(imagenesGenerar, idCliente, idCiudad, idSucursal, fechaAdmisionInical, fechaAdmisionFinal, guiaFacturaInicial, guiaFacturaFinal, ordenCompraInicial, ordenCompraFinal);
        }

        #endregion


        /// <summary>
        /// Metodo que obtiene los datos de archivo guía de mensajería    
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaxNumeroGuia(long numeroGuia)
        {
            return LIRepositorioPruebasEntrega.Instancia.ObtenerArchivoGuiaxNumeroGuia(numeroGuia);
        }


        /// <summary>
        /// Metodo que obtiene los valantes de una guía de mensajería    
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<string> ObtenerVolantesGuia(long numeroGuia)
        {
            return LIRepositorioDigitalizacion.Instancia.ObtenerVolantesGuia(numeroGuia);
        }


        public ADGuia ObtenerFechaEstimadaEntregaGuia(long numeroguia)
        {
            return LIDigitalizacionArchivo.Instancia.ObtenerFechaEstimadaEntregaGuia(numeroguia);
        }


        /// <summary>
        /// Metodo que obtiene información de Temelercadeo de la Guia seleccionada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIGestionesDC> ObtenerInformacionTelemercadeoGuia(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerInformacionTelemercadeoGuia(numeroGuia);
        }

        /// <summary>
        /// Metodo para obtener imagen fachada descargue app     
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<string> ObtenerImagenFachadaApp(long numeroGuia)
        {
            return LIRepositorioPruebasEntrega.Instancia.ObtenerImagenFachadaApp(numeroGuia);
        }


        #region Digitalizacion de agencias
        /// <summary>
        /// Método para validar archivos de guías, para clasificarlas de acuerdo a su posible resultado
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagenes"></param>
        /// <returns></returns>
        public List<LIArchivoGuiaMensajeriaDC> ValidarArchivosAgencias( List<LIArchivoGuiaMensajeriaDC> imagenes)
        {
            return LIDigitalizacionArchivo.Instancia.ValidarArchivosAgencias(imagenes);
        }

        #endregion

        #region Auditoria devolución Controller App

        /// <summary>
        /// Metodo para insertar la auditoria de devolucion de controller app
        /// </summary>
        /// <param name="liGestionAuditorDC"></param>
        /// <returns></returns>
        public bool InsertarAuditoriaDevolucion(LIGestionAuditorDC liGestionAuditorDC)
        {
            return LIRepositorioPruebasEntrega.Instancia.InsertarAuditoriaDevolucion(liGestionAuditorDC);
        }

        public bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega)
        {
            return LIRepositorioPruebasEntrega.Instancia.InsertarLecturaEcaptureArchivoPruebaEntrega(archivoPruebaEntrega);
        }

        internal bool ValidarRecepcionHistoricoEcapture(long numeroGuia, string codigoProceso)
        {
            return LIRepositorioPruebasEntrega.Instancia.ValidarRecepcionHistoricoEcapture(numeroGuia, codigoProceso);
        }

        public InformacionECAPTURE ValidarInformacionECapture(ADGuia guia)
        {
            ////TO DO: Realizar el consumo del servicio de ECAPTURE
            var informacionECAPTURE = new InformacionECAPTURE();

            return informacionECAPTURE;
        }

        public LIArchivoGuiaMensajeriaDC ArchivarGuiaPruebaEntregaWPF(LIArchivoGuiaMensajeriaDC guia)
        {            
            IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

            guia.IdCol = fachadaCentroServicio.ObteneColPropietarioBodega(guia.IdCol).IdResponsable;

            guia.IdCentroLogistico = guia.IdCol;
            guia.EstadoDatosEdicion.IdEstadoDato = LOIConstantesLogisticaInversa.ID_ESTADO_LEGIBLE;
            guia.EstadoDatosEntrega.IdEstadoDato = LOIConstantesLogisticaInversa.ID_ESTADO_LEGIBLE;
            guia.EstadoFisicoGuia.IdEstadoFisico = LOIConstantesLogisticaInversa.ID_ESTADO_BUENO;
            guia.CreadoPor = ControllerContext.Current.Usuario;
            guia.UsuarioArchivo = ControllerContext.Current.Usuario;
            guia.NumeroGuia = guia.NumeroGuia;            
                        
            if (guia.TipoArchivoPruebaEntrega == EnumTipoArchivo.FORMATO_GUIA_MENSAJERIA)
            {
                LIRepositorioDigitalizacionArchivo.Instancia.ObtenerCajaLotePosicion(guia);
                ArchivarPruebaEntregaGuiaMensajeria(ref guia);
            }
            else
            {   
                ArchivarPruebaIntentoEntrega(ref guia);
            }
            
            return guia;            
        }

        private void ArchivarPruebaIntentoEntrega(ref LIArchivoGuiaMensajeriaDC guia)
        {
            var g = LIRepositorioDigitalizacion.Instancia.ObtenerArchivoGuiaPruebaEntrega(guia);

            if (g == null)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, "0", "La evidencia no se pudo archivar porque la imagen no está sincronizada"));
            }

            var existeArchivoIntentoEntrega = LIRepositorioDigitalizacion.Instancia.ValidarArchivoIntentoEntregaExistente(guia);

            if (existeArchivoIntentoEntrega)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, "0", "La evidencia ingresada ya se encuentra archivada"));
            }

            lock (this)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {                                        
                    AsignacionCajasIntentoEntrega(ref guia);
                    LIRepositorioDigitalizacionArchivo.Instancia.ObtenerCajaLotePosicionIntentoEntrega(ref guia);
                    LIRepositorioDigitalizacion.Instancia.ArchivarIntentoEntrega(guia);                    
                    transaccion.Complete();
                }
            }

        }

        private void AsignacionCajasIntentoEntrega(ref LIArchivoGuiaMensajeriaDC guia)
        {
            PAConsecutivoDC consecutivo = PAAdministrador.Instancia.ObtenerDatosConsecutivoIntentoEntregaxCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
            
            if (consecutivo == null)
            {
                consecutivo = CrearRangoConsecutivoIntentoEntrega(ref guia);
                consecutivo.Actual = PAAdministrador.Instancia.ObtenerConsecutivoIntentoEntregaPorCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);                
            }            

            if (consecutivo.Actual < guia.Caja)
            {
                long nuevoConsec = 0;

                while (guia.Caja > nuevoConsec)
                {
                    nuevoConsec = PAAdministrador.Instancia.ObtenerConsecutivoIntentoEntregaPorCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
                }
            }

            if (guia.CajaLlena)
            {
                PAConsecutivoDC consecutivoActual = PAAdministrador.Instancia.ObtenerDatosConsecutivoIntentoEntregaxCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
                if (consecutivoActual == null)
                {
                    consecutivoActual = CrearRangoConsecutivoIntentoEntrega(ref guia);
                }

                long idConsecutivoNuevo = PAAdministrador.Instancia.ObtenerConsecutivoIntentoEntregaPorCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
                if (idConsecutivoNuevo > consecutivoActual.Actual + 1)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, "0", "Se detectó un error de concurrencia al actualizar el número de caja. Intente nuevamente la operación."));
                }
                guia.Caja = idConsecutivoNuevo;
            }
        }

        private PAConsecutivoDC CrearRangoConsecutivoIntentoEntrega(ref LIArchivoGuiaMensajeriaDC guia)
        {
            return PAAdministrador.Instancia.CrearRangoConsecutivoIntentoEntrega(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCentroLogistico, guia.CreadoPor);
        }

        private void ArchivarPruebaEntregaGuiaMensajeria(ref LIArchivoGuiaMensajeriaDC guia)
        {
            IADFachadaAdmisionesMensajeria fachadaAdmisiones = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

            var g = LIRepositorioDigitalizacion.Instancia.ObtenerArchivoGuiaFS(guia);

            if (g == null)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, "0", "La guía no se pudo archivar porque la imagen no esta sincronizada"));
            }

            ADGuia guiaMensajeria;

            guiaMensajeria = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia);
            guia.IdAdmisionMensajeria = guiaMensajeria.IdAdmision;
            
            
            lock (this)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {

                    GenerarCambioEstado(ref guia);
                    AsignacionCajas(ref guia);
                    LIRepositorioDigitalizacion.Instancia.ArchivarGuia(guia);
                    EnviarMensajeTexto(guiaMensajeria);
                    transaccion.Complete();
                }
            }
        }

        private void AsignacionCajas(ref LIArchivoGuiaMensajeriaDC guia)
        {
            PAConsecutivoDC consecutivo = PAAdministrador.Instancia.ObtenerDatosConsecutivoxCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);

            if (consecutivo.Actual < guia.Caja)
            {
                long nuevoConsec = 0;

                while (guia.Caja > nuevoConsec)
                {
                    nuevoConsec = PAAdministrador.Instancia.ObtenerConsecutivoPorCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
                }
            }

            if (guia.CajaLlena)
            {
                PAConsecutivoDC consecutivoActual = PAAdministrador.Instancia.ObtenerDatosConsecutivoxCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
                long idConsecutivoNuevo = PAAdministrador.Instancia.ObtenerConsecutivoPorCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
                if (idConsecutivoNuevo > consecutivoActual.Actual + 1)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, "0", "Se detectó un error de concurrencia al actualizar el número de caja. Intente nuevamente la operación."));
                }
                guia.Caja = idConsecutivoNuevo;
            }
        }

        private void GenerarCambioEstado(ref LIArchivoGuiaMensajeriaDC guia)
        {
            ADEnumEstadoGuia ultimoEstado = EstadosGuia.ObtenerUltimoEstadoxNumero(guia.NumeroGuia);

            ADTrazaGuia EstadoGuia = new ADTrazaGuia()
            {
                Ciudad = guia.NombreCiudad,
                DescripcionEstadoGuia = ultimoEstado.ToString(),
                FechaGrabacion = DateTime.Now,
                IdAdmision = guia.IdAdmisionMensajeria,
                IdCiudad = guia.IdCiudad,
                IdEstadoGuia = (short)ultimoEstado,
                IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Archivada,
                Modulo = COConstantesModulos.DIGITALIZACION_Y_ARCHIVO,
                NumeroGuia = long.Parse(guia.ValorDecodificado),
                Observaciones = string.Empty,
                Usuario = ControllerContext.Current.Usuario
            };

            if (EstadosGuia.ValidarInsertarEstadoGuia(EstadoGuia) == 0)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, "0", "La guía se encuentra en estado " + ultimoEstado.ToString() + " y no puede ser pasada a estado " + ADEnumEstadoGuia.Archivada.ToString() + "."));
            }
        }

        private void EnviarMensajeTexto(ADGuia guia)
        {
            if (guia.IdServicio == (short)EnumTipoServicio.Notificaciones)
            {
                new Task(() => Controller.Servidor.Integraciones.MensajesTexto.Instancia.EnviarMensajeNoCliente(
                                        guia.IdAdmision,
                                        (long)guia.NumeroGuia,
                                        Controller.Servidor.Integraciones.EnumMensajeNocliente.CertiJudicial.ToString(),
                                        guia.Remitente.Telefono,
                                        guia.ValorTotal)
                                    ).Start();
            }
        }

        /// <summary>
        /// Obtiene el identificador del estado de los datos de la guía
        /// </summary>
        /// <param name="estadoGuia">Estado guia</param>
        /// <returns>Identificador estado</returns>
        public string ObtenerIdentificadorEstadoDatosGuia(string estadoGuia)
        {
            if (estadoGuia == LIEnumEstadoDatosGuia.ILEGIBLE.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_ILEGIBLE;
            else if (estadoGuia == LIEnumEstadoDatosGuia.LEGIBLE.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_LEGIBLE;
            else if (estadoGuia == LIEnumEstadoDatosGuia.INCOMPLETA.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_INCOMPLETO;
            else
                return LOIConstantesLogisticaInversa.ID_ESTADO_NO_CLASIFICADO;
        }

        /// <summary>
        /// Obtiene el identificador del estado físico de la guía
        /// </summary>
        /// <param name="estadoGuia">Estado guia</param>
        /// <returns>Identificador estado</returns>
        public string ObtenerIdentificadorEstadoFisicoGuia(string estadoFisico)
        {
            if (estadoFisico == LIEnumEstadoFisicoGuia.BUENO.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_BUENO;
            else if (estadoFisico == LIEnumEstadoFisicoGuia.MALO.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_MALO;
            else if (estadoFisico == LIEnumEstadoFisicoGuia.REGULAR.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_REGULAR;
            else
                return LOIConstantesLogisticaInversa.ID_ESTADO_NO_CLASIFICADO;
        }

        #endregion

    }
}   