using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;

namespace CO.Servidor.Dominio.Comun.LogisticaInversa
{
    public interface ILIFachadaLogisticaInversa
    {
        /// <summary>
        /// Retorna el comprobante de pago de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        string ObtenerComprobantePagoGiro(long idAdmisionGiro);

        void RegistrarRecibidoGuiaManual(LIRecibidoGuia recibido);

        /// <summary>
        /// Valida que un comprobante de pago ya se encuentre digitalizado
        /// </summary>
        /// <param name="imagen"></param>
        /// <returns></returns>
        bool ConsultarArchivoComprobantePago(long idAdmisionGiro);

        /// <summary>
        /// Metodo usado para actualizar la lectura de marca realizada por ECAPTURE en el proceso archivo prueba de entrega
        /// </summary>
        /// <param name="archivoPruebaEntrega"></param>
        /// <returns></returns>
        bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega);

        bool ValidarRecepcionHistoricoEcapture(long numeroGuia, string codigoProceso);        
    }
}