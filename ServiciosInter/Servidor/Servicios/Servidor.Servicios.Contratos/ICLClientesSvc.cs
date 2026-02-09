using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ICLClientesSvc
    {
        #region Metodos basicos

        /// <summary>
        /// Obtiene una lista con los clientes para filtrar
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Tipos de envío</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLClientesDC> ObtenerClientesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Metodo para la manipulacion de clientes de acuerdo al estado del registro
        /// </summary>
        /// <param name="cliente"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ModificarCliente(CLClientesDC cliente);

        /// <summary>
        /// Obtiene las sucursales del cliente por racol
        /// </summary>
        /// <param name="idRacol">Id del racol</param>
        /// <param name="idCliente">id del cliente</param>
        /// <returns>Lista de las sucursales del cliente y del racol</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLSucursalDC> ObtenerSucursalesClienteRacol(long idRacol, long idCliente);

        /// <summary>
        /// Obtiene una lista de los clientes crédito de una localidad
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLClientesDC> ObtenerClientesLocalidad(string idLocalidad);

        #endregion Metodos basicos

        #region EstadosCliente

        /// <summary>
        /// Obtiene una lista con los estados de un cliente
        /// </summary>
        /// <returns>objeto lista de los estados de un cliente</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLEstadosClienteDC> ObtenerEstadosCliente(CLClientesDC cliente);

        /// <summary>
        /// Obtiene lista con los estados
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<EstadoDC> ObtenerEstados();

        /// <summary>
        /// Obtiene lista con los estados y motivos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLMotivoEstadosDC> ObtenerMotivosEstados();


        #endregion EstadosCliente

        #region Archivos

        /// <summary>
        /// Obtiene lista con los archivos de un cliente
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLArchivosDC> ObtenerArchivos(CLClientesDC cliente);

        /// <summary>
        ///Metodo para obtener un archivo asociado a un cliente
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerArchivo(CLArchivosDC archivo);

        #endregion Archivos

        #region Divulgación de cliente

        /// <summary>
        /// Envia la divulgacion de una agencia
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="destinatarios">Diccionario con la informacion de los destinatarios key=Email Value = NombreDestinatario</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void DivulgarCliente(CLContratosDC contrato, PADivulgacion divulgacion);

        #endregion Divulgación de cliente

        #region Sucursales

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <param name="totalRegistros">Id del cliente a filtrar</param>
        /// <returns>Tipos de envío</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLSucursalDC> ObtenerSucursalesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCliente);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLContratosDC> ObtenerContratosActivos_ClienteCredito(long idAgencia, int idCliente);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLSucursalContratoDC> ObtenerSucursalesContrato_CliCredito(long IdAgencia, int IdContrato);


        /// <summary>
        /// Metodo para modificar sucursales en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo sucursal</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ModificarSucursales(CLSucursalDC sucursal);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLSucursalDC> ObtenerSucursalesActivasCliente(int idClienteCredito);

        #endregion Sucursales

        #region Tipo de guia de la sucursal

        /// <summary>
        /// Obtiene el tipo de guia por sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CLSucursalGuiasDC ObtenerGuiaPorSucursal(int idSucursal);

        /// <summary>
        /// guarda los cambios de guia por sucursal
        /// </summary>
        /// <param name="guiaSucursal"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarCambiosGuiaPorSucursal(CLSucursalGuiasDC guiaSucursal);

        #endregion Tipo de guia de la sucursal

        #region Contratos

        /// <summary>
        /// Obtiene una lista con los contratos para filtrar a partir de una identificacion de un cliente
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <param name="totalRegistros">Id del cliente a filtrar</param>
        /// <returns>Tipos de envío</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLContratosDC> ObtenerContratosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCliente);

        /// <summary>
        /// Metodo para modificar contratos en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo contrato</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ModificarContrato(CLContratosDC contrato);

        /// <summary>
        /// Obtiene los contratos activos por cliente
        /// </summary>
        /// <param name="idCliente">lista con los contratios activos por cliente</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLContratosDC> ObtenerContratosActivos(int idCliente);

        /// <summary>
        /// Obtiene la lista de precios asociada a un contrato
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerListaPrecioContrato(int idContrato);

        #endregion Contratos

        #region Personal del contrato

        /// <summary>
        /// OObtiene las personas asociadas a un contrato
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PAPersonaInternaDC> ObtenerPersonalContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato);

        /// <summary>
        /// Metodo para modificar personal de un contrato
        /// </summary>
        /// <param name="persona"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ModificarPersonalContrato(PAPersonaInternaDC persona);

        #endregion Personal del contrato

        #region Contactos del contrato

        /// <summary>
        /// Obtiene loc contactos asociadas a un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLContactosDC> ObtenerContactosContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato);

        /// <summary>
        /// Operaciones sobre los contactos
        /// </summary>
        /// <param name="contacto">objeto de tipo contacto</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarCambiosContactos(CLContactosDC contacto);

        #endregion Contactos del contrato

        #region Deducciones del contrato

        /// <summary>
        /// Obtiene las deducciones asociadas a un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLDeduccionesContratoDC> ObtenerDeduccionesContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato);

        /// <summary>
        /// Operaciones sobre las deducciones de un contrato
        /// </summary>
        /// <param name="deduccion">objeto de tipo deduccion</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarCambiosDeducciones(CLDeduccionesContratoDC deduccion);

        #endregion Deducciones del contrato

        #region Otrosi del contrato

        /// <summary>
        /// obtiene los otro si de un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLOtroSiDC> ObtenerOtroSiContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato);

        /// <summary>
        /// Lista con los tipos de otrosi de un contrato
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLTipoOtroSiDC> ObtenerListaTiposOtrosi();

        /// <summary>
        /// Metodo encargado de guardar cambio de un otros si de un contrato
        /// </summary>
        /// <param name="otrosi"></param>am>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarCambiosOtroSi(CLOtroSiDC otrosi);

        #endregion Otrosi del contrato

        #region Archivos de contrato

        /// <summary>
        /// Obtiene lista con los archivos de un contrato
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLContratosArchivosDC> ObtenerArchivosContrato(CLContratosDC contrato);

        /// <summary>
        /// Adiciona o elimina los archivos de un contrato
        /// </summary>
        /// <param name="archivos">objeto de tipo lista con los archivos de un contrato</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void OperacionesArchivosContrato(IEnumerable<CLContratosArchivosDC> archivos);

        /// <summary>
        ///Metodo para obtener un archivo asociado a un contrato
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerArchivoContrato(CLContratosArchivosDC archivo);

        #endregion Archivos de contrato

        #region sucursales del contrato

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un contrato de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLSucursalContratoDC> ObtenerSucursalesContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato);

        /// <summary>
        /// Metodo para guardar cambios de las sucursales de un contrato
        /// </summary>
        /// <param name="sucursal"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarCambiosSucursalesContrato(CLSucursalContratoDC sucursal);

        /// <summary>
        /// Obtiene lista con los estados de una sucursal
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLSucursalEstadosDC> ObtenerSucursalEstados(int Idcontrato);

        /// <summary>
        /// Obtiene una lista con las sucursales de un cliente que no esten en un contrato especifico
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLSucursalDC> ObtenerSucursalesFiltroExcepcion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, int idContrato, int idCliente);

        #endregion sucursales del contrato

        #region Facturas

        /// <summary>
        /// Metodo encargado de traer los tipos de notacion para la factura
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLTipoNotacionDC> ObtenerTipoNotacion();

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un contrato de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLFacturaDC> ObtenerFacturasContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato);

        /// <summary>
        /// Metodo para guardar cambios de una factura de una contrato
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int GuardarCambiosFacturas(CLFacturaDC factura);

        #endregion Facturas

        #region Requisitos de la factura

        /// <summary>
        /// Obtiene una lista con los requisitos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los requisitos configuradas en la base de datos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLFacturaRequisitosDC> ObtenerRequisitosFacturas(int idFactura);

        /// <summary>
        /// Metodo para operaciones sobre requisitos de una factura
        /// </summary>
        /// <param name="requisito"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarRequisitosFacturas(CLFacturaRequisitosDC requisito);

        #endregion Requisitos de la factura

        #region Descuentos de la factura

        /// <summary>
        /// Obtiene una lista con los descuentos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los descuentos configuradas en la base de datos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLFacturaDescuentoDC> ObtenerDescuentosFacturas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idFactura);

        /// <summary>
        /// Metodo para guardar cambios descuentos a una factura
        /// </summary>
        /// <param name="descuento"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarCambiosDescuentoFactura(CLFacturaDescuentoDC descuento);

        #endregion Descuentos de la factura

        #region Servicios de la factura

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLFacturaServiciosDC> ObtenerServiciosFactura(CLFacturaDC factura);

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un servicio de un contrato
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLSucursalServicioDC> ObtenerSucursalesServicioFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, CLFacturaServiciosDC servicioFactura);

        /// <summary>
        /// Metodo para guardar cambios de Sucursales por servicio
        /// </summary>
        /// <param name="servicioSucursal"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarCambiosServiciosSucursales(CLFacturaServiciosDC servicioSucursal);

        /// <summary>
        /// Método para obtener las sucursales excluidas de una factura
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="servicioFactura"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLSucursalServicioDC> ObtenerSucursalesServicioExcluidosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, CLFacturaServiciosDC servicioFactura);

        #endregion Servicios de la factura

        #region ClienteContado

        /// <summary>
        /// Consulta la informacion de un cliente Contado a partir de un tipo de documento y un numero de documento
        /// </summary>
        /// <param name="tipoDocumento">Tipo de documento del cliente a consultar</param>
        /// <param name="numeroDocumento">Numéro del documento del cliente a consultar </param>
        /// <returns>Cliente Contado</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CLClienteContadoDC ConsultarClienteContado(string tipoDocumento, string numeroDocumento, bool conDestinatariosFrecuentes, string idMunicipioDestino);

        /// <summary>
        /// Enviar correo electronico cuando un cliente se encuentre en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EnviarCorreoListasRestrictivas(string identificacion, long idCentroServicios, string nombreCentroServicios);

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PATipoIdentificacion> ConsultarTiposIdentificacionReclamaGiros();

        #endregion ClienteContado

        #region Consultas

        /// <summary>
        /// Consulta la información de un cliente a partir de su número telefónico
        /// </summary>
        /// <param name="numTelefono"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClienteContadoDC> ConsultarClienteContadoPorTelefono(string numTelefono);

        /// <summary>
        /// Retorna los servicios asignados a la sucursal por el contrato
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal del cliente</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLServiciosSucursal> ObtenerServiciosSucursal(int idSucursal, int idListaPrecios);

        /// <summary>
        /// Valida que un cliente crédito pueda realizar venta de servicios y retorna  la lista de servicios habilitados.
        /// Para aquellos servicios que aparecen en más de un contrato, coge el primer contrato que se encuentre
        /// </summary>
        /// <param name="idSucursal">Identificador del centro de servicios</param>
        /// <returns>Lista de servicios ofrecidos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLServiciosSucursal> ObtenerServiciosSucursalSinValidarListaPrecios(int idSucursal);

        /// <summary>
        /// Consulta información de un cliente por su id
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CLClientesDC ObtenerCliente(int idCliente);

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLClientesDC> ObtenerClientes();

        /// <summary>
        /// Obtiene una lista de los clientes crédito de una agencia específica
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLClientesDC> ObtenerClientesxAgencia(int idAgencia);



        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClientesDC> ObtenerClientesCreditoXAgencia(long idAgencia);


        /// <summary>
        /// Trae el listado de los clientes crédito y sus respectivos contratos por una agencia y todas las agencias dependientes de la misma
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClientesDC> ObtenerCLientesContratosXAgenciaDependientes(long idAgencia);

        /// <summary>
        /// Retorna información del cliente crédito, si se requiere retornar sucursal y contrato se toma la primera sucursal activa y el primer contrato vigente de dicha sucursal
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="requiereSucursalContrato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CLClienteCreditoSucursalContrato ObtenerInformacionClienteCredito(int idCliente, bool requiereSucursalContrato);

        /// <summary>
        /// Retorna información del cliente crédito,
        /// retornar las  sucursal y
        /// los contrato
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="requiereSucursalContrato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CLClienteCreditoSucursalContrato ObtenerInfoClienteCreditoContratos(int idCliente);

        /// <summary>
        /// Obtiene una lista de los clientes crédito que van a ser usados para selecciónd e sucursal y contrato
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLClienteCreditoSucursalContrato> ObtenerClientesCredito();

        /// <summary>
        /// Obtiene los clientes y las sucursales con certificaciones de entrega pendientes
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClienteCreditoSucursalContrato> ObtenerClientesSucursalesCertificacionesEntregaPendientes(long idCol);
        /// <summary>
        /// Obtiene los clientes y las sucursales con certificaciones de devolucion pendientes
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClienteCreditoSucursalContrato> ObtenerClientesSucursalesCertificacionesDevolucionPendientes(long idCol);

        /// <summary>
        /// Retorna un contrato dado su ID
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CLContratosDC ObtenerContrato(int idContrato);

        /// <summary>
        /// Retorna la lista de clientes crédito que aplican a admisión peatón - convenio dadas la ciudad de origen y destino del envío. 
        /// Retorna las sucursales que tienen contratos vigentes para la ciudad de destino y valida que la ciudad de origen sea permitida 
        /// para el cliente crédito dado. Además valida que la lista de precios asociada al contrato esté vigente.  
        /// </summary>
        /// <param name="idCiudadOrigen">Id de la ciudad de origen</param>
        /// <param name="idCiudadDestino">Id de la ciudad de destino</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClienteSucConvenioNal> ObtenerClientesAplicanConvenio(string idCiudadOrigen, string idCiudadDestino);

        /// <summary>
        /// Obtiene una lista de todos los clientes crédito tienen convenio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLClientesDC> ObtenerTodosClientesConvenio();
        #endregion Consultas

        #region Validaciones

        /// <summary>
        /// Valida si una identificacion se encuentra en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ValidarNitExistente(string identificacion);

        /// <summary>
        /// Valida que el Nit exista y no se encuentre ne la lista restrictiva
        /// </summary>
        /// <param name="Nit"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ValidarNit(string Nit);



        #endregion Validaciones

        #region Referencia uso guia

        /// <summary>
        /// Inserta Modifica o elimina una  referencia uso guia interna
        /// </summary>
        /// <param name="referencia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarReferenciaUsoGuia(CLReferenciaUsoGuiaDC referencia);

        /// <summary>
        /// Obtiene  las referencias de uso de una guia interna
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idTipoMensajero">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con los conductores y auxiliares en un objeto tipo mensajero</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<CLReferenciaUsoGuiaDC> ObtenerReferenciaUsoGuia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Obtiene la referencia de uso guia por sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CLReferenciaUsoGuiaDC ObtenerReferenciaUsoGuiaPorSucursal(int idSucursal);

        #endregion Referencia uso guia

        #region Sucursal Suministro

        /// <summary>
        /// Obtiene las sucursales activas existentes en el sistema
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLSucursalDC> ObtenerSucursalesActivas();

        /// <summary>
        /// Obtiene los grupos de suministros configurados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<SUSuministroSucursalDC> ObtenerSucursalesSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idSucursal);

        /// <summary>
        /// agrega o modifica un suministro de una sucursal
        /// </summary>
        /// <param name="sumSuc"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarSuministroSucursal(List<SUSuministroSucursalDC> sumSuc);

        #endregion Sucursal Suministro

        #region Cliente convenio

        /// <summary>
        /// Método para obtener las localidades origen de un cliente convenio
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLConvenioLocalidadDC> ObtenerLocalidadesConvenio(int idCliente);

        #endregion

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLTipoClienteCreditoDC> ObtenerTipoClienteCredito();

    }
}