using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Planilla.Datos.Interfaces;
using VentaCredito.Transversal.Entidades.Planilla;

namespace VentaCredito.Planilla.Datos
{
    public class PlanillaDatos : IPlanillaDatos
    {
        #region Intancia
        private static volatile PlanillaDatos instancia;

        private string conexion = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionEstructuraNegocio = ConfigurationManager.ConnectionStrings["EstructuraNegocio"].ConnectionString;

        /// <summary>
        /// Atributo utilizado para evitar problemas con multithreading en el singleton.
        /// </summary>
        private static object syncRoot = new Object();

        public static PlanillaDatos Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (syncRoot)
                    {
                        if (instancia == null)
                        {
                            instancia = new PlanillaDatos();
                        }
                    }
                }
                return instancia;
            }
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Valida información del consolidado ingresado
        /// </summary>
        /// <param name="movimientoConsolidado"></param>
        /// <param name="idEstado"></param>
        /// <returns></returns>
        public bool ValidarInsertarMovimientoConsolidadoUrbano(CAMovimientoConsolidadoDCIntegra movimientoConsolidado, int idEstado)
        {
            bool rst = false;
            using (SqlConnection connection = new SqlConnection(conexion))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("paValidarInsertarMovimientoConsolidadoUrbano", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoConsolidado", movimientoConsolidado.IdTipoConsolidado);
                cmd.Parameters.AddWithValue("@NumeroConsolidado", movimientoConsolidado.NumeroConsolidado);
                cmd.Parameters.AddWithValue("@IdCentroServicio", movimientoConsolidado.IdCentroServicioDestino);
                cmd.Parameters.AddWithValue("@IdEstadoConsolidado", idEstado);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    rst = true;
                }
                connection.Close();
            }
            return rst;
        }

        /// <summary>
        /// Inserta movimiento del consolidado en la base de datos
        /// </summary>
        /// <param name="movimientoConsolidado"></param>
        /// <returns></returns>
        public int InsertarMovimientoConsolidado(CAMovimientoConsolidadoDCIntegra movimientoConsolidado)
        {
            using (SqlConnection connection = new SqlConnection(conexion))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("paInsertarMovimientoConsolidado_CAC", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@MCO_NumeroPrecinto", movimientoConsolidado.NumeroPrecinto));
                cmd.Parameters.Add(new SqlParameter("@MCO_IdCentroServicioDestino", movimientoConsolidado.IdCentroServicioDestino));
                cmd.Parameters.Add(new SqlParameter("@MCO_IdTipoMovimiento", movimientoConsolidado.IdTipoMovimiento));
                cmd.Parameters.Add(new SqlParameter("@MCO_CreadoPor", "Admin"));
                cmd.Parameters.Add(new SqlParameter("@MCO_NumeroConsolidado", movimientoConsolidado.NumeroConsolidado));
                cmd.Parameters.Add(new SqlParameter("@MCO_IdTipoConsolidado", movimientoConsolidado.IdTipoConsolidado));

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Valida la información del precinto ingresado
        /// </summary>
        /// <param name="logMovimiento"></param>
        /// <returns></returns>
        public bool ValidarInsertarMovimientoPrecinto(LogMovimientoPrecintoPUAIntegra logMovimiento, int idEstado, long idCentroServicio)
        {
            bool rst = false;
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paValidarInsertarMovimientoPrecinto", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroPrecinto", logMovimiento.NumeroPrecinto);
                cmd.Parameters.AddWithValue("@IdCentroServicio", idCentroServicio);
                cmd.Parameters.AddWithValue("@IdMovimiento", idEstado);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    rst = true;
                }
                conn.Close();
            }

            return rst;
        }

        /// <summary>
        /// Inserta movimiento de un precinto en la base de datos
        /// </summary>
        /// <param name="logMovimiento"></param>
        /// <returns></returns>
        public string InsertarLogMovimientoPrecinto(LogMovimientoPrecintoPUAIntegra logMovimiento)
        {
            string resultado = "";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString);
            try
            {
                SqlCommand cmdcommand = new SqlCommand("paInsertarLogMovimientoPrecinto_PUA", conn);
                cmdcommand.CommandType = System.Data.CommandType.StoredProcedure;
                cmdcommand.Parameters.AddWithValue("@IdTipoMovimiento", logMovimiento.IdTipoMovimientoPrecinto);
                cmdcommand.Parameters.AddWithValue("@NumeroPrecinto", logMovimiento.NumeroPrecinto);
                cmdcommand.Parameters.AddWithValue("@Fecha", logMovimiento.Fecha);
                cmdcommand.Parameters.AddWithValue("@Usuario", logMovimiento.Usuario);
                conn.Open();
                cmdcommand.ExecuteNonQuery();
                conn.Close();
                resultado = "OK";
            }
            catch (Exception ex)
            {
                resultado = ex.Message.ToString();
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return resultado;
        }

        public Persona_Huella_AUT ObtenerHuellaMensajero(string numeroDocumento)
        {
            Persona_Huella_AUT personaHuella = new Persona_Huella_AUT();
            using (SqlConnection connection = new SqlConnection(conexionEstructuraNegocio))
            {
                SqlCommand command = new SqlCommand("paObtenerHuellaPorIdentificacion",connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@identificacion", numeroDocumento);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    personaHuella.IdTercero = Convert.ToInt64(reader["PHU_IdTercero"]);
                    personaHuella.Identificación = Convert.ToString(reader["TER_Identificacion"]);
                    personaHuella.Huella_Id = Convert.ToInt64(reader["PHU_Huella_Uuid"]);
                    personaHuella.ManoDerecha = Convert.ToBoolean(reader["PHU_ManoDerecha"]);
                    personaHuella.Dedo = Convert.ToString(reader["PHU_Dedo"]);
                    personaHuella.SerializadoHuella = Convert.ToString(reader["PHU_XML_Huella"]);

                }
                connection.Close();
            }
                return personaHuella;
        }

        /// <summary>
        /// Obtiene mensajero por número de identificación
        /// </summary>
        /// <param name="numeroDocumento"></param>
        /// <returns></returns>
        public MensajeroPlanilla ObtenerDatosMensajeroXDocumento(string numeroDocumento)
        {
            MensajeroPlanilla mensajeroPlanilla = new MensajeroPlanilla();
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                SqlCommand command = new SqlCommand("paObtenerInformacionMensajeroXDocumento", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@NumeroDocumento", numeroDocumento);

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    mensajeroPlanilla.IdentificacionMensajero = Convert.ToString(reader["PEI_Identificacion"]);
                    mensajeroPlanilla.IdMensajero = Convert.ToInt32(reader["MEN_IdMensajero"]);
                    mensajeroPlanilla.NombreMensajero = Convert.ToString(reader["PEI_NombreCompleto"]);
                }
                conn.Close();
            }
                return mensajeroPlanilla;
        }
        /// <summary>
        /// Obtiene información del centro de servicio creado en controller por su id 
        /// </summary>
        /// <param name="IdCentroServicio"></param>
        /// <returns></returns>
        public CentroServicioPlanilla ObtenerInfoCentroServicioXId(long IdCentroServicio)
        {
            CentroServicioPlanilla centroServicioPlanilla = new CentroServicioPlanilla();
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                SqlCommand command = new SqlCommand("paObtenerInfoCentroServicioXId", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdCentroServicio", IdCentroServicio);

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    centroServicioPlanilla.NombreCentroServicio = Convert.ToString(reader["CES_Nombre"]);
                    centroServicioPlanilla.IdCentroServicio = Convert.ToInt32(reader["CES_IdCentroServicios"]);
                    centroServicioPlanilla.DireccionCentroServicio = Convert.ToString(reader["CES_Direccion"]);
                }
                conn.Close();
            }
            return centroServicioPlanilla;
        }
        /// <summary>
        /// Obtiene la fecha de creación de la planilla por su Id
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public DateTime ObtenerFechaPlanilla(long idPlanilla)
        {
            DateTime FechaPlanilla;
            string queryString = "SELECT PLR_FechaGrabacion FROM PlanillaRecogida_POS WHERE PLR_IdPlanilla = @IdPlanilla";
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@IdPlanilla", idPlanilla);
                conn.Open();
                FechaPlanilla = Convert.ToDateTime(command.ExecuteScalar());
                conn.Close();
            }
            return FechaPlanilla;
        }
        /// <summary>
        /// Obtiene la cantidad de envíos admitidos por el centro de servicio que se encuentran sin planilla
        /// </summary>
        /// <param name="IdCentroservicio"></param>
        /// <returns></returns>
        public int ObtenerCantidadEnviosSinPlanillarXIdCentroSrvicio(long IdCentroservicio)
        {
            int cantidadPendientePlanillar = 0;
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                SqlCommand command = new SqlCommand("paObtenerCantidadGuiasSinPlanillaXIdCentroServicio", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdCentroServicio", IdCentroservicio);
                conn.Open();
                cantidadPendientePlanillar = Convert.ToInt32(command.ExecuteScalar());
            }
                return cantidadPendientePlanillar;
        }
        #endregion
    }
}
