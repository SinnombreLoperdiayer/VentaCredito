using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Util;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Raps.Datos;
using CO.Servidor.RAPS.Reglas.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;

namespace CO.Servidor.Raps
{
    public class RASolicitud : ControllerBase
    {
        private static readonly RASolicitud instancia = (RASolicitud)FabricaInterceptores.GetProxy(new RASolicitud(), COConstantesModulos.MODULO_RAPS);
        IADFachadaAdmisionesMensajeria fachadaAdmisionMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private string rutaFolderImagenesRAPS = "";
        #region singleton

        public static RASolicitud Instancia
        {
            get { return RASolicitud.instancia; }
        }

        #endregion

        /// <summary>
        /// Crea una solucitud de tipo acumulativa
        /// </summary>
        /// <param name="regSolicitudAcumulativa"></param>
        /// <returns></returns>
        public bool CrearSolicitudAcumulativa(RegistroSolicitudAppDC regSolicitud)
        {
            bool resultado = false;
            List<RATiposDatosParametrizacionDC> lstParametros = new List<RATiposDatosParametrizacionDC>();
            DateTime fecha;
            string idSucursal = RARepositorio.Instancia.ObtenerSucursalNovasoft(regSolicitud.IdCiudad.Substring(0, 5));

            List<RASolicitudDC> listaParametrizaciones = RARepositorio.Instancia.ObtenerParametrizacionesSegunNovedad(regSolicitud.IdSistema, regSolicitud.IdTipoNovedad, regSolicitud.IdCiudad.Substring(0, 5));
            var rutaImagenes = PAParametros.Instancia.ConsultarParametrosFramework("FoldImgFallaAPP");
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    listaParametrizaciones.ForEach(solicitud =>
                    {

                        long idSolicitudAcumulativa = RARepositorioSolicitudes.Instancia.CrearSolicitudAcumulativa(solicitud);
                        //insertar parametros
                        lstParametros = RASolicitudes.Instancia.ObtenerParamametroPorIdDeParametrizacion(solicitud.IdParametrizacionRap);
                        lstParametros.ForEach(parametro =>
                        {
                            if (regSolicitud.ValoresParametros.ContainsKey(parametro.IdTipoParametro.ToString()))
                            {
                                bool correspondeTipoDato = false;
                                switch (parametro.IdTipoDato)
                                {
                                    case (int)RAEnumTipoDato.NUMERO:
                                        if (regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()].ToString().All(char.IsDigit))
                                            correspondeTipoDato = true;
                                        break;
                                    case (int)RAEnumTipoDato.CADENA:
                                        if (regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()] is string)
                                            correspondeTipoDato = true;
                                        break;
                                    case (int)RAEnumTipoDato.FECHA:
                                        //parametros[parametro.IdTipoDato.ToString()] is DateTime
                                        if (DateTime.TryParse(regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()].ToString(), out fecha))
                                            correspondeTipoDato = true;
                                        break;
                                    case (int)RAEnumTipoDato.TIPONOVEDAD:
                                        if (!RARepositorio.Instancia.ConsultarTipoNovedad(Convert.ToInt32(regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()]), parametro.IdTipoParametro))
                                            correspondeTipoDato = true;
                                        break;
                                    case (int)RAEnumTipoDato.ADJUNTO:
                                        if (regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()].ToString() is string)
                                        {
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()].ToString()))
                                                {
                                                    regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()] = RASolicitudes.Instancia.ObtenerRutaImagen(regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()].ToString(), rutaImagenes);
                                                }
                                                correspondeTipoDato = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                correspondeTipoDato = false;
                                            }
                                        }
                                        break;
                                    case (int)RAEnumTipoDato.FOTOGRAFIA:
                                        if (regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()].ToString() is string)
                                        {
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()].ToString()))
                                                {
                                                    regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()] = RASolicitudes.Instancia.ObtenerRutaImagen(regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()].ToString(), rutaImagenes);
                                                }
                                                correspondeTipoDato = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                correspondeTipoDato = false;
                                            }
                                        }
                                        break;
                                    default:
                                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO)));

                                }
                                if (!correspondeTipoDato)
                                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO)));
                                else
                                {

                                    RARepositorio.Instancia.InsertarParametroSolicitudAcumulativa(idSolicitudAcumulativa, parametro.IdTipoParametro, regSolicitud.ValoresParametros[parametro.IdTipoParametro.ToString()].ToString());
                                    resultado = true;
                                }
                            }
                            else
                            {
                                resultado = false;
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO)));
                            }
                        });
                    });
                    trans.Complete();
                }
                catch (Exception ex)
                {
                    Utilidades.AuditarExcepcion(ex, true);
                    throw ex;
                }
            }
            return resultado;
        }

        /// <summary>
        /// Crea una solicitud acumulativa para cuando se conoce el responsable
        /// </summary>
        /// <param name="regSolicitud"></param>
        /// <returns></returns>
        public bool CrearSolicitudAcumulativaManualesConoceResponsable(RegistroSolicitudAppDC regSolicitud)
        {
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();
            ADGuia datosGuia = new ADGuia();

            if (!fachadaAdmisionMensajeria.VerificarSiGuiaExiste(regSolicitud.NumeroGuia))
            {
                throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_GUIA_NO_EXISTE)));                
            }

            lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracionPorNovedad(regSolicitud.IdTipoNovedad, (RAEnumOrigenRaps)Enum.Parse(typeof(RAEnumOrigenRaps), regSolicitud.IdOrigenRaps.ToString()));

            if (regSolicitud.Parametros != null)
            {
                foreach (var itemp in lstParametros)
                {
                    if (regSolicitud.Parametros.Exists(i => i.IdParametro == itemp.IdParametro))
                    {
                        parametrosParametrizacion.Add(itemp.IdParametro.ToString(), regSolicitud.Parametros.Find(i => i.IdParametro == itemp.IdParametro).Valor);
                    }
                }
            }

            regSolicitud.ValoresParametros = parametrosParametrizacion;

            return CrearSolicitudAcumulativa(regSolicitud);
        }


        /// <summary>
        /// Crear Solicitud Acumulativa Personalizada
        /// </summary>
        /// <param name="regSolicitud"></param>
        /// <returns></returns>
        public bool CrearSolicitudAcumulativaPersonalizada(RegistroSolicitudAppDC regSolicitud)
        {
            RAParametrosSolicitudAcumulativaDC parametrosSolicitudAcumulativa = null;
            bool solicitudCreada = false;
            RADatosFallaDC datosFalla = null;

            /************************************** CONSULTAR IDNOVEDAD  ************************************************/

            /*ASIGNA NOVEDAD HIJA*/

            regSolicitud.IdTipoNovedad = RAIntegracionRaps.Instancia.ObtieneTipoNovedad(regSolicitud.IdTipoNovedad, regSolicitud.IdResponsable);

            if (!ValidaObjetoSolicitud(regSolicitud))
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_NOVEDAD_HIJA)));                
            }

            datosFalla = new RADatosFallaDC()
            {
                IdCentroLogistico = regSolicitud.DatosReponsable.Id,
                DocPersonaResponsable = regSolicitud.DatosReponsable.IdentificacionResponsable,
                NombreCompleto = regSolicitud.DatosReponsable.Nombre,

                Ciudad = regSolicitud.Guia.NombreCiudadOrigen,
                FechaAdmision = regSolicitud.Guia.FechaAdmision,
                NumeroGuia = regSolicitud.Guia.NumeroGuia,
                Observaciones = regSolicitud.Guia.Observaciones,
                IdCentroServicioDestino = regSolicitud.Guia.IdCentroServicioDestino,

                Adjunto = regSolicitud.Adjunto,
                FechaAsignacion = DateTime.Now,
                FechaDescarga = DateTime.Now,
                Foto = regSolicitud.Foto,
                TipoObjeto = regSolicitud.Adjunto,
                Parametros = regSolicitud.Parametros,
                IdTipoNovedad = regSolicitud.IdTipoNovedad,
                IdCiudad = regSolicitud.IdCiudad,
                IdSistema = regSolicitud.IdSistema,
            };

            parametrosSolicitudAcumulativa = new RAParametrosSolicitudAcumulativaDC();
            parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, (RAEnumOrigenRaps)Enum.ToObject(typeof(RAEnumOrigenRaps), regSolicitud.IdResponsable), regSolicitud.IdTipoNovedad);


            RegistroSolicitudAppDC registroSolicitud = new RegistroSolicitudAppDC
            {
                IdSistema = datosFalla.IdSistema,
                IdTipoNovedad = parametrosSolicitudAcumulativa.TipoNovedad.GetHashCode(),
                ValoresParametros = parametrosSolicitudAcumulativa.Parametrosparametrizacion,
                IdCiudad = datosFalla.IdCiudad,
            };

            /*****************************************CREA SOLICITUD ACUMULATIVA********************************************************/
            if (!parametrosSolicitudAcumulativa.EstaEnviado)
            {
                if (parametrosSolicitudAcumulativa.TipoNovedad != CoEnumTipoNovedadRaps.Pordefecto && parametrosSolicitudAcumulativa.Parametrosparametrizacion.Count > 0)
                {
                    solicitudCreada = CrearSolicitudAcumulativa(registroSolicitud);
                }
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_FALLA_YA_REGISTRADA_MISMO_RESPONSABLE)));                
            }

            return solicitudCreada;
        }

        private bool ValidaObjetoSolicitud(RegistroSolicitudAppDC regSolicitud)
        {

            if (regSolicitud.DatosReponsable == null)
            {
                return false;
            }
            else if (regSolicitud.Guia == null)
            {
                return false;
            }
            else if (regSolicitud.Parametros == null)
            {
                return false;
            }
            else if (regSolicitud.IdTipoNovedad == 0)
            {
                return false;
            }
            else if (regSolicitud.IdSistema == 0)
            {
                return false;
            }
            else if (regSolicitud.IdCiudad == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}
