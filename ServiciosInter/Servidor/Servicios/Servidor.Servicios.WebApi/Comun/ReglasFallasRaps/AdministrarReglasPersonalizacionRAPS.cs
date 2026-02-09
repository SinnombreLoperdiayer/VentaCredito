using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Comun.ReglasFallasRaps
{
    public class AdministrarReglasPersonalizacionRAPS
    {
        private int idTipoNovedadConsultada;

        private List<ContratoDatos.Raps.Configuracion.RAParametrosPersonalizacionRapsDC> listaParametros;

        private ContratoDatos.OperacionUrbana.OUDatosMensajeroDC datosMensajero;
        private ContratoDatos.Admisiones.Mensajeria.ADGuia datosGuia;

        private Dictionary<int, IEjecucionConsulta> listaFunciones;

        public AdministrarReglasPersonalizacionRAPS(int idTipoNovedad, 
            ContratoDatos.OperacionUrbana.OUDatosMensajeroDC mensajero, 
            ContratoDatos.Admisiones.Mensajeria.ADGuia guia)
        {
            idTipoNovedadConsultada = idTipoNovedad;
            datosMensajero = mensajero;
            datosGuia = guia;

            listaFunciones = new Dictionary<int, IEjecucionConsulta>();

            listaFunciones.Add(EnumReglasFallas.OBTENER_NOMBRE_RESPONSABLE.GetHashCode(), new ReglaObtenerNombreResponsable());
            listaFunciones.Add(EnumReglasFallas.OBTENER_ID_COL_RESPONSABLE.GetHashCode(), new ReglaObtenerIdColResponsable());
            listaFunciones.Add(EnumReglasFallas.OBTENER_IDENTIFICACION_RESPONSABLE.GetHashCode(), new ReglaObtenerIdentificacionResponsable());
            listaFunciones.Add(EnumReglasFallas.OBTENER_NUMERO_GUIA.GetHashCode(), new ReglaObtenerNumeroGuia());
            listaFunciones.Add(EnumReglasFallas.OBTENER_FECHA_ADMISION.GetHashCode(), new ReglaObtenerFechaAdmision());
            listaFunciones.Add(EnumReglasFallas.OBTENER_FECHA_DESCARGA.GetHashCode(), new ReglaObtenerFechaDescarga());
            listaFunciones.Add(EnumReglasFallas.OBTENER_FECHA_ASIGNACION.GetHashCode(), new ReglaObtenerFechaAsignacion());
            listaFunciones.Add(EnumReglasFallas.OBTENER_OBSERVACION.GetHashCode(), new ReglaObtenerObservacion());
            listaFunciones.Add(EnumReglasFallas.OBTENER_ADJUNTO.GetHashCode(), new ReglaObtenerAdjunto());
            listaFunciones.Add(EnumReglasFallas.OBTENER_FOTOGRAFIA.GetHashCode(), new ReglaObtenerFotografia());
        }

        private void ConsultarParametrosNovedad()
        {
            listaParametros = FabricaServicios.ServicioConfiguracionRaps.ListaParametrosPersonalizacionPorNovedad(idTipoNovedadConsultada);
        }


        public Dictionary<string, object> ObjtenerParametros()
        {
            if(listaParametros == null || listaParametros.Count() < 1)
            {
                throw new Exception("No existen parametros para aplicar las reglas");
            }

            if (idTipoNovedadConsultada == 0)
            {
                throw new Exception("Debe proporcionar el tipo de novedad para realizar el proceso");
            }

            if (datosMensajero == null)
            {
                throw new Exception("Debe proporcionar los datos del mensajero o dueño del suministro para realizar el proceso");
            }

            if (datosGuia == null)
            {
                throw new Exception("Debe proporcionar los datos de la guía para realizar el proceso");
            }

            var diccionario = new Dictionary<string, object>();

            ConsultarParametrosNovedad();

            foreach(var item in listaParametros)
            {
                diccionario.Add(item.IdParametro.ToString(), listaFunciones[item.IdFuncion].EjecucionRegla(datosMensajero, datosGuia));
            }

            return diccionario;
        }

    }
}