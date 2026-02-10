using ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.Infraestructura.AccesoDatos.Repository.Tarifas
{
    public class TarifasRepository
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static readonly TarifasRepository instancia = new TarifasRepository();

        public static TarifasRepository Instancia
        {
            get
            {
                return instancia;
            }
        }

        private TarifasRepository()
        {
        }

        /// <summary>
        /// Obtiene El valor comercial dependiento del peso
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ValorComercialResponse ConsultarValorComercialPeso(int peso)
        {
            ValorComercialResponse valorCom = null;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand(@"paConsultarValorComercialPorPeso_TAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Peso", peso);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    valorCom = new ValorComercialResponse();
                    if (reader.Read())
                    {
                        valorCom = new ValorComercialResponse()
                        {
                            
                            valorComercial = Convert.ToDecimal(reader["valorComercial"]),
                            fechaServidor = DateTime.Now,
                        };
                    }
                    
                }
                sqlConn.Close();
                return valorCom;
            }

        }
    }
}
