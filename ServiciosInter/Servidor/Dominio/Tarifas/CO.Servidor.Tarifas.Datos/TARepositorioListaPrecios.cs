using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.ListaPrecios;
using CO.Servidor.Tarifas.Datos.Modelo;
using Framework.Servidor.Comun;

namespace CO.Servidor.Tarifas.Datos
{
  /// <summary>
  /// Repositorio para lista de precios
  /// </summary>
  public class TARepositorioListaPrecios
  {
    private static TARepositorioListaPrecios instancia = new TARepositorioListaPrecios();
    private const string MOD_TARIFAS = "ModeloTarifas";

    public static TARepositorioListaPrecios Instancia
    {
      get { return TARepositorioListaPrecios.instancia; }
    }

    public void GuardarListaPrecios(TAListaPrecioDC listaPrecios)
    {
    }

    /// <summary>
    /// Retorna las listas de precio plenas activas que no sean de cliente
    /// </summary>
    /// <returns>Listado con las listas de precio</returns>
    public IEnumerable<TAListaPrecioDC> ObtenerListasPrecio()
    {
      using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(MOD_TARIFAS)))
      {
        return contexto.ListaPrecios_TAR.Where(r => r.LIP_Estado == ConstantesFramework.ESTADO_ACTIVO && r.LIP_EsTarifaPlena == true
          && r.LIP_EsListaCliente == false)
          .ToList()
          .ConvertAll(r => new TAListaPrecioDC()
          {
            IdListaPrecio = r.LIP_IdListaPrecios,
            Nombre = r.LIP_Nombre
          });
      }
    }

    public TAPrecioMensajariaExpresaDC ObtenerListaPreciosMesajeriaExpresa(int idListaPrecios)
    {
      TAPrecioMensajariaExpresaDC listaPrecios = null;

      using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(MOD_TARIFAS)))
      {
        var listaPreciosPrecioTrayecto = contexto.ListaPrecios_TAR  // your starting point
          .Join(
            contexto.ListaPrecioServicio_TAR,             // the source table of the inner join
            lp => lp.LIP_IdListaPrecios,                  // Select the primary key (the first part of the "on" clause in an sql "join"
            lps => lps.LPS_IdListaPrecios,                // Select the foreign key (the second part of the "on" clause)
            (listaPrecio, listaPrecioSvc) => new { listaPrecio, listaPrecioSvc })         // selection
          .Where(w => w.listaPrecio.LIP_IdListaPrecios == idListaPrecios) // where statement

          .Join(
          contexto.PrecioTrayecto_TAR,
          lp => lp.listaPrecioSvc.LPS_IdListaPrecioServicio,
          pt => pt.PTR_IdListaPrecioServicio,
          (lp, pt) => new { lp, pt })
          .Where(w => w.lp.listaPrecioSvc.LPS_IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA)  // where statement

          .Join(
          contexto.TrayectoSubTrayecto_TAR,
          ts => ts.pt.PTR_IdTrayectoSubTrayecto,
          tst => tst.TRS_IdTrayectoSubTrayecto,
          (ts, tst) => new { ts, tst })
          .Where(w => w.tst.TRS_IdTrayectoSubTrayecto == w.ts.pt.PTR_IdTrayectoSubTrayecto)

          .ToList();

        if (listaPreciosPrecioTrayecto == null)
        {
          throw new Exception("La lista de precios no tiene configuración de precios para el servicio de mensajería expresa");
        }

        listaPrecios = new TAPrecioMensajariaExpresaDC();
        listaPrecios.IdListaPrecios = idListaPrecios;
        listaPrecios.IdServicio = TAConstantesServicios.SERVICIO_MENSAJERIA;
        listaPrecios.PrecioTrayecto = new List<TAPrecioTrayectoControllerDC>(listaPreciosPrecioTrayecto.Count());

        //consolidar la información de la lista de precios con los precios configurados
        foreach (var item in listaPreciosPrecioTrayecto)
        {
          listaPrecios.PrecioTrayecto.Add(new TAPrecioTrayectoControllerDC
          {
            IdPrecioTrayecto = item.ts.pt.PTR_IdPrecioTrayectoSubTrayect,
            IdTipoTrayecto = item.tst.TRS_IdTipoTrayecto,
            IdTipoSubTrayecto = item.tst.TRS_IdTipoSubTrayecto,
            IdTipoTrayectoSubTrayecto = item.ts.pt.PTR_IdTrayectoSubTrayecto,
            Valor = item.ts.pt.PTR_ValorFijo
          });
        }
      }

      return listaPrecios;
    }
  }
}