using CO.Servidor.Raps.Comun;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public class AdministradorReglaEstadoRAPS : ControllerBase
    {
        #region Singleton
        private static readonly AdministradorReglaEstadoRAPS instancia = (AdministradorReglaEstadoRAPS)FabricaInterceptores.GetProxy(new AdministradorReglaEstadoRAPS(), COConstantesModulos.MODULO_RAPS);

        /// <summary>
        /// Singleton
        /// </summary>
        public static AdministradorReglaEstadoRAPS Instancia
        {
            get { return AdministradorReglaEstadoRAPS.instancia; }
        }
        public AdministradorReglaEstadoRAPS()
        {
            LLenarLstReglas();

        }
        #endregion

        #region Parametros
        private Dictionary<int, IEstadoRAPS> ListaEjecucionFunciones;

        #endregion


        #region MyRegion
        /// <summary>
        /// Llena lista ejecucion funciones y clases
        /// </summary>
        /// <param name="guia"></param>
        public void LLenarLstReglas()
        {

            ListaEjecucionFunciones = new Dictionary<int, IEstadoRAPS>();

            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Numero_Guia.GetHashCode(), new ReglaMapeoNumeroGuia());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Fecha_Descarga.GetHashCode(), new ReglaMapeoFechaDescarga());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Nombre_Completo_Responsable.GetHashCode(), new ReglaMapeoNombreCompletoResponsable());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Identificacion_Responsable.GetHashCode(), new ReglaMapeoIdentificacionResponsable());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_IdCol.GetHashCode(), new ReglaMapeoIdCol());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Observaciones_Guia.GetHashCode(), new ReglaMapeoObservaciones());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Codigo_Centro_Servicio_Destino.GetHashCode(), new ReglaMapeoCodigoCentroServicioDestino());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Nombre_Centro_Servicio_Destino.GetHashCode(), new ReglaMapeoNombreCentroServicioDestino());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_IdRacol_Destino.GetHashCode(), new ReglaMapeoIdRacol());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Fecha_Novedad.GetHashCode(), new ReglaMapeoFechaNovedad());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Ciudad_Novedad.GetHashCode(), new ReglaMapeoCiudadNovedad());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_IdCliente.GetHashCode(), new ReglaMapeoIdCliente());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_NombreCliente.GetHashCode(), new ReglaMapeoNombreCliente());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_IdCiudad.GetHashCode(), new ReglaMapeoIdCiudad());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Fecha_Programada.GetHashCode(), new ReglaMapeoFechaProgramada());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Direccion_Recogida.GetHashCode(), new ReglaMapeoDireccionRecogida());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Fecha_Admision.GetHashCode(), new ReglaMapeoFechaAdmision());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Fecha_Asignacion.GetHashCode(), new ReglaMapeoFechaAsignacion());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Adjunto.GetHashCode(), new ReglaMapeoAdjunto());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Fotografia.GetHashCode(), new ReglaMapeoFotografia());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Tipo_Objeto.GetHashCode(), new ReglaMapeoTipoObjeto());
        }

        /// <summary>
        /// Integrar fallas raps 
        /// </summary>
        /// <param name="datos"></param>
        /// <param name="origenRaps"></param>
        /// <param name="novedad"></param>
        public RAParametrosSolicitudAcumulativaDC IntegrarRapFallas(RADatosFallaDC datos, RAEnumOrigenRaps origenRaps, int novedad = 0)
        {
            Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();
            CoEnumTipoNovedadRaps tipoNovedad = (CoEnumTipoNovedadRaps)novedad;
            bool esUnicoEnvio = false;
            bool estaEnviado = false;
            int idParametroAgrupamiento = 0;
            RAParametrosSolicitudAcumulativaDC solicitud = null;

            parametrosParametrizacion = EjecucionReglasMotivosRaps(datos, origenRaps, ref tipoNovedad);

            if ((parametrosParametrizacion != null && parametrosParametrizacion.Count > 1) || tipoNovedad == 0)
            {
                idParametroAgrupamiento = Convert.ToInt32(parametrosParametrizacion["IdParametroAgrupamiento"]);
                /*****************************************VALIDA SI PARAMETRIZACION RESTRINGE UNICO ENVIO*******************************************/
                esUnicoEnvio = RAIntegracionRaps.Instancia.ValidarParametrizacionEsUnicoEnvio(tipoNovedad.GetHashCode());
                /*****************************************VALIDA SI EXISTE UNA SOLICITUD PARA RESPONSABLE YA CREADA*******************************/
                if (esUnicoEnvio)
                {
                    estaEnviado = RAIntegracionRaps.Instancia.ValidarExisteSolicitudParaResponsable(idParametroAgrupamiento, parametrosParametrizacion[idParametroAgrupamiento.ToString()].ToString());
                }

                solicitud = new RAParametrosSolicitudAcumulativaDC()
                {
                    Parametrosparametrizacion = parametrosParametrizacion,
                    TipoNovedad = tipoNovedad,
                    EstaEnviado = estaEnviado,
                };
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_NO_ASIGNO_TIPONOVEDAD)));
            }

            return solicitud;
        }




        /// <summary>
        /// ejecuta reglas motivos raps
        /// </summary>
        /// <param name="motivoRaps"></param>
        /// <param name="parametrosParametrizacion"></param>
        public Dictionary<string, object> EjecucionReglasMotivosRaps(RADatosFallaDC datos, RAEnumOrigenRaps origenRaps, ref CoEnumTipoNovedadRaps tipoNovedad)
        {

            Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();

            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();

            //CONSULTA PARAMETROS POR MOTIVOGUIA O POR TIPONOVEDAD

            if (tipoNovedad.GetHashCode() == 0)
            {
                lstParametros = RAIntegracionesRaps.Instancia.ObtenerParametrosPorIntegracion(datos.IdMotivoGuia, origenRaps);

            }
            else
            {
                lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracionPorNovedad(tipoNovedad.GetHashCode(), origenRaps);
            }

            //VALIDA PARAMETROS,FUNCIONES 

            if (lstParametros.Count == 0 || ListaEjecucionFunciones == null)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_NO_ASIGNO_TIPONOVEDAD)));
            }

            //AGREGA PARAMETROSPARAMETRIZACION CON PARAMETROS LLENADOS EN CLIENTE
            if (datos.Parametros != null)
            {
                foreach (var itemp in lstParametros)
                {
                    if (datos.Parametros.Exists(i => i.IdParametro == itemp.IdFuncion))
                    {
                        parametrosParametrizacion.Add(itemp.IdParametro.ToString(), datos.Parametros.Find(i => i.IdParametro == itemp.IdFuncion).Valor);
                    }
                }
            }
            //AGREGA IDPARAMETROAGRUPAMIENTO
            parametrosParametrizacion.Add("IdParametroAgrupamiento", lstParametros.Where(l => l.EsAgrupamiento == true).ToList()[0].IdParametro);

            //AGREGA PARAMETROSPARAMETRIZACION CON LLENADO DESDE EJECUCION REGLAS
            foreach (var itemp in lstParametros)
            {
                if (!parametrosParametrizacion.ContainsKey(itemp.IdParametro.ToString()))
                {
                    parametrosParametrizacion.Add(itemp.IdParametro.ToString(), ListaEjecucionFunciones[itemp.IdFuncion].EjecutarReglaParametros(datos));
                }
            }
            tipoNovedad = (CoEnumTipoNovedadRaps)lstParametros[0].IdTipoNovedad;

            return parametrosParametrizacion;
        }




        #endregion



    }
}
