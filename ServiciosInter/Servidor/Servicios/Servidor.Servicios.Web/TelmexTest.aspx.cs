using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CO.Servidor.Servicios.Web
{
  public partial class TelmexTest : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
      try
      {
        Servicio.COConsultasComcelSvcClient servicio = new Servicio.COConsultasComcelSvcClient();
        var resultado = servicio.ConsultarGuiaCliente(new Framework.Servidor.Servicios.ContratoDatos.ConsultasExternas.COConsultaComcelRequest
        {
          Cuenta = txtCuenta.Text,
          Fecha = txtFecha.Text,
          NomUsuario = "ComcelConsulta",
          Password = "C0mc3lInt3er2014*", Producto = "FACTURACION"
        });
        if (resultado != null && resultado.Count() > 0)
        {
          var info = resultado.First();
          txtNombreCliente.Text = info.NombreApellido;
          txtDireccion.Text = info.Direccion;
          txtMotivoDevolucion.Text = info.MotivoDevolucion;
          txtNumGuia.Text = info.NumeroGuia;
          if (info.Imagen != null)
          {
            imgGuia.ImageUrl = "data:image/png;base64," + info.Imagen;
          }
        }
        else
        {
          lblError.Text = "No se encontró información de la cuenta";
        }
      }
      catch (Exception exc)
      {
        lblError.Text = "Hay un error al intentar conectarse con el servicio";
      }
    }
  }
}