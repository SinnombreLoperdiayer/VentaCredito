using System.Collections.Generic;
using System.ServiceModel;
using CO.Servidor.Clientes.Comun;
using CO.Servidor.Clientes.Datos;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Clientes
{
  /// <summary>
  /// Clase para manejo de las operaciones de dominio sobre los contratos de un cliente
  /// </summary>
  internal class CLContrato
  {
    /// <summary>
    /// Identificador del Contrato
    /// </summary>
    private int idContrato
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    private static readonly CLContrato instancia = new CLContrato();

    /// <summary>
    /// Retorna la instancia de la clase TARepositorio
    /// </summary>
    public static CLContrato Instancia
    {
      get { return CLContrato.instancia; }
    }

    /// <summary>
    /// Número del Contrato
    /// </summary>
    private string numeroContrato
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    private CLCupoCliente cupoCliente
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public CLContrato()
    {
    }

    ~CLContrato()
    {
    }

    public CLCupoCliente CupoCliente
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
    /// Confirmar la reserva del cupo
    /// </summary>
    /// <param name="idReservaCupo">Identificador de la reserva del cupo</param>
    private void ConfirmarCupo(int idReservaCupo)
    {
    }

    /// <summary>
    /// Reservar cupo del cupo disponible
    /// </summary>
    /// <param name="montoReservar">Monto a reservar</param>
    private int HacerReservaCupo(double montoReservar)
    {
      return 0;
    }

    /// <summary>
    /// Validar los cupos de:
    /// Valor del Contrato
    /// Presupuesto mensual
    /// </summary>
    /// <param name="idContrato">Identificación del Contrato</param>
    /// <param name="valorTransaccion">Valor de la transacción</param>
    /// <returns>"True" si se superó el porcentaje mínimo de aviso.</returns>
    public bool ValidarCupo(int idContrato, decimal valorTransaccion)
    {
       return CLRepositorio.Instancia.ValidarCupoContrato (idContrato,valorTransaccion);

      //bool superoPorcentajeMinimoAviso = ValidarCupoContrato(idContrato, valorTransaccion);
      //CLCupoCliente.Instancia.ValidarCupoPeriodo(idContrato, valorTransaccion);
      //return superoPorcentajeMinimoAviso;
    }

    /// <summary>
    /// Metodo que obtiene el acumulado del consumo del contrato
    /// </summary>
    /// <param name="idContrato"></param>
    /// <returns>decimal con el valor</returns>
    private decimal ObtenerConsumoContrato(int idContrato)
    {
      return CLRepositorio.Instancia.ObtenerConsumoContrato(idContrato);
    }

    /// <summary>
    /// Metodo que obtiene el valor del presupuesto del contrato
    /// </summary>
    /// <param name="idContrato"></param>
    /// <returns>decimal con el valor</returns>
    private decimal ObtenerValorPresupuestoContrato(int idContrato)
    {
      return CLRepositorio.Instancia.ObtenerValorPresupuestoContrato(idContrato) +
        CLRepositorio.Instancia.ObtenerValorPresupuestoOtrosi(idContrato);
    }

    /// <summary>
    /// Valida el estado del Contrato
    /// </summary>
    /// <returns>"True" si se superó el porcentaje mínimo de aviso</returns>
    private bool ValidarCupoContrato(int idContrato, decimal valorTransaccion)
    {
      decimal consumoContrato = ObtenerConsumoContrato(idContrato);
      decimal presupuestoContrato = ObtenerValorPresupuestoContrato(idContrato);
      decimal consumoTotal = consumoContrato + valorTransaccion;
      if (presupuestoContrato < consumoTotal)
      {
        var x = new ControllerException
             (
             COConstantesModulos.CLIENTES,
             CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO.ToString(),
             CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO)
             );
        throw new FaultException<ControllerException>(x);
      }
      else
      {
        decimal porcentajeAviso = CLRepositorio.Instancia.ObtenerPorcentajeAvisoContrato(idContrato);
        decimal porcentajeConsumoActual = consumoTotal * 100 / presupuestoContrato;
        return porcentajeConsumoActual >= porcentajeAviso;
      }
    }

    /// <summary>
    /// Consultar una lista de precios a partir del id del contrato
    /// </summary>
    /// <param name="idContrato">id contrato</param>
    /// <returns>Lista Orecios</returns>
    public int ObtenerListaPrecioContrato(int idContrato)
    {
      return CLRepositorio.Instancia.ObtenerListaPrecioContrato(idContrato);
    }
  }
}