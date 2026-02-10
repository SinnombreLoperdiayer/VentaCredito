using CO.Servidor.LogisticaInversa;
using CO.Servidor.LogisticaInversa.Agencias;
using CO.Servidor.LogisticaInversa.EntregaAgencias.DescargueAgencias;
using CO.Servidor.LogisticaInversa.Notificaciones;
using CO.Servidor.LogisticaInversa.PruebasEntrega;
using CO.Servidor.LogisticaInversa.PruebasEntrega.Descargue;
using CO.Servidor.LogisticaInversa.PruebasEntrega.DescargueMovil;
using CO.Servidor.LogisticaInversa.PruebasEntrega.ReclameEnOficina;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;

namespace CO.Servidor.Servicios.Implementacion.LogisticaInversa
{
    /// <summary>
    /// Clase para los servicios de logistica inversa pruebas de entrega
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LILogisticaInversaSvc : ILILogisticaInversaSvc
    {
        #region Constructor

        public LILogisticaInversaSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion Constructor

        #region Manifiesto

        #region Consultar

        /// <summary>
        /// Método para consultar los tipos de manifiesto
        /// </summary>
        /// <returns>lista con los tipos de manifiesto</returns>
        public IEnumerable<LITipoManifiestoDC> ObtenerTiposManifiesto()
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerTiposManifiesto();
        }

        /// <summary>
        /// Metodo encargado de obtener los manifiestos asociados a una agencia
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns>Lista con los manifiestos filtrados</returns>
        public IEnumerable<LIManifiestoDC> ObtenerManifiestosFiltro(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerManifiestosFiltro(filtro, indicePagina, registrosPorPagina);
        }

        #endregion Consultar

        #region Adicionar

        /// <summary>
        /// Metodo para adicionar manifiestos
        /// </summary>
        /// <param name="manifiesto"></param>
        /// <returns>id del manifiesto generado</returns>
        public long AdicionarManifiesto(LIManifiestoDC manifiesto)
        {
            return LIAdministradorPruebasEntrega.Instancia.AdicionarManifiesto(manifiesto);
        }

        /// <summary>
        /// Método para insertar guías en un manifiesto
        /// </summary>
        /// <param name="guia">objeto tipo guía</param>
        public void AdicionarGuiaManifiesto(LIGuiaDC guia)
        {
            LIAdministradorPruebasEntrega.Instancia.AdicionarGuiaManifiesto(guia);
        }

        #endregion Adicionar

        #region Eliminar

        /// <summary>
        /// Elimina un manifiesto
        /// </summary>
        /// <param name="manifiesto"></param>
        public void EliminarManifiesto(LIManifiestoDC manifiesto)
        {
            LIAdministradorPruebasEntrega.Instancia.EliminarManifiesto(manifiesto);
        }

        /// <summary>
        /// Elimina una guia asociadad a un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        public void EliminarGuiaManifiesto(LIGuiaDC guia)
        {
            LIAdministradorPruebasEntrega.Instancia.EliminarGuiaManifiesto(guia);
        }

        #endregion Eliminar

        #endregion Manifiesto

        #region Descarga de Manifiesto

        #region Consultar

        /// <summary>
        /// Metodo encargado de obtener los manifiestos asociados a un Col destino
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IEnumerable<LIManifiestoDC> ObtenerManifiestosDestinoFiltro(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerManifiestosDestinoFiltro(filtro, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Metodo para obtener las guias por manifiesto
        /// </summary>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public IEnumerable<LIGuiaDC> ObtenerGuiasManifiestoDescarga(long idManifiesto)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerGuiasManifiestoDescarga(idManifiesto);
        }

        /// <summary>
        /// Metodo para obtener los motivos asociados a un tipo de motivo de una guía
        /// </summary>
        /// <param name="tipoMotivo">enumeracion de tipos de motivos posibles </param>
        /// <returns> lista de motivos guia</returns>
        public IList<ADMotivoGuiaDC> ObtenerMotivosGuias(ADEnumTipoMotivoDC tipoMotivo)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerMotivosGuias(tipoMotivo);
        }

