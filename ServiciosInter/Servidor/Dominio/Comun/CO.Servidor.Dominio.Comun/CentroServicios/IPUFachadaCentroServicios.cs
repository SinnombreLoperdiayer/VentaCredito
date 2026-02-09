using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;

namespace CO.Servidor.Dominio.Comun.CentroServicios
{
    /// <summary>
    /// Interfaz para acceso a la fachada de centros de servicios
    /// </summary>
    public interface IPUFachadaCentroServicios
    {
        /// <summary>
        /// Valida que una agencia pueda realizar la venta de un giro
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        void ValidarAgenciaServicioGiros(long idCentroServicios);

        /// <summary>
        /// Actualiza la información de validación de dos centros de servicios implicados en un trayecto
        /// </summary>
        /// <param name="localidadDestino">Localidad de destino del trayecto</param>
        /// <param name="idCentroServicio">Identificador del Centro de servicios que inicia la transacción</param>
        /// <param name="validacion">Contiene la información de las agencias implicadas en el proceso</param>
        void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, long idCentroServicio, PALocalidadDC localidadOrigen = null);

        /// <summary>
        /// Obtener información de validación del trayecto
        /// </summary>
        /// <param name="localidadOrigen"></param>
        /// <param name="idCentroServicioOrigen"></param>
        void ObtenerInformacionValidacionTrayectoOrigen(PALocalidadDC localidadOrigen, ADValidacionServicioTrayectoDestino validacion);

        /// <summary>
        /// Actualiza la información de validación de un trayecto establecido desde una sucursal de un cliente
        /// </summary>
        /// <param name="localidadDestino">Localidad de destino del envío</param>
        /// <param name="validacion">Contiene información con los reusltados de las validaciones relacionadas con el trayecto</param>
        /// <param name="idCliente">Identificador del cliente que ingresa</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, int idCliente, int idSucursal);

        /// <summary>
        /// Valida que el Centro de servicio no supere el valor maximo a enviar de giros y acumula el valor del giro a la agenci
        /// </summary>
        /// <param name="idCentroServicio"></param>
        void AcumularVentaGirosAgencia(GIAdmisionGirosDC giro);

