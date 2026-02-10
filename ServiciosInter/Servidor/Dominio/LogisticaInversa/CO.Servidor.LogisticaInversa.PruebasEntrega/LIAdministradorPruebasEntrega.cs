using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.LogisticaInversa.PruebasEntrega.Salida;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System.Collections.Generic;

namespace CO.Servidor.LogisticaInversa.PruebasEntrega
{
    /// <summary>
    /// Fachada para acceso a la administración de logística inversa para pruebas de entrga
    /// </summary>
    public class LIAdministradorPruebasEntrega
    {
        private static readonly LIAdministradorPruebasEntrega instancia = new LIAdministradorPruebasEntrega();

        public static LIAdministradorPruebasEntrega Instancia
        {
            get { return LIAdministradorPruebasEntrega.instancia; }
        }

        #region Manifiesto

        #region Consultar

        /// <summary>
        /// Método para consultar los tipos de manifiesto
        /// </summary>
        /// <returns>lista con los tipos de manifiesto</returns>
        public IEnumerable<LITipoManifiestoDC> ObtenerTiposManifiesto()
        {
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerTiposManifiesto();
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
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerManifiestosFiltro(filtro, indicePagina, registrosPorPagina);
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
            return LIConfiguradorPruebasEntrega.Instancia.AdicionarManifiesto(manifiesto);
        }

        /// <summary>
        /// Método para insertar guías en un manifiesto
        /// </summary>
        /// <param name="guia">objeto tipo guía</param>
        public void AdicionarGuiaManifiesto(LIGuiaDC guia)
        {
            LIConfiguradorPruebasEntrega.Instancia.AdicionarGuiaManifiesto(guia);
        }

        #endregion Adicionar

        #region Eliminar

        /// <summary>
        /// Elimina un manifiesto
        /// </summary>
        /// <param name="manifiesto"></param>
        public void EliminarManifiesto(LIManifiestoDC manifiesto)
        {
            LIConfiguradorPruebasEntrega.Instancia.EliminarManifiesto(manifiesto);
        }

        /// <summary>
        /// Elimina una guia asociadad a un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        public void EliminarGuiaManifiesto(LIGuiaDC guia)
        {
            LIConfiguradorPruebasEntrega.Instancia.EliminarGuiaManifiesto(guia);
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
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerManifiestosDestinoFiltro(filtro, indicePagina, registrosPorPagina);
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
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerGuiasManifiestoDescarga(idManifiesto);
        }



        #endregion Consultar

        #region Adicionar

        public OUEnumValidacionDescargue GuardarCambiosGuiaAgencia(LIGuiaDC guia)
        {
            return LIConfiguradorPruebasEntrega.Instancia.GuardarCambiosGuiaAgencia(guia);
        }

        /// <summary>
        /// Método para guardar un manifiesto manual y la guia asociada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="manifiesto"></param>
        /// <returns></returns>
        public LIManifiestoDC GuardarManifiestoManual(LIGuiaDC guia, LIManifiestoDC manifiesto)
        {
            return LIConfiguradorPruebasEntrega.Instancia.GuardarManifiestoManual(guia, manifiesto);
        }

        #endregion Adicionar

        #region Actualizar

        /// <summary>
        /// Método encargado de actualizar el inicio de la fecha de descarga de un manifiesto
        /// </summary>
        public void ActualizarManifiesto(long idManifiesto)
        {
            LIConfiguradorPruebasEntrega.Instancia.ActualizarManifiesto(idManifiesto);
        }

        #endregion Actualizar

        #endregion Descarga de Manifiesto

        #region Descargue de planillas

        #region Consulta

