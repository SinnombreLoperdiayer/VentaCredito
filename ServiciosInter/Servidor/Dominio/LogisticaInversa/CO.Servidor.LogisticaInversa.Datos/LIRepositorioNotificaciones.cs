
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Datos;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Transactions;

namespace CO.Servidor.LogisticaInversa.Datos
{
    public class LIRepositorioNotificaciones
    {
        #region Atributos

        private static readonly LIRepositorioNotificaciones instancia = new LIRepositorioNotificaciones();
        private const string NombreModelo = "ModeloLogisticaInversa";
        private string filePath = string.Empty;
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private short entrega;
        private short devolucion;
        private string imagen;

        #endregion Atributos




        /// <summary>
        /// Retorna la instancia de la clase LIRepositorioPruebasEntrega
        /// </summary>
        public static LIRepositorioNotificaciones Instancia
        {
            get { return LIRepositorioNotificaciones.instancia; }
        }


        #region Métodos

        /// <summary>
        /// Indica si el recibido de la guía ha sido registrado previamente
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool RecibidoRegistrado(long numeroGuia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RecibidoGuia_LOI.FirstOrDefault(r => r.REG_NumeroGuia == numeroGuia) != null;
            }
        }

        /// <summary>
        /// Registra recibido de guía manual
        /// </summary>
        /// <param name="recibido"></param>
        public void RegistrarRecibidoGuiaManual(LIRecibidoGuia recibido)
        {
            //using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.RecibidoGuia_LOI.Add(new RecibidoGuia_LOI
            //    {
            //        REG_CreadoPor = ControllerContext.Current.Usuario,
            //        REG_FechaEntrega = recibido.FechaEntrega,
            //        REG_FechaGrabacion = DateTime.Now,
            //        REG_IdAdminisionMensajeria = recibido.IdGuia,
            //        REG_Identificacion = recibido.Identificacion,
            //        REG_NumeroGuia = recibido.NumeroGuia,
            //        REG_OtrosDatos = recibido.Otros,
            //        REG_RecibidoPor = recibido.RecibidoPor,
            //        REG_Telefono = recibido.Telefono                    
            //    });
            //    contexto.SaveChanges();
            //}            
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paRegistrarRecibidoGuiaManual_LOI", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@REG_CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@REG_FechaEntrega", recibido.FechaEntrega);
                cmd.Parameters.AddWithValue("@REG_FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@REG_IdAdminisionMensajeria", recibido.IdGuia);
                cmd.Parameters.AddWithValue("@REG_Identificacion", recibido.Identificacion);
                cmd.Parameters.AddWithValue("@REG_NumeroGuia", recibido.NumeroGuia);
                cmd.Parameters.AddWithValue("@REG_OtrosDatos", recibido.Otros);
                cmd.Parameters.AddWithValue("@REG_RecibidoPor", recibido.RecibidoPor);
                cmd.Parameters.AddWithValue("@REG_Telefono", recibido.Telefono);
                cmd.Parameters.AddWithValue("@REG_Verificado", recibido.Verificado);
                //cmd.Parameters.AddWithValue("@REG_FechaVerificacion", recibido.Telefono);
                cmd.Parameters.AddWithValue("@REG_IdAplicacionOrigen", recibido.IdAplicacionOrigen.GetHashCode());
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

        }

        public void ModificarRecibidoGuiaManual(LIRecibidoGuia recibido)
        {
            //using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    RecibidoGuia_LOI rec = contexto.RecibidoGuia_LOI.Where(rc => rc.REG_NumeroGuia == recibido.NumeroGuia).FirstOrDefault();
            //    if (rec != null)
            //    {
            //        rec.REG_FechaEntrega = recibido.FechaEntrega;
            //        rec.REG_IdAdminisionMensajeria = recibido.IdGuia;
            //        rec.REG_Identificacion = recibido.Identificacion;
            //        rec.REG_NumeroGuia = recibido.NumeroGuia;
            //        rec.REG_OtrosDatos = recibido.Otros;
            //        rec.REG_RecibidoPor = recibido.RecibidoPor;
            //        rec.REG_Telefono = recibido.Telefono;
            //        contexto.SaveChanges();
            //    }
            //}
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paModificarRecibidoGuiaManual_LOI", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@REG_FechaEntrega", recibido.FechaEntrega);
                cmd.Parameters.AddWithValue("@REG_IdAdminisionMensajeria", recibido.IdGuia);
                cmd.Parameters.AddWithValue("@REG_Identificacion", recibido.Identificacion);
                cmd.Parameters.AddWithValue("@REG_NumeroGuia", recibido.NumeroGuia);
                cmd.Parameters.AddWithValue("@REG_OtrosDatos", recibido.Otros);
                cmd.Parameters.AddWithValue("@REG_RecibidoPor", recibido.RecibidoPor);
                cmd.Parameters.AddWithValue("@REG_Telefono", recibido.Telefono);
                cmd.Parameters.AddWithValue("@REG_Verificado", recibido.Verificado);
                cmd.Parameters.AddWithValue("@REG_IdAplicacionOrigen", recibido.IdAplicacionOrigen.GetHashCode());
                cmd.Parameters.AddWithValue("@REG_FechaVerificacion", recibido.FechaVerificacion);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Método para validar si una guia ya tiene un recibido
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ValidarRecibidoGuia(long numeroGuia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RecibidoGuia_LOI rec = contexto.RecibidoGuia_LOI.FirstOrDefault(rc => rc.REG_NumeroGuia == numeroGuia);
                if (rec == null)
                    return true;
                else
                    return false;
            }
        }

        #region Planilla Certificaciones

        /// <summary>
        /// Obtener planillas de certificacion
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<LIPlanillaCertificacionesDC> ObtenerPlanillasCertificaciones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, bool esDevolucion, out int totalRegistros)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (string.IsNullOrWhiteSpace(campoOrdenamiento))
                {
                    campoOrdenamiento = "PLC_FechaGrabacion";
                    ordenamientoAscendente = false;
                }
                string numeroGuia;
                long nGuia = 0;

                if (filtro.TryGetValue("flNumeroGuia", out numeroGuia))
                {
                    totalRegistros = 10;
                    nGuia = Convert.ToInt64(numeroGuia);
                    PlanillaCertificacionesGuia_LOI planillaGuia = contexto.PlanillaCertificacionesGuia_LOI
                     .Where(guia => guia.PLG_NumeroGuia == nGuia).FirstOrDefault();
                    if (planillaGuia != null)
                    {
                        var retorno = contexto.PlanillaCertificaciones_VLOI
                            .Where(pla => pla.PLC_IdPlanillaCertificaciones == planillaGuia.PLG_IdPlanillaCertificaciones && pla.PCD_Devolucion == esDevolucion)
                            .ToList()
                           .ConvertAll(r => new LIPlanillaCertificacionesDC()
                           {
                               NumeroPlanilla = r.PLC_IdPlanillaCertificaciones,
                               FechaPlanilla = r.PLC_FechaGrabacion,
                               CentroServiciosPlanilla = new PUCentroServiciosDC()
                               {
                                   IdCentroServicio = r.PLC_IdCentroServiciosPlanilla,
                                   Nombre = r.CES_Nombre,
                                   Direccion = r.CES_Direccion,
                                   Telefono1 = r.CES_Telefono1,
                                   CiudadUbicacion = new PALocalidadDC()
                                   {
                                       IdLocalidad = r.LOC_IdLocalidad,
                                       Nombre = r.LOC_Nombre
                                   }
                               },
                               RegionalAdmPlamilla = new PURegionalAdministrativa()
                               {
                                   IdRegionalAdmin = r.PLC_IdRegionalAdmPlanilla,
                                   Descripcion = r.REA_Descripcion
                               },
                               GuiaInterna = new ADGuiaInternaDC()
                               {
                                   NumeroGuia = r.NumGuia,
                                   IdAdmisionGuia = r.IdAdmision,
                               },
                               IdDestinatario = r.PCD_IdDestinatario,
                               DireccionDestinatario = r.PCD_Direccion,
                               TelefonoDestinatario = r.PCD_Telefono,
                               NombreDestinatario = r.PCD_Nombre,
                               TipoPlanilla = (LIEnumTipoPlanillaNotificacion)(Enum.Parse(typeof(LIEnumTipoPlanillaNotificacion), r.PCD_TipoPlanilla)),
                               Consolidado = r.PCD_Consolidada,
                               Devolucion = r.PCD_Devolucion,
                               TipoPlanillaDescripcion = (LIEnumTipoPlanillaNotificacion)(Enum.Parse(typeof(LIEnumTipoPlanillaNotificacion), r.PCD_TipoPlanilla)) == LIEnumTipoPlanillaNotificacion.CES ? "Centro Servicio" : "Cliente Credito",
                               Descargada = r.PLC_EstaDescargada,
                               Cerrada = r.PCD_Cerrada,
                               EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.MODIFICADO,
                               NombreClienteCredito = r.NomClienteCredito

                           });
                        return retorno;
                    }
                    else
                        return new List<LIPlanillaCertificacionesDC>();
                }
                else
                {
                    return contexto.ConsultarContainsPlanillaCertificaciones_VLOI(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                      .OrderByDescending(r => r.PLC_FechaGrabacion)
                      .ToList()
                      .ConvertAll(r => new LIPlanillaCertificacionesDC()
                      {
                          NumeroPlanilla = r.PLC_IdPlanillaCertificaciones,
                          FechaPlanilla = r.PLC_FechaGrabacion,
                          CentroServiciosPlanilla = new PUCentroServiciosDC()
                          {
                              IdCentroServicio = r.PLC_IdCentroServiciosPlanilla,
                              Nombre = r.CES_Nombre,
                              Direccion = r.CES_Direccion,
                              Telefono1 = r.CES_Telefono1,
                              CiudadUbicacion = new PALocalidadDC()
                              {
                                  IdLocalidad = r.LOC_IdLocalidad,
                                  Nombre = r.LOC_Nombre
                              }
                          },
                          RegionalAdmPlamilla = new PURegionalAdministrativa()
                          {
                              IdRegionalAdmin = r.PLC_IdRegionalAdmPlanilla,
                              Descripcion = r.REA_Descripcion
                          },
                          GuiaInterna = new ADGuiaInternaDC()
                          {
                              NumeroGuia = r.NumGuia,
                              IdAdmisionGuia = r.IdAdmision
                          },
                          IdDestinatario = r.PCD_IdDestinatario,
                          DireccionDestinatario = r.PCD_Direccion,
                          TelefonoDestinatario = r.PCD_Telefono,
                          NombreDestinatario = r.PCD_Nombre,
                          LocalidadDestino = new PALocalidadDC { IdLocalidad = r.PCD_IdLocalidad, Nombre = r.LOC_NombreDestino },
                          TipoPlanilla = (LIEnumTipoPlanillaNotificacion)(Enum.Parse(typeof(LIEnumTipoPlanillaNotificacion), r.PCD_TipoPlanilla)),
                          Consolidado = r.PCD_Consolidada,
                          Devolucion = r.PCD_Devolucion,
                          TipoPlanillaDescripcion = (LIEnumTipoPlanillaNotificacion)(Enum.Parse(typeof(LIEnumTipoPlanillaNotificacion), r.PCD_TipoPlanilla)) == LIEnumTipoPlanillaNotificacion.CES ? "Centro Servicio" : "Cliente Credito",
                          Descargada = r.PLC_EstaDescargada,
                          Cerrada = r.PCD_Cerrada,
                          EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.MODIFICADO,
                          NombreClienteCredito = r.NomClienteCredito
                      });
                }
            }
        }

        /// <summary>
        /// Obtener planillas de certificacion con Ado
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="esDevolucion"></param>
        /// <param name="totalPaginas"></param>
        /// <returns></returns>
        public IEnumerable<LIPlanillaCertificacionesDC> ObtenerPlanillasCertificacionesAdo(IDictionary<String, String> filtro, String campoOrdenamiento, Int32 indicePagina, Int32 registrosPorPagina, Boolean ordenamientoAscendente, Boolean esDevolucion, out Int32 totalPaginas)
        {
            List<LIPlanillaCertificacionesDC> resultado = null;
            totalPaginas = 0;
            using (SqlConnection conexion = new SqlConnection(this.conexionStringController))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("paObtenerPlanillasCertificaciones_LOI", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrdenamientoAscendente", ordenamientoAscendente);
                cmd.Parameters.AddWithValue("@esDevolucion", esDevolucion);
                cmd.Parameters.AddWithValue("@pagina", indicePagina);
                cmd.Parameters.AddWithValue("@registrosXPagina", registrosPorPagina);

                string numeroGuia;
                long nGuia = 0;
                if (filtro.TryGetValue("flNumeroGuia", out numeroGuia))
                {
                    nGuia = Convert.ToInt64(numeroGuia);
                    cmd.Parameters.AddWithValue("@flNumeroGuia", nGuia);

                }
                else
                {
                    string planilla;
                    long numPlanilla = 0;
                    if (filtro.TryGetValue("flPlanilla", out planilla))
                    {
                        numPlanilla = Convert.ToInt64(planilla);
                        cmd.Parameters.AddWithValue("@flPlanilla", numPlanilla);
                    }

                    string fecha;
                    DateTime dtFecha = DateTime.Now;
                    if (filtro.TryGetValue("flFecha", out fecha))
                    {
                        dtFecha = Convert.ToDateTime(fecha);
                        cmd.Parameters.AddWithValue("@flFecha", dtFecha);
                    }
                }

                string idCentroservicio;
                int centroServicio = 0;
                if (filtro.TryGetValue("PLC_IdCentroServiciosPlanilla", out idCentroservicio))
                {
                    centroServicio = Convert.ToInt32(idCentroservicio);
                    cmd.Parameters.AddWithValue("@flIdCentroServiciosPlanilla", centroServicio);
                }
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var r = new LIPlanillaCertificacionesDC
                        {
                            NumeroPlanilla = Convert.ToInt64(reader["PLC_IdPlanillaCertificaciones"]),
                            FechaPlanilla = Convert.ToDateTime(reader["PLC_FechaGrabacion"]),
                            CentroServiciosPlanilla = new PUCentroServiciosDC
                            {
                                IdCentroServicio = Convert.ToInt64(reader["PLC_IdCentroServiciosPlanilla"]),
                                Nombre = Convert.ToString(reader["CES_Nombre"]),
                                Direccion = Convert.ToString(reader["CES_Direccion"]),
                                Telefono1 = Convert.ToString(reader["CES_Telefono1"]),
                                CiudadUbicacion = new PALocalidadDC()
                                {
                                    IdLocalidad = Convert.ToString(reader["LOC_IdLocalidad"]),
                                    Nombre = Convert.ToString(reader["LOC_Nombre"])
                                }
                            },
                            RegionalAdmPlamilla = new PURegionalAdministrativa()
                            {
                                IdRegionalAdmin = Convert.ToInt64(reader["PLC_IdRegionalAdmPlanilla"]),
                                Descripcion = Convert.ToString(reader["REA_Descripcion"])
                            },
                            GuiaInterna = new ADGuiaInternaDC()
                            {
                                NumeroGuia = Convert.ToInt64(reader["NumGuia"]),
                                IdAdmisionGuia = Convert.ToInt32(reader["IdAdmision"])
                            },
                            IdDestinatario = Convert.ToInt64(reader["PCD_IdDestinatario"]),
                            DireccionDestinatario = Convert.ToString(reader["PCD_Direccion"]),
                            TelefonoDestinatario = Convert.ToString(reader["PCD_Telefono"]),
                            NombreDestinatario = Convert.ToString(reader["PCD_Nombre"]),
                            LocalidadDestino = new PALocalidadDC()
                            {
                                IdLocalidad = Convert.ToString(reader["PCD_IdLocalidad"]),   // r.PCD_IdLocalidad, 
                                Nombre = Convert.ToString(reader["LOC_NombreDestino"]),   //r.LOC_NombreDestino 
                            },
                            TipoPlanilla = (LIEnumTipoPlanillaNotificacion)(Enum.Parse(typeof(LIEnumTipoPlanillaNotificacion), Convert.ToString(reader["PCD_TipoPlanilla"]))),
                            Consolidado = Convert.ToBoolean(reader["PCD_Consolidada"]),
                            Devolucion = Convert.ToBoolean(reader["PCD_Devolucion"]),
                            TipoPlanillaDescripcion = (LIEnumTipoPlanillaNotificacion)(
                                            Enum.Parse(typeof(LIEnumTipoPlanillaNotificacion),
                                                Convert.ToString(reader["PCD_TipoPlanilla"]))) == LIEnumTipoPlanillaNotificacion.CES
                                                    ? "Centro Servicio"
                                                    : "Cliente Credito",
                            Descargada = Convert.ToBoolean(reader["PLC_EstaDescargada"]),
                            Cerrada = Convert.ToBoolean(reader["PCD_Cerrada"]),
                            EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.MODIFICADO,
                            NombreClienteCredito = Convert.ToString(reader["NomClienteCredito"]),

                        };

                        if (resultado == null)
                        {
                            resultado = new List<LIPlanillaCertificacionesDC>();
                            totalPaginas = Convert.ToInt32(reader["Total Paginas"]);
                        }

                        r.GuiaInterna.LocalidadDestino = ObtenerLocalidadOrigenDestinoGuia(r.GuiaInterna.NumeroGuia, true);
                        r.GuiaInterna.LocalidadOrigen = ObtenerLocalidadOrigenDestinoGuia(r.GuiaInterna.NumeroGuia, false);
                        resultado.Add(r);
                    }
                }
            }
            return resultado;
        }


        public PALocalidadDC ObtenerLocalidadOrigenDestinoGuia(long numeroGuia, bool esOrigen)
        {
            var localidad = new PALocalidadDC();

            using (SqlConnection conexion = new SqlConnection(this.conexionStringController))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("paObtenerLocalidadOrigenDestinoGuia_LOI", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@EsOrigen", esOrigen);

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        localidad.Nombre = Convert.ToString(reader["LOC_Nombre"]);
                        localidad.NombreCompleto = Convert.ToString(reader["LOC_Nombre"]);
                        localidad.NombreCorto = Convert.ToString(reader["LOC_NombreCorto"]);
                        localidad.NombreTipoLocalidad = Convert.ToString(reader["NombreTipo"]);
                        localidad.NombreAncestroPGrado = Convert.ToString(reader["NombreAncestroPrimerGrado"]);
                        localidad.NombreAncestroSGrado = Convert.ToString(reader["NombreAncestroSegundoGrado"]);
                        localidad.NombreAncestroTGrado = Convert.ToString(reader["NombreAncestroTercerGrado"]);
                        localidad.IdTipoLocalidad = Convert.ToString(reader["LOC_IdTipo"]);
                        localidad.IdAncestroPGrado = Convert.ToString(reader["LOC_IdAncestroPrimerGrado"]);
                        localidad.IdAncestroPGrado = Convert.ToString(reader["LOC_IdAncestroSegundoGrado"]);
                        localidad.IdAncestroPGrado = Convert.ToString(reader["LOC_IdAncestroTercerGrado"]);
                        localidad.CodigoPostal = Convert.ToString(reader["LOC_CodigoPostal"]);
                        localidad.Indicativo = Convert.ToString(reader["LOC_Indicativo"]);
                    }
                }
            }


            return localidad;
        }


        /// <summary>
        /// Método para obtener las guias de una planilla de certificacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<ADNotificacion> ObtenerGuiasPlanillaCertificacion(long idPlanilla)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PlanillaCertificacionesGuia_VLOI
                   .Where(guia => guia.PLG_IdPlanillaCertificaciones == idPlanilla)
                   .ToList()
                   .ConvertAll(g => new ADNotificacion
                   {
                       EstadoRegistro = EnumEstadoRegistro.MODIFICADO,
                       GuiaAdmision = new ADGuia
                       {
                           IdAdmision = g.PLG_IdAdminisionMensajeria,
                           NumeroGuia = g.PLG_NumeroGuia,
                           NombreCiudadDestino = g.ADM_NombreCiudadDestino,
                           NombreCiudadOrigen = g.ADM_NombreCiudadOrigen,
                           FechaAdmision = g.ADM_FechaGrabacion,
                           Entregada = g.ADM_EstaEntregada,
                       },
                       GuiaInterna = new ADGuiaInternaDC
                       {
                           IdAdmisionGuia = g.PLG_IdAdminisionGuiaInterna,
                           NumeroGuia = g.PLG_NumeroGuiaInterna,
                           //LocalidadDestino = new PALocalidadDC { IdLocalidad = g.ADM_IdCiudadDestinoInterna, Nombre = g.ADM_NombreCiudadDestinoInterna }
                           LocalidadDestino = ObtenerLocalidadOrigenDestinoGuia(g.PLG_NumeroGuia, true)
                       },
                       IdPlanillaCertificacionGuia = g.PLG_IdPlanillaCertificacionesGuia,
                       CentroServicioDestino = new PUCentroServiciosDC { IdCentroServicio = g.PLG_IdCentroServicioDestino }
                   });
            }
        }

        /// <summary>
        /// Guarda la planilla de certificaciones
        /// </summary>
        /// <param name="planilla"></param>
        public LIPlanillaCertificacionesDC GuardarPlanillaCertificacionesAdo(LIPlanillaCertificacionesDC planilla)
        {

            using (var tran = new TransactionScope())
            {
                using (var sqlconn = new SqlConnection(conexionStringController))
                {
                    sqlconn.Open();
                    var cmd = new SqlCommand("paCreaPlanillaCertificaciones_LOI", sqlconn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PLC_IdRegionalAdmPlanilla", planilla.RegionalAdmPlamilla.IdRegionalAdmin);
                    cmd.Parameters.AddWithValue("@PLC_IdCentroServiciosPlanilla", planilla.CentroServiciosPlanilla.IdCentroServicio);
                    cmd.Parameters.AddWithValue("@PLC_FechaGrabacion", DateTime.Now);
                    cmd.Parameters.AddWithValue("@PLC_CreadoPor", ControllerContext.Current.Usuario);
                    cmd.Parameters.AddWithValue("@PLC_IdAdmisionMensajeria", planilla.GuiaInterna.IdAdmisionGuia);
                    cmd.Parameters.AddWithValue("@PLC_NumeroGuia", planilla.GuiaInterna.NumeroGuia);
                    cmd.Parameters.AddWithValue("@PLC_FechaDescargue", ConstantesFramework.MinDateTimeController);
                    cmd.Parameters.AddWithValue("@PLC_EstaDescargada", false);
                    cmd.Parameters.AddWithValue("@PLC_DescargadaPor", String.Empty);
                    long IdPlanillaCertificaciones = Convert.ToInt64(cmd.ExecuteScalar());

                    cmd = new SqlCommand("paCreaPlanillaCertificacionDetalle_LOI", sqlconn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PCD_IdPlanillaCertificaciones", IdPlanillaCertificaciones);
                    cmd.Parameters.AddWithValue("@PCD_TipoPlanilla", planilla.TipoPlanilla.ToString());
                    cmd.Parameters.AddWithValue("@PCD_IdDestinatario", planilla.IdDestinatario);
                    cmd.Parameters.AddWithValue("@PCD_Nombre", planilla.NombreDestinatario);
                    cmd.Parameters.AddWithValue("@PCD_Direccion", planilla.DireccionDestinatario);
                    cmd.Parameters.AddWithValue("@PCD_Telefono", planilla.TelefonoDestinatario);
                    cmd.Parameters.AddWithValue("@PCD_IdLocalidad", planilla.LocalidadDestino.IdLocalidad);
                    cmd.Parameters.AddWithValue("@PCD_Consolidada", planilla.Consolidado);
                    cmd.Parameters.AddWithValue("@PCD_Devolucion", planilla.Devolucion);
                    cmd.Parameters.AddWithValue("@PCD_FechaGrabacion", DateTime.Now);
                    cmd.Parameters.AddWithValue("@PCD_CreadoPor", ControllerContext.Current.Usuario);
                    cmd.Parameters.AddWithValue("@PCD_Cerrada", false);
                    cmd.Parameters.AddWithValue("@PCD_FechaCierre", ConstantesFramework.MinDateTimeController);
                    cmd.Parameters.AddWithValue("@PCD_CerradoPor", string.Empty);
                    cmd.ExecuteNonQuery();

                    planilla.NumeroPlanilla = IdPlanillaCertificaciones;
                }
                tran.Complete();
            }
            return planilla;
        }

        /// <summary>
        /// Guarda la planilla de certificaciones
        /// </summary>
        /// <param name="planilla"></param>
        public LIPlanillaCertificacionesDC GuardarPlanillaCertificaciones(LIPlanillaCertificacionesDC planilla)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaCertificaciones_LOI pla = new PlanillaCertificaciones_LOI
                {
                    PLC_IdRegionalAdmPlanilla = planilla.RegionalAdmPlamilla.IdRegionalAdmin,
                    PLC_IdCentroServiciosPlanilla = planilla.CentroServiciosPlanilla.IdCentroServicio,
                    PLC_FechaGrabacion = DateTime.Now,
                    PLC_CreadoPor = ControllerContext.Current.Usuario,
                    PLC_IdAdmisionMensajeria = planilla.GuiaInterna.IdAdmisionGuia,
                    PLC_NumeroGuia = planilla.GuiaInterna.NumeroGuia,
                    PLC_EstaDescargada = false,
                    PLC_FechaDescargue = ConstantesFramework.MinDateTimeController,
                };
                contexto.PlanillaCertificaciones_LOI.Add(pla);
                PlanillaCertificacionDetalle_LOI planDet = new PlanillaCertificacionDetalle_LOI
                {
                    PCD_Consolidada = planilla.Consolidado,
                    PCD_CreadoPor = ControllerContext.Current.Usuario,
                    PCD_Devolucion = planilla.Devolucion,
                    PCD_Direccion = planilla.GuiaInterna.DireccionDestinatario,
                    PCD_TipoPlanilla = planilla.TipoPlanilla.ToString(),
                    PCD_IdDestinatario = planilla.IdDestinatario,
                    PCD_FechaGrabacion = DateTime.Now,
                    PCD_IdLocalidad = planilla.GuiaInterna.LocalidadDestino.IdLocalidad,
                    PCD_IdPlanillaCertificaciones = pla.PLC_IdPlanillaCertificaciones,
                    PCD_Nombre = planilla.GuiaInterna.NombreDestinatario,
                    PCD_Telefono = planilla.GuiaInterna.TelefonoDestinatario,
                    PCD_Cerrada = false,
                    PCD_CerradoPor = string.Empty,
                    PCD_FechaCierre = ConstantesFramework.MinDateTimeController,
                };
                contexto.PlanillaCertificacionDetalle_LOI.Add(planDet);
                contexto.SaveChanges();
                planilla.NumeroPlanilla = pla.PLC_IdPlanillaCertificaciones;
                return planilla;
            }
        }

        public void ActualizarGuiaInternaPlanilla(long idPlanilla, long idAdmision, long numeroGuia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaCertificaciones_LOI planilla = contexto.PlanillaCertificaciones_LOI.Where(r => r.PLC_IdPlanillaCertificaciones == idPlanilla).FirstOrDefault();

                if (planilla != null)
                {
                    planilla.PLC_IdAdmisionMensajeria = idAdmision;
                    planilla.PLC_NumeroGuia = numeroGuia;
                }

                contexto.SaveChanges();
            }
        }


        /// <summary>
        /// Guarda la planilla de certificacion
        /// </summary>
        /// <param name="planilla"></param>
        public void GuardarGuiaPlanillaCertificacion(ADNotificacion guia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaCertificacionesGuia_LOI plaGuia = new PlanillaCertificacionesGuia_LOI()
                {
                    PLG_NumeroGuia = guia.GuiaAdmision.NumeroGuia,
                    PLG_IdPlanillaCertificaciones = guia.IdPlanillaCertificacionGuia,
                    PLG_CreadoPor = ControllerContext.Current.Usuario,
                    PLG_FechaGrabacion = DateTime.Now,
                    PLG_IdAdminisionMensajeria = guia.GuiaAdmision.IdAdmision,
                    PLG_IdAdminisionGuiaInterna = guia.GuiaInterna.IdAdmisionGuia,
                    PLG_NumeroGuiaInterna = guia.GuiaInterna.NumeroGuia,
                    PLG_IdCentroServicioDestino = guia.CentroServicioDestino.IdCentroServicio
                };

                contexto.PlanillaCertificacionesGuia_LOI.Add(plaGuia);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina una guia de la planilla de certificaciones
        /// </summary>
        /// <param name="guiaCertificacion"></param>
        /// <returns></returns>
        public void EliminarGuiaPlanillaCertificaciones(ADNotificacion guiaCertificacion)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaCertificacionesGuia_LOI guiaPlanilla = contexto.PlanillaCertificacionesGuia_LOI.Where(g => g.PLG_IdPlanillaCertificacionesGuia == guiaCertificacion.IdPlanillaCertificacionGuia).SingleOrDefault();

                if (guiaPlanilla != null)
                {
                    contexto.PlanillaCertificacionesGuia_LOI.Remove(guiaPlanilla);
                    contexto.SaveChanges();
                }
                else
                {
                    ControllerException excepcion = new ControllerException(
                        COConstantesModulos.NOTIFICACIONES, LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE.ToString(), LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Guarda los envios de la planilla de asignacion
        /// </summary>
        /// <param name="planilla"></param>
        public void GuardarLstEnvioPlanillaCertificacion(LIPlanillaCertificacionesDC planilla)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<PlanillaCertificacionesGuia_LOI> referencias = new List<PlanillaCertificacionesGuia_LOI>();
                for (int i = 0; i < planilla.LstGuiasPlanilla.Count(); i++)
                {
                    PlanillaCertificacionesGuia_LOI plaGuia = new PlanillaCertificacionesGuia_LOI()
                    {
                        PLG_IdPlanillaCertificaciones = planilla.NumeroPlanilla,
                        PLG_CreadoPor = ControllerContext.Current.Usuario,
                        PLG_FechaGrabacion = DateTime.Now
                    };
                    referencias.Add(plaGuia);
                }

                // guardar el uno a uno de los suministros aprovisionados en la tabla de referencia
                GuardarGuiasPlanillaCertificacionBulk(referencias, NombreModelo);
            }
        }

        /// <summary>
        /// Obtiene las notificaciones del col, con los filtros seleccionados que esten sin planillar
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="idCol"></param>
        public List<ADNotificacion> ObtenerNotificacionesFiltroSinPla(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCol)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaInicial;
                DateTime fechaFinal;
                string idSucursal;
                string nitCliente;
                string idFormaPago;
                string s_FechaInicial;
                string s_FechaFinal;

                int? _idSucursal = null;
                int? _idFormaPago = null;

                filtro.TryGetValue("fechaInicial", out s_FechaInicial);
                filtro.TryGetValue("fechaFinal", out s_FechaFinal);
                filtro.TryGetValue("idSucursal", out idSucursal);
                filtro.TryGetValue("nitCliente", out nitCliente);
                filtro.TryGetValue("idFormaPago", out idFormaPago);

                if (!string.IsNullOrWhiteSpace(s_FechaFinal) && !string.IsNullOrWhiteSpace(s_FechaInicial))
                {
                    CultureInfo cultura = new CultureInfo("es-CO");
                    if (!DateTime.TryParse(s_FechaInicial, cultura, DateTimeStyles.None, out fechaInicial))
                    {
                        fechaInicial = ConstantesFramework.MinDateTimeController;
                    }
                    if (!DateTime.TryParse(s_FechaFinal, cultura, DateTimeStyles.None, out fechaFinal))
                    {
                        fechaFinal = DateTime.Now.Date.AddDays(1);
                    }
                    fechaInicial.AddDays(1);
                }
                else
                {
                    fechaInicial = ConstantesFramework.MinDateTimeController;
                    fechaFinal = DateTime.Now.Date.AddDays(1);
                }

                if (!string.IsNullOrWhiteSpace(idSucursal))
                {
                    _idSucursal = Convert.ToInt32(idSucursal);
                }

                if (!string.IsNullOrWhiteSpace(idFormaPago))
                {
                    _idFormaPago = Convert.ToInt32(idFormaPago);
                }

                return contexto.paObtenerNotificacionesNoPlan_LOI(idCol, _idSucursal, _idFormaPago, nitCliente == string.Empty ? null : nitCliente, fechaInicial, fechaFinal)
                  .ToList()
                  .ConvertAll(r => new ADNotificacion()
                  {
                      Apellido1Destinatario = r.ADN_Apellido1Destinatario,
                      Apellido2Destinatario = r.ADN_Apellido2Destinatario,
                      NombreDestinatario = r.ADN_NombreDestinatario,
                      DireccionDestinatario = r.ADN_DireccionDestinatario,
                      TelefonoDestinatario = r.ADN_TelefonoDestinatario,
                      EstadoGuia = new ADEstadoGuia()
                      {
                          Id = (short)(r.ADM_EstaEntregada == true ? ADEnumEstadoGuia.Entregada : ADEnumEstadoGuia.DevolucionRatificada),
                      },
                      TipoDestino = new Servicios.ContratoDatos.Tarifas.TATipoDestino()
                      {
                          Id = r.ADN_IdTipoDestino
                      },
                      CiudadDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                      {
                          IdLocalidad = r.ADN_IdCiudadDestino,
                          Nombre = r.ADN_NombreCiudadDestino
                      },
                      GuiaAdmision = new ADGuia()
                      {
                          IdAdmision = r.ADM_IdAdminisionMensajeria,
                          NumeroGuia = r.ADM_NumeroGuia,
                          IdCentroServicioDestino = r.ADM_IdCentroServicioDestino,
                          IdCentroServicioOrigen = r.ADM_IdCentroServicioOrigen,
                          NombreCentroServicioDestino = r.ADM_NombreCentroServicioDestino,
                          NombreCentroServicioOrigen = r.ADM_NombreCentroServicioOrigen,
                          IdCiudadDestino = r.ADM_IdCiudadDestino,
                          IdCiudadOrigen = r.ADM_IdCiudadOrigen,
                          NombreCiudadDestino = r.ADM_NombreCiudadDestino,
                          NombreCiudadOrigen = r.ADM_NombreCiudadOrigen,
                          Entregada = r.ADM_EstaEntregada,
                          FechaAdmision = r.ADM_FechaAdmision
                      }
                  });
            }
        }

        /// <summary>
        /// Obtiene una guia de la planilla de certificacion
        /// </summary>
        /// <param name="idAdmision"></param>
        public LIPlanillaCertificacionesDC ObtenerGuiaPlanillaCertificacion(long numeroGuia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                LIPlanillaCertificacionesDC guiaP = null;
                PlanillaCertificacionesGuia_LOI guiaPlanillada = contexto.PlanillaCertificacionesGuia_LOI.Where(r => r.PLG_NumeroGuia == numeroGuia).FirstOrDefault();

                if (guiaPlanillada != null)
                    guiaP = new LIPlanillaCertificacionesDC()
                    {
                        NumeroPlanilla = guiaPlanillada.PLG_IdPlanillaCertificaciones,
                        GuiaPlanilla = new ADNotificacion()
                        {
                            GuiaAdmision = new ADGuia()
                            {
                                IdAdmision = guiaPlanillada.PLG_IdAdminisionMensajeria,
                                NumeroGuia = guiaPlanillada.PLG_NumeroGuia,
                            }
                        }
                    };

                return guiaP;
            }
        }

        /// <summary>
        /// Guarda las guias en la planilla de certificacion por Bulk copy
        /// </summary>
        /// <param name="referencias"></param>
        /// <param name="nombreModelo"></param>
        public void GuardarGuiasPlanillaCertificacionBulk(IEnumerable<PlanillaCertificacionesGuia_LOI> referencias, string nombreModelo)
        {
            if (referencias == null)
            {
                throw new FaultException<ControllerException>(new ControllerException("LOI", "LOI", "Error en la creación de las guias de la planilla certificacion, la lista llegó vacía"));
            }

            if (referencias.Count() == 0)
            {
                return;
            }

            string conexion = COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionStringNoEntity(nombreModelo);

            BulkCopyController bulkCopy = new BulkCopyController
            {
                BatchSize = 200,
                ConnectionString = conexion,
                DestinationTableName = "dbo.PlanillaCertificacionesGuia_LOI"
            };

            bulkCopy.WriteToServer(referencias);
        }

        /// <summary>
        /// Retorna las guias internas de las planillas
        /// </summary>
        /// <param name="planillas"></param>
        public List<ADGuiaInternaDC> ObtenerGuiasInternasPlanilla(string planillas, long idCol)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerGuiasIntPlanCert_LOI(planillas, idCol)
                  .ToList()
                  .ConvertAll(r => new ADGuiaInternaDC()
                  {
                      FechaGrabacion = r.ADM_FechaGrabacion,
                      NumeroGuia = r.ADM_NumeroGuia,
                      LocalidadOrigen = new PALocalidadDC()
                      {
                          IdLocalidad = r.ADM_IdCiudadOrigen,
                          Nombre = r.ADM_NombreCiudadOrigen
                      },
                      LocalidadDestino = new PALocalidadDC()
                      {
                          IdLocalidad = r.ADM_IdCiudadDestino,
                          Nombre = r.ADM_NombreCiudadDestino
                      },
                      NombreRemitente = r.AGI_NombreRemitente,
                      DireccionRemitente = r.ADM_DireccionRemitente,
                      TelefonoRemitente = r.ADM_TelefonoRemitente,
                      NombreDestinatario = r.AGI_NombreDestinatario,
                      DireccionDestinatario = r.AGI_DireccionDestinatario,
                      TelefonoDestinatario = r.ADM_TelefonoDestinatario,
                      GestionDestino = new Servicios.ContratoDatos.Area.ARGestionDC()
                      {
                          IdGestion = r.AGI_IdGestionDestino,
                          Descripcion = r.AGI_DescripcionGestionDest
                      },
                      DiceContener = r.ADM_DiceContener
                  });
            }
        }

        /// <summary>
        /// Obtener Guias Internas Certificaciones de planilla
        /// </summary>
        /// <param name="planillas"></param>
        /// <returns></returns>
        public List<ADGuiaInternaDC> ObtenerGuiasInternasCertificacionesPlanilla(string planillas, long idCol)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerGuiasIntCertificaciones_LOI(planillas, idCol)
                  .ToList()
                  .ConvertAll(r => new ADGuiaInternaDC()
                  {
                      FechaGrabacion = r.ADM_FechaGrabacion,
                      NumeroGuia = r.ADM_NumeroGuia,
                      LocalidadOrigen = new PALocalidadDC()
                      {
                          IdLocalidad = r.ADM_IdCiudadOrigen,
                          Nombre = r.ADM_NombreCiudadOrigen
                      },
                      LocalidadDestino = new PALocalidadDC()
                      {
                          IdLocalidad = r.ADM_IdCiudadDestino,
                          Nombre = r.ADM_NombreCiudadDestino
                      },
                      NombreRemitente = r.AGI_NombreRemitente,
                      DireccionRemitente = r.ADM_DireccionRemitente,
                      TelefonoRemitente = r.ADM_TelefonoRemitente,
                      NombreDestinatario = r.AGI_NombreDestinatario,
                      DireccionDestinatario = r.AGI_DireccionDestinatario,
                      TelefonoDestinatario = r.ADM_TelefonoDestinatario,
                      GestionDestino = new Servicios.ContratoDatos.Area.ARGestionDC()
                      {
                          IdGestion = r.AGI_IdGestionDestino,
                          Descripcion = r.AGI_DescripcionGestionDest
                      },
                      DiceContener = r.ADM_DiceContener
                  });
            }
        }

        /// <summary>
        /// Descarga la planilla de certificacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void DescargarPlanillaCertificaciones(long idPlanilla)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaCertificaciones_LOI planilla = contexto.PlanillaCertificaciones_LOI.Where(r => r.PLC_IdPlanillaCertificaciones == idPlanilla).SingleOrDefault();

                if (planilla == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.NOTIFICACIONES, LOIEnumTipoErrorLogisticaInversa.EX_PLANILLA_NO_EXISE.ToString(), string.Format(LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_PLANILLA_CERTIFICACION), idPlanilla));
                    throw new FaultException<ControllerException>(excepcion);
                }

                planilla.PLC_FechaDescargue = DateTime.Now;
                planilla.PLC_EstaDescargada = true;
                planilla.PLC_DescargadaPor = ControllerContext.Current.Usuario;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Método para validar si una guía ya esta planillada
        /// </summary>
        /// <param name="numeroGuia"></param>
        public void ValidarNotificacionPlanillada(long numeroGuia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaCertificacionesGuia_LOI guia = contexto.PlanillaCertificacionesGuia_LOI
               .FirstOrDefault(g => g.PLG_NumeroGuia == numeroGuia);

                if (guia != null)
                {
                    ControllerException excepcion = new ControllerException(
                   COConstantesModulos.NOTIFICACIONES, LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_YA_PLANILLADA.ToString(),
                   String.Format(LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_YA_PLANILLADA), numeroGuia.ToString()));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Método para obtener los datos de una planilla de notificaciones
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public LIPlanillaCertificacionesDC ObtenerPlanillaCertificaciones(long numeroPlanilla)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaCertificaciones_VLOI planilla = contexto.PlanillaCertificaciones_VLOI.FirstOrDefault(pla => pla.PLC_IdPlanillaCertificaciones == numeroPlanilla);

                if (planilla != null)
                {
                    return new LIPlanillaCertificacionesDC()
                    {
                        NumeroPlanilla = planilla.PLC_IdPlanillaCertificaciones,
                        FechaPlanilla = planilla.PLC_FechaGrabacion,
                        CentroServiciosPlanilla = new PUCentroServiciosDC()
                        {
                            IdCentroServicio = planilla.PLC_IdCentroServiciosPlanilla,
                            Nombre = planilla.CES_Nombre,
                            Direccion = planilla.CES_Direccion,
                            Telefono1 = planilla.CES_Telefono1,
                            CiudadUbicacion = new PALocalidadDC()
                            {
                                IdLocalidad = planilla.LOC_IdLocalidad,
                                Nombre = planilla.LOC_Nombre
                            }
                        },
                        RegionalAdmPlamilla = new PURegionalAdministrativa()
                        {
                            IdRegionalAdmin = planilla.PLC_IdRegionalAdmPlanilla,
                            Descripcion = planilla.REA_Descripcion
                        },
                        GuiaInterna = new ADGuiaInternaDC()
                        {
                            NumeroGuia = planilla.NumGuia,
                            IdAdmisionGuia = planilla.IdAdmision,
                        },
                        IdDestinatario = planilla.PCD_IdDestinatario,
                        DireccionDestinatario = planilla.PCD_Direccion,
                        TelefonoDestinatario = planilla.PCD_Telefono,
                        NombreDestinatario = planilla.PCD_Nombre,
                        TipoPlanilla = (LIEnumTipoPlanillaNotificacion)(Enum.Parse(typeof(LIEnumTipoPlanillaNotificacion), planilla.PCD_TipoPlanilla)),
                        Consolidado = planilla.PCD_Consolidada,
                        Devolucion = planilla.PCD_Devolucion,
                        TipoPlanillaDescripcion = (LIEnumTipoPlanillaNotificacion)(Enum.Parse(typeof(LIEnumTipoPlanillaNotificacion), planilla.PCD_TipoPlanilla)) == LIEnumTipoPlanillaNotificacion.CES ? "Centro Servicio" : "Cliente Credito",
                        Descargada = planilla.PLC_EstaDescargada,
                        Cerrada = planilla.PCD_Cerrada,
                        EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.MODIFICADO
                    };
                }
                else
                    return new LIPlanillaCertificacionesDC();
            }
        }

        /// <summary>
        /// Método para cerrar una planilla de notificaciones
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        public void CerrarPlanillaNotificaciones(long numeroPlanilla)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaCertificacionDetalle_LOI planilla = contexto.PlanillaCertificacionDetalle_LOI.FirstOrDefault(pla => pla.PCD_IdPlanillaCertificaciones == numeroPlanilla);

                if (planilla != null)
                {
                    planilla.PCD_Cerrada = true;
                    planilla.PCD_CerradoPor = ControllerContext.Current.Usuario;
                    planilla.PCD_FechaCierre = DateTime.Now;
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Planilla Certificaciones

        #endregion Métodos


        #region Recibido Guia

        /// <summary>
        /// Método para obtener guias pendientes de captura datos de recibido
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<LIRecibidoGuia> ObtenerRecibidosPendientes(long idCol, DateTime fechaInicial, DateTime fechaFinal)
        {
            List<LIRecibidoGuia> recibidosPendientes = new List<LIRecibidoGuia>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand(@"paObtenerRecibidosPendientesCentroLogistico_LOI", conn);
                cmd.CommandTimeout = 300;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCentroLogistico", idCol);
                cmd.Parameters.AddWithValue("@fechaInicial", fechaInicial);
                cmd.Parameters.AddWithValue("@fechaFinal", fechaFinal);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                recibidosPendientes = dt.AsEnumerable().ToList().ConvertAll<LIRecibidoGuia>(c =>


                    new LIRecibidoGuia()
                    {
                        NumeroGuia = c.Field<long>("ADM_NumeroGuia"),
                        FechaEntrega = c.Field<DateTime>("ADM_FechaEstimadaEntrega"),
                        FechaAdmision = c.Field<DateTime>("ADM_FechaAdmision"),
                        RutaImagen = c.Field<string>("ARG_RutaArchivo"),
                        IdGuia = c.Field<long>("ADM_IdAdminisionMensajeria"),
                        IdAplicacionOrigen = this.ExtraerOrigen(c.Field<int?>("REG_IdAplicacionOrigen"))
                    });

            }
            return recibidosPendientes;
        }

        private LIEnumOrigenAplicacion ExtraerOrigen(int? valor)
        {
            if (valor != null)
            {
                return (LIEnumOrigenAplicacion)valor.Value;
            }
            else
            {
                return LIEnumOrigenAplicacion.POS;
            }
        }

        public List<LIRecibidoGuia> ObtenerRecibidosPendientesApp(long idCol, DateTime fechaInicial, DateTime fechaFinal, LIEnumOrigenAplicacion idAplicacionOrigen)
        {
            List<LIRecibidoGuia> recibidosPendientes = new List<LIRecibidoGuia>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand(@"paObtenerRecibidosPendientesAppCentroLogistico_LOI", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCentroLogistico", idCol);
                cmd.Parameters.AddWithValue("@fechaInicial", fechaInicial);
                cmd.Parameters.AddWithValue("@fechaFinal", fechaFinal);
                cmd.Parameters.AddWithValue("@IdAplicacionOrigen", idAplicacionOrigen);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                recibidosPendientes = dt.AsEnumerable().ToList().ConvertAll<LIRecibidoGuia>(c =>
                    new LIRecibidoGuia()
                    {
                        NumeroGuia = c.Field<long>("ADM_NumeroGuia"),
                        FechaEntrega = c.Field<DateTime>("REG_FechaEntrega"),
                        FechaAdmision = c.Field<DateTime>("ADM_FechaAdmision"),
                        IdGuia = c.Field<long>("ADM_IdAdminisionMensajeria"),
                        RutaImagen = c.Field<string>("IEGT_RutaImagen"),
                        RecibidoPor = c.Field<string>("REG_RecibidoPor"),
                        Identificacion = c.Field<string>("REG_Identificacion"),
                        Telefono = c.Field<string>("REG_Telefono"),
                        FechaVerificacion = c.Field<DateTime?>("REG_FechaVerificacion"),
                        IdAplicacionOrigen = this.ExtraerOrigen(c.Field<int?>("REG_IdAplicacionOrigen"))
                    });

            }
            return recibidosPendientes;
        }

        #endregion


        #region Certificaciones Web

        

        /// <summary>
        /// Método para validar si un envío tiene certifiación de entrega o de devolución
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public ADEstadoGuia ValidarEntregaoDevolucion(long NumeroGuia)
        {
            ADEstadoGuia estadoGuia = new ADEstadoGuia();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"paObtenerIdEntregaDevolucion_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", NumeroGuia));

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    estadoGuia.Id = Convert.ToInt16(reader["EGT_IdEstadoGuia"]);
                    estadoGuia.Descripcion = reader["FOP_Descripcion"].ToString();
                }

            }
            return estadoGuia;
        }


        /// <summary>
        /// Método que valida si existe la imagen de un envío
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public string ValidarImagenCertificacionWeb(long NumeroGuia)
        {
            string rutaImagen;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paValidarImagenCertificacionWeb_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", NumeroGuia));
                conn.Open();
                rutaImagen = Convert.ToString(cmd.ExecuteScalar());
                conn.Close();

                return rutaImagen;
            }
        }

        /// <summary>
        /// Método para validar el Recibido Capturado
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public long ValidarRecibidoCapturado(long NumeroGuia)
        {
            long recibido;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paValidarRecibidoCapturado_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", NumeroGuia));
                conn.Open();
                recibido = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();

                return recibido;
            }
        }

        /// <summary>
        /// Método para insertar la auditoria de las notificaciones impresas por un punto
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <param name="fechaimpresion"></param>
        /// <param name="usuarioimprime"></param>
        /// <param name="colid"></param>
        /// <param name="coldescripcion"></param>
        public void InsertarAuditoriaImpresionNotifPunto(LIReimpresionCertificacionDC auditoriaReimpresion)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paInsertarAuditoriaImpresionNotifPunto_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", auditoriaReimpresion.NumeroGuia);
                cmd.Parameters.AddWithValue("@FechaImpresion", DateTime.Now);
                cmd.Parameters.AddWithValue("@UsuarioImprime", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdCentroServicio", ControllerContext.Current.IdCentroServicio);
                cmd.Parameters.AddWithValue("@DirCentroServicio", auditoriaReimpresion.DireccionCentroServicio);
                cmd.Parameters.AddWithValue("@NombreSolicitante", auditoriaReimpresion.NombreSolicitante);
                cmd.Parameters.AddWithValue("@IdentificacionSolicitante", auditoriaReimpresion.IdentificacionSolicitante);
                cmd.Parameters.AddWithValue("@TelefonoSolicitante", auditoriaReimpresion.TelefonoSolicitante);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Método que valida si la Certificación ya se ha impreso con anterioridad
        /// </summary>
        /// <param name="numeroguia"></param>
        public long ValidarNotificacionExisteAuditoria(long numeroguia)
        {
            long numGuia;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paValidarNotificacionExisteAuditoria_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroguia);
                conn.Open();
                numGuia = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();

                return numGuia;
            }
        }

        /// <summary>
        /// Método que actualiza el campo está devuelta en la tabla Admision Notificaciones
        /// </summary>
        /// <param name="numeroguia"></param>
        public void ActualizaEstaDevueltaADMNotif(long numeroguia)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paActualizaEstaDevueltaADMNotif_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroguia);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Obtiene la información de una Notificacion y los datos de la persona que la solicita
        /// </summary>
        /// <param name="numGuia"></param>
        /// <returns></returns>
        public ADNotificacion ObtenerDatosFacturaReImpresionNotificacion(long numGuia)
        {
            var datosNotif = new ADNotificacion();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"paObtenerDatosFacturaReImpresionNotificacion_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numGuia);

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        datosNotif.IdDestinatario = reader["NIT"].ToString();
                        datosNotif.NombreDestinatario = reader["Cliente"].ToString();
                        datosNotif.DireccionDestinatario = reader["Direccion"].ToString();
                        datosNotif.NombreCiudadDestino = reader["Ciudad"].ToString();
                        datosNotif.FechaImpresion = Convert.ToDateTime(reader["FechaImpresion"]);
                        datosNotif.NumeroGuia = Convert.ToInt64(reader["NumeroGuia"]);
                        datosNotif.FormaPago = reader["FormaPago"].ToString();
                        datosNotif.IdentificacionSolicitante = reader["IdentificacionSolicitante"].ToString();
                        datosNotif.NombreSolicitante = reader["NombreSolicitante"].ToString();
                        datosNotif.TelefonoSolicitante = reader["TelefonoSolicitante"].ToString();
                    }
                }
            }
            return datosNotif;
        }


        #endregion
    }
}