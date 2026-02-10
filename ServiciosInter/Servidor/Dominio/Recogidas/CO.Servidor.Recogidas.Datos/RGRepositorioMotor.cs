using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Servidor.Recogidas.Datos
{
    public class RGRepositorioMotor
    {
        private static readonly RGRepositorioMotor instancia = new RGRepositorioMotor();
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        /// <summary>
        /// Retorna la instancia de la clase RGRepositorioMotor
        /// </summary>
        public static RGRepositorioMotor Instancia
        {
            get { return RGRepositorioMotor.instancia; }
        }
        /// <summary>
        /// constructor
        /// </summary>
        private RGRepositorioMotor()
        {
        }

        /// <summary>
        /// Vence las solicitudes asignadas y retorna la lista de dispositivos para notificar el vencimiento
        /// </summary>
        /// <returns></returns>
        public List<PADispositivoMovil> VencerSolicitudesRecogidas()
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                List<PADispositivoMovil> lstDispositivos = new List<PADispositivoMovil>();

               SqlCommand cmd = new SqlCommand("paVencerRecogidasAsignadas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PADispositivoMovil dispo = new PADispositivoMovil()
                    {
                        TokenDispositivo = reader["token"].ToString()
                    };
                    lstDispositivos.Add(dispo);
                }
                return lstDispositivos;
            }

        }

        /// <summary>
        /// Obtiene las recogidas nuevas para notificar a los mensajeros y obtiene las recogidas para cambiar de estado a ParaForzar
        /// </summary>
        /// <returns></returns>
        public List<RGRecogidaMotorDC> ObtenerRecogidasNuevasNotificarForzar()
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                List<RGRecogidaMotorDC> lstRecogidas = new List<RGRecogidaMotorDC>();

                SqlCommand cmd = new SqlCommand("paObtenerSolicituresRecogidasNotificarForzar_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RGRecogidaMotorDC dispo = new RGRecogidaMotorDC()
                    {
                         Accion = (EnumAccionMotor)Enum.Parse(typeof(EnumAccionMotor), reader["Accion"].ToString()),
                         Direccionrecogida = reader["SRP_DireccionRecogida"].ToString(),
                         IdLocalidadRecogida = reader["SRE_IdLocalidadRecogida"].ToString(),
                         IdSolicitudRecogida = Convert.ToInt64(reader["SRE_IdSolRecogida"]),
                         EsNueva = Convert.ToBoolean(reader["EsNueva"])
                    };
                    lstRecogidas.Add(dispo);
                }
                return lstRecogidas;
            }

        }

        /// <summary>
        /// Obtiene los datos del mensajero por medio de la cedula
        /// </summary>
        /// <param name="v"></param>
        public OUDatosMensajeroDC ObtenerDatosMensajeroPorCedula(long numeroIdentificacion)
        {
            OUDatosMensajeroDC datosMensajero = new OUDatosMensajeroDC();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDatosmensajeroPorNumeroCedula_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroCedula", numeroIdentificacion);
                conn.Open();
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    datosMensajero.IdMensajero = Convert.ToInt64(lector["MEN_IdMensajero"]);
                    datosMensajero.IdTipoMensajero = Convert.ToInt32(lector["MEN_IdTipoMensajero"]);
                    datosMensajero.IdAgencia = Convert.ToInt64(lector["MEN_IdAgencia"]);
                    datosMensajero.Telefono2 = Convert.ToString(lector["MEN_Telefono2"]);
                    datosMensajero.FechaIngreso = Convert.ToDateTime(lector["MEN_FechaIngreso"]);
                    datosMensajero.FechaTerminacionContrato = Convert.ToDateTime(lector["MEN_FechaTerminacionContrato"]);
                    datosMensajero.NumeroPase = Convert.ToString(lector["MEN_NumeroPase"]);
                    datosMensajero.FechaVencimientoPase = Convert.ToDateTime(lector["MEN_FechaVencimientoPase"]);
                    datosMensajero.Estado = Convert.ToString(lector["MEN_Estado"]);
                    datosMensajero.EsContratista = Convert.ToBoolean(lector["MEN_EsContratista"]);
                    datosMensajero.Descripcion = Convert.ToString(lector["TIM_Descripcion"]);
                    datosMensajero.EsVehicular = Convert.ToBoolean(lector["TIM_EsVehicular"]);
                    datosMensajero.IdPersonaInterna = Convert.ToInt64(lector["PEI_IdPersonaInterna"]);
                    datosMensajero.IdTipoIdentificacion = Convert.ToString(lector["PEI_IdTipoIdentificacion"]);
                    datosMensajero.Identificacion = Convert.ToString(lector["PEI_Identificacion"]);
                    datosMensajero.IdCargo = Convert.ToInt32(lector["PEI_IdCargo"]);
                    datosMensajero.NombreMensajero = Convert.ToString(lector["NombreMensajero"]);
                    datosMensajero.PrimerApellido = Convert.ToString(lector["PEI_PrimerApellido"]);
                    datosMensajero.SegundoApellido = Convert.ToString(lector["PEI_SegundoApellido"]);
                    datosMensajero.DireccionMensajero = Convert.ToString(lector["DireccionMensajero"]);
                    datosMensajero.Municipio = Convert.ToString(lector["PEI_Municipio"]);
                    datosMensajero.Telefono = Convert.ToString(lector["PEI_Telefono"]);
                    datosMensajero.EmailMensajero = Convert.ToString(lector["EmailMensajero"]);
                    datosMensajero.IdRegionalAdm = Convert.ToInt64(lector["PEI_IdRegionalAdm"]);
                    datosMensajero.Comentarios = Convert.ToString(lector["PEI_Comentarios"]);
                    datosMensajero.IdCentroServicios = Convert.ToInt64(lector["CES_IdCentroServicios"]);
                    datosMensajero.NombreCentroServicio = Convert.ToString(lector["NombreCentroServicio"]);
                    datosMensajero.Telefono1 = Convert.ToString(lector["CES_Telefono1"]);
                    datosMensajero.DireccionCentroServicio = Convert.ToString(lector["DireccionCentroServicio"]);
                    datosMensajero.IdMunicipio = Convert.ToString(lector["CES_IdMunicipio"]);
                    datosMensajero.NombreLocalidad = Convert.ToString(lector["NombreLocalidad"]);
                    datosMensajero.TipoContrato = Convert.ToInt32(lector["MEN_TipoContrato"]);
                }
                conn.Close();
            }
            return datosMensajero;
        }

        /// <summary>
        /// aumenta el contador de las recogidas nuevas o canceladas por mensajero
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="esNueva"></param>
        public void AumentarContadorNotificacionRecogidas(long idSolicitudRecogida, bool esNueva)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                string nombProcedimiento;
                if(esNueva)
                {
                    nombProcedimiento = "paIncrementarContadorNotificacionesRecNuevas_REC";
                }
                else
                {
                    nombProcedimiento = "paIncrementarContadorNotificacionesRecCanceladas_REC";
                }

                SqlCommand cmd = new SqlCommand(nombProcedimiento, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SNP_IdSolicitudRecogida", idSolicitudRecogida);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        
        public List<RGAsignarRecogidaDC> ObtenerRecogidasVencidas()
        {
            List<RGAsignarRecogidaDC> lstRecogidas = new List<RGAsignarRecogidaDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerRecogidasEsporadicasVencidas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RGAsignarRecogidaDC recogida = new RGAsignarRecogidaDC
                    {
                        IdCliente = reader["SRP_NUMERODOCUMENTO"].ToString(),
                        NombreCliente = reader["SRP_NOMBRECOMPLETO"].ToString(),
                        DocPersonaResponsable = reader["ASR_DOCPERSONARESPONSABLE"].ToString(),
                        DireccionCliente = reader["SRP_DIRECCIONRECOGIDA"].ToString(),
                        FechaProgramacionRecogida = Convert.ToDateTime(reader["SRE_FechaHoraRecogida"]),
                        IdCiudad = reader["SRE_IdLocalidadRecogida"].ToString()                                             
                    };
                    lstRecogidas.Add(recogida);
                }
                conn.Close();
            }
            return lstRecogidas;
        }

    }
}
