using System;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;

namespace CO.Servidor.Dominio.Comun.Clientes
{
    /// <summary>
    ///  Interfaz para acceso a la fachada de clientes
    /// </summary>
    public interface ICLFachadaClientes
    {
        /// <summary>
        /// Consulta la informacion de un cliente Contado a partir de un tipo de documento y un numero de documento
        /// </summary>
        /// <param name="tipoDocumento">Tipo de documento del cliente a consultar</param>
        /// <param name="numeroDocumento">Numéro del documento del cliente a consultar </param>
        /// <returns>Cliente Contado</returns>
        CLClienteContadoDC ConsultarClienteContado(string tipoDocumento, string numeroDocumento, bool conDestinatariosFrecuentes, string idMunicipioDestino);

        /// <summary>
        /// Consulta la información de un cliente a partir de su número telefónico
        /// </summary>
        /// <param name="numTelefono"></param>
        /// <returns></returns>
        List<CLClienteContadoDC> ConsultarClienteContado(string numTelefono);

        /// <summary>
        /// Valida si el suministro fue provisionado a la sucursal
        /// </summary>
        /// <param name="idSuministro">Id del suminitro a validar</param>
        /// <param name="idSucursal">Id de la sucursal</param>
        /// <returns></returns>
        bool ValidarSuministroProvisionado(int idSuministro, int idSucursal);

        /// <summary>
        /// Valida si el cliente tiene presupuesto
        /// </summary>
        /// <param name="idCliente">Id del cliente</param>
        /// <param name="idContrato">Id Contrato</param>
        /// <param name="valorTransaccion">Valor de la Transaccion</param>
        /// <returns></returns>
        bool ValidaCupoPresupuestoMensual(int idContrato, decimal valorTransaccion);

        /// <summary>
        /// Adiciona el cliente remitente y destinatario, adiciona los destinatarios frecuentes
        /// y el acumulado a cada clilente
        /// </summary>
        /// <param name="clienteContado"></param>
        /// <param name="valorGiro"></param>
        decimal AdmGuardarClienteContado(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, decimal valorGiro, string descUltimoCentroServDestino, long idUltimoCentroServDestino);

        /// <summary>
        /// Obtiene el tipo de guia por sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        CLSucursalGuiasDC ObtenerGuiaPorSucursal(int idSucursal);

        /// <summary>
        /// Obtiene el id de la sucursal de acuerdo suministro
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        int ObtenerSucursalPorSuministro(long numeroSerial, int idSuministro);

        /// <summary>
        /// Consultar la ultima cedula escaneada
        /// </summary>
        /// <returns> archivo del cliente</returns>
        string ConsultarDocumentoCliente(string tipoId, string identificacion);

        /// <summary>
        /// Almacenar la cedula del cliente que reclama el giro
        /// </summary>
        ///<param name="pagosGiros">informacion del pago</param>
        void AlmacenarCedulaCliente(CLClienteContadoDC clienteContado, string archivoCedulaClientePago);

        /// <summary>
        /// Adiciona el acumulado de pagos dinero al cliente
        /// </summary>
        /// <param name="clienteContado"></param>
        /// <returns>Valor acumulado de los pagos</returns>
        decimal AcumuladoPagos(CLClienteContadoDC clienteContado, decimal valorPago);

        /// <summary>
        /// Retorna el centro de servicios que administra la sucursal pasada como parámetro
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <returns></returns>
        CO.Servidor.Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC ObtenerCentroServiciosAdministraSucursal(int idSucursal);

        /// <summary>
        /// Enviar correo electronico cuando un cliente se encuentre en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        void EnviarCorreoListasRestrictivas(string identificacion, long idCentroServicios, string nombreCentroServicios);

        /// <summary>
        /// Modifica el acumulado de un contrato a partir de una valor de transaccion
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="valorTransaccion"></param>
        void ModificarAcumuladoContrato(int idContrato, decimal valorTransaccion);

        /// <summary>
        /// Validar el cupo de cliente
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="valorTransaccion"></param>
        /// <returns>"True" si se superó el porcentaje mínimo de aviso.</returns>
        bool ValidarCupoCliente(int idContrato, decimal valorTransaccion);

        /// <summary>
        /// Retorna la información básica de una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        CLSucursalDC ObtenerSucursal(int idSucursal, CLClientesDC cliente);

        CO.Servidor.Servicios.ContratoDatos.Clientes.CLClientesDC ObtenerCliente(int idCliente);

        /// <summary>
        /// Consulta información de un cliente por su nit
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        CLClientesDC ObtenerClientexNit(string nit);

        /// <summary>
        /// Consultar una lista de precios a partir del id del contrato
        /// </summary>
        /// <param name="idContrato">id contrato</param>
        /// <returns>Lista Orecios</returns>
        int ObtenerListaPrecioContrato(int idContrato);

        /// <summary>
        /// Registra destinatarios frecuentes
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="clienteContadoDestinatario"></param>
        /// <param name="descUltimoCentroServDestino"></param>
        /// <param name="idUltimoCentroServDestino"></param>
        void RegistrarClienteContado(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, string descUltimoCentroServDestino, long idUltimoCentroServDestino, string usuarioCreacion);

