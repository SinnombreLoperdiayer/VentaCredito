using CO.Servidor.OperacionUrbana.Datos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.OperacionUrbana
{
    public class OUAuditoriaControllerApp : ControllerBase
    {
        #region Singleton
        private static readonly OUAuditoriaControllerApp instancia = (OUAuditoriaControllerApp)FabricaInterceptores.GetProxy(new OUAuditoriaControllerApp(), COConstantesModulos.MODULO_OPERACION_URBANA);

        /// <summary>
        /// Retorna una instancia de
        /// recogidas
        /// /// </summary>
        public static OUAuditoriaControllerApp Instancia
        {
            get { return OUAuditoriaControllerApp.instancia; }
        }


        #endregion

        #region Metodos

        /// <summary>
        /// Metodo para obtener las guias planillas al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZona(long idMensajero)
        {
            return OURepositorioAuditoriaController.Instancia.ObtenerGuiasMensajeroEnZona(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener las guias entregadas planilladas del mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasMensajero(long idMensajero)
        {
            return OURepositorioAuditoriaController.Instancia.ObtenerGuiasEntregadasMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener las guias en devolucion del mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionMensajero(long idMensajero)
        {
            return OURepositorioAuditoriaController.Instancia.ObtenerGuiasDevolucionMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener las guias en zona del auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEnZonaAuditor(long idAuditor)
        {
            return OURepositorioAuditoriaController.Instancia.ObtenerGuiasEnZonaAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener las guias entregadas por auditor 
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasAuditor(long idAuditor)
        {
            return OURepositorioAuditoriaController.Instancia.ObtenerGuiasEntregadasAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener las guias en devolucion del auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionAuditor(long idAuditor)
        {
            return OURepositorioAuditoriaController.Instancia.ObtenerGuiasDevolucionAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener la guia en planilla para descargue controller app
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaDescargue(long numeroGuia, long idMensajero)
        {
            return OURepositorioAuditoriaController.Instancia.ObtenerGuiaEnPlanillaDescargue(numeroGuia, idMensajero);
        }
        /// <summary>
        /// Metodo para obtener la guia planillada para descargue controller app
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaAuditorDescargue(long numeroGuia, long idMensajero)
        {
            return OURepositorioAuditoriaController.Instancia.ObtenerGuiaEnPlanillaAuditorDescargue(numeroGuia, idMensajero);
        }

        #region Sispostal - Modulo de Masivos

        /// <summary>
        /// Metodo para obtener guias por estado Sispostal
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="estado"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZonaMasivos(long idMensajero, short estado)
        {
            return OURepositorioAuditoriaController.Instancia.ObtenerGuiasMensajeroEnZonaMasivos(idMensajero, estado);
        }

        #endregion

        #endregion
    }
}
