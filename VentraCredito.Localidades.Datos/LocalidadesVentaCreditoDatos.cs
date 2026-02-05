using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using VentaCredito.Transversal.Entidades;
using System.Data;
using System.Data.SqlClient;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;

namespace VentraCredito.Localidades.Datos
{
    public class LocalidadesVentaCreditoDatos
    {
        #region Singleton

        private static LocalidadesVentaCreditoDatos instancia = new LocalidadesVentaCreditoDatos();
        private string CnxController;
        public static LocalidadesVentaCreditoDatos Instancia
        {
            get
            {
                return instancia;
            }
        }

        #endregion  

        public LocalidadesVentaCreditoDatos()
        {
            CnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        }

        /// <summary>
        /// Retorna la existencia de una localidad (0 = No existe / 1 = Si existe)
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns>Existencia de la localidad</returns>
        public bool ConsultarExistenciaLocalidad(string idLocalidad)
        {
            bool existe = false;

            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("pa_ConsultarExistenciaLocalidad_PAR", sqlConn);
                cmd.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    existe = reader["ExisteLocalidad"].ToString() == "1" ? true : false;
                }

                sqlConn.Close();
            }

            return existe;
        }

        /// <summary>
        /// Obtiene las localidades de destino asociadas a una sucursal.
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns>Lista con nombre y id de las localidades de destino asociadas a un cliente credito, filtrado por sucursal.</returns>
        public List<LocalidadesCLI> ObtenerlocalidadesVentaCredito(int idSucursal)
        {
            List<LocalidadesCLI> lstLocalidades = new List<LocalidadesCLI>();

            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerLocalidadesPorSucursalYCliCre_CLI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idSucursal", idSucursal);
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var localidad = new LocalidadesCLI()
                        {
                            IdLocalidad = Convert.ToString(reader["IdLocalidad"]),
                            NombreCompleto = Convert.ToString(reader["NombreCompleto"])
                        };

                        lstLocalidades.Add(localidad);
                    }
                }
            }

            return lstLocalidades;
        }

        /// <summary>
        /// Retorna el Estado (0 = Inactivo / 1 = Activo) de una localidad para saber si es viable o no para que permita crear preenvios
        /// </summary>
        /// <param name="IdLocalidadDestino"></param>
        /// <returns>Estado de validez de la localidad destino</returns>
        public bool ConsultarValidezDestinoGeneracionGuias(string IdLocalidadDestino)
        {
            bool valido = false;

            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("pa_ConsultarValidezDestinoGeneracionGuias_PAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", IdLocalidadDestino);
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    valido = Convert.ToBoolean(reader["DestinoValidoGeneracionGuiaEstadoActivo"].ToString());
                }

                sqlConn.Close();
            }

            return valido;
        }

        /// <summary>
        /// Consulta el SubTipo de un Centro de Servicios asociado a una localidad
        /// </summary>
        /// <param name="idLocalidadCS"></param>
        /// <returns>Retorna el SubTipo de un Centro de Servicios asociado a una localidad</returns>
        public bool ConsultarSubtipoCentroServiciosPorLocalidad(string idLocalidadCS, int idTipoEntrega)
        {
            bool CentroServTieneSubTipoCP = false;

            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("pa_ConsultarSubtipoCentroServiciosPorLocalidad_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", idLocalidadCS);
                cmd.Parameters.AddWithValue("@IdTipoEntrega", idTipoEntrega);
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    CentroServTieneSubTipoCP = Convert.ToBoolean(reader["Response"].ToString());
                }

                sqlConn.Close();
            }

            return CentroServTieneSubTipoCP;
        }

        /// <summary>
        /// Retorna el Id de la Zona más idónea para la nueva localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns>Devuelve "-1" si la Localidad la maneja, sino, entonces la primera del listado en orden ascendente</returns>
        public string ConsultarZonaHabilitadaParaNuevaLocalidad(string idLocalidad)
        {
            string IdZona = string.Empty;

            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("pa_ConsultarZonaHabilitadaParaNuevaLocalidad_PAR", sqlConn);
                cmd.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    IdZona = reader["IdZona"].ToString();
                }

                sqlConn.Close();
            }

            return IdZona;
        }

        /// <summary>
        /// Retorna Lista de Zonas Dificil Acceso
        /// </summary>
        /// <returns>Retorna Lista de Zonas Dificil Acceso</returns>
        public List<PAZonaDificilAcceso> ConlsutarZonaDificilAcceso()
        {

            List<PAZonaDificilAcceso> lstZonaDificilAcceso = new List<PAZonaDificilAcceso>();

            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerZonaDificilAcceso_PAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PAZonaDificilAcceso ZonaDificilAcceso = new PAZonaDificilAcceso()
                        {
                            IdLocalidad = Convert.ToInt64(reader["ZDA_IdLocalidad"]),
                            ZonaDescripcion = Convert.ToString(reader["ZDA_ZonaDescripcion"]),
                            CreadoPor = Convert.ToString(reader["ZDA_CreadoPor"])
                        };

                        lstZonaDificilAcceso.Add(ZonaDificilAcceso);
                    }
                }
            }
            return lstZonaDificilAcceso;        
        }

    }
}