        /// <summary>
        /// Obtiene los clientes y sus contratos por agencia
        /// </summary>
        /// <param name="idAgencia">Identificador Agencia</param>
        /// <returns>Colección clientes y contratos</returns>
        List<CLClientesDC> ObtenerClientesContratosXAgencia(long idAgencia);

        /// <summary>
        /// Trae el listado de los clientes crédito y sus respectivos contratos por una agencia y todas las agencias dependientes de la misma
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        List<CLClientesDC> ObtenerCLientesContratosXAgenciaDependientes(long idAgencia);

        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        List<CLClientesDC> ObtenerClientesContratosGiros();

        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// y Cupo de Dispersion Aprobado
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        List<CLClientesDC> ObtenerTodosClientesContratosGiros();

        /// <summary>
        /// Almacenar a un cliente y acumular los valores
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        decimal GuardarClienteAcumularValores(CLClienteContadoDC clienteContado, decimal valorGiro);

        /// <summary>
        /// Adiciona, edita o elimina una de las condiciones para el servicio de giros para un cliente
        /// </summary>
        /// <param name="cuentaExterna">Objeto cuenta externa</param>
        void AdministrarClienteCondicionGiro(CLContratosDC contrato);

        /// <summary>
        /// Traduce a texto un id de notación consultando en la base de datos los valores de la misma
        /// </summary>
        /// <param name="idNotacionDia"></param>
        /// <returns></returns>
        string TraducirNotacionDiaInter(short idNotacionDia);

        /// <summary>
        /// Consulta el listado de sucursales que pertenecen a un agrupamiento de una factura
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        IEnumerable<CLSucursalDC> ConsultarSucursalesAgrupamientoFactura(int idAgrupamiento);

        /// <summary>
        /// Consulta el listado de servicios que pertenecen a un agrupamiento de una factura
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        IEnumerable<TAServicioDC> ConsultarServiciosAgrupamientoFactura(int idAgrupamiento);

        /// <summary>
        /// Consulta los requisitos asociados a un agrupamiento de una factura configurada
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        IEnumerable<string> ConsultarRequisitosAgrupamientoFactura(int idAgrupamiento);

        /// <summary>
        /// Cambia la agencia encargada de la sucursal
        /// </summary>
        /// <param name="AnteriorAgencia"></param>
        /// <param name="NuevaAgencia"></param>
        void ModificarAgenciaResponsableSucursal(long anteriorAgencia, long nuevaAgencia);

        /// <summary>
        /// Obtiene los clientes activos que tengan una sucursal activa por municipio
        /// </summary>
        List<CLClientesDC> ObtenerClientesSucursalesActivas(string idLocalidad);

        /// <summary>
        /// Obtiene las sucursales activas de un cliente
        /// </summary>
        /// <param name="idClienteCredito"></param>
        /// <returns></returns>
        List<CLSucursalDC> ObtenerSucursalesActivasCliente(int idClienteCredito);

        /// <summary>
        /// Obtener las sucursales de una Agencia
        /// </summary>
        /// <param name="idAgencia">id de la agencia encargada de las sucursales</param>
        /// <returns>Sucursales de la Agencia</returns>
        List<CLSucursalDC> ObtenerSucursalesPorIdAgencia(long idAgencia);

        /// <summary>
        /// Obtiene las sucursales activas de un cliente por ciudad de sucursal
        /// </summary>
        /// <param name="idClienteCredito"></param>
        /// <returns></returns>
        List<CLSucursalDC> ObtenerSucursalesXCiudadActivasCliente(int idClienteCredito, string idLocalidad);

        /// <summary>
        /// Obtiene los contratos activos por cliente
        /// </summary>
        /// <param name="idCliente">lista con los contratios activos por cliente</param>
        /// <returns>listas de contratos Activos</returns>
        IEnumerable<CLContratosDC> ObtenerContratosActivos(int idCliente);

        /// <summary>
        /// Obtiene las Sucursales por
        /// contrato
        /// </summary>
        /// <param name="idContrato">id del contrato</param>
        /// <returns>lista de Sucursales</returns>
        List<CLSucursalDC> ObtenerSucursalesPorContrato(int idContrato);

        /// <summary>
        /// Obtiene la lista de los contratos de las
        /// sucursales Activas
        /// </summary>
        /// <returns>lista de Contratos</returns>
        List<CLContratosDC> ObtenerContratosActivosDeSucursales();

        /// <summary>
        /// obtine el contrato de un cliente en una
        /// ciudad
        /// </summary>
        /// <param name="idCliente">id del cliente</param>
        /// <param name="idCiudad">id de la ciudad</param>
        /// <returns>lista de contratos del cliente en esa ciudad</returns>
        IEnumerable<CLContratosDC> ObtenerContratosClienteCiudad(int idCliente, string idCiudad);

        /// <summary>
        /// Obtiene las Sucursales por
        /// contrato y ciudad
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns>lista de Sucursales</returns>
        List<CLSucursalDC> ObtenerSucursalesPorContratoCiudad(int idContrato, string idCiudad);

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns>lista de Clientes</returns>
        IEnumerable<CLClientesDC> ObtenerClientes();


        
        /// <summary>
        /// Obtiene un cliente a partir del id
        /// </summary>
        /// <returns>Cliente</returns>
        CLClientesDC ObtenerClientexId(long idCliente);
    }
}