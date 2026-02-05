using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Clientes;
using System.Configuration;

namespace VentaCredito.Clientes.Datos.Repositorio
{
    public class AuditoriaRepositorio
    {

        private static string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #region Singleton

        private static AuditoriaRepositorio instancia = new AuditoriaRepositorio();

        public static AuditoriaRepositorio Instancia
        {
            get
            {
                return instancia;
            }
            
        }

        #endregion



        /// <summary>
        /// Metodo de auditoria cambios en Cliente Contado
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditModificarClienteContado(CLClienteContadoDC clienteContado, string usuario)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("paInsertarClienteContadoHist_CLI", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@Apellido1", clienteContado.Apellido1.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Apellido2", clienteContado.Apellido2.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Direccion", clienteContado.Direccion.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Email", clienteContado.Email == null ? string.Empty : clienteContado.Email.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Identificacion", clienteContado.Identificacion.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Nombre", clienteContado.Nombre.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Ocupacion", clienteContado.Ocupacion != null ? clienteContado.Ocupacion.IdOcupacion : (short)0);
                comm.Parameters.AddWithValue("@Telefono", clienteContado.Telefono.ToUpper().Trim());
                comm.Parameters.AddWithValue("@TipoId", clienteContado.TipoId);
                SqlParameter paramUltimaCedulaEscaneada = new SqlParameter();
                paramUltimaCedulaEscaneada.ParameterName = "@UltimaCedulaEscaneada";
                if (clienteContado.UltimaCedulaEscaneada.HasValue)
                {
                    paramUltimaCedulaEscaneada.Value = clienteContado.UltimaCedulaEscaneada.Value;
                }
                else
                {
                    paramUltimaCedulaEscaneada.Value = DBNull.Value;
                }
                comm.Parameters.Add(paramUltimaCedulaEscaneada);
                //comm.Parameters.AddWithValue("@UltimaCedulaEscaneada", clienteContado.UltimaCedulaEscaneada == 0 ? null : clienteContado.UltimaCedulaEscaneada);
                comm.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                comm.Parameters.AddWithValue("@CreadoPor", usuario);
                comm.Parameters.AddWithValue("@CambiadoPor", usuario);
                comm.Parameters.AddWithValue("@TipoCambio", "Modified");
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
