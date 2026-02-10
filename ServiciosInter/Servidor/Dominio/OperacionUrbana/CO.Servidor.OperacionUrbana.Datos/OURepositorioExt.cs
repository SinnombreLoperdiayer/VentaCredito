using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.OperacionUrbana.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Data.SqlClient;
using System.Data;

namespace CO.Servidor.OperacionUrbana.Datos
{
    public partial class OURepositorio
    {
        private string filePath = string.Empty;

        #region Consulta

        /// <summary>
        /// Obtiene los mensajeros de una agencia especifica
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroDescargueAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long puntoServicio)
        {
            IList<OUMensajeroDC> listaRetorna;
            bool Agencia;
            string esAgencia;
            filtro.TryGetValue("esAgencia", out esAgencia);
            filtro.Remove("esAgencia");
            Agencia = Convert.ToBoolean(esAgencia);
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (Agencia)
                {
                    filtro.Add("AGE_IdAgencia", puntoServicio.ToString());
                    listaRetorna = contexto.ConsultarContainsMensajerosAgenciaCol_VOPU(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                      .ToList().ConvertAll(r => new OUMensajeroDC()
                      {
                          NombreCompleto = r.NombreCompleto,
                          IdMensajero = r.MEN_IdMensajero,
                          IdTipoMensajero = r.MEN_IdTipoMensajero,
                          PersonaInterna = new OUPersonaInternaDC()
                          {
                              Identificacion = r.PEI_Identificacion,
                          },
                          Agencia = r.AGE_IdAgencia,
                          TipoMensajero = r.TIM_Descripcion,
                      });
                    filtro.Remove("AGE_IdAgencia");
                }
                else
                {
                    filtro.Add("AGE_IdCentroLogistico", puntoServicio.ToString());
                    listaRetorna = contexto.ConsultarContainsMensajerosAgenciaCol_VOPU(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                      .ToList().ConvertAll(r => new OUMensajeroDC()
                      {
                          NombreCompleto = r.NombreCompleto,
                          IdMensajero = r.MEN_IdMensajero,
                          IdTipoMensajero = r.MEN_IdTipoMensajero,
                          PersonaInterna = new OUPersonaInternaDC()
                          {
                              Identificacion = r.PEI_Identificacion,
                          },
                          Agencia = r.AGE_IdAgencia,
                          TipoMensajero = r.TIM_Descripcion,
                      });
                    filtro.Remove("AGE_IdCentroLogistico");
                }
            }
            return listaRetorna;
        }


        /// <summary>
        /// Obtiene los mensajeros de una punto especifico
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajerosAuditores(long puntoServicio, bool esAgencia)
        {
            IList<OUMensajeroDC> listaRetorna;
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (esAgencia)
                {
                    listaRetorna = contexto.MensajerosAgenciaCol_VOPU.Where(men => men.MEN_Estado == ConstantesFramework.ESTADO_ACTIVO
                        && men.AGE_IdAgencia == puntoServicio)
                      .ToList().ConvertAll(r => new OUMensajeroDC()
                      {
                          NombreCompleto = r.PEI_Identificacion + " - " + r.NombreCompleto,
                          IdMensajero = r.MEN_IdMensajero,
                          IdTipoMensajero = r.MEN_IdTipoMensajero,
                          PersonaInterna = new OUPersonaInternaDC()
                          {
                              Identificacion = r.PEI_Identificacion,
                          },
                          Agencia = r.AGE_IdAgencia,
                          TipoMensajero = r.TIM_Descripcion,
                          CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo
                          {
                              IdCargo = r.CAR_IdCargo,
                              DescripcionCargo = r.CAR_Descripcion
                          }
                      });
                }
                else
                {
                    listaRetorna = contexto.MensajerosAgenciaCol_VOPU.Where(men => men.MEN_Estado == ConstantesFramework.ESTADO_ACTIVO
                         && men.AGE_IdCentroLogistico == puntoServicio)
                         .ToList().ConvertAll(r => new OUMensajeroDC()
                         {
                             NombreCompleto = r.PEI_Identificacion + " - " + r.NombreCompleto,
                             IdMensajero = r.MEN_IdMensajero,
                             IdTipoMensajero = r.MEN_IdTipoMensajero,
                             PersonaInterna = new OUPersonaInternaDC()
                             {
                                 Identificacion = r.PEI_Identificacion,
                             },
                             Agencia = r.AGE_IdAgencia,
                             TipoMensajero = r.TIM_Descripcion,
                             CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo
                             {
                                 IdCargo = r.CAR_IdCargo,
                                 DescripcionCargo = r.CAR_Descripcion
                             }
                         });
                }
            }
            return listaRetorna;
        }


