using CO.Servidor.CentroServicios.Datos.Modelo;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;

namespace CO.Servidor.CentroServicios
{
    /// <summary>
    /// Fachada para centro de servicios para interfaz con modulos del dominio
    /// </summary>
    public class PUFachadaCentroServicios : IPUFachadaCentroServicios
    {
        /// <summary>
        /// Instancia Singleton
        /// </summary>
        private static readonly PUFachadaCentroServicios instancia = new PUFachadaCentroServicios();

        #region Propiedades

        /// <summary>
        /// Retorna una instancia de la fabrica de Dominio
        /// </summary>
        public static PUFachadaCentroServicios Instancia
        {
            get { return PUFachadaCentroServicios.instancia; }
        }

        #endregion Propiedades

        /// <summary>
        /// Valida que una agencia pueda realizar la venta de un giro
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        public void ValidarAgenciaServicioGiros(long idCentroServicios)
        {
            PUCentroServicios.Instancia.ValidarAgenciaServicioGiros(idCentroServicios);
        }

        /// <summary>
        /// Valida que una agencia pueda realizar el pago de un giro
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        public void ValidarAgenciaServicioPagos(long idCentroServicios)
        {
            PUCentroServicios.Instancia.ValidarAgenciaServicioPagos(idCentroServicios);
        }

        /// <summary>
        /// Actualiza la información de validación de dos centros de servicios implicados en un trayecto
        /// </summary>
        /// <param name="localidadDestino">Localidad de destino del trayecto</param>
        /// <param name="idCentroServicio">Identificador del Centro de servicios que inicia la transacción</param>
        /// <param name="validacion">Contiene la información de las agencias implicadas en el proceso</param>
        public void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, long idCentroServicio, PALocalidadDC localidadOrigen = null)
        {
            PUCentroServicios.Instancia.ObtenerInformacionValidacionTrayecto(localidadDestino, validacion, idCentroServicio, localidadOrigen);
        }

        /// <summary>
        /// Obtener información de validación del trayecto
        /// </summary>
        /// <param name="localidadOrigen"></param>
        /// <param name="idCentroServicioOrigen"></param>
        public void ObtenerInformacionValidacionTrayectoOrigen(PALocalidadDC localidadOrigen, ADValidacionServicioTrayectoDestino validacion)
        {
            PUCentroServicios.Instancia.ObtenerInformacionValidacionTrayectoOrigen(localidadOrigen, validacion);
        }

        /// <summary>
        /// Actualiza la información de validación de un trayecto establecido desde una sucursal de un cliente
        /// </summary>
        /// <param name="localidadDestino">Localidad de destino del envío</param>
        /// <param name="validacion">Contiene información con los reusltados de las validaciones relacionadas con el trayecto</param>
        /// <param name="idCliente">Identificador del cliente que ingresa</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        public void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, int idCliente, int idSucursal)
        {
            PUCentroServicios.Instancia.ObtenerInformacionValidacionTrayecto(localidadDestino, validacion, idCliente, idSucursal);
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
        /// Retorna la lista de puntos y agencias dependientes de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasDependientes(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerPuntosAgenciasDependientes(idCentroServicio);
        }

