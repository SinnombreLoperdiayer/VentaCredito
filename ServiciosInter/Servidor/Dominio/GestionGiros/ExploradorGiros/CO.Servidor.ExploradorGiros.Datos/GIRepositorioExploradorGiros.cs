using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using CO.Servidor.ExploradorGiros.Datos.Modelo;
using CO.Servidor.GestionGiros.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.ExploradorGiros;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using CO.Servidor.Servicios.Contratos;
using CO.Servidor.Dominio.Comun.LogisticaInversa;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Almacen;

namespace CO.Servidor.ExploradorGiros.Datos
{

    /// <summary>
    /// Clase que contiene los métodos de repositorio
    /// </summary>
    public class GIRepositorioExploradorGiros : IGIRepositorioExploradorGiros
    {
        #region Campos

        private const string NombreModelo = "ModeloExploradorGiros";
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringArchivo = ConfigurationManager.ConnectionStrings["ControllerArchivo"].ConnectionString;
        private CultureInfo cultura = new CultureInfo("es-CO");

        /// <summary>
        /// Atributo utilizado para evitar problemas con multithreading en el singleton.
        /// </summary>
        static object syncRoot = new Object();

        static volatile GIRepositorioExploradorGiros instancia;

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase
        /// </summary>
        /// 
        public static GIRepositorioExploradorGiros Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (syncRoot)
                    {
                        if (instancia == null)
                        {
                            instancia = new GIRepositorioExploradorGiros();
                        }
                    }
                }
                return instancia;
            }
        }
        #endregion Propiedades

        #region Métodos Públicos

        /// <summary>
        /// Obtetener colección giros
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="indicePagina">Indice Página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <returns>Colección giros</returns>
        public IEnumerable<GIAdmisionGirosDC> ObtenerGirosExplorador(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, bool incluyeFecha, out int totalRegistros)
        {
            using (EntidadesExploradorGiros contexto = new EntidadesExploradorGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idGiro;
                string idAgenciaOrigen;
                string idAgenciaDestino;
                string fecha;
                DateTime fechaInicial;
                DateTime fechaFinal;
                string idTipoIdentificacionRemitente;
                string idRemitente;
                string idTipoIdentificacionDestinatario;
                string idDestinatario;
                ObjectParameter totalRegistrosProcedimiento = new ObjectParameter("TotalRegistros", typeof(long));

                filtro.TryGetValue("ADG_IdGiro", out idGiro);
                filtro.TryGetValue("ADG_IdCentroServicioOrigen", out idAgenciaOrigen);
                filtro.TryGetValue("ADG_IdCentroServicioDestino", out idAgenciaDestino);
                filtro.TryGetValue("ADG_FechaGrabacion", out fecha);
                filtro.TryGetValue("ADG_IdTipoIdentificacionRemitente", out idTipoIdentificacionRemitente);
                filtro.TryGetValue("ADG_IdRemitente", out idRemitente);
                filtro.TryGetValue("ADG_IdTipoIdentificacionDestinatario", out idTipoIdentificacionDestinatario);
                filtro.TryGetValue("ADG_IdDestinatario", out idDestinatario);

                if (incluyeFecha)
                {
                    fechaInicial = (Convert.ToDateTime(fecha, Thread.CurrentThread.CurrentCulture)).AddHours(00).AddMinutes(00).AddSeconds(00);
                    fechaFinal = Convert.ToDateTime(fecha, Thread.CurrentThread.CurrentCulture).AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                else
                {
                    fechaInicial = DateTime.Now;
                    fechaFinal = DateTime.Now;
                }

                long? idOrigen = Convert.ToInt64(idAgenciaOrigen);
                if (idOrigen == 0)
                    idOrigen = null;

                long? idDestino = Convert.ToInt64(idAgenciaDestino);
                if (idDestino == 0)
                    idDestino = null;

                IEnumerable<GIAdmisionGirosDC> consulta = contexto.paObtenerGiroExplorador_GIR(indicePagina, registrosPorPagina, Convert.ToInt64(idGiro), idOrigen, idDestino, fechaInicial, fechaFinal, incluyeFecha, idTipoIdentificacionRemitente, idRemitente, idTipoIdentificacionDestinatario, idDestinatario, totalRegistrosProcedimiento)
                  .ToList()
                  .ConvertAll(r => new GIAdmisionGirosDC()
                  {
                      IdAdminGiro = r.ADG_IdAdmisionGiro,
                      IdGiro = r.ADG_IdGiro,
                      EstadoGiro = r.UltimoEstadoGiro,
                      FechaGrabacion = r.ADG_FechaGrabacion,
                      CodVerfiGiro = r.ADG_DigitoVerificacion,
                      IdTipoGiro = r.ADG_IdTipoGiro,
                      UsuarioCreacionGiro = r.ADG_CreadoPor,

                      AgenciaOrigen = new PUCentroServiciosDC()
                      {
                          IdCentroServicio = r.ADG_IdCentroServicioOrigen,
                          Nombre = r.ADG_NombreCentroServicioOrigen
                      },
                      AgenciaDestino = new PUCentroServiciosDC()
                      {
                          IdCentroServicio = r.ADG_IdCentroServicioDestino,
                          Nombre = r.ADG_NombreCentroServicioDestino
                      },
                      Precio = new TAPrecioDC()
                      {
                          ValorTotal = r.ADG_ValorTotal,
                          ValorGiro = r.ADG_ValorGiro,
                          ValorAdicionales = r.ADG_ValorAdicionales,
                          ValorImpuestos = r.ADG_ValorImpuestos,
                          ValorServicio = r.ADG_ValorPorte,
                          ValorTotalServicio = r.ADG_ValorPorte + r.ADG_ValorAdicionales + r.ADG_ValorImpuestos,
                      },
                      GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                      {
                          ClienteRemitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC()
                          {
                              Nombre = r.ADG_NombreRemitente
                          },
                          ClienteDestinatario = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC()
                          {
                              Nombre = r.ADG_NombreDestinatario
                          }
                      }
                  });

                if (consulta == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    totalRegistros = Convert.ToInt32(totalRegistrosProcedimiento.Value == null ? 0 : totalRegistrosProcedimiento.Value);
                    return consulta;
                }
            }
        }

        /// <summary>
        /// Obtiene los tipos de giros
        /// </summary>
        /// <returns>Colección con los tipos de giros</returns>
        public IEnumerable<GITipoGiroDC> ObtenerTiposGiros()
        {
            using (EntidadesExploradorGiros contexto = new EntidadesExploradorGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<GITipoGiroDC> tiposGiros = contexto.TipoGiro_GIR
                  .ToList()
                  .ConvertAll(r => new GITipoGiroDC()
                  {
                      IdTipoGiro = r.TIG_IdTipoGiro,
                      Descripcion = r.TIG_Descripcion
                  });

                if (tiposGiros == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                    return tiposGiros;
            }
        }

        /// <summary>
        ///  Metodo que obtiene el id de la admision a partir del numero del giro
        /// </summary>
        /// <param name="numeroGiro">Numero de la guía</param>
        /// <returns>Identificador de la admisión de la guía</returns>
        public long ValidarGiro(long numeroGiro)
        {
            using (EntidadesExploradorGiros contexto = new EntidadesExploradorGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var idGiro = contexto.paVerificarNumeroGiro_GIR(numeroGiro);
                long? idGiroAdmision = idGiro.FirstOrDefault();
                return idGiroAdmision.HasValue ? idGiroAdmision.Value : 0;
            }
        }

        /// <summary>
        ///    /// Metodo que obtiene el id de la admision a partir del pago
        /// </summary>
        /// <param name="numeroPago">Numero de la guía</param>
        public long ValidarPago(long numeroPago)
        {
            using (EntidadesExploradorGiros contexto = new EntidadesExploradorGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var idPago = contexto.paVerificarNumeroPago_GIR(numeroPago);
                long? idGiroAdmision = idPago.FirstOrDefault();
                return idGiroAdmision.HasValue ? idGiroAdmision.Value : 0;
            }
        }

        /// <summary>
        /// Obtiene los valores adicionales de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección valores adicionales</returns>
        public IEnumerable<TAValorAdicional> ObtenerValoresAdicionalesGiro(long idGiro)
        {
            using (EntidadesExploradorGiros contexto = new EntidadesExploradorGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<TAValorAdicional> adicional = contexto.paObtenerGiroAdicionales_GIR(idGiro)
                  .GroupBy(g => g.IdServicioAdicional)
                .ToList()
                .ConvertAll(r => new TAValorAdicional()
                {
                    Descripcion = r.First().DescripcionServicioAdicional,
                    PrecioValorAdicional = r.First().ValorAdicional,
                    CamposTipoValorAdicionalDC = r.ToList()
                  .ConvertAll(c => new TACampoTipoValorAdicionalDC()
                  {
                      Display = c.CAMPOADICIONAL,
                      ValorCampo = c.VALORCAMPOADICIONAL
                  }),
                });

                if (adicional == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                    return adicional;

                //return adicional.ToList();
            }
        }

        /// <summary>
        /// Obtiene los impuestos de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección impuestos giros</returns>
        public IEnumerable<TAImpuestoDelServicio> ObtenerImpuestosGiros(long idGiro)
        {
            using (EntidadesExploradorGiros contexto = new EntidadesExploradorGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<TAImpuestoDelServicio> impuestos = contexto.paObtenerGiroImpuestos_GIR(idGiro)
                  .ToList()
                  .ConvertAll(r => new TAImpuestoDelServicio()
                  {
                      DescripcionImpuesto = r.DescripcionImpuesto,
                      ValorImpuesto = r.TarifaImpuesto
                  });

                if (impuestos == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                    return impuestos;
            }
        }

        /// <summary>
        /// Obtiene la información del pago
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión</param>
        /// <returns>Objeto pago</returns>
        public PGPagosGirosDC ObtenerInformacionPago(long idAdmisionGiro)
        {
            using (EntidadesExploradorGiros contexto = new EntidadesExploradorGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var infoPago = contexto.paObtenerGiroInfoPago_GIR(idAdmisionGiro).FirstOrDefault();

                if (infoPago == null)
                {
                    return new PGPagosGirosDC();
                }
                else
                {
                    PGPagosGirosDC pagoGiro = new PGPagosGirosDC
                    {
                        IdCentroServiciosPagador = infoPago.PAG_IdCentroServiciosPagador,
                        NombreCentroServicios = infoPago.PAG_NombreCentroServicios,
                        UsuarioPago = infoPago.PAG_CreadoPor,
                        FechaHoraPago = infoPago.PAG_FechaGrabacion,
                        IdComprobantePago = infoPago.PAG_IdComprobantePago,
                        PagoAutorizadoPeaton = infoPago.PAG_PagoAutorizadoPeaton,
                        PagoAutorizadoEmpresarial = infoPago.PAG_PagoAutorizadoEmpresarial,
                        PagoAutomatico = infoPago.PAG_PagoAutomatico,
                        Observaciones = infoPago.PAG_Observaciones,
                        ClienteCobrador = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC()
                        {
                            TipoId = infoPago.PAG_TipoIdCobrador,
                            Identificacion = infoPago.PAG_IdCobrador,
                            Nombre = infoPago.PAG_NombreCobrador,
                            Apellido1 = infoPago.PAG_Apellido1Cobrador,
                            Apellido2 = infoPago.PAG_Apellido2Cobrador,
                            Telefono = infoPago.PAG_TelefonoCobrador,
                            Direccion = infoPago.PAG_DireccionCobrador,
                            Email = infoPago.PAG_EmailCobrador,
                            Ocupacion = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PAOcupacionDC()
                            {
                                DescripcionOcupacion = infoPago.PAG_OcupacionCobrador,
                            }
                        }
                    };

                    string query = "SELECT * FROM AlmacenArchivoGiro_GIR  INNER JOIN  ArchivosPagoGiro_GIR WITH (NOLOCK) ON  APG_IdArchivo = AGI_IdArchivo WHERE APG_IdAdmisionGiro = " + infoPago.PAG_IdAdmisionGiro;
                    using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                    {
                        pagoGiro.Archivos = new List<ASArchivoFramework>();
                        sqlConn.Open();
                        SqlCommand cmd = new SqlCommand(query, sqlConn);
                        DataTable dt = new DataTable();
                        dt.Load(cmd.ExecuteReader());
                        if (dt.Rows.Count != 0)
                            foreach (DataRow dr in dt.Rows)
                            {
                                pagoGiro.Archivos.Add(new ASArchivoFramework
                                {
                                    IdArchivo = Convert.ToInt64(dr["AGI_IdArchivo"]),
                                    NombreArchivo = dr["AGI_NombreAdjunto"].ToString(),
                                    Fecha = Convert.ToDateTime(dr["AGI_FechaGrabacion"])
                                });
                            };
                        sqlConn.Close();
                    }
                    return pagoGiro;
                }
            }
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
                SqlCommand cmd = new SqlCommand("paObtenerRutaArchivoPagoGiro", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGiro", numeroGiro);
                SqlDataReader read = cmd.ExecuteReader();
                if (read.HasRows)
                {
                    if (read.Read())
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
            }
            return archivoGiro;
        }

        /// <summary>
        /// Obtiene el archivo adjunto Solicitud.
        /// </summary>
        /// <param name="idArchivo">Valor del id del archivo adjunto.</param>
        /// <returns>la direccion en bd del archivo</returns>
        public string ObtenerArchivoAdjuntoSolicitud(long idArchivo)
        {
            string respuesta = "";
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasPorArchivoArchivosSolicitud_GIR", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdArchivo", idArchivo);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    respuesta = Convert.ToString(reader["ARG_RutaAdjunto"]);
                }
                sqlConn.Close();
                return respuesta;
            }
        }

        /// <summary>
        /// Obtiene la información del convenio peatón peatón
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns></returns>
        public GIGirosPeatonPeatonDC ObtenerConvenioPeatonPeaton(long idAdmisionGiro)
        {
            long idDestinatario;
            long idRemitente;
            using (EntidadesExploradorGiros contexto = new EntidadesExploradorGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIGirosPeatonPeatonDC remitenteDestinatario = null;

                GIpaObtenerGiroPeatonPeaton consulta = contexto.paObtenerGiroPeatonPeaton_GIR(idAdmisionGiro)
                  .FirstOrDefault();

                if (consulta == null)

                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    remitenteDestinatario = new GIGirosPeatonPeatonDC()
                    {
                        ClienteRemitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC()
                        {
                            IdClienteContado = long.TryParse(consulta.GPP_IdRemitente, out idRemitente) == true ? idRemitente : 0,
                            Identificacion = consulta.GPP_IdRemitente,
                            TipoId = consulta.GPP_TipoIdRemitente,
                            Nombre = consulta.GPP_NombreRemitente,
                            Apellido1 = consulta.GPP_Apellido1Remitente,
                            Apellido2 = consulta.GPP_Apellido2Remitente,
                            Telefono = consulta.GPP_TelefonoRemitente,
                            Direccion = consulta.GPP_DireccionRemitente,
                            Email = consulta.GPP_EmailRemitente,
                            Ocupacion = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PAOcupacionDC()
                            {
                                DescripcionOcupacion = consulta.GPP_OcupacionRemitente
                            }
                        },
                        ClienteDestinatario = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC()
                        {
                            IdClienteContado = long.TryParse(consulta.GPP_IdDestinatario, out idDestinatario) == true ? idDestinatario : 0,
                            Identificacion = consulta.GPP_IdDestinatario,
                            TipoId = consulta.GPP_TipoIdDestinatario,
                            Nombre = consulta.GPP_NombreDestinatario,
                            Apellido1 = consulta.GPP_Apellido1Destinatario,
                            Apellido2 = consulta.GPP_Apellido2Destinatario,
                            Telefono = consulta.GPP_TelefonoDestinatario,
                            Direccion = consulta.GPP_DireccionDestinatario,
                            Email = consulta.GPP_EmailDestinatario,
                            Ocupacion = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PAOcupacionDC()
                            {
                                DescripcionOcupacion = consulta.GPP_OcupacionDestinatario
                            }
                        }
                    };
                }

                return remitenteDestinatario;
            }
        }

        /// <summary>
        /// Consultar la informacion de la tabla peaton Convenio
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public GIGirosPeatonConvenioDC ConsultarInformacionPeatonConvenio(long idAdmisionGiro)
        {
            using (EntidadesExploradorGiros contexto = new EntidadesExploradorGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIpaObtenerGiroPeatonConvenio convenio = contexto.paObtenerGiroPeatonConvenio_GIR(idAdmisionGiro).FirstOrDefault();

                if (convenio == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                return new GIGirosPeatonConvenioDC()
                {
                    ClienteContado = new CLClienteContadoDC()
                    {
                        Apellido1 = convenio.GPC_Apellido1Remitente,
                        Apellido2 = convenio.GPC_Apellido2Remitente,
                        Direccion = convenio.GPC_DireccionRemitente,
                        Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = convenio.GPC_OcupacionRemitente.ToString() },
                        Email = convenio.GPC_EmailRemitente,
                        Identificacion = convenio.GPC_IdRemitente,
                        Nombre = convenio.GPC_NombreRemitente,
                        Telefono = convenio.GPC_TelefonoRemitente,
                        TipoId = convenio.GPC_TipoIdRemitente
                    },
                    ClienteConvenio = new CLClientesDC()
                    {
                        Nit = convenio.GPC_NitConvenioDestinatario,
                        RazonSocial = convenio.GPC_RazonSocialConvenioDestinatario
                    }
                };
            }
        }

        /// <summary>
        /// Obtiene los estados de un giro a traves del tiempo
        /// </summary>
        /// <param name="numeroGiro">Numero de giro</param>
        /// <returns>Lista de estados y Fecha</returns>
        public IList<GIEstadosGirosDC> ObtenerEstadosGiro(long numeroGiro)
        {
            IList<GIEstadosGirosDC> lEstadosGiro = null;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadosGiro", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGiro", numeroGiro);

                SqlDataReader read = cmd.ExecuteReader();
                lEstadosGiro = new List<GIEstadosGirosDC>();
                GIEstadosGirosDC estadoGiro = null;
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        estadoGiro = new GIEstadosGirosDC();
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

        /// <summary>
        /// Obtiene reporte para la uiaf de giros
        /// </summary>
        /// <param name="fechaGeneracion">Fecha para generar el reporte</param>
        /// <returns>Lista con los registros a reportar</returns>
        public List<string> ObtenerReporteUIAFGiros(DateTime fechaGeneracion)
        {
            using (EntidadesExploradorGiros contexto = new EntidadesExploradorGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paGenerarReporteUIAFGiros_GIR(fechaGeneracion).ToList().ConvertAll<string>(
                    r =>
                    {
                        return r.info;
                    });
            }
        }

        #endregion Métodos Públicos

        /// <summary>
        /// Metodo para Obtener información de un giro
        /// </summary>
        /// <param name="informacionGiro"></param>
        /// <returns></returns>
        public GIExploradorGirosWebDC ObtenerDatosGiros(GIExploradorGirosWebDC informacionGiro)
        {

            if (informacionGiro == null)
            {
                throw new ArgumentNullException(nameof(informacionGiro));
            }

            if (String.IsNullOrEmpty(informacionGiro.IdRemitente) && String.IsNullOrEmpty(informacionGiro.IdDestinatario))
            {
                throw new ArgumentException("Parametros no validos ");
            }

            GIExploradorGirosWebDC giro = ObtenerInformacionGiro(informacionGiro);
            if (giro == null) {
                throw new Exception("Giro no encontrado");
            }

            IList<GIEstadosGirosDC> lEstadosGiro = ObtenerEstadosGiro(informacionGiro.NumeroGiro);
            if (lEstadosGiro == null || lEstadosGiro.Count < 1)
            {
                throw new Exception("No se encuentra los estados del giro");
            }


            AlmacenArchivoPagoGiro_LOI almacenArchivo = ObtenerArchivosGiros(giro.NumeroGiro);

            string imagenComprobantePago = ObtenerImagenArchivo(almacenArchivo.APG_RutaAdjunto);
            giro.ImagenGiro = imagenComprobantePago;
            //Limpiar datos privados
            giro.DigitoVerificacion = null;
            giro.CreadoPor = null;
            giro.IdCentroServicioDestino = 0;
            giro.IdCentroServicioOrigen = 0;
            giro.EstadosGiro = lEstadosGiro;
            giro.IdAdmisionGiro = 0;
            return giro;

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

        private GIExploradorGirosWebDC ObtenerInformacionGiro(GIExploradorGirosWebDC informacionGiro)
        {
            GIExploradorGirosWebDC objNuevo = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerInformacionGiro_GIR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGiro", informacionGiro.NumeroGiro);
                if (String.IsNullOrEmpty(informacionGiro.IdDestinatario))
                {
                    cmd.Parameters.AddWithValue("@idRemitente", informacionGiro.IdRemitente);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@idDestinatario", informacionGiro.IdDestinatario);
                }

                SqlDataReader read = cmd.ExecuteReader();

                if (read.HasRows)
                {
                    if (read.Read())
                    {
                        objNuevo = new GIExploradorGirosWebDC();
                        objNuevo.NumeroGiro = Convert.ToInt64(read["ADG_IdGiro"]);
                        objNuevo.IdAdmisionGiro = Convert.ToInt64(read["ADG_IdAdmisionGiro"]);
                        objNuevo.CreadoPor = read["ADG_CreadoPor"].ToString();
                        objNuevo.DigitoVerificacion = read["ADG_DigitoVerificacion"].ToString();
                        objNuevo.FechaGrabacion = Convert.ToDateTime(read["ADG_FechaGrabacion"]);
                        objNuevo.Estado = read["ESG_Estado"].ToString();
                        objNuevo.ValorGiro = Convert.ToInt64(read["ADG_ValorGiro"]);
                        objNuevo.ValorTotal = Convert.ToInt64(read["ADG_ValorTotal"]);
                        objNuevo.IdTipoIdentificacionRemitente = read["ADG_IdTipoIdentificacionRemitente"].ToString();
                        objNuevo.IdRemitente = read["ADG_IdRemitente"].ToString();
                        objNuevo.NombreRemitente = read["ADG_NombreRemitente"].ToString();
                        objNuevo.TelefonoRemitente = read["ADG_TelefonoRemitente"].ToString();
                        objNuevo.EmailRemitente = read["ADG_EmailRemitente"].ToString();
                        objNuevo.IdCentroServicioOrigen = Convert.ToInt64(read["ADG_IdCentroServicioOrigen"]);
                        objNuevo.NombreCentroServicioOrigen = read["ADG_NombreCentroServicioOrigen"].ToString();
                        objNuevo.IdTipoIdentificacionDestinatario = read["ADG_IdTipoIdentificacionDestinatario"].ToString();
                        objNuevo.IdDestinatario = read["ADG_IdDestinatario"].ToString();
                        objNuevo.NombreDestinatario = read["ADG_NombreDestinatario"].ToString();
                        objNuevo.TelefonoDestinatario = read["ADG_TelefonoDestinatario"].ToString();
                        objNuevo.EmailDestinatario = read["ADG_EmailDestinatario"].ToString();
                        objNuevo.IdCentroServicioDestino = Convert.ToInt64(read["ADG_IdCentroServicioDestino"]);
                        objNuevo.NombreCentroServicioDestino = read["ADG_NombreCentroServicioDestino"].ToString();
                    }
                }
            }
            return objNuevo;
        }

        /// <summary>
        /// obtiene los motivos de bloqueo
        /// </summary>
        /// <returns></returns>
        public List<GIMotivosInactivacion> ObtenerMotivosGiros()
        {
            DataTable dtMotivos = new DataTable();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMotivos_GIR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                da = new SqlDataAdapter(cmd);
                da.Fill(dtMotivos);
            }

            List<GIMotivosInactivacion> lista = new List<GIMotivosInactivacion>();
            lista = dtMotivos.AsEnumerable().Select(r => new GIMotivosInactivacion()
            {
                IdMotivoInactivacion = r.Field<short>("MIG_IdMotivoInactivacion"),
                DescMotivoInactivacion = r.Field<string>("MIG_Descripcion")
            }).ToList();

            return lista;
        }

        /// <summary>
        /// obtiene todas los usuarios internas
        /// </summary>
        /// <returns></returns>
        public List<PUArchivosPersonas> ObtenerTodasPersonasInternas()
        {
            DataTable dtPersonas = new DataTable();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTodasPersonasInternas_PAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                da = new SqlDataAdapter(cmd);
                da.Fill(dtPersonas);
            }

            List<PUArchivosPersonas> lista = new List<PUArchivosPersonas>();
            lista = dtPersonas.AsEnumerable().Select(r => new PUArchivosPersonas()
            {
                NombreDocumento = r.Field<string>("PEI_Identificacion"),
                NombreCompleto = r.Field<string>("PEI_Nombre")
            }).ToList();

            return lista;
        }

        /// <summary>
        /// agrega el motivo de bloqueo
        /// </summary>
        /// <param name="GIMotivo"></param>
        public void AgregarMotivoBloqueo(GIMotivosBloqueoDesbloqueo GIMotivo)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paAgregarMotivoBloqueo_GIR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionGiroIdent", GIMotivo.IdAdmisionGiroIdent);
                cmd.Parameters.AddWithValue("@idIdentificacion", GIMotivo.IdIdentificacion);
                cmd.Parameters.AddWithValue("@nombre", GIMotivo.Nombre);
                cmd.Parameters.AddWithValue("@motivo", GIMotivo.Motivo);
                cmd.Parameters.AddWithValue("@fechaGrabacion", GIMotivo.FechaGrabacion);
                cmd.Parameters.AddWithValue("@idAdmisionGiro", GIMotivo.IdAdmisionGiro);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }


        }
    }
}