        /// <summary>
        /// Obtiene los mensajeros de una agencia especifica
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroDescargueAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalregistros, long puntoServicio)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerMensajeroDescargueAgencia
              (filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalregistros, puntoServicio);
        }

        /// <summary>
        /// Método para obtener las guías pendientes de un mensajero asignado a una agencia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerGuiasMensajero(idMensajero);
        }



        /// <summary>
        /// Método para obtener las guías pendientes de un auditor asignado a una COL
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerGuiasAuditor(idAuditor);
        }



        /// <summary>
        /// Método para obtener las guías pendientes del col asignadas a logistica invers
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasCol(long idCol)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerGuiasCol(idCol);
        }



        /// <summary>
        /// Método para obtener la última planilla de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUPlanillaAsignacionDC ObtenerUltimaPLanillaMensajero(long idMensajero)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerUltimaPLanillaMensajero(idMensajero);
        }

        /// <summary>
        /// retorna el numero de los motivos siempre y cuando sean diferentes
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>el numero de los motivos siempre y cuando sean diferentes</returns>
        public OUMotivosDevolucionMensajeDC ObtenerConteoMotivosDevolucion(long? numeroGuia, string nombreMensajero, long idCol, string NombreMotivo)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerConteoMotivosDevolucion(numeroGuia, nombreMensajero, idCol, NombreMotivo);
        }

        /// <summary>
        /// Obtiene los mensajeros que han estadpos asignados a un planilla de la cual se descargo una guia como no entregada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIMensajeroResponsableDC> ObtenerMensajerosResponsablesDescargue(long numeroGuia)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ObtenerMensajerosResponsablesDescargue(numeroGuia);
        }

        #endregion Consulta

        #region Adición

        /// <summary>
        /// Método para modifica o adicionar una guía de una planilla
        /// </summary>
        /// <param name="guia"></param>
        public OUEnumValidacionDescargue GuardarCambiosGuia(OUGuiaIngresadaDC guia, bool validaEntrega)
        {
            return LIConfiguradorPruebasEntrega.Instancia.GuardarCambiosGuia(guia, validaEntrega);
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
            LIConfiguradorPruebasEntrega.Instancia.DeshacerEntrega(guia);
        }

        #endregion Deshacer

        #endregion Descargue de planillas
        /// <summary>
        /// Metodo para obtener los motivos asociados a un tipo de motivo de una guía
        /// </summary>
        /// <param name="tipoMotivo">enumeracion de tipos de motivos posibles </param>
        /// <returns> lista de motivos guia</returns>
        public IList<ADMotivoGuiaDC> ObtenerMotivosGuias(ADEnumTipoMotivoDC tipoMotivo)
        {
            return EGMotivosGuia.ObtenerMotivosGuias(tipoMotivo);
        }

        /// <summary>
        /// Método para obtener los tipos de evidencia de mensajeria
        /// </summary>
        /// <returns></returns>
        public IList<LITipoEvidenciaDevolucionDC> ObtenerTiposEvidencia()
        {
            return EGMotivosGuia.ObtenerTiposEvidencia();
        }

        #region Salida Devoluciones



        /// <summary>
        /// Asigna guia a centro de acopio desde bodega logistica inversa
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC AsignarGuiaCentroAcopio(long numeroGuia, long idCentroServicio)
        {
            return LISalidaDevoluciones.Instancia.AsignarGuiaCentroAcopio(numeroGuia, idCentroServicio);
        }

        /// <summary>
        /// Asignar mensajero a planilla
        /// </summary>
        /// <param name="planilla"></param>
        public void AsignarPlanillaMensajero(OUPlanillaAsignacionDC planilla)
        {
            LISalidaDevoluciones.Instancia.AsignarPlanillaMensajero(planilla);
        }

        /// <summary>
        /// Metodo para obtener guia para asignar a planilla auditor
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerGuiaBodegaPorAsignarAuditor(long numeroGuia)
        {
            return LISalidaDevoluciones.Instancia.ObtenerGuiaBodegaPorAsignarAuditor(numeroGuia);
        }

        /// <summary>
        /// Metodo para crear y asignar planilla al mensajero
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public OUPlanillaAsignacionDC GestionarPlanillaAuditor(OUPlanillaAsignacionDC planilla)
        {
            return LISalidaDevoluciones.Instancia.GestionarPlanillaAuditor(planilla);
        }

        /// <summary>
        /// Metodo para obtener las planillas del centro logistico
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
            return LISalidaDevoluciones.Instancia.ObtenerPlanillasAsignacionCentroLogistico(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCentroServicios, incluyeFecha);
        }

        /// <summary>
        /// Metodo para obtener los mensajeros auditores
        /// </summary>
        /// <param name="puntoServicio"></param>
        /// <param name="esAgencia"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajerosAuditores(long puntoServicio, bool esAgencia)
        {
            return LISalidaDevoluciones.Instancia.ObtenerMensajerosAuditores(puntoServicio, esAgencia);

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
            return EGMotivosGuia.ObtenerMotivosDevolucionGuiasMasivos();
        }

        #endregion
    }
}