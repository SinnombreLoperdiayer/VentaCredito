using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Adicionales;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.ListaPrecios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos;
using CO.Servidor.Servicios.Contratos;
using CO.Servidor.Tarifas;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Novasoft;

namespace CO.Servidor.Servicios.Implementacion.Tarifas
{
    /// <summary>
    /// Clase para los servicios de administración de Tarifas
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class TATarifasSvc : ITATarifasSvc
    {
        #region Constructor

        public TATarifasSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion Constructor

        #region Configuración Tarifas Común

        /// <summary>
        /// Obtiene los tipos de envío que están en la base de datos
        /// </summary>
        /// <returns>Tipos de envío</returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATipoEnvio> ObtenerTiposEnvio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATipoEnvio>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerTiposEnvio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Retorna los tipos de envíos con los servicios asociados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TATipoEnvio> ObtenerTiposDeEnvio()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTiposDeEnvio();
        }

        /// <summary>
        /// Obtiene los tipos de trayecto de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de trayecto</returns>
        public IEnumerable<TATipoTrayecto> ObtenerTiposTrayectosGenerales()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTiposTrayectosGenerales();
        }

        /// <summary>
        /// Obtiene los tipos de subtrayectos de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de subtrayecto</returns>
        public IEnumerable<TATipoSubTrayecto> ObtenerTiposSubTrayectoGenerales()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTiposSubTrayectoGenerales();
        }

        /// <summary>
        /// Adicionar, editar o eliminar un tipo de envío
        /// </summary>
        /// <param name="tipoEnvio">Objeto tipo de envío</param>
        public void ActualizarTipoEnvio(TATipoEnvio tipoEnvio)
        {
            TAAdministradorTarifas.Instancia.ActualizarTipoEnvio(tipoEnvio);
        }

        /// <summary>
        /// Retorna una lista con los tipos de envio
        /// </summary>
        /// <returns></returns>
        public List<TATipoEnvio> ObtenerTipoEnvios()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTipoEnvios();
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAMonedaDC> ObtenerMoneda(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAMonedaDC>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerMoneda(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, edita o elimina moneda
        /// </summary>
        /// <param name="moneda">Objeto moneda</param>
        public void ActualizarMoneda(TAMonedaDC moneda)
        {
            TAAdministradorTarifas.Instancia.ActualizarMoneda(moneda);
        }

        /// <summary>
        /// Obtiene los tipos de empaque almacenados en la DB
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de empaque</returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATipoEmpaque> ObtenerTiposEmpaque(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATipoEmpaque>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerTiposEmpaque(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, edita o elimina tipos de empaque
        /// </summary>
        /// <param name="tipoEmpaque"></param>
        public void ActualizarTiposEmpaque(TATipoEmpaque tipoEmpaque)
        {
            TAAdministradorTarifas.Instancia.ActualizarTiposEmpaque(tipoEmpaque);
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATipoTramite> ObtenerTiposTramite(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATipoTramite>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerTiposTramite(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, edita o elimina un tipo de trámite
        /// </summary>
        /// <param name="tipoTramite">Objeto tipo trámite</param>
        public void ActualizarTiposTramite(TATipoTramite tipoTramite)
        {
            TAAdministradorTarifas.Instancia.ActualizarTiposTramite(tipoTramite);
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAValorAdicional> ObtenerValorAdicional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAValorAdicional>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerValorAdicional(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Retorna los valores adicionales que son de tipo embalaje
        /// </summary>
        /// <returns></returns>
        public List<TAValorAdicional> ConsultarValoresAdicionalesEmbalaje()
        {
            return TAAdministradorTarifas.Instancia.ConsultarValoresAdicionalesEmbalaje();
        }

        /// <summary>
        /// Retorna los valores adicionales que son de tipo embalaje para cliente credito
        /// </summary>
        /// <returns></returns>
        public List<TAValorAdicional> ConsultarValoresAdicionalesEmbalajeClienteCredito(int idListaPreciosClienteCredito)
        {
            return TAAdministradorTarifas.Instancia.ConsultarValoresAdicionalesEmbalajeClienteCredito(idListaPreciosClienteCredito);
        }

        /// <summary>
        /// Adiciona, edita o elimina un valor adicional
        /// </summary>
        /// <param name="valorAdicional"></param>
        public void ActualizarValorAdicional(TAValorAdicional valorAdicional)
        {
            TAAdministradorTarifas.Instancia.ActualizarValorAdicional(valorAdicional);
        }

        /// <summary>
        /// Obtiene la lista de valor adicional de la DB
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAServicioDC> ObtenerServicios()
        {
            return TAAdministradorTarifas.Instancia.ObtenerServicios();
        }

        /// <summary>
        /// Obtiene la lista de servicios por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Id de la unidad de negocio</param>
        /// <returns>Lista de Servicios de la unidad de negocio</returns>
        public IList<TAServicioDC> ObtenerServiciosUnidadNegocio(string IdUnidadNegocio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerServiciosUnidadNegocio(IdUnidadNegocio);
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATipoTrayecto> ObtenerTiposTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATipoTrayecto>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerTiposTrayecto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, edita o elimina un tipo de trayecto
        /// </summary>
        /// <param name="tipoTrayecto">Objeto tipo trayecto</param>
        public void ActualizarTiposTrayecto(TATipoTrayecto tipoTrayecto)
        {
            TAAdministradorTarifas.Instancia.ActualizarTiposTrayecto(tipoTrayecto);
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATipoSubTrayecto> ObtenerTiposSubTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TATipoSubTrayecto>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerTiposSubTrayecto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, edita o elimina un tipo de subtrayecto
        /// </summary>
        /// <param name="subTrayecto">Objeto tipo de subtrayecto</param>
        public void ActualizarTiposSubTrayecto(TATipoSubTrayecto subTrayecto)
        {
            TAAdministradorTarifas.Instancia.ActualizarTiposSubTrayecto(subTrayecto);
        }

        /// <summary>
        /// Obtiene formas de pago asignadas y sin asignar para un servicio
        /// </summary>
        /// <param name="IdServicio">Identificador del servicio</param>
        /// <returns>Objeto con las formas de pago asignadas y dispobibles</returns>
        public TAFormaPagoServicio ObtenerFormaPago(int IdServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerFormaPago(IdServicio);
        }

        /// <summary>
        /// Obtiene los impuestos de un servicio
        /// </summary>
        /// <param name="IdServicio">Identificador servicio</param>
        /// <returns>Colección</returns>
        public TAServicioImpuestosDC ObtenerImpuestosPorServicio(int IdServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerImpuestosPorServicio(IdServicio);
        }

        /// <summary>
        /// Valida si existe un trayecto para una ciudad de origen y un servicio
        /// </summary>
        public void ValidarServicioTrayectoOrigen(string idLocalidadOrigen, int idServicio)
        {
            TAAdministradorTarifas.Instancia.ValidarServicioTrayectoOrigen(idLocalidadOrigen, idServicio);
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAListaPrecioDC> ObtenerListaPrecio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAListaPrecioDC>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerListaPrecio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Retorna las listas de precio
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAListaPrecioDC> ObtenerListasPrecio()
        {
            return TAAdministradorTarifas.Instancia.ObtenerListasPrecio();
        }

        /// <summary>
        /// Adiciona, edita o elimina una lista de precio
        /// </summary>
        /// <param name="listaPrecio">Objeto lista de precio</param>
        public int ActualizarListaPrecio(TAListaPrecioDC listaPrecio)
        {
            return TAAdministradorTarifas.Instancia.ActualizarListaPrecio(listaPrecio);
        }

        /// <summary>
        /// Obtiene los tipos de moneda de la aplicación
        /// </summary>
        /// <returns>Objeto Lista Moneda</returns>
        public IEnumerable<TAMonedaDC> ObtenerTiposMoneda()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTiposMoneda();
        }

        /// <summary>
        /// Obtiene una lista con los estados del usuario
        /// </summary>
        /// <returns>Objeto lista estados</returns>
        public IEnumerable<EstadoDC> ObtenerEstadoActivoInactivo()
        {
            return TAAdministradorTarifas.Instancia.ObtenerEstadoActivoInactivo();
        }

        /// <summary>
        /// Adiciona, edita o elimina un servicio de una lista de precio
        /// </summary>
        /// <param name="listaPrecioServicio">Objeto lista de precio servicio</param>
        public void ActualizarListaPrecioServicio(TAListaPrecioServicio listaPrecioServicio)
        {
            TAAdministradorTarifas.Instancia.ActualizarListaPrecioServicio(listaPrecioServicio);
        }

        /// <summary>
        /// Retorna las unidades de negocio
        /// </summary>
        /// <returns>Objeto Unidad de negocio</returns>
        public IEnumerable<TAUnidadNegocio> ObtenerUnidadNegocio()
        {
            return TAAdministradorTarifas.Instancia.ObtenerUnidadNegocio();
        }

        /// <summary>
        /// Obtiene los tipos de impuestos de la aplicación
        /// </summary>
        /// <returns>Lista con los impuestos de la aplicación</returns>
        public IList<TAImpuestosDC> ObtenerImpuestos()
        {
            return TAAdministradorTarifas.Instancia.ObtenerImpuestos();
        }

        /// <summary>
        /// Consultar todos los servicios y sus impuestos
        /// </summary>
        /// <returns>Colección con la información de los servicios y sus impuestos</returns>
        public TAServicioImpuestosDC ObtenerServiciosImpuestos(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerServiciosImpuestos(idServicio);
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAImpuestosDC> ObtenerTiposImpuesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;

            GenericoConsultasFramework<TAImpuestosDC> resultdo = new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAImpuestosDC>();
            resultdo.Lista = TAAdministradorTarifas.Instancia.ObtenerTiposImpuesto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
            resultdo.TotalRegistros = totalRegistros;

            return resultdo;
        }

        /// <summary>
        /// Adiciona, edita o elimina un tipo de impuesto
        /// </summary>
        /// <param name="tipoImpuesto">Objeto Tipo Impuesto</param>
        public void ActualizarTipoImpuesto(TAImpuestosDC tipoImpuesto)
        {
            TAAdministradorTarifas.Instancia.ActualizarTipoImpuesto(tipoImpuesto);
        }

        /// <summary>
        /// Obtien las cuentas externas
        /// </summary>
        /// <returns>Objeto Cuenta Externa</returns>
        public IEnumerable<TACuentaExternaDC> ObtenerCuentaExterna()
        {
            return TAAdministradorTarifas.Instancia.ObtenerCuentaExterna();
        }

        /// <summary>
        /// Obtiene los Operadores Postales de la Aplicación
        /// </summary>
        /// <returns>Colección con los operadores postales</returns>
        public IEnumerable<TAOperadorPostalDC> ObtenerOperadoresPostales()
        {
            return TAAdministradorTarifas.Instancia.ObtenerOperadoresPostales();
        }

        /// <summary>
        /// Obtiene la zonas de la aplicación
        /// </summary>
        /// <returns>Colección con las zonas de la aplicación</returns>
        public IEnumerable<PAZonaDC> ObtenerZonas()
        {
            return TAAdministradorTarifas.Instancia.ObtenerZonas();
        }

        /// <summary>
        /// Obtiene los tipos de empaque de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de empaque</returns>
        public IEnumerable<TATipoEmpaque> ObtenerTiposEmpaqueTotal()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTiposEmpaqueTotal();
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAPrecioTrayectoDC> ObtenerPrecioTrayectoSubTrayectoRango(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAPrecioTrayectoDC>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerPrecioTrayectoSubTrayectoRango(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio),
                TotalRegistros = totalRegistros
            };
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAPrecioTrayectoDC> ObtenerPrecioTrayectoSubTrayectoRangoRapCarContraPago(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAPrecioTrayectoDC>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerPrecioTrayectoSubTrayectoRangoRapCarContraPago(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene los trayectos y subtrayectos de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de trayectos y subtrayectos</returns>
        public IEnumerable<TATrayectoSubTrayectoDC> ObtenerTrayectosSubtrayectos()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTrayectosSubtrayectos();
        }

        /// <summary>
        /// Obtiene los precios de los trayectos por servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <returns></returns>
        public IEnumerable<TAPrecioTrayectoDC> ObtenerPrecioTraySubTrayectoRango(int idListaPrecio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerPrecioTraySubTrayectoRango(idListaPrecio);
        }

        /// <summary>
        /// Obtiene de precio rango
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public IEnumerable<TAPrecioRangoDC> ObtenerPrecioRango(int idServicio, int idListaPrecio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerPrecioRango(idServicio, idListaPrecio);
        }

        /// <summary>
        /// Obtiene de precio rango para el servicio de giros
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public TAPrecioRangoImpuestosDC ObtenerPrecioRangoImpuestosGiro()
        {
            return TAAdministradorTarifas.Instancia.ObtenerPrecioRangoImpuestosGiro();
        }

        /// <summary>
        /// metodo que obtiene las formas de pago posibles
        /// </summary>
        /// <returns>lista con las formas de pago de tipo TAFormaPago </returns>
        public IEnumerable<TAFormaPago> ObtenerFormasPago(bool aplicaFactura)
        {
            return TAAdministradorTarifas.Instancia.ObtenerFormasPago(aplicaFactura);
        }

        /// <summary>
        /// Obtiene los tipos de trámite
        /// </summary>
        /// <returns>Colección</returns>
        public IEnumerable<TATipoTramite> ObtenerTiposTramitesGeneral()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTiposTramitesGeneral();
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TACuentaExternaDC> ObtenerCuentaExternaFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TACuentaExternaDC>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerCuentaExternaFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, edita o elimina una cuenta externa
        /// </summary>
        /// <param name="cuentaExterna">Objeto cuenta externa</param>
        public void ActualizaCuentaExterna(TACuentaExternaDC cuentaExterna)
        {
            TAAdministradorTarifas.Instancia.ActualizaCuentaExterna(cuentaExterna);
        }

        /// <summary>
        /// Obtiene formas de pago asignadas a un servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public TAFormaPagoServicio ObtenerFormasPagoAsignadaAServicio(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerFormasPagoAsignadaAServicio(idServicio);
        }

        /// <summary>
        /// Obtiene todas las formas de pago con los servicios que las tienen incluidas
        /// </summary>
        /// <returns></returns>
        public List<TAFormaPago> ObtenerFormasPagoConServicios()
        {
            return TAAdministradorTarifas.Instancia.ObtenerFormasPagoConServicios();
        }

        /// <summary>
        /// Retorna las formas de pago que aplican para un cliente crédito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAFormaPago> ObtenerFormasPagoClienteCredito()
        {
            return TAAdministradorTarifas.Instancia.ObtenerFormasPagoClienteCredito();
        }

        /// <summary>
        /// Obtiene lista de tipos de destino
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TATipoDestino> ObtenerTiposDestino()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTiposDestino();
        }

        /// <summary>
        /// Obtiene los tipos de valor adicional
        /// </summary>
        /// <returns>Colección con los valores adicionales</returns>
        public IEnumerable<TAValorAdicionalValorDC> ObtenerTiposValorAdicionalServicio(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerTiposValorAdicionalServicio(idServicio);
        }

        #endregion Configuración Tarifas Común

        #region Configuración Internacional

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAServicioInternacionalPrecioDC> ObtenerZonasPorOperadorPostal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAServicioInternacionalPrecioDC>()
            {
                Lista = TAAdministradorInternacional.Instancia.ObtenerZonasPorOperadorPostal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Guarda los cambios realizados en el Usercontrol de Tarifa Internacional
        /// </summary>
        /// <param name="consolidadoTarifaInternacional">Objeto con los cambios realizados</param>
        public void GuardarTarifaInternacional(TATarifaInternacionalDC consolidadoTarifaInternacional)
        {
            TAAdministradorInternacional.Instancia.GuardarTarifaInternacional(consolidadoTarifaInternacional);
        }

        /// <summary>
        /// Obtiene los pesos mínimo y máximo de un servicio
        /// </summary>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <returns>Objeto con los pesos</returns>
        public TAServicioPesoDC ObtenerServicioPeso(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerServicioPeso(idServicio);
        }

        /// <summary>
        /// Retorna los parámetros de lista precio servicio
        /// </summary>
        /// <param name="listaPrecioServicio">Objeto listaprecioservicio</param>
        /// <returns>Retorna parámetos listaprecioservicio</returns>
        public TAListaPrecioServicioParametrosDC ObtenerParametrosListaPrecioServicio(TAListaPrecioServicioParametrosDC listaPrecioServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerParametrosListaPrecioServicio(listaPrecioServicio);
        }

        /// <summary>
        /// Obtiene los datos básicos del servicio
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns>Objeto Servicio</returns>
        public TAServicioDC ObtenerDatosServicio(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerDatosServicio(idServicio);
        }

        /// <summary>
        /// Obtiene el porcentaje de Recargo
        /// </summary>
        /// <returns>double PorcentajedeRecargo</returns>
        public double ObtenerPorcentajeRecargo()
        {
            return TAAdministradorInternacional.Instancia.ObtenerPorcentajeRecargo();
        }

        /// <summary>
        /// Obtiene el Valor regiatrado por defecto
        /// del dolar cuando no alla red para consultarlo
        /// </summary>
        /// <returns>valor del dolar decimal</returns>
        public decimal ObtenerValorDolarSinRed()
        {
            return TAAdministradorInternacional.Instancia.ObtenerValorDolarSinRed();
        }

        #endregion Configuración Internacional

        #region Configuración Tarifa Rapi Carga

        /// <summary>
        /// Guarda los cambios realizados en RapiCarga
        /// </summary>
        /// <param name="consolidadoCambios">Objeto Consolidado de cambios</param>
        public void GuardarTarifaRapiCarga(TATarifaRapiCargaDC tarifaRapiCarga)
        {
            TAAdministradorTarifas.Instancia.GuadarTarifaRapiCarga(tarifaRapiCarga);
        }

        #endregion Configuración Tarifa Rapi Carga

        #region Configuración Tarifa Rapi Carga Contra Pago

        /// <summary>
        /// Guardar tarifa rapicarga contrapago
        /// </summary>
        /// <param name="tarifaRapiCargaContraPago"></param>
        public void GuardarTarifaRapiCargaContraPago(TATarifaRapiCargaContraPagoDC tarifaRapiCargaContraPago)
        {
            TAAdministradorTarifas.Instancia.GuadarTarifaRapiCargaContraPago(tarifaRapiCargaContraPago);
        }

        #endregion Configuración Tarifa Rapi Carga Contra Pago

        #region Obtener Valores Servicios

        /// <summary>
        /// Obtiene los tipos de valor y sus campos para el servicio de giros
        /// </summary>
        /// <param name="idServicio">ID del servicio</param>
        /// <returns>Lista con todos los tipos de valor  con sus campos</returns>
        public IEnumerable<TAValorAdicional> ObtenerTipoValorAdicionalConCamposGIROS()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTipoValorAdicionalConCamposGIROS();
        }

        #endregion Obtener Valores Servicios

        #region Requisito servicio

        /// <summary>
        /// Obtiene los requisitos de los servicios
        /// </summary>
        /// <returns></returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TARequisitoServicioDC> ObtenerDatosRequisitoServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TARequisitoServicioDC>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerDatosRequisitoServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, edita o elimina un requisito de servicio
        /// </summary>
        /// <param name="requisito">Objeto Tipo RequisitoServicio</param>
        public void ActualizarRequisitoServicio(TARequisitoServicioDC requisito)
        {
            TAAdministradorTarifas.Instancia.ActualizarRequisitoServicio(requisito);
        }

        /// <summary>
        /// Retorna los requisitos para un servicio solicitado
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns></returns>
        public IList<TARequisitoServicioDC> ObtenerRequisitosServicio(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerRequisitosServicio(idServicio);
        }

        #endregion Requisito servicio

        #region Comun

        /// <summary>
        /// Metodo encargado de devolver el id de la lista de precios vigente
        /// </summary>
        /// <returns>int con el id de la lista de precio</returns>
        public int ObtenerIdListaPrecioVigente()
        {
            TAFachadaTarifas fachada = new TAFachadaTarifas();
            return fachada.ObtenerIdListaPrecioVigente();
        }

        /// <summary>
        /// Metodo utilizado para conocer la lista de precios para determinado cliente credito
        /// </summary>
        /// <param name="IdClienteCredito"></param>
        /// <returns></returns>
        public int ObtenerIdListaPrecioClienteCredito(int IdClienteCredito)
        {
            TAFachadaTarifas fachada = new TAFachadaTarifas();
            return fachada.ObtenerIdListaPrecioClienteCredito(IdClienteCredito);
        }

        /// <summary>
        /// Metodo para obtener los servicios de una lista de precios
        /// </summary>
        /// <param name="listaPrecios"></param>
        /// <returns> lista de datos tipos servicio</returns>
        public IEnumerable<TAServicioDC> ObtenerServiciosporLista(int listaPrecios)
        {
            return TAAdministradorTarifas.Instancia.ObtenerServiciosporLista(listaPrecios);
        }

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

        public GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TATipoEntrega> ObtenerTipoEntregaFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.TATipoEntrega>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerTipoEntregaFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, edita o elimina una tipo entrega
        /// </summary>
        /// <param name="cuentaExterna">Objeto tipo entrega</param>
        public void ActualizarTipoEntrega(TATipoEntrega tipoEntrega)
        {
            TAAdministradorTarifas.Instancia.ActualizarTipoEntrega(tipoEntrega);
        }

        /// <summary>
        /// Obtien los tipos de entrega
        /// </summary>
        /// <returns>Objeto Tipo entrega</returns>
        public IEnumerable<TATipoEntrega> ObtenerTipoEntrega()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTipoEntrega();
        }

        #endregion Comun

        #region Trayectos

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos.TATrayectoDC> ObtenerTrayectos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos.TATrayectoDC>()

            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerTrayectos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Guarda trayectos
        /// </summary>
        /// <param name="consolidadoTrayectos">Colección de trayectos</param>
        public void GuardarTrayectos(ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos.TATrayectoDC> consolidadoTrayectos)
        {
            TAAdministradorTarifas.Instancia.GuardarTrayectos(consolidadoTrayectos);
        }

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
        public GenericoConsultasFramework<TATrayectoExcepcionDC> ObtenerExcepcionTrayectoSubTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idPrecioTrayecto)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<TATrayectoExcepcionDC>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerExcepcionTrayectoSubTrayecto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idPrecioTrayecto),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Metodo para guardar los cambios de una excepción a un trayecto subtrayecto
        /// </summary>
        /// <param name="excepcion"></param>
        public void GuardarCambiosExcepcionTrayectoSubTrayecto(TATrayectoExcepcionDC excepcion)
        {
            TAAdministradorTarifas.Instancia.GuardarCambiosExcepcionTrayectoSubTrayecto(excepcion);
        }

        #endregion Trayectos

        #region Centros de Correspondencia

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAServicioCentroDeCorrespondenciaDC> ObtenerCentrosDeCorrespondencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAServicioCentroDeCorrespondenciaDC>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerCentrosDeCorrespondencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Guarda Tarifa
        /// </summary>
        public void GuardarTarifaCentroDeCorrespondencia(TATarifaCentroDeCorrespondenciaDC tarifaCentroCorrespondencia)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaCentroDeCorrespondencia(tarifaCentroCorrespondencia);
        }

        #endregion Centros de Correspondencia

        #region Servicio Giros

        /// <summary>
        /// Guarda Tarifa
        /// </summary>
        public void GuardarTarifaGiros(TATarifaGirosDC tarifaGiros)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaGiros(tarifaGiros);
        }

        #endregion Servicio Giros

        #region Servicio Trámites

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAServicioTramiteDC> ObtenerTramites(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAServicioTramiteDC>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerTramites(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Guarda Tarifa Trámites
        /// </summary>
        /// <param name="tarifaTramites">Objeto Tarifa</param>
        public void GuardarTarifaTramites(TATarifaTramitesDC tarifaTramites)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaTramites(tarifaTramites);
        }

        #endregion Servicio Trámites

        #region Servicio Rapi Promocional

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAServicioRapiPromocionalDC> ObtenerRapiPromocional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idListaPrecio)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<TAServicioRapiPromocionalDC>()
            {
                Lista = TAAdministradorTarifas.Instancia.ObtenerRapiPromocional(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Guarda Tarifa rapi promicional
        /// </summary>
        /// <param name="tarifaTramites">Objeto Tarifa rapi promocional</param>
        public void GuardarTarifaRapiPromocional(TATarifaRapiPromocionalDC tarifaRapiPromocional)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiPromocional(tarifaRapiPromocional);
        }

        #endregion Servicio Rapi Promocional

        #region Servicio Mensajería

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaMensajeria(TATarifaMensajeriaDC tarifaMensajeria)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaMensajeria(tarifaMensajeria);
        }

        #endregion Servicio Mensajería

        #region Servicio Rapi AM

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaRapiAm(TATarifaMensajeriaDC tarifaRapiAm)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiAm(tarifaRapiAm);
        }

        #endregion Servicio Rapi AM

        #region Servicio Rapi Hoy

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaRapiHoy(TATarifaMensajeriaDC tarifaRapiHoy)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiHoy(tarifaRapiHoy);
        }

        #endregion Servicio Rapi Hoy

        #region Servicio Rapi Personalizado

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaRapiPersonalizado(TATarifaMensajeriaDC tarifaRapiPersonalizado)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiPersonalizado(tarifaRapiPersonalizado);
        }

        #endregion Servicio Rapi Personalizado

        #region Servicio Rapi Envíos Contrapago

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa rapi envíos contrapago</param>
        public void GuardarTarifaRapiEnviosContraPago(TATarifaRapiEnviosContraPagoDC tarifaRapiEnvio)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiEnviosContraPago(tarifaRapiEnvio);
        }

        #endregion Servicio Rapi Envíos Contrapago

        #region Servicio Notificaciones

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaNotificaciones(TATarifaNotificacionesDC tarifaNotificaciones)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaNotificaciones(tarifaNotificaciones);
        }

        #endregion Servicio Notificaciones

        #region Servicio Carga Express

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaCargaExpress(TATarifaMensajeriaDC tarifaCargaExpress)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaCargaExpress(tarifaCargaExpress);
        }

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaCargaAerea(TATarifaMensajeriaDC tarifaCargaExpress)
        {
          TAAdministradorTarifas.Instancia.GuardarTarifaCargaAerea(tarifaCargaExpress);
        }

        #endregion Servicio Carga Express

        #region Servicio Carga Express

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaRapiTulas(TATarifaMensajeriaDC tarifa)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiTulas(tarifa);
        }
        #endregion Servicio Carga Express

        public void GuardarTarifaRapiRadicado(TATarifaMensajeriaDC tarifa)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiRadicado(tarifa);
        }

        public void GuardarTarifaRapiValoresMsj(TATarifaMensajeriaDC tarifa)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiValoresMsj(tarifa);
        }

        public void GuardarTarifaRapiValoresCarga(TATarifaMensajeriaDC tarifa)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiValoresCarga(tarifa);
        }

        public void GuardarTarifaRapiCargaConsolidado(TATarifaMensajeriaDC tarifa)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiCargaConsolidado(tarifa);
        }

        public void GuardarTarifaRapiValijas(TATarifaMensajeriaDC tarifa)
        {
            TAAdministradorTarifas.Instancia.GuardarTarifaRapiValijas(tarifa);
        }

        #region Valor Peso Declarado

        /// <summary>
        /// Obtiene el valor peso declarado
        /// </summary>
        /// <returns>Colección con los valores peso declarados</returns>
        public IEnumerable<TAValorPesoDeclaradoDC> ObtenerValorPesoDeclarado(int idListaPrecio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerValorPesoDeclarado(idListaPrecio);
        }

        /// <summary>
        /// Adiciona, edita o elimina un valores peso declarado
        /// </summary>
        /// <param name="consolidadoCambios">Colección valor peso declarado</param>
        public void GuardarValorPesoDeclarado(ObservableCollection<TAValorPesoDeclaradoDC> consolidadoCambios)
        {
            TAAdministradorTarifas.Instancia.GuardarValorPesoDeclarado(consolidadoCambios);
        }

        #endregion Valor Peso Declarado

        #region UserControl Precio Trayecto Mensajería

        /// <summary>
        /// Obtiene los datos de precio trayecto subtrayecto de mensajería
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TATrayectoMensajeriaDC> ObtenerTiposTrayectoMensajeria(int idListaPrecio, int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerTiposTrayectoMensajeria(idListaPrecio, idServicio);
        }

        /// <summary>
        /// Adiciona, edita o elimina un trayeco de mensajería
        /// </summary>
        /// <param name="trayectoMensajeria">Objeto trayecto mensajería</param>
        public void GuardarTrayectoMensajeria(ObservableCollection<TATrayectoMensajeriaDC> trayectoMensajeria)
        {
            TAAdministradorTarifas.Instancia.GuardarTrayectoMensajeria(trayectoMensajeria);
        }

        /// <summary>
        /// Obtiene los trayectos que tienen asignados el subtrayecto kilo adicional
        /// </summary>
        /// <returns>Colección</returns>
        public IEnumerable<TATipoTrayecto> ObtenerTiposTrayectoGeneralMensajeria()
        {
            return TAAdministradorTarifas.Instancia.ObtenerTiposTrayectoGeneralMensajeria();
        }

        #endregion UserControl Precio Trayecto Mensajería

        #region UserControl Servicio Valor Adicional

        /// <summary>
        /// Obtiene el valor de los tipos de valor adicional para una lista de precio servicio
        /// </summary>
        /// <param name="idListaPrecio">Identificador Lista de Precio</param>
        /// <param name="idServicio">Identificador servicio</param>
        /// <returns>Colección con los valores adicionales</returns>
        public IEnumerable<TAValorAdicionalValorDC> ObtenerValoresAdicionalesServicio(int idListaPrecio, int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerValoresAdicionalesServicio(idListaPrecio, idServicio);
        }

        /// <summary>
        /// Guarda el valor de un tipo adicional
        /// </summary>
        /// <param name="valorAdicional">Objeto Valor adicional</param>
        /// <param name="idListaPrecio">Identificador Lista de Precio</param>
        /// <param name="idServicio">Identificador servicio</param>
        public void GuardarTipoValorAdicionalServicio(ObservableCollection<TAValorAdicionalValorDC> valorAdicional, int idListaPrecio, int idServicio)
        {
            TAAdministradorTarifas.Instancia.GuardarTipoValorAdicionalServicio(valorAdicional, idListaPrecio, idServicio);
        }

        #endregion UserControl Servicio Valor Adicional

        #region UserControl Excepciones por Ciudad de Origen

        /// <summary>
        /// Obtiene las excepciones por ciudad de origen
        /// </summary>
        public IEnumerable<TAPrecioExcepcionDC> ObtenerExcepcionesPorOrigen(int idServicio, int idListaPrecio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerExcepcionesPorOrigen(idServicio, idListaPrecio);
        }

        /// <summary>
        /// Guarda los cambios realizados de los precios excepciones
        /// </summary>
        /// <param name="preciosExcepciones">Consolidado de cambios</param>
        public void GuardarExcepcionesPorOrigen(ObservableCollection<TAPrecioExcepcionDC> consolidadoCambios)
        {
            TAAdministradorTarifas.Instancia.GuardarExcepcionesPorOrigen(consolidadoCambios);
        }

        /// <summary>
        /// Metodo para obtener los servicios por excepcion
        /// </summary>
        /// <param name="excepcion"></param>
        /// <returns></returns>
        public IEnumerable<TATrayectoExcepcionServicioDC> ObtenerServiciosPorExcepcion(TATrayectoExcepcionDC excepcion)
        {
            return TAAdministradorTarifas.Instancia.ObtenerServiciosPorExcepcion(excepcion);
        }

        #endregion UserControl Excepciones por Ciudad de Origen

        #region Precio Servicio Tipo Entrega

        /// <summary>
        /// Obtiene los precios de tipo por tipo de entrega por lista de precios
        /// </summary>
        /// <returns>lista de precios de tipo entrega por lista de precios</returns>
        public List<TAPrecioTipoEntrega> ObtenerPrecioTipoEntrega(int idServicio, int idListaPrecio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerPrecioTipoEntrega(idServicio, idListaPrecio);
        }

        /// <summary>
        /// Guarda los cambios realizados de los precios por tipo de entrega
        /// </summary>
        /// <param name="preciosExcepciones">Consolidado de cambios</param>
        public void GuardarPrecioTipoEntrega(ObservableCollection<TAPrecioTipoEntrega> consolidadoCambios)
        {
            TAAdministradorTarifas.Instancia.GuardarPrecioTipoEntrega(consolidadoCambios);
        }

        #endregion Precio Servicio Tipo Entrega

        #region lista precios

        public TAPrecioMensajariaExpresaDC ObtenerListaPreciosMensajeriaExpresa(int idListaPrecios)
        {
            return TAAdministradorTarifasExt.Instancia.ObtenerListaPreciosMesajeriaExpresa(idListaPrecios);
        }

        #endregion lista precios

        #region Consultas
        /// <summary>
        /// Obtiene los servicios parametrizados segun formas de pago y tipos de cuenta Novasoft
        /// </summary>
        /// <returns></returns>
        public List<CAServiciosFormaPagoDC> ObtenerServiciosFormasPagoNova()
        {
            return TAAdministradorTarifas.Instancia.ObtenerServicioFormaPago();
        }
        /// <summary>
        /// Actualizacion(Adicion, Eliminacion, Modificacion) Servicios Formas Pago Novasoft
        /// </summary>
        /// <param name="obj"></param>
        public void ActualizacionRegistrosParametrizacionServicioFormaPagoNova(CAServiciosFormaPagoDC obj)
        {
            TAAdministradorTarifas.Instancia.ActualizacionRegistrosParametrizacionServicioFormaPagoNova(obj);
        }
        #endregion Consultas


        #region ConsultasAppCliente

        /// <summary>
        /// Obtiene El valor comercial dependiento del peso
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public decimal ConsultarValorComercialPeso(int peso)
        {

            return TAAdministradorTarifas.Instancia.ConsultarValorComercialPeso(peso);

        }

        /// <summary>
        /// Consulta los servicios con los pesos minimos y maximos 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<TAServicioPesoDC> ConsultarServiciosPesosMinimoxMaximos()
        {
            return TAAdministradorTarifas.Instancia.ConsultarServiciosPesosMinimoxMaximos();
        }

        #endregion



        #region Calculo precio credito

        /// <summary>
        /// Retorna el valor de mensajeria credito
        /// </summary>
        /// <returns></returns>
        public TAPrecioMensajeriaDC ObtenerPrecioMensajeriaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega)
        {
            return TAAdministradorTarifasCredito.Instancia.ObtenerPrecioMensajeriaCredito(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

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
        public TAPrecioCargaDC ObtenerPrecioCargaCredito(int idServicio, int idListaPrecio,  string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega)
        {
            return TAAdministradorTarifasCredito.Instancia.ObtenerPrecioCargaCredito(idServicio, idListaPrecio,  idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }


        #endregion

    }
}