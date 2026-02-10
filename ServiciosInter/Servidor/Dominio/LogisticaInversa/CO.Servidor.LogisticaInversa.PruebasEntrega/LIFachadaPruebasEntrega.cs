using System;
using System.Collections.Generic;
using CO.Servidor.Dominio.Comun.LogisticaInversa;
using CO.Servidor.LogisticaInversa;
using CO.Servidor.LogisticaInversa.Notificaciones;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;

namespace CO.Servidor.LogisticaInversa.PruebasEntrega
{
    /// <summary>
    /// Fachada para acceso a la lógica de logística inversa
    /// </summary>
    public class LIFachadaPruebasEntrega : ILIFachadaLogisticaInversaPruebasEntrega
    {
        /// <summary>
        /// Método para validar si una guia ya tiene un recibido
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ValidarRecibidoGuia(long numeroGuia)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ValidarRecibidoGuia(numeroGuia);
        }

        public bool ValidarGuiaDescargadaXAppMasivos(long NGuia)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ValidarGuiaDescargadaXAppMasivos(NGuia);
        }

        /// <summary>
        /// Metodo usado para realizar la actualizacion de la lectura de marca realizada por ECAPTURE
        /// </summary>
        /// <param name="archivoPruebaEntrega">Datos de registro de la lectura</param>
        /// <returns>Flag con resultado del proceso</returns>
        public bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega)
        {
            return LIConfiguradorPruebasEntrega.Instancia.InsertarLecturaEcaptureArchivoPruebaEntrega(archivoPruebaEntrega);
        }

        public bool ValidarRecepcionHistoricoEcapture(long numeroGuia, string codigoProceso)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ValidarRecepcionHistoricoEcapture(numeroGuia, codigoProceso);
        }

        public List<ArchivoGuia> VerificarGuia(string numeroGuia)
        {
            return LIConfiguradorPruebasEntrega.Instancia.VerificarGuia(numeroGuia);
        }

        public void ActualizarArchivoGuiaDigitalizada(ArchivoGuia archivoGuia)
        {
            LIConfiguradorPruebasEntrega.Instancia.ActualizarArchivoGuiaDigitalizada(archivoGuia);
        }

        public void InsertaHistoricoArchivoGuiaDigitalizada(ArchivoGuia archivoGuia)
        {
            LIConfiguradorPruebasEntrega.Instancia.InsertaHistoricoArchivoGuiaDigitalizada(archivoGuia);
        }

        public List<ArchivoVolante> VerificarVolante(string numeroVolante)
        {
            return LIConfiguradorPruebasEntrega.Instancia.VerificarVolante(numeroVolante);
        }

        public void ActualizarArchivoVolanteSincronizado(ArchivoVolante archivoVolante)
        {
            LIConfiguradorPruebasEntrega.Instancia.ActualizarArchivoVolanteSincronizado(archivoVolante);
        }

        public int ConsultarOrigenGuia(long numeroGuia)
        {
            return LIConfiguradorPruebasEntrega.Instancia.ConsultarOrigenGuia(numeroGuia);
        }
    }
}