using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
//using CO.Servidor.Servicios.ContratoDatos.Rutas;
using System.Data;
using System.Data.SqlClient;
using CO.Servidor.Servicios.ContratoDatos.Rutas;

namespace CO.Servidor.Rutas.Datos
{
    public class RURepositorioCWeb
    {
        private static readonly RURepositorioCWeb instancia = new RURepositorioCWeb();
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static RURepositorioCWeb Instancia
        {
            get { return RURepositorioCWeb.instancia; }
        }
        /// <summary>
        /// constructor
        /// </summary>
        public RURepositorioCWeb() { }
        /// <summary>
        /// Obtiene información de la ruta y Coordenadas de centros de servicio de la ruta
        /// </summary>
        /// <returns>Onbjeto RURutaICWeb</returns>
        public List<RURutaICWeb> ObtenerRuta()
        {
            DataTable dsRes = new DataTable();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerInfoRuta_RUT", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                //cmd.Parameters.Add(new SqlParameter("@idRuta", 1));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }
            List<RURutaICWeb> lista = new List<RURutaICWeb>();

            lista = dsRes.AsEnumerable().Select(r => new RURutaICWeb()
            {
                IdRuta = r.Field<Int32>("rut_IdRuta"),
                Nombre = r.Field<string>("rut_Nombre"),
                IdLocalidadOrigen = r.Field<string>("RUT_IdLocalidadOrigen"),
                NombreLocalidadOrigen = r.Field<string>("RUT_NombreLocalidadOrigen"),
                PosOrigen = r.Field<string>("posOrigen"),
                IdLocalidadDestino = r.Field<string>("RUT_IdLocalidadDestino"),
                NombreLocalidadDestino = r.Field<string>("RUT_NombreLocalidadDestino"),
                PosDestino = r.Field<string>("posDestino")
            }).ToList();

            return lista;
        }
        /// <summary>
        /// obtiene centros de servicios de la ruta indicada
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RURutaCWebDetalleCentrosServicios> ObtenerRutaDetalleCentroServiciosRuta(int idRuta, int id)
        {
            DataSet dsResDet = new DataSet();
            DataSet dsResPtos = new DataSet();
            SqlDataAdapter da;
            #region comentariado
            //RURutaICWebDetalle respuesta = new RURutaICWebDetalle();

            //using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            //{
            //    sqlConn.Open();
            //    SqlCommand cmd = new SqlCommand("paObtenerInfoRuta_RUT", sqlConn);
            //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //    cmd.Parameters.Add(new SqlParameter("@idRuta", IdRuta));

            //    da = new SqlDataAdapter(cmd);
            //    da.Fill(dsResDet);
            //}
            //List<RURutaICWeb> lista = new List<RURutaICWeb>();

            //RURutaICWeb Ruta;
            //lista = dsResDet.Tables[0].AsEnumerable().Select( r => new RURutaICWeb()
            //    {
            //        IdRuta = Convert.ToInt32(r["IdRuta"]),
            //        Nombre = r["NombreRuta"].ToString(),
            //        NombreLocalidadOrigen = r["RUT_NombreLocalidadOrigen"].ToString(),
            //        NombreLocalidadDestino = r["RUT_NombreLocalidadDestino"].ToString(),
            //    } ).ToList();

            //foreach (DataRow item in dsResDet.Tables[0].Rows)
            //{
            //    Ruta = new RURutaICWeb()
            //    {
            //        IdRuta = Convert.ToInt32(item["IdRuta"]),
            //        Nombre = item["NombreRuta"].ToString(),
            //        NombreLocalidadOrigen = item["RUT_NombreLocalidadOrigen"].ToString(),
            //        NombreLocalidadDestino = item["RUT_NombreLocalidadDestino"].ToString(),
            //    };

            //    lista.Add(Ruta);
            //}
            //respuesta._RutaDetalle = lista;
            #endregion


            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerDetalleCentrosServiciosRuta_RUT", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idRuta", idRuta);
                cmd.Parameters.AddWithValue("@id", id);

                da = new SqlDataAdapter(cmd);
                da.Fill(dsResPtos);
            }

