using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CO.Servidor.Cajas.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Cajas.Transacciones;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Cierres;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Impresion;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using CO.Servidor.Cajas;

namespace CO.Servidor.Servicios.Implementacion.GestionCajas
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CAGestionCajasSvc : ICAGestionCajasSvc
    {
        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return GCAdministradorCajas.Instancia.ObtenerCentrosServicios(idRacol);
        }


        /// <summary>
        /// Obtiene los Centros de Servicios Activos e Inactivos de una Racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosTodos(long idRacol)
        {
            return GCAdministradorCajas.Instancia.ObtenerCentrosServiciosTodos(idRacol);
        }


        /// <summary>
        /// Registrar Transacciones entre RACOL- Agencia - Banco - Casa Matriz.
        /// </summary>
        /// <param name="infoTransaccion">The info transaccion.</param>
        /// <returns>Información de la transacción registrada</returns>
        public CAResultadoRegistroTransaccionDC TransaccionRacolBancoEmpresa(CAOperaRacolBancoEmpresaDC infoTransaccion)
        {
            return GCAdministradorCajas.Instancia.TransaccionRacolBancoEmpresa(infoTransaccion);
        }

        /// <summary>
        /// Obtiene todos los bancos
        /// </summary>
        /// <returns>Lista con los bancos</returns>
        public IList<PABanco> ObtenerTodosBancos()
        {
            return GCAdministradorCajas.Instancia.ObtenerTodosBancos();
        }

        /// <summary>
        /// Obtiene los Tipos de Documentos de Banco
        /// </summary>
        /// <returns>lista de los Tipos de Doc Banco</returns>
        public IList<PATipoDocumBancoDC> ObtenerTiposDocumentosBanco()
        {
            return GCAdministradorCajas.Instancia.ObtenerTiposDocumentosBanco();
        }

        /// <summary>
        /// Metodo para Obtener las Transacciones.
        /// realizadas por la Empresa.
        /// </summary>
        /// <param name="fechaTransaccion">The fecha transaccion.</param>
        /// <returns></returns>
        public IList<CACajaCasaMatrizDC> ObtenerTransaccionesEmpresa(DateTime fechaTransaccion, short idCasaMatriz)
        {
            return GCAdministradorCajas.Instancia.ObtenerTransaccionesEmpresa(fechaTransaccion, idCasaMatriz);
        }

        /// <summary>
        /// Metodo para obtener las operaciones del RACOL.
        /// </summary>
        /// <param name="fechaTransaccion">The fecha transaccion.</param>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<CACajaCasaMatrizDC> ObtenerTransaccionesRACOL(DateTime fechaTransaccion, long idCentroServicio)
        {
            return GCAdministradorCajas.Instancia.ObtenerTransaccionesRACOL(fechaTransaccion, idCentroServicio);
        }

        /// <summary>
        /// Obtiene la dupla del concepto.
        /// </summary>
        /// <param name="idConcepto">The id concepto.</param>
        /// <returns>dupla del concepto enviado</returns>
        public CAConceptoCajaDC ObtenerDuplaConcepto(int idConcepto)
        {
            return GCAdministradorCajas.Instancia.ObtenerDuplaConcepto(idConcepto);
        }

        /// <summary>
        /// Obtiene el centro servicio.
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {
            return GCAdministradorCajas.Instancia.ObtenerCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Retorna las cuentas externas del sistema
        /// </summary>
        /// <returns></returns>
        public List<CO.Servidor.Servicios.ContratoDatos.Cajas.CACuentaExterna> ObtenerCuentasExternas()
        {
            return GCAdministradorCajas.Instancia.ObtenerCuentasExternas();
        }

        /// <summary>
        /// retorna los conceptos filtrados por categoria
        /// </summary>
        /// <param name="categoria"></param>
        /// <returns></returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCajaCategoria(CAEnumCategoriasConceptoCaja categoria)
        {
            return GCAdministradorCajas.Instancia.ObtenerConceptosCaja(categoria);
        }

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
        public List<CAConceptoCajaDC> ObtenerConceptosCaja(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return GCAdministradorCajas.Instancia.ObtenerConceptosCaja(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Se aplican cambios realizados sobre un concepto de caja
        /// </summary>
        /// <param name="conceptoCaja"></param>
        public void ActualizarConceptoCaja(CAConceptoCajaDC conceptoCaja)
        {
            GCAdministradorCajas.Instancia.ActualizarConceptoCaja(conceptoCaja);
        }

        /// <summary>
        /// Se inserta un concepto de caja nuevo
        /// </summary>
        /// <param name="conceptoCaja"></param>
        public void AdicionarConceptoCaja(CAConceptoCajaDC conceptoCaja)
        {
            GCAdministradorCajas.Instancia.AdicionarConceptoCaja(conceptoCaja);
        }

        /// <summary>
        /// Obtener las operaciones de caja de Operación Nacional en una fecha
        /// </summary>
        /// <param name="idCasaMatriz">Identificador único de la casa matriz</param>
        /// <param name="fecha">Fecha en la cual se hace la consulta</param>
        /// <returns>Collección con la información de las operaciones</returns>
        public IList<CACajaCasaMatrizDC> ObtenerOperacionesOpn(DateTime fechaTransaccion, short idCasaMatriz)
        {
            return GCAdministradorCajas.Instancia.ObtenerOperacionesOpn(fechaTransaccion, idCasaMatriz);
        }

        /// <summary>
        /// Cerrar las aperturas de caja de Casa Matriz, Operación Naciona, Bancos y Centros de Servicios que el usuario ha hecho
        /// </summary>
        /// <param name="idCasaMatriz">Identificación de la casa matriz sobre la cual se hacen las aperturas</param>
        /// <param name="idCodigoUsuario">Código del usuario que hizo las aperturas</param>
        /// <param name="idRacol">Identificación del RACOL desde donde se hacen las operaciones</param>
        /// <remarks>Las aperturas sobre centros de servicos se hacen sobre la caja 0</remarks>
        public void CerrarCajaGestion(short idCasaMatriz, long idCodigoUsuario, long idRacol)
        {
            GCAdministradorCajas.Instancia.CerrarCajaGestion(idCasaMatriz, idCodigoUsuario, idRacol);
        }

        /// <summary>
        /// Obtener la información del cierre de las cajas de gestión
        /// </summary>
        /// <param name="idCasaMatriz">Identificador de la casa matriz donde se hace el cierre</param>
        /// <param name="idCodigoUsuario">Identificador del usuairo que hace el cierre</param>
        /// <returns>Colección con la información del cierre</returns>
        public IList<CACierreCajaGestionDC> ObtenerCierreCajasGestion(short idCasaMatriz, long idCodigoUsurio)
        {
            return GCAdministradorCajas.Instancia.ObtenerCierreCajaGestion(idCasaMatriz, idCodigoUsurio);
        }

        /// <summary>
        /// Carga información inicial para la impresión de formato GRI-R10
        /// </summary>
        /// <param name="idCentroServicios">Identificador centro de servicios donde se hace la impresión</param>
        /// <param name="valor">Valor de la operacion</param>
        /// <returns>Obejeto con los datos iniciales del formato</returns>
        /// <remarks>El objeto retornado no se llena completamente ya que hay algunos datos que se encuentran solo en el cliente</remarks>
        public CADatosImpresionGirR10DC ObtenerDatosFormatoGirR10(long idCentroServiciosOrigen, long idCentroServiciosDestino, bool esGestion, decimal valor, string bolsaSeguridad, long numeroGuia, string numeroPrecinto)
        {
            return GCAdministradorCajas.Instancia.ObtenerDatosFormatoGirR10(idCentroServiciosOrigen, idCentroServiciosDestino, esGestion, valor, bolsaSeguridad, numeroGuia, numeroPrecinto);
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el codigo del
        /// usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario)
        {
            return GCAdministradorCajas.Instancia.ObtenerUsuarioPorCodigo(idCodigoUsuario);
        }

       

        public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionOpn()
        {
            return GCAdministradorCajas.Instancia.ObtenerConceptosCajaGestionOpn();
        }

        public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionRacol()
        {
            return GCAdministradorCajas.Instancia.ObtenerConceptosCajaGestionRacol();
        }

        /// <summary>
        /// Obtiene la info la base de caja de Operacion Nacional
        /// de acuerdo con su Casa matriz
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns>info de base de caja de OpeNal</returns>
        public CABaseGestionCajasDC ObtenerBaseCajaOperacionNacional(int idCasaMatriz)
        {
            return GCAdministradorCajas.Instancia.ObtenerBaseCajaOperacionNacional(idCasaMatriz);
        }

        /// <summary>
        /// Obtiene la info la base de caja de Operacion Nacional
        /// de acuerdo con su Casa matriz
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns>lista de Bases de Caja de las Casas Matriz y Operacion Nal</returns>
        public List<CABaseGestionCajasDC> ObtenerBasesDeCajasOperacionNacional()
        {
            return GCAdministradorCajas.Instancia.ObtenerBasesDeCajasOperacionNacional();
        }

        /// <summary>
        /// Actualiza, Modifica, Borra la Base de la caja
        /// de Operacional Nacional por su Casa Matriz
        /// </summary>
        /// <param name="infoBaseCajaOpeNal">data de Entrada</param>
        public void GestionBaseCajaOperacionNacional(CABaseGestionCajasDC infoBaseCajaOpeNal)
        {
            GCAdministradorCajas.Instancia.GestionBaseCajaOperacionNacional(infoBaseCajaOpeNal);
        }

        /// <summary>
        /// Obtiene la info la base de caja de Operacion Nacional
        /// de acuerdo con su Casa matriz
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns>lista de Bases de Caja de las Casas Matriz y Operacion Nal</returns>
        public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionCasaMatriz()
        {
            return GCAdministradorCajas.Instancia.ObtenerConceptosCajaGestionCasaMatriz();
        }

        /// <summary>
        /// Retorna todas las casas matriz activas
        /// </summary>
        /// <returns>Colección con todas las casas matrices</returns>
        public IList<ARCasaMatrizDC> ObtenerTodasLasCasaMatriz()
        {
            return GCAdministradorCajas.Instancia.ObtenerTodasLasCasaMatriz();
        }

        /// <summary>
        /// Obtiene las categorias de los conceptos
        /// </summary>
        /// <returns>lista ordenada de Categorias</returns>
        public List<CAConceptoCajaCategoriaDC> ObtenerCategoriaConceptosCaja()
        {
            return GCAdministradorCajas.Instancia.ObtenerCategoriaConceptosCaja();
        }

        /// <summary>
        /// Ingresa un ajuste a la caja de un punto
        /// </summary>
        /// <param name="ajusteCaja"></param>
        public CAAjustesCajaNovedadesDC IngresarAjusteCajaNovedades(CAAjustesCajaNovedadesDC ajusteCaja)
        {
            return GCAdministradorCajas.Instancia.IngresarAjusteCajaNovedades(ajusteCaja);            
        }

        /// <summary>
        /// Almacena en la base de datos un ajuste de caja asociado a un banco y su correspondiente contrapartida
        /// </summary>
        /// <param name="ajusteCajaBanco"></param>
        public CAAjusteCajaBancoDC GuardarAjusteCajaBanco(CAAjusteCajaBancoDC ajusteCajaBanco)
        {
            return GCAdministradorCajas.Instancia.GuardarAjusteCajaBanco(ajusteCajaBanco);
        }

        #region tipos de Observacion recoleccion dinero puntos

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
        ///
        public IEnumerable<CATipoObsPuntoAgenciaDC> ObtenerTipoObservacionFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return GCAdministradorCajas.Instancia.ObtenerTipoObservacionFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        ///  Adicionar, editar , eliminar tipos de observacion
        /// </summary>
        /// <param name="tipoObservacion">Clase tipoObservacion</param>

        public void ActualizarTipoObservacion(CATipoObsPuntoAgenciaDC tipoObservacion)
        {
            GCAdministradorCajas.Instancia.ActualizarTipoObservacion(tipoObservacion);
        }

        /// <summary>
        /// Retorna la lista de tipos de observaciones observacions
        /// </summary>
        /// <returns>tipos de observaciones </returns>
        public IEnumerable<CATipoObsPuntoAgenciaDC> ObtenerTipoObservacion()
        {
            return GCAdministradorCajas.Instancia.ObtenerTipoObservacion();
        }

        #endregion tipos de Observacion recoleccion dinero puntos

        #region CajasDisponibles

        /// <summary>
        /// Obtiene las Cajas disponibles asignadas a los
        /// centros de Servicio
        /// </summary>
        /// <returns>Numero de Cajas Disponibles</returns>
        public int ObtenerNumeroCajasDisponibles()
        {
            return GCAdministradorCajas.Instancia.ObtenerNumeroCajasDisponibles();
        }

        /// <summary>
        /// Actualizar el numero de Cajas Disponibles agregando o eliminando
        /// segun sea el caso
        /// </summary>
        /// <param name="numeroCajas">Numero de Cajas a Actualizar</param>
        public void ActualizarCajasDisponibles(int numeroCajasActualizar)
        {
            GCAdministradorCajas.Instancia.ActualizarCajasDisponibles(numeroCajasActualizar);
        }

        #endregion CajasDisponibles

        /// <summary>
        /// Obtiene todos los valores de los parametros configurado en la tabla de parametros caja.
        /// </summary>        
        /// <returns></returns>
        public IEnumerable<CAParametroCaja> ObtenerParametrosCajas()
        {
            return CAAdministradorCajas.Instancia.ObtenerParametrosCajas();
        }

        /// <summary>
        /// Actualiza el valor de un parametro de cajas
        /// </summary>
        /// <param name="idParametro"></param>
        /// <param name="valor"></param>
        public void ActualizarParametroCaja(string idParametro, string valor)
        {
            CAAdministradorCajas.Instancia.ActualizarParametroCaja(idParametro, valor);
        }
    }
}