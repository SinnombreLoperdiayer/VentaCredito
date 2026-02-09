using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Adminisiones.Mensajeria.Comun;
using CO.Servidor.Adminisiones.Mensajeria.Datos;
using CO.Servidor.Adminisiones.Mensajeria.Servicios;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionNacional;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Rutas;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Adminisiones.Mensajeria.Contado;
using Framework.Servidor.Seguridad;

namespace CO.Servidor.Adminisiones.Mensajeria.Movil
{
    public class ADAdmisionMovil : ControllerBase
    {
        private static readonly ADAdmisionMovil instancia = (ADAdmisionMovil)FabricaInterceptores.GetProxy(new ADAdmisionMovil(), COConstantesModulos.MENSAJERIA);

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static ADAdmisionMovil Instancia
        {
            get { return ADAdmisionMovil.instancia; }
        }

        public ADAdmisionMovil()
        {
        }


        #region Fachadas

        private IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
        private IONFachadaOperacionNacional fachadaOperacionNacional = COFabricaDominio.Instancia.CrearInstancia<IONFachadaOperacionNacional>();
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        private ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
        private ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();


        #endregion


        #region Metodos

        /// <summary>
        /// Método para registrar guia manual
        /// </summary>
        public ADResultadoAdmision RegistrarGuiaManualMovil(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario , ADNotificacion notificacion = null , ADRapiRadicado radicado = null)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                int codigoUsuario = SEAdministradorSeguridad.Instancia.ValidarUsuarioPersonaInternaAPP(ControllerContext.Current.Usuario);
                if (codigoUsuario== 0)
                {
                    throw new Exception("EL mensajero no esta configurado");
                }
                else
                {
                    guia.IdCodigoUsuario = codigoUsuario;
                    ControllerContext.Current.CodigoUsuario = codigoUsuario;
                }


                guia.IdAdmision = RegistrarGuia(guia, remitenteDestinatario);

                AdicionarMovimientoCaja(guia);

                GuardarConsumoGuia(guia);

                if (guia.IdServicio == ConstantesServicios.SERVICIO_NOTIFICACIONES && notificacion != null)
                {
                    RegistrarNotificacion(guia.IdAdmision, notificacion);
                }
                else if (guia.IdServicio == ConstantesServicios.SERVICIO_RAPIRADICADO && radicado != null)
                {
                    RegistrarRadicado(guia.IdAdmision, radicado);
                }

