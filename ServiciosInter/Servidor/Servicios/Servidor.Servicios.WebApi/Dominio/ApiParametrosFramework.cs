using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiParametrosFramework : ApiDominioBase
    {

        private static readonly ApiParametrosFramework instancia = (ApiParametrosFramework)FabricaInterceptorApi.GetProxy(new ApiParametrosFramework(), COConstantesModulos.PARAMETROS_GENERALES);

        public static ApiParametrosFramework Instancia
        {
            get { return ApiParametrosFramework.instancia; }
        }

        private ApiParametrosFramework()
        {

        }


        /// <summary>
        /// Retorna las localidades que no son paises ni departamentos para Colombia
        /// </summary>
        /// <returns></returns>
        public List<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamentoColombia()
        {

            return FabricaServicios.ServicioParametros.ObtenerLocalidadesNoPaisNoDepartamentoColombia().ToList();
        }


        /// <summary>
        /// Consulta la fecha del servidor
        /// </summary>
        /// <returns></returns>      
        public string ConsultarFechaServidor()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        /// <summary>
        /// Consulta la fecha del servidor
        /// </summary>
        /// <returns></returns>      
        public List<PATipoIdentificacion> ConsultarTiposIdentificacion()
        {
            return FabricaServicios.ServicioParametros.ConsultarTiposIdentificacion().ToList();
        }

        /// <summary>
        /// Obtiene el valor de un parametro
        /// </summary>
        /// <param name="llave"></param>
        /// <returns></returns>
        public string ConsultarParametrosFramework(string llave)
        {
            return FabricaServicios.ServicioParametros.ConsultarParametrosFramework(llave).ValorParametro;
        }


        /// <summary>
        /// Obtiene los festivos para las recogidas
        /// </summary>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivos(DateTime fechaDesde, DateTime fechaHasta)
        {
            return FabricaServicios.ServicioParametros.ObtenerFestivos(fechaDesde, fechaHasta, "057");
        }

        /// <summary>
        /// Obtiene informacion de la entidad 
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public PAPropiedadesAplicacionDC ConsultarPropiedadesAplicacion(string codigo, int rol)
        {
            return FabricaServicios.ServicioParametros.ConsultarPropiedadesAplicacion(codigo, rol);
        }

        /// <summary>
        /// Obtiene los sectores a los que pertenece un cliente credito
        /// </summary>
        /// <returns></returns>
        public IList<PATipoSectorCliente> ObtenerTodosTipoSectorCliente()
        {
            return FabricaServicios.ServicioParametros.ObtenerTodosTipoSectorCliente();
        }

        /// <summary>
        /// Obtiene los festivos entre dos fechas pero no los agrega a la cache, con el fin de consultar fechas por meses
        /// </summary>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivosSinCache(DateTime fechaDesde, DateTime fechaHasta)
        {
            return FabricaServicios.ServicioParametros.ObtenerFestivosSinCache(fechaDesde, fechaHasta, "057");
        }

    }
}
