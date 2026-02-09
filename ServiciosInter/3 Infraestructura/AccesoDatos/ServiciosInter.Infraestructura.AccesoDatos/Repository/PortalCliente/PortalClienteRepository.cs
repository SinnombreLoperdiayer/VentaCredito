using ServiciosInter.DatosCompartidos.EntidadesNegocio.PortalCliente;
using ServiciosInter.Infraestructura.AccesoDatos.Repository.Mensajeria;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.Infraestructura.AccesoDatos.Repository.PortalCliente
{
    public class PortalClienteRepository
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static readonly PortalClienteRepository instancia = new PortalClienteRepository();

        public static PortalClienteRepository Instancia
        {
            get
            {
                return instancia;
            }
        }

        private PortalClienteRepository()
        {
        }

        /// <summary>
        /// Retornar los filtros de busqueda para Pago en Casa
        /// </summary>
        /// <returns></returns>
        public PCFiltrosResponse ObtenerFiltrosPortalCliente()
        {
            PCFiltrosResponse filtros = new PCFiltrosResponse();
            DataSet dataset = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerFiltrosPortalCliente_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;                
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dataset);
                sqlConn.Close();

                DataTable filtrosBusqueda;
                DataTable filtrosEstados;
                DataTable filtrosTiposEstados;

                filtrosBusqueda = (dataset.Tables[0].Rows.Count > 0) ? dataset.Tables[0] : new DataTable();

                filtrosEstados = (dataset.Tables[1].Rows.Count > 0) ? dataset.Tables[1] : new DataTable();

                filtrosTiposEstados = (dataset.Tables[2].Rows.Count > 0) ? dataset.Tables[2] : new DataTable();

                if (filtrosBusqueda.Rows.Count != 0)
                {
                    filtros.FiltrosBusqueda = filtrosBusqueda.AsEnumerable()
                        .ToList().ConvertAll(r => new PCFiltrosBusqueda
                        {
                            IdFiltro = Convert.ToInt32(r["FPC_IdFiltro"]),
                            DescripcionFiltro = (r["FPC_DescripcionFiltro"]).ToString()
                        });
                }

                if (filtrosEstados.Rows.Count != 0)
                {
                    filtros.FiltrosEstados = filtrosEstados.AsEnumerable()
                        .ToList().ConvertAll(r => new PCFiltrosEstados
                        {
                            IdEstadoFiltro = Convert.ToInt32(r["UEF_IdEstadoFiltro"]),
                            DescripcionEstadoFiltro = (r["UEF_DescripcionEstadoFiltro"]).ToString(),
                            TipoEstadoFiltro = Convert.ToInt32(r["UEF_TipoEstadoFiltro"]),
                            OrdenTipoEstadoFiltro = Convert.ToInt32(r["UEF_OrdenTipoEstadoFiltro"]),
                            IdEstadoPortalCliente = Convert.ToInt32(r["UEF_IdEstadoPortalCliente"])                            
                        }); 
                }

                if (filtrosTiposEstados.Rows.Count != 0)
                {
                    filtros.FiltrosTiposEstados = filtrosTiposEstados.AsEnumerable()
                        .ToList().ConvertAll(r => new PCFiltrosTiposEstado
                        {
                            IdTipoEstadofiltro = Convert.ToInt32(r["TEF_IdTipoEstadoFiltro"]),
                            DescripcionEstadoFiltro = (r["TEF_DescripcionEstadoFiltro"]).ToString(),
                            OrdenTipoestadoFiltro = Convert.ToInt32(r["TEF_OrdenTipoestadoFiltro"])
                        });
                }
            }
            return filtros;
        }
    }
}
