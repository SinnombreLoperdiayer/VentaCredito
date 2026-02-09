using System;
using System.Collections.Generic;
using System.Linq;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Rutas.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.Rutas
{
    internal class RURutaNacional : ControllerBase
    {
        private static readonly RURutaNacional instancia = (RURutaNacional)FabricaInterceptores.GetProxy(new RURutaNacional(), COConstantesModulos.MODULO_RUTAS);

        public static RURutaNacional Instancia
        {
            get { return RURutaNacional.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private RURutaNacional() { }

        /// <summary>
        /// Obtener los nombres de las rutas para un origen y un destino
        /// </summary>
        /// <param name="idLocalidadOrigen">Identificador de la ciudad de origen</param>
        /// <param name="idLocalidadDestino">Identificador de la ciudad de destino</param>
        /// <returns>Cadena con los nombres de las rutas separados por comas</returns>
        public string ObtenerNombresRuta(string idLocalidadOrigen, string idLocalidadDestino)
        {
            throw new NotImplementedException();
        }

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
            return RURepositorio.Instancia.ObtenerRutas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out  totalRegistros);
        }

        /// <summary>
        /// Obtiene una lista con todos los tipos de ruta, para llenar un comboBox
        /// </summary>
        /// <returns>Lista con los tipos de ruta</returns>
        public IList<RUTipoRuta> ObtenerTodosTipoRuta()
        {
            return RURepositorio.Instancia.ObtenerTodosTipoRuta();
        }

        /// <summary>
        /// Obtiene las empresas trasportadoras filtradas por el tipo de transporte
        /// </summary>
        /// <param name="idTipoTransporte"></param>
        /// <returns>Lista de empresas transportadoras filtradas por el tipo de transporte</returns>
        public IList<RUEmpresaTransportadora> ObtieneEmpresaTransportadoraTipoTransporte(int idTipoTransporte)
        {
            return RURepositorio.Instancia.ObtieneEmpresaTransportadoraTipoTransporte(idTipoTransporte);
        }

        /// <summary>
        /// Obtiene todas las empresas transportadoras dependiendo de un medio de transporte y tipo de transporte
        /// </summary>
        /// <param name="idMedioTransporte">Identificador del medio de transporte</param>
        /// <returns>Lista de empresas transportadoras</returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresaTransportadoraXMedioTransporte(int idMedioTransporte, int tipoTransporte)
        {
            if (tipoTransporte <= 0)
                return RURepositorio.Instancia.ObtenerEmpresaTransportadoraXMedioTransporte(idMedioTransporte);
            else
                return RURepositorio.Instancia.ObtenerEmpresaTransportadoraXMedioTransporteXTipoTransporte(idMedioTransporte, tipoTransporte);
        }

        /// <summary>
        /// Obtiene lista de tipos de transporte
        /// </summary>
        /// <returns>lista con los tipos de transporte</returns>
        public IList<RUTipoTransporte> ObtieneTodosTipoTransporte()
        {
            return RURepositorio.Instancia.ObtieneTodosTipoTransporte();
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
            return RURepositorio.Instancia.ObtenerEstacionesRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idRuta);
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
            return RURepositorio.Instancia.ObtenerCiudadesHijasEstacionesRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCiudadEstacion);
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
            return RURepositorio.Instancia.ObtenerCiudadesManifiestanEnRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idRuta);
        }

        /// <summary>
        /// Actualiza la informacion de una ciudad que se manifiesta en una ruta
        /// </summary>
        /// <param name="ciudad">Objeto con la informacion de la ciudad que se manifiesta en la ruta</param>
        public void ActualizarCiudadQueManifiestaenRuta(RUCiudadManifestadaEnRuta ciudad)
        {
            switch (ciudad.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    RURepositorio.Instancia.AdicionarCiudadQueManifiestaenRuta(ciudad);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    RURepositorio.Instancia.EliminarCiudadQueManifiestaenRuta(ciudad);
                    break;
            }
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
            return RURepositorio.Instancia.ObtenerCoberturaCiudadManifiestaEnRuta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCiudadManifiestaRuta);
        }

        /// <summary>
        /// Inserta o borra la cobertura de la ciudades que manifiesta por ruta
        /// </summary>
        /// <param name="ciudad"></param>
        public void ActualizarCoberturaCiudadManifiestaEnRuta(List<RUCoberturaCiudadManifiestaPorRuta> ciudades)
        {
            ciudades.ForEach(obj =>
              {
                  switch (obj.EstadoRegistro)
                  {
                      case EnumEstadoRegistro.ADICIONADO:
                          RURepositorio.Instancia.AdicionarCoberturaCiudadManifiestaEnRuta(obj);
                          break;

                      case EnumEstadoRegistro.BORRADO:
                          RURepositorio.Instancia.EliminarCoberturaCiudadManifiestaEnRuta(obj);
                          break;
                  }
              });
        }

        /// <summary>
        /// Adiciona edita o elimina una estacion ruta
        /// </summary>
        /// <param name="estacionRuta"></param>
        public void ActualizarEstacionRuta(RUEstacionRuta estacionRuta)
        {
            switch (estacionRuta.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    RURepositorio.Instancia.AdicionarEstacionRuta(estacionRuta);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    RURepositorio.Instancia.EditarEstacionRuta(estacionRuta);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    RURepositorio.Instancia.EliminarEstacionRuta(estacionRuta);
                    break;
            }
        }

        /// <summary>
        /// Adiciona o edita la informacion de una ruta
        /// </summary>
        /// <param name="ruta"></param>
        public int ActualizarRuta(RURutaDC ruta)
        {
            switch (ruta.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    return RURepositorio.Instancia.AdicionarRuta(ruta);

                case EnumEstadoRegistro.MODIFICADO:
                    RURepositorio.Instancia.EditarRuta(ruta);
                    return ruta.IdRuta;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Obtiene todas las rutas filtradas a partir de una localidad de orgen
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public IList<RURutaDC> ObtenerRutasXLocalidadOrigen(string idLocalidad)
        {
            return RURepositorio.Instancia.ObtenerRutasXLocalidadOrigen(idLocalidad);
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una ruta
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRuta(int idRuta)
        {
            return RURepositorio.Instancia.ObtenerEmpresasTransportadorasXRuta(idRuta);
        }

         /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una racol
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRacol(int idRacol)
        {
            return RURepositorio.Instancia.ObtenerEmpresasTransportadorasXRacol(idRacol);
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesYLocalidadesAdicionalesRuta(int idRuta)
        {
            IList<RUEstacionRuta> lstRutas = RURepositorio.Instancia.ObtenerEstacionesYLocalidadesAdicionalesRuta(idRuta);
            lstRutas.ToList().ForEach(e =>
              {
                  PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(e.CiudadEstacion.IdLocalidad);

                  e.SubTipoAgencia = centroServ.TipoSubtipo;
                  e.TipoAgencia = centroServ.Tipo;
              });

            return lstRutas;
        }

        /// <summary>
        /// Obtiene las estaciones-Ruta de un Manifiesto
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRutaDeManifiesto(long IdManifiesto)
        {
            IList<RUEstacionRuta> lstRutas = RURepositorio.Instancia.ObtenerEstacionesRutaDeManifiesto(IdManifiesto);
            return lstRutas;
        }


        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son conolidado o no
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRuta(int idRuta)
        {
            return RURepositorio.Instancia.ObtenerEstacionesRuta(idRuta);
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
            List<RURutaDC> rutas = RURepositorio.Instancia.ObtenerRutasXCiudadOrigenDestino(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idLocalidadAgencia);
            return rutas;
        }

        /// <summary>
        /// Obtiene las rutas de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        public List<RURutaDC> ObtenerRutasPorLocalidad(string idLocalidad)
        {
            return RURepositorio.Instancia.ObtenerRutasPorLocalidad(idLocalidad);
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estación
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacion(string idLocalidadEstacion)
        {
            List<RURutaDC> rutas = RURepositorio.Instancia.ObtenerRutasPerteneceEstacion(idLocalidadEstacion);
            rutas = rutas.GroupBy(r => r.IdRuta).Select(r => r.First()).OrderBy(r => r.NombreRuta).ToList();
            return rutas;
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estacion, incluye las rutas en las que la estacion es origen y destino
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacionOrigDest(string idLocalidadEstacion)
        {
            List<RURutaDC> rutas = RURepositorio.Instancia.ObtenerRutasPerteneceEstacionOrigDest(idLocalidadEstacion);
            rutas = rutas.GroupBy(r => r.IdRuta).Select(r => r.First()).OrderBy(r => r.NombreRuta).ToList();
            return rutas;
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son consolidado , ordenadas por el campo orden de la ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesCiudAdicionalesRuta(int idRuta)
        {
            return RURepositorio.Instancia.ObtenerEstacionesCiudAdicionalesRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las ciudades que oertenecen a una ruta, incluye estaciones de la ruta ciudades hijas de la estacion, ciudades adicionales ciudades hijas de las adicionales y la ciudad destino de la ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las ciudades de una ruta</returns>
        public IList<PALocalidadDC> ObtenerTodasCiudadesEnRuta(int idRuta)
        {
            return RURepositorio.Instancia.ObtenerTodasCiudadesEnRuta(idRuta);
        }

        /// <summary>
        /// Obtener una ruta
        /// </summary>
        /// <param name="idLocalidadOrigen">Ciudad origen</param>
        /// <param name="idLocalidadDestino">Ciudad Destino</param>
        /// <returns>La ruta encontrada</returns>
        public RURutaDC ObtenerRuta(string idLocalidadOrigen, string idLocalidadDestino)
        {
            return RURepositorio.Instancia.ObtenerRuta(idLocalidadOrigen, idLocalidadDestino);
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
            return RURepositorio.Instancia.ObtenerEmpresaTransportadora(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Metodo para Adicionar - Editar - Eliminar Empresas Transportadoras
        /// </summary>
        public void GestionarEmpresaTransportadora(RUEmpresaTransportadora empresaTransportadora)
        {
            if (empresaTransportadora.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                RURepositorio.Instancia.AdicionarEmpresaTransportadora(empresaTransportadora);
            }

            if (empresaTransportadora.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                RURepositorio.Instancia.ActualizarEmpresaTransportadora(empresaTransportadora);
            }

            if (empresaTransportadora.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                empresaTransportadora.EstadoEmpresa = ConstantesFramework.ESTADO_INACTIVO;
                RURepositorio.Instancia.ActualizarEmpresaTransportadora(empresaTransportadora);
            }
        }

        /// <summary>
        /// Obtiene la Info Inicial para la pantalla de Empresa Trasnportadora
        /// </summary>
        /// <returns>Listas de Inicializacion</returns>
        public RUEmpresaTransporteInfoInicialDC ObtenerInfoInicialEmpresaTransportadora()
        {
            RUEmpresaTransporteInfoInicialDC infoInicial = new RUEmpresaTransporteInfoInicialDC();

            infoInicial.ListaEstadosEmpresa = ObtenerTiposEstado();
            infoInicial.ListaRacoles = ObtenerRacols();
            infoInicial.ListaMedioTransporte = new List<PAMedioTransporte>(ObtenerMediosTransporte());
            infoInicial.ListaTiposTransporte = new List<RUTipoTransporte>(ObtieneTodosTipoTransporte());

            return infoInicial;
        }

        #endregion Empresa Transportadora

        #region Otros Modulos

        /// <summary>
        /// Obtiene posibles estados
        /// </summary>
        /// <returns>Lista de Tipos de Estado</returns>
        private List<SEEstadoUsuario> ObtenerTiposEstado()
        {
            return SEAdministradorSeguridad.Instancia.ObtenerTiposEstado();
        }

        /// <summary>
        /// Obtiene Todas las Regionales
        /// </summary>
        /// <returns>Lista de Regionales</returns>
        private List<PURegionalAdministrativa> ObtenerRacols()
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerRegionalAdministrativa();
        }

        /// <summary>
        /// Obtiene los Medios de Transporte
        /// </summary>
        /// <returns>Lista de los medios de Transportes</returns>
        private IList<PAMedioTransporte> ObtenerMediosTransporte()
        {
            return PAAdministrador.Instancia.ObtenerTodosMediosTrasporte();
        }

        #endregion Otros Modulos
    }
}