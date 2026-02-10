using CO.Servidor.Dominio.Comun.AdmEstadosGuia.Datos;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun.AdmEstadosGuia
{
    public static class EGMotivosGuia
    {
        
        /// <summary>
        /// Metodo para obtener los motivos asociados a un tipo de motivo de una guía
        /// </summary>
        /// <param name="tipoMotivo">enumeracion de tipos de motivos posibles </param>
        /// <returns> lista de motivos guia</returns>
        public static IList<ADMotivoGuiaDC> ObtenerMotivosGuias(ADEnumTipoMotivoDC tipoMotivo)
        {
            return EGRepositorio.Instancia.ObtenerMotivosGuias(tipoMotivo);
        }

        /// <summary>
        /// Método para obtener el tipo de evidencia segun el motivo y el intento de entrega
        /// </summary>
        /// <param name="motivo"></param>
        /// <param name="intentoEntrega"></param>
        /// <returns></returns>
        public static LITipoEvidenciaDevolucionDC ObtenerMotivosEvidencia(ADMotivoGuiaDC motivo, short intentoEntrega)
        {
            return EGRepositorio.Instancia.ObtenerMotivosEvidencia(motivo, intentoEntrega);
        }


                /// <summary>
        /// Método para obtener los tipos de evidencia de mensajeria
        /// </summary>
        /// <returns></returns>
        public static IList<LITipoEvidenciaDevolucionDC> ObtenerTiposEvidencia()
        {
            return EGRepositorio.Instancia.ObtenerTiposEvidencia();
        }


        /// <summary>
        /// Metodo para obtener el proximo estado de una guía de devolución
        /// </summary>
        /// <param name="idMotivo"></param>
        /// <param name="estadoActual"></param>
        /// <returns></returns>
        public static short ObtenerEstadoMotivo(short idMotivo, short estadoActual, short intentoEntrega = 1)
        {
            return EGRepositorio.Instancia.ObtenerEstadoMotivo(idMotivo, estadoActual, intentoEntrega);
        }


        #region Sispostal - Masivos

        /// <summary>
        /// Metodo para traer los motivos de devoluicion en Sispostal
        /// </summary>
        /// <returns></returns>
        /// <returns> lista de motivos de devolucion</returns>
        public static IList<ADMotivoGuiaDC> ObtenerMotivosDevolucionGuiasMasivos()
        {
            return EGRepositorio.Instancia.ObtenerMotivosDevolucionGuiasMasivos();
        }

        #endregion
    }
}
