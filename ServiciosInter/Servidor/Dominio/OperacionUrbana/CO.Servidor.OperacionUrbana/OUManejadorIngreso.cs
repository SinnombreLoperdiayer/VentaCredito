using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.ParametrosOperacion;
using CO.Servidor.Dominio.Comun.Rutas;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.OperacionUrbana.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using Framework.Servidor.Agenda;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;

namespace CO.Servidor.OperacionUrbana
{
    internal class OUManejadorIngreso : ControllerBase
    {
        private static readonly OUManejadorIngreso instancia = (OUManejadorIngreso)FabricaInterceptores.GetProxy(new OUManejadorIngreso(), COConstantesModulos.MODULO_OPERACION_URBANA);

        /// <summary>
        /// Retorna una instancia de OUManejadorIngreso
        /// /// </summary>
        public static OUManejadorIngreso Instancia
        {
            get { return OUManejadorIngreso.instancia; }
        }

        /// <summary>
        /// Guardar el ingreso de una guía la centro de acopio, y realiza las validaciones necesarias asi como el
        /// envio de las fallas
        /// </summary>
        /// <param name="numeroGuia"></param>
        public OUGuiaIngresadaDC GuardarIngreso(OUGuiaIngresadaDC guiaIngresada)
        {
            OUGuiaIngresadaDC guiaSistema = new OUGuiaIngresadaDC();
            ValidarGuia(guiaIngresada);

            //  if (guiaIngresada.DetalleGuia == null)
            guiaSistema = OURepositorio.Instancia.ConsultaGuia(guiaIngresada);

            if (guiaSistema.IdAdmision > 0)
            {
                guiaIngresada.Observaciones = string.Empty;

                ADTrazaGuia trazaGuia = new ADTrazaGuia
                {
                    NumeroGuia = guiaIngresada.NumeroGuia,
                    IdAdmision = guiaIngresada.IdAdmision,
                    Observaciones = guiaIngresada.Observaciones,
                    IdCiudad = guiaIngresada.IdCiudad,
                    Ciudad = guiaIngresada.Ciudad,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio,
                };

                EstadosGuia.InsertaEstadoGuia(trazaGuia);
                OURepositorio.Instancia.GuardarGuiaIngresada(guiaIngresada);
            }
            else
                OURepositorio.Instancia.GuardaGuiaNoRegistrada(guiaIngresada);

            guiaIngresada.IdAdmision = guiaSistema.IdAdmision;

            return guiaIngresada;
        }

        /// <summary>
        /// Obtiene la ruta por medio de la fachada de rutas
        /// </summary>
        /// <param name="guiaIngresada"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerRuta(OUGuiaIngresadaDC guiaIngresada)
        {
            RURutaDC ruta = COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerRuta(guiaIngresada.DetalleGuia.IdCiudadDestino, guiaIngresada.DetalleGuia.IdCiudadOrigen);
            if (ruta != null)
            {
                guiaIngresada.DetalleGuia.IdRutaDestino = ruta.IdRuta;
                guiaIngresada.DetalleGuia.RutaDestino = ruta.NombreRuta;
            }
            else
            {
                guiaIngresada.DetalleGuia.IdRutaDestino = OUConstantesOperacionUrbana.ID_RUTA_NO_ENCONTRADA;
                guiaIngresada.DetalleGuia.RutaDestino = OUConstantesOperacionUrbana.DESCRIPCION_RUTA_NO_ENCONTRADA;
            }

            return guiaIngresada;
        }

        /// <summary>
        /// Consulta la guia a partir del numero de guia ingresado
        /// </summary>
        /// <param name="guiaIngresada"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ConsultaGuia(OUGuiaIngresadaDC guiaIngresada)
        {
            return OURepositorio.Instancia.ConsultaGuia(guiaIngresada);
        }

        ///// <summary>
        ///// Realiza todas las validaciones necesarias para el ingreso de la guia
        ///// </summary>
        ///// <param name="guiaIngresada"></param>
        private void ValidarGuia(OUGuiaIngresadaDC guiaIngresada)
        {
            OUGuiaIngresadaDC guiaSistema = new OUGuiaIngresadaDC();
            //int valorMaxMensajeria;

            //if (guiaIngresada.DetalleGuia == null)
            guiaSistema = OURepositorio.Instancia.ConsultaGuia(guiaIngresada);

            short idEstadoEmpaque;
            string descEstadoEmpaque;
            bool suministroProvisionado;

            guiaIngresada.IdSucursal = 0;

            if (guiaSistema.IdAdmision > 0)
            {
                ///si la guia es un alcobro no esta pagada y es un reenvio no lo deja ingresar a centro acopio
                if (guiaSistema.CantidadReintentosEntrega > 0)
                    if (guiaSistema.EsAlCobro && !guiaSistema.EstaPagada)
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_ENVIO_ALCOBRO_NO_PAGO.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_ENVIO_ALCOBRO_NO_PAGO)));

