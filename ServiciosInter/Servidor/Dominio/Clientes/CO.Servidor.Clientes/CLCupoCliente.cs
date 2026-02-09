using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Clientes.Comun;
using CO.Servidor.Clientes.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Clientes
{
  /// <summary>
  /// Clase para las operaciones sobre el cupo de un contrato de un cliente
  /// </summary>
  internal class CLCupoCliente : IDisposable
  {
    /// <summary>
    /// Período de expiración en minutos
    /// </summary>
    private int periodoExpiracion
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public CLCupoCliente()
    {
    }

    ~CLCupoCliente()
    {
    }

    private static readonly CLCupoCliente instancia = new CLCupoCliente();

    /// <summary>
    /// Retorna la instancia de la clase TARepositorio
    /// </summary>
    public static CLCupoCliente Instancia
    {
      get { return CLCupoCliente.instancia; }
    }

    public virtual void Dispose()
    {
    }

    /// <summary>
    /// Reservar cupo de un Contrato de un cliente
    /// </summary>
    /// <param name="idCliente">Identificador del cliente</param>
    /// <param name="monto">Monto a reservar</param>
    private static void DescontarCupo(int idCliente, double monto)
    {
    }

    /// <summary>
    /// Metodo que obtiene la fecha de corte de facturacion
    /// </summary>
    /// <param name="idContrato"></param>
    /// <returns>fecha de corte</returns>
    private DateTime ObtenerFechaCorteFacturacion(int idContrato)
    {
      return CLRepositorio.Instancia.ObtenerFechaCorteFacturacion(idContrato);
    }

    /// <summary>
    /// Metodo que obtiene el consumo del periodo a partir de la fecha de facturacion
    /// </summary>
    /// <param name="fechaInicial"></param>
    /// <param name="idContrato"></param>
    /// <returns>valor consumo periodo</returns>
    private decimal ObtenerConsumoPeriodo(int idContrato)
    {
      return CLRepositorio.Instancia.ObtenerConsumoPeriodo(idContrato);
    }

    /// <summary>
    /// etodo que obtiene el valor del presupuesto mensual del contrato
    /// </summary>
    /// <param name="idContrato"></param>
    /// <returns></returns>
    private decimal ObtenerValorPresupuestoPeriodo(int idContrato)
    {
      return CLRepositorio.Instancia.ObtenerValorPresupuestoPeriodo(idContrato);
    }

    /// <summary>
    /// Retorna el valor total del contrato
    /// </summary>
    /// <param name="idContrato"></param>
    /// <returns></returns>
    private decimal ObtenerValorPresupuestoTotal(int idContrato)
    {
      return CLRepositorio.Instancia.ObtenerValorPresupuestoTotal(idContrato);
    }

    /// <summary>
    /// Metodo que valida el cupo del contrato en un periodo de facturación
    /// </summary>
    /// <param name="idContrato"></param>
    /// <param name="valorTransaccion"></param>
    public void ValidarCupoPeriodo(int idContrato, decimal valorTransaccion)
    {
      var errorCupo = new ControllerException
           (
           COConstantesModulos.CLIENTES,
           CLEnumTipoErrorCliente.EX_FALLO_PRESU_CONTRATO.ToString(),
           CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_PRESU_CONTRATO)
           );
      decimal presupuestoContrato = ObtenerValorPresupuestoTotal(idContrato);
      if (valorTransaccion > presupuestoContrato)
        throw new FaultException<ControllerException>(errorCupo);
      else
      {
        decimal consumoTotal = ObtenerConsumoPeriodo(idContrato);
        if (presupuestoContrato < (consumoTotal + valorTransaccion))
          throw new FaultException<ControllerException>(errorCupo);
      }
    }

    /// <summary>
    /// Metodo que valida el cupo del contrato en un periodo de facturación
    /// </summary>
    /// <param name="idContrato"></param>
    /// <param name="valorTransaccion"></param>
    public bool ValidaCupoPresupuestoMensual(int idContrato, decimal valorTransaccion)
    {
      decimal presupuestoMes = ObtenerValorPresupuestoPeriodo(idContrato);

      if (presupuestoMes < valorTransaccion)
        return false;
      else
      {
        decimal consumoMes = ObtenerConsumoPeriodo(idContrato);
        if (presupuestoMes < (consumoMes + valorTransaccion))
          return false;
        else
          return true;
      }
    }

    /// <summary>
    /// Incializar clase de reserva de cupo
    /// </summary>
    public static void Inicializar()
    {
    }
  }
}