using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using System.Threading;
using CO.Servidor.Produccion.Comun;
using CO.Servidor.Produccion.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Metadata.Edm;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Entity;
using System.Collections;
//using EntityFramework.BulkInsert.Extensions;
using System.Transactions;
using System.Configuration;

namespace CO.Servidor.Produccion.Datos
{
    public class PRRepositorio
    {

        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString + ";Connection Timeout=1800";

        #region Instancia singleton de la clase

        private static readonly PRRepositorio instancia = new PRRepositorio();

        public static PRRepositorio Instancia
        {
            get
            {
                return instancia;
            }
        }

        #endregion Instancia singleton de la clase

        #region Atributos

        /// <summary>
        /// Nombre del modelo
        /// </summary>
        private const string NombreModelo = "ModeloProduccion";

        #endregion Atributos

        #region Métodos
        public void GuardarNovedad()
        {

        }

        public void EliminarNovedad()
        {

        }

        #region Administración de motivos de novedades
        public void GuardarMotivoNovedad(PRMotivoNovedadDC motivoNovedad)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MotivoNovedad_PRO motivoNovedadPRO= contexto.MotivoNovedad_PRO.Where(m => m.MNO_IdMotivoNovedad == motivoNovedad.IdMotivoNovedad).FirstOrDefault();
                if (motivoNovedadPRO != null)
                {
                    motivoNovedadPRO.MNO_CreadoPor = ControllerContext.Current.Usuario;
                    motivoNovedadPRO.MNO_Descripcion = motivoNovedad.Descripcion;
                    motivoNovedadPRO.MNO_EsVisible = true;                    
                    motivoNovedadPRO.MNO_FechaGrabacion = DateTime.Now;
                    motivoNovedadPRO.MNO_IdTipoNovedad = (short)motivoNovedad.TipoNovedad;
                }
                else
                {
                    motivoNovedadPRO = new MotivoNovedad_PRO()
                    {
                        MNO_IdMotivoNovedad=motivoNovedad.IdMotivoNovedad,
                        MNO_CreadoPor = ControllerContext.Current.Usuario,
                        MNO_Descripcion = motivoNovedad.Descripcion,
                        MNO_Estado="ACT",
                        MNO_EsVisible = true,
                        MNO_FechaGrabacion = DateTime.Now,
                        MNO_IdTipoNovedad = (short)motivoNovedad.TipoNovedad
                    };
                    contexto.MotivoNovedad_PRO.Add(motivoNovedadPRO);
                }

