using CO.Servidor.ServicioalCliente.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.ServicioalCliente;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Excepciones;
namespace CO.Servidor.ServicioalCliente.Datos
{
    public class SCRepositorio
    {
        #region Campos

        private static readonly SCRepositorio instancia = new SCRepositorio();
        private const string NombreModelo = "ModeloServicioalCliente";
        private CultureInfo cultura = new CultureInfo("es-CO");

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase SCRepositorio
        /// </summary>
        public static SCRepositorio Instancia
        {
            get { return SCRepositorio.instancia; }
        }

        #endregion Propiedades
    
        #region Consultas

        /// <summary>
        /// Método para obtener una lista con los tipos de solicitud y subtipos asociados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACTipoSolicitudDC> ObtenerTiposSolicitud() 
        {
            using (EntidadesServicioalCliente contexto = new EntidadesServicioalCliente(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
              
              return contexto.TipoSolicitud_SAC.ToList().ConvertAll(ti => new SACTipoSolicitudDC
                  {
                   IdTipo = ti.TIS_IdTipoSolicitud,
                   Descripcion = ti.TIS_Descripción,  
                   ListaSubTipos =  contexto.SubTipoSolicitud_SAC.Where(sub=>sub.STS_IdTipoSolicitud == ti.TIS_IdTipoSolicitud).ToList().ConvertAll(sti=> new SACSubTipoSolicitudDC
                   {
                       IdTipo = sti.STS_IdTipoSolicitud,
                       IdSubtipo = sti.STS_IdSubTipoSolicitud,
                       Descripcion = sti.STS_Descripcion,
                   })
                 });
            }
        }


        /// <summary>
        /// Método para obtener una lista con los posibles estados de una solicitud
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACEstadosSolicitudDC> ObtenerEstados()
        {
            using (EntidadesServicioalCliente contexto = new EntidadesServicioalCliente(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                return contexto.EstadosSolicitud_SAC.ToList().ConvertAll(ti => new SACEstadosSolicitudDC
                    {
                        IdEstado = ti.ESO_IdEstado,
                        Descripcion = ti.ESO_Descripción,
                        Orden = ti.ESO_OrdenEstado,
                        EstadoRegistro = EnumEstadoRegistro.ADICIONADO,
                    });
            }
        }

        /// <summary>
        /// Método para obtener una lista con los tipos de seguimiento
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACTipoSeguimientoDC> ObtenerTiposSeguimiento()
        {
            using (EntidadesServicioalCliente contexto = new EntidadesServicioalCliente(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                return contexto.TipoSeguimientoSolicitud_SAC.ToList().ConvertAll(ti => new SACTipoSeguimientoDC
                    {
                        IdTipo = ti.TSS_IdTipoSeguimiento,
                        Descripcion = ti.TSS_Descripcion,
                    });
            }
        }


        /// <summary>
        /// Método para obtener una lista con los medios de recepción
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACMedioRecepcionDC> ObtenerMediosRecepcion()
        {
            using (EntidadesServicioalCliente contexto = new EntidadesServicioalCliente(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                return contexto.MedioRecepcionSolicitud_SAC.ToList().ConvertAll(me => new SACMedioRecepcionDC
                {
                    IdMedio = me.MRS_IdMedio,
                    Descripcion = me.MRS_Descripcion,
                });
            }
        }


        /// <summary>
        /// Método para obtener una lista con los medios de recepción
        /// </summary>
        /// <returns></returns>
        public SACSolicitudDC ObtenerSolicitud(long numeroGuia)
        {
            using (EntidadesServicioalCliente contexto = new EntidadesServicioalCliente(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Solicitud_SAC solicitud = contexto.Solicitud_SAC.Where(sol => sol.SOL_NumeroGuia == numeroGuia).FirstOrDefault();

               if (solicitud != null)
               {
                   return new SACSolicitudDC
                   {
                       Descripcion = solicitud.SOL_Descripcion,
                       DireccionReclamante = solicitud.SOL_DireccionReclamante,
                       EmailReclamante = solicitud.SOL_EmailReclamante,
                       FaxReclamante = solicitud.SOL_FaxReclamante,
                       IdSolicitud = solicitud.SOL_IdSolicitud,
                       Localidad = new PALocalidadDC { IdLocalidad = solicitud.SOL_IdLocalidad },
                       MedioRecepcion = new SACMedioRecepcionDC { IdMedio = solicitud.SOL_IdMedioRecepSolicitud },
                       NombreReclamante = solicitud.SOL_NombreReclamante,
                       NumeroGuia = solicitud.SOL_NumeroGuia,
                       SubTipoSolicitud = new SACSubTipoSolicitudDC { IdSubtipo = solicitud.SOL_IdSubtipoSolicitud, IdTipo = solicitud.SOL_IdTipoSolicitud },
                       TelefonoReclamante = solicitud.SOL_TelefonoReclamante,
                       FechaRadicacion = solicitud.SOL_FechaRadicacion.HasValue ?  solicitud.SOL_FechaRadicacion.Value : solicitud.SOL_FechaGrabacion ,
                       TipoSolicitud = new SACTipoSolicitudDC
                       { IdTipo = solicitud.SOL_IdTipoSolicitud ,
                         ListaSubTipos = contexto.SubTipoSolicitud_SAC.Where(sub => sub.STS_IdTipoSolicitud == solicitud.SOL_IdTipoSolicitud).ToList().ConvertAll(sti => new SACSubTipoSolicitudDC
                   {
                       IdTipo = sti.STS_IdTipoSolicitud,
                       IdSubtipo = sti.STS_IdSubTipoSolicitud,
                       Descripcion = sti.STS_Descripcion,
                   })
                       },
                       UsuarioCreacion = solicitud.SOL_CreadoPor,
                       EstadoRegistro = EnumEstadoRegistro.MODIFICADO,
                       ListaSeguimientos = contexto.SeguimientoSolicitud_SAC.Include("TipoSeguimientoSolicitud_SAC").Where(seg => seg.SSO_IdSolicitud == solicitud.SOL_IdSolicitud).ToList().ConvertAll(sti => new SACSeguimientoSolicitudDC
                       {
                           IdSeguimiento = sti.SSO_IdSeguimiento,
                           IdSolicitud = sti.SSO_IdSolicitud,
                           FechaSeguimiernto = sti.SSO_FechaGrabacion,
                           Descripcion = sti.SSO_Descripcion,
                           Usuario = sti.SSO_CreadoPor,
                           EstadoRegistro = EnumEstadoRegistro.MODIFICADO,
                           TipoSeguimiento = new SACTipoSeguimientoDC 
                           {
                             IdTipo = sti.SSO_IdTipoSeguimiento ,
                             Descripcion = sti.TipoSeguimientoSolicitud_SAC.TSS_Descripcion
                           },
                       }),
                       ListaEstadosSolicitud = contexto.SolicitudEstados_SAC.Include("EstadosSolicitud_SAC").Where(est => est.SES_IdSolicitud == solicitud.SOL_IdSolicitud).ToList().ConvertAll(ste => new SACSolicitudEstadosDC
                       {
                                       Estado = new SACEstadosSolicitudDC
                                       {
                                        IdEstado = ste.SES_IdEstado,
                                        Descripcion = ste.EstadosSolicitud_SAC.ESO_Descripción,
                                        Orden = ste.EstadosSolicitud_SAC.ESO_OrdenEstado,
                                        EstadoRegistro = EnumEstadoRegistro.MODIFICADO,
                                       },
                                       IdSolicitud = ste.SES_IdSolicitud,
                                       EstadoRegistro = EnumEstadoRegistro.MODIFICADO,
   
                       }),
                   };
               }
               else
                   return new SACSolicitudDC 
                   {
                       EstadoRegistro = EnumEstadoRegistro.ADICIONADO,
                       ListaSeguimientos = new List<SACSeguimientoSolicitudDC>(),
                       TipoSolicitud = new SACTipoSolicitudDC(),
                       SubTipoSolicitud = new SACSubTipoSolicitudDC(),
                       MedioRecepcion = new SACMedioRecepcionDC(),
                       ListaEstadosSolicitud = new List<SACSolicitudEstadosDC>(),
                       UsuarioCreacion = ControllerContext.Current != null ? ControllerContext.Current.Usuario : string.Empty,
                       FechaRadicacion = DateTime.Now.Date,
                   };
            }
        }


        #endregion

        #region Adicionar

        /// <summary>
        /// Método para adicionar una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public SACSolicitudDC AdicionarSolicitud(SACSolicitudDC solicitud)
        {
            using (EntidadesServicioalCliente contexto = new EntidadesServicioalCliente(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Solicitud_SAC solSac = new Solicitud_SAC 
                {
                    SOL_CreadoPor = ControllerContext.Current.Usuario,
                    SOL_Descripcion = solicitud.Descripcion,
                    SOL_DireccionReclamante = solicitud.DireccionReclamante,
                    SOL_EmailReclamante = solicitud.EmailReclamante,
                    SOL_FaxReclamante = solicitud.FaxReclamante,
                    SOL_FechaGrabacion = DateTime.Now,
                    SOL_IdLocalidad = solicitud.Localidad.IdLocalidad,
                    SOL_IdMedioRecepSolicitud = (byte)solicitud.MedioRecepcion.IdMedio,
                    SOL_IdSubtipoSolicitud = (byte)solicitud.SubTipoSolicitud.IdSubtipo,
                    SOL_IdTipoSolicitud = (byte)solicitud.SubTipoSolicitud.IdTipo,
                    SOL_NombreReclamante = solicitud.NombreReclamante,
                    SOL_NumeroGuia = solicitud.NumeroGuia,
                    SOL_TelefonoReclamante = solicitud.TelefonoReclamante,
                    SOL_FechaRadicacion = solicitud.FechaRadicacion,
                };
                contexto.Solicitud_SAC.Add(solSac);
                contexto.SaveChanges();
                solicitud.IdSolicitud = solSac.SOL_IdSolicitud;
                solicitud.EstadoRegistro = EnumEstadoRegistro.MODIFICADO;
                return solicitud;
            }
        }


        /// <summary>
        /// Método para adicionar un estado de una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public SACSolicitudEstadosDC AdicionarEstado(SACSolicitudEstadosDC estado)
        {
            using (EntidadesServicioalCliente contexto = new EntidadesServicioalCliente(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SolicitudEstados_SAC solEst = new SolicitudEstados_SAC
                {
                    SES_CreadoPor = ControllerContext.Current.Usuario,
                    SES_FechaGrabacion = DateTime.Now,
                    SES_IdEstado =(byte) estado.Estado.IdEstado,
                    SES_IdSolicitud = estado.IdSolicitud,

                };
                contexto.SolicitudEstados_SAC.Add(solEst);
                estado.EstadoRegistro = EnumEstadoRegistro.MODIFICADO;
                contexto.SaveChanges();
                return estado;
            }
        }


        /// <summary>
        /// Método para adicionar un estado de una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public SACSeguimientoSolicitudDC AdicionarSeguimiento(SACSeguimientoSolicitudDC seguimiento)
        {
            using (EntidadesServicioalCliente contexto = new EntidadesServicioalCliente(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SeguimientoSolicitud_SAC segSol = new SeguimientoSolicitud_SAC
                {
                    SSO_CreadoPor = ControllerContext.Current.Usuario,
                    SSO_FechaGrabacion = DateTime.Now,
                    SSO_IdSolicitud = seguimiento.IdSolicitud,
                    SSO_IdTipoSeguimiento = (byte)seguimiento.TipoSeguimiento.IdTipo,
                    SSO_Descripcion = seguimiento.Descripcion,
                };
                contexto.SeguimientoSolicitud_SAC.Add(segSol);
                seguimiento.EstadoRegistro = EnumEstadoRegistro.MODIFICADO;
                contexto.SaveChanges();
                return seguimiento;
            }
        }



        #endregion

    }
}
