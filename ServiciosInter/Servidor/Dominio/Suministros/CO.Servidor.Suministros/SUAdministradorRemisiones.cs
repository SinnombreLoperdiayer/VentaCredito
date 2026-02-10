using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Remision;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Suministros
{
  public class SUAdministradorRemisiones : ControllerBase
  {
    private static readonly SUAdministradorRemisiones instancia = (SUAdministradorRemisiones)FabricaInterceptores.GetProxy(new SUAdministradorRemisiones(), COConstantesModulos.MODULO_SUMINISTROS);

    /// <summary>
    /// Retorna una instancia de administrador de suministros
    /// /// </summary>
    public static SUAdministradorRemisiones Instancia
    {
      get { return SUAdministradorRemisiones.instancia; }
    }

    /// <summary>
    /// Retorna instancia del configurador de suministros
    /// </summary>
    internal CO.Servidor.Suministros.Remision.SUProvisionMensajero Provision
    {
      get
      {
        return new SUProvisionMensajero();
      }
    }

    /// <summary>
    /// Retorna instancia del configurador de suministros de proceso
    /// </summary>
    internal CO.Servidor.Suministros.Remision.SUProvisionProceso ProvisionProceso
    {
      get
      {
        return new SUProvisionProceso();
      }
    }

    /// <summary>
    /// Retorna instancia del configurador de suministros
    /// </summary>
    internal CO.Servidor.Suministros.Remision.SURemision Remision
    {
      get
      {
        return new SURemision();
      }
    }

    /// <summary>
    /// Retorna instancia del configurador de suministros
    /// </summary>
    internal CO.Servidor.Suministros.Remision.SUProvisionCanalVenta ProvisionCanalVenta
    {
      get
      {
        return new SUProvisionCanalVenta();
      }
    }

    /// <summary>
    /// Retorna instancia del configurador de suministros
    /// </summary>
    internal CO.Servidor.Suministros.Remision.SUProvisionCliente ProvisionCliente
    {
      get
      {
        return new SUProvisionCliente();
      }
    }

    /// <summary>
    /// Valida la asignacion del suministro
    /// </summary>
    /// <param name="suministro"></param>
    public void ValidaSuministroAsignacion(SUSuministro suministro)
    {
      Remision.ValidaSuministroAsignacion(suministro);
    }

    /// <summary>
    /// Genera la remision de suministros para el mensajero
    /// </summary>
    /// <param name="remision"></param>
    public long AdminRemisionMensajero(SURemisionSuministroDC remision)
    {
      return Provision.AdminRemisionMensajero(remision);
    }

    #region Proceso

    /// <summary>
    /// Genera la remisión de suministros para el proceso
    /// </summary>
    /// <param name="remision"></param>
    public long AdminRemisionProceso(SURemisionSuministroDC remision)
    {
      return ProvisionProceso.AdminRemisionProceso(remision);
    }

    #endregion Proceso

    #region Canal de venta

    /// <summary>
    /// Obtiene los suministros asignados al canal de venta
    /// </summary>
    /// <param name="idCentroServicios"></param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosAsignadosCanalVenta(long idCentroServicios)
    {
      return ProvisionCanalVenta.ObtenerSuministrosAsignadosCanalVenta(idCentroServicios);
    }

    /// <summary>
    /// Genera la remision de los suministros para el canal de ventas
    /// </summary>
    /// <param name="remision"></param>
    public long AdminRemisionCanalVenta(SURemisionSuministroDC remision)
    {
      return ProvisionCanalVenta.AdminRemisionCanalVenta(remision);
    }

    /// <summary>
    /// Genera la remision de los suministros para el canal de ventas
    /// </summary>
    /// <param name="remision"></param>
    public SURemisionSuministroDC GenerarRangoGuiaManualOffline(long idCentroServicio, int cantidad)
    {
       return ProvisionCanalVenta.GenerarRangoGuiaManualOffline(idCentroServicio, cantidad);
    }

    #endregion Canal de venta

    #region Clientes

    /// <summary>
    /// Obtiene los clientes activos con una sucursal activa por localidad
    /// </summary>
    /// <param name="idLocalidad"></param>
    /// <returns></returns>
    public List<CLClientesDC> ObtenerClientesConSucursalActiva(string idLocalidad)
    {
      return ProvisionCliente.ObtenerClientesConSucursalActiva(idLocalidad);
    }

    /// <summary>
    /// Obtiene las sucursales activas del cliente seleccionado
    /// </summary>
    /// <param name="idCliente"></param>
    /// <returns></returns>
    public List<CLSucursalDC> ObtenerSucursalesActivasCliente(int idCliente, string idLocalidad)
    {
      return ProvisionCliente.ObtenerSucursalesActivasCliente(idCliente, idLocalidad);
    }

    /// <summary>
    /// Obtiene los contratos activos por cliente
    /// </summary>
    /// <param name="idCliente">lista con los contratios activos por cliente</param>
    /// <returns>lista de contratos</returns>
    public IEnumerable<CLContratosDC> ObtenerContratosActivosClientes(int idCliente)
    {
      return ProvisionCliente.ObtenerContratosActivosClientes(idCliente);
    }

    /// <summary>
    /// Obtiene las Sucursales por
    /// contrato
    /// </summary>
    /// <param name="idContrato">id del contrato</param>
    /// <returns>lista de Sucursales</returns>
    public List<CLSucursalDC> ObtenerSucursalesPorContrato(int idContrato)
    {
      return ProvisionCliente.ObtenerSucursalesPorContrato(idContrato);
    }

    /// <summary>
    /// Obtiene las Sucursales por
    /// contrato y ciudad
    /// </summary>
    /// <param name="idContrato"></param>
    /// <returns>lista de Sucursales</returns>
    public List<CLSucursalDC> ObtenerSucursalesPorContratoCiudad(int idContrato, string idCiudad)
    {
      return ProvisionCliente.ObtenerSucursalesPorContratoCiudad(idContrato, idCiudad);
    }

    /// <summary>
    /// Obtiene la lista de los contratos de las
    /// sucursales Activas
    /// </summary>
    /// <returns>lista de Contratos</returns>
    public List<CLContratosDC> ObtenerContratosActivosDeSucursales()
    {
      return ProvisionCliente.ObtenerContratosActivosDeSucursales();
    }

    /// <summary>
    /// obtiene el contrato de un cliente en una
    /// ciudad
    /// </summary>
    /// <param name="idCliente">id del cliente</param>
    /// <param name="idCiudad">id de la ciudad</param>
    /// <returns>lista de contratos del cliente en esa ciudad</returns>
    public IEnumerable<CLContratosDC> ObtenerContratosClienteCiudad(int idCliente, string idCiudad)
    {
      return ProvisionCliente.ObtenerContratosClienteCiudad(idCliente, idCiudad);
    }

    /// <summary>
    /// Obtiene los suministros asignados a la sucursal
    /// </summary>
    /// <param name="idSucursal"></param>
    public List<SUSuministro> ObtenerSuministrosAsignadoSucursal(int idSucursal)
    {
      return ProvisionCliente.ObtenerSuministrosAsignadoSucursal(idSucursal);
    }

    /// <summary>
    /// Genera la remision de suministros para el mensajero
    /// </summary>
    /// <param name="remision"></param>
    public long AdminRemisionCliente(SURemisionSuministroDC remision)
    {
      return ProvisionCliente.AdminRemisionCliente(remision);
    }

    #endregion Clientes

    #region desasignacion de suministros

    /// <summary>
    /// Obtiene los grupos de suministros
    /// </summary>
    /// <returns></returns>
    public List<SUGrupoSuministrosDC> ObtenerGruposSuministros()
    {
      return Remision.ObtenerGruposSuministros();
    }

    /// <summary>
    /// Realiza la desasignacion de suministros de un mensajero
    /// </summary>
    public void DesasignarSuministrosRemision(SURemisionSuministroDC remision)
    {
      ///False, porque el proceso no corresponde a una modificacion, sino a una desasignacion
      Remision.DesasignarSuministrosRemision(remision, false);
    }

    /// <summary>
    /// Retorna los suministros que esten en el rango de fecha seleccionado
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <returns></returns>
    public List<SURemisionSuministroDC> ObtenerSuministrosRemisionXRangoFecha(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, string idGrupo)
    {
      return Remision.ObtenerSuministrosRemisionXRangoFecha(filtro, indicePagina, registrosPorPagina, idGrupo);
    }

    #endregion desasignacion de suministros

    #region Modificar Provision Suministro

    /// <summary>
    /// Administra la remision del suministro
    /// </summary>
    /// <param name="remisionDestino"></param>
    /// <param name="remisionOrigen"></param>
    public long AdministrarModificacionSuministro(SURemisionSuministroDC remisionDestino, SURemisionSuministroDC remisionOrigen)
    {
      return Remision.AdministrarModificacionSuministro(remisionDestino, remisionOrigen);
    }

    #endregion Modificar Provision Suministro
  }
}