using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun.CentroAcopio
{
    public interface ICAFachadaCentroAcopio
    {
        void CambiarTipoEntregaTelemercadeo_REO(long numeroguia, long IdCSDestino);

        /// <summary>
        /// cambia tipo de entraga de la admision
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <param name="IdCSDestino"></param>
        /// <returns></returns>
        CAAsignacionGuiaDC CambiarTipoEntrega_REO(long NumeroGuia, long IdCSDestino);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="numeroGuia">número de guía pore asignar a mensajero</param>
        /// <param name="idCSAsigna">centro de servicio que quiere asignar a mensajero</param>
        /// <returns></returns>
        bool validarAsignacionInventario(long numeroGuia, long idCSAsigna);


           /// <summary>
        /// Obtener Envios enviados desde LOI a Centro de Acopio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="idCentroServicioOrigen"></param>
        /// <returns></returns>
        List<CAAsignacionGuiaDC> ObtenerEnviosAsignadosporEstado(long idCentroServicioOrigen, ADEnumEstadoGuia idEstado);
       
    }


}
