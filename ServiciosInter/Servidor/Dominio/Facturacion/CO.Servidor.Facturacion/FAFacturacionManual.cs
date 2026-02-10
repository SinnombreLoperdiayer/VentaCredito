using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Facturacion.Datos;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Dominio.Comun.Tarifas;

namespace CO.Servidor.Facturacion
{
  /// <summary>
  /// Clase administradora del módulo de facturación
  /// </summary>
  public class FAFacturacionManual : ControllerBase
  {
    #region CrearInstancia

    private static readonly FAFacturacionManual instancia = (FAFacturacionManual)FabricaInterceptores.GetProxy(new FAFacturacionManual(), COConstantesModulos.MODULO_FACTURACION);

    /// <summary>
    /// Retorna una instancia de facturacion manual
    /// /// </summary>
    public static FAFacturacionManual Instancia
    {
      get { return FAFacturacionManual.instancia; }
    }

    #endregion CrearInstancia

    /// <summary>
    /// Almacena una factura nueva manual
    /// </summary>
    /// <param name="facturaManual"></param>
    public long GuardarFacturaManual(FAFacturaClienteDC facturaManual)
    {
      ISUFachadaSuministros FachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
      long idFacturaManual;
      //Se crean dos transacciones ya que esta transacción debe tener un periodo corto de vida porque el procedimiento
      //almacenado es muy concurrente
      using (TransactionScope scope = new TransactionScope())
      {
          long noUltimaFactura = FARepositorioProgrFactura.Instancia.ConsultarNoUltimaFactura();
          FachadaSuministros.ActualizarNumeroActualSuministro(SUEnumSuministro.FACTURA_COBRO_CLIENTE_CREDITO, noUltimaFactura);
          idFacturaManual = FachadaSuministros.ObtenerConsecutivoSuministro(SUEnumSuministro.FACTURA_COBRO_CLIENTE_CREDITO);
          facturaManual.NumeroFactura = idFacturaManual;
          scope.Complete();
      }
      using (TransactionScope scope = new TransactionScope())
      {
        FARepositorioFacManual.Instancia.GuardarFacturaManual(facturaManual);
        scope.Complete();
      }

      return idFacturaManual;
    }
  }
}