            List<RURutaCWebDetalleCentrosServicios> listaPtos = new List<RURutaCWebDetalleCentrosServicios>();

            //RURutaCWebDetalleCentrosServicios RutaDetalle;

            listaPtos = dsResPtos.Tables[0].AsEnumerable().Select(r => new RURutaCWebDetalleCentrosServicios()
            {
                NombreLocalidadEstacion = r.Field<string>("ESR_NombreLocalidadEstacion"),
                NombreCentroServicios = r.Field<string>("LCO_idlocalidad"),
                Latitud = r.Field<string>("LCO_Latitud"),
                Longitud = r.Field<string>("LCO_Longitud"),
                Posicion = r.Field<short>("ESR_OrdenEnRuta"),
                IdLocalidadCoordenada = r.Field<int>("ESR_IdLocalidadCoordenada")
            }).ToList();
            //foreach (DataRow item in dsResDet.Tables[0].Rows)
            //{
            //    RutaDetalle = new  RURutaCWebDetalleCentrosServicios()
            //    {


            //    };
            //    listaPtos.Add(RutaDetalle);
            //}

            //respuesta._RutaPtosControl = listaPtos;

            //return respuesta;
            return listaPtos;
        }

        #region INSERTAR

        /// <summary>
        /// elimina punto de ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        public void EliminarPtoRuta(PtoRuta datosPunto)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paEliminarPuntoRuta_RUT", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idPto", datosPunto.IdPto);
                cmd.Parameters.AddWithValue("@idRuta", datosPunto.IdRuta);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// agrega un punto a la ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        public void AgregarPtoRuta(PtoRuta datosPunto)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paAgregarPuntoRuta_RUT", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idRuta", datosPunto.IdRuta);
                cmd.Parameters.AddWithValue("@idPto", datosPunto.IdPto);
                cmd.Parameters.AddWithValue("@posicion", datosPunto.Posicion);
                cmd.Parameters.AddWithValue("@nombreLocalidadEstacion", datosPunto.NombreLocalidadEstacion);
                cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@creadoPor", datosPunto.CreadoPor);
                cmd.Parameters.AddWithValue("@IdLocalidadCoordenada", datosPunto.IdLocalidadCoordenada);

                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /* public void AgregarRuta(string origen, string destino,string nombre,int tipoRuta,int medioTransporte,int generaManifiesto)
         {
             using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
             {
                 sqlConn.Open();
                 SqlCommand cmd = new SqlCommand("", sqlConn);
                 cmd.CommandType = System.Data.CommandType.StoredProcedure;
                 cmd.Parameters.Add(new SqlParameter ("@origen",origen));
                 cmd.Parameters.Add(new SqlParameter ("@destino",destino));
                 cmd.Parameters.Add(new SqlParameter("@nombre", nombre));
                 cmd.Parameters.Add(new SqlParameter("@tipoRuta", tipoRuta));
                 cmd.Parameters.Add(new SqlParameter("@medioTransporte", medioTransporte));
                 cmd.Parameters.Add(new SqlParameter("@generaManifiesto", generaManifiesto));
                 cmd.ExecuteNonQuery();
                 sqlConn.Close();
             }
         }*/
        #endregion
        /// <summary>
        /// crear punto
        /// </summary>
        /// <param name="datosPunto"></param>
        public void CrearPunto(PtoRuta datosPunto)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarPuntoLocalidadCorrdenada_PAR", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LCO_TipoCoordenada", datosPunto.TipoCoordenada);
                cmd.Parameters.AddWithValue("@LCO_IdLocalidad", datosPunto.IdLocalidad);
                cmd.Parameters.AddWithValue("@LCO_Latitud", datosPunto.Latitud);
                cmd.Parameters.AddWithValue("@LCO_Longitud", datosPunto.Longitud);
                cmd.Parameters.AddWithValue("@LCO_Cobertura", datosPunto.Cobertura);
                cmd.Parameters.AddWithValue("@LCO_FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@LCO_CreadoPor", datosPunto.CreadoPor);
                cmd.Parameters.AddWithValue("@LCO_Nombre", datosPunto.NombreLocalidadEstacion);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }
        /// <summary>
        /// asigna posicion en ruta a punto indicado
        /// </summary>
        /// <param name="datosPunto"></param>
        public void OrganizarPtos(PtoRuta datosPunto)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paActualizarOrdenPtosRuta_ESR", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ordenEnRuta", datosPunto.OrdenEnRuta);
                cmd.Parameters.AddWithValue("@idLocalidadEstacion", datosPunto.IdPto);
                cmd.Parameters.AddWithValue("@idRuta", datosPunto.IdRuta);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }

        }
        /// <summary>
        /// obtiene todos los medios de transporte
        /// </summary>
        /// <returns></returns>
        public List<RUMedioTransporte> ObtenerMediosTransporte()
        {
            DataSet dsMT = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMedioTransporte_PAR", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                //  cmd.ExecuteNonQuery();
                da = new SqlDataAdapter(cmd);
                da.Fill(dsMT);
            }

            List<RUMedioTransporte> listaMedioTransporte = new List<RUMedioTransporte>();

            //RURutaCWebDetalleCentrosServicios RutaDetalle;

            listaMedioTransporte = dsMT.Tables[0].AsEnumerable().Select(r => new RUMedioTransporte()
            {
                IdMedioTransporte = r.Field<short>("MTR_IdMedioTransporte"),
                DescMedioTransporte = r.Field<string>("MTR_Descripcion"),
            }).ToList();

            return listaMedioTransporte;
        }
        /// <summary>
        /// obtiene todos lod tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        public List<RUTipoVehiculo> ObtenerTiposVehiculos()
        {
            DataSet dsTV = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTiposVehiculos_CPO", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                da = new SqlDataAdapter(cmd);
                da.Fill(dsTV);
            }

            List<RUTipoVehiculo> listaTiposVehiculos;

            listaTiposVehiculos = dsTV.Tables[0].AsEnumerable().Select(r => new RUTipoVehiculo()
            {
                IdTipoVehiculo = r.Field<short>("TIV_IdTipoVehiculo"),
                DescTipoVehiculo = r.Field<string>("TIV_Descripcion")
            }).ToList();

            return listaTiposVehiculos;

        }
        /// <summary>
        /// obtiene todos los tipos de ruta
        /// </summary>
        /// <returns></returns>
        public List<RUTipoRuta> ObtenerTiposRuta()
        {
            DataSet dsTR = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTiposRuta_RUT", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                da = new SqlDataAdapter(cmd);
                da.Fill(dsTR);
            }

            List<RUTipoRuta> listaTipoRuta;

            listaTipoRuta = dsTR.Tables[0].AsEnumerable().Select(r => new RUTipoRuta()
            {
                IdTipoRuta = int.Parse(r.Field<short>("TRU_IdTipoRuta").ToString()),
                NombreTipoRuta = r.Field<string>("TRU_Descripcion")
            }).ToList();

            return listaTipoRuta;
        }
        /// <summary>
        /// crea nueva ruta
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public int CrearRuta(RURutaICWeb ruta)
        {
            int idRuta;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarRuta_RUT", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", ruta.Nombre);
                cmd.Parameters.AddWithValue("@IdTipoRuta", ruta.IdTipoRuta);
                cmd.Parameters.AddWithValue("@IdMedioTransporte", ruta.IdMedioTransporte);
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", ruta.IdLocalidadOrigen);
                cmd.Parameters.AddWithValue("@NombreLocalidadOrigen", ruta.NombreLocalidadOrigen);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", ruta.IdLocalidadDestino);
                cmd.Parameters.AddWithValue("@NombreLocalidadDestino", ruta.NombreLocalidadDestino);
                cmd.Parameters.AddWithValue("@GeneraManifiestoMinisterio", ruta.GeneraManifiesto);
                cmd.Parameters.AddWithValue("@CostoMensualTotal", ruta.CostoMensualTotal);
                cmd.Parameters.AddWithValue("@IdTipoVehiculo", ruta.IdTipoVehiculo);
                cmd.Parameters.AddWithValue("@CreadoPor", ruta.CreadoPor);
                cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@RutaMasivos", ruta.RutaMasivos);
                idRuta = int.Parse(cmd.ExecuteScalar().ToString());
                sqlConn.Close();
            }

            return idRuta;
        }

    }
}
