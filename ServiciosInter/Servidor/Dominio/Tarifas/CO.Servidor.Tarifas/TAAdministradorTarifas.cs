using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Adicionales;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos;
using CO.Servidor.Tarifas.Comun;
using CO.Servidor.Tarifas.Datos;
using CO.Servidor.Tarifas.ListaPrecios;
using CO.Servidor.Tarifas.Servicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Novasoft;

namespace CO.Servidor.Tarifas
{
    /// <summary>
    /// Clase para exponer la lógica de administración de tarifas
    /// </summary>
    public class TAAdministradorTarifas : ControllerBase
    {
        #region Campos

        private static readonly TAAdministradorTarifas instancia = (TAAdministradorTarifas)FabricaInterceptores.GetProxy(new TAAdministradorTarifas(), COConstantesModulos.TARIFAS);

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna una instancia del administrador de tarifas
        /// </summary>
        public static TAAdministradorTarifas Instancia
        {
            get { return TAAdministradorTarifas.instancia; }
        }

        #endregion Propiedades

        #region Comunes Tarifas

        /// <summary>
        /// Obtiene los tipos de envío que están en la base de datos
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Tipos de envío</returns>
        public IEnumerable<TATipoEnvio> ObtenerTiposEnvio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerTiposEnvio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Retorna los tipos de envíos con los servicios asociados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TATipoEnvio> ObtenerTiposDeEnvio()
        {
            return TARepositorio.Instancia.ObtenerTiposDeEnvio();
        }

        /// <summary>
        /// Retorna una lista con los tipos de envio
        /// </summary>
        /// <returns></returns>
        public List<TATipoEnvio> ObtenerTipoEnvios()
        {
            return TARepositorio.Instancia.ObtenerTipoEnvios();
        }

