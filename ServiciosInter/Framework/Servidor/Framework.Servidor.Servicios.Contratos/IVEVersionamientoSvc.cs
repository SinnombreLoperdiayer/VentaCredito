using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Versionamiento;

namespace Framework.Servidor.Servicios.Contratos
{
  /// <summary>
  /// Servicio que provee operaciones de versionamiento
  /// </summary>
  [ServiceContract(Namespace = "http://contrologis.com")]
  public interface IVEVersionamientoSvc
  {
    /// <summary>
    /// Valida el Token de la sesión del equipo solicitante
    /// </summary>
    /// <param name="token">Identificador de la sesión de usuario</param>
    /// <param name="idMaquina">Identificador de la máquina</param>
    /// <returns>Se retorna si el token fué validado o no</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    bool ValidarToken(string token, string idMaquina);

    /// <summary>
    /// Se inicia la solicitud de alta del punto o agencia
    /// </summary>
    /// <param name="idPuntoAtencion">Identificador del punto o agencia a solicitar alta</param>
    /// <param name="caja">Número de caja a registrar</param>
    /// <param name="idMaquina">Identificador de la máquina</param>
    /// <param name="nombre">Nombre del usuario que hace la solicitud</param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void SolicitarAltaMaquina(string idMaquina, string usuario);

    /// <summary>
    /// Retorna la lista de módulos de la aplicación
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<VEModulo> ObtenerModulos();

    /// <summary>
    ///  Calcula la versión de acuerdo a un identificador de máquina
    /// </summary>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    VEVersionInfo CalcularVersionxIdMaquina(string idMaquina, string token, VEDatosIngresoUsuario datosIngreso);

    /// <summary>
    /// Calcula la versión de acuerdoa un centro de servicios
    /// </summary>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    VEVersionInfo CalcularVersionxCentroServicios(long idRacol, VEDatosIngresoUsuario datosIngreso);

    /// <summary>
    /// Calcula la versión de acuerdo a una gestión
    /// </summary>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    VEVersionInfo CalcularVersionxGestion(string idGestion, VEDatosIngresoUsuario datosIngreso);

    /// <summary>
    /// Calcula la versión de acuerdo a una sucursal de un cliente crédito
    /// </summary>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    VEVersionInfo CalcularVersionxSucursal(int idCliente, VEDatosIngresoUsuario datosIngreso);

    /// <summary>
    /// Obtiene la lista de clientes crédito
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<Framework.Servidor.Servicios.ContratoDatos.Versionamiento.VECliente> ObtenerClientes();

    /// <summary>
    /// Retorna lista de sucursales
    /// </summary>
    /// <param name="idCliente"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<Framework.Servidor.Servicios.ContratoDatos.Versionamiento.VESucursal> ObtenerSucursales(int idCliente);

    /// <summary>
    /// Retorna información de una sucursal de un cliente
    /// </summary>
    /// <param name="idSucursal">Identificador de la sucursal</param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    VESucursal ObtenerInfoSucursal(int idSucursal);

    /// <summary>
    /// Calcula el id del cargo del usuario autenticado
    /// </summary>
    /// <param name="idUsuario"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    int CalcularCargoUsuarioAutenticado(string idUsuario);

    /// <summary>
    /// Consulta las urls de los servicios disponibles en controller
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    Dictionary<string, string> ObtenerUrlsServicios();

    /// <summary>
    /// Obtiene la lista de menus capacitación
    /// </summary>    
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    List<VEMenuCapacitacion> ObtenerMenusCapacitacion();
  }
}