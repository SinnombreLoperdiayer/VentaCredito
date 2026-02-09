using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using CO.Servidor.Rutas;
using CO.Servidor.Rutas.Nacional;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.Implementacion.Rutas
{
    /// <summary>
    /// Clase para los servicios de administración de Clientes
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RURutasSvc : IRURutasSvc
    {
        public RURutasSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        public GenericoConsultasFramework<ContratoDatos.Rutas.RURutaDC> ObtenerRutas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<RURutaDC>()
            {
                Lista = RUAdministradorRutas.Instancia.ObtenerRutas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }


        /// <summary>
        /// Obtiene una lista con todos los tipos de ruta, para llenar un comboBox
        /// </summary>
        /// <returns>Lista con los tipos de ruta</returns>
        public IList<RUTipoRuta> ObtenerTodosTipoRuta()
        {
            return RUAdministradorRutas.Instancia.ObtenerTodosTipoRuta();
        }

        /// <summary>
        /// Obtiene las empresas trasportadoras filtradas por el tipo de transporte
        /// </summary>
        /// <param name="idTipoTransporte"></param>
        /// <returns>Lista de empresas transportadoras filtradas por el tipo de transporte</returns>
        public IList<RUEmpresaTransportadora> ObtieneEmpresaTransportadoraTipoTransporte(int idTipoTransporte)
        {
            return RUAdministradorRutas.Instancia.ObtieneEmpresaTransportadoraTipoTransporte(idTipoTransporte);
        }

        /// <summary>
        /// Obtiene lista de tipos de transporte
        /// </summary>
        /// <returns>lista con los tipos de transporte</returns>
        public IList<RUTipoTransporte> ObtieneTodosTipoTransporte()
        {
            return RUAdministradorRutas.Instancia.ObtieneTodosTipoTransporte();
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
        public GenericoConsultasFramework<RUEstacionRuta> ObtenerEstacionesRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idRuta)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<RUEstacionRuta>()
            {
                Lista = RUAdministradorRutas.Instancia.ObtenerEstacionesRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idRuta),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene  las ciudades hijas (cobertura) de una estacion de una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="idCiudadEstacion">Id de ciudad estacion para la cual se retornara la cobertura</param>
        /// <returns>Lista  con las ciudades hijas de la ciudad estacion (cobertura de la ciudad estacion)</returns>
        public GenericoConsultasFramework<RUCoberturaEstacion> ObtenerCiudadesHijasEstacionesRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCiudadEstacion)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<RUCoberturaEstacion>()
            {
                Lista = RUAdministradorRutas.Instancia.ObtenerCiudadesHijasEstacionesRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCiudadEstacion),
                TotalRegistros = totalRegistros
            };
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
        public GenericoConsultasFramework<RUCiudadManifestadaEnRuta> ObtenerCiudadesManifiestanEnRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idRuta)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<RUCiudadManifestadaEnRuta>()
            {
                Lista = RUAdministradorRutas.Instancia.ObtenerCiudadesManifiestanEnRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idRuta),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Actualiza la informacion de una ciudad que se manifiesta en una ruta
        /// </summary>
        /// <param name="ciudad">Objeto con la informacion de la ciudad que se manifiesta en la ruta</param>
        public void ActualizarCiudadQueManifiestaenRuta(RUCiudadManifestadaEnRuta ciudad)
        {
            RUAdministradorRutas.Instancia.ActualizarCiudadQueManifiestaenRuta(ciudad);
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
        public GenericoConsultasFramework<RUCoberturaCiudadManifiestaPorRuta> ObtenerCoberturaCiudadManifiestaEnRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCiudadManifiestaRuta)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<RUCoberturaCiudadManifiestaPorRuta>()
            {
                Lista = RUAdministradorRutas.Instancia.ObtenerCoberturaCiudadManifiestaEnRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCiudadManifiestaRuta),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Inserta o borra la cobertura de la ciudades que manifiesta por ruta
        /// </summary>
        /// <param name="ciudad"></param>
        public void ActualizarCoberturaCiudadManifiestaEnRuta(List<RUCoberturaCiudadManifiestaPorRuta> ciudades)
        {
            RUAdministradorRutas.Instancia.ActualizarCoberturaCiudadManifiestaEnRuta(ciudades);
        }

        /// <summary>
        /// Adiciona edita o elimina una estacion ruta
        /// </summary>
        /// <param name="estacionRuta"></param>
        public void ActualizarEstacionRuta(RUEstacionRuta estacionRuta)
        {
            RUAdministradorRutas.Instancia.ActualizarEstacionRuta(estacionRuta);
        }

        /// <summary>
        /// Adiciona o edita la informacion de una ruta
        /// </summary>
        /// <param name="ruta"></param>
        public int ActualizarRuta(RURutaDC ruta)
        {
            return RUAdministradorRutas.Instancia.ActualizarRuta(ruta);
        }

        /// <summary>
        /// Obtiene todas las empresas transportadoras dependiendo de un medio de transporte
        /// </summary>
        /// <param name="idMedioTransporte">Identificador del medio de transporte</param>
        /// <returns>Lista de empresas transportadoras</returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresaTransportadoraXMedioTransporte(int idMedioTransporte, int tipoTransporte)
        {
            return RUAdministradorRutas.Instancia.ObtenerEmpresaTransportadoraXMedioTransporte(idMedioTransporte, tipoTransporte);
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
        public GenericoConsultasFramework<RURutaDC> ObtenerRutasXCiudadOrigenDestino(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, string idLocalidadAgencia)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<RURutaDC>()
            {
                Lista = RUAdministradorRutas.Instancia.ObtenerRutasXCiudadOrigenDestino(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idLocalidadAgencia),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Calcula la ruta más optima y la ruta menos óptima desde un origen a un destino
        /// </summary>
        /// <param name="idLocalidadOrigen">Identificador de la localidad origen del envío</param>
        /// <param name="idLocalidadDestino">Identificador de la localidad destino del envío</param>
        /// <returns>ruta óptima calculada</returns>
        public RURutaOptimaCalculada CalcularRutaOptimaConFechaAdmision(string idLocalidadOrigen, string idLocalidadDestino, DateTime fechadmisionEnvio)
        {
            return GrafoRutas.Instancia.CalcularRutaOptima(idLocalidadOrigen, idLocalidadDestino, fechadmisionEnvio);
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
            return RUAdministradorRutas.Instancia.ObtenerEmpresaTransportadora(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Metodo para Adicionar - Editar - Eliminar Empresas Transportadoras
        /// </summary>
        public void GestionarEmpresaTransportadora(RUEmpresaTransportadora empresaTransportadora)
        {
            RUAdministradorRutas.Instancia.GestionarEmpresaTransportadora(empresaTransportadora);
        }

        /// <summary>
        /// Obtiene la Info Inicial para la pantalla de Empresa Trasnportadora
        /// </summary>
        /// <returns>Listas de Inicializacion</returns>
        public RUEmpresaTransporteInfoInicialDC ObtenerInfoInicialEmpresaTransportadora()
        {
            return RUAdministradorRutas.Instancia.ObtenerInfoInicialEmpresaTransportadora();
        }

        #endregion Empresa Transportadora


        #region rutas
        /// <summary>
        /// Obtiene una lista con todos los tipos de ruta, para llenar un comboBox
        /// </summary>
        /// <returns>Lista con los tipos de ruta</returns>
        //public RURutaICWebDetalle obtenerRutaDetalleCentroServiciosRuta(int idRuta)
        //{
        //    return RUAdministradorRutas.Instancia.obtenerRutaDetalleCentroServiciosRuta(idRuta);
        //}
        /// <summary>
        /// Obtiene rutas
        /// </summary>
        /// <returns></returns>
        public List<RURutaICWeb> ObtenerRuta()
        {
            return RUAdministradorRutas.Instancia.ObtenerRuta();
        }
        /// <summary>
        /// obtiene centros de servicios de la ruta indicada
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RURutaCWebDetalleCentrosServicios> ObtenerRutaDetalleCentroServiciosRuta(int idRuta, int id)
        {
            return RUAdministradorRutas.Instancia.ObtenerRutaDetalleCentroServiciosRuta(idRuta, id);
        }
        /// <summary>
        /// agrega un punto a la ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        public void AgregarPtoRuta(PtoRuta datosPunto)
        {
            RUAdministradorRutas.Instancia.AgregarPtoRuta(datosPunto);
        }
        /// <summary>
        /// crear nueva ruta
        /// </summary>
        /// <param name="origen"></param>
        /// <param name="destino"></param>
        /// <param name="nombre"></param>
        /// <param name="tipoRuta"></param>
        /// <param name="medioTransporte"></param>
        /// <param name="generaManifiesto"></param>
        /*public void AgregarRuta(string origen, string destino, string nombre, int tipoRuta, int medioTransporte, int generaManifiesto)
        {
            RUAdministradorRutas.Instancia.AgregarRuta(origen, destino, nombre, tipoRuta, medioTransporte, generaManifiesto);
        }**/
        /// <summary>
        /// elimina punto de ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        public void EliminarPtoRuta(PtoRuta datosPunto)
        {
            RUAdministradorRutas.Instancia.EliminarPtoRuta(datosPunto);
        }
        /// <summary>
        /// crear punto
        /// </summary>
        /// <param name="datosPunto"></param>
        public void CrearPunto(PtoRuta datosPunto)
        {
            RUAdministradorRutas.Instancia.CrearPunto(datosPunto);
        }
        /// <summary>
        /// asigna posicion en ruta a punto indicado
        /// </summary>
        /// <param name="datosPunto"></param>
        public void OrganizarPtos(PtoRuta datosPunto)
        {
            RUAdministradorRutas.Instancia.OrganizarPtos(datosPunto);
        }
        /// <summary>
        /// obtiene todos los medios de transporte
        /// </summary>
        /// <returns></returns>
        public List<RUMedioTransporte> ObtenerMediosTransporte()
        {
            return RUAdministradorRutas.Instancia.ObtenerMediosTransporte();
        }
        /// <summary>
        /// obtiene todos lod tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        public List<RUTipoVehiculo> ObtenerTiposVehiculos()
        {
            return RUAdministradorRutas.Instancia.ObtenerTiposVehiculos();
        }
        /// <summary>
        /// obtiene todos los tipos de ruta
        /// </summary>
        /// <returns></returns>
        public List<RUTipoRuta> ObtenerTiposRuta()
        {
            return RUAdministradorRutas.Instancia.ObtenerTiposRuta();
        }
        /// <summary>
        /// crea nueva ruta
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public int CrearRuta(RURutaICWeb ruta)
        {
            return RUAdministradorRutas.Instancia.CrearRuta(ruta);
        }
        #endregion





    }
}