using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Adminisiones.Mensajeria.Contado;
using CO.Servidor.Adminisiones.Mensajeria.Credito;
using CO.Servidor.Adminisiones.Mensajeria.GuiaInterna;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using System.Threading.Tasks;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System.ServiceModel.Channels;
using System.IO;
using System.Text;

namespace CO.Servidor.Adminisiones.Mensajeria
{
    public class ADFachadaDominio
    {
        private static readonly ADFachadaDominio instancia = new ADFachadaDominio();
        ITAFachadaTarifas fachadaTarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();

        /// <summary>
        /// Retorna una instancia de la fachada de admisiones
        /// /// </summary>
        public static ADFachadaDominio Instancia
        {
            get { return ADFachadaDominio.instancia; }
        }

        /// <summary>
        /// Valida si el servicio está habilitado para el trayecto dado por el municipio de origen y el de destino, debe retornar el peso máximo soportado para dicho trayecto
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="servicio"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, long centroServiciosOrigen, decimal pesoGuia, DateTime? fechadmisionEnvio = null, bool validarTrayecto = true)
        {
            return ADAdmisionContado.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, centroServiciosOrigen, pesoGuia, fechadmisionEnvio, validarTrayecto);
        }


        // TODO ID: Se adiciona este metodo para traer los datos del centro de Servicio Destino(AGE), cuando se activa en Admisiones el Tipo de entrega "RECLAME EN OFICINA"
        /// <summary>
        /// Valida la Agencia-Centro de Servicio Destino para tipo de Entrega "RECLAME EN OFICINA"
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarCentroServicioDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, long centroServiciosOrigen)
        {
            return ADAdmisionContado.Instancia.ValidarCentroServicioDestino(municipioOrigen, municipioDestino, centroServiciosOrigen);
        }



        /// <summary>
        /// Valida si el servicio está habilitado para el trayecto dado por el municipio de origen y el de destino, debe retornar el peso máximo soportado para
        /// dicho trayecto, la duración en días y la prima de seguro acordaba con el cliente en el contrato
        /// </summary>
        /// <param name="municipioOrigen">Municipio de origen</param>
        /// <param name="municipioDestino">Municipio de destino</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <param name="esDesdeSucursal">Indica si la transacción se va a realizar desde la propia sucursal del cliente o si se hace desde una agencia de Interrapidísimo</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <param name="idListaPrecios">Identificador de la lista de precios asociada al contrato</param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestinoCliente(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, int idSucursal, int idCliente, int idListaPrecios, decimal pesoGuia, DateTime? fechadmisionEnvio)
        {
            return ADAdmisionCredito.Instancia.ValidarServicioTrayectoDestinoCliente(municipioOrigen, municipioDestino, servicio, idSucursal, idCliente, idListaPrecios, pesoGuia, fechadmisionEnvio);
        }

        /// <summary>
        /// Obtiene la lista de motivos por los cuales no se hizo uso de la bolsa de seguridad
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADMotivoNoUsoBolsaSeguridad> ObtenerMotivosNoUsoBolsaSeguridad()
        {
            return ADConsultas.Instancia.ObtenerMotivosNoUsoBolsaSeguridad();
        }

        /// <summary>
        /// Retorna lista de objetos de prohibida circulación
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADObjetoProhibidaCirculacion> ObtenerObjetosProhibidaCirculacion()
        {
            return ADConsultas.Instancia.ObtenerObjetosProhibidaCirculacion();
        }

        /// <summary>
        /// Obtener guía por número de guía con información de cliente crédito si esta pertenece a un cliente crédito
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(long numeroGuia, long idAdmision)
        {
            return ADConsultas.Instancia.ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(numeroGuia, idAdmision);
        }

        /// <summary>
        /// Retorna la lista de tipos de entrega que pueden aplicar sobre el envío
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADTipoEntrega> ObtenerTiposEntrega()
        {
            return ADConsultas.Instancia.ObtenerTiposEntrega();
        }

        /// <summary>
        /// Consulta los parámetros de configuración de admisiones
        /// </summary>
        /// <returns></returns>
        public ADParametrosAdmisiones ObtenerParametrosAdmisiones()
        {
            return ADConsultas.Instancia.ObtenerParametrosAdmisiones();
        }

        /// <summary>
        /// Obtiene los motivos de anulación de una guía
        /// </summary>
        /// <returns>Colección motivos</returns>
        public List<ADMotivoAnulacionDC> ObtenerMotivosAnulacion()
        {
            return ADConsultas.Instancia.ObtenerMotivosAnulacion();
        }

        #region Cálculo de precios en tarifas

        /// <summary>
        /// Retorna el precio del servicio
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Colección con precios</returns>
        public TAPrecioCentroCorrespondenciaDC CalcularPrecioCentroCorrespondencia(int idListaPrecios)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioCentroCorrespondencia(idListaPrecios);
        }

        /// <summary>
        /// Calcular el precio para una tarifa internacional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valor Precio</returns>
        public TAPrecioServicioDC CalcularPrecioInternacional(int idListaPrecios, int tipoEmpaque, string idLocalidadDestino, decimal peso, string idZona, decimal valorDeclarado)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioInternacional(idListaPrecios, tipoEmpaque, idLocalidadDestino, peso, idZona, valorDeclarado);
        }

        /// <summary>
        /// Obtiene el precio del servicio trámites
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioTramiteDC CalcularPrecioTramites(int idListaPrecios, int idTramite)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioTramites(idListaPrecios, idTramite);
        }

        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiTulas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiTulas(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValoresMsj(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiValoresMsj(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValoresCarga(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiValoresCarga(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiCargaConsolidado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiCargaConsolidado(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValijas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiValijas(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        /// <summary>
        /// Obtiene el precio del servicio carga express
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <returns></returns>
        public TAPrecioMensajeriaDC CalcularPrecioCargaExpress(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioCargaExpress(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);

        }

        public TAPrecioMensajeriaDC CalcularPrecioCargaAerea(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioCargaAerea(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public List<TAPreciosAgrupadosDC> CalcularPrecioServicios(List<int> servicios, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega)
        {
            List<TAPreciosAgrupadosDC> precios = new List<TAPreciosAgrupadosDC>();

            if (Convert.ToInt16(idTipoEntrega) <= 2)
            {
                foreach (int servicio in servicios)
                {
                    switch (servicio)
                    {
                        case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                            TAPreciosAgrupadosDC prec = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                prec.Precio = CalcularPrecioCargaExpress(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(prec);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                prec.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_CARGA_AEREA:
                            TAPreciosAgrupadosDC precAr = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precAr.Precio = CalcularPrecioCargaAerea(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado);
                                precios.Add(precAr);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precAr.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_MENSAJERIA:
                            TAPreciosAgrupadosDC precMsj = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precMsj.Precio = CalcularPrecioMensajeria(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precMsj);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precMsj.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_NOTIFICACIONES:
                            TAPreciosAgrupadosDC precNot = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precNot.Precio = CalcularPrecioNotificaciones(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precNot);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precNot.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_AM:
                            TAPreciosAgrupadosDC precAm = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precAm.Precio = CalcularPrecioRapiAm(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precAm);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precAm.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_CARGA:
                            TAPreciosAgrupadosDC precCar = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                var precio = CalcularPrecioRapiCarga(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precCar.PrecioCarga = precio;
                                precCar.Precio = new TAPrecioMensajeriaDC()
                                {
                                    Impuestos = precio.Impuestos,
                                    Valor = precio.Valor,
                                    ValorContraPago = precio.ValorContraPago,
                                    ValorKiloAdicional = precio.ValorKiloAdicional,
                                    ValorPrimaSeguro = precio.ValorPrimaSeguro
                                };
                                precios.Add(precCar);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precCar.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_CARGA_CONTRA_PAGO:

                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO:

                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_HOY:
                            TAPreciosAgrupadosDC precHoy = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precHoy.Precio = CalcularPrecioRapiHoy(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precHoy);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precHoy.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO:
                            TAPreciosAgrupadosDC precPer = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precPer.Precio = CalcularPrecioRapiPersonalizado(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precPer);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precPer.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_PROMOCIONAL:
                            break;

                        case TAConstantesServicios.SERVICIO_RAPIRADICADO:
                            TAPreciosAgrupadosDC precRadicado = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precRadicado.Precio = CalcularPrecioRapiradicado(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precRadicado);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precRadicado.Mensaje = ex.Detail.Mensaje;
                            }
                            break;
                    }
                }
            }
            else
            {
                foreach (int servicio in servicios)
                {
                    switch (servicio)
                    {
                        case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                            TAPreciosAgrupadosDC prec = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                prec.Precio = CalcularPrecioCargaExpress(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(prec);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                prec.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_MENSAJERIA:
                            TAPreciosAgrupadosDC precMsj = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precMsj.Precio = CalcularPrecioMensajeria(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precMsj);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precMsj.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_NOTIFICACIONES:
                            TAPreciosAgrupadosDC precNot = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precNot.Precio = CalcularPrecioMensajeria(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precNot);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precNot.Mensaje = ex.Detail.Mensaje;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPIRADICADO:
                            TAPreciosAgrupadosDC precRadicado = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precRadicado.Precio = CalcularPrecioCargaExpress(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precRadicado);
                            }
                            catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            {
                                precRadicado.Mensaje = ex.Detail.Mensaje;
                            }
                            break;
                    }
                }
            }
            return precios;
        }


        /// <summary>
        /// Obtiene el precio del servicio rapi promocional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioServicioDC CalcularPrecioRapiPromocional(int idListaPrecios, decimal cantidad, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiPromocional(idListaPrecios, cantidad);
        }


        /// <summary>
        /// Mètodo para obtener informaciòn de precio servicio mobile
        /// </summary>
        /// <param name="servicios"></param>
        /// <param name="idListaPrecio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>
        public List<TAPreciosAgrupadosDC> CalcularPrecioServiciosMobile(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega)
        {
            ITAFachadaTarifas fachadaTarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();

            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

            List<TAServicioDC> listaServicios = fachadaTarifas.ObtenerServicios();
            int idListaPrecio = fachadaTarifas.ObtenerIdListaPrecioVigente();
            List<TAPreciosAgrupadosDC> precios = new List<TAPreciosAgrupadosDC>();


            foreach (TAServicioDC servicio in listaServicios)
            {

                string nombreServicio = fachadaTarifas.ObtenerDatosServicio(servicio.IdServicio).Descripcion;

                bool destinoCosta = fachadaTarifas.ValidarServicioTrayectoCasilleroAereo(idLocalidadOrigen, idLocalidadDestino, servicio.IdServicio);

                Dictionary<string, string> filtro = new Dictionary<string, string>();
                filtro.Add("MSA_IdLocalidad", idLocalidadDestino);

                int totalReg = 0;

                var centroSerDes = fachadaCentroServicios.ObtenerMunicipiosSinFormaPagoAlCobro(filtro, "", 0, 10, true, out totalReg);

                bool destinoPermiteAlcobro = centroSerDes.Count() > 0 ? false : true;

                switch (servicio.IdServicio)
                {
                    case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                        TAPreciosAgrupadosDC prec = new TAPreciosAgrupadosDC() { IdServicio = servicio.IdServicio };
                        try
                        {

                            prec.Precio = CalcularPrecioCargaExpress(servicio.IdServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                            int tiempoEntrega = fachadaTarifas.ValidarServicioTrayectoDestino(new PALocalidadDC { IdLocalidad = idLocalidadOrigen }, new PALocalidadDC { IdLocalidad = idLocalidadDestino }, new TAServicioDC { IdServicio = TAConstantesServicios.SERVICIO_CARGA_EXPRESS }).numeroDiasEntrega;
                            prec.TiempoEntrega = tiempoEntrega.ToString();
                            prec.NombreServicio = !destinoCosta ? nombreServicio : "Aereo_Rapicarga";
                            prec.FormaPagoServicio = new TAFormaPagoServicio()
                            {
                                IdServicio = servicio.IdServicio,
                                FormaPago = new List<TAFormaPago>()
                            };
                            prec.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                            if (destinoPermiteAlcobro)
                                prec.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });

                            precios.Add(prec);
                        }
                        catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        {
                            prec.Mensaje = ex.Detail.Mensaje;
                        }
                        break;
                    case TAConstantesServicios.SERVICIO_CARGA_AEREA:
                        TAPreciosAgrupadosDC precAerea = new TAPreciosAgrupadosDC() { IdServicio = servicio.IdServicio };
                        try
                        {
                            precAerea.Precio = CalcularPrecioCargaExpress(servicio.IdServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                            int tiempoEntrega = fachadaTarifas.ValidarServicioTrayectoDestino(new PALocalidadDC { IdLocalidad = idLocalidadOrigen }, new PALocalidadDC { IdLocalidad = idLocalidadDestino }, new TAServicioDC { IdServicio = TAConstantesServicios.SERVICIO_CARGA_AEREA }).numeroDiasEntrega;
                            precAerea.TiempoEntrega = tiempoEntrega.ToString();
                            precAerea.NombreServicio = nombreServicio;

                            precAerea.FormaPagoServicio = new TAFormaPagoServicio()
                            {
                                IdServicio = servicio.IdServicio,
                                FormaPago = new List<TAFormaPago>()
                            };
                            precAerea.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                            if (destinoPermiteAlcobro)
                                precAerea.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });

                            precios.Add(precAerea);
                        }
                        catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        {
                            precAerea.Mensaje = ex.Detail.Mensaje;
                        }
                        break;
                    case TAConstantesServicios.SERVICIO_MENSAJERIA:
                        TAPreciosAgrupadosDC precMsj = new TAPreciosAgrupadosDC() { IdServicio = servicio.IdServicio };
                        try
                        {
                            precMsj.Precio = CalcularPrecioMensajeria(servicio.IdServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);

                            //Validacion para el tipo de servicio 3 cuando es igual o mayor a 3 Kg el envio, se valida si es costa con el fin de asignar el servicio 17 en caso contrario se asigna el 6 o 3 segun corresponda
                            if (servicio.IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA && peso >= 3)
                            {
                                List<int> lstIdServicios = fachadaTarifas.ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(idLocalidadOrigen, idLocalidadDestino);
                                if (lstIdServicios != null && lstIdServicios.Count > 0)
                                {
                                    bool esCosta = fachadaTarifas.ValidarServicioTrayectoCasilleroAereo(idLocalidadOrigen, idLocalidadDestino, servicio.IdServicio);

                                    if (esCosta)
                                    {
                                        servicio.IdServicio = lstIdServicios.FirstOrDefault();
                                    }
                                    else if (lstIdServicios.Count == 1)
                                    {
                                        servicio.IdServicio = lstIdServicios.FirstOrDefault();
                                    }
                                    else
                                    {
                                        servicio.IdServicio = lstIdServicios.FirstOrDefault(s => s != 17);
                                    }
                                }
                            }

                            int tiempoEntrega = fachadaTarifas.ValidarServicioTrayectoDestino(new PALocalidadDC { IdLocalidad = idLocalidadOrigen }, new PALocalidadDC { IdLocalidad = idLocalidadDestino }, new TAServicioDC { IdServicio = TAConstantesServicios.SERVICIO_MENSAJERIA }).numeroDiasEntrega;
                            precMsj.TiempoEntrega = tiempoEntrega.ToString();

                            precMsj.NombreServicio = nombreServicio;

                            precMsj.FormaPagoServicio = new TAFormaPagoServicio()
                            {
                                IdServicio = servicio.IdServicio,
                                FormaPago = new List<TAFormaPago>()
                            };
                            precMsj.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                            if (destinoPermiteAlcobro)
                                precMsj.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });

                            precios.Add(precMsj);
                        }
                        catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        {
                            precMsj.Mensaje = ex.Detail.Mensaje;
                        }
                        break;

                    case TAConstantesServicios.SERVICIO_NOTIFICACIONES:
                        TAPreciosAgrupadosDC precNot = new TAPreciosAgrupadosDC() { IdServicio = servicio.IdServicio };
                        try
                        {
                            precNot.Precio = CalcularPrecioNotificaciones(servicio.IdServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                            int TiempoEntrega = fachadaTarifas.ValidarServicioTrayectoDestino(new PALocalidadDC { IdLocalidad = idLocalidadOrigen }, new PALocalidadDC { IdLocalidad = idLocalidadDestino }, new TAServicioDC { IdServicio = TAConstantesServicios.SERVICIO_NOTIFICACIONES }).numeroDiasEntrega;
                            precNot.TiempoEntrega = TiempoEntrega.ToString();
                            precNot.NombreServicio = nombreServicio;
                            precNot.FormaPagoServicio = new TAFormaPagoServicio()
                            {
                                IdServicio = servicio.IdServicio,
                                FormaPago = new List<TAFormaPago>()
                            };
                            precNot.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                            if (destinoPermiteAlcobro)
                                precNot.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });
                            precios.Add(precNot);
                        }
                        catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        {
                            precNot.Mensaje = ex.Detail.Mensaje;
                        }
                        break;

                    case TAConstantesServicios.SERVICIO_RAPI_AM:
                        TAPreciosAgrupadosDC precAm = new TAPreciosAgrupadosDC() { IdServicio = servicio.IdServicio };
                        try
                        {
                            precAm.Precio = CalcularPrecioRapiAm(servicio.IdServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                            int TiempoEntrega = fachadaTarifas.ValidarServicioTrayectoDestino(new PALocalidadDC { IdLocalidad = idLocalidadOrigen }, new PALocalidadDC { IdLocalidad = idLocalidadDestino }, new TAServicioDC { IdServicio = TAConstantesServicios.SERVICIO_RAPI_AM }).numeroDiasEntrega;
                            precAm.TiempoEntrega = TiempoEntrega.ToString();
                            precAm.NombreServicio = nombreServicio;
                            precAm.FormaPagoServicio = new TAFormaPagoServicio()
                            {
                                IdServicio = servicio.IdServicio,
                                FormaPago = new List<TAFormaPago>()
                            };
                            precAm.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                            if (destinoPermiteAlcobro)
                                precAm.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });
                            precios.Add(precAm);
                        }
                        catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        {
                            precAm.Mensaje = ex.Detail.Mensaje;
                        }
                        break;

                    case TAConstantesServicios.SERVICIO_RAPI_CARGA:
                        TAPreciosAgrupadosDC precCar = new TAPreciosAgrupadosDC() { IdServicio = servicio.IdServicio };
                        try
                        {
                            precCar.PrecioCarga = CalcularPrecioRapiCarga(servicio.IdServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                            int TiempoEntrega = fachadaTarifas.ValidarServicioTrayectoDestino(new PALocalidadDC { IdLocalidad = idLocalidadOrigen }, new PALocalidadDC { IdLocalidad = idLocalidadDestino }, new TAServicioDC { IdServicio = TAConstantesServicios.SERVICIO_RAPI_CARGA }).numeroDiasEntrega;
                            precCar.TiempoEntrega = TiempoEntrega.ToString();
                            precCar.NombreServicio = !destinoCosta ? nombreServicio : "Terrestre_Rapicarga";
                            precCar.FormaPagoServicio = new TAFormaPagoServicio()
                            {
                                IdServicio = servicio.IdServicio,
                                FormaPago = new List<TAFormaPago>()
                            };
                            precCar.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                            if (destinoPermiteAlcobro)
                                precCar.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });
                            precios.Add(precCar);
                        }
                        catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        {
                            precCar.Mensaje = ex.Detail.Mensaje;
                        }
                        break;

                    case TAConstantesServicios.SERVICIO_RAPI_CARGA_CONTRA_PAGO:

                        break;

                    case TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO:

                        break;

                    case TAConstantesServicios.SERVICIO_RAPI_HOY:
                        TAPreciosAgrupadosDC precHoy = new TAPreciosAgrupadosDC() { IdServicio = servicio.IdServicio };
                        try
                        {
                            precHoy.Precio = CalcularPrecioRapiHoy(servicio.IdServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                            int TiempoEntrega = fachadaTarifas.ValidarServicioTrayectoDestino(new PALocalidadDC { IdLocalidad = idLocalidadOrigen }, new PALocalidadDC { IdLocalidad = idLocalidadDestino }, new TAServicioDC { IdServicio = TAConstantesServicios.SERVICIO_RAPI_HOY }).numeroDiasEntrega;
                            precHoy.TiempoEntrega = TiempoEntrega.ToString();
                            precHoy.NombreServicio = nombreServicio;
                            precHoy.FormaPagoServicio = new TAFormaPagoServicio()
                            {
                                IdServicio = servicio.IdServicio,
                                FormaPago = new List<TAFormaPago>()
                            };
                            precHoy.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                            if (destinoPermiteAlcobro)
                                precHoy.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });
                            precios.Add(precHoy);
                        }
                        catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        {
                            precHoy.Mensaje = ex.Detail.Mensaje;
                        }
                        break;

                    case TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO:
                        TAPreciosAgrupadosDC precPer = new TAPreciosAgrupadosDC() { IdServicio = servicio.IdServicio };
                        try
                        {
                            precPer.Precio = CalcularPrecioRapiPersonalizado(servicio.IdServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                            int TiempoEntrega = fachadaTarifas.ValidarServicioTrayectoDestino(new PALocalidadDC { IdLocalidad = idLocalidadOrigen }, new PALocalidadDC { IdLocalidad = idLocalidadDestino }, new TAServicioDC { IdServicio = TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO }).numeroDiasEntrega;
                            precPer.TiempoEntrega = TiempoEntrega.ToString();
                            precPer.NombreServicio = nombreServicio;
                            precPer.FormaPagoServicio = new TAFormaPagoServicio()
                            {
                                IdServicio = servicio.IdServicio,
                                FormaPago = new List<TAFormaPago>()
                            };
                            precPer.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                            if (destinoPermiteAlcobro)
                                precPer.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });
                            precios.Add(precPer);
                        }
                        catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        {
                            precPer.Mensaje = ex.Detail.Mensaje;
                        }
                        break;

                    case TAConstantesServicios.SERVICIO_RAPI_PROMOCIONAL:
                        break;

                    case TAConstantesServicios.SERVICIO_RAPIRADICADO:
                        TAPreciosAgrupadosDC precRadicado = new TAPreciosAgrupadosDC() { IdServicio = servicio.IdServicio };
                        try
                        {
                            precRadicado.Precio = CalcularPrecioRapiradicado(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                            int TiempoEntrega = fachadaTarifas.ValidarServicioTrayectoDestino(new PALocalidadDC { IdLocalidad = idLocalidadOrigen }, new PALocalidadDC { IdLocalidad = idLocalidadDestino }, new TAServicioDC { IdServicio = TAConstantesServicios.SERVICIO_RAPIRADICADO }).numeroDiasEntrega;
                            precRadicado.TiempoEntrega = TiempoEntrega.ToString();
                            precRadicado.NombreServicio = nombreServicio;

                            precRadicado.FormaPagoServicio = new TAFormaPagoServicio()
                            {
                                IdServicio = servicio.IdServicio,
                                FormaPago = new List<TAFormaPago>()
                            };
                            precRadicado.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                            if (destinoPermiteAlcobro)
                                precRadicado.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });

                            precios.Add(precRadicado);
                        }
                        catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        {
                            precRadicado.Mensaje = ex.Detail.Mensaje;
                        }
                        break;
                }
            }
            return precios;
        }

        /// <summary>
        /// Nuevo Cotizador
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>
        public List<TAPreciosAgrupadosDC> CalculaServicioCotizador(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega)
        {
            int dia = (int)DateTime.Now.DayOfWeek; // día de la semana actual

            ITAFachadaTarifas fachadaTarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

            List<TAServicioDC> listaServicios = fachadaTarifas.ObtenerServicios();  // Obtengo todos los servicios habilitados           

            List<TAServicioPesoDC> lstServiciosPesos = fachadaTarifas.ConsultarServiciosPesosMinimoxMaximos(); //Metodo Mínimos y Máximos

            List<TAServicioDC> lstServiciosHabiles = new List<TAServicioDC>();

            TAServicioDC servicioRapicarga = null;
            listaServicios.ForEach(servicio =>
            {

                var servicioPeso = lstServiciosPesos.Where(p => p.IdServicio == servicio.IdServicio).FirstOrDefault();
                if (servicioPeso != null)
                {
                    if (peso >= servicioPeso.PesoMinimo && peso <= servicioPeso.PesoMaximo)
                    {

                        if (servicioPeso.IdServicio != TAConstantesServicios.SERVICIO_RAPI_CARGA)
                            lstServiciosHabiles.Add(servicio);
                        else
                            servicioRapicarga = servicio;
                    }
                }
            });





            int idListaPrecios = fachadaTarifas.ObtenerIdListaPrecioVigente();


            List<TAPreciosAgrupadosDC> lstPreciosAgrupados = CalcularPrecioServicios(lstServiciosHabiles.Select(s => s.IdServicio).ToList(), idListaPrecios, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega);

            if (servicioRapicarga != null)
            {
                decimal topeMinRapiCarga = ObtenerParametrosAdmisiones().TopeMinVlrDeclRapiCarga;
                List<int> lstSerRapicarga = new List<int>();
                lstSerRapicarga.Add(servicioRapicarga.IdServicio);
                lstPreciosAgrupados.AddRange(CalcularPrecioServicios(lstSerRapicarga, idListaPrecios, idLocalidadOrigen, idLocalidadDestino, peso, topeMinRapiCarga, idTipoEntrega));                
                lstServiciosHabiles.Add(servicioRapicarga);
            }


            PUCentroServiciosDC centroOrigen = fachadaCentroServicios.ObtenerAgenciaLocalidad(idLocalidadOrigen);

            PALocalidadDC localidadOrigen = new PALocalidadDC()
            {
                IdLocalidad = idLocalidadOrigen
            };
            PALocalidadDC localidadDestino = new PALocalidadDC()
            {
                IdLocalidad = idLocalidadDestino
            };




            Dictionary<string, string> filtro = new Dictionary<string, string>();
            filtro.Add("MSA_IdLocalidad", idLocalidadDestino);
            int totalReg = 0;
            var centroSerDes = fachadaCentroServicios.ObtenerMunicipiosSinFormaPagoAlCobro(filtro, "", 0, 10, true, out totalReg);

            bool destinoPermiteAlcobro = centroSerDes.Count() > 0 ? false : true;

            List<Task> lstTareas = new List<Task>();

            bool costa = false;
           lstPreciosAgrupados.ForEach(ps =>
               {

                   //lstTareas.Add(Task.Factory.StartNew(()=>
                   //    {

                    ADValidacionServicioTrayectoDestino validacionTiempo = ADAdmisionContado.Instancia.ValidarServicioTrayectoDestino(localidadOrigen, localidadDestino, new TAServicioDC() { IdServicio = ps.IdServicio }, centroOrigen.IdCentroServicio, peso);

                    ps.TiempoEntrega = (validacionTiempo.DuracionTrayectoEnHoras / 24).ToString();

                    bool destinoCosta = fachadaTarifas.ValidarServicioTrayectoCasilleroAereo(idLocalidadOrigen, idLocalidadDestino, ps.IdServicio);

                    var ser = lstServiciosHabiles.Where(s => s.IdServicio == ps.IdServicio).FirstOrDefault();
                    if (ser != null)
                    {
                        ps.NombreServicio = ser.Nombre;
                    }


                    ps.FormaPagoServicio = new TAFormaPagoServicio()
                    {
                        IdServicio = ps.IdServicio,
                        FormaPago = new List<TAFormaPago>()
                    };
                    ps.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                    if (destinoPermiteAlcobro && ps.IdServicio != TAConstantesServicios.SERVICIO_NOTIFICACIONES)
                        ps.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });


                   if(destinoCosta)
                   {
                       switch (ps.IdServicio)
                       {
                           case TAConstantesServicios.SERVICIO_RAPI_CARGA:                              
                                   ps.NombreServicio = "Carga Terrestre";
                                   costa = true;
                               break;

                           case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                                   ps.NombreServicio = "Aéreo Costa";
                                   costa = true;
                               break;
                       }

                   }

                   if (ps.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                   {
                       ps.NombreServicio = "Carga Terrestre";
                   }                  
               });

            //se revalida si no es costa, y estan los dos servicios de carga (ids: 6,17) se deja solo el terrestre(6)
            if (!costa)
            {

                if (lstPreciosAgrupados.Where(s => s.IdServicio == 6 || s.IdServicio == 17).Count() >= 2)
                {
                    var ser = lstPreciosAgrupados.Where(s => s.IdServicio == 17).FirstOrDefault();
                    if (ser != null)
                    {
                        lstPreciosAgrupados.Remove(ser);
                    }
                }
                else //Si hay un solo servicio de carga 6 o 17
                {
                    var ser = lstPreciosAgrupados.Where(s => s.IdServicio == 6 || s.IdServicio == 17).FirstOrDefault();
                    if (ser != null)
                    {
                        ser.NombreServicio = "Rapi Carga";
                    }
                }               

            }           

            return lstPreciosAgrupados;
        }

        public List<ADGuia> ObtenerReporteCajaGuiasMensajeroApp(long idMensajero)
        {
            return ADAdmisionMensajeria.Instancia.ObtenerReporteCajaGuiasMensajeroApp(idMensajero);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi radicado
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <returns></returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiradicado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiRadicado(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi personalizado
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiPersonalizado(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiPersonalizado(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi hoy
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiHoy(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiHoy(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi envíos contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiEnvioContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiEnvioContraPago(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorContraPago, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCargaContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiCargaContraPago(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorContraPago, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCarga(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiCarga(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi AM
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiAm(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiAm(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio notificaciones
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioNotificaciones(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioNotificaciones(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        #endregion Cálculo de precios en tarifas

        #region Inserciones

        /// <summary>
        /// Método para adicionar una guia interna
        /// </summary>
        /// <returns>Identificador de la admisión de la guía interna</returns>
        public ADGuiaInternaDC AdicionarGuiaInterna(ADGuiaInternaDC guiaInterna)
        {
            return ADAdmisionGuiaInterna.Instancia.AdicionarGuiaInterna(guiaInterna);
        }

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaManual(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManual(guia, idCaja, remitenteDestinatario, false);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManual(guia, idCaja, remitenteDestinatario);
            }
        }

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaManualCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, long idAgenciaRegistraAdmision)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManual(guia, idCaja, remitenteDestinatario, true, idAgenciaRegistraAdmision);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualCOL(guia, remitenteDestinatario, idAgenciaRegistraAdmision);
            }
        }

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaManualInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque, false);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque);
            }
        }

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaManualInternacionalCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, long idAgenciaRegistraAdmision)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque, true, idAgenciaRegistraAdmision);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualInternacionalCOL(guia, remitenteDestinatario, tipoEmpaque, idAgenciaRegistraAdmision);
            }
        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        public ADResultadoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaAutomatica(guia, idCaja, remitenteDestinatario);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaAutomatica(guia, idCaja, remitenteDestinatario);
            }
        }

        /// <summary>
        /// Registra guía automática internacional
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="tipoEmpaque"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaAutomaticaInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaAutomaticaInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaAutomaticaInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque);
            }
        }


        /// <summary>
        /// Registra guía automática internacional con DHL
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="tipoEmpaque"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaAutomaticaInternacional_DHL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, ADGuiaInternacionalDC guiaInternacional)
        {

            return ADAdmisionContado.Instancia.RegistrarGuiaAutomaticaInternacional_DHL(guia, idCaja, remitenteDestinatario, tipoEmpaque, guiaInternacional);
        }

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        public ADResultadoAdmision RegistrarGuiaAutomaticaNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaAutomaticaNotificacion(guia, idCaja, remitenteDestinatario, notificacion);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaAutomaticaNotificacion(guia, idCaja, remitenteDestinatario, notificacion);
            }
        }

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        public ADResultadoAdmision RegistrarGuiaManualNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualNotificacion(guia, idCaja, remitenteDestinatario, notificacion, false);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualNotificacion(guia, idCaja, remitenteDestinatario, notificacion);
            }
        }

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        public ADResultadoAdmision RegistrarGuiaManualNotificacionCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion, long idAgenciaRegistraAdmision)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualNotificacion(guia, idCaja, remitenteDestinatario, notificacion, true, idAgenciaRegistraAdmision);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualNotificacionCOL(guia, remitenteDestinatario, notificacion, idAgenciaRegistraAdmision);
            }
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        public ADResultadoAdmision RegistrarGuiaAutomaticaRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaAutomaticaRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaAutomaticaRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago);
            }
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        public ADResultadoAdmision RegistrarGuiaManualRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago, false);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago);
            }
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        public ADResultadoAdmision RegistrarGuiaManualRapiEnvioContraPagoCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago, long idAgenciaRegistraAdmision)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago, true, idAgenciaRegistraAdmision);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualRapiEnvioContraPagoCOL(guia, remitenteDestinatario, rapiEnvioContraPago, idAgenciaRegistraAdmision);
            }
        }

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        public ADResultadoAdmision RegistrarGuiaAutomaticaRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaAutomaticaRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaAutomaticaRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado);
            }
        }

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        public ADResultadoAdmision RegistrarGuiaManualRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado, false);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado);
            }
        }

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        public ADResultadoAdmision RegistrarGuiaManualRapiRadicadoCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado, long idAgenciaRegistraAdmision)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado, true, idAgenciaRegistraAdmision);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualRapiRadicadoCOL(guia, remitenteDestinatario, rapiRadicado, idAgenciaRegistraAdmision);
            }
        }

        /// <summary>
        /// Actualiza como pagada una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarPagadoGuia(long idAdmisionMensajeria)
        {
            ADAdmisionMensajeria.Instancia.ActualizarPagadoGuia(idAdmisionMensajeria);
        }

        /// <summary>
        /// Actualiza en supervision una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarSupervisionGuia(long idAdmisionMensajeria)
        {
            ADAdmisionMensajeria.Instancia.ActualizarSupervisionGuia(idAdmisionMensajeria);
        }

        #endregion Inserciones

        /// <summary>
        /// Genera solicitud de falla
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="valorCobrado"></param>
        /// <param name="valorCalculado"></param>
        public void GenerarFallaCalculoValorGuiaManual(long numeroGuia, decimal valorCobrado, decimal valorCalculado)
        {
            ADAdmisionContado.Instancia.GenerarFallaCalculoValorGuiaManual(numeroGuia, valorCobrado, valorCalculado);
        }

        /// <summary>
        /// Retorna la lista de condiciones dado el operador postal
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns></returns>
        public IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionesPorOperadorPostal(int idOperadorPostal)
        {
            return Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerCondicionesPorOperadorPostal(idOperadorPostal);
        }

        /// <summary>
        /// Consulta una guía por guid
        /// </summary>
        /// <param name="guid">Valor que identifica la transacción</param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision ObtenerGuiaPorGuid(string guid)
        {
            return ADConsultas.Instancia.ObtenerGuiaPorGuid(guid);
        }

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioGuia(long numeroGuia, long idSucursalCentroServicio)
        {
            return ADConsultas.Instancia.ObtenerPropietarioGuia(numeroGuia, idSucursalCentroServicio);
        }

        /// <summary>
        /// Retorna la información de una guía completa incluyendo la forma como se pagó, se construyó para generar impresión
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaPorNumeroDeGuiaCompleta(long numeroGuia, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            return ADConsultas.Instancia.ObtenerGuiaPorNumeroDeGuiaCompleta(numeroGuia, idCentroServicios, idSucursal, idCliente);
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaXNumeroGuia(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerGuiaXNumeroGuia(numeroGuia);
        }

        public ADGuia ObtenerGuiaSispostalXNumeroGuia(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerGuiaSispostalXNumeroGuia(numeroGuia);
        }


        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaXNumeroGuiaCredito(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerGuiaXNumeroGuiaCredito(numeroGuia);
        }

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idRemitente">Número de identificación del cliente remitente</param>
        /// <param name="tipoIdRemitente">Tipo de identificación del cliente remitente</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorRemitenteParaHoy(string idRemitente, string tipoIdRemitente, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            return ADConsultas.Instancia.ObtenerGuiasPorRemitenteParaHoy(idRemitente, tipoIdRemitente, idCentroServicios, idSucursal, idCliente);
        }

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idDestinatario">Número de identificación del cliente destinatario</param>
        /// <param name="tipoIdDestinatario">Tipo de identificación del cliente destinatario</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorDestinatarioParaHoy(string idDestinatario, string tipoIdDestinatario, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            return ADConsultas.Instancia.ObtenerGuiasPorDestinatarioParaHoy(idDestinatario, tipoIdDestinatario, idCentroServicios, idSucursal, idCliente);
        }

        /// <summary>
        /// Obtiene las guias al cobro no pagas.
        /// </summary>
        /// <param name="numeroGuia">The numero guia.</param>
        /// <param name="fechaInicial">The fecha inicial.</param>
        /// <returns>Lista de Guias al Cobro sin pagar</returns>
        public List<ADGuiaAlCobro> ObtenerGuiasAlCobroNoPagas(int indicePagina, int registrosPorPagina, long numeroGuia, DateTime fechaInicial, DateTime fechaFinal, long idCentroServicio)
        {
            return ADConsultas.Instancia.ObtenerGuiasAlCobroNoPagas(indicePagina, registrosPorPagina, numeroGuia, fechaInicial, fechaFinal, idCentroServicio);
        }

        /// <summary>
        /// Actualiza la guía y registra el valor en Caja de la transaccion.
        /// </summary>
        /// <param name="guiaAlCobro">The guia al cobro.</param>
        public ADRecaudoAlCobro ActualizarGuiaAlCobro(ADRecaudarDineroAlCobroDC guiaAlCobro)
        {
            return ADAdmisionMensajeria.Instancia.ActualizarGuiaAlCobro(guiaAlCobro);
        }

        /// <summary>
        /// Método para obtener un rango de guías internas
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public List<ADGuiaInternaDC> ObtenerGuiasInternas(long numeroInicial, long numeroFinal, List<long> listaNumeroGuias)
        {
            return ADConsultas.Instancia.ObtenerGuiasInternas(numeroInicial, numeroFinal, listaNumeroGuias);
        }

        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una lista de números de guías
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerListaGuias(List<long> listaNumerosGuias)
        {
            return ADConsultas.Instancia.ObtenerListaGuias(listaNumerosGuias);
        }

         /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una cadena separada por comas
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ADTrazaGuia> ObtenerListaGuiasSeparadaComas(string listaNumerosGuias)
        {
            return ADConsultas.Instancia.ObtenerListaGuiasSeparadaComas(listaNumerosGuias);
        }


        /// <summary>
        /// Crea una orden de servicio para asociar las guias de un cargue masivo de mensajeria
        /// </summary>
        /// <param name="idOrdenServicio">Retorna el número de orden de servicio creado</param>
        public long CrearOrdenServicioMasivo(ADOrdenServicioMasivoDC datosOrdenServicio)
        {
            return ADAdmisionMensajeria.Instancia.CrearOrdenServicioMasivo(datosOrdenServicio);
        }

        /// <summary>
        /// Validar la existencia de una orden de servicio masiva
        /// </summary>
        /// <param name="ordenServicio">número de la orden de servicio a validar</param>
        /// <returns></returns>
        public bool ValidarOrdenServicio(long ordenServicio)
        {
            return ADAdmisionMensajeria.Instancia.ValidarOrdenServicio(ordenServicio);
        }

        /// <summary>
        /// Valida cada uno de los envíos que se desea asociar a una orden de servicio antes de almacenarla en la base de datos
        /// </summary>
        /// <param name="enviosParaValidar">Listado de los envíos que se desean validar</param>
        /// <returns>Listado de los envíos validados y su respectivo resultado</returns>
        public List<ADDatosValidadosDC> ValidarDatosGuiasOrdenMasiva(List<ADDatosValidacionDC> enviosParaValidar, bool esAutomatica)
        {
            List<ADDatosValidadosDC> datosValidados = new List<ADDatosValidadosDC>();
            List<Task> lstTareas = new List<Task>();

            string usuario = ControllerContext.Current.Usuario;

            foreach (ADDatosValidacionDC datoValidacion in enviosParaValidar)
            {

                lstTareas.Add(Task.Factory.StartNew(() =>
                {
                    MockedOperationContextUtil.CrearControllerContextTareas();
                    ControllerContext.Current.Usuario = usuario;

                    ADDatosValidadosDC datoValidado = new ADDatosValidadosDC();
                    try
                    {
                        datoValidado.NoFila = datoValidacion.NoFila;

                        if (!esAutomatica)
                        {
                            if (datoValidacion.Credito)
                            {
                                SUPropietarioGuia propietarioGuia = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().ObtenerPropietarioSuministro(datoValidacion.Numeroguia, SUEnumSuministro.GUIA_TRANSPORTE_MANUAL, datoValidacion.IdcentroServiciosOrigen);
                                if (propietarioGuia.Id != datoValidacion.IdSucursalClienteOrigen)
                                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, "0", "Este número de guía no es válido para el cliente y sucursal seleccionados."));
                            }
                            else
                            {
                                SUPropietarioGuia propietarioGuia = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().ObtenerPropietarioSuministro(datoValidacion.Numeroguia, SUEnumSuministro.FACTURA_VENTA_MENSAJERIA_MANUAL, datoValidacion.IdcentroServiciosOrigen);
                                if (propietarioGuia.Id != datoValidacion.IdcentroServiciosOrigen)
                                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, "0", "Este número de factura no es válido para el centro de servicios origen."));
                            }
                        }



                        ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();

                        TAServicioPesoDC pesoServicio = tarifas.ObtenerServicioPeso(datoValidacion.TipoServicio.IdServicio);

                        if ((datoValidacion.peso < pesoServicio.PesoMinimo) || (datoValidacion.peso > pesoServicio.PesoMaximo))
                            throw new Exception("Peso no  valido");
                        
                        datoValidado.ValidacionTrayectoDestino = ValidarServicioTrayectoDestinoCliente(datoValidacion.MunicipioOrigen, datoValidacion.MunicipioDestino, datoValidacion.TipoServicio, Convert.ToInt32(datoValidacion.IdSucursalClienteOrigen), 0, datoValidacion.IdListaPrecios, datoValidacion.peso, DateTime.Now);

                        if (datoValidado.ValidacionTrayectoDestino.IdCentroServiciosDestino == 0 || datoValidado.ValidacionTrayectoDestino.IdCentroServiciosOrigen == 0)
                            throw new Exception("El id del centro de servicio de destino/origen se encuentra en cero, reintente la validación.");

                        datoValidado.ValidacionTrayectoDestino.NombreCiudadDestino = Framework.Servidor.ParametrosFW.PALocalidad.Instancia.ObtenerLocalidadPorId(datoValidacion.MunicipioDestino.IdLocalidad).NombreCompleto;

                        if (!datoValidacion.EsGuiaInterna)
                        {
                            switch (datoValidacion.TipoServicio.IdServicio)
                            {
                                case (int)EnumTipoServicio.Carga_Express:
                                    datoValidado.PrecioMensajeria = ADFachadaAdmisionesMensajeria.Instancia.CalcularPrecioMensajeriaCredito((int)datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true, datoValidacion.IdTipoEntrega);
                                    break;

                                case (int)EnumTipoServicio.Mensajería:
                                    datoValidado.PrecioMensajeria = ADFachadaAdmisionesMensajeria.Instancia.CalcularPrecioMensajeriaCredito((int)datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true, datoValidacion.IdTipoEntrega);
                                    break;

                                case (int)EnumTipoServicio.Rapi_AM:
                                    datoValidado.PrecioMensajeria = ADFachadaAdmisionesMensajeria.Instancia.CalcularPrecioMensajeriaCredito((int)datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true, datoValidacion.IdTipoEntrega);
                                    break;

                                case (int)EnumTipoServicio.Rapi_Carga:
                                    TAPrecioCargaDC precioCarga = ADFachadaAdmisionesMensajeria.Instancia.CalcularPrecioCargaCredito((int)datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true, datoValidacion.IdTipoEntrega);
                                    datoValidado.PrecioMensajeria = new TAPrecioMensajeriaDC()
                                    {
                                        Impuestos = precioCarga.Impuestos.ToList(),
                                        Valor = precioCarga.Valor,
                                        ValorKiloAdicional = precioCarga.ValorKiloAdicional,
                                        ValorKiloInicial = 0,
                                        ValorPrimaSeguro = precioCarga.ValorPrimaSeguro
                                    };
                                    break;

                                case (int)EnumTipoServicio.Rapi_Hoy:
                                    datoValidado.PrecioMensajeria = ADFachadaAdmisionesMensajeria.Instancia.CalcularPrecioMensajeriaCredito(datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true, datoValidacion.IdTipoEntrega);
                                    break;

                                case (int)EnumTipoServicio.Notificaciones:
                                    datoValidado.PrecioMensajeria = ADFachadaAdmisionesMensajeria.Instancia.CalcularPrecioMensajeriaCredito(datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true, datoValidacion.IdTipoEntrega);
                                    break;

                                case (int)EnumTipoServicio.Rapi_Radicado:
                                    datoValidado.PrecioMensajeria = ADFachadaAdmisionesMensajeria.Instancia.CalcularPrecioMensajeriaCredito(datoValidacion.TipoServicio.IdServicio,datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true, datoValidacion.IdTipoEntrega);
                                    break;

                                case (int)EnumTipoServicio.Rapi_Tula:
                                    datoValidado.PrecioMensajeria = ADFachadaAdmisionesMensajeria.Instancia.CalcularPrecioMensajeriaCredito(datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true);
                                   break;
                            };
                        }
                    }
                    catch (FaultException<ControllerException> exc)
                    {
                        datoValidado.Error = exc.Detail.Mensaje;
                    }
                    catch (Exception exc)
                    {
                        datoValidado.Error = exc.Message;
                    }
                    datosValidados.Add(datoValidado);



                }));
            }


            Task.WaitAll(lstTareas.ToArray());

            datosValidados = datosValidados.OrderBy(r => r.NoFila).ToList();

            return datosValidados;
        }

        /// <summary>
        /// Consulta los datos de las guías asociadas a una orden de servicio cargada masivamente
        /// </summary>
        /// <param name="idOrdenServicio">Número de la orden de servicio</param>
        /// <returns></returns>
        public List<ADGuia> ConsultarGuiasDeOrdenDeServicio(long? idOrdenServicio, int? pageSize, int? pageIndex)
        {
            return ADAdmisionMensajeria.Instancia.ConsultarGuiasDeOrdenDeServicio(idOrdenServicio, pageSize, pageIndex);
        }

        /// <summary>
        /// Consulta la cantidad de guias de admisión asociadas a una orden de servicio
        /// </summary>
        /// <param name="idOrdenServicio"></param>
        /// <returns></returns>
        public long ConsultarCantidadGuiasOrdenSerServicio(long idOrdenServicio)
        {
            return ADAdmisionMensajeria.Instancia.ConsultarCantidadGuiasOrdenSerServicio(idOrdenServicio);
        }

        /// <summary>
        /// Obtiene el listado de
        /// las ordenes de servicio por fecha
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns>lista de ordenes por fecha</returns>
        public List<ADOrdenServicioMasivoDC> ObtenerOrdenesServicioPorFecha(DateTime fechaInicial, DateTime fechaFinal)
        {
            return ADAdmisionMensajeria.Instancia.ObtenerOrdenesServicioPorFecha(fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Se registra un movimiento de mensajería asociado a una orden de servicio. Esto se utiliza en
        /// cuando se hace cargue masivo de guias o facturas.
        /// </summary>
        /// <param name="guia">Datos de la guia</param>
        /// <param name="idCaja">Caja que hace la operacion</param>
        /// <param name="remitenteDestinatario">Datos del remitente y el destinatario</param>
        /// <returns>Número de guía</returns>
        public List<ADRetornoAdmision> RegistrarGuiasDeOrdenDeServicio(List<ADGuiaOSDC> guias, bool conIngresoCentroAcopio)
        {

            List<ADRetornoAdmision> resultadosGrabacion = new List<ADRetornoAdmision>();

            // Se obtiene el número de guía
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            SUNumeradorPrefijo numeroSuministro;
                      
            guias.ForEach((g) =>
            {
                    ADRetornoAdmision retorno = new ADRetornoAdmision();
                    try
                    {
                        //modificacion para las guias que tiene el idcentroserviciodestino en cero
                        if (g.Guia.IdCentroServicioDestino == 0)
                        {
                            PUCentroServiciosDC centroServicioDestino = fachadaCentroServicio.ObtenerAgenciaLocalidad(g.Guia.IdCiudadDestino);
                            g.Guia.IdCentroServicioDestino = centroServicioDestino.IdCentroServicio;
                            g.Guia.NombreCentroServicioDestino = centroServicioDestino.Nombre;
                            g.Guia.NombreCiudadDestino = centroServicioDestino.NombreMunicipio;
                        }

                        if (g.Guia.NumeroGuia <= 0)
                        {
                            if (!g.SonGuiasInternas)
                            {

                                using (TransactionScope tx = new TransactionScope())
                                {
                                    if (g.Guia.IdCliente != 0)
                                    {
                                        numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.GUIA__TRANSPORTE_AUTOMATICA);
                                    }
                                    else
                                    {
                                        numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.FACTURA_VENTA_MENSAJERIA_AUTOMATICA);
                                    }

                                    g.Guia.NumeroGuia = numeroSuministro.ValorActual;
                                    g.Guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;
                                    tx.Complete();
                                }
                            }

                            using (TransactionScope transaccion = new TransactionScope())
                            {
                                if (!g.SonGuiasInternas)
                                {
                                    //ADAdmisionContado.Instancia.RegistrarGuiaAutomatica(g.Guia, g.IdCaja, g.DatosRemitenteDestinatario);
                                    if (g.Guia.IdServicio == TAConstantesServicios.SERVICIO_NOTIFICACIONES)
                                    {

                                        // Se establece que cuando se carga la guía por orden de servicio y es una notificación, entonces se graba como entregar a la dirección del remitente
                                        ADNotificacion infoNotificacion = new ADNotificacion()
                                        {
                                            IdDestinatario = g.Guia.Remitente.Identificacion,
                                            NombreDestinatario = g.Guia.Remitente.Nombre,
                                            TelefonoDestinatario = g.Guia.Remitente.Telefono,
                                            Apellido1Destinatario = g.Guia.Remitente.Apellido1,
                                            Apellido2Destinatario = g.Guia.Remitente.Apellido2,
                                            EmailDestinatario = g.Guia.Remitente.Email,
                                            TipoIdentificacionDestinatario = g.Guia.Remitente.TipoId,
                                            DireccionDestinatario = g.Guia.Remitente.Direccion,
                                            PaisDestino = new PALocalidadDC
                                            {
                                                IdLocalidad = g.Guia.IdPaisOrigen,
                                                Nombre = g.Guia.NombrePaisOrigen
                                            },
                                            CiudadDestino = new PALocalidadDC
                                            {
                                                IdLocalidad = g.Guia.IdCiudadOrigen,
                                                Nombre = g.Guia.NombreCiudadOrigen
                                            },
                                            TipoDestino = new TATipoDestino
                                            {
                                                Descripcion = "ENVIAR A LA DIRECCIÓN DEL REMITENTE",
                                                Id = "EDR"
                                            }
                                        };
                                        RegistrarGuiaAutomaticaNotificacion(g.Guia, g.IdCaja, g.DatosRemitenteDestinatario, infoNotificacion);

                                    }
                                    else if (g.Guia.IdServicio == TAConstantesServicios.SERVICIO_RAPIRADICADO)
                                    {

                                        // todo:id Se adiciona opcion de Guardar RAPIDADICADO para CargaMasiva

                                        ADRapiRadicado infoRapidadicado = new ADRapiRadicado()
                                        {
                                            IdDestinatario = g.Guia.Remitente.Identificacion,
                                            NombreDestinatario = g.Guia.Remitente.Nombre,
                                            TelefonoDestinatario = g.Guia.Remitente.Telefono,
                                            Apellido1Destinatario = g.Guia.Remitente.Apellido1,
                                            Apellido2Destinatario = g.Guia.Remitente.Apellido2,
                                            EmailDestinatario = g.Guia.Remitente.Email,
                                            TipoIdentificacionDestinatario = g.Guia.Remitente.TipoId,
                                            DireccionDestinatario = g.Guia.Remitente.Direccion,
                                            PaisDestino = new PALocalidadDC
                                            {
                                                IdLocalidad = g.Guia.IdPaisOrigen,
                                                Nombre = g.Guia.NombrePaisOrigen
                                            },
                                            CiudadDestino = new PALocalidadDC
                                            {
                                                IdLocalidad = g.Guia.IdCiudadOrigen,
                                                Nombre = g.Guia.NombreCiudadOrigen
                                            },
                                            TipoDestino = new TATipoDestino
                                            {
                                                Descripcion = "ENVIAR A LA DIRECCIÓN DEL REMITENTE",
                                                Id = "EDR"
                                            }
                                        };

                                        RegistrarGuiaAutomaticaRapiRadicado(g.Guia, g.IdCaja, g.DatosRemitenteDestinatario, infoRapidadicado);


                                    }
                                    else
                                    {

                                        RegistrarGuiaAutomatica(g.Guia, g.IdCaja, g.DatosRemitenteDestinatario);

                                    }



                                    ADAdmisionMensajeria.Instancia.GuardarGuiaOrdenServicio(g.Guia.IdAdmision, g.Guia.NumeroGuia, g.NoOrdenServicio);

                                }
                                else
                                {
                                    ADGuiaInternaDC guiaInterna = new ADGuiaInternaDC()
                                    {
                                        CreadoPor = ControllerContext.Current.Usuario,
                                        GuidDeChequeo = g.Guia.GuidDeChequeo,
                                        DiceContener = g.Guia.DiceContener,
                                        DireccionDestinatario = g.Guia.Destinatario.Direccion,
                                        DireccionRemitente = g.Guia.Remitente.Direccion,
                                        EsDestinoGestion = false,
                                        EsManual = false,
                                        EsOrigenGestion = false,
                                        FechaGrabacion = DateTime.Now,
                                        GestionDestino = new Servidor.Servicios.ContratoDatos.Area.ARGestionDC(),
                                        GestionOrigen = new Servidor.Servicios.ContratoDatos.Area.ARGestionDC(),
                                        IdCentroServicioDestino = g.Guia.IdCentroServicioDestino,
                                        IdCentroServicioOrigen = g.Guia.IdCentroServicioOrigen,
                                        LocalidadDestino = new PALocalidadDC()
                                        {
                                            IdLocalidad = g.Guia.IdCiudadDestino,
                                            Nombre = g.Guia.NombreCiudadDestino,
                                            NombreCompleto = g.Guia.NombreCiudadDestino
                                        },
                                        LocalidadOrigen = new PALocalidadDC()
                                        {
                                            IdLocalidad = g.Guia.IdCiudadOrigen,
                                            Nombre = g.Guia.NombreCiudadOrigen,
                                            NombreCompleto = g.Guia.NombreCiudadOrigen
                                        },
                                        NombreCentroServicioDestino = g.Guia.NombreCentroServicioDestino,
                                        NombreCentroServicioOrigen = g.Guia.NombreCentroServicioDestino,
                                        NombreDestinatario = g.Guia.Destinatario.NombreYApellidos,
                                        NombreRemitente = g.Guia.Remitente.NombreYApellidos,
                                        PaisDefault = new PALocalidadDC()
                                        {
                                            IdLocalidad = "057",
                                            Nombre = "COLOMBIA",
                                            NombreCompleto = "COLOMBIA"
                                        },
                                        TelefonoDestinatario = g.Guia.Destinatario.Telefono,
                                        TelefonoRemitente = g.Guia.Remitente.Telefono,
                                        IdentificacionDestinatario = g.Guia.Destinatario.Identificacion,
                                        TipoIdentificacionDestinatario = g.Guia.Destinatario.TipoId,
                                        IdentificacionRemitente = g.Guia.Remitente.Identificacion,
                                        TipoIdentificacionRemitente = g.Guia.Remitente.TipoId
                                    };

                                    ADAdmisionGuiaInterna.Instancia.AdicionarGuiaInterna(guiaInterna);
                                    g.Guia.NumeroGuia = guiaInterna.NumeroGuia;
                                    ADAdmisionMensajeria.Instancia.GuardarGuiaOrdenServicio(guiaInterna.IdAdmisionGuia, guiaInterna.NumeroGuia, g.NoOrdenServicio);
                                }

                                transaccion.Complete();
                            }
                        }
                        else
                        {

                            using (TransactionScope transaccion = new TransactionScope())
                            {
                                if (conIngresoCentroAcopio)
                                {
                                    if (g.Guia.IdCliente != 0)
                                    {
                                        if (g.Guia.IdServicio != TAConstantesServicios.SERVICIO_NOTIFICACIONES)
                                        {
                                            ADAdmisionCredito.Instancia.RegistrarGuiaManual(g.Guia, g.IdCaja, g.DatosRemitenteDestinatario, false);
                                        }
                                        else
                                        {
                                            // Se establece que cuando se carga la guía por orden de servicio y es una notificación, entonces se graba como entregar a la dirección del remitente
                                            ADNotificacion infoNotificacion = new ADNotificacion()
                                            {
                                                IdDestinatario = g.Guia.Remitente.Identificacion,
                                                NombreDestinatario = g.Guia.Remitente.Nombre,
                                                TelefonoDestinatario = g.Guia.Remitente.Telefono,
                                                Apellido1Destinatario = g.Guia.Remitente.Apellido1,
                                                Apellido2Destinatario = g.Guia.Remitente.Apellido2,
                                                EmailDestinatario = g.Guia.Remitente.Email,
                                                TipoIdentificacionDestinatario = g.Guia.Remitente.TipoId,
                                                DireccionDestinatario = g.Guia.Remitente.Direccion,
                                                PaisDestino = new PALocalidadDC
                                                {
                                                    IdLocalidad = g.Guia.IdPaisOrigen,
                                                    Nombre = g.Guia.NombrePaisOrigen
                                                },
                                                CiudadDestino = new PALocalidadDC
                                                {
                                                    IdLocalidad = g.Guia.IdCiudadOrigen,
                                                    Nombre = g.Guia.NombreCiudadOrigen
                                                },
                                                TipoDestino = new TATipoDestino
                                                {
                                                    Descripcion = "ENVIAR A LA DIRECCIÓN DEL REMITENTE",
                                                    Id = "EDR"
                                                }
                                            };
                                            ADAdmisionCredito.Instancia.RegistrarGuiaManualNotificacion(g.Guia, g.IdCaja, g.DatosRemitenteDestinatario, infoNotificacion, false);
                                        }
                                    }
                                    else
                                    {
                                        if (g.Guia.IdServicio != TAConstantesServicios.SERVICIO_NOTIFICACIONES)
                                        {
                                            ADAdmisionContado.Instancia.RegistrarGuiaManual(g.Guia, g.IdCaja, g.DatosRemitenteDestinatario);
                                        }
                                        else
                                        {
                                            // Se establece que cuando se carga la guía por orden de servicio y es una notificación, entonces se graba como entregar a la dirección del remitente
                                            ADNotificacion infoNotificacion = new ADNotificacion()
                                            {
                                                IdDestinatario = g.Guia.Remitente.Identificacion,
                                                NombreDestinatario = g.Guia.Remitente.Nombre,
                                                TelefonoDestinatario = g.Guia.Remitente.Telefono,
                                                Apellido1Destinatario = g.Guia.Remitente.Apellido1,
                                                Apellido2Destinatario = g.Guia.Remitente.Apellido2,
                                                EmailDestinatario = g.Guia.Remitente.Email,
                                                TipoIdentificacionDestinatario = g.Guia.Remitente.TipoId,
                                                DireccionDestinatario = g.Guia.Remitente.Direccion,
                                                PaisDestino = new PALocalidadDC
                                                {
                                                    IdLocalidad = g.Guia.IdPaisOrigen,
                                                    Nombre = g.Guia.NombrePaisOrigen
                                                },
                                                CiudadDestino = new PALocalidadDC
                                                {
                                                    IdLocalidad = g.Guia.IdCiudadOrigen,
                                                    Nombre = g.Guia.NombreCiudadOrigen
                                                },
                                                TipoDestino = new TATipoDestino
                                                {
                                                    Descripcion = "ENVIAR A LA DIRECCIÓN DEL REMITENTE",
                                                    Id = "EDR"
                                                }
                                            };
                                            ADAdmisionContado.Instancia.RegistrarGuiaManualNotificacion(g.Guia, g.IdCaja, g.DatosRemitenteDestinatario, infoNotificacion);
                                        }
                                    }


                                    ADTrazaGuia trazaGuia = new ADTrazaGuia
                                    {
                                        NumeroGuia = g.Guia.NumeroGuia,
                                        IdAdmision = g.Guia.IdAdmision,
                                        Observaciones = "CARGA MASIVA MENSAJERIA",
                                        IdCiudad = g.Guia.IdCiudadOrigen,
                                        Ciudad = g.Guia.NombreCiudadOrigen,
                                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio,
                                        Modulo = "MEN"
                                        //IdEstadoGuia = (short)EstadosGuia.ObtenerUltimoEstadoxNumero(g.Guia.NumeroGuia)
                                    };
                                    EstadosGuia.InsertaEstadoGuia(trazaGuia);


                                    // Lanzar mensaje de texto de cliente crédito si es una guía crédito y el destino requiere lanzamiento de mensaje
                                    // TODO: RON Agregar activación y / o desactivación de integración
                                    if (g.Guia.TipoCliente == ADEnumTipoCliente.CPE)
                                    {
                                        new System.Threading.Tasks.Task(() => CO.Controller.Servidor.Integraciones.MensajesTexto.Instancia.EnviarMensajeTexto(g.Guia.NumeroGuia)).Start();
                                    }
                                }
                                else
                                    ADAdmisionContado.Instancia.RegistrarGuiaManual(g.Guia, g.IdCaja, g.DatosRemitenteDestinatario);

                                ADAdmisionMensajeria.Instancia.GuardarGuiaOrdenServicio(g.Guia.IdAdmision, g.Guia.NumeroGuia, g.NoOrdenServicio);
                                transaccion.Complete();
                            }
                        }

                        retorno.Error = "";
                        retorno.FechaGrabacion = DateTime.Now;
                        retorno.NumeroGuia = g.Guia.NumeroGuia;
                        retorno.PrefijoGuia = g.Guia.PrefijoNumeroGuia;
                        retorno.NoFila = g.NoFila;
                    }
                    catch (FaultException<ControllerException> exc)
                    {
                        retorno.Error = exc.Detail.Mensaje;
                        retorno.NoFila = g.NoFila;
                        retorno.FechaGrabacion = DateTime.Now;
                    }
                    catch (Exception exc)
                    {
                        retorno.Error = exc.Message;
                        retorno.NoFila = g.NoFila;
                        retorno.FechaGrabacion = DateTime.Now;
                    }
                    resultadosGrabacion.Add(retorno);
            }
            );
           
            return resultadosGrabacion;
        }      

        /// <summary>
        /// Método para obtener la información de una guía rapiradicado
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADRapiRadicado ObtenerAdmisionRapiradicado(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerAdmisionRapiradicado(numeroGuia);
        }

        /// <summary>
        /// Audita todas las admisiones automaticas generadas
        /// </summary>
        public void GuardarAuditoriaGrabacionAdmisionMensajeria(int idCaja, string metodoEjecutado, ADRetornoAdmision retorno, ADGuia guiaEntrada, ADMensajeriaTipoCliente tipoCliente, object objetoAdicional = null)
        {
            ADAdmisionContado.Instancia.GuardarAuditoriaGrabacionAdmisionMensajeria(idCaja, metodoEjecutado, retorno, guiaEntrada, tipoCliente, objetoAdicional);
        }



        public string ObtenerParametrosEncabezado(string llave)
        {
            return ADConsultas.Instancia.ObtenerParametrosEncabezado(llave);
        }
    }
}