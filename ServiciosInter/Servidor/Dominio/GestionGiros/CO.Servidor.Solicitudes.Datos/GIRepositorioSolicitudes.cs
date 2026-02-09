using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using CO.Servidor.GestionGiros.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.ParametrosFW;
using CO.Servidor.Solicitudes.Datos.Modelo;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Data.SqlClient;
using System.Data;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using System.Drawing;
using System.Drawing.Imaging;
using Framework.Servidor.Comun.Util;

namespace CO.Servidor.Solicitudes.Datos
{
    /// <summary>
    /// Clase representa el Repositorio de las Solicitudes
    /// </summary>
    public class GIRepositorioSolicitudes
    {
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringExcepciones = ConfigurationManager.ConnectionStrings["ControllerExcepciones"].ConnectionString;

        #region Constructor

        private GIRepositorioSolicitudes()
        {
            filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];
        }

        #endregion Constructor

        #region Campos

        private static readonly GIRepositorioSolicitudes instancia = new GIRepositorioSolicitudes();
        private const string NombreModelo = "ModeloSolicitudes";
        private string filePath = string.Empty;
        private string conexionStringArchivo = ConfigurationManager.ConnectionStrings["ControllerArchivo"].ConnectionString;


        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Se crea la instancia.
        /// </summary>
        public static GIRepositorioSolicitudes Instancia
        {
            get
            {
                return GIRepositorioSolicitudes.instancia;
            }
        }

        #endregion Propiedades

        #region Motivos Solicitudes

        #region Consultas

        /// <summary>
        /// Obtiene los motivos solicitud.
        /// </summary>
        /// <param name="filtro">Valor del Filtro.</param>
        /// <param name="campoOrdenamiento">El campo de ordenamiento.</param>
        /// <param name="indicePagina">el indice pagina.</param>
        /// <param name="registrosPorPagina">los registros por pagina.</param>
        /// <param name="ordenamientoAscendente">Valor de si se prdema ascendentemente<c>true</c> [ordenamiento ascendente].</param>
        /// <param name="totalRegistros">el total registros.</param>
        /// <returns> la lista de la grilla de Motivos Solicitud</returns>
        public List<GIMotivoSolicitudDC> ObtenerMotivosSolicitud(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                totalRegistros = 0;
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lambda = contexto.CrearExpresionLambda<MotivoSolicitud_VGIR>("MOS_EsEditable", "true", OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                return contexto.ConsultarMotivoSolicitud_VGIR(filtro, where, campoOrdenamiento, out totalRegistros,
                                                              indicePagina, registrosPorPagina, ordenamientoAscendente)
                                                              .ToList()
                  .ConvertAll<GIMotivoSolicitudDC>(r => new GIMotivoSolicitudDC()
                {
                    IdMotivo = r.MOS_IdMotivoSolicitud,
                    DescripcionMotivo = r.MOS_Descripcion,
                    DescripcionTipo = r.TIS_Descripcion,
                    RetornaFlete = r.MOS_RetornaFlete.Value,
                    TipoSolicitud = new GITipoSolicitudDC()
                    {
                        Id = r.TIS_IdTipoSolicitud,
                        Descripcion = r.TIS_Descripcion
                    }
                });
            }
        }

        /// <summary>
        /// Consulta la info del Motivo de
        /// una solicitud por el id
        /// </summary>
        /// <param name="idMotivo"></param>
        /// <returns>info del Motivo</returns>
        public GIMotivoSolicitudDC ObtenerMotivoSolicitud(int idMotivo)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                MotivoSolicitud_GIR motivo = contexto.MotivoSolicitud_GIR.FirstOrDefault(mot => mot.MOS_IdMotivoSolicitud == idMotivo);
                GIMotivoSolicitudDC motivoSolicitud = null;

                if (motivo != null)
                {
                    motivoSolicitud = new GIMotivoSolicitudDC()
                    {
                        IdMotivo = motivo.MOS_IdMotivoSolicitud,
                        DescripcionMotivo = motivo.MOS_Descripcion,
                        TipoSolicitud = new GITipoSolicitudDC()
                        {
                            Id = motivo.MOS_IdTipoSolicitud
                        }
                    };
                }

