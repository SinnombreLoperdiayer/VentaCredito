using CO.Servidor.Dominio.Comun.CentroAcopio;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;
using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.CentroAcopio
{
    public class CAFachadaCentroAcopio : ICAFachadaCentroAcopio
    {

        private static readonly CAFachadaCentroAcopio instancia = new CAFachadaCentroAcopio();

        public void CambiarTipoEntregaTelemercadeo_REO(long numeroguia, long IdCSDestino)
        {
            CACentroAcopio.Instancia.CambiarTipoEntregaTelemercadeo_REO(numeroguia, IdCSDestino);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numeroGuia">número de guía pore asignar a mensajero</param>
        /// <param name="idCSAsigna">centro de servicio que quiere asignar a mensajero</param>
        /// <returns></returns>
        public bool validarAsignacionInventario(long numeroGuia, long idCSAsigna)
        {
            return CACentroAcopio.Instancia.validarAsignacionMovimientoInventario(numeroGuia, idCSAsigna);
        }
        #region Envio NN
        /// <summary>
        /// Inserta Envio NN
        /// </summary>
        /// <param name="numeroGuia">número de guía pore asignar a mensajero</param>
        /// <param name="idCSAsigna">centro de servicio que quiere asignar a mensajero</param>
        /// <returns></returns>
        public long InsertarEnvioNN(ADEnvioNN envioNN)
        {
            return CACentroAcopio.Instancia.InsertarEnvioNN(envioNN);
        }
        #endregion


        /// <summary>
        /// Obtener Envios enviados desde LOI a Centro de Acopio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="idCentroServicioOrigen"></param>
        /// <returns></returns>
        public List<CAAsignacionGuiaDC> ObtenerEnviosAsignadosporEstado(long idCentroServicioOrigen, ADEnumEstadoGuia idEstado)
        {
            return CACentroAcopio.Instancia.ObtenerReenviosBodegas_CAC(idCentroServicioOrigen, idEstado);
        }

        /// <summary>
        /// Obtiene las Guias que se Eliminan de la planilla desde Centro de Acopio por Envio Fuera de Zona
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public List<CAAsignacionGuiaDC> ObtenerGuiasEliminadasPlanillaCentroAcopio(string usuario)
        {
            return CACentroAcopio.Instancia.ObtenerGuiasEliminadasPlanillaCentroAcopio(usuario);
        }

        public CAAsignacionGuiaDC CambiarTipoEntrega_REO(long NumeroGuia, long IdCSDestino)
        {
            return CACentroAcopio.Instancia.CambiarTipoEntrega_REO(NumeroGuia, IdCSDestino);
        }
    }
}
