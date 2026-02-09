using System;
using System.Collections.Generic;
using CO.Servidor.Rutas.Datos;
using CO.Servidor.Rutas.Nacional;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Rutas;

namespace CO.Servidor.Rutas
{
    /// <summary>
    /// Administrador para la administración de rutas
    /// </summary>
    public class RUAdministradorRutas
    {
        private static readonly RUAdministradorRutas instancia = new RUAdministradorRutas();
      
        public static RUAdministradorRutas Instancia
        {
            get { return RUAdministradorRutas.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private RUAdministradorRutas() { }


        /// <summary>
        /// Obtiene  las rutas
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista  con las rutas</returns>
        public IList<RURutaDC> ObtenerRutas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return RURutaNacional.Instancia.ObtenerRutas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out  totalRegistros);
        }

        /// <summary>
        /// Obtiene una lista con todos los tipos de ruta, para llenar un comboBox
        /// </summary>
        /// <returns>Lista con los tipos de ruta</returns>
        public IList<RUTipoRuta> ObtenerTodosTipoRuta()
        {
            return RURutaNacional.Instancia.ObtenerTodosTipoRuta();
        }

        /// <summary>
        /// Obtiene las empresas trasportadoras filtradas por el tipo de transporte
        /// </summary>
        /// <param name="idTipoTransporte"></param>
        /// <returns>Lista de empresas transportadoras filtradas por el tipo de transporte</returns>
        public IList<RUEmpresaTransportadora> ObtieneEmpresaTransportadoraTipoTransporte(int idTipoTransporte)
        {
            return RURutaNacional.Instancia.ObtieneEmpresaTransportadoraTipoTransporte(idTipoTransporte);
        }

        /// <summary>
        /// Obtiene todas las empresas transportadoras dependiendo de un medio de transporte
        /// </summary>
        /// <param name="idMedioTransporte">Identificador del medio de transporte</param>
        /// <returns>Lista de empresas transportadoras</returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresaTransportadoraXMedioTransporte(int idMedioTransporte, int tipoTransporte)
        {
            return RURutaNacional.Instancia.ObtenerEmpresaTransportadoraXMedioTransporte(idMedioTransporte, tipoTransporte);
        }

        /// <summary>
        /// Obtiene lista de tipos de transporte
        /// </summary>
        /// <returns>lista con los tipos de transporte</returns>
        public IList<RUTipoTransporte> ObtieneTodosTipoTransporte()
        {
            return RURutaNacional.Instancia.ObtieneTodosTipoTransporte();
        }

        /// <summary>
        /// Obtiene  las ciudades estacion de una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idRuta">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con las rutas</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idRuta)
        {
            return RURutaNacional.Instancia.ObtenerEstacionesRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idRuta);
        }

