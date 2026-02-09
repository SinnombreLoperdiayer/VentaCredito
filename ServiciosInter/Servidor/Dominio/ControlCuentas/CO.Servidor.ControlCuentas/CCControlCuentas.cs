using CO.Servidor.ControlCuentas.Comun;
using CO.Servidor.ControlCuentas.Datos;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Facturacion;
using CO.Servidor.Dominio.Comun.LogisticaInversa;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Integraciones.Novasoft.Proxies;
using Integraciones.Novasoft.Proxies.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;

namespace CO.Servidor.ControlCuentas
{
    public partial class CCControlCuentas : ControllerBase
    {
        #region CrearInstancia

        private static readonly CCControlCuentas instancia = (CCControlCuentas)FabricaInterceptores.GetProxy(new CCControlCuentas(), COConstantesModulos.MODULO_CONTROL_CUENTAS);

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static CCControlCuentas Instancia
        {
            get { return CCControlCuentas.instancia; }
        }

        private IADFachadaAdmisionesMensajeria fachadaAdmisiones = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private IFAFachadaFacturacion fachadaFacturacion = COFabricaDominio.Instancia.CrearInstancia<IFAFachadaFacturacion>();

        private ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

        private ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();

        #endregion CrearInstancia

        /// <summary>
        /// Obtiene el ultimo estado y ubicacin de la admision mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        internal ADGuiaUltEstadoDC ObtenerMensajeriaUltimoEstado(long idNumeroGuia)
        {
            ADGuiaUltEstadoDC guiaUltEstado = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerMensajeriaUltimoEstado(idNumeroGuia);

            if (COFabricaDominio.Instancia.CrearInstancia<IFAFachadaFacturacion>().GuiaYaFacturada(idNumeroGuia))
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS, "0", "Número de guía de transporte ya facturada."));

