using System;
using CO.Servidor.Dominio.Comun.LogisticaInversa;
using CO.Servidor.LogisticaInversa.DigitalizacionArchivo;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;

namespace CO.Servidor.LogisticaInversa
{
    /// <summary>
    /// Fachada para los procesos de logística inversa
    /// </summary>
    public class LIFachadaLogisticaInversa : ILIFachadaLogisticaInversa
    {
        /// <summary>
        /// Retorna el comprobante de pago de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerComprobantePagoGiro(long idAdmisionGiro)
        {
            return LIDigitalizacionArchivo.Instancia.ObtenerComprobantePagoGiro(idAdmisionGiro);
        }

        public void RegistrarRecibidoGuiaManual(LIRecibidoGuia recibido)
        {
            LIAdministradorLogisticaInversa.Instancia.RegistrarRecibidoGuiaManual(recibido);
        }

        /// <summary>
        /// Valida que un comprobante de pago ya se encuentre digitalizado
        /// </summary>
        /// <param name="imagen"></param>
        /// <returns></returns>
        public bool ConsultarArchivoComprobantePago(long idAdmisionGiro)
        {
            return LIDigitalizacionArchivo.Instancia.ConsultarArchivoComprobantePago(idAdmisionGiro);
        }

        public bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega)
        {
            return LIAdministradorLogisticaInversa.Instancia.InsertarLecturaEcaptureArchivoPruebaEntrega(archivoPruebaEntrega);
        }

        public bool ValidarRecepcionHistoricoEcapture(long numeroGuia, string codigoProceso)
        {
            return LIAdministradorLogisticaInversa.Instancia.ValidarRecepcionHistoricoEcapture(numeroGuia, codigoProceso);
        }
    }
}