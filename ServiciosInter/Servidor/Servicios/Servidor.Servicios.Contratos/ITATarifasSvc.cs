using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Novasoft;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Adicionales;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.ListaPrecios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ITATarifasSvc
    {
        /// <summary>
        /// Obtiene los tipos de envío que están en la base de datos
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de envío</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TATipoEnvio> ObtenerTiposEnvio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Retorna los tipos de envíos con los servicios asociados
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATipoEnvio> ObtenerTiposDeEnvio();

        /// <summary>
        /// Retorna una lista con los tipos de envio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<TATipoEnvio> ObtenerTipoEnvios();

        /// <summary>
        /// Adicionar, editar o eliminar un tipo de envío
        /// </summary>
        /// <param name="tipoEnvio">Objeto con la información del tipo de envío</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTipoEnvio(TATipoEnvio tipoEnvio);

        /// <summary>
        /// Obtiene los tipos de moneda
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de moneda</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TAMonedaDC> ObtenerMoneda(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, edita o elimina moneda
        /// </summary>
        /// <param name="tipoEnvio">Objeto moneda</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarMoneda(TAMonedaDC tipoEnvio);

        /// <summary>
        /// Obtiene los tipos de empaque
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de empaque</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TATipoEmpaque> ObtenerTiposEmpaque(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, edita o elimina tipos de empaque
        /// </summary>
        /// <param name="tipoEnvio">Objeto tipo de empaque</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTiposEmpaque(TATipoEmpaque tipoEmpaque);

        /// <summary>
        /// Obtiene los tipos de trámite
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de trámite</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TATipoTramite> ObtenerTiposTramite(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, edita o elimina un tipo de trámite
        /// </summary>
        /// <param name="tipoTramite">Objeto tipo trámite</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTiposTramite(TATipoTramite tipoTramite);

        /// <summary>
        /// Obtiene los tipos de valor adicional
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de valor adicional</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TAValorAdicional> ObtenerValorAdicional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Retorna los valores adicionales que son de tipo embalaje
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<TAValorAdicional> ConsultarValoresAdicionalesEmbalaje();

        /// <summary>
        /// Adiciona, edita o elimina un valor adicional
        /// </summary>
        /// <param name="valorAdicional"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarValorAdicional(TAValorAdicional valorAdicional);

        /// <summary>
        /// Obtiene la lista de valor adicional de la DB
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAServicioDC> ObtenerServicios();

        /// <summary>
        /// Obtiene la lista de servicios por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Id de la unidad de negocio</param>
        /// <returns>Lista de Servicios de la unidad de negocio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<TAServicioDC> ObtenerServiciosUnidadNegocio(string IdUnidadNegocio);

        /// <summary>
        /// Obtiene los tipos de trayecto
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de trayecto</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TATipoTrayecto> ObtenerTiposTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, edita o elimina un tipo de trayecto
        /// </summary>
        /// <param name="tipoTrayecto">Objeto tipo trayecto</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTiposTrayecto(TATipoTrayecto tipoTrayecto);

        /// <summary>
        /// Obtiene los tipos de Subtrayecto
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de Subtrayecto</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TATipoSubTrayecto> ObtenerTiposSubTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, edita o elimina un tipo de subtrayecto
        /// </summary>
        /// <param name="subTrayecto">Objeto tipo de subtrayecto</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTiposSubTrayecto(TATipoSubTrayecto subTrayecto);

        /// <summary>
        /// Obtiene formas de pago asignadas y sin asignar para un servicio
        /// </summary>
        /// <param name="IdServicio">Identificador del servicio</param>
        /// <returns>Objeto con las formas de pago asignadas y dispobibles</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAFormaPagoServicio ObtenerFormaPago(int IdServicio);

        /// <summary>
        /// Obtiene los impuestos de un servicio
        /// </summary>
        /// <param name="IdServicio">Identificador servicio</param>
        /// <returns>Colección</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAServicioImpuestosDC ObtenerImpuestosPorServicio(int IdServicio);

        /// <summary>
        /// Obtiene las listas de precio de la aplicación
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de listas de precio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TAListaPrecioDC> ObtenerListaPrecio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio);

        /// <summary>
        /// Retorna
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAListaPrecioDC> ObtenerListasPrecio();

        /// <summary>
        /// Adiciona, edita o elimina una lista de precio
        /// </summary>
        /// <param name="listaPrecio">Objeto lista de precio</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ActualizarListaPrecio(TAListaPrecioDC listaPrecio);

        /// <summary>
        /// Obtiene los tipos de moneda de la aplicación
        /// </summary>
        /// <returns>Objeto Lista Moneda</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAMonedaDC> ObtenerTiposMoneda();

        /// <summary>
        /// Obtiene una lista con los estados del usuario
        /// </summary>
        /// <returns>Objeto lista estados</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<EstadoDC> ObtenerEstadoActivoInactivo();

        /// <summary>
        /// Adiciona, edita o elimina un servicio de una lista de precio
        /// </summary>
        /// <param name="listaPrecioServicio">Objeto lista de precio servicio</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarListaPrecioServicio(TAListaPrecioServicio listaPrecioServicio);

        /// <summary>
        /// Retorna las unidades de negocio
        /// </summary>
        /// <returns>Objeto Unidad de negocio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAUnidadNegocio> ObtenerUnidadNegocio();

        /// <summary>
        /// Obtiene los tipos de valor adicional
        /// </summary>
        /// <returns>Colección con los valores adicionales</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAValorAdicionalValorDC> ObtenerTiposValorAdicionalServicio(int idServicio);

        /// <summary>
        /// Obtiene los tipos de impuestos de la aplicación
        /// </summary>
        /// <returns>Lista con los impuestos de la aplicación</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<TAImpuestosDC> ObtenerImpuestos();

        /// <summary>
        /// Consultar todos los servicios y sus impuestos
        /// </summary>
        /// <returns>Colección con la información de los servicios y sus impuestos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAServicioImpuestosDC ObtenerServiciosImpuestos(int idServicio);

        /// <summary>
        /// Obtiene los tipos de impuesto
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de impuesto</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TAImpuestosDC> ObtenerTiposImpuesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, edita o elimina un tipo de impuesto
        /// </summary>
        /// <param name="tipoImpuesto">Objeto Tipo Impuesto</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTipoImpuesto(TAImpuestosDC tipoImpuesto);

        /// <summary>
        /// Obtien las cuentas externas
        /// </summary>
        /// <returns>Objeto Cuenta Externa</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TACuentaExternaDC> ObtenerCuentaExterna();

        /// <summary>
        /// Obtiene los Operadores Postales de la Aplicación
        /// </summary>
        /// <returns>Colección con los operadores postales</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAOperadorPostalDC> ObtenerOperadoresPostales();

        /// <summary>
        /// Obtiene la zonas de la aplicación
        /// </summary>
        /// <returns>Colección con las zonas de la aplicación</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PAZonaDC> ObtenerZonas();

        /// <summary>
        /// Obtiene los tipos de empaque de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de empaque</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATipoEmpaque> ObtenerTiposEmpaqueTotal();

        /// <summary>
        /// Obtiene precio internacional
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de precio internacional</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TAServicioInternacionalPrecioDC> ObtenerZonasPorOperadorPostal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio);

        /// <summary>
        /// Guarda los cambios realizados en el Usercontrol de Tarifa Internacional
        /// </summary>
        /// <param name="consolidadoTarifaInternacional">Objeto con los cambios realizados</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaInternacional(TATarifaInternacionalDC consolidadoTarifaInternacional);

        /// <summary>
        /// Obtiene los pesos mínimo y máximo de un servicio
        /// </summary>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <returns>Objeto con los pesos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAServicioPesoDC ObtenerServicioPeso(int idServicio);

        /// <summary>
        /// Retorna los parámetros de lista precio servicio
        /// </summary>
        /// <param name="listaPrecioServicio">Objeto listaprecioservicio</param>
        /// <returns>Retorna parámetos listaprecioservicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAListaPrecioServicioParametrosDC ObtenerParametrosListaPrecioServicio(TAListaPrecioServicioParametrosDC listaPrecioServicio);

        /// <summary>
        /// Obtiene los datos básicos del servicio
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns>Objeto Servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAServicioDC ObtenerDatosServicio(int idServicio);

        /// <summary>
        /// Obtiene Precio trayecto Rango
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Precio trayecto rango</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TAPrecioTrayectoDC> ObtenerPrecioTrayectoSubTrayectoRango(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio);

        /// <summary>
        /// Obtiene Precio trayecto Rango
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Precio trayecto rango</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TAPrecioTrayectoDC> ObtenerPrecioTrayectoSubTrayectoRangoRapCarContraPago(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio);

        /// <summary>
        /// Obtiene los trayectos y subtrayectos de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de trayectos y subtrayectos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATrayectoSubTrayectoDC> ObtenerTrayectosSubtrayectos();

        /// <summary>
        /// Guarda los cambios realizados en RapiCarga
        /// </summary>
        /// <param name="consolidadoCambios">Objeto Consolidado de cambios</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiCarga(TATarifaRapiCargaDC tarifaRapiCarga);

        /// <summary>
        /// Guarda los cambios realizados en RapiCarga Contra Pago
        /// </summary>
        /// <param name="consolidadoCambios">Objeto Consolidado de cambios</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiCargaContraPago(TATarifaRapiCargaContraPagoDC tarifaRapiCargaContraPago);

        /// <summary>
        /// Obtiene los tipos de valor y sus campos para el servicio de giros
        /// </summary>
        /// <param name="idServicio">ID del servicio</param>
        /// <returns>Lista con todos los tipos de valor  con sus campos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAValorAdicional> ObtenerTipoValorAdicionalConCamposGIROS();

        /// <summary>
        /// Obtiene los requisitos de servicio de la aplicacion
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TARequisitoServicioDC> ObtenerDatosRequisitoServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, actualiza o elimina un requisito al servicio
        /// </summary>
        /// <param name="requisito"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarRequisitoServicio(TARequisitoServicioDC requisito);

        /// <summary>
        /// Obtiene de precio rango para el servicio de giros
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioRangoImpuestosDC ObtenerPrecioRangoImpuestosGiro();

        /// <summary>
        /// Metodo encargado de devolver el id de la lista de precios vigente
        /// </summary>
        /// <returns>int con el id de la lista de precio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerIdListaPrecioVigente();

        /// <summary>
        /// Metodo utilizado para conocer la lista de precios para determinado cliente credito
        /// </summary>
        /// <param name="IdClienteCredito"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerIdListaPrecioClienteCredito(int IdClienteCredito);


        /// <summary>
        /// Retorna los valores adicionales que son de tipo embalaje para cliente credito
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<TAValorAdicional> ConsultarValoresAdicionalesEmbalajeClienteCredito(int idClienteCredito);

        /// <summary>
        /// Obtiene los trayectos
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Trayectos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos.TATrayectoDC> ObtenerTrayectos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Guarda trayectos
        /// </summary>
        /// <param name="consolidadoTrayectos">Colección de trayectos</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTrayectos(ObservableCollection<TATrayectoDC> consolidadoTrayectos);

        /// <summary>
        /// Obtiene los tipos de trayecto de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de trayecto</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATipoTrayecto> ObtenerTiposTrayectosGenerales();

        /// <summary>
        /// Obtiene los tipos de subtrayectos de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de subtrayecto</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATipoSubTrayecto> ObtenerTiposSubTrayectoGenerales();

        /// <summary>
        /// metodo que obtiene las formas de pago posibles
        /// </summary>
        /// <returns>lista con las formas de pago de tipo TAFormaPago </returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAFormaPago> ObtenerFormasPago(bool aplicaFactura);

        /// <summary>
        /// Valida si existe un trayecto para una ciudad de origen y un servicio
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ValidarServicioTrayectoOrigen(string idLocalidadOrigen, int idServicio);

        /// <summary>
        /// Obtiene los centros de correspondencia de una lista de precio servicio
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <returns>Colección centros de correspondencia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios.TAServicioCentroDeCorrespondenciaDC> ObtenerCentrosDeCorrespondencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio);

        /// <summary>
        /// Guarda Tarifa
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaCentroDeCorrespondencia(TATarifaCentroDeCorrespondenciaDC tarifaCentroCorrespondencia);

        /// <summary>
        /// Obtiene trámites
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <returns>Colección trámites</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios.TAServicioTramiteDC> ObtenerTramites(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio);

        /// <summary>
        /// Guarda Tarifa Trámites
        /// </summary>
        /// <param name="tarifaTramites">Objeto Tarifa</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaTramites(TATarifaTramitesDC tarifaTramites);

        /// <summary>
        /// Obtiene los tipos de trámite
        /// </summary>
        /// <returns>Colección</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATipoTramite> ObtenerTiposTramitesGeneral();

        /// <summary>
        /// Metodo para obtener los servicios de una lista de precios
        /// </summary>
        /// <param name="listaPrecios"></param>
        /// <returns> lista de datos tipos servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAServicioDC> ObtenerServiciosporLista(int listaPrecios);

        /// <summary>
        /// Obtiene rapi promocional
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <returns>Colección rapi promocional</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios.TAServicioRapiPromocionalDC> ObtenerRapiPromocional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio);

        /// <summary>
        /// Guarda Tarifa rapi promicional
        /// </summary>
        /// <param name="tarifaTramites">Objeto Tarifa rapi promocional</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiPromocional(TATarifaRapiPromocionalDC tarifaRapiPromocional);

        /// <summary>
        /// Obtiene excepciones de precio trayecto subtrayecto
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>excepción trayecto subtrayecto</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATrayectoExcepcionDC> ObtenerExcepcionTrayectoSubTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idPrecioTrayecto);

        /// <summary>
        /// Metodo para guardar los cambios de una excepción a un trayecto subtrayecto
        /// </summary>
        /// <param name="excepcion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarCambiosExcepcionTrayectoSubTrayecto(TATrayectoExcepcionDC excepcion);

        /// <summary>
        /// Obtiene los datos de precio trayecto subtrayecto de mensajería
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATrayectoMensajeriaDC> ObtenerTiposTrayectoMensajeria(int idListaPrecio, int idServicio);

        /// <summary>
        /// Adiciona, edita o elimina un trayeco de mensajería
        /// </summary>
        /// <param name="trayectoMensajeria">Objeto trayecto mensajería</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTrayectoMensajeria(ObservableCollection<TATrayectoMensajeriaDC> trayectoMensajeria);

        /// <summary>
        /// Obtiene los trayectos que tienen asignados el subtrayecto kilo adicional
        /// </summary>
        /// <returns>Colección</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATipoTrayecto> ObtenerTiposTrayectoGeneralMensajeria();

        /// <summary>
        /// Obtiene el valor de los tipos de valor adicional para una lista de precio servicio
        /// </summary>
        /// <param name="idListaPrecio">Identificador Lista de Precio</param>
        /// <param name="idServicio">Identificador servicio</param>
        /// <returns>Colección con los valores adicionales</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAValorAdicionalValorDC> ObtenerValoresAdicionalesServicio(int idListaPrecio, int idServicio);

        /// <summary>
        /// Guarda el valor de un tipo adicional
        /// </summary>
        /// <param name="valorAdicional">Objeto Valor adicional</param>
        /// <param name="idListaPrecio">Identificador Lista de Precio</param>
        /// <param name="idServicio">Identificador servicio</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTipoValorAdicionalServicio(ObservableCollection<TAValorAdicionalValorDC> valorAdicional, int idListaPrecio, int idServicio);

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaMensajeria(TATarifaMensajeriaDC tarifaMensajeria);

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa rapi AM</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiAm(TATarifaMensajeriaDC tarifaRapiAm);

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa rapi hoy</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiHoy(TATarifaMensajeriaDC tarifaRapiHoy);

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa rapi personalizado</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiPersonalizado(TATarifaMensajeriaDC tarifaRapiPersonalizado);

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa rapi envíos contrapago</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiEnviosContraPago(TATarifaRapiEnviosContraPagoDC tarifaRapiEnvio);

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaNotificaciones(TATarifaNotificacionesDC tarifaNotificaciones);

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaCargaExpress(TATarifaMensajeriaDC tarifaCargaExpress);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaCargaAerea(TATarifaMensajeriaDC tarifaCargaExpress);

        /// <summary>
        /// Guardar tarifa Rapi Tulas
        /// </summary>
        /// <param name="tarifa"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiTulas(TATarifaMensajeriaDC tarifa);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiRadicado(TATarifaMensajeriaDC tarifa);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiValoresMsj(TATarifaMensajeriaDC tarifa);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiValoresCarga(TATarifaMensajeriaDC tarifa);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiCargaConsolidado(TATarifaMensajeriaDC tarifa);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaRapiValijas(TATarifaMensajeriaDC tarifa);

        /// <summary>
        /// Obtiene de precio rango
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAPrecioRangoDC> ObtenerPrecioRango(int idServicio, int idListaPrecio);

        /// <summary>
        /// Obtiene las excepciones por ciudad de origen
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAPrecioExcepcionDC> ObtenerExcepcionesPorOrigen(int idServicio, int idListaPrecio);

        /// <summary>
        /// Guarda los cambios realizados de los precios excepciones
        /// </summary>
        /// <param name="preciosExcepciones">Consolidado de cambios</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarExcepcionesPorOrigen(ObservableCollection<TAPrecioExcepcionDC> consolidadoCambios);

        /// <summary>
        /// Metodo para obtener los servicios por excepcion
        /// </summary>
        /// <param name="excepcion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATrayectoExcepcionServicioDC> ObtenerServiciosPorExcepcion(TATrayectoExcepcionDC excepcion);

        /// <summary>
        /// Obtiene el valor peso declarado
        /// </summary>
        /// <returns>Colección con los valores peso declarados</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAValorPesoDeclaradoDC> ObtenerValorPesoDeclarado(int idListaPrecio);

        /// <summary>
        /// Adiciona, edita o elimina un valores peso declarado
        /// </summary>
        /// <param name="consolidadoCambios">Colección valor peso declarado</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarValorPesoDeclarado(ObservableCollection<TAValorPesoDeclaradoDC> consolidadoCambios);

        /// <summary>
        /// Obtiene los tipos de cuenta externa
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de cuenta externa</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TACuentaExternaDC> ObtenerCuentaExternaFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, edita o elimina una cuenta externa
        /// </summary>
        /// <param name="cuentaExterna">Objeto cuenta externa</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizaCuentaExterna(TACuentaExternaDC cuentaExterna);

        /// <summary>
        /// Obtiene formas de pago asignadas a un servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAFormaPagoServicio ObtenerFormasPagoAsignadaAServicio(int idServicio);

        /// <summary>
        /// Obtiene todas las formas de pago con los servicios que las tienen incluidas
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<TAFormaPago> ObtenerFormasPagoConServicios();

        /// <summary>
        /// Retorna las formas de pago que aplican para un cliente crédito
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAFormaPago> ObtenerFormasPagoClienteCredito();

        /// <summary>
        /// Obtiene lista de tipos de destino
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATipoDestino> ObtenerTiposDestino();

        /// <summary>
        /// Retorna los requisitos para un servicio solicitado
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<TARequisitoServicioDC> ObtenerRequisitosServicio(int idServicio);

        /// <summary>
        /// Guarda Tarifa del servicio giros
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTarifaGiros(TATarifaGirosDC tarifaGiros);

        #region Lista Precios

        /// <summary>
        /// Consultar los precios de mensajería expresa
        /// </summary>
        /// <param name="idListaPrecios">Identificador de la lista de precios</param>
        /// <returns>Precios de mensajería expresa</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajariaExpresaDC ObtenerListaPreciosMensajeriaExpresa(int idListaPrecios);

        #endregion Lista Precios

        /// <summary>
        /// Obtiene los precios de los trayectos por servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAPrecioTrayectoDC> ObtenerPrecioTraySubTrayectoRango(int idListaPrecio);

        /// <summary>
        /// Obtiene el porcentaje de Recargo
        /// </summary>
        /// <returns>double PorcentajedeRecargo</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        double ObtenerPorcentajeRecargo();

        /// <summary>
        /// Obtiene el Valor regiatrado por defecto
        /// del dolar cuando no alla red para consultarlo
        /// </summary>
        /// <returns>valor del dolar decimal</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        decimal ObtenerValorDolarSinRed();

        /// <summary>
        /// Obtiene los precios de tipo por tipo de entrega por lista de precios
        /// </summary>
        /// <returns>lista de precios de tipo entrega por lista de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<TAPrecioTipoEntrega> ObtenerPrecioTipoEntrega(int idServicio, int idListaPrecio);

        /// <summary>
        /// Guarda los cambios realizados de los precios por tipo de entrega
        /// </summary>
        /// <param name="preciosExcepciones">Consolidado de cambios</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarPrecioTipoEntrega(ObservableCollection<TAPrecioTipoEntrega> consolidadoCambios);

        /// <summary>
        /// Adiciona, edita o elimina una tipo entrega
        /// </summary>
        /// <param name="cuentaExterna">Objeto tipo entrega</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTipoEntrega(TATipoEntrega tipoEntrega);

        /// <summary>
        /// Obtiene los tipos de entrega de mensajeria
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de entrega mensajeria</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TATipoEntrega> ObtenerTipoEntregaFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Obtien los tipos de entrega
        /// </summary>
        /// <returns>Objeto Tipo entrega</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TATipoEntrega> ObtenerTipoEntrega();

        /// <summary>
        /// Obtiene los servicios parametrizados de Novasoft
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAServiciosFormaPagoDC> ObtenerServiciosFormasPagoNova();

        /// <summary>
        /// Actualizacion(Adicion, Eliminacion, Modificacion) Servicios Formas Pago Novasoft
        /// </summary>
        /// <param name="obj"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizacionRegistrosParametrizacionServicioFormaPagoNova(CAServiciosFormaPagoDC obj);


          #region ConsultasAppCliente

        /// <summary>
        /// Obtiene El valor comercial dependiento del peso
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        decimal ConsultarValorComercialPeso(int peso);

         /// <summary>
        /// Consulta los servicios con los pesos minimos y maximos 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        List<TAServicioPesoDC> ConsultarServiciosPesosMinimoxMaximos();
        #endregion




        #region Calculo precio credito

        /// <summary>
        /// Retorna el valor de mensajeria credito
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC ObtenerPrecioMensajeriaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega );

        /// <summary>
        /// Calcula precio rapicarga, el calculo del precio se realiza de acuerdo al peso ingresado y los rangos configurados
        /// Si el peso ingresado esta en un valor intermedio se aplica la siguiente formula
        /// valor=(valorRango * pesoRangoFinal) +(kilosAdicionales * valorRango)
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Objeto precio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioCargaDC ObtenerPrecioCargaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega );

        #endregion


    }
}