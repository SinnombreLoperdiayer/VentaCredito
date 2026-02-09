using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Adminisiones.Mensajeria.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.RAPS.Reglas.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System.ServiceModel;
using CO.Servidor.Raps.Comun;

namespace CO.Servidor.Adminisiones.Mensajeria.Novedades
{
    public class ADNovedades : ControllerBase
    {
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        #region crear Instancia

        private static readonly ADNovedades instancia = (ADNovedades)FabricaInterceptores.GetProxy(new ADNovedades(), COConstantesModulos.MENSAJERIA);

        /// <summary>
        /// Retorna una instancia para el manejo de novedades en mensajeria
        /// /// </summary>
        public static ADNovedades Instancia
        {
            get { return ADNovedades.instancia; }
        }

        #endregion crear Instancia

        #region Metodos

        /// <summary>
        /// Adicionar novedades de una guia
        /// </summary>
        /// <param name="novedad"></param>
        public void AdicionarNovedad(ADNovedadGuiaDC novedad, Dictionary<CCEnumNovedadRealizada, string> datosAdicionalesNovedad)
        {
            ADRepositorio.Instancia.AdicionarNovedad(novedad, datosAdicionalesNovedad);
        }

        /// <summary>|
        /// Actualizar destino de una guia
        /// </summary>
        /// <param name="centroServicioDestino"></param>
        public void ActualizarDestinoGuia(long idAdmisionMensajeria, PUCentroServiciosDC centroServicioDestino, CCReliquidacionDC valorReliquidado, TAEnumFormaPago? formaPago, string idTipoEntrega, string descripcionTipoEntrega)
        {
            ADRepositorio.Instancia.ActualizarDestinoGuia(idAdmisionMensajeria, centroServicioDestino, valorReliquidado, formaPago, idTipoEntrega, descripcionTipoEntrega);
        }
        /// <summary>
        /// Actualiza en la base de datos los datos del remitente y del destinatario de una guía o factura
        /// </summary>
        /// <param name="novedadGuia"></param>

        public void ActualizarRemitenteDestinatarioGuia(CCNovedadCambioRemitenteDC novedadGuia)
        {
            ADRepositorio.Instancia.ActualizarRemitenteDestinatarioGuia(novedadGuia);
        }



        /// <summary>
        /// Actualiza en la base de datos los datos del remitente y del destinatario de una guía o factura
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void ActualizarRemitenteDestinatarioGuiaV7(CCNovedadCambioRemitenteDC novedadGuia)
        {
            ADRepositorio.Instancia.ActualizarRemitenteDestinatarioGuia(novedadGuia);
            IntegrarRapsNovedadesGuia(novedadGuia.Guia.IdAdmision, CCEnumTipoNovedadGuia.ModificarDestinoGuia);
        }

        /// <summary>
        /// metodo para integrar la falla de recogidas segun corresponda
        /// </summary>
        /// <param name="gestion"></param>
        /// <param name="guia"></param>
        private void IntegrarRapsNovedadesGuia(long idAdmisionGuia, CCEnumTipoNovedadGuia tipoNovedadModificacion)
        {
            RGEmpleadoDC datosEmpleado = ADRepositorio.Instancia.ObtenerDatosEmpleadoPorIdAdmisionGuiaAdmitida(idAdmisionGuia);
            ADGuia guia = ADRepositorio.Instancia.ObtenerGuia(idAdmisionGuia);
            //PUAgenciaDeRacolDC colResponsable = fachadaCentroServicio.ObteneColPropietarioBodega(datosEmpleado.IdCentroServicios);
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            RADatosFallaDC datosFalla = null;
            RAFallaMapper ma = null;
            RAParametrosSolicitudAcumulativaDC parametrosSolicitudAcumulativa = new RAParametrosSolicitudAcumulativaDC();
            ma = new RAFallaMapper();
            datosFalla = ma.MapperDatosFallaAutomaticaGuia(guia, datosEmpleado, Servidor.Servicios.ContratoDatos.Raps.Configuracion.RAEnumSistemaOrigen.CONTROLLER.GetHashCode());

            if (tipoNovedadModificacion == CCEnumTipoNovedadGuia.ModificarDestinoGuia)
            {
                parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Agencias, CoEnumTipoNovedadRaps.GUIA_MAL_EDITADA_DATOS_AGE_AUTO.GetHashCode());
            }

