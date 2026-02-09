using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using System.Collections.Generic;

namespace CO.Servidor.Dominio.Comun.LogisticaInversa
{
    /// <summary>
    /// Interfaz para la fachada de logística inversa para pruebas de entrega
    /// </summary>
    public interface ILIFachadaLogisticaInversaPruebasEntrega
    {
        /// <summary>
        /// Método para validar si una guia ya tiene un recibido
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        bool ValidarRecibidoGuia(long numeroGuia);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NGuia"></param>
        /// <returns></returns>
        bool ValidarGuiaDescargadaXAppMasivos(long NGuia);

        /// <summary>
        /// Metodo encargado de insertar la lectura realizada por ECAPTURE
        /// </summary>
        /// <param name="archivoPruebaEntrega"></param>
        /// <returns></returns>
        bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega);

        /// <summary>
        /// Metodo encargado de validar registro de sincronizacion folder ECAPTURE
        /// </summary>
        /// <param name="archivoPruebaEntrega"></param>
        /// <returns></returns>
        bool ValidarRecepcionHistoricoEcapture(long numeroGuia, string codigoProceso);

        List<ArchivoGuia> VerificarGuia(string numeroGuia);

        void ActualizarArchivoGuiaDigitalizada(ArchivoGuia archivoGuia);

        void InsertaHistoricoArchivoGuiaDigitalizada(ArchivoGuia archivoGuia);

        List<ArchivoVolante> VerificarVolante(string numeroVolante);

        void ActualizarArchivoVolanteSincronizado(ArchivoVolante archivoVolante);

        int ConsultarOrigenGuia(long numeroGuia);

    }
}