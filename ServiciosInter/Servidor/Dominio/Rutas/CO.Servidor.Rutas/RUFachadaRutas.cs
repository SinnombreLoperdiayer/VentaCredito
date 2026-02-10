using System;
using System.Collections.Generic;
using CO.Servidor.Dominio.Comun.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Rutas
{
    /// <summary>
    /// Fachada para la lógica de rutas para interfaz con otros módulos de la aplicación
    /// </summary>
    public class RUFachadaRutas : IFachadaRutas
    {
        internal RURutaNacional RURutaNacional
        {
            get;
            set;
        }

        /// <summary>
        /// Obtener una ruta
        /// </summary>
        /// <param name="idLocalidadOrigen">Ciudad origen</param>
        /// <param name="idLocalidadDestino">Ciudad Destino</param>
        /// <returns>La ruta encontrada</returns>
        public RURutaDC ObtenerRuta(string idLocalidadOrigen, string idLocalidadDestino)
        {
            //return new RURutaDC()
            //{
            //    NombreRuta = "Occidente",
            //    IdRuta = 7611
            //};

            return RUAdministradorRutas.Instancia.ObtenerRuta(idLocalidadOrigen, idLocalidadDestino);
        }

        /// <summary>
        /// Calcula la ruta más corta y la ruta más larga para entregar un envío
        /// </summary>
        /// <param name="idLocalidadOrigen">Ciudad Origen del envío</param>
        /// <param name="idLocalidadDestino">Ciudad destino de envío</param>
        /// <returns></returns>
        public RURutaOptimaCalculada CalcularRutaOptima(string idLocalidadOrigen, string idLocalidadDestino, DateTime? fechadmisionEnvio = null, int cantRutasABuscar = 1)
        {
            return RUAdministradorRutas.Instancia.CalcularRutaOptima(idLocalidadOrigen, idLocalidadDestino, fechadmisionEnvio, cantRutasABuscar);
        }

        /// <summary>
        /// Calcula la ruta más corta para entregar un envío pero sin tener un listado de ciudades especificas
        /// </summary>
        /// <param name="idLocalidadOrigen">Ciudad Origen del envío</param>
        /// <param name="idLocalidadDestino">Ciudad destino de envío</param>
        /// <returns></returns>
        public RURutaOptimaCalculada CalcularRutaOptimaOmitiendoCiudades(string idLocalidadOrigen, string idLocalidadDestino, List<string> idLocalidadesAOmitir, DateTime? fechadmisionEnvio = null)
        {
            return RUAdministradorRutas.Instancia.CalcularRutaOptimaOmitiendoCiudades(idLocalidadOrigen, idLocalidadDestino, idLocalidadesAOmitir, fechadmisionEnvio);
        }

        /// <summary>
        /// Obtiene todas las rutas filtradas a partir de una localidad de orgen
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public IList<RURutaDC> ObtenerRutasXLocalidadOrigen(string idLocalidad)
        {
            return RUAdministradorRutas.Instancia.ObtenerRutasXLocalidadOrigen(idLocalidad);
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una ruta
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRuta(int idRuta)
        {
            return RUAdministradorRutas.Instancia.ObtenerEmpresasTransportadorasXRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una racol
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRacol(int idRacol)
        {
             return RUAdministradorRutas.Instancia.ObtenerEmpresasTransportadorasXRacol(idRacol);
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesYLocalidadesAdicionalesRuta(int idRuta)
        {
            return RUAdministradorRutas.Instancia.ObtenerEstacionesYLocalidadesAdicionalesRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las estaciones-ruta de un Manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRutaDeManifiesto(long idManifiesto)
        {
            return RUAdministradorRutas.Instancia.ObtenerEstacionesRutaDeManifiesto(idManifiesto);
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son conolidado o no
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRuta(int idRuta)
        {
            return RUAdministradorRutas.Instancia.ObtenerEstacionesRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las rutas de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        public List<RURutaDC> ObtenerRutasPorLocalidad(string idLocalidad)
        {
            return RUAdministradorRutas.Instancia.ObtenerRutasPorLocalidad(idLocalidad);
        }

        /// <summary>
        /// Obtiene la informacion de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns></returns>
        public RURutaDC ObtenerRutaIdRuta(int idRuta)
        {
            return RUAdministradorRutas.Instancia.ObtenerRutaIdRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estación
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacion(string idLocalidadEstacion)
        {
            return RUAdministradorRutas.Instancia.ObtenerRutasPerteneceEstacion(idLocalidadEstacion);
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estacion, incluye las rutas en las que la estacion es origen y destino
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacionOrigDest(string idLocalidadEstacion)
        {
            return RUAdministradorRutas.Instancia.ObtenerRutasPerteneceEstacionOrigDest(idLocalidadEstacion);
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son consolidado , ordenadas por el campo orden de la ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesCiudAdicionalesRuta(int idRuta)
        {
            return RUAdministradorRutas.Instancia.ObtenerEstacionesCiudAdicionalesRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las ciudades que oertenecen a una ruta, incluye estaciones de la ruta ciudades hijas de la estacion, ciudades adicionales ciudades hijas de las adicionales y la ciudad destino de la ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las ciudades de una ruta</returns>
        public IList<PALocalidadDC> ObtenerTodasCiudadesEnRuta(int idRuta)
        {
            return RUAdministradorRutas.Instancia.ObtenerTodasCiudadesEnRuta(idRuta);
        }
    }
}