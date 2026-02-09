using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Clientes.Datos;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Clientes
{
  /// <summary>
  /// Clase de negocio para operaciones sobre cliente crédito
  /// </summary>
  internal class CLClienteCredito : ControllerBase
  {
    #region Singleton

    private static readonly CLClienteCredito instancia = (CLClienteCredito)FabricaInterceptores.GetProxy(new CLClienteCredito(), COConstantesModulos.CLIENTES);

    public static CLClienteCredito Instancia
    {
      get { return CLClienteCredito.instancia; }
    }

    #endregion Singleton

    /// <summary>
    /// Identificación del cliente
    /// </summary>
    private int idCliente
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public CLClienteCredito()
    {
    }

    public CLContrato Contrato
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public virtual void Dispose()
    {
    }

    /// <summary>
    /// Obtener información del cliente
    /// </summary>
    /// <param name="idCliente">Identificador del cliente</param>
    /// <param name="idServicio">Identificación del servicio</param>
    public void ObtenerCliente(int idCliente, int idServicio)
    {
    }

    /// <summary>
    /// Valida la información del cliente
    /// </summary>
    public void ValidarCliente()
    {
    }

    /// <summary>
    /// Validar el cupo de cliente
    /// </summary>
    /// <param name="idContrato"></param>
    /// <param name="valorTransaccion"></param>
    /// <returns>"True" si se superó el porcentaje mínimo de aviso.</returns>
    public bool ValidarCupoCliente(int idContrato, decimal valorTransaccion)
    {
      return CLContrato.Instancia.ValidarCupo(idContrato, valorTransaccion);
    }

    /// <summary>
    /// Modifica el acumulado de un contrato a partir de una valor de transaccion
    /// </summary>
    /// <param name="idContrato"></param>
    /// <param name="valorTransaccion"></param>
    public void ModificarAcumuladoContrato(int idContrato, decimal valorTransaccion)
    {
      CLRepositorio.Instancia.ModificarAcumuladoContrato(idContrato, valorTransaccion);
    }

    /// <summary>
    /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
    /// </summary>
    /// <returns>Colección clientes y contratos</returns>
    public List<CLClientesDC> ObtenerClientesContratosGiros()
    {
      return CLRepositorio.Instancia.ObtenerClientesContratosGiros();
    }

    /// <summary>
    /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
    /// y Cupo de Dispersion Aprobado
    /// </summary>
    /// <returns>Colección clientes y contratos</returns>
    public List<CLClientesDC> ObtenerTodosClientesContratosGiros()
    {
      return CLRepositorio.Instancia.ObtenerTodosClientesContratosGiros();
    }

    /// <summary>
    /// Adiciona, edita o elimina una de las condiciones para el servicio de giros de un cliente
    /// </summary>
    public void AdministrarClienteCondicionGiro(CLContratosDC contrato)
    {
      if (contrato.ClienteCondicionGiro.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
      {
        CLRepositorio.Instancia.AdicionarClienteCondicionGiro(contrato);
      }
      else if (contrato.ClienteCondicionGiro.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
      {
        CLRepositorio.Instancia.EditarClienteCondicionGiro(contrato);
      }
    }

    /// <summary>
    /// Cambia la agencia encargada de la sucursal
    /// </summary>
    /// <param name="AnteriorAgencia"></param>
    /// <param name="NuevaAgencia"></param>
    public void ModificarAgenciaResponsableSucursal(long anteriorAgencia, long nuevaAgencia)
    {
      CLRepositorio.Instancia.ModificarAgenciaResponsableSucursal(anteriorAgencia, nuevaAgencia);
    }

    /// <summary>
    /// Obtiene los clientes activos que tengan una sucursal activa por municipio
    /// </summary>
    public List<CLClientesDC> ObtenerClientesSucursalesActivas(string idLocalidad)
    {
      return CLRepositorio.Instancia.ObtenerClientesSucursalesActivas(idLocalidad);
    }

    /// <summary>
    /// Obtiene las sucursales activas de un cliente
    /// </summary>
    /// <param name="idClienteCredito"></param>
    /// <returns></returns>
    public List<CLSucursalDC> ObtenerSucursalesActivasCliente(int idClienteCredito)
    {
      return CLRepositorio.Instancia.ObtenerSucursalesActivasCliente(idClienteCredito);
    }

    /// <summary>
    /// Obtiene las sucursales activas de un cliente por ciudad de sucursal
    /// </summary>
    /// <param name="idClienteCredito"></param>
    /// <returns></returns>
    public List<CLSucursalDC> ObtenerSucursalesXCiudadActivasCliente(int idClienteCredito, string idLocalidad)
    {
      return CLRepositorio.Instancia.ObtenerSucursalesXCiudadActivasCliente(idClienteCredito, idLocalidad);
    }

    /// <summary>
    /// Obtiene las Sucursales por
    /// contrato
    /// </summary>
    /// <param name="idContrato">id del contrato</param>
    /// <returns>lista de Sucursales</returns>
    public List<CLSucursalDC> ObtenerSucursalesPorContrato(int idContrato)
    {
      return CLRepositorio.Instancia.ObtenerSucursalesPorContrato(idContrato);
    }

    /// <summary>
    /// Obtiene las Sucursales por
    /// contrato y ciudad
    /// </summary>
    /// <param name="idContrato"></param>
    /// <returns>lista de Sucursales</returns>
    public List<CLSucursalDC> ObtenerSucursalesPorContratoCiudad(int idContrato, string idCiudad)
    {
      return CLRepositorio.Instancia.ObtenerSucursalesPorContratoCiudad(idContrato, idCiudad);
    }

    /// <summary>
    /// Obtiene la lista de los contratos de las
    /// sucursales Activas
    /// </summary>
    /// <returns>lista de Contratos</returns>
    public List<CLContratosDC> ObtenerContratosActivosDeSucursales()
    {
      return CLRepositorio.Instancia.ObtenerContratosActivosDeSucursales();
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
      return CLRepositorio.Instancia.ObtenerContratosClienteCiudad(idCliente, idCiudad);
    }

    /// <summary>
    /// Obtiene una lista de todos los clientes crédito tienen convenio
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CLClientesDC> ObtenerTodosClientesConvenio()
    {
        return CLRepositorio.Instancia.ObtenerTodosClientesConvenio();
    }

    #region Referencia uso guia

    /// <summary>
    /// Inserta Modifica o elimina una  referencia uso guia interna
    /// </summary>
    /// <param name="referencia"></param>
    public void ActualizarReferenciaUsoGuia(CLReferenciaUsoGuiaDC referencia)
    {
      switch (referencia.EstadoRegistro)
      {
        case EnumEstadoRegistro.ADICIONADO:
          CLRepositorio.Instancia.AgregarReferenciaUsoGuia(referencia);
          break;

        case EnumEstadoRegistro.MODIFICADO:
          CLRepositorio.Instancia.EditarReferenciaUsoGuia(referencia);
          break;

        case EnumEstadoRegistro.BORRADO:
          CLRepositorio.Instancia.BorrarReferenciaUsoGuia(referencia);
          break;
      }
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
      return CLRepositorio.Instancia.ObtenerReferenciaUsoGuia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtiene la referencia de uso guia por sucursal
    /// </summary>
    /// <param name="idSucursal"></param>
    /// <returns></returns>
    public CLReferenciaUsoGuiaDC ObtenerReferenciaUsoGuiaPorSucursal(int idSucursal)
    {
      return CLRepositorio.Instancia.ObtenerReferenciaUsoGuiaPorSucursal(idSucursal);
    }

    #endregion Referencia uso guia
  }
}