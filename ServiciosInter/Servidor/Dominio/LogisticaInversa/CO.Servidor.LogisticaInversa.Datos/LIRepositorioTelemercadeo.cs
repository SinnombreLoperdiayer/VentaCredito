using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.ServiceModel;

namespace CO.Servidor.LogisticaInversa.Datos
{
    public partial class LIRepositorioTelemercadeo
    {
        #region Campos

        private static readonly LIRepositorioTelemercadeo instancia = new LIRepositorioTelemercadeo();
        private const string NombreModelo = "ModeloLogisticaInversa";
        private CultureInfo cultura = new CultureInfo("es-CO");
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase LIRepositorioTelemercadeo
        /// </summary>
        public static LIRepositorioTelemercadeo Instancia
        {
            get { return LIRepositorioTelemercadeo.instancia; }
        }

        #endregion Propiedades

        #region Telemercadeo

        #region Consultas

        /// <summary>
        /// Método para obtener el detalle de una guia con el número de gestiones
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGuiaGestiones(long numeroGuia, short idEstado, string localidad)
        {
            return new LIGestionesGuiaDC();
            //using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    LIGestionesGuiaDC guiaGestion = new LIGestionesGuiaDC();
            //    var guiaGestiones = contexto.paObtenerGestionesGuia_LOI(numeroGuia, idEstado, localidad).FirstOrDefault();
            //    if (guiaGestiones != null)
            //    {
            //        guiaGestion.idTrazaGuia = guiaGestiones.EGT_IdEstadoGuiaLog;
            //        guiaGestion.idAdmision = guiaGestiones.EGT_IdAdminisionMensajeria;
            //        guiaGestion.Motivo = guiaGestiones.MOG_Descripcion;
            //        guiaGestion.NombreDestinatario = guiaGestiones.ADM_NombreDestinatario;
            //        guiaGestion.NumeroGuia = guiaGestiones.EGT_NumeroGuia;
            //        guiaGestion.TelefonoDestinatario = guiaGestiones.ADM_TelefonoDestinatario;
            //        guiaGestion.Gestiones = guiaGestiones.Gestiones.Value;
            //        // guiaGestion.DiasTelemercadeo = guiaGestiones.DiasTelemercadeo.Value;
            //    }
            //    return guiaGestion;
            //}
        }