        /// <summary>
        /// Obtuene el centro de Servicio Activo
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        bool ObtenerCentroServicioActivo(long idCentroServicio);

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// y puntos de atención de los Racoles
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol);

        /// <summary>
        /// Obtiene los Centros de Servicios Activos e Inactivos de una Racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        List<PUCentroServiciosDC> ObtenerCentrosServiciosTodos(long idRacol);

        /// <summary>
        /// Obtener el centro logistico en el que se apoya un municipio
        /// </summary>
        /// <param name="localidad"></param>
        PUCentroServiciosDC ObtenerCentroLogisticoApoyaMunicipio(string localidad);


        /// <summary>
        /// Metodo para obtener los RACOLs
        /// </summary>
        /// <returns></returns>
        List<PURegionalAdministrativa> ObtenerRegionalAdministrativa();

        /// <summary>
        /// Metodo para Obtener la RACOL de un municipio
        /// </summary>
        /// <returns></returns>
        PURegionalAdministrativa ObtenerRegionalAdministrativa(string idMunicipio);

        /// <summary>
        /// Obtiene el centro servicio.
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio);

        /// <summary>
        /// Obtiene la Agencia responsable del Punto
        /// </summary>
        /// <param name="idPuntoServicio">el id punto servicio.</param>
        /// <returns></returns>
        PUAgenciaDeRacolDC ObtenerAgenciaResponsable(long idPuntoServicio);

        /// <summary>
        /// Obtiene las agencias de la aplicación sin filtro
        /// </summary>
        List<PUCentroServiciosDC> ObtenerAgencias();

        /// <summary>
        /// Obtiene las agencias y bodegas de la aplicación
        /// </summary>
        List<PUCentroServiciosDC> ObtenerAgenciasBodegas(IDictionary<string, string> filtro);

        /// <summary>
        /// Obtiene el Racol responsable de la Agencia.
        /// </summary>
        /// <param name="idAgencia">el id agencia.</param>
        /// <returns></returns>
        PUAgenciaDeRacolDC ObtenerRacolResponsable(long idAgencia);



        /// <summary>
        /// Obtiene el Racol responsable de la Agencia.
        /// </summary>
        /// <param name="idAgencia">el id agencia.</param>
        /// <returns></returns>
        PUAgenciaDeRacolDC ObteneColPropietarioBodega(long idBodega);

        /// <summary>
        /// Metodo para validar la provisión de un suministro en un centro de servicio
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="idSuministro"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns> objeto de tipo suministro </returns>
        SUSuministro ValidarSuministroSerial(long serial, int idSuministro, long idCentroServicio);

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio);

        /// <summary>
        /// Retorna la lista de puntos y agencias dependientes de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        List<PUCentroServiciosDC> ObtenerPuntosAgenciasDependientes(long idCentroServicio);

        /// <summary>
        /// Obtener la agencia a partir de la localida
        /// </summary>
        /// <param name="localidad"></param>
        PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad);

        /// <summary>
        /// Método para obtener el representante legal de un punto
        /// </summary>
        /// <param name="idcentroservicio"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerRepresentanteLegalPunto(long idcentroservicio);

        /// <summary>
        /// Obtiene los centros de servicio que tienen giros pendientes a tranasmitir
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        List<PUCentroServiciosDC> ObtenerCentroserviciosGirosATransmitir(long idRacol);

        /// <summary>
        /// Obtiene el centro de servicios responsable
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        long ObtenerCentroLogisticoApoyo(long idCentroServicios);

        /// <summary>
        /// Obtiene el centro de servicio responsable de un centro servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerCentroLogisticoResponsableCentroServicio(long idCentroServicio);

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
        IList<PUCentroServiciosDC> ObtenerCentrosServicioAgenciasPuntos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        /// <summary>
        /// Consulta si el centro de servicios puede pagar giros
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        bool ObtenerCentroServiciosPuedePagarGiros(long idCentroServicios);

        /// <summary>
        /// Obtiene el centro de servicios y el responsable
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerCentroServiciosPersonaResponsable(long idCentroServicios);

        /// <summary>
        /// Obtener centros de servicios y racol
        /// </summary>
        /// <returns></returns>
        List<PUAgenciaDeRacolDC> ObtenerCentroServiciosRacol();

        /// <summary>
        /// Actualiza la informacin de una agencia para el servicio de giros
        /// </summary>
        /// <param name="centroServicio"></param>
        void ActualizarConfiguracionGiros(PUCentroServiciosDC centroServicio);

        /// <summary>
        /// Obtener las observaciones de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        IList<PUObservacionCentroServicioDC> ObtenerObservacionCentroServicio(long idCentroServicio);

        /// <summary>
        /// Obtiene el centro de servicio responsable de una agencia
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerCentroLogisticoAgencia(long idAgencia);

        /// <summary>
        /// Obtiene los centros logisticos
        /// </summary>
        /// <returns></returns>
        List<PUCentroServicioApoyo> ObtenerCentroLogistico();

        /// <summary>
        /// Obtiene la informacion de un centro se servicio por el id del centro de servicio
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerInformacionCentroServicioPorId(long idCentroServicios);

        /// <summary>
        /// Obtiene el COL responsable de un centro de servicios
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerCOLResponsable(long idCentroServicios);

        /// <summary>
        /// Obtiene el col responsable de la agencia de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtieneCOLResponsableAgenciaLocalidad(string idLocalidad);

        PUCentroServiciosDC ObtieneCOLPorLocalidad(string idLocalidad);

        /// <summary>
        /// Obtiene el responsable segun el tipo del Cerntro de Svr
        /// </summary>
        /// <param name="idCentroSrv"></param>
        /// <param name="tipoCentroSrv"></param>
        /// <returns>info del centro de servicio responsable</returns>

        PUAgenciaDeRacolDC ObtenerResponsableCentroSrvSegunTipo(long idCentroSrv, string tipoCentroSrv);

        /// <summary>
        /// Obtiene las Agencias que pueden pagar Giros
        /// </summary>
        /// <returns>lista de Agencias que pueden pagar giros</returns>
        IList<PUCentroServiciosDC> ObtenerAgenciasPuedenPagarGiros();

        /// <summary>
        /// Obtiene la informacion de una agencia dependiendo del id
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerAgencia(long idAgencia);

        /// <summary>
        /// Retorna la lista de centros de servicios activos en el sistema
        /// </summary>
        /// <returns></returns>
        IList<PUCentroServiciosDC> ObtenerCentrosServiciosActivos();

        /// <summary>
        /// Metodo que consulta todas las agencias y puntos de un RACOL
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        List<PUCentroServiciosDC> ObtenerAgenciasYPuntosRacolActivos(long idRacol);

        /// <summary>
        /// obtiene el centro de servicio Adscrito a un racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerCentroServicioAdscritoRacol(long idRacol, long idCentroServicio);

        /// <summary>
        /// Obtiene los COL de un Racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>lista de Col de un Racol</returns>
        List<PUCentroServicioApoyo> ObtenerCentrosLogisticosRacol(long idRacol);

        /// <summary>
        /// Validar si una ciudad se apoya en un col
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <param name="IdCol"></param>
        /// <returns></returns>
        bool ValidarCiudadSeApoyaCOL(string idLocalidad, long idCol);

        /// <summary>
        /// Obtiene el id col responsable de la agencia de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        long ObtieneIdCOLResponsableAgenciaLocalidad(string idLocalidad);

        #region  Custodia

        /// <summary>
        /// ingresar a custodia la guia
        /// </summary>                


        bool AdicionarCustodia(PUCustodia Custodia);


        List<PUCustodia> ObtenerGuiasCustodia(int idTipoMovimiento, Int16 idEstadoGuia, long numeroGuia, bool muestraReportemuestraTodosreporte);


        #endregion

        #region AdjuntosMovimientoInventario

        void AdicionarAdjuntoMovimientoInventario(PUAdjuntoMovimientoInventario AdjuntoMovimientoInventario);


        #endregion

        #region Bodegas


        long AdicionarMovimientoInventario(PUMovimientoInventario movimientoInventario);
        /// <summary>
        /// <Proceso para obtener el centro de confirmaciones y devoluciones de una localidad>
        /// </summary>
        /// <param name="localidad"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerCentroConfirmacionesDevoluciones(PALocalidadDC localidad);


        /// <summary>
        /// <Proceso para obtenerla bodega de custodia
        /// </summary>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerBodegaCustodia();


        #endregion


        #region Bodegas adiciona movimiento guia - reclame en oficina


        PUMovimientoInventario ConsultaUltimoMovimientoGuia(long NumeroGuia);



        #endregion


         List<PUCentroServicioApoyo> ObtenerPuntosREOSegunUbicacionDestino(int idLocalidadDestino);

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
         List<PUMunicipiosSinAlCobro> ObtenerMunicipiosSinFormaPagoAlCobro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        /// <summary>
        /// obtiene tipo y centro servicio responsable de centro servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        PUCentroServiciosDC ObtenerTipoYResponsableCentroServicio(long idCentroServicio);
    }
}