                transaccion.Complete();
                return new ADResultadoAdmision
                {
                    NumeroGuia = guia.NumeroGuia,
                    IdAdmision = guia.IdAdmision
                };
            }

        }

        /// <summary>
        /// Método para registrar guia manual
        /// </summary>
        public ADResultadoAdmision RegistrarGuiaAutomaticaMovil()
        {
            return new ADResultadoAdmision();
        }


        /// <summary>
        /// Guarda la guia de mensajería como consumida
        /// </summary>
        /// <param name="guia"></param>
        private void GuardarConsumoGuia(ADGuia guia)
        {
            SUConsumoSuministroDC consumo = new SUConsumoSuministroDC()
            {
                Cantidad = 1,
                EstadoConsumo = SUEnumEstadoConsumo.CON,
                IdServicioAsociado = guia.IdServicio,
                NumeroSuministro = guia.NumeroGuia,
                Suministro = SUEnumSuministro.FACTURA_VENTA_MENSAJERIA_MANUAL
            };

            if (guia.IdMensajero > 0)
            {
                consumo.GrupoSuministro = SUEnumGrupoSuministroDC.MEN;
                consumo.IdDuenoSuministro = guia.IdMensajero;
            }
            else
            {
                PUCentroServiciosDC centroServ = fachadaCentroServicio.ObtenerCentroServicio(guia.IdCentroServicioOrigen);
                SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), centroServ.Tipo);

                consumo.GrupoSuministro = grupo;
                consumo.IdDuenoSuministro = guia.IdCentroServicioOrigen;
            }
            fachadaSuministros.GuardarConsumoSuministro(consumo);
        }

        /// <summary>
        /// Registra una guía en el sistema
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        private long RegistrarGuia(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, bool validaIngresoACentroAcopio = false, long? agenciaRegistraAdmision = null)
        {
            remitenteDestinatario = ProcesarInfoRemitenteDestinatario(remitenteDestinatario, guia.TipoCliente, guia.IdCentroServicioOrigen, guia.NombreCentroServicioOrigen);


            if (guia.DiceContener != null)
            {
                guia.DiceContener = ADFunciones.CleanInput(guia.DiceContener);
            }

            if (guia.Observaciones != null)
            {
                guia.Observaciones = ADFunciones.CleanInput(guia.Observaciones);

            }

            // El campo de la guía "EsAlCobro" se llena a partir de las formas de pago seleccionadas
            ADGuiaFormaPago formaPagoAlcobro = guia.FormasPago.FirstOrDefault(formaPago => formaPago.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO && formaPago.Valor > 0);
            guia.EsAlCobro = (formaPagoAlcobro != null);
            guia.EstaPagada = !guia.EsAlCobro;
            guia.FechaPago = guia.EsAlCobro ? ConstantesFramework.MinDateTimeController : DateTime.Now;

            //validar fechas estimadas entrega, archivo, digitalizacion
            long idAdmisionMensajeria = ADRepositorio.Instancia.AdicionarAdmision(guia, remitenteDestinatario);

            guia.IdAdmision = idAdmisionMensajeria;

            // Se insertan las formas de pago
            ADRepositorio.Instancia.AdicionarGuiaFormasPago(idAdmisionMensajeria, guia.FormasPago, ControllerContext.Current.Usuario);


            // Con base en el tipo de cliente se inserta en las tablas relacionadas
            switch (guia.TipoCliente)
            {
                case ADEnumTipoCliente.CCO:

                    // La factura se le debe cargar al remitente
                    remitenteDestinatario.FacturaRemitente = true;
                    ADRepositorio.Instancia.AdicionarConvenioConvenio(idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, guia.IdCliente);
                    break;

                case ADEnumTipoCliente.CPE:
                    ADRepositorio.Instancia.AdicionarConvenioPeaton(idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, guia.IdCliente);
                    break;

                case ADEnumTipoCliente.PCO:
                    ADRepositorio.Instancia.AdicionarPeatonConvenio(guia.IdCliente, idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario);
                    break;

                case ADEnumTipoCliente.PPE:
                    ADRepositorio.Instancia.AdicionarPeatonPeaton(idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, guia.IdCentroServicioOrigen, guia.NombreCentroServicioOrigen);
                    break;
            }
            return idAdmisionMensajeria;
        }




        /// <summary>
        /// Adiciona un movimiento de caja para la transacción de guia creada
        /// </summary>
        /// <param name="idCaja"></param>
        /// <param name="fachadaCajas"></param>
        /// <param name="comision"></param>
        private void AdicionarMovimientoCaja(ADGuia guia)
        {
            CARegistroTransacCajaDC registro = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = guia.IdCaja,
                    IdCodigoUsuario = guia.IdCodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                IdCentroResponsable = guia.IdCentroServicioOrigen,
                IdCentroServiciosVenta = guia.IdCentroServicioOrigen,
                NombreCentroResponsable = guia.NombreCentroServicioOrigen,
                NombreCentroServiciosVenta = guia.NombreCentroServicioOrigen,
                RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
        {
          new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC() {
             Cantidad = 1,
             ConceptoCaja = new CAConceptoCajaDC() { IdConceptoCaja = guia.IdConceptoCaja },
             ConceptoEsIngreso = true,
             EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
             FechaFacturacion = DateTime.Now,
             Numero = guia.NumeroGuia,
             NumeroFactura = guia.NumeroGuia.ToString(),
             Observacion = guia.Observaciones,
             ValorDeclarado = guia.ValorDeclarado,
             ValoresAdicionales = guia.ValorAdicionales,
             ValorImpuestos = guia.ValorTotalImpuestos,
             ValorPrimaSeguros = guia.ValorPrimaSeguro, ValorRetenciones = guia.ValorTotalRetenciones,
             ValorServicio = guia.ValorServicio,
             ValorTercero = 0
          }
        },
                ValorTotal = guia.ValorAdmision + guia.ValorPrimaSeguro + guia.ValorAdicionales,
                TotalImpuestos = guia.ValorTotalImpuestos,
                TotalRetenciones = guia.ValorTotalRetenciones,
                Usuario = ControllerContext.Current.Usuario,
                RegistroVentaFormaPago = guia.FormasPago.ConvertAll(formaPago => new CARegistroVentaFormaPagoDC
                {
                    Valor = formaPago.Valor,
                    IdFormaPago = formaPago.IdFormaPago,
                    Descripcion = formaPago.Descripcion,
                    NumeroAsociado = formaPago.NumeroAsociadoFormaPago
                })
            };
            fachadaCajas.AdicionarMovimientoCaja(registro);
        }



        private ADMensajeriaTipoCliente ProcesarInfoRemitenteDestinatario(ADMensajeriaTipoCliente remitenteDestinatario, ADEnumTipoCliente tipoCliente, long idCentroServicio, string nombreCentroServicio)
        {
            string usuario = ControllerContext.Current.Usuario;

            if (remitenteDestinatario.PeatonRemitente != null)
            {
                remitenteDestinatario.PeatonRemitente.Apellido1 = ADFunciones.CleanInput(remitenteDestinatario.PeatonRemitente.Apellido1);
                remitenteDestinatario.PeatonRemitente.Apellido2 = ADFunciones.CleanInput(remitenteDestinatario.PeatonRemitente.Apellido2);
                remitenteDestinatario.PeatonRemitente.Nombre = ADFunciones.CleanInput(remitenteDestinatario.PeatonRemitente.Nombre);
                remitenteDestinatario.PeatonRemitente.Direccion = ADFunciones.CleanInput(remitenteDestinatario.PeatonRemitente.Direccion);
                remitenteDestinatario.PeatonRemitente.Telefono = ADFunciones.CleanInput(remitenteDestinatario.PeatonRemitente.Telefono);
            }

            if (remitenteDestinatario.PeatonDestinatario != null)
            {
                remitenteDestinatario.PeatonDestinatario.Apellido1 = ADFunciones.CleanInput(remitenteDestinatario.PeatonRemitente.Apellido1);
                remitenteDestinatario.PeatonDestinatario.Apellido2 = ADFunciones.CleanInput(remitenteDestinatario.PeatonRemitente.Apellido2);
                remitenteDestinatario.PeatonDestinatario.Nombre = ADFunciones.CleanInput(remitenteDestinatario.PeatonRemitente.Nombre);
                remitenteDestinatario.PeatonDestinatario.Direccion = ADFunciones.CleanInput(remitenteDestinatario.PeatonRemitente.Direccion);
                remitenteDestinatario.PeatonDestinatario.Telefono = ADFunciones.CleanInput(remitenteDestinatario.PeatonRemitente.Telefono);
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    // Grabar remitente y destinatario
                    CLClienteContadoDC remitente = new CLClienteContadoDC();
                    CLClienteContadoDC destinario = new CLClienteContadoDC();
                    if (tipoCliente == ADEnumTipoCliente.PCO || tipoCliente == ADEnumTipoCliente.PPE)
                    {
                        if (remitenteDestinatario.PeatonRemitente.Identificacion != null && remitenteDestinatario.PeatonRemitente.Identificacion != "0")
                        {
                            remitente = new Servidor.Servicios.ContratoDatos.Clientes.CLClienteContadoDC()
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
                    if (tipoCliente == ADEnumTipoCliente.CPE || tipoCliente == ADEnumTipoCliente.PPE)
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
                    if (tipoCliente != ADEnumTipoCliente.CCO)
                    {

                        fachadaClientes.RegistrarClienteContado(remitente, destinario, nombreCentroServicio, idCentroServicio, usuario);
                    }


                }
                catch (Exception exc)
                {
                    AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.ToString(), COConstantesModulos.MENSAJERIA);
                }
            }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);

            return remitenteDestinatario;
        }

        /// <summary>
        /// Método para registrar notificacion
        /// </summary>
        public void RegistrarNotificacion(long idAdmision , ADNotificacion notificacion)
        {
            ADAdmisionNotificacion.Instancia.AdicionarNotificacion(idAdmision, notificacion);
        }


        /// <summary>
        /// Método para registrar radicado
        /// </summary>
        public void RegistrarRadicado(long idAdmision, ADRapiRadicado rapiRadicado)
        {
            ADRepositorio.Instancia.AdicionarRapiRadicado(idAdmision, rapiRadicado, ControllerContext.Current.Usuario);

        }


        #endregion
    }
}
