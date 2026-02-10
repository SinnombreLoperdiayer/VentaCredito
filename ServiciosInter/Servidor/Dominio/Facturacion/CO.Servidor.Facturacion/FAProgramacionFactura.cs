using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Facturacion.Comun;
using CO.Servidor.Facturacion.Datos;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Facturacion
{
  public class FAProgramacionFactura : ControllerBase
  {
    #region CrearInstancia

    private static readonly FAProgramacionFactura instancia = (FAProgramacionFactura)FabricaInterceptores.GetProxy(new FAProgramacionFactura(), COConstantesModulos.MODULO_FACTURACION);

    /// <summary>
    /// Retorna una instancia de programación de facturas
    /// /// </summary>
    public static FAProgramacionFactura Instancia
    {
      get { return FAProgramacionFactura.instancia; }
    }

    #endregion CrearInstancia

    #region Metodos Públicos

    /// <summary>
    /// Consulta las programaciones que existen ya creadas en el sistema según los criterios de búsqueda establecidos
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public IEnumerable<FAProgramacionFacturaDC> ConsultarProgramacionesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return FARepositorioProgrFactura.Instancia.ConsultarProgramacionesFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out  totalRegistros);
    }

    /// <summary>
    /// Consulta las programaciones no ejecutadas de un cliente y un rango de fechas
    /// </summary>        
    /// <returns></returns>
    public IEnumerable<FAProgramacionFacturaDC> ConsultarProgramacionesNoEjecutadas(DateTime fechaDesde, DateTime fechaHasta, int idCliente)
    {
        return FARepositorioProgrFactura.Instancia.ConsultarProgramacionesNoEjecutadas(fechaDesde, fechaHasta, idCliente);
    }

    /// <summary>
    /// Inserta una nueva exclusión asociada a una programación
    /// </summary>
    /// <param name="exclusion"></param>
    public List<FAExclusionProgramacionDC> ExcluirMovimientoDeProgramacion(FAExclusionProgramacionDC exclusion)
    {
      ICAFachadaCajas FachadaCaja = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
      List<CAOperacionDeClienteDC> OperacionesCliente = FachadaCaja.ConsultarClientePropDeOperacion(exclusion.NumeroOperacion, exclusion.NumeroOperacionHasta);
      if (OperacionesCliente == null || OperacionesCliente.Count==0)
        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_FACTURACION, FAEnumTipoErrorFacturacion.EX_MOVIMIENTO_NOASIGNADO.ToString(), FAMensajesFacturacion.CargarMensaje(FAEnumTipoErrorFacturacion.EX_MOVIMIENTO_NOASIGNADO)));

      List<FAExclusionProgramacionDC> exclusiones = new List<FAExclusionProgramacionDC>();

      using (TransactionScope scope = new TransactionScope())
      {
          OperacionesCliente.ForEach((OperacionCliente) =>
          {
              if (FARepositorioProgrFactura.Instancia.MovimientoYaFacturado(OperacionCliente.IdMovimiento, OperacionCliente.IdServicio))
                  throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_FACTURACION, "0", "El número de movimiento " + exclusion.NumeroOperacion + " ya se encuentra facturado"));

              FAExclusionProgramacionDC exc = new FAExclusionProgramacionDC()
              {
                  NombreServicio = OperacionCliente.NombreServicio,
                  IdServicio = OperacionCliente.IdServicio,
                  IdProgramacion=exclusion.IdProgramacion,
                  Valor = OperacionCliente.ValorOperacion,
                  CreadoPor = ControllerContext.Current.Usuario,
                  NumeroOperacion=OperacionCliente.IdMovimiento,
                  NumeroOperacionHasta=OperacionCliente.IdMovimiento,
                  FechaGrabacion = DateTime.Now,
                  FechaOperacion = OperacionCliente.FechaOperacion
              };

              FARepositorioProgrFactura.Instancia.ExcluirMovimientoDeProgramacion(exc);
              exclusiones.Add(exc);
          });
          scope.Complete();
      }
      return exclusiones;
    }

    /// <summary>
    /// Permite deshacer la exclusión de un movimiento ya excluido previamente
    /// </summary>
    /// <param name="exclusion">Exclusión que se desea deshacer</param>
    public void DeshacerExclusionDeMovimiento(FAExclusionProgramacionDC exclusion)
    {
      FARepositorioProgrFactura.Instancia.DeshacerExclusionDeMovimiento(exclusion);
    }

    public long GenerarFacturaProgramada(FAProgramacionFacturaDC programacion)
    {
      long idFacturaCredito = 0;
      long noUltimaFactura = 0;
      //Se crean dos transacciones ya que esta transacción debe tener un periodo corto de vida porque el procedimiento
      //almacenado es muy concurrente
      using (TransactionScope scope = new TransactionScope())
      {
        noUltimaFactura=FARepositorioProgrFactura.Instancia.ConsultarNoUltimaFactura();        
        ISUFachadaSuministros FachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
        FachadaSuministros.ActualizarNumeroActualSuministro(SUEnumSuministro.FACTURA_COBRO_CLIENTE_CREDITO, noUltimaFactura);
        idFacturaCredito = FachadaSuministros.ObtenerConsecutivoSuministro(SUEnumSuministro.FACTURA_COBRO_CLIENTE_CREDITO);
        scope.Complete();   
      }
      
      FARepositorioProgrFactura.Instancia.GenerarFacturaProgramada(programacion, noUltimaFactura+1);   
   
      return idFacturaCredito;
    }

    /// <summary>
    /// Agrega un concepto nuevo a una programación de una factura.
    /// </summary>
    /// <param name="concepto"></param>
    public int GuardaConceptoProgramado(FAConceptoProgramadoDC concepto)
    {
      return FARepositorioProgrFactura.Instancia.GuardaConceptoProgramado(concepto);
    }

    /// <summary>
    /// Elimina un concepto de una programación
    /// </summary>
    /// <param name="concepto"></param>
    public void EliminarConceptoProgramacion(FAConceptoProgramadoDC concepto)
    {
      FARepositorioProgrFactura.Instancia.EliminarConceptoProgramacion(concepto);
    }

    /// <summary>
    /// Cambia la programación de una factura a una nueva fecha
    /// </summary>
    /// <param name="nuevaProgramacion"></param>
    public void CambiarProgramacionFactura(FAProgramacionFacturaDC nuevaProgramacion, DateTime nuevaFecha)
    {
      FARepositorioProgrFactura.Instancia.CambiarProgramacionFactura(nuevaProgramacion, nuevaFecha);
    }

    #endregion Metodos Públicos
  }
}