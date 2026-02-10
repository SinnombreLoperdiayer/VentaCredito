using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;

namespace CO.Servidor.Recogidas.Datos.Administrador
{
    public class RGRepositorioAdministrador
    {
        private static readonly RGRepositorioAdministrador instancia = new RGRepositorioAdministrador();
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static RGRepositorioAdministrador Instancia
        {
            get { return RGRepositorioAdministrador.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public RGRepositorioAdministrador() { }

        /// <summary>
        /// Obtiene los registros a mostrar en el tablero de administracion de  solicitud de recogidas y generar los conteos
        /// </summary>
        /// <param name="idCentroservicio"></param>
        /// <param name="fechaConteo"></param>
        /// <returns></returns>
        public List<RGDetalleConteoAdminRecogidasDC> ConsultaDetConteosRecogidas(string idCentroservicio, DateTime fechaConteo, long documento, string municipio)
        {
            List<RGDetalleConteoAdminRecogidasDC> resultado = null;
            using (SqlConnection SqlConn = new SqlConnection(CadCnxController))
            {

                var cmd = new SqlCommand("paConsultaDetConteosAdminRecogidas_REC", SqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCentroservicio", idCentroservicio);
                cmd.Parameters.AddWithValue("@fechaConteo", fechaConteo);
                if (documento != -1)
                {
                    cmd.Parameters.AddWithValue("@IdDocumento", documento);
                }
                if (municipio != string.Empty && municipio != "00000000")
                {
                    cmd.Parameters.AddWithValue("@IdMunicipio", municipio);
                }
                SqlConn.Open();
                var reader = cmd.ExecuteReader();

                resultado = MapperAdministrador.ToDetalleConteoAdminRecogida(reader);

            }

            return resultado;
        }

        /// <summary>
        /// Obtine las posiciones de dispositivos cercanos a la coordenada cercana
        /// </summary>
        /// <param name="latitud"></param>
        /// <param name="longitud"></param>
        /// <returns></returns>
        public List<RGDispositivoMensajeroDC> ObtenerMensajerosCercanos(decimal latitud, decimal longitud)
        {
            List<RGDispositivoMensajeroDC> resultado = null;

            using (SqlConnection SqlConn = new SqlConnection(CadCnxController))
            {
                var cmd = new SqlCommand("paObtenerMensajerosCercanos_REC", SqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@latitud ", latitud);
                cmd.Parameters.AddWithValue("@longitud ", longitud);
                SqlConn.Open();
                var reader = cmd.ExecuteReader();

                resultado = MapperAdministrador.ToListDispositivoMensajero(reader);
            }

            return resultado;

        }

        public List<RGMensajeroLocalidadDC> ObtenerMensajerosLocalidad(string idLocalidad)
        {
            List<RGMensajeroLocalidadDC> resultado = null;

            using (SqlConnection SqlConn = new SqlConnection(CadCnxController))
            {
                var cmd = new SqlCommand("paObtenerMensajeroslocalidad_REC", SqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idLocalidad", idLocalidad);
                SqlConn.Open();
                var reader = cmd.ExecuteReader();

                resultado = MapperAdministrador.ToListMensajeroLocalidad(reader);
            }

            return resultado;
        }

    }
}
