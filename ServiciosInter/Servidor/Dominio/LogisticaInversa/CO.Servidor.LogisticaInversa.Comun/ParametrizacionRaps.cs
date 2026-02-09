using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System.Reflection;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Raps.Comun.Integraciones;
using System.Linq;
namespace CO.Servidor.LogisticaInversa.Comun
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

        /// <summary>
        /// Metodo para crear parametros 
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="motivoRaps"></param>
        /// <param name="parametrosParametrizacion"></param>
        /// <returns></returns>
        public bool CreaParametrosRaps(OUGuiaIngresadaDC guia, out EnumTipoNovedadRaps motivoRaps, out Dictionary<string, object> parametrosParametrizacion)
        {

            parametrosParametrizacion = new Dictionary<string, object>();
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            string[] datosMensajero = guia.NombreCompleto == null ? new string[1] : guia.NombreCompleto.Split('-');
            switch ((EnumMotivoGuiaRaps)guia.Motivo.IdMotivoGuia)
            {
                case EnumMotivoGuiaRaps.AVERIADO:
                    lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion("Averiado");
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NumeroGuia").FirstOrDefault().IdParametro.ToString(), guia.NumeroGuia);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "FechaDescarga").FirstOrDefault().IdParametro.ToString(), guia.FechaMotivoDevolucion);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NombreCompleto").FirstOrDefault().IdParametro.ToString(), datosMensajero[1]);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdentificacionMensajero").FirstOrDefault().IdParametro.ToString(), datosMensajero[0].Trim());
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdCol").FirstOrDefault().IdParametro.ToString(), guia.IdCentroLogistico);
                    motivoRaps = EnumTipoNovedadRaps.NoAlcanzo;
                    break;
                case EnumMotivoGuiaRaps.HURTO:
                    lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion("Hurto");
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NumeroGuia").FirstOrDefault().IdParametro.ToString(), guia.NumeroGuia);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "FechaDescarga").FirstOrDefault().IdParametro.ToString(), guia.FechaMotivoDevolucion);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NombreCompleto").FirstOrDefault().IdParametro.ToString(), datosMensajero[1]);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdentificacionMensajero").FirstOrDefault().IdParametro.ToString(), datosMensajero[0].Trim());
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdCol").FirstOrDefault().IdParametro.ToString(), guia.IdCentroLogistico);
                    motivoRaps = EnumTipoNovedadRaps.NoAlcanzo;
                    break;
                case EnumMotivoGuiaRaps.CONTENIDO_INCOMPLETO_HURTO:
                    lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion("Saqueo");
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NumeroGuia").FirstOrDefault().IdParametro.ToString(), guia.NumeroGuia);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "FechaDescarga").FirstOrDefault().IdParametro.ToString(), guia.FechaMotivoDevolucion);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NombreCompleto").FirstOrDefault().IdParametro.ToString(), datosMensajero[1]);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdentificacionMensajero").FirstOrDefault().IdParametro.ToString(), datosMensajero[0].Trim());
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdCol").FirstOrDefault().IdParametro.ToString(), guia.IdCentroLogistico);
                    motivoRaps = EnumTipoNovedadRaps.NoAlcanzo;
                    break;
                case EnumMotivoGuiaRaps.RESIDENTE_AUSENTE_NO_ALCANZO_EL_MENSAJERO:
                    lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion("NoAlcanzo");
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NumeroGuia").FirstOrDefault().IdParametro.ToString(), guia.NumeroGuia);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "FechaDescarga").FirstOrDefault().IdParametro.ToString(), guia.FechaMotivoDevolucion);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NombreCompleto").FirstOrDefault().IdParametro.ToString(), datosMensajero[1]);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdentificacionMensajero").FirstOrDefault().IdParametro.ToString(), datosMensajero[0].Trim());
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdCol").FirstOrDefault().IdParametro.ToString(), guia.IdCentroLogistico);
                    motivoRaps = EnumTipoNovedadRaps.NoAlcanzo;
                    break;
                case EnumMotivoGuiaRaps.FUERA_DE_ZONA:
                    lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion("FueraDeZona");
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NumeroGuia").FirstOrDefault().IdParametro.ToString(), guia.NumeroGuia);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "FechaDescarga").FirstOrDefault().IdParametro.ToString(), guia.FechaMotivoDevolucion);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NombreCompleto").FirstOrDefault().IdParametro.ToString(), datosMensajero[1]);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdentificacionMensajero").FirstOrDefault().IdParametro.ToString(), datosMensajero[0].Trim());
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdCol").FirstOrDefault().IdParametro.ToString(), guia.IdCentroLogistico);
                    motivoRaps = EnumTipoNovedadRaps.FueraDeZona;
                    break;
                default:
                    motivoRaps = EnumTipoNovedadRaps.Pordefecto;
                    return false;
            }

            return true;
        }



        /// <summary>
        /// Metodo para obtener parametros por integracion 
        /// </summary>
        /// <param name="tipoParametro"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracion(string tipoParametro)
        {
            return RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion(tipoParametro);
        }
    }
}
