using CO.Servidor.CentroServicios.Comun;
using CO.Servidor.CentroServicios.Datos;
using CO.Servidor.CentroServicios.Datos.Modelo;
using CO.Servidor.CentroServicios.Suministros;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Archivo;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.ParametrosFW.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Transactions;

namespace CO.Servidor.CentroServicios
{
    public class PUCentroServicios : ControllerBase
    {
        private static readonly PUCentroServicios instancia = (PUCentroServicios)FabricaInterceptores.GetProxy(new PUCentroServicios(), COConstantesModulos.CENTRO_SERVICIOS);


        #region Fachadas

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        #endregion Fachadas
        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static PUCentroServicios Instancia
        {
            get { return PUCentroServicios.instancia; }
        }

        private PUCentroServicios()
        { }

        #region CRUD Tipo de comision fija

        /// <summary>
        /// Otiene los tipos de comision fija
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista de tipos de comision fija</returns>
        public IList<PUTipoComisionFija> ObtenerTiposComisionFija(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PURepositorio.Instancia.ObtenerTiposComisionFija(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona Modifica o elimina un tipo de comision fija
        /// </summary>
        /// <param name="tipoComisionFija">Objeto tipo de comision fija</param>
        public void ActualizarTipoComisionFija(PUTipoComisionFija tipoComisionFija)
        {
            switch (tipoComisionFija.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PURepositorio.Instancia.AdicionarTipoComisionFija(tipoComisionFija);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PURepositorio.Instancia.EliminarTipoComisionFija(tipoComisionFija);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PURepositorio.Instancia.EditarTipoComisionFija(tipoComisionFija);
                    break;
            }
        }

        #endregion CRUD Tipo de comision fija

        #region CRUD Tipo de descuento

        /// <summary>
        /// Otiene los tipos de descuento
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista de tipos de descuento</returns>
        public IList<PUTiposDescuento> ObtenerTiposDescuento(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PURepositorio.Instancia.ObtenerTiposDescuento(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene el numero total de envios en pendientyes por ingr a custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        public int ObtenerConteoPendIngrCustodia(int idTipoMovimiento, int idEstadoGuia)
        {
            return PURepositorio.Instancia.ObtenerConteoPendIngrCustodia(idTipoMovimiento, idEstadoGuia);
        }

        /// <summary>
        /// Obtiene el numero total de envios en custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        public int ObtenerConteoGuiasCustodia(int idTipoMovimiento, int idEstadoGuia)
        {
            return PURepositorio.Instancia.ObtenerConteoGuiasCustodia(idTipoMovimiento, idEstadoGuia);
        }

        /// <summary>
        /// Adiciona Modifica o elimina un tipo de descuento
        /// </summary>
        /// <param name="tipoDescuento">Objeto tipo de descuento</param>
        public void ActializarTipoDescuento(PUTiposDescuento tipoDescuento)
        {
            switch (tipoDescuento.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PURepositorio.Instancia.AdicionarTipoDescuento(tipoDescuento);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PURepositorio.Instancia.EliminarTipoDescuento(tipoDescuento);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PURepositorio.Instancia.EditarTipoDescuento(tipoDescuento);
                    break;
            }
        }

        #endregion CRUD Tipo de descuento

        #region CRUD Documentos centro de servicio

        /// <summary>
        /// Obtiene los destinos de para la creacion de los documentos de referencia en centros de servicio
        /// </summary>
        /// <returns></returns>
        public IList<PUTipoDocumentosCentrosServicios> ObtenerTiposDocumentosCentroServicios()
        {
            List<PUTipoDocumentosCentrosServicios> lstTiposDocumentos = new List<PUTipoDocumentosCentrosServicios>();

            lstTiposDocumentos.Add(new PUTipoDocumentosCentrosServicios() { IdTipo = PUConstantesCentroServicios.TipoDocumentosAgenteComercial, Descripcion = MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.IN_TIPO_DOCUMENTO_AGENTE_COMERCIAL) });
            lstTiposDocumentos.Add(new PUTipoDocumentosCentrosServicios() { IdTipo = PUConstantesCentroServicios.TipoDocumentosCentroServicios, Descripcion = MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.IN_TIPO_DOCUMENTO_CENTRO_SERVICIOS) });

            return lstTiposDocumentos.OrderBy(t => t.Descripcion).ToList();
        }

        /// <summary>
        /// Otiene los documentos de referencia de los centros de servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de documentos de referencia de los centros de servicio</returns>
        public IList<PUDocuCentroServicio> ObtenerDocuCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            var docu = PURepositorio.Instancia.ObtenerDocuCentrosServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
            docu.ToList().ForEach(obj =>
            {
                switch (obj.Tipo)
                {
                    case PUConstantesCentroServicios.TipoDocumentosAgenteComercial:
                        obj.DescripcionTipo = MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.IN_TIPO_DOCUMENTO_AGENTE_COMERCIAL);
                        break;

                    case PUConstantesCentroServicios.TipoDocumentosCentroServicios:
                        obj.DescripcionTipo = MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.IN_TIPO_DOCUMENTO_CENTRO_SERVICIOS);
                        break;

                    default:
                        obj.DescripcionTipo = obj.Tipo;
                        break;
                }
            });

            return docu;
        }

        /// <summary>
        /// Adiciona modifica o elimina un documento de referencia de los centros de servicio
        /// </summary>
        /// <param name="docuCentroServ">Objeto documento centro de servicio</param>
        public void ActualizarDocuCentrosServicio(PUDocuCentroServicio docuCentroServ)
        {
            switch (docuCentroServ.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PURepositorio.Instancia.AdicionarDocuCentrosServicio(docuCentroServ);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PURepositorio.Instancia.EliminarDocuCentrosServicio(docuCentroServ);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PURepositorio.Instancia.EditarDocuCentrosServicio(docuCentroServ);
                    break;
            }
        }

        #endregion CRUD Documentos centro de servicio

        #region CRUD Suministros

        /// <summary>
        /// Otiene los suministros
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de suministros</returns>
        public IList<PUSuministro> ObtenerSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PURepositorio.Instancia.ObtenerSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona modifica o elimina  un suministro
        /// </summary>
        /// <param name="suministro">Objeto suministro</param>
        public void ActualizarSuministro(PUSuministro suministro)
        {
            switch (suministro.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PURepositorio.Instancia.AdicionarSuministro(suministro);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PURepositorio.Instancia.EliminarSuministro(suministro);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PURepositorio.Instancia.EditarSuministro(suministro);
                    break;
            }
        }

        #endregion CRUD Suministros

        #region CRUD Servicio descuento Referencia

        /// <summary>
        /// Otiene los descuentos referencia de un servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de descuentos </returns>
        public IList<PUServicioDescuentoRef> ObtenerDescuentoReferencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PURepositorio.Instancia.ObtenerDescuentoReferencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, modifica o elimina un descuento referencia de un servicio
        /// </summary>
        /// <param name="descuentoRef">Objeto descuento referencia</param>
        public void ActualizarDescuentoRef(PUServicioDescuentoRef descuentoRef)
        {
            switch (descuentoRef.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PURepositorio.Instancia.AdicionarDescuentoRef(descuentoRef);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PURepositorio.Instancia.EliminarDescuentoRef(descuentoRef);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PURepositorio.Instancia.EditarDescuentoRef(descuentoRef);
                    break;
            }
        }

        #endregion CRUD Servicio descuento Referencia

        #region CRUD Servicio comision Referencia

        /// <summary>
        /// Obtiene las comisiones de referencia
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de comisiones </returns>
        public IList<PUServicioComisionRef> ObtenerComisionReferencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PURepositorio.Instancia.ObtenerComisionReferencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, modifica o elimina una comision referencia a un servicio
        /// </summary>
        /// <param name="descuentoRef">Objeto comision referencia</param>
        public void ActualizarComisionReferencia(PUServicioComisionRef comisionRef)
        {
            switch (comisionRef.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PURepositorio.Instancia.AdicionarComisionReferencia(comisionRef);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PURepositorio.Instancia.EliminarComisionReferencia(comisionRef);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PURepositorio.Instancia.EditarComisionReferencia(comisionRef);
                    break;
            }
        }

        #endregion CRUD Servicio comision Referencia

        #region Propietarios (Concesionarios)