        /// <summary>
        /// Obtiene  las ciudades hijas (cobertura) de una estacion de una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idCiudadEstacion">Id de ciudad estacion para la cual se retornara la cobertura</param>
        /// <returns>Lista  con las ciudades hijas de la ciudad estacion (cobertura de la ciudad estacion)</returns>
        public IList<RUCoberturaEstacion> ObtenerCiudadesHijasEstacionesRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idCiudadEstacion)
        {
            return RURutaNacional.Instancia.ObtenerCiudadesHijasEstacionesRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCiudadEstacion);
        }

        /// <summary>
        /// Obtiene  las ciudades que se manifiestan por una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idRuta">Id de la ruta por la cual se manifiestan las ciudades</param>
        /// <returns>Lista  con las ciudades que se manifiestan por una ruta</returns>
        public IList<RUCiudadManifestadaEnRuta> ObtenerCiudadesManifiestanEnRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idRuta)
        {
            return RURutaNacional.Instancia.ObtenerCiudadesManifiestanEnRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idRuta);
        }

        /// <summary>
        /// Actualiza la informacion de una ciudad que se manifiesta en una ruta
        /// </summary>
        /// <param name="ciudad">Objeto con la informacion de la ciudad que se manifiesta en la ruta</param>
        public void ActualizarCiudadQueManifiestaenRuta(RUCiudadManifestadaEnRuta ciudad)
        {
            RURutaNacional.Instancia.ActualizarCiudadQueManifiestaenRuta(ciudad);
        }

        /// <summary>
        /// Obtiene  la cobertura de una ciudad que se manifiestan por una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idCiudadManifiestaRuta">Id de la localidadManifiestaenRuta</param>
        /// <returns>cobertura de una ciudad que se manifiesta en una ruta</returns>
        public IList<RUCoberturaCiudadManifiestaPorRuta> ObtenerCoberturaCiudadManifiestaEnRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idCiudadManifiestaRuta)
        {
            return RURutaNacional.Instancia.ObtenerCoberturaCiudadManifiestaEnRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCiudadManifiestaRuta);
        }

        /// <summary>
        /// Inserta o borra la cobertura de la ciudades que manifiesta por ruta
        /// </summary>
        /// <param name="ciudad"></param>
        public void ActualizarCoberturaCiudadManifiestaEnRuta(List<RUCoberturaCiudadManifiestaPorRuta> ciudades)
        {
            RURutaNacional.Instancia.ActualizarCoberturaCiudadManifiestaEnRuta(ciudades);
        }

        /// <summary>
        /// Adiciona edita o elimina una estacion ruta
        /// </summary>
        /// <param name="estacionRuta"></param>
        public void ActualizarEstacionRuta(RUEstacionRuta estacionRuta)
        {
            RURutaNacional.Instancia.ActualizarEstacionRuta(estacionRuta);
        }

        /// <summary>
        /// Adiciona o edita la informacion de una ruta
        /// </summary>
        /// <param name="ruta"></param>
        public int ActualizarRuta(RURutaDC ruta)
        {
            return RURutaNacional.Instancia.ActualizarRuta(ruta);
        }

        /// <summary>
        /// Calcula la ruta más corta y la ruta más larga para entregar un envío
        /// </summary>
        /// <param name="idLocalidadOrigen">Ciudad Origen del envío</param>
        /// <param name="idLocalidadDestino">Ciudad destino de envío</param>
        /// <returns></returns>
        public RURutaOptimaCalculada CalcularRutaOptima(string idLocalidadOrigen, string idLocalidadDestino, DateTime? fechadmisionEnvio = null, int cantRutasABuscar = 1)
        {
            return GrafoRutas.Instancia.CalcularRutaOptima(idLocalidadOrigen, idLocalidadDestino, fechadmisionEnvio, cantRutasABuscar);
        }

        public RURutaOptimaCalculada CalcularRutaOptimaOmitiendoCiudades(string idLocalidadOrigen, string idLocalidadDestino, List<string> idLocalidadesAOmitir, DateTime? fechadmisionEnvio = null)
        {
            return GrafoRutas.Instancia.CalcularRutaOptimaOmitiendoCiudades(idLocalidadOrigen, idLocalidadDestino, idLocalidadesAOmitir, fechadmisionEnvio);
        }

        /// <summary>
        /// Obtiene todas las rutas filtradas a partir de una localidad de orgen
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public IList<RURutaDC> ObtenerRutasXLocalidadOrigen(string idLocalidad)
        {
            return RURutaNacional.Instancia.ObtenerRutasXLocalidadOrigen(idLocalidad);
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una ruta
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRuta(int idRuta)
        {
            return RURutaNacional.Instancia.ObtenerEmpresasTransportadorasXRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una racol
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRacol(int idRacol)
        {
            return RURutaNacional.Instancia.ObtenerEmpresasTransportadorasXRacol(idRacol);
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesYLocalidadesAdicionalesRuta(int idRuta)
        {
            return RURutaNacional.Instancia.ObtenerEstacionesYLocalidadesAdicionalesRuta(idRuta);
        }


        /// <summary>
        /// Obtiene las estaciones-Ruta de un Manifiesto
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRutaDeManifiesto(long idManifiesto)
        {
            return RURutaNacional.Instancia.ObtenerEstacionesRutaDeManifiesto(idManifiesto);
        }


        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son conolidado o no
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRuta(int idRuta)
        {
            return RURutaNacional.Instancia.ObtenerEstacionesRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las rutas filtradas por la ciudad de origen o destino
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idLocalidadAgencia"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasXCiudadOrigenDestino(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, string idLocalidadAgencia)
        {
            return RURutaNacional.Instancia.ObtenerRutasXCiudadOrigenDestino(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idLocalidadAgencia);
        }

        /// <summary>
        /// Obtiene las rutas de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        public List<RURutaDC> ObtenerRutasPorLocalidad(string idLocalidad)
        {
            return RURutaNacional.Instancia.ObtenerRutasPorLocalidad(idLocalidad);
        }

        /// <summary>
        /// Obtiene la informacion de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns></returns>
        public RURutaDC ObtenerRutaIdRuta(int idRuta)
        {
            return RURepositorio.Instancia.ObtenerRutaIdRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estación
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacion(string idLocalidadEstacion)
        {
            return RURutaNacional.Instancia.ObtenerRutasPerteneceEstacion(idLocalidadEstacion);
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estacion, incluye las rutas en las que la estacion es origen y destino
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacionOrigDest(string idLocalidadEstacion)
        {
            return RURutaNacional.Instancia.ObtenerRutasPerteneceEstacionOrigDest(idLocalidadEstacion);
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son consolidado , ordenadas por el campo orden de la ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesCiudAdicionalesRuta(int idRuta)
        {
            return RURutaNacional.Instancia.ObtenerEstacionesCiudAdicionalesRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las ciudades que oertenecen a una ruta, incluye estaciones de la ruta ciudades hijas de la estacion, ciudades adicionales ciudades hijas de las adicionales y la ciudad destino de la ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las ciudades de una ruta</returns>
        public IList<PALocalidadDC> ObtenerTodasCiudadesEnRuta(int idRuta)
        {
            return RURutaNacional.Instancia.ObtenerTodasCiudadesEnRuta(idRuta);
        }

        /// <summary>
        /// Obtener una ruta
        /// </summary>
        /// <param name="idLocalidadOrigen">Ciudad origen</param>
        /// <param name="idLocalidadDestino">Ciudad Destino</param>
        /// <returns>La ruta encontrada</returns>
        public RURutaDC ObtenerRuta(string idLocalidadOrigen, string idLocalidadDestino)
        {
            return RURutaNacional.Instancia.ObtenerRuta(idLocalidadOrigen, idLocalidadDestino);
        }

        #region Empresa Transportadora

        /// <summary>
        /// Metodo que consulta las empresas
        /// transportadoras
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns>lista de empresas transportadoras</returns>
        public List<RUEmpresaTransportadora> ObtenerEmpresaTransportadora(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return RURutaNacional.Instancia.ObtenerEmpresaTransportadora(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Metodo para Adicionar - Editar - Eliminar Empresas Transportadoras
        /// </summary>
        public void GestionarEmpresaTransportadora(RUEmpresaTransportadora empresaTransportadora)
        {
            RURutaNacional.Instancia.GestionarEmpresaTransportadora(empresaTransportadora);
        }

        /// <summary>
        /// Obtiene la Info Inicial para la pantalla de Empresa Trasnportadora
        /// </summary>
        /// <returns>Listas de Inicializacion</returns>
        public RUEmpresaTransporteInfoInicialDC ObtenerInfoInicialEmpresaTransportadora()
        {
            return RURutaNacional.Instancia.ObtenerInfoInicialEmpresaTransportadora();
        }

        #endregion Empresa Transportadora




        #region rutas
        /// <summary>
        /// Obtiene  información de la ruta y Coordenadas de centros de servicio de la ruta
        /// </summary>
        /// <param name="IdRuta">
        /// <returns></returns>
        //public RURutaICWebDetalle obtenerRutaDetalleCentroServiciosRuta(int IdRuta)
        //{
        //    return RURutaCWeb.Instancia.obtenerRutaDetalleCentroServiciosRuta(IdRuta);
        //}
        /// <summary>
        /// Obtiene rutas
        /// </summary>
        /// <returns></returns>
        public List<RURutaICWeb> ObtenerRuta()
        {
            return RURutaCWeb.Instancia.ObtenerRutaDetalle();
        }
        /// <summary>
        /// obtiene centros de servicios de la ruta indicada
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RURutaCWebDetalleCentrosServicios> ObtenerRutaDetalleCentroServiciosRuta(int IdRuta, int id)
        {
            return RURutaCWeb.Instancia.ObtenerRutaDetalleCentroServiciosRuta(IdRuta, id);
        }

        /// <summary>
        /// agrega un punto a la ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        public void AgregarPtoRuta(PtoRuta datosPunto)
        {
            RURutaCWeb.Instancia.AgregarPtoRuta(datosPunto);
        }
        //public void AgregarRuta(string origen, string destino, string nombre, int tipoRuta, int medioTransporte, int generaManifiesto)
        //{
        //    RURutaCWeb.Instancia.AgregarRuta(origen, destino, nombre, tipoRuta, medioTransporte, generaManifiesto);
        //}
        /// <summary>
        /// elimina punto de ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        public void EliminarPtoRuta(PtoRuta datosPunto)
        {
            RURutaCWeb.Instancia.EliminarPtoRuta(datosPunto);
        }
        /// <summary>
        /// crear punto
        /// </summary>
        /// <param name="datosPunto"></param>
        public void CrearPunto(PtoRuta datosPunto)
        {
            RURutaCWeb.Instancia.CrearPunto(datosPunto);
        }
        /// <summary>
        /// asigna posicion en ruta a punto indicado
        /// </summary>
        /// <param name="datosPunto"></param>
        public void OrganizarPtos(PtoRuta datosPunto)
        {
            RURutaCWeb.Instancia.OrganizarPtos(datosPunto);
        }
        /// <summary>
        /// obtiene todos los medios de transporte
        /// </summary>
        /// <returns></returns>
        public List<RUMedioTransporte> ObtenerMediosTransporte()
        {
            return RURutaCWeb.Instancia.ObtenerMediosTransporte();
        }
        /// <summary>
        /// obtiene todos lod tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        public List<RUTipoVehiculo> ObtenerTiposVehiculos()
        {
            return RURutaCWeb.Instancia.ObtenerTiposVehiculos();
        }
        /// <summary>
        /// obtiene todos los tipos de ruta
        /// </summary>
        /// <returns></returns>
        public List<RUTipoRuta> ObtenerTiposRuta()
        {
            return RURutaCWeb.Instancia.ObtenerTiposRuta();
        }
        /// <summary>
        /// crea nueva ruta
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public int CrearRuta(RURutaICWeb ruta)
        {
            return RURutaCWeb.Instancia.CrearRuta(ruta);
        }

        #endregion
    }
}