                string valorParametro = OURepositorio.Instancia.ObtenerValorParametro(OUConstantesOperacionUrbana.PARAMETRO_PESO_MAXIMO_MENSAJERIA);

                //**********Piden que no se relice la validacion de peso bug 2617-----------------
                ///Mayor un kg. el envio se ingreso por carga
                //if (guiaIngresada.MayorUnKg)
                //{
                //  if (!string.IsNullOrEmpty(valorParametro))
                //    if (int.TryParse(valorParametro, out valorMaxMensajeria))
                //      if (guiaSistema.PesoSistema < valorMaxMensajeria)

                //       // throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_INGRESE_ENVIO_EN_SECCION_MENSAJERIA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_INGRESE_ENVIO_EN_SECCION_MENSAJERIA)));
                //}

                ////si no es mayor a 1 kg, en envio se ingreso por mensajeria
                //else
                //{
                //  if (!string.IsNullOrEmpty(valorParametro))
                //    if (int.TryParse(valorParametro, out valorMaxMensajeria))
                //      if (guiaSistema.PesoSistema > valorMaxMensajeria)
                //        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_INGRESE_ENVIO_EN_SECCION_CARGA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_INGRESE_ENVIO_EN_SECCION_CARGA)));
                //}
            }

            //si la guia esta registrada valida que no haya sido descargada
            if (guiaIngresada.GuiaRegistrada)
            {
                //OURepositorio.Instancia.ValidaGuiaRegistrada(guiaIngresada.NumeroGuia.Value);
                string valor = OURepositorio.Instancia.ObtenerParametroDesfasePeso();
                decimal valorDesfase = Convert.ToDecimal(valor);

                //valida peso y envia una tarea
                OUManejadorFallas.DespacharFallaPorDiferenciaPeso(guiaIngresada, valorDesfase, ControllerContext.Current.Usuario);
            }

            //si la guia no esta registrada valida que no haya sido descargada
            else
                OURepositorio.Instancia.ValidaGuiaNoRegistrada(guiaIngresada.NumeroGuia.Value);

            // si a guia esta registrada en el sistema consulta la sucursal del cliente
            if (guiaIngresada.GuiaRegistrada)
            {
                if (OUEnumTiposCliente.CCO.ToString().CompareTo(guiaIngresada.TipoCliente) == 0)
                {
                    guiaIngresada = OURepositorio.Instancia.ConsultaClienteConvenioConvenio(guiaIngresada);
                }
                else if (OUEnumTiposCliente.CPE.ToString().CompareTo(guiaIngresada.TipoCliente) == 0)
                {
                    guiaIngresada = OURepositorio.Instancia.ConsultaClienteConvenioPeaton(guiaIngresada);
                }
            }

            // si la guia no esta en sistema llama al metodo de clientes ObtenerSucursalPorNumeroGuia
            else
                guiaIngresada.IdSucursal = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ObtenerSucursalPorSuministro(guiaIngresada.NumeroGuia.Value, OUConstantesOperacionUrbana.SUMINISTRO_GUIA_MANUAL);

            if (guiaIngresada.MayorUnKg)
            {
                idEstadoEmpaque = guiaIngresada.EstadoEmpaqueMayorUnKG.IdEstadoEmpaque;
                descEstadoEmpaque = guiaIngresada.EstadoEmpaqueMayorUnKG.DescripcionEstado;
            }
            else
            {
                idEstadoEmpaque = guiaIngresada.EstadoEmpaqueMenorUnKG.IdEstadoEmpaque;
                descEstadoEmpaque = guiaIngresada.EstadoEmpaqueMenorUnKG.DescripcionEstado;
            }

            //valida si tiene bolsa de seguridad
            if (guiaIngresada.IdSucursal > 0)
            {
                //Valida si el cliente tiene bolsa de seguridad

                if (idEstadoEmpaque == OUConstantesOperacionUrbana.ID_ESTADO_EMPAQUE_SIN_BOLSA_SEGURIDAD)
                {
                    suministroProvisionado = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ValidarSuministroProvisionado(OUConstantesOperacionUrbana.SUMINISTRO_BOLSA_SEGURIDAD, guiaIngresada.IdSucursal);

                    // si el cliente tiene suministro de bolsa de seguridad envia una falla
                    if (suministroProvisionado)
                        OUManejadorFallas.DespacharFallaPorBolsaDeSeguridad(guiaIngresada, ControllerContext.Current.Usuario, descEstadoEmpaque);

                    //Si el cliente no tiene suministro de bolsa de seguridad asigana el estado del empaque sin suministro
                    else
                    {
                        guiaIngresada.EstadoEmpaqueMenorUnKG.IdEstadoEmpaque = OUConstantesOperacionUrbana.ID_ESTADO_EMPAQUE_SIN_SUMINISTRO;
                        guiaIngresada.EstadoEmpaqueMenorUnKG.DescripcionEstado = OUConstantesOperacionUrbana.ESTADO_EMPAQUE_SIN_SUMINISTRO;
                    }
                }
            }
            else if (idEstadoEmpaque == OUConstantesOperacionUrbana.ID_ESTADO_EMPAQUE_SIN_BOLSA_SEGURIDAD)
            {
                OUManejadorFallas.DespacharFallaPorBolsaDeSeguridad(guiaIngresada, ControllerContext.Current.Usuario, descEstadoEmpaque);
            }

            // si el estado del empaque es mal embalado envia una falla
            if (idEstadoEmpaque == OUConstantesOperacionUrbana.ID_ESTADO_EMPAQUE_MAL_EMBALADO)
                OUManejadorFallas.DespacharFallaPorBolsaDeSeguridad(guiaIngresada, ControllerContext.Current.Usuario, descEstadoEmpaque);
        }
        /// <summary>
        /// obtiene id mensajero por identificación
        /// </summary>
        /// <param name="identificacionMensajero"></param>
        /// <returns></returns>
        public int ObtenerIdMensajeroPorIdentificacion(string identificacionMensajero)
        {
            return OURepositorio.Instancia.ObtenerIdMensajeroPorIdentificacion(identificacionMensajero);
        }

        /// <summary>
        /// Obtener la informacion de un mensajero por medio de su identificador
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Datos mensajero</returns>
        public OUDatosMensajeroDC ObtenerDatosMensajero(long idMensajero)
        {
            return OURepositorio.Instancia.ObtenerDatosMensajero(idMensajero);
        }

        /// <summary>
        /// obtiene los estados de los empaques para mensajeria y carga
        /// </summary>
        /// <returns></returns>
        public List<OUEstadosEmpaqueDC> ObtenerEstadosEmpaque()
        {
            return OURepositorio.Instancia.ObtenerEstadosEmpaque();
        }

        /// <summary>
        /// Obtiene los mensajeros del centro logisitico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long centroLogistico)
        {
            return OURepositorio.Instancia.ObtenerMensajeroCentroLogistico(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, centroLogistico);
        }

        /// <summary>
        /// Obtiene todos los mensajeros de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensajerosCol(long idCol)
        {
            return OURepositorio.Instancia.ObtenerMensajerosCol(idCol);
        }

        /// <summary>
        /// Obtiene el total de los envios planillados para el mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int TotalEnviosPlanillados(long idMensajero)
        {
            return OURepositorio.Instancia.TotalEnviosPlanillados(idMensajero);
        }

        /// <summary>
        /// Verifica el soat y la revision tecnomecanica del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>
        /// true = si el soat y la tecnomecanica estan vigentes
        /// false = si el soat y la tecnomecanica estan vencidos
        /// </returns>
        public bool VerificaMensajeroSoatTecnoMecanica(long idMensajero)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IPOFachadaParametrosOperacion>().VerificaMensajeroSoatTecnoMecanica(idMensajero);
        }

        /// <summary>
        /// retorna el valor del parametro
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        public string ObtenerValorParametro(string idParametro)
        {
            return OURepositorio.Instancia.ObtenerValorParametro(idParametro);
        }

        /// <summary>
        /// Obtiene el total de los envios pendientes asignados por planilla de venta al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosPendientes(long idMensajero)
        {
            return OURepositorio.Instancia.ObtenerTotalEnviosPendientes(idMensajero);
        }

        /// <summary>
        /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
        /// </summary>
        /// <param name="numeroGuia"></param>.
        /// <param name="idAgencia"></param>
        /// <returns>True si la guía ya fué ingresada</returns>
        public bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia)
        {
            return OURepositorio.Instancia.ValidarGuiaCentroAcopio(numeroGuia, idAgencia);
        }

        /// <summary>
        /// Indica si una guía, dado su número ya ha sido ingresada a centro de acopio pero no habiá sido creada en el sistema
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <returns>Retorna el número de la agencia uqe hizo el ingreso</returns>
        public long GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(long numeroGuia)
        {
            return OURepositorio.Instancia.GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(numeroGuia);
        }

        public OUDatosMensajeroDC ObtenerDatosMensajeroPorNumeroDeCedula(string identificacionMensajero)
        {
            return OURepositorio.Instancia.ObtenerDatosMensajeroPorNumeroDeCedula(identificacionMensajero);
        }
    }
}