                contexto.SaveChanges();
            }
        }

        public List<PRMotivoNovedadDC> ConsultarMotivosNovedad()
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MotivoNovedad_PRO.ToList().ConvertAll<PRMotivoNovedadDC>((a) =>
                {
                    return new PRMotivoNovedadDC()
                    {
                        CreadoPor = a.MNO_CreadoPor,
                        FechaGrabacion = a.MNO_FechaGrabacion,
                        Descripcion = a.MNO_Descripcion,
                        IdMotivoNovedad = a.MNO_IdMotivoNovedad,
                        IdMotivoNovedadOriginal = a.MNO_IdMotivoNovedad,
                        TipoNovedad = (PREnumTipoNovedadDC)a.MNO_IdTipoNovedad
                    };
                });
            }
        }

        public void BorrarMotivoNovedad(PRMotivoNovedadDC motivoNovedad)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MotivoNovedad_PRO motivoNovedadPRO = contexto.MotivoNovedad_PRO.Where(m => m.MNO_IdMotivoNovedad == motivoNovedad.IdMotivoNovedad).FirstOrDefault();
                if (motivoNovedadPRO != null)
                {
                    contexto.MotivoNovedad_PRO.Remove(motivoNovedadPRO);
                    contexto.SaveChanges();
                }
            }
        }
        #endregion


        #region Administración de retenciones
        public void GuardarValoresRetencion(PRRetencionProduccionDC retencion)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RetencionLiqGlobal_PRO retencionDB = contexto.RetencionLiqGlobal_PRO.Include("RetencionConcepto_PRO").Where(r => r.REL_IdRetencion == retencion.Retencion.IdRetencion).FirstOrDefault();

                if (retencionDB != null)
                {
                    retencionDB.REL_CreadoPor = ControllerContext.Current.Usuario;
                    retencionDB.REL_CuentaContableNovasoft = retencion.CuentaContableNovasoft;
                    retencionDB.REL_FechaGrabacion = DateTime.Now;
                    retencionDB.REL_PorcRetencion = retencion.PorcRetencion/100;
                    retencionDB.REL_TextoDelConcepto = retencion.TextoEnLaProduccion;
                    retencionDB.REL_BaseRetencion = retencion.BaseRetencion;

                    retencionDB.RetencionConcepto_PRO.ToList().ForEach((r) =>
                    {
                        contexto.RetencionConcepto_PRO.Remove(r);
                    });

                    foreach (PRConceptoRetencionDC valorBase in retencion.BasesRetencion)
                    {
                        contexto.RetencionConcepto_PRO.Add(new RetencionConcepto_PRO()
                        {
                            REN_CreadoPor = ControllerContext.Current.Usuario,
                            REN_FechaGrabacion = DateTime.Now,
                            REN_IdConcepto = valorBase.IdConcepto,
                            REN_IdRetencion = retencion.Retencion.IdRetencion,
                            REN_TipoConcepto = valorBase.TipoConceptoRetencion.ToString()
                        });
                    }

                    contexto.SaveChanges();
                }
            }
        }

        public List<PRRetencionProduccionDC> ConsultarValoresRetenciones()
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RetencionLiqGlobal_PRO.Include("Retencion_TAR").Include("RetencionConcepto_PRO").ToList().ConvertAll<PRRetencionProduccionDC>((a) =>
                {
                    PRRetencionProduccionDC retencionesResult= new PRRetencionProduccionDC()
                    {
                        BaseRetencion = a.REL_BaseRetencion,
                        CreadoPor = a.REL_CreadoPor,
                        CuentaContableNovasoft = a.REL_CuentaContableNovasoft,
                        FechaGrabacion = a.REL_FechaGrabacion,
                        Retencion = new PRRetencionDC() { IdRetencion = a.REL_IdRetencion, NombreRetencion = a.Retencion_TAR.RET_Descripcion },
                        PorcRetencion = a.REL_PorcRetencion*100,
                        TextoEnLaProduccion = a.REL_TextoDelConcepto                        
                    };
                    retencionesResult.BasesRetencion = new List<PRConceptoRetencionDC>();
                    a.RetencionConcepto_PRO.ToList().ForEach(r => 
                    {
                        retencionesResult.BasesRetencion.Add(new PRConceptoRetencionDC() { TipoConceptoRetencion = (PREnumTipoConcepRetencionDC)Enum.Parse(typeof(PREnumTipoConcepRetencionDC), r.REN_TipoConcepto), IdConcepto = r.REN_IdConcepto });
                    });
                    return retencionesResult;
                });
            }
        }

        public void BorrarRetencion(PRRetencionProduccionDC retencion)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RetencionLiqGlobal_PRO retencionDB = contexto.RetencionLiqGlobal_PRO.Where(r => r.REL_IdRetencion == retencion.Retencion.IdRetencion).FirstOrDefault();

                if (retencionDB != null)
                {
                    contexto.RetencionLiqGlobal_PRO.Remove(retencionDB);
                    contexto.SaveChanges();
                }
            }
        }

        public void GuardarRetencionXCiudad(PRRetencionXCiudadDC retencionXCiudad)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RetencionLiqXCiudad_PRO reteCiudad = contexto.RetencionLiqXCiudad_PRO.Include("RetencionXCiudadConcepto_PRO").Include("Retencion_TAR").Where(r => r.REC_IdLocalidad == retencionXCiudad.Localidad.IdLocalidad && r.REC_IdRetencion == retencionXCiudad.Retencion.IdRetencion).FirstOrDefault();

                if (reteCiudad != null)
                {
                    reteCiudad.REC_IdLocalidad = retencionXCiudad.Localidad.IdLocalidad;
                    reteCiudad.REC_IdRetencion = retencionXCiudad.Retencion.IdRetencion;
                    reteCiudad.REC_FechaGrabacion = DateTime.Now;
                    reteCiudad.REC_CreadoPor = ControllerContext.Current.Usuario;
                    reteCiudad.REL_PorcRetencion = retencionXCiudad.PorcRetencion / 100;
                    reteCiudad.REL_BaseRetencion = retencionXCiudad.BaseRetencion;
                    reteCiudad.REL_TextoDelConcepto = retencionXCiudad.TextoEnLaProduccion;
                    reteCiudad.REL_CuentaContableNovasoft = retencionXCiudad.CuentaContableNovasoft;

                }
                else
                {
                    reteCiudad = new RetencionLiqXCiudad_PRO()
                    {
                        REC_IdLocalidad = retencionXCiudad.Localidad.IdLocalidad,
                        REC_IdRetencion = retencionXCiudad.Retencion.IdRetencion,
                        REC_FechaGrabacion = DateTime.Now,
                        REC_CreadoPor = ControllerContext.Current.Usuario,
                        REL_PorcRetencion = retencionXCiudad.PorcRetencion / 100,
                        REL_BaseRetencion = retencionXCiudad.BaseRetencion,
                        REL_TextoDelConcepto = retencionXCiudad.TextoEnLaProduccion,
                        REL_CuentaContableNovasoft = retencionXCiudad.CuentaContableNovasoft,
                    };
                    contexto.RetencionLiqXCiudad_PRO.Add(reteCiudad);
                }

                reteCiudad.RetencionXCiudadConcepto_PRO.ToList().ForEach((r) =>
                {
                    contexto.RetencionXCiudadConcepto_PRO.Remove(r);
                });

                foreach (PRConceptoRetencionDC valorBase in retencionXCiudad.BasesRetencion)
                {
                    contexto.RetencionXCiudadConcepto_PRO.Add(new RetencionXCiudadConcepto_PRO()
                    {
                        RCN_CreadoPor = ControllerContext.Current.Usuario,
                        RCN_FechaGrabacion = DateTime.Now,
                        RCN_IdConcepto = valorBase.IdConcepto,
                        RCN_IdRetencionCiudad = reteCiudad.REC_IdRetencionCiudad,                         
                        RCN_TipoConcepto = valorBase.TipoConceptoRetencion.ToString()                         
                    });
                }
                contexto.SaveChanges();
            }
        }

        public void BorrarRetencionXCiudad(PRRetencionXCiudadDC retencionXCiudad)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.RetencionXCiudadConcepto_PRO.Include("RetencionLiqXCiudad_PRO").Where(r => r.RetencionLiqXCiudad_PRO.REC_IdLocalidad == retencionXCiudad.Localidad.IdLocalidad && r.RetencionLiqXCiudad_PRO.REC_IdRetencion == retencionXCiudad.Retencion.IdRetencion).ToList().ForEach(ret =>
                {
                    contexto.RetencionXCiudadConcepto_PRO.Remove(ret);
                });

                RetencionLiqXCiudad_PRO reteCiudad = contexto.RetencionLiqXCiudad_PRO.Where(r => r.REC_IdLocalidad == retencionXCiudad.Localidad.IdLocalidad && r.REC_IdRetencion == retencionXCiudad.Retencion.IdRetencion).FirstOrDefault();

                if (reteCiudad != null)
                {
                    contexto.RetencionLiqXCiudad_PRO.Remove(reteCiudad);                    
                }

                contexto.SaveChanges();
            }
        }

        public List<PRRetencionXCiudadDC> ConsultarRetencionesXCiudad()
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RetencionLiqXCiudad_PRO.Include("RetencionXCiudadConcepto_PRO").Include("Retencion_TAR").Include("Localidad_PAR").ToList().ConvertAll<PRRetencionXCiudadDC>((a) =>
                {
                    PRRetencionXCiudadDC retencionesResult = new PRRetencionXCiudadDC()
                    {
                        BaseRetencion = a.REL_BaseRetencion,
                        CreadoPor = a.REC_CreadoPor,
                        CuentaContableNovasoft = a.REL_CuentaContableNovasoft,
                        FechaGrabacion = a.REC_FechaGrabacion,
                        Localidad=new PRCiudadDC(){IdLocalidad = a.REC_IdLocalidad,NombreLocalidad=a.Localidad_PAR.LOC_Nombre},
                        Retencion =new PRRetencionDC(){IdRetencion= a.REC_IdRetencion,NombreRetencion=a.Retencion_TAR.RET_Descripcion},
                        PorcRetencion = a.REL_PorcRetencion == null ? 0 : a.REL_PorcRetencion.Value * 100,
                        TextoEnLaProduccion = a.REL_TextoDelConcepto
                    };

                    retencionesResult.BasesRetencion = new List<PRConceptoRetencionDC>();
                    a.RetencionXCiudadConcepto_PRO.ToList().ForEach(r =>
                    {
                        retencionesResult.BasesRetencion.Add(new PRConceptoRetencionDC() { TipoConceptoRetencion = (PREnumTipoConcepRetencionDC)Enum.Parse(typeof(PREnumTipoConcepRetencionDC), r.RCN_TipoConcepto), IdConcepto = r.RCN_IdConcepto });
                    });

                    return retencionesResult;
                });
            }
        }

        public List<PRRetencionDC> ConsultarTiposRetencion()
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Retencion_TAR.ToList().ConvertAll((r) =>
                {
                    return new PRRetencionDC()
                    {
                        IdRetencion = r.RET_IdRetencion,
                        NombreRetencion = r.RET_Descripcion
                    };
                });
            }
        }
        
        #endregion

        #region Administrar Novedades
        public void GuardarNovedadesProduccion(List<PRNovedadProduccionDC> novedadesProduccion)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {              
                DataTable novedadesBulk = new DataTable("CargaNovedadesProduccion_PRO");
                novedadesBulk.Columns.Add("NCS_AnoVigencia");
                novedadesBulk.Columns.Add("NCS_Cantidad");
                novedadesBulk.Columns.Add("NCS_Cargada");
                novedadesBulk.Columns.Add("NCS_CreadoPor");
                novedadesBulk.Columns.Add("NCS_FechaGrabacion");
                novedadesBulk.Columns.Add("NCS_IdCentroServicios");
                novedadesBulk.Columns.Add("NCS_IdMotivoNovedad");
                novedadesBulk.Columns.Add("NCS_IdServicio");
                novedadesBulk.Columns.Add("NCS_MesVigencia");
                novedadesBulk.Columns.Add("NCS_Observacion");
                novedadesBulk.Columns.Add("NCS_Valor");

                novedadesProduccion.ForEach(n =>
                {
                    DataRow rowNovedad = novedadesBulk.NewRow();
                    rowNovedad["NCS_AnoVigencia"] = n.AnoVigencia;
                    rowNovedad["NCS_Cantidad"] = n.Cantidad;
                    rowNovedad["NCS_Cargada"] = n.Cargada;
                    rowNovedad["NCS_CreadoPor"] = ControllerContext.Current.Usuario;
                    rowNovedad["NCS_FechaGrabacion"] = DateTime.Now;
                    rowNovedad["NCS_IdCentroServicios"] = n.IdCentroServicios;
                    rowNovedad["NCS_IdMotivoNovedad"] = n.IdMotivoNovedad;
                    if(n.IdServicio==0)
                        rowNovedad["NCS_IdServicio"] =DBNull.Value;
                    else
                        rowNovedad["NCS_IdServicio"] = n.IdServicio;
                    rowNovedad["NCS_MesVigencia"] = n.MesVigencia;
                    rowNovedad["NCS_Observacion"] = n.Observacion==null?"":n.Observacion;
                    rowNovedad["NCS_Valor"] = n.Valor;

                    novedadesBulk.Rows.Add(rowNovedad);
                });

                using (SqlBulkCopy s = new SqlBulkCopy(contexto.Database.Connection.ConnectionString))
                {
                    s.DestinationTableName = novedadesBulk.TableName;

                    foreach (var column in novedadesBulk.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());

                    s.WriteToServer(novedadesBulk);
                }           
            }
        }


        public List<PRNovedadProduccionDC> ConsultarNovedadesNoCargadas(int ano, int mes, long idCentroServicios)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<PRNovedadProduccionDC> novedadesResponse = new List<PRNovedadProduccionDC>();
                IQueryable<CargaNovedadesProduccion_PRO> novedadesNocargadas;
                if (idCentroServicios == 0)
                    novedadesNocargadas = contexto.CargaNovedadesProduccion_PRO.Include("CentroServicios_PUA").Include("MotivoNovedad_PRO").Include("Servicio_TAR").Where(n => n.NCS_MesVigencia == mes && n.NCS_AnoVigencia == ano);
                else
                    novedadesNocargadas = contexto.CargaNovedadesProduccion_PRO.Include("CentroServicios_PUA").Include("MotivoNovedad_PRO").Include("Servicio_TAR").Where(n => n.NCS_MesVigencia == mes && n.NCS_AnoVigencia == ano && n.NCS_IdCentroServicios == idCentroServicios);

                novedadesNocargadas.ToList().ForEach(n=>
                {
                    novedadesResponse.Add(new PRNovedadProduccionDC()
                    {
                        AnoVigencia = n.NCS_AnoVigencia,
                        Cantidad = n.NCS_Cantidad,
                        Cargada = n.NCS_Cargada,
                        CreadoPor = n.NCS_CreadoPor,
                        FechaGrabacion = n.NCS_FechaGrabacion,
                        DescCentroServicios = n.CentroServicios_PUA.CES_IdCentroServicios.ToString() + "-" + n.CentroServicios_PUA.CES_Nombre,
                        DescMotivo = n.MotivoNovedad_PRO.MNO_Descripcion,
                        DescServicio = n.NCS_IdServicio == null ? "NA" : n.Servicio_TAR.SER_Descripcion,
                        IdCentroServicios = n.NCS_IdCentroServicios,
                        IdMotivoNovedad = n.NCS_IdMotivoNovedad,
                        IdNovedadProduccion = n.CNP_IdNovedadProduccion,
                        IdServicio = n.NCS_IdServicio == null ? 0 : (int)n.NCS_IdServicio,
                        MesVigencia = n.NCS_MesVigencia,
                        Observacion = n.NCS_Observacion,
                        Valor = n.NCS_Valor
                    });
                });                

                return novedadesResponse;
            }
        }

        public void EliminarNovedad(long Idnovedad)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CargaNovedadesProduccion_PRO novedadCargada = contexto.CargaNovedadesProduccion_PRO.Where(n => n.CNP_IdNovedadProduccion == Idnovedad).FirstOrDefault();

                if (novedadCargada != null)
                    if(novedadCargada.NCS_Cargada==false)
                        contexto.CargaNovedadesProduccion_PRO.Remove(novedadCargada);
                    else
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_PRODUCCION,"", "La novedad que intenta eliminar ya está cargada a la producción y no puede ser eliminada."));

                contexto.SaveChanges();
            }
        }
        #endregion

        #region Liquidaciones

        public void GenerarLiquidacionCentroServicio(long idCentroServicio, int mes, int ano)
        {

            //SqlTransaction trans =null;
            try
            {
                using (SqlConnection conn = new SqlConnection(CadCnxController))
                {
                    conn.Open();
                    //trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand("paLiquidacionProduccionCentroServicio_PRO", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@mes", mes);
                    cmd.Parameters.AddWithValue("@ano", ano);
                    cmd.Parameters.AddWithValue("@IdCentroServicio", idCentroServicio);
                    cmd.Parameters.AddWithValue("@Usuario", ControllerContext.Current.Usuario);
                    cmd.ExecuteNonQuery();
                    //trans.Commit();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                // if(trans!=null)
                //   trans.Rollback();
                throw ex;
            }
        }
        public void GenerarLiquidacionTodos(int mes, int ano)
        {

            //SqlTransaction trans = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(CadCnxController))
                {
                    conn.Open();
                    //trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand("paLiquidacionProduccionTodos_PRO", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@mes", mes);
                    cmd.Parameters.AddWithValue("@ano", ano);
                    cmd.Parameters.AddWithValue("@Usuario", ControllerContext.Current.Usuario);
                    cmd.ExecuteNonQuery();
                    //trans.Commit();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                //if (trans != null)
                //   trans.Rollback();
                throw ex;
            }
        }


        public void AprobarLiquidaciones(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta)
        {

             SqlTransaction trans =null;
             try
             {

                 using (SqlConnection conn = new SqlConnection(CadCnxController))
                 {
                     conn.Open();
                     trans = conn.BeginTransaction();
                     SqlCommand cmd = new SqlCommand("paAprobarLiquidaciones_PRO", conn,trans);
                     cmd.CommandType = CommandType.StoredProcedure;
                     cmd.CommandTimeout = 0;
                     cmd.Parameters.AddWithValue("@Mes", mes);
                     cmd.Parameters.AddWithValue("@Ano", ano);
                     cmd.Parameters.AddWithValue("@IdRacol", idRacol);
                     cmd.Parameters.AddWithValue("@IdCiudad", idCiudad);
                     cmd.Parameters.AddWithValue("@IdCS", idCentroServicio);
                     cmd.Parameters.AddWithValue("@IdLiqDesde", idLiqDesde);
                     cmd.Parameters.AddWithValue("@IdLiqHasta", idLiqHasta);                    
                     cmd.ExecuteNonQuery();
                     trans.Commit();
                     conn.Close();
                 }
             }
             catch (Exception ex)
             {
                 if (trans != null)
                     trans.Rollback();
                 throw ex;
             }       

           /* using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paAprobarLiquidaciones_PRO(mes, ano, idRacol, idCiudad, idCentroServicio, idLiqDesde, idLiqHasta);
            }*/
        }

        public void EliminarLiquidacionProduccion(long idLiqProduccion)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarLiqProduccion_PRO(idLiqProduccion);
            }
        }

        public void CargarLiquidacionEnCaja(int mes, int ano)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCargarLiquidacionCaja_PRO(mes, ano);
            }
        }

        public void ActualizarNumeroGuiaEnLiquidacion(long idLiquidacion, long numeroGuia)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                LiquidacionProduccion_PRO liqProd= contexto.LiquidacionProduccion_PRO.Where(l => l.LPR_IdLiquidacionProduccion == idLiquidacion).FirstOrDefault();

                if (liqProd != null)
                {
                    liqProd.LPR_NumeroGuiaInterna = numeroGuia;
                    contexto.SaveChanges();
                }
            }
        }

        public List<PRLiquidacionProduccionDC> ConsultarLiquidacionProduccion(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta)
        {
            using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {                
                return contexto.paConsultarLiquidaciones_PRO(mes, ano, idRacol, idCiudad, idCentroServicio,idLiqDesde,idLiqHasta).ToList().ConvertAll<PRLiquidacionProduccionDC>((l) =>
                {
                    return new PRLiquidacionProduccionDC()
                    {
                        Ano = ano,
                        CreadoPor = l.LPR_CreadoPor,
                        EstadoLiquidacionProduccion = l.DescEstado,
                        FechaAprobacion = l.LPR_FechaAprobacion,
                        FechaGrabacion = l.LPR_FechaGrabacion,
                        IdCentroServicios = l.LPR_IdCentroServicios,
                        IdEstadoLiquidacionProduccion = l.LPR_IdEstadoLiquidacionProduccion,
                        IdLiquidacionProduccion = l.LPR_IdLiquidacionProduccion,
                        Mes = mes,
                        NombreCentroServicios = l.NombreCentroServicios,
                        NumeroGuiaInterna = l.LPR_NumeroGuiaInterna,
                        TotalDeducciones = l.LPR_TotalDeducciones,
                        TotalPagos = l.LPR_TotalPagos,
                        UsuarioAprueba = l.LPR_UsuarioAprueba,
                        IdTransaccionCaja=l.IdTransaccionCaja,
                        IdLiquNumMostrar=l.LPR_NumAMostrar
                    };
                });             
            }
        }
        #endregion


        #endregion
    }
}