using CO.Servidor.Clientes.Datos;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CO.Servidor.Clientes
{
    /// <summary>
    /// Clase de fachada para clientes
    /// </summary>
    public class CLAdministradorClientes
    {
        #region Declaraciones

        private static readonly CLAdministradorClientes instancia = new CLAdministradorClientes();

        /// <summary>
        /// Retorna una instancia del administrador de clientes
        /// </summary>
        public static CLAdministradorClientes Instancia
        {
            get { return CLAdministradorClientes.instancia; }
        }

        private CLConfiguradorClientes NegocioClientes;

        private CLAdministradorClientes()
        {
            ValidarInstancia();
        }

        /// <summary>
        /// Metodo para validar la creacion de la instancia del objeto de la clase de negocio
        /// </summary>
        private void ValidarInstancia()
        {
            if (NegocioClientes == null)
            {
                NegocioClientes = CLConfiguradorClientes.Instancia;
            }
        }

        #endregion Declaraciones

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
        public IEnumerable<CLClientesDC> ObtenerClientesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return NegocioClientes.ObtenerClientesFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Metodo para la manipulacion de clientes de acuerdo al estado del registro
        /// </summary>
        /// <param name="cliente"></param>
        public void ModificarCliente(CLClientesDC cliente)
        {
            NegocioClientes.ModificarCliente(cliente);
        }

        /// <summary>
        /// Obtiene las sucursales del cliente por racol
        /// </summary>
        /// <param name="idRacol">Id del racol</param>
        /// <param name="idCliente">id del cliente</param>
        /// <returns>Lista de las sucursales del cliente y del racol</returns>
        public List<CLSucursalDC> ObtenerSucursalesClienteRacol(long idRacol, long idCliente)
        {
            return CLConfiguradorClientes.Instancia.ObtenerSucursalesClienteRacol(idRacol, idCliente);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito de una localidad
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientesLocalidad(string idLocalidad)
        {
            return CLConfiguradorClientes.Instancia.ObtenerClientesLocalidad(idLocalidad);
        }

        #endregion Metodos basicos

        #region EstadosCliente

        /// <summary>
        /// Obtiene lista con los estados de un cliente
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLEstadosClienteDC> ObtenerEstadosCliente(CLClientesDC cliente)
        {
            return NegocioClientes.ObtenerEstadosCliente(cliente);
        }

        /// <summary>
        /// Obtiene lista con los estados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EstadoDC> ObtenerEstados()
        {
            return NegocioClientes.ObtenerEstados();
        }

        /// <summary>
        /// Obtiene lista con los estados y motivos
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLMotivoEstadosDC> ObtenerMotivosEstados()
        {
            return NegocioClientes.ObtenerMotivosEstados();
        }




        #endregion EstadosCliente

        #region Archivos cliente

        /// <summary>
        /// Obtiene lista con los archivos de un cliente
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLArchivosDC> ObtenerArchivosCliente(CLClientesDC cliente)
        {
            return NegocioClientes.ObtenerArchivosCliente(cliente);
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un cliente
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoCliente(CLArchivosDC archivo)
        {
            return NegocioClientes.ObtenerArchivoCliente(archivo);
        }

        #endregion Archivos cliente

        #region Divulgación de cliente

        /// <summary>
        /// Envia la divulgacion de una agencia
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="destinatarios">Diccionario con la informacion de los destinatarios key=Email Value = NombreDestinatario</param>
        public void DivulgarCliente(CLContratosDC contrato, PADivulgacion divulgacion)
        {
            NegocioClientes.DivulgarCliente(contrato, divulgacion);
        }

        #endregion Divulgación de cliente

        #region Sucursales

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un nit de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalDC> ObtenerSucursalesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idCliente)
        {
            return NegocioClientes.ObtenerSucursalesFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCliente);
        }

        /// <summary>
        /// Metodo para modificar sucursales en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo sucursal</param>
        public int ModificarSucursal(CLSucursalDC sucursal)
        {
            return NegocioClientes.ModificarSucursal(sucursal);
        }

        /// <summary>
        /// Obtiene las sucursales activas de un cliente
        /// </summary>
        /// <param name="idClienteCredito"></param>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesActivasCliente(int idClienteCredito)
        {
            return CLClienteCredito.Instancia.ObtenerSucursalesActivasCliente(idClienteCredito);
        }


        #endregion Sucursales

        #region Tipo de guia de la sucursal

        /// <summary>
        /// Obtiene el tipo de guia por sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public CLSucursalGuiasDC ObtenerGuiaPorSucursal(int idSucursal)
        {
            return NegocioClientes.ObtenerGuiaPorSucursal(idSucursal);
        }

        /// <summary>
        /// guarda los cambios de guia por sucursal
        /// </summary>
        /// <param name="guiaSucursal"></param>
        public void GuardarCambiosGuiaPorSucursal(CLSucursalGuiasDC guiaSucursal)
        {
            NegocioClientes.GuardarCambiosGuiaPorSucursal(guiaSucursal);
        }

        #endregion Tipo de guia de la sucursal

        #region Contratos

        /// <summary>
        /// Obtiene una lista con los contratos para filtrar a partir de una identificacion de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public List<CLContratosDC> ObtenerContratosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCliente)
        {
            return NegocioClientes.ObtenerContratosFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCliente);
        }

        /// <summary>
        /// Metodo para modificar contratos en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo contrato</param>
        public int ModificarContrato(CLContratosDC contrato)
        {
            return NegocioClientes.ModificarContrato(contrato);
        }

        /// <summary>
        /// Obtiene los contratos activos por cliente
        /// </summary>
        /// <param name="idCliente">lista con los contratios activos por cliente</param>
        /// <returns></returns>
        public IEnumerable<CLContratosDC> ObtenerContratosActivos(int idCliente)
        {
            return NegocioClientes.ObtenerContratosActivos(idCliente);
        }

        /// <summary>
        /// Consultar una lista de precios a partir del id del contrato
        /// </summary>
        /// <param name="idContrato">id contrato</param>
        /// <returns>Lista Orecios</returns>
        public int ObtenerListaPrecioContrato(int idContrato)
        {
            return CLContrato.Instancia.ObtenerListaPrecioContrato(idContrato);
        }

        #endregion Contratos

        #region Personal del contrato

        /// <summary>
        /// Obtiene las personas asociadas a un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public IEnumerable<PAPersonaInternaDC> ObtenerPersonalContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return NegocioClientes.ObtenerPersonalContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        /// <summary>
        /// Metodo para modificar personal de un contrato
        /// </summary>
        /// <param name="persona"></param>
        /// <returns></returns>
        public void ModificarPersonalContrato(PAPersonaInternaDC persona)
        {
            NegocioClientes.ModificarPersonalContrato(persona);
        }

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
        public IEnumerable<CLContactosDC> ObtenerContactosContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return NegocioClientes.ObtenerContactosContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        /// <summary>
        /// Operaciones sobre los contactos
        /// </summary>
        /// <param name="contacto">objeto de tipo contacto</param>
        public void GuardarCambiosContactos(CLContactosDC contacto)
        {
            NegocioClientes.GuardarCambiosContactos(contacto);
        }

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
        public IEnumerable<CLDeduccionesContratoDC> ObtenerDeduccionesContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return NegocioClientes.ObtenerDeduccionesContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        /// <summary>
        /// Operaciones sobre las deducciones de un contrato
        /// </summary>
        /// <param name="deduccion">objeto de tipo deduccion</param>
        public void GuardarCambiosDeducciones(CLDeduccionesContratoDC deduccion)
        {
            NegocioClientes.GuardarCambiosDeducciones(deduccion);
        }

        #endregion Deducciones del contrato

        #region otrosi del contrato

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
        public IEnumerable<CLOtroSiDC> ObtenerOtroSiContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return NegocioClientes.ObtenerOtroSiContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        /// <summary>
        /// Lista con los tipos de otrosi de un contrato
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLTipoOtroSiDC> ObtenerListaTiposOtrosi()
        {
            return NegocioClientes.ObtenerListaTiposOtrosi();
        }

        /// <summary>
        /// Metodo encargado de guardar cambio de un otros si de un contrato
        /// </summary>
        /// <param name="otrosi"></param>
        public void GuardarCambiosOtroSi(CLOtroSiDC otrosi)
        {
            NegocioClientes.GuardarCambiosOtroSi(otrosi);
        }

        #endregion otrosi del contrato

        #region Archivos contrato

        /// <summary>
        /// Obtiene lista con los archivos de un contrato
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLContratosArchivosDC> ObtenerArchivosContrato(CLContratosDC contrato)
        {
            return NegocioClientes.ObtenerArchivosContrato(contrato);
        }

        /// <summary>
        /// Adiciona o elimina los archivos de un contrato
        /// </summary>
        /// <param name="archivos">objeto de tipo lista con los archivos de un contrato</param>
        public void OperacionesArchivosContrato(IEnumerable<CLContratosArchivosDC> archivos)
        {
            NegocioClientes.OperacionesArchivosContrato(archivos);
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un contrato
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoContrato(CLContratosArchivosDC archivo)
        {
            return NegocioClientes.ObtenerArchivoContrato(archivo);
        }

        #endregion Archivos contrato

        #region sucursales del contrato

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un contrato de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalContratoDC> ObtenerSucursalesContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return NegocioClientes.ObtenerSucursalesContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        public List<CLContratosDC> ObtenerContratosActivos_ClienteCredito(long idAgencia, int idCliente)
        {
            return NegocioClientes.ObtenerContratosActivos_ClienteCredito(idAgencia, idCliente);
        }

        public List<CLSucursalContratoDC> ObtenerSucursalesContrato_CliCredito(long IdAgencia, int IdContrato)
        {
            return NegocioClientes.ObtenerSucursalesContrato_CliCredito(IdAgencia, IdContrato);
        }


        /// <summary>
        /// Metodo para guardar cambios de las sucursales de un contrato
        /// </summary>
        /// <param name="sucursal"></param>
        public void GuardarCambiosSucursalesContrato(CLSucursalContratoDC sucursal)
        {
            NegocioClientes.GuardarCambiosSucursalesContrato(sucursal);
        }

        /// <summary>
        /// Obtiene lista con los estados de una sucursal
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLSucursalEstadosDC> ObtenerSucursalEstados(int Idcontrato)
        {
            return NegocioClientes.ObtenerSucursalEstados(Idcontrato);
        }

        /// <summary>
        /// Obtiene una lista con las sucursales de un cliente que no esten en un contrato especifico
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalDC> ObtenerSucursalesFiltroExcepcion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, int idContrato, int idCliente)
        {
            return NegocioClientes.ObtenerSucursalesFiltroExcepcion(filtro, indicePagina, registrosPorPagina, idContrato, idCliente);
        }

        /// <summary>
        /// Obtiene la lista de los contratos de las
        /// sucursales Activas
        /// </summary>
        /// <returns>lista de Contratos</returns>
        public List<CLContratosDC> ObtenerContratosActivosDeSucursales()
        {
            return CLClienteCredito.Instancia.ObtenerContratosActivosDeSucursales();
        }

        #endregion sucursales del contrato

        #region Facturas

        /// <summary>
        /// Metodo encargado de traer los tipos de notacion para la factura
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLTipoNotacionDC> ObtenerTipoNotacion()
        {
            return NegocioClientes.ObtenerTipoNotacion();
        }

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un contrato de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLFacturaDC> ObtenerFacturasContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return NegocioClientes.ObtenerFacturasContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        /// <summary>
        /// Metodo para guardar cambios de una factura de una contrato
        /// </summary>
        /// <param name="?"></param>
        public int GuardarCambiosFacturas(CLFacturaDC factura)
        {
            return NegocioClientes.GuardarCambiosFacturas(factura);
        }

        #endregion Facturas

        #region Requisitos de la factura

        /// <summary>
        /// Obtiene una lista con los requisitos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los requisitos configuradas en la base de datos</returns>
        public IEnumerable<CLFacturaRequisitosDC> ObtenerRequisitosFacturas(int idFactura)
        {
            return NegocioClientes.ObtenerRequisitosFacturas(idFactura);
        }

        /// <summary>
        /// Metodo para operaciones sobre requisitos de una factura
        /// </summary>
        /// <param name="requisito"></param>
        public void GuardarRequisitosFacturas(CLFacturaRequisitosDC requisito)
        {
            NegocioClientes.GuardarRequisitosFacturas(requisito);
        }

        #endregion Requisitos de la factura

        #region Descuentos de la factura

        /// <summary>
        /// Obtiene una lista con los descuentos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los descuentos configuradas en la base de datos</returns>
        public IEnumerable<CLFacturaDescuentoDC> ObtenerDescuentosFacturas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idFactura)
        {
            return NegocioClientes.ObtenerDescuentosFacturas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idFactura);
        }

        /// <summary>
        /// Metodo para guardar cambios descuentos a una factura
        /// </summary>
        /// <param name="descuento"></param>
        public void GuardarCambiosDescuentoFactura(CLFacturaDescuentoDC descuento)
        {
            NegocioClientes.GuardarCambiosDescuentoFactura(descuento);
        }

        #endregion Descuentos de la factura

        #region Servicios de la factura

        /// <summary>
        /// Obtiene los servicios de una factura
        /// </summary>
        /// <param name="factura"></param>
        /// <returns></returns>
        public IEnumerable<CLFacturaServiciosDC> ObtenerServiciosFactura(CLFacturaDC factura)
        {
            return NegocioClientes.ObtenerServiciosFactura(factura);
        }

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un servicio de un contrato
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalServicioDC> ObtenerSucursalesServicioFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, CLFacturaServiciosDC servicioFactura)
        {
            return NegocioClientes.ObtenerSucursalesServicioFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, servicioFactura);
        }

        /// <summary>
        /// Metodo para guardar cambios de Sucursales por servicio
        /// </summary>
        /// <param name="servicioSucursal"></param>
        public void GuardarCambiosServiciosSucursales(CLFacturaServiciosDC servicioSucursal)
        {
            NegocioClientes.GuardarCambiosServiciosSucursales(servicioSucursal);
        }

        /// <summary>
        /// Método para obtener las sucursales excluidas de una factura
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="servicioFactura"></param>
        /// <returns></returns>
        public IEnumerable<CLSucursalServicioDC> ObtenerSucursalesServicioExcluidosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, CLFacturaServiciosDC servicioFactura)
        {
            return NegocioClientes.ObtenerSucursalesServicioExcluidosFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, servicioFactura);
        }

        #endregion Servicios de la factura

        #region Proceso

        public ICliente ICliente
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public void ConfirmarCupo()
        {
        }

        /// <summary>
        /// Obtiene la lista de precios para un cliente y un contrato dado
        /// </summary>
        /// <param name="idCliente">Identificación del cliente</param>
        /// <param name="idContrato">Identificación del contrato</param>
        public int ObtenerListaPrecios(int idCliente, int idContrato)
        {
            return 0;
        }

        public void ValidarCliente()
        {
        }

        #endregion Proceso

        #region Consultas

        /// <summary>
        /// Retorna los servicios asignados a la sucursal por el contrato
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal del cliente</param>
        public IEnumerable<CLServiciosSucursal> ObtenerServiciosSucursal(int idSucursal, int idListaPrecios)
        {
            return CLConsultas.Instancia.ObtenerServiciosSucursalPorUnidadesNegocio(idSucursal, TAConstantesServicios.UNIDAD_MENSAJERIA, TAConstantesServicios.UNIDAD_CARGA, idListaPrecios);
        }

        /// <summary>
        /// Valida que un cliente crédito pueda realizar venta de servicios y retorna  la lista de servicios habilitados.
        /// Para aquellos servicios que aparecen en más de un contrato, coge el primer contrato que se encuentre
        /// </summary>
        /// <param name="idSucursal">Identificador del centro de servicios</param>
        /// <returns>Lista de servicios ofrecidos</returns>
        public IEnumerable<CLServiciosSucursal> ObtenerServiciosSucursal(int idSucursal)
        {
            return CLConsultas.Instancia.ObtenerServiciosSucursalPorUnidadesDeNegocio(idSucursal, TAConstantesServicios.UNIDAD_MENSAJERIA, TAConstantesServicios.UNIDAD_CARGA);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientes()
        {
            return CLConsultas.Instancia.ObtenerClientes();
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito de una agencia específica
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientesxAgencia(int idAgencia)
        {
            return CLConsultas.Instancia.ObtenerClientesxAgencia(idAgencia);
        }


        // Masivos (1)
        public List<CLClientesDC> ObtenerClientesCreditoXAgencia(long idAgencia)
        {
            return CLConsultas.Instancia.ObtenerClientesCreditoXAgencia(idAgencia);
        }



        /// <summary>
        /// Trae el listado de los clientes crédito y sus respectivos contratos por una agencia y todas las agencias dependientes de la misma
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public List<CLClientesDC> ObtenerCLientesContratosXAgenciaDependientes(long idAgencia)
        {
            return CLConsultas.Instancia.ObtenerCLientesContratosXAgenciaDependientes(idAgencia);
        }

        /// <summary>
        /// Consulta información de un cliente por su id
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public CLClientesDC ObtenerCliente(int idCliente)
        {
            return CLConsultas.Instancia.ObtenerCliente(idCliente);
        }

        /// <summary>
        /// Retorna información del cliente crédito, si se requiere retornar sucursal y contrato se toma la primera sucursal activa y el primer contrato vigente de dicha sucursal
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="requiereSucursalContrato"></param>
        /// <returns></returns>
        public CLClienteCreditoSucursalContrato ObtenerInformacionClienteCredito(int idCliente, bool requiereSucursalContrato)
        {
            return CLConsultas.Instancia.ObtenerInformacionClienteCredito(idCliente, requiereSucursalContrato);
        }

        /// <summary>
        /// Retorna información del cliente crédito,
        /// retornar las  sucursal y
        /// los contrato
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="requiereSucursalContrato"></param>
        /// <returns></returns>
        public CLClienteCreditoSucursalContrato ObtenerInfoClienteCreditoContratos(int idCliente)
        {
            return CLConsultas.Instancia.ObtenerInfoClienteCreditoContratos(idCliente);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito que van a ser usados para selecciónd e sucursal y contrato
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClienteCreditoSucursalContrato> ObtenerClientesCredito()
        {
            return CLConsultas.Instancia.ObtenerClientesCredito();
        }

        /// <summary>
        /// Obtiene la lista de las sucursales de clientes credito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLSucursalDC> ObtenerSucursalesClientesCredito(DateTime fechaInicial, DateTime fechaFinal, string idMensajero, int idEstado)
        {
            return CLRepositorio.Instancia.ObtenerSucursalesClientesCredito(fechaInicial, fechaFinal, idMensajero, idEstado);
        }

        /// <summary>
        /// Obtiene los clientes y las sucursales con certificaciones de entrega pendientes
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<CLClienteCreditoSucursalContrato> ObtenerClientesSucursalesCertificacionesEntregaPendientes(long idCol)
        {
            return CLConsultas.Instancia.ObtenerClientesSucursalesCertificacionesEntregaPendientes(idCol);
        }

        /// <summary>
        /// Obtiene los clientes y las sucursales con certificaciones de devolucion pendientes
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<CLClienteCreditoSucursalContrato> ObtenerClientesSucursalesCertificacionesDevolucionPendientes(long idCol)
        {
            return CLConsultas.Instancia.ObtenerClientesSucursalesCertificacionesDevolucionPendientes(idCol);
        }

        /// <summary>
        /// Retorna un contrato dado su ID
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public CLContratosDC ObtenerContrato(int idContrato)
        {
            return CLConsultas.Instancia.ObtenerContrato(idContrato);
        }

        /// <summary>
        /// Retorna la lista de clientes crédito que aplican a admisión peatón - convenio dadas la ciudad de origen y destino del envío. 
        /// Retorna las sucursales que tienen contratos vigentes para la ciudad de destino y valida que la ciudad de origen sea permitida 
        /// para el cliente crédito dado. Además valida que la lista de precios asociada al contrato esté vigente.  
        /// </summary>
        /// <param name="idCiudadOrigen">Id de la ciudad de origen</param>
        /// <param name="idCiudadDestino">Id de la ciudad de destino</param>
        /// <returns></returns>
        public List<CLClienteSucConvenioNal> ObtenerClientesAplicanConvenio(string idCiudadOrigen, string idCiudadDestino)
        {
            return CLConsultas.Instancia.ObtenerClientesAplicanConvenio(idCiudadOrigen, idCiudadDestino);
        }

        /// <summary>
        /// Obtiene una lista de todos los clientes crédito tienen convenio
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerTodosClientesConvenio()
        {
            return CLClienteCredito.Instancia.ObtenerTodosClientesConvenio();
        }
        #endregion Consultas

        #region Validaciones

        /// <summary>
        /// Valida si una identificacion se encuentra en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public bool ValidarNitExistente(string identificacion)
        {
            return NegocioClientes.ValidarNitExistente(identificacion);
        }


        /// <summary>
        /// Valida que el Nit exista y no se encuentre ne la lista restrictiva
        /// </summary>
        /// <param name="Nit"></param>
        public void ValidarNit(string Nit)
        {
            NegocioClientes.ValidarNit(Nit);
        }

        #endregion Validaciones

        #region Cliente Contado

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ConsultarTiposIdentificacionReclamaGiros()
        {
            return COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>().ConsultarTiposIdentificacionReclamaGiros();
        }

        #endregion Cliente Contado

        #region Referencia uso guia

        /// <summary>
        /// Inserta Modifica o elimina una  referencia uso guia interna
        /// </summary>
        /// <param name="referencia"></param>
        public void ActualizarReferenciaUsoGuia(CLReferenciaUsoGuiaDC referencia)
        {
            CLClienteCredito.Instancia.ActualizarReferenciaUsoGuia(referencia);
        }

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
        public IList<CLReferenciaUsoGuiaDC> ObtenerReferenciaUsoGuia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return CLClienteCredito.Instancia.ObtenerReferenciaUsoGuia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene la referencia de uso guia por sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public CLReferenciaUsoGuiaDC ObtenerReferenciaUsoGuiaPorSucursal(int idSucursal)
        {
            return CLClienteCredito.Instancia.ObtenerReferenciaUsoGuiaPorSucursal(idSucursal);
        }

        #endregion Referencia uso guia

        #region Sucursal Suministros

        /// <summary>
        /// Obtiene las sucursales activas existentes en el sistema
        /// </summary>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesActivas()
        {
            return CLConsultas.Instancia.ObtenerSucursalesActivas();
        }

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
        public List<SUSuministroSucursalDC> ObtenerSucursalesSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idSucursal)
        {
            if (!filtro.ContainsKey("GRS_IdGrupoSuministro"))
                filtro.Add("GRS_IdGrupoSuministro", "CLI");

            List<SUGrupoSuministrosDC> grupoSuministro = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().ObtenerGrupoSuministrosConSuminGrupo(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
            if (grupoSuministro == null)
                grupoSuministro = new List<SUGrupoSuministrosDC>();

            List<SUSuministroSucursalDC> suministroSuc = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().ObtenerSuministrosSucursal(idSucursal);
            if (suministroSuc == null)
                suministroSuc = new List<SUSuministroSucursalDC>();

            List<SUSuministroSucursalDC> sumSuc = new List<SUSuministroSucursalDC>();

            grupoSuministro.ForEach(g =>
            {
                g.SuministrosGrupo.ForEach(sg =>
                {
                    if (suministroSuc.Where(s => s.IdSuministro == sg.Id).Count() <= 0)
                    {
                        sg.SuministroAutorizado = false;
                        sumSuc.Add(new SUSuministroSucursalDC()
                        {
                            IdSucursal = idSucursal,
                            CantidadInicialAutorizada = sg.CantidadInicialAutorizada,
                            IdSuministro = sg.Id,
                            IdSuministroSucursal = 0,
                            StockMinimo = sg.StockMinimo,
                            EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                            Suministro = sg
                        });
                    }
                });

                suministroSuc.ForEach(s =>
                {
                    s.Suministro.SuministroAutorizado = true;
                    sumSuc.Add(new SUSuministroSucursalDC()
                    {
                        IdSucursal = idSucursal,
                        CantidadInicialAutorizada = s.CantidadInicialAutorizada,
                        IdSuministro = s.IdSuministro,
                        IdSuministroSucursal = s.IdSuministroSucursal,
                        StockMinimo = s.StockMinimo,
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                        Suministro = s.Suministro
                    });
                });
            });

            sumSuc = sumSuc.OrderBy(s => s.Suministro.Descripcion).ToList();

            return sumSuc;
        }

        /// <summary>
        /// agrega o modifica un suministro de una sucursal
        /// </summary>
        /// <param name="sumSuc"></param>
        public void ActualizarSuministroSucursal(List<SUSuministroSucursalDC> sumSuc)
        {
            COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().AgregarModificarSuministroSucursal(sumSuc);
        }

        #endregion Sucursal Suministros

        #region Cliente convenio

        /// <summary>
        /// Método para obtener las localidades origen de un cliente convenio
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public IEnumerable<CLConvenioLocalidadDC> ObtenerLocalidadesConvenio(int idCliente)
        {
            return CLRepositorio.Instancia.ObtenerLocalidadesConvenio(idCliente);
        }


        #endregion

        #region PBX

        public string ObtenerNombreyDireccionCliente(string Telefono)
        {
            return CLRepositorio.Instancia.ObtenerNombreyDireccionCliente(Telefono);
        }



        #endregion



        public IEnumerable<CLTipoClienteCreditoDC> ObtenerTipoClienteCredito()
        {
            return CLConfiguradorClientes.Instancia.ObtenerTipoClienteCredito();
        }
    }
}