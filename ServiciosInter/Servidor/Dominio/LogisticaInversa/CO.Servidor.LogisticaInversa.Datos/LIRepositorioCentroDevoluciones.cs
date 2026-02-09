using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Datos
{
    public partial class LIRepositorioCentroDevoluciones
    {
        #region Campos
        private static readonly LIRepositorioCentroDevoluciones instancia = new LIRepositorioCentroDevoluciones();
        private string conexionStringArchivo = ConfigurationManager.ConnectionStrings["ControllerArchivo"].ConnectionString;
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private const string NombreModelo = "ModeloLogisticaInversa";
        #endregion

        #region Propiedades
        public static LIRepositorioCentroDevoluciones Instancia
        {
            get { return LIRepositorioCentroDevoluciones.instancia; }
        }
        #endregion

        #region Métodos

        /// <summary>
        /// Metodo para obtener las guias por asignar de una respectiva bodega
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerGuiaBodegaPorAsignar(long numeroGuia)
        {
            OUGuiaIngresadaDC guiaPorAsignar = null;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"paObtenerGuiaBodegaPorAsignarCentroAcopio_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCentroServicio", ControllerContext.Current.IdCentroServicio);
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    guiaPorAsignar = new OUGuiaIngresadaDC()
                    {
                        IdAdmision = (long)reader["ADM_IdAdminisionMensajeria"],
                        NumeroGuia = (long)reader["ADM_NumeroGuia"],
                        EsAlCobro = (bool)reader["ADM_EsAlCobro"],
                        GuiaAutomatica = (bool)reader["ADM_EsAutomatico"],
                        DireccionDestinatario = reader["ADM_DireccionDestinatario"].ToString(),
                        Ciudad = reader["ADM_NombreCiudadDestino"].ToString(),
                        Peso = Math.Round((decimal)reader["ADM_Peso"], 1),
                        NombreTipoEnvio = reader["ADM_NombreTipoEnvio"].ToString(),
                        DiceContener = reader["ADM_DiceContener"].ToString(),
                        FechaAsignacion = (DateTime)reader["INV_FechaGrabacion"],
                        EsReclameEnOficina = ((Convert.ToInt32(reader["ADM_IdTipoEntrega"]) == 2) ? true : false)
                    };
                }
                cmd.Dispose();
                conn.Close();
            }
            return guiaPorAsignar;
        }

        /// <summary>
        /// Metodo para obtener la guia por asignar de un auditor 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerGuiaBodegaPorAsignarAuditor(long numeroGuia)
        {
            OUGuiaIngresadaDC guiaPorAsignar = null;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"paObtenerGuiaBodegaPorAsignarAuditor_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCentroServicio", ControllerContext.Current.IdCentroServicio);
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    guiaPorAsignar = new OUGuiaIngresadaDC()
                    {
                        IdAdmision = (long)reader["ADM_IdAdminisionMensajeria"],
                        NumeroGuia = (long)reader["ADM_NumeroGuia"],
                        EsAlCobro = (bool)reader["ADM_EsAlCobro"],
                        GuiaAutomatica = (bool)reader["ADM_EsAutomatico"],
                        DireccionDestinatario = reader["ADM_DireccionDestinatario"].ToString(),
                        Ciudad = reader["ADM_NombreCiudadDestino"].ToString(),
                        IdCiudad = reader["ADM_IdCiudadDestino"].ToString(),
                        Peso = Math.Round((decimal)reader["ADM_Peso"], 1),
                        NombreTipoEnvio = reader["ADM_NombreTipoEnvio"].ToString(),
                        DiceContener = reader["ADM_DiceContener"].ToString(),
                        FechaAsignacion = (DateTime)reader["INV_FechaGrabacion"]
                    };
                }
                cmd.Dispose();
                conn.Close();
            }
            return guiaPorAsignar;
        }
        #endregion

    }
}