            else if (tipoNovedadModificacion == CCEnumTipoNovedadGuia.ModificarPesoGuia)
            {
                if (datosEmpleado.TipoCentroServicio == PUEnumTipoCentroServicioDC.PTO)
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Puntos, CoEnumTipoNovedadRaps.GUIA_MAL_DILIGENCIADA_PESO_PTO_AUTO.GetHashCode());
                }
                else if (datosEmpleado.TipoCentroServicio == PUEnumTipoCentroServicioDC.AGE)
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Agencias, CoEnumTipoNovedadRaps.GUIA_MAL_DILIGENCIADA_PESO_AGE_AUTO.GetHashCode());
                }
            }
            else if (tipoNovedadModificacion == CCEnumTipoNovedadGuia.ModificarTipoServicioGuia)
            {
                if (datosEmpleado.TipoCentroServicio == PUEnumTipoCentroServicioDC.PTO)
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Puntos, CoEnumTipoNovedadRaps.GUIA_MAL_DILIGENCIADA_TIPO_SERVICIO_PTO_AUTO.GetHashCode());
                }
                else if (datosEmpleado.TipoCentroServicio == PUEnumTipoCentroServicioDC.AGE)
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Agencias, CoEnumTipoNovedadRaps.GUIA_MAL_DILIGENCIADA_TIPO_SERVICIO_AGE_AUTO.GetHashCode());
                }
            }

            /*****************************************CREA SOLICITUD ACUMULATIVA********************************************************/
            if (!parametrosSolicitudAcumulativa.EstaEnviado)
            {
                if (parametrosSolicitudAcumulativa.TipoNovedad != CoEnumTipoNovedadRaps.Pordefecto && parametrosSolicitudAcumulativa.Parametrosparametrizacion.Count > 0)
                {
                    RAIntegracionesRaps.Instancia.CrearSolicitudAcumulativaRaps((CoEnumTipoNovedadRaps)parametrosSolicitudAcumulativa.TipoNovedad.GetHashCode(), parametrosSolicitudAcumulativa.Parametrosparametrizacion, datosFalla.IdCiudad.Substring(0, 5), ControllerContext.Current == null ? "MotorRaps" : ControllerContext.Current.Usuario, datosFalla.IdSistema);
                }
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD.ToString(), "La falla ya fue registrada para el responsable solicitado"));
            }
        }


        /// <summary>
        /// Actualizar formas de pago de una guia
        /// </summary>
        public void ActualizarFormaPagoGuia(CCNovedadCambioFormaPagoDC novedadGuia)
        {
            ADRepositorio.Instancia.ActualizarFormaPagoGuia(novedadGuia, EnumEstadoRegistro.MODIFICADO);
        }

        /// <summary>
        /// Actualiza el tipo de servicio de una admisión
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void ActualizarTipoServicioGuia(long idAdmisionGuia, int idServicio)
        {
            ADRepositorio.Instancia.ActualizarTipoServicioGuia(idAdmisionGuia, idServicio);
            IntegrarRapsNovedadesGuia(idAdmisionGuia, CCEnumTipoNovedadGuia.ModificarDestinoGuia);
        }

        /// <summary>
        /// Actualizar es alcobro
        /// </summary>
        public void ActualizarEsAlCobro(long idAdmisionGuia, bool esAlCobro)
        {
            ADRepositorio.Instancia.ActualizarEsAlCobro(idAdmisionGuia, esAlCobro);
        }

        /// <summary>
        /// Método para actualizar
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <param name="numeroGuia"></param>
        public void ActualizarNotificacion(long idAdmision, long numeroGuia)
        {
            ADRepositorio.Instancia.ActualizarNotificacion(idAdmision, numeroGuia);
        }


        /// <summary>
        /// Método para actualizar una notificacion cuando es sacada de una planilla
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <param name="numeroGuia"></param>
        public void ActualizarNotificacionPlanilla(long idAdmision)
        {
            ADRepositorio.Instancia.ActualizarNotificacionPlanilla(idAdmision);
        }

        /// <summary>
        /// Actualizar el valor total de una guía dada
        /// </summary>
        /// <param name="idAdmisionGuia"></param>
        /// <param name="ValorTotal"></param>
        public void ActualizarValorTotalGuia(CCNovedadCambioValorTotal novedadGuia)
        {
            ADRepositorio.Instancia.ActualizarValorTotalGuia(novedadGuia);
        }

        /// <summary>
        /// Actualizar valores de la guía dada
        /// </summary>
        /// <param name="idAdmisionGuia">nmero de la guia</param>
        /// <param name="valores">valores a modificar</param>
        /// <param name="valorAdicionales">valores adicionales que se agregan al total</param>
        public void ActualizarValoresGuia(long idAdmisionGuia, CCReliquidacionDC valores, decimal valorAdicionales)
        {
            ADRepositorio.Instancia.ActualizarValoresGuia(idAdmisionGuia, valores, valorAdicionales);
        }

        /// <summary>
        /// Actualiza el Valor del Peso de la Guía dada
        /// </summary>
        /// <param name="idAdmisionGuia">numeor de la Guía</param>
        /// <param name="valorPeso">Valor del peso a actualizar</param>
        public void ActualizarValorPesoGuia(long idAdmisionGuia, decimal valorPeso)
        {
            ADRepositorio.Instancia.ActualizarValorPesoGuia(idAdmisionGuia, valorPeso);
            IntegrarRapsNovedadesGuia(idAdmisionGuia, CCEnumTipoNovedadGuia.ModificarDestinoGuia);
        }

        #endregion Metodos
    }
}