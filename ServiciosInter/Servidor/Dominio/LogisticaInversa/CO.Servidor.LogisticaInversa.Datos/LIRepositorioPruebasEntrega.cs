using CO.Servidor.Dominio.Comun.Util;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;

namespace CO.Servidor.LogisticaInversa.Datos
{
    /// <summary>
    /// Clase que representa el repositorio de logistica inversa
    /// </summary>
    public class LIRepositorioPruebasEntrega
    {
        #region Campos

        private static readonly LIRepositorioPruebasEntrega instancia = new LIRepositorioPruebasEntrega();
        private const string NombreModelo = "ModeloLogisticaInversa";
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringRaps = ConfigurationManager.ConnectionStrings["rapsTransaccional"].ConnectionString;
        private string CadCnxSispostalController = ConfigurationManager.ConnectionStrings["Sispostal"].ConnectionString;
        private string filePath = string.Empty;

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase LIRepositorioPruebasEntrega
        /// </summary>
        public static LIRepositorioPruebasEntrega Instancia
        {
            get { return LIRepositorioPruebasEntrega.instancia; }
        }

        #endregion Propiedades

        #region Generación de Manifiesto

        #region Consultar

        /// <summary>
        /// Metodo encargado de consultar los tipos de manifiesto
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LITipoManifiestoDC> ObtenerTiposManifiesto()
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoManifiestoLogInv_LOI
                 .ToList()
                 .ConvertAll(r => new LITipoManifiestoDC
                 {
                     IdTipoManifiesto = r.TIM_IdTipoManifiesto,
                     NombreTipoManifiesto = r.TIM_Descripcion
                 });
            }
        }

        /// <summary>
        /// Metodo encargado de obtener los manifiestos asociados a una agencia
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IEnumerable<LIManifiestoDC> ObtenerManifiestosFiltro(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string IdManifiesto;
                string guiaInterna;
                string fechaInicial;
                string fechaFinal;
                string agencia;
                DateTime fechaI;
                DateTime fechaF;

                filtro.TryGetValue("MAN_IdManifiestoLogInv", out IdManifiesto);
                filtro.TryGetValue("MAN_NumeroGuiaInterna", out guiaInterna);
                filtro.TryGetValue("agencia", out agencia);
                filtro.TryGetValue("fechaInicial", out fechaInicial);
                filtro.TryGetValue("fechaFinal", out fechaFinal);

                if (fechaInicial == null)
                    fechaInicial = ConstantesFramework.MinDateTimeController.ToString();
                if (fechaFinal == null)
                    fechaFinal = DateTime.Now.ToString();

                fechaI = Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture);
                fechaF = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture);

                fechaI = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 00, 00, 00);
                fechaF = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 00);

                var manifiestos = contexto.paObtenerManifiestos_LOI(indicePagina, registrosPorPagina, Convert.ToInt64(agencia), null, Convert.ToInt64(IdManifiesto), Convert.ToInt64(guiaInterna), fechaI, fechaF)
                   .ToList();

                if (manifiestos.Any())
                {
                    return manifiestos.ConvertAll(r => new LIManifiestoDC
                    {
                        EsManifiestoAutomatico = r.MAN_EsManifiestoAutomatico,
                        FechaCreacion = r.MAN_FechaGrabacion,
                        FechaDescargue = r.MAN_FechaInicioDescarga,
                        NumeroGuiaInterna = r.MAN_NumeroGuiaInterna,
                        IdGuiaInterna = r.MAN_IdGuiaInterna == null ? 0 : (int)r.MAN_IdGuiaInterna,
                        IdManifiesto = r.MAN_IdManifiestoLogInv,
                        TipoManifiesto = new LITipoManifiestoDC { IdTipoManifiesto = r.MAN_IdTipoManifiesto, NombreTipoManifiesto = r.TIM_Descripcion },
                        CreadoPor = r.MAN_CreadoPor,
                        LocalidadDestino = new LILocalidadColDC()
                        {
                            IdLocalidad = r.MAN_IdLocalidadDestino,
                            NombreLocalidad = r.MAN_NombreLocalidadDestino,
                            IdCentroServicio = r.MAN_IdAgenciaDestino,
                            NombreCentroServicio = r.MAN_NombreAgenciaDestino,
                            Telefono = r.AGI_TelefonoDestinatario,
                            Direccion = r.AGI_DireccionDestinatario,
                        },
                        LocalidadOrigen = new LILocalidadColDC()
                        {
                            IdLocalidad = r.MAN_IdLocalidadOrigen,
                            NombreLocalidad = r.MAN_NombreLocalidadOrigen,
                            IdCentroServicio = r.MAN_IdAgenciaOrigen,
                            NombreCentroServicio = r.MAN_NombreAgenciaOrigen,
                            Telefono = r.AGI_TelefonoRemitente,
                            Direccion = r.AGI_DireccionRemitente,
                        }
                      ,
                        GuiaInterna = new ADGuiaInternaDC
                        {
                            CreadoPor = r.MAN_CreadoPor,
                            DireccionDestinatario = r.AGI_DireccionDestinatario,
                            DireccionRemitente = r.AGI_DireccionRemitente,
                            FechaGrabacion = r.MAN_FechaGrabacion,
                            IdAdmisionGuia = r.MAN_IdGuiaInterna == null ? 0 : (long)r.MAN_IdGuiaInterna,
                            GestionOrigen = new ARGestionDC
                            {
                                IdGestion = r.AGI_IdGestionOrigen == null ? 0 : (long)r.AGI_IdGestionOrigen,
                                Descripcion = r.AGI_DescripcionGestionOrig
                            },
                            GestionDestino = new ARGestionDC
                            {
                                IdGestion = r.AGI_IdGestionDestino == null ? 0 : (long)r.AGI_IdGestionDestino,
                                Descripcion = r.AGI_DescripcionGestionDest
                            },

                            IdCentroServicioDestino = r.MAN_IdAgenciaDestino,
                            IdCentroServicioOrigen = r.MAN_IdAgenciaOrigen,
                            LocalidadDestino = new PALocalidadDC
                            {
                                IdLocalidad = r.MAN_IdLocalidadDestino,
                                Nombre = r.MAN_NombreLocalidadDestino,
                            },
                            LocalidadOrigen = new PALocalidadDC
                            {
                                IdLocalidad = r.MAN_IdLocalidadOrigen,
                                Nombre = r.MAN_NombreLocalidadOrigen,
                            },
                            NombreCentroServicioDestino = r.MAN_NombreAgenciaDestino,
                            NombreCentroServicioOrigen = r.MAN_NombreAgenciaOrigen,
                            NombreDestinatario = r.MAN_NombreAgenciaDestino,
                            NombreRemitente = r.MAN_NombreAgenciaOrigen,
                            NumeroGuia = r.MAN_NumeroGuiaInterna == null ? 0 : (long)r.MAN_NumeroGuiaInterna,
                            TelefonoDestinatario = r.AGI_TelefonoDestinatario,
                            TelefonoRemitente = r.AGI_TelefonoRemitente,
                        },
                    });
                }
                else
                    return new List<LIManifiestoDC>();
            }
        }

        public void InsertarGestionAuditor(LIGestionAuditorDC gestionAuditor)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarGestionAuditor_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstadoGuiaLog", gestionAuditor.IdEstadoGuiaLog);
                cmd.Parameters.AddWithValue("@NumeroGuia", gestionAuditor.NumeroGuia);
                cmd.Parameters.AddWithValue("@IdPlanillaAsignacion", gestionAuditor.IdPlanillaAsignacion);
                cmd.Parameters.AddWithValue("@DescripcionInmueble", gestionAuditor.DescripcionInmueble);
                cmd.Parameters.AddWithValue("@NombreReceptorAuditoria", gestionAuditor.NombreReceptorAuditoria);
                cmd.Parameters.AddWithValue("@CedulaReceptorAuditoria", gestionAuditor.CedulaReceptorAuditoria);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Metodo que valida que una guía este en un manifiesto
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public bool ObtenerGuiaManifiesto(long? numeroGuia, long idManifiesto)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ManifiestoLogInvDetalle_LOI guiaDetalle = contexto.ManifiestoLogInvDetalle_LOI.Where
                  (w => w.MLD_NumeroAdmision == numeroGuia && w.MLD_IdManifiesto == idManifiesto)
                  .FirstOrDefault();
                if (guiaDetalle == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Método que conulta la descripción de un tipo de manifiesto
        /// </summary>
        /// <param name="idTipoManifiesto"></param>
        /// <returns></returns>
        public string ConsultaNombreTipoManifiesto(short idTipoManifiesto)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoManifiestoLogInv_LOI.Where(j => j.TIM_IdTipoManifiesto == idTipoManifiesto).FirstOrDefault().TIM_Descripcion;
            }
        }

        #endregion Consultar

        #region Adicionar

        /// <summary>
        /// Metodo para adicionar manifiestos
        /// </summary>
        /// <param name="manifiesto"></param>
        /// <returns>id del manifiesto generado</returns>
        public long AdicionarManifiesto(LIManifiestoDC manifiesto)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (manifiesto.GuiaInterna == null)
                {
                    var numeroManifiesto = contexto.paInsertarManifiesto_LOI
                         (manifiesto.TipoManifiesto.IdTipoManifiesto,
                         manifiesto.LocalidadOrigen.IdLocalidad,
                         manifiesto.LocalidadOrigen.NombreLocalidad,
                         manifiesto.LocalidadDestino.IdLocalidad,
                         manifiesto.LocalidadDestino.NombreLocalidad,
                         manifiesto.LocalidadOrigen.IdCentroServicio,
                         manifiesto.LocalidadOrigen.NombreCentroServicio,
                         manifiesto.LocalidadDestino.IdCentroServicio,
                         manifiesto.LocalidadDestino.NombreCentroServicio,
                         null,
                         null,
                         manifiesto.EsManifiestoAutomatico,
                         DateTime.Now, manifiesto.FechaDescargue,
                         ControllerContext.Current.Usuario).FirstOrDefault();
                    if (numeroManifiesto.HasValue)
                        return Convert.ToInt64(numeroManifiesto.Value);
                    else
                        return 0;
                }
                else
                {
                    var numeroManifiesto = contexto.paInsertarManifiesto_LOI
                      (manifiesto.TipoManifiesto.IdTipoManifiesto,
                      manifiesto.LocalidadOrigen.IdLocalidad,
                      manifiesto.LocalidadOrigen.NombreLocalidad,
                      manifiesto.LocalidadDestino.IdLocalidad,
                      manifiesto.LocalidadDestino.NombreLocalidad,
                      manifiesto.LocalidadOrigen.IdCentroServicio,
                      manifiesto.LocalidadOrigen.NombreCentroServicio,
                      manifiesto.LocalidadDestino.IdCentroServicio,
                      manifiesto.LocalidadDestino.NombreCentroServicio,
                      manifiesto.GuiaInterna.IdAdmisionGuia,
                      manifiesto.GuiaInterna.NumeroGuia,
                      manifiesto.EsManifiestoAutomatico,
                      DateTime.Now, manifiesto.FechaDescargue,
                      ControllerContext.Current.Usuario).FirstOrDefault();
                    if (numeroManifiesto.HasValue)
                        return Convert.ToInt64(numeroManifiesto.Value);
                    else
                        return 0;
                }
            }
        }


        /// <summary>
        /// Metodo para adicionar guías a un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        public void AdicionarGuiaManifiesto(LIGuiaDC guia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paInsertarGuiaManifiesto_LOI
                 (guia.IdManifiesto,
                  guia.IdGuia,
                  guia.NumeroGuia,
                  guia.IdServicio,
                  guia.EstaDescargada,
                  guia.ManifestadaOrigen,
                  guia.FechaDescarga,
                  guia.UsuarioDescarga,
                  guia.EstadoGuia.Trim(),
                  DateTime.Now,
                  ControllerContext.Current.Usuario);
            }
        }

        #endregion Adicionar

        #region Eliminar

        /// <summary>
        /// Eliminar manifiesto
        /// </summary>
        /// <param name="manifiesto"></param>
        public void EliminarManifiesto(LIManifiestoDC manifiesto)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarManifiesto_LOI(manifiesto.IdManifiesto, ControllerContext.Current.Usuario, LOIConstantesLogisticaInversa.ACCION_ELIMINAR);
            }
        }

        /// <summary>
        ///  Método que elimina una guía de un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        public void EliminarGuiaManifiesto(LIGuiaDC guia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ManifiestoLogInvDetalle_LOI elimina = contexto.ManifiestoLogInvDetalle_LOI
                .Where(r => r.MLD_NumeroAdmision == guia.NumeroGuia && r.MLD_IdManifiesto == guia.IdManifiesto)
                .FirstOrDefault();

                if (elimina == null)
                {
                    ControllerException excepcion =
                    new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                      LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE.ToString(),
                      LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    contexto.ManifiestoLogInvDetalle_LOI.Remove(elimina);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método encargado de auditar la eliminación de una detalle en unamanifiesto
        /// </summary>
        /// <param name="guia"></param>
        public void AuditarGuia(LIGuiaDC guia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ManifiestoLogInvDetalle_LOI manifiestoDetalle = contexto.ManifiestoLogInvDetalle_LOI
                  .Where(r => r.MLD_NumeroAdmision == guia.NumeroGuia && r.MLD_IdManifiesto == guia.IdManifiesto)
                  .FirstOrDefault();

                if (manifiestoDetalle != null)
                {
                    ManifiestoLogInvDetallHist_LOI guiaHistorico = new ManifiestoLogInvDetallHist_LOI
                    {
                        MLD_IdAdminision = manifiestoDetalle.MLD_IdAdminision,
                        MLD_IdManifiesto = manifiestoDetalle.MLD_IdManifiesto,
                        MLD_IdManifiestoLogInvDetalle = manifiestoDetalle.MLD_IdManifiestoLogInvDetalle,
                        MLD_CambiadoPor = ControllerContext.Current.Usuario,
                        MLD_CreadoPor = manifiestoDetalle.MLD_CreadoPor,
                        MLD_EstaDescargada = manifiestoDetalle.MLD_EstaDescargada,
                        MLD_EstadoEnManifiesto = manifiestoDetalle.MLD_EstadoEnManifiesto,
                        MLD_FechaCambio = DateTime.Now,
                        MLD_FechaDescarga = manifiestoDetalle.MLD_FechaDescarga,
                        MLD_FechaGrabacion = manifiestoDetalle.MLD_FechaGrabacion,
                        MLD_FueManifestadaEnOrigen = manifiestoDetalle.MLD_FueManifestadaEnOrigen,
                        MLD_IdServicio = manifiestoDetalle.MLD_IdServicio,
                        MLD_NumeroAdmision = manifiestoDetalle.MLD_NumeroAdmision,
                        MLD_TipoCambio = LOIConstantesLogisticaInversa.ACCION_ELIMINAR,
                        MLD_UsuarioDescarga = manifiestoDetalle.MLD_UsuarioDescarga,
                    };
                    contexto.ManifiestoLogInvDetallHist_LOI.Add(guiaHistorico);
                    contexto.SaveChanges();
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        #endregion Eliminar

        #endregion Generación de Manifiesto

        #region Descarga de Manifiesto

        #region Consultar

        /// <summary>
        /// Metodo encargado de obtener los manifiestos asociados a un Col destino
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IEnumerable<LIManifiestoDC> ObtenerManifiestosDestinoFiltro(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string IdManifiesto;
                string guiaInterna;
                string fechaInicial;
                string fechaFinal;
                string agencia;
                DateTime fechaI;
                DateTime fechaF;

                filtro.TryGetValue("MAN_IdManifiestoLogInv", out IdManifiesto);
                filtro.TryGetValue("MAN_NumeroGuiaInterna", out guiaInterna);
                filtro.TryGetValue("agencia", out agencia);
                filtro.TryGetValue("fechaInicial", out fechaInicial);
                filtro.TryGetValue("fechaFinal", out fechaFinal);

                if (fechaInicial == null)
                    fechaInicial = ConstantesFramework.MinDateTimeController.ToString();
                if (fechaFinal == null)
                    fechaFinal = DateTime.Now.ToString();

                fechaI = Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture);
                fechaF = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture);

                fechaI = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 00, 00, 00);
                fechaF = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 00);

                var manifiestos = contexto.paObtenerManifiestos_LOI(indicePagina, registrosPorPagina, 0, Convert.ToInt64(agencia), Convert.ToInt64(IdManifiesto), Convert.ToInt64(guiaInterna), fechaI, fechaF)
                   .ToList();

                if (manifiestos.Any())
                {
                    return manifiestos.ConvertAll(r => new LIManifiestoDC
                    {
                        EsManifiestoAutomatico = r.MAN_EsManifiestoAutomatico,
                        FechaCreacion = r.MAN_FechaGrabacion,
                        FechaDescargue = r.MAN_FechaInicioDescarga,
                        NumeroGuiaInterna = r.MAN_NumeroGuiaInterna,
                        IdGuiaInterna = r.MAN_IdGuiaInterna == null ? 0 : (long)r.MAN_IdGuiaInterna,
                        IdManifiesto = r.MAN_IdManifiestoLogInv,
                        TipoManifiesto = new LITipoManifiestoDC { IdTipoManifiesto = r.MAN_IdTipoManifiesto, NombreTipoManifiesto = r.TIM_Descripcion },
                        CreadoPor = r.MAN_CreadoPor,
                        LocalidadDestino = new LILocalidadColDC()
                        {
                            IdLocalidad = r.MAN_IdLocalidadDestino,
                            NombreLocalidad = r.MAN_NombreLocalidadDestino,
                            IdCentroServicio = r.MAN_IdAgenciaDestino,
                            NombreCentroServicio = r.MAN_NombreAgenciaDestino,
                            Telefono = r.AGI_TelefonoDestinatario,
                            Direccion = r.AGI_DireccionDestinatario,
                        },
                        LocalidadOrigen = new LILocalidadColDC()
                        {
                            IdLocalidad = r.MAN_IdLocalidadOrigen,
                            NombreLocalidad = r.MAN_NombreLocalidadOrigen,
                            IdCentroServicio = r.MAN_IdAgenciaOrigen,
                            NombreCentroServicio = r.MAN_NombreAgenciaOrigen,
                            Telefono = r.AGI_TelefonoRemitente,
                            Direccion = r.AGI_DireccionRemitente,
                        }
                      ,
                        GuiaInterna = new ADGuiaInternaDC
                        {
                            CreadoPor = r.MAN_CreadoPor,
                            DireccionDestinatario = r.AGI_DireccionDestinatario,
                            DireccionRemitente = r.AGI_DireccionRemitente,
                            FechaGrabacion = r.MAN_FechaGrabacion,
                            IdAdmisionGuia = r.MAN_IdGuiaInterna == null ? 0 : (long)r.MAN_IdGuiaInterna,
                            GestionOrigen = new ARGestionDC
                            {
                                IdGestion = r.AGI_IdGestionOrigen == null ? 0 : (long)r.AGI_IdGestionOrigen,
                                Descripcion = r.AGI_DescripcionGestionOrig
                            },
                            GestionDestino = new ARGestionDC
                            {
                                IdGestion = r.AGI_IdGestionDestino == null ? 0 : (long)r.AGI_IdGestionDestino,
                                Descripcion = r.AGI_DescripcionGestionDest
                            },

                            IdCentroServicioDestino = r.MAN_IdAgenciaDestino,
                            IdCentroServicioOrigen = r.MAN_IdAgenciaOrigen,
                            LocalidadDestino = new PALocalidadDC
                            {
                                IdLocalidad = r.MAN_IdLocalidadDestino,
                                Nombre = r.MAN_NombreLocalidadDestino,
                            },
                            LocalidadOrigen = new PALocalidadDC
                            {
                                IdLocalidad = r.MAN_IdLocalidadOrigen,
                                Nombre = r.MAN_NombreLocalidadOrigen,
                            },
                            NombreCentroServicioDestino = r.MAN_NombreAgenciaDestino,
                            NombreCentroServicioOrigen = r.MAN_NombreAgenciaOrigen,
                            NombreDestinatario = r.MAN_NombreAgenciaDestino,
                            NombreRemitente = r.MAN_NombreAgenciaOrigen,
                            NumeroGuia = r.MAN_NumeroGuiaInterna == null ? 0 : (long)r.MAN_NumeroGuiaInterna,
                            TelefonoDestinatario = r.AGI_TelefonoDestinatario,
                            TelefonoRemitente = r.AGI_TelefonoRemitente,
                        },
                    });
                }
                else
                {
                    return new List<LIManifiestoDC>();
                }
            }
        }

        public bool ActualizarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paActualizarLecturaPruebaEntregaEC_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@IdCodigoProceso", SqlDbType.VarChar));
                cmd.Parameters["@IdCodigoProceso"].Value = archivoPruebaEntrega.IdCodigoProceso;
                cmd.Parameters.Add(new SqlParameter("@FechaRecepcion", SqlDbType.DateTime));
                cmd.Parameters["@FechaRecepcion"].Value = archivoPruebaEntrega.FechaRecepcion;
                cmd.Parameters.Add(new SqlParameter("@FechaLeida", SqlDbType.DateTime));
                cmd.Parameters["@EstadoEnvio"].Value = archivoPruebaEntrega.EstadoEnvio;
                cmd.Parameters.Add(new SqlParameter("@NumeroFormato", SqlDbType.BigInt));
                cmd.Parameters["@NumeroFormato"].Value = archivoPruebaEntrega.NumeroFormato;

                var result = cmd.ExecuteNonQuery();
                sqlConn.Close();
                return result == 1;
            }
        }

        public bool ValidarRecepcionHistoricoEcapture(long numeroGuia, string codigoProceso)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarHistoricoEcapture_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroFormato", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdCodigoProceso", codigoProceso));
                var result = cmd.ExecuteReader();

                if (result.HasRows)
                {
                    sqlConn.Close();
                    return true;
                }

                sqlConn.Close();
                return false;
            }
        }

        public bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarLecturaPruebaEntregaEC_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@FechaRegistro", SqlDbType.DateTime));
                cmd.Parameters["@FechaRegistro"].Value = archivoPruebaEntrega.FechaRegistro;
                cmd.Parameters.Add(new SqlParameter("@IdCodigoProceso", SqlDbType.VarChar));
                cmd.Parameters["@IdCodigoProceso"].Value = archivoPruebaEntrega.IdCodigoProceso;
                cmd.Parameters.Add(new SqlParameter("@FechaRecepcion", SqlDbType.DateTime));
                cmd.Parameters["@FechaRecepcion"].Value = archivoPruebaEntrega.FechaRecepcion;
                cmd.Parameters.Add(new SqlParameter("@FechaLeida", SqlDbType.DateTime));
                cmd.Parameters["@FechaLeida"].Value = archivoPruebaEntrega.FechaLeida;
                cmd.Parameters.Add(new SqlParameter("@EstadoEnvio", SqlDbType.Int));
                cmd.Parameters["@EstadoEnvio"].Value = archivoPruebaEntrega.EstadoEnvio;
                cmd.Parameters.Add(new SqlParameter("@NumeroFormato", SqlDbType.BigInt));
                cmd.Parameters["@NumeroFormato"].Value = archivoPruebaEntrega.NumeroFormato;

                var result = cmd.ExecuteNonQuery();

                sqlConn.Close();
                return result == 1;
            }
        }

        /// <summary>
        /// Verifica si la guia ya esta digitalizada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ArchivoGuia> VerificarGuia(string numeroGuia)
        {
            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmdTablasSync = new SqlCommand();
                SqlDataAdapter daTablasSync = new SqlDataAdapter();
                cmdTablasSync.CommandText = @"paObtenerArchivoGuiaFileSystem_LOI";
                cmdTablasSync.Connection = cnn;
                cmdTablasSync.CommandType = CommandType.StoredProcedure;
                cmdTablasSync.Parameters.Add(new SqlParameter("@NumeroGuia", Convert.ToInt64(numeroGuia)));

                DataSet ds = new DataSet();
                daTablasSync.SelectCommand = cmdTablasSync;
                daTablasSync.Fill(ds);

                List<ArchivoGuia> lst = ds.Tables[0].AsEnumerable().ToList().ConvertAll(t =>
                     new ArchivoGuia()
                     {
                         IdAdmisionMensajeria = t.Field<long>("ARG_IdAdminisionMensajeria", DataRowVersion.Current),
                         FechaGrabacion = t.Field<DateTime>("ARG_FechaGrabacion", DataRowVersion.Current),
                         IdArchivo = t.Field<long>("ARG_IdArchivo", DataRowVersion.Current),
                         IdCentroLogistico = t.Field<long>("ARG_IdCentroLogistico", DataRowVersion.Current),
                         ImagenSincronizada = t.Field<bool>("ARG_ImagenSincronizada", DataRowVersion.Current),
                         NumeroGuia = t.Field<long>("ARG_NumeroGuia", DataRowVersion.Current),
                         RutaArchivo = t.Field<string>("ARG_RutaArchivo", DataRowVersion.Current),
                         CreadoPor = t.Field<string>("ARG_CreadoPor", DataRowVersion.Current)
                     });

                cnn.Close();
                return lst;
            }
        }


        /// <summary>
        /// Actualiza el archivo guia indicando que ya esta digitalizada
        /// </summary>
        /// <param name="archivoGuia"></param>
        public void ActualizarArchivoGuiaDigitalizada(ArchivoGuia archivoGuia)
        {
            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                cnn.Open();
                SqlCommand cmdTablasSync = new SqlCommand();
                cmdTablasSync.CommandText = @"paActualizarArchivoGuiaFileSystem_LOI";
                cmdTablasSync.Connection = cnn;
                cmdTablasSync.CommandType = CommandType.StoredProcedure;
                cmdTablasSync.Parameters.Add(new SqlParameter("@NumeroGuia", archivoGuia.NumeroGuia));
                cmdTablasSync.Parameters.Add(new SqlParameter("@RutaArchivo", archivoGuia.RutaArchivo));
                cmdTablasSync.ExecuteNonQuery();
                cnn.Close();
            }
        }

        /// <summary>
        /// Inserta historico el archivo guia indicando que ya esta digitalizada
        /// </summary>
        /// <param name="archivoGuia"></param>
        public void InsertaHistoricoArchivoGuiaDigitalizada(ArchivoGuia archivoGuia)
        {
            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                cnn.Open();
                SqlCommand cmdTablasSync = new SqlCommand();
                cmdTablasSync.CommandText = @"paInsertarHistoricoArchivoGuiaFileSystem_LOI";
                cmdTablasSync.Connection = cnn;
                cmdTablasSync.CommandType = CommandType.StoredProcedure;
                cmdTablasSync.Parameters.Add("@ARG_NumeroGuia", SqlDbType.BigInt).Value = archivoGuia.NumeroGuia;
                cmdTablasSync.Parameters.Add("@ARG_IdCentroLogistico", SqlDbType.BigInt).Value = archivoGuia.IdCentroLogistico;
                cmdTablasSync.Parameters.Add("@ARG_RutaArchivo", SqlDbType.VarChar).Value = archivoGuia.RutaArchivo;
                cmdTablasSync.Parameters.Add("@ARG_IdAdminisionMensajeria", SqlDbType.BigInt).Value = archivoGuia.IdAdmisionMensajeria;
                cmdTablasSync.Parameters.Add("@ARG_CreadoPor", SqlDbType.VarChar).Value = archivoGuia.CreadoPor;
                cmdTablasSync.ExecuteNonQuery();
                cnn.Close();
            }
        }

        //VOLANTES
        /// <summary>
        /// Verifica si la guia ya esta digitalizada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ArchivoVolante> VerificarVolante(string numeroVolante)
        {
            List<ArchivoVolante> lst = new List<ArchivoVolante>();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet ds = new DataSet();

            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                SqlCommand comando = new SqlCommand(@"paObtenerArchivoEvidendia_MEN", cnn);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add(new SqlParameter("@NumeroVolante", Convert.ToInt64(numeroVolante)));

                adapter.SelectCommand = comando;
                adapter.Fill(ds);

                lst = ds.Tables[0].AsEnumerable().ToList().ConvertAll(t =>
                     new ArchivoVolante()
                     {
                         IdArchivo = t.Field<int>("ARV_IdEvidencia", DataRowVersion.Current),
                         FechaGrabacion = t.Field<DateTime>("ARV_FechaGrabacion", DataRowVersion.Current),

                         //IdCentroLogistico = t.Field<long>("ARG_IdCentroLogistico", DataRowVersion.Current),
                         //ImagenSincronizada = t.Field<bool>("ARG_ImagenSincronizada", DataRowVersion.Current),
                         //NumeroVolante = t.Field<long>("ARG_NumeroGuia", DataRowVersion.Current),
                         //RutaArchivo = t.Field<string>("ARG_RutaArchivo", DataRowVersion.Current),
                         //CreadoPor = t.Field<string>("ARG_CreadoPor", DataRowVersion.Current)
                     });

                cnn.Close();
            }
            return lst;

        }

        /// <summary>
        /// Actualiza el archivo guia indicando que ya esta digitalizada
        /// </summary>
        /// <param name="archivoGuia"></param>
        public void ActualizarArchivoVolanteSincronizado(ArchivoVolante archivoVolante)
        {
            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                cnn.Open();
                SqlCommand cmdTablasSync = new SqlCommand("paActualizaArchivoVolanteSincronizado_LOI", cnn);
                cmdTablasSync.CommandType = CommandType.StoredProcedure;
                cmdTablasSync.Parameters.Add(new SqlParameter("@IdArchivoEvidencia", archivoVolante.IdArchivo));
                cmdTablasSync.Parameters.Add(new SqlParameter("@RutaArchivo", archivoVolante.RutaArchivo));

                cmdTablasSync.ExecuteNonQuery();
                cnn.Close();
            }
        }

        public int ConsultarOrigenGuia(long numeroGuia)
        {
            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                cnn.Open();
                SqlCommand cmdTablasSync = new SqlCommand("paObtenerOrigenAdmisionPorNumeroGuia_MEN", cnn);
                cmdTablasSync.CommandType = CommandType.StoredProcedure;
                cmdTablasSync.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));

                var result = cmdTablasSync.ExecuteReader();
                if (result.Read())
                {
                    return Convert.ToInt32(result["IA_IdOrigenAplicacion"]);
                }
                return 0;


            }

        }


        public LIArchivoGuiaMensajeriaDC ArchivarGuiaPruebaEntregaWPF(ADGuia guia)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Metodo para obtener las guias por manifiesto
        /// </summary>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public IEnumerable<LIGuiaDC> ObtenerGuiasManifiestoDescarga(long idManifiesto)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ManifiestoLogisticaInversa_LOI manifiesto = contexto.ManifiestoLogisticaInversa_LOI.Where(j => j.MAN_IdManifiestoLogInv == idManifiesto).FirstOrDefault();

                var guias = contexto.paObtenerGuiaManifDescarga_LOI(idManifiesto)
                  .OrderBy(o => o.MLD_NumeroAdmision)
                  .ThenBy(t => t.MLD_EstadoEnManifiesto).ToList();

                if (guias.Any())
                {
                    return guias
                    .ConvertAll(r => new LIGuiaDC
                    {
                        EstaDescargada = r.MLD_EstaDescargada,
                        FechaDescarga = r.MLD_FechaDescarga,
                        NumeroGuia = r.MLD_NumeroAdmision,
                        IdGuia = r.MLD_IdAdminision,
                        IdServicio = r.MLD_IdServicio,
                        IdManifiestoGuia = r.MLD_IdManifiestoLogInvDetalle,
                        IdManifiesto = r.MLD_IdManifiesto,
                        UsuarioDescarga = r.MLD_UsuarioDescarga,
                        EstadoGuia = r.MLD_EstadoEnManifiesto,
                        ManifestadaOrigen = r.MLD_FueManifestadaEnOrigen,
                        DescripcionEstadoGuia = r.DescripcionEstadoManifiesto,
                        CreadoPor = r.MLD_CreadoPor,
                        FechaGrabacion = r.MLD_FechaGrabacion,
                        FechaDescargaString = r.MLD_FechaDescarga.ToString()
                    });
                }
                else
                {
                    if (manifiesto.MAN_EsManifiestoAutomatico == false)
                    {
                        return new List<LIGuiaDC>();
                    }
                    else
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                          LOIEnumTipoErrorLogisticaInversa.EX_MANIFIESTO_SIN_GUIAS.ToString(),
                          LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_MANIFIESTO_SIN_GUIAS));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
            }
        }

        /// <summary>
        /// Metodo que obtiene las guías no descargadas de los manifiestos del dia anterior
        /// </summary>
        /// <returns>lista con los numeros de guía</returns>
        public List<long> ObtenerGuiasNoDescargadas()
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<long> listaGuias = new List<long>();
                var guiasNoDescargadas = contexto.ManifiestoDetalleLogInv_VLOI
                  .Where(d => d.MLD_FechaDescarga == DateTime.Now.AddDays(-1).Date && d.MLD_EstaDescargada == false)
                  .ToList();
                if (guiasNoDescargadas != null)
                {
                    foreach (ManifiestoDetalleLogInv_VLOI guia in guiasNoDescargadas)
                    {
                        listaGuias.Add(guia.MLD_IdManifiesto);
                    }
                }
                return listaGuias;
            }
        }

        /// <summary>
        /// Metodo que obtiene las guías descargadas con estado mal diligenciada
        /// </summary>
        /// <returns>lista con los numeros de guía</returns>
        public List<long> ObtenerGuiasMalDiligenciadas()
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<long> listaGuias = new List<long>();
                var guiasNoDescargadas = contexto.ManifiestoDetalleLogInv_VLOI
                  .Where(d => d.MLD_FechaDescarga == DateTime.Now.AddDays(-1).Date && d.MLD_EstadoEnManifiesto == LOIConstantesLogisticaInversa.ENTREGA_MAL_DILIGENCIADA)
                  .ToList();
                if (guiasNoDescargadas != null)
                {
                    foreach (ManifiestoDetalleLogInv_VLOI guia in guiasNoDescargadas)
                    {
                        listaGuias.Add(guia.MLD_IdManifiesto);
                    }
                }
                return listaGuias;
            }
        }


        #endregion Consultar

        #region Actualizar

        /// <summary>
        /// Método encargado de actualizar el estado de una guía en un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        public void ActualizarEstadoGuiaManifiesto(LIGuiaDC guia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarEstadoGuiaMan_LOI(guia.IdManifiestoGuia, guia.NuevoEstadoGuia, guia.EstaDescargada, DateTime.Now, guia.UsuarioDescarga);
            }
        }

        /// <summary>
        /// Método encargado de actualizar el inicio de la fecha de descarga de un manifiesto
        /// </summary>
        public void ActualizarManifiesto(long idManifiesto)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarFecDescMani_LOI(idManifiesto);
            }
        }


        public void ActualizarGuiaManifiesto(long numeroGuia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ManifiestoLogInvDetalle_LOI guia = contexto.ManifiestoLogInvDetalle_LOI
                    .Where(gu => gu.MLD_NumeroAdmision == numeroGuia)
                    .FirstOrDefault();

                if (guia != null)
                {
                    guia.MLD_EstaDescargada = true;
                    guia.MLD_EstadoEnManifiesto = LOIConstantesLogisticaInversa.ENTREGA_EXITOSA;
                    contexto.SaveChanges();
                }

            }
        }

        #endregion Actualizar

        #endregion Descarga de Manifiesto

        #region Descargue WPF

        /// <summary>
        /// Metodo para guardar un volante de devolución
        /// </summary>
        /// <param name="evidenciaDevolucion"></param>
        /// <returns></returns>
        public void AdicionarEvidenciaDevolucion(LIEvidenciaDevolucionDC evidenciaDevolucion)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paCrearEvidenciaDevolucion_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numeroEvidencia", evidenciaDevolucion.NumeroEvidencia));
                cmd.Parameters.Add(new SqlParameter("@idEstadoGuiaLog", evidenciaDevolucion.IdEstadoGuialog));
                cmd.Parameters.Add(new SqlParameter("@idTipoEvidencia", evidenciaDevolucion.TipoEvidencia.IdTipo));
                cmd.Parameters.Add(new SqlParameter("@estaDigitalizado ", evidenciaDevolucion.EstaDigitalizado));
                cmd.Parameters.Add(new SqlParameter("@descripcionTipoEvidencia", evidenciaDevolucion.TipoEvidencia.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@creadoPor", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }



        /// <summary>
        /// Metodo para modificar un volante de devolución
        /// </summary>
        /// <param name="evidenciaDevolucion"></param>
        /// <returns></returns>
        public void ModificarEvidenciaDevolucion(LIEvidenciaDevolucionDC evidenciaDevolucion)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paModificarEvidenciaDevolucion_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numeroEvidencia", evidenciaDevolucion.NumeroEvidencia));
                cmd.Parameters.Add(new SqlParameter("@idEstadoGuiaLog", evidenciaDevolucion.IdEstadoGuialog));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }


        /// <summary>
        /// ´Método para validar que un numero de evidencia no exista
        /// </summary>
        /// <param name="numeroEvidencia"></param>
        /// <returns></returns>
        public bool ValidarEvidenciaDevolucion(long numeroEvidencia, short idTipoEvidencia)
        {
            //todo:walter hacer sp
            string query = "SELECT 1 FROM EvidenciaDevolucion_MEN WITH (NOLOCK) WHERE VOD_NumeroEvidencia = " + numeroEvidencia.ToString().Trim() + " AND VOD_IdTipoEvidenciaDevolucion = " + idTipoEvidencia.ToString().Trim();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.CommandType = CommandType.Text;

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                    return false;
            }
        }


        /// <summary>
        /// Método para ingresar una novedad de logistica inversa
        /// </summary>
        /// <param name="novedad"></param>
        public void IngresarNovedad(CONovedadGuiaDC novedad)
        {
            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paCrearNovedadGuia_LOI", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", novedad.NumeroGuia);
                cmd.Parameters.AddWithValue("@idNovedad", novedad.TipoNovedad.IdTipoNovedadGuia);
                cmd.Parameters.AddWithValue("@idCentroServicio", ControllerContext.Current.IdCentroServicio);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }

        #endregion

        #region Valida guia asignada a reclame en oficina

        /// <summary>
        /// Metodo para validar guia asignada a reclame en oficina
        /// </summary>
        /// <param name="IdCentroServicio"></param>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public PUMovimientoInventario ConsultaUltimoMovimientoBodegaGuia(long numeroGuia)
        {
            PUMovimientoInventario MovimientoINV = null;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarUltimoMovimientoGuia_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@INV_NumeroGuia", numeroGuia));

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                MovimientoINV = new PUMovimientoInventario();
                if (dt.Rows.Count > 0)
                {
                    MovimientoINV.TipoMovimiento = (PUEnumTipoMovimientoInventario)Convert.ToInt64(dt.Rows[0]["INV_IdTipoMovimiento"]);
                    MovimientoINV.FechaGrabacion = Convert.ToDateTime(dt.Rows[0]["INV_FechaGrabacion"]);

                    MovimientoINV.Bodega = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = Convert.ToInt32(dt.Rows[0]["INV_IdCentroServicio"])
                    };
                }
                sqlConn.Close();
            }
            return MovimientoINV;
        }


        #endregion

        #region Ingreso Reclame en Oficina

        /// <summary>
        /// Metodo para adicionar guia en reclame en oficina
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public void AdicionarReclameEnOficina(LIReclameEnOficinaDC reclameEnOficina)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paAdicionarReclameEnOficina_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@REO_IdReclameEnOficina", reclameEnOficina.IdReclameEnOficina));
                cmd.Parameters.Add(new SqlParameter("@REO_Ubicacion", reclameEnOficina.Ubicacion));
                cmd.Parameters.Add(new SqlParameter("@REO_TipoUbicacion", reclameEnOficina.TipoUbicacion));
                cmd.Parameters.Add(new SqlParameter("@REO_CreadoPor", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Metodo para adicionar la evidencia de una entrega reclame en oficina
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public void InsertarEvidenciaEntregaReclameEnOficina(LIReclameEnOficinaDC reclameEnOficina, string rutaImagen)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarImagenEvicenciaEntregaReclamaOficina_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@EER_NumeroGuia", reclameEnOficina.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@EER_RutaImagen", rutaImagen));
                cmd.Parameters.Add(new SqlParameter("@EER_CreadoPor", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }




        #endregion

        #region Consulta Guias Reclame en Oficina

        /// <summary>
        /// Metodo para consultar los envios reclame en oficina por punto y tipoMovimiento
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public List<LIReclameEnOficinaDC> ConsultarGuiasReclameEnOficina(long idCentroServicio, int idTipoMovimiento, bool filtroDevolucion)
        {
            List<LIReclameEnOficinaDC> ListaReclameEnOficina = new List<LIReclameEnOficinaDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarReclameEnOficina_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@INV_IdCentroServicio", idCentroServicio));
                cmd.Parameters.Add(new SqlParameter("@FiltroDevolucion", filtroDevolucion));
                cmd.Parameters.Add(new SqlParameter("@idTipoMovimiento", idTipoMovimiento));
                SqlDataReader read = cmd.ExecuteReader();
                ListaReclameEnOficina = new List<LIReclameEnOficinaDC>();
                while (read.Read())
                {
                    LIReclameEnOficinaDC objREO = new LIReclameEnOficinaDC();
                    objREO.NumeroGuia = Convert.ToInt64(read["ADM_NumeroGuia"]);
                    objREO.Peso = Convert.ToInt64(read["ADM_Peso"]);
                    objREO.DocumentoDestinatario = read["ADM_IdDestinatario"].ToString();
                    objREO.NombreDestinatario = read["ADM_NombreDestinatario"].ToString();
                    objREO.TelefonoDestinatario = read["ADM_TelefonoDestinatario"].ToString();
                    objREO.EsAlCobro = Convert.ToBoolean(read["ADM_EsAlCobro"]);
                    objREO.EstaEntregada = Convert.ToBoolean(read["ADM_EstaEntregada"]);
                    objREO.ValorTotal = Convert.ToDecimal(read["ADM_ValorTotal"]);
                    objREO.DiceContener = read["ADM_DiceContener"].ToString();
                    objREO.DiasTranscurridos = Convert.ToInt32(read["DiasTranscurridos"]);

                    objREO.FormaPago = new ADGuiaFormaPago();
                    if (objREO.DiasTranscurridos >= 30)
                        objREO.EstadoDevolucion = true;
                    else
                        objREO.EstadoDevolucion = false;

                    if (read["REO_TipoUbicacion"] != DBNull.Value)
                    {
                        objREO.TipoUbicacion = (PUEnumTipoUbicacion)Convert.ToInt32(read["REO_TipoUbicacion"]);
                        objREO.Ubicacion = Convert.ToInt32(read["REO_Ubicacion"]);
                        objREO.UbicacionDetalle = objREO.TipoUbicacion.ToString() + "  " + objREO.Ubicacion.ToString();
                    }

                    objREO.FechaAsignacion = Convert.ToDateTime(read["FechaAsignacion"]);
                    objREO.UsuarioAsigna = read["UsuarioAsigna"].ToString();

                    if (read["REO_FechaCreacion"] != DBNull.Value)
                    {
                        objREO.FechaINGRESO = Convert.ToDateTime(read["REO_FechaCreacion"]);
                    }

                    objREO.EstadoGuiaenPRO = (PUEnumTipoMovimientoInventario)Convert.ToInt32(read["INV_IdTipoMovimiento"].ToString());


                    ListaReclameEnOficina.Add(objREO);
                }
            }

            if (ListaReclameEnOficina.Any())
                return ListaReclameEnOficina.OrderBy(x => x.DiasTranscurridos).ToList();
            else
                return ListaReclameEnOficina;
        }



        /// <summary>
        /// consulta las guias reclame en oficina utilizando filtros
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public List<LIReclameEnOficinaDC> ConsultarGuiasReclameEnOficinaFiltros(long idCentroServicio, Dictionary<string, string> filtros)
        {
            List<LIReclameEnOficinaDC> ListaReclameEnOficina = new List<LIReclameEnOficinaDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarReclameEnOficinaFiltros_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandTimeout = 120;


                cmd.Parameters.AddWithValue("@IdCentroServicio", idCentroServicio);

                filtros.ToList().ForEach(f =>
                {
                    if (!string.IsNullOrWhiteSpace(f.Value))
                    {

                        cmd.Parameters.AddWithValue(f.Key, f.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(f.Key, DBNull.Value);
                    }


                });

                SqlDataReader read = cmd.ExecuteReader();
                ListaReclameEnOficina = new List<LIReclameEnOficinaDC>();
                while (read.Read())
                {
                    LIReclameEnOficinaDC objREO = new LIReclameEnOficinaDC();
                    objREO.NumeroGuia = Convert.ToInt64(read["ADM_NumeroGuia"]);
                    objREO.Peso = Convert.ToInt64(read["ADM_Peso"]);
                    objREO.DocumentoDestinatario = read["ADM_IdDestinatario"].ToString();
                    objREO.NombreDestinatario = read["ADM_NombreDestinatario"].ToString();
                    objREO.EsAlCobro = Convert.ToBoolean(read["ADM_EsAlCobro"]);
                    objREO.EstaEntregada = Convert.ToBoolean(read["ADM_EstaEntregada"]);
                    objREO.ValorTotal = Convert.ToDecimal(read["ADM_ValorTotal"]);
                    objREO.DiceContener = read["ADM_DiceContener"].ToString();
                    objREO.DiasTranscurridos = Convert.ToInt32(read["DiasTranscurridos"]);

                    objREO.FormaPago = new ADGuiaFormaPago();
                    if (objREO.DiasTranscurridos >= 30)
                        objREO.EstadoDevolucion = true;
                    else
                        objREO.EstadoDevolucion = false;

                    if (read["REO_TipoUbicacion"] != DBNull.Value)
                    {
                        objREO.TipoUbicacion = (PUEnumTipoUbicacion)Convert.ToInt32(read["REO_TipoUbicacion"]);
                        objREO.Ubicacion = Convert.ToInt32(read["REO_Ubicacion"]);
                        objREO.UbicacionDetalle = objREO.TipoUbicacion.ToString() + "  " + objREO.Ubicacion.ToString();
                    }

                    objREO.FechaAsignacion = Convert.ToDateTime(read["FechaAsignacion"]);
                    objREO.UsuarioAsigna = read["UsuarioAsigna"].ToString();
                    //objREO.FechaINGRESO = Convert.ToDateTime(read["FechaINGRESO"]); PENDIENTE


                    objREO.EstadoGuiaenPRO = (PUEnumTipoMovimientoInventario)Convert.ToInt32(read["INV_IdTipoMovimiento"].ToString());


                    ListaReclameEnOficina.Add(objREO);
                }
            }

            if (ListaReclameEnOficina.Any())
                return ListaReclameEnOficina.OrderBy(x => x.DiasTranscurridos).ToList();
            else
                return ListaReclameEnOficina;
        }



        /// <summary>
        /// Metodo para consultar los totales de los  envios Asignados, Ingresados y para Devolucion de reclame en oficina por punto
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public Dictionary<string, int> ConsultarContadoresGuiasReclameEnOficina(long idCentroServicio)
        {
            Dictionary<string, int> totales = new Dictionary<string, int>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand("paConsultarContadoresReclameEnOficina_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@INV_IdCentroServicio", idCentroServicio));
                sqlConn.Open();
                SqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    totales.Add("TotalAsignadas", Convert.ToInt32(read["TotalAsignadas"]));
                    totales.Add("TotalIngresadas", Convert.ToInt32(read["TotalIngresadas"]));
                    totales.Add("TotalDevolucion", Convert.ToInt32(read["TotalDevolucion"]));
                }
                sqlConn.Close();
            }

            return totales;
        }


        #endregion

        /// <summary>
        /// Metodo para adicionar Obtener Archivo Guia x Numero Guia
        /// </summary>
        /// <param name="ObtenerArchivoGuiaxNumeroGuia"></param>
        /// <returns></returns>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaxNumeroGuia(long numeroGuia)
        {

            LIArchivoGuiaMensajeriaDC objNuevo = new LIArchivoGuiaMensajeriaDC();


            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerArchivoGuia_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                SqlDataReader read = cmd.ExecuteReader();

                if (read.Read())
                {
                    objNuevo.Archivo = read["ARG_RutaArchivo"].ToString();
                    objNuevo.NumeroGuia = Convert.ToInt64(read["ARG_NumeroGuia"]);

                }
            }

            return objNuevo;

        }

        #region Descargue Controller App
        /// <summary>
        /// Metodo para descargue envios controller app mensajero/auditor
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public void InsertarImagenesPruebaEntrega(LIDescargueControllerAppDC descargue)
        {
            string rutaImagenes;
            string carpetaDestino;

            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                int file = 0;
                rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FoldImgLOIPruEntrega");
                carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }
                if (descargue.TipoEvidencia != null)
                {
                    foreach (LITipoEvidenciaControllerAppDC item in descargue.TipoEvidencia)
                    {
                        foreach (string imagen in item.Imagenes)
                        {
                            file++;
                            byte[] bytebuffer = Convert.FromBase64String(imagen);
                            MemoryStream memoryStream = new MemoryStream(bytebuffer);
                            var image = Image.FromStream(memoryStream);
                            ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
                            string ruta = carpetaDestino + "\\" + descargue.NumeroGuia + "-" + file + ".jpg";
                            Encoder myEncoder = Encoder.Quality;
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
                            myEncoderParameters.Param[0] = myEncoderParameter;
                            image.Save(ruta, jpgEncoder, myEncoderParameters);

                            SqlCommand cmd1 = new SqlCommand("paGuardarImagenPruebasEntregaMensajeroAuditor_LOI", cnn);
                            cmd1.CommandType = CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@IdMensajero", descargue.IdMensajero);
                            cmd1.Parameters.AddWithValue("@Identificacion", descargue.IdentificacionQuienRecibe);
                            cmd1.Parameters.AddWithValue("@NombreRecibidoPor", string.IsNullOrEmpty(descargue.NombreQuienRecibe) ? string.Empty : descargue.NombreQuienRecibe);
                            cmd1.Parameters.AddWithValue("@NumeroGuia", descargue.NumeroGuia);
                            cmd1.Parameters.AddWithValue("@RutaImagen", ruta);
                            cmd1.Parameters.AddWithValue("@Latitud", descargue.Latitud);
                            cmd1.Parameters.AddWithValue("@Longitud", descargue.Longitud);
                            cmd1.Parameters.AddWithValue("@IdCiudad", descargue.IdCiudad);
                            cmd1.Parameters.AddWithValue("@Ciudad", descargue.NombreCiudad);
                            cmd1.Parameters.AddWithValue("@TipoEnvidencia", item.TipoEvidenciaControllerApp);
                            cmd1.Parameters.AddWithValue("@DescripcionEvidencia", item.NombreEvidenciaControllerApp);
                            cmd1.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                            cnn.Open();
                            cmd1.ExecuteNonQuery();
                            cnn.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metodo para validar si la guia está planilla al mensajero 
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <param name="IdMensajero"></param>
        /// <returns></returns>
        public bool ValidarPlanillaDescargueMensajero(long NumeroGuia, long IdMensajero)
        {
            bool respuesta = false;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paValidarPlanillaAsignacionDescargueMensajero_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", NumeroGuia);
                cmd.Parameters.AddWithValue("@IdMensajero", IdMensajero);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    respuesta = true;
                }
                conn.Close();
            }
            return respuesta;
        }

        /// <summary>
        /// Metodo para obtener imagen fachada descargue app 
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>

        public List<string> ObtenerImagenFachadaApp(long numeroGuia)
        {
            List<string> lstfachada = new List<string>();

            using (SqlConnection sqlconn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerImagenFachadaApp_LOI", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue(@"IEGT_NuemroGuia", numeroGuia);
                DataTable dt = new DataTable();
                sqlconn.Open();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow item in dt.Rows)
                {
                    if (item["IEGT_Imagen"] != DBNull.Value)
                    {
                        string ruta = item.Field<string>("IEGT_Imagen");
                        FileStream stream = File.OpenRead(ruta);
                        byte[] fileBytes = new byte[stream.Length];
                        stream.Read(fileBytes, 0, fileBytes.Length);
                        stream.Close();
                        lstfachada.Add(Convert.ToBase64String(fileBytes));

                    }

                }

            }
            return lstfachada;
        }

        /// <summary>
        /// inserta el conteo de las guias entregadas y devueltas desde la app por el mensajero
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <param name="entrega"></param>
        /// <param name="descargue"></param>
        public void InsertarConteoDescarguesYDevolucionesPorApp(long numeroPlanilla, int entrega, int devolucion)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarConteoDevolucionesYEntregasPorApp_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPlanilla", numeroPlanilla);
                cmd.Parameters.AddWithValue("@Entregas", entrega);
                cmd.Parameters.AddWithValue("@Devoluciones", devolucion);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        #endregion

        #region Tapas de logistica
        /// <summary>
        /// Método para adicionar una tapa de impresion
        /// </summary>
        /// <param name="tapaLogistica"></param>
        public void AdicionarTapaLogistica(LITapaLogisticaDC tapaLogistica)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarTapa_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", tapaLogistica.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@NumeroTapa", tapaLogistica.NumeroTapaLogistica.HasValue ? tapaLogistica.NumeroTapaLogistica.Value : new long?()));
                cmd.Parameters.Add(new SqlParameter("@Tipo", (short)tapaLogistica.Tipo));
                cmd.Parameters.Add(new SqlParameter("@Impreso", tapaLogistica.Impresa));
                cmd.Parameters.Add(new SqlParameter("@FechaImpresion", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Verifica si una guia tiene almenos una tapa por tipo tapa
        /// </summary>
        /// <param name="tapaLogistica"></param>
        /// <returns></returns>
        public bool VerificarGuiaConTapa(LITapaLogisticaDC tapaLogistica)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand("paVerificarGuiaConTapa_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", tapaLogistica.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@Tipo", (short)tapaLogistica.Tipo));
                sqlConn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0 ? true : false;
            }
        }

        /// <summary>
        /// Verifica si un numero de guia esta en la tabla gestionAuditoria para imprimir la tapa
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool VerificaGuiaConGestionAuditor(long numeroGuia)
        {
            bool resultado = false;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerConteoDeGuiaEnGestionGuia_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                conn.Open();
                resultado = Convert.ToInt32(cmd.ExecuteScalar()) > 0 ? true : false;
                conn.Close();
            }
            return resultado;
        }

        #endregion

        #region Agencias


        /// <summary>
        /// Método para obtener los pendientes de la agencia
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public List<LIDescargueGuiaAgenciaDC> ObtenerPendientesAgencia(long idAgencia)
        {
            List<LIDescargueGuiaAgenciaDC> listaPendientesAgencia = new List<LIDescargueGuiaAgenciaDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEnviosPendientesAgencia_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idAgencia", idAgencia));
                SqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    LIEnumSaleParaPendientesAgencia name = (LIEnumSaleParaPendientesAgencia)Enum.Parse(typeof(LIEnumSaleParaPendientesAgencia), Convert.ToString(read["SalePara"]));
                    string salePara = Utilidades.GetDescription(name);
                    LIDescargueGuiaAgenciaDC objPendiente = new LIDescargueGuiaAgenciaDC()
                    {
                        Estado = new ADTrazaGuia
                        {
                            IdEstadoGuia = read.GetInt16(1),
                            DescripcionEstadoGuia = read.GetString(3)
                        },
                        Guia = new OUGuiaIngresadaDC
                        {
                            NumeroGuia = read.GetInt64(0),
                            FechaAsignacion = DBNull.Value.Equals(read.GetValue(9)) ? read.GetDateTime(8) : read.GetDateTime(9)
                        },
                        Resultado = (LIEnumPendientesAgencia)(Enum.Parse(typeof(LIEnumPendientesAgencia), Convert.ToString(read["Resultado"]))),
                        Mensaje = salePara
                    };
                    listaPendientesAgencia.Add(objPendiente);
                }
            }
            return listaPendientesAgencia;
        }

        #endregion

        /// <summary>
        /// Obtiene los mensajeros que han estadpos asignados a un planilla de la cual se descargo una guia como no entregada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIMensajeroResponsableDC> ObtenerMensajerosResponsablesDescargue(long numeroGuia)
        {
            List<LIMensajeroResponsableDC> lstMensajeros = new List<LIMensajeroResponsableDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerMensajerosResponsablesDescargue_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LIMensajeroResponsableDC mensajero = new LIMensajeroResponsableDC
                    {
                        IdMensajero = reader["PEI_Identificacion"].ToString(),
                        Nombremensajero = reader["NombreCompleto"].ToString(),
                        IdPlanilla = Convert.ToInt64(reader["PAG_IdPlanillaAsignacionEnvio"]),
                        FechaGrabacion = Convert.ToDateTime(reader["PAG_FechaGrabacion"])
                    };

                    lstMensajeros.Add(mensajero);
                }
                conn.Close();
            }
            return lstMensajeros;
        }

        /// <summary>
        /// Metodo para validar si se debe enviar un mensaje de texto desde la aplicacion cuando se entrega
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool ValidarGuiaMensajeTextoEntrega(long numeroGuia)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                bool respuesta = false;
                SqlCommand cmd = new SqlCommand("paVerificarEntregaMensajeTexto_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    respuesta = true;
                }
                conn.Close();
                return respuesta;
            }
        }

        /// <summary>
        /// Metodo para validar si se debe enviar un mensaje de texto cuando un envio cumpla 25 dias en reclame oficina
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool ValidarMensajeReo_LOI(long numeroGuia)
        {
            bool respuesta = false;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand("paVerificarMensajeReo_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var resultado = reader.GetInt64(0);
                    if (resultado != 0)
                    {
                        respuesta = true;
                    }
                }
                conn.Close();
            }
            return respuesta;
        }

        #region Auditoria

        /// <summary>
        /// Método para insertar auditoria de devolución (Auditores App Controller) 
        /// </summary>
        /// <returns></returns>
        public bool InsertarAuditoriaDevolucion(LIGestionAuditorDC liGestionAuditorDC)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarGestionAuditoriaDevolucion_LOI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", liGestionAuditorDC.NumeroGuia);
                cmd.Parameters.AddWithValue("@idEstadoGuiaLog", liGestionAuditorDC.IdEstadoGuiaLog);
                cmd.Parameters.AddWithValue("@idPlanillaAsignacion", liGestionAuditorDC.IdPlanillaAsignacion);
                cmd.Parameters.AddWithValue("@descripcionInmueble", liGestionAuditorDC.DescripcionInmueble);
                cmd.Parameters.AddWithValue("@nombreReceptorAuditoria", liGestionAuditorDC.NombreReceptorAuditoria);
                cmd.Parameters.AddWithValue("@cedulaReceptorAuditoria", liGestionAuditorDC.CedulaReceptorAuditoria);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Identificacion);
                int pk = (int)cmd.ExecuteScalar();
                if (pk > 0)
                    return true;
            }
            return true;
        }

        #endregion

        #region SISPOSTAL Masivos

        public bool ValidarGuiaDescargadaXAppMasivos(long nGuia)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxSispostalController))
            {
                bool respuesta = false;
                SqlCommand cmd1 = new SqlCommand("Consultar_PruebaEntrega", cnn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@NumeroGuia", nGuia);
                cnn.Open();
                SqlDataReader reader = cmd1.ExecuteReader();
                if (reader.Read())
                {
                    respuesta = DBNull.Value == reader[0] ? false : Convert.ToBoolean(reader[0]);
                }
                cnn.Close();

                return respuesta;
            }
        }

        /// <summary>
        /// Metodo para insertar la prueba de entrega en Masivos
        /// </summary>
        /// <param name="descargue"></param>
        /// <param name="intentoEntrega"></param>
        /// <returns></returns>
        public string InsertarPruebasEntregaMasivos(LIDescargueControllerAppDC descargue, bool intentoEntrega)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxSispostalController))
            {
                string respuesta = "";
                SqlCommand cmd1 = new SqlCommand("Inserta_PruebaEntrega", cnn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@NumeroGuia", descargue.NumeroGuia);
                cmd1.Parameters.AddWithValue("@Latitud", descargue.Latitud);
                cmd1.Parameters.AddWithValue("@Longitud", descargue.Longitud);
                cmd1.Parameters.AddWithValue("@TipoContador", intentoEntrega ? descargue.TipoContador : 0);
                cmd1.Parameters.AddWithValue("@NumeroContador", intentoEntrega ? descargue.NumeroContador : "");
                cmd1.Parameters.AddWithValue("@CreadoPor", descargue.Usuario);
                cnn.Open();
                SqlDataReader reader = cmd1.ExecuteReader();
                if (reader.Read())
                {
                    respuesta = reader[0].ToString().Trim();
                }
                cnn.Close();

                return respuesta;
            }
        }

        /// <summary>
        /// Metodo para insertar imagenes como prueba de entrega en Masivos
        /// </summary>
        /// <param name="descargue"></param>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public void InsertarImagenesPruebaEntregaMasivos(LIDescargueControllerAppDC descargue, string ruta)
        {

            string carpetaDestino = Path.Combine(ConfigurationManager.AppSettings["App.RutaImagenesServidorSispostal"].ToString(), ruta);
            string rutaImagen = Path.Combine(carpetaDestino, descargue.NumeroGuia + ".png");

            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }
            if (descargue.TipoEvidencia != null)
            {
                foreach (LITipoEvidenciaControllerAppDC item in descargue.TipoEvidencia)
                {
                    if (item.Imagenes != null)
                    {
                        foreach (string imagen in item.Imagenes)
                        {
                            byte[] bytebuffer = Convert.FromBase64String(imagen);
                            MemoryStream memoryStream = new MemoryStream(bytebuffer);
                            var image = Image.FromStream(memoryStream);
                            ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
                            Encoder myEncoder = Encoder.Quality;
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
                            myEncoderParameters.Param[0] = myEncoderParameter;
                            image.Save(rutaImagen, jpgEncoder, myEncoderParameters);
                        }
                    }
                }
            }
        }

        #endregion
    }
}