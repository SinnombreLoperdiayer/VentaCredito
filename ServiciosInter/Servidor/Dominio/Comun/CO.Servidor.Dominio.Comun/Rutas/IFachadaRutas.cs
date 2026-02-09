using System;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Dominio.Comun.Rutas
{
  /// <summary>
  /// Interfaz para acceso a la fachada de rutas
  /// </summary>
  public interface IFachadaRutas
  {
    /// <summary>
    /// Obtener una ruta dado un origen y un destino
    /// </summary>
    /// <param name="idLocalidadOrigen">Identificación de la localidad origen</param>
    /// <param name="idLocalidadDestino">Identificación de la localidad origen</param>
    /// <returns>Información de la ruta</returns>
    RURutaDC ObtenerRuta(string idLocalidadOrigen, string idLocalidadDestino);

    /// <summary>
    /// Calcula la ruta más corta y la ruta más larga para entregar un envío
    /// </summary>
    /// <param name="idLocalidadOrigen">Ciudad Origen del envío</param>
    /// <param name="idLocalidadDestino">Ciudad destino de envío</param>
    /// <returns></returns>
    RURutaOptimaCalculada CalcularRutaOptima(string idLocalidadOrigen, string idLocalidadDestino, DateTime? fechadmisionEnvio = null, int cantRutasABuscar = 1);

    /// <summary>
    /// Calcula la ruta más corta para entregar un envío pero sin tener un listado de ciudades especificas
    /// </summary>
    /// <param name="idLocalidadOrigen">Ciudad Origen del envío</param>
    /// <param name="idLocalidadDestino">Ciudad destino de envío</param>
    /// <returns></returns>
    RURutaOptimaCalculada CalcularRutaOptimaOmitiendoCiudades(string idLocalidadOrigen, string idLocalidadDestino, List<string> idLocalidadesAOmitir, DateTime? fechadmisionEnvio = null);

    /// <summary>
    /// Obtiene todas las rutas filtradas a partir de una localidad de origen
    /// </summary>
    /// <param name="IdLocalidad"></param>
    /// <returns></returns>
    IList<RURutaDC> ObtenerRutasXLocalidadOrigen(string idLocalidad);

    /// <summary>
    /// Obtiene las empresas transportadoras asociadas a una ruta
    /// </summary>
    /// <param name="idRuta">id de la ruta</param>
    /// <returns></returns>
    IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRuta(int idRuta);

      /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una racol
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
    IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRacol(int idRacol);        

    /// <summary>
    /// Obtiene las estaciones y localidades adicionales de una ruta
    /// </summary>
    /// <param name="idRuta"></param>
    /// <returns>Lista con las estaciones de la ruta</returns>
    IList<RUEstacionRuta> ObtenerEstacionesYLocalidadesAdicionalesRuta(int idRuta);

    /// <summary>
    /// Obtiene las estaciones-Ruta de un Manifiesto
    /// </summary>
    /// <param name="idManifiesto"></param>
    /// <returns>Lista con las estaciones-ruta de un Manifiesto</returns>
    IList<RUEstacionRuta> ObtenerEstacionesRutaDeManifiesto(long idManifiesto);

    /// <summary>
    /// Obtiene las rutas de una localidad
    /// </summary>
    /// <param name="idLocalidad"></param>
    List<RURutaDC> ObtenerRutasPorLocalidad(string idLocalidad);

    /// <summary>
    /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son consolidado o no
    /// </summary>
    /// <param name="idRuta"></param>
    /// <returns>Lista con las estaciones de la ruta</returns>
    IList<RUEstacionRuta> ObtenerEstacionesRuta(int idRuta);

    /// <summary>
    /// Obtiene la información de una ruta
    /// </summary>
    /// <param name="idRuta"></param>
    /// <returns></returns>
    RURutaDC ObtenerRutaIdRuta(int idRuta);

    /// <summary>
    /// Obtiene las rutas a las cuales pertenece una estación
    /// </summary>
    /// <param name="idLocalidadEstacion"></param>
    /// <returns></returns>
    List<RURutaDC> ObtenerRutasPerteneceEstacion(string idLocalidadEstacion);

    /// <summary>
    /// Obtiene las rutas a las cuales pertenece una estacion, incluye las rutas en las que la estacion es origen y destino
    /// </summary>
    /// <param name="idLocalidadEstacion"></param>
    /// <returns></returns>
    List<RURutaDC> ObtenerRutasPerteneceEstacionOrigDest(string idLocalidadEstacion);

    /// <summary>
    /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son consolidado , ordenadas por el campo orden de la ruta
    /// </summary>
    /// <param name="idRuta"></param>
    /// <returns>Lista con las estaciones de la ruta</returns>
    IList<RUEstacionRuta> ObtenerEstacionesCiudAdicionalesRuta(int idRuta);

    /// <summary>
    /// Obtiene las ciudades que oertenecen a una ruta, incluye estaciones de la ruta ciudades hijas de la estacion, ciudades adicionales ciudades hijas de las adicionales y la ciudad destino de la ruta
    /// </summary>
    /// <param name="idRuta"></param>
    /// <returns>Lista con las ciudades de una ruta</returns>
    IList<PALocalidadDC> ObtenerTodasCiudadesEnRuta(int idRuta);
  }
}