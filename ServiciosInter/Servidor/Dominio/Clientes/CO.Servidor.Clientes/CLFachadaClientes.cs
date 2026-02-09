using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Clientes
{
    /// <summary>
    /// Fachada de la lógica de Clientes
    /// </summary>
    public class CLFachadaClientes : ICLFachadaClientes
    {
        /// <summary>
        /// Instancia Singleton
        /// </summary>
        private static readonly CLFachadaClientes instancia = new CLFachadaClientes();

        #region Propiedades

        /// <summary>
        /// Retorna una instancia de la fabrica de Dominio
        /// </summary>
        public static CLFachadaClientes Instancia
        {
            get { return CLFachadaClientes.instancia; }
        }

        #endregion Propiedades

        /// <summary>
        /// Consulta la informacion de un cliente Contado a partir de un tipo de documento y un numero de documento
        /// </summary>
        /// <param name="tipoDocumento">Tipo de documento del cliente a consultar</param>
        /// <param name="numeroDocumento">Numéro del documento del cliente a consultar </param>
        /// <returns>Cliente Contado</returns>
        public CLClienteContadoDC ConsultarClienteContado(string tipoDocumento, string numeroDocumento, bool conDestinatariosFrecuentes, string idMunicipioDestino)
        {
            return CLClienteContado.Instancia.ConsultarClienteContado(tipoDocumento, numeroDocumento, conDestinatariosFrecuentes, idMunicipioDestino);
        }

        /// <summary>
        /// Consulta la información de un cliente a partir de su número telefónico
        /// </summary>
        /// <param name="numTelefono"></param>
        /// <returns></returns>
        public List<CLClienteContadoDC> ConsultarClienteContado(string numTelefono)
        {
            return CLConsultas.Instancia.ConsultarClienteContado(numTelefono);
        }

        /// <summary>
        /// Enviar correo electronico cuando un cliente se encuentre en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        public void EnviarCorreoListasRestrictivas(string identificacion, long idCentroServicios, string nombreCentroServicios)
        {
            CLClienteContado.Instancia.EnviarCorreoListasRestrictivas(identificacion, idCentroServicios, nombreCentroServicios);
        }

        /// <summary>
        /// Adiciona el cliente remitente y destinatario, adiciona los destinatarios frecuentes
        /// y el acumulado a cada clilente
        /// </summary>
        /// <param name="clienteContado"></param>
        /// <param name="valorGiro"></param>
        public decimal AdmGuardarClienteContado(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, decimal valorGiro, string descUltimoCentroServDestino, long idUltimoCentroServDestino)
        {
            return CLClienteContado.Instancia.AdmGuardarClienteContado(clienteContadoRemitente, clienteContadoDestinatario, valorGiro, descUltimoCentroServDestino, idUltimoCentroServDestino);
        }

        /// <summary>
        /// Almacenar a un cliente y acumular los valores
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public decimal GuardarClienteAcumularValores(CLClienteContadoDC clienteContado, decimal valorGiro)
        {
            return CLClienteContado.Instancia.GuardarClienteAcumularValores(clienteContado, valorGiro);
        }

        /// <summary>
        /// Adiciona el acumulado de pagos dinero al cliente
        /// </summary>
        /// <param name="clienteContado"></param>
        /// <returns>Valor acumulado de los pagos</returns>
        public decimal AcumuladoPagos(CLClienteContadoDC clienteContado, decimal valorPago)
        {
            return CLClienteContado.Instancia.AcumuladoPagos(clienteContado, valorPago);
        }

        /// <summary>
        /// Consultar la ultima cedula escaneada
        /// </summary>
        /// <returns> archivo del cliente</returns>
        public string ConsultarDocumentoCliente(string tipoId, string identificacion)
        {
            return CLClienteContado.Instancia.ConsultarDocumentoCliente(tipoId, identificacion);
        }

        /// <summary>
        /// Almacenar la cedula del cliente que reclama el giro
        /// </summary>
        ///<param name="pagosGiros">informacion del pago</param>
        public void AlmacenarCedulaCliente(CLClienteContadoDC clienteContado, string archivoCedulaClientePago)
        {
            CLClienteContado.Instancia.AlmacenarCedulaCliente(clienteContado, archivoCedulaClientePago);
        }

        /// <summary>
        ///Validar suministros provisionados
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public bool ValidarSuministroProvisionado(int idSuministro, int idSucursal)
        {
            return CLConsultas.Instancia.ValidarSuministroProvisionado(idSucursal, idSuministro);
        }

        /// <summary>
        /// Validar suministros provisionados
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idContrato"></param>
        /// <param name="valorTransaccion"></param>
        /// <returns></returns>
        public bool ValidaCupoPresupuestoMensual(int idContrato, decimal valorTransaccion)
        {
            return CLCupoCliente.Instancia.ValidaCupoPresupuestoMensual(idContrato, valorTransaccion);
        }

        /// <summary>
        /// Obtiene el tipo de guia por sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public CLSucursalGuiasDC ObtenerGuiaPorSucursal(int idSucursal)
        {
            return CLConfiguradorClientes.Instancia.ObtenerGuiaPorSucursal(idSucursal);
        }

        /// <summary>
        /// Validar el cupo de cliente
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="valorTransaccion"></param>
        /// <returns>"True" si se superó el porcentaje mínimo de aviso.</returns>
        public bool ValidarCupoCliente(int idContrato, decimal valorTransaccion)
        {
            return CLClienteCredito.Instancia.ValidarCupoCliente(idContrato, valorTransaccion);
        }

        /// <summary>
        /// Modifica el acumulado de un contrato a partir de una valor de transaccion
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="valorTransaccion"></param>
        public void ModificarAcumuladoContrato(int idContrato, decimal valorTransaccion)
        {
            CLClienteCredito.Instancia.ModificarAcumuladoContrato(idContrato, valorTransaccion);
        }

        /// <summary>
        /// Retorna el id de la sucursal
        /// </summary>
        /// <param name="numeroGuia">Numero de Guia</param>
        /// <returns>Id de la sucursal, si la guia no esta provisiona en una sucursal retorna 0</returns>
        public int ObtenerSucursalPorSuministro(long numeroSerial, int idSuministro)
        {
            return CLConsultas.Instancia.ObtenerSucursalPorNumeroGuia(numeroSerial, idSuministro);
        }

        /// <summary>
        /// Retorna el centro de servicios que administra la sucursal pasada como parámetro
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <returns></returns>
        public CO.Servidor.Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC ObtenerCentroServiciosAdministraSucursal(int idSucursal)
        {
            return CLConsultas.Instancia.ObtenerCentroServiciosAdministraSucursal(idSucursal);
        }

        /// <summary>
        /// Obtener las sucursales de una Agencia
        /// </summary>
        /// <param name="idAgencia">id de la agencia encargada de las sucursales</param>
        /// <returns>Sucursales de la Agencia</returns>
        public List<CLSucursalDC> ObtenerSucursalesPorIdAgencia(long idAgencia)
        {
            return CLConsultas.Instancia.ObtenerSucursalesPorIdAgencia(idAgencia);
        }

        public CO.Servidor.Servicios.ContratoDatos.Clientes.CLClientesDC ObtenerCliente(int idCliente)
        {
            return CLConsultas.Instancia.ObtenerCliente(idCliente);
        }

        /// <summary>
        /// Consulta información de un cliente por su nit
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public CLClientesDC ObtenerClientexNit(string nit)
        {
            return CLConsultas.Instancia.ObtenerClientexNit(nit);
        }

        /// <summary>
        /// Retorna la información básica de una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public CLSucursalDC ObtenerSucursal(int idSucursal, CLClientesDC cliente)
        {
            return CLConsultas.Instancia.ObtenerSucursalCliente(idSucursal, cliente);
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

        /// <summary>
        /// Registra destinatarios frecuentes
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="clienteContadoDestinatario"></param>
        /// <param name="descUltimoCentroServDestino"></param>
        /// <param name="idUltimoCentroServDestino"></param>
        public void RegistrarClienteContado(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, string descUltimoCentroServDestino, long idUltimoCentroServDestino, string usuarioCreacion)
        {
            CLClienteContado.Instancia.RegistrarClienteContado(clienteContadoRemitente, clienteContadoDestinatario, descUltimoCentroServDestino, idUltimoCentroServDestino, usuarioCreacion); 
        }

        /// <summary>
        /// Obtiene los clientes y sus contratos por agencia
        /// </summary>
        /// <param name="idAgencia">Identificador Agencia</param>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerClientesContratosXAgencia(long idAgencia)
        {
            return CLConsultas.Instancia.ObtenerClientesContratosXAgencia(idAgencia);
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
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerClientesContratosGiros()
        {
            return CLClienteCredito.Instancia.ObtenerClientesContratosGiros();
        }

        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// y Cupo de Dispersion Aprobado
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerTodosClientesContratosGiros()
        {
            return CLClienteCredito.Instancia.ObtenerTodosClientesContratosGiros();
        }

        /// <summary>
        /// Adiciona, edita o elimina una de las condiciones para el servicio de giros para un cliente
        /// </summary>

        public void AdministrarClienteCondicionGiro(CLContratosDC contrato)
        {
            CLClienteCredito.Instancia.AdministrarClienteCondicionGiro(contrato);
        }

        /// <summary>
        /// Traduce a texto un id de notación consultando en la base de datos los valores de la misma
        /// </summary>
        /// <param name="idNotacionDia"></param>
        /// <returns></returns>
        public string TraducirNotacionDiaInter(short idNotacionDia)
        {
            return CLConsultas.Instancia.TraducirNotacionDiaInter(idNotacionDia);
        }

        /// <summary>
        /// Consulta el listado de sucursales que pertenecen a un agrupamiento de una factura
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        public IEnumerable<CLSucursalDC> ConsultarSucursalesAgrupamientoFactura(int idAgrupamiento)
        {
            return CLConsultas.Instancia.ConsultarSucursalesAgrupamientoFactura(idAgrupamiento);
        }

        /// <summary>
        /// Consulta el listado de servicios que pertenecen a un agrupamiento de una factura
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        public IEnumerable<TAServicioDC> ConsultarServiciosAgrupamientoFactura(int idAgrupamiento)
        {
            return CLConsultas.Instancia.ConsultarServiciosAgrupamientoFactura(idAgrupamiento);
        }

        /// <summary>
        /// Consulta los requisitos asociados a un agrupamiento de una factura configurada
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        public IEnumerable<string> ConsultarRequisitosAgrupamientoFactura(int idAgrupamiento)
        {
            return CLConsultas.Instancia.ConsultarRequisitosAgrupamientoFactura(idAgrupamiento);
        }

        /// <summary>
        /// Cambia la agencia encargada de la sucursal
        /// </summary>
        /// <param name="AnteriorAgencia"></param>
        /// <param name="NuevaAgencia"></param>
        public void ModificarAgenciaResponsableSucursal(long anteriorAgencia, long nuevaAgencia)
        {
            CLClienteCredito.Instancia.ModificarAgenciaResponsableSucursal(anteriorAgencia, nuevaAgencia);
        }

        /// <summary>
        /// Obtiene los clientes activos que tengan una sucursal activa por municipio
        /// </summary>
        public List<CLClientesDC> ObtenerClientesSucursalesActivas(string idLocalidad)
        {
            return CLClienteCredito.Instancia.ObtenerClientesSucursalesActivas(idLocalidad);
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

        /// <summary>
        /// Obtiene las sucursales activas de un cliente por ciudad de sucursal
        /// </summary>
        /// <param name="idClienteCredito"></param>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesXCiudadActivasCliente(int idClienteCredito, string idLocalidad)
        {
            return CLClienteCredito.Instancia.ObtenerSucursalesXCiudadActivasCliente(idClienteCredito, idLocalidad);
        }

        /// <summary>
        /// Obtiene los contratos activos por cliente
        /// </summary>
        /// <param name="idCliente">lista con los contratios activos por cliente</param>
        /// <returns></returns>
        public IEnumerable<CLContratosDC> ObtenerContratosActivos(int idCliente)
        {
            return CLConfiguradorClientes.Instancia.ObtenerContratosActivos(idCliente);
        }

        /// <summary>
        /// Obtiene las Sucursales por
        /// contrato
        /// </summary>
        /// <param name="idContrato">id del contrato</param>
        /// <returns>lista de Sucursales</returns>
        public List<CLSucursalDC> ObtenerSucursalesPorContrato(int idContrato)
        {
            return CLClienteCredito.Instancia.ObtenerSucursalesPorContrato(idContrato);
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

        /// <summary>
        /// obtine el contrato de un cliente en una
        /// ciudad
        /// </summary>
        /// <param name="idCliente">id del cliente</param>
        /// <param name="idCiudad">id de la ciudad</param>
        /// <returns>lista de contratos del cliente en esa ciudad</returns>
        public IEnumerable<CLContratosDC> ObtenerContratosClienteCiudad(int idCliente, string idCiudad)
        {
            return CLClienteCredito.Instancia.ObtenerContratosClienteCiudad(idCliente, idCiudad);
        }

        /// <summary>
        /// Obtiene las Sucursales por
        /// contrato y ciudad
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns>lista de Sucursales</returns>
        public List<CLSucursalDC> ObtenerSucursalesPorContratoCiudad(int idContrato, string idCiudad)
        {
            return CLClienteCredito.Instancia.ObtenerSucursalesPorContratoCiudad(idContrato, idCiudad);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns>lista de Clientes</returns>
        public IEnumerable<CLClientesDC> ObtenerClientes()
        {
            return CLConsultas.Instancia.ObtenerClientes();
        }


        /// <summary>
        /// Obtiene un cliente a partir del id
        /// </summary>
        /// <returns>Cliente</returns>
        public CLClientesDC ObtenerClientexId(long idCliente)
        {
            return CLConsultas.Instancia.ObtenerCliente(idCliente);
        }
    }
}