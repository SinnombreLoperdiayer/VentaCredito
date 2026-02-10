using CO.Servidor.CentroServicios;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun.ReglasNegocioRAPS
{
    public class AdministradorReglaEstadoRAPS
    {
        //private List<ParametrosReglasEstadosRaps> lstParametrosReglas;
        //private List<ParametrosReglasEstadosRaps> lstParametrosReglasParametros;
        private int idMotivoGuia;
        private OUGuiaIngresadaDC guia;
        private Dictionary<int, IEstadoRAPS> ListaEjecucionFunciones;
        private string[] datosMensajero;
        private PUAgenciaDeRacolDC racolDestino = new PUAgenciaDeRacolDC();
        private List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
        // private EnumTipoNovedadRaps motivoRaps;
        // private Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();

        public AdministradorReglaEstadoRAPS()
        {
        }

        /// <summary>
        /// Llena lista ejecucion funciones y clases
        /// </summary>
        /// <param name="guia"></param>
        public void LLenarLstReglas(int origenRaps, OUGuiaIngresadaDC guia)
        {
            this.origenRaps = origenRaps;
            this.guia = guia;
            idMotivoGuia = guia.Motivo.IdMotivoGuia;
            datosMensajero = guia.NombreCompleto == null ? new string[1] : guia.NombreCompleto.Split('-');
            racolDestino = PUCentroServicios.Instancia.ObtenerRacolResponsable(guia.IdCentroServicioDestino);


            ListaEjecucionFunciones = new Dictionary<int, IEstadoRAPS>();

            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Numero_Guia.GetHashCode(), new ReglaMapeoNumeroGuia());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Fecha_Descarga.GetHashCode(), new ReglaMapeoFechaDescarga());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Nombre_Completo_Mensajero.GetHashCode(), new ReglaMapeoNombreCompleto());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Identificacion_Mensajero.GetHashCode(), new ReglaMapeoIdentificacionMensajero());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_IdCol.GetHashCode(), new ReglaMapeoIdCol());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Observaciones_Guia.GetHashCode(), new ReglaMapeoObservaciones());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Codigo_Centro_Servicio_Destino.GetHashCode(), new ReglaMapeoCodigoCentroServicioDestino());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Nombre_Centro_Servicio_Destino.GetHashCode(), new ReglaMapeoNombreCentroServicioDestino());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_IdRacol_Destino.GetHashCode(), new ReglaMapeoIdRacol());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Fecha_Novedad.GetHashCode(), new ReglaMapeoFechaNovedad());
            ListaEjecucionFunciones.Add(EnumFuncionesReglasParametrizacionRaps.Parametro_Ciudad_Novedad.GetHashCode(), new ReglaMapeoCiudadNovedad());
        }

        //public void ConsultarReglasParametrizacionEstadosRaps(int idMotivo)
        //{
        //    //lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion("Averiado");

        //    lstParametrosReglas = RAIntegracionRaps.Instancia.ObtenerParametrizacionReglasEstadosRaps(idMotivo);
        //}

        //public void paObtenerParametrizacionReglasParametrosRaps(int idMotivo)
        //{
        //    //lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion("Averiado");

        //    lstParametrosReglasParametros = RAIntegracionRaps.Instancia.paObtenerParametrizacionReglasParametrosRaps(idMotivo);
        //}

        /// <summary>
        /// consulta parametros por integracion raps
        /// </summary>
        public void ConsultarParametrosIntegracion()
        {
            lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion(idMotivoGuia);
        }

        /// <summary>
        /// asigna el tipo de novedad 
        /// </summary>
        /// <param name="IdTipoNovedad"></param>
        /// <param name="motivoRaps"></param>
        public void AsignarTipoNovedad(int IdTipoNovedad, ref CoEnumTipoNovedadRaps tipoNovedad)
        {
            tipoNovedad = (CoEnumTipoNovedadRaps)IdTipoNovedad;
        }

        /// <summary>
        /// ejecuta reglas motivos raps
        /// </summary>
        /// <param name="motivoRaps"></param>
        /// <param name="parametrosParametrizacion"></param>
        public void EjecucionReglasMotivosRaps(out CoEnumTipoNovedadRaps tipoNovedad, out Dictionary<string, object> parametrosParametrizacion)
        {
            parametrosParametrizacion = new Dictionary<string, object>();
            tipoNovedad = new CoEnumTipoNovedadRaps();
            //  ConsultarReglasParametrizacionEstadosRaps(idMotivoGuia);
            ConsultarParametrosIntegracion();

            if (ListaEjecucionFunciones != null)
            {
                //foreach (var item in lstParametrosReglas)
                //{

                foreach (var itemp in lstParametros)
                {
                    ListaEjecucionFunciones[itemp.IdFuncion].EjecutarReglaParametros(guia, itemp.IdParametro, ref parametrosParametrizacion);
                }
                tipoNovedad = (CoEnumTipoNovedadRaps)lstParametros[0].IdTipoNovedad;
                //AsignarTipoNovedad(lstParametros[0].IdTipoNovedad,ref motivoRaps);
                //ListaEjecucionFunciones[item.IdFuncion].EjecutarRegla(guia, lstParametros, datosMensajero, racolDestino, out motivoRaps, item.IdParametroAsociado);

                // }
            }
        }

    }
}