        /// <summary>
        /// Adicionar, editar o eliminar un tipo de envío
        /// </summary>
        /// <param name="tipoEnvio">Objeto con la información del tipo de envío</param>
        public void ActualizarTipoEnvio(TATipoEnvio tipoEnvio)
        {
            if (tipoEnvio.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                TARepositorio.Instancia.AdicionarTiposEnvio(tipoEnvio);
            }
            else if (tipoEnvio.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                TARepositorio.Instancia.EditarTiposEnvio(tipoEnvio);
            }
            else if (tipoEnvio.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                TARepositorio.Instancia.EliminarTiposEnvio(tipoEnvio);
            }
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
        public IEnumerable<TAMonedaDC> ObtenerMoneda(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerMoneda(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, edita o elimina moneda
        /// </summary>
        /// <param name="moneda">Objeto moneda</param>
        public void ActualizarMoneda(TAMonedaDC moneda)
        {
            if (moneda.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                TARepositorio.Instancia.AdicionarMoneda(moneda);
            }
            else if (moneda.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                TARepositorio.Instancia.EditarMoneda(moneda);
            }
            else if (moneda.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                TARepositorio.Instancia.EliminarMoneda(moneda);
            }
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
        public IEnumerable<TATipoEmpaque> ObtenerTiposEmpaque(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerTiposEmpaque(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, edita o elimina tipos de empaque
        /// </summary>
        /// <param name="tipoEmpaque"></param>
        public void ActualizarTiposEmpaque(TATipoEmpaque tipoEmpaque)
        {
            if (tipoEmpaque.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                TARepositorio.Instancia.AdicionarTiposEmpaque(tipoEmpaque);
            }
            else if (tipoEmpaque.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                TARepositorio.Instancia.EditarTiposEmpaque(tipoEmpaque);
            }
            else if (tipoEmpaque.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                TARepositorio.Instancia.EliminarTiposEmpaque(tipoEmpaque);
            }
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
        public IEnumerable<TATipoTramite> ObtenerTiposTramite(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerTiposTramite(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, edita o elimina un tipo de trámite
        /// </summary>
        /// <param name="tipoTramite">Objeto tipo trámite</param>
        public void ActualizarTiposTramite(TATipoTramite tipoTramite)
        {
            if (tipoTramite.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                TARepositorio.Instancia.AdicionarTiposTramite(tipoTramite);
            }
            else if (tipoTramite.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                TARepositorio.Instancia.EditarTiposTramite(tipoTramite);
            }
            else if (tipoTramite.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                TARepositorio.Instancia.EliminarTiposTramite(tipoTramite);
            }
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
        public IEnumerable<TAValorAdicional> ObtenerValorAdicional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerValorAdicional(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Retorna los valores adicionales que son de tipo embalaje
        /// </summary>
        /// <returns></returns>
        public List<TAValorAdicional> ConsultarValoresAdicionalesEmbalaje()
        {
            long idListaPreciosVigente = TARepositorio.Instancia.ObtenerIdListaPrecioVigente();
            return TARepositorio.Instancia.ConsultarValoresAdicionalesEmbalaje(idListaPreciosVigente);
        }

        /// <summary>
        /// retorna los valore adicionales para embalaje para cliente credito
        /// </summary>
        /// <param name="idClienteCredito"></param>
        /// <returns></returns>
        public List<TAValorAdicional> ConsultarValoresAdicionalesEmbalajeClienteCredito(int idListaPreciosClienteCredito)
        {
            //long idListaPreciosClienteCredito = Convert.ToInt64(TARepositorio.Instancia.ObtenerIdListaPrecioClienteCredito(idClienteCredito));
            return TARepositorio.Instancia.ConsultarValoresAdicionalesEmbalaje(idListaPreciosClienteCredito);
        }

        /// <summary>
        /// Adiciona, edita o elimina un valor adicional
        /// </summary>
        /// <param name="valorAdicional"></param>
        public void ActualizarValorAdicional(TAValorAdicional valorAdicional)
        {
            if (valorAdicional.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                TARepositorio.Instancia.AdicionarValorAdicional(valorAdicional);
            }
            else if (valorAdicional.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                TARepositorio.Instancia.EditarValorAdicional(valorAdicional);
            }
            else if (valorAdicional.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                TARepositorio.Instancia.EliminarValorAdicional(valorAdicional);
            }
        }

        /// <summary>
        /// Obtiene la lista de valor adicional de la DB
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAServicioDC> ObtenerServicios()
        {
            return TARepositorio.Instancia.ObtenerServicios();
        }

        /// <summary>
        /// Obtiene la lista de servicios por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Id de la unidad de negocio</param>
        /// <returns>Lista de Servicios de la unidad de negocio</returns>
        public IList<TAServicioDC> ObtenerServiciosUnidadNegocio(string IdUnidadNegocio)
        {
            return TARepositorio.Instancia.ObtenerServiciosUnidadNegocio(IdUnidadNegocio);
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
        public IEnumerable<TATipoTrayecto> ObtenerTiposTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerTiposTrayecto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, edita o elimina un tipo de trayecto
        /// </summary>
        /// <param name="tipoTrayecto">Objeto tipo de trayecto</param>
        public void ActualizarTiposTrayecto(TATipoTrayecto tipoTrayecto)
        {
            if (tipoTrayecto.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                TARepositorio.Instancia.AdicionarTiposTrayecto(tipoTrayecto);
            }
            else if (tipoTrayecto.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                TARepositorio.Instancia.EditarTiposTrayecto(tipoTrayecto);
            }
            else if (tipoTrayecto.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                TARepositorio.Instancia.EliminarTiposTrayecto(tipoTrayecto);
            }
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
        public IEnumerable<TATipoSubTrayecto> ObtenerTiposSubTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerTiposSubTrayecto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, edita o elimina un tipo de subtrayecto
        /// </summary>
        /// <param name="subTrayecto">Objeto tipo de subtrayecto</param>
        public void ActualizarTiposSubTrayecto(TATipoSubTrayecto subTrayecto)
        {
            if (subTrayecto.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                TARepositorio.Instancia.AdicionarTipoSubTrayecto(subTrayecto);
            }
            else if (subTrayecto.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                TARepositorio.Instancia.EditarTipoSubTrayecto(subTrayecto);
            }
            else if (subTrayecto.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                TARepositorio.Instancia.EliminarTiposSubTrayecto(subTrayecto);
            }
        }

        /// <summary>
        /// Obtiene formas de pago asignadas y sin asignar para un servicio
        /// </summary>
        /// <param name="IdServicio">Identificador del servicio</param>
        /// <returns>Objeto con las formas de pago asignadas y dispobibles</returns>
        public TAFormaPagoServicio ObtenerFormaPago(int IdServicio)
        {
            return TARepositorio.Instancia.ObtenerFormaPago(IdServicio);
        }

        /// <summary>
        /// Adiciona y elimina las formas de pago asignadas a un servicio
        /// </summary>
        /// <param name="formaPago">Objeto forma de pago</param>
        public void ModificarFormaPagoServicio(ObservableCollection<TAFormaPago> formaPago, int idServicio)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                formaPago.ToList().ForEach(fp =>
                {
                    if (fp.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    {
                        TARepositorio.Instancia.AdicionarFormaPagoServicio(fp, idServicio);
                    }
                    else if (fp.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    {
                        TARepositorio.Instancia.EliminaFormaPagoServicio(fp, idServicio);
                    }
                });

                transaccion.Complete();
            }
        }

        /// <summary>
        /// Obtiene los impuestos de un servicio
        /// </summary>
        /// <param name="IdServicio">Identificador servicio</param>
        /// <returns>Colección</returns>
        public TAServicioImpuestosDC ObtenerImpuestosPorServicio(int IdServicio)
        {
            return TARepositorio.Instancia.ObtenerImpuestosPorServicio(IdServicio);
        }

        /// <summary>
        /// Adiciona, edita o elimina los impuestos de un servicio
        /// </summary>
        /// <param name="impuesto"></param>
        /// <param name="idServicio"></param>
        public void ModificarImpuestosPorServicio(ObservableCollection<TAImpuestosDC> impuesto, int idServicio)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                impuesto.ToList().ForEach(fp =>
                {
                    if (fp.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    {
                        TARepositorio.Instancia.AdicionarImpuestoPorServicio(fp, idServicio);
                    }
                    else if (fp.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    {
                        TARepositorio.Instancia.EliminaImpuestoPorServicio(fp, idServicio);
                    }
                });

                transaccion.Complete();
            }
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
        public IEnumerable<TAListaPrecioDC> ObtenerListaPrecio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            if (idListaPrecio != -1)
            {
                var a = TARepositorio.Instancia.ObtenerListaPrecio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio);
                return a;
            }
            else
            {
                return TARepositorio.Instancia.ObtenerListaPrecio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
            }
        }

        public IEnumerable<TAListaPrecioDC> ObtenerListasPrecio()
        {
            return TARepositorioListaPrecios.Instancia.ObtenerListasPrecio();
        }

        /// <summary>
        /// Adiciona, edita o elimina una lista de precio
        /// </summary>
        /// <param name="listaPrecio">Objeto lista de precio</param>
        public int ActualizarListaPrecio(TAListaPrecioDC listaPrecio)
        {
            bool validaListaPrecio;
            int? idListaCreada = new int();
            DateTime fechaActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            //Comprobar si existe una tarifa plena, vigente y activa
            if (listaPrecio.TarifaPlena == true && listaPrecio.Estado == ConstantesFramework.ESTADO_ACTIVO && (DateTime.Now >= listaPrecio.Inicio && DateTime.Now <= listaPrecio.Fin))
                validaListaPrecio = TARepositorio.Instancia.ValidaListaPrecio(listaPrecio.IdListaPrecio);
            else
                validaListaPrecio = false;

            if (listaPrecio.EstadoRegistro == EnumEstadoRegistro.ADICIONADO && validaListaPrecio == false)
            {
                if (listaPrecio.Inicio >= fechaActual && listaPrecio.Fin > listaPrecio.Inicio)
                {
                    idListaCreada = TARepositorio.Instancia.AdicionarListaPrecio(listaPrecio);
                    if (!idListaCreada.HasValue)
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_ERROR_CREANDO_LISTA_PRECIOS.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_ERROR_CREANDO_LISTA_PRECIOS)));
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_FECHA_ADICIONADO_LISTA_PRECIO_NO_CUMPLE.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_FECHA_ADICIONADO_LISTA_PRECIO_NO_CUMPLE)));
            }
            else if (listaPrecio.EstadoRegistro == EnumEstadoRegistro.ADICIONADO && validaListaPrecio == true)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_EXISTE_TARIFA_PLENA_VIGENTE_ACTIVA.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_EXISTE_TARIFA_PLENA_VIGENTE_ACTIVA)));
            }

            if (listaPrecio.EstadoRegistro == EnumEstadoRegistro.MODIFICADO && validaListaPrecio == false)
            {
                if (listaPrecio.Fin > listaPrecio.Inicio)
                    TARepositorio.Instancia.EditarListaPrecio(listaPrecio);
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_FECHA_MODIFICADO_LISTA_PRECIO_NO_CUMPLE.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_FECHA_MODIFICADO_LISTA_PRECIO_NO_CUMPLE)));
            }
            else if (listaPrecio.EstadoRegistro == EnumEstadoRegistro.MODIFICADO && validaListaPrecio == true)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_EXISTE_TARIFA_PLENA_VIGENTE_ACTIVA.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_EXISTE_TARIFA_PLENA_VIGENTE_ACTIVA)));
            }

            if (listaPrecio.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                TARepositorio.Instancia.EliminarListaPrecio(listaPrecio);

            return idListaCreada.Value;
        }

        /// <summary>
        /// Obtiene los tipos de moneda de la aplicación
        /// </summary>
        /// <returns>Objeto Lista Moneda</returns>
        public IEnumerable<TAMonedaDC> ObtenerTiposMoneda()
        {
            return TARepositorio.Instancia.ObtenerTiposMoneda();
        }

        /// <summary>
        /// Obtiene una lista con los estados del usuario
        /// </summary>
        /// <returns>Objeto lista estados</returns>
        public IEnumerable<EstadoDC> ObtenerEstadoActivoInactivo()
        {
            return TARepositorio.Instancia.ObtenerEstadoActivoInactivo();
        }

        /// <summary>
        /// Adiciona, edita o elimina un servicio de una lista de precio
        /// </summary>
        /// <param name="listaPrecioServicio">Objeto lista de precio servicio</param>
        public void ActualizarListaPrecioServicio(TAListaPrecioServicio listaPrecioServicio)
        {
            if (listaPrecioServicio.PrimaSeguro < 0 || listaPrecioServicio.PrimaSeguro > 100)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_ERROR_RANGO_PRIMA_SEGURO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_ERROR_RANGO_PRIMA_SEGURO)));
            }
            else
            {
                if (listaPrecioServicio.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                {
                    ///Los servicios de las unidades de negocio de mensajeria y de carga se basan en los trayectos y subtrayectos
                    ///configurados en el servicio de mensajeria segun control de cambios
                    if (listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_HOY
                      || listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_AM
                      || listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO
                      || listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_NOTIFICACIONES
                      || listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA
                      || listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_CARGA_EXPRESS
                      || listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_TULAS
                      || listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_VALORES_CARGA
                      || listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_VALORES_MENSAJERIA
                      || listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA_CONSOLIDADA
                      || listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_VALIJAS)
                    {
                        if (TARepositorio.Instancia.ObtenerListaPrecioServicioMensajeria(listaPrecioServicio.IdListaPrecio) == null)
                        {
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_MENSAJERIA_NO_CONF_LISTA_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_MENSAJERIA_NO_CONF_LISTA_SERVICIO));
                            throw new FaultException<ControllerException>(excepcion);
                        }
                        TARepositorio.Instancia.AdicionarListaPrecioServicioBaseMensajeria(listaPrecioServicio);
                    }
                    else
                        TARepositorio.Instancia.AdicionarListaPrecioServicio(listaPrecioServicio);
                }
                else if (listaPrecioServicio.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                {
                    TARepositorio.Instancia.EditarListaPrecioServicio(listaPrecioServicio);
                }
                else if (listaPrecioServicio.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                {
                    if (listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA)
                        TARepositorio.Instancia.EliminarListaPrecioServicioMensajeria(listaPrecioServicio);
                    else
                        TARepositorio.Instancia.EliminarListaPrecioServicio(listaPrecioServicio);
                }
            }
        }

        /// <summary>
        /// Obtiene los servicios de rapicarga, Rapi Carga Terrestre y mensajeria por municipio origen y destino 
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <returns></returns>
        public List<int> ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(string municipioOrigen, string municipioDestino)
        {
            return TARepositorio.Instancia.ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(municipioOrigen, municipioDestino);
        }

        /// <summary>
        /// Retorna las unidades de negocio
        /// </summary>
        /// <returns>Objeto Unidad de negocio</returns>
        public IEnumerable<TAUnidadNegocio> ObtenerUnidadNegocio()
        {
            return TARepositorio.Instancia.ObtenerUnidadNegocio();
        }

        /// <summary>
        /// Obtener todos los impuesto configurados en el sistema
        /// </summary>
        /// <returns>Colección con todos los impuestos del sistema</returns>
        public IList<TAImpuestosDC> ObtenerImpuestos()
        {
            return TARepositorio.Instancia.ObtenerImpuestos();
        }

        /// <summary>
        /// Consultar todos los servicios y sus impuestos
        /// </summary>
        /// <returns>Colección con la información de los servicios y sus impuestos</returns>
        public TAServicioImpuestosDC ObtenerServiciosImpuestos(int idServicio)
        {
            return TARepositorio.Instancia.ObtenerServiciosImpuestos(idServicio);
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
        public IEnumerable<TAImpuestosDC> ObtenerTiposImpuesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerTiposImpuesto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, edita o elimina un tipo de impuesto
        /// </summary>
        /// <param name="tipoImpuesto">Objeto Tipo Impuesto</param>
        public void ActualizarTipoImpuesto(TAImpuestosDC tipoImpuesto)
        {
            if (tipoImpuesto.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                TARepositorio.Instancia.AdicionarTipoImpuesto(tipoImpuesto);
            }
            else if (tipoImpuesto.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                TARepositorio.Instancia.EditarTipoImpuesto(tipoImpuesto);
            }
            else if (tipoImpuesto.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                TARepositorio.Instancia.EliminarTipoImpuesto(tipoImpuesto);
            }
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
        public IEnumerable<TACuentaExternaDC> ObtenerCuentaExternaFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerCuentaExternaFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, edita o elimina una cuenta externa
        /// </summary>
        /// <param name="cuentaExterna">Objeto cuenta externa</param>
        public void ActualizaCuentaExterna(TACuentaExternaDC cuentaExterna)
        {
            if (cuentaExterna.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                TARepositorio.Instancia.AdicionarCuentaExterna(cuentaExterna);
            else if (cuentaExterna.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                TARepositorio.Instancia.EditarCuentaExterna(cuentaExterna);
            else if (cuentaExterna.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                TARepositorio.Instancia.EliminarCuentaExterna(cuentaExterna);
        }

        /// <summary>
        /// Obtien las cuentas externas
        /// </summary>
        /// <returns>Objeto Cuenta Externa</returns>
        public IEnumerable<TACuentaExternaDC> ObtenerCuentaExterna()
        {
            return TARepositorio.Instancia.ObtenerCuentaExterna();
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
        public IEnumerable<TATipoEntrega> ObtenerTipoEntregaFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerTipoEntregaFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, edita o elimina una tipo entrega
        /// </summary>
        /// <param name="tipoEntrega">Objeto tipo entrega</param>
        public void ActualizarTipoEntrega(TATipoEntrega tipoEntrega)
        {
            if (tipoEntrega.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                TARepositorio.Instancia.AdicionarTipoEntrega(tipoEntrega);
            else if (tipoEntrega.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                TARepositorio.Instancia.EditarTipoEntrega(tipoEntrega);
            else if (tipoEntrega.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                TARepositorio.Instancia.EliminarTipoEntrega(tipoEntrega);
        }

        /// <summary>
        /// Obtien los tipos de entrega
        /// </summary>
        /// <returns>Objeto Tipo entrega</returns>
        public IEnumerable<TATipoEntrega> ObtenerTipoEntrega()
        {
            return TARepositorio.Instancia.ObtenerTipoEntrega();
        }

        /// <summary>
        /// Obtiene los Operadores Postales de la Aplicación
        /// </summary>
        /// <returns>Colección con los operadores postales</returns>
        public IEnumerable<TAOperadorPostalDC> ObtenerOperadoresPostales()
        {
            return TARepositorio.Instancia.ObtenerOperadoresPostales();
        }

        /// <summary>
        /// Obtiene la zonas de la aplicación
        /// </summary>
        /// <returns>Colección con las zonas de la aplicación</returns>
        public IEnumerable<PAZonaDC> ObtenerZonas()
        {
            return TARepositorio.Instancia.ObtenerZonas();
        }

        /// <summary>
        /// Obtiene los tipos de empaque de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de empaque</returns>
        public IEnumerable<TATipoEmpaque> ObtenerTiposEmpaqueTotal()
        {
            return TARepositorio.Instancia.ObtenerTiposEmpaqueTotal();
        }

        /// <summary>
        /// Obtiene los pesos mínimo y máximo de un servicio
        /// </summary>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <returns>Objeto con los pesos</returns>
        public TAServicioPesoDC ObtenerServicioPeso(int idServicio)
        {
            return TARepositorio.Instancia.ObtenerServicioPeso(idServicio);
        }

        /// <summary>
        /// Retorna los parámetros de lista precio servicio
        /// </summary>
        /// <param name="listaPrecioServicio">Objeto listaprecioservicio</param>
        /// <returns>Retorna parámetos listaprecioservicio</returns>
        public TAListaPrecioServicioParametrosDC ObtenerParametrosListaPrecioServicio(TAListaPrecioServicioParametrosDC listaPrecioServicio)
        {
            return TARepositorio.Instancia.ObtenerParametrosListaPrecioServicio(listaPrecioServicio);
        }

        /// <summary>
        /// Obtiene los datos básicos del servicio
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns>Objeto Servicio</returns>
        public TAServicioDC ObtenerDatosServicio(int idServicio)
        {
            return TARepositorio.Instancia.ObtenerDatosServicio(idServicio);
        }

        /// <summary>
        /// Obtiene los trayectos y subtrayectos de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de trayectos y subtrayectos</returns>
        public IEnumerable<TATrayectoSubTrayectoDC> ObtenerTrayectosSubtrayectos()
        {
            return TARepositorio.Instancia.ObtenerTrayectosSubtrayectos();
        }

        /// <summary>
        /// Obtiene de precio rango
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public IEnumerable<TAPrecioRangoDC> ObtenerPrecioRango(int idServicio, int idListaPrecio)
        {
            return TARepositorio.Instancia.ObtenerPrecioRango(idServicio, idListaPrecio);
        }

        /// <summary>
        /// Obtiene de precio rango
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public IEnumerable<TAPrecioRangoDC> ObtenerPrecioRango(int idServicio)
        {
            int idListaPrecio = TARepositorio.Instancia.ObtenerIdListaPrecioVigente();

            return TARepositorio.Instancia.ObtenerPrecioRango(idServicio, idListaPrecio);
        }

        /// <summary>
        /// Obtiene de precio rango para el servicio de giros
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public TAPrecioRangoImpuestosDC ObtenerPrecioRangoImpuestosGiro()
        {
            TAPrecioRangoImpuestosDC precioRangoImpuestos = new TAPrecioRangoImpuestosDC();
            int idListaPrecio = TARepositorio.Instancia.ObtenerIdListaPrecioVigente();

            precioRangoImpuestos.LstPrecioRango = TARepositorio.Instancia.ObtenerPrecioRango(TAConstantesServicios.SERVICIO_GIRO, idListaPrecio);
            precioRangoImpuestos.Impuestos = TARepositorio.Instancia.ObtenerServiciosImpuestos(TAConstantesServicios.SERVICIO_GIRO);

            return precioRangoImpuestos;
        }

        /// <summary>
        /// Obtener los impuestos de el servicio de giros
        /// </summary>
        /// <returns></returns>
        public TAServicioImpuestosDC ObtenerImpuestosGiros()
        {
            return TARepositorio.Instancia.ObtenerServiciosImpuestos(TAConstantesServicios.SERVICIO_GIRO);
        }

        /// <summary>
        /// Obtiene el valor total,servicio,tarifas de un giro dirigido a un cliente contado a partir de un contrato
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public TAPrecioDC ObtenerValorGiroClienteContadoGiro(int idContrato, decimal valor)
        {
            TAPrecioDC precioGiro = new TAPrecioDC()
              {
                  ValorGiro = valor
              };

            var lstPrecioRango = TARepositorio.Instancia.ObtenerPrecioRangoClienteContado(idContrato, TAConstantesServicios.SERVICIO_GIRO);
            var impuestos = TARepositorio.Instancia.ObtenerServiciosImpuestos(TAConstantesServicios.SERVICIO_GIRO);

            TAPrecioRangoDC precioRango = lstPrecioRango.FirstOrDefault(p =>
             p.PrecioInicial <= precioGiro.ValorGiro &&
             p.PrecioFinal >= precioGiro.ValorGiro);

            if (precioRango != null)
            {
                precioGiro.ValorServicio = precioRango.Valor + ((precioRango.Porcentaje / 100) * valor);
                precioGiro.TarifaFijaPorte = precioRango.Valor;
                precioGiro.TarifaPorcPorte = precioRango.Porcentaje;
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_VALOR_COBRAR_FUERA_RANGOS.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_VALOR_COBRAR_FUERA_RANGOS));
                throw new FaultException<ControllerException>(excepcion);
            }

            if (impuestos != null)
            {
                impuestos.ImpuestosAsignados.ToList().ForEach(
                  imp => { imp.ValorImpuestoAplicado = precioGiro.ValorServicio * (imp.ValorImpuesto / 100); });

                precioGiro.InfoImpuestos = impuestos.ImpuestosAsignados.ToList();
            }

            // Suma los impuestos asociados al giro
            if (precioGiro.InfoImpuestos != null)
            {
                precioGiro.ValorImpuestos = precioGiro.InfoImpuestos.ToList().Sum(pr => pr.ValorImpuestoAplicado);
            }

            // Suma los servicios aicionales al giro
            if (precioGiro.ServiciosSolicitados != null)
            {
                precioGiro.ValorAdicionales = precioGiro.ServiciosSolicitados.Sum(ss => ss.PrecioValorAdicional);
            }

            precioGiro.ValorTotal = precioGiro.ValorGiro + precioGiro.ValorServicio + precioGiro.ValorAdicionales + precioGiro.ValorImpuestos;

            return precioGiro;
        }

        /// <summary>
        ///  Obtiene el valor a cobrar a un servicio segun su  valor total.
        /// </summary>
        public TAPrecioDC CalcularValorServicoAPartirValorTotal(int idContrato, decimal valorTotal)
        {
            TAPrecioDC precioGiro = new TAPrecioDC()
            {
                ValorTotal = valorTotal
            };

            var lstPrecioRango = TARepositorio.Instancia.ObtenerPrecioRangoClienteContado(idContrato, TAConstantesServicios.SERVICIO_GIRO);
            var impuestos = TARepositorio.Instancia.ObtenerServiciosImpuestos(TAConstantesServicios.SERVICIO_GIRO);

            decimal valorAdicional = 0;
            decimal valorCalcular = 0;

            // Suma los servicios aicionales al giro
            if (precioGiro.ServiciosSolicitados != null)
                valorAdicional += precioGiro.ServiciosSolicitados.ToList().Sum(ss => ss.PrecioValorAdicional);

            valorCalcular = precioGiro.ValorTotal - valorAdicional;

            TAPrecioRangoDC precioRango = lstPrecioRango.FirstOrDefault(p =>
            p.PrecioInicial <= valorCalcular - ObtenerValorServicio(valorCalcular, p.Valor, p.Porcentaje, impuestos) &&
            p.PrecioFinal >= valorCalcular - ObtenerValorServicio(valorCalcular, p.Valor, p.Porcentaje, impuestos));

            if (precioRango != null)
            {
                precioGiro.TarifaFijaPorte = precioRango.Valor;
                precioGiro.TarifaPorcPorte = precioRango.Porcentaje;

                precioGiro.ValorServicio = precioRango.Valor + (valorCalcular - (valorCalcular * 100) / (100 + precioRango.Porcentaje));
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_VALOR_COBRAR_FUERA_RANGOS.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_VALOR_COBRAR_FUERA_RANGOS));
                throw new FaultException<ControllerException>(excepcion);
            }

            if (impuestos != null)
            {
                impuestos.ImpuestosAsignados.ToList().ForEach(
                  imp => { imp.ValorImpuestoAplicado = precioGiro.ValorServicio * (imp.ValorImpuesto / 100); });

                precioGiro.InfoImpuestos = impuestos.ImpuestosAsignados.ToList();
            }

            // Suma los impuestos asociados al giro
            if (precioGiro.InfoImpuestos != null)
            {
                precioGiro.ValorImpuestos = precioGiro.InfoImpuestos.ToList().Sum(pr => pr.ValorImpuestoAplicado);
            }

            precioGiro.ValorGiro = valorCalcular - precioGiro.ValorServicio - precioGiro.ValorImpuestos;

            return precioGiro;
        }

        /// <summary>
        /// Realiza el calculo para obtener el valor del giro
        /// </summary>
        /// <returns></returns>
        private decimal ObtenerValorServicio(decimal valorTotal, decimal valorFlete, decimal porcentajeFlete, TAServicioImpuestosDC impuestos)
        {
            decimal valorImpuestos = 0;
            decimal valorServicio = 0;

            if (impuestos != null)
            {
                impuestos.ImpuestosAsignados.ToList().ForEach(imp =>
                {
                    valorImpuestos += (imp.ValorImpuesto / 100 * valorFlete);
                });
            }

            valorServicio = valorFlete + ((porcentajeFlete / 100) * (valorTotal - valorFlete)) + valorImpuestos;
            return valorServicio;
        }

        /// <summary>
        /// Obtiene los tipos de trayecto de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de trayecto</returns>
        public IEnumerable<TATipoTrayecto> ObtenerTiposTrayectosGenerales()
        {
            return TARepositorio.Instancia.ObtenerTiposTrayectoGenerales();
        }

        /// <summary>
        /// Obtiene los tipos de subtrayectos de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de subtrayecto</returns>
        public IEnumerable<TATipoSubTrayecto> ObtenerTiposSubTrayectoGenerales()
        {
            return TARepositorio.Instancia.ObtenerTiposSubTrayectoGenerales();
        }

        /// <summary>
        /// metodo que obtiene las formas de pago posibles
        /// </summary>
        /// <returns>lista con las formas de pago de tipo TAFormaPago </returns>
        public IEnumerable<TAFormaPago> ObtenerFormasPago(bool aplicaFactura)
        {
            return TARepositorio.Instancia.ObtenerFormasPago(aplicaFactura);
        }

        /// <summary>
        /// Obtiene los tipos de trámite
        /// </summary>
        /// <returns>Colección</returns>
        public IEnumerable<TATipoTramite> ObtenerTiposTramitesGeneral()
        {
            return TARepositorio.Instancia.ObtenerTiposTramitesGeneral();
        }

        /// <summary>
        /// Valida si existe un trayecto para una ciudad de origen y un servicio
        /// </summary>
        public void ValidarServicioTrayectoOrigen(string idLocalidadOrigen, int idServicio)
        {
            TATrayecto.Instancia.ValidarServicioTrayectoOrigen(idLocalidadOrigen, idServicio);
        }

        /// <summary>
        /// Metodo para obtener los servicios de una lista de precios
        /// </summary>
        /// <param name="listaPrecios"></param>
        /// <returns> lista de datos tipos servicio</returns>
        public IEnumerable<TAServicioDC> ObtenerServiciosporLista(int listaPrecios)
        {
            return TARepositorio.Instancia.ObtenerServiciosporLista(listaPrecios);
        }

        /// <summary>
        /// Obtiene formas de pago asignadas a un servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public TAFormaPagoServicio ObtenerFormasPagoAsignadaAServicio(int idServicio)
        {
            return TARepositorio.Instancia.ObtenerFormasPagoAsignadaAServicio(idServicio);
        }

        /// <summary>
        /// Obtiene todas las formas de pago con los servicios que las tienen incluidas
        /// </summary>
        /// <returns></returns>
        public List<TAFormaPago> ObtenerFormasPagoConServicios()
        {
            return TARepositorio.Instancia.ObtenerFormasPagoConServicios();
        }

        /// <summary>
        /// Retorna las formas de pago que aplican para un cliente crédito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAFormaPago> ObtenerFormasPagoClienteCredito()
        {
            return TARepositorio.Instancia.ObtenerFormasPagoClienteCredito();
        }

        /// <summary>
        /// Obtiene los tipos de valor adicional
        /// </summary>
        /// <returns>Colección con los valores adicionales</returns>
        public IEnumerable<TAValorAdicionalValorDC> ObtenerTiposValorAdicionalServicio(int idServicio)
        {
            return TARepositorio.Instancia.ObtenerTiposValorAdicionalServicio(idServicio);
        }

        /// <summary>
        /// Obtiene lista de tipos de destino
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TATipoDestino> ObtenerTiposDestino()
        {
            return TARepositorio.Instancia.ObtenerTiposDestino();
        }

        /// <summary>
        /// Retorna un parámetro de configuración de tarifas
        /// </summary>
        /// <param name="nombreParametro"></param>
        /// <returns></returns>
        public string ObtenerParametro(string nombreParametro)
        {
            return TARepositorio.Instancia.ObtenerParametro(nombreParametro);
        }

        #endregion Comunes Tarifas

        #region Requisito Servicio

        /// <summary>
        /// Retorna los requisitos para un servicio solicitado
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns></returns>
        public IList<TARequisitoServicioDC> ObtenerRequisitosServicio(int idServicio)
        {
            return TARepositorio.Instancia.ObtenerRequisitosServicio(idServicio);
        }

        /// <summary>
        /// Adiciona, Edita o elimmina un requisito de servicio
        /// </summary>
        /// <param name="requisito"></param>
        public void ActualizarRequisitoServicio(TARequisitoServicioDC requisito)
        {
            if (requisito.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                TARepositorio.Instancia.AdicionarRequisitoServicio(requisito);
            }
            else if (requisito.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                TARepositorio.Instancia.EditarRequisitoServicio(requisito);
            }
            else if (requisito.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                TARepositorio.Instancia.EliminarRequisitoServicio(requisito);
            }
        }

        /// <summary>
        /// Obtiene los requisitos del servicio
        /// </summary>
        /// <returns>Lista con los requisitos de servicio </returns>
        public IList<TARequisitoServicioDC> ObtenerDatosRequisitoServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerDatosRequisitoServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        #endregion Requisito Servicio

        #region Configuración Rapi Carga

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
        public IEnumerable<TAPrecioTrayectoDC> ObtenerPrecioTrayectoSubTrayectoRango(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            return TAServicioRapiCarga.Instancia.ObtenerPrecioTrayectoSubTrayectoRango(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio);
        }

        /// <summary>
        /// Obtiene los precios de los trayectos por servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <returns></returns>
        public IEnumerable<TAPrecioTrayectoDC> ObtenerPrecioTraySubTrayectoRango(int idListaPrecio)
        {
            return TAServicioRapiCarga.Instancia.ObtenerPrecioTraySubTrayectoRango(idListaPrecio);
        }

        /// <summary>
        /// Guarda los cambios realizados en RapiCarga
        /// </summary>
        /// <param name="consolidadoCambios">Objeto Consolidado de cambios</param>
        public void GuadarTarifaRapiCarga(TATarifaRapiCargaDC tarifaRapiCarga)
        {
            TAServicioRapiCarga.Instancia.GuardarTarifa(tarifaRapiCarga);
        }

        #endregion Configuración Rapi Carga

        #region Configuración Rapi Carga Contra Pagos

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
        public IEnumerable<TAPrecioTrayectoDC> ObtenerPrecioTrayectoSubTrayectoRangoRapCarContraPago(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            return TAServicioRapiCargaContraPago.Instancia.ObtenerPrecioTrayectoSubTrayectoRangoRapCarContraPago(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio);
        }

        /// <summary>
        /// Guardar Tarifa Rapicarga Contra Pago
        /// </summary>
        public void GuadarTarifaRapiCargaContraPago(TATarifaRapiCargaContraPagoDC tarifaRapiCargaContraPago)
        {
            TAServicioRapiCargaContraPago.Instancia.GuardarTarifaRapiCargaContraPago(tarifaRapiCargaContraPago);
        }

        #endregion Configuración Rapi Carga Contra Pagos

        #region Obtener Valores Servicios

        /// <summary>
        /// Obtiene los tipos de valor y sus campos para el servicio de giros
        /// </summary>
        /// <param name="idServicio">ID del servicio</param>
        /// <returns>Lista con todos los tipos de valor  con sus campos</returns>
        public List<TAValorAdicional> ObtenerTipoValorAdicionalConCamposGIROS()
        {
            return TARepositorio.Instancia.ObtenerTipoValorAdicionalConCampos(TAConstantesServicios.SERVICIO_GIRO);
        }

        #endregion Obtener Valores Servicios

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
        public IEnumerable<TATrayectoDC> ObtenerTrayectos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TATrayecto.Instancia.ObtenerTrayectos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Guarda trayectos
        /// </summary>
        /// <param name="consolidadoTrayectos">Colección de trayectos</param>
        public void GuardarTrayectos(ObservableCollection<TATrayectoDC> consolidadoTrayectos)
        {
            TATrayecto.Instancia.GuardarTrayectos(consolidadoTrayectos);
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
        public IEnumerable<TATrayectoExcepcionDC> ObtenerExcepcionTrayectoSubTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            return TATrayecto.Instancia.ObtenerExcepcionTrayectoSubTrayecto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio);
        }

        /// <summary>
        /// Metodo para guardar los cambios de una excepción a un trayecto subtrayecto
        /// </summary>
        /// <param name="excepcion"></param>
        public void GuardarCambiosExcepcionTrayectoSubTrayecto(TATrayectoExcepcionDC excepcion)
        {
            TATrayecto.Instancia.GuardarCambiosExcepcionTrayectoSubTrayecto(excepcion);
        }

        /// <summary>
        /// Metodo para obtener los servicios por excepcion
        /// </summary>
        /// <param name="excepcion"></param>
        /// <returns></returns>
        public IEnumerable<TATrayectoExcepcionServicioDC> ObtenerServiciosPorExcepcion(TATrayectoExcepcionDC excepcion)
        {
            return TATrayecto.Instancia.ObtenerServiciosPorExcepcion(excepcion);
        }

        #endregion Trayectos

        #region Centros de Servicio

        public IEnumerable<TAServicioCentroDeCorrespondenciaDC> ObtenerCentrosDeCorrespondencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            return TAServicioCentroCorrespondencia.Instancia.ObtenerCentrosDeCorrespondencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio);
        }

        /// <summary>
        /// Guarda Tarifa
        /// </summary>
        public void GuardarTarifaCentroDeCorrespondencia(TATarifaCentroDeCorrespondenciaDC tarifaCentroCorrespondencia)
        {
            TAServicioCentroCorrespondencia.Instancia.GuardarTarifaCentroDeCorrespondencia(tarifaCentroCorrespondencia);
        }

        #endregion Centros de Servicio

        #region Servicio Giros

        /// <summary>
        /// Guarda Tarifa
        /// </summary>
        public void GuardarTarifaGiros(TATarifaGirosDC tarifaGiros)
        {
            TAServicioGiro.Instancia.GuardarTarifaGiros(tarifaGiros);
        }

        /// <summary>
        /// Obtener concepto de caja a partir del numero del servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public int ObtenerConceptoCaja(int idServicio)
        {
            return TARepositorio.Instancia.ObtenerConceptoCaja(idServicio);
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
        public IEnumerable<TAServicioTramiteDC> ObtenerTramites(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            return TAServicioTramites.Instancia.ObtenerTramites(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio);
        }

        /// <summary>
        /// Guarda Tarifa Trámites
        /// </summary>
        /// <param name="tarifaTramites">Objeto Tarifa</param>
        public void GuardarTarifaTramites(TATarifaTramitesDC tarifaTramites)
        {
            TAServicioTramites.Instancia.GuardarTarifaTramites(tarifaTramites);
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
        public IEnumerable<TAServicioRapiPromocionalDC> ObtenerRapiPromocional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            return TAServicioRapiPromocional.Instancia.ObtenerRapiPromocional(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio);
        }

        /// <summary>
        /// Guarda Tarifa rapi promicional
        /// </summary>
        /// <param name="tarifaTramites">Objeto Tarifa rapi promocional</param>
        public void GuardarTarifaRapiPromocional(TATarifaRapiPromocionalDC tarifaRapiPromocional)
        {
            TAServicioRapiPromocional.Instancia.GuardarTarifaRapiPromocional(tarifaRapiPromocional);
        }

        #endregion Servicio Rapi Promocional

        #region Servicio Mensajeria

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaMensajeria(TATarifaMensajeriaDC tarifaMensajeria)
        {
            TAServicioMensajeria.Instancia.GuardarTarifaMensajeria(tarifaMensajeria);
        }

        #endregion Servicio Mensajeria

        #region Servicio Rapi AM

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaRapiAm(TATarifaMensajeriaDC tarifaRapiAm)
        {
            TAServicioRapiAm.Instancia.GuardarTarifaRapiAm(tarifaRapiAm);
        }

        #endregion Servicio Rapi AM

        #region Servicio Rapi Hoy

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaRapiHoy(TATarifaMensajeriaDC tarifaRapiHoy)
        {
            TAServicioRapiHoy.Instancia.GuardarTarifaRapiHoy(tarifaRapiHoy);
        }

        #endregion Servicio Rapi Hoy

        #region Servicio Rapi Personalizado

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaRapiPersonalizado(TATarifaMensajeriaDC tarifaRapiPersonalizado)
        {
            TAServicioRapiPersonalizado.Instancia.GuardarTarifaRapiPersonalizado(tarifaRapiPersonalizado);
        }

        #endregion Servicio Rapi Personalizado

        #region Servicio Rapi Envíos Contrapago

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa rapi envíos contrapago</param>
        public void GuardarTarifaRapiEnviosContraPago(TATarifaRapiEnviosContraPagoDC tarifaRapiEnvio)
        {
            TAServicioRapiEnviosContraPago.Instancia.GuardarTarifaRapiEnviosContraPago(tarifaRapiEnvio);
        }

        #endregion Servicio Rapi Envíos Contrapago

        #region Servicio Notificaciones

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaNotificaciones(TATarifaNotificacionesDC tarifaNotificaciones)
        {
            TAServicioNotificaciones.Instancia.GuardarTarifaNotificaciones(tarifaNotificaciones);
        }

        #endregion Servicio Notificaciones

        #region Servicio Carga Express

        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaCargaExpress(TATarifaMensajeriaDC tarifaCargaExpress)
        {
            TAServicioCargaExpress.Instancia.GuardarTarifaCargaExpress(tarifaCargaExpress);
        }

        public void GuardarTarifaCargaAerea(TATarifaMensajeriaDC tarifaCargaExpress)
        {
          TAServicioCargaExpressAerea.Instancia.GuardarTarifaCargaExpressAerea(tarifaCargaExpress);
        }

        #endregion Servicio Carga Express
        #region Servicio Rapi Tulas
        /// <summary>
        /// Guarda tarifa
        /// </summary>
        /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
        public void GuardarTarifaRapiTulas(TATarifaMensajeriaDC tarifa)
        {
            TAServicioRapiTulas.Instancia.GuardarTarifa(tarifa);
        }

        #endregion Servicio Rapi Tulas

        public void GuardarTarifaRapiRadicado(TATarifaMensajeriaDC tarifa)
        {
            TAServicioRapiRadicado.Instancia.GuardarTarifa(tarifa);
        }

        public void GuardarTarifaRapiValoresMsj(TATarifaMensajeriaDC tarifa)
        {
            TAServicioRapiValoresMsj.Instancia.GuardarTarifa(tarifa);
        }

        public void GuardarTarifaRapiValoresCarga(TATarifaMensajeriaDC tarifa)
        {
            TAServicioRapiValoresCarga.Instancia.GuardarTarifa(tarifa);
        }

        public void GuardarTarifaRapiCargaConsolidado(TATarifaMensajeriaDC tarifa)
        {
            TAServicioRapiCargaConsolidado.Instancia.GuardarTarifa(tarifa);
        }

        public void GuardarTarifaRapiValijas(TATarifaMensajeriaDC tarifa)
        {
            TAServicioRapiValijas.Instancia.GuardarTarifa(tarifa);
        }


        #region Valor Peso Declarado

        /// <summary>
        /// Obtiene el valor peso declarado
        /// </summary>
        /// <returns>Colección con los valores peso declarados</returns>
        public IEnumerable<TAValorPesoDeclaradoDC> ObtenerValorPesoDeclarado(int idListaPrecio)
        {
            return TARepositorio.Instancia.ObtenerValorPesoDeclarado(idListaPrecio);
        }

        /// <summary>
        /// Adiciona, edita o elimina un valores peso declarado
        /// </summary>
        /// <param name="consolidadoCambios">Colección valor peso declarado</param>
        public void GuardarValorPesoDeclarado(ObservableCollection<TAValorPesoDeclaradoDC> consolidadoCambios)
        {
            consolidadoCambios.ToList().ForEach(valorPeso =>
              {
                  if (valorPeso.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                      TARepositorio.Instancia.AdicionarValorPesoDeclarado(valorPeso);
                  else if (valorPeso.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                      TARepositorio.Instancia.EditarValorPesoDeclarado(valorPeso);
                  else if (valorPeso.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                      TARepositorio.Instancia.EliminarValorPesoDeclarado(valorPeso);
              });
        }

        #endregion Valor Peso Declarado

        #region UserControl Precio Trayecto Mensajería

        /// <summary>
        /// Obtiene los datos de precio trayecto subtrayecto de mensajería
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TATrayectoMensajeriaDC> ObtenerTiposTrayectoMensajeria(int idListaPrecio, int idServicio)
        {
            if (idServicio == TAConstantesServicios.SERVICIO_MENSAJERIA)
                return TARepositorio.Instancia.ObtenerTiposTrayectoMensajeria(idListaPrecio, idServicio);
            else
                return TARepositorio.Instancia.ObtenerTiposTrayectoUnidadNegocioMensajeria(idListaPrecio, idServicio);
        }

        /// <summary>
        /// Adiciona, edita o elimina un trayeco de mensajería
        /// </summary>
        /// <param name="trayectoMensajeria">Objeto trayecto mensajería</param>
        public void GuardarTrayectoMensajeria(ObservableCollection<TATrayectoMensajeriaDC> trayectoMensajeria)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                trayectoMensajeria.ToList().ForEach(t =>
                  {
                      if (t.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                          TARepositorio.Instancia.AdicionarTrayectoMensajeria(t);
                      else if (t.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                          TARepositorio.Instancia.EditarTrayectoMensajeria(t);
                      else if (t.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                          TARepositorio.Instancia.EliminarTrayectoMensajeria(t);
                  });

                transaccion.Complete();
            }
        }

        /// <summary>
        /// Obtiene los trayectos que tienen asignados el subtrayecto kilo adicional
        /// </summary>
        /// <returns>Colección</returns>
        public IEnumerable<TATipoTrayecto> ObtenerTiposTrayectoGeneralMensajeria()
        {
            return TARepositorio.Instancia.ObtenerTiposTrayectoGeneralMensajeria();
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
            return TARepositorio.Instancia.ObtenerValoresAdicionalesServicio(idListaPrecio, idServicio);
        }

        /// <summary>
        /// Guarda el valor de un tipo adicional
        /// </summary>
        /// <param name="valorAdicional">Objeto Valor adicional</param>
        /// <param name="idListaPrecio">Identificador Lista de Precio</param>
        /// <param name="idServicio">Identificador servicio</param>
        public void GuardarTipoValorAdicionalServicio(ObservableCollection<TAValorAdicionalValorDC> valorAdicional, int idListaPrecio, int idServicio)
        {
            string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);
            int idLp = int.Parse(idListaPrecioServicio);

            valorAdicional.ToList().ForEach(precio =>
              {
                  if (precio.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                      TARepositorio.Instancia.AdicionarValorAdicionalServicio(precio, idLp);
                  else if (precio.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                      TARepositorio.Instancia.EditarValorAdicionalServicio(precio, idLp);
                  else if (precio.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                      TARepositorio.Instancia.EliminarValorAdicionalServicio(precio, idLp);
              });
        }

        #endregion UserControl Servicio Valor Adicional

        #region UserControl Excepciones por Ciudad de Origen

        /// <summary>
        /// Obtiene las excepciones por ciudad de origen
        /// </summary>
        public IEnumerable<TAPrecioExcepcionDC> ObtenerExcepcionesPorOrigen(int idServicio, int idListaPrecio)
        {
            return TARepositorio.Instancia.ObtenerExcepcionesPorOrigen(idServicio, idListaPrecio);
        }

        /// <summary>
        /// Guarda los cambios realizados de los precios excepciones
        /// </summary>
        /// <param name="preciosExcepciones">Consolidado de cambios</param>
        public void GuardarExcepcionesPorOrigen(ObservableCollection<TAPrecioExcepcionDC> consolidadoCambios)
        {
            consolidadoCambios.ToList().ForEach(precioExcepcion =>
            {
                precioExcepcion.EsDestinoTodoElPais = null;
                precioExcepcion.EsOrigenTodoElPais = null;

                if (precioExcepcion.CiudadDestino.IdLocalidad == "-1")
                {
                    precioExcepcion.CiudadDestino = precioExcepcion.CiudadOrigen;
                    precioExcepcion.EsDestinoTodoElPais = true;
                }

                if (precioExcepcion.CiudadOrigen.IdLocalidad == "-1")
                {
                    precioExcepcion.CiudadOrigen = precioExcepcion.CiudadDestino;
                    precioExcepcion.EsOrigenTodoElPais = true;
                }

                if (precioExcepcion.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    TARepositorio.Instancia.AdicionarExcepcionPorOrigen(precioExcepcion);
                else if (precioExcepcion.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                    TARepositorio.Instancia.EditarExcepcionPorOrigen(precioExcepcion);
                else if (precioExcepcion.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    TARepositorio.Instancia.EliminarExcepcionPorOrigen(precioExcepcion);
            });
        }

        #endregion UserControl Excepciones por Ciudad de Origen

        #region Precio Servicio Tipo Entrega

        /// <summary>
        /// Obtiene los precios de tipo por tipo de entrega por lista de precios
        /// </summary>
        /// <returns>lista de precios de tipo entrega por lista de precios</returns>
        public List<TAPrecioTipoEntrega> ObtenerPrecioTipoEntrega(int idServicio, int idListaPrecio)
        {
            string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);
            int idLp = int.Parse(idListaPrecioServicio);
            return TARepositorio.Instancia.ObtenerPrecioTipoEntrega(idServicio, idLp);
        }

        /// <summary>
        /// Guarda los cambios realizados de los precios por tipo de entrega
        /// </summary>
        /// <param name="preciosExcepciones">Consolidado de cambios</param>
        public void GuardarPrecioTipoEntrega(ObservableCollection<TAPrecioTipoEntrega> consolidadoCambios)
        {
            consolidadoCambios.ToList().ForEach(precioExcepcion =>
            {
                if (precioExcepcion.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    TARepositorio.Instancia.AdicionarPrecioTipoEntrega(precioExcepcion);
                else if (precioExcepcion.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                    TARepositorio.Instancia.EditarPrecioTipoEntrega(precioExcepcion);
                else if (precioExcepcion.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    TARepositorio.Instancia.EliminarTipoEntrega(precioExcepcion);
            });
        }

        #endregion Precio Servicio Tipo Entrega

        #region Consultas

        /// <summary>
        /// Obtiene los valores adicionales de un servicio
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <returns>Colección de valores adicionales</returns>
        public IEnumerable<TAValorAdicional> ObtenerValorValoresAdicionalesServicio(int idServicio)
        {
            return TARepositorio.Instancia.ObtenerValorValoresAdicionalesServicio(idServicio);
        }

        /// <summary>
        /// Obtiene los servicios parametrizados segun forma de pago y tipo cuenta NOVASOFT
        /// </summary>
        /// <returns></returns>
        public List<CAServiciosFormaPagoDC> ObtenerServicioFormaPago()
        {
            return TARepositorio.Instancia.ObtenerServiciosFormasPagoNovasoft();
        }
        /// <summary>
        /// Actualizacion(Adicion, Eliminacion, Modificacion) Servicios Formas Pago Novasoft
        /// </summary>
        /// <param name="obj"></param>
        public void ActualizacionRegistrosParametrizacionServicioFormaPagoNova(CAServiciosFormaPagoDC obj)
        {
            TARepositorio.Instancia.ActualizacionRegistrosParametrizacionServicioFormaPagoNova(obj);
        }
        /// <summary>
        /// Obtiene formas de pago segun servicios
        /// </summary>
        /// <param name="lstServicios"></param>
        /// <returns></returns>
        public TAFormaPagoServicio ObtenerListaFormasPagoPorServicio(int servicio)
        {
            return TARepositorio.Instancia.ObtenerListaFormasPagoPorServicio(servicio);
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

            return TARepositorio.Instancia.ConsultarValorComercialPeso(peso);

        }

         /// <summary>
        /// Consulta los servicios con los pesos minimos y maximos 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<TAServicioPesoDC> ConsultarServiciosPesosMinimoxMaximos()
        {
            return TARepositorio.Instancia.ConsultarServiciosPesosMinimoxMaximos();
        }


        #endregion

    }
}