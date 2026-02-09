using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Facturacion.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using Framework.Servidor.Comun;

namespace CO.Servidor.Facturacion.Datos
{
  /// <summary>
  /// Clase que interactua con la base de datos en el módulo de facturación
  /// </summary>
  public class FARepositorioFacManual
  {
    #region Atributos

    private static readonly FARepositorioFacManual instancia = new FARepositorioFacManual();
    private const string NombreModelo = "ModeloFacturacion";
    private const string Creada = "CRE";
    private const string Manual = "MAN";
    private const string DescripcionEstado = "CREADO";

    #endregion Atributos

    #region Propiedades

    /// <summary>
    /// Retorna la instancia de la clase FARepositorio
    /// </summary>
    public static FARepositorioFacManual Instancia
    {
      get { return FARepositorioFacManual.instancia; }
    }

    #endregion Propiedades

    #region Metodos

    /// <summary>
    /// Almacena una factura nueva manual
    /// </summary>
    /// <param name="facturaManual"></param>
    public void GuardarFacturaManual(FAFacturaClienteDC facturaManual)
    {
      using (FacturacionEntities contexto = new FacturacionEntities(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        PURegionalAdministrativa regional = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerRegionalAdministrativa(facturaManual.CiudadRadicacion.IdLocalidad);
        ResumenFactura_FAC resumen = new ResumenFactura_FAC()
        {
          REF_CiudadRadicacion = facturaManual.CiudadRadicacion.IdLocalidad,
          REF_CreadoPor = facturaManual.CreadoPor,
          REF_DireccionRadicacion = facturaManual.DireccionRadicacion,
          REF_DirigidoA = facturaManual.DirigidoA,
          REF_FormaPago = facturaManual.FormaPago,
          REF_IdCliente = facturaManual.IdCliente,
          REF_IdContrato = facturaManual.IdContrato,
          REF_NombreContrato = facturaManual.NombreContrato,
          REF_NumeroContrato = facturaManual.NumeroContrato,
          REF_NumeroFactura = facturaManual.NumeroFactura,
          REF_PlazoPago = (short)facturaManual.PlazoPago,
          REF_RazonSocial = facturaManual.RazonSocial,
          REF_TelefonoRadicacion = facturaManual.TelefonoRadicacion,
          REF_TipoFacturacion = Manual,
          REF_ValorDescuentos = facturaManual.ValorDescuentos,
          REF_ValorImpuestos = facturaManual.ValorImpuestos,
          REF_ValorNeto = facturaManual.ValorNeto,
          REF_ValorTotal = facturaManual.ValorTotal,
          REF_FechaGrabacion = DateTime.Now,
          ESF_DescEstadoActual = Creada,
          REF_IdRacol = regional.IdRegionalAdmin,
          REF_NombreRacol = regional.Descripcion
        };

        facturaManual.ConceptosFactura.ToList()
       .ForEach(con =>
         {
           DetalleXServicioFactura_FAC detalleFac = new DetalleXServicioFactura_FAC()
                {
                  DSF_Cantidad = con.Cantidad,
                  DSF_ConceptoManual = true,
                  DSF_CreadoPor = con.CreadoPor,
                  DSF_DescripcionConcepto = con.DescripcionConcepto,
                  DSF_DescripcionSucursal = con.Sucursal.Nombre,
                  DSF_IdServicio = con.IdServicio,
                  DSF_IdSucursal = con.Sucursal.IdSucursal,
                  DSF_NumeroFactura = resumen.REF_NumeroFactura,
                  DSF_ValorUnitario = con.ValorUnitario,
                  DSF_Total = con.Total,
                  DSF_TotalImpuestos = con.TotalImpuestos,
                  DSF_TotalNeto = con.TotalNeto,
                  DSF_FechaGrabacion = DateTime.Now
                };
           con.ImpuestosConcepto.ToList()
             .ForEach(imp =>
             {
               detalleFac.ImpuestosDetalleFactura_FAC.Add
               (new ImpuestosDetalleFactura_FAC()
               {
                 IDF_BaseCalculo = imp.BaseCalculo,
                 IDF_CreadoPor = imp.CreadoPor,
                 IDF_Descripcion = imp.Descripcion,
                 IDF_IdImpuesto = imp.IdImpuesto,
                 IDF_ValorPorc = imp.ValorPorc,
                 IDF_FechaGrabacion = DateTime.Now,
                 IDF_Total = imp.Total,
               });
             });
           resumen.DetalleXServicioFactura_FAC.Add(detalleFac);

         }
       );

        resumen.EstadosFactura_FAC.Add(new EstadosFactura_FAC()
          {
              ESF_CreadoPor = facturaManual.CreadoPor,
              ESF_DescripcionEstado = DescripcionEstado,
              ESF_Estado = Creada,
              ESF_NumeroFactura = resumen.REF_NumeroFactura,
              ESF_Observaciones = string.Empty,
              ESF_FechaGrabacion = DateTime.Now
          });

        contexto.ResumenFactura_FAC.Add(resumen);
        contexto.SaveChanges();

      }
    }

    #endregion Metodos
  }
}