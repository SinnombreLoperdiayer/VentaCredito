using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ServiciosInter.DatosCompartidos.Comun;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Recogidas;
using System.Collections.Generic;

namespace ServiciosInter.Infraestructura.AccesoDatos.Repository.Recogidas
{
    public class RecogidasRepository
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static readonly RecogidasRepository instancia = new RecogidasRepository();

        public static RecogidasRepository Instancia
        {
            get
            {
                return instancia;
            }
        }

        private RecogidasRepository()
        {
        }

        /// <summary>
        /// Obtiene la ultima solicitud registrada
        /// </summary>
        /// <returns></returns>
        public RGRecogidasDC ObtenerUltimaSolicitud(string numeroDocumento)
        {
            RGRecogidasDC recogida = new RGRecogidasDC();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarUltimaSolicitud_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroDocumento", numeroDocumento);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    recogida.Nombre = reader["SRP_NombreCompleto"].ToString();
                    recogida.Direccion = reader["SRP_DireccionRecogida"].ToString();
                    recogida.Correo = reader["SRP_CorreoElectronico"].ToString();
                    recogida.Ciudad = reader["SRE_IdLocalidadRecogida"].ToString();
                    recogida.Latitud = reader["SRE_Latitud"].ToString();
                    recogida.Longitud = reader["SRE_Longitud"].ToString();
                    recogida.NombreCiudad = reader["LOC_Nombre"].ToString();
                    recogida.Id = Convert.ToInt64(reader["SRP_NumeroDocumento"]);
                    recogida.NumeroTelefono = reader["SRP_NumeroTelefonico"].ToString();
                    recogida.FechaRecogida = Convert.ToDateTime(reader["SRE_FechaHoraRecogida"]);
                    recogida.PreguntarPor = reader["SRP_PreguntarPor"].ToString();
                    recogida.DescripcionEnvios = reader["SRP_DescripcionEnvios"].ToString();
                    recogida.TotalPiezas = Convert.ToInt32(reader["SRP_TotalPiezas"]);
                    recogida.PesoAproximado = Convert.ToInt32(reader["SRP_PesoAproximado"]);
                    recogida.TipoDocumento = reader["SRP_TipoDocumento"].ToString();
                }
                conn.Close();
            }
            return recogida;
        }

        /// <summary>
        /// Inserta las recogidas esporadicas
        /// </summary>
        /// <param name="recogida"></param>
        /// <returns></returns>
        public long InsertarSolicitudRecogida(RGRecogidasDC recogida)
        {
            long idRecogida = 0;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarSolicitudRecogida_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoSolRecogida", recogida.TipoRecogida);
                cmd.Parameters.AddWithValue("@IdOrigenSolRecogida", ServiciosInterContext.Current.IdAplicativoOrigen);
                cmd.Parameters.AddWithValue("@IdEstadoSolicitud", EnumEstadoSolicitudRecogida.Creado);
                cmd.Parameters.AddWithValue("@IdClienteContado", recogida.idCliente);
                cmd.Parameters.AddWithValue("@FechaHoraRecogida", recogida.FechaRecogida);
                cmd.Parameters.AddWithValue("@CreadoPor", ServiciosInterContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@Longitud", recogida.Longitud);
                cmd.Parameters.AddWithValue("@Latitud", recogida.Latitud);
                cmd.Parameters.AddWithValue("@IdLocalidadRecogida", recogida.Ciudad);
                cmd.Parameters.AddWithValue("@EsEsporadicaCliente", 1);
                if (recogida.IdSucursal > 0)
                {
                    cmd.Parameters.AddWithValue("@IdSucursal", recogida.IdSucursal);
                }
                cmd.Parameters.AddWithValue("@IdCentroServicios", recogida.IdCentroServicio);
                cmd.Parameters.AddWithValue("@TipoDocumento", recogida.TipoDocumento);
                cmd.Parameters.AddWithValue("@NumeroDocumento", recogida.NumeroDocumento);
                cmd.Parameters.AddWithValue("@NombreCompleto", recogida.Nombre);
                cmd.Parameters.AddWithValue("@DireccionRecogida", recogida.Direccion);
                cmd.Parameters.AddWithValue("@CorreoElectronico", recogida.Correo);
                cmd.Parameters.AddWithValue("@NumeroTelefonico", recogida.NumeroTelefono);
                cmd.Parameters.AddWithValue("@PreguntarPor", recogida.PreguntarPor);
                cmd.Parameters.AddWithValue("@DescripcionEnvios", recogida.DescripcionEnvios);
                cmd.Parameters.AddWithValue("@TotalPiezas", recogida.TotalPiezas);
                cmd.Parameters.AddWithValue("@PesoAproximado", recogida.PesoAproximado);
                conn.Open();
                var resultado = cmd.ExecuteScalar();
                conn.Close();
                if (resultado != null)
                {
                    idRecogida = Convert.ToInt64(resultado);
                }
            }
            return idRecogida;
        }


        /// <summary>
        /// Obtiene las recogidas de un cliente peaton
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public List<RGRecogidaEsporadicaDC> ObtenerMisRecogidasClientePeaton(string idUsuario)
        {
            List<RGRecogidaEsporadicaDC> misRecogidas = new List<RGRecogidaEsporadicaDC>();
            using (SqlConnection con = new SqlConnection(CadCnxController))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMisRecogidasClientePeaton_REC", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));
                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    misRecogidas.Add(new RGRecogidaEsporadicaDC
                    {
                        IdSolRecogida = (long)lector["SRE_IdSolRecogida"],
                        FechaHoraRecogida = Convert.ToDateTime(lector["SRE_FechaHoraRecogida"].ToString()),
                        FechaGrabacion = Convert.ToDateTime(lector["SRE_FechaGrabacion"].ToString()),
                        DireccionRecogida = lector["SRP_DireccionRecogida"].ToString(),
                        DescripcionEstado = lector["SRE_DescripcionEstadoSol"].ToString(),
                        Mensajero = lector["Mensajero"].ToString(),
                        IdLocalidad = lector["SRE_IdLocalidadRecogida"].ToString(),
                        IdEstadoRecogida = Convert.ToInt32(lector["SRE_IdEstadoSolicitud"])
                    });
                }
                con.Close();
                con.Dispose();
            }

            return misRecogidas;

        }


        /// <summary>
        /// Retorna el valor del parametro del id indicado
        /// </summary>
        /// <returns></returns>
        public string ObtenerHoraLimiteRecogida()
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametroOperacionUrbana_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@POU_IdParametro", "HoraLimiteProgRecogi");
                conn.Open();
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }
    }
}