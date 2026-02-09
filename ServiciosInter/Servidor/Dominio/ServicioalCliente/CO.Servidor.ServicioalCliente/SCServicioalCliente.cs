using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.ServicioalCliente.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.ServicioalCliente;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.ServiceModel;
using CO.Servidor.ServicioalCliente.Comun;

namespace CO.Servidor.ServicioalCliente
{
    class SCServicioalCliente : ControllerBase
    {
        private static readonly SCServicioalCliente instancia = (SCServicioalCliente)FabricaInterceptores.GetProxy(new SCServicioalCliente(), COConstantesModulos.SERVICIO_AL_CLIENTE);

        public static SCServicioalCliente Instancia
        {
            get { return SCServicioalCliente.instancia; }
        }


        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        #region Consultas


        /// <summary>
        /// Método para obtener una lista con los tipos de solicitud y subtipos asociados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACTipoSolicitudDC> ObtenerTiposSolicitud()
        {
            return SCRepositorio.Instancia.ObtenerTiposSolicitud();
        }


        /// <summary>
        /// Método para obtener una lista con los posibles estados de una solicitud
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACEstadosSolicitudDC> ObtenerEstados()
        {
            return SCRepositorio.Instancia.ObtenerEstados();
        }

        /// <summary>
        /// Método para obtener una lista con los tipos de seguimiento
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACTipoSeguimientoDC> ObtenerTiposSeguimiento()
        {
            return SCRepositorio.Instancia.ObtenerTiposSeguimiento();
        }


        /// <summary>
        /// Método para obtener una lista con los medios de recepción
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SACMedioRecepcionDC> ObtenerMediosRecepcion()
        {
            return SCRepositorio.Instancia.ObtenerMediosRecepcion();
        }

        /// <summary>
        /// Método para obtener una lista con los medios de recepción
        /// </summary>
        /// <returns></returns>
        public SACSolicitudDC ObtenerSolicitud(long numeroGuia)
        {
            ADGuia guiaAdmision = fachadaMensajeria.ObtenerInfoGuiaXNumeroGuia(numeroGuia);
            if (guiaAdmision == null)
                return null;
            else
                return SCRepositorio.Instancia.ObtenerSolicitud(numeroGuia);
        }


        #endregion

        #region Adicionar

        /// <summary>
        /// Método para adicionar una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public SACSolicitudDC GuardarCambiosSolicitud(SACSolicitudDC solicitud)
        {
            ADGuia adGuia = new ADGuia();
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (solicitud.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    solicitud = SCRepositorio.Instancia.AdicionarSolicitud(solicitud);
                SACSolicitudEstadosDC estado = solicitud.ListaEstadosSolicitud.Where(es => es.EstadoRegistro == EnumEstadoRegistro.ADICIONADO).FirstOrDefault();
                if (estado != null)
                {
                    estado.IdSolicitud = solicitud.IdSolicitud;
                    SCRepositorio.Instancia.AdicionarEstado(estado);
                    solicitud.ListaEstadosSolicitud.Add(estado);
                }


                adGuia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(solicitud.NumeroGuia);
                //Tipo Solicitud : Peticion , SubTipo: Retiro de reclame en oficina
                if (solicitud.SubTipoSolicitud.IdTipo == 3 && solicitud.SubTipoSolicitud.IdSubtipo == 33)
                {
                    //RECLAME EN OFICINA
                    if (Convert.ToInt32(adGuia.IdTipoEntrega.ToString().Trim()) == 2)
                    {
                        //Cambia la fecha de entrega al dia anterior
                        EGTipoNovedadGuia.CambiarFechaEntregaDiaAnterior(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC()
                        {
                            Guia = new ADGuia()
                            {
                                NumeroGuia = solicitud.NumeroGuia,
                            },
                            TiempoAfectacion = 24
                        });
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.SERVICIO_AL_CLIENTE, SCEnumTipoError.EX_ERROR_GUIA_NO_RECLAME_EN_OFICINA.ToString(), string.Format(SCMensajesCliente.CargarMensaje(SCEnumTipoError.EX_ERROR_GUIA_NO_RECLAME_EN_OFICINA), adGuia.NumeroGuia)));
                    }

                }

                transaccion.Complete();
                return solicitud;
            }
        }

        /// <summary>
        /// Método para adicionar un estado de una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public SACSeguimientoSolicitudDC GuardarSeguimiento(SACSeguimientoSolicitudDC seguimiento)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (seguimiento.TipoSeguimiento.IdTipo == 9)
                {
                    //Obtener información de la guía
                    ADGuia  adGuia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(seguimiento.NumeroGuia);
                    //RECLAME EN OFICINA

                    if (string.Equals (adGuia.IdTipoEntrega.Trim(), "2"))
                    {
                        //Cambia la fecha de entrega al dia anterior
                        EGTipoNovedadGuia.CambiarFechaEntregaDiaAnterior(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC()
                        {
                            Guia = new ADGuia()
                            {
                                NumeroGuia = seguimiento.NumeroGuia,
                            },
                            TiempoAfectacion = 24
                        });
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.SERVICIO_AL_CLIENTE, SCEnumTipoError.EX_ERROR_GUIA_NO_RECLAME_EN_OFICINA.ToString(), string.Format(SCMensajesCliente.CargarMensaje(SCEnumTipoError.EX_ERROR_GUIA_NO_RECLAME_EN_OFICINA), adGuia.NumeroGuia)));
                    }
                }
                    SACSeguimientoSolicitudDC seg = SCRepositorio.Instancia.AdicionarSeguimiento(seguimiento);
                    //Tipo seguimiento Retiro de reclame en oficina
                transaccion.Complete();
                return seg;
            }
        }


        #endregion

    }
}
