using System;
using System.Collections.Generic;
using System.Linq;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System.Reflection;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.CentroServicios;
using CO.Servidor.RAPS.Reglas.Automaticas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Raps.Comun;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.RAPS.Reglas.Automaticas
{
    public class ParametrizacionRaps
    {
        private static readonly ParametrizacionRaps instancia = new ParametrizacionRaps();

        public static ParametrizacionRaps Instancia
        {
            get
            {
                return instancia;
            }
        }

        //    public void IntegrarRapFallas(IDictionary<string, object> datos, RAEnumOrigenRaps origenRaps)
        //public void IntegrarRapFallas(RADatosFallaAutomatica datos, RAEnumOrigenRaps origenRaps)
        //{
        //    //if (datos.ContainsKey("guia"))
        //    //{
        //    //    OUGuiaIngresadaDC guia = new OUGuiaIngresadaDC();
        //    //    guia = (OUGuiaIngresadaDC)(datos["guia"]);
        //    //}
        //    Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();
        //    CoEnumTipoNovedadRaps tipoNovedad = CoEnumTipoNovedadRaps.Pordefecto;
        //    if (!ParametrizacionRaps.Instancia.CreaParametrosRaps(datos, origenRaps, ref tipoNovedad, out parametrosParametrizacion))
        //    {
        //        return;
        //    }

        //    if (tipoNovedad != CoEnumTipoNovedadRaps.Pordefecto)
        //        //RAIntegracionRaps.Instancia.CrearSolicitudAcumulativaRaps((Raps.Comun.Integraciones.EnumTipoNovedadRaps)motivoRaps.GetHashCode(), parametrosParametrizacion, guia.IdCiudad.Substring(0, 5), ControllerContext.Current.Usuario);
        //        RAIntegracionRaps.Instancia.CrearSolicitudAcumulativaRaps((CoEnumTipoNovedadRaps)tipoNovedad.GetHashCode(), parametrosParametrizacion, datos.IdCiudad, ControllerContext.Current.Usuario);
        //}

        //public void IntegrarRapFallasSinGuia(RADatosFallaAutomatica datos, RAEnumOrigenRaps origenRaps, int novedad)
        //{
        //    Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();
        //    CoEnumTipoNovedadRaps tipoNovedad = (CoEnumTipoNovedadRaps)novedad;
        //    if (!CreaParametrosRaps(datos, origenRaps, ref tipoNovedad, out parametrosParametrizacion))
        //    {
        //        return;
        //    }

        //    if (tipoNovedad != CoEnumTipoNovedadRaps.Pordefecto)
        //    {
        //        //RAIntegracionRaps.Instancia.CrearSolicitudAcumulativaRaps((Raps.Comun.Integraciones.EnumTipoNovedadRaps)motivoRaps.GetHashCode(), parametrosParametrizacion, guia.IdCiudad.Substring(0, 5), ControllerContext.Current.Usuario);
        //        RAIntegracionRaps.Instancia.CrearSolicitudAcumulativaRaps((CoEnumTipoNovedadRaps)tipoNovedad.GetHashCode(), parametrosParametrizacion, datos.IdCiudad, ControllerContext.Current == null ? "MotorRaps" : ControllerContext.Current.Usuario);
        //    }
        //}

        ///// <summary>
        ///// Metodo para crear parametros 
        ///// </summary>
        ///// <param name="guia"></param>
        ///// <param name="motivoRaps"></param>
        ///// <param name="parametrosParametrizacion"></param>
        ///// <returns></returns>
        ///// 


        //public bool CreaParametrosRaps(RADatosFallaAutomatica datos, RAEnumOrigenRaps origenRaps, ref CoEnumTipoNovedadRaps tipoNovedad, out Dictionary<string, object> parametrosParametrizacion)
        //{

        //    //IDictionary<string, object> datos = new Dictionary<string, object>();
        //    //datos.Add("guia", guia);

        //   // AdministradorReglaEstadoRAPS admin = new AdministradorReglaEstadoRAPS(origenRaps, datos);
        //    // admin.LLenarLstReglas();
        //    //admin.ConsultarParametrosIntegracionPorNovedad(tipoNovedad);
        //    //admin.EjecucionReglasMotivosRaps(out tipoNovedad, out parametrosParametrizacion);


        //    return true;
        //}



        /// <summary>
        /// Metodo para obtener parametros por integracion 
        /// </summary>
        /// <param name="tipoParametro"></param>
        /// <returns></returns>
        //public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracion(int tipoMotivo)
        //{
        //    return RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion(tipoMotivo);
        //}
    }
}
