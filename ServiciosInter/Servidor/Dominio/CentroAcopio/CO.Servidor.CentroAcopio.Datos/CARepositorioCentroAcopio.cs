using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Data.SqlClient;
using System.Data;

using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Excepciones;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.ParametrosFW;
using CO.Servidor.CentroAcopio.Datos.Mapper;

namespace CO.Servidor.CentroAcopio.Datos
{
    public class CARepositorioCentroAcopio
    {
        private static readonly CARepositorioCentroAcopio instancia = new CARepositorioCentroAcopio();
        public static CARepositorioCentroAcopio Instancia
        {
            get { return CARepositorioCentroAcopio.instancia; }
        }

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringPosController = ConfigurationManager.ConnectionStrings["PorController"].ConnectionString;
        

        public List<CAAsignacionGuiaDC> ConsultarGuias_EnCenAco_ParaREO(long idCentroServicio)
        {
            List<CAAsignacionGuiaDC> ListaReclameEnOficina = new List<CAAsignacionGuiaDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarGuiasenCenAco_paraREO_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCentroServicio", idCentroServicio));
                SqlDataReader read = cmd.ExecuteReader();
                ListaReclameEnOficina = new List<CAAsignacionGuiaDC>();
                while (read.Read())
                {
                    CAAsignacionGuiaDC objAsignacion = new CAAsignacionGuiaDC();

                    objAsignacion.NumeroGuia = Convert.ToInt64(read["ADM_NumeroGuia"]);
                    objAsignacion.TipoEnvio = read["ADM_NombreTipoEnvio"].ToString();
                    objAsignacion.Peso = Convert.ToInt64(read["ADM_Peso"]);
                    objAsignacion.DiceContener = read["ADM_DiceContener"].ToString();

                    objAsignacion.IdCentroServicioDestino = Convert.ToInt64(read["ADM_IdCentroServicioDestino"]);
                    objAsignacion.NombreCentroServicioDestino = read["ADM_NombreCentroServicioDestino"].ToString();
                    objAsignacion.DireccionDestino = read["ADM_NombreCentroServicioDestino"].ToString();

                    if (read["FechaAsignacionAPro"] != DBNull.Value)
                    {
                        objAsignacion.FechaAsignacion = Convert.ToDateTime(read["FechaAsignacionAPro"]);
                        objAsignacion.EstaAsignada = true;
                    }
                    objAsignacion.LocalidadDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC
                    {
                        IdLocalidad = read["ADM_IdCiudadDestino"].ToString(),
                        Nombre = read["ADM_NombreCiudadDestino"].ToString()
                    };
                    objAsignacion.Estado = new Servicios.ContratoDatos.Admisiones.Mensajeria.ADTrazaGuia
                    {
                        IdEstadoGuia = (Convert.ToInt16(read["ADM_IdEstadoGuia"])),
                        DescripcionEstadoGuia = read["ADM_DescripcionEstado"].ToString(),
                        IdCiudad = read["ADM_IdLocalidadEstado"].ToString(),
                        Ciudad = read["ADM_NombreLocalidadEstado"].ToString(),
                        IdCentroServicioEstado = (Convert.ToInt64(read["ADM_IdCentroServicioEstado"])),
                        NombreCentroServicioEstado = read["ADM_NombreCentroServicioEstado"].ToString(),
                        FechaGrabacion = Convert.ToDateTime(read["ADM_FechaGrabacionEstado"]),
                    };
                    objAsignacion.Respuesta = OUEnumValidacionDescargue.Exitosa;

                    ListaReclameEnOficina.Add(objAsignacion);
                }
            }

            return ListaReclameEnOficina;
        }

        public List<CAAsignacionGuiaDC> ConsultarGuias_EnConfirmacionesyDev(long idCentroServicio, long idBodegaConfirDev)
        {
            List<CAAsignacionGuiaDC> ListaGuias = new List<CAAsignacionGuiaDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarGuiasEnConfirmacionesyDev_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCentroServicioCOL", idCentroServicio));
                cmd.Parameters.Add(new SqlParameter("@IdBodegaCenSer", idBodegaConfirDev));
                SqlDataReader read = cmd.ExecuteReader();
                ListaGuias = new List<CAAsignacionGuiaDC>();
                while (read.Read())
                {
                    CAAsignacionGuiaDC objAsignacion = new CAAsignacionGuiaDC();

                    objAsignacion.NumeroGuia = Convert.ToInt64(read["ADM_NumeroGuia"]);
                    objAsignacion.TipoEnvio = read["ADM_NombreTipoEnvio"].ToString();
                    objAsignacion.Peso = Convert.ToInt64(read["ADM_Peso"]);
                    objAsignacion.DiceContener = read["ADM_DiceContener"].ToString();
                    objAsignacion.IdCentroServicioDestino = Convert.ToInt64(read["ADM_IdCentroServicioDestino"]);
                    objAsignacion.DireccionDestino = read["ADM_NombreCentroServicioDestino"].ToString();

                    if (read["FechaAsignacion"] != DBNull.Value)
                    {
                        objAsignacion.FechaAsignacion = Convert.ToDateTime(read["FechaAsignacion"]);
                        objAsignacion.EstaAsignada = true;
                    }

                    objAsignacion.LocalidadDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC
                    {
                        IdLocalidad = read["ADM_IdCiudadDestino"].ToString(),
                        Nombre = read["ADM_NombreCiudadDestino"].ToString()
                    };

                    objAsignacion.Estado = new Servicios.ContratoDatos.Admisiones.Mensajeria.ADTrazaGuia
                    {
                        IdEstadoGuia = (Convert.ToInt16(read["ADM_IdEstadoGuia"])),
                        DescripcionEstadoGuia = read["ADM_DescripcionEstado"].ToString(),
                        IdCiudad = read["ADM_IdLocalidadEstado"].ToString(),
                        Ciudad = read["ADM_NombreLocalidadEstado"].ToString(),
                        IdCentroServicioEstado = (Convert.ToInt64(read["ADM_IdCentroServicioEstado"])),
                        NombreCentroServicioEstado = read["ADM_NombreCentroServicioEstado"].ToString(),
                        FechaGrabacion = Convert.ToDateTime(read["ADM_FechaGrabacionEstado"]),
                    };

                    objAsignacion.Respuesta = OUEnumValidacionDescargue.Exitosa;

                    ListaGuias.Add(objAsignacion);
                }
            }