        /// <summary>
        /// Obtiene los propietarios(concesionarios)
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los concesionarios</returns>
        public IList<PUPropietario> ObtenerPropietarios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PURepositorio.Instancia.ObtenerPropietarios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, modifica o elimina un propietario
        /// </summary>
        /// <param name="propietario">Objeto propietario</param>
        public void ActualizarPropietario(PUPropietario propietario)
        {
            int idProp;
            if (PAAdministrador.Instancia.ValidarListaRestrictiva(propietario.Nit))
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_ALERTA_LISTAS_RESTRICTIVAS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_ALERTA_LISTAS_RESTRICTIVAS)));

            switch (propietario.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        idProp = PURepositorio.Instancia.AdicionarPropietario(propietario);
                        transaccion.Complete();
                    }
                    if (propietario.ArchivosPropietarios != null)
                        this.OperacionesArchivosPropietarios(propietario.ArchivosPropietarios, idProp);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PURepositorio.Instancia.EliminarPropietario(propietario);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        PURepositorio.Instancia.EditarPropietario(propietario);
                        transaccion.Complete();
                    }
                    if (propietario.ArchivosPropietarios != null)
                        this.OperacionesArchivosPropietarios(propietario.ArchivosPropietarios);
                    break;
            }
        }

        #region Archivos

        /// <summary>
        /// Obtiene lista con los archivos de los propietarios
        /// </summary>
        /// <returns>lista con los archivos de los propietarios</returns>
        public IEnumerable<PUArchivosPropietario> ObtenerArchivosPropietarios(PUPropietario propietario)
        {
            return PURepositorio.Instancia.ObtenerArchivosPropietarios(propietario);
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un propietario
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoPropietario(PUArchivosPropietario archivo)
        {
            return PURepositorio.Instancia.ObtenerArchivoPropietario(archivo);
        }

        /// <summary>
        /// Adiciona o elimina los archivos de un propietario
        /// </summary>
        /// <param name="archivos">objeto de tipo lista con los archivos de un cliente</param>
        private void OperacionesArchivosPropietarios(IEnumerable<PUArchivosPropietario> archivos, int? IdPropietario = null)
        {
            foreach (PUArchivosPropietario archivo in archivos)
            {
                if (archivo.ArchivosPersonas.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                {
                    if (IdPropietario != null)
                        archivo.IdPropietario = IdPropietario.Value;
                    PURepositorio.Instancia.AdicionarArchivoPropietario(archivo);
                }
                if (archivo.ArchivosPersonas.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    PURepositorio.Instancia.EliminarArchivoPropietario(archivo);
            }
        }

        #endregion Archivos

        #endregion Propietarios (Concesionarios)

        #region Codeudor

        /// <summary>
        /// Obtiene lista de codeudores de un Centro de servicio
        /// </summary>
        /// <returns>Lista con los codeudores de un  Centro de servicio</returns>
        public IList<PUCodeudor> ObtenerCodeudoresXCentroServicio(long idCentroServicio, IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PURepositorio.Instancia.ObtenerCodeudoresXCentroServicio(idCentroServicio, filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Actualiza un codeudor
        /// </summary>
        /// <param name="codeudor">Objeto codeudor</param>
        public void ActualizarCodeudor(PUCodeudor codeudor)
        {
            if (PAAdministrador.Instancia.ValidarListaRestrictiva(codeudor.PersonaExterna.Identificacion))
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_ALERTA_LISTAS_RESTRICTIVAS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_ALERTA_LISTAS_RESTRICTIVAS)));

            switch (codeudor.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PURepositorio.Instancia.AdicionarCodeudor(codeudor);
                    break;

                case EnumEstadoRegistro.BORRADO:

                    break;

                case EnumEstadoRegistro.MODIFICADO:

                    //Se reutiliza el mismo metodo de editar responsable legal debido a que se afecta las mismas tablas de la misma forma
                    codeudor.EstadoRegistro = EnumEstadoRegistro.MODIFICADO;

                    PAResponsableLegal responsable = new PAResponsableLegal()
                    {
                        Email = codeudor.Email,
                        EmpresaEmpleador = codeudor.EmpresaEmpleador,
                        EstadoRegistro = EnumEstadoRegistro.MODIFICADO,
                        Fax = codeudor.Fax,
                        IngresosEmpleoActual = codeudor.IngresosEmpleoActual,
                        Ocupacion = codeudor.Ocupacion,
                        PoseeFincaRaiz = codeudor.PoseeFincaRaiz,
                        PersonaExterna = codeudor.PersonaExterna,
                        Telefono = codeudor.PersonaExterna.Telefono,
                        IdResponsable = codeudor.PersonaExterna.IdPersonaExterna
                    };

                    PAAdministrador.Instancia.ActualizarResponsableLegal(responsable);
                    break;
            }
        }

        #endregion Codeudor

        #region Consultas

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
            return PURepositorio.Instancia.ObtenerMunicipiosSinFormaPagoAlCobro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Agrega municipio a la lista de municipios que no admiten forma de pago "al cobro"
        /// </summary>
        /// <param name="municipio"></param>
        public void RegistrarMunicipioSinFormaPagoAlCobro(PALocalidadDC municipio)
        {
            PURepositorio.Instancia.RegistrarMunicipioSinFormaPagoAlCobro(municipio);
        }

        /// <summary>
        /// Quita municipio de la lista de municipios que no admiten forma de pago "al cobro"
        /// </summary>
        /// <param name="municipio"></param>
        public void RemoverMunicipioSinFormaPagoAlCobro(PALocalidadDC municipio)
        {
            PURepositorio.Instancia.RemoverMunicipioSinFormaPagoAlCobro(municipio);
        }

        /// <summary>
        /// Obtener la agencia a partir de la localida
        /// </summary>
        /// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad)
        {
            return PURepositorio.Instancia.ObtenerAgenciaLocalidad(localidad);
        }

        /// <summary>
        /// Método para obtener el representante legal de un punto
        /// </summary>
        /// <param name="idcentroservicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerRepresentanteLegalPunto(long idcentroservicio)
        {
            return PURepositorio.Instancia.ObtenerRepresentanteLegalPunto(idcentroservicio);
        }

        /// <summary>
        /// Otiene todos los tipos de descuento
        /// </summary>
        /// <returns>Lista con los tipos de descuento</returns>
        public IList<PUTiposDescuento> ObtenerTiposDescuento()
        {
            return PURepositorio.Instancia.ObtenerTiposDescuento();
        }

        /// <summary>
        /// Obtiene todos los servicios
        /// </summary>
        /// <returns>Lista con los servicios</returns>
        public IList<PUServicio> ObtenerServicios()
        {
            return PURepositorio.Instancia.ObtenerServicios();
        }

        /// <summary>
        /// Otiene todos los tipos de comision
        /// </summary>
        /// <returns>Lista con los tipos de comision</returns>
        public IList<PUTiposComision> ObtenerTiposComision()
        {
            return PURepositorio.Instancia.ObtenerTiposComision();
        }

        /// <summary>
        /// Obtiene todos los tipos de sociedad
        /// </summary>
        /// <returns>Lista con los tipos de sociedad</returns>
        public IList<PUTipoSociedad> ObtenerTipoSociedad()
        {
            return PURepositorio.Instancia.ObtenerTipoSociedad();
        }

        /// <summary>
        /// Obtiene todos los tipos de regimen tributario
        /// </summary>
        /// <returns>Lista con los tipos de regimen tributario</returns>
        public IList<PUTipoRegimen> ObtenerTiposRegimen()
        {
            return PURepositorio.Instancia.ObtenerTiposRegimen();
        }

        /// <summary>
        /// Valida que una agencia pueda realizar venta de mensajería y retorna  la lista de servicios de mensajería habilitados
        /// </summary>
        /// <param name="idCentroServicios"></param>
        public IEnumerable<TAServicioDC> ObtenerServiciosMensajeria(long idCentroServicios, int idListaPrecios)
        {
            // Los servicios de mensajería no son solo los servicios de mensajería sino también lo de carga.
            return PURepositorio.Instancia.ObtenerServiciosPorUnidadesDeNegocio(idCentroServicios, TAConstantesServicios.UNIDAD_MENSAJERIA, TAConstantesServicios.UNIDAD_CARGA, idListaPrecios);
        }

        /// <summary>
        /// Indica si el centro de servicio pasado como parámetro asociado tiene el servicio de Komprech
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public bool CentroServicioTieneServicioKomprechAsociado(long idCentroServicios)
        {
            return PURepositorio.Instancia.CentroServicioTieneServicioKomprechAsociado(idCentroServicios, TAConstantesServicios.UNIDAD_KOMPRECH, TAConstantesServicios.SERVICIO_KOMPRECH);
        }

        /// <summary>
        /// Valida que una agencia pueda realizar venta de mensajería y retorna  la lista de servicios de mensajería habilitados
        /// </summary>
        /// <param name="idCentroServicios"></param>
        public IEnumerable<TAServicioDC> ObtenerServiciosMensajeriaSinValidacionHorario(long idCentroServicios, int idListaPrecios)
        {
            // Los servicios de mensajería no son solo los servicios de mensajería sino también lo de carga.
            return PURepositorio.Instancia.ObtenerServiciosPorUnidadesDeNegocioSinValidacionHorario(idCentroServicios, TAConstantesServicios.UNIDAD_MENSAJERIA, TAConstantesServicios.UNIDAD_CARGA, idListaPrecios);
        }

        /// <summary>
        /// Actualiza la informació de validación de dos centros de servicios implicados en un trayecto
        /// </summary>
        /// <param name="localidadDestino">Localidad de destino del trayecto</param>
        /// <param name="idCentroServicio">Identificador del Centro de servicios que inicia la transacción</param>
        /// <param name="validacion">Contiene la información de las agencias implicadas en el proceso</param>
        public void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, long idCentroServicio, PALocalidadDC localidadOrigen = null)
        {
            PURepositorio.Instancia.ObtenerInformacionValidacionTrayectoAdo(localidadDestino, validacion, idCentroServicio, localidadOrigen);
        }

        /// <summary>
        /// Obtener información de validación del trayecto
        /// </summary>
        /// <param name="localidadOrigen"></param>
        /// <param name="idCentroServicioOrigen"></param>
        public void ObtenerInformacionValidacionTrayectoOrigen(PALocalidadDC localidadOrigen, ADValidacionServicioTrayectoDestino validacion)
        {
            PURepositorio.Instancia.ObtenerInformacionValidacionTrayectoOrigen(localidadOrigen, validacion);
        }

        /// <summary>
        /// Actualiza la información de validación de un trayecto establecido desde una sucursal de un cliente
        /// </summary>
        /// <param name="localidadDestino">Localidad de destino del envío</param>
        /// <param name="validacion">Contiene información con los reusltados de las validaciones relacionadas con el trayecto</param>
        /// <param name="idCliente">Identificador del cliente que ingresa</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        internal void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, int idCliente, int idSucursal)
        {
            PURepositorio.Instancia.ObtenerInformacionValidacionTrayecto(localidadDestino, validacion, idCliente, idSucursal);
        }

        /// <summary>
        /// Obtiene las agencias de la aplicación
        /// </summary>
        public List<PUCentroServiciosDC> ObtenerAgencias(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PURepositorio.Instancia.ObtenerAgencias(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }


        /// <summary>
        /// Obtener Agencias y Bodegas para Validación y Archivo - Control de Cuentas
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasBodegas(IDictionary<string, string> filtro)//, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PURepositorio.Instancia.ObtenerAgenciasBodegas(filtro);
        }


        /// <summary>
        /// Obtiene las agencias de la aplicación sin filtro
        /// </summary>
        public List<PUCentroServiciosDC> ObtenerAgencias()
        {
            return PURepositorio.Instancia.ObtenerAgencias();
        }

        /// <summary>
        /// Obtiene todas las opciones de clasificador canal ventas
        /// </summary>
        /// <returns></returns>
        public IList<PUClasificadorCanalVenta> ObtenerTodosClasificadorCanalVenta()
        {
            return PURepositorio.Instancia.ObtenerTodosClasificadorCanalVenta();
        }

        /// <summary>
        /// Retorna los centro de servicio que reportan a un centro de servicio dado
        /// </summary>
        /// <param name="idCentroServicio">Id del centro de servicio</param>
        /// <returns></returns>
        public List<PUCentroServicioReporte> ObtenerCentrosServicioReportanCentroServicio(long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerCentrosServicioReportanCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los COL de un Racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>lista de Col de un Racol</returns>
        public List<PUCentroServicioApoyo> ObtenerCentrosLogisticosRacol(long idRacol)
        {
            return PURepositorio.Instancia.ObtenerCentrosLogisticosRacol(idRacol);
        }

        /// <summary>
        /// Obtiene el centro de acopio de una bodega
        /// </summary>
        /// <param name="idBodega"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroDeAcopioBodega(long idBodega, long idUsuario)
        {
            return PURepositorio.Instancia.ObtenerCentroDeAcopioBodega(idBodega, idUsuario);
        }

        #endregion Consultas

        #region Servicio Giros

        /// <summary>
        /// Valida que una agencia pueda realizar la venta de un giro
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        public void ValidarAgenciaServicioGiros(long idCentroServicios)
        {
            if (ConstantesFramework.ESTADO_ACTIVO != PURepositorio.Instancia.ConsultarEstadoAgencia(idCentroServicios).IdEstado)
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_ACTIVO.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_ACTIVO)));

            if (!PURepositorio.Instancia.ConsultarPuedeRecibirGiros(idCentroServicios))
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_PUEDE_VENDER_GIROS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_PUEDE_VENDER_GIROS)));

            if (!PURepositorio.Instancia.ConsultarServiciosGirosEnAgencia(idCentroServicios, TAConstantesServicios.SERVICIO_GIRO))
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_TIENE_SERVICIO_GIROS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_TIENE_SERVICIO_GIROS)));
        }

        /// <summary>
        /// Valida que una agencia pueda realizar el pago de un giro
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        public void ValidarAgenciaServicioPagos(long idCentroServicios)
        {
            if (ConstantesFramework.ESTADO_ACTIVO != PURepositorio.Instancia.ConsultarEstadoAgencia(idCentroServicios).IdEstado)
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_ACTIVO.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_ACTIVO)));

            if (!PURepositorio.Instancia.ConsultarPuedePagarGiros(idCentroServicios, TAConstantesServicios.SERVICIO_GIRO))
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_PUEDE_PAGAR_GIROS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_PUEDE_VENDER_GIROS)));

            if (!PURepositorio.Instancia.ConsultarServiciosGirosEnAgencia(idCentroServicios, TAConstantesServicios.SERVICIO_GIRO))
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_TIENE_SERVICIO_GIROS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_TIENE_SERVICIO_GIROS)));
        }

        /// <summary>
        /// Consulta si el centro de servicios puede pagar giros
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public bool ObtenerCentroServiciosPuedePagarGiros(long idCentroServicios)
        {
            return PURepositorio.Instancia.ConsultarPuedePagarGiros(idCentroServicios, TAConstantesServicios.SERVICIO_GIRO);
        }

        /// <summary>
        /// Obtiene la lista de las agencias que pueden realizar pagos de giros
        /// </summary>
        public IList<PUCentroServiciosDC> ObtenerAgenciasPuedenPagarGiros()
        {
            return PURepositorio.Instancia.ObtenerAgenciasPuedenPagarGiros(TAConstantesServicios.SERVICIO_GIRO);
        }

        /// <summary>
        /// Retorna las agencias creadas en el sistemas que se encuentran activas
        /// </summary>
        /// <returns></returns>
        public List<PUAgencia> ObtenerAgenciasActivas()
        {
            return PURepositorio.Instancia.ObtenerAgenciasActivas();
        }

        /// <summary>
        /// Obtiene los datos básicos de los centros de servivios de giros
        /// </summary>
        /// <returns>Colección centros de servicio</returns>
        public IEnumerable<PUCentroServiciosDC> ObtenerCentrosServicioGiros()
        {
            return PURepositorio.Instancia.ObtenerCentrosServicioGiros(TAConstantesServicios.SERVICIO_GIRO);
        }

        /// <summary>
        /// Valida que el Centro de servicio no supere el valor maximo a enviar de giros y acumula el valor del giro a la agenci
        /// </summary>
        /// <param name="idCentroServicio"></param>
        public void AcumularVentaGirosAgencia(GIAdmisionGirosDC giro)
        {
            PURepositorio.Instancia.AcumularVentaGirosAgencia(giro);
        }

        /// <summary>
        /// Actualiza la informacin de una agencia para el servicio de giros
        /// </summary>
        /// <param name="centroServicio"></param>
        public void ActualizarConfiguracionGiros(PUCentroServiciosDC centroServicio)
        {
            PURepositorio.Instancia.ActualizarConfiguracionGiros(centroServicio);
        }

        /// <summary>
        /// Obtener las observaciones de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public IList<PUObservacionCentroServicioDC> ObtenerObservacionCentroServicio(long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerObservacionCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene la informacion de un centro se servicio por el id del centro de servicio
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerInformacionCentroServicioPorId(long idCentroServicios)
        {
            return PURepositorio.Instancia.ObtenerInformacionCentroServicioPorId(idCentroServicios);
        }

        /// <summary>
        /// Obtiene las direcciones de los puntos segun localidad destinatario que tienen habilitado Reclame en oficina.
        /// </summary>
        /// <param name="idLocalidadDestinatario"></param>
        /// <returns></returns>
        public List<string> ObtenerDireccionPuntosSegunLocalidadDestinatario(int idLocalidadDestinatario)
        {
            return PURepositorio.Instancia.ObtenerDireccionesPuntosSegunUbicacionDestino(idLocalidadDestinatario);
        }

        public List<PUCentroServicioApoyo> ObtenerPuntosREOSegunUbicacionDestino(int idLocalidadDestino)
        {
            return PURepositorio.Instancia.ObtenerPuntosREOSegunUbicacionDestino(idLocalidadDestino);
        }

        #endregion Servicio Giros

        #region Datos Bancarios

        /// <summary>
        /// Adiciona o modifica los datos bancarios de un agente comercial (propietario)
        /// </summary>
        /// <param name="datosBanca">Objeto datos bancarios</param>
        public void ActualizaInfoBancaria(PUPropietarioBanco datosBanca)
        {
            switch (datosBanca.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PURepositorio.Instancia.AdicionarInfoBancaria(datosBanca);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PURepositorio.Instancia.EditarInfoBancaria(datosBanca);
                    break;
            }
        }

        /// <summary>
        /// Obtiene la informacion bancaria de un agente comercial(propietario)
        /// </summary>
        /// <param name="idPropietario"></param>
        /// <returns></returns>
        public PUPropietarioBanco ObtenerDatosBancariosPropietario(int idPropietario)
        {
            return PURepositorio.Instancia.ObtenerDatosBancariosPropietario(idPropietario);
        }

        #endregion Datos Bancarios

        #region Centro de servicios

        /// <summary>
        /// Obtiene el centro de servicios y el responsable
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroServiciosPersonaResponsable(long idCentroServicios)
        {
            return PURepositorio.Instancia.ObtenerCentroServiciosPersonaResponsable(idCentroServicios);
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
        public IList<PUCentroServiciosDC> ObtenerCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int? idPropietario)
        {
            return PURepositorio.Instancia.ObtenerCentrosServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idPropietario);
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio que no son Racol con el servicio Komprech activado para una localidad dada
        /// </summary>
        /// <param name="idMunicipio">Id del municipio</param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasPuntosActivosKomprechPorLocalidad(string idMunicipio)
        {
            return PURepositorio.Instancia.ObtenerAgenciasPuntosActivosKomprechPorLocalidad(idMunicipio, TAConstantesServicios.SERVICIO_KOMPRECH);
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio activos por una localidad
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicioActivosLocalidad(string idMunicipio)
        {
            return PURepositorio.Instancia.ObtenerCentrosServicioActivosLocalidad(idMunicipio);
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio de una actividad
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServicioPorLocalidad(string idMunicipio)
        {
            return PURepositorio.Instancia.ObtenerTodosCentrosServicioPorLocalidad(idMunicipio);
        }

        /// <summary>
        /// obtener todos los tipos de ciudad
        /// </summary>
        /// <returns></returns>
        public List<PUTipoCiudad> ObtenerTiposCiudades()
        {
            return PURepositorio.Instancia.ObtenerTiposCiudades();
        }

        /// <summary>
        /// obtener todos los tipos de zona
        /// </summary>
        /// <returns></returns>
        public List<PUTipoZona> ObtenerTiposZona()
        {
            return PURepositorio.Instancia.ObtenerTiposZona();
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasPuntosActivosPorLocalidad(string idMunicipio)
        {
            return PURepositorio.Instancia.ObtenerAgenciasPuntosActivosPorLocalidad(idMunicipio);
        }

        /// <summary>
        /// Obtiene las agencias y los puntos
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
            return PURepositorio.Instancia.ObtenerCentrosServicioAgenciasPuntos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
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
            return PURepositorio.Instancia.ObtenerCentrosServicioReportan(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicioAQuienReportan);
        }

        /// <summary>
        /// Retorna la lista de centros de servicios activos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosActivos()
        {
            return PURepositorio.Instancia.ObtenerCentrosServiciosActivos();
        }

        /// <summary>
        /// Retorna la lista con todos los centros de servicios del sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServicios()
        {
            return PURepositorio.Instancia.ObtenerTodosCentrosServicios();
        }

        /// <summary>
        /// Obtener todos los coles activos
        /// </summary>
        /// <returns>Colección con los coles activos</returns>
        public List<PUCentroServiciosDC> ObtenerTodosColes()
        {
            return PURepositorio.Instancia.ObtenerTodosColes();
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
        public IList<PUCentroServiciosDC> ObtenerCentrosServicioPorLocalidad(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, string IdLocalidad, PUEnumTipoCentroServicioDC tipoCES)
        {
            List<PUCentroServiciosDC> listaCES = new List<PUCentroServiciosDC>();
            if (tipoCES != PUEnumTipoCentroServicioDC.PAS)
                listaCES = PURepositorio.Instancia.ObtenerCentrosServicioPorLocalidad(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdLocalidad).ToList();
            else
            {
                totalRegistros = 100;
                long idRacol = PURepositorio.Instancia.ObtenerRacoles().FirstOrDefault(ra => ra.IdMunicipio == IdLocalidad).IdCentroServicio;
                listaCES = PURepositorio.Instancia.ObtenerCentroServicioPASPorRacol(idRacol);
            }


            if (listaCES.Any())
                return listaCES;
            else
                throw new FaultException<ControllerException>
                  (new ControllerException(
                    COConstantesModulos.CENTRO_SERVICIOS,
                    EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(),
                   String.Format(MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS), IdLocalidad)));
        }



        public List<PUCentroServiciosDC> ObtenerCentroServicioPASPorRacol(long idRacol)
        {
            return PURepositorio.Instancia.ObtenerCentroServicioPASPorRacol(idRacol);
        }

        /// <summary>
        /// Adiciona modifica o elimina un centro de servicios
        /// </summary>
        /// <param name="descuentoRef">Objeto centro de servicios</param>
        public void ActualizarCentrosServicio(PUCentroServiciosDC centroServicios)
        {
            if (!string.IsNullOrWhiteSpace(centroServicios.IdCentroCostos))
                centroServicios.IdCentroCostos = centroServicios.IdCentroCostos.Trim();

            switch (centroServicios.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:

                    switch (centroServicios.Tipo)
                    {
                        case ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA:

                            if (centroServicios.IdTipoAgencia == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA || centroServicios.IdTipoAgencia == PUConstantesCentroServicios.TipoCentroServicio_AgenciaARO)
                            {
                                using (TransactionScope transaccion = new TransactionScope())
                                {
                                    long anteriorAgencia = 0;

                                    centroServicios.IdCentroServicio = PURepositorio.Instancia.AdicionarAgencia(centroServicios, ref anteriorAgencia);

                                    ///Heredar las sucursales de los clientes a la nueva agencia, desde una agencia inactiva
                                    if (anteriorAgencia > 0)
                                    {
                                        COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ModificarAgenciaResponsableSucursal(anteriorAgencia, centroServicios.IdCentroServicio);

                                        // todo: el arquitecto solicita comentariar este metodo por que esta presentando problemas con algunos centros de servicio al trasladar y modificar los suministros
                                        //    COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().ModificarSuministroAgencia(anteriorAgencia, centroServicios.IdCentroServicio);
                                    }
                                    PURepositorio.Instancia.AdicionarHorarios(centroServicios);

                                    //EMRL se agrega funcionalidad para guardar segun marca
                                    if (centroServicios.MisPendientes)
                                    {
                                        PURepositorio.Instancia.GuardarCentroServicioDistribucion(centroServicios.IdCentroServicio, centroServicios.CreadoPor);
                                    }                                    

                                    transaccion.Complete();
                                }
                                if (centroServicios.ArchivosCentroServicios != null)
                                    this.OperacionesArchivosCentrosServicios(centroServicios.ArchivosCentroServicios, centroServicios.IdCentroServicio);
                            }

                            if (centroServicios.IdTipoAgencia == ConstantesFramework.TIPO_CENTRO_SERVICIO_COL)
                                using (TransactionScope transaccion = new TransactionScope())
                                {
                                    centroServicios.IdCentroServicio = PURepositorio.Instancia.AdicionarCOL(centroServicios);
                                    PURepositorio.Instancia.AdicionarHorarios(centroServicios);
                                    transaccion.Complete();
                                }
                            if (centroServicios.ArchivosCentroServicios != null)
                                this.OperacionesArchivosCentrosServicios(centroServicios.ArchivosCentroServicios, centroServicios.IdCentroServicio);
                            break;

                        case ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO:
                            using (TransactionScope transaccion = new TransactionScope())
                            {
                                centroServicios.IdCentroServicio = PURepositorio.Instancia.AdicionarPunto(centroServicios);
                                PURepositorio.Instancia.AdicionarHorarios(centroServicios);
                                transaccion.Complete();
                            }
                            if (centroServicios.ArchivosCentroServicios != null)
                                this.OperacionesArchivosCentrosServicios(centroServicios.ArchivosCentroServicios, centroServicios.IdCentroServicio);
                            break;

                        case ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL:
                            using (TransactionScope transaccion = new TransactionScope())
                            {
                                centroServicios.IdCentroServicio = PURepositorio.Instancia.AdicionarRacol(centroServicios);
                                PURepositorio.Instancia.AdicionarHorarios(centroServicios);
                                transaccion.Complete();
                            }
                            if (centroServicios.ArchivosCentroServicios != null)
                                this.OperacionesArchivosCentrosServicios(centroServicios.ArchivosCentroServicios, centroServicios.IdCentroServicio);
                            break;
                    }

                    break;

                case EnumEstadoRegistro.BORRADO:
                    PURepositorio.Instancia.EliminarCentroServicio(centroServicios);
                    break;

                case EnumEstadoRegistro.MODIFICADO:

                    switch (centroServicios.Tipo)
                    {
                        case ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA:

                            if (centroServicios.IdTipoAgencia == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA || centroServicios.IdTipoAgencia == PUConstantesCentroServicios.TipoCentroServicio_AgenciaARO)
                            {
                                using (TransactionScope transaccion = new TransactionScope())
                                {
                                    PURepositorio.Instancia.EditarAgencia(centroServicios);
                                    PURepositorio.Instancia.EditarCentrosServicio(centroServicios);
                                    PURepositorio.Instancia.AdicionarHorarios(centroServicios);

                                    //EMRL se agrega funcionalidad para guardar segun marca
                                    if (centroServicios.MisPendientes)
                                    {
                                        PURepositorio.Instancia.GuardarCentroServicioDistribucion(centroServicios.IdCentroServicio, centroServicios.CreadoPor);
                                    }
                                    else
                                    {
                                        PURepositorio.Instancia.BorrarCentroServicioDistribucion(centroServicios.IdCentroServicio);
                                    }
                                    transaccion.Complete();
                                }
                                if (centroServicios.ArchivosCentroServicios != null)
                                    this.OperacionesArchivosCentrosServicios(centroServicios.ArchivosCentroServicios, centroServicios.IdCentroServicio);
                            }
                            if (centroServicios.IdTipoAgencia == ConstantesFramework.TIPO_CENTRO_SERVICIO_COL)
                                using (TransactionScope transaccion = new TransactionScope())
                                {
                                    PURepositorio.Instancia.EditarCOL(centroServicios);
                                    PURepositorio.Instancia.EditarCentrosServicio(centroServicios);
                                    PURepositorio.Instancia.AdicionarHorarios(centroServicios);
                                    transaccion.Complete();
                                }
                            if (centroServicios.ArchivosCentroServicios != null)
                                this.OperacionesArchivosCentrosServicios(centroServicios.ArchivosCentroServicios, centroServicios.IdCentroServicio);

                            break;

                        case ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO:
                            using (TransactionScope transaccion = new TransactionScope())
                            {
                                PURepositorio.Instancia.EditarPunto(centroServicios);
                                PURepositorio.Instancia.EditarCentrosServicio(centroServicios);
                                PURepositorio.Instancia.AdicionarHorarios(centroServicios);
                                transaccion.Complete();
                            }
                            if (centroServicios.ArchivosCentroServicios != null)
                                this.OperacionesArchivosCentrosServicios(centroServicios.ArchivosCentroServicios, centroServicios.IdCentroServicio);
                            break;

                        case ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL:
                            using (TransactionScope transaccion = new TransactionScope())
                            {
                                PURepositorio.Instancia.EditarRacol(centroServicios);
                                PURepositorio.Instancia.EditarCentrosServicio(centroServicios);
                                PURepositorio.Instancia.AdicionarHorarios(centroServicios);
                                transaccion.Complete();
                            }
                            if (centroServicios.ArchivosCentroServicios != null)
                                this.OperacionesArchivosCentrosServicios(centroServicios.ArchivosCentroServicios, centroServicios.IdCentroServicio);
                            break;
                    }

                    break;
            }
        }

        /// <summary>
        /// Metodo que permite inhabilitar al los usuarios de un centro de servicio en especifico que se quiera inhabilitar
        /// </summary>
        /// <param name="idCentroServicio"> recibe el id del centro de servicio seleciionado por el usuario</param>
        public void InhabilitarUsuariosCentroServicio(long idCentroServicio)
        {

            PURepositorio.Instancia.InhabilitarUsuariosCentroServicio(idCentroServicio);

        }

        /// <summary>
        /// Metodo que consulta todas las agencias y puntos de un RACOL
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasYPuntosRacolActivos(long idRacol)
        {
            return PURepositorio.Instancia.ObtenerAgenciasYPuntosRacolActivos(idRacol);
        }

        /// <summary>
        /// obtiene el centro de servicio Adscrito a un racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroServicioAdscritoRacol(long idRacol, long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerCentroServicioAdscritoRacol(idRacol, idCentroServicio);
        }

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasRacolActivos(long idRacol)
        {
            return PURepositorio.Instancia.ObtenerAgenciasRacolActivos(idRacol);
        }

        /// <summary>
        /// Consulta los tipos de agencia
        /// </summary>
        /// <returns>Lista con los tipos de agencia</returns>
        public IList<PUTipoAgencia> ObtenerTiposAgencia()
        {
            return PURepositorio.Instancia.ObtenerTiposAgencia();
        }

        /// <summary>
        /// Obtiene los tipos de propiedad
        /// </summary>
        /// <returns></returns>
        public IList<PUTipoPropiedad> ObtenerTiposPropiedad()
        {
            return PURepositorio.Instancia.ObtenerTiposPropiedad();
        }

        /// <summary>
        /// Obtiene el id y la descripcion de todos los centros logisticos activos y racol activos
        /// </summary>
        /// <returns>lista de centros logisticos y racol </returns>
        public IList<PUCentroServicioApoyo> ObtenerCentrosServicioApoyo()
        {
            return PURepositorio.Instancia.ObtenerCentrosServicioApoyo();
        }


        /// <summary>
        /// Obtiene Id y Descripción de las Territoriales
        /// </summary>
        /// <returns></returns>
        public IList<PUTerritorialDC> ObtenerTerritoriales()
        {
            return PURepositorio.Instancia.ObtenerTerritoriales();
        }

        /// <summary>
        /// Obtiene Lista de Centros de costo de NOVASOFT activos
        /// </summary>
        /// <returns>Lista de Centros de costo de NOVASOFT activos</returns>
        public IList<string> ObtenerCenCostoNovasoft()
        {
            return PURepositorio.Instancia.ObtenerCenCostoNovasoft();
        }


        /// <summary>
        /// Obtuene el centro de Servicio Activo
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public bool ObtenerCentroServicioActivo(long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerCentroServicioActivo(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los estados para los centros de servicio
        /// </summary>
        /// <returns></returns>
        public IList<PUEstadoDC> ObtenerEstados()
        {
            IList<PUEstadoDC> lstEstados = PURepositorio.Instancia.ObtenerEstados();
            lstEstados.Add(new PUEstadoDC() { Descripcion = MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.IN_ESTADO_LIQUIDACION), IdEstado = PUConstantesCentroServicios.EstadoLiquidacion });
            lstEstados.OrderBy(obj => obj.Descripcion);
            return lstEstados;
        }

        /// <summary>
        /// Obtiene todas las listas necesarias para parametrizar los centros de servicio
        /// </summary>
        /// <returns>Objeto con las lista requeridas en los centros de servicio</returns>
        public PUListasCentrosServicio ObtenerListasCentrosServicio()
        {
            PUListasCentrosServicio listas = new PUListasCentrosServicio()
            {
                CentrosServiciosApoyo = this.ObtenerCentrosServicioApoyo(),
                Estados = this.ObtenerEstados(),
                TiposAgencia = this.ObtenerTiposAgencia(),
                TiposPropiedad = this.ObtenerTiposPropiedad(),
                TiposCentroServicio = this.ObtenerTipoCentroServicio(),
                ClasificadoresCanalVenta = this.ObtenerTodosClasificadorCanalVenta().ToList()
            };

            return listas;
        }


        /// <summary>
        /// Obtiene los tipos de centro de servicio
        /// </summary>
        /// <returns>Lista con los tipos de centro de servicio</returns>
        public IList<PUTipoCentroServicio> ObtenerTipoCentroServicio()
        {
            List<PUTipoCentroServicio> lstTipoCentroServicio = new List<PUTipoCentroServicio>();

            lstTipoCentroServicio.Add(new PUTipoCentroServicio()
            {
                Descripcion = MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_RACOL),
                IdTipo = ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL
            });

            lstTipoCentroServicio.Add(new PUTipoCentroServicio()
            {
                Descripcion = MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_AGENCIA),
                IdTipo = ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA
            });

            lstTipoCentroServicio.Add(new PUTipoCentroServicio()
            {
                Descripcion = MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_PUNTO),
                IdTipo = ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO
            });

            return lstTipoCentroServicio;
        }

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// y puntos de atención de los Racoles
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return PURepositorio.Instancia.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Obtiene los Centros de Servicios Activos e Inactivos de una Racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosTodos(long idRacol)
        {
            return PURepositorio.Instancia.ObtenerCentrosServiciosTodos(idRacol);
        }

        /// <summary>
        /// Consulta todos los RACOL activos
        /// </summary>
        /// <returns>Colección con los RACOL activos</returns>
        public IList<PUCentroServiciosDC> ObtenerObtenerCentrosServiciosDeRacolYTodosRacol(long idRacol)
        {
            IList<PUCentroServiciosDC> resultado = ObtenerCentrosServicios(idRacol);

            resultado = resultado
              .Union(PURepositorio.Instancia.ObtenerRacoles())
              .OrderBy(o => o.Nombre)
              .ToList();

            return resultado;
        }

        /// <summary>
        /// Obtiene los centros de servicio que tienen giros pendientes a tranasmitir
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentroserviciosGirosATransmitir(long idRacol)
        {
            return PURepositorio.Instancia.ObtenerCentroserviciosGirosATransmitir(idRacol);
        }

        /// <summary>
        /// Obtiene lista de los horarios de un centro de servicio
        /// </summary>
        /// <returns></returns>
        public System.Collections.ObjectModel.ObservableCollection<PADia> ObtienerHorariosCentroServicios(long idCentroServicios)
        {
            var dias = PURepositorio.Instancia.ObtienerHorariosCentroServicios(idCentroServicios).ToList();

            System.Collections.ObjectModel.ObservableCollection<PADia> ObserDias = new System.Collections.ObjectModel.ObservableCollection<PADia>();

            dias.ForEach(d =>
            {
                ObserDias.Add(d);
            });

            return ObserDias;
        }

        /// <summary>
        /// Metodo para obtener los RACOLs
        /// </summary>
        /// <returns></returns>
        public List<PURegionalAdministrativa> ObtenerRegionalAdministrativa()
        {
            return PURepositorio.Instancia.ObtenerRegionalAdministrativa();
        }

        /// <summary>
        /// Metodo para Obtener la RACOL de un municipio
        /// </summary>
        /// <returns></returns>
        public PURegionalAdministrativa ObtenerRegionalAdministrativa(string idMunicipio)
        {
            return PURepositorio.Instancia.ObtenerRegionalAdministrativa(idMunicipio);
        }

        /// <summary>
        /// Obtiene el centro de servicio.
        /// para el valor de la BaseInicial
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene la Agencia responsable del Punto
        /// </summary>
        /// <param name="idPuntoServicio">el id punto servicio.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObtenerAgenciaResponsable(long idPuntoServicio)
        {
            return PURepositorio.Instancia.ObtenerAgenciaResponsable(idPuntoServicio);
        }

        /// <summary>
        /// obtiene tipo y centro servicio responsable de centro servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerTipoYResponsableCentroServicio(long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerTipoYResponsableCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene el responsable segun el tipo del Cerntro de Svr
        /// </summary>
        /// <param name="idCentroSrv"></param>
        /// <param name="tipoCentroSrv"></param>
        /// <returns>info del centro de servicio responsable</returns>
        public PUAgenciaDeRacolDC ObtenerResponsableCentroSrvSegunTipo(long idCentroSrv, string tipoCentroSrv)
        {
            switch (tipoCentroSrv)
            {
                case ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO:
                    return ObtenerAgenciaResponsable(idCentroSrv);

                case ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA:
                    return ObtenerCOLResponsable(idCentroSrv).infoResponsable;

                case ConstantesFramework.TIPO_CENTRO_SERVICIO_COL:
                    return ObtenerRacolResponsable(idCentroSrv);

                default:
                    PUCentroServiciosDC infoCentroSrv = ObtenerCentroServicio(idCentroSrv);

                    PUAgenciaDeRacolDC infoResponsable = new PUAgenciaDeRacolDC()
                    {
                        IdCentroServicio = infoCentroSrv.IdCentroServicio,
                        NombreCentroServicio = infoCentroSrv.Nombre,
                        IdResponsable = infoCentroSrv.IdCentroServicio,
                        NombreResponsable = infoCentroSrv.Nombre
                    };
                    return infoResponsable;
            };
        }

        /// <summary>
        /// Obtiene el centro de servicio responsable de un centro servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public long ObtenerCentroLogisticoResponsable(long idCentroServicio)
        {
            PUCentroServiciosDC Col = new PUCentroServiciosDC();
            var centroServicio = PURepositorio.Instancia.ObtenerCentroServicio(idCentroServicio);
            if (centroServicio.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO)
            {
                Col = PURepositorio.Instancia.ObtenerAgenciaColCentroServicios(idCentroServicio);
            }
            else if (centroServicio.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA)
            {
                Col = PURepositorio.Instancia.ObtenerAgenciaColAgencia(idCentroServicio);
            }
            else if (centroServicio.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL)
                return centroServicio.IdCentroServicio;

            return Col.IdColRacolApoyo.Value;
        }

        /// <summary>
        /// Obtiene el centro de servicio responsable de un centro servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroLogisticoResponsableCentroServicio(long idCentroServicio)
        {
            PUCentroServiciosDC Col = new PUCentroServiciosDC();
            var centroServicio = PURepositorio.Instancia.ObtenerCentroServicio(idCentroServicio);
            if (centroServicio.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO)
            {
                Col = PURepositorio.Instancia.ObtenerAgenciaColCentroServicios(idCentroServicio);
            }
            else if (centroServicio.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA)
            {
                Col = PURepositorio.Instancia.ObtenerAgenciaColAgencia(idCentroServicio);
            }
            else if (centroServicio.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL)
            {
                centroServicio.IdColRacolApoyo = centroServicio.IdCentroServicio;
                return centroServicio;
            }

            return Col;
        }

        /// <summary>
        /// Obtiene el centro de servicio responsable de un centro servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCOLResponsable(long idCentroServicio)
        {
            PUCentroServiciosDC Col = new PUCentroServiciosDC();
            var centroServicio = PURepositorio.Instancia.ObtenerCentroServicio(idCentroServicio);
            if (centroServicio.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO)
            {
                Col = PURepositorio.Instancia.ObtenerAgenciaColCentroServicios(idCentroServicio);
            }
            else if (centroServicio.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA)
            {
                Col = PURepositorio.Instancia.ObtenerAgenciaColAgencia(idCentroServicio);
            }

            return Col;
        }


        /// <summary>
        /// Obtiene el centro de servicio responsable de un municipio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCOLResponsableMunicipio(string idMunicipío)
        {
            return PURepositorio.Instancia.ObtenerCOLResponsableMunicipio(idMunicipío);
        }

        /// <summary>
        /// Obtiene el centro de servicio responsable de una agencia
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroLogisticoAgencia(long idAgencia)
        {
            return PURepositorio.Instancia.ObtenerAgenciaColAgencia(idAgencia);
        }

        /// <summary>
        /// Obtiene el Racol responsable de la Agencia.
        /// </summary>
        /// <param name="idAgencia">el id agencia.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObtenerRacolResponsable(long idAgencia)
        {
            PUAgenciaDeRacolDC ces = PURepositorio.Instancia.ObteneRacolPropietarioBodega(idAgencia);
            if (ces != null)
            {
                return ces;
            }
            else
            {
                return PURepositorio.Instancia.ObtenerRacolResponsable(idAgencia);
            }
        }

        /// <summary>
        /// Obtiene el Racol responsable de la Agencia.
        /// </summary>
        /// <param name="idAgencia">el id agencia.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObteneColPropietarioBodega(long idBodega)
        {
            return PURepositorio.Instancia.ObteneColPropietarioBodega(idBodega);

        }


        /// <summary>
        /// Metodo para consultar las localidades donde existen centros logisticos
        /// </summary>
        /// <returns></returns>
        public IList<LILocalidadColDC> ObtenerLocalidadesCol()
        {
            return PURepositorio.Instancia.ObtenerLocalidadesCol();
        }

        /// <summary>
        /// Obtiene los puntos del centro logistico
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosServiciosCol(long idCol)
        {
            return PURepositorio.Instancia.ObtenerPuntosServiciosCol(idCol);
        }

        /// <summary>
        /// Metodo para consultar las agencias que dependen de un COL
        /// </summary>
        /// <returns></returns>
        public IList<LILocalidadColDC> ObtenerAgenciasCol(long idCol)
        {
            return PURepositorio.Instancia.ObtenerAgenciasCol(idCol);
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerPuntosDeAgencia(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgenciaActivos(long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerPuntosDeAgenciaActivos(idCentroServicio);
        }

        /// <summary>
        /// Retorna la lista de puntos y agencias dependientes de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasDependientes(long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerPuntosAgenciasDependientes(idCentroServicio);
        }

        /// <summary>
        /// Obtener centros de servicios y racol
        /// </summary>
        /// <returns></returns>
        public List<PUAgenciaDeRacolDC> ObtenerCentroServiciosRacol()
        {
            return PURepositorio.Instancia.ObtenerCentroServiciosRacol();
        }

        /// <summary>
        /// Obtiene los centros logisticos
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioApoyo> ObtenerCentroLogistico()
        {
            return PURepositorio.Instancia.ObtenerCentroLogistico();
        }

        /// <summary>
        /// Obtiene el col responsable de la agencia de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtieneCOLResponsableAgenciaLocalidad(string idLocalidad)
        {
            PUCentroServiciosDC agencia = this.ObtenerAgenciaLocalidad(idLocalidad);
            PUCentroServiciosDC col = this.ObtenerCentroLogisticoAgencia(agencia.IdCentroServicio);
            col = this.ObtenerCentroServicio(col.IdColRacolApoyo.Value);
            return col;
        }

        public PUCentroServiciosDC ObtieneCOLPorLocalidad(string idLocalidad)
        {
            PUCentroServiciosDC agencia = this.ObtenerAgenciaLocalidad(idLocalidad);
            return this.ObtenerCentroLogisticoAgencia(agencia.IdCentroServicio);
        }

        /// <summary>
        /// Obtiene el id del col responsable de la agencia de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public long ObtieneIdCOLResponsableAgenciaLocalidad(string idLocalidad)
        {
            PUCentroServiciosDC agencia = this.ObtenerAgenciaLocalidad(idLocalidad);
            PUCentroServiciosDC col = this.ObtenerCentroLogisticoAgencia(agencia.IdCentroServicio);
            return col.IdColRacolApoyo.Value;
        }

        /// <summary>
        /// Adiciona el centro de servicios al que se le reporta
        /// </summary>
        /// <param name="idCentroServicioAQuienReporta"></param>
        /// <param name="idCentroServicioReporta"></param>
        public void AdicionarCentroServicioReporte(long idCentroServicioAQuienReporta, long idCentroServicioReporta)
        {
            PURepositorio.Instancia.AdicionarCentroServicioReporte(idCentroServicioAQuienReporta, idCentroServicioReporta);
        }

        /// <summary>
        /// Eliminar el centro de servicios al que se le reporta
        /// </summary>
        /// <param name="idCentroServicioAQuienReporta"></param>
        /// <param name="idCentroServicioReporta"></param>
        public void EliminarCentroServicioReporte(long idCentroServicioAQuienReporta, long idCentroServicioReporta)
        {
            PURepositorio.Instancia.EliminarCentroServicioReporte(idCentroServicioAQuienReporta, idCentroServicioReporta);
        }

        /// <summary>
        /// Obtiene el horario de la recogida de un centro de Servicio
        /// </summary>
        /// <param name="idCentroSvc">es le id del centro svc</param>
        /// <returns>info de la recogida</returns>
        public IList<PUHorarioRecogidaCentroSvcDC> ObtenerHorariosRecogidasCentroSvc(long idCentroSvc)
        {
            return PURepositorio.Instancia.ObtenerHorariosRecogidasCentroSvc(idCentroSvc);
        }

        /// <summary>
        /// obtiene todos los centros servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicio()
        {
            return PURepositorio.Instancia.ObtenerCentrosServicio();
        }

        public List<PUCentroServiciosDC> ObtenerCentrosServicioTipo()
        {
            return PURepositorio.Instancia.ObtenerCentrosServicioTipo();
        }

        /// <summary>
        /// Adiciona los Horarios de las recogidas
        /// de los centros de Svc
        /// </summary>
        /// <param name="centroServicios">info del Centro de Servicio</param>
        public void AdicionarHorariosRecogidasCentroSvc(PUCentroServiciosDC centroServicios)
        {
            PURepositorio.Instancia.AdicionarHorariosRecogidasCentroSvc(centroServicios);
        }

        /// <summary>
        /// Obtiene la informacion de una agencia dependiendo del id
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerAgencia(long idAgencia)
        {
            return PURepositorio.Instancia.ObtenerAgencia(idAgencia);
        }

        #region Archivos

        /// <summary>
        /// Obtiene lista con los archivos de los centros de servicio
        /// </summary>
        /// <returns>objeto de centro de servicio</returns>
        public IEnumerable<PUArchivoCentroServicios> ObtenerArchivosCentroServicios(PUCentroServiciosDC centroServicios)
        {
            return PURepositorio.Instancia.ObtenerArchivosCentroServicios(centroServicios);
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un centro Servicio
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoCentroServicio(PUArchivoCentroServicios archivo)
        {
            return PURepositorio.Instancia.ObtenerArchivoCentroServicio(archivo);
        }

        /// <summary>
        /// Adiciona o elimina los archivos de un cliente
        /// </summary>
        /// <param name="archivos">objeto de tipo lista con los archivos de un cliente</param>
        private void OperacionesArchivosCentrosServicios(IEnumerable<PUArchivoCentroServicios> archivos, long idCentroServicios)
        {
            foreach (PUArchivoCentroServicios archivo in archivos)
            {
                if (archivo.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    PURepositorio.Instancia.AdicionarArchivoCentroServicio(archivo, idCentroServicios);
                if (archivo.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    PURepositorio.Instancia.EliminarArchivoCentroServicio(archivo);
            }
        }

        #endregion Archivos

        #region Suministros

        /// <summary>
        /// Obtiene suministros de un centro de servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista suministros del centro de servicio</returns>
        public IList<PUCentroServiciosSuministro> ObtenerSuministrosPorCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServicios)
        {
            return PURepositorio.Instancia.ObtenerSuministrosPorCentrosServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios);
        }

        /// <summary>
        /// Obtiene todos los suministros
        /// </summary>
        /// <returns></returns>
        public IList<PUSuministro> ObtenerTodosSuministros()
        {
            return PURepositorio.Instancia.ObtenerTodosSuministros();
        }

        /// <summary>
        /// Adiciona modifica o elimina un centro de servicios
        /// </summary>
        /// <param name="descuentoRef">Objeto centro de servicios</param>
        public void ActualizarCentroServiciosSuministros(PUCentroServiciosSuministro PUCentroServiciosSuministro)
        {
            switch (PUCentroServiciosSuministro.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PURepositorio.Instancia.AdicionarCentroServiciosSuministro(PUCentroServiciosSuministro);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PURepositorio.Instancia.EliminarCentroServiciosSuministro(PUCentroServiciosSuministro);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PURepositorio.Instancia.EditarCentroServiciosSuministro(PUCentroServiciosSuministro);
                    break;
            }
        }

        #endregion Suministros

        #region Divulgacion de agencias

        /// <summary>
        /// Envia la divulgacion de una agencia
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="divulgacion">Objeto con la informacion de los contactos a divulgar la agencia</param>
        public void DivulgarAgencia(long idCentroServicios, PADivulgacion divulgacion)
        {
            int TotalReg;

            int IdPlantilla = int.Parse(PAAdministrador.Instancia.ConsultarParametrosFramework("IdPlantDivuCentroSrv"));

            byte[] Plantilla = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerPlantillaFramework(IdPlantilla);

            if (Plantilla == null)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_PLANTILLA_DIVULGACION_NO_CONFIGURADA.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_PLANTILLA_DIVULGACION_NO_CONFIGURADA)));
            }

            string pathArchivos = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivosDivulgacion);

            pathArchivos = Path.Combine(pathArchivos, COConstantesModulos.CENTRO_SERVICIOS);
            string nombrePlantilla = Path.Combine(pathArchivos, "P_" + Guid.NewGuid().ToString() + ".docx");

            if (!Directory.Exists(pathArchivos))
            {
                Directory.CreateDirectory(pathArchivos);
            }

            File.WriteAllBytes(nombrePlantilla, Plantilla);

            PUCentroServiciosDC InfoCentroSer = PURepositorio.Instancia.ObtenerCentrosServicio(new Dictionary<string, string>(), "", 0, 10, true, out TotalReg, null, idCentroServicios).ToList().FirstOrDefault();
            List<PUHorarioRecogidaCentroSvcDC> Horarios = PURepositorio.Instancia.ObtenerHorariosRecogidasCentroSvc(idCentroServicios).OrderBy(h => h.NombreDia).ToList();

            List<CMServiciosCentroServicios> Servicios = PURepositorio.Instancia.ObtenerServiciosCentroServicios(idCentroServicios).ToList();

            InformacionAlerta informacionAlerta = PARepositorio.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_DIVULGACION_CENTRO_SERVICIOS);

            Dictionary<string, string> valores = new Dictionary<string, string>();

            valores.Add("FechaDivulgacion", DateTime.Now.ToShortDateString());
            if (InfoCentroSer != null)
            {
                PUPropietarioBanco infoBancaria = PURepositorio.Instancia.ObtenerDatosBancariosPropietario(InfoCentroSer.IdPropietario);
                valores.Add("Codigo", InfoCentroSer.IdCentroServicio.ToString());
                valores.Add("NombreCiudad", InfoCentroSer.NombreMunicipio);
                valores.Add("NitRut", InfoCentroSer.IdentificacionPropietario);
                valores.Add("RepresentanteLeg", InfoCentroSer.NombrePersonaResponsable);
                valores.Add("IdentificacionRepLeg", InfoCentroSer.IdentificacionPersonaResponsable);
                valores.Add("Direccion", InfoCentroSer.Direccion);
                valores.Add("Telefono", InfoCentroSer.Telefono1);
                valores.Add("Celular", InfoCentroSer.CelularPersonaResponsable);
                valores.Add("Email", InfoCentroSer.Email);
                PUCentroServiciosDC responsable = this.ObtenerCOLResponsable(InfoCentroSer.IdCentroServicio);
                string nombreColResponsable = "";

                if (responsable != null && responsable.infoResponsable != null)
                    nombreColResponsable = responsable.infoResponsable.NombreResponsable;
                valores.Add("ColPertenece", nombreColResponsable);
                valores.Add("FechaInicio", InfoCentroSer.Fecha.ToString("dd/MM/yyyy"));

                if (infoBancaria != null)
                {
                    valores.Add("Banco", infoBancaria.NombreBanco);
                    valores.Add("TipoCuenta", infoBancaria.TipoCuenta);
                    valores.Add("NumeroCuenta", infoBancaria.NumeroCuenta.ToString());
                    valores.Add("TitularCuenta", infoBancaria.TitularCuenta);
                    valores.Add("NumeroCedula", infoBancaria.Identificacion);
                }
                valores.Add("ValorBasico", InfoCentroSer.BaseInicialCaja.ToString("C"));
            }
            DataSet dataset = new DataSet();
            dataset.Tables.Add("SERVICIOS");
            dataset.Tables["SERVICIOS"].Columns.Add("Nombre");
            dataset.Tables["SERVICIOS"].Columns.Add("UnidadNegocio");

            dataset.Tables.Add("HORARIOS");
            dataset.Tables["HORARIOS"].Columns.Add("NombreDia");
            dataset.Tables["HORARIOS"].Columns.Add("Hora");

            Servicios.ForEach(s =>
            {
                dataset.Tables["SERVICIOS"].Rows.Add(s.Servicios.Nombre, s.Servicios.UnidadNegocio);
            });

            Horarios.ForEach(h =>
            {
                dataset.Tables["HORARIOS"].Rows.Add(h.NombreDia, h.Hora.ToShortTimeString());
            });

            string archivoSalida = Path.Combine(pathArchivos, "S_" + Guid.NewGuid().ToString() + ".docx");
            DocumentosWordMerge.CombinarPlantilla(nombrePlantilla, archivoSalida, dataset, valores);

            string destinatarios = string.Empty;

            if (divulgacion.DestinatariosAdicionales != null)
                divulgacion.DestinatariosAdicionales.ToList().ForEach(d =>
                {
                    destinatarios = destinatarios != string.Empty ? string.Join(",", destinatarios, d) : d;
                });

            divulgacion.Grupos.ToList().ForEach(g =>
            {
                if (g.Seleccionado)
                    destinatarios = destinatarios != string.Empty ? string.Join(",", destinatarios, g.CorrerosDestinatarios) : g.CorrerosDestinatarios;
            });
            if (string.IsNullOrWhiteSpace(destinatarios))
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CLIENTES, ETipoErrorFramework.EX_NO_EXISTEN_DESTINATARIOS_CONFIGURADOS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_EXISTEN_DESTINATARIOS_CONFIGURADOS)));

            CorreoElectronico.Instancia.EnviarAdjunto(destinatarios, informacionAlerta.Asunto, informacionAlerta.Mensaje, archivoSalida);
        }

        #endregion Divulgacion de agencias

        /// <summary>
        /// Guarda en la base de datos el municipio con su respectivo centro logistico de apoyo
        /// </summary>
        /// <param name="municipioCol"></param>
        public void GuardarMunicipioCol(PUMunicipioCentroLogisticoDC municipioCol)
        {
            PURepositorio.Instancia.GuardarMunicipioCol(municipioCol);
        }

        /// <summary>
        /// Validar si una ciudad se apoya en un col
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <param name="IdCol"></param>
        /// <returns></returns>
        public bool ValidarCiudadSeApoyaCOL(string idLocalidad, long idCol)
        {
            return PURepositorio.Instancia.ValidarCiudadSeApoyaCOL(idLocalidad, idCol);
        }

        #endregion Centro de servicios

        #region Validación Suministros

        /// <summary>
        /// Metodo para validar la provisión de un suministro en un centro de servicio
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="idSuministro"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns> objeto de tipo suministro </returns>
        public SUSuministro ValidarSuministroSerial(long serial, int idSuministro, long idCentroServicio)
        {
            return PURepositorio.Instancia.ValidarSuministroSerial(serial, idSuministro, idCentroServicio);
        }

        #endregion Validación Suministros

        #region Clasificador canal de venta

        /// <summary>
        /// Inserta un nuevo clasificador de canal de venta
        /// </summary>
        /// <param name="clasificadorCanalVenta"></param>
        public void AgregarClasificarCanalVenta(PUClasificadorCanalVenta clasificadorCanalVenta)
        {
            PURepositorio.Instancia.AgregarClasificarCanalVenta(clasificadorCanalVenta);
        }

        /// <summary>
        /// Modifica un clasificador de canal de venta
        /// </summary>
        /// <param name="clasificadorCanalVenta"></param>
        public void ModificarClasificadorCanalVenta(PUClasificadorCanalVenta clasificadorCanalVenta)
        {
            PURepositorio.Instancia.ModificarClasificadorCanalVenta(clasificadorCanalVenta);
        }

        /// <summary>
        /// Borra un clasificador de canal de ventas
        /// </summary>
        /// <param name="clasificadorCanalVenta"></param>
        public void BorrarClasificadorCanalVenta(PUClasificadorCanalVenta clasificadorCanalVenta)
        {
            PURepositorio.Instancia.BorrarClasificadorCanalVenta(clasificadorCanalVenta);
        }

        /// <summary>
        /// Obtiene los clasificadores del canal de ventas
        /// </summary>
        public List<PUClasificadorCanalVenta> ObtenerClasificadorCanalVenta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PURepositorio.Instancia.ObtenerClasificadorCanalVenta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Selecciona todos los tipos de centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUTipoCentroServicio> ObtenerTodosTipoCentroServicio()
        {
            return PURepositorio.Instancia.ObtenerTodosTipoCentroServicio();
        }

        #endregion Clasificador canal de venta

        #region Autorizacion Suministros

        /// <summary>
        /// Guardar los suministros que posee un centro de servicio
        /// </summary>
        /// <param name="suministroCentroServicio"></param>
        public void GuardarSuministroCentroServicio(SUSuministroCentroServicioDC suministroCentroServicio)
        {
            PUAutorizadorSuministros.Instancia.GuardarSuministroCentroServicio(suministroCentroServicio);
        }

        /// <summary>
        /// Obtienen todos los municipios de un racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>municipios del racol</returns>
        public List<PALocalidadDC> ObtenerMunicipiosDeRacol(long idRacol)
        {
            return PURepositorio.Instancia.ObtenerMunicipiosDeRacol(idRacol);
        }

        #endregion Autorizacion Suministros

        #region Casa Matriz

        /// <summary>
        /// Obtener la información basica de las Regionales Administrativas activas de una Casa Matriz
        /// </summary>
        /// <param name="idCasaMatriz">Identificador de la Casa Matriz</param>
        /// <returns>Colección con la información básica de las regionales</returns>
        public IList<PURegionalAdministrativa> ObtenerRegionalesDeCasaMatriz(short idCasaMatriz)
        {
            return PURepositorio.Instancia.ObtenerRegionalesDeCasaMatriz(idCasaMatriz);
        }

        #endregion Casa Matriz

        public List<string> ConsultarRacoles()
        {
            return PURepositorio.Instancia.ConsultarRacoles();

        }

        /// <summary>
        /// Obtiene todas las agencias y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosAgenciayPuntosActivos()
        {
            return PURepositorio.Instancia.ObtenerTodosAgenciayPuntosActivos();
        }

        /// <summary>
        /// Obtiene todas las agencias, col y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosAgenciaColPuntosActivos()
        {
            return PURepositorio.Instancia.ObtenerTodosAgenciaColPuntosActivos();

        }


        #region Ingreso Custodia

        /// <summary>
        /// ingresar a custodia la guia
        /// </summary>                
        public void IngresoCustodia(PUCustodia Custodia, List<string> Adjuntos)
        {
            throw new Exception("ahora se utiliza adicionarCustodia");
        }

        #endregion Casa Matriz

        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasColReclamaOficina(long idCol)
        {
            return PURepositorio.Instancia.ObtenerPuntosAgenciasColReclamaOficina(idCol);
        }


        /// <summary>
        /// Obtiene los horarios de un centro de servicios para la app de recogidas
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<string> ObtenerHorariosCentroServicioAppRecogidas(long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerHorariosCentroServicioAppRecogidas(idCentroServicio);
        }

        #region  Custodia

        /// <summary>
        /// ingresar a custodia la guia
        /// </summary>                




        #region Ingreso Custodia

        /// <summary>
        /// ingresar a custodia la guia
        /// </summary>                
        public bool AdicionarCustodia(PUCustodia Custodia)
        {

            if (PUEnumTipoMovimientoInventario.Asignacion == PURepositorio.Instancia.ConsultaGuiaAsignada(Custodia))
            {
                Custodia.MovimientoInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
                Custodia.MovimientoInventario.NumeroGuia = Custodia.MovimientoInventario.NumeroGuia;
                Custodia.MovimientoInventario.FechaGrabacion = DateTime.Now;
                Custodia.MovimientoInventario.FechaEstimadaIngreso = DateTime.Now;
                Custodia.MovimientoInventario.CreadoPor = ControllerContext.Current.Usuario;
                Custodia.MovimientoInventario.Bodega = PURepositorio.Instancia.ObtenerCentroServicio(Custodia.MovimientoInventario.IdBodega);
                Custodia.MovimientoInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Ingreso;
                Custodia.IdCustodia = AdicionarMovimientoInventario(Custodia.MovimientoInventario);
                PURepositorio.Instancia.AdicionarCustodia(Custodia);
                return true;
            }
            else
                return false;
        }


        #endregion Casa Matriz


        #region Salida Custodia

        /// <summary>
        /// ingresar a custodia la guia
        /// </summary>                
        public List<string> SalidaCustodia(PUCustodia Custodia)
        {
            List<string> ListaRespuesta = new List<string>();
            PUMovimientoInventario UltimoMovimiento = PURepositorio.Instancia.ConsultaUltimoMovimientoGuia(Custodia.MovimientoInventario.NumeroGuia);
            PUEnumTipoMovimientoInventario tipoMovimientoActual = UltimoMovimiento.TipoMovimiento;

            if (PUEnumTipoMovimientoInventario.Ingreso == tipoMovimientoActual)
            {
                List<string> Adjuntos = Custodia.AdjuntosPrecargue;
                string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderServerDigita");
                string fechaActual = DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day;
                rutaImagenes = "C:/Controller/evidencia/" + fechaActual;
                if (!Directory.Exists(rutaImagenes))
                {
                    Directory.CreateDirectory(rutaImagenes);
                }
                rutaImagenes = rutaImagenes + "/" + Custodia.MovimientoInventario.NumeroGuia;
                if (!Directory.Exists(rutaImagenes))
                {
                    Directory.CreateDirectory(rutaImagenes);
                }


                ADGuia Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(Custodia.MovimientoInventario.NumeroGuia);
                //ingresar a bodega        
                Custodia.MovimientoInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
                Custodia.MovimientoInventario.NumeroGuia = Guia.NumeroGuia;
                Custodia.MovimientoInventario.FechaGrabacion = DateTime.Now;
                Custodia.MovimientoInventario.FechaEstimadaIngreso = DateTime.Now;
                Custodia.MovimientoInventario.CreadoPor = ControllerContext.Current.Usuario;
                Custodia.IdCustodia = fachadaCentroServicios.AdicionarMovimientoInventario(Custodia.MovimientoInventario);
                //PURepositorio.Instancia.AdicionarCustodia(Custodia);




                //cambio estado entregado por reclame en oficina

                ADTrazaGuia estadoGuia;
                ADEstadoGuiaMotivoDC estadoMotivoGuia;
                Custodia.MovimientoInventario.Bodega = PURepositorio.Instancia.ObtenerCentroServicio(Custodia.MovimientoInventario.Bodega.IdCentroServicio);

                estadoGuia = new ADTrazaGuia
                {
                    Ciudad = Custodia.MovimientoInventario.Bodega.CiudadUbicacion.Nombre,
                    IdCiudad = Custodia.MovimientoInventario.Bodega.CiudadUbicacion.IdLocalidad,
                    IdAdmision = Guia.IdAdmision,
                    IdEstadoGuia = (short)ADEnumEstadoGuia.Custodia,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DisposicionFinal,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = Custodia.MovimientoInventario.NumeroGuia,
                    Observaciones = string.Empty,
                    FechaGrabacion = DateTime.Now
                };
                estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);

                ADMotivoGuiaDC motivo = new ADMotivoGuiaDC();
                motivo.IdMotivoGuia = Convert.ToInt16(Custodia.AccionSalida);
                estadoMotivoGuia = new ADEstadoGuiaMotivoDC()
                {
                    IdTrazaGuia = estadoGuia.IdTrazaGuia,
                    Motivo = motivo,
                    Observaciones = string.Empty,
                    FechaMotivo = DateTime.Now
                };
                EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);


                string path;
                int NumeroAdjunto = 1;
                PUAdjuntoMovimientoInventario AdjuntoCus;
                foreach (var Data in Adjuntos)
                {
                    byte[] img = Convert.FromBase64String(Data);
                    var imagen = Encoding.UTF8.GetString(img);
                    path = rutaImagenes + "/" + Custodia.MovimientoInventario.NumeroGuia + "_Evidencia" + NumeroAdjunto + ".png";
                    File.WriteAllBytes(path, img);

                    AdjuntoCus = new PUAdjuntoMovimientoInventario();
                    AdjuntoCus.MovimientoInventario = new PUMovimientoInventario() { IdMovimientoInventario = Custodia.IdCustodia };
                    AdjuntoCus.RutaAdjunto = path;
                    AdjuntoCus.FormatoAdjunto = "png";
                    NumeroAdjunto++;
                    fachadaCentroServicios.AdicionarAdjuntoMovimientoInventario(AdjuntoCus);
                }
                ListaRespuesta.Add("1");
                ListaRespuesta.Add("Se ingreso la guía a custodia");
            }
            else
            {
                ListaRespuesta.Add("0");
                ListaRespuesta.Add("La guía no se encuentra asignada a custodia");
            }
            return ListaRespuesta;
        }

        #endregion Casa Matriz

        public List<PUCustodia> ObtenerGuiasCustodia(int idTipoMovimiento, Int16 idEstadoGuia, long? numeroGuia, bool muestraReportemuestraTodosreporte)
        {
            if (idTipoMovimiento == 1 && idEstadoGuia == 22)
            {
                return PURepositorio.Instancia.ObtenerGuiasCustodiaParaIngreso(idTipoMovimiento, idEstadoGuia, muestraReportemuestraTodosreporte);
            }
            else
                return PURepositorio.Instancia.ObtenerGuiasCustodia(idTipoMovimiento, idEstadoGuia, numeroGuia, muestraReportemuestraTodosreporte);
        }
        #endregion

        #region AdjuntosMovimientoInventario

        public void AdicionarAdjuntoMovimientoInventario(PUAdjuntoMovimientoInventario AdjuntoMovimientoInventario)
        {
            PURepositorio.Instancia.AdicionarAdjuntoMovimientoInventario(AdjuntoMovimientoInventario);
        }

        #endregion

        #region Bodegas


        public long AdicionarMovimientoInventario(PUMovimientoInventario movimientoInventario)
        {
            return PURepositorio.Instancia.AdicionarMovimientoInventario(movimientoInventario);
        }

        /// <summary>
        /// <Proceso para obtener el centro de confirmaciones y devoluciones de una localidad>
        /// </summary>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroConfirmacionesDevoluciones(PALocalidadDC localidad)
        {
            return PURepositorio.Instancia.ObtenerCentroConfirmacionesDevoluciones(localidad);
        }


        /// <summary>
        /// <Proceso para obtenerla bodega de custodia
        /// </summary>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerBodegaCustodia()
        {
            return PURepositorio.Instancia.ObtenerBodegaCustodia();
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
        //    PURepositorio.Instancia.AdicionarMovimientoInventario_CAC(IdCentroServicio, IdTipoMovimiento, NumeroGuia, FechaGrabacion, CreadoPor);
        //}

        #endregion

        #region ConsultaUltimoMovimientoGuia

        public PUMovimientoInventario ConsultaUltimoMovimientoGuia(long NumeroGuia)
        {
            return PURepositorio.Instancia.ConsultaUltimoMovimientoGuia(NumeroGuia);
        }

        #endregion


        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerInformacionGeneralCentrosServicio()
        {
            return PURepositorio.Instancia.ObtenerInformacionGeneralCentrosServicio();
        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerPosicionesCanalesVenta(DateTime fechaInicial, DateTime fechaFinal, string idMensajero, string idCentroServicio, int idEstado)
        {
            return PURepositorio.Instancia.ObtenerPosicionesCanalesVenta(fechaInicial, fechaFinal, idMensajero, idCentroServicio, idEstado);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerServiciosCentroServicio(long idCentroServicio)
        {
            return PURepositorio.Instancia.ObtenerServiciosCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerCentrosServicioPorServicio(int idServicio)
        {
            return PURepositorio.Instancia.ObtenerCentrosServicioPorServicio(idServicio);
        }

        /// <summary>
        /// Obtiene los centros de servicio a los cuales tiene acceso el usuario
        /// </summary>
        /// <param name="identificacionUsuario"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerLocacionesAutorizadas(string usuario)
        {
            return PURepositorio.Instancia.ObtenerLocacionesAutorizadas(usuario);
        }
    }
}