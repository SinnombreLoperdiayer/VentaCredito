using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Almacen;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Giros;
using ServiciosInter.DatosCompartidos.Wrappers;
using ServiciosInter.Infraestructura.AccesoDatos.Interfaces;

namespace ServiciosInter.Infraestructura.AccesoDatos
{
    /// <summary>
    /// Clase que contiene los métodos de repositorio
    /// </summary>
    public class ExploradorGirosRepository : IExploradorGirosRepository
    {
        #region Campos

        private const string NombreModelo = "ModeloExploradorGiros";
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringArchivo = ConfigurationManager.ConnectionStrings["ControllerArchivo"].ConnectionString;

        /// <summary>
        /// Atributo utilizado para evitar problemas con multithreading en el singleton.
        /// </summary>
        private static object syncRoot = new Object();

        private static volatile ExploradorGirosRepository instancia;

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase
        /// </summary>
        ///
        public static ExploradorGirosRepository Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (syncRoot)
                    {
                        if (instancia == null)
                        {
                            instancia = new ExploradorGirosRepository();
                        }
                    }
                }
                return instancia;
            }
        }

        #endregion Propiedades

        #region Métodos

        /// <summary>
        /// Obtiene los estados de un giro a traves del tiempo
        /// </summary>
        /// <param name="numeroGiro">Numero de giro</param>
        /// <returns>Lista de estados y Fecha</returns>
        public IList<EstadosGiro_GIR> ObtenerEstadosGiro(long numeroGiro)
        {
            IList<EstadosGiro_GIR> lEstadosGiro = null;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadosGiro_GIR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGiro", numeroGiro);

                SqlDataReader read = cmd.ExecuteReader();
                lEstadosGiro = new List<EstadosGiro_GIR>();
                EstadosGiro_GIR estadoGiro = null;
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        estadoGiro = new EstadosGiro_GIR();
                        estadoGiro.EstadoGiro = read["ESG_Estado"].ToString();
                        string FechaCambioEstado = read["ESG_FechaGrabacion"].ToString();
                        DateTime FechaCambioEstadoDT = DateTime.Parse(FechaCambioEstado);
                        estadoGiro.FechaCambioEstado = FechaCambioEstadoDT;
                        estadoGiro.idGiro = numeroGiro;
                        lEstadosGiro.Add(estadoGiro);
                    }
                }
            }

            return lEstadosGiro;
        }

        #endregion Métodos

        /// <summary>
        /// Metodo para Obtener información de un giro
        /// </summary>
        /// <param name="informacionGiro"></param>
        /// <returns></returns>
        public ExploradorGirosWrapper ObtenerDatosGiros(ExploradorGirosWrapper informacionGiro)
        {
            if (informacionGiro == null)
            {
                throw new ArgumentNullException(nameof(informacionGiro));
            }

            if (String.IsNullOrEmpty(informacionGiro.IdRemitente) && String.IsNullOrEmpty(informacionGiro.IdDestinatario))
            {
                throw new ArgumentException("Parametros no validos ");
            }

            ExploradorGirosWrapper giro = ObtenerInformacionGiro(informacionGiro);
            if (giro == null)
            {
                throw new Exception("Giro no encontrado");
            }

            IList<EstadosGiro_GIR> lEstadosGiro = ObtenerEstadosGiro(informacionGiro.NumeroGiro);
            if (lEstadosGiro == null || lEstadosGiro.Count < 1)
            {
                throw new Exception("No se encuentra los estados del giro");
            }

            AlmacenArchivoPagoGiro_LOI almacenArchivo = ObtenerArchivosGiros(giro.NumeroGiro);

            string imagenComprobantePago = ObtenerImagenArchivo(almacenArchivo.APG_RutaAdjunto);

            giro.ImagenGiro = imagenComprobantePago;
            giro.EstadosGiro = lEstadosGiro;

            //Limpiar datos privados
            giro.DigitoVerificacion = null;
            giro.CreadoPor = null;
            giro.IdCentroServicioDestino = 0;
            giro.IdCentroServicioOrigen = 0;
            giro.IdAdmisionGiro = 0;
            return giro;
        }

        /// <summary>
        /// Retorna el comprobante de pago de un giro
        /// </summary>
        /// <param name="numeroGiro">NUmero factura del giro</param>
        /// <returns>Archivo</returns>
        public AlmacenArchivoPagoGiro_LOI ObtenerArchivosGiros(long numeroGiro)
        {
            AlmacenArchivoPagoGiro_LOI archivoGiro = new AlmacenArchivoPagoGiro_LOI();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutaArchivoPagoGiro_GIR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGiro", numeroGiro);
                SqlDataReader read = cmd.ExecuteReader();
                if (read.HasRows && read.Read())
                {
                    archivoGiro.APG_CreadoPor = read["APG_CreadoPor"].ToString();
                    archivoGiro.APG_Decodificada = Convert.ToBoolean(read["APG_Decodificada"]);
                    archivoGiro.APG_FechaGrabacion = Convert.ToDateTime(read["APG_FechaGrabacion"]);
                    archivoGiro.APG_IdAdjunto = Guid.Parse(read["APG_IdAdjunto"].ToString());
                    archivoGiro.APG_IdArchivo = Convert.ToInt64(read["APG_IdArchivo"].ToString());
                    archivoGiro.APG_IdComprobantePago = Convert.ToInt64(read["APG_IdComprobantePago"].ToString());
                    archivoGiro.APG_IdGiro = Convert.ToInt64(read["APG_IdGiro"].ToString());
                    archivoGiro.APG_Manual = Convert.ToBoolean(read["APG_Manual"].ToString());
                    archivoGiro.APG_NombreAdjunto = read["APG_NombreAdjunto"].ToString();
                    archivoGiro.APG_RutaAdjunto = read["APG_RutaAdjunto"].ToString();
                }
            }
            return archivoGiro;
        }

        private string ObtenerImagenArchivo(string rutaArchivo)
        {
            if (String.IsNullOrEmpty(rutaArchivo))
            {
                return null;
            }
            if (!File.Exists(rutaArchivo))
            {
                return null;
            }
            return Convert.ToBase64String(File.ReadAllBytes(rutaArchivo));
        }

        private ExploradorGirosWrapper ObtenerInformacionGiro(ExploradorGirosWrapper informacionGiro)
        {
            ExploradorGirosWrapper giroDB = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerInformacionGiro_GIR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGiro", informacionGiro.NumeroGiro);
                //cmd.Parameters.AddWithValue("@DigitoVerificacion", informacionGiro.DigitoVerificacion);
                if (String.IsNullOrEmpty(informacionGiro.IdDestinatario))
                {
                    cmd.Parameters.AddWithValue("@idRemitente", informacionGiro.IdRemitente);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@idDestinatario", informacionGiro.IdDestinatario);
                }

                SqlDataReader read = cmd.ExecuteReader();

                if (read.HasRows && read.Read())
                {
                    giroDB = new ExploradorGirosWrapper();
                    giroDB.NumeroGiro = Convert.ToInt64(read["ADG_IdGiro"]);
                    giroDB.IdAdmisionGiro = Convert.ToInt64(read["ADG_IdAdmisionGiro"]);
                    giroDB.CreadoPor = read["ADG_CreadoPor"].ToString();
                    giroDB.DigitoVerificacion = read["ADG_DigitoVerificacion"].ToString();
                    giroDB.FechaGrabacion = Convert.ToDateTime(read["ADG_FechaGrabacion"]);
                    giroDB.Estado = read["ESG_Estado"].ToString();
                    giroDB.ValorGiro = Convert.ToInt64(read["ADG_ValorGiro"]);
                    giroDB.ValorTotal = Convert.ToInt64(read["ADG_ValorTotal"]);
                    giroDB.IdTipoIdentificacionRemitente = read["ADG_IdTipoIdentificacionRemitente"].ToString();
                    giroDB.IdRemitente = read["ADG_IdRemitente"].ToString();
                    giroDB.NombreRemitente = read["ADG_NombreRemitente"].ToString();
                    giroDB.TelefonoRemitente = read["ADG_TelefonoRemitente"].ToString();
                    giroDB.EmailRemitente = read["ADG_EmailRemitente"].ToString();
                    giroDB.IdCentroServicioOrigen = Convert.ToInt64(read["ADG_IdCentroServicioOrigen"]);
                    giroDB.NombreCentroServicioOrigen = read["ADG_NombreCentroServicioOrigen"].ToString();
                    giroDB.IdTipoIdentificacionDestinatario = read["ADG_IdTipoIdentificacionDestinatario"].ToString();
                    giroDB.IdDestinatario = read["ADG_IdDestinatario"].ToString();
                    giroDB.NombreDestinatario = read["ADG_NombreDestinatario"].ToString();
                    giroDB.TelefonoDestinatario = read["ADG_TelefonoDestinatario"].ToString();
                    giroDB.EmailDestinatario = read["ADG_EmailDestinatario"].ToString();
                    giroDB.IdCentroServicioDestino = Convert.ToInt64(read["ADG_IdCentroServicioDestino"]);
                    giroDB.NombreCentroServicioDestino = read["ADG_NombreCentroServicioDestino"].ToString();
                }
            }
            return giroDB;
        }
    }
}