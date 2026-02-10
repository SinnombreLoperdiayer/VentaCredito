using System.Linq;
using System.Collections.Generic;
using CO.Servidor.Clientes.Datos;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;

namespace CO.Servidor.Clientes
{
    /// <summary>
    /// Contiene las consultas asociadas a clientes
    /// </summary>
    internal class CLConsultas : ControllerBase
    {
        #region Singleton

        private static readonly CLConsultas instancia = (CLConsultas)FabricaInterceptores.GetProxy(new CLConsultas(), COConstantesModulos.CLIENTES);

        public static CLConsultas Instancia
        {
            get { return CLConsultas.instancia; }
        }

        #endregion Singleton

        #region Métodos

        /// <summary>
        /// Obtiene las sucursales activas existentes en el sistema
        /// </summary>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesActivas()
        {
            return CLRepositorio.Instancia.ObtenerSucursalesActivas();
        }

        /// <summary>
        /// Consulta la información de un cliente a partir de su número telefónico
        /// </summary>
        /// <param name="numTelefono"></param>
        /// <returns></returns>
        internal List<CLClienteContadoDC> ConsultarClienteContado(string numTelefono)
        {
            List<CLClienteContadoDC> clienteContado = CLRepositorio.Instancia.ConsultarClienteContado(numTelefono);
            return clienteContado;
        }

        /// <summary>
        /// Retorna los servicios asignados a la sucursal por el contrato
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal del cliente</param>
        /// <param name="unidadnegocio">Unidad de negocio</param>
        public IEnumerable<CLServiciosSucursal> ObtenerServiciosSucursalPorUnidadNegocio(int idSucursal, string idUnidadMensajeria, int idListaPrecios)
        {
            return CLRepositorio.Instancia.ObtenerServiciosSucursalPorUnidadNegocio(idSucursal, idUnidadMensajeria, idListaPrecios);
        }

        /// <summary>
        /// Retorna los servicios asignados a la sucursal por el contrato
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal del cliente</param>
        /// <param name="unidadnegocio">Unidad de negocio</param>
        public IEnumerable<CLServiciosSucursal> ObtenerServiciosSucursalPorUnidadesNegocio(int idSucursal, string idUnidadMensajeria, string idUnidadCarga, int idListaPrecios)
        {
            return CLRepositorio.Instancia.ObtenerServiciosPorUnidadesDeNegocio(idSucursal, idUnidadMensajeria, idUnidadCarga, idListaPrecios);
        }

