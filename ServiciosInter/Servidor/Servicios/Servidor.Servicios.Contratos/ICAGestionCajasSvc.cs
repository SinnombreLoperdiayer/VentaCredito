using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Cajas.Transacciones;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Cierres;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Impresion;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ICAGestionCajasSvc
    {
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServiciosTodos(long idRacol);

        /// <summary>
        /// Registrar Transacciones entre RACOL- Agencia - Banco - Casa Matriz.
        /// </summary>
        /// <param name="infoTransaccion">The info transaccion.</param>
        /// <returns>Información de la transacción registrada</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAResultadoRegistroTransaccionDC TransaccionRacolBancoEmpresa(CAOperaRacolBancoEmpresaDC infoTransaccion);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PABanco> ObtenerTodosBancos();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PATipoDocumBancoDC> ObtenerTiposDocumentosBanco();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<CACajaCasaMatrizDC> ObtenerTransaccionesEmpresa(DateTime fechaTransaccion, short idCasaMatriz);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CACajaCasaMatrizDC> ObtenerTransaccionesRACOL(DateTime fechaTransaccion, long idCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAConceptoCajaDC ObtenerDuplaConcepto(int idConcepto);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio);

        /// <summary>
        /// Retorna las cuentas externas del sistema
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CO.Servidor.Servicios.ContratoDatos.Cajas.CACuentaExterna> ObtenerCuentasExternas();

        /// <summary>
        /// retorna los conceptos filtrados por categoria
        /// </summary>
        /// <param name="categoria"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAConceptoCajaDC> ObtenerConceptosCajaCategoria(CAEnumCategoriasConceptoCaja categoria);

        /// <summary>
        /// Consulta los conceptos de caja filtrados, paginas y ordenados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAConceptoCajaDC> ObtenerConceptosCaja(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        /// <summary>
        /// Se aplican cambios realizados sobre un concepto de caja
        /// </summary>
        /// <param name="conceptoCaja"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarConceptoCaja(CAConceptoCajaDC conceptoCaja);

        /// <summary>
        /// Se inserta un concepto de caja nuevo
        /// </summary>
        /// <param name="conceptoCaja"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarConceptoCaja(CAConceptoCajaDC conceptoCaja);

        /// <summary>
        /// Obtener las operaciones de caja de Operación Nacional en una fecha
        /// </summary>
        /// <param name="idCasaMatriz">Identificador único de la casa matriz</param>
        /// <param name="fecha">Fecha en la cual se hace la consulta</param>
        /// <returns>Collección con la información de las operaciones</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<CACajaCasaMatrizDC> ObtenerOperacionesOpn(DateTime fechaTransaccion, short idCasaMatriz);

        /// <summary>
        /// Cerrar las aperturas de caja de Casa Matriz, Operación Naciona, Bancos y Centros de Servicios que el usuario ha hecho
        /// </summary>
        /// <param name="idCasaMatriz">Identificación de la casa matriz sobre la cual se hacen las aperturas</param>
        /// <param name="idCodigoUsuario">Código del usuario que hizo las aperturas</param>
        /// <param name="idRacol">Identificación del RACOL desde donde se hacen las operaciones</param>
        /// <remarks>Las aperturas sobre centros de servicos se hacen sobre la caja 0</remarks>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CerrarCajaGestion(short idCasaMatriz, long idCodigoUsuario, long idRacol);

        /// <summary>
        /// Obtener la información del cierre de las cajas de gestión
        /// </summary>
        /// <param name="idCasaMatriz">Identificador de la casa matriz donde se hace el cierre</param>
        /// <param name="idCodigoUsuario">Identificador del usuairo que hace el cierre</param>
        /// <returns>Colección con la información del cierre</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<CACierreCajaGestionDC> ObtenerCierreCajasGestion(short idCasaMatriz, long idCodigoUsurio);

        /// <summary>
        /// Carga información inicial para la impresión de formato GRI-R10
        /// </summary>
        /// <param name="idCentroServicios">Identificador centro de servicios destino de la transacción</param>
        /// <param name="valor">Valor de la operacion</param>
        /// <param name="bolsaSeguridad">Número de bolsa de seguridad</param>
        /// <param name="numeroGuia">Número de guía interna con la cual se hace el movimiento</param>
        /// <param name="numeroPrecinto">Número del precinto, este puede ser null</param>
        /// <returns>Obejeto con los datos iniciales del formato</returns>
        /// <remarks>El objeto retornado no se llena completamente ya que hay algunos datos que se encuentran solo en el cliente</remarks>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CADatosImpresionGirR10DC ObtenerDatosFormatoGirR10(long idCentroServiciosOrigen, long idCentroServiciosDestino, bool esGestion, decimal valor, string bolsaSeguridad, long numeroGuia, string numeroPrecinto);

        /// <summary>
        /// Obtiene los Datos del usuasrio Nombres  completos caja
        /// y demas
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns>nombre completo de Usuario</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario);

       

        /// <summary>
        /// Obtener los conceptos de Caja de Gestión de de Operación Nacional
        /// </summary>
        /// <returns>Arreglo con los conceptos de caja de una Caja de Gestión de Operación Nacional</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionOpn();

        /// <summary>
        /// Obtener los conceptos de Caja de Gestión de un RACOL
        /// </summary>
        /// <returns>Arreglo con los conceptos de caja de una Caja de Gestión de RACOL</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionRacol();

        /// <summary>
        /// Obtiene la info la base de caja de Operacion Nacional
        /// de acuerdo con su Casa matriz
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns>info de base de caja de OpeNal</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CABaseGestionCajasDC ObtenerBaseCajaOperacionNacional(int idCasaMatriz);

        /// <summary>
        /// Obtiene la info la base de caja de Operacion Nacional
        /// de acuerdo con su Casa matriz
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns>lista de Bases de Caja de las Casas Matriz y Operacion Nal</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CABaseGestionCajasDC> ObtenerBasesDeCajasOperacionNacional();

        /// <summary>
        /// Actualiza, Modifica, Borra la Base de la caja
        /// de Operacional Nacional por su Casa Matriz
        /// </summary>
        /// <param name="infoBaseCajaOpeNal">data de Entrada</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GestionBaseCajaOperacionNacional(CABaseGestionCajasDC infoBaseCajaOpeNal);

        /// <summary>
        /// Obtener los conceptos de Caja de Gestión de Casa Matriz
        /// </summary>
        /// <returns>Arreglo con los conceptos de caja de una Caja de Gestión de Casa Matriz</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionCasaMatriz();

        /// <summary>
        /// Retorna todas las casas matriz activas
        /// </summary>
        /// <returns>Colección con todas las casas matrices</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ARCasaMatrizDC> ObtenerTodasLasCasaMatriz();

        /// <summary>
        /// Obtiene las categorias de los conceptos
        /// </summary>
        /// <returns>lista ordenada de Categorias</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAConceptoCajaCategoriaDC> ObtenerCategoriaConceptosCaja();

              /// <summary>
        /// Ingresa un ajuste a la caja de un punto
        /// </summary>
        /// <param name="ajusteCaja"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAAjustesCajaNovedadesDC IngresarAjusteCajaNovedades(CAAjustesCajaNovedadesDC ajusteCaja);

        /// <summary>
        /// Obtiene los tipos de Observaciones
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Obtiene los tipos de Observaciones</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CATipoObsPuntoAgenciaDC> ObtenerTipoObservacionFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        ///  Adicionar, editar , eliminar tipos de observacion
        /// </summary>
        /// <param name="tipoObservacion">Clase tipoObservacion</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTipoObservacion(CATipoObsPuntoAgenciaDC tipoObservacion);

        /// <summary>
        /// Retorna la lista de tipos de observaciones observacions
        /// </summary>
        /// <returns>tipos de observaciones </returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CATipoObsPuntoAgenciaDC> ObtenerTipoObservacion();

        /// <summary>
        /// Almacena en la base de datos un ajuste de caja asociado a un banco y su correspondiente contrapartida
        /// </summary>
        /// <param name="ajusteCajaBanco"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAAjusteCajaBancoDC GuardarAjusteCajaBanco(CAAjusteCajaBancoDC ajusteCajaBanco);
        

        #region CajasDisponibles

        /// <summary>
        /// Obtiene las Cajas disponibles asignadas a los
        /// centros de Servicio
        /// </summary>
        /// <returns>Numero de Cajas Disponibles</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerNumeroCajasDisponibles();

        /// <summary>
        /// Actualizar el numero de Cajas Disponibles agregando o eliminando
        /// segun sea el caso
        /// </summary>
        /// <param name="numeroCajas">Numero de Cajas a Actualizar</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarCajasDisponibles(int numeroCajasActualizar);

        #endregion CajasDisponibles

        /// <summary>
        /// Obtiene todos los valores de los parametros configurado en la tabla de parametros caja.
        /// </summary>        
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CAParametroCaja> ObtenerParametrosCajas();

        /// <summary>
        /// Actualiza el valor de un parametro de cajas
        /// </summary>
        /// <param name="idParametro"></param>
        /// <param name="valor"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]        
        void ActualizarParametroCaja(string idParametro, string valor);
    }
}