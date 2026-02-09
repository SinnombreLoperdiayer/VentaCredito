using System;
using System.Collections.Generic;
using CO.Servidor.Admisiones.Giros.Comun;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;

namespace CO.Servidor.Admisiones.Giros.TransaccionCaja
{
  /// <summary>
  /// Administra las transacciones realizadas a caja por el modulo de Giros
  /// </summary>
  internal class GITransaccionCaja
  {
    /// <summary>
    /// Envia informacion a los modulos de comision y cajas
    /// Para la Venta de Giros
    /// </summary>
    /// <param name="giro"></param>
    internal static void EnviarTransaccionCajaVentaGiro(GIAdmisionGirosDC giro)
    {
      ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
      ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

      int idConceptoCaja = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>().ObtenerConceptoCaja(GIConstantesAdmisionesGiros.SERVICIO_GIRO);

      CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentas(
        new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
        {
          IdCentroServicios = (int)giro.AgenciaOrigen.IdCentroServicio,
          IdServicio = GIConstantesAdmisionesGiros.SERVICIO_GIRO,
          TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Vender,
          ValorBaseComision = giro.Precio.ValorServicio,
          NumeroOperacion = giro.IdGiro.Value,
        });

      fachadaComisiones.GuardarComision(comision);

      // Se adiciona el movimiento de caja
      fachadaCajas.AdicionarMovimientoCaja(new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
      {
        InfoAperturaCaja = new CAAperturaCajaDC()
          {
            IdCaja = giro.IdCaja,
            IdCodigoUsuario = giro.IdCodigoUsuario
          },

        IdCentroResponsable = comision.IdCentroServicioResponsable,
        IdCentroServiciosVenta = comision.IdCentroServicioVenta,
        NombreCentroResponsable = comision.NombreCentroServicioResponsable,
        NombreCentroServiciosVenta = comision.NombreCentroServicioVenta,
        ValorTotal = giro.Precio.ValorServicio + giro.Precio.ValorGiro + giro.Precio.ValorAdicionales,
        TotalImpuestos = giro.Precio.ValorImpuestos,
        TotalRetenciones = 0,
        RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
          {
            new CARegistroTransacCajaDetalleDC()
            {
              ConceptoCaja= new CAConceptoCajaDC()
              {
                IdConceptoCaja= idConceptoCaja,
                EsIngreso=true,
              },
              Cantidad=1,
              EstadoFacturacion=CAEnumEstadoFacturacion.FAC,
              FechaFacturacion=DateTime.Now,
              ValorServicio= giro.Precio.ValorServicio,
              ValorImpuestos=giro.Precio.ValorImpuestos,
              ValorPrimaSeguros=0,
              ValorRetenciones=0,
              Numero=giro.IdGiro.Value,
              NumeroFactura=giro.IdGiro.ToString(),
              ValorDeclarado=0,
              ValoresAdicionales=giro.Precio.ValorAdicionales,
              Observacion=giro.Observaciones,
              ConceptoEsIngreso=true,
              ValorTercero= giro.Precio.ValorGiro,
              NumeroComprobante = "0"
            }
          },
        Usuario = Framework.Servidor.Excepciones.ControllerContext.Current.Usuario,
        TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
        RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>()
          {
            new CARegistroVentaFormaPagoDC()
            {
              IdFormaPago= TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
              Descripcion=TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
              Valor=giro.Precio.ValorTotal
            }
          }
      });
    }

    /// <summary>
    /// Envia informacion a los modulos de comision y cajas
    /// Para la Venta de Giros CONVENIO
    /// </summary>
    /// <param name="giro"></param>
    internal static void EnviarTransaccionCajaVentaGiroConvenio(GIAdmisionGirosDC giro)
    {
      ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
      ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

      int idConceptoCaja = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>().ObtenerConceptoCaja(GIConstantesAdmisionesGiros.SERVICIO_GIRO);

      CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentas(
        new CMConsultaComisionVenta()
        {
          IdCentroServicios = (int)giro.AgenciaOrigen.IdCentroServicio,
          IdServicio = GIConstantesAdmisionesGiros.SERVICIO_GIRO,
          TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Vender,
          ValorBaseComision = giro.Precio.ValorServicio,
          NumeroOperacion = giro.IdGiro.Value,
        });

      fachadaComisiones.GuardarComision(comision);

      // Se adiciona el movimiento de caja
      fachadaCajas.AdicionarMovimientoCaja(new CARegistroTransacCajaDC()
      {
        InfoAperturaCaja = new CAAperturaCajaDC()
        {
          IdCaja = giro.IdCaja,
          IdCodigoUsuario = giro.IdCodigoUsuario
        },
        IdCentroResponsable = comision.IdCentroServicioResponsable,
        IdCentroServiciosVenta = comision.IdCentroServicioVenta,
        NombreCentroResponsable = comision.NombreCentroServicioResponsable,
        NombreCentroServiciosVenta = comision.NombreCentroServicioVenta,
        ValorTotal = giro.Precio.ValorServicio + giro.Precio.ValorGiro + giro.Precio.ValorAdicionales,
        TotalImpuestos = giro.Precio.ValorImpuestos,
        RegistroVentaClienteCredito = new CARegistroTransClienteCreditoDC()
        {
          IdCliente = giro.GirosPeatonConvenio.ClienteConvenio.IdCliente,
          IdContrato = giro.GirosPeatonConvenio.ClienteConvenio.Contrato.IdContrato,
          IdSucursal = 0,
          NombreSucursal = string.Empty,
          NitCliente = giro.GirosPeatonConvenio.ClienteConvenio.Nit,
          NombreCliente = giro.GirosPeatonConvenio.ClienteConvenio.RazonSocial,
          NumeroContrato = giro.GirosPeatonConvenio.ClienteConvenio.Contrato.NombreContrato
        },
        TotalRetenciones = 0,
        RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
          {
            new CARegistroTransacCajaDetalleDC()
            {
              ConceptoCaja= new CAConceptoCajaDC()
              {
                IdConceptoCaja= idConceptoCaja,
                EsIngreso=true,
              },
              Cantidad=1,
              EstadoFacturacion=CAEnumEstadoFacturacion.FAC,
              FechaFacturacion=DateTime.Now,
              ValorServicio= giro.Precio.ValorServicio,
              ValorImpuestos=giro.Precio.ValorImpuestos,
              ValorPrimaSeguros=0,
              ValorRetenciones=0,
              Numero=giro.IdGiro.Value,
              NumeroFactura=giro.IdGiro.ToString(),
              ValorDeclarado=0,
              ValoresAdicionales=giro.Precio.ValorAdicionales,
              Observacion=giro.Observaciones,
              ConceptoEsIngreso=true,
              ValorTercero= giro.Precio.ValorGiro
            }
          },
        Usuario = Framework.Servidor.Excepciones.ControllerContext.Current.Usuario,
        TipoDatosAdicionales = CAEnumTipoDatosAdicionales.CRE,
        RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>()
          {
            new CARegistroVentaFormaPagoDC()
            {
              IdFormaPago= TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
              Descripcion=TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
              Valor=giro.Precio.ValorGiro
            },
            new CARegistroVentaFormaPagoDC()
            {
              IdFormaPago= TAConstantesServicios.ID_FORMA_PAGO_CREDITO,
              Descripcion=TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CREDITO,
              Valor=giro.Precio.ValorServicio
            }
          }
      });
    }
  }
}