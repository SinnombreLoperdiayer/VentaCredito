using System;
using System.Collections.Generic;
using CO.Servidor.CentroServicios.Datos;
using CO.Servidor.CentroServicios.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Linq;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.CentroServicios
{
    /// <summary>
    /// Clase de fachada para Centro Servicios
    /// </summary>
    public class PUAdministradorCentroServicios : ControllerBase
    {
        private static readonly PUAdministradorCentroServicios instancia = (PUAdministradorCentroServicios)FabricaInterceptores.GetProxy(new PUAdministradorCentroServicios(), COConstantesModulos.CENTRO_SERVICIOS);

        /// <summary>
        /// Retorna una instancia del administrador de Centro de servicios
        /// </summary>
        public static PUAdministradorCentroServicios Instancia
        {
            get { return PUAdministradorCentroServicios.instancia; }
        }

        /// <summary>
        /// Retorna los municipios que no permiten forma de pago "Al Cobro"
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<PUMunicipiosSinAlCobro> ObtenerMunicipiosSinFormaPagoAlCobro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PUCentroServicios.Instancia.ObtenerMunicipiosSinFormaPagoAlCobro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Agrega municipio a la lista de municipios que no admiten forma de pago "al cobro"
        /// </summary>
        /// <param name="municipio"></param>
        public void RegistrarMunicipioSinFormaPagoAlCobro(PALocalidadDC municipio)
        {
            PUCentroServicios.Instancia.RegistrarMunicipioSinFormaPagoAlCobro(municipio);
        }

        /// <summary>
        /// Quita municipio de la lista de municipios que no admiten forma de pago "al cobro"
        /// </summary>
        /// <param name="municipio"></param>
        public void RemoverMunicipioSinFormaPagoAlCobro(PALocalidadDC municipio)
        {
            PUCentroServicios.Instancia.RemoverMunicipioSinFormaPagoAlCobro(municipio);
        }

        /// <summary>
        /// Obtiene la lista de las agencias que pueden realizar pagos de giros
        /// </summary>
        public IList<PUCentroServiciosDC> ObtenerAgenciasPuedenPagarGiros()
        {
            return PUCentroServicios.Instancia.ObtenerAgenciasPuedenPagarGiros();
        }

        /// <summary>
        /// Retorna las agencias creadas en el sistemas que se encuentran activas
        /// </summary>
        /// <returns></returns>
        public List<PUAgencia> ObtenerAgenciasActivas()
        {
            return PUCentroServicios.Instancia.ObtenerAgenciasActivas();
        }

        /// <summary>
        /// Obtuene el centro de Servicio Activo
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public bool ObtenerCentroServicioActivo(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerCentroServicioActivo(idCentroServicio);
        }

        /// <summary>
        /// Valida que una agencia pueda realizar venta de mensajería y retorna  la lista de servicios de mensajería habilitados
        /// </summary>
        /// <param name="idCentroServicios"></param>
        public IEnumerable<TAServicioDC> ObtenerServiciosMensajeria(long idCentroServicios, int idListaPrecios)
        {
            return PUCentroServicios.Instancia.ObtenerServiciosMensajeria(idCentroServicios, idListaPrecios);
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio que no son Racol con el servicio Komprech activado para una localidad dada
        /// </summary>
        /// <param name="idMunicipio">Id del municipio</param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasPuntosActivosKomprechPorLocalidad(string idMunicipio)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciasPuntosActivosKomprechPorLocalidad(idMunicipio);
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicioActivosLocalidad(string idMunicipio)
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServicioActivosLocalidad(idMunicipio);
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio de una actividad
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServicioPorLocalidad(string idMunicipio)
        {
            return PUCentroServicios.Instancia.ObtenerTodosCentrosServicioPorLocalidad(idMunicipio);
        }

        /// <summary>
        /// obtener todos los tipos de ciudad
        /// </summary>
        /// <returns></returns>
        public List<PUTipoCiudad> ObtenerTiposCiudades()
        {
            return PUCentroServicios.Instancia.ObtenerTiposCiudades();
        }

        /// <summary>
        /// obtener todos los tipos de zona
        /// </summary>
        /// <returns></returns>
        public List<PUTipoZona> ObtenerTiposZona()
        {
            return PUCentroServicios.Instancia.ObtenerTiposZona();
        }


        /// <summary>
        /// Valida que una agencia pueda realizar venta de mensajería y retorna  la lista de servicios de mensajería habilitados
        /// </summary>
        /// <param name="idCentroServicios"></param>
        public IEnumerable<TAServicioDC> ObtenerServiciosMensajeriaSinValidacionHorario(long idCentroServicios, int idListaPrecios)
        {
            return PUCentroServicios.Instancia.ObtenerServiciosMensajeriaSinValidacionHorario(idCentroServicios, idListaPrecios);
        }

        /// <summary>
        /// Obtiene los Centros de servicio de acuerdo a una localidad
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los centros de servicio</returns>
        //public IList<PUCentroServiciosDC> ObtenerCentrosServicioPorLocalidad(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, string IdLocalidad, PUEnumTipoCentroServicioDC tipoCES)
        //{
        //    if (tipoCES != PUEnumTipoCentroServicioDC.PAS)
        //        return PUCentroServicios.Instancia.ObtenerCentrosServicioPorLocalidad(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdLocalidad);
        //    else 
        //    {
        //      totalRegistros = 100;
        //      long idRacol =  PURepositorio.Instancia.ObtenerRacoles().FirstOrDefault(ra => ra.IdMunicipio == IdLocalidad).IdCentroServicio;
        //      return PUCentroServicios.Instancia.ObtenerCentroServicioPASPorRacol(idRacol);
        //    }

        //}

        /// <summary>
        /// Retorna la lista de centros de servicios activos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosActivos()
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServiciosActivos();
        }

        /// <summary>
        /// Retorna la lista con todos los centros de servicios del sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServicios()
        {
            return PUCentroServicios.Instancia.ObtenerTodosCentrosServicios();
        }

        public List<PUCentroServiciosDC> ObtenerTodosCentrosServiciosXEstado(PAEnumEstados estado)
        {
            return PURepositorio.Instancia.ObtenerTodosCentrosServiciosXEstado(estado);
        }

        public List<PUCentroServiciosDC> ObtenerTodosCentrosServiciosNoInactivos()
        {
            return PURepositorio.Instancia.ObtenerTodosCentrosServiciosNoInactivos();
        }

        /// <summary>
        /// Obtener todos los coles activos
        /// </summary>
        /// <returns>Colección con los coles activos</returns>
        public List<PUCentroServiciosDC> ObtenerTodosColes()
        {
            return PUCentroServicios.Instancia.ObtenerTodosColes();
        }

        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasCol(long idCol)
        {
            return PURepositorio.Instancia.ObtenerPuntosAgenciasCol(idCol);
        }

        /// <summary>
        /// Obtiene todos los col con sus puntos y agencias
        /// </summary>
        /// <returns></returns>
        public List<PUCentroLogistico> ObtenerColConPuntosAgencias()
        {
            List<PUCentroServiciosDC> coles = PUCentroServicios.Instancia.ObtenerTodosColes();


            List<PUCentroLogistico> colesPuntos = coles.ConvertAll<PUCentroLogistico>(c =>
            {
                PUCentroLogistico col = new PUCentroLogistico()
                {
                    IdRegionalAdm = c.IdRegionalAdministrativa,
                    Nombre = c.Nombre
                };
                col.PuntosAgencias = PURepositorio.Instancia.ObtenerPuntosAgenciasCol(c.IdCentroServicio);

                return col;
            });

            return colesPuntos;
        }


        /// <summary>
        /// Retorna la lista de centro de servicios que reportan dinero a un centro de servicio dado
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicioAQuienReportan"></param>
        /// <returns></returns>
        public List<PUCentroServicioReporte> ObtenerCentrosServicioReportan(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServicioAQuienReportan)
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServicioReportan(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicioAQuienReportan);
        }

        /// <summary>
        /// Retorna los centro de servicio que reportan a un centro de servicio dado
        /// </summary>
        /// <param name="idCentroServicio">Id del centro de servicio</param>
        /// <returns></returns>
        public List<PUCentroServicioReporte> ObtenerCentrosServicioReportanCentroServicio(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServicioReportanCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// y puntos de atención de los Racoles
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Obtiene los Centros de Servicios Activos e Inactivos de una Racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosTodos(long idRacol)
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServiciosTodos(idRacol);
        }

        /// <summary>
        /// Método para obtener las agencias de un COL que sean de tipo ARO, mas los puntos de la ciudad del COL
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosAsignacionTulas(long idCol)
        {
            return PURepositorio.Instancia.ObtenerCentrosServiciosAsignacionTulas(idCol);
        }

        /// <summary>
        /// Consultar todos los centros de servicios de un RACOL y todos los RACOL activos
        /// </summary>
        /// <param name="idRacol">Identificador del RACOL</param>
        /// <returns>Colección de los centros de servicios de un RACOL y todos los RACOL activos</returns>
        public IList<PUCentroServiciosDC> ObtenerObtenerCentrosServiciosDeRacolYTodosRacol(long idRacol)
        {
            return PUCentroServicios.Instancia.ObtenerObtenerCentrosServiciosDeRacolYTodosRacol(idRacol);
        }

        /// <summary>
        /// Metodo para obtener los RACOLs
        /// </summary>
        /// <returns></returns>
        public List<PURegionalAdministrativa> ObtenerRegionalAdministrativa()
        {
            return PUCentroServicios.Instancia.ObtenerRegionalAdministrativa();
        }

        /// <summary>
        /// Metodo para Obtener la RACOL de un municipio
        /// </summary>
        /// <returns></returns>
        public PURegionalAdministrativa ObtenerRegionalAdministrativa(string idMunicipio)
        {
            return PUCentroServicios.Instancia.ObtenerRegionalAdministrativa(idMunicipio);
        }

        /// <summary>
        /// Envia la divulgacion de una agencia
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="divulgacion">Objeto con la informacion de los contactos a divulgar la agencia</param>
        public void DivulgarAgencia(long idCentroServicios, PADivulgacion divulgacion)
        {
            PUCentroServicios.Instancia.DivulgarAgencia(idCentroServicios, divulgacion);
        }

        /// <summary>
        /// Obtiene el centro de servicio.
        /// para el valor de la BaseInicial
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Adiciona el centro de servicios al que se le reporta
        /// </summary>
        /// <param name="idCentroServicioAQuienReporta"></param>
        /// <param name="idCentroServicioReporta"></param>
        public void AdicionarCentroServicioReporte(long idCentroServicioAQuienReporta, long idCentroServicioReporta)
        {
            PUCentroServicios.Instancia.AdicionarCentroServicioReporte(idCentroServicioAQuienReporta, idCentroServicioReporta);
        }

        /// <summary>
        /// Eliminar el centro de servicios al que se le reporta
        /// </summary>
        /// <param name="idCentroServicioAQuienReporta"></param>
        /// <param name="idCentroServicioReporta"></param>
        public void EliminarCentroServicioReporte(long idCentroServicioAQuienReporta, long idCentroServicioReporta)
        {
            PUCentroServicios.Instancia.EliminarCentroServicioReporte(idCentroServicioAQuienReporta, idCentroServicioReporta);
        }

        /// <summary>
        /// Obtiene la Agencia responsable del Punto
        /// </summary>
        /// <param name="idPuntoServicio">el id punto servicio.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObtenerAgenciaResponsable(long idPuntoServicio)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciaResponsable(idPuntoServicio);
        }

        /// <summary>
        /// Obtiene el Racol responsable de la Agencia.
        /// </summary>
        /// <param name="idAgencia">el id agencia.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObtenerRacolResponsable(long idAgencia)
        {
            return PUCentroServicios.Instancia.ObtenerRacolResponsable(idAgencia);
        }

        /// <summary>
        /// Metodo para consultar las localidades donde existen centros logisticos
        /// </summary>
        /// <returns></returns>
        public IList<LILocalidadColDC> ObtenerLocalidadesCol()
        {
            return PUCentroServicios.Instancia.ObtenerLocalidadesCol();
        }

        /// <summary>
        /// Metodo para consultar las agencias que dependen de un COL
        /// </summary>
        /// <returns></returns>
        public IList<LILocalidadColDC> ObtenerAgenciasCol(long idCol)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciasCol(idCol);
        }

        /// <summary>
        /// Obtiene los datos básicos de los centros de servivios de giros
        /// </summary>
        /// <returns>Colección centros de servicio</returns>
        public IEnumerable<PUCentroServiciosDC> ObtenerCentrosServicioGiros()
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServicioGiros();
        }

        /// <summary>
        /// Obtiene los puntos del centro logistico
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosServiciosCol(long idCol)
        {
            return PUCentroServicios.Instancia.ObtenerPuntosServiciosCol(idCol);
        }


        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasColReclamaOficina(long idCol)
        {
            return PUCentroServicios.Instancia.ObtenerPuntosAgenciasColReclamaOficina(idCol);
        }


        /// <summary>
        /// Obtiene el id y la descripcion de todos los centros logisticos activos y racol activos
        /// </summary>
        /// <returns>lista de centros logisticos y racol </returns>
        public IList<PUCentroServicioApoyo> ObtenerCentrosServicioApoyo()
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServicioApoyo();
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerPuntosDeAgencia(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgenciaActivos(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerPuntosDeAgenciaActivos(idCentroServicio);
        }

        /// <summary>
        /// Obtener la agencia a partir de la localida
        /// </summary>
        /// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciaLocalidad(localidad);
        }

        /// <summary>
        /// Método para obtener el representante legal de un punto
        /// </summary>
        /// <param name="idcentroservicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerRepresentanteLegalPunto(long idcentroservicio)
        {
            return PUCentroServicios.Instancia.ObtenerRepresentanteLegalPunto(idcentroservicio);
        }

        /// <summary>
        /// Obtiene las agencias de la aplicación
        /// </summary>
        public List<PUCentroServiciosDC> ObtenerAgencias(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PUCentroServicios.Instancia.ObtenerAgencias(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtener Agencias y Bodegas para Validación y Archivo - Control de Cuentas, según filtro
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasBodegas(IDictionary<string, string> filtro)//, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciasBodegas(filtro);
        }

        /// <summary>
        /// Obtiene las agencias de la aplicación sin filtro
        /// </summary>
        public List<PUCentroServiciosDC> ObtenerAgencias()
        {
            return PUCentroServicios.Instancia.ObtenerAgencias();
        }

        /// <summary>
        /// Metodo que consulta todas las agencias y puntos de un RACOL
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasYPuntosRacolActivos(long idRacol)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciasYPuntosRacolActivos(idRacol);
        }

        /// <summary>
        /// obtiene el centro de servicio Adscrito a un racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroServicioAdscritoRacol(long idRacol, long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerCentroServicioAdscritoRacol(idRacol, idCentroServicio);
        }

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasRacolActivos(long idRacol)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciasRacolActivos(idRacol);
        }

        /// <summary>
        /// Actualiza la informacin de una agencia para el servicio de giros
        /// </summary>
        /// <param name="centroServicio"></param>
        public void ActualizarConfiguracionGiros(PUCentroServiciosDC centroServicio)
        {
            PUCentroServicios.Instancia.ActualizarConfiguracionGiros(centroServicio);
        }

        /// <summary>
        /// Obtener las observaciones de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public IList<PUObservacionCentroServicioDC> ObtenerObservacionCentroServicio(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerObservacionCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene todas las opciones de clasificador canal ventas
        /// </summary>
        /// <returns></returns>
        public IList<PUClasificadorCanalVenta> ObtenerTodosClasificadorCanalVenta()
        {
            return PUCentroServicios.Instancia.ObtenerTodosClasificadorCanalVenta();
        }


        public List<PALocalidadDC> ObtenerMunicipiosXCol(long IdCol)
        {
            return PURepositorio.Instancia.ObtenerMunicipiosXCol(IdCol);
        }

        /// <summary>
        /// Consulta los municipios y su respectivo centro logístico asociado
        /// </summary>
        /// <param name="IdDepartamento">Id del departamento por el cual se quiere filtrar</param>
        /// <returns></returns>
        public List<PUMunicipioCentroLogisticoDC> ConsultarMunicipiosCol(string IdDepartamento)
        {
            return PURepositorio.Instancia.ConsultarMunicipiosCol(IdDepartamento);
        }

        /// <summary>
        /// Guarda en la base de datos el municipio con su respectivo centro logistico de apoyo
        /// </summary>
        /// <param name="municipioCol"></param>
        public void GuardarMunicipioCol(PUMunicipioCentroLogisticoDC municipioCol)
        {
            PUCentroServicios.Instancia.GuardarMunicipioCol(municipioCol);
        }

        #region Clasificador canal de venta

        /// <summary>
        /// Inserta Modifica o Elimina un registro de clasificador de canal de venta
        /// </summary>
        /// <param name="clasificadorCanalVenta"></param>
        public void ActualizarClasificarCanalVenta(PUClasificadorCanalVenta clasificadorCanalVenta)
        {
            switch (clasificadorCanalVenta.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PUCentroServicios.Instancia.AgregarClasificarCanalVenta(clasificadorCanalVenta);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PUCentroServicios.Instancia.BorrarClasificadorCanalVenta(clasificadorCanalVenta);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PUCentroServicios.Instancia.ModificarClasificadorCanalVenta(clasificadorCanalVenta);
                    break;
            }
        }

        /// <summary>
        /// Obtiene los clasificadores del canal de ventas
        /// </summary>
        public List<PUClasificadorCanalVenta> ObtenerClasificadorCanalVenta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PUCentroServicios.Instancia.ObtenerClasificadorCanalVenta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Selecciona todos los tipos de centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUTipoCentroServicio> ObtenerTodosTipoCentroServicio()
        {
            return PUCentroServicios.Instancia.ObtenerTodosTipoCentroServicio();
        }

        #endregion Clasificador canal de venta

        /// <summary>
        /// Obtienen todos los municipios de un racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>municipios del racol</returns>
        public List<PALocalidadDC> ObtenerMunicipiosDeRacol(long idRacol)
        {
            return PUCentroServicios.Instancia.ObtenerMunicipiosDeRacol(idRacol);
        }

        /// <summary>
        /// Obtiene el horario de la recogida de un centro de Servicio
        /// </summary>
        /// <param name="idCentroSvc">es le id del centro svc</param>
        /// <returns>info de la recogida</returns>
        public IList<PUHorarioRecogidaCentroSvcDC> ObtenerHorariosRecogidasCentroSvc(long idCentroSvc)
        {
            return PUCentroServicios.Instancia.ObtenerHorariosRecogidasCentroSvc(idCentroSvc);
        }

        /// <summary>
        /// obtiene todos los centros servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicio()
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServicio();
        }

        public List<PUCentroServiciosDC> ObtenerCentrosServicioTipo()
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServicioTipo();
        }

        /// <summary>
        /// Adiciona los Horarios de las recogidas
        /// de los centros de Svc
        /// </summary>
        /// <param name="centroServicios">info del Centro de Servicio</param>
        public void AdicionarHorariosRecogidasCentroSvc(PUCentroServiciosDC centroServicios)
        {
            PUCentroServicios.Instancia.AdicionarHorariosRecogidasCentroSvc(centroServicios);
        }

        /// <summary>
        /// Obtiene la informacion de una agencia dependiendo del id
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerAgencia(long idAgencia)
        {
            return PUCentroServicios.Instancia.ObtenerAgencia(idAgencia);
        }

        /// <summary>
        /// Obtiene los COL de un Racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>lista de Col de un Racol</returns>
        public List<PUCentroServicioApoyo> ObtenerCentrosLogisticosRacol(long idRacol)
        {
            return PUCentroServicios.Instancia.ObtenerCentrosLogisticosRacol(idRacol);
        }

        /// <summary>
        /// Obtiene el col responsable de la agencia de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public long ObtieneIdCOLResponsableAgenciaLocalidad(string idLocalidad)
        {
            return PUCentroServicios.Instancia.ObtieneIdCOLResponsableAgenciaLocalidad(idLocalidad);
        }
        #region Regionales Casa Matriz

        /// <summary>
        /// Obtener la información basica de las Regionales Administrativas activas de una Casa Matriz
        /// </summary>
        /// <param name="idCasaMatriz">Identificador de la Casa Matriz</param>
        /// <returns>Colección con la información básica de las regionales</returns>
        public IList<PURegionalAdministrativa> ObtenerRegionalesDeCasaMatriz(short idCasaMatriz)
        {
            return PUCentroServicios.Instancia.ObtenerRegionalesDeCasaMatriz(idCasaMatriz);
        }

        #endregion Regionales Casa Matriz

        public List<string> ConsultarRacoles()
        {
            return PUCentroServicios.Instancia.ConsultarRacoles();
        }

        /// <summary>
        /// Obtiene todas las agencias y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosAgenciayPuntosActivos()
        {
            return PUCentroServicios.Instancia.ObtenerTodosAgenciayPuntosActivos();
        }
        /// <summary>
        /// Obtiene todas las agencias, col y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosAgenciaColPuntosActivos()
        {
            return PUCentroServicios.Instancia.ObtenerTodosAgenciaColPuntosActivos();

        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerInformacionGeneralCentrosServicio()
        {
            return PUCentroServicios.Instancia.ObtenerInformacionGeneralCentrosServicio();
        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerPosicionesCanalesVenta(DateTime fechaInicial, DateTime fechaFinal, string idMensajero, string idCentroServicio, int idEstado)
        {
            return PUCentroServicios.Instancia.ObtenerPosicionesCanalesVenta(fechaInicial, fechaFinal, idMensajero, idCentroServicio, idEstado);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerServiciosCentroServicio(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerServiciosCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerCentrosServicioPorServicio(int idServicio)
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServicioPorServicio(idServicio);
        }

        /// <summary>
        /// Obtiene la bodega custodia 
        /// </summary>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerBodegaCustodia()
        {
            return PUCentroServicios.Instancia.ObtenerBodegaCustodia();
        }

        /// <summary>
        /// Inserta el movimiento inventario solo para el ingreso a CAC desde LOI o Custodia
        /// </summary>
        /// <param name="IdCentroServicio"></param>
        /// <param name="IdTipoMovimiento"></param>
        /// <param name="NumeroGuia"></param>
        /// <param name="FechaGrabacion"></param>
        /// <param name="CreadoPor"></param>
        //public void AdicionarMovimientoInventario_CAC(long IdCentroServicio, int IdTipoMovimiento, long NumeroGuia, DateTime FechaGrabacion, string CreadoPor)
        //{
        //    PUCentroServicios.Instancia.AdicionarMovimientoInventario_CAC(IdCentroServicio, IdTipoMovimiento, NumeroGuia, FechaGrabacion, CreadoPor);
        //}


        /// <summary>
        /// Obtiene el centro de acopio correspondiente a una bodega.
        /// </summary>
        /// <param name="idBodega"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroDeAcopioBodega(long idBodega, long idUsuario)
        {
            return PUCentroServicios.Instancia.ObtenerCentroDeAcopioBodega(idBodega, idUsuario);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroConfirmacionesDevoluciones(PALocalidadDC localidad)
        {
            return PUCentroServicios.Instancia.ObtenerCentroConfirmacionesDevoluciones(localidad);
        }

        /// <summary>
        /// Metodo para adicionar movimiento inventario
        /// </summary>
        /// <param name="movimientoInventario"></param>
        /// <returns></returns>
        public long AdicionarMovimientoInventario(PUMovimientoInventario movimientoInventario)
        {
            return PUCentroServicios.Instancia.AdicionarMovimientoInventario(movimientoInventario);
        }

        /// <summary>
        /// Obtiene la lista de las Territoriales
        /// </summary>
        /// <returns></returns>
        public List<PUTerritorialDC> ObtenerTerritoriales()
        {
            return PUCentroServicios.Instancia.ObtenerTerritoriales().ToList();
        }

        /// <summary>
        /// Obtiene los centros de servicio a los cuales tiene acceso el usuario
        /// </summary>
        /// <param name="identificacionUsuario"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerLocacionesAutorizadas(string usuario)
        {
            return PUCentroServicios.Instancia.ObtenerLocacionesAutorizadas(usuario);
        }
    }
}