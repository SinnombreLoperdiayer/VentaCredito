using CO.Servidor.Adminisiones.Mensajeria.Datos;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CO.Servidor.Adminisiones.Mensajeria
{
    public class ADConsultas : ControllerBase
    {
        private static readonly ADConsultas instancia = (ADConsultas)FabricaInterceptores.GetProxy(new ADConsultas(), COConstantesModulos.MENSAJERIA);

        /// <summary>
        /// Retorna una instancia de Consultas de admisiones de mensajeria
        /// /// </summary>
        public static ADConsultas Instancia
        {
            get { return ADConsultas.instancia; }
        }

        /// <summary>
        /// Retona un bool que indica si se debe hacer integración con el sistema "Mensajero"
        /// </summary>
        /// <returns></returns>
        public bool ObtenerParametroIntegraConMensajero()
        {
            return ADRepositorio.Instancia.ObtenerParametroIntegraConMensajero();
        }

        /// <summary>
        /// Obtiene la lista de motivos por los cuales no se hizo uso de la bolsa de seguridad
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADMotivoNoUsoBolsaSeguridad> ObtenerMotivosNoUsoBolsaSeguridad()
        {
            return ADRepositorio.Instancia.ObtenerMotivosNoUsoBolsaSeguridad();
        }

        /// <summary>
        /// Retorna lista de objetos de prohibida circulación
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADObjetoProhibidaCirculacion> ObtenerObjetosProhibidaCirculacion()
        {
            return ADRepositorio.Instancia.ObtenerObjetosProhibidaCirculacion();
        }

        /// <summary>
        /// Retorna la lista de tipos de entrega que pueden aplicar sobre el envío
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADTipoEntrega> ObtenerTiposEntrega()
        {
            return ADRepositorio.Instancia.ObtenerTiposEntrega();
        }

        /// <summary>
        /// Consulta los parámetros de configuración de admisiones
        /// </summary>
        /// <returns></returns>
        public ADParametrosAdmisiones ObtenerParametrosAdmisiones()
        {
            return ADRepositorio.Instancia.ObtenerParametrosAdmisiones();
        }

        /// <summary>
        /// Consulta una guía por guid
        /// </summary>
        /// <param name="guid">Valor que identifica la transacción</param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision ObtenerGuiaPorGuid(string guid)
        {
            return ADRepositorio.Instancia.ObtenerGuiaPorGuid(guid);
        }

        /// <summary>
        /// Obtiene el ultimo estado y ubicacin de la admision mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public ADGuiaUltEstadoDC ObtenerMensajeriaUltimoEstado(long idNumeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerMensajeriaUltimoEstado(idNumeroGuia);
        }

        /// <summary>
        /// Obtiene las formas de pago de una guia
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public List<ADGuiaFormaPago> ObtenerFormasPagoGuia(long idGuia)
        {
            return ADRepositorio.Instancia.ObtenerFormasPagoGuia(idGuia);
        }

        /// <summary>
        /// Obtiene la afectación a caja de una guia
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public DatosAfectacionCaja ObtenerAfectacionCaja(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerAfectacionCaja(numeroGuia);
        }

        /// <summary>
        /// Obtener informacion de la guia de mensajeria y las formas de pago
        /// </summary>
        /// <returns></returns>
        public ADGuiaUltEstadoDC ObtenerMensajeriaFormaPago(long idAdmision)
        {
            return ADRepositorio.Instancia.ObtenerMensajeriaFormaPago(idAdmision);
        }

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioGuia(long numeroGuia, long idSucursalCentroServicio)
        {
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            //SUPropietarioGuia propietario = null;
            //try
            //{
            //    propietario = fachadaSuministros.ObtenerPropietarioSuministro(numeroGuia, SUEnumSuministro.FACTURA_VENTA_MENSAJERIA_MANUAL, idSucursalCentroServicio);
            //}
            //catch (FaultException<ControllerException> ex)
            //{
            //  if (ex.Detail.TipoError == "4") // Guía sin asignar
            //      propietario = fachadaSuministros.ObtenerPropietarioSuministro(numeroGuia, SUEnumSuministro.GUIA_TRANSPORTE_MANUAL, idSucursalCentroServicio);
            //  else
            //    throw ex;
            //}
            //return propietario;
            return fachadaSuministros.ObtenerPropietarioGuia(numeroGuia);
        }

        /// <summary>
        /// Consultar el contrato de un cliente Convenio
        /// </summary>
        /// <param name="TipoCliente"></param>
        /// <param name="idAdmisionMensajeria"></param>
        /// <returns></returns>
        public int ObtenerContratoClienteConvenio(ADEnumTipoCliente tipoCliente, long idAdmisionMensajeria)
        {
            return ADRepositorio.Instancia.ObtenerContratoClienteConvenio(tipoCliente, idAdmisionMensajeria);
        }

        /// <summary>
        /// Metodo que obtiene la información de una admisión de mensajeria a partir del numero de la misma
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuia(long idAdmision)
        {
            return ADRepositorio.Instancia.ObtenerGuia(idAdmision);
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaXNumeroGuia(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerGuiaXNumeroGuia(numeroGuia);
        }

        public ADGuia ObtenerGuiaSispostalXNumeroGuia(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerGuiaSispostalXNumeroGuia(numeroGuia);
        }



        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaXNumeroGuiaCredito(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerGuiaXNumeroGuiaCredito(numeroGuia);
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerInfoGuiaXNumeroGuia(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerInfoGuiaXNumeroGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene el identificador de la admisión
        /// </summary>
        /// <param name="numeroGuia">Número de guía</param>
        /// <returns>Identificador admisión</returns>
        public long ObtenerIdentificadorAdmision(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerIdentificadorAdmision(numeroGuia);
        }

        /// <summary>
        /// Retorna la información de una guía completa incluyendo la forma como se pagó, se construyó para generar impresión
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaPorNumeroDeGuiaCompleta(long numeroGuia, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            if (idCentroServicios.HasValue)
            {
                ADGuia guia = ADRepositorio.Instancia.ObtenerGuiaPorNumeroDeGuiaCompleta(numeroGuia, idCentroServicios.Value);
                if (guia != null)
                {
                    // Obtener información agencia de ciudad de origen y ciudad de destino, esto es informativo
                    var agenciaOrigen = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadOrigen);
                    if (agenciaOrigen != null && agenciaOrigen.IdMunicipio == guia.IdCiudadOrigen)
                    {
                        guia.DireccionAgenciaCiudadOrigen = "Oficina " + guia.NombreCiudadOrigen.Split('\\')[0] + ": " + agenciaOrigen.Direccion;
                    }
                    if (guia.IdServicio != CO.Servidor.Dominio.Comun.Tarifas.TAConstantesServicios.SERVICIO_INTERNACIONAL)
                    {
                        var agenciaDestino = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadDestino);
                        if (agenciaDestino != null && agenciaDestino.IdMunicipio == guia.IdCiudadDestino)
                        {
                            guia.DireccionAgenciaCiudadDestino = "Oficina " + guia.NombreCiudadDestino.Split('\\')[0] + ": " + agenciaDestino.Direccion;
                        }
                    }

                    if (guia.NumeroGuiaDHL > 0)
                    {
                        guia.DatosGuiaInternacional = ADRepositorio.Instancia.ObtenerDatosGuiaInternacionalDHL(guia.NumeroGuiaDHL);
                    }
                }
                return guia;
            }
            else if (idSucursal.HasValue && idCliente.HasValue)
            {
                ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(idSucursal.Value, new CLClientesDC() { IdCliente = idCliente.Value });
                ADGuia guia = ADRepositorio.Instancia.ObtenerGuiaPorNumeroDeGuiaCompleta(numeroGuia, sucursal.Agencia);
                // Obtener información agencia de ciudad de origen y ciudad de destino, esto es informativo
                var agenciaOrigen = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadOrigen);
                if (agenciaOrigen != null && agenciaOrigen.IdMunicipio == guia.IdCiudadOrigen)
                {
                    guia.DireccionAgenciaCiudadOrigen = "Oficina " + guia.NombreCiudadOrigen + ": " + agenciaOrigen.Direccion;
                }
                var agenciaDestino = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadDestino);
                if (agenciaDestino != null && agenciaDestino.IdMunicipio == guia.IdCiudadDestino)
                {
                    guia.DireccionAgenciaCiudadDestino = "Oficina " + guia.NombreCiudadDestino + ": " + agenciaDestino.Direccion;
                }
                return guia;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idRemitente">Número de identificación del cliente remitente</param>
        /// <param name="tipoIdRemitente">Tipo de identificación del cliente remitente</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorRemitenteParaHoy(string idRemitente, string tipoIdRemitente, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            if (idCentroServicios.HasValue)
            {
                return ADRepositorio.Instancia.ObtenerGuiasPorRemitenteParaHoy(idRemitente, tipoIdRemitente, idCentroServicios.Value);
            }
            else if (idSucursal.HasValue && idCliente.HasValue)
            {
                ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(idSucursal.Value, new CLClientesDC() { IdCliente = idCliente.Value });
                return ADRepositorio.Instancia.ObtenerGuiasPorRemitenteParaHoy(idRemitente, tipoIdRemitente, sucursal.Agencia);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idDestinatario">Número de identificación del cliente destinatario</param>
        /// <param name="tipoIdDestinatario">Tipo de identificación del cliente destinatario</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorDestinatarioParaHoy(string idDestinatario, string tipoIdDestinatario, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            if (idCentroServicios.HasValue)
            {
                return ADRepositorio.Instancia.ObtenerGuiasPorDestinatarioParaHoy(idDestinatario, tipoIdDestinatario, idCentroServicios.Value);
            }
            else if (idSucursal.HasValue && idCliente.HasValue)
            {
                ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(idSucursal.Value, new CLClientesDC() { IdCliente = idCliente.Value });
                return ADRepositorio.Instancia.ObtenerGuiasPorDestinatarioParaHoy(idDestinatario, tipoIdDestinatario, sucursal.Agencia);
            }
            else return null;
        }

        /// <summary>
        /// Obtiene las guias al cobro no pagas.
        /// </summary>
        /// <param name="numeroGuia">The numero guia.</param>
        /// <param name="fechaInicial">The fecha inicial.</param>
        /// <returns>Lista de Guias al Cobro sin pagar</returns>
        public List<ADGuiaAlCobro> ObtenerGuiasAlCobroNoPagas(int indicePagina, int registrosPorPagina, long numeroGuia, DateTime fechaInicial, DateTime fechaFinal, long idCentroServicio)
        {
            if (fechaInicial == DateTime.MinValue)
                fechaInicial = DateTime.Now;
            if (fechaFinal == DateTime.MinValue)
                fechaFinal = DateTime.Now;

            return ADRepositorio.Instancia.ObtenerGuiasAlCobroNoPagas(indicePagina, registrosPorPagina, numeroGuia, fechaInicial, fechaFinal, idCentroServicio);
        }

        /// <summary>
        /// Método para obtener una guía de temelercado con sus respectivos valores adicionales
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaTelemercadeo(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerGuiaTelemercadeo(numeroGuia);
        }

        /// <summary>
        /// Método para obtener un rango de guías internas
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public List<ADGuiaInternaDC> ObtenerGuiasInternas(long numeroInicial, long numeroFinal, List<long> listaNumeroGuias)
        {
            int maximoRegistros;

            if (Cache.Instancia.ContainsKey(ConstantesFramework.CACHE_MAXIMO_REGISTROS))
            {
                int.TryParse(Cache.Instancia[ConstantesFramework.CACHE_MAXIMO_REGISTROS].ToString(), out maximoRegistros);
            }
            else
            {
                // Si no existe en caché, consultarlo en la base de datos y agregarlo al caché
                maximoRegistros = Convert.ToInt32(Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.CACHE_MAXIMO_REGISTROS));
                Cache.Instancia.Add(ConstantesFramework.CACHE_MAXIMO_REGISTROS, maximoRegistros);
            }

            if (((numeroFinal - numeroInicial) > maximoRegistros) || (listaNumeroGuias.Count > maximoRegistros))
            {
                return null;
            }
            else
            {
                return ADRepositorio.Instancia.ObtenerGuiasInternas(numeroInicial, numeroFinal, listaNumeroGuias);
            }
        }

        /// <summary>
        /// Método para obtener una guía interna
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public ADGuiaInternaDC ObtenerGuiaInterna(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerGuiaInterna(numeroGuia);
        }

        /// <summary>
        /// Método para obtener una guía interna a partir de un numero de guia, si no existe la guia genere excepción
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public ADGuiaInternaDC ObtenerGuiaInternaNumeroGuia(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerGuiaInternaNumeroGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene los motivos de anulación de una guía
        /// </summary>
        /// <returns>Colección motivos</returns>
        public List<ADMotivoAnulacionDC> ObtenerMotivosAnulacion()
        {
            return ADRepositorio.Instancia.ObtenerMotivosAnulacion();
        }

        /// <summary>
        /// Método para obtener información de los rapiradicados asociados a una admision
        /// </summary>
        /// <returns></returns>
        public List<ADRapiRadicado> ObtenerRapiradicadosGuia(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerRapiradicadosGuia(numeroGuia);
        }

        /// <summary>
        /// Obtener las notificaciones de una guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADNotificacion ObtenerNotificacionGuia(long idAdmision)
        {
            return ADRepositorio.Instancia.ObtenerNotificacionGuia(idAdmision);
        }

        /// <summary>
        /// Retorna el archivo digitalizado de una guía
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADArchivoAlmacenGuia ObtenerArchivoAlmacenGuia(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerArchivoAlmacenGuia(numeroGuia);
        }

        /// <summary>
        /// Retorna la información de una guía dada su forma de pago, en un rango de fechas de admisión, que pertenezcan al cliente dado y al RACOL dado, que
        /// sean del servicio de notificaciones, tipo de envío certificación, que estén descargadas como entrega correcta, que no tengan capturado los datos de
        /// recibido y estén digitalizadas
        /// </summary>
        /// <param name="idFormaPago">Forma de pago</param>
        /// <param name="fechaInicio">Fecha Inicial</param>
        /// <param name="fechaFin">Fecha Final</param>
        /// <param name="idCliente">Id del Cliente</param>
        /// <param name="idRacol">Id del Racol</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasParaCapturaAutomatica(short idFormaPago, DateTime fechaInicio, DateTime fechaFin, int? idCliente, long idRacol)
        {
            return ADRepositorio.Instancia.ObtenerGuiasParaCapturaAutomatica(idFormaPago, fechaInicio, fechaFin, idCliente, idRacol, CO.Servidor.Dominio.Comun.Tarifas.TAConstantesServicios.SERVICIO_NOTIFICACIONES);
        }

        /// <summary>
        /// Retorna información de guía que para ser recibida en forma manual en la parte de notificaciones.
        /// La guía debe estar en estado "Devolución" o "Entrega" y la prueba de entrega o de devolución
        /// correspondiente debe estar digitalizada en la aplicación
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaParaRecibirManualNotificaciones(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerGuiaParaRecibirManualNotificaciones(numeroGuia);
        }

        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una lista de números de guías
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerListaGuias(List<long> listaNumerosGuias)
        {
            return ADRepositorio.Instancia.ObtenerListaGuias(listaNumerosGuias);
        }

        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una cadena separada por comas
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns> 
        public List<ADTrazaGuia> ObtenerListaGuiasSeparadaComas(string listaNumerosGuias)
        {
            List<ADTrazaGuia> listaEstadosGuia = new List<ADTrazaGuia>();

            List<long> lstGuias = listaNumerosGuias.Split(',').ToList().ConvertAll<long>(l =>
            {
                long n = 0;
                long.TryParse(l, out n);
                return n;
            });

            lstGuias = lstGuias.Where(l => l != 0).ToList();

            lstGuias.ForEach(guia =>
            {
                ADTrazaGuia guiaTraza = null;

                if (guia.ToString().Length > 12 && guia.ToString().StartsWith("8"))
                {
                    guiaTraza = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuiaSispostal(guia);

                }
                else
                {
                    guiaTraza = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(guia);
                }




                if (guiaTraza.IdEstadoGuia == 16 || guiaTraza.IdEstadoGuia == 13)
                {
                    List<ADTrazaGuia> lstEstados = EstadosGuia.ObtenerEstadosGuia(guiaTraza.NumeroGuia.Value);
                    var estado = lstEstados.Where(l => l.IdEstadoGuia == 10 || l.IdEstadoGuia == 11).FirstOrDefault();
                    if (estado != null)
                    {
                        guiaTraza.IdEstadoGuia = estado.IdEstadoGuia;
                        guiaTraza.DescripcionEstadoGuia = estado.DescripcionEstadoGuia;
                    }
                }
                listaEstadosGuia.Add(guiaTraza);
            });

            return listaEstadosGuia;
        }



        /// <summary>
        /// Método para obtener las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ADNotificacion> ObtenerNotificacionesRecibido(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return ADRepositorio.Instancia.ObtenerNotificacionesRecibido(filtro, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Obtiene la admision de la mensajeria cliente convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaConvenioConvenio(long idAdmision)
        {
            return ADRepositorio.Instancia.ObtenerAdmisionMensajeriaConvenioConvenio(idAdmision);
        }

        /// <summary>
        /// Obtiene la admision de la mensajeria cliente convenio - peaton
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaConvenioPeaton(long idAdmision)
        {
            return ADRepositorio.Instancia.ObtenerAdmisionMensajeriaConvenioPeaton(idAdmision);
        }

        /// <summary>
        /// Obtiene la admision de la mensajeria cliente peaton - peaton
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaPeatonPeaton(long idAdmision)
        {
            return ADRepositorio.Instancia.ObtenerAdmisionMensajeriaPeatonPeaton(idAdmision);
        }

        /// <summary>
        /// Retorna lista de valores adicionales agregados a una admisión
        /// </summary>
        /// <param name="IdAdmision"></param>
        /// <returns></returns>
        public List<TAValorAdicional> ObtenerValoresAdicionales(long IdAdmision)
        {
            return ADRepositorio.Instancia.ObtenerValoresAdicionales(IdAdmision);
        }

        /// <summary>
        /// Método para obtener los id de las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<long> ObtenerIdNotificaciones(IDictionary<string, string> filtro)
        {
            return ADRepositorio.Instancia.ObtenerIdNotificaciones(filtro);
        }

        /// <summary>
        /// Obtiene la admision de mensajeria peaton-convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaPeatonConvenio(long idAdmision)
        {
            return ADRepositorio.Instancia.ObtenerAdmisionMensajeriaPeatonConvenio(idAdmision);
        }

        /// <summary>
        /// Obtiene la admision de mensajeria peaton-convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADRapiEnvioContraPagoDC ObtenerRapiEnvioContraPago(long idAdmision)
        {
            return ADRepositorio.Instancia.ObtenerRapiEnvioContraPago(idAdmision);
        }

        /// <summary>
        /// Método para obtener las guías de servicio rapiradicado
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<ADRapiRadicado> ObtenerGuiasRapiradicados(IDictionary<string, string> filtro)
        {
            return ADRepositorio.Instancia.ObtenerGuiasRapiradicados(filtro);
        }

        /// <summary>
        /// Obtener guía por número de guía con información de cliente crédito si esta pertenece a un cliente crédito
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(long numeroGuia, long idAdmision)
        {
            return ADRepositorio.Instancia.ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(numeroGuia, idAdmision);
        }

        /// <summary>
        /// Obtiene todas las guias en estado en centro de acopio en una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        public List<ADGuiaUltEstadoDC> ObtenerGuiasEnCentroAcopioLocalidad(string idLocalidad)
        {
            return ADRepositorio.Instancia.ObtenerGuiasEnCentroAcopioLocalidad(idLocalidad);
        }

        /// <summary>
        /// Método para obtener la información de una guía rapiradicado
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADRapiRadicado ObtenerAdmisionRapiradicado(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerAdmisionRapiradicado(numeroGuia);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ObtenerImagenPublicitariaGuia()
        {
            try
            {
                return Convert.ToBase64String(System.IO.File.ReadAllBytes(ADRepositorio.Instancia.ObtenerParametrosAdmisiones().ImagenPublicidadGuia));
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public List<ADGuia> ObtenerAdmisionMensajeriaSinEntregar(AdEnvioNNFiltro envioNNFiltro)
        {
            return ADRepositorio.Instancia.ObtenerAdmisionMensajeriaSinEntregar(envioNNFiltro);
        }





        #region Notificaciones

        /// <summary>
        /// Obtiene la admision de mensajeria para el servicio de notificaciones
        /// </summary>
        /// <param name="numeroGuia"></param>
        public ADNotificacion ObtenerAdmMenNotEntregaDevolucion(long numeroGuia)
        {
            ///Consulta la admision con tipo de servicio de notificacion
            return ADRepositorio.Instancia.ObtenerAdmisionMensajeriaNotificaciones(numeroGuia);
        }

        /// <summary>
        /// Retorna información de guía que para ser recibida en forma manual en la parte de notificaciones.
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerNotificacion(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerNotificacion(numeroGuia);
        }

        /// <summary>
        /// Método para validar una guia notificacion en devolucion
        /// </summary>
        /// <param name="idAdmision"></param>
        public ADNotificacion ValidarNotificacionDevolucion(long idAdmision)
        {
            return ADRepositorio.Instancia.ValidarNotificacionDevolucion(idAdmision);
        }

        /// <summary>
        /// Método para actualizar la tabla notificacion campo ADN_EstaDevuelta , campo ADN_NumeroGuiaInterna
        /// </summary>
        /// <param name="guia"></param>
        public void ActualizarPLanilladaNotificacion(ADNotificacion guia)
        {
            ADRepositorio.Instancia.ActualizarPLanilladaNotificacion(guia);
        }

        /// <summary>
        /// Retorna los datos de las notificaciones no planilladas de tipo CES
        /// </summary>
        /// <param name="idAdmision"></param>
        public List<ADNotificacion> ObtenerNotificacionesEntregaCES(long idCentroServicio, long idCentroServicioOrigen, DateTime fechaInicial, DateTime fechaFinal)
        {
            return ADRepositorio.Instancia.ObtenerNotificacionesEntregaCES(idCentroServicio, idCentroServicioOrigen, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Retorna los datos de las notificaciones no planilladas de tipo CRE
        /// </summary>
        /// <param name="idAdmision"></param>
        public List<ADNotificacion> ObtenerNotificacionesEntregaCRE(DateTime fechaInicial, DateTime fechaFinal, long idCol, long idSucursal)
        {
            return ADRepositorio.Instancia.ObtenerNotificacionesEntregaCRE(fechaInicial, fechaFinal, idCol, idSucursal);
        }

        /// <summary>
        /// Método para obtener las guías internas de una planilla de notificación
        /// </summary>
        /// <param name="idplanilla"></param>
        /// <returns></returns>
        public List<ADGuiaInternaDC> ObtenerGuiasInternasNotificaciones(long idplanilla)
        {
            return ADRepositorio.Instancia.ObtenerGuiasInternasNotificaciones(idplanilla);
        }

        #endregion Notificaciones

        /// <summary>
        /// Consulta el último estado válido para el cliente final con el fin de responder la llamada via IVR
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public string ConsultarEstadoGuiaIVR(long numeroGuia)
        {
            return ADRepositorio.Instancia.ConsultarEstadoGuiaIVR(numeroGuia);
        }

        /// <summary>
        /// Retorna el último estado de una guía
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="_idEstado"></param>
        /// <returns></returns>
        public bool ConsultarUltimoEstadoGuia(long numeroGuia, out int _idEstado, out long idAdmisionMensajeria)
        {
            return ADRepositorio.Instancia.ConsultarUltimoEstadoGuia(numeroGuia, out _idEstado, out idAdmisionMensajeria);
        }


        public bool ExisteCentroServicio(long IdCenSvc)
        {
            return ADRepositorio.Instancia.ExisteCentroServicio(IdCenSvc);
        }

        /// Método para verificar una direccion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="verificadoPor"></param>
        /// <param name="destinatario"></param>
        public void ConfirmarDireccion(long numeroGuia, string verificadoPor, bool destinatario, bool remitente)
        {
            ADRepositorio.Instancia.ConfirmarDireccion(numeroGuia, verificadoPor, destinatario, remitente);
        }

        /// <summary>
        /// Obtiene la ubicacion de una guia para la app del cliente
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADUbicacionGuia ObtenerUbicacionGuia(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerUbicacionGuia(numeroGuia);
        }


        public void ActualizarGuiaImpresa(long NumeroGuia)
        {
            ADRepositorio.Instancia.ActualizarGuiaImpresa(NumeroGuia);
        }


        /// <summary>
        /// Obtiene los estados  de una guia en una localidad
        /// </summary>
        /// <returns></returns>
        public static List<ADTrazaGuia> ObtenerEstadosGuia(long numeroGuia)
        {
            return EstadosGuia.ObtenerEstadosGuia(numeroGuia);
        }


        /// <summary>
        /// Obtiene los Estados y Motivos de la Guia seleccionada
        /// </summary>
        /// <returns></returns>
        public static List<ADEstadoGuiaMotivoDC> ObtenerEstadosMotivosGuia(long numeroGuia)
        {
            return EstadosGuia.ObtenerEstadosMotivosGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene las guias para gestion del estado Indicado
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="IdCentroServicioDestino"></param>
        /// <returns>Lista de ADTrazaGuia</returns>
        public List<ADTrazaGuia> ObtenerGuiasGestion(int idEstadoGuia, long IdCentroServicioDestino)
        {
            return ADRepositorioExt.Instancia.ObtenerGuiasGestion(idEstadoGuia, IdCentroServicioDestino);
        }

        /// <summary>
        /// Obtiene las guias de las agencias para su gestión
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="IdCol"></param>
        /// <returns>Lista de ADTrazaGuiaAgencia</returns>
        public List<ADTrazaGuiaAgencia> ObtenerGuiasGestionAgencias(int idEstadoGuia, long IdCol)
        {
            return ADRepositorioExt.Instancia.ObtenerGuiasGestionAgencias(idEstadoGuia, IdCol);
        }

        /// <summary>
        /// retorna parametros de encabezado guia
        /// </summary>
        /// <param name="llave"></param>
        /// <returns></returns>
        public string ObtenerParametrosEncabezado(string llave)
        {
            return ADRepositorio.Instancia.ObtenerParametrosEncabezado(llave);
        }

        /// <summary>
        /// Consulta la informacion remitente detinatario por numero guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerRemitenteDestinatarioGuia(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerRemitenteDestinatarioGuia(numeroGuia);
        }
        /// <summary>
        /// Consulta la guia validando el cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ConsultarGuia(int idCliente, long numeroGuia)
        {
            return ADRepositorio.Instancia.ConsultarGuia(idCliente, numeroGuia);
        }
    }
}