        /// <summary>
        /// Retorna las guías asociadas a la localidad pasada en el estado solicitado
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <param name="numeroPagina"></param>
        /// <param name="tamanoPagina"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGestionesGuiasCol(short idEstado, long idCentroServicio, long numeroGuia)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGestionesGuiasCOL_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@estadoGuia", idEstado));
                cmd.Parameters.Add(new SqlParameter("@idcentroServicio", idCentroServicio));
                cmd.CommandTimeout = 600;
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var r = ds.Tables[0].Rows[0];
                    return new LIGestionesGuiaDC
                    {
                        idTrazaGuia = Convert.ToInt64(r["EGT_IdEstadoGuiaLog"]),
                        Motivo = new ADMotivoGuiaDC
                        {
                            IdMotivoGuia = Convert.ToInt16(r["MOG_IdMotivoGuia"]),
                            Descripcion = Convert.ToString(r["MOG_Descripcion"]),
                            Tipo = ADEnumTipoMotivoDC.Devolucion,
                        },
                        ArchivosAdjuntos = new List<LIEvidenciaDevolucionDC>(),
                        DiasTelemercadeo = r["DiasTelemercadeo"] == null ? 0 : Convert.ToInt32(r["DiasTelemercadeo"]),
                        GuiaAdmision = new ADGuia
                        {
                            IdAdmision = Convert.ToInt64(r["EGT_IdAdminisionMensajeria"]),
                            NumeroGuia = Convert.ToInt64(r["EGT_NumeroGuia"]),
                            IdTipoEntrega = Convert.ToString(r["ADM_IdTipoEntrega"]),
                            DescripcionTipoEntrega = Convert.ToString(r["ADM_DescripcionTipoEntrega"]),
                            Destinatario = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                            {
                                Identificacion = Convert.ToString(r["ADM_IdDestinatario"]),
                                Nombre = Convert.ToString(r["ADM_NombreDestinatario"]),
                                Direccion = Convert.ToString(r["ADM_DireccionDestinatario"]),
                                Telefono = Convert.ToString(r["ADM_TelefonoDestinatario"]),
                                TipoId = Convert.ToString(r["ADM_IdTipoIdentificacionDestinatario"]),
                                Email = Convert.ToString(r["ADM_EmailDestinatario"]),
                            },
                            Remitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                            {
                                Identificacion = Convert.ToString(r["ADM_IdRemitente"]),
                                Nombre = Convert.ToString(r["ADM_NombreRemitente"]),
                                Direccion = Convert.ToString(r["ADM_DireccionRemitente"]),
                                Telefono = Convert.ToString(r["ADM_TelefonoRemitente"]),
                                Email = Convert.ToString(r["ADM_EmailRemitente"]),
                                TipoId = Convert.ToString(r["ADM_IdTipoIdentificacionRemitente"]),
                            },
                            NombreCiudadOrigen = Convert.ToString(r["ADM_NombreCiudadOrigen"]),
                            NombreCiudadDestino = Convert.ToString(r["ADM_NombreCiudadDestino"]),
                            NombreCentroServicioOrigen = Convert.ToString(r["ADM_NombreCentroServicioOrigen"]),
                            NombreCentroServicioDestino = Convert.ToString(r["ADM_NombreCentroServicioDestino"]),
                            TipoCliente = ADEnumTipoCliente.PPE,
                            EstadoGuia = ADEnumEstadoGuia.Telemercadeo,
                            NombreServicio = Convert.ToString(r["ADM_NombreServicio"]),
                            FechaGrabacion = Convert.ToDateTime(r["ADM_FechaGrabacion"]),
                            FormasPagoDescripcion = Convert.ToString(r["FOP_Descripcion"]),
                            ValorTotal = Convert.ToInt64(r["ADM_ValorTotal"]),
                            DiceContener = Convert.ToString(r["ADM_DiceContener"]),
                            Peso = Convert.ToDecimal(r["ADM_Peso"]),
                            ValorDeclarado = Convert.ToInt64(r["ADM_ValorDeclarado"]),
                            NumeroPieza = Convert.ToInt16(r["ADM_NumeroPieza"]),
                            FechaEstimadaEntrega = Convert.ToDateTime(r["ADM_FechaGrabacionEstado"]),
                            IdTipoEnvio = Convert.ToInt16(r["ADM_idTipoEnvio"]),
                            NombreTipoEnvio = Convert.ToString(r["TEN_nombre"]),
                            Alto = Convert.ToInt16(r["ADM_Alto"]),
                            Ancho = Convert.ToInt16(r["ADM_Ancho"]),
                            Largo = Convert.ToInt16(r["ADM_Largo"]),
                            EsPesoVolumetrico = Convert.ToBoolean(r["ADM_EsPesoVolumetrico"]),
                            PesoLiqVolumetrico = Convert.ToInt16(r["ADM_PesoLiqVolumetrico"]),
                            EsRecomendado = Convert.ToBoolean(r["ADM_EsRecomendado"]),
                            TotalPiezas = Convert.ToInt16(r["ADM_TotalPiezas"]),
                            NumeroBolsaSeguridad = Convert.ToString(r["ADM_NumeroBolsaSeguridad"]),
                            Observaciones = Convert.ToString(r["ADM_Observaciones"]),
                            Supervision = Convert.ToBoolean(r["ADM_EsSupervisada"]),
                            ValorAdmision = Convert.ToInt64(r["ADM_ValorAdmision"]),
                            ValorPrimaSeguro = Convert.ToInt64(r["ADM_ValorPrimaSeguro"]),
                            FechaAdmision = Convert.ToDateTime(r["ADM_FechaAdmision"]),
                            CreadoPor = Convert.ToString(r["ADM_CreadoPor"]),
                            ObservacionEstadoGuia = Convert.ToString(r["EGT_DescripcionEstado"]),
                        },
                        Gestiones = 0,
                    };
                }
                else
                    return new LIGestionesGuiaDC();
            }
        }

        /// <summary>
        /// PERMITE ELIMINAR EL NUMERO DE GUIA 
        /// </summary>
        /// <param name="idTrazaGuia"></param>
        public void EliminarCustodiaPorDisposicionFinal(long numeroGuia)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {                
                SqlCommand cmd = new SqlCommand("paEliminarCustodiaPorDispFinal_PUA", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));
                cmd.Parameters.Add(new SqlParameter("@TipoCambio", "Deleted"));
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Retorna las guías asociadas a la localidad pasada en el estado solicitado
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <param name="numeroPagina"></param>
        /// <param name="tamanoPagina"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGestionesGuiasAgencia(short idEstado, long idCentroServicio, long numeroGuia)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGestionesGuias_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@estadoGuia", idEstado));
                cmd.Parameters.Add(new SqlParameter("@idcentroServicio", idCentroServicio));
                cmd.CommandTimeout = 600;
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var r = ds.Tables[0].Rows[0];
                    return new LIGestionesGuiaDC
                    {
                        idTrazaGuia = Convert.ToInt64(r["EGT_IdEstadoGuiaLog"]),
                        Motivo = new ADMotivoGuiaDC
                        {
                            IdMotivoGuia = Convert.ToInt16(r["MOG_IdMotivoGuia"]),
                            Descripcion = Convert.ToString(r["MOG_Descripcion"]),
                            Tipo = ADEnumTipoMotivoDC.Devolucion,
                        },
                        ArchivosAdjuntos = new List<LIEvidenciaDevolucionDC>(),
                        DiasTelemercadeo = r["DiasTelemercadeo"] == null ? 0 : Convert.ToInt32(r["DiasTelemercadeo"]),
                        GuiaAdmision = new ADGuia
                        {
                            IdAdmision = Convert.ToInt64(r["EGT_IdAdminisionMensajeria"]),
                            NumeroGuia = Convert.ToInt64(r["EGT_NumeroGuia"]),
                            Destinatario = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                            {
                                Identificacion = Convert.ToString(r["ADM_IdDestinatario"]),
                                Nombre = Convert.ToString(r["ADM_NombreDestinatario"]),
                                Direccion = Convert.ToString(r["ADM_DireccionDestinatario"]),
                                Telefono = Convert.ToString(r["ADM_TelefonoDestinatario"]),
                            },

                            Remitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                            {
                                Identificacion = Convert.ToString(r["ADM_IdRemitente"]),
                                Nombre = Convert.ToString(r["ADM_NombreRemitente"]),
                                Direccion = Convert.ToString(r["ADM_DireccionRemitente"]),
                                Telefono = Convert.ToString(r["ADM_TelefonoRemitente"]),
                            },
                            NombreCiudadOrigen = Convert.ToString(r["ADM_NombreCiudadOrigen"]),
                            NombreCiudadDestino = Convert.ToString(r["ADM_NombreCiudadDestino"]),
                            NombreCentroServicioOrigen = Convert.ToString(r["ADM_NombreCentroServicioOrigen"]),
                            NombreCentroServicioDestino = Convert.ToString(r["ADM_NombreCentroServicioDestino"]),
                            TipoCliente = ADEnumTipoCliente.PPE,
                            EstadoGuia = ADEnumEstadoGuia.Telemercadeo,
                            NombreServicio = Convert.ToString(r["ADM_NombreServicio"]),
                            FechaGrabacion = Convert.ToDateTime(r["ADM_FechaGrabacion"]),
                            FormasPagoDescripcion = Convert.ToString(r["FOP_Descripcion"]),
                            ValorTotal = Convert.ToInt64(r["ADM_ValorTotal"]),
                            DiceContener = Convert.ToString(r["ADM_DiceContener"]),
                            Peso = Convert.ToDecimal(r["ADM_Peso"]),
                            ValorDeclarado = Convert.ToInt64(r["ADM_ValorDeclarado"]),
                            NumeroPieza = Convert.ToInt16(r["ADM_NumeroPieza"]),
                        },
                        Gestiones = 0,
                    };
                }
                else
                    return new LIGestionesGuiaDC();
            }
        }

        /// <summary>
        /// Consulta las guias de acuerdo a un  estado y una localidad
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <param name="numeroPagina"></param>
        /// <param name="tamanoPagina"></param>
        /// <returns></returns>
        public List<LIGuiaCustodiaDC> ObtenerGuiasEstado(short idEstado, string localidad, int numeroPagina, int tamanoPagina)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var guias = contexto.paObtenerGuiasEstado_LOI(idEstado, localidad, numeroPagina, tamanoPagina).ToList();

                if (guias != null)
                {
                    return guias.ConvertAll(g => new LIGuiaCustodiaDC
                    {
                        TrazaGuia = new ADTrazaGuia
                        {
                            IdTrazaGuia = g.EGT_IdEstadoGuiaLog,
                            FechaGrabacion = g.EGT_FechaGrabacion,
                            IdEstadoGuia = g.EGT_IdEstadoGuia,
                            DescripcionEstadoGuia = g.EGT_DescripcionEstado,
                            IdCiudad = g.EGT_IdLocalidad,
                            Usuario = g.EGT_CreadoPor
                        },
                        idAdmision = g.EGT_IdAdminisionMensajeria,
                        DiasCustodia = g.DiasEstado.Value,
                        NombreDestinatario = g.ADM_NombreDestinatario,
                        NumeroGuia = g.EGT_NumeroGuia,
                        NombreRemitente = g.ADM_NombreRemitente,
                    });
                }
                else
                {
                    return new List<LIGuiaCustodiaDC>();
                }
            }
        }

        /// <summary>
        /// Método para obtener una guía en custodia
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public LIGuiaCustodiaDC ObtenerGuiaCustodia(short idEstado, long numeroGuia, string localidad)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var guia = contexto.paObtenerGuiaCustodia_LOI(idEstado, numeroGuia, localidad).FirstOrDefault();

                if (guia != null)
                {
                    return new LIGuiaCustodiaDC
                    {
                        TrazaGuia = new ADTrazaGuia
                        {
                            IdTrazaGuia = guia.EGT_IdEstadoGuiaLog,
                            FechaGrabacion = guia.EGT_FechaGrabacion,
                            IdEstadoGuia = guia.EGT_IdEstadoGuia,
                            DescripcionEstadoGuia = guia.EGT_DescripcionEstado,
                            IdCiudad = guia.EGT_IdLocalidad,
                            Usuario = guia.EGT_CreadoPor
                        },
                        idAdmision = guia.EGT_IdAdminisionMensajeria,
                        DiasCustodia = guia.DiasEstado.Value,
                        NombreDestinatario = guia.ADM_NombreDestinatario,
                        NumeroGuia = guia.EGT_NumeroGuia,
                        NombreRemitente = guia.ADM_NombreRemitente
                    };
                }
                else
                    return new LIGuiaCustodiaDC();
            }
        }

        public List<LIEvidenciaDevolucionDC> ObtenerArchivosEvidenciaDevolucion(long idEstadoGuia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<LIEvidenciaDevolucionDC> evidencias = contexto.paObtenerArchEviDevolucion_LOI(idEstadoGuia).ToList().ConvertAll(l => new LIEvidenciaDevolucionDC
                {
                    IdEvidenciaDevolucion = l.ARV_IdEvidenciaDevolucion,
                    IdEstadoGuialog = l.VOD_IdEstadoGuiaLog,
                    NombreArchivo = l.ARV_NombreAdjunto
                });
                evidencias.ForEach(e => e.NombreArchivo = e.NombreArchivo.Split('\\').Count() > 0 ? e.NombreArchivo.Split('\\').Last() : e.NombreArchivo);
                return evidencias;
            }
        }

        /// <summary>
        /// Metodo que obtiene los archivos de los intentos de entrega
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIEvidenciaDevolucionDC> ObtenerArchivosIntentosEntrega(long numeroGuia)
        {
            return new List<LIEvidenciaDevolucionDC>();
        }

        /// <summary>
        /// Método encargado de obtener gestiones de una guía
        /// </summary>
        /// <param name="idTrazaguia"></param>
        /// <returns></returns>
        public IList<LIGestionesDC> ObtenerGestionesGuia(long idTrazaguia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var listaGestiones = contexto.paObtenerGestionesTrazaGuia_LOI(idTrazaguia).ToList();

                if (listaGestiones.Any())
                {
                    return listaGestiones
                      .ConvertAll(r => new LIGestionesDC
                    {
                        FechaGestion = r.GGT_FechaGrabacion,
                        IdGestion = r.GGT_IdGestionGuiaTelemercadeo,
                        idTrazaGuia = r.GGT_IdEstadoGuiaLog,
                        Pariente = new PAParienteDC { IdPariente = r.GGT_IdParentestoConDestinatari, NombrePariente = r.PAR_Descripcion },
                        PersonaContesta = r.GGT_PersonaContesta,
                        Resultado = new LIResultadoTelemercadeoDC { IdResultado = r.GGT_IdResultadoTelemercadeo, Descripcion = r.RTM_Descripcion },
                        Telefono = r.GGT_TelefonoMarcado,
                        NuevoTelefono = r.GGT_NuevoTelefono,
                        NuevaDireccion = r.GGT_NuevaDireccionEnvio,
                        NuevoContacto = r.GGT_NuevoContaco,
                        Usuario = r.GGT_CreadoPor,
                        Observaciones = r.GGT_Observacion,

                    }).OrderByDescending(h => h.FechaGestion).ToList();
                }
            }
            return new List<LIGestionesDC>();
        }

        public LIEstadoYMotivoGuiaDC ObtenerEstadoYMotivoGuia(long numeroGuia)
        {
            LIEstadoYMotivoGuiaDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadoYMotivoGuia_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", numeroGuia));
                cmd.CommandTimeout = 600;
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        var r = new LIEstadoYMotivoGuiaDC
                        {
                            IdEstadoGuia = Convert.ToInt16(reader["EGT_IdEstadoGuia"]),
                            DescripcionEstado = Convert.ToString(reader["EGT_DescripcionEstado"]),
                            EstadoLocalidad = Convert.ToString(reader["EstadoLocalidad"]),
                            EstadoModulo = Convert.ToString(reader["EstadoModulo"]),
                            EstadoUsuario = Convert.ToString(reader["EstadoUsuario"]),
                            IdMotivoGuia = Convert.ToInt16(reader["MOG_IdMotivoGuia"]),
                            MotivoDescripcion = Convert.ToString(reader["MotivoDescripcion"]),
                            MotivoUsuario = Convert.ToString(reader["MotivoUsuario"]),
                            IdTipoMotivoGuia = Convert.ToInt16(reader["MOG_IdTipoMotivoGuia"]),
                            MotivoTipo = Convert.ToString(reader["MotivoTipo"]),
                            IdMotivoGuiaCRC = Convert.ToInt16(reader["MOG_IdMotivoGuiaCRC"]),
                            MotivoCRC = Convert.ToString(reader["MotivoCRC"]),
                        };

                        if (!DBNull.Value.Equals(reader["EstadoFechaGravacion"]))
                        {
                            r.EstadoFechaGravacion = Convert.ToDateTime(reader["EstadoFechaGravacion"]);
                        }

                        if (!DBNull.Value.Equals(reader["MotivoFechaGravacion"]))
                        {
                            r.MotivoFechaGravacion = Convert.ToDateTime(reader["MotivoFechaGravacion"]);
                        }
                        resultado = r;
                    }
                }
            }
            return resultado;
        }

        /// <summary>
        /// Método para obtener los resultados posibles de una gestión de telemercadeo
        /// </summary>
        /// <returns></returns>
        public IList<LIResultadoTelemercadeoDC> ObtenerResultados()
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ResultadoTelemercadeo_LOI.Select(j =>
                  new LIResultadoTelemercadeoDC()
                  {
                      Descripcion = j.RTM_Descripcion,
                      Estado = j.RTM_Estado,
                      IdResultado = j.RTM_IdResultadoTelemercadeo,
                      @namespace = j.RTM_Namespace,
                      nombreAssembly = j.RTM_NombreAssembly,
                      nombreClase = j.RTM_NombreClase
                  }
                  ).ToList();
            }
        }

        /// <summary>
        /// Método para obtener las razones de borrado de una gestión
        /// </summary>
        /// <returns></returns>
        public IList<LIGestionMotivoBorradoDC> ObtenerMotivosBorrado()
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MotivoBorraGtionGuiaTeleme_LOI.Where(j => j.MBT_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(
                  j => new LIGestionMotivoBorradoDC()
                  {
                      IdMotivo = j.MBT_IdMotivoBorrado,
                      Descripcion = j.MBT_Descripcion,
                  });
            }
        }

        #endregion Consultas

        #region Inserciones

        /// <summary>
        /// Método para insertar una gestión de telemercadeo
        /// </summary>
        /// <param name="?"></param>
        public int InsertarGestion(LIGestionesDC gestion)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ObjectResult<paCrearGestionGuia_LOI> id = contexto.paCrearGestionGuia_LOI(
                   gestion.idAdmisionGuia,
                   gestion.NumeroGuia,
                   gestion.Resultado.IdResultado,
                   gestion.Telefono,
                   gestion.PersonaContesta,
                   gestion.Pariente.IdPariente,
                   gestion.NuevoTelefono,
                   gestion.NuevoContacto,
                   gestion.NuevaDireccion,
                   gestion.Observaciones,
                   gestion.idTrazaGuia,
                   gestion.FechaGestion, gestion.Usuario);

                return Convert.ToInt32(id.FirstOrDefault().idGestionGuiaTelemercadeo);
            }
        }

        public void InsertarGestionDAO(LIGestionesDC gestion)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand comando = new SqlCommand("paCrearGestionGuia_LOI", conn);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@GGT_IdAdminisionMensajeria", gestion.idAdmisionGuia);
                comando.Parameters.AddWithValue("@GGT_NumeroGuia", gestion.NumeroGuia);
                comando.Parameters.AddWithValue("@GGT_IdResultadoTelemercadeo", (short)gestion.TipoGestion);
                comando.Parameters.AddWithValue("@GGT_TelefonoMarcado", gestion.Telefono);
                comando.Parameters.AddWithValue("@GGT_PersonaContesta", gestion.PersonaContesta);
                comando.Parameters.AddWithValue("@GGT_IdParentestoConDestinatari", 7); // Otros
                comando.Parameters.AddWithValue("@GGT_NuevoTelefono", gestion.NuevoTelefono);
                comando.Parameters.AddWithValue("@GGT_NuevoContaco", gestion.NuevoContacto);
                comando.Parameters.AddWithValue("@GGT_NuevaDireccionEnvio", gestion.NuevaDireccion);
                comando.Parameters.AddWithValue("@GGT_Observacion", gestion.Observaciones);
                comando.Parameters.AddWithValue("@GGT_IdEstadoGuiaLog", gestion.idTrazaGuia);
                comando.Parameters.AddWithValue("@GGT_FechaGrabacion", gestion.FechaGestion);
                comando.Parameters.AddWithValue("@GGT_CreadoPor", ControllerContext.Current.Usuario);

                comando.ExecuteNonQuery();
            }
        }

        #endregion Inserciones

        #region Eliminar

        /// <summary>
        /// Método para guardar
        /// </summary>
        /// <param name="idGestion"></param>
        public void EliminarGestion(LIGestionesDC Gestion)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarGestionGuia_LOI(Gestion.MotivoBorrado.IdMotivo, Gestion.IdGestion, Gestion.Usuario, LOIConstantesLogisticaInversa.ACCION_ELIMINAR);
            }
        }

        #endregion Eliminar

        #endregion Telemercadeo

        #region Rapiradicados

        #region Consultas

        /// <summary>
        /// Método  para obtener las guias en estado rapiradicado y en estado supervision
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IList<ADTrazaGuia> ObtenerGuiasRapiradicados(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string numeroGuia;
                string localidad;
                string fechaInicial;
                string fechaFinal;
                DateTime fechaI;
                DateTime fechaF;

                filtro.TryGetValue("numeroGuia", out numeroGuia);
                filtro.TryGetValue("localidad", out localidad);
                filtro.TryGetValue("fechaInicial", out fechaInicial);
                filtro.TryGetValue("fechaFinal", out fechaFinal);

                if (fechaInicial == null)
                    fechaInicial = ConstantesFramework.MinDateTimeController.ToString();
                if (fechaFinal == null)
                    fechaFinal = DateTime.Now.ToString();
                if (numeroGuia == null)
                    numeroGuia = "0";

                fechaI = Convert.ToDateTime(fechaInicial, cultura);
                fechaF = Convert.ToDateTime(fechaFinal, cultura);

                fechaI = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 00, 00, 00);
                fechaF = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 00);

                var guias = contexto.paObtenerGuiasSupervision_LOI
                  (indicePagina, registrosPorPagina, Convert.ToInt64(numeroGuia), (short)ADEnumEstadoGuia.Entregada, localidad, TAConstantesServicios.SERVICIO_RAPIRADICADO, fechaI, fechaF).ToList();

                if (guias.Any())
                {
                    return guias.ConvertAll(r => new ADTrazaGuia
                    {
                        Ciudad = r.EGT_IdLocalidad,
                        FechaGrabacion = r.EGT_FechaGrabacion,
                        Usuario = r.EGT_CreadoPor,
                        IdAdmision = r.EGT_IdAdminisionMensajeria,
                        IdCiudad = r.EGT_IdLocalidad,
                        IdEstadoGuia = r.EGT_IdEstadoGuia,
                        DescripcionEstadoGuia = ((ADEnumEstadoGuia)r.EGT_IdEstadoGuia).ToString(),
                        Modulo = r.EGT_IdModulo,
                        NumeroGuia = r.EGT_NumeroGuia,
                        Observaciones = r.EGT_Observacion,
                    });
                }
                else
                    return new List<ADTrazaGuia>();
            }
        }

        #endregion Consultas

        #endregion Rapiradicados

        #region Planillas

        #region Consultas

        /// <summary>
        /// Método para obtener planillas
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IList<LIPlanillaDC> ObtenerPlanillas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, ADEnumTipoImpreso tipoImpreso)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string numeroPlanilla;
                string centroServicio;
                string tipoCliente;
                string fechaInicial;
                string fechaFinal;
                DateTime fechaI;
                DateTime fechaF;

                filtro.TryGetValue("PGG_NumeroPlanilla", out numeroPlanilla);
                filtro.TryGetValue("centroServicio", out centroServicio);
                filtro.TryGetValue("tipoCliente", out tipoCliente);
                filtro.TryGetValue("fechaInicial", out fechaInicial);
                filtro.TryGetValue("fechaFinal", out fechaFinal);

                if (fechaInicial == null)
                    fechaInicial = ConstantesFramework.MinDateTimeController.ToString();
                if (fechaFinal == null)
                    fechaFinal = DateTime.Now.ToString();
                if (numeroPlanilla == null)
                    numeroPlanilla = "0";

                fechaI = Convert.ToDateTime(fechaInicial, cultura);
                fechaF = Convert.ToDateTime(fechaFinal, cultura);

                fechaI = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 00, 00, 00);
                fechaF = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 00);

                var planillas = contexto.paObtenerPlanillasRelacion_LOI
                  (indicePagina, registrosPorPagina, Convert.ToInt64(numeroPlanilla), Convert.ToInt64(centroServicio), tipoCliente, fechaI, fechaF, (short)tipoImpreso).ToList();

                if (planillas.Any())
                {
                    return planillas.ConvertAll(r => new LIPlanillaDC
                    {
                        NumeroPlanilla = r.PIG_NumeroPlanilla,
                        TipoCliente = (ADEnumTipoCliente)(Enum.Parse(typeof(ADEnumTipoCliente), r.PIG_TipoCliente)),
                        CentroServicios = new PUCentroServiciosDC
                        {
                            IdCentroServicio = r.PIG_IdCentroServicios,
                            Nombre = r.PIG_NombreCentroSvc
                        },
                        CreadoPor = r.PIG_CreadoPor,
                        FechaGrabacion = r.PIG_FechaGrabacion,
                        NombreTipoCliente = r.NombreTipocliente,
                        EsConsolidado = r.PIG_EsConsolidado,
                        ClienteCredito = new Servicios.ContratoDatos.Clientes.CLClientesDC { Nit = r.PIG_NitCliente },
                        TipoPlanilla = tipoImpreso
                    });
                }
                else
                    return new List<LIPlanillaDC>();
            }
        }

        /// <summary>
        /// Método para obtener las guias de una planilla
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public IList<LIPlanillaDetalleDC> ObtenerGuiasPlanilla(LIPlanillaDC planilla)
        {
            List<LIPlanillaDetalleDC> lstGuiasPlanilla = new List<LIPlanillaDetalleDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerGuiasPlanilla_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroPlanilla", planilla.NumeroPlanilla);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LIPlanillaDetalleDC guiaPlanilla = new LIPlanillaDetalleDC()
                    {
                        NumeroPlanilla = Convert.ToInt64(reader["PGG_NumeroPlanilla"]),
                        AdmisionMensajeria = new ADGuia
                        {
                            NumeroGuia = Convert.ToInt64(reader["PGG_NumeroGuiaAnterior"]),
                            IdAdmision = Convert.ToInt64(reader["PGG_IdAdminisionAnterior"]),
                            Destinatario = new CLClienteContadoDC
                            {
                                Nombre = reader["ADM_NombreDestinatario"].ToString(),
                                Direccion = reader["ADM_DireccionDestinatario"].ToString(),
                                Telefono = reader["ADM_TelefonoDestinatario"].ToString()
                            },
                            Remitente = new CLClienteContadoDC
                            {
                                Nombre = reader["ADM_NombreRemitente"].ToString(),
                                Direccion = reader["ADM_DireccionRemitente"].ToString(),
                                Telefono = reader["ADM_TelefonoRemitente"].ToString()
                            },
                            Peso = Convert.ToDecimal(reader["ADM_Peso"]),
                            ValorAdmision = Convert.ToDecimal(reader["ADM_ValorTotal"]),
                            NombreServicio = reader["FormasPago"].ToString(),
                        },
                        AdmisionMensajeriaNueva = new ADGuia
                        {
                            NumeroGuia = Convert.ToInt64(reader["PGG_NumeroGuiaNueva"]),
                            IdAdmision = Convert.ToInt64(reader["PGG_IdAdmisionNueva"]),
                            Destinatario = new CLClienteContadoDC
                            {
                                Nombre = reader["ADM_NombreDestinatarioNueva"].ToString(),
                                Direccion = reader["ADM_DireccionDestinatarioNueva"].ToString(),
                                Telefono = reader["ADM_TelefonoDestinatarioNueva"].ToString(),
                                Identificacion = reader["ADM_IdDestinatarioNueva"].ToString(),
                                TipoId = reader["ADM_TipoIdentificacionDestinatarioNueva"].ToString()                                
                            },
                            Remitente = new CLClienteContadoDC
                            {
                                Nombre = reader["ADM_NombreRemitenteNueva"].ToString(),
                                Direccion = reader["ADM_DireccionRemitenteNueva"].ToString(),
                                Telefono = reader["ADM_TelefonoRemitenteNueva"].ToString(),
                                Identificacion = reader["ADM_IdRemitenteNueva"].ToString(),
                                TipoId = reader["ADM_TipoIdentificacionRemitenteNueva"].ToString()
                            },
                            Peso = Convert.ToDecimal(reader["ADM_Peso"]),
                            ValorAdmision = Convert.ToDecimal(reader["ADM_ValorTotal"]),
                            IdCiudadDestino = reader["ADM_IdCiudadDestinoNueva"].ToString(),
                            NombreCiudadDestino = reader["ADM_NombreCiudadDestinoNueva"].ToString(),
                            NombreServicio = reader["ADM_NombreServicioNueva"].ToString(),
                            FormasPagoDescripcion = reader["FormasPagoNueva"].ToString(),
                            IdCiudadOrigen = reader["ADM_IdCiudadOrigenNueva"].ToString(),
                            NombreCiudadOrigen = reader["ADM_NombreCiudadOrigenNueva"].ToString(),
                            ValorTotal = Convert.ToDecimal(reader["ADM_ValorTotalNueva"]),
                            ValorAdicionales = Convert.ToDecimal(reader["ADM_ValorAdicionalesNueva"]),
                            ValorPrimaSeguro = Convert.ToDecimal(reader["ADM_ValorPrimaSeguroNueva"]),
                            ValorServicio = Convert.ToDecimal(reader["ADM_ValorAdmisionNueva"]),
                            FechaEstimadaEntrega = Convert.ToDateTime(reader["ADM_FechaEstimadaEntregaNueva"]),
                            FechaAdmision = Convert.ToDateTime(reader["ADM_FechaAdmisionNueva"]),
                            NumeroBolsaSeguridad = reader["ADM_NumeroBolsaSeguridadNueva"].ToString(),
                            PesoLiqMasa = Convert.ToDecimal(reader["ADM_PesoKilosNueva"]),
                            PesoLiqVolumetrico = Convert.ToDecimal(reader["ADM_PesoVolumetricoNueva"]),
                            NumeroPieza = Convert.ToInt16(reader["ADM_NumeroPiezaNueva"]),
                            ValorDeclarado = Convert.ToDecimal(reader["ADM_ValorDeclaradoNueva"]),
                            MotivoNoUsoBolsaSeguriDesc = reader["INFO_Casillero"].ToString() //se trae la informacion del casillero con esta variable ya que no se encontraba en uso
                        },
                        GuiaInterna = new ADGuiaInternaDC
                        {
                            IdAdmisionGuia = Convert.ToInt64(reader["PGG_IdAdmisionNueva"]),
                            NumeroGuia = Convert.ToInt64(reader["PGG_NumeroGuiaNueva"]),
                            DireccionDestinatario = reader["ADM_DireccionDestinatarioNueva"].ToString(),
                            LocalidadDestino = new PALocalidadDC
                            {
                                Nombre = reader["ADM_NombreCiudadDestino"].ToString(),
                                IdLocalidad = reader["ADM_IdCiudadDestino"].ToString()
                            },
                            IdCentroServicioDestino = Convert.ToInt64(reader["ADM_IdCentroServicioDestino"]),
                            NombreDestinatario = reader["ADM_NombreDestinatarioNueva"].ToString(),
                            TelefonoDestinatario = reader["ADM_TelefonoDestinatarioNueva"].ToString(),
                            DireccionRemitente = reader["ADM_DireccionRemitente"].ToString(),
                            LocalidadOrigen = new PALocalidadDC
                            {
                                Nombre = reader["ADM_NombreCiudadDestino"].ToString(),
                                IdLocalidad = reader["ADM_IdCiudadDestino"].ToString()
                            },
                            IdCentroServicioOrigen = Convert.ToInt64(reader["ADM_IdCentroServicioOrigen"]),
                            NombreRemitente = reader["ADM_NombreRemitente"].ToString(),
                            TelefonoRemitente = reader["ADM_TelefonoRemitente"].ToString()
                        },
                        IdPlanillaGuia = Convert.ToInt64(reader["PGG_IdPlanillaRelacionGuiaGuia"]),
                        CreadoPor = reader["PGG_CreadoPor"].ToString(),
                        FechaGrabacion = Convert.ToDateTime(reader["PGG_FechaGrabacion"])
                    };
                    lstGuiasPlanilla.Add(guiaPlanilla);
                }
                conn.Close();
            }
            return lstGuiasPlanilla;
        }

        public IList<LISalidaCustodia> ObtenerSalidasCustodiaPorDia(long idCentroServicio, DateTime fechaConsulta)
        {
            List<LISalidaCustodia> result = new List<LISalidaCustodia>();

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarSalidaCustodias_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCentroServicio", idCentroServicio));
                cmd.Parameters.Add(new SqlParameter("@FechaConsulta", fechaConsulta));
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    LISalidaCustodia salida = new LISalidaCustodia
                    {
                        NumeroGuia = Convert.ToInt64(read["Guia"]),
                        Clasificacion = read["Clasificacion"].ToString(),
                        Pago = read["Pago"].ToString(),
                        Contenido = read["Contenido"].ToString(),
                        Destino = read["Destino"].ToString()
                    };

                    result.Add(salida);
                }
            }

            return result;
        }

        /// <summary>
        /// Método para obtener una planilla a partir del número de planilla
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public LIPlanillaDC ObtenerPlanilla(long numeroPlanilla)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaRelacionGuia_LOI planilla = contexto.PlanillaRelacionGuia_LOI
                    .FirstOrDefault(pla => pla.PIG_NumeroPlanilla == numeroPlanilla);

                if (planilla != null)
                {
                    return new LIPlanillaDC
                    {
                        NumeroPlanilla = planilla.PIG_NumeroPlanilla,
                        TipoCliente = (ADEnumTipoCliente)(Enum.Parse(typeof(ADEnumTipoCliente), planilla.PIG_TipoCliente)),
                        CentroServicios = new PUCentroServiciosDC
                        {
                            IdCentroServicio = planilla.PIG_IdCentroServicios,
                            Nombre = planilla.PIG_NombreCentroSvc
                        },
                        CreadoPor = planilla.PIG_CreadoPor,
                        FechaGrabacion = planilla.PIG_FechaGrabacion,
                        EsConsolidado = planilla.PIG_EsConsolidado,
                        ClienteCredito = new Servicios.ContratoDatos.Clientes.CLClientesDC { Nit = planilla.PIG_NitCliente },
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método para validar que una guía no se encuentre planillada
        /// </summary>
        /// <param name="numeroGuia"></param>
        public void ValidarGuiaPlanilla(long numeroGuia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaRelacionGuiaGuia_LOI guia = contexto.PlanillaRelacionGuiaGuia_LOI
                    .FirstOrDefault(gu => gu.PGG_NumeroGuiaAnterior == numeroGuia);

                if (guia != null)
                    throw new FaultException<ControllerException>(
                    new ControllerException(COConstantesModulos.TELEMERCADEO,
                     LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_YA_DEVUELTA.ToString(),
                     string.Format(LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_YA_DEVUELTA), guia.PGG_NumeroPlanilla.ToString())));
            }
        }

        #endregion Consultas

        #region Inserciones

        /// <summary>
        /// Método para insertar guías en una planilla
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public long AdicionarGuiaPlanilla(LIPlanillaDetalleDC guia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //try
                //{
                var numeroGuiaPlanilla = contexto.PaCrearGuiaPlanillaGuia_LOI(guia.NumeroPlanilla, guia.AdmisionMensajeria.IdAdmision, guia.AdmisionMensajeria.NumeroGuia, guia.AdmisionMensajeriaNueva.IdAdmision, guia.AdmisionMensajeriaNueva.NumeroGuia, DateTime.Now, ControllerContext.Current.Usuario).FirstOrDefault();
                return (long)numeroGuiaPlanilla.idPlanillaGuia;
                // }
                // catch (Exception ex)
                // {
                //     if (ex.InnerException != null && ex.InnerException is System.Data.SqlClient.SqlException)
                //     {
                //         if (((System.Data.SqlClient.SqlException)(ex.InnerException)).Number == 2627)
                //         {
                //             long numeroPlanilla = contexto.PlanillaRelacionGuiaGuia_LOI.Where(pla => pla.PGG_IdAdminisionAnterior == guia.AdmisionMensajeria.IdAdmision).FirstOrDefault().PGG_IdPlanillaRelacionGuiaGuia;

                //         throw new FaultException<ControllerException>(
                //         new ControllerException(COConstantesModulos.TELEMERCADEO,
                //         LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_YA_DEVUELTA.ToString(),
                //         string.Format (LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_YA_DEVUELTA),numeroPlanilla.ToString())));
                //         }
                //     }
                //     throw;
                //}
            }
        }

        /// <summary>
        /// Método para insertar una nueva planilla de guías internas
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public long AdicionarPlanilla(LIPlanillaDC planilla)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PaCrearPlanilla_LOI planillaRetorna = contexto.PaCrearPlanilla_LOI((short?)planilla.TipoPlanilla, planilla.CentroServicios.IdCentroServicio, planilla.CentroServicios.Nombre, planilla.TipoCliente.ToString(), planilla.ClienteCredito.Nit, planilla.EsConsolidado, DateTime.Now, ControllerContext.Current.Usuario).FirstOrDefault();
                return (long)planillaRetorna.idPlanilla;
            }
        }

        #endregion Inserciones

        #region Eliminaciones

        /// <summary>
        /// Método para eliminar una guia de una planilla con auditoria
        /// </summary>
        /// <param name="guia"></param>
        public void EliminarGuiaPLanilla(LIPlanillaDetalleDC guia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarGuiaPlanillaGuia_LOI(guia.IdPlanillaGuia, ControllerContext.Current.Usuario, LOIConstantesLogisticaInversa.ACCION_ELIMINAR);
            }
        }

        #endregion Eliminaciones


        #endregion Planillas


        ///// <summary>
        ///// Metodo para adicionar Telemercadeo de la guia
        ///// </summary>
        ///// <param name="ObtenerInformacionTelemercadeoGuia"></param>
        ///// <returns></returns>
        public List<LIGestionesDC> ObtenerInformacionTelemercadeoGuia(long numeroGuia)
        {
            List<LIGestionesDC> Lista = new List<LIGestionesDC>();

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerInformacionTelemercadeoGuia_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    LIGestionesDC newObjGestionDC = new LIGestionesDC
                    {
                        NumeroGuia = Convert.ToInt64(read["GGT_NumeroGuia"]),
                        IdGestion = Convert.ToInt64(read["GGT_IdGestionGuiaTelemercadeo"]),
                        Telefono = read["GGT_TelefonoMarcado"].ToString(),
                        PersonaContesta = read["GGT_PersonaContesta"].ToString(),
                        FechaGestion = Convert.ToDateTime(read["GGT_FechaGrabacion"]),
                        Observaciones = read["GGT_Observacion"].ToString(),
                        NuevaDireccion = read["GGT_NuevaDireccionEnvio"].ToString(),
                        Resultado = new LIResultadoTelemercadeoDC()
                        {
                            Descripcion = read["EGT_DescripcionEstado"].ToString(),  //Tele
                            Estado = read["RTM_Estado"].ToString(), //act
                            Ciudad = read["NombreCompleto"].ToString()
                        }
                    };
                    Lista.Add(newObjGestionDC);
                }
            }

            return Lista;
        }


        /// <summary>
        /// Consulta la informacion de flujo de la Guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIFlujoGuiaDC> ObtenerFlujoGuia(long numeroGuia)
        {
            List<LIFlujoGuiaDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerFlujoGuia_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (resultado == null)
                        {
                            resultado = new List<LIFlujoGuiaDC>();
                        }

                        resultado.Add(new LIFlujoGuiaDC
                        {
                            IdLocalidad = Convert.ToString(reader["EGT_IdLocalidad"]),
                            Ciudad = Convert.ToString(reader["EGT_NombreLocalidad"]),
                            FechaEstado = Convert.ToDateTime(reader["EGT_FechaGrabacion"]),
                            Usuario = Convert.ToString(reader["EGT_CreadoPor"]),
                            IdEstado = Convert.ToInt64(reader["EGT_IdEstadoGuia"]),
                            DescripcionEstado = Convert.ToString(reader["EGT_DEscripcionEstado"]),
                            NumeroGuia = Convert.ToInt64(reader["EGT_NumeroGuia"]),
                            IdEstadoLog = Convert.ToInt64(reader["EGT_IdEstadoGuiaLog"]),

                        });
                    }
                }
            }
            return resultado;
        }

        /// <summary>
        /// Consulta la informacion de la admision en el flujo de la quia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIAdmisionGuiaFlujoDC ObtenerAdmisionGuiaFlujo(long numeroGuia)
        {
            LIAdmisionGuiaFlujoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAdmisionGuiaFlujo_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        resultado = new LIAdmisionGuiaFlujoDC
                        {
                            NumeroGuia = Convert.ToInt64(reader["ADM_NumeroGuia"]),
                            TipoCliente = Convert.ToString(reader["ADM_TipoCliente"]),
                            Creadopor = Convert.ToString(reader["ADM_Creadopor"]),
                            EsAutomatico = Convert.ToBoolean(reader["ADM_EsAutomatico"]),
                            DiasDeEntrega = Convert.ToInt16(reader["ADM_DiasDeEntrega"]),
                            NombreCentroServicioOrigen = Convert.ToString(reader["ADM_NombreCentroServicioOrigen"]),
                            NombreCentroServicioDestino = Convert.ToString(reader["ADM_NombreCentroServicioDestino"]),
                            NombreAplicacion = Convert.ToString(reader["AA_NombreAplicacion"]),
                        };

                        if (!DBNull.Value.Equals(reader["ADM_FechaEstimadaEntregaNew"]))
                        {
                            resultado.FechaEstimadaEntregaNew = Convert.ToDateTime(reader["ADM_FechaEstimadaEntregaNew"]);
                        }

                        if (!DBNull.Value.Equals(reader["ADM_fechaPago"]))
                        {
                            resultado.fechaPago = Convert.ToDateTime(reader["ADM_fechaPago"]);
                        }

                    }

                }

            }

            return resultado;
        }

        /// <summary>
        /// Consultaa la informacion de los ingresos al centro de acopio nacional
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIIngresoCentroAcopioNacionalDC> ObtenerIngresoAcopioNacional(long numeroGuia)
        {
            List<LIIngresoCentroAcopioNacionalDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paRptObtIngresoAcopioNacionalExp_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroDeGuia", numeroGuia);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var r = new LIIngresoCentroAcopioNacionalDC
                        {
                            NumeroGuia = Convert.ToInt64(reader["NumeroGuia"]),
                            CiudadDeIngreso = Convert.ToString(reader["CiudadDeIngreso"]),
                            Ruta = Convert.ToString(reader["Ruta"]),
                            PlacaVehiculo = Convert.ToString(reader["PlacaVehiculo"]),
                            NombreDelConductor = Convert.ToString(reader["NombreDelConductor"]),
                            Novedad = Convert.ToString(reader["Novedad"]),
                        };

                        if (!DBNull.Value.Equals(reader["FechaDeIngreso"]))
                        {
                            r.FechaDeIngreso = Convert.ToDateTime(reader["FechaDeIngreso"]);
                        }

                        if (resultado == null)
                        {
                            resultado = new List<LIIngresoCentroAcopioNacionalDC>();
                        }
                        resultado.Add(r);
                    }

                }

            }

            return resultado;
        }

        /// <summary>
        /// Consulta la informacion del ingreos a centro de acopio urbano
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIIngresoCentroAcopioUrbanoDC> ObtenerIngresoCentroAcopioUrbano(long numeroGuia)
        {
            List<LIIngresoCentroAcopioUrbanoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paRptObtIngresoAcopioUrbanoExp_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroDeGuia", numeroGuia);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var r = new LIIngresoCentroAcopioUrbanoDC
                        {
                            NumeroGuia = Convert.ToInt64(reader["NumeroGuia"]),
                            NumeroPlanilla = Convert.ToInt64(reader["NumeroPlanilla"]),
                            EstadoEmpaque = Convert.ToString(reader["EstadoEmpaque"]),
                            IngresoPlanilla = Convert.ToInt64(reader["IngresoPlanilla"]),
                            NombreCiudad = Convert.ToString(reader["NombreCiudad"]),
                            CedulaMensajero = Convert.ToString(reader["CedulaMensajero"]),
                            NombreMensajero = Convert.ToString(reader["NombreMensajero"]),
                            UsuarioIngreso = Convert.ToString(reader["UsuarioIngreso"]),
                        };

                        if (!DBNull.Value.Equals(reader["FechaIngreso"]))
                        {
                            r.FechaIngreso = Convert.ToDateTime(reader["FechaIngreso"]);
                        }

                        if (resultado == null)
                        {
                            resultado = new List<LIIngresoCentroAcopioUrbanoDC>();
                        }
                        resultado.Add(r);
                    }

                }

            }

            return resultado;
        }

        /// <summary>
        /// Consulta de asignaciones a mensajero para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIAsignacionMensajero> ObtenerAsignacionMensajero(long numeroGuia)
        {
            List<LIAsignacionMensajero> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAsignacionMensajeroCOL_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var r = new LIAsignacionMensajero
                        {
                            EstadoGuia = Convert.ToString(reader["EstadoGuia"]),
                            PlanillaAsignacion = Convert.ToInt64(reader["PlanillaAsignacion"]),
                            CedulaMensajero = Convert.ToString(reader["CedulaMensajero"]),
                            UsuarioDescargue = Convert.ToString(reader["UsuarioDescargue"]),
                            NombreMensajero = Convert.ToString(reader["NombreMensajero"]),
                            UsuarioAsigna = Convert.ToString(reader["UsuarioAsigna"]),
                            NumeroGuia = Convert.ToInt64(reader["NumeroGuia"]),
                            EstaDescargada = Convert.ToBoolean(reader["EstaDescargada"]),
                        };
                        if (!DBNull.Value.Equals(reader["MotivoGuia"]))
                        {
                            r.MotivoGuia = Convert.ToString(reader["MotivoGuia"]);
                        }
                        if (!DBNull.Value.Equals(reader["NumeroAuditoria"]))
                        {
                            r.NumeroAuditoria = Convert.ToInt64(reader["NumeroAuditoria"]);
                        }

                        if (!DBNull.Value.Equals(reader["Auditor"]))
                        {
                            r.Auditor = Convert.ToString(reader["Auditor"]);
                        }

                        if (!DBNull.Value.Equals(reader["FechaDescargue"]))
                        {
                            r.FechaDescargue = Convert.ToDateTime(reader["FechaDescargue"]);
                        }

                        if (!DBNull.Value.Equals(reader["FechaAsigna"]))
                        {
                            r.FechaAsigna = Convert.ToDateTime(reader["FechaAsigna"]);
                        }

                        if (!DBNull.Value.Equals(reader["FechaAuditoria"]))
                        {
                            r.FechaAuditoria = Convert.ToDateTime(reader["FechaAuditoria"]);
                        }

                        if (resultado == null)
                        {
                            resultado = new List<LIAsignacionMensajero>();
                        }
                        resultado.Add(r);
                    }

                }

            }

            return resultado;
        }

        /// <summary>
        /// Consulta la informacion del manifiestó para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIManifiestoMercadeoDC> ObtenerManifiesto(long numeroGuia)
        {
            List<LIManifiestoMercadeoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerManifiestoCOL_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var r = new LIManifiestoMercadeoDC
                        {

                            NumeroGuia = Convert.ToInt64(reader["NumeroGuia"]),
                            EstaDescargada = Convert.ToBoolean(reader["EstaDescargada"]),

                            UsuarioManifesto = Convert.ToString(reader["UsuarioManifesto"]),
                            IdManfiestoOperaNacioConso = Convert.ToInt64(reader["IdManfiestoOperaNacioConso"]),
                            IdAdminisionMensajeria = Convert.ToInt64(reader["IdAdminisionMensajeria"]),
                            IdManifiestoOperacionNacio = Convert.ToInt64(reader["IdManifiestoOperacionNacio"]),
                            LocalidadManifestada = Convert.ToString(reader["LocalidadManifestada"]),
                            TipoConsolidadoDetalle = Convert.ToInt16(reader["TipoConsolidadoDetalle"]),
                            DescpConsolidadoDetalle = Convert.ToString(reader["DescpConsolidadoDetalle"]),
                            NumeroContenedorTula = Convert.ToString(reader["NumeroContenedorTula"]),


                            NombreLocalidadDespacho = Convert.ToString(reader["MON_NombreLocalidadDespacho"]),
                            MON_IdManifiestoOperacionNacional = Convert.ToInt64(reader["MON_IdManifiestoOperacionNacio"]),

                            EstaDescargado = Convert.ToBoolean(reader["MON_EstaDescargado"]),

                            NumeroManifiestoCarga = Convert.ToInt64(reader["MON_NumeroManifiestoCarga"]),
                            IdEmpresaTransportadora = Convert.ToInt16(reader["MON_IdEmpresaTransportadora"]),
                            ETR_Nombre = Convert.ToString(reader["ETR_Nombre"]),
                            RUT_Nombre = Convert.ToString(reader["RUT_Nombre"]),


                        };
                        if (!DBNull.Value.Equals(reader["FechaGrabacion"]))
                        {
                            r.FechaGrabacion = Convert.ToDateTime(reader["FechaGrabacion"]);
                        }
                        if (!DBNull.Value.Equals(reader["MON_FechaDescargue"]))
                        {
                            r.FechaDescargue = Convert.ToDateTime(reader["MON_FechaDescargue"]);
                        }

                        if (!DBNull.Value.Equals(reader["GuiaInterna"]))
                        {
                            r.GuiaInterna = Convert.ToInt64(reader["GuiaInterna"]);
                        }

                        if (!DBNull.Value.Equals(reader["IdGuiaInterna"]))
                        {
                            r.IdGuiaInterna = Convert.ToInt64(reader["IdGuiaInterna"]);
                        }

                        if (!DBNull.Value.Equals(reader["NumeroPrecintoSalida"]))
                        {
                            r.NumeroPrecintoSalida = Convert.ToInt64(reader["NumeroPrecintoSalida"]);
                        }

                        if (!DBNull.Value.Equals(reader["MOT_Placa"]))
                        {
                            r.Placa = Convert.ToString(reader["MOT_Placa"]);
                        }

                        if (!DBNull.Value.Equals(reader["MOT_NombreConductor"]))
                        {
                            r.NombreConductor = Convert.ToString(reader["MOT_NombreConductor"]);
                        }

                        if (!DBNull.Value.Equals(reader["MOT_IdManifiestoOperacionNacio"]))
                        {
                            r.MOT_IdManifiestoOperacionNacio = Convert.ToInt64(reader["MOT_IdManifiestoOperacionNacio"]);
                        }

                        if (resultado == null)
                        {
                            resultado = new List<LIManifiestoMercadeoDC>();
                        }
                        resultado.Add(r);
                    }

                }

            }

            return resultado;
        }

        /// <summary>
        /// Consulta la informacion del archivo de la prueba de entrega
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIArchivoEntregaDC ObtenerArchivoPruebaEntrega(long numeroGuia)
        {
            LIArchivoEntregaDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paArchivoPruebaEntrega_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PAR_NumeroDeGuia", numeroGuia);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        resultado = new LIArchivoEntregaDC
                        {
                            NumeroGuia = Convert.ToInt64(reader["ARG_NumeroGuia"]),
                            Caja = Convert.ToInt64(reader["ARG_Caja"]),
                            Lote = Convert.ToInt64(reader["ARG_Lote"]),
                            Posicion = Convert.ToInt64(reader["ARG_Posicion"]),
                            UsuarioArchiva = Convert.ToString(reader["ARG_UsuarioArchiva"]),
                            DatosdeEdicion = Convert.ToString(reader["DatosdeEdicion"]),
                            DatosdeEntrega = Convert.ToString(reader["DatosdeEntrega"]),
                            EstadoFisico = Convert.ToString(reader["EstadoFisico"]),
                            CreadoPor = Convert.ToString(reader["ARG_CreadoPor"]),
                        };
                        if (!DBNull.Value.Equals(reader["ARG_FechaEntrega"]))
                        {
                            resultado.FechaEntrega = Convert.ToDateTime(reader["ARG_FechaEntrega"]);
                        }

                        if (!DBNull.Value.Equals(reader["ARG_FechaGrabacion"]))
                        {
                            resultado.FechaGrabacion = Convert.ToDateTime(reader["ARG_FechaGrabacion"]);
                        }

                        if (!DBNull.Value.Equals(reader["CES_IdMunicipio"]))
                        {
                            resultado.IdMunicipio = Convert.ToString(reader["CES_IdMunicipio"]);
                        }

                        if (!DBNull.Value.Equals(reader["LOC_Nombre"]))
                        {
                            resultado.Nombre = Convert.ToString(reader["LOC_Nombre"]);
                        }

                        if (!DBNull.Value.Equals(reader["ARG_FechaArchivo"]))
                        {
                            resultado.FechaArchivo = Convert.ToDateTime(reader["ARG_FechaArchivo"]);
                        }
                    }
                }
            }

            return resultado;
        }


        public LIDetalleTelemercadeoDC ObtenerUltimaGestionTelemercadeoGuia(long numeroGuia)
        {
            LIDetalleTelemercadeoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("PaObtenerUltimaGestionTelemercadeoGuia_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                cmd.CommandTimeout = 600;
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        resultado = new LIDetalleTelemercadeoDC
                        {
                            IdGestionGuiaTelemercadeo = Convert.ToInt64(reader["IdGestionGuiaTelemercadeo"]),
                            DescripcionTelemercadeo = reader["DescripcionTelemercadeo"].ToString(),
                            TelefonoMarcado = reader["TelefonoMarcado"].ToString(),
                            PersonaContesta = reader["PersonaContesta"].ToString(),
                            IdResultadoTelemercadeo = Convert.ToInt32(reader["IdResultadoTelemercadeo"]),
                            FechaGrabacion = Convert.ToDateTime(reader["FechaGrabacion"]),
                            Observacion = reader["Observacion"].ToString(),
                            NuevaDireccionEnvio = reader["NuevaDireccionEnvio"].ToString(),
                            IdParentestoConDestinatari = Convert.ToInt32(reader["IdParentestoConDestinatario"]),
                            Parentesco = reader["Parentesco"].ToString(),
                            NuevoTelefono = reader["NuevoTelefono"].ToString(),
                            NuevoContaco = reader["NuevoContaco"].ToString(),
                            IdEstadoGuiaLog = Convert.ToInt64(reader["IdEstadoGuiaLog"]),
                            DescripcionEstado = reader["DescripcionEstado"].ToString(),
                            CreadoPor = reader["CreadoPor"].ToString()
                        };
                    }
                }
            }
            return resultado;
        }

        /// <summary>
        /// Retorna las guías asociadas a la localidad pasada en el estado solicitado
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="idCentroServicio"></param>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGestionesGuiasTelemercadeoCol(short idEstado, long idCentroServicio, long numeroGuia)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGestionesGuiasTelemercadeoCOL_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@estadoGuia", idEstado));
                cmd.Parameters.Add(new SqlParameter("@idcentroServicio", idCentroServicio));
                cmd.CommandTimeout = 600;
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var r = ds.Tables[0].Rows[0];
                    var result = new LIGestionesGuiaDC
                    {
                        idTrazaGuia = Convert.ToInt64(r["ADM_IdEstadoGuiaLog"]),
                        Motivo = new ADMotivoGuiaDC
                        {
                            IdMotivoGuia = Convert.ToInt16(r["MOG_IdMotivoGuia"]),
                            Descripcion = Convert.ToString(r["MOG_Descripcion"]),
                            Tipo = ADEnumTipoMotivoDC.Devolucion,
                        },
                        ArchivosAdjuntos = new List<LIEvidenciaDevolucionDC>(),
                        DiasTelemercadeo = r["DiasTelemercadeo"] == null ? 0 : Convert.ToInt32(r["DiasTelemercadeo"]),
                        GuiaAdmision = new ADGuia
                        {
                            IdAdmision = Convert.ToInt64(r["ADM_IdAdminisionMensajeria"]),
                            NumeroGuia = Convert.ToInt64(r["ADM_NumeroGuia"]),
                            IdTipoEntrega = Convert.ToString(r["ADM_IdTipoEntrega"]),
                            DescripcionTipoEntrega = Convert.ToString(r["ADM_DescripcionTipoEntrega"]),
                            Destinatario = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                            {
                                Identificacion = Convert.ToString(r["ADM_IdDestinatario"]),
                                Nombre = Convert.ToString(r["ADM_NombreDestinatario"]),
                                Direccion = Convert.ToString(r["ADM_DireccionDestinatario"]),
                                Telefono = Convert.ToString(r["ADM_TelefonoDestinatario"]),
                                TipoId = Convert.ToString(r["ADM_IdTipoIdentificacionDestinatario"]),
                                Email = Convert.ToString(r["ADM_EmailDestinatario"]),
                            },
                            Remitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                            {
                                Identificacion = Convert.ToString(r["ADM_IdRemitente"]),
                                Nombre = Convert.ToString(r["ADM_NombreRemitente"]),
                                Direccion = Convert.ToString(r["ADM_DireccionRemitente"]),
                                Telefono = Convert.ToString(r["ADM_TelefonoRemitente"]),
                                Email = Convert.ToString(r["ADM_EmailRemitente"]),
                                TipoId = Convert.ToString(r["ADM_IdTipoIdentificacionRemitente"]),
                            },
                            IdCiudadDestino = Convert.ToString(r["ADM_IdCiudadDestino"]),
                            NombreCiudadOrigen = Convert.ToString(r["ADM_NombreCiudadOrigen"]),
                            NombreCiudadDestino = Convert.ToString(r["ADM_NombreCiudadDestino"]),
                            NombreCentroServicioOrigen = Convert.ToString(r["ADM_NombreCentroServicioOrigen"]),
                            NombreCentroServicioDestino = Convert.ToString(r["ADM_NombreCentroServicioDestino"]),
                            TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), Convert.ToString(r["ADM_TipoCliente"])),
                            EstadoGuia = ADEnumEstadoGuia.Telemercadeo,
                            NombreServicio = Convert.ToString(r["ADM_NombreServicio"]),
                            FechaGrabacion = Convert.ToDateTime(r["ADM_FechaGrabacion"]),
                            FormasPago = new List<ADGuiaFormaPago>
                                                {
                                                    new ADGuiaFormaPago {                                                        
                                                    }
                                                },

                            ValorTotal = Convert.ToInt64(r["ADM_ValorTotal"]),
                            DiceContener = Convert.ToString(r["ADM_DiceContener"]),
                            Peso = Convert.ToDecimal(r["ADM_Peso"]),
                            ValorDeclarado = Convert.ToInt64(r["ADM_ValorDeclarado"]),
                            NumeroPieza = Convert.ToInt16(r["ADM_NumeroPieza"]),
                            FechaEstimadaEntrega = Convert.ToDateTime(r["ADM_FechaGrabacionEstado"]),
                            IdTipoEnvio = Convert.ToInt16(r["ADM_idTipoEnvio"]),
                            NombreTipoEnvio = Convert.ToString(r["TEN_nombre"]),
                            Alto = Convert.ToInt16(r["ADM_Alto"]),
                            Ancho = Convert.ToInt16(r["ADM_Ancho"]),
                            Largo = Convert.ToInt16(r["ADM_Largo"]),
                            EsPesoVolumetrico = Convert.ToBoolean(r["ADM_EsPesoVolumetrico"]),
                            PesoLiqVolumetrico = Convert.ToInt16(r["ADM_PesoLiqVolumetrico"]),
                            EsRecomendado = Convert.ToBoolean(r["ADM_EsRecomendado"]),
                            TotalPiezas = Convert.ToInt16(r["ADM_TotalPiezas"]),
                            NumeroBolsaSeguridad = Convert.ToString(r["ADM_NumeroBolsaSeguridad"]),
                            Observaciones = Convert.ToString(r["ADM_Observaciones"]),
                            Supervision = Convert.ToBoolean(r["ADM_EsSupervisada"]),
                            ValorAdmision = Convert.ToInt64(r["ADM_ValorAdmision"]),
                            ValorPrimaSeguro = Convert.ToInt64(r["ADM_ValorPrimaSeguro"]),
                            FechaAdmision = Convert.ToDateTime(r["ADM_FechaAdmision"]),
                            CreadoPor = Convert.ToString(r["ADM_CreadoPor"]),
                            ObservacionEstadoGuia = Convert.ToString(r["ADM_DescripcionEstado"]),

                        },
                        Gestiones = 0,
                    };
                    if (!DBNull.Value.Equals(r["AGF_IdFormaPago"]))
                    {
                        result.GuiaAdmision.FormasPago[0].IdFormaPago = Convert.ToInt16(r["AGF_IdFormaPago"]);
                    }

                    if (!DBNull.Value.Equals(r["FOP_Descripcion"]))
                    {
                        result.GuiaAdmision.FormasPago[0].Descripcion = Convert.ToString(r["FOP_Descripcion"]);
                        result.GuiaAdmision.FormasPagoDescripcion = Convert.ToString(r["FOP_Descripcion"]);
                    }

                    return result;
                }
                else
                    return new LIGestionesGuiaDC();
            }
        }

        /// <summary>
        /// Consulta La gestion realizada por el usuario conectado el dia de hoy
        /// </summary>
        /// <returns>objeto estadistica telemercadeo</returns>
        public LIEstadisticaTelemercadeoDC ObtenerEstadisticaGestion()
        {
            DataSet ds = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerConteoGestionUsuarioTelemercadeoCOL_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@UserName", ControllerContext.Current.Usuario));
                cmd.CommandTimeout = 600;
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    return MapperEstadisticaGestion(resultReader);
                }
                //if (ds.Tables[0].Rows.Count > 0)
                //{
                //    var r = ds.Tables[0].Rows[0];
                //    return new LIEstadisticaTelemercadeoDC{

                //    }
                //}
                return null;
            }
        }

        private LIEstadisticaTelemercadeoDC MapperEstadisticaGestion(SqlDataReader reader)
        {
            LIEstadisticaTelemercadeoDC resultado = null;
            while (reader.Read())
            {
                if (resultado == null) { resultado = new LIEstadisticaTelemercadeoDC(); }
                switch (Convert.ToString(reader["Gestion"]))
                {
                    case "Auditoria":
                        resultado.Auditoria = Convert.ToInt64(reader["Cantidad"]);
                        break;
                    case "Custodia":
                        resultado.Custodia = Convert.ToInt64(reader["Cantidad"]);
                        break;
                    case "DevolucionRatificada":
                        resultado.DevolucionRatificada = Convert.ToInt64(reader["Cantidad"]);
                        break;
                    case "NuevaDireccion":
                        resultado.NuevaDireccion = Convert.ToInt64(reader["Cantidad"]);
                        break;
                    case "ReclameOficina":
                        resultado.ReclameOficina = Convert.ToInt64(reader["Cantidad"]);
                        break;
                    case "Reenvio":
                        resultado.Reenvio = Convert.ToInt64(reader["Cantidad"]);
                        break;
                    case "Telemercadeo":
                        resultado.Telemercadeo = Convert.ToInt64(reader["Cantidad"]);                    
                        break;
                    case "DevolverALaRacol":
                        resultado.DevolverALaRacol = Convert.ToInt64(reader["Cantidad"]);
                        break;
                    default:
                        break;
                }
            }
            return resultado;
        }

        /// <summary>
        /// Obtener detalle de Telemercadeo al observar el flujo de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIDetalleTelemercadeoDC> ObtenerDetalleTelemercadeoGuia(long numeroGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("PaObtenerDetalleGestionTelemercado_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Numeroguia", numeroGuia));
                cmd.CommandTimeout = 600;
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    return MapperDetalleTelemercadeo(resultReader);
                }

                return null;
            }
        }

        private List<LIDetalleTelemercadeoDC> MapperDetalleTelemercadeo(SqlDataReader reader)
        {
            List<LIDetalleTelemercadeoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<LIDetalleTelemercadeoDC>();
                }
                var r = new LIDetalleTelemercadeoDC
                {
                    IdGestionGuiaTelemercadeo = Convert.ToInt64(reader["IdGestionGuiaTelemercadeo"]),
                    DescripcionTelemercadeo = Convert.ToString(reader["DescripcionTelemercadeo"]),
                    TelefonoMarcado = Convert.ToString(reader["TelefonoMarcado"]),
                    PersonaContesta = Convert.ToString(reader["PersonaContesta"]),
                    IdResultadoTelemercadeo = Convert.ToInt32(reader["IdResultadoTelemercadeo"]),
                    Observacion = Convert.ToString(reader["Observacion"]),
                    NuevaDireccionEnvio = Convert.ToString(reader["NuevaDireccionEnvio"]),
                    IdParentestoConDestinatari = Convert.ToInt32(reader["IdParentestoConDestinatario"]),
                    Parentesco = Convert.ToString(reader["Parentesco"]),
                    NuevoTelefono = Convert.ToString(reader["NuevoTelefono"]),
                    NuevoContaco = Convert.ToString(reader["NuevoContaco"]),
                    IdEstadoGuiaLog = Convert.ToInt64(reader["IdEstadoGuiaLog"]),
                    DescripcionEstado = Convert.ToString(reader["DescripcionEstado"]),
                    CreadoPor = Convert.ToString(reader["CreadoPor"]),

                };
                if (!DBNull.Value.Equals(reader["FechaGrabacion"]))
                {
                    r.FechaGrabacion = Convert.ToDateTime(reader["FechaGrabacion"]);
                }

                resultado.Add(r);
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene el detalle de los motivos de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIDetalleMotivoGuiaDC> ObtenerDetalleMotivoGuia(long numeroGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMotivosGuia_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                cmd.CommandTimeout = 600;
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    return MapperDetalleMotivoGuia(resultReader);
                }

                return null;
            }
        }

        private List<LIDetalleMotivoGuiaDC> MapperDetalleMotivoGuia(SqlDataReader reader)
        {
            List<LIDetalleMotivoGuiaDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<LIDetalleMotivoGuiaDC>();
                }
                var r = new LIDetalleMotivoGuiaDC
                {
                    IdMotivoGuia = Convert.ToInt32(reader["IdMotivoGuia"]),
                    Motivoguia = Convert.ToString(reader["Motivoguia"]),
                    Observacion = Convert.ToString(reader["Observacion"]),
                    CreadoPor = Convert.ToString(reader["CreadoPor"]),
                };
                if (!DBNull.Value.Equals(reader["FechaGrabacion"]))
                {
                    r.FechaGrabacion = Convert.ToDateTime(reader["FechaGrabacion"]);
                }

                resultado.Add(r);
            }
            return resultado;
        }

        /// <summary>
        /// Consulta el historial de entregas de una direccion para una localidad
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<LIHistorialEntregaDC> ObtenerHistorialEntregas(string direccion, string idLocalidad)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paBuscarHistorialentregas_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Direccion", direccion));
                cmd.Parameters.Add(new SqlParameter("@EsDestinatario", 1));
                cmd.Parameters.Add(new SqlParameter("@Localidad", idLocalidad));
                cmd.CommandTimeout = 600;
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    return MapperHistorialEntregas(resultReader);
                }

                return null;
            }
        }

        private List<LIHistorialEntregaDC> MapperHistorialEntregas(SqlDataReader reader)
        {
            List<LIHistorialEntregaDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<LIHistorialEntregaDC>();
                }
                var r = new LIHistorialEntregaDC
                {
                    IdDireccion = Convert.ToInt64(reader["IdDireccion"]),
                    Direccion = Convert.ToString(reader["Direccion"]),
                    Idlocalidad = Convert.ToString(reader["Idlocalidad"]),
                    EsVerificada = Convert.ToBoolean(reader["EsVerificada"]),
                    CiudadDestino = Convert.ToString(reader["CiudadDestino"]),
                    NumeroGuia = Convert.ToInt64(reader["NumeroGuia"]),
                    Estado = Convert.ToString(reader["Estado"]),                    
                };
                if (!DBNull.Value.Equals(reader["FechaEntrega"]))
                {
                    r.FechaEntrega = Convert.ToDateTime(reader["FechaEntrega"]);
                }

                resultado.Add(r);
            }

            return resultado;
        }

        /// <summary>
        /// Consulta las reclamaciondes de la guia
        /// </summary>
        /// <param name="numeroGuia">Guia a consultar</param>
        /// <returns>Lista de reclamaciones</returns>
        public List<LIReclamacionesGuiaDC> ObtenerReclamacionesGuia(long numeroGuia)
        {
             using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerReclamacionesGuia_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));                
                cmd.CommandTimeout = 600;
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    return MapperReclamacionesGuia(resultReader);
                }

                return null;
            }
        }

        private List<LIReclamacionesGuiaDC> MapperReclamacionesGuia(SqlDataReader reader)
        {
            List<LIReclamacionesGuiaDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<LIReclamacionesGuiaDC>();
                }
                var r = new LIReclamacionesGuiaDC
                {
                    idsolicitud = Convert.ToInt64(reader["idsolicitud"]),
                    IdMedioRecepSolicitud = Convert.ToInt16(reader["IdMedioRecepSolicitud"]),
                    MedioRecepSolicitud = Convert.ToString(reader["MedioRecepSolicitud"]),
                    IdTipoSolicitud = Convert.ToInt16(reader["IdTipoSolicitud"]),
                    TipoSolicitud = Convert.ToString(reader["TipoSolicitud"]),
                    IdSubtipoSolicitud = Convert.ToInt16(reader["IdSubtipoSolicitud"]),
                    SubtipoSolicitud = Convert.ToString(reader["SubtipoSolicitud"]),
                    Reclamante = Convert.ToString(reader["Reclamante"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),

                };                

                resultado.Add(r);
            }

            return resultado;
        }

        public int ObtenerNumeroDeEnviosEnUbicacion(int tipoUbicacion, int ubicacion)
        {
            int resultadoConteo = 0;
            using(SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCantidadDeEnviosPorUbidcacion_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tipoUbicacion", tipoUbicacion);
                cmd.Parameters.AddWithValue("@ubicacion", ubicacion);
                conn.Open();
                var conteo = cmd.ExecuteScalar();

                if (conteo != null)
                {
                    resultadoConteo = (Convert.ToInt32(conteo));
                }  
                conn.Close();
            }
            return resultadoConteo;
        }
    }
}