            return ListaGuias;
        }

        public List<TipoConsolidado> ObtenerTipoConsolidado()
        {
            List<TipoConsolidado> listaTipoConsolidado = new List<TipoConsolidado>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarTipoConsolidado_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    var tipoConsolidado = new TipoConsolidado
                    {
                        IdTipoConsolidado = Convert.ToInt32(read["TIC_IdTipoConsolidado"]),
                        Descripcion = Convert.ToString(read["TIC_Descripcion"]),
                        CreadoPor = Convert.ToString(read["TIC_CreadoPor"]),
                        FechaGrabacion = Convert.ToDateTime(read["TIC_FechaGrabacion"])
                    };

                    listaTipoConsolidado.Add(tipoConsolidado);
                }
            }

            return listaTipoConsolidado;
        }

        public MovimientoConsolidado MovimientoConsolidadoVigente(string numeroConsolidado, CACEnumTipoConsolidado tipoConsolidado)
        {
            MovimientoConsolidado movimientoConsolidado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarMovimientoConsolidadoVigente_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroConsolidado", numeroConsolidado));
                cmd.Parameters.Add(new SqlParameter("@TipoConsolidado", tipoConsolidado.GetHashCode()));
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    movimientoConsolidado = new MovimientoConsolidado();
                    movimientoConsolidado.IdMovimiento = read["MCO_IdMovimiento"] is DBNull ? 0 : Convert.ToInt32(read["MCO_IdMovimiento"]);
                    movimientoConsolidado.NumeroPrecinto = read["MCO_NumeroPrecinto"] is DBNull ? 0 : Convert.ToInt64(read["MCO_NumeroPrecinto"]);
                    movimientoConsolidado.IdCentroServicioDestino = Convert.ToInt64(read["CON_IdCentroServicioDestino"]);
                    movimientoConsolidado.IdTipoMovimiento = read["MCO_IdTipoMovimiento"] is DBNull ? 0 : Convert.ToInt32(read["MCO_IdTipoMovimiento"]);
                    movimientoConsolidado.FechaMovimiento = read["MCO_FechaMovimiento"] is DBNull ? string.Empty : Convert.ToString(read["MCO_FechaMovimiento"]);
                    movimientoConsolidado.CreadoPor = read["MCO_CreadoPor"] is DBNull ? string.Empty : Convert.ToString(read["MCO_CreadoPor"]);
                    movimientoConsolidado.NumeroConsolidado = Convert.ToString(read["CON_NumeroConsolidado"]);
                    movimientoConsolidado.IdTipoConsolidado = Convert.ToInt32(read["CON_IdTipoConsolidado"]);
                    movimientoConsolidado.IdMovimientoLog = read["CON_IdMovimientoLog"] is DBNull ? 0 : Convert.ToInt32(read["CON_IdMovimientoLog"]);
                    movimientoConsolidado.IdLocalidadOrigen = Convert.ToString(read["CES_IdMunicipioOrigen"]);
                    movimientoConsolidado.IdLocalidadDestino = Convert.ToString(read["CES_IdMunicipioDestino"]);
                    movimientoConsolidado.IdLocalidadMovimiento = Convert.ToString(read["IdLocalidadMovimientoDestino"]);
                }
            }

            return movimientoConsolidado;
        }

        public int InsertarMovimientoConsolidado(MovimientoConsolidado movimientoConsolidado)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarMovimientoConsolidado_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@MCO_NumeroPrecinto", movimientoConsolidado.NumeroPrecinto));
                cmd.Parameters.Add(new SqlParameter("@MCO_IdCentroServicioDestino", movimientoConsolidado.IdCentroServicioDestino));
                cmd.Parameters.Add(new SqlParameter("@MCO_IdTipoMovimiento", movimientoConsolidado.IdTipoMovimiento));
                cmd.Parameters.Add(new SqlParameter("@MCO_FechaMovimiento", movimientoConsolidado.FechaMovimiento));
                cmd.Parameters.Add(new SqlParameter("@MCO_CreadoPor", movimientoConsolidado.CreadoPor));
                cmd.Parameters.Add(new SqlParameter("@MCO_NumeroConsolidado", movimientoConsolidado.NumeroConsolidado));
                cmd.Parameters.Add(new SqlParameter("@MCO_IdTipoConsolidado", movimientoConsolidado.IdTipoConsolidado));
                SqlDataReader read = cmd.ExecuteReader();

                if (read.Read())
                {
                    movimientoConsolidado.Novedad.IdMovimiento = Convert.ToInt32(read["IdMovimiento"]);
                }

                return movimientoConsolidado.Novedad.IdMovimiento;
            }
        }

        public void InsertarNovedadConsolidado(NovedadConsolidado novedadConsolidado, string numeroConsolidado, long numeroPrecinto)
        {
            string rutaImagenes;
            string carpetaDestino;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarNovedadConsolidado_OPN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Descripcion", novedadConsolidado.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@FechaGrabacion", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", novedadConsolidado.CreadoPor));
                cmd.Parameters.Add(new SqlParameter("@IdTipoNovedad", novedadConsolidado.IdTipoNovedad));
                cmd.Parameters.Add(new SqlParameter("@IdMovimiento", novedadConsolidado.IdMovimiento));
                cmd.Parameters.Add(new SqlParameter("@IdConsolidado", novedadConsolidado.IdConsolidado));
                cmd.ExecuteNonQuery();

                int file = 0;
                rutaImagenes = PAParametros.Instancia.ConsultarParametrosFramework("FoldImgCAConsolidado");
                carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }

                foreach (var item in novedadConsolidado.AdjuntosConsolidado)
                {
                    file++;
                    byte[] bytebuffer = Convert.FromBase64String(item.Imagen);
                    MemoryStream memoryStream = new MemoryStream(bytebuffer);
                    var image = Image.FromStream(memoryStream);
                    ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
                    item.Ruta = carpetaDestino + "\\" + numeroConsolidado + "-" + numeroPrecinto + "-" + file + ".jpg";
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    image.Save(item.Ruta, jpgEncoder, myEncoderParameters);

                    SqlCommand cmd1 = new SqlCommand("paInsertarAdjuntosConsolidado_OPN", sqlConn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add(new SqlParameter("@Ruta", item.Ruta));
                    cmd1.Parameters.Add(new SqlParameter("@IdMovimiento", novedadConsolidado.IdMovimiento));
                    cmd1.Parameters.Add(new SqlParameter("@FechaGrabacion", DateTime.Now));
                    cmd1.ExecuteNonQuery();
                }
            }
        }

        public List<CAAsignacionGuiaDC> ConsultarGuias_EnCustodia(long idCentroServicio, long idBodegaCustodia)
        {
            List<CAAsignacionGuiaDC> ListaGuias = new List<CAAsignacionGuiaDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarGuiasEnCustodia_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCentroServicioCOL", idCentroServicio));
                cmd.Parameters.Add(new SqlParameter("@IdBodegaCenSer", idBodegaCustodia));
                SqlDataReader read = cmd.ExecuteReader();
                ListaGuias = new List<CAAsignacionGuiaDC>();
                while (read.Read())
                {
                    CAAsignacionGuiaDC objAsignacion = new CAAsignacionGuiaDC();

                    objAsignacion.NumeroGuia = Convert.ToInt64(read["ADM_NumeroGuia"]);
                    objAsignacion.TipoEnvio = read["ADM_NombreTipoEnvio"].ToString();
                    objAsignacion.Peso = Convert.ToInt64(read["ADM_Peso"]);
                    objAsignacion.DiceContener = read["ADM_DiceContener"].ToString();
                    objAsignacion.IdCentroServicioDestino = Convert.ToInt64(read["ADM_IdCentroServicioDestino"]);
                    objAsignacion.DireccionDestino = read["ADM_NombreCentroServicioDestino"].ToString();

                    if (read["FechaAsignacion"] != DBNull.Value)
                    {
                        objAsignacion.FechaAsignacion = Convert.ToDateTime(read["FechaAsignacion"]);
                        objAsignacion.EstaAsignada = true;
                    }

                    objAsignacion.LocalidadDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC
                    {
                        IdLocalidad = read["ADM_IdCiudadDestino"].ToString(),
                        Nombre = read["ADM_NombreCiudadDestino"].ToString()
                    };

                    objAsignacion.Estado = new Servicios.ContratoDatos.Admisiones.Mensajeria.ADTrazaGuia
                    {
                        IdEstadoGuia = (Convert.ToInt16(read["ADM_IdEstadoGuia"])),
                        DescripcionEstadoGuia = read["ADM_DescripcionEstado"].ToString(),
                        IdCiudad = read["ADM_IdLocalidadEstado"].ToString(),
                        Ciudad = read["ADM_NombreLocalidadEstado"].ToString(),
                        IdCentroServicioEstado = (Convert.ToInt64(read["ADM_IdCentroServicioEstado"])),
                        NombreCentroServicioEstado = read["ADM_NombreCentroServicioEstado"].ToString(),
                        FechaGrabacion = Convert.ToDateTime(read["ADM_FechaGrabacionEstado"]),
                    };


                    objAsignacion.Respuesta = OUEnumValidacionDescargue.Exitosa;

                    ListaGuias.Add(objAsignacion);
                }
            }

            return ListaGuias;
        }


        public List<CAManifiestoREO> ConsultarManifiestosREO(DateTime Fecha, long IdManifiesto)
        {
            List<CAManifiestoREO> newlist = new List<CAManifiestoREO>();
            CAManifiestoREO objMan;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarManifiestosREO_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@FECHA", Fecha));
                cmd.Parameters.Add(new SqlParameter("@IdManifiesto", IdManifiesto));
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objMan = new CAManifiestoREO();
                    objMan.IdManifiestoREO = Convert.ToInt64(reader["IdManifiesto"]);
                    objMan.CantidadGuias = Convert.ToInt32(reader["CntGuias"]);
                    objMan.IdCenSerManifiesta = Convert.ToInt64(reader["IdCSManifiesta"]);
                    objMan.NombreCenSerManifiesta = reader["NombreCSManifiesta"].ToString();
                    objMan.IdCenSerDestino = Convert.ToInt64(reader["IdCSDestino"]);
                    objMan.NombreCenSerDestino = reader["NombreCSDestino"].ToString();
                    objMan.IdVehiculo = Convert.ToInt32(reader["VEH_IdVehiculo"]);
                    objMan.PlacaVehiculo = reader["PlacaVehiculo"].ToString();
                    objMan.IdMensajero = Convert.ToInt64(reader["VEH_IdVehiculo"]);
                    objMan.CedulaMensajero = reader["CedulaMensajero"].ToString();
                    objMan.NombreMensajero = reader["NombreMensajero"].ToString();
                    objMan.FechaCreacion = Convert.ToDateTime(reader["FechaGrabacion"]);

                    newlist.Add(objMan);
                }
            }

            return newlist;
        }

        public List<CAAsignacionGuiaDC> ConsultarManifiestoREO_Guias(long IdManifiesto)
        {
            List<CAAsignacionGuiaDC> newlist = new List<CAAsignacionGuiaDC>();
            CAAsignacionGuiaDC objManGuia;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarManifiestosREOGuias_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdManifiesto", IdManifiesto));
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objManGuia = new CAAsignacionGuiaDC();
                    objManGuia.NumeroGuia = Convert.ToInt64(reader["ADM_NumeroGuia"]);
                    objManGuia.IdAdmision = Convert.ToInt64(reader["ADM_IdAdminisionMensajeria"]);
                    objManGuia.TipoEnvio = reader["ADM_NombreTipoEnvio"].ToString();
                    objManGuia.Peso = Convert.ToInt64(reader["ADM_Peso"]);
                    objManGuia.DiceContener = reader["ADM_DiceContener"].ToString();

                    objManGuia.IdCentroServicioDestino = Convert.ToInt64(reader["ADM_IdCentroServicioDestino"]);
                    objManGuia.NombreCentroServicioDestino = reader["ADM_NombreCentroServicioDestino"].ToString();
                    objManGuia.DireccionDestino = reader["ADM_NombreCentroServicioDestino"].ToString();

                    if (reader["FechaAsignacionAPro"] != DBNull.Value)
                    {
                        objManGuia.FechaAsignacion = Convert.ToDateTime(reader["FechaAsignacionAPro"]);
                        objManGuia.EstaAsignada = true;
                    }
                    objManGuia.LocalidadDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC
                    {
                        IdLocalidad = reader["ADM_IdCiudadDestino"].ToString(),
                        Nombre = reader["ADM_NombreCiudadDestino"].ToString()
                    };
                    objManGuia.Estado = new Servicios.ContratoDatos.Admisiones.Mensajeria.ADTrazaGuia
                    {
                        IdEstadoGuia = (Convert.ToInt16(reader["ADM_IdEstadoGuia"])),
                        DescripcionEstadoGuia = reader["ADM_DescripcionEstado"].ToString(),
                        IdCiudad = reader["ADM_IdLocalidadEstado"].ToString(),
                        Ciudad = reader["ADM_NombreLocalidadEstado"].ToString(),
                        IdCentroServicioEstado = (Convert.ToInt64(reader["ADM_IdCentroServicioEstado"])),
                        NombreCentroServicioEstado = reader["ADM_NombreCentroServicioEstado"].ToString(),
                        FechaGrabacion = Convert.ToDateTime(reader["ADM_FechaGrabacionEstado"]),
                    };

                    newlist.Add(objManGuia);
                }
            }

            return newlist;
        }

        //Crea el manifiesto y devuelve el Id del Manifiesto
        public long CrearManifiesto_REO(long IdCSManif, long IdCSDesti, long IdVehiculo, long IdMensajero, string CreadoPor)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarManifiesto_REO", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCSManif", IdCSManif));
                cmd.Parameters.Add(new SqlParameter("@IdCSDesti", IdCSDesti));
                cmd.Parameters.Add(new SqlParameter("@IdVehiculo", IdVehiculo));
                cmd.Parameters.Add(new SqlParameter("@IdMensajero", IdMensajero));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", CreadoPor));
                long num = Convert.ToInt64(cmd.ExecuteScalar());
                sqlConn.Close();

                return num;
            }

        }

        //Crea el detalle del manifiesto
        public void CreaDetalleManifiesto_REO(long IdManifiesto, long IdCSOrigen, long IdCSDestino)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarDetalleManifiesto_REO", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdManifiesto", IdManifiesto));
                cmd.Parameters.Add(new SqlParameter("@IdCSOrigen", IdCSOrigen));
                cmd.Parameters.Add(new SqlParameter("@IdCSDestino", IdCSDestino));
                cmd.ExecuteNonQuery();
            }
        }


        //Modifica el Tipo de Entrega de la Guia a Reclame en Oficina
        public void CambiarTipoEntrega_REO(long NumeroGuia, long IdCSDestino)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paCambiaTipoEntrega_REO", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdCSDestino", IdCSDestino));
                cmd.ExecuteNonQuery();
            }
        }

        //Obtiene el id Asignacion Movimiento Inventario retorna falso si es nu
        public bool ObtieneIdAsignacionMovInventario(long numeroGuia, long idCentroServicio)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtieneIdAsignacionMovInventario", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@idCentroServicio", idCentroServicio));
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return true;
                }
            }
            return false;

        }

        #region ENVIOS NN
        /// <summary>
        /// Inserta los Envíos NN  
        /// </summary>
        /// <param name="envioNN"></param>
        public void InsertarEnvioNN(ADEnvioNN envioNN)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {


                SqlCommand cmd = new SqlCommand("paInsertarAdmisionMensajeriaNN_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@AMN_NumeroMensajeriaNN", envioNN.NumeroMensajeriaNN));
                cmd.Parameters.Add(new SqlParameter("@AMN_IdCiudadCaptura", envioNN.IdCiudadCaptura));
                cmd.Parameters.Add(new SqlParameter("@AMN_NombreCiudadCaptura", envioNN.CiudadCaptura));
                cmd.Parameters.Add(new SqlParameter("@AMN_IdCentroServicioOrigenCaptura", envioNN.IdCentroServicioCaptura));
                cmd.Parameters.Add(new SqlParameter("@AMN_NombreCentroServicioCaptura", envioNN.CentroServicioCaptura));
                cmd.Parameters.Add(new SqlParameter("@AMN_IdCiudadOrigen", envioNN.EnvioNN.IdCiudadOrigen));
                cmd.Parameters.Add(new SqlParameter("@AMN_NombreCiudadOrigen", envioNN.EnvioNN.NombreCiudadOrigen));
                cmd.Parameters.Add(new SqlParameter("@AMN_IdCiudadDestino", envioNN.EnvioNN.IdCiudadDestino));
                cmd.Parameters.Add(new SqlParameter("@AMN_NombreCiudadDestino", envioNN.EnvioNN.NombreCiudadDestino));
                cmd.Parameters.Add(new SqlParameter("@AMN_FechaCaptura", envioNN.FechaCaptura));
                cmd.Parameters.Add(new SqlParameter("@AMN_IdTipoIdentificacionRemitente", envioNN.EnvioNN.Remitente.TipoId));
                cmd.Parameters.Add(new SqlParameter("@AMN_IdRemitente", envioNN.EnvioNN.Remitente.Identificacion));
                cmd.Parameters.Add(new SqlParameter("@AMN_TelefonoRemitente", envioNN.EnvioNN.Remitente.Telefono));
                cmd.Parameters.Add(new SqlParameter("@AMN_NombreRemitente", envioNN.EnvioNN.Remitente.Nombre));
                cmd.Parameters.Add(new SqlParameter("@AMN_PrimerApellidoRemitente", envioNN.EnvioNN.Remitente.Apellido1));
                cmd.Parameters.Add(new SqlParameter("@AMN_SegundoApellidoRemitente", envioNN.EnvioNN.Remitente.Apellido2));
                cmd.Parameters.Add(new SqlParameter("@AMN_DireccionRemitente", envioNN.EnvioNN.Remitente.Direccion));
                cmd.Parameters.Add(new SqlParameter("@AMN_EmailRemitente", envioNN.EnvioNN.Remitente.Email));
                cmd.Parameters.Add(new SqlParameter("@AMN_IdTipoIdentificacionDestinatario", envioNN.EnvioNN.Destinatario.TipoId));
                cmd.Parameters.Add(new SqlParameter("@AMN_IdDestinatario", envioNN.EnvioNN.Destinatario.Identificacion));
                cmd.Parameters.Add(new SqlParameter("@AMN_TelefonoDestinatario", envioNN.EnvioNN.Destinatario.Telefono));
                cmd.Parameters.Add(new SqlParameter("@AMN_NombreDestinatario", envioNN.EnvioNN.Destinatario.Nombre));
                cmd.Parameters.Add(new SqlParameter("@AMN_PrimerApellidoDestinatario", envioNN.EnvioNN.Destinatario.Apellido1));
                cmd.Parameters.Add(new SqlParameter("@AMN_SegundoApellidoDestinatario", envioNN.EnvioNN.Destinatario.Apellido2));
                cmd.Parameters.Add(new SqlParameter("@AMN_DireccionDestinatario", envioNN.EnvioNN.Destinatario.Direccion));
                cmd.Parameters.Add(new SqlParameter("@AMN_EmailDestinatario", envioNN.EnvioNN.Destinatario.Email));
                cmd.Parameters.Add(new SqlParameter("@AMN_TotalPiezas", envioNN.EnvioNN.TotalPiezas));
                cmd.Parameters.Add(new SqlParameter("@AMN_Peso", envioNN.EnvioNN.Peso));
                cmd.Parameters.Add(new SqlParameter("@AMN_IdTipoEnvio", envioNN.EnvioNN.IdTipoEnvio));
                cmd.Parameters.Add(new SqlParameter("@AMN_NumeroBolsaSeguridad", envioNN.EnvioNN.NumeroBolsaSeguridad));
                cmd.Parameters.Add(new SqlParameter("@AMN_DiceContener", envioNN.EnvioNN.DiceContener));
               // cmd.Parameters.Add(new SqlParameter("@AMN_Observaciones", envioNN.EnvioNN.Observaciones));
                cmd.Parameters.Add(new SqlParameter("@AMN_FechaGrabacion", envioNN.EnvioNN.FechaGrabacion));
                cmd.Parameters.Add(new SqlParameter("@AMN_CreadoPor", envioNN.EnvioNN.CreadoPor));
                cmd.Parameters.Add(new SqlParameter("@AMN_Largo", envioNN.EnvioNN.Largo));
                cmd.Parameters.Add(new SqlParameter("@AMN_Ancho", envioNN.EnvioNN.Ancho));
                cmd.Parameters.Add(new SqlParameter("@AMN_Alto", envioNN.EnvioNN.Alto));
                cmd.Parameters.Add(new SqlParameter("@ADM_idOperativoOrigen", envioNN.IdOperativoOrigen));
                cmd.Parameters.Add(new SqlParameter("@ADM_OperativoOrigen", envioNN.OperativoOrigen));
                cmd.Parameters.Add(new SqlParameter("@ADM_idEstado", envioNN.IdEstado));
                cmd.Parameters.Add(new SqlParameter("@AMN_DescripcionEmpaque", envioNN.DescripcionEmpaque));
                cmd.Parameters.Add(new SqlParameter("@AMN_IdClasificacion", envioNN.Clasificacion.IdClasificacion));
                sqlConn.Open();
                long idMensajeria = Convert.ToInt64(cmd.ExecuteScalar());
                sqlConn.Close();

            }
        }
        /// <summary>
        /// Inserta la ruta de la imagen de una admision NN
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="idAdmisionNN"></param>
        public void InsertarImagenAdmisionNN(string ruta, long NumeroMensajeriaNN)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarRutaImagenEnvioNN_CAC", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AAN_NumeroMensajeriaNN", NumeroMensajeriaNN);
                cmd.Parameters.AddWithValue("@AAN_Archivo", ruta);
                cmd.Parameters.AddWithValue("@AAN_CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public List<ADEnvioNN> ObtieneEnvioNN(AdEnvioNNFiltro envioNNFiltro)
        {
            DataTable dtEnviosNN = new DataTable();
            SqlDataAdapter da;


            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAdmisionMensajeriaNNxFiltro_CAC", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@indicador", envioNNFiltro.Indicador);
                cmd.Parameters.AddWithValue("@NumeroMesnajeriaNN", envioNNFiltro.NumeroMensajeriaNN);
                cmd.Parameters.AddWithValue("@FechaIncial", envioNNFiltro.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFinal", envioNNFiltro.FechaFin);
                cmd.Parameters.AddWithValue("@idCiudadOrigen", envioNNFiltro.CiudadOrigen);
                cmd.Parameters.AddWithValue("@idCiudadDestino", envioNNFiltro.CiudadDestino);
                cmd.Parameters.AddWithValue("@nombreRemitente", envioNNFiltro.NombreRemitente == string.Empty ? null : envioNNFiltro.NombreRemitente);
                cmd.Parameters.AddWithValue("@nombreDestinatario", envioNNFiltro.NombreDestinatario == string.Empty ? null : envioNNFiltro.NombreDestinatario);
                cmd.Parameters.AddWithValue("@AMN_IdRemitente", envioNNFiltro.IdentificacionRemitente == string.Empty ? null : envioNNFiltro.IdentificacionRemitente);
                cmd.Parameters.AddWithValue("@AMN_IdDestinatario", envioNNFiltro.IdentificacionDestinatario == string.Empty ? null : envioNNFiltro.IdentificacionDestinatario);
                cmd.Parameters.AddWithValue("@AMN_DiceContener", envioNNFiltro.DiceContener ==string.Empty ? null : envioNNFiltro.DiceContener);
                cmd.Parameters.AddWithValue("@AMN_TelefonoDestinatario", envioNNFiltro.TelefonoDestinatario == string.Empty ? null : envioNNFiltro.TelefonoDestinatario);
                cmd.Parameters.AddWithValue("@AMN_EmailRemitente", envioNNFiltro.CorreoRemitente == string.Empty ? null : envioNNFiltro.CorreoRemitente);
                cmd.Parameters.AddWithValue("@AMN_NumeroBolsaSeguridad", envioNNFiltro.NumeroBolsaSeguridad == string.Empty ? null : envioNNFiltro.NumeroBolsaSeguridad);

                cmd.Parameters.AddWithValue("@AMN_TelefonoRemitente", envioNNFiltro.TelefonoRemitente == string.Empty ? null : envioNNFiltro.TelefonoRemitente);
                cmd.Parameters.AddWithValue("@AMN_DireccionDestinatario", envioNNFiltro.DireccionDestinatario == string.Empty ? null : envioNNFiltro.DireccionDestinatario);
                cmd.Parameters.AddWithValue("@AMN_DireccionRemitente", envioNNFiltro.DireccionRemitente == string.Empty ? null : envioNNFiltro.DireccionRemitente);
                cmd.Parameters.AddWithValue("@AMN_EmailDestinatario", envioNNFiltro.CorreoDestinatario == string.Empty ? null : envioNNFiltro.CorreoDestinatario);
                long idCentroServicio = 0;
                if (envioNNFiltro.CentroServicio != null)
                {
                    idCentroServicio = envioNNFiltro.CentroServicio.IdCentroServicio;
                
                }
                cmd.Parameters.AddWithValue("@idCSCapturado", idCentroServicio);

                cmd.Parameters.AddWithValue("@AMN_IdOperativo", envioNNFiltro.IdOperativo);
                cmd.Parameters.AddWithValue("@AMN_CantidadPiezas", envioNNFiltro.CantidadPiezas);
                cmd.Parameters.AddWithValue("@AMN_DescripcionEmpaque", envioNNFiltro.DescripcionEmpaque == string.Empty ? null : envioNNFiltro.DescripcionEmpaque);
                int idClasificacion = 0;
                if (envioNNFiltro.Clasificacion != null)
                {
                    idClasificacion = envioNNFiltro.Clasificacion.IdClasificacion;

                }
                cmd.Parameters.AddWithValue("@AMN_IdClasificacion", idClasificacion);



                da = new SqlDataAdapter(cmd);
                da.Fill(dtEnviosNN);
            }

            List<ADEnvioNN> listaEnviosNN = new List<ADEnvioNN>();

            //RURutaCWebDetalleCentrosServicios RutaDetalle;

            if (envioNNFiltro.Indicador == 0)
            {
                listaEnviosNN = dtEnviosNN.AsEnumerable().Select(r => new ADEnvioNN()
                {
                    NumeroMensajeriaNN = r.Field<long>("AMN_NumeroMensajeriaNN"),
                    IdEstado = r.Field<int>("AMN_idEstado"),
                    Estado = Convert.ToBoolean(r.Field<int>("AMN_idEstado")),
                    EnvioNN = new ADGuia
                    {
                        NumeroGuia = r.Field<long>("REG_IdAdmisionMensajeria")
                    }
                }).ToList();

            }
            else
            {
                listaEnviosNN = dtEnviosNN.AsEnumerable().Select(r => new ADEnvioNN()
                {
                    NumeroMensajeriaNN = r.Field<long>("AMN_NumeroMensajeriaNN"),
                    IdCiudadCaptura = r.Field<string>("AMN_IdCiudadCaptura"),
                    CiudadCaptura = r.Field<string>("AMN_NombreCiudadCaptura"),
                    IdCentroServicioCaptura = r.Field<Int32>("AMN_IdCentroServicioOrigenCaptura"),
                    CentroServicioCaptura = r.Field<string>("AMN_NombreCentroServicioCaptura"),
                    FechaCaptura = r.Field<DateTime>("AMN_FechaCaptura"),
                    EnvioNN = new ADGuia
                    {
                        IdCiudadOrigen = r.Field<string>("AMN_IdCiudadOrigen"),
                        NombreCiudadOrigen = r.Field<string>("AMN_NombreCiudadOrigen"),
                        IdCiudadDestino = r.Field<string>("AMN_IdCiudadDestino"),
                        NombreCiudadDestino = r.Field<string>("AMN_NombreCiudadDestino"),


                        TotalPiezas = r.Field<short>("AMN_TotalPiezas"),
                        Peso = r.Field<decimal>("AMN_Peso"),
                        IdTipoEnvio = r.Field<short>("AMN_IdTipoEnvio"),
                        NumeroBolsaSeguridad = r.Field<string>("AMN_NumeroBolsaSeguridad"),
                        DiceContener = r.Field<string>("AMN_DiceContener"),
                       // Observaciones = r.Field<string>("AMN_Observaciones"),
                        FechaGrabacion = r.Field<DateTime>("AMN_FechaGrabacion"),
                        CreadoPor = r.Field<string>("AMN_CreadoPor"),
                        Largo = r.Field<decimal>("AMN_Largo"),
                        Ancho = r.Field<decimal>("AMN_Ancho"),
                        Alto = r.Field<decimal>("AMN_Alto"),
                        
                        Remitente = new CLClienteContadoDC
                        {
                            TipoId = r.Field<string>("AMN_IdTipoIdentificacionRemitente"),
                            Identificacion = r.Field<string>("AMN_IdRemitente"),
                            Telefono = r.Field<string>("AMN_TelefonoRemitente"),
                            Nombre = r.Field<string>("AMN_NombreRemitente"),
                            Apellido1 = r.Field<string>("AMN_PrimerApellidoRemitente"),
                            Apellido2 = r.Field<string>("AMN_SegundoApellidoRemitente"),
                            Direccion = r.Field<string>("AMN_DireccionRemitente"),
                            Email = r.Field<string>("AMN_EmailRemitente"),
                        },

                        Destinatario = new CLClienteContadoDC
                        {

                            TipoId = r.Field<string>("AMN_IdTipoIdentificacionDestinatario"),
                            Identificacion = r.Field<string>("AMN_IdDestinatario"),
                            Telefono = r.Field<string>("AMN_TelefonoDestinatario"),
                            Nombre = r.Field<string>("AMN_NombreDestinatario"),
                            Apellido1 = r.Field<string>("AMN_PrimerApellidoDestinatario"),
                            Apellido2 = r.Field<string>("AMN_SegundoApellidoDestinatario"),
                            Direccion = r.Field<string>("AMN_DireccionDestinatario"),
                            Email = r.Field<string>("AMN_EmailDestinatario"),
                        },



                    },
                    DescripcionEmpaque=r.Field<string>("AMN_DescripcionEmpaque"),
                    IdOperativoOrigen = r.Field<Int32>("AMN_idOperativoOrigen"),
                    OperativoOrigen = r.Field<string>("AMN_OperativoOrigen"),
                    Clasificacion=new ClasificacionEnvioNN {
                        IdClasificacion = r.Field<int>("CAM_IdClasificacion"),
                        Clasificacion=r.Field<string>("CAM_NombreClasificacion")
                    }
                    

                }).ToList();

            }

            return listaEnviosNN;

        }


        /// <summary>
        /// Asigna un número de Guía a un envio NN
        /// </summary>
        /// <param name="numeroEnvioNN"></param>
        /// <param name="idGuia"></param>
        /// <param name="creadoPor"></param>

        public bool AsignacionGuiaAEnvioNN(long numeroEnvioNN, long idGuia, string creadoPor)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarRelacionEnvioNNGuia_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@REG_NumeroMensajeriaNN", numeroEnvioNN);
                cmd.Parameters.AddWithValue("@REG_idGruia", idGuia);
                cmd.Parameters.AddWithValue("@REG_CreadoPor", creadoPor);
              
                cmd.ExecuteNonQuery();
                sqlConn.Close();

                return true;
            }
        }

        /// <summary>
        /// obtiene la ruta de las imagenes asociadas al envio NN
        /// </summary>
        /// <param name="numeroEnvioNN"></param>
        /// <returns></returns>
        public List<RutasImagenesEnvioNN> ObtieneRutaImagenesEnvioNN(long numeroEnvioNN)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasImagenEnvioNN_CAC", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroMesnajeriaNN", numeroEnvioNN);

                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            List<RutasImagenesEnvioNN> listaImagenesEnvioNN = new List<RutasImagenesEnvioNN>();

            listaImagenesEnvioNN = dt.AsEnumerable().Select(r => new RutasImagenesEnvioNN()
            {
                IdArchivo = r.Field<long>("AAN_IdArchivo"),
                RutaArchivo = r.Field<string>("AAN_Archivo")

            }).ToList();


            return listaImagenesEnvioNN;
        }

        public List<ClasificacionEnvioNN> ObtieneClasificacionEnvioNN()
        {
            List<ClasificacionEnvioNN> clasificacionEnvioNN = new List<ClasificacionEnvioNN>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarClasificacionAdmisionNN_CAC", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                sqlConn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    clasificacionEnvioNN=CARepositorioMapper.ToListClasificacionEnvioNN(resultado);
                }
                return clasificacionEnvioNN;
            }
        }

        #endregion

        #region Bodegas

        /// <summary>
        /// Obtiene los reenvíos que se hacen de LOI a Centro de Acopio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="idCentroServicioOrigen"></param>
        /// <returns>Retorna Lista de los reenvíos</returns>
        public List<CAAsignacionGuiaDC> ObtenerReenviosBodegas_CAC(long idCentroServicioOrigen, ADEnumEstadoGuia idEstado)
        {
            List<CAAsignacionGuiaDC> ListaGuias = new List<CAAsignacionGuiaDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerReenviosBodegas_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCServicio", ControllerContext.Current.IdCentroServicio));
                cmd.Parameters.Add(new SqlParameter("@IdCServicioOrigen", idCentroServicioOrigen));
                cmd.Parameters.Add(new SqlParameter("@IdEstado", (Int16)idEstado));
                SqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    CAAsignacionGuiaDC objAsignacion = new CAAsignacionGuiaDC();

                    objAsignacion.NumeroGuia = Convert.ToInt64(read["NumeroGuia"]);
                    objAsignacion.DireccionDestino = read["DireccionDestinatario"].ToString();
                    objAsignacion.CiudadDestino = read["CiudadDestino"].ToString();
                    objAsignacion.Peso = Convert.ToDecimal(read["ADM_Peso"]);
                    objAsignacion.BolsaSeguridad = read["BolsaSeguridad"].ToString();
                    objAsignacion.TipoEnvio = read["TipoEnvio"].ToString();
                    objAsignacion.DiceContener = read["Contenido"].ToString();
                    objAsignacion.FechaAsignacion = Convert.ToDateTime(read["FechaAsignacion"]);
                    objAsignacion.TipoEnvio = read["ADM_TipoCliente"].ToString();
                    objAsignacion.Respuesta = OUEnumValidacionDescargue.Exitosa;
                    ListaGuias.Add(objAsignacion);
                }
                return ListaGuias;
            }
        }


        /// <summary>
        /// Guarda la Novedad de la Guia Ingresada
        /// </summary>
        /// <param name="novedad"></param>
        /// <param name="IdIngresoGuia"></param>
        public void GuardarNovedadGuiaIngresada(OUNovedadIngresoDC novedad, long IdIngresoGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paCrearIngGuiaAgeNovedades_OPU", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IGN_IdNovedad", novedad.IdNovedad));
                cmd.Parameters.Add(new SqlParameter("@IGN_IdIngresoGuiaAgencia", IdIngresoGuia));
                cmd.Parameters.Add(new SqlParameter("@IGN_CreadoPor", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
            }

        }

        /// <summary>
        /// Guarda el Ingreso de la Guia
        /// </summary>
        /// <param name="estadoGuia"></param>
        /// <returns></returns>
        public long GuardarIngresoGuia(ADTrazaGuia estadoGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paCrearIngresoGuiaAgencia_OPU", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter paramOut = new SqlParameter("@IdIngreso", SqlDbType.BigInt);
                paramOut.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramOut);
                cmd.Parameters.Add(new SqlParameter("@IGA_IdAdminisionMensajeria", estadoGuia.IdAdmision.Value));
                cmd.Parameters.Add(new SqlParameter("@IGA_IdAgencia", ControllerContext.Current.IdCentroServicio));
                cmd.Parameters.Add(new SqlParameter("@IGA_NumeroGuia", estadoGuia.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IGA_NumeroPlanilla", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@IGA_IdMensajero", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@IGA_IngresoPorPlanilla", 1));
                cmd.Parameters.Add(new SqlParameter("@IGA_NumeroPieza", 1));
                cmd.Parameters.Add(new SqlParameter("@IGA_TotalPiezas", 1));
                cmd.Parameters.Add(new SqlParameter("@IGA_FechaGrabacion", estadoGuia.FechaGrabacion));
                cmd.Parameters.Add(new SqlParameter("@IGA_CreadoPor", ControllerContext.Current.Usuario));
                //cmd.Parameters.Add(new SqlParameter("@IdIngreso", 0));
                //long idIngreso = Convert.ToInt64(cmd.ExecuteScalar());
                cmd.ExecuteNonQuery();
                long IdIngreso = Convert.ToInt32(paramOut.Value);
                sqlConn.Close();


                return IdIngreso;
            }
        }

        /// <summary>
        /// Obtiene las Guias que se Eliminan de la planilla desde Centro de Acopio por Envio Fuera de Zona
        /// </summary>
        /// <param name="usuario"></param>
        public List<CAAsignacionGuiaDC> ObtenerGuiasEliminadasPlanillaCentroAcopio(string usuario)
        {
            List<CAAsignacionGuiaDC> lista = new List<CAAsignacionGuiaDC>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerGuiasEliminadasPlanilla_CAC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Usuario", ControllerContext.Current.Usuario));
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new CAAsignacionGuiaDC
                    {
                        NumeroGuia = Convert.ToInt64(reader["ADM_NumeroGuia"]),
                        DireccionDestino = Convert.ToString(reader["ADM_DireccionDestinatario"]),
                        CiudadDestino = Convert.ToString(reader["ADM_NombreCiudadDestino"]),
                        Peso = Convert.ToDecimal(reader["ADM_Peso"]),
                        BolsaSeguridad = Convert.ToString(reader["ADM_NumeroBolsaSeguridad"]),
                        TipoEnvio = Convert.ToString(reader["ADM_NombreTipoEnvio"]),
                        DiceContener = Convert.ToString(reader["ADM_DiceContener"]),
                    });
                }
                conn.Close();
                conn.Dispose();
            }
            return lista;
        }

        #endregion





    }
}
