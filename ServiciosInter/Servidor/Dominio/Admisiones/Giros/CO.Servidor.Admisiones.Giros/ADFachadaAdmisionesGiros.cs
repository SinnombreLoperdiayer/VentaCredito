using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Admisiones.Giros.Pago;
using CO.Servidor.Admisiones.Giros.Venta;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;

namespace CO.Servidor.Admisiones.Giros
{
    /// <summary>
    /// Fachada de Admisiones Giros
    /// </summary>
    public class ADFachadaAdmisionesGiros : IADFachadaAdmisionesGiros
    {
        /// <summary>
        /// Obtiene el identificador de admisión giro
        /// </summary>
        /// <param name="idGiro">Identificador giro</param>
        /// <returns>Identificador admisión giro</returns>
        public long? ObtenerIdentificadorAdmisionGiro(long idGiro)
        {
            return GIVentaGiros.Instancia.ObtenerIdentificadorAdmisionGiro(idGiro);
        }

        /// <summary>
        /// Obtiene el identificador de pagos giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión</param>
        /// <returns>Identificador</returns>
        public long ObtenerIdentificadorPagosGiro(long idAdmisionGiro)
        {
            return PGPagosGiros.Instancia.ObtenerIdentificadorPagosGiro(idAdmisionGiro);
        }

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ConsultarTiposIdentificacionReclamaGiros()
        {
            return GIVentaGiros.Instancia.ConsultarTiposIdentificacionReclamaGiros();
        }

        /// <summary>
        /// Obtiene la informacion de un giro por el numero de comprobante de pago
        /// </summary>
        /// <param name="idComprobantePago"></param>
        /// <returns></returns>
        public PGPagosGirosDC ObtenerGiroPorComprobantePago(long idComprobantePago)
        {
            return PGPagosGiros.Instancia.ObtenerGiroPorComprobantePago(idComprobantePago);
        }

        /// <summary>
        /// Realiza el pago del giro
        /// </summary>
        public PGComprobantePagoDC PagarGiro(PGPagosGirosDC pagosGiros)
        {
            return PGPagosGiros.Instancia.PagarGiro(pagosGiros);
        }

        /// <summary>
        /// Creacion de un giro
        /// </summary>
        /// <param name="giro"></param>
        public GINumeroGiro CrearGiroProduccion(GIAdmisionGirosDC giro)
        {
            return GIVentaGiros.Instancia.CrearGiroProduccion(giro);
        }

        /// <summary>
        /// Obtiene los giros no pagos por centro SVC.
        /// </summary>
        /// <param name="idCentroSrv">The id centro SRV.</param>
        /// <returns>el numero de giros y valor total de los giros no pagos por un centro de servicio </returns>
        public PGTotalPagosDC ConsultarPagosAgencia(long idCentroSrv)
        {
            return PGPagosGiros.Instancia.ConsultarPagosAgencia(idCentroSrv);
        }

        /// <summary>
        /// Consultar la informacion de la tabla peaton Convenio
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public GIAdmisionGirosDC ConsultarInformacionPeatonConvenio(long idAdmisionGiro)
        {
            return GIVentaGiros.Instancia.ConsultarInformacionPeatonConvenio(idAdmisionGiro);
        }

        /// <summary>
        /// Consultar la informacion de la tabla peaton peaton
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public GIAdmisionGirosDC ConsultarInformacionPeatonPeaton(long idAdmisionGiro)
        {
            return GIVentaGiros.Instancia.ConsultarInformacionPeatonPeaton(idAdmisionGiro);
        }

        public bool ValidarDeclaracionFondos(decimal valorAcumulado)
        {
            return GIVentaGiros.Instancia.ValidarDeclaracionFondos(valorAcumulado);
        }

        /// <summary>
        /// envia informacion a los modulos de comision y cajas
        /// </summary>
        /// <param name="pago"></param>
        public void EnviarTransaccionCaja(PGPagosGirosDC pago, PGComprobantePagoDC comprobante, bool esDevolGiro = false, bool esDevolFlete=false)
        {
            PGPagosGiros.Instancia.EnviarTransaccionCaja(pago, comprobante,esDevolGiro,esDevolFlete);
        }

        /// <summary>
        /// Consultar el giro por numero de giro
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroXNumGiro(long idGiro)
        {
            return PGPagosGiros.Instancia.ConsultarGiroXNumGiro(idGiro);
        }

        ///// <summary>
        ///// Consultar el giro por numero de giro
        ///// y centro Servicio destino
        ///// </summary>
        ///// <param name="idGiro"></param>
        //public GIAdmisionGirosDC ConsultarGiroXNumGiroCentroServicio(long idGiro, long idCentroSvc)
        //{
        //    return PGPagosGiros.Instancia.ConsultarGiroXNumGiroCentroServicio(idGiro, idCentroSvc);
        //}

        /// <summary>
        /// Actualiza la Informacion del Giro
        /// por una Solicitud
        /// </summary>
        /// <param name="giroUpdate">info del giro a Actualizar</param>
        public void ActualizarInfoGiro(GIAdmisionGirosDC giroUpdate)
        {
            GIAdministracionGiros.Instancia.ActualizarInfoGiro(giroUpdate);
        }

        /// <summary>
        /// Consulta un parametro de Giros según el id del Parametro
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        public GIParametrosGirosDC ConsultarParametrosGiros(string idParametro)
        {
            return GIAdministracionGiros.Instancia.ConsultarParametrosGiros(idParametro);
        }
    }
}