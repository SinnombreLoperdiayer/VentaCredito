using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Servicio.Entidades.Clientes;
using System.Data.SqlClient;
using System.Configuration;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace VentaCredito.Clientes.Datos.Repositorio
{
    public class CLSucursalRepositorio
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static CLSucursalRepositorio instancia = new CLSucursalRepositorio();

        public static CLSucursalRepositorio Instancia
        {
            get
            {
                return instancia;
            }

        }

        public CLSucursalDC ObtenerSucursalCliente(int idSucursal, CLClientesDC cliente)
        {
            CLSucursalDC sucursal = new CLSucursalDC();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerSucursalCliente_CLI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("idSucursal", idSucursal);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        cliente.IdCliente = Convert.ToInt32(reader["CLI_IdCliente"]);
                        cliente.Telefono = reader["CLI_Telefono"].ToString();
                        cliente.Direccion = reader["CLI_Direccion"].ToString();
                        cliente.RazonSocial = reader["CLI_RazonSocial"].ToString();
                        sucursal.IdSucursal = Convert.ToInt32(reader["SUC_IdSucursal"]);
                        sucursal.Nombre = reader["SUC_Nombre"].ToString();
                        sucursal.IdCliente = Convert.ToInt32(reader["SUC_ClienteCredito"]);
                        sucursal.Agencia = Convert.ToInt64(reader["SUC_AgenciaEncargada"]);
                        sucursal.Ciudad = new PALocalidadDC() { IdLocalidad = reader["SUC_Municipio"].ToString() };
                        sucursal.IdBodega = reader["SUC_IdBodega"].ToString();
                    }
                }
                return sucursal;
            }
        }
    }
}