            if (guiaUltEstado != null && guiaUltEstado.Guia != null)
            {
                guiaUltEstado.FormasPago = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerFormasPagoGuia(guiaUltEstado.Guia.IdAdmision);
                guiaUltEstado.Guia.FormasPago = guiaUltEstado.FormasPago;
                int conceptoCaja = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>().ObtenerConceptoCaja(guiaUltEstado.Guia.IdServicio);
                guiaUltEstado.Guia.IdConceptoCaja = conceptoCaja;
                guiaUltEstado.IdRacol = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerRacolResponsable(guiaUltEstado.Guia.IdCentroServicioOrigen).IdResponsable;
            }
            return guiaUltEstado;
        }

        /// <summary>
        /// Obtener informacion de la guia de mensajeria y las formas de pago
        /// </summary>
        /// <returns></returns>
        internal ADGuiaUltEstadoDC ObtenerMensajeriaFormaPago(long idAdmision)
        {
            return fachadaAdmisiones.ObtenerMensajeriaFormaPago(idAdmision);
        }

        /// <summary>
        /// Calcula el precio de un guia para cambio de destino
        /// </summary>
        internal decimal ReLiquidacion(ADGuia guia)
        {
            CCReliquidacionDC valorReliquidado = Reliquidacion(guia, guia.IdCiudadOrigen, guia.IdCiudadDestino);
            if (valorReliquidado == null)
                return 0;
            return valorReliquidado.ValorTotal;
        }

        /// <summary>
        /// Calcula el precio de un guia para cambio de destino
        /// </summary>
        internal CCBolsaNovedadesReliquidacionDC ReLiquidacionBolsaNovedades(ADGuia guia)
        {
            CCReliquidacionDC valorReliquidado = Reliquidacion(guia, guia.IdCiudadOrigen, guia.IdCiudadDestino);
            if (valorReliquidado == null)
            {
                return new CCBolsaNovedadesReliquidacionDC();
            }
            else
            {
                CCBolsaNovedadesReliquidacionDC valor = new CCBolsaNovedadesReliquidacionDC()
                {
                    ValorImpuestos = valorReliquidado.ValorImpuestos,
                    ValorPrimaSeguro = valorReliquidado.ValorPrimaSeguro,
                    ValorTransporte = valorReliquidado.ValorTransporte,
                    ValorTotal = valorReliquidado.ValorTransporte + valorReliquidado.ValorPrimaSeguro
                };
                return valor;
            }
        }

        /// <summary>
        /// Recalcula la prima de un factura
        /// </summary>
        internal decimal ReLiquidacionPrima(ADGuia guia)
        {
            CCReliquidacionDC valorReliquidado = Reliquidacion(guia, guia.IdCiudadOrigen, guia.IdCiudadDestino);
            if (valorReliquidado == null)
                return 0;
            return valorReliquidado.ValorPrimaSeguro;
        }

        /// <summary>
        /// Metodo que calcula el precio de una guía con un origen y un destino especifico que puede ser diferente a los originales
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCiudadOrigen"></param>
        /// <param name="idCiudadDestino"></param>
        /// <returns></returns>
        private CCReliquidacionDC Reliquidacion(ADGuia guia, string idCiudadOrigen, string idCiudadDestino)
        {
            CCReliquidacionDC precio = null;
            decimal peso = 0;
            int idListaPrecios;
            int idContrato;

            // Buscar la lista de precios segun el tipo de cliente
            if (guia.TipoCliente == ADEnumTipoCliente.CCO || guia.TipoCliente == ADEnumTipoCliente.CPE)
            {
                idContrato = fachadaAdmisiones.ObtenerContratoClienteConvenio(guia.TipoCliente, guia.IdAdmision);
                idListaPrecios = fachadaClientes.ObtenerListaPrecioContrato(idContrato);
            }
            else
            {
                idListaPrecios = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>().ObtenerIdListaPrecioVigente();
            }

            peso = guia.Peso;

            switch (guia.IdServicio)
            {
                case TAConstantesServicios.SERVICIO_CENTRO_CORRESPONDENCIA:

                    // NO ESTÁ DEFINIDO CLARAMENTE ESTE TIPO DE SERVICIO
                    TAPrecioCentroCorrespondenciaDC precioCorrepondencia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioCentroCorrespondencia(idListaPrecios);

                    break;

                case TAConstantesServicios.SERVICIO_MENSAJERIA:
                    TAPrecioMensajeriaDC precioMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioMensajeria(guia.IdServicio, idListaPrecios, idCiudadOrigen, idCiudadDestino, peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioMensajeria.Valor,
                        ValorPrimaSeguro = precioMensajeria.ValorPrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                    TAPrecioMensajeriaDC precioCargaExpress = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioMensajeria(guia.IdServicio, idListaPrecios, idCiudadOrigen, idCiudadDestino, peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioCargaExpress.Valor,
                        ValorPrimaSeguro = precioCargaExpress.ValorPrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_NOTIFICACIONES:
                    TAPrecioMensajeriaDC precioNotificaciones = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioNotificaciones(guia.IdServicio, idListaPrecios, idCiudadOrigen, idCiudadDestino, peso, guia.ValorDeclarado, true);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioNotificaciones.Valor,
                        ValorPrimaSeguro = precioNotificaciones.ValorPrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_RAPI_AM:
                    var precioRapiAM = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioRapiAm(guia.IdServicio, idListaPrecios, idCiudadOrigen, idCiudadDestino, peso, guia.ValorDeclarado);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioRapiAM.Valor,
                        ValorPrimaSeguro = precioRapiAM.ValorPrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_RAPI_CARGA:
                    TAPrecioCargaDC precioCarga = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioRapiCarga(guia.IdServicio, idListaPrecios, idCiudadOrigen, idCiudadDestino, peso, guia.ValorDeclarado);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioCarga.Valor,
                        ValorPrimaSeguro = precioCarga.ValorPrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_RAPI_CARGA_CONTRA_PAGO:
                    TAPrecioCargaDC preciocargaContraPago = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioRapiCargaContraPago(guia.IdServicio, idListaPrecios, idCiudadOrigen, idCiudadDestino, peso, guia.ValorTotal, guia.ValorDeclarado);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = preciocargaContraPago.Valor,
                        ValorPrimaSeguro = preciocargaContraPago.ValorPrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO:
                    TAPrecioMensajeriaDC precioEnvio = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioRapiEnvioContraPago(guia.IdServicio, idListaPrecios, idCiudadOrigen, idCiudadDestino, peso, guia.ValorTotal, guia.ValorDeclarado);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioEnvio.Valor,
                        ValorPrimaSeguro = precioEnvio.ValorPrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_RAPI_HOY:
                    TAPrecioMensajeriaDC precioRapaHoy = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioRapiHoy(guia.IdServicio, idListaPrecios, idCiudadOrigen, idCiudadDestino, peso, guia.ValorDeclarado);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioRapaHoy.Valor,
                        ValorPrimaSeguro = precioRapaHoy.ValorPrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_RAPI_MASIVOS:
                    throw new NotImplementedException();
                case TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO:
                    TAPrecioMensajeriaDC precioRapiPersonalizado = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioRapiPersonalizado(guia.IdServicio, idListaPrecios, idCiudadOrigen, idCiudadDestino, peso, guia.ValorDeclarado);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioRapiPersonalizado.Valor,
                        ValorPrimaSeguro = precioRapiPersonalizado.ValorPrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_RAPI_PROMOCIONAL:
                    TAPrecioServicioDC precioPromocional = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioRapiPromocional(idListaPrecios, 0);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioPromocional.Valor,
                        ValorPrimaSeguro = precioPromocional.PrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_RAPIRADICADO:
                    TAPrecioMensajeriaDC precioRapiRadicado = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioMensajeria(guia.IdServicio, idListaPrecios, idCiudadOrigen, idCiudadDestino, peso, guia.ValorDeclarado);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioRapiRadicado.Valor,
                        ValorPrimaSeguro = precioRapiRadicado.ValorPrimaSeguro
                    };
                    break;

                case TAConstantesServicios.SERVICIO_TRAMITES:
                    TAPrecioTramiteDC precioTramite = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().CalcularPrecioTramites(idListaPrecios, 0);
                    precio = new CCReliquidacionDC()
                    {
                        ValorTransporte = precioTramite.Valor,
                        ValorPrimaSeguro = 0
                    };
                    break;
            }

            return precio;
        }

        /// <summary>
        /// Obtener el empleado en NovaSoft
        /// </summary>
        /// <param name="identificacion"></param>
        internal CCEmpleadoNovaSoftDC ObtenerEmpleadoNovaSoft(string identificacion)
        {
            INEmpleado empleadoNova;
            IIntegracionNovasoft integracion = new ImpIntegracionNovasoft();

            empleadoNova = integracion.ObtenerEmpleado(identificacion);

            if (empleadoNova == null)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS, EnumTipoErrorControlCuentas.EX_PERSONA_NO_EXISTE.ToString(), MensajesControlCuentas.CargarMensaje(EnumTipoErrorControlCuentas.EX_PERSONA_NO_EXISTE)));
            }

            return new CCEmpleadoNovaSoftDC()
            {
                Cargo = empleadoNova.Cargo,
                Nombre = empleadoNova.Nombre,
                PrimerApellido = empleadoNova.PrimerApellido,
                SegundoApellido = empleadoNova.SegundoApellido,
                Identificacion = empleadoNova.Identificacion
            };
        }

        #region Cambios Guia (Destino-Peso-FormaPago-Anulacion-Valor)

        /// <summary>
        /// Crear novedad cambio de destino
        /// </summary>
        /// <param name="novedadGuia"></param>
        internal void CrearNovedadCambioDestino(CCNovedadCambioDestinoDC novedadGuia)
        {
            novedadGuia.TipoNovedad = CCEnumTipoNovedadGuia.ModificarDestinoGuia;
            TAServicioDC servicio = new TAServicioDC() { IdServicio = novedadGuia.Guia.IdServicio };
            PALocalidadDC localidadorigen = new PALocalidadDC() { IdLocalidad = novedadGuia.Guia.IdCiudadOrigen };
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

            List<TAFormaPago> formPagoAfectacion = null;

            novedadGuia.IdModulo = COConstantesModulos.MODULO_CONTROL_CUENTAS;

            // Validacion del nuevo trayecto
            ADValidacionServicioTrayectoDestino validacionTrayecto = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ValidarServicioTrayectoDestino(localidadorigen, novedadGuia.NuevaLocalidadDestino, servicio, 0, novedadGuia.Guia.Peso);

            //Datos del pais de la localidad destino para poder hacer el cambio logístico
            PALocalidadDC pais = Framework.Servidor.Seguridad.SEAdministradorSeguridad.Instancia.ObtenerPaisPorLocalidad(novedadGuia.NuevaLocalidadDestino.IdLocalidad);
            PUCentroServiciosDC centroServicioDestino = new PUCentroServiciosDC();
            centroServicioDestino.IdCentroServicio = validacionTrayecto.IdCentroServiciosDestino;
            centroServicioDestino.Nombre = validacionTrayecto.NombreCentroServiciosDestino;

            centroServicioDestino.IdPais = pais.IdLocalidad;
            centroServicioDestino.NombrePais = pais.Nombre;
            centroServicioDestino.IdMunicipio = novedadGuia.NuevaLocalidadDestino.IdLocalidad;
            centroServicioDestino.NombreMunicipio = novedadGuia.NuevaLocalidadDestino.Nombre;
            centroServicioDestino.CodigoPostal = validacionTrayecto.CodigoPostalDestino;

            // Afectar caja de acuerdo a la forma de pago
            ADGuiaFormaPago formaPago = novedadGuia.Guia.FormasPago.FirstOrDefault();
            Dictionary<CCEnumNovedadRealizada, string> datosAdicionales = new Dictionary<CCEnumNovedadRealizada, string>();

            using (TransactionScope transaccion = new TransactionScope())
            {
                if (formaPago != null)
                {
                    bool esIngreso = true;
                    CAConceptoCajaDC concepto = null;
                    CCReliquidacionDC valorTrayectoNuevo = null;
                    PUCentroServiciosDC centroServicioAfectado = null;

                    concepto = new CAConceptoCajaDC()
                    {
                        IdConceptoCaja = (int)CAEnumConceptosCaja.AJUSTE_CAMBIO_DESTINO,
                        Nombre = CCConstantesControlCuentas.CONCEPTO_AJUSTE_X_CAMBIO_DESTINO,
                        Descripcion = "Ajuste por Cambio de destino a " + novedadGuia.NuevaLocalidadDestino.NombreCompleto
                    };

                    decimal valorRecalculado = 0;
                    decimal valorAdicionales = 0;
                    formPagoAfectacion = new List<TAFormaPago>()
                            {
                                new TAFormaPago()
                                {
                                    IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                                    Descripcion = "Contado",
                                    Valor = valorRecalculado
                                }
                            };

                    #region Forma de Pago Contado

                    bool actualizarValor = false;

                    if (formaPago.IdFormaPago.Equals((short)TAEnumFormaPago.EFECTIVO))
                    {
                        valorTrayectoNuevo = Reliquidacion(novedadGuia.Guia, novedadGuia.Guia.IdCiudadOrigen, novedadGuia.NuevaLocalidadDestino.IdLocalidad);
                        valorTrayectoNuevo.ValorImpuestos = novedadGuia.Guia.ValorTotalImpuestos;

                        if (!novedadGuia.ResponsableNovedad.Responsable.Equals(CCTipoResponsableDC.FUNCIONARIO))
                        {
                            centroServicioAfectado = new PUCentroServiciosDC()
                            {
                                IdCentroServicio = novedadGuia.Guia.IdCentroServicioOrigen,
                                Nombre = novedadGuia.Guia.NombreCentroServicioOrigen
                            };

                            IPUFachadaCentroServicios fachadaCS = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
                            PUCentroServiciosDC CSColOrigenGuia = fachadaCS.ObtieneCOLResponsableAgenciaLocalidad(novedadGuia.Guia.IdCiudadOrigen);
                            PUCentroServiciosDC CSColUbicacionActualGuia = fachadaCS.ObtieneCOLResponsableAgenciaLocalidad(novedadGuia.LocalidadActual.IdLocalidad);

                            //Si el envío continúa en el mismo col donde fué admitido, es decir aún no ha sido manifestado
                            if (CSColOrigenGuia.IdCentroServicio == CSColUbicacionActualGuia.IdCentroServicio)
                            {
                                //Reliquido desde la ciudad origen hacia el nuevo destino
                                valorTrayectoNuevo = Reliquidacion(novedadGuia.Guia, novedadGuia.Guia.IdCiudadOrigen, novedadGuia.NuevaLocalidadDestino.IdLocalidad);
                                valorTrayectoNuevo.ValorImpuestos = novedadGuia.Guia.ValorTotalImpuestos;

                                //El valor recalculado es mayor que el valor registrado en el sistema
                                if (valorTrayectoNuevo.ValorTotal > novedadGuia.Guia.ValorTotal)
                                {
                                    concepto.EsIngreso = true;
                                    concepto.EsEgreso = false;
                                    valorRecalculado = valorTrayectoNuevo.ValorTotal - novedadGuia.Guia.ValorTotal;
                                }
                                else if (valorTrayectoNuevo.ValorTotal < novedadGuia.Guia.ValorTotal)
                                {
                                    concepto.EsIngreso = false;
                                    concepto.EsEgreso = true;
                                    valorRecalculado = novedadGuia.Guia.ValorTotal - valorTrayectoNuevo.ValorTotal;
                                }

                                if (valorRecalculado > 0)
                                {
                                    formPagoAfectacion.FirstOrDefault().Valor = valorRecalculado;
                                    concepto.Descripcion = "Ajuste por Cambio de destino a " + novedadGuia.NuevaLocalidadDestino.NombreCompleto;
                                    AfectarCaja(novedadGuia, concepto.EsIngreso, concepto, valorRecalculado, centroServicioAfectado, formPagoAfectacion, valorAdicionales);
                                }
                            }
                            //Si ya fué manifestado, es decir ya fué despachado para la ciudad destino
                            else
                            {
                                valorTrayectoNuevo = Reliquidacion(novedadGuia.Guia, novedadGuia.Guia.IdCiudadOrigen, novedadGuia.NuevaLocalidadDestino.IdLocalidad);
                                valorTrayectoNuevo.ValorImpuestos = novedadGuia.Guia.ValorTotalImpuestos;

                                //El valor recalculado es mayor que el valor registrado en el sistema
                                if (valorTrayectoNuevo.ValorTotal > novedadGuia.Guia.ValorTotal)
                                {
                                    concepto.EsIngreso = true;
                                    concepto.EsEgreso = false;
                                    valorRecalculado = valorTrayectoNuevo.ValorTotal - novedadGuia.Guia.ValorTotal;
                                }

                                //Hacer movimiento en caja
                                if (valorRecalculado > 0)
                                {
                                    formPagoAfectacion.FirstOrDefault().Valor = valorRecalculado;
                                    concepto.Descripcion = "Ajuste por Cambio de destino a " + novedadGuia.NuevaLocalidadDestino.NombreCompleto;
                                    AfectarCaja(novedadGuia, concepto.EsIngreso, concepto, valorRecalculado, centroServicioAfectado, formPagoAfectacion, valorAdicionales);
                                }

                                valorRecalculado = 0;
                                //Valor trayecto desde la ciudad actual hacia la ciudad destino real
                                valorTrayectoNuevo = Reliquidacion(novedadGuia.Guia, novedadGuia.LocalidadActual.IdLocalidad, novedadGuia.NuevaLocalidadDestino.IdLocalidad);
                                valorTrayectoNuevo.ValorImpuestos = novedadGuia.Guia.ValorTotalImpuestos;

                                decimal valorSuma = valorTrayectoNuevo.ValorTotal + novedadGuia.Guia.ValorTotal;

                                //Valor trayecto desde la ciudad origen hacia la ciudad destino real
                                //CCReliquidacionDC valorTrayectoReal = Reliquidacion(novedadGuia.Guia, novedadGuia.Guia.IdCiudadOrigen, novedadGuia.NuevaLocalidadDestino.IdLocalidad);

                                //El valor recalculado es mayor que el valor registrado en el sistema
                                if (valorSuma > novedadGuia.Guia.ValorTotal)
                                {
                                    concepto.EsIngreso = true;
                                    concepto.EsEgreso = false;
                                    valorRecalculado = (valorSuma - novedadGuia.Guia.ValorTotal);
                                }
                                else
                                {
                                    concepto.EsIngreso = false;
                                    concepto.EsEgreso = true;
                                    valorRecalculado = novedadGuia.Guia.ValorTotal - valorSuma;
                                }

                                if (valorRecalculado > 0)
                                {
                                    formPagoAfectacion.FirstOrDefault().Valor = valorRecalculado;
                                    concepto.Descripcion = "Ajuste por Cambio de destino a " + novedadGuia.NuevaLocalidadDestino.NombreCompleto + ". REDESPACHO";
                                    AfectarCaja(novedadGuia, concepto.EsIngreso, concepto, valorRecalculado, centroServicioAfectado, formPagoAfectacion, valorAdicionales);
                                }
                            }
                        }
                    }

                    #endregion Forma de Pago Contado

                    #region Forma Pago AlCobro

                    else if (formaPago.IdFormaPago.Equals((short)TAEnumFormaPago.AL_COBRO))
                    {
                        valorTrayectoNuevo = Reliquidacion(novedadGuia.Guia, novedadGuia.Guia.IdCiudadOrigen, novedadGuia.NuevaLocalidadDestino.IdLocalidad);
                        valorTrayectoNuevo.ValorImpuestos = novedadGuia.Guia.ValorTotalImpuestos;

                        //Buscar las planillas donde esta y si no está descargada de esa planilla se debe afectar la caja del mensajero con egreso de esa guia
                        List<OUGuiaIngresadaDC> planillasGuia = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().ObtenerPlanillasGuia(novedadGuia.Guia.IdAdmision);

                        planillasGuia.ForEach(p =>
                        {
                            if (p.IdMensajero > 0 && p.EstadoGuiaPlanilla != "DEV" && !p.EstaPagada)
                            {
                                fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC
                                {
                                    ConceptoEsIngreso = esIngreso,
                                    ConceptoCajaMensajero = concepto,
                                    FechaGrabacion = DateTime.Now,
                                    Mensajero = new OUNombresMensajeroDC
                                    {
                                        IdPersonaInterna = p.IdMensajero,
                                        NombreApellido = p.NombreCompleto
                                    },
                                    Valor = valorRecalculado,
                                    NumeroDocumento = novedadGuia.Guia.NumeroGuia,
                                    Observaciones = concepto.Descripcion,
                                    UsuarioRegistro = ControllerContext.Current.Usuario,
                                });
                            }
                        });

                        //Si esta pagada no fue asignada a ningun mensajero hace las afectaciones correspondientes en las cajas de la agencia destino original y destino nueva
                        if (novedadGuia.Guia.EstaPagada)
                        {
                            //validar que el destino reciba al cobro cuando la guia sea al cobro
                            if (!validacionTrayecto.DestinoAdmiteFormaPagoAlCobro)
                            {
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS, EnumTipoErrorControlCuentas.EX_PUNTO_NO_ADMITE_FORMA_PAGO_ALCOBRO.ToString(), MensajesControlCuentas.CargarMensaje(EnumTipoErrorControlCuentas.EX_PUNTO_NO_ADMITE_FORMA_PAGO_ALCOBRO)));
                            }
                            bool hacerAfectacion = false;
                            //si la guia no esta planillada a un mensajero
                            if (planillasGuia.Count == 0)
                            {
                                decimal valorCargadoCaja = 0;
                                PUCentroServiciosDC centroServicioPago = fachadaCajas.ConsultarCentroDeServiciosPagoAlCobro(novedadGuia.Guia.NumeroGuia, out valorCargadoCaja);
                                //Egreso a la agencia que recibió el pago del al cobro por el valor inicial de la factura
                                if (centroServicioPago != null)
                                {
                                    esIngreso = false;
                                    centroServicioAfectado = new PUCentroServiciosDC()
                                    {
                                        IdCentroServicio = centroServicioPago.IdCentroServicio,
                                        Nombre = centroServicioPago.Nombre
                                    };
                                    hacerAfectacion = true;
                                }
                            }
                            //  si la guia esta planillada a un mensajero y ya esta pagada (descargada)
                            else if (planillasGuia.FirstOrDefault().EstaPagada)
                            {
                                //Egreso a la agencia que descargó el al cobro
                                esIngreso = false;
                                centroServicioAfectado = new PUCentroServiciosDC()
                                {
                                    IdCentroServicio = planillasGuia.FirstOrDefault().IdCentroLogistico,
                                    Nombre = planillasGuia.FirstOrDefault().NombreCentroLogistico
                                };
                                hacerAfectacion = true;
                            }

                            if (hacerAfectacion)
                            {
                                //Lleno la data para hacer el egreso Correspondiente de la Caja
                                valorTrayectoNuevo = new CCReliquidacionDC()
                                {
                                    ValorTransporte = novedadGuia.Guia.ValorServicio,
                                    ValorPrimaSeguro = novedadGuia.Guia.ValorPrimaSeguro,
                                    ValorImpuestos = novedadGuia.Guia.ValorTotalImpuestos,
                                };
                                formPagoAfectacion = new List<TAFormaPago>()
                              {
                                  new TAFormaPago()
                                  {
                                    IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                                    Descripcion = "Contado",
                                    Valor = valorTrayectoNuevo.ValorTotal
                                  }
                              };
                                AfectarCaja(novedadGuia, esIngreso, concepto, valorTrayectoNuevo, centroServicioAfectado, formPagoAfectacion);
                            }

                            //Ingreso a la caja de la nueva agencia destino por el valor inicial de la factura
                            esIngreso = true;
                            centroServicioAfectado = new PUCentroServiciosDC()
                            {
                                IdCentroServicio = validacionTrayecto.IdCentroServiciosDestino,
                                Nombre = validacionTrayecto.NombreCentroServiciosDestino
                            };

                            //Reliquido el Nuevo destino
                            valorTrayectoNuevo = Reliquidacion(novedadGuia.Guia, novedadGuia.LocalidadActual.IdLocalidad, novedadGuia.NuevaLocalidadDestino.IdLocalidad);
                            valorTrayectoNuevo.ValorImpuestos = novedadGuia.Guia.ValorTotalImpuestos;

                            formPagoAfectacion = new List<TAFormaPago>()
                            {
                                new TAFormaPago()
                                {
                                    IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                                    Descripcion = "Contado",
                                    Valor = valorTrayectoNuevo.ValorTotal
                                }
                            };
                            AfectarCaja(novedadGuia, esIngreso, concepto, valorTrayectoNuevo, centroServicioAfectado, formPagoAfectacion);
                        }
                    }

                    #endregion Forma Pago AlCobro

                    else
                    {
                        valorTrayectoNuevo = Reliquidacion(novedadGuia.Guia, novedadGuia.Guia.IdCiudadOrigen, novedadGuia.NuevaLocalidadDestino.IdLocalidad);
                        valorTrayectoNuevo.ValorImpuestos = novedadGuia.Guia.ValorTotalImpuestos;
                        actualizarValor = true;
                    }
                    //actualizarValor = true;
                    #region Actualizacion de la Guia y Novedad

                    if (valorTrayectoNuevo != null)
                    {
                        //AFECTAR LOGÍSTICAMENTE
                        fachadaAdmisiones.ActualizarDestinoGuia(novedadGuia.Guia.IdAdmision, centroServicioDestino, valorTrayectoNuevo, (TAEnumFormaPago)formaPago.IdFormaPago, novedadGuia.Guia.IdTipoEntrega, novedadGuia.Guia.DescripcionTipoEntrega);

                        valorTrayectoNuevo = Reliquidacion(novedadGuia.Guia, novedadGuia.Guia.IdCiudadOrigen, novedadGuia.NuevaLocalidadDestino.IdLocalidad);
                        valorTrayectoNuevo.ValorImpuestos = novedadGuia.Guia.ValorTotalImpuestos;

                        // Aplicar logísticamente el cambio en tbl AdminMensajeria
                        CCNovedadCambioValorTotal novedadCambioValor = new CCNovedadCambioValorTotal()
                        {
                            NuevoValorTransporte = valorTrayectoNuevo.ValorTransporte,
                            NuevoValorPrima = valorTrayectoNuevo.ValorPrimaSeguro,
                            NuevoValorComercial = novedadGuia.Guia.ValorDeclarado,
                            Guia = novedadGuia.Guia,
                        };

                        if (actualizarValor)
                            fachadaAdmisiones.ActualizarValorTotalGuia(novedadCambioValor);
                        //COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ActualizarValoresGuia(novedadGuia.Guia.IdAdmision, valorOperacion, novedadGuia.Guia.ValorAdicionales);

                        //AGREGAR NOVEDAD GUIA
                        datosAdicionales.Add(CCEnumNovedadRealizada.ModificacionValorTotal, novedadCambioValor.NuevoValorTotal.ToString());
                        datosAdicionales.Add(CCEnumNovedadRealizada.ModificacionDestino, novedadGuia.NuevaLocalidadDestino.IdLocalidad + "-" + novedadGuia.NuevaLocalidadDestino.Nombre);

                        //datosAdicionales.Add(CCEnumNovedadRealizada.ModificacionValor, valorOperacion.ValorTotal.ToString());
                        fachadaAdmisiones.AdicionarNovedad(novedadGuia, datosAdicionales);
                    }

                    #endregion Actualizacion de la Guia y Novedad
                }
                else if (novedadGuia.Guia.TipoCliente.Equals(ADEnumTipoCliente.INT))
                    //AFECTAR LOGÍSTICAMENTE
                    fachadaAdmisiones.ActualizarDestinoGuia(novedadGuia.Guia.IdAdmision, centroServicioDestino, new CCReliquidacionDC() { ValorTransporte = 0, ValorImpuestos = 0, ValorPrimaSeguro = 0 }, null, novedadGuia.Guia.IdTipoEntrega, novedadGuia.Guia.DescripcionTipoEntrega);
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.
                        MODULO_CONTROL_CUENTAS, "0", MensajesControlCuentas.CargarMensaje(EnumTipoErrorControlCuentas.EX_ERROR_FORMA_DE_PAGO_NO_ENCONTRADA)));

                transaccion.Complete();
            }
        }

        /// <summary>
        /// Crear novedad cambio de Peso
        /// </summary>
        /// <param name="novedadGuia"></param>
        internal void CrearNovedadCambioPeso(CCNovedadCambioPesoDC novedadGuia)
        {
            novedadGuia.TipoNovedad = CCEnumTipoNovedadGuia.ModificarDestinoGuia;
            TAServicioDC servicio = new TAServicioDC() { IdServicio = novedadGuia.Guia.IdServicio };
            PALocalidadDC localidadorigen = new PALocalidadDC() { IdLocalidad = novedadGuia.Guia.IdCiudadOrigen };
            List<TAFormaPago> formPagoAfectacion = null;
            novedadGuia.IdModulo = COConstantesModulos.MODULO_CONTROL_CUENTAS;
            novedadGuia.Guia.Peso = novedadGuia.NuevoPeso;

            // Validacion del nuevo trayecto
            // ADValidacionServicioTrayectoDestino validacionTrayecto = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ValidarServicioTrayectoDestino(localidadorigen, new PALocalidadDC() { IdLocalidad = novedadGuia.Guia.IdCiudadDestino }, servicio, 0);

            // Afectar caja de acuerdo a la forma de pago y al responsable
            ADGuiaFormaPago formaPago = novedadGuia.Guia.FormasPago.First();

            using (TransactionScope transaccion = new TransactionScope())
            {
                if (formaPago != null)
                {
                    bool esIngreso = true;
                    CAConceptoCajaDC concepto = null;
                    CCReliquidacionDC valorOperacion = null;
                    PUCentroServiciosDC centroServicioAfectado = null;
                    decimal valorAdicionales = 0;

                    concepto = new CAConceptoCajaDC()
                    {
                        IdConceptoCaja = (int)CAEnumConceptosCaja.AJUSTE_CAMBIO_PESO,
                        Nombre = CCConstantesControlCuentas.CONCEPTO_AJUSTE_X_CAMBIO_DE_PESO,
                        Descripcion = string.Format("{0}/factura No. {1}", CCConstantesControlCuentas.CONCEPTO_AJUSTE_X_CAMBIO_DE_PESO, novedadGuia.Guia.NumeroGuia),
                    };

                    //Reliquido con nuevo peso
                    valorOperacion = Reliquidacion(novedadGuia.Guia, novedadGuia.Guia.IdCiudadOrigen, novedadGuia.Guia.IdCiudadDestino);
                    valorOperacion.ValorImpuestos = novedadGuia.Guia.ValorTotalImpuestos;

                    decimal valorRecalculado;
                    if (novedadGuia.Guia.ValorTotal < valorOperacion.ValorTotal)
                    {
                        esIngreso = true;
                        valorRecalculado = valorOperacion.ValorTotal - novedadGuia.Guia.ValorTotal;
                    }
                    else
                    {
                        esIngreso = false;
                        valorRecalculado = novedadGuia.Guia.ValorTotal - valorOperacion.ValorTotal;
                    }

                    formPagoAfectacion = new List<TAFormaPago>()
                            {
                                new TAFormaPago()
                                {
                                    IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                                    Descripcion = "Contado",
                                    Valor = valorRecalculado
                                }
                            };

                    #region Forma de Pago Contado y Al Cobro

                    if (formaPago.IdFormaPago.Equals((short)TAEnumFormaPago.EFECTIVO))
                    {
                        //Ingreso a la agencia origen por el nuevo valor desde la ubicación actual hasta el nuevo destino
                        centroServicioAfectado = new PUCentroServiciosDC()
                        {
                            IdCentroServicio = novedadGuia.Guia.IdCentroServicioOrigen,
                            Nombre = novedadGuia.Guia.NombreCentroServicioOrigen
                        };

                        if (novedadGuia.Guia.IdMensajero == 0)
                            AfectarCaja(novedadGuia, esIngreso, concepto, valorRecalculado, centroServicioAfectado, formPagoAfectacion, valorAdicionales);
                        else
                        {
                            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

                            fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC
                            {
                                ConceptoEsIngreso = esIngreso,
                                ConceptoCajaMensajero = concepto,
                                FechaGrabacion = DateTime.Now,
                                Mensajero = new OUNombresMensajeroDC
                                {
                                    IdPersonaInterna = novedadGuia.Guia.IdMensajero,
                                    NombreApellido = novedadGuia.Guia.NombreMensajero,
                                },
                                Valor = valorRecalculado,
                                NumeroDocumento = novedadGuia.Guia.NumeroGuia,
                                Observaciones = concepto.Descripcion,
                                UsuarioRegistro = ControllerContext.Current.Usuario,
                            });
                        }
                    }

                    #endregion Forma de Pago Contado y Al Cobro

                    #region Forma de Pago AlCobro

                    else if (formaPago.IdFormaPago.Equals((short)TAEnumFormaPago.AL_COBRO))
                    {
                        //Buscar las planillas donde esta y si no está descargada de esa planilla se debe afectar la caja del mensajero con egreso de esa guia
                        List<OUGuiaIngresadaDC> planillasGuia = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().ObtenerPlanillasGuia(novedadGuia.Guia.IdAdmision);

                        planillasGuia.ForEach(p =>
                        {
                            if (p.IdMensajero >= 0 && p.EstadoGuiaPlanilla != "DEV" && !p.EstaPagada)
                            {
                                ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
                                fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC
                                {
                                    ConceptoEsIngreso = esIngreso,
                                    ConceptoCajaMensajero = concepto,
                                    FechaGrabacion = DateTime.Now,
                                    Mensajero = new OUNombresMensajeroDC
                                    {
                                        IdPersonaInterna = p.IdMensajero,
                                        NombreApellido = p.NombreCompleto
                                    },
                                    Valor = valorRecalculado,
                                    NumeroDocumento = novedadGuia.Guia.NumeroGuia,
                                    Observaciones = concepto.Descripcion,
                                    UsuarioRegistro = ControllerContext.Current.Usuario,
                                });
                            }
                        });

                        //Si esta entregada    se hace el movimiento en la caja de la agencia
                        if (novedadGuia.Guia.Entregada)
                        {
                            bool hacerAfectacion = false;
                            //si la guia no esta planillada a un mensajero
                            if (planillasGuia.Count == 0)
                            {
                                //afectacion en caja a la agencia destino por el valor inicial de la factura
                                centroServicioAfectado = new PUCentroServiciosDC()
                                {
                                    IdCentroServicio = novedadGuia.Guia.IdCentroServicioDestino,
                                    Nombre = novedadGuia.Guia.NombreCentroServicioDestino
                                };
                                hacerAfectacion = true;
                            }
                            //  si la guia esta planillada a un mensajero y ya esta pagada (descargada)
                            else if (planillasGuia.FirstOrDefault().EstaPagada)
                            {
                                //afectacion en caja a la agencia que descargó el al cobro
                                centroServicioAfectado = new PUCentroServiciosDC()
                                {
                                    IdCentroServicio = planillasGuia.FirstOrDefault().IdCentroLogistico,
                                    Nombre = planillasGuia.FirstOrDefault().NombreCentroLogistico
                                };
                                hacerAfectacion = true;
                            }

                            if (hacerAfectacion)
                            {
                                formPagoAfectacion = new List<TAFormaPago>()
                            {
                                new TAFormaPago()
                                {
                                    IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                                    Descripcion = "Contado",
                                    Valor = valorRecalculado
                                }
                            };

                                AfectarCaja(novedadGuia, esIngreso, concepto, valorRecalculado, centroServicioAfectado, formPagoAfectacion, valorAdicionales);
                            }
                        }
                    }

                    #endregion Forma de Pago AlCobro

                    #region Actualizacion y Novedad de Guia

                    //Se actualiza el valor peso de la Guía
                    fachadaAdmisiones.ActualizarValorPesoGuia(novedadGuia.Guia.IdAdmision, novedadGuia.Guia.Peso);

                    // Aplicar logísticamente el cambio en tbl AdminMensajeria
                    CCNovedadCambioValorTotal novedadCambioValor = new CCNovedadCambioValorTotal()
                    {
                        NuevoValorTransporte = valorOperacion.ValorTransporte,
                        NuevoValorPrima = valorOperacion.ValorPrimaSeguro,
                        NuevoValorComercial = novedadGuia.Guia.ValorDeclarado,
                        Guia = novedadGuia.Guia,
                    };
                    //Se Actualiza los valores de la Guía
                    fachadaAdmisiones.ActualizarValorTotalGuia(novedadCambioValor);

                    //Actualizo la Forma de pago en la Tabla de formas de pago de la Guía
                    CCNovedadCambioFormaPagoDC novedadCambio = new CCNovedadCambioFormaPagoDC()
                    {
                        Guia = new ADGuia() { IdAdmision = novedadGuia.Guia.IdAdmision },
                        FormaPagoAnterior = new ADGuiaFormaPago()
                        {
                            IdFormaPago = novedadGuia.Guia.FormasPago.First().IdFormaPago,
                            Valor = novedadGuia.Guia.FormasPago.First().Valor
                        },
                        FormaPagoNueva = new TAFormaPago()
                        {
                            IdFormaPago = novedadGuia.Guia.FormasPago.First().IdFormaPago,
                            Valor = valorOperacion.ValorTotal + valorOperacion.ValorImpuestos
                        }
                    };
                    fachadaAdmisiones.ActualizarFormaPagoGuia(novedadCambio);

                    //AGREGAR NOVEDAD GUIA
                    Dictionary<CCEnumNovedadRealizada, string> datosAdicionales = new Dictionary<CCEnumNovedadRealizada, string>();
                    datosAdicionales.Add(CCEnumNovedadRealizada.ModificacionValorTotal, valorOperacion.ValorTotal.ToString());
                    datosAdicionales.Add(CCEnumNovedadRealizada.ModificacionPeso, novedadGuia.NuevoPeso.ToString());
                    COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarNovedad(novedadGuia, datosAdicionales);

                    #endregion Actualizacion y Novedad de Guia
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.
                                MODULO_CONTROL_CUENTAS, "0", MensajesControlCuentas.CargarMensaje(EnumTipoErrorControlCuentas.EX_ERROR_FORMA_DE_PAGO_NO_ENCONTRADA)));
                }
                transaccion.Complete();
            }
        }

        /// <summary>
        /// crear novedad cambio forma de pago
        /// </summary>
        /// <param name="novedadGuia"></param>
        internal void CrearNovedadFormaPago(CCNovedadCambioFormaPagoDC novedadGuia)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (novedadGuia.FormaPagoAnterior.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CREDITO || novedadGuia.FormaPagoNueva.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CREDITO)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS, EnumTipoErrorControlCuentas.EX_FORMA_PAGO_CREDITO_INVALIDA.ToString(), MensajesControlCuentas.CargarMensaje(EnumTipoErrorControlCuentas.EX_FORMA_PAGO_CREDITO_INVALIDA)));
                }

                bool formaContadoAlCobro = novedadGuia.FormaPagoAnterior.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CONTADO && novedadGuia.FormaPagoNueva.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO;
                bool formaAlCobroContado = novedadGuia.FormaPagoAnterior.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO && novedadGuia.FormaPagoNueva.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CONTADO;

                if (!(formaContadoAlCobro || formaAlCobroContado))
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS,
                      EnumTipoErrorControlCuentas.EX_FORMA_PAGO_INVALIDA.ToString(),
                      string.Format(MensajesControlCuentas.CargarMensaje(EnumTipoErrorControlCuentas.EX_FORMA_PAGO_INVALIDA),
                                        novedadGuia.FormaPagoAnterior.Descripcion, novedadGuia.FormaPagoNueva.Descripcion)));
                }

                //Ingresar datos fijos de la novedad
                novedadGuia.IdModulo = COConstantesModulos.MODULO_CONTROL_CUENTAS;
                novedadGuia.TipoNovedad = CCEnumTipoNovedadGuia.ModificarFormaPagoGuia;
                novedadGuia.NovedadCentroServicios.FechaAplicacionPr = Framework.Servidor.Comun.ConstantesFramework.MinDateTimeController;
                novedadGuia.NovedadCentroServicios.IdProduccion = 0;

                // Adicionar tabla NovedadGuia_MEN
                Dictionary<CCEnumNovedadRealizada, string> datosAdicionales = new Dictionary<CCEnumNovedadRealizada, string>();
                datosAdicionales.Add(CCEnumNovedadRealizada.ModificacionFormaPago, ((TAEnumFormaPago)novedadGuia.FormaPagoNueva.IdFormaPago).ToString());
                fachadaAdmisiones.AdicionarNovedad(novedadGuia, datosAdicionales);

                // Si la nueva forma de pago es "Al Cobro" entonces se debe poner campo en true en la base de datos, si el origen es "Al cobro" se debe poner en false en la admisión original
                if (formaContadoAlCobro)
                {
                    //Actualizacion de la guia alcobro en true
                    fachadaAdmisiones.ActualizarEsAlCobro(novedadGuia.Guia.IdAdmision, true);
                    AfectarCajaCambioFormaPagoContadoAlCobro(novedadGuia);
                }
                else if (formaAlCobroContado)
                {
                    fachadaAdmisiones.ActualizarEsAlCobro(novedadGuia.Guia.IdAdmision, false);
                    AfectarCajaCambioFormaPagoAlCobroContado(novedadGuia);
                    fachadaAdmisiones.ActualizarPagadoGuia(novedadGuia.Guia.IdAdmision, true);
                }

                //Actualizar forma de pago
                fachadaAdmisiones.ActualizarFormaPagoGuia(novedadGuia);

                transaccion.Complete();
            }
        }

        /// <summary>
        /// Anula una guía de mensajería
        /// </summary>
        /// <param name="anulacion">Objeto</param>
        internal CCResultadoAnulacionGuia AnularGuia(CCAnulacionGuiaMensajeriaDC anulacion)
        {
            CCResultadoAnulacionGuia resultado = new CCResultadoAnulacionGuia()
            {
                GuiaExiste = true,
                NumeroGuia = anulacion.NumeroGuia
            };
            using (TransactionScope transaccion = new TransactionScope())
            {
                ADGuia admisionMensajeria = null;
                try
                {
                    //admisionMensajeria = fachadaAdmisiones.ObtenerGuiaXNumeroGuiaCredito(System.Convert.ToInt64(anulacion.NumeroGuia));
                    admisionMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(System.Convert.ToInt64(anulacion.NumeroGuia));
                }
                catch (FaultException<ControllerException> ex)
                {
                    if (ex.Detail.TipoError == ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString() || ex.Detail.TipoError == "EX_NUMERO_GUIA_NO_EXISTE")
                    {
                        admisionMensajeria = null;
                    }
                    else
                    {
                        throw ex;
                    }
                }
                if (admisionMensajeria != null && admisionMensajeria.IdAdmision != 0)
                {
                    anulacion.Guia = admisionMensajeria;

                    ADEnumEstadoGuia ultimoEstadoGuia = EstadosGuia.ObtenerUltimoEstado(admisionMensajeria.IdAdmision);
                    bool CambioEstado = EstadosGuia.ValidarCambioEstado(ultimoEstadoGuia, ADEnumEstadoGuia.Anulada);

                    // TODO ID: Validacion Antes de Anular. Si esta en Centro de acopio solo se puede Anular cuando ingresar al Centro de acopio automatico
                    if (CambioEstado && ultimoEstadoGuia == ADEnumEstadoGuia.CentroAcopio)
                    {
                        CambioEstado = EstadosGuia.EnCentrodeAcopio_Automatico(admisionMensajeria.NumeroGuia);
                    }

                    CAConceptoCajaDC concepto = new CAConceptoCajaDC()
                    {
                        IdConceptoCaja = (int)CAEnumConceptosCaja.CONCEPTOCAJA_GUIA_ANULADA,
                        Descripcion = "Anulación de la guía No. " + admisionMensajeria.NumeroGuia,
                        EsIngreso = false
                    };

                    if (CambioEstado)
                    {
                        List<ADGuiaFormaPago> formasPago = fachadaAdmisiones.ObtenerFormasPagoGuia(admisionMensajeria.IdAdmision);
                        anulacion.TrazaGuia.IdAdmision = admisionMensajeria.IdAdmision;
                        anulacion.TrazaGuia.NumeroGuia = anulacion.NumeroGuia;
                        anulacion.TrazaGuia.Modulo = COConstantesModulos.MODULO_CONTROL_CUENTAS;
                        anulacion.TrazaGuia.Usuario = ControllerContext.Current.Usuario;
                        anulacion.TrazaGuia.IdEstadoGuia = (short)(ultimoEstadoGuia);
                        anulacion.TrazaGuia.IdNuevoEstadoGuia = (short)(ADEnumEstadoGuia.Anulada);
                        anulacion.TrazaGuia.FechaGrabacion = DateTime.Now;

                        EstadosGuia.InsertaEstadoGuia(anulacion.TrazaGuia);

                        if (admisionMensajeria.TipoCliente != ADEnumTipoCliente.INT)
                        {
                            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
                            ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

                            PUCentroServiciosDC csMov = new PUCentroServiciosDC
                            {
                                Nombre = admisionMensajeria.NombreCentroServicioOrigen,
                                IdCentroServicio = admisionMensajeria.IdCentroServicioOrigen
                            };

                            if (admisionMensajeria.FormasPago.FirstOrDefault().IdFormaPago.Equals((short)TAEnumFormaPago.EFECTIVO))
                            {
                                //Afectar caja de la agencia origen del envio con un egreso por el mismo valor y misma forma de pago
                                AfectarCaja(anulacion, false, concepto,
                                            admisionMensajeria.ValorServicio + admisionMensajeria.ValorPrimaSeguro + admisionMensajeria.ValorAdicionales,
                                            csMov,
                                            formasPago.ConvertAll(f => new TAFormaPago
                                            {
                                                IdFormaPago = f.IdFormaPago,
                                                Descripcion = f.Descripcion,
                                                NumeroAsociadoFormaPago = f.NumeroAsociadoFormaPago,
                                                Valor = admisionMensajeria.ValorServicio + admisionMensajeria.ValorPrimaSeguro + admisionMensajeria.ValorAdicionales
                                            }),
                                            admisionMensajeria.ValorAdicionales,
                                            admisionMensajeria.ValorPrimaSeguro,
                                            admisionMensajeria.ValorDeclarado,
                                            admisionMensajeria.ValorServicio);
                            }
                            else if (admisionMensajeria.FormasPago.FirstOrDefault().IdFormaPago.Equals((short)TAEnumFormaPago.AL_COBRO))
                            {
                                //Afectar caja de la agencia origen del envio con un egreso por el mismo valor y misma forma de pago
                                AfectarCaja(anulacion, false, concepto,
                                                admisionMensajeria.ValorServicio + admisionMensajeria.ValorPrimaSeguro + admisionMensajeria.ValorAdicionales,
                                                csMov,
                                                formasPago.ConvertAll(f => new TAFormaPago
                                                {
                                                    IdFormaPago = f.IdFormaPago,
                                                    Descripcion = f.Descripcion,
                                                    NumeroAsociadoFormaPago = f.NumeroAsociadoFormaPago,
                                                    Valor = admisionMensajeria.ValorServicio + admisionMensajeria.ValorPrimaSeguro + admisionMensajeria.ValorAdicionales
                                                }),
                                                admisionMensajeria.ValorAdicionales,
                                                admisionMensajeria.ValorPrimaSeguro,
                                                admisionMensajeria.ValorDeclarado,
                                                admisionMensajeria.ValorServicio);

                                decimal valorCargado = 0;
                                decimal valorAdicionales = 0;
                                PUCentroServiciosDC csAfectado = fachadaCajas.ConsultarCentroDeServiciosPagoAlCobro(
                                                    admisionMensajeria.NumeroGuia, out valorCargado);
                                //Si ya fué pagado se hace un egreso a la agencia a la cual se le cargó el pago.
                                if (csAfectado != null)
                                {
                                    concepto.Descripcion = "Devolución de al cobro x anulación de factura No." + admisionMensajeria.NumeroGuia.ToString();
                                    csMov.IdCentroServicio = csAfectado.IdCentroServicio;
                                    csMov.Nombre = csAfectado.Nombre;
                                    AfectarCaja(anulacion, false, concepto, valorCargado, csMov, formasPago.ConvertAll(f => new TAFormaPago
                                    {
                                        IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                                        Descripcion = f.Descripcion,
                                        NumeroAsociadoFormaPago = f.NumeroAsociadoFormaPago,
                                        Valor = valorCargado
                                    }), valorAdicionales);
                                }
                            }
                            else if (admisionMensajeria.FormasPago.FirstOrDefault().IdFormaPago.Equals((short)TAEnumFormaPago.CREDITO))
                            {
                                if (!fachadaFacturacion.GuiaYaFacturada(admisionMensajeria.NumeroGuia))
                                {
                                    ADGuia guiaCliente = fachadaAdmisiones.ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(admisionMensajeria.NumeroGuia, admisionMensajeria.IdAdmision);
                                    string usr = ControllerContext.Current.Usuario;
                                    System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
                                        {
                                            ControllerContext.Current.Usuario = usr;
                                            fachadaClientes.ModificarAcumuladoContrato(guiaCliente.IdContrato, admisionMensajeria.ValorTotal * -1);
                                        });
                                    t.Start();
                                }
                                else
                                {
                                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS, "0", "La guía crédito que intenta anular ya se encuentra factura y no puede ser anulada."));
                                }
                            }

                            // Aplicar logísticamente el cambio en tbl AdminMensajeria
                            CCNovedadCambioValorTotal novedadCambioValor = new CCNovedadCambioValorTotal()
                            {
                                NuevoValorTransporte = 0,
                                NuevoValorPrima = 0,
                                Guia = admisionMensajeria,
                                NuevoValorComercial = 0
                            };
                            fachadaAdmisiones.ActualizarValorTotalGuia(novedadCambioValor);
                        }

                        // Grabar registro de novedad
                        Dictionary<CCEnumNovedadRealizada, string> datosAdicionales = new Dictionary<CCEnumNovedadRealizada, string>();
                        anulacion.TipoNovedad = CCEnumTipoNovedadGuia.AnulacionGuia;
                        anulacion.IdModulo = COConstantesModulos.MODULO_CONTROL_CUENTAS;
                        anulacion.QuienSolicita = string.Empty;
                        anulacion.ResponsableNovedad = new CCResponsableCambioDC() { DescripcionResponsable = string.Empty };
                        datosAdicionales.Add(CCEnumNovedadRealizada.AnulacionGuia, anulacion.MotivoAnulacion.IdMotivoAnulacion.ToString());
                        fachadaAdmisiones.AdicionarNovedad(anulacion, datosAdicionales);
                        resultado.FueAnulada = true;
                        resultado.Mensaje = "OK";
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS, EnumTipoErrorControlCuentas.EX_ESTADO_GUIA_NO_VALIDO.ToString(), MensajesControlCuentas.CargarMensaje(EnumTipoErrorControlCuentas.EX_ESTADO_GUIA_NO_VALIDO)));
                    }
                }
                else
                {
                    resultado.NumeroGuia = anulacion.NumeroGuia;
                    resultado.GuiaExiste = false;
                    resultado.FueAnulada = false;
                    resultado.Mensaje = MensajesControlCuentas.CargarMensaje(EnumTipoErrorControlCuentas.IN_GUIA_NO_EXISTE);

                    // Verificar que guía esté aprovisionada
                    try
                    {
                        CO.Servidor.Servicios.ContratoDatos.Suministros.SUPropietarioGuia prop = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerPropietarioGuia(anulacion.NumeroGuia, 0L);
                        switch (prop.Propietario)
                        {
                            case Servicios.ContratoDatos.Suministros.SUEnumGrupoSuministroDC.AGE:
                            case Servicios.ContratoDatos.Suministros.SUEnumGrupoSuministroDC.PTO:
                            case Servicios.ContratoDatos.Suministros.SUEnumGrupoSuministroDC.RAC:
                                resultado.Guia = new ADGuia
                                {
                                    IdCentroServicioOrigen = prop.CentroServicios.IdCentroServicio,
                                    NombreCentroServicioOrigen = prop.CentroServicios.Nombre,
                                    NumeroGuia = anulacion.NumeroGuia
                                };
                                break;

                            case Servicios.ContratoDatos.Suministros.SUEnumGrupoSuministroDC.CLI:
                                resultado.Guia = new ADGuia
                                {
                                    IdCentroServicioOrigen = prop.CentroServicios.IdCentroServicio,
                                    NombreCentroServicioOrigen = prop.CentroServicios.Nombre,
                                    IdSucursal = (int)prop.Id,
                                    NumeroGuia = anulacion.NumeroGuia
                                };
                                break;

                            case Servicios.ContratoDatos.Suministros.SUEnumGrupoSuministroDC.MEN:
                                resultado.Guia = new ADGuia
                                {
                                    IdCentroServicioOrigen = prop.CentroServicios.IdCentroServicio,
                                    NombreCentroServicioOrigen = prop.CentroServicios.Nombre,
                                    IdMensajero = prop.Id,
                                    NumeroGuia = anulacion.NumeroGuia
                                };
                                break;
                        }
                    }
                    catch (FaultException<ControllerException> ex)
                    {
                        if (ex.Detail.TipoError == "4") // Guía sin asignar
                        {
                            resultado.Mensaje = MensajesControlCuentas.CargarMensaje(EnumTipoErrorControlCuentas.IN_GUIA_NO_APROVISIONADA);
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                }

                transaccion.Complete();
            }
            return resultado;
        }

        /// <summary>
        /// Crea novedad de cambio de tipo de servicio en una admisión
        /// </summary>
        /// <param name="novedadGuia"></param>
        internal void CrearNovedadCambioTipoServicio(CCNovedadCambioTipoServicio novedadGuia)
        {
            ADGuiaFormaPago formaPago = novedadGuia.Guia.FormasPago.First();

            using (TransactionScope transaccion = new TransactionScope())
            {
                decimal valoAjustarCambioServicio = 0;
                decimal valorAdicionales = 0;
                bool esIngreso = false;
                novedadGuia.TipoNovedad = CCEnumTipoNovedadGuia.ModificarTipoServicioGuia;
                novedadGuia.IdModulo = COConstantesModulos.MODULO_CONTROL_CUENTAS;
                

                //// Si la guía es de un cliente crédito, valida si ha sido facturada, de lo contrario continue normalmente
                if (novedadGuia.Guia.IdCliente > 0 && fachadaFacturacion.GuiaYaFacturada(novedadGuia.Guia.NumeroGuia))
                {
                    throw new System.ServiceModel.FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS, "0", "No es posible hacer el cambio de tipo de servicio porque la guía ya se encuentra facturada"));
                }

                if (novedadGuia.IdServicio == (int)TAEnumServiciosDC.Notificaciones)
                {
                    fachadaAdmisiones.AdicionarNotificacion(novedadGuia.Guia.NumeroGuia);
                }

                
                // Aplicar movimiento de caja
                CAConceptoCajaDC concepto = new CAConceptoCajaDC()
                {
                    IdConceptoCaja = (int)CAEnumConceptosCaja.DESCUENTO_POR_CAMBIO_TIPO_SERVICIO,
                    Descripcion = string.Format("{0} para la guía No. {1}", CCConstantesControlCuentas.CONCEPTO_DIFERENCIA_X_CAMBIO_TIPO_SERVICIO, novedadGuia.Guia.NumeroGuia),
                    EsIngreso = true
                };

                // Si el valor cobrado es inferior al valor que se debía cobrar
                if (novedadGuia.NuevoValorTotal > novedadGuia.Guia.ValorTotal)
                {
                    esIngreso = true;
                    valoAjustarCambioServicio = novedadGuia.NuevoValorTotal - novedadGuia.Guia.ValorTotal;
                }
                // Si el valor cobrado es superior al valor que se debía cobrar
                else if (novedadGuia.NuevoValorTotal < novedadGuia.Guia.ValorTotal)
                {
                    //el movimiento de caja es un egreso
                    concepto.EsIngreso = false;
                    esIngreso = false;
                    valoAjustarCambioServicio = novedadGuia.Guia.ValorTotal - novedadGuia.NuevoValorTotal;
                }

                //Si la forma de pago es efectivo
                if (formaPago.IdFormaPago.Equals((short)TAEnumFormaPago.EFECTIVO))
                {
                    if (novedadGuia.Guia.IdMensajero == 0)
                        //AfectarCaja del ps origen con el valor calculado
                        AfectarCajaTipoServicio(novedadGuia, valoAjustarCambioServicio, concepto);
                    else
                    {
                        fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC
                        {
                            ConceptoEsIngreso = esIngreso,
                            ConceptoCajaMensajero = concepto,
                            FechaGrabacion = DateTime.Now,
                            Mensajero = new OUNombresMensajeroDC
                            {
                                IdPersonaInterna = novedadGuia.Guia.IdMensajero,
                                NombreApellido = novedadGuia.Guia.NombreMensajero,
                            },
                            Valor = valoAjustarCambioServicio,
                            NumeroDocumento = novedadGuia.Guia.NumeroGuia,
                            Observaciones = concepto.Descripcion,
                            UsuarioRegistro = ControllerContext.Current.Usuario,
                        });
                    }
                }
                else   //Si la forma de pago es al cobro
                    if (formaPago.IdFormaPago.Equals((short)TAEnumFormaPago.AL_COBRO))
                {
                    //Buscar las planillas donde esta y si no está descargada de esa planilla se debe afectar la caja del mensajero con egreso de esa guia
                    List<OUGuiaIngresadaDC> planillasGuia = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().ObtenerPlanillasGuia(novedadGuia.Guia.IdAdmision);

                    planillasGuia.ForEach(p =>
                    {
                        if (p.IdMensajero >= 0 && p.EstadoGuiaPlanilla != "DEV" && !p.EstaPagada)
                        {
                            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
                            fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC
                            {
                                ConceptoEsIngreso = esIngreso,
                                ConceptoCajaMensajero = concepto,
                                FechaGrabacion = DateTime.Now,
                                Mensajero = new OUNombresMensajeroDC
                                {
                                    IdPersonaInterna = p.IdMensajero,
                                    NombreApellido = p.NombreCompleto
                                },
                                Valor = valoAjustarCambioServicio,
                                NumeroDocumento = novedadGuia.Guia.NumeroGuia,
                                Observaciones = concepto.Descripcion,
                                UsuarioRegistro = ControllerContext.Current.Usuario,
                            });
                        }
                    });

                    //Si esta entregada    se hace el movimiento en la caja de la agencia
                    if (novedadGuia.Guia.Entregada)
                    {
                        PUCentroServiciosDC centroServicioAfectado = null;
                        bool hacerAfectacion = false;
                        //si la guia no esta planillada a un mensajero
                        if (planillasGuia.Count == 0)
                        {
                            //Egreso a la agencia destino por el valor inicial de la factura
                            centroServicioAfectado = new PUCentroServiciosDC()
                            {
                                IdCentroServicio = novedadGuia.Guia.IdCentroServicioDestino,
                                Nombre = novedadGuia.Guia.NombreCentroServicioDestino
                            };
                            hacerAfectacion = true;
                        }
                        //  si la guia esta planillada a un mensajero y ya esta pagada (descargada)
                        else if (planillasGuia.FirstOrDefault().EstaPagada)
                        {
                            //Egreso a la agencia que descargó el al cobro
                            centroServicioAfectado = new PUCentroServiciosDC()
                            {
                                IdCentroServicio = planillasGuia.FirstOrDefault().IdCentroLogistico,
                                Nombre = planillasGuia.FirstOrDefault().NombreCentroLogistico
                            };
                            hacerAfectacion = true;
                        }

                        if (hacerAfectacion)
                        {
                            List<TAFormaPago> formPagoAfectacion = new List<TAFormaPago>()
                            {
                                new TAFormaPago()
                                {
                                    IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                                    Descripcion = "Contado",
                                    Valor = valoAjustarCambioServicio
                                }
                            };

                            AfectarCaja(novedadGuia, esIngreso, concepto, valoAjustarCambioServicio, centroServicioAfectado, formPagoAfectacion, valorAdicionales);
                        }
                    }
                }

                // Aplicar logísticamente el cambio en tbl AdminMensajeria
                CCNovedadCambioValorTotal novedadCambioValor = new CCNovedadCambioValorTotal()
                {
                    NuevoValorTransporte = novedadGuia.NuevoValorTransporte,
                    NuevoValorPrima = novedadGuia.NuevoValorPrima,
                    NuevoValorComercial = novedadGuia.Guia.ValorDeclarado,
                    Guia = novedadGuia.Guia,
                };

                fachadaAdmisiones.ActualizarValorTotalGuia(novedadCambioValor);

                // Aplicar logísticamente el cambio
                fachadaAdmisiones.ActualizarTipoServicioGuia(novedadGuia.Guia.IdAdmision, novedadGuia.IdServicio);

                // Grabar registro de novedad
                Dictionary<CCEnumNovedadRealizada, string> datosAdicionales = new Dictionary<CCEnumNovedadRealizada, string>();
                datosAdicionales.Add(CCEnumNovedadRealizada.ModificacionValorTotal, novedadGuia.NuevoValorTotal.ToString());
                datosAdicionales.Add(CCEnumNovedadRealizada.ModificacionServicio, novedadGuia.IdServicio.ToString());
                COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarNovedad(novedadGuia, datosAdicionales);

                transaccion.Complete();
            }
        }

        /// <summary>
        /// Crea novedad de cambio de valor total de una guía y aplica las modificaciones relacionadas
        /// </summary>
        /// <param name="novedadGuia"></param>
        internal void CrearNovedadCambioValorTotal(CCNovedadCambioValorTotal novedadGuia)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                novedadGuia.TipoNovedad = CCEnumTipoNovedadGuia.ModificarValorTotalGuia;
                novedadGuia.IdModulo = COConstantesModulos.MODULO_CONTROL_CUENTAS;

                ADGuia facturaOriginal = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuia(novedadGuia.Guia.IdAdmision);
                List<ADGuiaFormaPago> formasPago = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerFormasPagoGuia(novedadGuia.Guia.IdAdmision);

                // Afectar caja de acuerdo a la forma de pago y al responsable
                ADGuiaFormaPago formaPago = novedadGuia.Guia.FormasPago.First();

                List<TAFormaPago> formPagoAfectacion = null;
                decimal valorTotalRecalculado = 0;
                decimal valoPrimaRecalculado = 0;
                decimal valorDeclaradoRecalc = 0;
                decimal? valorTransporteRecalc = 0;
                decimal valorAdicionales = 0;

                bool esIngreso = true;
                CAConceptoCajaDC concepto = null;

                PUCentroServiciosDC centroServicioAfectado = null;

                // Aplicar movimiento de caja
                if (novedadGuia.Guia.ValorTotal < novedadGuia.NuevoValorTotal)
                {
                    esIngreso = true;
                }
                else
                {
                    esIngreso = false;
                }

                valoPrimaRecalculado = facturaOriginal.ValorPrimaSeguro - novedadGuia.NuevoValorPrima;
                valorDeclaradoRecalc = Math.Abs(facturaOriginal.ValorDeclarado - novedadGuia.NuevoValorComercial);
                valorTransporteRecalc = facturaOriginal.ValorServicio - novedadGuia.NuevoValorTransporte;

                valorTotalRecalculado = Math.Abs(valoPrimaRecalculado + valorTransporteRecalc.Value);
                valorAdicionales = facturaOriginal.ValorAdicionales;

                valorTransporteRecalc = valorTransporteRecalc == 0 ? null : valorTransporteRecalc;
                valoPrimaRecalculado = Math.Abs(valoPrimaRecalculado);
                if (valorTransporteRecalc != null)
                    valorTransporteRecalc = Math.Abs(valorTransporteRecalc.Value);

                concepto = new CAConceptoCajaDC()
                {
                    IdConceptoCaja = (int)CAEnumConceptosCaja.DIFERENCIA_EN_VALOR_TOTAL,
                    Descripcion = string.Format("{0} para la guía No. {1}", CCConstantesControlCuentas.CONCEPTO_DIFERENCIA_X_CAMBIO_VALOR_TOTAL, novedadGuia.Guia.NumeroGuia),
                    EsIngreso = esIngreso
                };

                string observaciones = "";
                CCEnumNovedadRealizada novedadRealizada = CCEnumNovedadRealizada.ModificacionValorTotal;

                if (facturaOriginal.ValorServicio == novedadGuia.NuevoValorTransporte && facturaOriginal.ValorDeclarado != novedadGuia.NuevoValorComercial)
                {
                    observaciones = "Cambio de Valor Comercial factura No:" + facturaOriginal.NumeroGuia.ToString();
                    novedadRealizada = CCEnumNovedadRealizada.ModificacionValorDeclarado;
                    novedadGuia.TipoNovedad = CCEnumTipoNovedadGuia.ModificarValorDeclarao;
                }
                else
                    if (facturaOriginal.ValorServicio == novedadGuia.NuevoValorTransporte && facturaOriginal.ValorDeclarado == novedadGuia.NuevoValorComercial && facturaOriginal.ValorPrimaSeguro != novedadGuia.NuevoValorPrima)
                {
                    observaciones = "Cambio de Valor Prima factura No:" + facturaOriginal.NumeroGuia.ToString();
                    novedadRealizada = CCEnumNovedadRealizada.ModificacionPrima;
                    novedadGuia.TipoNovedad = CCEnumTipoNovedadGuia.ModificarValorPrima;
                }
                else if (facturaOriginal.ValorServicio != novedadGuia.NuevoValorTransporte && facturaOriginal.ValorDeclarado == novedadGuia.NuevoValorComercial && facturaOriginal.ValorPrimaSeguro == novedadGuia.NuevoValorPrima)
                {
                    observaciones = "Cambio de Valor Transporte factura No:" + facturaOriginal.NumeroGuia.ToString();
                    novedadRealizada = CCEnumNovedadRealizada.ModificacionValorTransporte;
                    novedadGuia.TipoNovedad = CCEnumTipoNovedadGuia.ModificarValorTransporte;
                }
                else
                    observaciones = "Cambio de Valor Total factura No:" + facturaOriginal.NumeroGuia.ToString();

                #region Forma de Pago Contado

                if (formaPago.IdFormaPago.Equals((short)TAEnumFormaPago.EFECTIVO))
                {
                    //afectación a la agencia origen por el nuevo valor
                    centroServicioAfectado = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = novedadGuia.Guia.IdCentroServicioOrigen,
                        Nombre = novedadGuia.Guia.NombreCentroServicioOrigen
                    };

                    formPagoAfectacion = new List<TAFormaPago>()
                            {
                                new TAFormaPago()
                                {
                                    IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                                    Descripcion = "Contado",
                                    Valor = valorTotalRecalculado
                                }
                            };
                    if (novedadGuia.Guia.IdMensajero == 0)
                    {
                        AfectarCaja(novedadGuia, esIngreso, concepto, valorTotalRecalculado, centroServicioAfectado, formPagoAfectacion, valorAdicionales, valoPrimaRecalculado, valorDeclaradoRecalc, valorTransporteRecalc, observaciones);
                    }
                    else
                    {
                        ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

                        fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC
                        {
                            ConceptoEsIngreso = true,
                            ConceptoCajaMensajero = concepto,
                            FechaGrabacion = DateTime.Now,
                            Mensajero = new OUNombresMensajeroDC
                            {
                                IdPersonaInterna = novedadGuia.Guia.IdMensajero,
                                NombreApellido = novedadGuia.Guia.NombreMensajero,
                            },
                            Valor = novedadGuia.NuevoValorTotal,
                            NumeroDocumento = novedadGuia.Guia.NumeroGuia,
                            Observaciones = observaciones,
                            UsuarioRegistro = ControllerContext.Current.Usuario,
                        });
                    }
                }

                #endregion Forma de Pago Contado

                #region Forma de Pago AlCobro

                else if (formaPago.IdFormaPago.Equals((short)TAEnumFormaPago.AL_COBRO))
                {
                    //Se afecta la caja de la oficina origen con forma de pago al cobro por la diferencia
                    centroServicioAfectado = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = facturaOriginal.IdCentroServicioOrigen,
                        Nombre = facturaOriginal.NombreCentroServicioOrigen,
                    };
                    formPagoAfectacion = new List<TAFormaPago>()
                    {
                                new TAFormaPago()
                                {
                                    IdFormaPago = (short)TAEnumFormaPago.AL_COBRO,
                                    Descripcion = "Al Cobro",
                                    Valor = valorTotalRecalculado
                                }
                    };
                    AfectarCaja(novedadGuia, esIngreso, concepto, valorTotalRecalculado, centroServicioAfectado, formPagoAfectacion, valorAdicionales, valoPrimaRecalculado, valorDeclaradoRecalc, valorTransporteRecalc, observaciones);

                    ///Se verifiva si ya está pagada
                    decimal valorCaja = 0;
                    PUCentroServiciosDC CSCajaAfectada = fachadaCajas.ConsultarCajaAfectadaPorPagoDeAlCobro(novedadGuia.Guia.NumeroGuia, out valorCaja);

                    //Si ya fué recaudada por una agencia
                    if (CSCajaAfectada != null)
                    {
                        //Se afecta la caja por la diferencia en la caja del centro de servicio que la recaudo
                        centroServicioAfectado = new PUCentroServiciosDC()
                        {
                            IdCentroServicio = CSCajaAfectada.IdCentroServicio,
                            Nombre = CSCajaAfectada.Nombre,
                        };
                        formPagoAfectacion = new List<TAFormaPago>()
                            {
                                new TAFormaPago()
                                {
                                    IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                                    Descripcion = "Contado",
                                    Valor = valorTotalRecalculado
                                }
                            };
                        AfectarCaja(novedadGuia, esIngreso, concepto, valorTotalRecalculado, centroServicioAfectado, formPagoAfectacion, valorAdicionales, valoPrimaRecalculado, valorDeclaradoRecalc, valorTransporteRecalc, observaciones);
                    }

                    IOUFachadaOperacionUrbana fachaOperUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
                    //Puede estar asignada a un mensajero
                    long numeroComprobante = fachaOperUrbana.AlCobroReportadoEnCaja(novedadGuia.Guia.NumeroGuia);

                    //No ha sido reportado en caja
                    if (numeroComprobante == 0)
                    {
                        fachaOperUrbana.NivelarCuentasMensajerosACeroXFactura(novedadGuia.Guia.NumeroGuia, concepto.Descripcion, concepto.IdConceptoCaja);

                        OUNombresMensajeroDC ultimoMensajeroAsignado = fachaOperUrbana.ConsultarUltimoMensajeroGuia(facturaOriginal.IdAdmision);

                        if (ultimoMensajeroAsignado != null)
                        {
                            fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
                            fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC
                            {
                                ConceptoEsIngreso = true,
                                ConceptoCajaMensajero = concepto,
                                FechaGrabacion = DateTime.Now,
                                Mensajero = new OUNombresMensajeroDC
                                {
                                    IdPersonaInterna = ultimoMensajeroAsignado.IdMensajero,
                                    NombreApellido = ultimoMensajeroAsignado.NombreApellido
                                },
                                Valor = novedadGuia.NuevoValorTotal,
                                NumeroDocumento = novedadGuia.Guia.NumeroGuia,
                                Observaciones = observaciones,
                                UsuarioRegistro = ControllerContext.Current.Usuario,
                            });
                        }
                    }
                }

                #endregion Forma de Pago AlCobro

                #region Forma de Pago Crédito

                if (formaPago.IdFormaPago.Equals((short)TAEnumFormaPago.CREDITO))
                {
                    //afectación a la agencia origen por el nuevo valor
                    centroServicioAfectado = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = novedadGuia.Guia.IdCentroServicioOrigen,
                        Nombre = novedadGuia.Guia.NombreCentroServicioOrigen
                    };

                    formPagoAfectacion = new List<TAFormaPago>()
                            {
                                new TAFormaPago()
                                {
                                    IdFormaPago = (short)TAEnumFormaPago.CREDITO,
                                    Descripcion = "Credito",
                                    Valor = valorTotalRecalculado
                                }
                            };
                    if (novedadGuia.Guia.IdMensajero == 0)
                    {
                        AfectarCaja(novedadGuia, esIngreso, concepto, valorTotalRecalculado, centroServicioAfectado, formPagoAfectacion, valorAdicionales, valoPrimaRecalculado, valorDeclaradoRecalc, valorTransporteRecalc, observaciones);
                    }
                }

                #endregion Forma de Pago Crédito

                #region Actualizacion de la Guia y Novedad

                // Aplicar logísticamente el cambio
                COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ActualizarValorTotalGuia(novedadGuia);

                // Grabar registro de novedad
                Dictionary<CCEnumNovedadRealizada, string> datosAdicionales = new Dictionary<CCEnumNovedadRealizada, string>();
                datosAdicionales.Add(novedadRealizada, novedadGuia.NuevoValorTotal.ToString());

                COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarNovedad(novedadGuia, datosAdicionales);

                #endregion Actualizacion de la Guia y Novedad

                transaccion.Complete();
            }
        }

        #endregion Cambios Guia (Destino-Peso-FormaPago-Anulacion-Valor)

        #region Cambio Forma de pago al cobro a crédito

        /// <summary>
        /// Ejecuta toda la lógica para cambiar una factura de forma de pago al cobro a crédito
        /// </summary>
        /// <param name="cambioFPAlCobroCredito">Datos del cambio</param>
        public void CambiarFPAlCobroACredito(CCNovedadFPAlCobroCreditoDC cambioFPAlCobroCredito)
        {
            using (TransactionScope tx = new TransactionScope())
            {
                CCRepositorio.Instancia.CambiarFPAlCobroACredito(cambioFPAlCobroCredito);
                tx.Complete();
            }
        }

        #endregion Cambio Forma de pago al cobro a crédito

        #region Afectacion Caja

        /// <summary>
        /// Afecta la caja x concepto de cambio de destino de un envío.
        /// La caja se afecta de acuerdo a los parámetros recibidos
        /// </summary>
        /// <param name="novedadGuia"></param>
        /// <param name="?"></param>
        public void AfectarCaja(
                ADNovedadGuiaDC novedadGuia, bool esIngreso, CAConceptoCajaDC concepto, CCReliquidacionDC valorOperacion,
                PUCentroServiciosDC centroServicioAfectado, List<TAFormaPago> formasPago)
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

            // Se debe consultar el centro de servicios responsable de la comisión del que vende
            PUCentroServiciosDC csResponsable = fachadaComisiones.ObtenerCentroServicioResponsableComisiones(centroServicioAfectado.IdCentroServicio, novedadGuia.Guia.IdServicio);

            CARegistroTransacCajaDC registroCentrSvcMenor = new CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = 0,
                    IdCodigoUsuario = novedadGuia.IdCodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                IdCentroResponsable = csResponsable.IdCentroServicio,
                IdCentroServiciosVenta = centroServicioAfectado.IdCentroServicio,
                NombreCentroResponsable = csResponsable.Nombre,
                NombreCentroServiciosVenta = centroServicioAfectado.Nombre,
                RegistrosTransacDetallesCaja = new List<CARegistroTransacCajaDetalleDC>()
                {
                  new CARegistroTransacCajaDetalleDC()
                  {
                     Cantidad = 1,
                     ConceptoEsIngreso = esIngreso,
                     ConceptoCaja = concepto,
                     Descripcion = concepto.Descripcion,
                     EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
                     FechaFacturacion = DateTime.Now,
                     Numero = novedadGuia.Guia.NumeroGuia,
                     NumeroFactura = novedadGuia.Guia.NumeroGuia.ToString(),
                     ValorDeclarado = 0,
                     ValoresAdicionales = novedadGuia.Guia.ValorAdicionales,
                     ValorImpuestos = valorOperacion.ValorImpuestos,
                     ValorPrimaSeguros = valorOperacion.ValorPrimaSeguro,
                     ValorRetenciones = 0,
                     ValorServicio = valorOperacion.ValorTransporte,
                     ValorTercero = 0,
                  }
                },

                ValorTotal = valorOperacion.ValorTotal,
                TotalImpuestos = valorOperacion.ValorImpuestos,
                TotalRetenciones = 0,
                Usuario = ControllerContext.Current.Usuario
            };

            registroCentrSvcMenor.RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>();
            foreach (TAFormaPago formaPago in formasPago)
            {
                registroCentrSvcMenor.RegistroVentaFormaPago.Add(
                      new CARegistroVentaFormaPagoDC
                      {
                          IdFormaPago = formaPago.IdFormaPago,
                          Descripcion = formaPago.Descripcion,
                          NumeroAsociado = formaPago.NumeroAsociadoFormaPago,
                          Valor = formaPago.Valor
                      }
                    );
            }
            fachadaCajas.AdicionarMovimientoCaja(registroCentrSvcMenor);
        }

        /// <summary>
        /// Afecta la caja x concepto de cambio de destino de un envío.
        /// La caja se afecta de acuerdo a los parámetros recibidos
        /// </summary>
        /// <param name="novedadGuia"></param>
        /// <param name="?"></param>
        public void AfectarCaja(ADNovedadGuiaDC novedadGuia, bool esIngreso, CAConceptoCajaDC concepto, decimal valorOperacion,
                                PUCentroServiciosDC centroServicioAfectado, List<TAFormaPago> formasPago, decimal valorAdicional, decimal valorPrima = 0,
                                decimal valordeclarado = 0, decimal? valorTransporte = 0, string observacion = "")
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

            // Se debe consultar el centro de servicios responsable de la comisión del que vende
            PUCentroServiciosDC csResponsable = fachadaComisiones.ObtenerCentroServicioResponsableComisiones(centroServicioAfectado.IdCentroServicio, novedadGuia.Guia.IdServicio);

            if (valorTransporte == 0)
                valorTransporte = valorOperacion;
            else if (valorTransporte == null)
                valorTransporte = 0;

            CARegistroTransacCajaDC registroCentrSvcMenor = new CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = 0,
                    IdCodigoUsuario = novedadGuia.IdCodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                IdCentroResponsable = csResponsable.IdCentroServicio,
                IdCentroServiciosVenta = centroServicioAfectado.IdCentroServicio,
                NombreCentroResponsable = csResponsable.Nombre,
                NombreCentroServiciosVenta = centroServicioAfectado.Nombre,

                RegistrosTransacDetallesCaja = new List<CARegistroTransacCajaDetalleDC>()
                {
                  new CARegistroTransacCajaDetalleDC()
                  {
                     Cantidad = 1,
                     ConceptoEsIngreso = esIngreso,
                     ConceptoCaja = concepto,
                     Descripcion = concepto.Descripcion,
                     EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
                     FechaFacturacion = DateTime.Now,
                     Numero = novedadGuia.Guia.NumeroGuia,
                     NumeroFactura = novedadGuia.Guia.NumeroGuia.ToString(),
                     ValorDeclarado = valordeclarado,
                     ValoresAdicionales = valorAdicional,
                     ValorImpuestos = 0,
                     ValorPrimaSeguros = valorPrima,
                     ValorRetenciones = 0,
                     ValorServicio = valorTransporte.Value,
                     ValorTercero = 0,
                     Observacion=observacion
                  }
                },

                ValorTotal = valorOperacion,
                TotalImpuestos = 0,
                TotalRetenciones = 0,
                Usuario = ControllerContext.Current.Usuario
            };

            registroCentrSvcMenor.RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>();
            foreach (TAFormaPago formaPago in formasPago)
            {
                registroCentrSvcMenor.RegistroVentaFormaPago.Add(
                      new CARegistroVentaFormaPagoDC
                      {
                          IdFormaPago = formaPago.IdFormaPago,
                          Descripcion = formaPago.Descripcion,
                          NumeroAsociado = formaPago.NumeroAsociadoFormaPago,
                          Valor = formaPago.Valor
                      }
                    );
            }
            fachadaCajas.AdicionarMovimientoCaja(registroCentrSvcMenor);
        }

        /// <summary>
        /// Aplica los movimientos de caja requeridos cuando el cambio de la forma de pago es de "Al cobro" a "Contado"
        /// </summary>
        /// <param name="novedadGuia"></param>
        /// <param name="descripcionOperacion"></param>
        private void AfectarCajaCambioFormaPagoAlCobroContado(CCNovedadCambioFormaPagoDC novedadGuia)
        {
            CAConceptoCajaDC concepto = null;
            PUCentroServiciosDC centroServicioAfectado = null;
            List<TAFormaPago> formPagoAfectacion;
            decimal valorAdicionales = 0;

            ///////////////////////////////////
            //afectación caja mensjeros y caja agencia destino
            concepto = new CAConceptoCajaDC()
            {
                IdConceptoCaja = (int)CAEnumConceptosCaja.DESCUENTO_POR_CAMBIO_FORMA_PAGO,
                Nombre = CCConstantesControlCuentas.CONCEPTO_AJUSTE_X_CAMBIO_FORMA_PAGO_AL_CT,
                Descripcion = string.Format("{0} guía/factura No. {1}", CCConstantesControlCuentas.CONCEPTO_AJUSTE_X_CAMBIO_FORMA_PAGO_AL_CT, novedadGuia.Guia.NumeroGuia),
            };

            ///Se saca todo lo que haya exista afectando la cuenta de mensajeros
            IOUFachadaOperacionUrbana fachaOperUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
            fachaOperUrbana.NivelarCuentasMensajerosACeroXFactura(novedadGuia.Guia.NumeroGuia, concepto.Descripcion, concepto.IdConceptoCaja);

            List<OUGuiaIngresadaDC> planillasGuia = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().ObtenerPlanillasGuia(novedadGuia.Guia.IdAdmision);
            OUGuiaIngresadaDC ultimaAsignacion = planillasGuia.OrderByDescending(o => o.FechaPlanilla).FirstOrDefault();

            ////Egreso con forma de pago al cobro en caja de centro de servicio origen
            formPagoAfectacion = new List<TAFormaPago>()
                          {new TAFormaPago()
                          {
                            IdFormaPago = (short)TAEnumFormaPago.AL_COBRO,
                            Descripcion = "Al Cobro",
                            Valor = novedadGuia.FormaPagoAnterior.Valor
                          }};
            concepto.EsIngreso = false;
            concepto.EsEgreso = true;

            centroServicioAfectado = new PUCentroServiciosDC()
            {
                IdCentroServicio = novedadGuia.Guia.IdCentroServicioOrigen,
                Nombre = novedadGuia.Guia.NombreCentroServicioOrigen
            };

            AfectarCaja(novedadGuia, false, concepto, novedadGuia.FormaPagoAnterior.Valor, centroServicioAfectado, formPagoAfectacion, valorAdicionales);

            ///Ingreso con forma de pago contado en caja de centro de servicio origen y contrapartida en casa matriz  (El concepto debe estar marcado para la contrapartida de casa matriz)
            formPagoAfectacion = new List<TAFormaPago>()
                          {new TAFormaPago()
                          {
                            IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                            Descripcion = "Contado",
                            Valor = novedadGuia.FormaPagoAnterior.Valor
                          }};
            concepto.EsIngreso = true;
            concepto.EsEgreso = false;

            centroServicioAfectado = new PUCentroServiciosDC()
            {
                IdCentroServicio = novedadGuia.Guia.IdCentroServicioOrigen,
                Nombre = novedadGuia.Guia.NombreCentroServicioOrigen
            };

            AfectarCaja(novedadGuia, true, concepto, novedadGuia.FormaPagoAnterior.Valor, centroServicioAfectado, formPagoAfectacion, valorAdicionales);

            ///Si ya está pagada
            decimal valorCaja = 0;
            PUCentroServiciosDC CSCajaAfectada = fachadaCajas.ConsultarCajaAfectadaPorPagoDeAlCobro(novedadGuia.Guia.NumeroGuia, out valorCaja);

            if (CSCajaAfectada != null)
            {
                ///Egreso con forma de pago contado al centro de servicio que descargó el al cobro y Contrapartida en casa matriz (El concepto debe estar marcado para la contrapartida de casa matriz)
                formPagoAfectacion = new List<TAFormaPago>()
                          {new TAFormaPago()
                          {
                            IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                            Descripcion = "Contado",
                            Valor = novedadGuia.FormaPagoAnterior.Valor
                          }};
                concepto.EsIngreso = false;
                concepto.EsEgreso = true;

                centroServicioAfectado = new PUCentroServiciosDC()
                {
                    IdCentroServicio = CSCajaAfectada.IdCentroServicio,
                    Nombre = CSCajaAfectada.Nombre
                };

                AfectarCaja(novedadGuia, false, concepto, novedadGuia.FormaPagoAnterior.Valor, centroServicioAfectado, formPagoAfectacion, valorAdicionales);
            }
        }

        /// <summary>
        /// Aplica movimientos de caja requeridos pra cuando el cambio de forma de pago es "Contado" a "Al Cobro"
        /// </summary>
        /// <param name="novedadGuia"></param>
        /// <param name="descripcionOperacion"></param>
        private void AfectarCajaCambioFormaPagoContadoAlCobro(CCNovedadCambioFormaPagoDC novedadGuia)
        {
            CAConceptoCajaDC concepto = null;
            PUCentroServiciosDC centroServicioAfectado = null;
            decimal valorAdicionales = 0;

            concepto = new CAConceptoCajaDC()
            {
                IdConceptoCaja = (int)CAEnumConceptosCaja.DESCUENTO_POR_CAMBIO_FORMA_PAGO,
                Nombre = CCConstantesControlCuentas.CONCEPTO_AJUSTE_X_CAMBIO_FORMA_PAGO_CT_AL,
                Descripcion = string.Format("{0} guía/factura No. {1}", CCConstantesControlCuentas.CONCEPTO_AJUSTE_X_CAMBIO_FORMA_PAGO_CT_AL, novedadGuia.Guia.NumeroGuia)
            };

            ///Se saca todo lo que haya exista afectando la cuenta de mensajeros
            IOUFachadaOperacionUrbana fachaOperUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
            fachaOperUrbana.NivelarCuentasMensajerosACeroXFactura(novedadGuia.Guia.NumeroGuia, concepto.Descripcion, concepto.IdConceptoCaja);

            List<OUGuiaIngresadaDC> planillasGuia = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().ObtenerPlanillasGuia(novedadGuia.Guia.IdAdmision);
            OUGuiaIngresadaDC ultimaAsignacion = planillasGuia.OrderByDescending(o => o.FechaPlanilla).FirstOrDefault();

            List<TAFormaPago> formPagoAfectacion;

            ////Ingreso con forma de pago al cobro en caja de centro de servicio origen
            formPagoAfectacion = new List<TAFormaPago>()
                          {new TAFormaPago()
                          {
                            IdFormaPago = (short)TAEnumFormaPago.AL_COBRO,
                            Descripcion = "Al Cobro",
                            Valor = novedadGuia.FormaPagoAnterior.Valor
                          }};
            concepto.EsIngreso = true;
            concepto.EsEgreso = false;

            centroServicioAfectado = new PUCentroServiciosDC()
            {
                IdCentroServicio = novedadGuia.Guia.IdCentroServicioOrigen,
                Nombre = novedadGuia.Guia.NombreCentroServicioOrigen
            };

            AfectarCaja(novedadGuia, true, concepto, novedadGuia.FormaPagoAnterior.Valor, centroServicioAfectado, formPagoAfectacion, valorAdicionales);

            ///Egreso con forma de pago contado en caja de centro de servicio origen y contrapartida en casa matriz  (El concepto debe estar marcado para la contrapartida de casa matriz)
            formPagoAfectacion = new List<TAFormaPago>()
                          {new TAFormaPago()
                          {
                            IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                            Descripcion = "Contado",
                            Valor = novedadGuia.FormaPagoAnterior.Valor
                          }};
            concepto.EsIngreso = false;
            concepto.EsEgreso = true;

            centroServicioAfectado = new PUCentroServiciosDC()
            {
                IdCentroServicio = novedadGuia.Guia.IdCentroServicioOrigen,
                Nombre = novedadGuia.Guia.NombreCentroServicioOrigen
            };

            AfectarCaja(novedadGuia, false, concepto, novedadGuia.FormaPagoAnterior.Valor, centroServicioAfectado, formPagoAfectacion, valorAdicionales);

            bool afectoamensajero = false;
            if (ultimaAsignacion != null)
            {
                ////Si está Planillada a mensajero
                if (ultimaAsignacion.IdMensajero >= 0 && ultimaAsignacion.EstadoGuiaPlanilla != "DEV")
                {
                    afectoamensajero = true;
                    ////Ingreso con forma de pago contado en la cuenta del mensajero
                    ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
                    concepto.EsIngreso = true;
                    concepto.EsEgreso = false;
                    fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC
                    {
                        ConceptoEsIngreso = true,
                        ConceptoCajaMensajero = concepto,
                        FechaGrabacion = DateTime.Now,
                        Mensajero = new OUNombresMensajeroDC
                        {
                            IdPersonaInterna = ultimaAsignacion.IdMensajero,
                            NombreApellido = ultimaAsignacion.NombreCompleto
                        },
                        Valor = novedadGuia.FormaPagoAnterior.Valor,
                        NumeroDocumento = novedadGuia.Guia.NumeroGuia,
                        Observaciones = concepto.Descripcion,
                        UsuarioRegistro = ControllerContext.Current.Usuario,
                    });

                    fachadaAdmisiones.ActualizarPagadoGuia(novedadGuia.Guia.IdAdmision, false);
                }
            }

            ///Si no se cargó a ningún mensajero y está ya entregada
            if (!afectoamensajero && novedadGuia.Guia.Entregada)
            {
                ///Ingreso con forma de pago contado al centro de servicio destino y Contrapartida en casa matriz (El concepto debe estar marcado para la contrapartida de casa matriz)
                formPagoAfectacion = new List<TAFormaPago>()
                          {new TAFormaPago()
                          {
                            IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                            Descripcion = "Contado",
                            Valor = novedadGuia.FormaPagoAnterior.Valor
                          }};
                concepto.EsIngreso = true;
                concepto.EsEgreso = false;

                centroServicioAfectado = new PUCentroServiciosDC()
                {
                    IdCentroServicio = novedadGuia.Guia.IdCentroServicioDestino,
                    Nombre = novedadGuia.Guia.NombreCentroServicioDestino
                };

                fachadaAdmisiones.ActualizarPagadoGuia(novedadGuia.Guia.IdAdmision, true);

                AfectarCaja(novedadGuia, true, concepto, novedadGuia.FormaPagoAnterior.Valor, centroServicioAfectado, formPagoAfectacion, valorAdicionales);
            }
            else
                fachadaAdmisiones.ActualizarPagadoGuia(novedadGuia.Guia.IdAdmision, false);
        }

        /// <summary>
        /// Afecta la caja x concepto de cambio de destino de un envío.
        /// La caja se afecta de acuerdo a los parámetros recibidos
        /// </summary>
        /// <param name="novedadGuia"></param>
        /// <param name="?"></param>
        internal void AfectarCajaCambioTipoServicio(CCNovedadCambioTipoServicio novedadGuia, bool esIngreso, CAConceptoCajaDC concepto, decimal valorOperacion, PUCentroServiciosDC centroServicioAfectado, TAFormaPago formaPago)
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

            // Se debe consultar el centro de servicios responsable de la comisión del que vende
            PUCentroServiciosDC csResponsable = fachadaComisiones.ObtenerCentroServicioResponsableComisiones(centroServicioAfectado.IdCentroServicio, novedadGuia.IdServicio);

            CARegistroTransacCajaDC registroCentrSvcMenor = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = 0,
                    IdCodigoUsuario = novedadGuia.IdCodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                IdCentroResponsable = csResponsable.IdCentroServicio,
                IdCentroServiciosVenta = centroServicioAfectado.IdCentroServicio,
                NombreCentroResponsable = csResponsable.Nombre,
                NombreCentroServiciosVenta = centroServicioAfectado.Nombre,
                RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
            {
              new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC()
              {
                 Cantidad = 1,
                 ConceptoEsIngreso = esIngreso,
                 ConceptoCaja = concepto,
                 Descripcion = concepto.Descripcion,
                 EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
                 FechaFacturacion = DateTime.Now,
                 Numero = novedadGuia.Guia.NumeroGuia,
                 NumeroFactura = novedadGuia.Guia.NumeroGuia.ToString(),
                 ValorDeclarado = 0,
                 ValoresAdicionales = 0,
                 ValorImpuestos = 0,
                 ValorPrimaSeguros = 0,
                 ValorRetenciones = 0,
                 ValorServicio = valorOperacion,
                 ValorTercero = 0,
              }
            },
                ValorTotal = valorOperacion,
                TotalImpuestos = 0,
                TotalRetenciones = 0,
                Usuario = ControllerContext.Current.Usuario,
                RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>()
            {
              new CARegistroVentaFormaPagoDC
              {
                 IdFormaPago = formaPago.IdFormaPago,
                 Descripcion = formaPago.Descripcion,
                 NumeroAsociado = novedadGuia.Guia.NumeroGuia.ToString(),
                 Valor = valorOperacion
              }
            }
            };
            fachadaCajas.AdicionarMovimientoCaja(registroCentrSvcMenor);
        }

        /// <summary>
        /// Metodo de realiza el proceso de afectacion de caja por
        /// tipo de servicio
        /// </summary>
        /// <param name="novedadGuia"></param>
        /// <param name="valoAjustarCambioServicio"></param>
        /// <param name="concepto"></param>
        private void AfectarCajaTipoServicio(CCNovedadCambioTipoServicio novedadGuia, decimal valoAjustarCambioServicio, CAConceptoCajaDC concepto)
        {
            AfectarCajaCambioTipoServicio(novedadGuia, concepto.EsIngreso, concepto, valoAjustarCambioServicio, new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC
            {
                IdCentroServicio = novedadGuia.Guia.IdCentroServicioOrigen,
                Nombre = novedadGuia.Guia.NombreCentroServicioOrigen
            },
            new Servicios.ContratoDatos.Tarifas.TAFormaPago
            {
                IdFormaPago = (short)TAEnumFormaPago.EFECTIVO,
                Valor = valoAjustarCambioServicio,
                Descripcion = "Contado",
            });
        }

        #endregion Afectacion Caja

        #region Otros

        /// <summary>
        /// Actualiza en la base de datos los datos del remitente y del destinatario de una guía o factura
        /// </summary>
        /// <param name="novedadGuia"></param>
        internal void ActualizarRemitenteDestinatarioGuia(CCNovedadCambioRemitenteDC novedadGuia)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                //Actualizar forma de pago
                COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ActualizarRemitenteDestinatarioGuia(novedadGuia);

                novedadGuia.IdModulo = COConstantesModulos.MODULO_CONTROL_CUENTAS;
                novedadGuia.TipoNovedad = CCEnumTipoNovedadGuia.ModificarRemDest;

                // Adicionar tabla NovedadGuia_MEN
                Dictionary<CCEnumNovedadRealizada, string> datosAdicionales = new Dictionary<CCEnumNovedadRealizada, string>();
                COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarNovedad(novedadGuia, datosAdicionales);

                transaccion.Complete();
            }
        }

        /// <summary>
        /// Se realiza el ingreso a mensajero, ese solo apilca cuando se hace cambio de forma de pago de "Contado" a "Al cobro"
        /// </summary>
        /// <param name="novedadGuia"></param>
        private void hacerIngresoMensajero(ADNovedadGuiaDC novedadGuia, string descripcion)
        {
            // Obtener estado de la guía
            IOUFachadaOperacionUrbana fachadaOpUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
            ADEnumEstadoGuia estadoGuia = EstadosGuia.ObtenerUltimoEstado(novedadGuia.Guia.IdAdmision);

            if (estadoGuia == ADEnumEstadoGuia.EnReparto || estadoGuia == ADEnumEstadoGuia.Entregada)
            {
                // Hacer ingreso a la cuenta del último mensajero que tiene planillado ese envío por concepto de pago al cobro
                OUPlanillaAsignacionMensajero asignacion = fachadaOpUrbana.ObtenerUltimaPlanillaMensajeroGuia(novedadGuia.Guia.NumeroGuia);
                if (asignacion != null)
                {
                    ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
                    fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC
                    {
                        ConceptoEsIngreso = true,
                        ConceptoCajaMensajero = new CAConceptoCajaDC()
                        {
                            EsIngreso = true,
                            IdConceptoCaja = (int)CAEnumConceptosCaja.PAGO_DE_ENVIO_AL_COBRO,
                            Descripcion = descripcion
                        },
                        FechaGrabacion = DateTime.Now,
                        Mensajero = new OUNombresMensajeroDC
                        {
                            IdPersonaInterna = asignacion.Mensajero.IdMensajero,
                            NombreApellido = asignacion.Mensajero.NombreCompleto
                        },
                        Valor = novedadGuia.Guia.ValorTotal,
                        NumeroDocumento = novedadGuia.Guia.NumeroGuia,
                        Observaciones = descripcion,
                        UsuarioRegistro = ControllerContext.Current.Usuario,
                    });

                    // Si el estado de la factura es entrega exitosa, se debe marcar como pagada
                    if (estadoGuia == ADEnumEstadoGuia.Entregada)
                    {
                        COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ActualizarPagadoGuia(novedadGuia.Guia.IdAdmision);
                    }
                }
            }
        }

        private void aplicarDescuentoCambioFormaPago(CCNovedadCambioFormaPagoDC novedadGuia, long idCentroServicios, CAConceptoCajaDC concepto, string descripcionConcepto, bool esIngreso, string nombreCentroServicios)
        {
            // Afectar caja de acuerdo a la forma de pago
            // Se debe consultar el centro de servicios responsable de la comisión del que vende
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();
            PUCentroServiciosDC csResponsable = fachadaComisiones.ObtenerCentroServicioResponsableComisiones(idCentroServicios, novedadGuia.Guia.IdServicio);
            CARegistroTransacCajaDC registroCentrSvcMenor = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = 0,
                    IdCodigoUsuario = novedadGuia.IdCodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                IdCentroResponsable = csResponsable.IdCentroServicio,
                IdCentroServiciosVenta = idCentroServicios,
                NombreCentroResponsable = csResponsable.Nombre,
                NombreCentroServiciosVenta = nombreCentroServicios,
                RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
            {
              new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC()
              {
                 Cantidad = 1,
                 ConceptoEsIngreso = esIngreso,
                 ConceptoCaja = concepto,
                 Descripcion = descripcionConcepto,
                 EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
                 FechaFacturacion = DateTime.Now,
                 Numero = novedadGuia.Guia.NumeroGuia,
                 NumeroFactura = novedadGuia.Guia.NumeroGuia.ToString(),
                 ValorDeclarado = 0,
                 ValoresAdicionales = 0,
                 ValorImpuestos = 0,
                 ValorPrimaSeguros = 0,
                 ValorRetenciones = 0,
                 ValorServicio = novedadGuia.FormaPagoNueva.Valor,
                 ValorTercero = 0,
              }
            },
                ValorTotal = novedadGuia.FormaPagoNueva.Valor,
                TotalImpuestos = 0,
                TotalRetenciones = 0,
                Usuario = ControllerContext.Current.Usuario,
                RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>()
            {
              new CARegistroVentaFormaPagoDC
              {
                 IdFormaPago = novedadGuia.FormaPagoNueva.IdFormaPago,
                 Descripcion = novedadGuia.FormaPagoNueva.Descripcion,
                 NumeroAsociado = novedadGuia.FormaPagoNueva.NumeroAsociadoFormaPago,
                 Valor = novedadGuia.FormaPagoNueva.Valor
              }
            }
            };
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
            fachadaCajas.AdicionarMovimientoCaja(registroCentrSvcMenor);
        }

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgencia(long idAgencia)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().ObtenerMensajerosDependientesCentroServicio(idAgencia);
        }

        /// <summary>
        /// Obtiene los clientes y sus contratos por agencia
        /// </summary>
        /// <param name="idAgencia">Identificador Agencia</param>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerClientesContratosXAgencia(long idAgencia)
        {
            return COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ObtenerClientesContratosXAgencia(idAgencia);
        }

        /// <summary>
        /// Trae el listado de los clientes crédito y sus respectivos contratos por una agencia y todas las agencias dependientes de la misma
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public List<CLClientesDC> ObtenerCLientesContratosXAgenciaDependientes(long idAgencia)
        {
            return COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ObtenerCLientesContratosXAgenciaDependientes(idAgencia);
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerPuntosDeAgencia(idCentroServicio);
        }

        /// <summary>
        /// Retorna la lista de puntos y agencias dependientes de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasDependientes(long idCentroServicio)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerPuntosAgenciasDependientes(idCentroServicio);
        }

        /// <summary>
        /// Obtiene las guías de mensajería a partir de una agencia
        /// </summary>
        /// <param name="agencia">Objeto Agencia</param>
        /// <returns>Colección de guías</returns>
        public List<ADGuia> ObtenerGuiasAgencia(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
        {
            return CCRepositorio.Instancia.ObtenerGuiasAgencia(agencia, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Obtiene las guías de mensajería a partir de una agencia y un cliente
        /// </summary>
        /// <param name="agencia">Objeto Agencia</param>
        /// <param name="cliente">Objeto Cliente</param>
        /// <returns>Colección guías de mensajería</returns>
        public List<ADGuia> ObtenerGuiasClienteCredito(PUCentroServiciosDC agencia, CLClientesDC cliente, DateTime fechaInicial, DateTime fechaFinal, int idSucursal)
        {
            return CCRepositorio.Instancia.ObtenerGuiasClienteCredito(agencia, cliente, fechaInicial, fechaFinal, idSucursal);
        }

        /// <summary>
        /// Obtiene las guías de mensajería a partir de una agencia y un mensajero
        /// </summary>
        /// <param name="agencia">Objeto Agencia</param>
        /// <param name="cliente">Objeto Cliente</param>
        /// <returns>Colección guías de mensajería</returns>
        public List<ADGuia> ObtenerGuiasMensajero(PUCentroServiciosDC agencia, OUNombresMensajeroDC mensajero, DateTime fechaInicial, DateTime fechaFinal)
        {
            return CCRepositorio.Instancia.ObtenerGuiasMensajero(agencia, mensajero, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Adiciona un registro al almacen de control de cuentas
        /// </summary>
        /// <param name="almacen">Objeto almacen</param>
        public CCAlmacenDC AdicionarAlmacenControlCuentas(CCAlmacenDC almacen)
        {
            ///////Si es un comprobante de pago debe validar que ya se encuentre digitalizada
            if (almacen.IdTipoOperacion == (short)CCEnumOperacion.PagoGiro)
            {
                ILIFachadaLogisticaInversa fachadaLogInversa = COFabricaDominio.Instancia.CrearInstancia<ILIFachadaLogisticaInversa>();
                if (!fachadaLogInversa.ConsultarArchivoComprobantePago(almacen.IdOperacion))
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS, "0", "El comprobante no puede ser archivado porque no se encuentra digitalizado."));
            }
            ///////////////////////////////////////////////////////////////////////////

            PAConsecutivoDC consecutivo = PAAdministrador.Instancia.ObtenerDatosConsecutivo(almacen.TipoConsecutivo);
            CCRepositorio.Instancia.ObtenerDatosAlmacen(almacen);
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (almacen.CajaLlena)
                {
                    long idConsecutivoNuevo = PAAdministrador.Instancia.ObtenerConsecutivo(almacen.TipoConsecutivo);
                    if (idConsecutivoNuevo > consecutivo.Actual + 1)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS, "0", "Se detectó un error de concurrencia al actualizar el número de caja. Intente nuevamente la operación."));
                    }
                    almacen.Caja = idConsecutivoNuevo;
                }

                CCRepositorio.Instancia.AdicionarAlmacenControlCuentas(almacen);

                transaccion.Complete();
            }
            return almacen;
        }

        /// <summary>
        /// Adiciona un registro al almacen de control de cuentas sin archivar, es decir que no adicionar lote, posición ni caja
        /// </summary>
        /// <param name="almacen">Objeto almacen</param>
        public CCAlmacenDC AdicionarAlmacenControlCuentasSinArchivar(CCAlmacenDC almacen)
        {
            almacen.Caja = 0;
            almacen.Lote = 0;
            almacen.Posicion = 0;
            CCRepositorio.Instancia.AdicionarAlmacenControlCuentas(almacen);
            return almacen;
        }

        /// <summary>
        /// Adicionar varios registros de almacén de control de cuentas sin archivar.
        /// </summary>
        /// <param name="almacen"></param>
        /// <param name="operaciones"></param>
        public void AdicionarVariosAlmacenControlCuentasSinArchivar(CCAlmacenDC almacen, List<long> operaciones)
        {
            almacen.Caja = 0;
            almacen.Lote = 0;
            almacen.Posicion = 0;
            operaciones.ForEach(op =>
            {
                almacen.IdOperacion = op;
                CCRepositorio.Instancia.AdicionarAlmacenControlCuentas(almacen);
            });
        }

        /// <summary>
        /// Obtiene los giros de una agencia
        /// </summary>
        /// <returns>Colección giros</returns>
        public List<GIAdmisionGirosDC> ObtenerGirosAgencia(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta)
        {
            return CCRepositorio.Instancia.ObtenerGirosAgencia(agencia, fechaDesde, fechaHasta);
        }

        /// <summary>
        /// Obtiene las admisiones y pagos de una agencia
        /// </summary>
        /// <param name="agencia">Objeto agencia</param>
        /// <returns>Colección admisiones pagos</returns>
        public List<PGPagoAdmisionGiroDC> ObtenerAdmisionPagoGiroAgencia(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta)
        {
            return CCRepositorio.Instancia.ObtenerAdmisionPagoGiroAgencia(agencia, fechaDesde, fechaHasta);
        }

        /// <summary>
        /// Obtiene los gastos de caja
        /// </summary>
        /// <param name="agencia">Objeto agencia</param>
        /// <returns>Colección de gastos</returns>
        public List<CAMovimientoCajaDC> ObtenerGastosCaja(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta)
        {
            return CCRepositorio.Instancia.ObtenerGastosCaja(agencia, fechaDesde, fechaHasta);
        }

        /// <summary>
        /// Obtiene los movimientos de caja para un centro de servicio dado en un rago de fechas que difieren de ventas de mensajería,
        /// pago de al cobros, venta de giros, pago de giros y ventas de pines prepago.
        /// </summary>
        ///<param name="agencia"></param>
        ///<param name="fechaFinal"></param>
        ///<param name="fechaInicial"></param>
        public List<CAMovimientoCajaDC> ObtenerOtrosMovimientosCaja(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
        {
            return CCRepositorio.Instancia.ObtenerOtrosMovimientosCaja(agencia, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Retorna las ventas de pin prepago realizads por la gencia en el rango de fechas dado
        /// </summary>
        /// <param name="agencia"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<CAMovimientoCajaDC> ObtenerVentasPinPrepago(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
        {
            return CCRepositorio.Instancia.ObtenerVentasPinPrepago(agencia, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Retorna las operaciones de caja que tengan el concepto de pago "Al Cobro" realizadas por la agencia en el rango de fechas dado
        /// </summary>
        /// <param name="agencia"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<CAMovimientoCajaDC> ObtenerRecaudosAlCobro(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
        {
            return CCRepositorio.Instancia.ObtenerRecaudosAlCobro(agencia, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Obtiene los motivos de anulación de una guía
        /// </summary>
        /// <returns>Colección motivos</returns>
        public List<ADMotivoAnulacionDC> ObtenerMotivosAnulacion()
        {
            return COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerMotivosAnulacion();
        }

        /// <summary
        /// Obtiene las agencias de la aplicación sin filtro
        /// </summary>
        public List<PUCentroServiciosDC> ObtenerAgencias()
        {
            return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgencias();
        }


        /// <summary>
        /// Obtiene la informacion del almacen
        /// </summary>
        /// <param name="idOperacion">Numero del giro</param>
        /// <returns></returns>
        public List<CCAlmacenDC> ObtenerAlmacenControlCuentas(long idOperacion)
        {
            return CCRepositorio.Instancia.ObtenerAlmacenControlCuentas(idOperacion);
        }

        /// <summary>
        /// Obtiene el almacen CtrolCuentas Giros
        /// </summary>
        /// <param name="idOperacion"></param>
        /// <returns>info del Archivo Control Ctas</returns>
        public CCAlmacenDC ObtenerAlmacenControlCuentasGiros(long idOperacion)
        {
            return CCRepositorio.Instancia.ObtenerAlmacenControlCuentasGiros(idOperacion);
        }

        #endregion Otros

        public decimal RegistrarEncabezadoCargueArchivoAjuste(CCEncabezadoArchivoAjusteGuiaDC encabezadoArchivo)
        {
            return CCRepositorio.Instancia.RegistrarEncabezadoCargueArchivoAjuste(encabezadoArchivo);
        }

        public void RegistarDetalleCargueArchivoAjuste(CCDetalladoArchivoAjusteGuiaDC detalladoArchivo)
        {
            CCRepositorio.Instancia.RegistarDetalleCargueArchivoAjuste(detalladoArchivo);
        }

        public void ActualizarEncabezadoCargueArchivoAjuste(long IdArchivo, short idEstado)
        {
            CCRepositorio.Instancia.ActualizarEncabezadoCargueArchivoAjuste(IdArchivo, idEstado);
        }

        public List<CCEncabezadoArchivoAjusteGuiaDC> ConsultarUltimosRegistrosCargueArchivoUsuario(string usuario)
        {
            return CCRepositorio.Instancia.ConsultarUltimosRegistrosCargueArchivoUsuario(usuario);
        }

        public List<long> ConsultarIdsArchivoNoProcesados(long idArchivo)
        {
            return CCRepositorio.Instancia.ConsultarIdsArchivoNoProcesados(idArchivo);
        }

        public void ProcesarRegistroArchivo(long idRegistro, string usuario)
        {
            CCRepositorio.Instancia.ProcesarRegistroArchivo(idRegistro);

            System.Threading.Tasks.Task.Factory.StartNew(() =>
               {
                   // 1. Obtener datos necesarios de la guía para calcular el precio
                   var guia = CCRepositorio.Instancia.ObtenerInformacionBasicaGuiaRegistroModificada(idRegistro);
                   decimal valorPrima = 0;
                   decimal valorTransporte = 0;
                   if (guia != null)
                   {
                       // 2. Calcular precio
                       try
                       {
                           var servicioAdmisiones = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
                           switch (guia.IdServicio)
                           {
                               case TAConstantesServicios.SERVICIO_CENTRO_CORRESPONDENCIA:
                                   break;
                               case TAConstantesServicios.SERVICIO_MENSAJERIA:
                                   var precioMensajeria = servicioAdmisiones.CalcularPrecioMensajeria(guia.IdServicio, guia.IdListaPrecios.Value, guia.IdCiudadOrigen, guia.IdCiudadDestino, guia.Peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                                   valorTransporte = precioMensajeria.Valor;
                                   valorPrima = precioMensajeria.ValorPrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                                   var preciocarga = servicioAdmisiones.CalcularPrecioCargaExpress(guia.IdServicio, guia.IdListaPrecios.Value, guia.IdCiudadOrigen, guia.IdCiudadDestino, guia.Peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                                   valorTransporte = preciocarga.Valor;
                                   valorPrima = preciocarga.ValorPrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_CARGA_AEREA:
                                   var precioCargaa = servicioAdmisiones.CalcularPrecioCargaAerea(guia.IdServicio, guia.IdListaPrecios.Value, guia.IdCiudadOrigen, guia.IdCiudadDestino, guia.Peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                                   valorTransporte = precioCargaa.Valor;
                                   valorPrima = precioCargaa.ValorPrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_NOTIFICACIONES:
                                   var precioNotifi = servicioAdmisiones.CalcularPrecioNotificaciones(guia.IdServicio, guia.IdListaPrecios.Value, guia.IdCiudadOrigen, guia.IdCiudadDestino, guia.Peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                                   valorTransporte = precioNotifi.Valor;
                                   valorPrima = precioNotifi.ValorPrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_RAPI_AM:
                                   var precioRapiAm = servicioAdmisiones.CalcularPrecioRapiAm(guia.IdServicio, guia.IdListaPrecios.Value, guia.IdCiudadOrigen, guia.IdCiudadDestino, guia.Peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                                   valorTransporte = precioRapiAm.Valor;
                                   valorPrima = precioRapiAm.ValorPrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_RAPI_CARGA:
                                   var precioRapiCarga = servicioAdmisiones.CalcularPrecioRapiCarga(guia.IdServicio, guia.IdListaPrecios.Value, guia.IdCiudadOrigen, guia.IdCiudadDestino, guia.Peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                                   valorTransporte = precioRapiCarga.Valor;
                                   valorPrima = precioRapiCarga.ValorPrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_RAPI_CARGA_CONTRA_PAGO:
                                   var precioRapicar = servicioAdmisiones.CalcularPrecioRapiCargaContraPago(guia.IdServicio, guia.IdListaPrecios.Value, guia.IdCiudadOrigen, guia.IdCiudadDestino, guia.Peso, 0, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                                   valorTransporte = precioRapicar.Valor;
                                   valorPrima = precioRapicar.ValorPrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO:
                                   break;
                               case TAConstantesServicios.SERVICIO_RAPI_HOY:
                                   var precioRapihoy = servicioAdmisiones.CalcularPrecioRapiHoy(guia.IdServicio, guia.IdListaPrecios.Value, guia.IdCiudadOrigen, guia.IdCiudadDestino, guia.Peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                                   valorTransporte = precioRapihoy.Valor;
                                   valorPrima = precioRapihoy.ValorPrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_RAPI_MASIVOS:

                               case TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO:
                                   var precioRapiPer = servicioAdmisiones.CalcularPrecioRapiPersonalizado(guia.IdServicio, guia.IdListaPrecios.Value, guia.IdCiudadOrigen, guia.IdCiudadDestino, guia.Peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                                   valorTransporte = precioRapiPer.Valor;
                                   valorPrima = precioRapiPer.ValorPrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_RAPI_PROMOCIONAL:
                                   var precioRapiPro = servicioAdmisiones.CalcularPrecioRapiPromocional(guia.IdListaPrecios.Value, 0, guia.IdTipoEntrega);
                                   valorTransporte = precioRapiPro.Valor;
                                   valorPrima = precioRapiPro.PrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_INTERNACIONAL:
                                   break;
                               case TAConstantesServicios.SERVICIO_RAPIRADICADO:
                                   var precioRapiRad = servicioAdmisiones.CalcularPrecioRapiRadicado(guia.IdListaPrecios.Value, guia.IdCiudadOrigen, guia.IdCiudadDestino, guia.Peso, guia.ValorDeclarado, true, guia.IdTipoEntrega);
                                   valorTransporte = precioRapiRad.Valor;
                                   valorPrima = precioRapiRad.ValorPrimaSeguro;
                                   break;
                               case TAConstantesServicios.SERVICIO_TRAMITES:
                                   break;
                           }

                           // 3.2 Si no hay error comparar los valores calculados con los obtenidos en la base de datos, si NO son iguales: auditar, de lo contrario no se hace nada.
                           if (guia.ValorPrima != valorPrima || guia.ValorTransporte != valorTransporte)
                           {
                               CCRepositorio.Instancia.InsertarLogErrorCalculoPrecios(guia.NumeroGuia, "OK", guia.IdListaPrecios, valorPrima, valorTransporte, valorPrima + valorTransporte, usuario, 2);
                           }

                       }
                       catch (FaultException<ControllerException> cex)
                       {
                           // 3.1 Si hay error, se debe auditar.
                           CCRepositorio.Instancia.InsertarLogErrorCalculoPrecios(guia.NumeroGuia, cex.Detail.Mensaje, guia.IdListaPrecios, null, null, null, usuario, 2);
                       }
                       catch (Exception ex)
                       {
                           // 3.1 Si hay error, se debe auditar.
                           CCRepositorio.Instancia.InsertarLogErrorCalculoPrecios(guia.NumeroGuia, ex.ToString(), guia.IdListaPrecios, null, null, null, usuario, 2);
                       }
                   }

               });
            // Acá se lanza de manera asíncrona el cálculo de precio.
            //COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>().CalcularPrecioMensajeria(3, "", "", "", 0, 0, true, 666);

            // Validar que solo se debe permitir modificar guías, no facturas.
        }
    }
}