        /// <summary>
        /// Método para obtener las guías pendientes de un mensajero asignado a una agencia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero)
        {

            List<OUGuiaIngresadaDC> listaRetorna = new List<OUGuiaIngresadaDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGuiasMensajero_OPU", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idMensajero", idMensajero);
                cmd.Parameters.AddWithValue("@estadoGuia", "PLA");
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count != 0)
                {
                    return
                                dt.AsEnumerable()
                                .ToList()
                                .ConvertAll(r => new OUGuiaIngresadaDC
                                {
                                    NumeroGuia = Convert.ToInt64(r["PAG_NumeroGuia"]),
                                    IdAdmision = Convert.ToInt64(r["PAG_IdAdminisionMensajeria"]),
                                    IdCentroLogistico = Convert.ToInt64(r["PAE_IdAgencia"]),
                                    Planilla = Convert.ToInt64(r["PAM_IdPlanillaAsignacionEnvio"]),
                                    FechaPlanilla = Convert.ToDateTime(r["PAE_FechaGrabacion"]),
                                    FechaAsignacion = Convert.ToDateTime(r["PAE_FechaGrabacion"]),
                                    EstadoGuiaPlanilla = r["PAG_EstadoEnPlanilla"].ToString(),
                                    EstadoDescripcionGuiaPlanilla = r["DescripcionEstadoManifiesto"].ToString(),
                                    UsuarioDescarga = r["PAG_DescargadaPor"].ToString(),
                                    FechaDescarga = Convert.ToDateTime(r["PAG_FechaDescargue"]),
                                    EstaDescargada = Convert.ToBoolean(r["PAG_EstaDescargada"]),
                                    EsAlCobro = Convert.ToBoolean(r["PAG_RecogeDinero"]),
                                    IdMensajero = Convert.ToInt64(r["PAM_IdMensajero"]),
                                    EstaPlanillada = Convert.ToBoolean(r["PAG_FuePlanillada"]),
                                    TipoImpreso = ADEnumTipoImpreso.Planilla,
                                    Ciudad = r["LOC_Nombre"].ToString(),
                                    IdCiudad = r["PAE_IdMunicipioAsignacion"].ToString(),
                                    CantidadReintentosEntrega = Convert.ToInt16(r["ADM_CantidadReintentosEntrega"]),
                                    FechaAuditoria = Convert.ToDateTime(r["ADM_FechaEstimadaEntregaNew"]),
                                    FechaActual = Convert.ToDateTime(r["ADM_FechaAdmision"]),
                                    DireccionDestinatario = r["ADM_DireccionDestinatario"].ToString(),
                                    TipoCliente = r["ADM_TipoCliente"].ToString(),
                                    Peso = Convert.ToInt64(r["ADM_Peso"]),
                                    IdEstadoGuia = Convert.ToInt16(r["ADM_IdEstadoGuia"]),
                                    CiudadDestino = new PALocalidadDC
                                    {
                                        IdLocalidad = r["ADM_IdCiudadDestino"].ToString(),
                                    },
                                    Servicio = new Servicios.ContratoDatos.Tarifas.TAServicioDC
                                    {
                                        IdServicio = Convert.ToInt32(r["ADM_IdServicio"]),
                                    }
                                });
                }
                else
                    return new List<OUGuiaIngresadaDC>();

                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        public int ObtenerIdMensajeroPorIdentificacion(string identificacionMensajero)
        {
           
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerIdMensajeroPorIdentificacion", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Identificacion", identificacionMensajero);
                conn.Open();
                var idMensajero = cmd.ExecuteScalar();

                return idMensajero == DBNull.Value ? 0 : (int)idMensajero;
            }
        }

        /// <summary>
        /// Obtener la informacion de un mensajero por medio de su identificador
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Datos mensajero</returns>
        public OUDatosMensajeroDC ObtenerDatosMensajero(long idMensajero)
        {
            OUDatosMensajeroDC datosMensajero = new OUDatosMensajeroDC();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMensajeroGuia_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);                
                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    datosMensajero.IdMensajero = Convert.ToInt64(lector["MEN_IdMensajero"]);                    
                    datosMensajero.IdTipoMensajero = Convert.ToInt32(lector["MEN_IdTipoMensajero"]);
                    datosMensajero.IdAgencia = Convert.ToInt64(lector["MEN_IdAgencia"]);
                    datosMensajero.Telefono2 = Convert.ToString(lector["MEN_Telefono2"]);
                    datosMensajero.FechaIngreso = Convert.ToDateTime(lector["MEN_FechaIngreso"]);
                    datosMensajero.FechaTerminacionContrato = Convert.ToDateTime(lector["MEN_FechaTerminacionContrato"]);
                    datosMensajero.NumeroPase = Convert.ToString(lector["MEN_NumeroPase"]);
                    datosMensajero.FechaVencimientoPase = Convert.ToDateTime(lector["MEN_FechaVencimientoPase"]);
                    datosMensajero.Estado = Convert.ToString(lector["MEN_Estado"]);
                    datosMensajero.EsContratista = Convert.ToBoolean(lector["MEN_EsContratista"]);
                    datosMensajero.Descripcion = Convert.ToString(lector["TIM_Descripcion"]);
                    datosMensajero.EsVehicular = Convert.ToBoolean(lector["TIM_EsVehicular"]);
                    datosMensajero.IdPersonaInterna = Convert.ToInt64(lector["PEI_IdPersonaInterna"]);
                    datosMensajero.IdTipoIdentificacion = Convert.ToString(lector["PEI_IdTipoIdentificacion"]);
                    datosMensajero.Identificacion = Convert.ToString(lector["PEI_Identificacion"]);
                    datosMensajero.IdCargo = Convert.ToInt32(lector["PEI_IdCargo"]);
                    datosMensajero.NombreMensajero = Convert.ToString(lector["NombreMensajero"]);
                    datosMensajero.PrimerApellido = Convert.ToString(lector["PEI_PrimerApellido"]);
                    datosMensajero.SegundoApellido = Convert.ToString(lector["PEI_SegundoApellido"]);
                    datosMensajero.DireccionMensajero = Convert.ToString(lector["DireccionMensajero"]);
                    datosMensajero.Municipio = Convert.ToString(lector["PEI_Municipio"]);
                    datosMensajero.Telefono = Convert.ToString(lector["PEI_Telefono"]);
                    datosMensajero.EmailMensajero = Convert.ToString(lector["EmailMensajero"]);
                    datosMensajero.IdRegionalAdm = Convert.ToInt64(lector["PEI_IdRegionalAdm"]);
                    datosMensajero.Comentarios = Convert.ToString(lector["PEI_Comentarios"]);
                    datosMensajero.IdCentroServicios = Convert.ToInt64(lector["CES_IdCentroServicios"]);
                    datosMensajero.NombreCentroServicio = Convert.ToString(lector["NombreCentroServicio"]);
                    datosMensajero.Telefono1 = Convert.ToString(lector["CES_Telefono1"]);
                    datosMensajero.DireccionCentroServicio = Convert.ToString(lector["DireccionCentroServicio"]);
                    datosMensajero.IdMunicipio = Convert.ToString(lector["CES_IdMunicipio"]);
                    datosMensajero.NombreLocalidad = Convert.ToString(lector["NombreLocalidad"]);                    
                    datosMensajero.TipoContrato = Convert.ToInt32(lector["MEN_TipoContrato"]);
                }
                sqlConn.Close();
            }
            return datosMensajero;
        }

        /// <summary>
        /// Método para consultar un motivo de descargue ingresado desde el APP
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public LIMotivoEvidenciaGuiaDC ObtenerFechaIntentoYMotivoGuia(long numeroGuia, long idPlanilla)
        {
            LIMotivoEvidenciaGuiaDC guiaMotivo = new LIMotivoEvidenciaGuiaDC();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGuiaConIdmotivoyDescripcion_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@idPlanilla", idPlanilla);
                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    guiaMotivo.IdEstadoGuialog = Convert.ToInt64(lector["EGT_IdEstadoGuiaLog"]);
                    guiaMotivo.Motivo = new LIDetalleMotivoGuiaDC()
                    {
                        IdMotivoGuia = Convert.ToInt32(lector["EGM_IdMotivoGuia"]),
                        Motivoguia = Convert.ToString(lector["MOG_Descripcion"])
                    };
                    guiaMotivo.NumeroEvidencia = lector["VOD_NumeroEvidencia"] as long? ?? null;
                    guiaMotivo.TipoEvidencia = lector["VOD_TipoEvidencia"] as short? ?? 0;   
                    guiaMotivo.FechaMotivo = Convert.ToDateTime(lector["EGM_FechaMotivo"]);
                }
                sqlConn.Close();
            }
            return guiaMotivo;
        }

        /// <summary>
        /// Método para obtener las guías pendientes de un auditor asignado a un COL
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor)
        {

            List<OUGuiaIngresadaDC> listaRetorna = new List<OUGuiaIngresadaDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGuiasAuditor_OPU", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAuditor", idAuditor);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count != 0)
                {
                    return
                                dt.AsEnumerable()
                                .ToList()
                                .ConvertAll(r => new OUGuiaIngresadaDC
                                {
                                    NumeroGuia = Convert.ToInt64(r["PAG_NumeroGuia"]),
                                    IdAdmision = Convert.ToInt64(r["PAG_IdAdminisionMensajeria"]),
                                    IdCentroLogistico = Convert.ToInt64(r["PAE_IdAgencia"]),
                                    Planilla = Convert.ToInt64(r["PAM_IdPlanillaAsignacionEnvio"]),
                                    FechaPlanilla = Convert.ToDateTime(r["PAE_FechaGrabacion"]),
                                    FechaAsignacion = Convert.ToDateTime(r["PAE_FechaGrabacion"]),
                                    EstadoGuiaPlanilla = r["PAG_EstadoEnPlanilla"].ToString(),
                                    EstadoDescripcionGuiaPlanilla = r["DescripcionEstadoManifiesto"].ToString(),
                                    UsuarioDescarga = r["PAG_DescargadaPor"].ToString(),
                                    FechaDescarga = Convert.ToDateTime(r["PAG_FechaDescargue"]),
                                    EstaDescargada = Convert.ToBoolean(r["PAG_EstaDescargada"]),
                                    EsAlCobro = Convert.ToBoolean(r["PAG_RecogeDinero"]),
                                    IdMensajero = Convert.ToInt64(r["PAM_IdMensajero"]),
                                    EstaPlanillada = Convert.ToBoolean(r["PAG_FuePlanillada"]),
                                    TipoImpreso = ADEnumTipoImpreso.Planilla,
                                    Ciudad = r["LOC_Nombre"].ToString(),
                                    IdCiudad = r["PAE_IdMunicipioAsignacion"].ToString(),
                                    CantidadReintentosEntrega = Convert.ToInt16(r["ADM_CantidadReintentosEntrega"]),
                                    FechaAuditoria = Convert.ToDateTime(r["ADM_FechaEstimadaEntregaNew"]),
                                    DireccionDestinatario = r["ADM_DireccionDestinatario"].ToString(),
                                    TipoCliente = r["ADM_TipoCliente"].ToString(),
                                    Peso = Convert.ToInt64(r["ADM_Peso"]),
                                    IdEstadoGuia = Convert.ToInt16(r["ADM_IdEstadoGuia"]),
                                    CiudadDestino = new PALocalidadDC
                                    {
                                        IdLocalidad = r["ADM_IdCiudadDestino"].ToString(),
                                    },
                                    Servicio = new Servicios.ContratoDatos.Tarifas.TAServicioDC
                                    {
                                        IdServicio = Convert.ToInt32(r["ADM_IdServicio"]),
                                    }
                                });
                }
                else
                    return new List<OUGuiaIngresadaDC>();
               
               
            }
        }


        /// <summary>
        /// Método para obtener las guías pendientes de un mensajero asignado a una agencia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public OUGuiaIngresadaDC ObtenerGuiaPlanilla(long numeroGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaAsignacionGuia_OPU guia = contexto.PlanillaAsignacionGuia_OPU.Where(gu => gu.PAG_NumeroGuia == numeroGuia && gu.PAG_EstaDescargada == false)
                    .FirstOrDefault();

                if (guia != null)
                {
                    return new OUGuiaIngresadaDC
                    {
                        NumeroGuia = guia.PAG_NumeroGuia,
                        IdAdmision = guia.PAG_IdAdminisionMensajeria,
                        UsuarioDescarga = guia.PAG_DescargadaPor,
                        FechaDescarga = guia.PAG_FechaDescargue,
                        EstaDescargada = guia.PAG_EstaDescargada,
                        EsAlCobro = guia.PAG_RecogeDinero,
                        EstaPlanillada = guia.PAG_FuePlanillada,
                        Planilla = guia.PAG_IdPlanillaAsignacionEnvio,
                        TipoImpreso = ADEnumTipoImpreso.Planilla,
                    };
                }
                else return null;
            }
        }

        /// <summary>
        /// Método para obtener la última planilla de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUPlanillaAsignacionDC ObtenerUltimaPLanillaMensajero(long idMensajero)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaAsignaEnvioMensaj_OPU planilla = contexto.PlanillaAsignaEnvioMensaj_OPU
                  .Where(m => m.PAM_IdMensajero == idMensajero)
                  .OrderByDescending(f => f.PAM_FechaGrabacion)
                  .FirstOrDefault();
                if (planilla != null)
                    return new OUPlanillaAsignacionDC()
                    {
                        IdPlanillaAsignacion = planilla.PAM_IdPlanillaAsignacionEnvio,
                        FechaAsignacion = planilla.PAM_FechaGrabacion
                    };
                else
                    return new OUPlanillaAsignacionDC();
            }
        }

        /// <summary>
        /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
        /// </summary>
        /// <param name="numeroGuia"></param>.
        /// <param name="idAgencia"></param>
        /// <returns>True si la guía ya fué ingresada</returns>
        [Obsolete("Metodo comentareado por optimizacion se cambia por ValidarGuiaCentroAcopio que esta en un procedimiento almacenado")]
        public bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IngresoGuiaNoRegAgencia_OPU guia = contexto.IngresoGuiaNoRegAgencia_OPU.FirstOrDefault(g => g.IGN_NumeroGuia == numeroGuia);// && g.IGN_IdAgencia == idAgencia);
                return guia != null;
            }
        }

        /// <summary>
        /// Indica si una guía, dado su número ya ha sido ingresada a centro de acopio pero no habiá sido creada en el sistema
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <returns>Retorna el número de la agencia uqe hizo el ingreso</returns>
        public long GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(long numeroGuia)
        {
            //using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paValidarGuiaIngresoACentroDeAcopio_OPU", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConn.Close();
                var guia = dt.AsEnumerable().FirstOrDefault();
                //IngresoGuiaNoRegAgencia_OPU guia = contexto.IngresoGuiaNoRegAgencia_OPU.OrderByDescending(g => g.IGN_FechaGrabacion).FirstOrDefault(g => g.IGN_NumeroGuia == numeroGuia);
                if (guia != null)
                {
                    return guia.Field<long>("IGN_IdAgencia");
                }
                return 0;
            }
        }

        /// <summary>
        /// Retorn el numero de motivos diferente porque ha sido devuelta la guia
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns>numero de motivos diferente porque ha sido devuelta la guia</returns>
        public int ObtenerConteoMotivosDevolucion(long? NumeroGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<short?> lista = contexto.paObtenerNumeroDeMotivosDevolucion_MEN(NumeroGuia).ToList();
                int numeroMotivosDevolucion = lista.Count();
                return numeroMotivosDevolucion;

            }
        }

        public List<string> ObtenerHistoricoMotivosDevolucion(long? numeroGuia)
        {
            List<string> lstHistoricos = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringControllerAud))
            {
                sqlConn.Open();
                string comando = @"SELECT ADM_MotivoActual FROM AuditoriaDescMensajero_AUD  with(nolock) WHERE ADM_NumeroGuia = @numeroGuia ORDER BY ADM_FechaGrabacion ASC  ";
                //SqlCommand cmd = new SqlCommand(@"paObtenerHistMotivosDevolucion_MEN", sqlConn);
                SqlCommand cmd = new SqlCommand(comando, sqlConn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", numeroGuia));
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lstHistoricos.Add(reader["ADM_MotivoActual"].ToString());
                }
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //DataTable dt = new DataTable();                
                //da.Fill(dt);
                sqlConn.Close();

                //List<string> lstHistoricos = dt.AsEnumerable().ToList().ConvertAll<string>(h =>
                //    {
                //        return h.Field<string>("ADM_MotivoActual");
                //    });
                return lstHistoricos;
            }
        }

        #endregion Consulta

        #region Insertar

        public void AuditarDescMensajerosMotivos(long? numeroGuia, string nombreMensajero, string motivoActual, int cantidadMotivos, long idCol)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringControllerAud))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(@"paAuditarDescargaGuiasMensajeros_AUD", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@nombreMensajero", nombreMensajero);
                cmd.Parameters.AddWithValue("@motivoActual", motivoActual);
                cmd.Parameters.AddWithValue("@cantidadMotivos", cantidadMotivos);
                cmd.Parameters.AddWithValue("@idCol", idCol);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }
        #endregion

        #region Actualización

        /// <summary>
        /// Método para actualizar el estado de una guía de una planilla de mensajero
        /// </summary>
        /// <param name="guia"></param>
        public void ActualizarGuiaMensajero(OUGuiaIngresadaDC guia)
        {           
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paActualizarGuiaMensajero_OPU", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
                cmd.Parameters.AddWithValue("@estadoEnPlanilla", guia.NuevoEstadoGuia);
                cmd.Parameters.AddWithValue("@usuario", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@estaDescargada", guia.EstaDescargada);
                cmd.Parameters.AddWithValue("@IdPlanillaAsignacionEnvio", guia.Planilla);

                cmd.ExecuteNonQuery();
            }
        }

        public void ActualizarGuiaMensajeroAdo(OUGuiaIngresadaDC guia)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paActualizarGuiaMensajero_OPU", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
                cmd.Parameters.AddWithValue("@estadoEnPlanilla", guia.NuevoEstadoGuia);
                cmd.Parameters.AddWithValue("@usuario", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@estaDescargada", guia.EstaDescargada);
                cmd.Parameters.AddWithValue("@IdPlanillaAsignacionEnvio", guia.Planilla);

                cmd.ExecuteNonQuery();
            }
        }

        #endregion Actualización

        #region Planilla de ventas

        /// <summary>
        /// Consulta si un punto de servicio tiene almenos una planilla de recoleccion en punto (planilla ventas)  abierta
        /// </summary>
        /// <returns></returns>
        public bool ValidarPlanillasAbiertasPorPuntoVenta(long idPuntoServicio)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.PlanillaVentas_OPU.Where(p => p.PLA_IdPuntoServicio == idPuntoServicio && !p.PLA_EstaCerrada).Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Obtiene una lista de las asignaciones de tulas y precintos por punto de servicio,  y por estado
        /// </summary>
        /// <param name="idPuntoServicio"></param>
        /// <returns></returns>
        public List<OUAsignacionDC> ObtenerAsignacionTulaPrecintoPuntoServicio(long idPuntoServicio, string estadoAsignacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerAsigTulaPrecinPtnServicioDest_OPU(idPuntoServicio, estadoAsignacion).ToList().ConvertAll<OUAsignacionDC>(
                a =>
                    new OUAsignacionDC()
                    {
                        CentroServicioDestino = new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC() { IdCentroServicio = a.ATP_IdCentroServicioDestino },
                        CentroServicioOrigen = new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC() { IdCentroServicio = a.ATP_IdCentroServicioOrigen },
                        FechaCreacion = a.ATP_FechaGrabacion,
                        IdAsignacion = a.ATP_IdAsignacionTula,
                        NoPrecinto = a.ATP_NoPrecinto,
                        NoTula = a.ATP_NoTula,
                        NumContTransDespacho = a.ATP_NumContTransDespacho,
                        NumContTransRetorno = a.ATP_NumContTransRetorno,
                        TipoAsignacion = new OUTipoAsignacionDC() { IdTipoAsignacion = a.ATP_IdTipoAsignacion },
                        Estado = a.ATP_Estado,
                    });
            }
        }

        #endregion Planilla de ventas

        #region Ingreso a centro de acopio nacional

        /// <summary>
        /// Método para obtener las asignaciones
        /// </summary>
        /// <param name="controlTrans"></param>
        /// <param name="noPrecinto"></param>
        /// <param name="noConsolidado"></param>
        /// <returns></returns>
        public List<OUAsignacionDC> ObtenerAsignaciones(long controlTrans, long noPrecinto, string noConsolidado)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<OUAsignacionDC> asignaciones = new List<OUAsignacionDC>();
                IDictionary<string, string> filtro = new Dictionary<string, string>();
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                if (controlTrans != 0)
                {
                    LambdaExpression lamda = contexto.CrearExpresionLambda<AsignacionConsolidado_VOPU>("PVG_NumContTransRetorno", controlTrans.ToString(), OperadorComparacion.Equal);
                    where.Add(lamda, OperadorLogico.And);
                }
                if (noPrecinto != 0)
                {
                    LambdaExpression lamda1 = contexto.CrearExpresionLambda<AsignacionConsolidado_VOPU>("ATP_NoPrecinto", noPrecinto.ToString(), OperadorComparacion.Equal);
                    where.Add(lamda1, OperadorLogico.And);
                }
                if (!String.IsNullOrEmpty(noConsolidado))
                {
                    LambdaExpression lamda2 = contexto.CrearExpresionLambda<AsignacionConsolidado_VOPU>("ATP_NoTula", noConsolidado, OperadorComparacion.Contains);
                    where.Add(lamda2, OperadorLogico.And);
                }
                LambdaExpression lamda3 = contexto.CrearExpresionLambda<AsignacionConsolidado_VOPU>("ATP_Estado", OUConstantesOperacionUrbana.ESTADO_ASIGNADA, OperadorComparacion.Contains);
                where.Add(lamda3, OperadorLogico.And);

                int totalRegistros;
                var asignacionesPlanilla = contexto.ConsultarAsignacionConsolidado_VOPU(filtro, where, string.Empty, out totalRegistros, 0, 100, true)
                    .Where(aS => aS.PLA_EstaCerrada == true)
                    .ToList();
                if (asignacionesPlanilla != null && asignacionesPlanilla.Any())
                {
                    asignacionesPlanilla.ForEach(asg =>
                    {
                        asignaciones.Add(
                                            new OUAsignacionDC
                                            {
                                                IdAsignacion = asg.PVG_IdAsignacionTula.Value,
                                                TipoAsignacion = new OUTipoAsignacionDC { IdTipoAsignacion = asg.ATP_IdTipoAsignacion, DescripcionTipoAsignacion = asg.TAS_Descripcion },
                                                NumContTransDespacho = asg.ATP_NumContTransDespacho,
                                                NumContTransRetorno = asg.ATP_NumContTransRetorno,
                                                NoPrecinto = asg.ATP_NoPrecinto,
                                                NoTula = asg.ATP_NoTula
                                            });
                    });
                }
                return asignaciones;
            }
        }

        /// <summary>
        /// Método para guardar los consolidados ingresados a centro de acopio
        /// </summary>
        /// <param name="asignacion"></param>
        /// <param name="mensajero"></param>
        /// <param name="idCentroServicios"></param>
        public void IngresoConsolidado(OUAsignacionDC asignacion, OUMensajeroDC mensajero, long idCentroServicio)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IngresoConsolidadoAgencia_OPU consolidado = new IngresoConsolidadoAgencia_OPU
                {
                    IGA_CreadoPor = ControllerContext.Current.Usuario,
                    IGA_FechaGrabacion = DateTime.Now,
                    IGA_IdAgencia = idCentroServicio,
                    IGA_IdAsignacionTula = asignacion.IdAsignacion,
                    IGA_IdMensajero = mensajero.IdMensajero,
                };
                contexto.IngresoConsolidadoAgencia_OPU.Add(consolidado);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Método para obtener una guia asociada a una planilla
        /// </summary>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        public OUPlanillaVentaGuiasDC ConsultarGuiäPlanilla(OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaVentasGuia_OPU guiaplaDB = contexto.PlanillaVentasGuia_OPU.Where(gu => gu.PVG_NumeroGuia == guiaPlanilla.NumeroGuia)
                    .FirstOrDefault();
                if (guiaplaDB != null)
                {
                    return new OUPlanillaVentaGuiasDC
                    {
                        NumeroGuia = guiaplaDB.PVG_NumeroGuia,
                        IdAdmision = guiaplaDB.PVG_IdAdminisionMensajeria,
                        EsRecomendado = guiaplaDB.PVG_EsRecomendado,
                        Peso = guiaplaDB.PVG_Peso,
                        IdUnidadNegocio = guiaplaDB.PVG_IdUnidadNegocio,
                        IdAsignacionTula = guiaplaDB.PVG_IdAsignacionTula,
                        TotalPiezasRotulo = guiaPlanilla.TotalPiezasRotulo,
                        PiezaActualRotulo = guiaPlanilla.PiezaActualRotulo,
                        IdPuntoServicio = guiaPlanilla.IdPuntoServicio,
                        IdCiudadOrigen = guiaplaDB.PVG_IdCiudadOrigenGuia,
                        IdCiudadOrigenGuia = guiaPlanilla.IdCiudadOrigenGuia,
                        DiceContener = guiaplaDB.PVG_DiceContener,
                        IdPlanilla = guiaplaDB.PVG_IdPlanilla,
                        Mensajero = guiaPlanilla.Mensajero,
                        NombreCiudadOrigenGuia = guiaPlanilla.NombreCiudadOrigenGuia,
                    };
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Método para ingresar una guia a centro de acopio
        /// </summary>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        public long GuardarIngresoGuiaAgencia(OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long? idMensajero = null;
                if (guiaPlanilla.Mensajero != null)
                {
                    idMensajero = guiaPlanilla.Mensajero.IdMensajero;
                }

                System.Data.Objects.ObjectParameter id = new System.Data.Objects.ObjectParameter("IdIngreso", typeof(long));
                contexto.paCrearIngresoGuiaAgencia_OPU(
                    guiaPlanilla.IdAdmision,
                    guiaPlanilla.IdPuntoServicio,
                    guiaPlanilla.NumeroGuia,
                    guiaPlanilla.IdPlanilla,
                    idMensajero,
                     false,
                    (short)guiaPlanilla.PiezaActualRotulo,
                    (short)guiaPlanilla.TotalPiezasRotulo,
                   DateTime.Now,
                   ControllerContext.Current.Usuario, id);
                return Convert.ToInt64(id.Value);

            }
        }

        /// <summary>
        /// Método para ingresar una novedad de una guia planillada
        /// </summary>
        /// <param name="novedad"></param>
        /// <param name="idIngresoGuia"></param>
        public void GuardarNovedadGuiaIngresada(OUNovedadIngresoDC novedad, long idIngresoGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearIngGuiaAgeNovedades_OPU
                    (novedad.IdNovedad,
                    idIngresoGuia,
                    ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Método para ingresar una guía que no se encuentra reguistrada
        /// </summary>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        public long GuardarIngresoGuiaNoAgencia(OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                System.Data.Objects.ObjectParameter id = new System.Data.Objects.ObjectParameter("IdIngreso", typeof(long));

                contexto.paCrearIngresoGuiaNoRegAgencia_OPU
                    (guiaPlanilla.IdPuntoServicio,
                    guiaPlanilla.Mensajero.IdMensajero,
                    guiaPlanilla.NumeroGuia,
                    ConstantesFramework.MinDateTimeController,
                    ControllerContext.Current.Usuario,
                    id);
                return Convert.ToInt64(id.Value);
            }
        }

        /// <summary>
        /// Método para ingresar una novedad de una guia no ingresada
        /// </summary>
        /// <param name="novedad"></param>
        /// <param name="idIngresoGuia"></param>
        public void GuardarNovedadGuiaNoIngresada(OUNovedadIngresoDC novedad, long idIngresoGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearIngNoRegNovedades_OPU(novedad.IdNovedad, idIngresoGuia, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Valida si una guia esta dentro del centro de acopio de un centro se servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="numeroGuia"></param>
        /// <returns>True: si esta ingresada la guia,  False: no esta en el centro de acopio</returns>
        public bool ValidarGuiaCentroAcopio(long numeroGuia, long idCentroServicio)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int validacion = Convert.ToInt32(contexto.paValidarGuiaCentroAcopio_OPU(numeroGuia, idCentroServicio).FirstOrDefault());
                return validacion == 0 ? false : true;
            }
        }     

        #endregion Ingreso a centro de acopio nacional
    }
}