//using CO.Servidor.Raps.ReglasFallasRaps;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace CO.Servidor.Raps.Datos
{
    public class RARepositorioSolicitudes : ControllerBase
    {
        private string conexionStringRaps = ConfigurationManager.ConnectionStrings["rapsTransaccional"].ConnectionString;
        private string conexionStringNovasoft = ConfigurationManager.ConnectionStrings["novasoftTransaccional"].ConnectionString;
        private static readonly RARepositorioSolicitudes instance = new RARepositorioSolicitudes();

        #region singleton

        public static RARepositorioSolicitudes Instancia
        {
            get { return RARepositorioSolicitudes.instance; }
        }

        public RARepositorioSolicitudes() { }

        #endregion

        #region metodos

        /// <summary>
        /// Reasigna una solicitud escalada por vencimiento de tiempo
        /// </summary>
        /// <param name="solicitud"></param>
        public void ReasignarSolicitudEscaldaVencimiento(RASolicitudDC solicitud)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paEscalarSolicitudVencida_RAP", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCargoNuevoResponsable", solicitud.IdCargoResponsable);
                cmd.Parameters.AddWithValue("@DocumentoNuevoResponsable", solicitud.DocumentoResponsable);
                cmd.Parameters.AddWithValue("@NuevaFechaVencimiento", solicitud.FechaVencimiento);
                cmd.Parameters.AddWithValue("@IdSolicitud", solicitud.IdSolicitud);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

            }

        }
        #endregion

        #region gestion
        /// <summary>
        /// Crea un gestion de una solicitud
        /// </summary>
        /// <param name="Gestion"></param>
        /// <returns></returns>
        public long CrearGestion(RAGestionDC Gestion)
        {

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarGestion_RAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdSolicitud", Gestion.IdSolicitud);
                cmd.Parameters.AddWithValue("@Comentario", Gestion.Comentario == null ? "" : Gestion.Comentario);
                cmd.Parameters.AddWithValue("@IdCargoGestiona", Gestion.IdCargoGestiona);
                cmd.Parameters.AddWithValue("@CorreoEnvia", string.IsNullOrEmpty(Gestion.CorreoEnvia) ? "" : Gestion.CorreoEnvia);// DE ACUERDO AL CARGO QUE GESTIONA
                //cmd.Parameters.AddWithValue("@IdAccion", Gestion.IdAccion);
                cmd.Parameters.AddWithValue("@IdCargoDestino", Gestion.IdCargoDestino);
                cmd.Parameters.AddWithValue("@CorreoDestino", string.IsNullOrEmpty(Gestion.CorreoDestino) ? "" : Gestion.CorreoDestino);//
                cmd.Parameters.AddWithValue("@IdResponsable", Gestion.IdResponsable); // SOLICITUD
                cmd.Parameters.AddWithValue("@IdEstado", Gestion.IdEstado);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current == null ? Gestion.IdUsuario : ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@FechaVencimiento", Gestion.FechaVencimiento);
                cmd.Parameters.AddWithValue("@DocumentoSolicita", Gestion.DocumentoSolicita);
                cmd.Parameters.AddWithValue("@DocumentoResponsable", Gestion.DocumentoResponsable);
                return Convert.ToInt64(cmd.ExecuteScalar());
            }

        }

        /// <summary>
        ///Inserta en el log de notifificaciones de escalamiento enviadas por el motor
        /// </summary>
        /// <param name="solicitud"></param>
        public void InsertarNotificacionGestionMotor(RAEscalonamientoDC escalonamiento, RACargoEscalarDC cargoEscalar, long idGestion, long idSolicitud, string observaciones = null)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paGuardarNotificacionGestion_RAP", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdGestion", idGestion);
                cmd.Parameters.AddWithValue("@CorreoNotificado", cargoEscalar.Correo);
                cmd.Parameters.AddWithValue("@NumeroDocumento", cargoEscalar.DocumentoEmpleado);
                cmd.Parameters.AddWithValue("@IdCargo", cargoEscalar.IdCargoController);
                cmd.Parameters.AddWithValue("@CreadoPor", "MotorRAPS");
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", escalonamiento.IdParametrizacionRap);
                cmd.Parameters.AddWithValue("@Orden", escalonamiento.Orden);
                cmd.Parameters.AddWithValue("@Observaciones", observaciones);
                cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitud);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

            }

        }

        /// <summary>
        /// obtiene los datos como correo envia, correo destino documento del responsable e id cargo responsable
        /// </summary>
        public RAGestionDC ObtenerDatosParaInsertarGestion_RAP(long idSolicitud, string idCargoGestiona)
        {
            RAGestionDC gestionDatos = new RAGestionDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDatosParaInsertarGestion_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                cmd.Parameters.AddWithValue("@idCargoGestiona", idCargoGestiona);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    gestionDatos.CorreoEnvia = reader["CORREO_ENVIA"].ToString();
                    gestionDatos.CorreoDestino = reader["CORREO_DESTINO"].ToString();
                    gestionDatos.IdResponsable = reader["IdCargoResponsable"].ToString();
                    gestionDatos.IdCargoDestino = gestionDatos.IdResponsable;
                    gestionDatos.DocumentoResponsable = reader["DocumentoResponsable"].ToString();
                }
                conn.Close();
            }
            return gestionDatos;
        }

        /// <summary>
        /// obtiene la informacion de un empleado desde novasoft 
        /// </summary>
        public RAIdentificaEmpleadoDC ObtenerDatosEmpleado(string cedula)
        {
            RAIdentificaEmpleadoDC datos = new RAIdentificaEmpleadoDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paConsultaEmpleadoNovasoft", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Documento", cedula);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    datos.NumeroIdentificacion = reader["Codigoempleado"].ToString();
                    datos.Nombre = reader["NombreEmpleado"].ToString();
                    datos.IdCargo = reader["CodigoCargo"].ToString();
                    datos.email = reader["e_mail"].ToString();
                    datos.DescripcionCargo = reader["nombreCargo"].ToString();
                    datos.CodigoPlanta = reader["CodigoPlanta"].ToString();
                }
                conn.Close();
            }
            return datos;
        }




        #endregion

        #region adjunto


        /// <summary>
        /// Crear adjunto
        /// </summary>
        /// <param name="adjunto"></param>
        /// <returns></returns>
        public bool CrearAdjunto(RAAdjuntoDC adjunto)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarAdjuntoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", adjunto.IdSolicitud);
                cmd.Parameters.AddWithValue("@IdGestion", adjunto.IdGestion);
                cmd.Parameters.AddWithValue("@Tamaño", adjunto.Tamaño);
                cmd.Parameters.AddWithValue("@Extension", adjunto.Extension);
                cmd.Parameters.AddWithValue("@UbicacionNombre", adjunto.UbicacionNombre);


                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }


        /// <summary>
        /// Lista los adjuntos de una gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        public List<RAAdjuntoDC> ListarAdjunto(long idSolicitud)
        {
            List<RAAdjuntoDC> resultado = new List<RAAdjuntoDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarAdjuntoRAP", sqlConn);
                cmd.CommandTimeout = 60;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdSolicitud", idSolicitud));

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperListaAdjunto(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Obtener una adjunto
        /// </summary>
        /// <param name="idAdjunto"></param>
        /// <returns></returns>
        public RAAdjuntoDC ObtenerAdjunto(long idAdjunto)
        {
            RAAdjuntoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAdjuntoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", idAdjunto));

                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperAAdjunto(resultReader);
                }

            }
            return resultado;
        }

        #endregion

        #region Creacionsolicitudes
        /// <summary>
        /// lista parametroRaps activos del tipoRaps 
        /// </summary>
        /// <param name="idTipoRap"></param>
        /// <returns></returns>
        public IEnumerable<RAParametrizacionRapsDC> ListarParametroRapXTipoRapAct(int idTipoRap)
        {
            IEnumerable<RAParametrizacionRapsDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarParametroRapsXTipoRapRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdTipoRap", idTipoRap));

                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAListaParametrizacionRaps(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea una nueva solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public long CrearSolicitud(RASolicitudDC solicitud)
        {
            long resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarSolicitudRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", solicitud.IdParametrizacionRap);
                cmd.Parameters.AddWithValue("@IdCargoSolicita", solicitud.IdCargoSolicita);
                cmd.Parameters.AddWithValue("@IdCargoResponsable", solicitud.IdCargoResponsable);
                cmd.Parameters.AddWithValue("@FechaCreacion", solicitud.FechaCreacion);
                cmd.Parameters.AddWithValue("@FechaInicio", solicitud.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaVencimiento", solicitud.FechaVencimiento);
                cmd.Parameters.AddWithValue("@IdEstado", solicitud.IdEstado);
                cmd.Parameters.AddWithValue("@Descripcion", solicitud.Descripcion);
                cmd.Parameters.AddWithValue("@IdSolicitudPadre", solicitud.IdSolicitudPadre);
                cmd.Parameters.AddWithValue("@DocumentoSolicita", solicitud.DocumentoSolicita);
                cmd.Parameters.AddWithValue("@DocumentoResponsable", solicitud.DocumentoResponsable);
                cmd.Parameters.AddWithValue("@CodigoSucursal", solicitud.idSucursal.ToString());
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@EsAcumulativa", solicitud.EsAcumulativa);
                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    if (resultReader.Read())
                    {
                        resultado = Convert.ToInt64(resultReader["Solicitud"]);
                    }
                }
            }

            return resultado;
        }

        /// <summary>
        /// Crea una solucitud de tipo acumulativa
        /// </summary>
        /// <param name="regSolicitudAcumulativa"></param>
        /// <returns></returns>
        public long CrearSolicitudAcumulativa(RASolicitudDC solicitud)
        {
            long resultado = 0;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarSolicitudAcumulativa_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", solicitud.IdParametrizacionRap);
                cmd.Parameters.AddWithValue("@DescripcionSolicitud", solicitud.Descripcion);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@CodigoSucursal", solicitud.idSucursal);
                conn.Open();
                resultado = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();
            }
            return resultado;
        }

        #endregion

        #region gestion
        public RAIdentificaEmpleadoDC ConsultaInformacionSolicitante(long idSolicitud)
        {
            RAIdentificaEmpleadoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultaEmpleadoSolicitanteRaps", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdSolicitud", idSolicitud));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperAIdentificaEmpleado(resultReader);
                }
            }

            return resultado;

        }
        #endregion

        #region consultas
        public List<RAConteoEstadosSolicitante> ObtenerConteoEstadosSolicitudes(string idDocumentoSolicita)
        {
            List<RAConteoEstadosSolicitante> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConteoEstadosSolicitante", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DocumentoSolicita", idDocumentoSolicita));

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperConteoEstadosSolicitudes(resultReader);
                }
            }

            return resultado;
        }

        public List<RAObtenerListaSolicitudesRaps> ObtenerListaSolicitudesRaps(long DocumentoSolicita, int IdEstado)
        {
            List<RAObtenerListaSolicitudesRaps> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerListaSolicitudesRaps", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DocumentoSolicita", DocumentoSolicita));
                cmd.Parameters.Add(new SqlParameter("@IdEstado", IdEstado));

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperObtenerListaSolicitudesRaps(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Obtiene el detalle de los parametros de una solicitud acumulativa
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RAListaParametrosAcumulativasDC> ObtenerDetalleParametrosAcumulativas(long idsolicitud, long idParametrizacion)
        {
            List<RAListaParametrosAcumulativasDC> lstAgrupamientoParametros = new List<RAListaParametrosAcumulativasDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {

                SqlCommand cmd = new SqlCommand("paObtenerSolicitudesAcumulativasPorIdSolicitud_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", idsolicitud);
                cmd.Parameters.AddWithValue("@IdParametrizacion", idParametrizacion);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAListaParametrosAcumulativasDC AgrupamientoParametros = new RAListaParametrosAcumulativasDC();
                    SqlCommand cmd2 = new SqlCommand("paObtenerDetalleParametrosAcumulativas_RAPS", conn);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@IdSolicitudAcumulativa", Convert.ToInt64(reader["SOA_IdSolicitudAcumulativa"]));
                    SqlDataReader reader2 = cmd2.ExecuteReader();
                    List<RADetalleParametrosAcumulativasDC> lstdetalle = new List<RADetalleParametrosAcumulativasDC>();
                    while (reader2.Read())
                    {
                        RADetalleParametrosAcumulativasDC detalle = new RADetalleParametrosAcumulativasDC();
                        detalle.Descripcion = reader2["DescripcionParametro"].ToString();
                        detalle.TipoDato = Convert.ToInt32(reader2["IdTipoDato"]);
                        if (detalle.TipoDato == 5 || detalle.TipoDato == 6)
                        {
                            using (FileStream fs = File.OpenRead(reader2["PSA_Valor"].ToString()))
                            {
                                byte[] bytes = new byte[fs.Length];
                                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                                fs.Close();
                                detalle.Valor = Convert.ToBase64String(bytes);
                            }
                        }
                        else
                        {
                            detalle.Valor = reader2["PSA_Valor"].ToString();
                        }
                        lstdetalle.Add(detalle);
                    }
                    AgrupamientoParametros.ListaParametros = lstdetalle;
                    lstAgrupamientoParametros.Add(AgrupamientoParametros);
                }
                conn.Close();
            }
            return lstAgrupamientoParametros;
        }

        /// <summary>
        /// obtiene las gestiones de una solicitud
        /// </summary>
        /// <param name="IdSolicitud"></param>
        /// <returns></returns>
        public List<RAGestionDC> ListarGestion(long IdSolicitud)
        {
            List<RAGestionDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarGestionRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdSolicitud", IdSolicitud));
                cmd.CommandTimeout = 60;

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperListaGestion(resultReader);
                }
            }

            return resultado;

        }

        /// <summary>
        /// Obtiene los adjuntos de dicha gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        public List<RAAdjuntoDC> ObtenerAdjuntosPorGestion(long idGestion)
        {
            List<RAAdjuntoDC> lstAdjuntos = new List<RAAdjuntoDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd2 = new SqlCommand("paObtenerAdjuntosPorGestion_RAP", sqlConn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.AddWithValue("@IdGestion", idGestion);
                sqlConn.Open();
                SqlDataReader reader = cmd2.ExecuteReader();
                while (reader.Read())
                {
                    RAAdjuntoDC adjunto = new RAAdjuntoDC();
                    using (FileStream fs = File.OpenRead(reader["UbicacionNombre"].ToString()))
                    {
                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                        fs.Close();
                        adjunto.AdjuntoBase64 = Convert.ToBase64String(bytes);
                    }
                    adjunto.NombreArchivo = reader["UbicacionNombre"].ToString();
                    adjunto.Extension = reader["Extension"].ToString();
                    adjunto.Tamaño = Convert.ToDecimal(reader["Tamaño"]);
                    lstAdjuntos.Add(adjunto);
                }
                sqlConn.Close();
            }
            return lstAdjuntos;
        }

        /// <summary>
        /// obtiene la informacion  de un item de gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        public RAGestionDC ObtenerGestion(long idGestion)
        {
            RAGestionDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGestionRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdGestion", idGestion));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperAGestion(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Obtiene una plantilla de correo
        /// </summary>
        /// <param name="idPlantilla"></param>
        /// <returns></returns>
        public RAPantillaAccionCorreoDC ObtenerPantillaAccionCorreo(long idPlantilla)
        {
            RAPantillaAccionCorreoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerPantillaAccionCorreoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdPlantilla", idPlantilla));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperAPantillaAccionCorreo(resultReader);
                }
            }

            return resultado;
        }

        public List<RASolicitudItemDC> ListarSolicitudes(string responsableSolicitud, RAEnumEstados estadoSolicitud)
        {
            List<RASolicitudItemDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarSolicitudes_Raps", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ResponsableSolicitud", responsableSolicitud);
                cmd.Parameters.AddWithValue("@EstadoSolicitud", estadoSolicitud);

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperListaSolicitudItem(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Listar solicitudes
        /// </summary>
        /// <returns></returns>
        public List<RASolicitudDC> ListarSolicitud()
        {
            List<RASolicitudDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarSolicitudRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperListaSolicitud(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// obtener solicitud por id
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public RASolicitudDC ObtenerSolicitudRap(long idSolicitud)
        {
            RASolicitudDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerSolicitudRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdSolicitud", idSolicitud));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperASolicitud(resultReader);
                }
            }

            return resultado;
        }

        public List<RAResultadoConsultaSolicitudesDC> ListarSolicitudesPaginada(RaParametrosConsultaSolicitudesDC parametrosConsultaSolicitudes)
        {
            List<RAResultadoConsultaSolicitudesDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarSolicitudesPaginada", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@pagina", parametrosConsultaSolicitudes.Pagina));
                cmd.Parameters.Add(new SqlParameter("@registrosXPagina", parametrosConsultaSolicitudes.RegistrosXPagina));
                cmd.Parameters.Add(new SqlParameter("@ordenaPor", parametrosConsultaSolicitudes.OrdenarPor));
                cmd.Parameters.Add(new SqlParameter("@IdEstado", parametrosConsultaSolicitudes.IdEstado));
                cmd.Parameters.Add(new SqlParameter("@Filtro", parametrosConsultaSolicitudes.Filtro));
                cmd.Parameters.Add(new SqlParameter("@ValorFiltro", parametrosConsultaSolicitudes.ValorFiltro));



                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperAResultadoConsultaSolicitudes(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene los tipod de novedad segun el sistema origen
        /// </summary>
        /// <param name="idSistemaOrigen"></param>
        /// <returns></returns>
        public List<RANovedadDC> ObtenerTiposNovedad(int idSistemaOrigen, int idTipoNovedad)
        {
            List<RANovedadDC> tiposNovedad = new List<RANovedadDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposNovedad_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idSistemaOrigen", idSistemaOrigen);
                cmd.Parameters.AddWithValue("@idTipoNovedad", idTipoNovedad);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RANovedadDC novedad = new RANovedadDC()
                    {
                        idTipoNovedad = Convert.ToInt32(reader["TPN_IdTipoNovedad"]),
                        descripcionNovedad = reader["TPN_DescripcionTipoNovedad"].ToString()
                    };
                    tiposNovedad.Add(novedad);
                }
                conn.Close();
            }
            return tiposNovedad;
        }

        /// <summary>
        /// Retorna las veces que esta un tipo de novedad en parametrizaciones activas
        /// </summary>
        /// <param name="idTipoNovedad">Id de la novedad</param>
        /// <returns></returns>
        public RANovedadDC ObtenerCantidadTiposNovedad(long idTipoNovedad)
        {
            Int16 cantidad = 0;
            RANovedadDC tipoNovedad = new RANovedadDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCantidadTiposNovedad_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idTipoNovedad", idTipoNovedad);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tipoNovedad.idTipoNovedad = (int)idTipoNovedad;
                    tipoNovedad.Cantidad = Convert.ToInt16(reader["CountTipoNovedad"]);
                }
                conn.Close();
                return tipoNovedad;
            }
        }

        public RASolicitudConsultaDC ObtenerSolicitud(long idSolicitud)
        {
            RASolicitudConsultaDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("PaObtenerSolicitudRap", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdSolicitud", idSolicitud));
                cmd.CommandTimeout = 60;
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioSolicitudesMapper.MapperAResultadoConsultaSolicitud(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la persona a la cual se le asigna un rap, a partir de la parametrizacion y la ciudad
        /// </summary>
        /// <param name="idParametrizacion"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public RAEscalonamientoDC ObtenerPersonaAsignarRap(long idParametrizacion, string idCiudad, string idSucursalDestino = null)
        {
            string idSucursal;
            var idCiudadNovasoft = idCiudad.Substring(0, 5);


            if (idCiudadNovasoft == "11001")
            {
                idSucursal = !string.IsNullOrEmpty(idSucursalDestino) && idSucursalDestino == "201" ? "201" : "111";
            }
            else
            {
                idSucursal = RARepositorio.Instancia.ObtenerSucursalNovasoft(idCiudadNovasoft);
            }

            DataTable dt = new DataTable();
            Dictionary<string, DataTable> dicPersonasCargo = new Dictionary<string, DataTable>();

            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {


                SqlCommand cmd = new SqlCommand("paObtenerEscalonamientoParametrizacionRapsSinInfoNova_Raps", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", idParametrizacion);
                cmd.Parameters.AddWithValue("@CodSucursal", idSucursal);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dtEscalona = new DataTable();
                da.Fill(dtEscalona);
                conn.Close();

                var escalonamiento = dtEscalona.AsEnumerable().ToList();
                List<RAEscalonamientoDC> lstEscalonamiento = new List<RAEscalonamientoDC>();
                //agrupa por cargo y extrae el escalonamiento completo del rap
                ///verifica si el cargo tiene varios empleados, y toma el que tenga menos solicitudes asignadas abiertas                          
                if (escalonamiento != null && escalonamiento.Count > 0)
                {

                    foreach (var esca in escalonamiento)
                    {
                        Dictionary<string, int> cantidades = new Dictionary<string, int>();
                        DataTable dtPersonaCargo = new DataTable();
                        string llaveDiccionario = idSucursal + "-" + esca.Field<string>("IdCargo").Trim();

                        lock (this)
                        {
                            //Cache personas asociadas al cargo. Se verifica si la sucursal con el cargo ya fue consultado para reutilizarlo
                            if (dicPersonasCargo.ContainsKey(llaveDiccionario))
                            {
                                dtPersonaCargo = dicPersonasCargo[llaveDiccionario];
                            }
                            else
                            {
                                using (SqlConnection connNova = new SqlConnection(conexionStringNovasoft))
                                {
                                    //busca las personas asociadas al cargo en la sucursal, si no existe en la sucursal (regional) la busca en casa matriz (201)
                                    cmd = new SqlCommand("paBuscarPersonaCargoSucursalNovasoft_RAP", connNova);
                                    cmd.CommandTimeout = 1200;
                                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@CodSucursal", idSucursal);
                                    cmd.Parameters.AddWithValue("@CodCargo", esca.Field<string>("IdCargo").Trim());
                                    cmd.Parameters.AddWithValue("@IdProceso", esca["IdProceso"] != DBNull.Value ? esca.Field<string>("IdProceso").Trim() : "");
                                    cmd.Parameters.AddWithValue("@IdProcedimiento", esca["IdProcedimiento"] != DBNull.Value ? esca.Field<string>("IdProcedimiento").Trim() : "");
                                    cmd.Parameters.AddWithValue("@CodCasaMatriz", ObtenerCodigoCasaMatriz());
                                    connNova.Open();
                                    da = new SqlDataAdapter(cmd);
                                    da.Fill(dtPersonaCargo);
                                    connNova.Close();
                                }
                                dicPersonasCargo.Add(llaveDiccionario, dtPersonaCargo);
                            }
                        }

                        if (dtPersonaCargo.AsEnumerable().Count() > 0)
                        {

                            //Busca las asignaciones por persona
                            dtPersonaCargo.AsEnumerable().ToList().ForEach(pCargo =>
                            {
                                cmd = new SqlCommand("paVerificarCantidadAsignacionesEmpleado_RAP", conn);
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.CommandTimeout = 120;
                                cmd.Parameters.AddWithValue("@documentoEmpleado", pCargo.Field<string>("Cod_emp").Trim());
                                conn.Open();
                                int cantidad = Convert.ToInt32(cmd.ExecuteScalar());
                                conn.Close();
                                cantidades.Add(pCargo.Field<string>("Cod_emp").Trim(), cantidad);
                            });
                            string documentoAsignar = "", emailAsignar = "", codSucursalAsignar = "", codPlantaAsignar = "";
                            documentoAsignar = cantidades.OrderBy(c => c.Value).First().Key;
                            var personaAsig = dtPersonaCargo.AsEnumerable().ToList().Where(p => p.Field<string>("Cod_emp") == documentoAsignar).First();
                            emailAsignar = personaAsig.Field<string>("e_mail_alt");
                            codSucursalAsignar = personaAsig.Field<string>("cod_suc");
                            codPlantaAsignar = personaAsig.Field<string>("codPlantaNova");


                            RAEscalonamientoDC escalona = new RAEscalonamientoDC()
                            {
                                CorreoEscalar = emailAsignar,
                                idCargo = esca.Field<string>("idCargo").Trim(),
                                IdParametrizacionRap = idParametrizacion,
                                IdSucursalEscalar = codSucursalAsignar,
                                IdTipoHora = esca.Field<int>("IdTipoHora"),
                                Orden = esca.Field<int>("Orden"),
                                HorasEscalar = esca.Field<byte>("HorasEscalar"),
                                DocumentoEmpeladoEscalar = documentoAsignar,
                                CargoEscalar = new RACargoEscalarDC()
                                {
                                    Correo = emailAsignar,
                                    DocumentoEmpleado = documentoAsignar,
                                    IdCargoController = esca.Field<string>("idCargo").Trim(),
                                    CodPlantaNovasoft = codPlantaAsignar,
                                    IdCargoNovasoft = esca.Field<string>("idCargo").Trim(),
                                    IdCiudad = personaAsig.Field<string>("cod_ciu"),
                                    NombreEmpleado = personaAsig.Field<string>("NombreEmpleado"),
                                    Sucursal = codSucursalAsignar,
                                    HorarioEmpleado = new List<RAHorarioEmpleadoDC>()
                                }
                            };

                            cmd = new SqlCommand("paObtenerHorarioEmpleadoNovasoft_Raps", conn);
                            cmd.CommandTimeout = 120;
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@documentoEmpl", documentoAsignar);
                            //  conn.Open();
                            da = new SqlDataAdapter(cmd);
                            DataTable dthorarioempleado = new DataTable();
                            da.Fill(dthorarioempleado);
                            //  conn.Close();
                            int i = 0;
                            dthorarioempleado.AsEnumerable().ToList().ForEach(hora =>
                            {
                                escalona.CargoEscalar.HorarioEmpleado.Add(
                                                                    new RAHorarioEmpleadoDC()
                                                                    {
                                                                        IdDia = hora.Field<int>("diaSemana"),
                                                                        HoraEntrada = DateTime.Parse(hora["HoraEntrada"].ToString()).AddDays(i),
                                                                        HoraSalida = DateTime.Parse(hora["HoraSalida"].ToString()).AddDays(i)
                                                                    }
                                                                     );
                                i++;
                            });

                            lstEscalonamiento.Add(escalona);
                            break;
                        }
                    }
                }

                return lstEscalonamiento.FirstOrDefault();
            }

        }

        public string ObtenerCodigoCasaMatriz()
        {
            string codigoCasaMatriz = "";

            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCodigoCasaMatriz_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                var resultado = cmd.ExecuteScalar();
                if (resultado != null)
                {
                    codigoCasaMatriz = resultado.ToString();
                }
                conn.Close();
            };

            return codigoCasaMatriz;
        }

        /// <summary>
        /// Obtiene los responsables por los que ha pasado una solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <param name="documentoResponsable"></param>
        public List<string> ObtenerResponsablesDeSolicitud(long idSolicitud, string documentoResponsable)
        {
            List<string> lstResponsables = new List<string>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paConsultaResponsablesDeSolicitud", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitud);
                cmd.Parameters.AddWithValue("@Documento", documentoResponsable);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string responsable = reader["DocumentoResponsable"].ToString();
                    lstResponsables.Add(responsable);
                }
                conn.Close();
            }
            return lstResponsables;
        }

        /// <summary>
        /// obtiene todos los escalonamientos asociados a un id de parametrizacion
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        public List<RAEscalonamientoDC> ObtenerEscalonamientoPorIdParametrizacion(int idParametrizacionRap)
        {
            List<RAEscalonamientoDC> lstEscalonamientos = new List<RAEscalonamientoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEscalamientoPorIdParametrizacion_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idparametrizacionRap", idParametrizacionRap);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAEscalonamientoDC escalonamiento = new RAEscalonamientoDC();
                    escalonamiento.idCargo = reader["idCargo"].ToString();
                    escalonamiento.IdProceso = reader["IdProceso"].ToString();
                    escalonamiento.IdProcedimiento = reader["IdProcedimiento"].ToString();
                    escalonamiento.IdSucursalEscalar = reader["CodigoSucursal"].ToString();
                    escalonamiento.CorreoEscalar = reader["CorreoCorporativo"].ToString();
                    escalonamiento.IdTipoHora = Convert.ToInt32(reader["IdTipoHora"]);
                    escalonamiento.HorasEscalar = Convert.ToInt32(reader["HorasEscalar"]);
                    escalonamiento.DocumentoEmpeladoEscalar = reader["cod_emp"].ToString();
                    lstEscalonamientos.Add(escalonamiento);
                }
                conn.Close();
            }
            return lstEscalonamientos;

        }

        public List<RACargoEscalonamientoDC> ObtenerEscalonamiento(long idParametrizacionRap, long idSolicitud)
        {
            List<RACargoEscalonamientoDC> lstEscalonamiento = new List<RACargoEscalonamientoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEscalonamientoPorIdParametrizacion_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;
                cmd.Parameters.AddWithValue("@IdParametrizacion", idParametrizacionRap);
                cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitud);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RACargoEscalonamientoDC item = new RACargoEscalonamientoDC
                        {
                            IdCargo = reader["IdCargo"].ToString(),
                            IdProcedimiento = reader["IdProcedimiento"].ToString(),
                            IdProceso = reader["IdProceso"].ToString(),
                            Orden = Convert.ToInt32(reader["Orden"])
                        };
                        lstEscalonamiento.Add(item);
                    }
                }
                conn.Close();
            }
            return lstEscalonamiento;
        }

        /// <summary>
        /// obtiene el correo segun el id de cargo
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string ObtenerCorreoSegunIdCargo(string idCargo)
        {
            string correo = "";
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCorreoSegunIdCargo_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCargo", idCargo);
                conn.Open();
                var email = cmd.ExecuteScalar();
                conn.Close();
                correo = email != null ? email.ToString() : "No Registra";
            }
            return correo;
        }

        /// <summary>
        /// obtiene el nombre de la calse namespace y assembly de kla regla a ejecutar segun el tipo de escalonamiento Modi
        /// </summary>
        /// <param name="eSCALONAMIENTO_RESPONSABLE_CENTRO_SERVICIO"></param>
        /// <returns></returns>
        public RAReglasIngrecionesManualDC ObtenerReglasIntegracionesTipoEscalonamiento(RAEnumTipoEscalonamiento tipoEscalonamiento)
        {
            RAReglasIngrecionesManualDC regla = new RAReglasIngrecionesManualDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerReglaIntegracionTipoEscalonamiento_Raps", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoEscalonamiento", (int)tipoEscalonamiento);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    regla.IdRegla = Convert.ToInt32(reader["RRE_IdRegla"]);
                    regla.NombreRegla = reader["RRE_DescripcionRegla"].ToString();
                    regla.IdTipoEscalonamiento = Convert.ToInt32(reader["RRE_IdTipoEscalonamiento"]);
                    regla.Assembly = reader["RRE_Assembly"].ToString();
                    regla.NameSpace = reader["RRE_NameSpace"].ToString();
                    regla.Clase = reader["RRE_Clase"].ToString();
                }
                conn.Close();

                return regla;
            }
        }

        /// <summary>
        /// Obtiene el responsable asignado para responder los raps de las fallas causadas por determinado centro de servicio Modi
        /// </summary>
        /// <param name="identificacionResponsableFalla"></param>
        /// <returns></returns>
        public RACargoEscalarDC ObtenerResponsableCentroServicioParaAsignarRaps(string identificacionResponsableFalla)
        {
            RACargoEscalarDC cargoEscalar = new RACargoEscalarDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerResponsableCentroserviocioParaAsignarRaps_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroservicio", identificacionResponsableFalla);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cargoEscalar.DocumentoEmpleado = reader["Cod_emp"].ToString();
                    cargoEscalar.Correo = reader["e_mail_alt"].ToString();
                    cargoEscalar.Sucursal = reader["cod_suc"].ToString();
                    cargoEscalar.CodPlantaNovasoft = reader["codPlantaNova"].ToString();
                    cargoEscalar.IdCiudad = reader["cod_ciu"].ToString();
                    cargoEscalar.NombreEmpleado = reader["NombreEmpleado"].ToString();
                    cargoEscalar.IdCargoController = reader["cod_car"].ToString();
                }
                conn.Close();
            }
            return cargoEscalar;
        }

        /// <summary>
        /// Obtiene el horario de un empleado por su numero de identificacion
        /// </summary>
        /// <param name="documentoEmpleado"></param>
        /// <returns></returns>
        public List<RAHorarioEmpleadoDC> ObtenerHorariosEmpleadoPorIdentificacion(string documentoEmpleado)
        {
            List<RAHorarioEmpleadoDC> lstHorarios = new List<RAHorarioEmpleadoDC>();

            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerHorarioEmpleadoNovasoft_Raps", conn);
                cmd.CommandTimeout = 600;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@documentoEmpl", documentoEmpleado);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    RAHorarioEmpleadoDC horario = new RAHorarioEmpleadoDC
                    {
                        IdDia = Convert.ToInt32(reader["diaSemana"]),
                        HoraEntrada = DateTime.Parse(reader["HoraEntrada"].ToString()).AddDays(i),
                        HoraSalida = DateTime.Parse(reader["HoraSalida"].ToString()).AddDays(i)
                    };
                    lstHorarios.Add(horario);
                    i++;
                }
                conn.Close();
            }
            return lstHorarios;
        }
        #endregion

        #region modificacion
        /// <summary>
        /// actualiza el estado de una solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <param name="idEstado"></param>
        public void ActualizarSolicitud(long idSolicitud, RAEnumEstados idEstado, string documentoResponsable)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paActualizarSolicitud_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                cmd.Parameters.AddWithValue("@idEstado", idEstado);
                cmd.Parameters.AddWithValue("@documentoResponsable", documentoResponsable);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Obtiene las personas asociadas a una sucursal y un grupo especifico
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public List<RAIdentificaEmpleadoDC> ObtenerEmpleadosPorGrupoYSucursal(int IdGrupo, int IdSucursal)
        {
            List<RAIdentificaEmpleadoDC> lstEmpleados = new List<RAIdentificaEmpleadoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEmpleadosPorGrupoYSucursal_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGrupo", IdGrupo);
                cmd.Parameters.AddWithValue("@CodSucursal", IdSucursal);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RAIdentificaEmpleadoDC empleado = new RAIdentificaEmpleadoDC
                        {
                            Nombre = reader["NombreEmpleado"].ToString(),
                            NumeroIdentificacion = reader["Identificacion"].ToString(),
                            IdCargo = reader["IdCargo"].ToString()
                        };
                        lstEmpleados.Add(empleado);
                    }
                }
                conn.Close();
            }
            return lstEmpleados;
        }

        /// <summary>
        /// Obtiene las fallas cometidas por un mensajero en el dia anterior al actual
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RAFallaMensajeroDC> ObtenerReporteFallasPorMensajero(string idMensajero)
        {
            List<RAFallaMensajeroDC> lstFallas = null;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerFallasPorMensajero_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    lstFallas = new List<RAFallaMensajeroDC>();
                    while (reader.Read())
                    {
                        RAFallaMensajeroDC falla = new RAFallaMensajeroDC
                        {
                            TipoFalla = reader["SOA_DescripcionSolicitud"].ToString(),
                            ValorDeParametro = reader["PSA_Valor"].ToString(),
                            FechaCrecionSolicitud = Convert.ToDateTime(reader["SOA_FechaCreacionSolicitud"]),
                            IdParametrizacionRaps = Convert.ToInt64(reader["SOA_IdParametrizacionRap"]),
                            NivelGravedad = string.IsNullOrEmpty(reader["NivelGravedad"].ToString()) ? 0 : Convert.ToInt32(reader["NivelGravedad"])
                        };
                        lstFallas.Add(falla);
                    }
                }
                conn.Close();
            }
            return lstFallas;
        }


        #endregion

        #region FallasMensajero

        public List<RASolucitudesAutomaticasDC> ObtenerTodasFallasPorMensajero(string idresponsable, string idestado)
        {
            List<RASolucitudesAutomaticasDC> lstFallas = null;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerListaSolicitudesFallasMensajeroVigenteV2_Raps", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;
                cmd.Parameters.AddWithValue("@IdResponsable", idresponsable);
                cmd.Parameters.AddWithValue("@IdEstado", idestado);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    lstFallas = new List<RASolucitudesAutomaticasDC>();
                    while (reader.Read())
                    {
                        RASolucitudesAutomaticasDC falla = new RASolucitudesAutomaticasDC
                        {
                            //IdSolicitud = Convert.ToInt64(reader["IdSolicitud"]),
                            Descripcion = reader["Descripcion"].ToString(),
                            NombreTipo = reader["NombreTipo"].ToString(),
                            Cuenta = Convert.ToInt32(reader["Cuenta"]),
                            ValorParametroAgrupamiento = reader["ValorParametroAgrupamiento"].ToString()
                        };
                        lstFallas.Add(falla);
                    }
                }
                conn.Close();
            }
            return lstFallas;
        }
        public RAEstadoActualSolicitudRapsDC ObtenerEstadoActualSolicitud(string idmensajero)
        {
            RAEstadoActualSolicitudRapsDC estadoactual = new RAEstadoActualSolicitudRapsDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEstadoActualSolicitudAutomatica", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;
                cmd.Parameters.AddWithValue("@IdMensajero", idmensajero);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    estadoactual = new RAEstadoActualSolicitudRapsDC()
                    {
                        IdMensajero = reader["Cedula"].ToString(),
                        NombreMensajero = reader["Nombre"].ToString(),
                        TelefonoMensajero = reader["Celular"].ToString(),
                        SucursalMensajero = reader["Sucursal"].ToString(),
                        FotoMensajero = reader["Foto"] != DBNull.Value ? Convert.ToBase64String((Byte[])reader["Foto"]) : ""
                    };
                }

                conn.Close();
            }
            return estadoactual;
        }



        public List<RAFallasMensajeroDC> ObtenerFallasMensajero(string idmensajero, string idresponsable, int estado)
        {
            List<RAFallasMensajeroDC> lstFallas = null;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerFallasMensajero", conn);
                cmd.CommandTimeout = 60;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensajero", idmensajero);
                cmd.Parameters.AddWithValue("@IdResponsable", idresponsable);
                cmd.Parameters.AddWithValue("@Estado", estado);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    lstFallas = new List<RAFallasMensajeroDC>();
                    while (reader.Read())
                    {
                        RAFallasMensajeroDC falla = new RAFallasMensajeroDC
                        {
                            IdParametrizacion = Convert.ToInt32(reader["SOA_IdParametrizacionRap"]),
                            Descripcion = reader["SOA_DescripcionSolicitud"].ToString(),
                            IdSolicitud = Convert.ToInt32(reader["SOA_IdSolicitudCreada"]),
                            Cuenta = Convert.ToInt32(reader["Cuenta"]),
                            NivelGravedad = Convert.ToInt32(reader["NivelGravedad"]),
                            FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]),
                            Anchor = Convert.ToBase64String((Byte[])reader["Anchor"])
                        };
                        lstFallas.Add(falla);
                    }
                }
                conn.Close();
            }
            return lstFallas;
        }





        /// <summary>
        /// Obtiene el detalle de la solicitud acumulativa a partir de la solicitud creada y la parametrizacion
        /// </summary>
        /// <param name="idsolicitud"></param>
        /// <param name="idParametrizacion"></param>
        /// <returns></returns>
        public List<RGDictionary> ObtenerDetalleSolicitudesAcumulativas(long idsolicitud, long idParametrizacion)
        {
            List<RGDictionary> lstDetalles = null;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDetalleSolicitudAcumulativaParametrizacion_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", idsolicitud);
                cmd.Parameters.AddWithValue("@IdParametrizacion", idParametrizacion);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    lstDetalles = new List<RGDictionary>();
                    while (reader.Read())
                    {
                        RGDictionary solicitud = new RGDictionary()
                        {
                            Name = reader["SOA_IdSolicitudAcumulativa"].ToString(),
                            Value = reader["Descripcion"].ToString()
                        };
                        lstDetalles.Add(solicitud);
                    }
                }
                conn.Close();
            }
            return lstDetalles;
        }

        public long ObtenerIdParametrizacionPadrePorIdHijo(int idParametrizacion)
        {
            long idPadre;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerIdParametrizacionPadre_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idParametrizacion", idParametrizacion);
                conn.Open();
                var result = cmd.ExecuteScalar();
                idPadre = result != null ? Convert.ToInt64(result.ToString()) : 0;
                conn.Close();
            }
            return idPadre;
        }

        /// <summary>
        /// Obtiene el codigo del proceso segun el id de la persona
        /// </summary>
        /// <param name="cargo"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public RAPersonaDC ObtenerProcesoPorPersona(string cargo, string idSucursal, string idProceso, string idProcedimiento)
        {
            RAPersonaDC datapersonaEscalona = new RAPersonaDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerProcesoSegunPersona_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("@CodCargo", cargo);
                cmd.Parameters.AddWithValue("@CodSucursal", idSucursal);
                cmd.Parameters.AddWithValue("@IdProceso", idProceso);
                cmd.Parameters.AddWithValue("@IdProcedimiento", idProcedimiento);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    datapersonaEscalona.Proceso = reader["Descripcion"].ToString();
                    datapersonaEscalona.NumeroDocumento = Convert.ToInt64(reader["cod_emp"]);
                    datapersonaEscalona.NombreCompleto = reader["NombreEmpleado"].ToString();
                    datapersonaEscalona.Telefono = reader["tel_cel"].ToString();
                }
            }
            return datapersonaEscalona;
        }

        /// <summary>
        /// Obtiene el cargo segun el nivel y el id de parametrizacion que se consulte
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public string ObtenerCargoSegunNivelYParametrizacion(long idParametrizacion, short nivel)
        {
            string idCargo = "";
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCargoNivelEscalonamiento_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("@Idparametrizacion", idParametrizacion);
                cmd.Parameters.AddWithValue("@orden", nivel);
                conn.Open();
                var result = cmd.ExecuteScalar();
                idCargo = result != null ? result.ToString() : "";
                conn.Close();
            }
            return idCargo;
        }

        #endregion


    }
}
