using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.ListaPrecios;
using CO.Servidor.Tarifas.ListaPrecios;

namespace CO.Servidor.Tarifas
{
  public class TAAdministradorTarifasExt
  {
    private static TAAdministradorTarifasExt instancia = new TAAdministradorTarifasExt();

    public static TAAdministradorTarifasExt Instancia
    {
      get { return TAAdministradorTarifasExt.instancia; }
    }

    public TAPrecioMensajariaExpresaDC ObtenerListaPreciosMesajeriaExpresa(int idListaPrecios)
    {
      TAListaPreciosConfigurador conf = TAListaPreciosConfigurador.CrearInstancia();

      return conf.ObtenerListaPreciosMesajeriaExpresa(idListaPrecios);
    }
  }
}
