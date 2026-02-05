using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Servicio.Entidades.Admisiones.Mensajeria;
using Servicio.Entidades.CentroServicios;
using Servicio.Entidades.Clientes;
using Servicio.Entidades.Comisiones;
using Servicio.Entidades.Suministros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using VentaCredito.CentroServicios;
using VentaCredito.Clientes;
using VentaCredito.Comisiones;
using VentaCredito.Datos.Repositorio;
using VentaCredito.Negocio.Contado;
using VentaCredito.Suministros;
using VentaCredito.Tarifas;
using VentaCredito.Transversal;
using VentaCredito.Transversal.Entidades;
using VentaCredito.Transversal.Utilidades;

namespace VentaCredito.Negocio
{
    public class ADAdmisionMensajeria
    {
        private static ADAdmisionMensajeria instancia = new ADAdmisionMensajeria();

        public static ADAdmisionMensajeria Instancia { get { return instancia; } }

        public HashSet<ADRangoTrayecto> TrayectosCasillero { get; set; }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        public ADResultadoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            int idBelcorp = -1;
            ADResultadoAdmision resultado = null;
            //if (guia.TokenClienteCredito != Guid.Empty)
            //{
            //    //idBelcorp = Convert.ToInt32(Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("idClienteBelcorp"));
            //    idBelcorp = Convert.ToInt32(Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("idClienteBelcorp"));

            //    if (guia.IdCliente == idBelcorp)
            //    {
            //        if (!ApiIntegracion.Instancia.VerificarTransaccionInventarioDevolucion(guia.TokenClienteCredito))
            //        {
            //            throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MENSAJERIA,
            //             ADEnumTipoErrorMensajeria.EX_ERROR_TRANSACCION_INVENT_DEVOLUCION_CLIENTE_CREDITO.ToString(),
            //             ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_TRANSACCION_INVENT_DEVOLUCION_CLIENTE_CREDITO)));
            //        }
            //    }
            //}
            var contrato = ObtenerContratoSucursal(guia.IdSucursal);
            guia.IdContrato = contrato.IdContrato;
            guia.IdListaPrecios = contrato.ListaPrecios;
            remitenteDestinatario.ConvenioRemitente.Contrato = guia.IdContrato;


            // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo            
            CLSucursalDC sucursal = CLConsultas.Instancia.ObtenerSucursalCliente(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
            if (sucursal != null)
            {
                if (guia.TipoCliente != ADEnumTipoCliente.PCO)
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
                }

                // Se obtiene el número de guía

                SUNumeradorPrefijo numeroSuministro;
                if (guia.NumeroGuia == 0)
                {

                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        numeroSuministro = Suministro.Instancia.ObtenerNumeroPrefijoValor(SUEnumSuministro.GUIA__TRANSPORTE_AUTOMATICA);
                        transaccion.Complete();
                    }

                    guia.NumeroGuia = numeroSuministro.ValorActual;
                    guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;
                }

                guia.ValorTotal = ObtenerValorTotal(ref guia);
                guia.NombreTipoEnvio = ObtenerNombreTipoEnvio(guia.IdTipoEnvio, guia.IdServicio);
                guia.FormasPago[0].Valor = guia.ValorTotal;



                using (TransactionScope transaccion = new TransactionScope())
                {
                    bool avisoPorcentajeMinimoAviso = CLClienteCredito.Instancia.ValidarCupoCliente(guia.IdContrato, guia.ValorTotal);
                    CLClienteCredito.Instancia.ModificarAcumuladoContrato(guia.IdContrato, guia.ValorTotal);

                    // Se deben calcular las comisiones de ventas
                    var comisiones = Comision.Instancia;
                    CMComisionXVentaCalculadaDC comision = CalcularComisionesPorVentas(guia, comisiones);
                    CMLiquidadorComisiones.Instancia.GuardarComision(comision);

                    // Se adiciona el movimiento de caja
                    //bool anulacionGuia = false;
                    // CATransaccionCaja.Instancia.AdicionarMovimientoCaja(guia, anulacionGuia);

                    // Se adiciona la admisión
                    guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                    guia.FechaAdmision = DateTime.Now;
                    long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);
                    guia.IdAdmision = idAdmisionMensajeria;
                    GuardarConsumoGuiaAutomatica(guia);

                    if (guia.IdServicio == 16)
                    {
                        AdmisionMensajeriaRepositorio.Instancia.AdicionarAdminRapiRadicado(guia.NumeroGuia);
                    }

                    if (guia.IdCliente == idBelcorp)
                    {
                        //asocia token de cliente credito con numero de guia
                        ApiIntegracion.Instancia.ActualizarTransaccionInventario(guia.TokenClienteCredito, guia.NumeroGuia);
                    }

