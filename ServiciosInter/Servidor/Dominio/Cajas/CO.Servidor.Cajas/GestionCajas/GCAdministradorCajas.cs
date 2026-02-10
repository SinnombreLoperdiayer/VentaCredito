using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CO.Servidor.Cajas.Apertura;
using CO.Servidor.Cajas.CACierreCaja;
using CO.Servidor.Cajas.CajaFinanciera;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Cajas.Transacciones;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Cierres;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Impresion;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Excepciones;
using System.Transactions;
using System.ServiceModel;

namespace CO.Servidor.Cajas.GestionCajas
{
    /// <summary>
    /// Fachada para los controles visuales de Gestión Cajas
    /// </summary>
    public class GCAdministradorCajas
    {
        #region Atributos

        /// <summary>
        /// creo la Instancia
        /// </summary>
        private static GCAdministradorCajas instancia = new GCAdministradorCajas();

        #endregion Atributos

        #region Instancia

        /// <summary>
        /// Obtengo la instancia.
        /// </summary>
        public static GCAdministradorCajas Instancia
        {
            get { return GCAdministradorCajas.instancia; }
        }

        #endregion Instancia

        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return CACaja.Instancia.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Obtiene los Centros de Servicios Activos e Inactivos de una Racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosTodos(long idRacol)
        {
            return CACaja.Instancia.ObtenerCentrosServiciosTodos(idRacol);
        }

