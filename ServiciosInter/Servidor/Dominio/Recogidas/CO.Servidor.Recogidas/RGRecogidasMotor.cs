using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.OperacionUrbana;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.RAPS.Reglas.Parametros;
using CO.Servidor.Recogidas.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CO.Servidor.Recogidas
{
    public class RGRecogidasMotor : ControllerBase
    {
        private static readonly RGRecogidasMotor instancia = (RGRecogidasMotor)FabricaInterceptores.GetProxy(new RGRecogidasMotor(), COConstantesModulos.MODULO_RECOGIDAS);
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        public static RGRecogidasMotor Instancia
        {
            get { return RGRecogidasMotor.instancia; }
        }
        /// <summary>
        /// constructor
        /// </summary>
        private RGRecogidasMotor()
        { }


        /// <summary>
        /// Vence las solicitudes asignadas y retorna la lista de dispositivos para notificar el vencimiento
        /// </summary>
        /// <returns></returns>
        public List<PADispositivoMovil> VencerSolicitudesRecogidas()
        {
            return RGRepositorioMotor.Instancia.VencerSolicitudesRecogidas();
        }

        public List<PADispositivoMovil> VencerSolicitudesRecogidasV7()
        {
            IntegrarRapsRecogidasEsporadicas();
            return RGRepositorioMotor.Instancia.VencerSolicitudesRecogidas();
        }

        /// <summary>
        /// Obtiene las recogidas nuevas para notificar a los mensajeros y obtiene las recogidas para cambiar de estado a ParaForzar
        /// </summary>
        /// <returns></returns>
        public List<RGRecogidaMotorDC> ObtenerRecogidasNuevasNotificarForzar()
        {
            return RGRepositorioMotor.Instancia.ObtenerRecogidasNuevasNotificarForzar();
        }

        /// <summary>
        /// aumenta el contador de las recogidas nuevas o canceladas por mensajero
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="esNueva"></param>
        public void AumentarContadorNotificacionRecogidas(long idSolicitudRecogida, bool esNueva)
        {
            RGRepositorioMotor.Instancia.AumentarContadorNotificacionRecogidas(idSolicitudRecogida, esNueva);
        }

        public void IntegrarRapsRecogidasEsporadicas()
        {
            AdministradorReglaEstadoRAPS parametrizacionRaps = null;
            RADatosFallaDC datosFalla = null;
            RAParametrosSolicitudAcumulativaDC parametrosSolicitudAcumulativa = new RAParametrosSolicitudAcumulativaDC();
            List<RGAsignarRecogidaDC> lstRecogidasvencidas = new List<RGAsignarRecogidaDC>();
            lstRecogidasvencidas = RGRepositorioMotor.Instancia.ObtenerRecogidasVencidas();

            lstRecogidasvencidas.ForEach(r => {

                CoEnumTipoNovedadRaps tipoNovedad = CoEnumTipoNovedadRaps.Pordefecto;
                Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();
                List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
                OUDatosMensajeroDC datosMensajero = RGRepositorioMotor.Instancia.ObtenerDatosMensajeroPorCedula(Convert.ToInt64(r.DocPersonaResponsable));
                r.Mensajero = new OUDatosMensajeroDC();
                r.Mensajero.IdCentroServicios = datosMensajero.IdCentroServicios;
                r.Mensajero.NombreMensajero = datosMensajero.NombreMensajero;
                r.DocPersonaResponsable = datosMensajero.IdMensajero.ToString();
                RAFallaMapper ma = new RAFallaMapper();
                datosFalla = ma.MapperDatosFallaAutomaticaRecogidas(r, RAEnumSistemaOrigen.CONTROLLER.GetHashCode());

                if (datosMensajero.IdTipoMensajero != 7)
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Mensajero, CoEnumTipoNovedadRaps.NO_REALIZÓ_RECOGIDA_ESPORADICA_MENSAJERO_AUTO.GetHashCode());
                }
                else
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Mensajero, CoEnumTipoNovedadRaps.NO_REALIZÓ_RECOGIDA_ESPORADICA_PUNTO_AUTO.GetHashCode());
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
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, Raps.Comun.RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD.ToString(), "La falla ya fue registrada para el responsable solicitado"));
                }

            });
        }

    }
}