        /// <summary>
        /// Valida que el Centro de servicio no supere el valor maximo a enviar de giros y acumula el valor del giro a la agenci
        /// </summary>
        /// <param name="idCentroServicio"></param>
        public void AcumularVentaGirosAgencia(GIAdmisionGirosDC giro)
        {
            PUCentroServicios.Instancia.AcumularVentaGirosAgencia(giro);
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
        /// Obtiene los centros de servicio que tienen giros pendientes a tranasmitir
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentroserviciosGirosATransmitir(long idRacol)
        {
            return PUCentroServicios.Instancia.ObtenerCentroserviciosGirosATransmitir(idRacol);
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
        /// Obtiene la Agencia responsable del Punto
        /// </summary>
        /// <param name="idPuntoServicio">el id punto servicio.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObtenerAgenciaResponsable(long idPuntoServicio)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciaResponsable(idPuntoServicio);
        }

        /// <summary>
        /// Obtiene el centro de servicios responsable
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public long ObtenerCentroLogisticoApoyo(long idCentroServicios)
        {
            return PUCentroServicios.Instancia.ObtenerCentroLogisticoResponsable(idCentroServicios);
        }

        /// <summary>
        /// Obtiene el centro de servicio responsable de un centro servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroLogisticoResponsableCentroServicio(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerCentroLogisticoResponsableCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene el COL responsable de un centro de servicios
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCOLResponsable(long idCentroServicios)
        {
            return PUCentroServicios.Instancia.ObtenerCOLResponsable(idCentroServicios);
        }

        /// <summary>
        /// Obtiene el col responsable de la agencia de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtieneCOLResponsableAgenciaLocalidad(string idLocalidad)
        {
            return PUCentroServicios.Instancia.ObtieneCOLResponsableAgenciaLocalidad(idLocalidad);
        }

        public PUCentroServiciosDC ObtieneCOLPorLocalidad(string idLocalidad)
        {
            return PUCentroServicios.Instancia.ObtieneCOLResponsableAgenciaLocalidad(idLocalidad);
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
        /// Obtiene el Racol responsable de la Agencia.
        /// </summary>
        /// <param name="idAgencia">el id agencia.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObteneColPropietarioBodega(long idAgencia)
        {
            return PUCentroServicios.Instancia.ObteneColPropietarioBodega(idAgencia);

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
        /// Obtener la agencia a partir de la localida
        /// </summary>
        /// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerAgenciaLocalidad(localidad);
        }

        /// <summary>
        /// Método para obtener el representante legal de un punto
        /// </summary>
        /// <param name="idcentroservicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerRepresentanteLegalPunto(long idcentroservicio)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerRepresentanteLegalPunto(idcentroservicio);
        }

        /// <summary>
        /// Obtener centros de servicios y racol
        /// </summary>
        /// <returns></returns>
        public List<PUAgenciaDeRacolDC> ObtenerCentroServiciosRacol()
        {
            return PUCentroServicios.Instancia.ObtenerCentroServiciosRacol();
        }

        /// <summary>
        /// Obtiene los Centros de servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los centros de servicio</returns>
        public IList<PUCentroServiciosDC> ObtenerCentrosServicioAgenciasPuntos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServicioAgenciasPuntos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Consulta si el centro de servicios puede pagar giros
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public bool ObtenerCentroServiciosPuedePagarGiros(long idCentroServicios)
        {
            return PUCentroServicios.Instancia.ObtenerCentroServiciosPuedePagarGiros(idCentroServicios);
        }

        /// <summary>
        /// Obtiene el centro de servicios y el responsable
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroServiciosPersonaResponsable(long idCentroServicios)
        {
            return PUCentroServicios.Instancia.ObtenerCentroServiciosPersonaResponsable(idCentroServicios);
        }

        /// <summary>
        /// Obtiene las agencias de la aplicación sin filtro
        /// </summary>
        public List<PUCentroServiciosDC> ObtenerAgencias()
        {
            return PUCentroServicios.Instancia.ObtenerAgencias();
        }

        /// <summary>
        /// Obtiene las agencias de la aplicación
        /// </summary>
        public List<PUCentroServiciosDC> ObtenerAgenciasBodegas(IDictionary<string, string> filtro)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciasBodegas(filtro);
        }



        /// <summary>
        /// Actualiza la informacin de una agencia para el servicio de giros
        /// </summary>
        /// <param name="centroServicio"></param>
        public void ActualizarConfiguracionGiros(PUCentroServiciosDC centroServicio)
        {
            PUAdministradorCentroServicios.Instancia.ActualizarConfiguracionGiros(centroServicio);
        }

        /// <summary>
        /// Obtiene el centro de servicio responsable de una agencia
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroLogisticoAgencia(long idAgencia)
        {
            return PUCentroServicios.Instancia.ObtenerCentroLogisticoAgencia(idAgencia);
        }

        /// <summary>
        /// Obtener las observaciones de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public IList<PUObservacionCentroServicioDC> ObtenerObservacionCentroServicio(long idCentroServicio)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerObservacionCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los centros logisticos
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioApoyo> ObtenerCentroLogistico()
        {
            return PUCentroServicios.Instancia.ObtenerCentroLogistico();
        }

        /// <summary>
        /// Obtiene la informacion de un centro se servicio por el id del centro de servicio
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerInformacionCentroServicioPorId(long idCentroServicios)
        {
            return PUCentroServicios.Instancia.ObtenerInformacionCentroServicioPorId(idCentroServicios);
        }

        /// <summary>
        /// Obtiene el responsable segun el tipo del Cerntro de Svr
        /// </summary>
        /// <param name="idCentroSrv"></param>
        /// <param name="tipoCentroSrv"></param>
        /// <returns>info del centro de servicio responsable</returns>
        public PUAgenciaDeRacolDC ObtenerResponsableCentroSrvSegunTipo(long idCentroSrv, string tipoCentroSrv)
        {
            return PUCentroServicios.Instancia.ObtenerResponsableCentroSrvSegunTipo(idCentroSrv, tipoCentroSrv);
        }

