using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.ModelosRequest.Bodega;
using CO.Servidor.Servicios.WebApi.ModelosRequest.CentroServicio;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiCentrosServicio : ApiDominioBase
    {

        private static readonly ApiCentrosServicio instancia = (ApiCentrosServicio)FabricaInterceptorApi.GetProxy(new ApiCentrosServicio(), COConstantesModulos.CENTRO_SERVICIOS);

        public static ApiCentrosServicio Instancia
        {
            get { return ApiCentrosServicio.instancia; }
        }

        private ApiCentrosServicio()
        {
        }

        /// <summary>
        /// Obtiene los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentroServicio()
        {
            List<PUCentroServiciosDC> retorno = FabricaServicios.ServicioCentroServicios.ObtenerCentrosServiciosActivos();
            //Test a = new Test();                
            return retorno;
        }


        /// <summary>
        /// Ingresa una guia a custodia
        /// </summary>
        /// <param name="custodia"></param>
        //public List<string> IngresarGuiaCustodia(PUCustodia custodia)
        //{

        //      return FabricaServicios.ServicioCentroServicios.IngresoCustodia(custodia);

        //}

        /// <summary>
        /// Consulta las guias de custodia
        /// </summary>
        /// <returns></returns>
        public List<PUCustodia> ConsultaGuiasCustodia(int idTipoMovimiento, Int16 idEstadoGuia, long? numeroGuia, bool muestraReportemuestraTodosreporte)
        {

            return FabricaServicios.ServicioCentroServicios.ObtenerGuiasCustodia(idTipoMovimiento, idEstadoGuia, numeroGuia, muestraReportemuestraTodosreporte);

        }


        /// <summary>
        /// Obtiene los horarios de un centro de servicios para la app de recogidas
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<string> ObtenerHorariosCentroServicioAppRecogidas(long idCentroServicio)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerHorariosCentroServicioAppRecogidas(idCentroServicio);
        }


        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerInformacionGeneralCentrosServicio()
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerInformacionGeneralCentrosServicio();
        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerPosicionesCanalesVenta(DateTime fechaInicial, DateTime fechaFinal, string idMensajero, string idCentroServicio, int idEstado)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerPosicionesCanalesVenta(fechaInicial, fechaFinal, idMensajero, idCentroServicio, idEstado);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerServiciosCentroServicio(long idCentroServicio)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerServiciosCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerCentrosServicioPorServicio(int idServicio)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerCentrosServicioPorServicio(idServicio);
        }

        /// <summary>
        /// Obtener todos los coles activos
        /// </summary>
        /// <returns>Colección con los coles activos</returns>
        public List<PUCentroServiciosDC> ObtenerTodosColes()
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerTodosColes();
        }


        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasCol(long idCol)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerPuntosAgenciasCol(idCol);
        }

        /// <summary>
        /// Obtiene los centros de servicio a los cuales tiene acceso el usuario
        /// </summary>
        /// <param name="identificacionUsuario"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerLocacionesAutorizadas(string usuario)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerLocacionesAutorizadas(usuario);
        }

        /// <summary>
        /// Obtiene el numero total de envios en custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        public int ObtenerConteoGuiasCustodia(int idTipoMovimiento, int idEstadoGuia)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerConteoGuiasCustodia(idTipoMovimiento, idEstadoGuia);
        }

        /// <summary>
        /// Obtiene el numero total de envios en pendientyes por ingr a custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        public int ObtenerConteoPendIngrCustodia(int idTipoMovimiento, int idEstadoGuia)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerConteoPendIngrCustodia(idTipoMovimiento, idEstadoGuia);
        }

        /// <summary>
        /// Obtiene los municipios segun el id del col
        /// </summary>
        /// <param name="IdCol"></param>
        /// <returns></returns>
        public List<PALocalidadDC> ObtenerMunicipiosXCol(long IdCol)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerMunicipiosXCol(IdCol);
        }

        /// <summary>
        /// Obtiene las agencias y puntos por Racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosPorRacol(long idRacol)
        {
            return FabricaServicios.ServicioCajas.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Obtiene los horarios de recogida del centro de servicio
        /// </summary>
        /// <param name="idCentroSvc"></param>
        /// <returns></returns>
        public IList<PUHorarioRecogidaCentroSvcDC> ObtenerHorariosRecogidasCentroSvc(long idCentroSvc)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerHorariosRecogidasCentroSvc(idCentroSvc);
        }

        /// <summary>
        /// obtiene todos los centros servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicio()
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerCentrosServicio();
        }

        public List<PUCentroServiciosDC> ObtenerCentrosServicioTipo()
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerCentrosServicioTipo();
        }
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServicioPorLocalidad(string idMunicipio)
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerTodosCentrosServicioPorLocalidad(idMunicipio);
        }

        public List<PUTipoCiudad> ObtenerTiposCiudades()
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerTiposCiudades();
        }

        public List<PUTipoZona> ObtenerTiposZona()
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerTiposZona();
        }

        /// <summary>
        /// obtiene centros servicios Agencias por idregional
        /// </summary>
        /// <param name="idRegional"></param>
        /// <returns></returns>
        //public PUAgencia ObtenerCentrosServiciosAge(int idRegional)
        //{
        //    return FabricaServicios.ServicioCentroServicios.ObtenerCentrosServiciosAge(idRegional);
        //}


        #region InterLogis

        /// <summary>
        /// Metodo para obtener los centro servicios activos inter logis
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioResponse> ObtenerCentrosServiciosActivosInterLogis()
        {
            List<PUCentroServicioResponse> misCentroServicios = null;
            List<PUCentroServiciosDC> retorno = FabricaServicios.ServicioCentroServicios.ObtenerCentrosServiciosActivos();
            if (retorno != null)
            {
                misCentroServicios = new List<PUCentroServicioResponse>();
                foreach (var item in retorno)
                {
                    misCentroServicios.Add(new PUCentroServicioResponse()
                    {
                        IdCentroServicio = item.IdCentroServicio,
                        Nombre = item.Nombre,
                        Tipo = item.Tipo
                    });
                }
            }

            return misCentroServicios;
        }

        #endregion
    }
}