                    transaccion.Complete();

                    string usuarioConexion = ContextoSitio.Current.Usuario;
                    string idClienteConexion = ContextoSitio.Current.IdCliente.ToString();
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        NotificarCliente(guia, usuarioConexion, idClienteConexion);
                    });
                    resultado = new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        IdAdmision = guia.IdAdmision,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = avisoPorcentajeMinimoAviso
                    };
                }



                //throw new FaultException<ControllerException>(
                //    new ControllerException(COConstantesModulos.MENSAJERIA,
                //    ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(),
                //    ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));

            }
            return resultado;
        }

        private string ObtenerNombreTipoEnvio(int idTipoEnvio, int idServicio)
        {
            return Tarifas.Parametros.Instancia.ObtenerNombreTipoEnvio(idTipoEnvio, idServicio);
        }

        private CLContratosDC ObtenerContratoSucursal(int idSucursal)
        {
            return Clientes.CLConsultas.Instancia.ObtenerContratosSucursal(idSucursal).OrderBy(c => c.FechaInicial).ToList().FirstOrDefault();
        }


        private decimal ObtenerValorTotal(ref ADGuia guia)
        {
            decimal valorTotal = 0;
            decimal valorMinimoDeclarado = ObtenerValorMinimoDeclarado(guia.IdListaPrecios.Value, guia.Peso);
            if (guia.ValorDeclarado < valorMinimoDeclarado)
            {
                guia.ValorDeclarado = valorMinimoDeclarado;
            }

            var listaValorServicio = CalcularPrecioServicios(new List<int> { guia.IdServicio },
                guia.IdListaPrecios.Value,
                guia.IdCiudadOrigen,
                guia.IdCiudadDestino,
                guia.Peso,
                guia.ValorDeclarado, guia.IdTipoEntrega).FirstOrDefault();

            if (listaValorServicio != null)
            {
                decimal valorImpuestos = listaValorServicio.Precio.Impuestos != null ? listaValorServicio.Precio.Impuestos.FirstOrDefault().Valor : 0;
                guia.ValorTotalImpuestos = valorImpuestos;
                guia.ValorAdmision = listaValorServicio.Precio.Valor;
                guia.ValorPrimaSeguro = listaValorServicio.Precio.ValorPrimaSeguro;
                guia.ValorServicio = listaValorServicio.Precio.Valor;
                valorTotal = listaValorServicio.Precio.ValorPrimaSeguro + listaValorServicio.Precio.Valor + valorImpuestos;
            }

            //var ValorPrimaSeguro = VentaCredito.Tarifas.Datos.Repositorio.PrecioMensajeria.Instancia.ObtenerPrimaSeguro((int)guia.IdListaPrecios, guia.ValorDeclarado, guia.IdServicio);

            return valorTotal;
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
                        //case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                        //    TAPreciosAgrupadosDC prec = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                        //    try
                        //    {
                        //        prec.Precio = CalcularPrecioCargaExpress(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                        //        precios.Add(prec);
                        //    }
                        //    catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        //    {
                        //        prec.Mensaje = ex.Detail.Mensaje;
                        //    }
                        //    break;

                        //case TAConstantesServicios.SERVICIO_CARGA_AEREA:
                        //    TAPreciosAgrupadosDC precAr = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                        //    try
                        //    {
                        //        precAr.Precio = CalcularPrecioCargaAerea(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado,idTipoEntrega);
                        //        precios.Add(precAr);
                        //    }
                        //    catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        //    {
                        //        precAr.Mensaje = ex.Detail.Mensaje;
                        //    }
                        //    break;

                        case TAConstantesServicios.SERVICIO_MENSAJERIA:
                            TAPreciosAgrupadosDC precMsj = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            //try
                            //{
                            precMsj.Precio = CalcularPrecioMensajeria(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                            precios.Add(precMsj);
                            //}
                            //catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                            //{
                            //    precMsj.Mensaje = ex.Detail.Mensaje;
                            //}
                            break;

                        //case TAConstantesServicios.SERVICIO_NOTIFICACIONES:
                        //    TAPreciosAgrupadosDC precNot = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                        //    try
                        //    {
                        //        precNot.Precio = CalcularPrecioNotificaciones(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                        //        precios.Add(precNot);
                        //    }
                        //    catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        //    {
                        //        precNot.Mensaje = ex.Detail.Mensaje;
                        //    }
                        //    break;

                        //case TAConstantesServicios.SERVICIO_RAPI_AM:
                        //    TAPreciosAgrupadosDC precAm = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                        //    try
                        //    {
                        //        precAm.Precio = CalcularPrecioRapiAm(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                        //        precios.Add(precAm);
                        //    }
                        //    catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        //    {
                        //        precAm.Mensaje = ex.Detail.Mensaje;
                        //    }
                        //    break;

                        //case TAConstantesServicios.SERVICIO_RAPI_CARGA:
                        //    TAPreciosAgrupadosDC precCar = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                        //    try
                        //    {
                        //        var precio = CalcularPrecioRapiCarga(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                        //        precCar.PrecioCarga = precio;
                        //        precCar.Precio = new TAPrecioMensajeriaDC()
                        //        {
                        //            Impuestos = precio.Impuestos,
                        //            Valor = precio.Valor,
                        //            ValorContraPago = precio.ValorContraPago,
                        //            ValorKiloAdicional = precio.ValorKiloAdicional,
                        //            ValorPrimaSeguro = precio.ValorPrimaSeguro
                        //        };
                        //        precios.Add(precCar);
                        //    }
                        //    catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        //    {
                        //        precCar.Mensaje = ex.Detail.Mensaje;
                        //    }
                        //    break;

                        case TAConstantesServicios.SERVICIO_RAPI_CARGA_CONTRA_PAGO:

                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO:

                            break;

                        //case TAConstantesServicios.SERVICIO_RAPI_HOY:
                        //    TAPreciosAgrupadosDC precHoy = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                        //    try
                        //    {
                        //        precHoy.Precio = CalcularPrecioRapiHoy(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                        //        precios.Add(precHoy);
                        //    }
                        //    catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        //    {
                        //        precHoy.Mensaje = ex.Detail.Mensaje;
                        //    }
                        //    break;

                        //case TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO:
                        //    TAPreciosAgrupadosDC precPer = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                        //    try
                        //    {
                        //        precPer.Precio = CalcularPrecioRapiPersonalizado(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                        //        precios.Add(precPer);
                        //    }
                        //    catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        //    {
                        //        precPer.Mensaje = ex.Detail.Mensaje;
                        //    }
                        //    break;

                        //case TAConstantesServicios.SERVICIO_RAPI_PROMOCIONAL:
                        //    break;

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
                        //case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                        //    TAPreciosAgrupadosDC prec = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                        //    try
                        //    {
                        //        prec.Precio = CalcularPrecioCargaExpress(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                        //        precios.Add(prec);
                        //    }
                        //    catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        //    {
                        //        prec.Mensaje = ex.Detail.Mensaje;
                        //    }
                        //    break;

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

                        //case TAConstantesServicios.SERVICIO_NOTIFICACIONES:
                        //    TAPreciosAgrupadosDC precNot = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                        //    try
                        //    {
                        //        precNot.Precio = CalcularPrecioMensajeria(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                        //        precios.Add(precNot);
                        //    }
                        //    catch (FaultException<Framework.Servidor.Excepciones.ControllerException> ex)
                        //    {
                        //        precNot.Mensaje = ex.Detail.Mensaje;
                        //    }
                        //    break;

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


            TAPrecioMensajeriaDC tarifa = new TAPrecioMensajeriaDC();
            Servicio.Entidades.Tarifas.Precios.TAPrecioMensajeriaDC PRecioReturn = TAServicioMensajeria.Instancia.ObtenerPrecioMensajeriaCredito(TAConstantesServicios.SERVICIO_RAPIRADICADO, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);

            var Impestosreturn = Tarifas.Datos.Repositorio.PrecioMensajeria.Instancia.ObtenerValorImpuestosServicio(TAConstantesServicios.SERVICIO_RAPIRADICADO)?.ToList();
            List<TAImpuestosDC> impuestos = null;
            if (Impestosreturn != null)
            {
                impuestos = new List<TAImpuestosDC>();
                foreach (var impuestort in Impestosreturn)
                {
                    TAImpuestosDC Impuesto = new TAImpuestosDC()
                    {
                        Actual = impuestort.Actual,
                        Asignado = impuestort.Asignado,
                        Descripcion = impuestort.Descripcion,
                        EstadoRegistro = impuestort.EstadoRegistro,
                        Identificador = impuestort.Identificador,
                        LiquidacionImpuesto = impuestort.LiquidacionImpuesto,
                        Valor = impuestort.Valor
                    };

                    impuestos.Add(Impuesto);
                }
            }


            tarifa.Impuestos = impuestos;
            tarifa.Valor = PRecioReturn.Valor;
            tarifa.ValorContraPago = PRecioReturn.ValorContraPago;
            tarifa.ValorKiloAdicional = PRecioReturn.ValorKiloAdicional;
            tarifa.ValorKiloInicial = PRecioReturn.ValorKiloInicial;
            tarifa.ValorOtrosServicios = PRecioReturn.ValorOtrosServicios;
            tarifa.ValorPrimaSeguro = PRecioReturn.ValorPrimaSeguro;
            return tarifa;
        }

        private decimal ObtenerValorMinimoDeclarado(int idListaPrecio, decimal peso)
        {
            return Tarifas.TAServicioMensajeria.Instancia.ObtenerValorMinimoDeclarado(idListaPrecio, peso);
        }

        private TAPrecioMensajeriaDC CalcularPrecioMensajeria(int servicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega, bool aplicaContraPago = false)
        {
            if (Tarifas.Datos.Repositorio.PrecioMensajeriaCredito.Instancia.ValorCarga == 0)
            {
                Tarifas.Datos.Repositorio.PrecioMensajeriaCredito.Instancia.ValorCarga = ADAdmisionMensajeria.Instancia.ObtenerParametrosAdmisiones().TopeMinVlrDeclRapiCarga;
            }

            // HACER mapper y  actualizar nugets co.servidor.contratosdatos y servicio.entidades
            var precioMensajeria = Tarifas.TAServicioMensajeria.Instancia.ObtenerPrecioMensajeriaCredito(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega, aplicaContraPago);

            var resultado =
             new TAPrecioMensajeriaDC
             {

                 Impuestos =
                            precioMensajeria.Impuestos != null
                            ? precioMensajeria.Impuestos.Select(c =>
                            new TAImpuestosDC
                            {
                                Actual = c.Actual,
                                Asignado = c.Asignado,
                                CuentaExterna = new TACuentaExternaDC
                                {
                                    IdCuentaExterna = c.CuentaExterna.IdCuentaExterna,
                                    Codigo = c.CuentaExterna.Codigo,
                                    Descripcion = c.CuentaExterna.Descripcion,
                                    EstadoRegistro = c.CuentaExterna.EstadoRegistro,
                                    IdNaturaliza = c.CuentaExterna.IdNaturaliza
                                },
                                Descripcion = c.Descripcion,
                                EstadoRegistro = c.EstadoRegistro,
                                Identificador = c.Identificador,
                                LiquidacionImpuesto = c.LiquidacionImpuesto,
                                Valor = c.Valor
                            }).ToList()
                : null,
                 Valor = precioMensajeria.Valor,
                 ValorContraPago = precioMensajeria.ValorContraPago,
                 ValorKiloAdicional = precioMensajeria.ValorKiloAdicional,
                 ValorKiloInicial = precioMensajeria.ValorKiloInicial,
                 ValorOtrosServicios = precioMensajeria.ValorOtrosServicios,
                 ValorPrimaSeguro = precioMensajeria.ValorPrimaSeguro,
             };

            return resultado;
        }

        private ADParametrosAdmisiones ObtenerParametrosAdmisiones()
        {
            return Datos.Repositorio.AdmisionMensajeriaRepositorio.Instancia.ObtenerParametrosAdmisiones();
        }

        private TAPrecioMensajeriaDC CalcularPrecioCargaAerea(int servicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega)
        {
            //return Tarifas.TATrayecto.Instancia.CalcularPrecioCargaAerea(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
            throw new NotImplementedException();
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
            //throw new NotImplementedException();

            TAPrecioMensajeriaDC tarifa = new TAPrecioMensajeriaDC();
            Servicio.Entidades.Tarifas.Precios.TAPrecioMensajeriaDC PRecioReturn =
                TATrayecto.Instancia.CalcularPrecioCargaExpress(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
            tarifa.Impuestos = new List<TAImpuestosDC>();
            tarifa.Valor = PRecioReturn.Valor;
            tarifa.ValorContraPago = PRecioReturn.ValorContraPago;
            tarifa.ValorKiloAdicional = PRecioReturn.ValorKiloAdicional;
            tarifa.ValorKiloInicial = PRecioReturn.ValorKiloInicial;
            tarifa.ValorOtrosServicios = PRecioReturn.ValorOtrosServicios;
            tarifa.ValorPrimaSeguro = PRecioReturn.ValorPrimaSeguro;
            return tarifa;
        }


        private static void NotificarCliente(ADGuia guia, string usuarioConexion, string idClienteConexion)
        {
            var notificacion = new ADTransmisionNotificacion
            {
                IdAdmisionMensajeria = guia.IdAdmision,
                IdEstadoNovMotivoGuia = (int)guia.EstadoGuia,
                NumeroGuia = guia.NumeroGuia,
                TipoNotificacion = EnumTipoNotificacion.EstadoGuia,
                Usuario = usuarioConexion,
                idCliente = guia.IdCliente

            };

            ServicioREST.Instancia.NotificarClienteCredito(notificacion, usuarioConexion, idClienteConexion);
        }

        /// <summary>
        /// Guarda la guia de mensajería como consumida
        /// </summary>
        /// <param name="guia"></param>
        private void GuardarConsumoGuiaAutomatica(ADGuia guia)
        {
            PUCentroServiciosDC centroServ = PUCentroServicios.Instancia.ObtenerCentroServicio(guia.IdCentroServicioOrigen);
            SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), centroServ.Tipo);

            SUConsumoSuministroDC consumo = new SUConsumoSuministroDC()
            {
                Cantidad = 1,
                EstadoConsumo = SUEnumEstadoConsumo.CON,
                GrupoSuministro = grupo,
                IdDuenoSuministro = guia.IdCentroServicioOrigen,
                IdServicioAsociado = guia.IdServicio,
                NumeroSuministro = 0,
                Suministro = SUEnumSuministro.GUIA__TRANSPORTE_AUTOMATICA
            };
            //ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            Suministro.Instancia.GuardarConsumoSuministro(consumo);
        }

        /// <summary>
        /// Retorna los rangos de peso y casilleros por un trayecto dado (ciudad origen y ciudad destino)
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public ADRangoTrayecto ConsultarCasilleroTrayecto(string idLocalidadOrigen, string idLocalidadDestino)
        {
            if (TrayectosCasillero == null)
            {
                TrayectosCasillero = ADTrayectoRepositorio.Instancia.ObtenerCasillerosTrayectos();
            }
            return TrayectosCasillero.FirstOrDefault(t => t.IdLocalidadOrigen == idLocalidadOrigen && t.IdLocalidadDestino == idLocalidadDestino);

        }

        /// <summary>
        /// Calcula las comisiones de venta para la admisión registrada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="fachadaComisiones"></param>
        /// <returns></returns>
        private CMComisionXVentaCalculadaDC CalcularComisionesPorVentas(ADGuia guia, Comision fachadaComisiones)
        {
            CMComisionXVentaCalculadaDC comision = Comision.Instancia.CalcularComisionesxVentas(
              new Servicio.Entidades.Comisiones.CMConsultaComisionVenta()
              {
                  IdCentroServicios = (int)guia.IdCentroServicioOrigen,
                  IdServicio = guia.IdServicio,
                  TipoComision = Servicio.Entidades.Comisiones.CMEnumTipoComision.Vender,
                  ValorBaseComision = guia.ValorServicio,
                  NumeroOperacion = guia.NumeroGuia,
              });
            return comision;
        }

        /// <summary>
        /// Registra una guía automática en el sistema
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        private long RegistrarGuia(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, bool validaIngresoACentroAcopio = false, long? agenciaRegistraAdmision = null)
        {
            //string usuario = "admin";
            string usuario = ContextoSitio.Current.Usuario;
            //int idServicioMaxtiempo = 0;
            //se eliminan caracteres tales como ", <, > Y SE REEMPLAZAN POR -  
            //Destinatario
            if (remitenteDestinatario.PeatonDestinatario != null)
            {
                remitenteDestinatario.PeatonDestinatario.Apellido1 = remitenteDestinatario.PeatonDestinatario.Apellido1.Replace("<", "-");
                remitenteDestinatario.PeatonDestinatario.Apellido1 = remitenteDestinatario.PeatonDestinatario.Apellido1.Replace(">", "-");
                remitenteDestinatario.PeatonDestinatario.Apellido1 = remitenteDestinatario.PeatonDestinatario.Apellido1.Replace('"', '-');
                remitenteDestinatario.PeatonDestinatario.Apellido2 = remitenteDestinatario.PeatonDestinatario.Apellido2.Replace("<", "-");
                remitenteDestinatario.PeatonDestinatario.Apellido2 = remitenteDestinatario.PeatonDestinatario.Apellido2.Replace(">", "-");
                remitenteDestinatario.PeatonDestinatario.Apellido2 = remitenteDestinatario.PeatonDestinatario.Apellido2.Replace('"', '-');
                remitenteDestinatario.PeatonDestinatario.Nombre = remitenteDestinatario.PeatonDestinatario.Nombre.Replace("<", "-");
                remitenteDestinatario.PeatonDestinatario.Nombre = remitenteDestinatario.PeatonDestinatario.Nombre.Replace(">", "-");
                remitenteDestinatario.PeatonDestinatario.Nombre = remitenteDestinatario.PeatonDestinatario.Nombre.Replace('"', '-');
                remitenteDestinatario.PeatonDestinatario.Direccion = remitenteDestinatario.PeatonDestinatario.Direccion.Replace("<", "-");
                remitenteDestinatario.PeatonDestinatario.Direccion = remitenteDestinatario.PeatonDestinatario.Direccion.Replace(">", "-");
                remitenteDestinatario.PeatonDestinatario.Direccion = remitenteDestinatario.PeatonDestinatario.Direccion.Replace('"', '-');
                remitenteDestinatario.PeatonDestinatario.Telefono = remitenteDestinatario.PeatonDestinatario.Telefono.Replace("<", "-");
                remitenteDestinatario.PeatonDestinatario.Telefono = remitenteDestinatario.PeatonDestinatario.Telefono.Replace(">", "-");
                remitenteDestinatario.PeatonDestinatario.Telefono = remitenteDestinatario.PeatonDestinatario.Telefono.Replace('"', '-');
            }
            //Otros
            guia.DiceContener = guia.DiceContener.Replace("<", "-");
            guia.DiceContener = guia.DiceContener.Replace(">", "-");
            guia.DiceContener = guia.DiceContener.Replace('"', '-');
            guia.Observaciones = guia.Observaciones.Replace("<", "-");
            guia.Observaciones = guia.Observaciones.Replace(">", "-");
            guia.Observaciones = guia.Observaciones.Replace('"', '-');
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                // Grabar remitente y destinatario
                CLClienteContadoDC remitente = new CLClienteContadoDC();
                    CLClienteContadoDC destinario = new CLClienteContadoDC();
                    if (guia.TipoCliente == ADEnumTipoCliente.PCO || guia.TipoCliente == ADEnumTipoCliente.PPE)
                    {
                        if (remitenteDestinatario.PeatonRemitente.Identificacion != null && remitenteDestinatario.PeatonRemitente.Identificacion != "0")
                        {
                            remitente = new CLClienteContadoDC()
                            {
                                Apellido1 = remitenteDestinatario.PeatonRemitente.Apellido1,
                                Apellido2 = remitenteDestinatario.PeatonRemitente.Apellido2,
                                Direccion = remitenteDestinatario.PeatonRemitente.Direccion,
                                Email = remitenteDestinatario.PeatonRemitente.Email,
                                Identificacion = remitenteDestinatario.PeatonRemitente.Identificacion,
                                Nombre = remitenteDestinatario.PeatonRemitente.Nombre,
                                Telefono = remitenteDestinatario.PeatonRemitente.Telefono,
                                TipoId = remitenteDestinatario.PeatonRemitente.TipoIdentificacion,
                                ClienteModificado = true
                            };
                        }
                    }
                    if (guia.TipoCliente == ADEnumTipoCliente.CPE || guia.TipoCliente == ADEnumTipoCliente.PPE)
                    {
                        if (remitenteDestinatario.PeatonDestinatario.Identificacion != null && remitenteDestinatario.PeatonDestinatario.Identificacion != "0")
                        {
                            destinario = new CLClienteContadoDC()
                            {
                                Apellido1 = remitenteDestinatario.PeatonDestinatario.Apellido1,
                                Apellido2 = remitenteDestinatario.PeatonDestinatario.Apellido2,
                                Direccion = remitenteDestinatario.PeatonDestinatario.Direccion,
                                Email = remitenteDestinatario.PeatonDestinatario.Email,
                                Identificacion = remitenteDestinatario.PeatonDestinatario.Identificacion,
                                Nombre = remitenteDestinatario.PeatonDestinatario.Nombre,
                                Telefono = remitenteDestinatario.PeatonDestinatario.Telefono,
                                TipoId = remitenteDestinatario.PeatonDestinatario.TipoIdentificacion,
                                ClienteModificado = true
                            };
                        }
                    }

                // Solo grabo los clientes frecuentes y los clientes contado cuando el envío no es convenio convenio dado que este aplica para clientes crédito
                if (guia.TipoCliente != ADEnumTipoCliente.CCO)
                    {
                        ClienteContado fachadaClientes = ClienteContado.Instancia;
                        fachadaClientes.RegistrarClienteContado(remitente, destinario, guia.NombreCentroServicioDestino, guia.IdCentroServicioDestino, usuario);
                    }
                }
                catch (Exception exc)
                {
                    AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.ToString(), COConstantesModulos.MENSAJERIA);
                }
            }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);

            // El campo de la guía "EsAlCobro" se llena a partir de las formas de pago seleccionadas
            ADGuiaFormaPago formaPagoAlcobro = guia.FormasPago.FirstOrDefault(formaPago => formaPago.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO && formaPago.Valor > 0);
            guia.EsAlCobro = (formaPagoAlcobro != null);
            guia.EstaPagada = !guia.EsAlCobro;
            if (!guia.EsAlCobro && guia.FormasPago.Exists(formaPago => formaPago.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CREDITO))
            {
                guia.EstaPagada = false;
            }
            guia.FechaPago = guia.EsAlCobro ? ConstantesFramework.MinDateTimeController : DateTime.Now;

            DateTime fechaActual = DateTime.Now;
            ADValidacionServicioTrayectoDestino SegundaValidacion = new ADValidacionServicioTrayectoDestino();
            List<ADTipoEntrega> tiposEntrega = Negocio.Parametros.Instancia.ObtenerTiposEntrega().ToList();
            guia.DescripcionTipoEntrega = tiposEntrega.Where(te => te.Id.Trim().Equals(guia.IdTipoEntrega)).FirstOrDefault().Descripcion;
            PALocalidadDC municipioOrigen = new PALocalidadDC { IdLocalidad = guia.IdCiudadOrigen, Nombre = guia.NombreCiudadOrigen };
            PALocalidadDC municipioDestino = new PALocalidadDC { IdLocalidad = guia.IdCiudadDestino, Nombre = guia.NombreCiudadDestino };

            TAServicioDC servicio = new TAServicioDC
            {
                IdServicio = guia.IdServicio,
                Nombre = Tarifas.Parametros.Instancia.ObtenerServicios().ToList().Where(t => t.IdServicio == guia.IdServicio).FirstOrDefault().Nombre
            };
            SegundaValidacion = ADAdmisionContado.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino,
                new Servicio.Entidades.Tarifas.TAServicioDC
                {
                    IdServicio = servicio.IdServicio,
                    Nombre = servicio.Nombre
                }
                , guia.IdCentroServicioOrigen, guia.Peso);
            if (SegundaValidacion.DuracionTrayectoEnHoras == 0)
            {
                ADValidacionServicioTrayectoDestino ValidacionCero = new ADValidacionServicioTrayectoDestino();
                ValidacionCero = ADAdmisionContado.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino,
                    new Servicio.Entidades.Tarifas.TAServicioDC
                    {
                        IdServicio = TAConstantesServicios.SERVICIO_MENSAJERIA,
                    }, guia.IdCentroServicioOrigen, guia.Peso);
                SegundaValidacion.DuracionTrayectoEnHoras = ValidacionCero.DuracionTrayectoEnHoras;
            }
            guia.FechaEstimadaEntrega = DateTime.Now.AddHours(SegundaValidacion.DuracionTrayectoEnHoras).Date.AddHours(18);
            guia.FechaEstimadaDigitalizacion = fechaActual.AddHours(SegundaValidacion.NumeroHorasDigitalizacion);
            guia.FechaEstimadaDigitalizacion = new DateTime(guia.FechaEstimadaDigitalizacion.Year, guia.FechaEstimadaDigitalizacion.Month, guia.FechaEstimadaDigitalizacion.Day, guia.FechaEstimadaDigitalizacion.Hour, 0, 0);
            guia.FechaEstimadaArchivo = fechaActual.AddHours(SegundaValidacion.NumeroHorasArchivo);
            guia.FechaEstimadaArchivo = new DateTime(guia.FechaEstimadaArchivo.Year, guia.FechaEstimadaArchivo.Month, guia.FechaEstimadaArchivo.Day, guia.FechaEstimadaArchivo.Hour, 0, 0);
            guia.NombreServicio = servicio.Nombre;

            PUCentroServiciosDC centroSerOrigen = PUAdministradorCentroServicios.Instancia.ObtenerAgenciaLocalidad(municipioOrigen.IdLocalidad);
            PUCentroServiciosDC centroSerDestino = PUAdministradorCentroServicios.Instancia.ObtenerAgenciaLocalidad(municipioDestino.IdLocalidad);

            guia.NombreCentroServicioOrigen = centroSerOrigen.Nombre;
            guia.IdCentroServicioDestino = centroSerDestino.IdCentroServicio;
            guia.NombreCentroServicioDestino = centroSerDestino.Nombre;
            guia.NombrePaisOrigen = "COLOMBIA";
            guia.NombreCiudadOrigen = guia.NombreCiudadOrigen;
            guia.CodigoPostalOrigen = centroSerOrigen.IdMunicipio;
            guia.NombrePaisDestino = "COLOMBIA";
            guia.NombreCiudadDestino = guia.NombreCiudadDestino;
            guia.CodigoPostalDestino = centroSerDestino.IdMunicipio;
            guia.NumeroBolsaSeguridad = "";
            long idAdmisionMensajeria = AdmisionMensajeriaRepositorio.Instancia.AdicionarAdmision(guia, remitenteDestinatario);
            guia.IdAdmision = idAdmisionMensajeria;
            // Agregar bit que indica si el destinatario autoriza envío de mensaje de texto
            if (guia.TipoCliente == ADEnumTipoCliente.CPE)
            {
                MensajesTextoRepositorio.Instancia.ValidarSiDestinatarioAutorizaEnvioMensajeTexto(guia.IdCliente,
                  remitenteDestinatario.PeatonDestinatario.TipoIdentificacion, remitenteDestinatario.PeatonDestinatario.Identificacion,
                  idAdmisionMensajeria, guia.NumeroGuia, guia.NoPedido
                  );
            }


            // Acá se debe validar si se debe generar ingreso a centro de acopio, esto debe aplicar solo para admisión manual COL
            if (validaIngresoACentroAcopio && agenciaRegistraAdmision.HasValue)
            {
                ADCentroAcopioRepositorio.Instancia.IngresarGuiaManualCentroAcopio(guia);
                //if (COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().GuiaYaFueIngresadaACentroDeAcopio(guia.NumeroGuia, agenciaRegistraAdmision.Value)
                //  || COFabricaDominio.Instancia.CrearInstancia<IONFachadaOperacionNacional>().GuiaYaFueIngresadaACentroDeAcopio(guia.NumeroGuia, agenciaRegistraAdmision.Value))
                //{
                //    PUCentroServiciosDC cs = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(agenciaRegistraAdmision.Value);
                //    COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().CambiarEstado(new Servidor.Servicios.ContratoDatos.OperacionUrbana.OUGuiaIngresadaDC
                //    {
                //        NumeroGuia = guia.NumeroGuia,
                //        IdAdmision = idAdmisionMensajeria,
                //        Observaciones = guia.Observaciones,
                //        IdCiudad = cs.CiudadUbicacion.IdLocalidad,
                //        Ciudad = cs.CiudadUbicacion.Nombre
                //    }, ADEnumEstadoGuia.EnCentroAcopio);
                //}
            }

            if (!string.IsNullOrEmpty(guia.NumeroBolsaSeguridad))
            {
                //Adminisiones.Mensajeria.Contado.ADAdmisionContado.Instancia.GuardarConsumoBolsaSeguridad(guia);
            }

            // Se insertan las formas de pago
            foreach (var formaPago in guia.FormasPago)
            {
                AdmisionMensajeriaRepositorio.Instancia.AdicionarGuiaFormasPago(idAdmisionMensajeria, formaPago, ContextoSitio.Current.Usuario);
            }

            // Se insertan los valores adicionales

            foreach (var valorAdicional in guia.ValoresAdicionales)
            {
                if (valorAdicional != null)
                {
                    AdmisionMensajeriaRepositorio.Instancia.AdicionarValoresAdicionales(idAdmisionMensajeria, valorAdicional, ContextoSitio.Current.Usuario);
                }
            }



            // Con base en el tipo de cliente se inserta en las tablas relacionadas
            switch (guia.TipoCliente)
            {
                case ADEnumTipoCliente.CCO:

                    // La factura se le debe cargar al remitente
                    remitenteDestinatario.FacturaRemitente = true;
                    AdmisionMensajeriaRepositorio.Instancia.AdicionarConvenioConvenio(idAdmisionMensajeria, remitenteDestinatario, ContextoSitio.Current.Usuario, guia.IdCliente);
                    break;

                case ADEnumTipoCliente.CPE:
                    AdmisionMensajeriaRepositorio.Instancia.AdicionarConvenioPeaton(idAdmisionMensajeria, remitenteDestinatario, ContextoSitio.Current.Usuario, guia.IdCliente);
                    break;

                case ADEnumTipoCliente.PCO:
                    AdmisionMensajeriaRepositorio.Instancia.AdicionarPeatonConvenio(guia.IdCliente, idAdmisionMensajeria, remitenteDestinatario, ContextoSitio.Current.Usuario);
                    break;

                case ADEnumTipoCliente.PPE:
                    AdmisionMensajeriaRepositorio.Instancia.AdicionarPeatonPeaton(idAdmisionMensajeria, remitenteDestinatario, ContextoSitio.Current.Usuario, guia.IdCentroServicioOrigen, guia.NombreCentroServicioOrigen);
                    break;
            }
            return idAdmisionMensajeria;
        }

        public AdmisionEnvioResponse RegistrarEnvioAutomatico(AdmisionEnvioRequest admisionEnvio)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registra en IController la venta para generar factura manual en el plazo de tiempo configurado
        /// en la tabla ParametrosFramework
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns>Si se ingreso con exito el registro</returns>
        public void RegistrarVentaParaFacturaManual(long idCliente, long numeroGuia, long idContrato, long idSucursal, string idLocalidad, decimal valorTotal, short idFormaPago)
        {
            AdmisionMensajeriaRepositorio.Instancia.RegistrarVentaParaFacturaManual(idCliente, numeroGuia, idContrato, idSucursal, idLocalidad, valorTotal, idFormaPago);
        }
    }
}
