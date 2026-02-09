using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;

namespace CO.Servidor.Dominio.Comun.Admisiones
{
    /// <summary>
    /// Interfaz de la fachada de admisiones giros
    /// </summary>
    public interface IADFachadaAdmisionesGiros
    {
        /// <summary>
        /// Obtiene el identificador de admisión giro
        /// </summary>
        /// <param name="idGiro">Identificador giro</param>
        /// <returns>Identificador admisión giro</returns>
        long? ObtenerIdentificadorAdmisionGiro(long idGiro);

        /// <summary>
        /// Obtiene el identificador de pagos giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión</param>
        /// <returns>Identificador</returns>
        long ObtenerIdentificadorPagosGiro(long idAdmisionGiro);

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        IList<PATipoIdentificacion> ConsultarTiposIdentificacionReclamaGiros();

        /// <summary>
        /// Obtiene la informacion de un giro por el numero de comprobante de pago
        /// </summary>
        /// <param name="idComprobantePago"></param>
        /// <returns></returns>
        PGPagosGirosDC ObtenerGiroPorComprobantePago(long idComprobantePago);

        /// <summary>
        /// Creacion de un giro
        /// </summary>
        /// <param name="giro"></param>
        GINumeroGiro CrearGiroProduccion(GIAdmisionGirosDC giro);

        /// <summary>
        /// Obtiene los giros no pagos por centro SVC.
        /// </summary>
        /// <param name="idCentroSrv">The id centro SRV.</param>
        /// <returns>el numero de giros y valor total de los giros no pagos por un centro de servicio </returns>
        PGTotalPagosDC ConsultarPagosAgencia(long idCentroSrv);

        /// <summary>
        /// Consultar la informacion de la tabla peaton Convenio
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        GIAdmisionGirosDC ConsultarInformacionPeatonConvenio(long idAdmisionGiro);

        /// <summary>
        /// Consultar la informacion de la tabla peaton peaton
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        GIAdmisionGirosDC ConsultarInformacionPeatonPeaton(long idAdmisionGiro);

        /// <summary>
        /// Valida si el monto del giro necesita declaracion voluntaria de fondos
        /// </summary>
        /// <param name="valorAcumulado"></param>
        /// <returns></returns>
        bool ValidarDeclaracionFondos(decimal valorAcumulado);

        /// <summary>
        /// envia informacion a los modulos de comision y cajas
        /// </summary>
        /// <param name="pago"></param>
        void EnviarTransaccionCaja(PGPagosGirosDC pago, PGComprobantePagoDC comprobante, bool esDevolGiro = false, bool esDevolFlete = false);

        /// <summary>
        /// Consultar el giro por numero de giro
        /// </summary>
        /// <param name="idGiro"></param>
        GIAdmisionGirosDC ConsultarGiroXNumGiro(long idGiro);

        /// <summary>
        /// Actualiza la Informacion del Giro
        /// por una Solicitud
        /// </summary>
        /// <param name="giroUpdate">info del giro a Actualizar</param>
        void ActualizarInfoGiro(GIAdmisionGirosDC giroUpdate);


        /// <summary>
        /// Consulta un parametro de Giros según el id del Parametro
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        GIParametrosGirosDC ConsultarParametrosGiros(string idParametro);
    }
        
}