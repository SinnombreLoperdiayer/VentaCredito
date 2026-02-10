using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Servidor.OperacionUrbana.Datos
{
    public class OURepositorioAuditoriaController : ControllerBase
    {
        #region Singleton

        private static readonly OURepositorioAuditoriaController instancia = (OURepositorioAuditoriaController)FabricaInterceptores.GetProxy(new OURepositorioAuditoriaController(), COConstantesModulos.MODULO_OPERACION_URBANA);
        public static OURepositorioAuditoriaController Instancia
        {
            get { return OURepositorioAuditoriaController.instancia; }
        }
        #endregion

        #region Variables

        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string CadCnxSispostal = ConfigurationManager.ConnectionStrings["Sispostal"].ConnectionString;

        #endregion

        #region Metodos


        #region Auditoria Mensajero Controller App
        /// <summary>
        /// Metodo para obtener las guias planillas 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZona(long idMensajero)
        {
            List<OUGuiaIngresadaAppDC> guias = new List<OUGuiaIngresadaAppDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerGuiasMensajeroEnZona_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                cmd.Parameters.AddWithValue("@EstadoGuia", "PLA");

                conn.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    guias.Add(new OUGuiaIngresadaAppDC()
                    {
                        NumeroGuia = Convert.ToInt64(r["PAG_NumeroGuia"]),
                        Planilla = Convert.ToInt64(r["PAM_IdPlanillaAsignacionEnvio"]),
                        FechaAsignacion = Convert.ToDateTime(r["PAE_FechaGrabacion"]),
                        EsAlCobro = Convert.ToBoolean(r["PAG_RecogeDinero"]),
                        IdEstadoGuia = (short)r["ADM_IdEstadoGuia"],
                        Ciudad = r["LOC_Nombre"].ToString(),
                        IdCiudad = r["PAE_IdMunicipioAsignacion"].ToString(),
                        FechaAuditoria = Convert.ToDateTime(r["ADM_FechaEstimadaEntregaNew"]),
                        DireccionDestinatario = r["ADM_DireccionDestinatario"].ToString(),
                        Servicio = new TAServicioDC
                        {
                            IdServicio = Convert.ToInt32(r["ADM_IdServicio"]),
                        },
                        TipoCliente = r["ADM_TipoCliente"].ToString()
                    });
                }

                return guias;
            }
        }

        /// <summary>
        /// Metodo para obtener las guias entregadas planilladas al mensajero  
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasMensajero(long idMensajero)
        {
            List<OUGuiaIngresadaAppDC> guias = new List<OUGuiaIngresadaAppDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerGuiasEntregadasMensajero_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                cmd.Parameters.AddWithValue("@EstadoGuia", "PLA");

                conn.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    guias.Add(new OUGuiaIngresadaAppDC()
                    {
                        NumeroGuia = Convert.ToInt64(r["PAG_NumeroGuia"]),
                        Planilla = Convert.ToInt64(r["PAM_IdPlanillaAsignacionEnvio"]),
                        FechaAsignacion = Convert.ToDateTime(r["PAE_FechaGrabacion"]),
                        EsAlCobro = Convert.ToBoolean(r["PAG_RecogeDinero"]),
                        IdEstadoGuia = (short)r["ADM_IdEstadoGuia"],
                        Ciudad = r["LOC_Nombre"].ToString(),
                        IdCiudad = r["PAE_IdMunicipioAsignacion"].ToString(),
                        FechaAuditoria = Convert.ToDateTime(r["ADM_FechaEstimadaEntregaNew"]),
                        DireccionDestinatario = r["ADM_DireccionDestinatario"].ToString(),
                        Servicio = new TAServicioDC
                        {
                            IdServicio = Convert.ToInt32(r["ADM_IdServicio"]),
                        },
                        TipoCliente = r["ADM_TipoCliente"].ToString()
                    });
                }
                return guias;
            }
        }




        /// <summary>
        /// Metodo para obtener las guias en devolucion del mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionMensajero(long idMensajero)
        {
            List<OUGuiaIngresadaAppDC> guias = new List<OUGuiaIngresadaAppDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerGuiasDevolucionMensajero_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                cmd.Parameters.AddWithValue("@EstadoGuia", "PLA");

                conn.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    guias.Add(new OUGuiaIngresadaAppDC()
                    {
                        NumeroGuia = Convert.ToInt64(r["PAG_NumeroGuia"]),
                        Planilla = Convert.ToInt64(r["PAM_IdPlanillaAsignacionEnvio"]),
                        FechaAsignacion = Convert.ToDateTime(r["PAE_FechaGrabacion"]),
                        EsAlCobro = Convert.ToBoolean(r["PAG_RecogeDinero"]),
                        IdEstadoGuia = (short)r["ADM_IdEstadoGuia"],
                        Ciudad = r["LOC_Nombre"].ToString(),
                        IdCiudad = r["PAE_IdMunicipioAsignacion"].ToString(),
                        FechaAuditoria = Convert.ToDateTime(r["ADM_FechaEstimadaEntregaNew"]),
                        DireccionDestinatario = r["ADM_DireccionDestinatario"].ToString(),
                        Servicio = new TAServicioDC
                        {
                            IdServicio = Convert.ToInt32(r["ADM_IdServicio"]),
                        },
                        TipoCliente = r["ADM_TipoCliente"].ToString()
                    });
                }
                return guias;
            }
        }


        #endregion

        #region Auditoria Auditor Controller App
        /// <summary>
        /// Metodo para obtener las guias en zona del auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEnZonaAuditor(long idAuditor)
        {
            List<OUGuiaIngresadaAppDC> guias = new List<OUGuiaIngresadaAppDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerGuiasEnZonaAuditor_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAuditor", idAuditor);

                conn.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    guias.Add(new OUGuiaIngresadaAppDC()
                    {
                        NumeroGuia = Convert.ToInt64(r["PAG_NumeroGuia"]),
                        Planilla = Convert.ToInt64(r["PAM_IdPlanillaAsignacionEnvio"]),
                        FechaAsignacion = Convert.ToDateTime(r["PAE_FechaGrabacion"]),
                        EsAlCobro = Convert.ToBoolean(r["PAG_RecogeDinero"]),
                        IdEstadoGuia = (short)r["ADM_IdEstadoGuia"],
                        Ciudad = r["LOC_Nombre"].ToString(),
                        IdCiudad = r["PAE_IdMunicipioAsignacion"].ToString(),
                        FechaAuditoria = Convert.ToDateTime(r["ADM_FechaEstimadaEntregaNew"]),
                        DireccionDestinatario = r["ADM_DireccionDestinatario"].ToString(),
                        Servicio = new TAServicioDC
                        {
                            IdServicio = Convert.ToInt32(r["ADM_IdServicio"]),
                        },
                        TipoCliente = r["ADM_TipoCliente"].ToString()
                    });
                }

                return guias;
            }
        }

        /// <summary>
        /// Metodo para obtener las guias entregadas por auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasAuditor(long idAuditor)
        {
            List<OUGuiaIngresadaAppDC> guias = new List<OUGuiaIngresadaAppDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerGuiasEntregadasAuditor_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAuditor", idAuditor);

                conn.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    guias.Add(new OUGuiaIngresadaAppDC()
                    {
                        NumeroGuia = Convert.ToInt64(r["PAG_NumeroGuia"]),
                        Planilla = Convert.ToInt64(r["PAM_IdPlanillaAsignacionEnvio"]),
                        FechaAsignacion = Convert.ToDateTime(r["PAE_FechaGrabacion"]),
                        EsAlCobro = Convert.ToBoolean(r["PAG_RecogeDinero"]),
                        IdEstadoGuia = (short)r["ADM_IdEstadoGuia"],
                        Ciudad = r["LOC_Nombre"].ToString(),
                        IdCiudad = r["PAE_IdMunicipioAsignacion"].ToString(),
                        FechaAuditoria = Convert.ToDateTime(r["ADM_FechaEstimadaEntregaNew"]),
                        DireccionDestinatario = r["ADM_DireccionDestinatario"].ToString(),
                        Servicio = new TAServicioDC
                        {
                            IdServicio = Convert.ToInt32(r["ADM_IdServicio"]),
                        },
                        TipoCliente = r["ADM_TipoCliente"].ToString()

                    });
                }
                return guias;
            }
        }

        /// <summary>
        /// Metodo para obtener las devoluciones en zona del auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionAuditor(long idAuditor)
        {
            List<OUGuiaIngresadaAppDC> guias = new List<OUGuiaIngresadaAppDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerGuiasDevolucionAuditor_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAuditor", idAuditor);

                conn.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    guias.Add(new OUGuiaIngresadaAppDC()
                    {
                        NumeroGuia = Convert.ToInt64(r["PAG_NumeroGuia"]),
                        Planilla = Convert.ToInt64(r["PAM_IdPlanillaAsignacionEnvio"]),
                        FechaAsignacion = Convert.ToDateTime(r["PAE_FechaGrabacion"]),
                        EsAlCobro = Convert.ToBoolean(r["PAG_RecogeDinero"]),
                        IdEstadoGuia = (short)r["ADM_IdEstadoGuia"],
                        Ciudad = r["LOC_Nombre"].ToString(),
                        IdCiudad = r["PAE_IdMunicipioAsignacion"].ToString(),
                        FechaAuditoria = Convert.ToDateTime(r["ADM_FechaEstimadaEntregaNew"]),
                        DireccionDestinatario = r["ADM_DireccionDestinatario"].ToString(),
                        Servicio = new TAServicioDC
                        {
                            IdServicio = Convert.ToInt32(r["ADM_IdServicio"]),
                        },
                        TipoCliente = r["ADM_TipoCliente"].ToString()
                    });
                }
                return guias;
            }
        }

        /// <summary>
        /// Metodo para obtener la guia planillada para descargue controller app 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaDescargue(long numeroGuia, long idMensajero)
        {
            OUGuiaIngresadaAppDC guia = new OUGuiaIngresadaAppDC();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerGuiaEnPlanillaControllerApp_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                cmd.Parameters.AddWithValue("@EstadoGuia", "PLA");

                conn.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    guia.NumeroGuia = r["PAG_NumeroGuia"] == DBNull.Value ? 0 : Convert.ToInt64(r["PAG_NumeroGuia"]);
                    guia.IdEstadoGuia = r["ADM_IdEstadoGuia"] == DBNull.Value ? 0 : Convert.ToInt16(r["ADM_IdEstadoGuia"]);
                    guia.DireccionDestinatario = r["ADM_DireccionDestinatario"] == DBNull.Value ? string.Empty : r["ADM_DireccionDestinatario"].ToString();
                    guia.Servicio = new TAServicioDC()
                    {
                        IdServicio = r["ADM_IdServicio"] == DBNull.Value ? 0 : Convert.ToInt16(r["ADM_IdServicio"])
                    };
                    guia.Planilla = r["PAM_IdPlanillaAsignacionEnvio"] == DBNull.Value ? 0 : Convert.ToInt64(r["PAM_IdPlanillaAsignacionEnvio"]);
                    guia.FechaAsignacion = r["PAE_FechaGrabacion"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["PAE_FechaGrabacion"]);
                    guia.FechaAuditoria = r["ADM_FechaEstimadaEntregaNew"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["ADM_FechaEstimadaEntregaNew"]);
                    guia.EsAlCobro = r["PAG_RecogeDinero"] == DBNull.Value ? false : Convert.ToBoolean(r["PAG_RecogeDinero"]);
                    guia.Ciudad = r["LOC_Nombre"] == DBNull.Value ? string.Empty : r["LOC_Nombre"].ToString();
                    guia.IdCiudad = r["PAE_IdMunicipioAsignacion"] == DBNull.Value ? string.Empty : r["PAE_IdMunicipioAsignacion"].ToString();
                    guia.TipoCliente = r["ADM_TipoCliente"] == DBNull.Value ? string.Empty : r["ADM_TipoCliente"].ToString();
                }
                return guia;
            }
        }

        /// <summary>
        /// Metodo para obtener la guia planillada para descargue auditor
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaAuditorDescargue(long numeroGuia, long idMensajero)
        {
            OUGuiaIngresadaAppDC guia = new OUGuiaIngresadaAppDC();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerGuiaEnPlanillaAuditorControllerApp_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                cmd.Parameters.AddWithValue("@EstadoGuia", "PLA");

                conn.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    guia.NumeroGuia = r["PAG_NumeroGuia"] == DBNull.Value ? 0 : Convert.ToInt64(r["PAG_NumeroGuia"]);
                    guia.IdEstadoGuia = r["ADM_IdEstadoGuia"] == DBNull.Value ? 0 : Convert.ToInt16(r["ADM_IdEstadoGuia"]);
                    guia.DireccionDestinatario = r["ADM_DireccionDestinatario"] == DBNull.Value ? string.Empty : r["ADM_DireccionDestinatario"].ToString();
                    guia.Servicio = new TAServicioDC()
                    {
                        IdServicio = r["ADM_IdServicio"] == DBNull.Value ? 0 : Convert.ToInt16(r["ADM_IdServicio"])
                    };
                    guia.Planilla = r["PAM_IdPlanillaAsignacionEnvio"] == DBNull.Value ? 0 : Convert.ToInt64(r["PAM_IdPlanillaAsignacionEnvio"]);
                    guia.FechaAsignacion = r["PAE_FechaGrabacion"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["PAE_FechaGrabacion"]);
                    guia.FechaAuditoria = r["ADM_FechaEstimadaEntregaNew"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["ADM_FechaEstimadaEntregaNew"]);
                    guia.EsAlCobro = r["PAG_RecogeDinero"] == DBNull.Value ? false : Convert.ToBoolean(r["PAG_RecogeDinero"]);
                    guia.Ciudad = r["LOC_Nombre"] == DBNull.Value ? string.Empty : r["LOC_Nombre"].ToString();
                    guia.IdCiudad = r["PAE_IdMunicipioAsignacion"] == DBNull.Value ? string.Empty : r["PAE_IdMunicipioAsignacion"].ToString();
                    guia.TipoCliente = r["ADM_TipoCliente"] == DBNull.Value ? string.Empty : r["ADM_TipoCliente"].ToString();
                }
                return guia;
            }
        }

        #endregion

        #region Sispostal - Modulo de Masivos

        /// <summary>
        /// Metodo para obtener guias por estado Sispostal
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="estado"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZonaMasivos(long idMensajero, short estado)
        {
            List<OUGuiaIngresadaAppDC> guias = new List<OUGuiaIngresadaAppDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxSispostal))
            {
                SqlCommand cmd;

                cmd = new SqlCommand(@"INF_MENSAJERO_TODOS", conn);

                if ((short)ADEnumEstadoGuiaMasivos.EnZona == estado)
                {
                    cmd.Parameters.AddWithValue("@fecha1", DateTime.Now.AddDays(-8).ToShortDateString());
                }
                else
                {
                    cmd.Parameters.AddWithValue("@fecha1", DateTime.Now.ToShortDateString());
                }
                cmd.Parameters.AddWithValue("@estado", estado);
                cmd.Parameters.AddWithValue("@msj", idMensajero);
                cmd.Parameters.AddWithValue("@fecha2", DateTime.Now.AddDays(1).ToShortDateString());
                cmd.Parameters.AddWithValue("@sucursal", "BOG");
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    bool insertar = false;
                    if (estado == Convert.ToInt32(r["Estado"]))
                    {
                        if (estado == (short)ADEnumEstadoGuiaMasivos.EnZona
                            && DBNull.Value.Equals(r["FECHA_ENTREGA"]))
                        {
                            insertar = true;
                        }
                        else if (estado != (short)ADEnumEstadoGuiaMasivos.EnZona)
                        {
                            insertar = true;
                        }
                    }
                    if (insertar)
                    {
                        guias.Add(new OUGuiaIngresadaAppDC()
                        {
                            NumeroGuia = DBNull.Value == r["Guia"] ? 0 : Convert.ToInt64(r["Guia"]),
                            Planilla = DBNull.Value == r["Manifiesto"] ? 0 : Convert.ToInt64(r["Manifiesto"]),
                            FechaAsignacion = DBNull.Value == r["Salida"] ? new DateTime() : Convert.ToDateTime(r["Salida"]),
                            EsAlCobro = false,
                            IdEstadoGuia = DBNull.Value == r["Estado"] ? 0 : Convert.ToInt32(r["Estado"]),
                            NombreEstadoGuia = DBNull.Value == r["Est_Actual"] ? "" : r["Est_Actual"].ToString(),
                            Ciudad = DBNull.Value == r["Ciudad"] ? "" : r["Ciudad"].ToString(),
                            IdCiudad = "00",
                            FechaAuditoria = DBNull.Value == r["Salida"] ? new DateTime() : Convert.ToDateTime(r["Salida"]),
                            DireccionDestinatario = r["Direccion"].ToString(),
                            Servicio = new TAServicioDC
                            {
                                IdServicio = DBNull.Value == r["Servicio"] ? 0 : Convert.ToInt32(r["Servicio"]),
                            },
                            TipoCliente = "00"
                        });
                    }
                }

                return guias;
            }
        }

        #endregion

        #endregion

    }
}
