using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using System.ServiceModel;
using System.Collections.Generic;

namespace CO.Servidor.Servicios.Contratos
{
    /// <summary>
    /// Contratos WCF de venta de giros
    /// </summary>
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IGIAdmisionesGirosSvc
    {

        #region Venta Giros

        /// <summary>
        /// Consulta la agencia a la cual se le suministro la factura de venta, con el numero de giro IdGiro
        /// </summary>
        /// <param name="IdGiro">Numero del giro</param>
        /// <returns>Centro de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUCentroServiciosDC ObtenerAgenciaPropietariaDelNumeroGiro(long idGiro, long idCentroServicio, bool esUsuarioRacol, long idRacol);

        /// <summary>
        /// Creacion de un giro
        /// </summary>
        /// <param name="giro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GINumeroGiro CrearGiro(GIAdmisionGirosDC giro);

        /// <summary>
        /// Consulta la informacion de un giro a partir de el guid
        /// </summary>
        /// <param name="GuidDeChequeo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GINumeroGiro ConsultarGiroPorGuid(string guidDeChequeo);

        /// <summary>
        /// consulta los giros activos realizados el dia actual
        /// No retorna todos los valores del giro
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgencia(long idCentroServicio, long idGiro, string tipoIdRemitente, string identificacionRemitente, string tipoIdDestinatario, string identificacionDestinatario, string estadoGiro, int indicePagina, int registrosPorPagina, string tipoCentroServicio);

        /// <summary>
        /// consulta los giros activos realizados el dia actual Peaton convenio
        /// No retorna todos los valores del giro
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIAdmisionGirosDC> ConsultarGirosPeatonConvenioPorAgencia(long idCentroServicio, long idGiro, string tipoIdRemitente, string identificacionRemitente, string tipoIdDestinatario, string identificacionDestinatario, string tipoDocumentoGiro, int indicePagina, int registrosPorPagina, string tipoCentroServicio);


        // TODO:ID Consulta de Giros para Integracion 742
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIAdmisionGirosDC> ConsultarGirosPPEIntegracionSieteCuatroDos(string pEstado);



        /// <summary>
        /// Consultar la informacion de la tabla peaton peaton
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ConsultarInformacionPeatonPeaton(long idAdmisionGiro);

        /// <summary>
        /// Consultar la informacion de la tabla peaton Convenio
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ConsultarInformacionPeatonConvenio(long idAdmisionGiro);

        /// <summary>
        /// Valida que una agencia pueda realizar la venta de un giro
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ValidarAgenciaServicioGiros(long idCentroServicios);

        /// <summary>
        /// Obtiene informacion inicial del peaton inicial
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ConsultarInformacionPeatonPeatonInicial(long idAdmisionGiro);

        /// <summary>
        /// Obtiene informacion inical del peaton convenio
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ConsultarInformacionPeatonConvenioInicial(long idAdmisionGiro);

        /// <summary>
        /// Consulta la tabla Parametros Giros
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIParametrosGirosDC ConsultarParametrosGiros(string idParametro);

        #endregion Venta Giros

        #region Pagos

        #region Consultas de Giros a pagar

        /// <summary>
        /// Consultar la cantidad de pagos y la sumatoria total de los mismos
        /// por agencia
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PGTotalPagosDC ConsultarPagosAgencia(long idCentroServicio);

        /// <summary>
        /// Consultar el giro por numero de giro
        /// </summary>
        /// <param name="idGiro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ConsultarGiroXNumGiro(long idGiro);

        /// <summary>
        /// Consultar el giro por numero de giro y la ciudad de destino
        /// </summary>
        /// <param name="idGiro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ConsultarGiroNumGiroCiudadDestino(long idGiro, string idCiudadDestino);


        /// <summary>
        /// Consultar el giro por numero de giro
        /// y centro Servicio destino
        /// </summary>
        /// <param name="idGiro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ConsultarGiroXNumGiroCentroServicio(long idGiro, long idCentroSvc);

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIAdmisionGirosDC> ConsultarGiroXIdentificacion(string tipoId, string identificacion);

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// y por la ciudad de donde se realiza la consulta
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        /// <param name="idCiudadConsulta"></param>
        /// <returns>Lista de Giros por la ciudad consultada</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIAdmisionGirosDC> ConsultarGiroXIdentificacionCiudad(string tipoId, string identificacion, string idCiudadConsulta);

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// y por la ciudad de donde se realiza la consulta
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        /// <param name="idCiudadConsulta"></param>
        /// <returns>Lista de Giros por la ciudad consultada</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIAdmisionGirosDC> ConsultarGiroXIdentificacionCentroServiciosDestino(string tipoId, string identificacion, long idCentroServicio);



        /// <summary>
        /// Consulta la informacion de un pago
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PGPagosGirosDC ConsultarInformacionPago(long idAdmisionGiro);

        #endregion Consultas de Giros a pagar

        #region Realizar el proceso de pago

        /// <summary>
        /// Valida que el giro este disponible para pagar, no tenga solicitudes
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ValidacionPago(long idAdmisiongiro);

        /// <summary>
        /// Consulta si el pago se realizo exitosamente
        /// </summary>
        /// <param name="idAdmisiongiro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ConsultarPago(long idAdmisiongiro);

        /// <summary>
        /// Realiza el pago del giro
        /// </summary>
        /// <param name="idAdmisiongiro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PGComprobantePagoDC PagarGiro(PGPagosGirosDC pagosGiros);

        #endregion Realizar el proceso de pago

        #region Crear Solicitudes

        /// <summary>
        /// Crear solicitud de giros cambio de agencia
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GISolicitudGiroDC CrearSolicitud(GIEnumMotivoSolicitudDC motivo, GISolicitdPagoDC solicitud);

        #endregion Crear Solicitudes

        #endregion Pagos

        #region Giros Convenio

        /// <summary>
        /// Obtiene el valor total,servicio,tarifas de un giro dirigido a un cliente contado a partir de un contrato
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioDC ObtenerValorGiroClienteContadoGiro(int idContrato, decimal valor);

        /// <summary>
        ///  Obtiene el valor a cobrar a un servicio segun su  valor total.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioDC CalcularValorServicoAPartirValorTotal(int idContrato, decimal valorTotal);

        /// <summary>
        /// Obtener los impuestos de el servicio de giros
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAServicioImpuestosDC ObtenerImpuestosGiros();

        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClientesDC> ObtenerClientesContratosGiros();

        /// <summary>
        /// Guardar los giros convenio.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GINumeroGiro GuardarGiroPeatonConvenio(GIAdmisionGirosDC giro);

        #endregion Giros Convenio
    }
}