                return motivoSolicitud;
            }
        }

        /// <summary>
        /// Obtiene los Tipos d Solicitudes
        /// </summary>
        /// <returns>Una lista de tipos de Solicitud</returns>
        public List<GITipoSolicitudDC> ObtenerTiposSolicitud()
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoSolicitud_GIR.Include("MotivoSolicitud_GIR").OrderBy(o => o.TIS_Descripcion)
                  .ToList()
                  .ConvertAll<GITipoSolicitudDC>(r => new GITipoSolicitudDC()
                {
                    Id = r.TIS_IdTipoSolicitud,
                    Descripcion = r.TIS_Descripcion,
                    MotivosSolicitudes = r.MotivoSolicitud_GIR.ToList().ConvertAll<GIMotivoSolicitudDC>
                    (sol => new GIMotivoSolicitudDC
                    {
                        IdMotivo = sol.MOS_IdMotivoSolicitud,
                        DescripcionMotivo = sol.MOS_Descripcion
                    })
                });
            }
        }

        /// <summary>
        /// Consulta la info de un tipo de Solicitud
        /// por su Id
        /// </summary>
        /// <param name="idTipoSolicitud"></param>
        /// <returns></returns>
        public GITipoSolicitudDC ObtenerTipoSolicitud(int idTipoSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                GITipoSolicitudDC tipoSolicitud = null;
                TipoSolicitud_GIR tipoSol = contexto.TipoSolicitud_GIR.FirstOrDefault(tip => tip.TIS_IdTipoSolicitud == idTipoSolicitud);

                if (tipoSol != null)
                {
                    tipoSolicitud = new GITipoSolicitudDC()
                    {
                        Id = tipoSol.TIS_IdTipoSolicitud,
                        Descripcion = tipoSol.TIS_Descripcion,
                    };
                }
                return tipoSolicitud;
            }
        }

        #endregion Consultas

        #region Inserciones

        /// <summary>
        /// Adicionar  el motivo solicitud.
        /// </summary>
        /// <param name="motivoSolicitud">Propiedad del motivo solicitud a insertar.</param>
        public void AdicionarMotivoSolicitud(GIMotivoSolicitudDC motivoSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                contexto.MotivoSolicitud_GIR.Add(new MotivoSolicitud_GIR()
                {
                    MOS_IdTipoSolicitud = Convert.ToInt16(motivoSolicitud.TipoSolicitud.Id),
                    MOS_Descripcion = motivoSolicitud.DescripcionMotivo,
                    MOS_EsEditable = true,
                    MOS_CreadoPor = ControllerContext.Current.Usuario,
                    MOS_FechaGrabacion = DateTime.Now,
                    MOS_RetornaFlete = motivoSolicitud.RetornaFlete
                });
                GIRepositorioSolicitudesAuditoria.MapeoAuditMotivoSolicitud(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion Inserciones

        #region Actualizacion

        /// <summary>
        /// Actualiza los motivo solicitud.
        /// </summary>
        /// <param name="motivoSolicitud">Propiedad del Motivo a Actualizar.</param>
        public void ModificarMotivoSolicitud(GIMotivoSolicitudDC motivoSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                MotivoSolicitud_GIR motivoSolicitudDB = contexto.MotivoSolicitud_GIR
                  .Where(r => r.MOS_IdMotivoSolicitud == motivoSolicitud.IdMotivo)
                  .SingleOrDefault();

                if (motivoSolicitudDB != null)
                {
                    motivoSolicitudDB.MOS_IdTipoSolicitud = Convert.ToInt16(motivoSolicitud.TipoSolicitud.Id);
                    motivoSolicitudDB.MOS_Descripcion = motivoSolicitud.DescripcionMotivo;
                    motivoSolicitudDB.MOS_EsEditable = motivoSolicitud.EsEditable;
                    motivoSolicitudDB.MOS_RetornaFlete = motivoSolicitud.RetornaFlete;
                    GIRepositorioSolicitudesAuditoria.MapeoAuditMotivoSolicitud(contexto);
                    contexto.SaveChanges();
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(),
                      MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        #endregion Actualizacion

        #region Eliminacion

        /// <summary>
        /// Eliminar el motivo de la solicitud.
        /// </summary>
        /// <param name="motivoSolicitud">propiedad del motivo de la solicitud.</param>
        public void EliminarMotivoSolicitud(GIMotivoSolicitudDC motivoSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                MotivoSolicitud_GIR eliminarMotivoSolicitud = contexto.MotivoSolicitud_GIR
                  .Where(r => r.MOS_IdMotivoSolicitud == motivoSolicitud.IdMotivo)
                  .SingleOrDefault();

                if (eliminarMotivoSolicitud != null)
                {
                    contexto.MotivoSolicitud_GIR.Remove(eliminarMotivoSolicitud);
                    GIRepositorioSolicitudesAuditoria.MapeoAuditMotivoSolicitud(contexto);
                    contexto.SaveChanges();
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        #endregion Eliminacion

        #endregion Motivos Solicitudes

        #region Solicitudes por Giro

        #region Consultas

        /// <summary>
        /// Consulta el estado del giro y que la agencia de origen pertenezca al Racol
        /// </summary>
        /// <param name="idGiro"></param>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public GIAdmisionGirosDC ObtenerGiroAgenciaOrigenRacol(long idGiro, long idRacol)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var giro = contexto.paObtenerGirosOrigenCentroSvcRacol_GIR(idGiro, idRacol).FirstOrDefault();

                if (giro == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.ERROR_GIRO_NO_EXISTE_O_ORIGEN_NO_PERTENECE_RACOL.ToString(),
                                                        GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_GIRO_NO_EXISTE_O_ORIGEN_NO_PERTENECE_RACOL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                    return new GIAdmisionGirosDC()
                    {
                        IdAdminGiro = giro.ADG_IdAdmisionGiro,
                        IdGiro = giro.ADG_IdGiro,
                        EstadoGiro = giro.UltimoEstado
                    };
            }
        }

        /// <summary>
        /// Consulta el estado del giro y de la solicitud para realizar
        /// La creacion de una Solicitud Nueva
        /// </summary>
        /// <param name="idGiro">Identificador del giro.</param>
        /// <returns>Información del giro</returns>
        /// <remarks>Genera excepción si encuentra solicitudes activas para el id giro dado</remarks>
        public GISolicitudGiroDC ObtenerGiro(long idGiro)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                GiroSinSolicitud_GIR informacionGiro = contexto.paObtenerGiroSinSolicitud_GIR(idGiro)
               .FirstOrDefault();

                if (informacionGiro != null)
                {
                    if (informacionGiro.UltimoEstadoGiro != GIConstantesSolicitudes.ESTADO_GIRO_ANU)
                    {
                        if (informacionGiro.UltimoEstadoGiro != GIConstantesSolicitudes.ESTADO_GIRO_BLQ)
                        {
                            if (informacionGiro.ADG_IdGiro != 0)
                            {
                                return new GISolicitudGiroDC()
                                {
                                    AdmisionGiro = new GIAdmisionGirosDC()
                                    {
                                        IdAdminGiro = informacionGiro.ADG_IdAdmisionGiro,
                                        IdGiro = informacionGiro.ADG_IdGiro,
                                        EstadoGiro = informacionGiro.UltimoEstadoGiro,
                                        GiroAutomatico = informacionGiro.ADG_AdmisionAutomatica,
                                        Precio = new TAPrecioDC()
                                        {
                                            ValorGiro = Convert.ToDecimal(informacionGiro.ADG_ValorGiro),
                                            ValorServicio = Convert.ToDecimal(informacionGiro.ADG_ValorPorte)
                                        },
                                        GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                                        {
                                            ClienteRemitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC()
                                            {
                                                Nombre = informacionGiro.GPP_NombreRemitente,
                                                Apellido1 = informacionGiro.GPP_Apellido1Remitente,
                                                Apellido2 = informacionGiro.GPP_Apellido2Remitente,
                                                Identificacion = informacionGiro.GPP_IdRemitente
                                            },
                                        },
                                        FechaGrabacion = informacionGiro.ADG_FechaGrabacion,

                                        AgenciaOrigen = new PUCentroServiciosDC()
                                        {
                                            IdCentroServicio = informacionGiro.ADG_IdCentroServicioOrigen,
                                            Nombre = informacionGiro.ADG_NombreCentroServicioOrigen,

                                            CiudadUbicacion = new PALocalidadDC()
                                            {
                                                IdLocalidad = informacionGiro.ADG_IdCiudadOrigen,
                                                Nombre = informacionGiro.ADG_DescCiudadOrigen
                                            }
                                        },
                                        AgenciaDestino = new PUCentroServiciosDC()
                                        {
                                            IdCentroServicio = informacionGiro.ADG_IdCentroServicioDestino,
                                            Nombre = informacionGiro.ADG_NombreCentroServicioDestino,
                                            CiudadUbicacion = new PALocalidadDC()
                                            {
                                                IdLocalidad = informacionGiro.ADG_IdCiudadDestino,
                                                Nombre = informacionGiro.ADG_DescCiudadDestino
                                            }
                                        },
                                    },
                                    CambioDestinatario = new GISolCambioDestDC()
                                    {
                                        IdentActual = informacionGiro.GPP_IdDestinatario,
                                        TipDocActual = informacionGiro.GPP_TipoIdDestinatario,
                                        NombreActual = informacionGiro.GPP_NombreDestinatario,
                                        Apellido1Actual = informacionGiro.GPP_Apellido1Destinatario,
                                        Apellido2Actual = informacionGiro.GPP_Apellido2Destinatario,
                                        TelefonoActual = informacionGiro.GPP_TelefonoDestinatario,
                                        DirActual = informacionGiro.GPP_DireccionDestinatario,
                                        MailActual = informacionGiro.GPP_EmailDestinatario,
                                        TipIdReclamaActual = informacionGiro.GPP_TipoIdentificacionReclamoGiro,
                                        OcupacionActual = informacionGiro.GPP_OcupacionDestinatario,

                                        //info de cambio
                                        IdentNuevo = informacionGiro.GPP_IdDestinatario,
                                        TipDocNuevo = informacionGiro.GPP_TipoIdDestinatario,
                                        NombreNuevo = informacionGiro.GPP_NombreDestinatario,
                                        Apellido1Nuevo = informacionGiro.GPP_Apellido1Destinatario,
                                        Apellido2Nuevo = informacionGiro.GPP_Apellido2Destinatario,
                                        TelefonoNuevo = informacionGiro.GPP_TelefonoDestinatario,
                                        DirNuevo = informacionGiro.GPP_DireccionDestinatario,
                                        MailNuevo = informacionGiro.GPP_EmailDestinatario,
                                        TipIdReclamaNuevo = informacionGiro.GPP_TipoIdentificacionReclamoGiro,
                                        OcupacionNuevo = informacionGiro.GPP_OcupacionDestinatario,
                                        TipoDocumentoNuevo = new PATipoIdentificacion()
                                        {
                                            IdTipoIdentificacion = informacionGiro.GPP_TipoIdDestinatario
                                        },
                                        TipoDocumentoReclamaNuevo = new PATipoIdentificacion()
                                        {
                                            IdTipoIdentificacion = informacionGiro.GPP_TipoIdentificacionReclamoGiro
                                        }
                                    },

                                    //Esta es la info del punto o centro de servicio inicial de la solicitud
                                    CentroServicioOrigen = new PUCentroServiciosDC()
                                    {
                                        IdCentroServicio = informacionGiro.ADG_IdCentroServicioDestino,
                                        Nombre = informacionGiro.ADG_NombreCentroServicioDestino
                                    }
                                };
                            }
                            else
                            {
                                //Giro con una Solicitud Activa
                                ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.GIRO_CON_SOLICITUDES_EN_ESTADO_ACTIVO.ToString(),
                                                              GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.GIRO_CON_SOLICITUDES_EN_ESTADO_ACTIVO));
                                throw new FaultException<ControllerException>(excepcion);
                            }
                        }
                        else
                        {
                            //Giro con estado Bloqueado
                            ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.GIRO_CON_ESTADO_BLOQUEADO.ToString(),
                                                          GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.GIRO_CON_ESTADO_BLOQUEADO));
                            throw new FaultException<ControllerException>(excepcion);
                        }
                    }
                    else
                    {
                        //Giro con Estado Anulado
                        ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.GIRO_CON_ESTADO_ANULADO.ToString(),
                                                       GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.GIRO_CON_ESTADO_ANULADO));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
                else
                {
                    //Giro con estado PAGO
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.GIRO_CON_ESTADO_PAGO.ToString(),
                                                    GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.GIRO_CON_ESTADO_PAGO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Trae la info del detalle de la solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public GISolicitudGiroDC ObtenerDataSolicitudGiro(long idSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                InfoSolicitudGiro_GGI informacionGiro = contexto.paObtenerInfoSolicitudGiro_GGI(idSolicitud).FirstOrDefault();

                if (informacionGiro != null && informacionGiro.SOG_IdGiro != 0)
                {
                    GISolicitudGiroDC dataSolicitudGiro = new GISolicitudGiroDC()
                    {
                        IdGiro = informacionGiro.SOG_IdGiro,
                        IdSolicitud = informacionGiro.SOG_IdSolicitudGiro,
                        CentroSolicita = informacionGiro.SOG_NombreCentroServicioSolicita,
                        EstadoSol = ConvertirEstadoSolGiro(informacionGiro.SOG_Estado).ToString(),

                        TipoSolDesc = informacionGiro.SOG_DescripcionTipoSolicitud,
                        CambioDestinatario = new GISolCambioDestDC()
                        {
                            TipDocActual = informacionGiro.GPP_TipoIdDestinatario,
                            IdentActual = informacionGiro.GPP_IdDestinatario,
                            NombreActual = informacionGiro.GPP_NombreDestinatario,
                            Apellido1Actual = informacionGiro.GPP_Apellido1Destinatario,
                            Apellido2Actual = informacionGiro.GPP_Apellido2Destinatario,
                            DirActual = informacionGiro.GPP_DireccionDestinatario,
                            MailActual = informacionGiro.GPP_EmailDestinatario,
                            TelefonoActual = informacionGiro.GPP_TelefonoDestinatario,
                            TipIdReclamaActual = informacionGiro.GPP_TipoIdentificacionReclamoGiro,
                        },
                        TipoSolicitud = new GITipoSolicitudDC()
                        {
                            Id = informacionGiro.SOG_IdTipoSolicitud,
                            Descripcion = informacionGiro.SOG_DescripcionTipoSolicitud
                        },

                        AdmisionGiro = new GIAdmisionGirosDC()
                        {
                            IdGiro = informacionGiro.SOG_IdGiro,
                            IdAdminGiro = informacionGiro.ADG_IdAdmisionGiro,
                            Precio = new TAPrecioDC()
                            {
                                ValorGiro = Convert.ToDecimal(informacionGiro.ADG_ValorGiro)
                            },
                            GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                            {
                                ClienteRemitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC()
                                {
                                    Nombre = informacionGiro.GPP_NombreRemitente,
                                    Apellido1 = informacionGiro.GPP_Apellido1Remitente,
                                    Apellido2 = informacionGiro.GPP_Apellido2Remitente,
                                    Identificacion = informacionGiro.GPP_IdRemitente
                                },
                            },
                        },
                        CentroServicioOrigen = new PUCentroServiciosDC()
                        {
                            IdCentroServicio = Convert.ToInt64(informacionGiro.SCA_IdCentroServiciosActual),
                            Nombre = informacionGiro.SCA_DescCentroServiciosActual == null ? string.Empty : informacionGiro.SCA_DescCentroServiciosActual,

                            CiudadUbicacion = new PALocalidadDC()
                            {
                                IdLocalidad = informacionGiro.ADG_IdCiudadDestino,
                                Nombre = informacionGiro.ADG_DescCiudadDestino
                            },
                        },
                        CambioAgenciaPorAgencia = new PUCentroServiciosDC()
                        {
                            IdCentroServicio = Convert.ToInt64(informacionGiro.SCA_IdCentroServiciosNuevo),
                            Nombre = informacionGiro.SCA_DescCentroServiciosNuevo == null ? string.Empty : informacionGiro.SCA_DescCentroServiciosNuevo,

                            CiudadUbicacion = new PALocalidadDC()
                            {
                                IdLocalidad = informacionGiro.ADG_IdCiudadDestino,
                                Nombre = informacionGiro.ADG_DescCiudadDestino
                            },
                        },
                    };

                    return dataSolicitudGiro;
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                            .ERROR_AL_TRAER_DETALLE_SOLICITUD.ToString(), GISolicitudesServidorMensajes
                                                                            .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_AL_TRAER_DETALLE_SOLICITUD));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Obtene las solicitudes por agencia.
        /// </summary>
        /// <param name="filtro">Campo por que flitraria.</param>
        /// <param name="campoOrdenamiento">Campo de ordenamiento.</param>
        /// <param name="indicePagina">Campo de indice pagina.</param>
        /// <param name="registrosPorPagina">Campo de registros por pagina.</param>
        /// <param name="ordenamientoAscendente">si es verdadero  ordena ascendentemente <c>true</c> [ordenamiento ascendente].</param>
        /// <param name="totalRegistros">Campo de total registros.</param>
        /// <param name="idRacol">Campo de id racol para filtrar.</param>
        /// <param name="idAgencia">Campo de id agencia para filtrar.</param>
        /// <returns>Lista de Solicitudes por agencia</returns>
        public List<GISolicitudGiroDC> ObtenerSolicitudesPorAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina,
                                                                          bool ordenamientoAscendente, out int totalRegistros, string idRacol, string idAgencia)
        {
            totalRegistros = 0;
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<SolicitudesPorGiro_VGIR>("SOG_IdRegionalAdm", idRacol.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                if (idAgencia != null)
                {
                    lamda = contexto.CrearExpresionLambda<SolicitudesPorGiro_VGIR>("SOG_IdCentroServicioSolicita", idAgencia.ToString(), OperadorComparacion.Equal);
                    where.Add(lamda, OperadorLogico.And);
                }

                List<GISolicitudGiroDC> solicitudesGiro = contexto.ConsultarSolicitudesPorGiro_VGIR(filtro, where, "SOG_IdSolicitudGiro", out totalRegistros,
                                                              indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll<GISolicitudGiroDC>(s => new GISolicitudGiroDC()
                  {
                      IdSolicitud = s.SOG_IdSolicitudGiro,
                      CentroSolicita = s.SOG_NombreCentroServicioSolicita,
                      EstadoSol = ConvertirEstadoSolGiro(s.SOG_Estado).ToString(),
                      TipoSolDesc = s.SOG_DescripcionTipoSolicitud,
                      FGrabacionSol = s.SOG_FechaGrabacion,
                      Usuario = s.SOG_CreadoPor,
                      CambioAgenciaPorAgencia = new PUCentroServiciosDC()
                      {
                          IdCentroServicio = s.SOG_IdCentroServicioSolicita,
                          Nombre = s.SOG_NombreCentroServicioSolicita,
                      },

                      AdmisionGiro = new GIAdmisionGirosDC()
                      {
                          IdAdminGiro = s.ADG_IdAdmisionGiro.HasValue ? s.ADG_IdAdmisionGiro.Value : 0,
                          IdGiro = s.ADG_IdGiro,
                          EstadoGiro = s.UltimoEstadoGiro,
                          Precio = new TAPrecioDC()
                          {
                              ValorGiro = s.ADG_ValorGiro.HasValue ? s.ADG_ValorGiro.Value : 0,
                              ValorTotal = s.ADG_ValorTotal.HasValue ? s.ADG_ValorTotal.Value : 0,
                              TarifaFijaPorte = s.ADG_ValorPorte.HasValue ? s.ADG_ValorPorte.Value : 0
                          }

                          //NombreDestinatario = s
                      },

                      TipoSolicitud = new GITipoSolicitudDC()
                      {
                          Id = s.SOG_IdTipoSolicitud,
                          Descripcion = s.SOG_DescripcionTipoSolicitud
                      },
                      RegionalAdministrativa = new PURegionalAdministrativa()
                      {
                          IdRegionalAdmin = s.SOG_IdRegionalAdm,
                          Descripcion = s.SOG_DescripcionRegionalAdm,
                      },
                  });
                return solicitudesGiro;
            }
        }

        /// <summary>
        /// Obtiene las solicitudes aprobadas por id de la solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public GISolicitudGiroDC ObtenerSolictudAprobada(long idSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                GISolicitudGiroDC detalleSol = null;
                var sol = contexto.paObtenerDetalleSolicitudGiro_GIR(idSolicitud).FirstOrDefault();

                if (sol != null)
                {
                    if (sol.SOG_Estado == GIConstantesSolicitudes.ESTADO_SOL_APR)
                    {
                        detalleSol = new GISolicitudGiroDC()
                        {
                            IdSolicitud = sol.SOG_IdSolicitudGiro,
                            IdGiro = sol.ADG_IdGiro,
                            FGrabacionSol = sol.SOG_FechaGrabacion,
                            ObservSolicitud = sol.SOG_Observaciones,
                            EstadoSol = sol.SOG_Estado,
                            TipoSolDesc = sol.SOG_DescripcionTipoSolicitud,
                            DescrMotivoSol = sol.SOG_DescripcionMotivoSolicitud,
                            Usuario = sol.SOG_CreadoPor,
                            IdRegionalAdmin = sol.SOG_IdRegionalAdm,
                            AdmisionGiro = new GIAdmisionGirosDC()
                            {
                                IdGiro = sol.ADG_IdGiro,
                                IdAdminGiro = sol.ADG_IdAdmisionGiro,
                                NombreDestinatario = sol.ADG_NombreDestinatario,
                                NombreRemitente = sol.ADG_NombreRemitente,
                                AgenciaDestino = new PUCentroServiciosDC()
                                {
                                    CiudadUbicacion = new PALocalidadDC()
                                    {
                                        IdLocalidad = sol.ADG_IdCiudadDestino,
                                        Nombre = sol.ADG_DescCiudadDestino
                                    },
                                    IdCentroServicio = sol.ADG_IdCentroServicioDestino,
                                    Nombre = sol.ADG_NombreCentroServicioDestino
                                },
                                AgenciaOrigen = new PUCentroServiciosDC()
                                {
                                    CiudadUbicacion = new PALocalidadDC()
                                    {
                                        IdLocalidad = sol.ADG_IdCiudadOrigen.ToString(),
                                        Nombre = sol.ADG_DescCiudadOrigen
                                    },
                                    IdCentroServicio = sol.ADG_IdCentroServicioOrigen,
                                    Nombre = sol.ADG_NombreCentroServicioOrigen
                                },
                                Precio = new TAPrecioDC()
                                {
                                    ValorGiro = sol.ADG_ValorGiro
                                },
                                GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                                {
                                    ClienteRemitente = new CLClienteContadoDC()
                                    {
                                        Nombre = sol.ADG_NombreRemitente,
                                        Apellido1 = string.Empty,
                                        Apellido2 = string.Empty,
                                        Identificacion = sol.ADG_IdRemitente,
                                        TipoId = sol.ADG_IdTipoIdentificacionRemitente,
                                        Direccion = sol.ADG_DireccionRemitente,
                                        Telefono = sol.ADG_TelefonoRemitente,
                                        Ocupacion = new PAOcupacionDC()
                                        {
                                            DescripcionOcupacion = sol.GPP_OcupacionRemitente
                                        }
                                    },
                                    ClienteDestinatario = new CLClienteContadoDC()
                                    {
                                        Nombre = sol.ADG_NombreDestinatario,
                                        Apellido1 = string.Empty,
                                        Apellido2 = string.Empty,
                                        Identificacion = sol.ADG_IdDestinatario,
                                        TipoId = sol.ADG_IdTipoIdentificacionDestinatario,
                                        Direccion = sol.ADG_DireccionDestinatario,
                                        Telefono = sol.ADG_TelefonoDestinatario,
                                        Ocupacion = new PAOcupacionDC()
                                        {
                                            DescripcionOcupacion = sol.GPP_OcupacionDestinatario
                                        }
                                    }
                                },
                            },

                            CambioDestinatario = new GISolCambioDestDC()
                            {
                                TipDocActual = sol.SCD_TipoDocActual == null ? string.Empty : sol.SCD_TipoDocActual,
                                IdentActual = sol.SCD_IdentificacionActual == null ? string.Empty : sol.SCD_IdentificacionActual,
                                NombreActual = sol.SCD_NombreActual == null ? string.Empty : sol.SCD_NombreActual,
                                Apellido1Actual = sol.SCD_Apellido1Actual == null ? string.Empty : sol.SCD_Apellido1Actual,
                                Apellido2Actual = sol.SCD_Apellido2Actual == null ? string.Empty : sol.SCD_Apellido2Actual,
                                DirActual = sol.SCD_DireccionActual == null ? string.Empty : sol.SCD_DireccionActual,
                                MailActual = sol.SCD_EmailActual == null ? string.Empty : sol.SCD_EmailActual,
                                TelefonoActual = sol.SCD_TelefonoActual == null ? string.Empty : sol.SCD_TelefonoActual,
                                TipIdReclamaActual = sol.SCD_TipoIdReclamaActual == null ? string.Empty : sol.SCD_TipoIdReclamaActual,

                                TipDocNuevo = sol.SCD_TipoDocNuevo == null ? string.Empty : sol.SCD_TipoDocNuevo,
                                IdentNuevo = sol.SCD_IdentificacionNueva == null ? string.Empty : sol.SCD_IdentificacionNueva,
                                NombreNuevo = sol.SCD_NombreNuevo == null ? string.Empty : sol.SCD_NombreNuevo,
                                Apellido1Nuevo = sol.SCD_Apellido1Nuevo == null ? string.Empty : sol.SCD_Apellido1Nuevo,
                                Apellido2Nuevo = sol.SCD_Apellido2Nuevo == null ? string.Empty : sol.SCD_Apellido2Nuevo,
                                DirNuevo = sol.SCD_DireccionNueva == null ? string.Empty : sol.SCD_DireccionNueva,
                                MailNuevo = sol.SCD_EmailNuevo == null ? string.Empty : sol.SCD_EmailNuevo,
                                TelefonoNuevo = sol.SCD_TelefonoNuevo == null ? string.Empty : sol.SCD_TelefonoNuevo,
                                TipIdReclamaNuevo = sol.SCD_TipoIdReclamaNueva == null ? string.Empty : sol.SCD_TipoIdReclamaNueva,
                                TipoDocumentoNuevo = new PATipoIdentificacion()
                                {
                                    IdTipoIdentificacion = sol.SCD_TipoDocNuevo,
                                },
                                TipoDocumentoReclamaNuevo = new PATipoIdentificacion() { IdTipoIdentificacion = sol.SCD_TipoIdReclamaNueva == null ? string.Empty : sol.SCD_TipoIdReclamaNueva }
                            },

                            TipoSolicitud = new GITipoSolicitudDC()
                            {
                                Id = sol.SOG_IdTipoSolicitud,
                                Descripcion = sol.SOG_DescripcionTipoSolicitud
                            },

                            MotivoSolicitud = new GIMotivoSolicitudDC()
                            {
                                IdMotivo = sol.SOG_IdMotivoSolicitud,
                                DescripcionMotivo = sol.SOG_DescripcionMotivoSolicitud,
                            },

                            SolicitudAtendida = new GISolAtendidasDC()
                            {
                                NumeroAtencion = sol.SOA_NumeroAtencion.Value,
                                ObservSolAten = sol.SOA_ObservacionesAtencion,
                                UsuarioSolAtendio = sol.SOA_UsuarioQueAtendio,
                                NombreUsuarioSolAten = sol.SOA_NombreUsuarioQueAtendio
                            },

                            //Es el centro de servicio destino del requerimiento
                            CambioAgenciaPorAgencia = new PUCentroServiciosDC()
                            {
                                IdCentroServicio = Convert.ToInt64(sol.SCA_IdCentroServiciosNuevo),
                                Nombre = sol.SCA_DescCentroServiciosNuevo == null ? string.Empty : sol.SCA_DescCentroServiciosNuevo,
                            },
                            CentroServicioOrigen = new PUCentroServiciosDC()
                            {
                                IdCentroServicio = Convert.ToInt64(sol.SCA_IdCentroServiciosActual),
                                Nombre = sol.SCA_DescCentroServiciosActual == null ? string.Empty : sol.SCA_DescCentroServiciosActual,
                            },
                        };
                    }
                    else if (sol.SOG_Estado == GIConstantesSolicitudes.ESTADO_SOL_ACT)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                                .ERROR_SOLICITUD_ACTIVA_RECHAZADA.ToString(), string.Format(GISolicitudesServidorMensajes
                                                                                .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_ACTIVA_RECHAZADA), GIConstantesSolicitudes.ESTADO_SOL_ACTIVA));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    else if (sol.SOG_Estado == GIConstantesSolicitudes.ESTADO_SOL_REC)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                                .ERROR_SOLICITUD_ACTIVA_RECHAZADA.ToString(), string.Format(GISolicitudesServidorMensajes
                                                                                .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_ACTIVA_RECHAZADA), GIConstantesSolicitudes.ESTADO_SOL_RECHAZADA));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                          .ERROR_SOLICITUD_NO_EXISTE.ToString(), GISolicitudesServidorMensajes
                                                                          .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }

                return detalleSol;
            }
        }

        /// <summary>
        /// Obtiene las solicitudes activas.
        /// </summary>
        /// <param name="filtro">Campo por que filtraria.</param>
        /// <param name="campoOrdenamiento">Campo de ordenamiento.</param>
        /// <param name="indicePagina">Campo de indice pagina.</param>
        /// <param name="registrosPorPagina">Campo de registros por pagina.</param>
        /// <param name="ordenamientoAscendente">si es verdadero  ordena ascendentemente <c>true</c> [ordenamiento ascendente].</param>
        /// <param name="totalRegistros">Campo de total registros.</param>
        /// <returns>LIsta de solicitudes Activas</returns>
        public List<GISolicitudGiroDC> ObtenerSolicitudesActivas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina,
                                                                          bool ordenamientoAscendente, out int totalRegistros, long idRegional)
        {
            totalRegistros = 0;
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda;

                string estado;
                filtro.TryGetValue("SOG_Estado", out estado);
                if (!String.IsNullOrWhiteSpace(estado))
                {
                    if (estado == GIConstantesSolicitudes.ESTADO_SOL_INA)
                        estado = GIConstantesSolicitudes.ESTADO_SOL_ACT;

                    lamda = contexto.CrearExpresionLambda<SolicitudesPorGiro_VGIR>("SOG_Estado", estado, OperadorComparacion.Equal);
                    where.Add(lamda, OperadorLogico.And);
                }
                else
                {
                    lamda = contexto.CrearExpresionLambda<SolicitudesPorGiro_VGIR>("SOG_Estado", GIConstantesSolicitudes.ESTADO_SOL_ACT, OperadorComparacion.Equal);
                    where.Add(lamda, OperadorLogico.And);
                }

                if (idRegional != GIConstantesSolicitudes.VALOR_GRAL_RACOL)
                {
                    lamda = contexto.CrearExpresionLambda<SolicitudesPorGiro_VGIR>("SOG_IdRegionalAdm", idRegional.ToString(), OperadorComparacion.Equal);
                    where.Add(lamda, OperadorLogico.And);
                }

                List<GISolicitudGiroDC> Tabla = contexto.ConsultarSolicitudesPorGiro_VGIR(filtro, where, "SOG_IdSolicitudGiro", out totalRegistros,
                                                               indicePagina, registrosPorPagina, ordenamientoAscendente)
                   .ToList()
                   .ConvertAll<GISolicitudGiroDC>(s => new GISolicitudGiroDC()
                   {
                       IdSolicitud = s.SOG_IdSolicitudGiro,
                       CentroQueSolicita = new PUCentroServiciosDC()
                       {
                           IdCentroServicio = s.SOG_IdCentroServicioSolicita,
                           Nombre = s.SOG_NombreCentroServicioSolicita,
                       },

                       EstadoSol = ConvertirEstadoSolGiro(s.SOG_Estado).ToString(),
                       TipoSolDesc = s.SOG_DescripcionTipoSolicitud,
                       IdGiro = s.ADG_IdGiro.HasValue ? s.ADG_IdGiro.Value : 0,
                       Usuario = s.SOG_CreadoPor,
                       TipoSolicitud = new GITipoSolicitudDC()
                       {
                           Id = s.SOG_IdTipoSolicitud,
                           Descripcion = s.SOG_DescripcionTipoSolicitud
                       },

                       FGrabacionSol = s.SOG_FechaGrabacion,

                       AdmisionGiro = new GIAdmisionGirosDC()
                       {
                           IdAdminGiro = s.ADG_IdAdmisionGiro.HasValue ? s.ADG_IdAdmisionGiro.Value : 0,
                           IdGiro = s.ADG_IdGiro.HasValue ? s.ADG_IdGiro.Value : s.SOG_IdGiro,
                           EstadoGiro = s.UltimoEstadoGiro,
                           Precio = new TAPrecioDC()
                           {
                               ValorGiro = s.ADG_ValorGiro.HasValue ? s.ADG_ValorGiro.Value : 0,
                               ValorTotal = s.ADG_ValorTotal.HasValue ? s.ADG_ValorTotal.Value : 0,
                               TarifaFijaPorte = s.ADG_ValorPorte.HasValue ? s.ADG_ValorPorte.Value : 0
                           }
                       },

                       RegionalAdministrativa = new PURegionalAdministrativa()
                       {
                           IdRegionalAdmin = s.SOG_IdRegionalAdm,
                           Descripcion = s.SOG_DescripcionRegionalAdm,
                       },
                       GiroTransmitido = s.ADG_Transmitido == true ? GIConstantesSolicitudes.GIRO_TRANSMITIDO : string.Empty
                   });
                return Tabla;
            }
        }

        /// <summary>
        /// Obtiene el detalle de una solicitud.
        /// </summary>
        /// <param name="idSolicitud">Valor de la solicitud a consultar.</param>
        /// <returns>el detalle de una solicitud</returns>
        public GISolicitudGiroDC ObtenerDetalleSolicitud(long idSolicitud)
        {
            string query;
            GISolicitudGiroDC solicitudR;
            IEnumerable<GISolicitudGiroDC> solicitud;
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                solicitud = contexto.paObtenerDetalleSolicitudGiro_GIR(idSolicitud)
                   .GroupBy(g => g.SOG_IdSolicitudGiro,
                     (idSolicitudGiro, sol) => new GISolicitudGiroDC
                     {
                         IdSolicitud = sol.First().SOG_IdSolicitudGiro,
                         FGrabacionSol = sol.First().SOG_FechaGrabacion,
                         ObservSolicitud = sol.First().SOG_Observaciones,
                         EstadoSol = sol.First().SOG_Estado,
                         TipoSolDesc = sol.First().SOG_DescripcionTipoSolicitud,
                         DescrMotivoSol = sol.First().SOG_DescripcionMotivoSolicitud,
                         Usuario = sol.First().SOG_CreadoPor,
                         AdmisionGiro = new GIAdmisionGirosDC()
                         {
                             NombreDestinatario = sol.First().ADG_NombreDestinatario,
                             NombreRemitente = sol.First().ADG_NombreRemitente,
                             GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                             {
                                 ClienteRemitente = new CLClienteContadoDC()
                                 {
                                     Nombre = sol.First().ADG_NombreRemitente,
                                     Apellido1 = string.Empty,
                                     Apellido2 = string.Empty,
                                     Identificacion = sol.First().ADG_IdRemitente,
                                 },
                                 ClienteDestinatario = new CLClienteContadoDC()
                                 {
                                     Nombre = sol.First().ADG_NombreDestinatario,
                                     Apellido1 = string.Empty,
                                     Apellido2 = string.Empty,
                                     Identificacion = sol.First().ADG_IdDestinatario
                                 }
                             },
                         },

                         CambioDestinatario = new GISolCambioDestDC()
                           {
                               TipDocActual = sol.First().SCD_TipoDocActual == null ? string.Empty : sol.First().SCD_TipoDocActual,
                               IdentActual = sol.First().SCD_IdentificacionActual == null ? string.Empty : sol.First().SCD_IdentificacionActual,
                               NombreActual = sol.First().SCD_NombreActual == null ? string.Empty : sol.First().SCD_NombreActual,
                               Apellido1Actual = sol.First().SCD_Apellido1Actual == null ? string.Empty : sol.First().SCD_Apellido1Actual,
                               Apellido2Actual = sol.First().SCD_Apellido2Actual == null ? string.Empty : sol.First().SCD_Apellido2Actual,
                               DirActual = sol.First().SCD_DireccionActual == null ? string.Empty : sol.First().SCD_DireccionActual,
                               MailActual = sol.First().SCD_EmailActual == null ? string.Empty : sol.First().SCD_EmailActual,
                               TelefonoActual = sol.First().SCD_TelefonoActual == null ? string.Empty : sol.First().SCD_TelefonoActual,
                               TipIdReclamaActual = sol.First().SCD_TipoIdReclamaActual == null ? string.Empty : sol.First().SCD_TipoIdReclamaActual,

                               TipDocNuevo = sol.First().SCD_TipoDocNuevo == null ? string.Empty : sol.First().SCD_TipoDocNuevo,
                               IdentNuevo = sol.First().SCD_IdentificacionNueva == null ? string.Empty : sol.First().SCD_IdentificacionNueva,
                               NombreNuevo = sol.First().SCD_NombreNuevo == null ? string.Empty : sol.First().SCD_NombreNuevo,
                               Apellido1Nuevo = sol.First().SCD_Apellido1Nuevo == null ? string.Empty : sol.First().SCD_Apellido1Nuevo,
                               Apellido2Nuevo = sol.First().SCD_Apellido2Nuevo == null ? string.Empty : sol.First().SCD_Apellido2Nuevo,
                               DirNuevo = sol.First().SCD_DireccionNueva == null ? string.Empty : sol.First().SCD_DireccionNueva,
                               MailNuevo = sol.First().SCD_EmailNuevo == null ? string.Empty : sol.First().SCD_EmailNuevo,
                               TelefonoNuevo = sol.First().SCD_TelefonoNuevo == null ? string.Empty : sol.First().SCD_TelefonoNuevo,
                               TipIdReclamaNuevo = sol.First().SCD_TipoIdReclamaNueva == null ? string.Empty : sol.First().SCD_TipoIdReclamaNueva,
                               TipoDocumentoNuevo = new PATipoIdentificacion()
                               {
                                   IdTipoIdentificacion = sol.First().SCD_TipoDocNuevo,
                               },
                               TipoDocumentoReclamaNuevo = new PATipoIdentificacion() { IdTipoIdentificacion = sol.First().SCD_TipoIdReclamaNueva == null ? string.Empty : sol.First().SCD_TipoIdReclamaNueva }
                           },

                         MotivoSolicitud = new GIMotivoSolicitudDC()
                         {
                             IdMotivo = sol.First().SOG_IdMotivoSolicitud,
                             DescripcionMotivo = sol.First().SOG_DescripcionMotivoSolicitud,
                         },

                         SolicitudAtendida = new GISolAtendidasDC()
                         {
                             ObservSolAten = sol.First().SOA_ObservacionesAtencion,
                             UsuarioSolAtendio = sol.First().SOA_UsuarioQueAtendio,
                             NombreUsuarioSolAten = sol.First().SOA_NombreUsuarioQueAtendio
                         },

                         //Es el centro de servicio destino del requerimiento
                         CambioAgenciaPorAgencia = new PUCentroServiciosDC()
                         {
                             IdCentroServicio = Convert.ToInt64(sol.First().SCA_IdCentroServiciosNuevo),
                             Nombre = sol.First().SCA_DescCentroServiciosNuevo == null ? string.Empty : sol.First().SCA_DescCentroServiciosNuevo,
                         },
                         CentroServicioOrigen = new PUCentroServiciosDC()
                         {
                             IdCentroServicio = Convert.ToInt64(sol.First().SCA_IdCentroServiciosActual),
                             Nombre = sol.First().SCA_DescCentroServiciosActual == null ? string.Empty : sol.First().SCA_DescCentroServiciosActual,
                         },

                         ValorRealGiro = sol.FirstOrDefault().SCV_ValorRealGiro.HasValue ? sol.FirstOrDefault().SCV_ValorRealGiro.Value : 0,
                         AjusteAlGiro = sol.FirstOrDefault().SCV_ValorAAjustar.HasValue ? sol.FirstOrDefault().SCV_ValorAAjustar.Value : 0,
                         AjusteAlPorte = sol.FirstOrDefault().SCV_ValorPorteDiferencia.HasValue ? sol.FirstOrDefault().SCV_ValorPorteDiferencia.Value : 0,


                     });
                solicitudR = solicitud.FirstOrDefault();
            }

            if (solicitudR != null)
            {

                solicitudR.ArchivosAdjuntos = new List<GIArchivosAdjuntosDC>();
                using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("paObtenerRutasPorSolicitudArchivosSolicitud_GIR", sqlConn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitud);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        GIArchivosAdjuntosDC temp = new GIArchivosAdjuntosDC()
                        {
                            IdArchivo = Convert.ToInt64(reader["ARG_IdArchivo"]),
                            NombreArchivo = Convert.ToString(reader["ARG_NombreAdjunto"])
                        };
                        solicitudR.ArchivosAdjuntos.Add(temp);
                    }
                    sqlConn.Close();
                }
                return solicitudR;
                /*query = @"SELECT * FROM dbo.ArchivosSolicitud_GIR WHERE ARG_IdSolicitudGiro = " + idSolicitud;
                string archivo = string.Empty;

                using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand(query, sqlConn);

                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    sqlConn.Close();
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            solicitudR.ArchivosAdjuntos.Add(
                                new GIArchivosAdjuntosDC
                                            {
                                                IdArchivo = Convert.ToInt64(r["ARG_IdArchivo"]),
                                                NombreArchivo = r["ARG_NombreAdjunto"].ToString()
                                            });
                        };
                    }
                }

                return solicitudR;*/
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                        .ERROR_AL_TRAER_DETALLE_SOLICITUD.ToString(), GISolicitudesServidorMensajes
                                                                        .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_AL_TRAER_DETALLE_SOLICITUD));
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        /// <summary>
        /// Obtener data de la solicitud por el
        /// idgiroAdmision
        /// </summary>
        /// <param name="idGiroAdmision">es el id giro de la Admision</param>
        /// <returns>Data de la solicitud</returns>
        public GISolicitudGiroDC ObtenerSolicitudPorIdGiroAdmision(long idGiroAdmision, int idTipoSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                long idSolicitud = contexto.SolicitudGiro_GIR.OrderByDescending(or => or.SOG_FechaGrabacion)
                                            .FirstOrDefault(i => i.SOG_IdAdmisionGiro == idGiroAdmision &&
                                                            i.SOG_IdTipoSolicitud == idTipoSolicitud &&
                                                            i.SOG_Estado != GIConstantesSolicitudes.ESTADO_SOL_ACT).SOG_IdSolicitudGiro;
                GISolicitudGiroDC infoSolicitud = null;

                infoSolicitud = ObtenerDetalleSolicitud(idSolicitud);

                return infoSolicitud;
            }
        }

        /// <summary>
        /// Obtiene el archivo adjunto.
        /// </summary>
        /// <param name="idArchivo">Valor del id del archivo adjunto.</param>
        /// <returns>la direccion en bd del archivo</returns>
        public string ObtenerArchivoAdjunto(long idArchivo)
        {
            /*string respuesta = "";
            string query = "SELECT * FROM dbo.ArchivosSolicitud_GIR WHERE ARG_IdArchivo = " + idArchivo;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count != 0)
                    respuesta = Convert.ToBase64String(dt.Rows[0]["ARG_Adjunto"] as byte[]);

                sqlConn.Close();
                return respuesta;
            }*/

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
        /// Obtien las solicitudes anteriores.
        /// </summary>
        /// <param name="idAdmisionGiro">Valor del idAdmisionGiro.</param>
        /// <returns>una lista de las solicitudes anteriores</returns>
        public List<GISolicitudGiroDC> ObtenerSolicitudesAnteriores(long idAdmisionGiro)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SolicitudGiro_GIR.Where(sol => sol.SOG_IdAdmisionGiro == idAdmisionGiro)
                  .ToList()
                  .ConvertAll<GISolicitudGiroDC>(s => new GISolicitudGiroDC()
                  {
                      IdSolicitud = s.SOG_IdSolicitudGiro,
                      CentroSolicita = s.SOG_NombreCentroServicioSolicita,
                      EstadoSol = ConvertirEstadoSolGiro(s.SOG_Estado).ToString(),
                      TipoSolDesc = s.SOG_DescripcionTipoSolicitud,
                      FGrabacionSol = s.SOG_FechaGrabacion,
                      Usuario = s.SOG_CreadoPor
                  });
            }
        }

        /// <summary>
        /// Obtiene los centros de servicios que pueden pagar giros y la cantidad de giros pendientes por transmitir
        /// para cada agencia
        /// </summary>
        /// <param name="racol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentroServiciosTransmision(long racol)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerCentroServGirTran_GIR(racol)
                 .OrderByDescending(r => r.Cantidad)
                 .ToList()
                 .ConvertAll<PUCentroServiciosDC>(r => new PUCentroServiciosDC()
                 {
                     IdCentroServicio = r.CES_IdCentroServicios,
                     Nombre = r.CES_Nombre,
                     Tipo = r.CES_Tipo,
                     IdColRacolApoyo = r.REA_IdRegionalAdm,
                     DescripcionRacol = r.REA_Descripcion,
                     Sistematizado = r.CES_Sistematizada,
                     NombreMunicipio = r.LOC_Nombre,
                     Cantidad = r.Cantidad.Value,
                     EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                 });
            }
        }

        /// <summary>
        /// Obtiene el del estado giro.
        /// </summary>
        /// <param name="idAdmisionGiro">campo indexado del giro.</param>
        /// <returns>el estado del giro</returns>
        public bool ObtenerEstadoGiro(long idAdmisionGiro)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                int total = contexto.paObtenerUltimoEstadoGiro_GIR(idAdmisionGiro).Count(r => r.ESG_Estado != GIConstantesSolicitudes.ESTADO_GIRO_PAG);
                return (total != 0);
            }
        }

        /// <summary>
        /// Retorna el Ultimo Estado del Giro,
        /// retorna Null si no lo encuentra
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns></returns>
        public string ObtenerUltimoEstadoGiro(long idAdmisionGiro)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerUltimoEstadoGiro_GIR(idAdmisionGiro).FirstOrDefault().ESG_Estado;
            }
        }

        /// <summary>
        /// Obtiene los datos remitente.
        /// </summary>
        /// <param name="solicitudGiro">propiedad de la solicitud giro.</param>
        /// <returns>los datos del remitente y destinatario del giro en la solicitud</returns>
        public GIGirosPeatonPeatonDC ObtenerDatosRemitente(GISolicitudGiroDC solicitudGiro)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.GirosPeatonPeaton_GIR.Where(gp => gp.GPP_IdAdmisionGiro == solicitudGiro.AdmisionGiro.IdAdminGiro)
                  .ToList()
                  .ConvertAll<GIGirosPeatonPeatonDC>(gipea => new GIGirosPeatonPeatonDC
                  {
                      ClienteRemitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC()
                      {
                          Apellido1 = gipea.GPP_Apellido1Remitente,
                          Apellido2 = gipea.GPP_Apellido2Remitente,
                          Nombre = gipea.GPP_NombreRemitente,
                          Email = gipea.GPP_EmailRemitente
                      }
                  }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Consulta la descripcion de un motivo de solicitud
        /// </summary>
        /// <param name="idMotivo"></param>
        /// <returns></returns>
        public string ConsultarDesMotivo(short idMotivo)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MotivoSolicitud_GIR.Where(motivo => motivo.MOS_IdMotivoSolicitud == idMotivo).FirstOrDefault().MOS_Descripcion;
            };
        }

        /// <summary>
        /// Consulta la descripcion de una solicitud
        /// </summary>
        /// <param name="idTipoSolicitud"></param>
        /// <returns></returns>
        public string ConsultarTipoSolicitud(short idTipoSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoSolicitud_GIR.Where(tipo => tipo.TIS_IdTipoSolicitud == idTipoSolicitud).FirstOrDefault().TIS_Descripcion;
            };
        }

        /// <summary>
        /// Retorna o asigna las solicitudes de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador Admisión giro</param>
        /// <returns>Colección de solicitudes</returns>
        public IEnumerable<GISolicitudGiroDC> ObtenerSolicitudesGiros(long idAdmisionGiro)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<GISolicitudGiroDC> consulta = contexto.paObtenerGiroSolicitud_GIR(idAdmisionGiro)
                  .ToList()
                  .ConvertAll(r => new GISolicitudGiroDC()
                  {
                      CentroSolicita = r.SOG_NombreCentroServicioSolicita,
                      IdSolicitud = r.SOG_IdSolicitudGiro,
                      TipoSolicitud = new GITipoSolicitudDC()
                      {
                          Descripcion = r.SOG_DescripcionTipoSolicitud
                      },
                      FGrabacionSol = r.SOG_FechaGrabacion,
                      EstadoSol = r.SOG_Estado,
                      Usuario = r.SOG_CreadoPor,
                      UsuarioAtendio = r.SOA_NombreUsuarioQueAtendio,
                      FechaAtencion = r.SOA_FechaAtencion,
                      ObservacionesAtencion = r.SOA_ObservacionesAtencion,
                      DescrMotivoSol = r.SOG_DescripcionMotivoSolicitud,
                      ObservSolicitud = r.SOG_Observaciones,
                  });

                if (consulta == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(),
                      MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    foreach (GISolicitudGiroDC r in consulta)
                    {
                        r.ArchivosAsociados = new List<GIArchSolicitudDC>();
                        r.ArchivosAdjuntos = new List<GIArchivosAdjuntosDC>();

                        using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                        {
                            sqlConn.Open();
                            SqlCommand cmd = new SqlCommand("paObtenerRutasPorSolicitudArchivosSolicitud_GIR", sqlConn);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IdSolicitud", r.IdSolicitud);

                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                r.ArchivosAdjuntos.Add(
                                        new GIArchivosAdjuntosDC
                                        {
                                            IdArchivo = Convert.ToInt64(reader["ARG_IdArchivo"]),
                                            NombreArchivo = Convert.ToString(reader["ARG_NombreAdjunto"].ToString())
                                        });

                                r.ArchivosAsociados.Add(
                                    new GIArchSolicitudDC
                                    {
                                        IdArchivo = Convert.ToInt64(reader["ARG_IdArchivo"]),
                                        DescripCargaArchivo = Convert.ToString(reader["ARG_Descripcion"].ToString())
                                    });
                            }
                            sqlConn.Close();
                        }

                        /*string query = @"SELECT * FROM dbo.ArchivosSolicitud_GIR WHERE ARG_IdSolicitudGiro = " + r.IdSolicitud;
                        string archivo = string.Empty;

                        using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
                        {
                            sqlConn.Open();
                            SqlCommand cmd = new SqlCommand(query, sqlConn);

                            DataTable dt = new DataTable();
                            dt.Load(cmd.ExecuteReader());
                            sqlConn.Close();
                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    r.ArchivosAdjuntos.Add(
                                        new GIArchivosAdjuntosDC
                                                    {
                                                        IdArchivo = Convert.ToInt64(row["ARG_IdArchivo"]),
                                                        NombreArchivo = row["ARG_NombreAdjunto"].ToString()
                                                    });

                                    r.ArchivosAsociados.Add(
                                        new GIArchSolicitudDC
                                                    {
                                                        IdArchivo = Convert.ToInt64(row["ARG_IdArchivo"]),
                                                        DescripCargaArchivo = row["ARG_Descripcion"].ToString()
                                                    });
                                };
                            }
                        }*/
                    };
                }
                return consulta;
            }
        }

        /// <summary>
        /// Retorna el archivo seleccionado de la solicitud
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerArchivoSolicitud(long idSolicitud, GIArchSolicitudDC archivoSeleccionado)
        {

            /*string respuesta = "";
            string query = "SELECT * FROM dbo.ArchivosSolicitud_GIR WHERE ARG_IdSolicitudGiro ==  " + idSolicitud + " AND ARG_IdArchivo = " + archivoSeleccionado.IdArchivo;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count != 0)
                    respuesta = Convert.ToBase64String(dt.Rows[0]["ARG_Adjunto"] as byte[]);

                sqlConn.Close();
                return respuesta;
            }*/
            string respuesta = "";
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasImagenArchivosSolicitud_GIR", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdArchivo", archivoSeleccionado.IdArchivo);
                cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitud);

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
        /// Consulta la Solicitud por el campo de IdGiro y
        /// que este Activo
        /// </summary>
        /// <param name="numeroAsociado"></param>
        /// <returns></returns>
        public GISolicitudGiroDC ConsultarSolicitudActivaPorNumeroGiro(long numeroAsociado)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GISolicitudGiroDC solicitudActivo = null;

                SolicitudGiro_GIR sol = contexto.SolicitudGiro_GIR.FirstOrDefault(num => num.SOG_IdGiro == numeroAsociado &&
                                                                                  num.SOG_Estado == GIConstantesSolicitudes.ESTADO_GIRO_ACT);

                if (sol != null)
                {
                    solicitudActivo = new GISolicitudGiroDC()
                    {
                        IdSolicitud = sol.SOG_IdSolicitudGiro,
                        EstadoSol = ConvertirEstadoSolGiro(sol.SOG_Estado).ToString(),
                        FGrabacionSol = sol.SOG_FechaGrabacion,
                    };
                }

                return solicitudActivo;
            }
        }

        /// <summary>
        /// Obtiene la Info del Giro
        /// remitente, destinatario,origen,destino
        /// </summary>
        /// <param name="idAdmision">idAdmisiongiro</param>
        /// <returns>info del Giro</returns>
        public GIAdmisionGirosDC ObtenerInfoGiro(long idAdmision)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIAdmisionGirosDC infoGiro = null;

                paObtenerAdmisionGiro_GIR giro = contexto.paObtenerAdmisionGiro_GIR(idAdmision).FirstOrDefault();
                paObtenerGiroPeatonPeaton_GIR giroPeaton = contexto.paObtenerGiroPeatonPeaton_GIR(idAdmision).FirstOrDefault();

                if (giro != null && giroPeaton != null)
                {
                    infoGiro = new GIAdmisionGirosDC()
                    {
                        IdAdminGiro = idAdmision,
                        IdGiro = giro.ADG_IdGiro,
                        EstadoGiro = giro.ESG_Estado,
                        GiroAutomatico = giro.ADG_AdmisionAutomatica,

                        Precio = new TAPrecioDC()
                        {
                            ValorGiro = Convert.ToDecimal(giro.ADG_ValorGiro),
                            ValorServicio = Convert.ToDecimal(giro.ADG_ValorPorte),
                            ValorTotal = giro.ADG_ValorTotal,
                            TarifaFijaPorte = giro.ADG_ValorPorte
                        },

                        FechaGrabacion = giro.ADG_FechaGrabacion,

                        AgenciaOrigen = new PUCentroServiciosDC()
                        {
                            IdCentroServicio = giro.ADG_IdCentroServicioOrigen,
                            Nombre = giro.ADG_NombreCentroServicioOrigen,

                            CiudadUbicacion = new PALocalidadDC()
                            {
                                IdLocalidad = giro.ADG_IdCiudadOrigen,
                                Nombre = giro.ADG_DescCiudadOrigen
                            },
                            PaisCiudad = new PALocalidadDC()
                            {
                                IdLocalidad = giro.ADG_IdPaisOrigen,
                                Nombre = giro.ADG_DescPaisOrigen
                            }
                        },
                        AgenciaDestino = new PUCentroServiciosDC()
                        {
                            IdCentroServicio = giro.ADG_IdCentroServicioDestino,
                            Nombre = giro.ADG_NombreCentroServicioDestino,
                            CiudadUbicacion = new PALocalidadDC()
                            {
                                IdLocalidad = giro.ADG_IdCiudadDestino,
                                Nombre = giro.ADG_DescCiudadDestino
                            },
                            PaisCiudad = new PALocalidadDC()
                            {
                                IdLocalidad = giro.ADG_IdPaisDestino,
                                Nombre = giro.ADG_DescPaisDestino
                            }
                        },

                        GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                        {
                            ClienteDestinatario = new CLClienteContadoDC()
                            {
                                Apellido1 = giroPeaton.GPP_Apellido1Destinatario,
                                Apellido2 = giroPeaton.GPP_Apellido2Destinatario != null ? giroPeaton.GPP_Apellido2Destinatario : string.Empty,
                                Direccion = giroPeaton.GPP_DireccionDestinatario != null ? giroPeaton.GPP_DireccionDestinatario : string.Empty,
                                Email = giroPeaton.GPP_EmailDestinatario != null ? giroPeaton.GPP_EmailDestinatario : string.Empty,
                                Identificacion = giroPeaton.GPP_IdDestinatario,
                                Nombre = giroPeaton.GPP_NombreDestinatario,
                                Ocupacion = new PAOcupacionDC()
                                {
                                    DescripcionOcupacion = giroPeaton.GPP_OcupacionDestinatario != null ? giroPeaton.GPP_OcupacionDestinatario : string.Empty,
                                },
                                Telefono = giroPeaton.GPP_TelefonoDestinatario,
                                TipoId = giroPeaton.GPP_TipoIdDestinatario
                            },
                            ClienteRemitente = new CLClienteContadoDC()
                            {
                                Apellido1 = giroPeaton.GPP_Apellido1Remitente,
                                Apellido2 = giroPeaton.GPP_Apellido2Remitente != null ? giroPeaton.GPP_Apellido2Remitente : string.Empty,
                                Direccion = giroPeaton.GPP_DireccionRemitente != null ? giroPeaton.GPP_DireccionRemitente : string.Empty,
                                Email = giroPeaton.GPP_EmailRemitente != null ? giroPeaton.GPP_EmailRemitente : string.Empty,
                                Identificacion = giroPeaton.GPP_IdRemitente,
                                Nombre = giroPeaton.GPP_NombreRemitente,
                                Ocupacion = new PAOcupacionDC()
                                {
                                    DescripcionOcupacion = giroPeaton.GPP_OcupacionRemitente != null ? giroPeaton.GPP_OcupacionRemitente : string.Empty,
                                },
                                Telefono = giroPeaton.GPP_TelefonoRemitente,
                                TipoId = giroPeaton.GPP_TipoIdRemitente,
                            },
                        }
                    };
                }
                return infoGiro;
            }
        }

        #endregion Consultas

        #region Inserción

        /// <summary>
        /// Adiciona los Archivos adjuntos a una solicitud
        /// </summary>
        /// <param name="nvaSolicitud"></param>
        public void AdicionarArchivosSolicitud(GIArchSolicitudDC archivo)
        {
            filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            string rutaArchivo;
            rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.MODULO_GESTION_GIROS, archivo.NombreDocAsociado);
            Image imagenArchivo;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                imagenArchivo = Image.FromStream(fs);
                fs.Close();
            }
            string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderArchSolicitud");
            string carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }
            ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
            string ruta = carpetaDestino + "\\" + Guid.NewGuid() + ".jpg";
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            var im = new Bitmap(imagenArchivo);
            im.Save(ruta, jpgEncoder, myEncoderParameters);

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarArchivosSolicitud", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idSolicitudGiro", archivo.IdSolicitud);
                cmd.Parameters.AddWithValue("@idDocumento", archivo.TipDocAsoCambDest.IdDocCambDest);
                cmd.Parameters.AddWithValue("@nombreAdjunto", archivo.NombreDocAsociado);
                cmd.Parameters.AddWithValue("@rutaAdjunto", ruta);
                cmd.Parameters.AddWithValue("@idAdjunto", Guid.NewGuid());
                //cmd.Parameters.AddWithValue("@adjuntoSincronizado", retorno);
                //cmd.Parameters.AddWithValue("@fechaGrabacion", );
                cmd.Parameters.AddWithValue("@usuario", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@descripcion", archivo.TipDocAsoCambDest.DescrTipDocCambDest);
                cmd.ExecuteNonQuery();
                sqlConn.Close(); 
            }

            /*string rutaArchivo;
            rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.MODULO_GESTION_GIROS, arch.NombreDocAsociado);

            byte[] archivoImagen;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }

            string query = @"INSERT INTO [ArchivosSolicitud_GIR] WITH (ROWLOCK)" +
            " ([ARG_NombreAdjunto] ,[ARG_Adjunto] ,[ARG_IdAdjunto]  ,[ARG_IdDocumento]  ,[ARG_IdSolicitudGiro] ,[ARG_FechaCargaArchivo] ,[ARG_Usuario], [ARG_Descripcion])  " +
           " VALUES(@NombreAdjunto ,@Adjunto ,@IdAdjunto ,@IdDocumento,@IdSolicitudGiro ,GETDATE() ,@CreadoPor, @Descripcion)";

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.Add(new SqlParameter("@NombreAdjunto", arch.NombreDocAsociado));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@IdDocumento", arch.TipDocAsoCambDest.IdDocCambDest));
                cmd.Parameters.Add(new SqlParameter("@IdSolicitudGiro ", arch.IdSolicitud));
                cmd.Parameters.Add(new SqlParameter("@Adjunto", (object)archivoImagen));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", arch.TipDocAsoCambDest.DescrTipDocCambDest));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }*/
        }

        /// <summary>
        /// Adiciona Solicitud por cambio de Destinatario
        /// </summary>
        /// <param name="nvaSolicitud">Info de Solicitud</param>
        public long AdicionarSolicitudPorCambioDestinatario(GISolicitudGiroDC nvaSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                long idSolicitud = AdicionarNvoRegistroSolicitud(nvaSolicitud);

                GirosPeatonPeaton_GIR antDest =
                  contexto.GirosPeatonPeaton_GIR.Where(t => t.GPP_IdAdmisionGiro == nvaSolicitud.AdmisionGiro.IdAdminGiro)
                                                .FirstOrDefault();

                SolicitudCambioDestinatario_GIR nvDest = new SolicitudCambioDestinatario_GIR()
                {
                    SCD_IdSolicitudGiro = Convert.ToInt64(nvaSolicitud.IdSolicitud),
                    SCD_TipoDocNuevo = nvaSolicitud.CambioDestinatario.TipoDocumentoNuevo.IdTipoIdentificacion,
                    SCD_IdentificacionNueva = nvaSolicitud.CambioDestinatario.IdentNuevo,
                    SCD_NombreNuevo = nvaSolicitud.CambioDestinatario.NombreNuevo,
                    SCD_Apellido1Nuevo = nvaSolicitud.CambioDestinatario.Apellido1Nuevo,
                    SCD_Apellido2Nuevo = nvaSolicitud.CambioDestinatario.Apellido2Nuevo != null ? nvaSolicitud.CambioDestinatario.Apellido2Nuevo : string.Empty,
                    SCD_DireccionNueva = nvaSolicitud.CambioDestinatario.DirNuevo != null ? nvaSolicitud.CambioDestinatario.DirNuevo : string.Empty,
                    SCD_EmailNuevo = nvaSolicitud.CambioDestinatario.MailNuevo != null ? nvaSolicitud.CambioDestinatario.MailNuevo : string.Empty,
                    SCD_TelefonoNuevo = nvaSolicitud.CambioDestinatario.TelefonoNuevo,
                    SCD_TipoIdReclamaNueva = nvaSolicitud.CambioDestinatario.TipoDocumentoReclamaNuevo != null ? nvaSolicitud.CambioDestinatario.TipoDocumentoReclamaNuevo.IdTipoIdentificacion : nvaSolicitud.CambioDestinatario.TipIdReclamaNuevo,
                    SCD_CreadoPor = ControllerContext.Current.Usuario,
                    SCD_FechaGrabacion = DateTime.Now,
                    SCD_OcupacionNueva = nvaSolicitud.CambioDestinatario.OcupacionNuevo != null ? nvaSolicitud.CambioDestinatario.OcupacionNuevo : string.Empty,

                    //Se agrega la Info del Anterior Destinatario

                    SCD_TipoDocActual = antDest.GPP_TipoIdDestinatario,
                    SCD_IdentificacionActual = antDest.GPP_IdDestinatario,
                    SCD_NombreActual = antDest.GPP_NombreDestinatario,
                    SCD_Apellido1Actual = antDest.GPP_Apellido1Destinatario,
                    SCD_Apellido2Actual = antDest.GPP_Apellido2Destinatario != null ? antDest.GPP_Apellido2Destinatario : string.Empty,
                    SCD_DireccionActual = antDest.GPP_DireccionDestinatario != null ? antDest.GPP_DireccionDestinatario : string.Empty,
                    SCD_EmailActual = antDest.GPP_EmailDestinatario != null ? antDest.GPP_EmailDestinatario : string.Empty,
                    SCD_OcupacionActual = antDest.GPP_OcupacionDestinatario != null ? antDest.GPP_OcupacionDestinatario : string.Empty,
                    SCD_TelefonoActual = antDest.GPP_TelefonoDestinatario,
                    SCD_TipoIdReclamaActual = antDest.GPP_TipoIdentificacionReclamoGiro,
                };

                contexto.SolicitudCambioDestinatario_GIR.Add(nvDest);
                contexto.SaveChanges();

                return idSolicitud;
            }
        }

        /// <summary>
        /// Adiciona un nuevo registro de Solicitud
        /// </summary>
        /// <param name="nvaSolicitud">clase con info de la Solciitud</param>
        /// <param name="contexto"></param>
        public long AdicionarNvoRegistroSolicitud(GISolicitudGiroDC nvaSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                SolicitudGiro_GIR nvSol = new SolicitudGiro_GIR()
                {
                    SOG_IdSolicitudGiro = Convert.ToInt64(nvaSolicitud.IdSolicitud),
                    SOG_IdAdmisionGiro = nvaSolicitud.AdmisionGiro.IdAdminGiro,
                    SOG_Estado = GIConstantesSolicitudes.ESTADO_SOL_ACT,
                    SOG_FechaGrabacion = DateTime.Now,
                    SOG_CreadoPor = ControllerContext.Current.Usuario,
                    SOG_IdMotivoSolicitud = Convert.ToInt16(nvaSolicitud.MotivoSolicitud.IdMotivo),
                    SOG_DescripcionMotivoSolicitud = nvaSolicitud.MotivoSolicitud.DescripcionMotivo,
                    SOG_IdTipoSolicitud = Convert.ToInt16(nvaSolicitud.TipoSolicitud.Id),
                    SOG_DescripcionTipoSolicitud = nvaSolicitud.TipoSolicitud.Descripcion,
                    SOG_IdCentroServicioSolicita = nvaSolicitud.CentroQueSolicita.IdCentroServicio,
                    SOG_NombreCentroServicioSolicita = nvaSolicitud.CentroQueSolicita.Nombre,
                    SOG_Observaciones = nvaSolicitud.ObservSolicitud,
                    SOG_IdRegionalAdm = nvaSolicitud.IdRegionalAdmin,
                    SOG_DescripcionRegionalAdm = nvaSolicitud.RegionalAdminDescripcion,
                    SOG_IdGiro = nvaSolicitud.AdmisionGiro.IdGiro.Value
                };
                contexto.SolicitudGiro_GIR.Add(nvSol);
                contexto.SaveChanges();
                return nvSol.SOG_IdSolicitudGiro;
            }
        }

        /// <summary>
        /// Adiciona la data del cambio de la Agencia
        /// </summary>
        /// <param name="nvaSolicitud"></param>
        public void AdicionarSolicitudCambioAgencia(GISolicitudGiroDC nvaSolicitud)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                SolicitudCambioAgenciaDestino_GIR cambioAgencia = new SolicitudCambioAgenciaDestino_GIR()
                {
                    SCA_IdSolicitudGiro = nvaSolicitud.IdSolicitud.Value,
                    SCA_IdCentroServiciosActual = nvaSolicitud.CentroServicioOrigen.IdCentroServicio,
                    SCA_DescCentroServiciosActual = nvaSolicitud.CentroServicioOrigen.Nombre,
                    SCA_IdCentroServiciosNuevo = nvaSolicitud.CambioAgenciaPorAgencia.IdCentroServicio,
                    SCA_DescCentroServiciosNuevo = nvaSolicitud.CambioAgenciaPorAgencia.Nombre,
                    SCA_FechaGrabacion = DateTime.Now,
                    SCA_CreadoPor = ControllerContext.Current.Usuario,
                };
                contexto.SolicitudCambioAgenciaDestino_GIR.Add(cambioAgencia);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// se inserta la solicitud atendida en la tbla solicitud anulación
        /// Numero Comprobante de Pago Guio
        /// </summary>
        /// <param name="solicitudAtendida">la Informacion para insertar la anulación</param>
        public void AdicionarSolicitudAnulacion(GISolicitudGiroDC solicitudAtendida)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                SolicitudAnulacion_GIR nvaAnulacion = new SolicitudAnulacion_GIR()
                {
                    SOA_IdSolicitudGiro = solicitudAtendida.IdSolicitud.Value,
                    SOA_NoDocumento = solicitudAtendida.AdmisionGiro.IdGiro.Value,
                    SOA_FechaGrabacion = DateTime.Now,
                    SOA_CreadoPor = ControllerContext.Current.Usuario,
                };
                contexto.SolicitudAnulacion_GIR.Add(nvaAnulacion);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Inserta el Cambio del estado del giro
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        /// <param name="contexto"></param>
        public void InsertarCambioEstadoGiro(long idAdmisionGiro, string estadoGiro)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                contexto.paInsertarEstadoGiro_GIR(idAdmisionGiro, estadoGiro, DateTime.Now, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Adiciona los Datos del valores y ajustes
        /// para el cambio de una solicitud
        /// </summary>
        /// <param name="solicitudAtendida">info de la nva Sol</param>
        public void AdicionarSolicitudCambioValor(GISolicitudGiroDC solicitudAtendida)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                SolicitudCambioValor_GIR datosAjuste = new SolicitudCambioValor_GIR()
                {
                    SCV_IdSolicitudGiro = solicitudAtendida.IdSolicitud.Value,
                    SCV_ValorRealGiro = solicitudAtendida.ValorRealGiro,
                    SCV_ValorAAjustar = solicitudAtendida.AjusteAlGiro,
                    SCV_ValorPorteInicial = solicitudAtendida.AdmisionGiro.Precio.ValorServicio,
                    SCV_ValorPorteDiferencia = solicitudAtendida.AjusteAlPorte,
                    SCV_CreadoPor = ControllerContext.Current.Usuario,
                    SCV_FechaGrabacion = DateTime.Now
                };
                contexto.SolicitudCambioValor_GIR.Add(datosAjuste);
                contexto.SaveChanges();
            }
        }

        #endregion Inserción

        #region Edición

        /// <summary>
        /// Inserta el Cambio del destinatario por
        /// una solicitud
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        /// <param name="contexto"></param>
        /// <param name="updateDestinatario"></param>
        public void ActualizarSolicitudPorDevolucion(GISolicitudGiroDC solicitudAtendida)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                GirosPeatonPeaton_GIR updateDestinatario = contexto.GirosPeatonPeaton_GIR.FirstOrDefault(upd => upd.GPP_IdAdmisionGiro == solicitudAtendida.AdmisionGiro.IdAdminGiro);

                if (updateDestinatario != null)
                {
                    //inserto la informacion en la tbl de solicitudCambioDest
                    SolicitudCambioDestinatario_GIR destDevolucion = new SolicitudCambioDestinatario_GIR()
                    {
                        SCD_IdSolicitudGiro = Convert.ToInt64(solicitudAtendida.IdSolicitud),
                        SCD_Apellido1Nuevo = updateDestinatario.GPP_Apellido1Remitente,
                        SCD_Apellido2Nuevo = updateDestinatario.GPP_Apellido2Remitente != null ? updateDestinatario.GPP_Apellido2Remitente : string.Empty,
                        SCD_DireccionNueva = updateDestinatario.GPP_DireccionRemitente != null ? updateDestinatario.GPP_DireccionRemitente : string.Empty,
                        SCD_EmailNuevo = updateDestinatario.GPP_EmailRemitente != null ? updateDestinatario.GPP_EmailRemitente : string.Empty,
                        SCD_IdentificacionNueva = updateDestinatario.GPP_IdRemitente,
                        SCD_NombreNuevo = updateDestinatario.GPP_NombreRemitente,
                        SCD_OcupacionNueva = updateDestinatario.GPP_OcupacionRemitente != null ? updateDestinatario.GPP_OcupacionRemitente : string.Empty,
                        SCD_TelefonoNuevo = updateDestinatario.GPP_TelefonoRemitente,
                        SCD_TipoDocNuevo = updateDestinatario.GPP_TipoIdRemitente,
                        SCD_TipoIdReclamaNueva = updateDestinatario.GPP_TipoIdentificacionReclamoGiro,

                        SCD_Apellido1Actual = updateDestinatario.GPP_Apellido1Remitente,
                        SCD_Apellido2Actual = updateDestinatario.GPP_Apellido2Remitente != null ? updateDestinatario.GPP_Apellido2Remitente : string.Empty,
                        SCD_DireccionActual = updateDestinatario.GPP_DireccionRemitente != null ? updateDestinatario.GPP_DireccionRemitente : string.Empty,
                        SCD_EmailActual = updateDestinatario.GPP_EmailRemitente != null ? updateDestinatario.GPP_EmailRemitente : string.Empty,
                        SCD_IdentificacionActual = updateDestinatario.GPP_IdRemitente,
                        SCD_NombreActual = updateDestinatario.GPP_NombreRemitente,
                        SCD_OcupacionActual = updateDestinatario.GPP_OcupacionRemitente != null ? updateDestinatario.GPP_OcupacionRemitente : string.Empty,
                        SCD_TelefonoActual = updateDestinatario.GPP_TelefonoRemitente,
                        SCD_TipoDocActual = updateDestinatario.GPP_TipoIdRemitente,
                        SCD_TipoIdReclamaActual = updateDestinatario.GPP_TipoIdentificacionReclamoGiro,
                        SCD_CreadoPor = ControllerContext.Current.Usuario,
                        SCD_FechaGrabacion = DateTime.Now,
                    };
                    contexto.SolicitudCambioDestinatario_GIR.Add(destDevolucion);

                    //Actulizo los datos en tbl giro peaton peaton, del destinatario con los datos del remitente para que lo pueda reclamar
                    updateDestinatario.GPP_Apellido1Destinatario = updateDestinatario.GPP_Apellido1Remitente;
                    updateDestinatario.GPP_Apellido2Destinatario = updateDestinatario.GPP_Apellido2Remitente != null ? updateDestinatario.GPP_Apellido2Remitente : string.Empty;
                    updateDestinatario.GPP_DireccionDestinatario = updateDestinatario.GPP_DireccionRemitente != null ? updateDestinatario.GPP_DireccionRemitente : string.Empty;
                    updateDestinatario.GPP_EmailDestinatario = updateDestinatario.GPP_EmailRemitente != null ? updateDestinatario.GPP_EmailRemitente : string.Empty;
                    updateDestinatario.GPP_IdDestinatario = updateDestinatario.GPP_IdRemitente;
                    updateDestinatario.GPP_NombreDestinatario = updateDestinatario.GPP_NombreRemitente;
                    updateDestinatario.GPP_OcupacionDestinatario = updateDestinatario.GPP_OcupacionRemitente != null ? updateDestinatario.GPP_OcupacionRemitente : string.Empty;
                    updateDestinatario.GPP_TelefonoDestinatario = updateDestinatario.GPP_TelefonoRemitente;
                    updateDestinatario.GPP_TipoIdDestinatario = updateDestinatario.GPP_TipoIdRemitente;
                    updateDestinatario.GPP_TipoIdentificacionReclamoGiro = updateDestinatario.GPP_TipoIdentificacionReclamoGiro;

                    contexto.SaveChanges();

                    //Actualizo la Data en la clase enviada con la info de devolución para la impresión del formato
                    solicitudAtendida.AdmisionGiro = ObtenerInfoGiro(solicitudAtendida.AdmisionGiro.IdAdminGiro.Value);

                    //Se actualiza la Ciudad
                    solicitudAtendida.AdmisionGiro.AgenciaDestino.CiudadUbicacion.IdLocalidad = solicitudAtendida.AdmisionGiro.AgenciaOrigen.CiudadUbicacion.IdLocalidad;
                    solicitudAtendida.AdmisionGiro.AgenciaDestino.CiudadUbicacion.Nombre = solicitudAtendida.AdmisionGiro.AgenciaOrigen.CiudadUbicacion.Nombre;

                    //Se actualiza el Centro se Servicio
                    solicitudAtendida.AdmisionGiro.AgenciaDestino.IdCentroServicio = solicitudAtendida.AdmisionGiro.AgenciaOrigen.IdCentroServicio;
                    solicitudAtendida.AdmisionGiro.AgenciaDestino.Nombre = solicitudAtendida.AdmisionGiro.AgenciaOrigen.Nombre;

                    //Se actualiza el pais
                    solicitudAtendida.AdmisionGiro.AgenciaDestino.PaisCiudad.IdLocalidad = solicitudAtendida.AdmisionGiro.AgenciaOrigen.PaisCiudad.IdLocalidad;
                    solicitudAtendida.AdmisionGiro.AgenciaDestino.PaisCiudad.Nombre = solicitudAtendida.AdmisionGiro.AgenciaOrigen.PaisCiudad.Nombre;

                    //se actualiza la observacion de la solicitud agregandole la palabra "DEVOLUCION"
                    solicitudAtendida.AdmisionGiro.Observaciones = GIConstantesSolicitudes.OBSERVACION_DEVOLUCION;
                }

                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                     .ERROR_DATOS_EN_LA_INFORMACION_DEL_GIRO.ToString(), GISolicitudesServidorMensajes
                                     .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_DATOS_EN_LA_INFORMACION_DEL_GIRO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Actualiza la info del Cambio de destinatario
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        public void ActualizarSolicitudPorCambioDestinatario(GISolicitudGiroDC solicitudAtendida)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                SolicitudCambioDestinatario_GIR soliCambioDestestinatario = contexto.SolicitudCambioDestinatario_GIR.FirstOrDefault(cDest => cDest.SCD_IdSolicitudGiro == solicitudAtendida.IdSolicitud);
                GirosPeatonPeaton_GIR updateDestinatario = contexto.GirosPeatonPeaton_GIR.FirstOrDefault(upd => upd.GPP_IdAdmisionGiro == solicitudAtendida.AdmisionGiro.IdAdminGiro);

                if (soliCambioDestestinatario != null)
                {
                    //Se actualizan los cambios con la Nueva informacion para que pueda ser reclamado el giro
                    updateDestinatario.GPP_Apellido1Destinatario = solicitudAtendida.CambioDestinatario.Apellido1Nuevo;
                    updateDestinatario.GPP_Apellido2Destinatario = solicitudAtendida.CambioDestinatario.Apellido2Nuevo != null ? solicitudAtendida.CambioDestinatario.Apellido2Nuevo : string.Empty;
                    updateDestinatario.GPP_DireccionDestinatario = solicitudAtendida.CambioDestinatario.DirNuevo != null ? solicitudAtendida.CambioDestinatario.DirNuevo : string.Empty;
                    updateDestinatario.GPP_EmailDestinatario = solicitudAtendida.CambioDestinatario.MailNuevo != null ? solicitudAtendida.CambioDestinatario.MailNuevo : string.Empty;
                    updateDestinatario.GPP_IdDestinatario = solicitudAtendida.CambioDestinatario.IdentNuevo;
                    updateDestinatario.GPP_NombreDestinatario = solicitudAtendida.CambioDestinatario.NombreNuevo;
                    updateDestinatario.GPP_OcupacionDestinatario = solicitudAtendida.CambioDestinatario.OcupacionNuevo != null ? solicitudAtendida.CambioDestinatario.OcupacionNuevo : string.Empty;
                    updateDestinatario.GPP_TelefonoDestinatario = solicitudAtendida.CambioDestinatario.TelefonoNuevo;
                    updateDestinatario.GPP_TipoIdDestinatario = solicitudAtendida.CambioDestinatario.TipoDocumentoNuevo.IdTipoIdentificacion;
                    updateDestinatario.GPP_TipoIdentificacionReclamoGiro = solicitudAtendida.CambioDestinatario.TipIdReclamaNuevo;

                    contexto.SaveChanges();

                    solicitudAtendida.AdmisionGiro = ObtenerInfoGiro(solicitudAtendida.AdmisionGiro.IdAdminGiro.Value);
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                   .ERROR_NO_TIENE_UN_DESTINATARIO_DEFINIDO.ToString(), GISolicitudesServidorMensajes
                                                                   .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_NO_TIENE_UN_DESTINATARIO_DEFINIDO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Adicionar Solicitud Atendida
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        /// <param name="contexto"></param>
        public GISolicitudGiroDC AdicionarActualizarSolicitudAtendida(GISolicitudGiroDC solicitudAtendida)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                //Actualizo la solicitud Gestionada
                SolicitudGiro_GIR updateSolicitud = contexto.SolicitudGiro_GIR.FirstOrDefault(up => up.SOG_IdSolicitudGiro == solicitudAtendida.IdSolicitud);
                if (updateSolicitud != null)
                {
                    updateSolicitud.SOG_Estado = solicitudAtendida.EstadoSol;
                    contexto.SaveChanges();
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                              .ERROR_SOLICITUD_NO_EXISTE.ToString(), GISolicitudesServidorMensajes
                                                                              .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }

                //Se crea la solicitud atendida para los casos de aprobacion
                //de cambio de agencia dentro de la misma ciudad
                if (solicitudAtendida.SolicitudAtendida == null)
                {
                    solicitudAtendida.SolicitudAtendida = new GISolAtendidasDC();
                }

                //Adiciono la Solicitud Atendida
                SolicitudesAtendidas_GIR SolicitudAtendidatbl = new SolicitudesAtendidas_GIR()
                {
                    SOA_IdSolicitudGiro = Convert.ToInt64(solicitudAtendida.IdSolicitud),
                    SOA_CreadoPor = ControllerContext.Current.Usuario,
                    SOA_FechaGrabacion = DateTime.Now,
                    SOA_UsuarioQueAtendio = ControllerContext.Current.Usuario,
                    SOA_NombreUsuarioQueAtendio = solicitudAtendida.SolicitudAtendida.NombreUsuarioSolAten != null ? solicitudAtendida.SolicitudAtendida.NombreUsuarioSolAten : ControllerContext.Current.Usuario,
                    SOA_ObservacionesAtencion = solicitudAtendida.SolicitudAtendida.ObservSolAten != null ? solicitudAtendida.SolicitudAtendida.ObservSolAten : string.Empty,
                    SOA_FechaAtencion = DateTime.Now,
                };
                contexto.SolicitudesAtendidas_GIR.Add(SolicitudAtendidatbl);
                contexto.SaveChanges();

                solicitudAtendida.SolicitudAtendida.NumeroAtencion = SolicitudAtendidatbl.SOA_NumeroAtencion;

                return solicitudAtendida;
            }
        }

        public void ModificarObservacionesAdmisionGiro(long numeroGiro, string observaciones)
        {
            using (ModeloSolicitudes contexto = new ModeloSolicitudes(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                contexto.paModificarObservacionesGiros_GIR(numeroGiro, observaciones);
            }
        }

        #endregion Edición

        #region Enumerables

        /// <summary>
        /// Convierte el estado de la solicitud.
        /// </summary>
        /// <param name="estadoSol">valor actual del estado de la solicitud.</param>
        /// <returns>la palabra del Estado</returns>
        public GIEnumEstadosSolGirosDC ConvertirEstadoSolGiro(string estadoSol)
        {
            switch (estadoSol)
            {
                case GIConstantesSolicitudes.ESTADO_SOL_ACT:
                    return GIEnumEstadosSolGirosDC.ACTIVA;
                case GIConstantesSolicitudes.ESTADO_SOL_APR:
                    return GIEnumEstadosSolGirosDC.APROBADA;
                default:
                    return GIEnumEstadosSolGirosDC.RECHAZADA;
            }
        }

        /// <summary>
        /// Convierte el estado de la solicitud.
        /// </summary>
        /// <param name="estadoSol">valor actual del estado de la solicitud.</param>
        /// <returns>la palabra del Estado</returns>
        internal string ConvertirEstadoSolGiroOriginal(string estadoSol)
        {
            switch (estadoSol)
            {
                case GIConstantesSolicitudes.ESTADO_SOL_ACTIVA:
                    return GIConstantesSolicitudes.ESTADO_SOL_ACT;
                case GIConstantesSolicitudes.ESTADO_SOL_APROBADA:
                    return GIConstantesSolicitudes.ESTADO_SOL_APR;
                default:
                    return GIConstantesSolicitudes.ESTADO_SOL_REC;
            }
        }

        #endregion Enumerables

        #endregion Solicitudes por Giro

        #region integraciones

        public List<puntoDeAtencion> consultaIngresosEgresosPuntosDeAtencion(string fechaInicial, string fechaFinal)
        {
            List<puntoDeAtencion> lstPuntos = new List<puntoDeAtencion>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerVentasYPagosGirosDePuntos_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@horaInicial", DateTime.Parse(fechaInicial));
                cmd.Parameters.AddWithValue("@horaFinal", DateTime.Parse(fechaFinal));
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    puntoDeAtencion punto = new puntoDeAtencion();
                    punto.montoIngreso = Convert.ToInt64(reader["MontoIngreso"]);
                    punto.montoEgreso = Convert.ToInt64(reader["MontoEgreso"]);
                    punto.ingresoNeto = 0;
                    punto.codigoPuntoAtencion = reader["IdCentroServicio"].ToString();
                    lstPuntos.Add(punto);
                }
                sqlConn.Close();
            }
            return lstPuntos;
        }

        public bool ValidarPassword472(credencialDTO credencial)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paValidarUsuarioIntegraciones_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CampoUsuario", "Usuario472");
                cmd.Parameters.AddWithValue("@CampoPass", "Contraseña472");
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConn.Close();
                if (dt != null)
                {
                    System.Diagnostics.Debug.WriteLine(dt.Rows.Count.ToString());

                    if (dt.Rows[0]["PAI_ValorParametro"].ToString().Trim() == credencial.usuario
                        && dt.Rows[1]["PAI_ValorParametro"].ToString().Trim() == credencial.clave)                    
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// obtiene el valor real de la caja en LOS puntoS
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<puntoDeAtencion> consultaValorRealPorPuntosDeAtencion()
        {
            List<puntoDeAtencion> lstPuntos = new List<puntoDeAtencion>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerValorRealPorPuntosDeAtencion_GIR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    puntoDeAtencion punto = new puntoDeAtencion();
                    punto.montoIngreso = 0;
                    punto.montoEgreso = 0;
                    punto.ingresoNeto = Convert.ToInt64(reader["IngresoTotal"]);
                    punto.codigoPuntoAtencion = reader["IdCentroServicio"].ToString();
                    lstPuntos.Add(punto);
                }
                sqlConn.Close();
            }
            return lstPuntos;
        }

        /// <summary>
        /// obtiene el valor real de la caja en el punto
        /// </summary>
        /// <returns></returns>
        public List<puntoDeAtencion> consultaValorRealPorPuntoDeAtencion(string idCentroServicio)
        {
            List<puntoDeAtencion> lstPunto = new List<puntoDeAtencion>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerValorRealPorPuntoDeAtencion_GIR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCentroServicio", idCentroServicio);
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    puntoDeAtencion punto = new puntoDeAtencion();
                    punto.montoIngreso = 0;
                    punto.montoEgreso = 0;
                    punto.ingresoNeto = Convert.ToInt64(reader["IngresoTotal"]);
                    punto.codigoPuntoAtencion = reader["IdCentroServicio"].ToString();
                    lstPunto.Add(punto);
                }
                sqlConn.Close();
            }
            return lstPunto;
        }

        #endregion


        /// <summary>
        /// Inserta en la tabla de [AuditoriaIntegraciones_AUD] cada vez que sesa consumido el servicio por 472
        /// </summary>
        public void RegistrarAuditoria472(string tipoIntegracion, string request, string response)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringExcepciones))
            {
                SqlCommand cmd = new SqlCommand("paAuditarIntegracion_AUD",sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@AUI_TipoIntegraciOn", tipoIntegracion);
                cmd.Parameters.Add("@AUI_Request", request);
                cmd.Parameters.Add("@AUI_Response", response);
                sqlConn.Open();
                cmd.ExecuteNonQuery();
                sqlConn.Close();
                sqlConn.Dispose();
            }
        }
    }
}