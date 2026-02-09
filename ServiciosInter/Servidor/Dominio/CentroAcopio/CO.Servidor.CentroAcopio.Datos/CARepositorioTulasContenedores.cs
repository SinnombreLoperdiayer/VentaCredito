using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using CO.Servidor.CentroAcopio.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;

namespace CO.Servidor.CentroAcopio.Datos
{
    public class CARepositorioTulasContenedores
    {
        private static readonly CARepositorioTulasContenedores instancia = new CARepositorioTulasContenedores();
        public static CARepositorioTulasContenedores Instancia
        {
            get { return CARepositorioTulasContenedores.instancia; }
        }

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;



        /// <summary>
        /// Método para crear el consolidado Tula ó Contenedor
        /// </summary>
        public void InsertarConsolidado(CATipoConsolidado Consolidado)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarConsolidado_OPN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoConsolidado", Consolidado.IdTipoConsolidado);
                cmd.Parameters.AddWithValue("@IdCentroServicioPropietario", ControllerContext.Current.IdCentroServicio);
                cmd.Parameters.AddWithValue("@IdCentroServicioDestino", Consolidado.IdCentroServicioDestino);
                cmd.Parameters.AddWithValue("@NumeroConsolidado", Consolidado.NumeroConsolidado);
                cmd.Parameters.AddWithValue("@Estado", Consolidado.Estado);
                cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                sqlConn.Open();
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }


        /// <summary>
        /// Consulta las Tulas y Contenedores de un Centro de Servicio
        /// </summary>
        /// <returns></returns>
        public List<CATipoConsolidado> ObtenerConsolidadosCSPropietario()
        {
            List<CATipoConsolidado> listaConsolidado = new List<CATipoConsolidado>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarConsolidadosCSPropietario_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicio", ControllerContext.Current.IdCentroServicio);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CATipoConsolidado objConsolidado = new CATipoConsolidado();

                    objConsolidado.IdTipoConsolidado = Convert.ToInt32(reader["IdTipoConsolidado"]);
                    objConsolidado.Descripcion = reader["TipoConsolidado"].ToString();
                    objConsolidado.NumeroConsolidado = reader["NumeroConsolidado"].ToString();
                    objConsolidado.IdCentroServicioDestino = Convert.ToInt64(reader["IdCentroServicioDestino"]);
                    objConsolidado.NombreCentroServicioDestino = reader["NombreCentroServicioDestino"].ToString();
                    objConsolidado.IdRacolDestino = Convert.ToInt64(reader["IdRacolDestino"]);
                    objConsolidado.NombreRacolDestino = reader["RacolDestino"].ToString();
                    objConsolidado.FechaGrabacion = Convert.ToDateTime(reader["FechaCreacion"]);
                    objConsolidado.Estado = Convert.ToBoolean(reader["Estado"]);

                    listaConsolidado.Add(objConsolidado);
                }
                return listaConsolidado;
            }
        }

        /// <summary>
        /// Actualiza el Centro de Servicio Destino del Contenedor o Tula y su Estado
        /// </summary>
        public void ModificarCentroServicioDestinoConsolidado(List<CATipoConsolidado> listaTipoConsolidado)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                foreach (var item in listaTipoConsolidado)
                {
                    SqlCommand cmd = new SqlCommand("paActualizaCentroServicioDestinoConsolidado_CAC", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NumeroConsolidado", item.NumeroConsolidado);
                    cmd.Parameters.AddWithValue("@IdCentroServicioDestino", item.CentroServicioDestinoSeleccionado.IdCentroServicio);
                    cmd.Parameters.AddWithValue("@IdTipConsolidado", item.IdTipoConsolidado);
                    cmd.Parameters.AddWithValue("@Estado", item.Estado);
                    sqlConn.Open();
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }
    }
}
