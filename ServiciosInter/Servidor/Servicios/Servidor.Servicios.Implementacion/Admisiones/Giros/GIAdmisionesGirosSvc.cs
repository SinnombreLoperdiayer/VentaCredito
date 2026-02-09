using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CO.Servidor.Admisiones.Giros;
using CO.Servidor.Admisiones.Giros.Pago;
using CO.Servidor.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;

namespace CO.Servidor.Servicios.Implementacion.Admisiones.Giros
{
    /// <summary>
    /// Clase para los servicios WCF del modulo Centros de servicios
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    internal class GIAdmisionesGirosSvc : IGIAdmisionesGirosSvc
    {
        #region Giros

        /// <summary>
        /// Consulta la agencia a la cual se le suministro la factura de venta, con el numero de giro IdGiro
        /// </summary>
        /// <param name="IdGiro">Numero del giro</param>
        /// <returns>Centro de servicio</returns>
        public PUCentroServiciosDC ObtenerAgenciaPropietariaDelNumeroGiro(long idGiro, long idCentroServicio, bool esUsuarioRacol, long idRacol)
        {
            return GIVentaGiros.Instancia.ObtenerAgenciaPropietariaDelNumeroGiro(idGiro, idCentroServicio, esUsuarioRacol, idRacol);
        }

        /// <summary>
        /// Creacion de un giro
        /// </summary>
        /// <param name="giro"></param>
        public GINumeroGiro CrearGiro(GIAdmisionGirosDC giro)
        {
            return GIVentaGiros.Instancia.CrearGiro(giro);
        }

        /// <summary>
        /// Consulta la informacion de un giro a partir de el guid
        /// </summary>
        /// <param name="GuidDeChequeo"></param>
        /// <returns></returns>
        public GINumeroGiro ConsultarGiroPorGuid(string guidDeChequeo)
        {
            return GIVentaGiros.Instancia.ConsultarGiroPorGuid(guidDeChequeo);
        }

        /// <summary>
        /// consulta los giros activos realizados el dia actual
        /// No retorna todos los valores del giro
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public IList<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgencia(long idCentroServicio, long idGiro, string tipoIdRemitente, string identificacionRemitente, string tipoIdDestinatario, string identificacionDestinatario, string estadoGiro, int indicePagina, int registrosPorPagina, string tipoCentroServicio)
        {
            return GIVentaGiros.Instancia.ConsultarGirosPeatonPeatonPorAgencia(idCentroServicio, idGiro, tipoIdRemitente, identificacionRemitente, tipoIdDestinatario, identificacionDestinatario, estadoGiro, indicePagina, registrosPorPagina, tipoCentroServicio);
        }

        /// <summary>
        /// consulta los giros activos realizados el dia actual Peaton convenio
        /// No retorna todos los valores del giro
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<GIAdmisionGirosDC> ConsultarGirosPeatonConvenioPorAgencia(long idCentroServicio, long idGiro, string tipoIdRemitente, string identificacionRemitente, string tipoIdDestinatario, string identificacionDestinatario, string tipoDocumentoGiro, int indicePagina, int registrosPorPagina, string tipoCentroServicio)
        {
            return GIVentaGiros.Instancia.ConsultarGirosPeatonConvenioPorAgencia(idCentroServicio, idGiro, tipoIdRemitente, identificacionRemitente, tipoIdDestinatario, identificacionDestinatario, tipoDocumentoGiro, indicePagina, registrosPorPagina, tipoCentroServicio);
        }


        /// <summary>
        /// Consultar la informacion de la tabla peaton peaton
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public GIAdmisionGirosDC ConsultarInformacionPeatonPeaton(long idAdmisionGiro)
        {
            return GIVentaGiros.Instancia.ConsultarInformacionPeatonPeaton(idAdmisionGiro);
        }

        // TODO:ID Consulta de Giros para Integracion 742
        public List<GIAdmisionGirosDC> ConsultarGirosPPEIntegracionSieteCuatroDos(string pEstado)
        {
            return GIVentaGiros.Instancia.ConsultarGirosPPEIntegracionSieteCuatroDos(pEstado);
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
        /// Valida que una agencia pueda realizar la venta de un giro
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        public void ValidarAgenciaServicioGiros(long idCentroServicios)
        {
            GIVentaGiros.Instancia.ValidarAgenciaServicioGiros(idCentroServicios);
        }

        /// <summary>
        /// Obtiene la informacion inicial del peaton peaton
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns></returns>
        public GIAdmisionGirosDC ConsultarInformacionPeatonPeatonInicial(long idAdmisionGiro)
        {
            return GIVentaGiros.Instancia.ConsultarInformacionPeatonPeatonInicial(idAdmisionGiro);
        }

        /// <summary>
        /// Obtiene la informacion inicial del peaton convenio
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns></returns>
        public GIAdmisionGirosDC ConsultarInformacionPeatonConvenioInicial(long idAdmisionGiro)
        {
            return GIVentaGiros.Instancia.ConsultarInformacionPeatonConvenioInicial(idAdmisionGiro);
        }


        #endregion Giros

        #region Pagos

        #region Consultas de Giros a pagar

        /// <summary>
        /// Consultar la cantidad de pagos y la sumatoria total de los mismos
        /// por agencia
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public PGTotalPagosDC ConsultarPagosAgencia(long idCentroServicio)
        {
            return PGPagosGiros.Instancia.ConsultarPagosAgencia(idCentroServicio);
        }

        /// <summary>
        /// Consultar el giro por numero de giro
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroXNumGiro(long idGiro)
        {
            return PGPagosGiros.Instancia.ConsultarGiroXNumGiro(idGiro);
        }

        /// <summary>
        /// Consultar el giro por numero de giro y la ciudad de destino
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroNumGiroCiudadDestino(long idGiro, string idCiudadDestino)
        {
            return PGPagosGiros.Instancia.ConsultarGiroNumGiroCiudadDestino(idGiro, idCiudadDestino);
        }

        /// <summary>
        /// Consultar el giro por numero de giro
        /// y centro Servicio destino
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroXNumGiroCentroServicio(long idGiro, long idCentroSvc)
        {
            return PGPagosGiros.Instancia.ConsultarGiroXNumGiroCentroServicio(idGiro, idCentroSvc);
        }

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        public List<GIAdmisionGirosDC> ConsultarGiroXIdentificacion(string tipoId, string identificacion)
        {
            return PGPagosGiros.Instancia.ConsultarGiroXIdentificacion(tipoId, identificacion);
        }

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// y por la ciudad de donde se realiza la consulta
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        /// <param name="idCiudadConsulta"></param>
        /// <returns>Lista de Giros por la ciudad consultada</returns>
        public List<GIAdmisionGirosDC> ConsultarGiroXIdentificacionCiudad(string tipoId, string identificacion, string idCiudadConsulta)
        {
            return PGPagosGiros.Instancia.ConsultarGiroXIdentificacionCiudad(tipoId, identificacion, idCiudadConsulta);
        }

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// y por la ciudad de donde se realiza la consulta
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        /// <param name="idCiudadConsulta"></param>
        /// <returns>Lista de Giros por la ciudad consultada</returns>
        public List<GIAdmisionGirosDC> ConsultarGiroXIdentificacionCentroServiciosDestino(string tipoId, string identificacion, long idCentroServicio)
        {
            return PGPagosGiros.Instancia.ConsultarGiroXIdentificacionCentroServiciosDestino(tipoId, identificacion, idCentroServicio);
        }


        /// <summary>
        /// Consulta la informacion de un pago
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public PGPagosGirosDC ConsultarInformacionPago(long idAdmisionGiro)
        {
            return PGPagosGiros.Instancia.ConsultarInformacionPago(idAdmisionGiro);
        }

        #endregion Consultas de Giros a pagar

        #region Realizar el proceso de pago

        /// <summary>
        /// Valida que el giro este disponible para pagar, no tenga solicitudes
        /// </summary>
        public void ValidacionPago(long idAdmisiongiro)
        {
            PGPagosGiros.Instancia.ValidacionPago(idAdmisiongiro);
        }

        /// <summary>
        /// Consulta si el pago se realizo exitosamente
        /// </summary>
        /// <param name="idAdmisiongiro"></param>
        public void ConsultarPago(long idAdmisiongiro)
        {
            PGPagosGiros.Instancia.ConsultarPago(idAdmisiongiro);
        }

        /// <summary>
        /// Realiza el pago del giro
        /// </summary>
        public PGComprobantePagoDC PagarGiro(PGPagosGirosDC pagosGiros)
        {
            return PGPagosGiros.Instancia.PagarGiro(pagosGiros);
        }

        #endregion Realizar el proceso de pago

        #region Crear Solicitudes

        /// <summary>
        /// Crear solicitud de giros
        /// </summary>
        public GISolicitudGiroDC CrearSolicitud(GIEnumMotivoSolicitudDC motivo, GISolicitdPagoDC solicitud)
        {
            return PGPagosGiros.Instancia.CrearSolicitud(motivo, solicitud);
        }

        #endregion Crear Solicitudes

        #endregion Pagos

        #region Giros Convenio

        /// <summary>
        /// Obtiene el valor total,servicio,tarifas de un giro dirigido a un cliente contado a partir de un contrato
        /// </summary>
        /// <returns>Precio</returns>
        public TAPrecioDC ObtenerValorGiroClienteContadoGiro(int idContrato, decimal valor)
        {
            return GIAdministracionGiros.Instancia.ObtenerValorGiroClienteContadoGiro(idContrato, valor);
        }

        /// <summary>
        ///  Obtiene el valor a cobrar a un servicio segun su  valor total.
        /// </summary>
        public TAPrecioDC CalcularValorServicoAPartirValorTotal(int idContrato, decimal valorTotal)
        {
            return GIAdministracionGiros.Instancia.CalcularValorServicoAPartirValorTotal(idContrato, valorTotal);
        }

        /// <summary>
        /// Obtener los impuestos de el servicio de giros
        /// </summary>
        /// <returns></returns>
        public TAServicioImpuestosDC ObtenerImpuestosGiros()
        {
            return GIAdministracionGiros.Instancia.ObtenerImpuestosGiros();
        }

        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerClientesContratosGiros()
        {
            return GIAdministracionGiros.Instancia.ObtenerClientesContratosGiros();
        }

        /// <summary>
        /// Guardar los giros peaton convenio.
        /// </summary>
        public GINumeroGiro GuardarGiroPeatonConvenio(GIAdmisionGirosDC giro)
        {
            return GIAdministracionGiros.Instancia.GuardarGiroPeatonConvenio(giro);
        }

        #endregion Giros Convenio

        #region ParametrosGiros

        /// <summary>
        /// Consulta la tabla Parametros Giros
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        public GIParametrosGirosDC ConsultarParametrosGiros(string idParametro)
        {
            return GIAdministracionGiros.Instancia.ConsultarParametrosGiros(idParametro);
        }
        

        #endregion
    }
}