        /// <summary>
        /// Método para obtener los tipos de evidencia de mensajeria
        /// </summary>
        /// <returns></returns>
        public IList<LITipoEvidenciaDevolucionDC> ObtenerTiposEvidencia()
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerTiposEvidencia();
        }

        #endregion Consultar

        #region Adicionar

        /// <summary>
        /// Método para insertar guías en un manifiesto
        /// </summary>
        /// <param name="guia">objeto tipo guía</param>
        public OUEnumValidacionDescargue GuardarCambiosGuiaAgencia(LIGuiaDC guia)
        {
            return LIAdministradorPruebasEntrega.Instancia.GuardarCambiosGuiaAgencia(guia);
        }

        /// <summary>
        /// Método para guardar un manifiesto manual y la guia asociada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="manifiesto"></param>
        /// <returns></returns>
        public LIManifiestoDC GuardarManifiestoManual(LIGuiaDC guia, LIManifiestoDC manifiesto)
        {
            return LIAdministradorPruebasEntrega.Instancia.GuardarManifiestoManual(guia, manifiesto);
        }

        #endregion Adicionar

        #region Actualizar

        /// <summary>
        /// Método encargado de actualizar el inicio de la fecha de descarga de un manifiesto
        /// </summary>
        public void ActualizarManifiesto(long idManifiesto)
        {
            LIAdministradorPruebasEntrega.Instancia.ActualizarManifiesto(idManifiesto);
        }

        #endregion Actualizar

        #endregion Descarga de Manifiesto

        #region Digitalizacion y Archivo

        /// <summary>
        /// Válida si existe el giro y si tiene un archivo
        /// </summary>
        /// <param name="imagenes">Imágenes</param>
        /// <returns>Imágenes</returns>
        public List<LIArchivoGiroDC> ExisteGiroArchivo(List<LIArchivoGiroDC> imagenes)
        {
            return LIAdministradorLogisticaInversa.Instancia.ExisteGiroArchivo(imagenes);
        }

        /// <summary>
        /// Adiciona un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public LIArchivoGiroDC AdicionarArchivoGiro(LIArchivoGiroDC imagen)
        {
            return LIAdministradorLogisticaInversa.Instancia.AdicionarArchivoGiro(imagen);
        }

        /// <summary>
        /// Editar un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoGiro(LIArchivoGiroDC imagen)
        {
            LIAdministradorLogisticaInversa.Instancia.EditarArchivoGiro(imagen);
        }

        /// <summary>
        /// Adicionar archivos de giro
        /// </summary>
        /// <param name="imagenes">Objeto imagenes</param>
        public List<LIArchivoGiroDC> AdicionarArchivosGiro(List<LIArchivoGiroDC> imagenes)
        {
            return LIAdministradorLogisticaInversa.Instancia.AdicionarArchivosGiro(imagenes);
        }

        /// <summary>
        /// Valida si se puede digitalizar la guía de acuerdo al estado
        /// </summary>
        /// <param name="imagenes">Colección de imágenes</param>
        public void GuardarGuiasCorrectas(List<LIArchivoGuiaMensajeriaDC> imagenes, bool entregaExitosa)
        {
            LIAdministradorLogisticaInversa.Instancia.GuardarGuiasCorrectas(imagenes, entregaExitosa);
        }

        /// <summary>
        /// Adiciona un archivo guía mensajería
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagen"></param>
        public void AdicionarArchivoGuiaMensajeria(bool entregaExitosa, LIArchivoGuiaMensajeriaDC imagen)
        {
            // LIAdministradorLogisticaInversa.Instancia.GuardarGuiasCorrectas(entregaExitosa, imagen);
        }

        /// <summary>
        /// Edita un archivo guía de mensajería
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoGuiaMensajeria(LIArchivoGuiaMensajeriaDC imagen)
        {
            LIAdministradorLogisticaInversa.Instancia.EditarArchivoGuiaMensajeria(imagen);
        }

        /// <summary>
        /// Válida si existe el giro y si tiene un archivo
        /// </summary>
        /// <param name="imagenes">Imágenes</param>
        /// <returns>Imágenes</returns>
        public List<LIArchivoComprobantePagoDC> ExisteComprobanteGiroArchivo(List<LIArchivoComprobantePagoDC> imagenes)
        {
            return LIAdministradorLogisticaInversa.Instancia.ExisteComprobanteGiroArchivo(imagenes);
        }

        /// <summary>
        /// Adiciona un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public LIArchivoComprobantePagoDC AdicionarComprobanteArchivoGiro(LIArchivoComprobantePagoDC imagen)
        {
            return LIAdministradorLogisticaInversa.Instancia.AdicionarComprobanteArchivoGiro(imagen);
        }

        /// <summary>
        /// Editar un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarComprobanteArchivoGiro(LIArchivoComprobantePagoDC imagen)
        {
            LIAdministradorLogisticaInversa.Instancia.EditarComprobanteArchivoGiro(imagen);
        }

        /// <summary>
        /// Adicionar comprobantes de pago de gito
        /// </summary>
        /// <param name="imagenes">Objeto imagenes</param>
        public List<LIArchivoComprobantePagoDC> AdicionarComprobanteArchivosGiro(List<LIArchivoComprobantePagoDC> imagenes)
        {
            return LIAdministradorLogisticaInversa.Instancia.AdicionarComprobanteArchivosGiro(imagenes);
        }

        /// <summary>
        /// Archiva una guía
        /// </summary>
        /// <param name="guia">Objeto guía</param>
        public LIArchivoGuiaMensajeriaDC GuardarArchivoGuiaMensajeria(LIArchivoGuiaMensajeriaDC guia)
        {
            return LIAdministradorLogisticaInversa.Instancia.GuardarArchivoGuiaMensajeria(guia);
        }

        /// <summary>
        /// Método para obtener un archivo guia
        /// </summary>
        /// <param name="imagen"></param>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaFS(LIArchivoGuiaMensajeriaDC archivoGuia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerArchivoGuiaFS(archivoGuia);
        }

        /// <summary>
        /// Método para obtener un archivo guia
        /// </summary>
        /// <param name="imagen"></param>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaSispostal(LIArchivoGuiaMensajeriaDC archivoGuia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerArchivoGuiaSispostal(archivoGuia);
        }



        /// <summary>
        /// Método para obtener un Imagen de fachada de una guia
        /// </summary>
        /// <param name="imagen"></param>
        public List<LIArchivoGuiaMensajeriaFachadaDC> ObtenerArchivoGuiaFachadaFS(long numeroGuia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerArchivoGuiaFachadaFS(numeroGuia);
        }


        /// <summary>
        /// Obtiene las guías archivadas
        /// </summary>
        /// <returns>Colección guías archivadas</returns>
        public List<LIArchivoGuiaMensajeriaDC> ObtenerArchivoGuia(long idCol)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerArchivoGuia(idCol);
        }

        /// <summary>
        /// Método para validar archivos de guías, para clasificarlas de acuerdo a su posible resultado
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagenes"></param>
        /// <returns></returns>
        public List<LIArchivoGuiaMensajeriaDC> ValidarArchivosGuias(bool entregaExitosa, List<LIArchivoGuiaMensajeriaDC> imagenes)
        {
            return LIAdministradorLogisticaInversa.Instancia.ValidarArchivosGuias(entregaExitosa, imagenes);
        }

        /// <summary>
        /// Método para validar la digitalización de los volantes de devolución
        /// </summary>
        /// <param name="imagen"></param>
        /// <returns></returns>
        public List<LIEvidenciaDevolucionDC> ValidarArchivosVolantes(List<LIEvidenciaDevolucionDC> imagenes)
        {
            return LIAdministradorLogisticaInversa.Instancia.ValidarArchivosVolantes(imagenes);
        }


        public List<LIEvidenciaDevolucionDC> ValidarArchivosVolantesWPF(List<LIEvidenciaDevolucionDC> imagenes)
        {
            return LIAdministradorLogisticaInversa.Instancia.ValidarArchivosVolantesWPF(imagenes);
        }

        public LIEvidenciaDevolucionDC AsociarNumeroGuiaAVolante(LIEvidenciaDevolucionDC imagenVolante)
        {
            return LIAdministradorLogisticaInversa.Instancia.AsociarNumeroGuiaAVolante(imagenVolante);
        }

        public List<LIEvidenciaDevolucionDC> ObtenerEvidenciaDevolucionxGuia(long NumeroGuia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerEvidenciaDevolucionxGuia(NumeroGuia);
        }

        /// <summary>
        /// Guarda un archivo de un volante
        /// </summary>
        /// <param name="imagenes">Colección de imágenes</param>
        public void GuardarVolantesCorrectos(List<LIEvidenciaDevolucionDC> imagenes)
        {
            LIAdministradorLogisticaInversa.Instancia.GuardarVolantesCorrectos(imagenes);
        }

        /// <summary>
        /// Edita un archivo de un volante
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoVolante(LIArchivosDC imagen)
        {
            LIAdministradorLogisticaInversa.Instancia.EditarArchivoVolante(imagen);
        }

        public ADGuia ObtenerFechaEstimadaEntregaGuia(long numeroguia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerFechaEstimadaEntregaGuia(numeroguia);
        }



        /// <summary>
        /// Metodo que obtiene los datos de archivo guía de mensajería    
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaxNumeroGuia(long numeroGuia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerArchivoGuiaxNumeroGuia(numeroGuia);
        }


        /// <summary>
        /// Metodo que obtiene los datos de volantes guía de mensajería    
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<string> ObtenerVolantesGuia(long numeroGuia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerVolantesGuia(numeroGuia);
        }


        #endregion Digitalizacion y Archivo

        #region Descargue de planillas

        #region Consulta

        /// <summary>
        /// Obtiene los mensajeros de una agencia especifica
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajeroDescargueAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long puntoServicio)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<OUMensajeroDC>()
            {
                Lista = LIAdministradorPruebasEntrega.Instancia.ObtenerMensajeroDescargueAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, puntoServicio),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Método para obtener las guías pendientes de un mensajero asignado a una agencia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerGuiasMensajero(idMensajero);
        }

        /// <summary>
        /// Método para obtener las guías pendientes de un auditor asignado a una COL
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerGuiasAuditor(idAuditor);
        }


        /// <summary>
        /// Método para obtener las guías pendientes del col asignadas a logistica invers
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasCol(long idCol)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerGuiasCol(idCol);
        }


        /// <summary>
        /// Método para obtener la última planilla de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUPlanillaAsignacionDC ObtenerUltimaPLanillaMensajero(long idMensajero)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerUltimaPLanillaMensajero(idMensajero);
        }

        /// <summary>
        /// Retorn el numero de motivos diferente porque ha sido devuelta la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>numero de motivos diferente porque ha sido devuelta la guia</returns>
        public OUMotivosDevolucionMensajeDC ObtenerConteoMotivosDevolucion(long? numeroGuia, string nombreMensajero, long idCol, string NombreMotivo)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerConteoMotivosDevolucion(numeroGuia, nombreMensajero, idCol, NombreMotivo);
        }

        /// <summary>
        /// Obtiene los mensajeros que han estadpos asignados a un planilla de la cual se descargo una guia como no entregada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIMensajeroResponsableDC> ObtenerMensajerosResponsablesDescargue(long numeroGuia)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerMensajerosResponsablesDescargue(numeroGuia);
        }

        /// <summary>
        /// obtener imagen fachada descargue app 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<string> ObtenerImagenFachadaApp(long numeroGuia)
        {

            return LIAdministradorLogisticaInversa.Instancia.ObtenerImagenFachadaApp(numeroGuia);

        }

        #endregion Consulta

        #region Adición

        /// <summary>
        /// Método para modifica o adicionar una guía de una planilla
        /// </summary>
        /// <param name="guia"></param>
        public OUEnumValidacionDescargue GuardarCambiosGuia(OUGuiaIngresadaDC guia, bool validaEntrega)
        {
            return LIAdministradorPruebasEntrega.Instancia.GuardarCambiosGuia(guia, validaEntrega);
        }

        #endregion Adición

        #region Deshacer

        /// <summary>
        /// Método para deshacer la entrega exitosa de una prueba de entrega
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public void DeshacerEntrega(OUGuiaIngresadaDC guia)
        {
            LIAdministradorPruebasEntrega.Instancia.DeshacerEntrega(guia);
        }

        #endregion Deshacer

        #endregion Descargue de planillas

        #region Notificaciones

        /// <summary>
        /// Retorna el archivo digitalizado de una guía
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADArchivoAlmacenGuia ObtenerArchivoAlmacenGuia(long numeroGuia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerArchivoAlmacenGuia(numeroGuia);
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
            return LIAdministradorLogisticaInversa.Instancia.ObtenerGuiasParaCapturaAutomatica(idFormaPago, fechaInicio, fechaFin, idCliente, idRacol);
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
            return LIAdministradorLogisticaInversa.Instancia.ObtenerGuiaParaRecibirManualNotificaciones(numeroGuia);
        }

        /// <summary>
        /// Registra recibido de guía manual
        /// </summary>
        /// <param name="recibido"></param>
        public void RegistrarRecibidoGuiaManual(LIRecibidoGuia recibido)
        {
            LIAdministradorLogisticaInversa.Instancia.RegistrarRecibidoGuiaManual(recibido);
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
            return LIAdministradorLogisticaInversa.Instancia.ObtenerNotificacionesRecibido(filtro, indicePagina, registrosPorPagina);
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
            return LIAdministradorLogisticaInversa.Instancia.GenerarGuiasInternasNotificacion(filtro, indicePagina, registrosPorPagina);
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
            return LIAdministradorLogisticaInversa.Instancia.ObtenerIdNotificaciones(filtro);
        }

        #endregion Notificaciones

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
        public GenericoConsultasFramework<LIPlanillaCertificacionesDC> ObtenerPlanillasCertificaciones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, bool esDevolucion)
        {
            int totalRegistros;
            return new GenericoConsultasFramework<LIPlanillaCertificacionesDC>()
            {
                Lista = LIAdministradorLogisticaInversa.Instancia.ObtenerPlanillasCertificaciones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, esDevolucion, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtener planillas de certificacion con Adoo
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="esDevolucion"></param>
        /// <returns></returns>
        public GenericoConsultasFramework<LIPlanillaCertificacionesDC> ObtenerPlanillasCertificacionesAdo(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, bool esDevolucion)
        {
            int totalPaginas;
            return new GenericoConsultasFramework<LIPlanillaCertificacionesDC>()
            {
                Lista = LIAdministradorLogisticaInversa.Instancia.ObtenerPlanillasCertificacionesAdo(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, esDevolucion, out totalPaginas),
                TotalRegistros = totalPaginas
            };
        }

        /// <summary>
        /// Método para obtener las guias de una planilla de certificacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<ADNotificacion> ObtenerGuiasPlanillaCertificacion(long idPlanilla)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerGuiasPlanillaCertificacion(idPlanilla);
        }

        /// <summary>
        /// Guarda los envios de la planilla de asignacion
        /// </summary>
        /// <param name="planilla"></param>
        public void GuardarLstEnvioPlanillaCertificacion(LIPlanillaCertificacionesDC planilla)
        {
            LIAdministradorLogisticaInversa.Instancia.GuardarLstEnvioPlanillaCertificacion(planilla);
        }

        /// <summary>
        /// Guarda guia en la planilla de certificacion
        /// </summary>
        /// <param name="planilla"></param>
        public LIPlanillaCertificacionesDC GuardarGuiaPlanillaCertificacion(LIPlanillaCertificacionesDC planilla)
        {
            return LIAdministradorLogisticaInversa.Instancia.GuardarGuiaPlanillaCertificacion(planilla);
        }

        /// <summary>
        /// Elimina una guia de la planilla de certificaciones
        /// </summary>
        /// <param name="guiaCertificacion"></param>
        /// <returns></returns>
        public ADNotificacion EliminarGuiaPlanillaCertificaciones(ADNotificacion guiaCertificacion)
        {
            return LIAdministradorLogisticaInversa.Instancia.EliminarGuiaPlanillaCertificaciones(guiaCertificacion);
        }

        /// <summary>
        /// Valida que la guia ingresada cumpla con las condiciones para la certificacion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADNotificacion ValidarGuiaCertificacionNotificacion(long numeroGuia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ValidarGuiaCertificacionNotificacion(numeroGuia);
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
            return LIAdministradorLogisticaInversa.Instancia.ObtenerNotificacionesFiltroSinPla(filtro, indicePagina, registrosPorPagina, idCol);
        }

        /// <summary>
        /// Retorna las guias internas de las planillas
        /// </summary>
        /// <param name="planillas"></param>
        public List<ADGuiaInternaDC> ObtenerGuiasInternasPlanilla(string planillas, bool esPlanilla, long idCol)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerGuiasInternasPlanilla(planillas, esPlanilla, idCol);
        }

        /// <summary>
        /// Descarga la planilla de certificacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void DescargarPlanillaCertificaciones(long idPlanilla)
        {
            LIAdministradorLogisticaInversa.Instancia.DescargarPlanillaCertificaciones(idPlanilla);
        }

        /// <summary>
        /// Método que adiciona una planilla de certificación
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public LIPlanillaCertificacionesDC AdicionarPlanilla(LIPlanillaCertificacionesDC planilla)
        {
            return LIAdministradorLogisticaInversa.Instancia.AdicionarPlanilla(planilla);
        }

        /// <summary>
        /// Método para adicionar una guía a una planilla
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public ADNotificacion AdicionarGuiaPlanilla(ADNotificacion guia)
        {
            return LIAdministradorLogisticaInversa.Instancia.AdicionarGuiaPlanilla(guia);
        }

        /// <summary>
        /// Método para cerrar una planilla de notificaciones
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        public void CerrarPlanillaNotificaciones(long numeroPlanilla)
        {
            LIAdministradorLogisticaInversa.Instancia.CerrarPlanillaNotificaciones(numeroPlanilla);
        }

        /// <summary>
        /// Método para generar las guias de la planilla de entrega
        /// </summary>
        /// <param name="planilla"></param>
        public void AdicionarGuiasPlanillaEntrega(LIPlanillaCertificacionesDC planilla)
        {
            LIAdministradorLogisticaInversa.Instancia.AdicionarGuiasPlanillaEntrega(planilla);
        }

        #endregion Planilla Certificacion

        #region GeneracionImagenes

        /// <summary>
        /// Obtiene los numeros de guias y la ruta
        /// </summary>
        /// <returns>Colección numeros de guia y rutas</returns>
        public List<LIArchivoGuiaMensajeriaDC> ObtenerGuiasRuta(string imagenesGenerar, int idCliente, string idCiudad, int idSucursal, DateTime fechaAdmisionInical, DateTime fechaAdmisionFinal, long guiaFacturaInicial, long guiaFacturaFinal, long ordenCompraInicial, long ordenCompraFinal)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerGuiasRuta(imagenesGenerar, idCliente, idCiudad, idSucursal, fechaAdmisionInical, fechaAdmisionFinal, guiaFacturaInicial, guiaFacturaFinal, ordenCompraInicial, ordenCompraFinal);
        }

        #endregion GeneracionImagenes

        #region Descargue de Devoluciones

        /// <summary>
        /// Método para obtener la informacion de una guia en el descargue de devoluciones por mensajero
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerInfoGuiaDevolucion(long numeroGuia, long idMensajero)
        {
            return LIDescarguePruebasEntrega.Instancia.ObtenerInfoGuiaDevolucion(numeroGuia, idMensajero);
        }

        /// <summary>
        /// Registro de una entrega desde la agencia
        /// </summary>
        /// <param name="guia"></param>
        public LIDescargueGuiaDC DescargueEntregaAgencia(OUGuiaIngresadaDC guia)
        {
            return LIDescarguePruebasEntrega.Instancia.DescargueEntregaAgencia(guia);
        }

        /// <summary>
        /// Método para descargar las devoluciones de una guia mensajero
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public LIDescargueGuiaDC GuardarDevolucion(OUGuiaIngresadaDC guia)
        {
            return LIDescarguePruebasEntrega.Instancia.GuardarDevolucion(guia);
        }

        /// <summary>
        /// Método para descargar las devoluciones de una guia mensajero
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public LIDescargueGuiaDC GuardarDevolucionAgencia(OUGuiaIngresadaDC guia)
        {
            return LIDescarguePruebasEntrega.Instancia.GuardarDevolucionAgencia(guia);
        }


        /// <summary>
        /// Guarda las devoluciones de auditoria
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public LIDescargueGuiaDC GuardarDevolucionAuditoria(OUGuiaIngresadaDC guia)
        {
            return LIDescarguePruebasEntrega.Instancia.GuardarDevolucionAuditoria(guia);
        }

        /// <summary>
        /// Guarda las devoluciones de auditoria
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public LIDescargueGuiaDC GuardarDevolucionCol(OUGuiaIngresadaDC guia)
        {
            return LIDescarguePruebasEntrega.Instancia.GuardarDevolucionCol(guia);
        }


        #endregion Descargue de Devoluciones

        #region Ingresa guia a reclame en oficina

        /// <summary>
        /// Método  Consulta guia asignada reclame en oficina
        /// </summary>
        /// <param name="IdCentroServicio"></param>
        /// /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public LIReclameEnOficinaDC AdicionarReclameEnOficina(LIReclameEnOficinaDC guiaReclameOficina)
        {
            return LIReclameEnOficina.Instancia.AdicionarReclameEnOficina(guiaReclameOficina);
        }


        #endregion

        #region Consulta guias ingresadas reclame en oficina

        /// <summary>
        /// Metodo para consultar los envios reclame en oficina por punto y tipoMovimiento
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public List<LIReclameEnOficinaDC> ConsultarGuiasReclameEnOficina(long idCentroServicio, int idTipoMovimiento, bool filtroDevolucion)
        {
            return LIReclameEnOficina.Instancia.ConsultarGuiasReclameEnOficina(idCentroServicio, idTipoMovimiento, filtroDevolucion);
        }

        /// <summary>
        /// consulta las guias reclame en oficina utilizando filtros
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public List<LIReclameEnOficinaDC> ConsultarGuiasReclameEnOficinaFiltros(long idCentroServicio, Dictionary<string, string> filtros)
        {
            return LIReclameEnOficina.Instancia.ConsultarGuiasReclameEnOficinaFiltros(idCentroServicio, filtros);
        }

        /// <summary>
        /// Metodo para consultar los totales de los  envios Asignados, Ingresados y para Devolucion de reclame en oficina por punto
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public Dictionary<string, int> ConsultarContadoresGuiasReclameEnOficina(long idCentroServicio)
        {
            return LIReclameEnOficina.Instancia.ConsultarContadoresGuiasReclameEnOficina(idCentroServicio);
        }


        #endregion

        #region metodo para ejecutar accion (entrega o devolucion) reclame en oficina


        public LIReclameEnOficinaDC DevolucionReclameOficina(LIReclameEnOficinaDC guiaReclameOficina)
        {
            return LIReclameEnOficina.Instancia.DevolucionReclameOficina(guiaReclameOficina);
        }

        public LIReclameEnOficinaDC EntregaReclameOficina(LIReclameEnOficinaDC guiaReclameOficina)
        {
            return LIReclameEnOficina.Instancia.EntregaReclameOficina(guiaReclameOficina);
        }


        #endregion

        public List<LIGestionesDC> ObtenerInformacionTelemercadeoGuia(long numeroGuia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ObtenerInformacionTelemercadeoGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene las guias por asignar de una respectiva bodega
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        /// 
        public OUGuiaIngresadaDC AsignarGuiaCentroAcopio(long numeroGuia, long idCentroServicio)
        {
            return LIAdministradorPruebasEntrega.Instancia.AsignarGuiaCentroAcopio(numeroGuia, idCentroServicio);
        }

        /// <summary>
        /// Crea la planilla de asignacion para auditores en terreno
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public OUPlanillaAsignacionDC GestionarPlanillaAuditor(OUPlanillaAsignacionDC planilla)
        {
            return LIAdministradorPruebasEntrega.Instancia.GestionarPlanillaAuditor(planilla);
        }

        /// <summary>
        /// Asignar mensajero a planilla
        /// </summary>
        /// <param name="planilla"></param>
        public void AsignaMensajeroPlanilla(OUPlanillaAsignacionDC planilla)
        {
            LIAdministradorPruebasEntrega.Instancia.AsignarPlanillaMensajero(planilla);
        }


        /// <summary>
        /// Metodo para obtener una guia para adicionar en planilla auditoria
        /// </summary>
        /// <param name="NUmeroGuia"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerGuiaBodegaPorAsignarAuditor(long NUmeroGuia)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerGuiaBodegaPorAsignarAuditor(NUmeroGuia);
        }

        /// <summary>
        /// Verifica si una guia tiene almenos una tapa por tipo tapa
        /// </summary>
        /// <param name="tapaLogistica"></param>
        /// <returns></returns>
        public bool VerificarGuiaConTapa(LITapaLogisticaDC tapaLogistica)
        {
            return LITapasLogisticaInversa.Instancia.VerificarGuiaConTapa(tapaLogistica);
        }

        /// <summary>
        /// Verifica si un numero de guia esta en la tabla gestionAuditoria para imprimir la tapa
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        /// 
        public bool VerificaGuiaConGestionAuditor(long numeroGuia)
        {
            return LITapasLogisticaInversa.Instancia.VerificaGuiaConGestionAuditor(numeroGuia);
        }


        /// <summary>
        /// Metodo para obtener las planillas centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="incluyeFecha"></param>
        /// <returns></returns>
        public List<OUPlanillaAsignacionDC> ObtenerPlanillasAsignacionCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, bool incluyeFecha)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerPlanillasAsignacionCentroLogistico(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCentroServicios, incluyeFecha);
        }

        /// <summary>
        /// Metodo para obtener mensajeros auditores
        /// </summary>
        /// <param name="puntoServicio"></param>
        /// <param name="esAgencia"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajerosAuditores(long puntoServicio, bool esAgencia)
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerMensajerosAuditores(puntoServicio, esAgencia);
        }


        #region Recibido Guia

        /// <summary>
        /// Método para obtener guias pendientes de captura datos de recibido
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<LIRecibidoGuia> ObtenerRecibidosPendientes(long idCol, DateTime fechaInicial, DateTime fechaFinal)
        {
            return LOINotificaciones.Instancia.ObtenerRecibidosPendientes(idCol, fechaInicial, fechaFinal);
        }

        public List<LIRecibidoGuia> ObtenerRecibidosPendientesApp(long idCol, DateTime fechaInicial, DateTime fechaFinal, LIEnumOrigenAplicacion idAplicacionOrigen)
        {
            return LOINotificaciones.Instancia.ObtenerRecibidosPendientesApp(idCol, fechaInicial, fechaFinal, idAplicacionOrigen);
        }

        #endregion

        #region Certificaciones Web

        /// <summary>
        /// Método para consultar la certificación WEB
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idRemitente"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        public ADNotificacion ValidarCertificacionWeb(LIReimpresionCertificacionDC reimprecionCertif)
        {
            return LOICertificacionWeb.Instancia.ValidarCertificacionWeb(reimprecionCertif);
        }



        /// <summary>
        /// Método para validar si la Certificación ya se ha impreso con anterioridad
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <returns></returns>
        public long ValidarNotificacionExisteAuditoria(long numeroguia)
        {
            return LOICertificacionWeb.Instancia.ValidarNotificacionExisteAuditoria(numeroguia);
        }

        /// <summary>
        /// Método que actualiza el campo está devuelta en la tabla Admision Notificaciones
        /// </summary>
        /// <param name="numeroguia"></param>
        public void ActualizaEstaDevueltaADMNotif(long numeroguia)
        {
            LOICertificacionWeb.Instancia.ActualizaEstaDevueltaADMNotif(numeroguia);
        }

        /// <summary>
        /// Método para validar si un envío tiene certifiación de entrega o de devolución
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public ADEstadoGuia ValidarEntregaoDevolucion(long NumeroGuia)
        {
            return LOICertificacionWeb.Instancia.ValidarEntregaoDevolucion(NumeroGuia);
        }

        /// <summary>
        /// Método que valida si existe la imagen de un envío
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public string ValidarImagenCertificacionWeb(long NumeroGuia)
        {
            return LOICertificacionWeb.Instancia.ValidarImagenCertificacionWeb(NumeroGuia);
        }

        /// <summary>
        /// Método para validar el Recibido Capturado
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public long ValidarRecibidoCapturado(long NumeroGuia)
        {
            return LOICertificacionWeb.Instancia.ValidarRecibidoCapturado(NumeroGuia);
        }

        /// <summary>
        /// Obtiene los datos de la Notificacion para la reimpresión
        /// </summary>
        /// <param name="numGuia"></param>
        /// <returns></returns>
        public ADNotificacion ObtenerDatosFacturaReImpresionNotificacion(long numGuia)
        {
            return LOICertificacionWeb.Instancia.ObtenerDatosFacturaReImpresionNotificacion(numGuia);
        }

        /// <summary>
        /// Realiza la afectación a caja para las Reimpresiones
        /// </summary>
        /// <param name="afectacionCajaReimp"></param>
        public void AfectacionCajaCertificacion(ADNotificacion afectacionCajaReimp)
        {
            LOICertificacionWeb.Instancia.AfectacionCajaCertificacion(afectacionCajaReimp);
        }

        /// <summary>
        /// Audita las guias reimpresas
        /// </summary>
        /// <param name="auditoriaReimpresion"></param>
        public void InsertarAuditoriaReimpresion(LIReimpresionCertificacionDC auditoriaReimpresion)
        {
            LOICertificacionWeb.Instancia.InsertarAuditoriaReimpresion(auditoriaReimpresion);
        }

        #endregion

            #region Agencias

            /// <summary>
            /// Método para obtener los pendientes de la agencia
            /// </summary>
            /// <param name="idAgencia"></param>
            /// <returns></returns>
        public List<LIDescargueGuiaAgenciaDC> ObtenerPendientesAgencia(long idAgencia)
        {
            return LIAgencia.Instancia.ObtenerPendientesAgencia(idAgencia);
        }



        #endregion

        #region Gestion Descargue Controller App


        /// <summary>
        /// Metodo para descargar guias mensajero
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueMensajeroControllerApp(LIDescargueControllerAppDC descargue)
        {
            return LIAdministradorDescargueMovil.Instancia.DescargueMensajeroControllerApp(descargue);
        }

        /// <summary>
        /// Metodo para descargar guias auditor
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueAuditorControllerApp(LIDescargueControllerAppDC descargue)
        {
            return LIAdministradorDescargueMovil.Instancia.DescargueAuditorControllerApp(descargue);
        }

        /// <summary>
        /// Metodo para devolucion mensajero controller app
        /// </summary>
        /// <param name="devolucion"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DevolucionMensajeroControllerApp(LIDescargueControllerAppDC devolucion)
        {
            return LIAdministradorDescargueMovil.Instancia.DevolucionMensajeroControllerApp(devolucion);
        }

        /// <summary>
        /// Metodo para devolucion auditor controller app 
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DevolucionRatificadaAuditorControllerApp(LIDescargueControllerAppDC devolucion)
        {
            return LIAdministradorDescargueMovil.Instancia.DevolucionRatificadaAuditorControllerApp(devolucion);
        }

        /// <summary>ap
        /// Metodo para descargue entrega maestra auditor controller app
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueEMAuditorControllerApp(LIDescargueControllerAppDC descargue)
        {
            return LIAdministradorDescargueMovil.Instancia.DescargueEMAuditorControllerApp(descargue);
        }

        #endregion

        #region Digitalizacion de agencias


        /// <summary>
        /// Método para validar archivos de guías, para clasificarlas de acuerdo a su posible resultado
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagenes"></param>
        /// <returns></returns>
        public List<LIArchivoGuiaMensajeriaDC> ValidarArchivosAgencias(List<LIArchivoGuiaMensajeriaDC> imagenes)
        {
            return LIAdministradorLogisticaInversa.Instancia.ValidarArchivosAgencias(imagenes);
        }

        #endregion

        #region Fallas Interlogis / web
        /// <summary>
        /// Metodo para obtener parametros por integracion 
        /// </summary>
        /// <param name="tipoParametro"></param>
        /// <returns></returns>
        [System.Obsolete()]
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracion(string tipoParametro)
        {
            return LIDescarguePruebasEntrega.Instancia.ObtenerParametrosPorIntegracion(tipoParametro);
        }

        #endregion

        #region Auditoria devolucion Controller App (Tapas)
        /// <summary>
        /// Metodo para insertar la auditoria de devolucion de controller app
        /// </summary>
        /// <param name="liGestionAuditorDC"></param>
        /// <returns></returns>
        public bool InsertarAuditoriaDevolucion(LIGestionAuditorDC liGestionAuditorDC)
        {
            return LIAdministradorLogisticaInversa.Instancia.InsertarAuditoriaDevolucion(liGestionAuditorDC);
        }

        public InformacionECAPTURE ValidarInformacionECapture(ADGuia guia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ValidarInformacionECapture(guia);
        }

        public LIArchivoGuiaMensajeriaDC ArchivarGuiaPruebaEntregaWPF(LIArchivoGuiaMensajeriaDC guia)
        {
            return LIAdministradorLogisticaInversa.Instancia.ArchivarGuiaPruebaEntregaWPF(guia);
        }

        #endregion

        #region Sispostal - Masivos

        /// <summary>
        /// Metodo para traer los motivos de devoluicion en Sispostal
        /// </summary>
        /// <returns></returns>
        /// <returns> lista de motivos de devolucion</returns>
        public IList<ADMotivoGuiaDC> ObtenerMotivosDevolucionGuiasMasivos()
        {
            return LIAdministradorPruebasEntrega.Instancia.ObtenerMotivosDevolucionGuiasMasivos();
        }

        #endregion

        #region Sispostal - Masivos

        /// <summary>
        /// Metodo para descargar guias mensajero
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueMasivosControllerApp(LIDescargueControllerAppDC descargue)
        {
            return LIAdministradorDescargueMovil.Instancia.DescargueMasivosControllerApp(descargue);
        }

        public bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega)
        {
            return LIAdministradorLogisticaInversa.Instancia.InsertarLecturaEcaptureArchivoPruebaEntrega(archivoPruebaEntrega);
        }

        #endregion


    }
}