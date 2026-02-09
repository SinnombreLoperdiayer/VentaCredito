using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Versionamiento;
using Framework.Servidor.Servicios.Contratos;
using Framework.Servidor.Versionamiento;

namespace Framework.Servidor.Servicios.Implementacion
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class VEVersionamientoSvc : IVEVersionamientoSvc
  {
    public VEVersionamientoSvc()
    {
      Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
    }

    /// <summary>
    /// Valida el Token de la sesión del equipo solicitante
    /// </summary>
    /// <param name="token">Identificador de la sesión de usuario</param>
    /// <param name="idMaquina">Identificador de la máquina</param>
    /// <returns>Se retorna si el token fué validado o no</returns>
    public bool ValidarToken(string token, string idMaquina)
    {
      return VEVersion.Instancia.ValidarToken(token, idMaquina);
    }

    /// <summary>
    /// Se inicia la solicitud de alta del punto o agencia
    /// </summary>
    /// <param name="idPuntoAtencion">Identificador del punto o agencia a solicitar alta</param>
    /// <param name="caja">Número de caja a registrar</param>
    /// <param name="idMaquina">Identificador de la máquina</param>
    /// <param name="nombre">Nombre del usuario que hace la solicitud</param>
    public void SolicitarAltaMaquina(string idMaquina, string usuario)
    {
      VEVersion.Instancia.SolicitarAltaMaquina(idMaquina, usuario);
    }

    /// <summary>
    /// Retorna la lista de módulos de la aplicación
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VEModulo> ObtenerModulos()
    {
      return VERepositorio.Instancia.ObtenerModulos();
    }

    /// <summary>
    ///  Calcula la versión de acuerdo a un identificador de máquina
    /// </summary>
    public VEVersionInfo CalcularVersionxIdMaquina(string idMaquina, string token, VEDatosIngresoUsuario datosIngreso)
    {
      return VEVersion.Instancia.CalcularVersionxIdMaquina(idMaquina, token, datosIngreso);
    }

    /// <summary>
    /// Calcula la versión de acuerdoa un centro de servicios
    /// </summary>
    public VEVersionInfo CalcularVersionxCentroServicios(long idCentroServicios, VEDatosIngresoUsuario datosIngreso)
    {
      return VEVersion.Instancia.CalcularVersionxCentroServicios(idCentroServicios, datosIngreso);
    }

    /// <summary>
    /// Calcula la versión de acuerdo a una gestión
    /// </summary>
    public VEVersionInfo CalcularVersionxGestion(string idGestion, VEDatosIngresoUsuario datosIngreso)
    {
      return VEVersion.Instancia.CalcularVersionxGestion(idGestion, datosIngreso);
    }

    /// <summary>
    /// Calcula la versión de acuerdo a una sucursal de un cliente crédito
    /// </summary>
    public VEVersionInfo CalcularVersionxSucursal(int idSucursal, VEDatosIngresoUsuario datosIngreso)
    {
      return VEVersion.Instancia.CalcularVersionxSucursal(idSucursal, datosIngreso);
    }

    /// <summary>
    /// Obtiene la lista de clientes crédito
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VECliente> ObtenerClientes()
    {
      return VEVersion.Instancia.ObtenerClientes();
    }

    /// <summary>
    /// Retorna lista de sucursales
    /// </summary>
    /// <param name="idCliente"></param>
    /// <returns></returns>
    public IEnumerable<Framework.Servidor.Servicios.ContratoDatos.Versionamiento.VESucursal> ObtenerSucursales(int idCliente)
    {
      return VEVersion.Instancia.ObtenerSucursales(idCliente);
    }

    /// <summary>
    /// Retorna información de una sucursal de un cliente
    /// </summary>
    /// <param name="idSucursal">Identificador de la sucursal</param>
    /// <returns></returns>
    public VESucursal ObtenerInfoSucursal(int idSucursal)
    {
      return VEVersion.Instancia.ObtenerInfoSucursal(idSucursal);
    }

    /// <summary>
    /// Retorna el id del cargo del usuario autenticado
    /// </summary>
    /// <param name="idUsuario"></param>
    /// <returns></returns>
    public int CalcularCargoUsuarioAutenticado(string idUsuario)
    {
      return VEVersion.Instancia.CalcularCargoUsuarioAutenticado(idUsuario);
    }

    /// <summary>
    /// Consulta las urls de los servicios disponibles en controller
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> ObtenerUrlsServicios()
    {
        return VEVersion.Instancia.ObtenerUrlsServicios();
    }

    /// <summary>
    /// Obtiene la lista de menus capacitación
    /// </summary>       
    public List<VEMenuCapacitacion> ObtenerMenusCapacitacion()
    {
        return VEVersion.Instancia.ObtenerMenusCapacitacion();
    }
  }
}