        /// <summary>
        /// Obtiene el centro servicio.
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {
            return CACaja.Instancia.ObtenerCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Registrar Transacciones entre RACOL- Agencia - Banco - Casa Matriz.
        /// </summary>
        /// <param name="infoTransaccion">The info transaccion.</param>
        /// <returns>Información de la transacción registrada</returns>
        public CAResultadoRegistroTransaccionDC TransaccionRacolBancoEmpresa(CAOperaRacolBancoEmpresaDC infoTransaccion)
        {
            return CACajaFinanciera.Instancia.TransaccionRacolBancoCasaMatriz(infoTransaccion);
        }

        ///// <summary>
        ///// Transacciones entre RACOL- Agencia - Banco - Casa Matriz.
        ///// </summary>
        ///// <param name="infoTransaccion">Información de la transaccion.</param>
        //public void RegistrarOperacionesCajaGestion(CAOperacionCajaGestionCentroServiciosDC infoTransaccion)
        //{
        //  CAApertura.Instancia.RegistrarOperacionCajaGestion(infoTransaccion);
        //}

        /// <summary>
        /// Obtiene todos los bancos
        /// </summary>
        /// <returns>Lista con los bancos</returns>
        public IList<PABanco> ObtenerTodosBancos()
        {
            return CACaja.Instancia.ObtenerTodosBancos();
        }

        /// <summary>
        /// Obtiene los Tipos de Documentos de Banco
        /// </summary>
        /// <returns>lista de los Tipos de Doc Banco</returns>
        public IList<PATipoDocumBancoDC> ObtenerTiposDocumentosBanco()
        {
            return CACaja.Instancia.ObtenerTiposDocumentosBanco();
        }

        /// <summary>
        /// Metodo para Obtener las Transacciones.
        /// realizadas por la Empresa.
        /// </summary>
        /// <param name="fechaTransaccion">The fecha transaccion.</param>
        /// <returns></returns>
        public IList<CACajaCasaMatrizDC> ObtenerTransaccionesEmpresa(DateTime fechaTransaccion, short idCasaMatriz)
        {
            return CACajaFinanciera.Instancia.ObtenerTransaccionesEmpresa(fechaTransaccion, idCasaMatriz);
        }

        /// <summary>
        /// Metodo para obtener las operaciones del RACOL.
        /// </summary>
        /// <param name="fechaTransaccion">The fecha transaccion.</param>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<CACajaCasaMatrizDC> ObtenerTransaccionesRACOL(DateTime fechaTransaccion, long idCentroServicio)
        {
            return CACajaFinanciera.Instancia.ObtenerTransaccionesRACOL(fechaTransaccion, idCentroServicio);
        }

        /// <summary>
        /// Obtener las operaciones de caja de Operación Nacional en una fecha
        /// </summary>
        /// <param name="idCasaMatriz">Identificador único de la casa matriz</param>
        /// <param name="fecha">Fecha en la cual se hace la consulta</param>
        /// <returns>Collección con la información de las operaciones</returns>
        public IList<CACajaCasaMatrizDC> ObtenerOperacionesOpn(DateTime fechaTransaccion, short idCasaMatriz)
        {
            return CACajaFinanciera.Instancia.ObtenerOperacionesOpn(fechaTransaccion, idCasaMatriz);
        }

        /// <summary>
        /// Retorna las cuentas externas del sistema
        /// </summary>
        /// <returns></returns>
        public List<CO.Servidor.Servicios.ContratoDatos.Cajas.CACuentaExterna> ObtenerCuentasExternas()
        {
            return CACaja.Instancia.ObtenerCuentasExternas();
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el codigo del
        /// usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario)
        {
            return CACaja.Instancia.ObtenerUsuarioPorCodigo(idCodigoUsuario);
        }

        /// <summary>
        /// Obtiene todas la Agencias-ptos-col-Racol
        /// </summary>
        /// <returns>listaCentrosServicio</returns>
        public IList<PUCentroServiciosDC> ObtenerTodosCentrosServicios()
        {
            return CACaja.Instancia.ObtenerTodosCentrosServicios();
        }

        public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionCasaMatriz()
        {
            return CACajaFinanciera.Instancia.ObtenerConceptosCajaGestionCasaMatriz();
        }

        public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionOpn()
        {
            return CACajaFinanciera.Instancia.ObtenerConceptosCajaGestionOpn();
        }

        public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionRacol()
        {
            return CACajaFinanciera.Instancia.ObtenerConceptosCajaGestionRacol();
        }

        /// <summary>
        /// Obtiene la info la base de caja de Operacion Nacional
        /// de acuerdo con su Casa matriz
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns>info de base de caja de OpeNal</returns>
        public CABaseGestionCajasDC ObtenerBaseCajaOperacionNacional(int idCasaMatriz)
        {
            return CACajaOperacionNacional.Instancia.ObtenerBaseCajaOperacionNacional(idCasaMatriz);
        }

        /// <summary>
        /// Obtiene la info la base de caja de Operacion Nacional
        /// de acuerdo con su Casa matriz
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns>lista de Bases de Caja de las Casas Matriz y Operacion Nal</returns>
        public List<CABaseGestionCajasDC> ObtenerBasesDeCajasOperacionNacional()
        {
            return CACajaOperacionNacional.Instancia.ObtenerBasesDeCajasOperacionNacional();
        }

        /// <summary>
        /// Actualiza, Modifica, Borra la Base de la caja
        /// de Operacional Nacional por su Casa Matriz
        /// </summary>
        /// <param name="infoBaseCajaOpeNal">data de Entrada</param>
        public void GestionBaseCajaOperacionNacional(CABaseGestionCajasDC infoBaseCajaOpeNal)
        {
            CACajaOperacionNacional.Instancia.GestionBaseCajaOperacionNacional(infoBaseCajaOpeNal);
        }

        /// <summary>
        /// Retorna todas las casas matriz activas
        /// </summary>
        /// <returns>Colección con todas las casas matrices</returns>
        public IList<ARCasaMatrizDC> ObtenerTodasLasCasaMatriz()
        {
            return CACaja.Instancia.ObtenerTodasLasCasaMatriz();
        }

        #region Conceptos de Caja

        /// <summary>
        /// Obtiene las categorias de los conceptos
        /// </summary>
        /// <returns>lista ordenada de Categorias</returns>
        public List<CAConceptoCajaCategoriaDC> ObtenerCategoriaConceptosCaja()
        {
            return CACaja.Instancia.ObtenerCategoriaConceptosCaja();
        }


        /// <summary>
        /// retorna los conceptos filtrados por categoria
        /// </summary>
        /// <param name="categoria"></param>
        /// <returns></returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCaja(CAEnumCategoriasConceptoCaja categoria)
        {
            return CACaja.Instancia.ObtenerConceptosCaja(categoria);
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
            return CACaja.Instancia.ObtenerConceptosCaja(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Se aplican cambios realizados sobre un concepto de caja
        /// </summary>
        /// <param name="conceptoCaja"></param>
        public void ActualizarConceptoCaja(CAConceptoCajaDC conceptoCaja)
        {
            CACaja.Instancia.ActualizarConceptoCaja(conceptoCaja);
        }

        /// <summary>
        /// Se inserta un concepto de caja nuevo
        /// </summary>
        /// <param name="conceptoCaja"></param>
        public void AdicionarConceptoCaja(CAConceptoCajaDC conceptoCaja)
        {
            CACaja.Instancia.AdicionarConceptoCaja(conceptoCaja);
        }

        /// <summary>
        /// Obtiene la dupla del concepto.
        /// </summary>
        /// <param name="idConcepto">The id concepto.</param>
        /// <returns>dupla del concepto enviado</returns>
        public CAConceptoCajaDC ObtenerDuplaConcepto(int idConcepto)
        {
            return CACaja.Instancia.ObtenerDuplaConcepto(idConcepto);
        }

        /// <summary>
        /// Obtiene los conceptos de Caja por especificacion de
        /// visibilidad para mensajero - punto/Agencia - Racol.
        /// </summary>
        /// <param name="filtroCampoVisible">The filtro campo visible.</param>
        /// <returns>Lista de Conceptos de Caja por el filtro de Columna</returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCajaPorCategoria(int idCategoria)
        {
            return CACaja.Instancia.ObtenerConceptosCajaPorCategoria(idCategoria);
        }

        #endregion Conceptos de Caja

        #region Cierre Caja

        /// <summary>
        /// Cerrar las aperturas de caja de Casa Matriz, Operación Naciona, Bancos y Centros de Servicios que el usuario ha hecho
        /// </summary>
        /// <param name="idCasaMatriz">Identificación de la casa matriz sobre la cual se hacen las aperturas</param>
        /// <param name="idCodigoUsuario">Código del usuario que hizo las aperturas</param>
        /// <param name="idRacol">Identificación del RACOL desde donde se hacen las operaciones</param>
        /// <remarks>Las aperturas sobre centros de servicos se hacen sobre la caja 0</remarks>
        public void CerrarCajaGestion(short idCasaMatriz, long idCodigoUsuario, long idRacol)
        {
            CACierreCajaGestion.Instancia.CerrarCajaGestion(idCasaMatriz, idCodigoUsuario, idRacol);
        }

        /// <summary>
        /// Obtener la información del cierre de las cajas de gestión
        /// </summary>
        /// <param name="idCasaMatriz">Identificador de la casa matriz donde se hace el cierre</param>
        /// <param name="idCodigoUsuario">Identificador del usuairo que hace el cierre</param>
        /// <returns>Colección con la información del cierre</returns>
        public IList<CACierreCajaGestionDC> ObtenerCierreCajaGestion(short idCasaMatriz, long idCodigoUsuario)
        {
            return CACierreCajaGestion.Instancia.ObtenerCierreCajaGestion(idCasaMatriz, idCodigoUsuario);
        }

        /// <summary>
        /// Carga información inicial para la impresión de formato GRI-R10
        /// </summary>
        /// <param name="idCentroServiciosOrigen">Identificador centro de servicios origen de la transacción</param>
        /// <param name="idCentroServiciosDestino">Identificador centro de servicios destino de la transacción</param>
        /// <param name="esGestion">True, el origen es una gestion, False es un centro de servicios</param>
        /// <param name="valor">alor de la operacion</param>
        /// <param name="bolsaSeguridad">Número de bolsa de seguridad</param>
        /// <param name="numeroGuia">Número de guía interna con la cual se hace el movimiento</param>
        /// <param name="numeroPrecinto">Número del precinto, este puede ser null</param>
        /// <returns>Obejeto con los datos iniciales del formato</returns>
        /// <remarks>El objeto retornado no se llena completamente ya que hay algunos datos que se encuentran solo en el cliente</remarks>
        public CADatosImpresionGirR10DC ObtenerDatosFormatoGirR10(long idCentroServiciosOrigen, long idCentroServiciosDestino, bool esGestion, decimal valor, string bolsaSeguridad, long numeroGuia, string numeroPrecinto)
        {
            return CACajaFinanciera.Instancia.ObtenerDatosFormatoGirR10(idCentroServiciosOrigen, idCentroServiciosDestino, esGestion, valor, bolsaSeguridad, numeroGuia, numeroPrecinto);
        }

        #endregion Cierre Caja

        #region AjusteCajas
        /// <summary>
        /// Ingresa un ajuste a la caja de un punto
        /// </summary>
        /// <param name="ajusteCaja"></param>
        public CAAjustesCajaNovedadesDC IngresarAjusteCajaNovedades(CAAjustesCajaNovedadesDC ajusteCaja)
        {
            return CACaja.Instancia.IngresarAjusteCajaNovedades(ajusteCaja);
        }


        /// <summary>
        /// Almacena en la base de datos un ajuste de caja asociado a un banco y su correspondiente contrapartida
        /// </summary>
        /// <param name="ajusteCajaBanco"></param>
        public CAAjusteCajaBancoDC GuardarAjusteCajaBanco(CAAjusteCajaBancoDC ajusteCajaBanco)
        {
            //Validar num consignacion
            if (!string.IsNullOrEmpty(ajusteCajaBanco.NoConsignacion) && CARepositorioGestionCajas.Instancia.NumConsignacionExistente(ajusteCajaBanco.NoConsignacion, ajusteCajaBanco.CuentaOrigen.Banco.IdBanco)==null)
            {
                throw new FaultException<ControllerException>(
                new ControllerException(COConstantesModulos.CAJA, "00",
                "El número de consignación ingresado nunca ha sido ingresado en la caja del banco " + ajusteCajaBanco.CuentaOrigen.Banco.Descripcion));
            }
            using (TransactionScope tx = new TransactionScope())
            {

                long consecutivoComprobante = 0;
                consecutivoComprobante = CACaja.Instancia.ObtenerConsecutivoComprobateCajaEgreso();

                CACajaBancoDC operacionBancoOrigen = new CACajaBancoDC()
                {
                    ConceptoCaja = ajusteCajaBanco.ConceptoCaja,
                    CreadoPor = ControllerContext.Current.Usuario,
                    DescripcionBanco = ajusteCajaBanco.CuentaOrigen.Banco.Descripcion,
                    DocumentoBancario = new CADocumentoBancarioDC()
                    {
                        DescripcionTipoDocBancario = "Consignación Bancaria",
                        NumeroDocBancario = ajusteCajaBanco.NoConsignacion,
                        TipoDocBancario = 2
                    },
                    FechaGrabacion = DateTime.Now,
                    FechaMovimiento = DateTime.Now,
                    IdBanco = ajusteCajaBanco.CuentaOrigen.Banco.IdBanco,
                    MovHechoPor = "EMP",
                    NumeroCta = ajusteCajaBanco.CuentaOrigen.NumeroCuenta,
                    Observacion = ajusteCajaBanco.Observacion,
                    Valor = ajusteCajaBanco.Valor,
                    IdCasaMatriz = ajusteCajaBanco.CuentaOrigen.IdCasaMatriz,
                    NumeroComprobante=consecutivoComprobante.ToString()
                };

                CAApertura.Instancia.RegistrarOperacionBanco(operacionBancoOrigen);

                if (ajusteCajaBanco.CuentaDestino != null)
                {
                    ajusteCajaBanco.ConceptoCaja.EsIngreso = !ajusteCajaBanco.ConceptoCaja.EsIngreso;

                    CACajaBancoDC operacionBancoDestino = new CACajaBancoDC()
                    {
                        ConceptoCaja = ajusteCajaBanco.ConceptoCaja,
                        CreadoPor = ControllerContext.Current.Usuario,
                        DescripcionBanco = ajusteCajaBanco.CuentaDestino.Banco.Descripcion,
                        DocumentoBancario = new CADocumentoBancarioDC()
                        {
                            DescripcionTipoDocBancario = "Consignación Bancaria",
                            NumeroDocBancario = ajusteCajaBanco.NoConsignacion,
                            TipoDocBancario = 2
                        },
                        FechaGrabacion = DateTime.Now,
                        FechaMovimiento = DateTime.Now,
                        IdBanco = ajusteCajaBanco.CuentaDestino.Banco.IdBanco,
                        MovHechoPor = "EMP",
                        NumeroCta = ajusteCajaBanco.CuentaDestino.NumeroCuenta,
                        Observacion = ajusteCajaBanco.Observacion,
                        Valor = ajusteCajaBanco.Valor,
                        IdCasaMatriz = ajusteCajaBanco.CuentaDestino.IdCasaMatriz,
                        NumeroComprobante = consecutivoComprobante.ToString()
                    };

                    CAApertura.Instancia.RegistrarOperacionBanco(operacionBancoDestino);
                }

                ajusteCajaBanco.FechaGrabacion = DateTime.Now;
                ajusteCajaBanco.NumeroComprobante = consecutivoComprobante.ToString();
                tx.Complete();
            }
            return ajusteCajaBanco;
        }
        #endregion

        #region Administrar Tipos de Observacion

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
        public IEnumerable<CATipoObsPuntoAgenciaDC> ObtenerTipoObservacionFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return CARepositorioGestionCajas.Instancia.ObtenerTipoObservacionFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        ///  Adicionar, editar , eliminar tipos de observacion
        /// </summary>
        /// <param name="tipoObservacion">Clase tipoObservacion</param>

        public void ActualizarTipoObservacion(CATipoObsPuntoAgenciaDC tipoObservacion)
        {
            if (tipoObservacion.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                CARepositorioGestionCajas.Instancia.AdicionarTipoObservacion(tipoObservacion);
            else if (tipoObservacion.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                CARepositorioGestionCajas.Instancia.EditarTipoEbservacion(tipoObservacion);
            else if (tipoObservacion.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                CARepositorioGestionCajas.Instancia.EliminarTipoEbservacion(tipoObservacion);
        }

        /// <summary>
        /// Retorna la lista de tipos de observaciones observacions
        /// </summary>
        /// <returns>tipos de observaciones </returns>
        public IEnumerable<CATipoObsPuntoAgenciaDC> ObtenerTipoObservacion()
        {
            return CARepositorioGestionCajas.Instancia.ObtenerTipoObservacion();
        }

        #endregion Administrar Tipos de Observacion

        #region CajasDisponibles

        /// <summary>
        /// Obtiene las Cajas disponibles asignadas a los
        /// centros de Servicio
        /// </summary>
        /// <returns>Numero de Cajas Disponibles</returns>
        public int ObtenerNumeroCajasDisponibles()
        {
            return CARepositorioGestionCajas.Instancia.ObtenerNumeroCajasDisponibles();
        }

        /// <summary>
        /// Actualizar el numero de Cajas Disponibles agregando o eliminando
        /// segun sea el caso
        /// </summary>
        /// <param name="numeroCajas">Numero de Cajas a Actualizar</param>
        public void ActualizarCajasDisponibles(int numeroCajasActualizar)
        {
            int numeroCajasActual = ObtenerNumeroCajasDisponibles();

            if (numeroCajasActualizar > numeroCajasActual)
            {
                int cajasAgregar = numeroCajasActualizar - numeroCajasActual;

                for (int i = 1; i <= cajasAgregar; i++)
                {
                    //Agrego la caja nueva
                    CARepositorioGestionCajas.Instancia.AgregarCajaDisponible();
                }
            }
            else if (numeroCajasActualizar < numeroCajasActual)
            {
                int cajaEliminar = numeroCajasActual - numeroCajasActualizar;

                for (int i = 1; i <= cajaEliminar; i++)
                {
                    //Elimino la Ultima caja creada
                    CARepositorioGestionCajas.Instancia.EliminarCajaDisponible();
                }
            }
        }

        #endregion CajasDisponibles
    }
}