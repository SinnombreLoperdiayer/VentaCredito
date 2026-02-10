using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using CO.Servidor.Raps.Comun.Integraciones.Datos.Datos;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Raps;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.Raps.Comun.Integraciones.Datos
{
    internal class RARepositorioIntegraciones
    {

        private static readonly RARepositorioIntegraciones instancia = new RARepositorioIntegraciones();

        internal static RARepositorioIntegraciones Instancia
        {
            get
            {
                return instancia;
            }
        }


        string conStr = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringRaps = ConfigurationManager.ConnectionStrings["rapsTransaccional"].ConnectionString;

        private RARepositorioIntegraciones()
        {

        }
        

        /// <summary>
        /// Obtiene los parametros por tipo de integracion
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracion(int tipoMotivo, int origenRaps)
        {
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrosIntegracion_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMotivo", tipoMotivo);
                cmd.Parameters.AddWithValue("@IdOrigenRaps", origenRaps);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    lstParametros = MapperRepositorioIntegracion.ToParametrizacionReglasRaps(reader);
                }
                conn.Close();
            }
            return lstParametros;
        }
        

    }
}