        /// <summary>
        /// Valida que un cliente crédito pueda realizar venta de servicios y retorna  la lista de servicios habilitados.
        /// Para aquellos servicios que aparecen en más de un contrato, coge el primer contrato que se encuentre
        /// </summary>
        /// <param name="idCentroServicios">Identificador del centro de servicios</param>
        /// <returns>Lista de servicios ofrecidos</returns>
        public IEnumerable<CLServiciosSucursal> ObtenerServiciosSucursalPorUnidadesDeNegocio(int idSucursal, string idUnidadMensajeria, string idUnidadCarga)
        {
            return CLRepositorio.Instancia.ObtenerServiciosPorUnidadesDeNegocio(idSucursal, idUnidadMensajeria, idUnidadCarga);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientes()
        {
            return CLRepositorio.Instancia.ObtenerClientes();
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito de una agencia específica
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientesxAgencia(int idAgencia)
        {
            return CLRepositorio.Instancia.ObtenerClientesxAgencia(idAgencia);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito que van a ser usados para selecciónd e sucursal y contrato
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClienteCreditoSucursalContrato> ObtenerClientesCredito()
        {
            return CLRepositorio.Instancia.ObtenerClientesCredito();
        }

        /// <summary>
        /// Obtiene los clientes y las sucursales con certificaciones de entrega pendientes
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<CLClienteCreditoSucursalContrato> ObtenerClientesSucursalesCertificacionesEntregaPendientes(long idCol)
        {
            return CLRepositorio.Instancia.ObtenerClientesSucursalesCertificacionesEntregaPendientes(idCol);
        }

        /// <summary>
        /// Obtiene los clientes y las sucursales con certificaciones de devolucion pendientes
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<CLClienteCreditoSucursalContrato> ObtenerClientesSucursalesCertificacionesDevolucionPendientes(long idCol)
        {
            return CLRepositorio.Instancia.ObtenerClientesSucursalesCertificacionesDevolucionPendientes(idCol);
        }

        /// <summary>
        /// Consulta información de un cliente por su id
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public CLClientesDC ObtenerCliente(int idCliente)
        {
            return CLRepositorio.Instancia.ObtenerCliente(idCliente);
        }

        /// <summary>
        /// Consulta información de un cliente por su nit
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public CLClientesDC ObtenerClientexNit(string nit)
        {
            return CLRepositorio.Instancia.ObtenerClientexNit(nit);
        }

        /// <summary>
        /// Retorna información del cliente crédito, si se requiere retornar sucursal y contrato se toma la primera sucursal activa y el primer contrato vigente de dicha sucursal
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="requiereSucursalContrato"></param>
        /// <returns></returns>
        public CLClienteCreditoSucursalContrato ObtenerInformacionClienteCredito(int idCliente, bool requiereSucursalContrato)
        {
            return CLRepositorio.Instancia.ObtenerInformacionClienteCredito(idCliente, requiereSucursalContrato);
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
            return CLRepositorio.Instancia.ObtenerInfoClienteCreditoContratos(idCliente);
        }

        /// <summary>
        /// Retorna el id de la sucursal
        /// </summary>
        /// <param name="numeroGuia">Numero de Guia</param>
        /// <returns>Id de la sucursal, si la guia no esta provisiona en una sucursal retorna 0</returns>
        public int ObtenerSucursalPorNumeroGuia(long numeroGuia, int idSuministro)
        {
            return CLRepositorio.Instancia.ObtenerSucursalSuministroSerial(numeroGuia, idSuministro);
        }

        /// <summary>
        /// Metodo  que valida si un suministro con un serial esta asignado
        /// </summary>
        /// <param name="Serial"></param>
        /// <param name="suministro"></param>
        /// <returns></returns>
        public bool ValidarSuministroProvisionado(int sucursal, int idSuministro)
        {
            if (CLRepositorio.Instancia.ObtenerSuministrosSucursal(sucursal, idSuministro) == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Retorna el centro de servicios que administra la sucursal pasada como parámetro
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <returns></returns>
        public CO.Servidor.Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC ObtenerCentroServiciosAdministraSucursal(int idSucursal)
        {
            return CLRepositorio.Instancia.ObtenerCentroServiciosAdministraSucursal(idSucursal);
        }

        /// <summary>
        /// Obtener las sucursales de una Agencia
        /// </summary>
        /// <param name="idAgencia">id de la agencia encargada de las sucursales</param>
        /// <returns>Sucursales de la Agencia</returns>
        public List<CLSucursalDC> ObtenerSucursalesPorIdAgencia(long idAgencia)
        {
            return CLRepositorio.Instancia.ObtenerSucursalesPorIdAgencia(idAgencia);
        }

        /// <summary>
        /// Retorna la información básica de una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        internal CLSucursalDC ObtenerSucursalCliente(int idSucursal, CLClientesDC cliente)
        {
            return CLRepositorio.Instancia.ObtenerSucursalCliente(idSucursal, cliente);
        }

        /// <summary>
        /// Obtiene los clientes y sus contratos por agencia
        /// </summary>
        /// <param name="idAgencia">Identificador Agencia</param>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerClientesContratosXAgencia(long idAgencia)
        {
            return CLRepositorio.Instancia.ObtenerClientesContratosXAgencia(idAgencia);
        }


        // Masivos (1)
        public List<CLClientesDC> ObtenerClientesCreditoXAgencia(long idAgencia)
        {
            return CLRepositorio.Instancia.ObtenerClientesCreditoXAgencia(idAgencia);
        }


        /// <summary>
        /// Trae el listado de los clientes crédito y sus respectivos contratos por una agencia y todas las agencias dependientes de la misma
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public List<CLClientesDC> ObtenerCLientesContratosXAgenciaDependientes(long idAgencia)
        {
            List<CLClientesDC> clientes = ObtenerClientesContratosXAgencia(idAgencia);
            List<PUCentroServiciosDC> centrosServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerPuntosAgenciasDependientes(idAgencia);

            // Ahora obtener los clientes con sus respectivos contratos por cada centro de servicio
            foreach (PUCentroServiciosDC cs in centrosServicio)
            {
                List<CLClientesDC> clientesCs = ObtenerClientesContratosXAgencia(cs.IdCentroServicio);
                clientes.AddRange(clientesCs);
            }

            return clientes;
        }



        /// <summary>
        /// Traduce a texto un id de notación consultando en la base de datos los valores de la misma
        /// </summary>
        /// <param name="idNotacionDia"></param>
        /// <returns></returns>
        public string TraducirNotacionDiaInter(short idNotacionDia)
        {
            return CLRepositorio.Instancia.TraducirNotacionDiaInter(idNotacionDia);
        }

        /// <summary>
        /// Consulta el listado de sucursales que pertenecen a un agrupamiento de una factura
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        public IEnumerable<CLSucursalDC> ConsultarSucursalesAgrupamientoFactura(int idAgrupamiento)
        {
            return CLRepositorio.Instancia.ConsultarSucursalesAgrupamientoFactura(idAgrupamiento);
        }

        /// <summary>
        /// Consulta el listado de servicios que pertenecen a un agrupamiento de una factura
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        public IEnumerable<TAServicioDC> ConsultarServiciosAgrupamientoFactura(int idAgrupamiento)
        {
            return CLRepositorio.Instancia.ConsultarServiciosAgrupamientoFactura(idAgrupamiento);
        }

        /// <summary>
        /// Consulta los requisitos asociados a un agrupamiento de una factura configurada
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        public IEnumerable<string> ConsultarRequisitosAgrupamientoFactura(int idAgrupamiento)
        {
            return CLRepositorio.Instancia.ConsultarRequisitosAgrupamientoFactura(idAgrupamiento);
        }

        /// <summary>
        /// Retorna un contrato dado su ID
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public CLContratosDC ObtenerContrato(int idContrato)
        {
            return CLRepositorio.Instancia.ObtenerContrato(idContrato);
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
            return CLRepositorio.Instancia.ObtenerClientesAplicanConvenio(idCiudadOrigen, idCiudadDestino);
        }

        /// <summary>
        /// Obtiene un cliente a partir del id
        /// </summary>
        /// <returns>Cliente</returns>
        internal CLClientesDC ObtenerCliente(long idCliente)
        {
            return CLRepositorio.Instancia.ObtenerCliente(idCliente);
        }

        #endregion Métodos

    }
}