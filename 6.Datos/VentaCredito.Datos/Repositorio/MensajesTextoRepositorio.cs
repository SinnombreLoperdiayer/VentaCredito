using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Servidor.Excepciones;
using System.Data.SqlClient;
using System.Data;
using VentaCredito.Transversal;

namespace VentaCredito.Datos.Repositorio
{
    public class MensajesTextoRepositorio
    {

        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static MensajesTextoRepositorio instancia = new MensajesTextoRepositorio();
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        public static MensajesTextoRepositorio Instancia { get { return instancia; } }

        public void ValidarSiDestinatarioAutorizaEnvioMensajeTexto(int idCliente, string tipoIdDestinatario, string identificacionDestinatario, long idAdmision, long numeroGuia, string NumPedido)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paDestDeseaRecibirNotifiMsjTexto_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DAE_Identificacion", identificacionDestinatario);
                cmd.Parameters.AddWithValue("@DAE_TipoIdentificacion", tipoIdDestinatario);
                cmd.Parameters.AddWithValue("@DAE_IdClienteCredito", idCliente);
                cmd.Parameters.AddWithValue("@ADM_IdAdminisionMensajeria", idAdmision);
                cmd.Parameters.AddWithValue("@ADM_NumeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@ADM_NoPedido", NumPedido);
                cmd.Parameters.AddWithValue("@AEM_CreadoPor", ContextoSitio.Current.Usuario);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
