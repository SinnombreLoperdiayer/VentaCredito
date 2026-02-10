using CO.Servidor.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;

namespace CO.Servidor.Servicios.Implementacion.Clientes
{
    /// <summary>
    /// Clase para los servicios de administración de Clientes
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CLClientesSvc : ICLClientesSvc
    {
        #region Constructor

        public CLClientesSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion Constructor

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLClientesDC> ObtenerClientesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLClientesDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerClientesFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Metodo para la manipulacion de clientes de acuerdo al estado del registro
        /// </summary>
        /// <param name="cliente"></param>
        public void ModificarCliente(CLClientesDC cliente)
        {
            CLAdministradorClientes.Instancia.ModificarCliente(cliente);
        }

        /// <summary>
        /// Obtiene las sucursales del cliente por racol
        /// </summary>
        /// <param name="idRacol">Id del racol</param>
        /// <param name="idCliente">id del cliente</param>
        /// <returns>Lista de las sucursales del cliente y del racol</returns>
        public List<CLSucursalDC> ObtenerSucursalesClienteRacol(long idRacol, long idCliente)
        {
            return CLAdministradorClientes.Instancia.ObtenerSucursalesClienteRacol(idRacol, idCliente);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito de una localidad
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientesLocalidad(string idLocalidad)
        {
            return CLAdministradorClientes.Instancia.ObtenerClientesLocalidad(idLocalidad);
        }

        #endregion Metodos basicos

        #region EstadosCliente

        /// <summary>
        /// Obtiene una lista con los estados de un cliente
        /// </summary>
        /// <returns>objeto lista de los estados de un cliente</returns>
        public IEnumerable<CLEstadosClienteDC> ObtenerEstadosCliente(CLClientesDC cliente)
        {
            return CLAdministradorClientes.Instancia.ObtenerEstadosCliente(cliente);
        }

        /// <summary>
        /// Obtiene lista con los estados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EstadoDC> ObtenerEstados()
        {
            return CLAdministradorClientes.Instancia.ObtenerEstados();
        }

        /// <summary>
        /// Obtiene lista con los estados y motivos
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLMotivoEstadosDC> ObtenerMotivosEstados()
        {
            return CLAdministradorClientes.Instancia.ObtenerMotivosEstados();
        }

        /// <summary>
        /// Obtiene Lista Tipos CLiente Credito
        /// </summary>
        public IEnumerable<CLTipoClienteCreditoDC> ObtenerTipoClienteCredito()
        {
            return CLAdministradorClientes.Instancia.ObtenerTipoClienteCredito();
        }

        #endregion EstadosCliente

        #region Archivos clientes

        /// <summary>
        /// Obtiene lista con los archivos de un cliente
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLArchivosDC> ObtenerArchivos(CLClientesDC cliente)
        {
            return CLAdministradorClientes.Instancia.ObtenerArchivosCliente(cliente);
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un cliente
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivo(CLArchivosDC archivo)
        {
            return CLAdministradorClientes.Instancia.ObtenerArchivoCliente(archivo);
        }

        #endregion Archivos clientes

        #region Divulgación de cliente

        /// <summary>
        /// Envia la divulgacion de una agencia
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="destinatarios">Diccionario con la informacion de los destinatarios key=Email Value = NombreDestinatario</param>
        public void DivulgarCliente(CLContratosDC contrato, PADivulgacion divulgacion)
        {
            CLAdministradorClientes.Instancia.DivulgarCliente(contrato, divulgacion);
        }

        #endregion Divulgación de cliente

        #region Sucursales

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un nit de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLSucursalDC> ObtenerSucursalesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCliente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLSucursalDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerSucursalesFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCliente),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Metodo para modificar sucursales en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo sucursal</param>
        public int ModificarSucursales(CLSucursalDC sucursal)
        {
            return CLAdministradorClientes.Instancia.ModificarSucursal(sucursal);
        }


        /// <summary>
        /// Obtiene las sucursales activas de un cliente
        /// </summary>
        /// <param name="idClienteCredito"></param>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesActivasCliente(int idClienteCredito)
        {
            return CLAdministradorClientes.Instancia.ObtenerSucursalesActivasCliente(idClienteCredito);
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
            return CLAdministradorClientes.Instancia.ObtenerGuiaPorSucursal(idSucursal);
        }

        /// <summary>
        /// guarda los cambios de guia por sucursal
        /// </summary>
        /// <param name="guiaSucursal"></param>
        public void GuardarCambiosGuiaPorSucursal(CLSucursalGuiasDC guiaSucursal)
        {
            CLAdministradorClientes.Instancia.GuardarCambiosGuiaPorSucursal(guiaSucursal);
        }

        #endregion Tipo de guia de la sucursal

        #region Contratos

        /// <summary>
        /// Obtiene una lista con los contratos para filtrar a partir de una identificacion de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLContratosDC> ObtenerContratosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCliente)
        {
            int totalRegistros = 0;
            List<CLContratosDC> lstContratos = new List<CLContratosDC>();
            lstContratos = CLAdministradorClientes.Instancia.ObtenerContratosFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCliente);
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLContratosDC>()
            {
                Lista = lstContratos,
                TotalRegistros = lstContratos.Count()
            };
        }

        /// <summary>
        /// Metodo para modificar contratos  en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo contrato</param>
        public int ModificarContrato(CLContratosDC contrato)
        {
            return CLAdministradorClientes.Instancia.ModificarContrato(contrato);
        }

        /// <summary>
        /// Obtiene los contratos activos por cliente
        /// </summary>
        /// <param name="idCliente">lista con los contratios activos por cliente</param>
        /// <returns></returns>
        public IEnumerable<CLContratosDC> ObtenerContratosActivos(int idCliente)
        {
            return CLAdministradorClientes.Instancia.ObtenerContratosActivos(idCliente);
        }

        /// <summary>
        /// Obtiene la lista de precios asociada a un contrato
        /// </summary>
        public int ObtenerListaPrecioContrato(int idContrato)
        {
            return CLAdministradorClientes.Instancia.ObtenerListaPrecioContrato(idContrato);
        }

        #endregion Contratos

        #region Personal del contrato

        /// <summary>
        /// OObtiene las personas asociadas a un contrato
        /// </summary>
        /// <returns></returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PAPersonaInternaDC> ObtenerPersonalContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PAPersonaInternaDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerPersonalContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Metodo para modificar personal de un contrato
        /// </summary>
        /// <param name="persona"></param>
        /// <returns></returns>
        public void ModificarPersonalContrato(PAPersonaInternaDC persona)
        {
            CLAdministradorClientes.Instancia.ModificarPersonalContrato(persona);
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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLContactosDC> ObtenerContactosContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLContactosDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerContactosContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Operaciones sobre los contactos
        /// </summary>
        /// <param name="contacto">objeto de tipo contacto</param>
        public void GuardarCambiosContactos(CLContactosDC contacto)
        {
            CLAdministradorClientes.Instancia.GuardarCambiosContactos(contacto);
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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLDeduccionesContratoDC> ObtenerDeduccionesContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLDeduccionesContratoDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerDeduccionesContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Operaciones sobre las deducciones de un contrato
        /// </summary>
        /// <param name="deduccion">objeto de tipo deduccion</param>
        public void GuardarCambiosDeducciones(CLDeduccionesContratoDC deduccion)
        {
            CLAdministradorClientes.Instancia.GuardarCambiosDeducciones(deduccion);
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLOtroSiDC> ObtenerOtroSiContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLOtroSiDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerOtroSiContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Lista con los tipos de otrosi de un contrato
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLTipoOtroSiDC> ObtenerListaTiposOtrosi()
        {
            return CLAdministradorClientes.Instancia.ObtenerListaTiposOtrosi();
        }

        /// <summary>
        /// Metodo encargado de guardar cambio de un otros si de un contrato
        /// </summary>
        /// <param name="otrosi"></param>
        public void GuardarCambiosOtroSi(CLOtroSiDC otrosi)
        {
            CLAdministradorClientes.Instancia.GuardarCambiosOtroSi(otrosi);
        }

        #endregion Otrosi del contrato

        #region Archivos contrato

        /// <summary>
        /// Obtiene lista con los archivos de un contrato
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLContratosArchivosDC> ObtenerArchivosContrato(CLContratosDC contrato)
        {
            return CLAdministradorClientes.Instancia.ObtenerArchivosContrato(contrato);
        }

        /// <summary>
        /// Adiciona o elimina los archivos de un contrato
        /// </summary>
        /// <param name="archivos">objeto de tipo lista con los archivos de un contrato</param>
        public void OperacionesArchivosContrato(IEnumerable<CLContratosArchivosDC> archivos)
        {
            CLAdministradorClientes.Instancia.OperacionesArchivosContrato(archivos);
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un contrato
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoContrato(CLContratosArchivosDC archivo)
        {
            return CLAdministradorClientes.Instancia.ObtenerArchivoContrato(archivo);
        }

        #endregion Archivos contrato

        #region Sucursales del contrato

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un contrato de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLSucursalContratoDC> ObtenerSucursalesContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLSucursalContratoDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerSucursalesContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato),
                TotalRegistros = totalRegistros
            };
        }


        /// <summary>
        /// Metodo para guardar cambios de las sucursales de un contrato
        /// </summary>
        /// <param name="sucursal"></param>
        public void GuardarCambiosSucursalesContrato(CLSucursalContratoDC sucursal)
        {
            CLAdministradorClientes.Instancia.GuardarCambiosSucursalesContrato(sucursal);
        }

        /// <summary>
        /// Obtiene lista con los estados de una sucursal
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLSucursalEstadosDC> ObtenerSucursalEstados(int Idcontrato)
        {
            return CLAdministradorClientes.Instancia.ObtenerSucursalEstados(Idcontrato);
        }

        /// <summary>
        /// Obtiene una lista con las sucursales de un cliente que no esten en un contrato especifico
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalDC> ObtenerSucursalesFiltroExcepcion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, int idContrato, int idCliente)
        {
            return CLAdministradorClientes.Instancia.ObtenerSucursalesFiltroExcepcion(filtro, indicePagina, registrosPorPagina, idContrato, idCliente);
        }

        #endregion Sucursales del contrato

        #region Facturas

        /// <summary>
        /// Metodo encargado de traer los tipos de notacion para la factura
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLTipoNotacionDC> ObtenerTipoNotacion()
        {
            return CLAdministradorClientes.Instancia.ObtenerTipoNotacion();
        }

        /// <summary>
        /// Metodo para guardar cambios de una factura de una contrato
        /// </summary>
        /// <param name="?"></param>
        public int GuardarCambiosFacturas(CLFacturaDC factura)
        {
            return CLAdministradorClientes.Instancia.GuardarCambiosFacturas(factura);
        }

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un contrato de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLFacturaDC> ObtenerFacturasContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idContrato)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLFacturaDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerFacturasContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato),
                TotalRegistros = totalRegistros
            };
        }

        #endregion Facturas

        #region Requisitos de la factura

        /// <summary>
        /// Obtiene una lista con los requisitos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los requisitos configuradas en la base de datos</returns>
        public IEnumerable<CLFacturaRequisitosDC> ObtenerRequisitosFacturas(int idFactura)
        {
            return CLAdministradorClientes.Instancia.ObtenerRequisitosFacturas(idFactura);
        }

        /// <summary>
        /// Metodo para operaciones sobre requisitos de una factura
        /// </summary>
        /// <param name="requisito"></param>
        public void GuardarRequisitosFacturas(CLFacturaRequisitosDC requisito)
        {
            CLAdministradorClientes.Instancia.GuardarRequisitosFacturas(requisito);
        }

        #endregion Requisitos de la factura

        #region Descuentos de la factura

        /// <summary>
        /// Obtiene una lista con los descuentos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los descuentos configuradas en la base de datos</returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLFacturaDescuentoDC> ObtenerDescuentosFacturas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idFactura)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLFacturaDescuentoDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerDescuentosFacturas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idFactura),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Metodo para guardar cambios descuentos a una factura
        /// </summary>
        /// <param name="descuento"></param>
        public void GuardarCambiosDescuentoFactura(CLFacturaDescuentoDC descuento)
        {
            CLAdministradorClientes.Instancia.GuardarCambiosDescuentoFactura(descuento);
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
            return CLAdministradorClientes.Instancia.ObtenerServiciosFactura(factura);
        }

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un servicio de un contrato
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLSucursalServicioDC> ObtenerSucursalesServicioFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, CLFacturaServiciosDC servicioFactura)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CLSucursalServicioDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerSucursalesServicioFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, servicioFactura),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Metodo para guardar cambios de Sucursales por servicio
        /// </summary>
        /// <param name="servicioSucursal"></param>
        public void GuardarCambiosServiciosSucursales(CLFacturaServiciosDC servicioSucursal)
        {
            CLAdministradorClientes.Instancia.GuardarCambiosServiciosSucursales(servicioSucursal);
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
            return CLAdministradorClientes.Instancia.ObtenerSucursalesServicioExcluidosFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, servicioFactura);
        }

        #endregion Servicios de la factura

        #region ClienteContado

        /// <summary>
        /// Consulta la informacion de un cliente Contado a partir de un tipo de documento y un numero de documento
        /// </summary>
        /// <param name="tipoDocumento">Tipo de documento del cliente a consultar</param>
        /// <param name="numeroDocumento">Numéro del documento del cliente a consultar </param>
        /// <returns>Cliente Contado</returns>
        public CLClienteContadoDC ConsultarClienteContado(string tipoDocumento, string numeroDocumento, bool conDestinatariosFrecuentes, string idMunicipioDestino)
        {
            return CLFachadaClientes.Instancia.ConsultarClienteContado(tipoDocumento, numeroDocumento, conDestinatariosFrecuentes, idMunicipioDestino);
        }

        /// <summary>
        /// Consulta la información de un cliente a partir de su número telefónico
        /// </summary>
        /// <param name="numTelefono"></param>
        /// <returns></returns>
        public List<CLClienteContadoDC> ConsultarClienteContadoPorTelefono(string numTelefono)
        {
            return CLFachadaClientes.Instancia.ConsultarClienteContado(numTelefono);
        }

        /// <summary>
        /// Enviar correo electronico cuando un cliente se encuentre en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        public void EnviarCorreoListasRestrictivas(string identificacion, long idCentroServicios, string nombreCentroServicios)
        {
            CLFachadaClientes.Instancia.EnviarCorreoListasRestrictivas(identificacion, idCentroServicios, nombreCentroServicios);
        }

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ConsultarTiposIdentificacionReclamaGiros()
        {
            return CLAdministradorClientes.Instancia.ConsultarTiposIdentificacionReclamaGiros();
        }

        #endregion ClienteContado

        #region Consultas

        /// <summary>
        /// Retorna los servicios asignados a la sucursal por el contrato
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal del cliente</param>
        public IEnumerable<CLServiciosSucursal> ObtenerServiciosSucursal(int idSucursal, int idListaPrecios)
        {
            return CLAdministradorClientes.Instancia.ObtenerServiciosSucursal(idSucursal, idListaPrecios);
        }

        /// <summary>
        /// Valida que un cliente crédito pueda realizar venta de servicios y retorna  la lista de servicios habilitados.
        /// Para aquellos servicios que aparecen en más de un contrato, coge el primer contrato que se encuentre
        /// </summary>
        /// <param name="idSucursal">Identificador del centro de servicios</param>
        /// <returns>Lista de servicios ofrecidos</returns>
        public IEnumerable<CLServiciosSucursal> ObtenerServiciosSucursalSinValidarListaPrecios(int idSucursal)
        {
            return CLAdministradorClientes.Instancia.ObtenerServiciosSucursal(idSucursal);
        }

        /// <summary>
        /// Consulta información de un cliente por su id
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public CLClientesDC ObtenerCliente(int idCliente)
        {
            return CLAdministradorClientes.Instancia.ObtenerCliente(idCliente);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientes()
        {
            return CLAdministradorClientes.Instancia.ObtenerClientes();
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito de una agencia específica
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientesxAgencia(int idAgencia)
        {
            return CLAdministradorClientes.Instancia.ObtenerClientesxAgencia(idAgencia);
        }

        // Masivos (1)
        public List<CLClientesDC> ObtenerClientesCreditoXAgencia(long idAgencia)
        {
            return CLAdministradorClientes.Instancia.ObtenerClientesCreditoXAgencia(idAgencia);
        }
        // Masivos (2)
        public List<CLContratosDC> ObtenerContratosActivos_ClienteCredito(long idAgencia, int idCliente)
        {
            return CLAdministradorClientes.Instancia.ObtenerContratosActivos_ClienteCredito(idAgencia, idCliente);
        }
        // Masivos (3)
        public List<CLSucursalContratoDC> ObtenerSucursalesContrato_CliCredito(long IdAgencia, int IdContrato)
        {
            return CLAdministradorClientes.Instancia.ObtenerSucursalesContrato_CliCredito(IdAgencia, IdContrato);
        }






        /// <summary>
        /// Trae el listado de los clientes crédito y sus respectivos contratos por una agencia y todas las agencias dependientes de la misma
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public List<CLClientesDC> ObtenerCLientesContratosXAgenciaDependientes(long idAgencia)
        {
            return CLAdministradorClientes.Instancia.ObtenerCLientesContratosXAgenciaDependientes(idAgencia);
        }

        /// <summary>
        /// Retorna información del cliente crédito, si se requiere retornar sucursal y contrato se toma la primera sucursal activa y el primer contrato vigente de dicha sucursal
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="requiereSucursalContrato"></param>
        /// <returns></returns>
        public CLClienteCreditoSucursalContrato ObtenerInformacionClienteCredito(int idCliente, bool requiereSucursalContrato)
        {
            return CLAdministradorClientes.Instancia.ObtenerInformacionClienteCredito(idCliente, requiereSucursalContrato);
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
            return CLAdministradorClientes.Instancia.ObtenerInfoClienteCreditoContratos(idCliente);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito que van a ser usados para selecciónd e sucursal y contrato
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClienteCreditoSucursalContrato> ObtenerClientesCredito()
        {
            return CLAdministradorClientes.Instancia.ObtenerClientesCredito();
        }

        /// <summary>
        /// Obtiene la lista de las sucursales de clientes credito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLSucursalDC> ObtenerSucursalesClientesCredito(DateTime fechaInicial, DateTime fechaFinal, string idMensajero, int idEstado)
        {
            return CLAdministradorClientes.Instancia.ObtenerSucursalesClientesCredito(fechaInicial, fechaFinal, idMensajero, idEstado);
        }

        /// <summary>
        /// Obtiene los clientes y las sucursales con certificaciones de entrega pendientes
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<CLClienteCreditoSucursalContrato> ObtenerClientesSucursalesCertificacionesEntregaPendientes(long idCol)
        {
            return CLAdministradorClientes.Instancia.ObtenerClientesSucursalesCertificacionesEntregaPendientes(idCol);
        }
        /// <summary>
        /// Obtiene los clientes y las sucursales con certificaciones de devolucion pendientes
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<CLClienteCreditoSucursalContrato> ObtenerClientesSucursalesCertificacionesDevolucionPendientes(long idCol)
        {
            return CLAdministradorClientes.Instancia.ObtenerClientesSucursalesCertificacionesDevolucionPendientes(idCol);
        }



        /// <summary>
        /// Retorna un contrato dado su ID
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public CLContratosDC ObtenerContrato(int idContrato)
        {
            return CLAdministradorClientes.Instancia.ObtenerContrato(idContrato);
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
            return CLAdministradorClientes.Instancia.ObtenerClientesAplicanConvenio(idCiudadOrigen, idCiudadDestino);
        }

        /// <summary>
        /// Obtiene una lista de todos los clientes crédito tienen convenio
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerTodosClientesConvenio()
        {
            return CLAdministradorClientes.Instancia.ObtenerTodosClientesConvenio();
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
            return CLAdministradorClientes.Instancia.ValidarNitExistente(identificacion);
        }

        /// <summary>
        /// Valida que el Nit exista y no se encuentre ne la lista restrictiva
        /// </summary>
        /// <param name="Nit"></param>
        public void ValidarNit(string Nit)
        {
            CLAdministradorClientes.Instancia.ValidarNit(Nit);
        }

        #endregion Validaciones

        #region Referencia uso guia

        /// <summary>
        /// Inserta Modifica o elimina una  referencia uso guia interna
        /// </summary>
        /// <param name="referencia"></param>
        public void ActualizarReferenciaUsoGuia(CLReferenciaUsoGuiaDC referencia)
        {
            CLAdministradorClientes.Instancia.ActualizarReferenciaUsoGuia(referencia);
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
        public GenericoConsultasFramework<CLReferenciaUsoGuiaDC> ObtenerReferenciaUsoGuia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<CLReferenciaUsoGuiaDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerReferenciaUsoGuia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene la referencia de uso guia por sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public CLReferenciaUsoGuiaDC ObtenerReferenciaUsoGuiaPorSucursal(int idSucursal)
        {
            return CLAdministradorClientes.Instancia.ObtenerReferenciaUsoGuiaPorSucursal(idSucursal);
        }

        #endregion Referencia uso guia

        #region Sucursal Suministro

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
        public GenericoConsultasFramework<SUSuministroSucursalDC> ObtenerSucursalesSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idSucursal)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<SUSuministroSucursalDC>()
            {
                Lista = CLAdministradorClientes.Instancia.ObtenerSucursalesSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idSucursal),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene las sucursales activas existentes en el sistema
        /// </summary>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesActivas()
        {
            return CLAdministradorClientes.Instancia.ObtenerSucursalesActivas();
        }

        /// <summary>
        /// agrega o modifica un suministro de una sucursal
        /// </summary>
        /// <param name="sumSuc"></param>
        public void ActualizarSuministroSucursal(List<SUSuministroSucursalDC> sumSuc)
        {
            CLAdministradorClientes.Instancia.ActualizarSuministroSucursal(sumSuc);
        }

        #endregion Sucursal Suministro

        #region Cliente convenio

        /// <summary>
        /// Método para obtener las localidades origen de un cliente convenio
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public IEnumerable<CLConvenioLocalidadDC> ObtenerLocalidadesConvenio(int idCliente)
        {
            return CLAdministradorClientes.Instancia.ObtenerLocalidadesConvenio(idCliente);
        }


        #endregion

        #region PBX

        public string ObtenerNombreyDireccionCliente(string Telefono)
        {
            return CLAdministradorClientes.Instancia.ObtenerNombreyDireccionCliente(Telefono);
        }



        #endregion
    }
}