        /// <summary>
        /// Obtiene la informacion de una agencia dependiendo del id
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerAgencia(long idAgencia)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerAgencia(idAgencia);
        }

        /// <summary>
        /// Obtiene las Agencias que pueden pagar Giros
        /// </summary>
        /// <returns>lista de Agencias que pueden pagar giros</returns>
        public IList<PUCentroServiciosDC> ObtenerAgenciasPuedenPagarGiros()
        {
            return PUCentroServicios.Instancia.ObtenerAgenciasPuedenPagarGiros();
        }

        /// <summary>
        /// Retorna la lista de centros de servicios activos en el sistema
        /// </summary>
        /// <returns></returns>
        public IList<PUCentroServiciosDC> ObtenerCentrosServiciosActivos()
        {
            return PUCentroServicios.Instancia.ObtenerCentrosServiciosActivos();
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
        /// Obtiene los COL de un Racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>lista de Col de un Racol</returns>
        public List<PUCentroServicioApoyo> ObtenerCentrosLogisticosRacol(long idRacol)
        {
            return PUCentroServicios.Instancia.ObtenerCentrosLogisticosRacol(idRacol);
        }

        /// <summary>
        /// Validar si una ciudad se apoya en un col
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <param name="IdCol"></param>
        /// <returns></returns>
        public bool ValidarCiudadSeApoyaCOL(string idLocalidad, long idCol)
        {
            return PUCentroServicios.Instancia.ValidarCiudadSeApoyaCOL(idLocalidad, idCol);
        }

        #region Validación Suministros

        /// <summary>
        /// Obtener el centro logistico en el que se apoya un municipio
        /// </summary>
        /// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerCentroLogisticoApoyaMunicipio(string localidad)
        {
            return PUCentroServicios.Instancia.ObtenerCOLResponsableMunicipio(localidad);
        }

        /// <summary>
        /// Metodo para validar la provisión de un suministro en un centro de servicio
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="idSuministro"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns> objeto de tipo suministro </returns>
        public SUSuministro ValidarSuministroSerial(long serial, int idSuministro, long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ValidarSuministroSerial(serial, idSuministro, idCentroServicio);
        }

        #endregion Validación Suministros
        #region  Custodia

        /// <summary>
        /// ingresar a custodia la guia
        /// </summary>                
        public bool AdicionarCustodia(PUCustodia Custodia)
        {
            return PUCentroServicios.Instancia.AdicionarCustodia(Custodia);
        }



        public List<PUCustodia> ObtenerGuiasCustodia(int idTipoMovimiento, Int16 idEstadoGuia, long numeroGuia, bool muestraReportemuestraTodosreporte)
        {
            return PUCentroServicios.Instancia.ObtenerGuiasCustodia(idTipoMovimiento, idEstadoGuia, numeroGuia, muestraReportemuestraTodosreporte);

        }


        #endregion


        #region AdjuntosMovimientoInventario

        public void AdicionarAdjuntoMovimientoInventario(PUAdjuntoMovimientoInventario AdjuntoMovimientoInventario)
        {
            PUCentroServicios.Instancia.AdicionarAdjuntoMovimientoInventario(AdjuntoMovimientoInventario);
        }

        #endregion

        #region Bodegas


        public long AdicionarMovimientoInventario(PUMovimientoInventario movimientoInventario)
        {
            return PUCentroServicios.Instancia.AdicionarMovimientoInventario(movimientoInventario);
        }


        /// <summary>
        /// <Proceso para obtener el centro de confirmaciones y devoluciones de una localidad>
        /// </summary>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroConfirmacionesDevoluciones(PALocalidadDC localidad)
        {
            return PUCentroServicios.Instancia.ObtenerCentroConfirmacionesDevoluciones(localidad);
        }

        /// <summary>
        /// <Proceso para obtenerla bodega de custodia
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

        #endregion



        #region Bodegas consultar ultimo movimiento guia


        public PUMovimientoInventario ConsultaUltimoMovimientoGuia(long NumeroGuia)
        {
            return PUCentroServicios.Instancia.ConsultaUltimoMovimientoGuia(NumeroGuia);
        }


        #endregion



        public List<PUCentroServicioApoyo> ObtenerPuntosREOSegunUbicacionDestino(int idLocalidadDestino)
        {
            return PUCentroServicios.Instancia.ObtenerPuntosREOSegunUbicacionDestino(idLocalidadDestino);
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
        /// obtiene tipo y centro servicio responsable de centro servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerTipoYResponsableCentroServicio(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerTipoYResponsableCentroServicio(idCentroServicio);
        }
        
    }
}