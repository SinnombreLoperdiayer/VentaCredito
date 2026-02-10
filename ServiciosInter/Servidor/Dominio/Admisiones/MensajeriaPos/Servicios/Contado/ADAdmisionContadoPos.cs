
using CO.Servidor.Adminisiones.Mensajeria.Comun;
using CO.Servidor.Admisiones.MensajeriaPos.Servicios;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.MensajeriaPos.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Transactions;

namespace CO.Servidor.Admisiones.MensajeriaPos.Contado
{

    public class ADAdmisionContadoPos
    {

        #region Singleton

        private static readonly ADAdmisionContadoPos instancia = new ADAdmisionContadoPos();

        /// <summary>
        /// Retorna una instancia de Consultas de admision guía interna
        /// /// </summary>
        public static ADAdmisionContadoPos Instancia
        {
            get { return ADAdmisionContadoPos.instancia; }
        }

        #endregion Singleton

        #region propiedades
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #endregion

        #region metodos



        /// <summary>
        /// Registra los detalles de caja para los pagos con multiples medios de pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="conexion"></param>
        /// <param name="transaccion"></param>
        /// <param name="idTransaccionCaja"></param>
        private void RegistrarDetalleComisionesYMovimientosCaja(
            ADGuia guia,
            int idCaja,
            SqlConnection conexion,
            SqlTransaction transaccion,
            long idTransaccionCaja,
            ICAFachadaCajas fachadaCajas,
            long IdCentroServiciosVenta)
        {
            var registroDetalle =
                new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC()
                {
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
                    ValorPrimaSeguros = guia.ValorPrimaSeguro,
                    ValorRetenciones = guia.ValorTotalRetenciones,
                    ValorServicio = guia.ValorServicio,
                    ValorTercero = 0,
                    IdRegistroTranscaccion = idTransaccionCaja,

                };
            fachadaCajas.AdicionarDetalleMovimientoCaja(registroDetalle, conexion, transaccion, idTransaccionCaja,
                                        ControllerContext.Current.Usuario);

        }
        private CMComisionXVentaCalculadaDC CalcularComisionesPorVentasConMediosPago(
                    int idServicio,
                    int idCentroServicioOrigen,
                    ICMFachadaComisiones fachadaComisiones,
                    List<ADRegistroMediosPagoDC> mediosPago)
        {
            CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentasPos(
              new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
              {
                  IdCentroServicios = idCentroServicioOrigen,
                  IdServicio = idServicio,
                  TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Vender,
                  ValorBaseComision = mediosPago.Sum(mp => mp.Valor),
                  // NumeroOperacion = guia.NumeroGuia, // TODO: validar si este u otro numero si ó va ó no
              });

            return comision;
        }



        /// <summary>
        /// Adicionar registro transaccion caja con medios pago
        /// </summary>
        /// <param name="guias"></param>
        /// <param name="idCaja"></param>
        /// <param name="fachadaCajasPos"></param>
        /// <param name="mediosPago"></param>
        /// <param name="conexion"></param>
        /// <param name="registro"></param>
        /// <returns></returns>
        private long AdicionarRegistroTransaccionCajaConMediosPago(List<ADRegistroAdmisiones> guias, int idCaja, ICAFachadaCajasPos fachadaCajasPos, List<ADRegistroMediosPagoDC> mediosPago, SqlConnection conexion, out CARegistroTransacCajaDC registro)
        {
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

            CMComisionXVentaCalculadaDC comision =
                CalcularComisionesPorVentasConMediosPago(
                            guias.FirstOrDefault().IdServicio,
                            (int)guias.FirstOrDefault().Admision.IdCentroServicioOrigen,
                            fachadaComisiones,
                            mediosPago);

            registro = new CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = idCaja,
                    IdCodigoUsuario = guias.FirstOrDefault().Admision.IdCodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                IdCentroResponsable = comision.IdCentroServicioResponsable,
                IdCentroServiciosVenta = comision.IdCentroServicioVenta,
                NombreCentroResponsable = comision.NombreCentroServicioResponsable,
                NombreCentroServiciosVenta = comision.NombreCentroServicioVenta,

                ValorTotal = guias.Sum(g => g.Admision.ValorAdmision + g.Admision.ValorPrimaSeguro + g.Admision.ValorAdicionales),
                TotalImpuestos = guias.Sum(g => g.Admision.ValorTotalImpuestos),
                TotalRetenciones = guias.Sum(g => g.Admision.ValorTotalRetenciones),
                Usuario = ControllerContext.Current.Usuario,
                RegistroVentaFormaPago = guias.FirstOrDefault().Admision.FormasPago.ConvertAll(formaPago => new CARegistroVentaFormaPagoDC
                {
                    Valor = formaPago.Valor,
                    IdFormaPago = formaPago.IdFormaPago,
                    Descripcion = formaPago.Descripcion,
                    NumeroAsociado = formaPago.NumeroAsociadoFormaPago
                })
            };

            return fachadaCajasPos.AdicionarMovimientoCaja(registro, conexion).IdTransaccionCaja;
        }


        private void AdicionarMediosdePago(int idCaja, ICAFachadaCajasPos fachadaCajasPos, long idTransaccionCaja,
                                            List<ADRegistroMediosPagoDC> mediosPago, SqlConnection conexion)
        {

            var medios = mediosPago.Select(mp => new ADRegistroMediosPagoDC
            {
                IdRegTxCaja = idTransaccionCaja,
                IdMedioPago = mp.IdMedioPago,
                CodigoTx = mp.CodigoTx,
                CreadoPor = mp.CreadoPor,
                FechaGrabacion = mp.FechaGrabacion,
                Valor = mp.Valor,
                Id = mp.Id
            }).ToList();

            fachadaCajasPos.AdicionarRegistroMediosPagoCaja(medios, conexion);
        }

        /// <summary>
        /// Retorna el consecutivo del número de factura de venta
        /// </summary>
        /// <returns></returns>
        public SUNumeradorPrefijo ObtenerConsecutivoFacturaVenta()
        {
            // Se obtiene el número de guía
            SUNumeradorPrefijo numeroSuministro = ADRepositorioPos.Instancia.ObtenerNumeroFacturaAutomatica();
            return numeroSuministro;
        }


        /// <summary>
        /// Registra una guía en el sistema
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        private long RegistrarGuia(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, SqlConnection conexion, SqlTransaction transaccion, bool validaIngresoACentroAcopio = false, long? agenciaRegistraAdmision = null)
        {
            string usuario = ControllerContext.Current.Usuario;

            //se eliminan caracteres tales como ", <, > Y SE REEMPLAZAN POR -  
            //Remitente
            remitenteDestinatario.PeatonRemitente.Apellido1 = remitenteDestinatario.PeatonRemitente.Apellido1.Replace("<", "-");
            remitenteDestinatario.PeatonRemitente.Apellido1 = remitenteDestinatario.PeatonRemitente.Apellido1.Replace(">", "-");
            remitenteDestinatario.PeatonRemitente.Apellido1 = remitenteDestinatario.PeatonRemitente.Apellido1.Replace('"', '-');
            remitenteDestinatario.PeatonRemitente.Apellido2 = remitenteDestinatario.PeatonRemitente.Apellido2.Replace("<", "-");
            remitenteDestinatario.PeatonRemitente.Apellido2 = remitenteDestinatario.PeatonRemitente.Apellido2.Replace(">", "-");
            remitenteDestinatario.PeatonRemitente.Apellido2 = remitenteDestinatario.PeatonRemitente.Apellido2.Replace('"', '-');
            remitenteDestinatario.PeatonRemitente.Nombre = remitenteDestinatario.PeatonRemitente.Nombre.Replace("<", "-");
            remitenteDestinatario.PeatonRemitente.Nombre = remitenteDestinatario.PeatonRemitente.Nombre.Replace(">", "-");
            remitenteDestinatario.PeatonRemitente.Nombre = remitenteDestinatario.PeatonRemitente.Nombre.Replace('"', '-');
            remitenteDestinatario.PeatonRemitente.Direccion = remitenteDestinatario.PeatonRemitente.Direccion.Replace("<", "-");
            remitenteDestinatario.PeatonRemitente.Direccion = remitenteDestinatario.PeatonRemitente.Direccion.Replace(">", "-");
            remitenteDestinatario.PeatonRemitente.Direccion = remitenteDestinatario.PeatonRemitente.Direccion.Replace('"', '-');
            remitenteDestinatario.PeatonRemitente.Telefono = remitenteDestinatario.PeatonRemitente.Telefono.Replace("<", "-");
            remitenteDestinatario.PeatonRemitente.Telefono = remitenteDestinatario.PeatonRemitente.Telefono.Replace(">", "-");
            remitenteDestinatario.PeatonRemitente.Telefono = remitenteDestinatario.PeatonRemitente.Telefono.Replace('"', '-');
            //Destinatario
            if (guia.TipoCliente == ADEnumTipoCliente.CPE || guia.TipoCliente == ADEnumTipoCliente.PPE)
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
                        if (remitenteDestinatario.PeatonRemitente.Identificacion != null &&
                            remitenteDestinatario.PeatonRemitente.Identificacion != "0")
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
                        ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                        fachadaClientes.RegistrarClienteContado(remitente,
                            destinario,
                            guia.NombreCentroServicioDestino,
                            guia.IdCentroServicioDestino,
                            usuario);
                    }
                }
                catch (Exception exc)
                {
                    AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.ToString(), COConstantesModulos.MENSAJERIA);
                }
            }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);


            // El campo de la guía "EsAlCobro" se llena a partir de las formas de pago seleccionadas
            ADGuiaFormaPago formaPagoAlcobro = guia.FormasPago.FirstOrDefault(
                            formaPago => formaPago.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO && formaPago.Valor > 0);
            guia.EsAlCobro = (formaPagoAlcobro != null);
            guia.EstaPagada = !guia.EsAlCobro;
            guia.FechaPago = guia.EsAlCobro ? ConstantesFramework.MinDateTimeController : DateTime.Now;

            // se crea validacion para registrar el tiempo de digitalizacion y archivo de la guia
            if (guia.FechaEstimadaDigitalizacion.Year <= 1 && guia.FechaEstimadaArchivo.Year <= 1)
            {
                DateTime fechaActual = DateTime.Now;
                fechaActual = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, 18, 0, 0);
                ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
                TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
                tiempos = tarifas.ObtenerTiempoDigitalizacionArchivo(guia.IdCiudadOrigen, guia.IdCiudadDestino);
                ADValidacionServicioTrayectoDestino validacionTiempoDigitalizacionArchivo = new ADValidacionServicioTrayectoDestino();
                DateTime fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(guia.FechaEstimadaEntrega, ref validacionTiempoDigitalizacionArchivo, tiempos.numeroDiasDigitalizacion, (guia.DiasDeEntrega * 24));
                guia.FechaEstimadaDigitalizacion = fechaActual.AddHours(validacionTiempoDigitalizacionArchivo.NumeroHorasDigitalizacion);
                ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionTiempoDigitalizacionArchivo, tiempos.numeroDiasArchivo, validacionTiempoDigitalizacionArchivo.NumeroHorasDigitalizacion);
                guia.FechaEstimadaArchivo = fechaActual.AddHours(validacionTiempoDigitalizacionArchivo.NumeroHorasArchivo);
            }
            long idAdmisionMensajeria = ADRepositorioPos.Instancia.AdicionarAdmision(guia, remitenteDestinatario, conexion, transaccion);
            guia.IdAdmision = idAdmisionMensajeria;


            //// Adiciona a Movimiento Inventario
            PUMovimientoInventario newMovInv = new PUMovimientoInventario();
            newMovInv.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
            newMovInv.Bodega = new PUCentroServiciosDC() { IdCentroServicio = ControllerContext.Current.IdCentroServicio };
            newMovInv.NumeroGuia = guia.NumeroGuia;
            newMovInv.FechaEstimadaIngreso = DateTime.Now;
            newMovInv.FechaGrabacion = DateTime.Now;
            newMovInv.CreadoPor = ControllerContext.Current.Usuario;

            switch (ControllerContext.Current.IdAplicativoOrigen)
            {
                case 2:  // POS
                    newMovInv.TipoMovimiento = PUEnumTipoMovimientoInventario.Ingreso;
                    /////////////ADRepositorio.Instancia.AdicionarMovimientoInventario(newMovInv, conexion, transaccion);
                    break;
                case 4:  // PAM
                    newMovInv.TipoMovimiento = PUEnumTipoMovimientoInventario.Asignacion;
                    ADRepositorioPos.Instancia.AdicionarMovimientoInventario(newMovInv, conexion, transaccion);
                    break;
                default:
                    break;
            }



            // TODO ID, Se quita la insersion en la tabla AdmisionRotulos_MEN
            if (guia.TotalPiezas > 1)
            {
                ADRepositorioPos.Instancia.AdicionarRotulosAdmision(guia.TotalPiezas, guia.NumeroGuia, idAdmisionMensajeria, conexion, transaccion);
            }

            // Acá se debe validar si se debe generar ingreso a centro de acopio, esto debe aplicar solo para admisión manual COL

            if (validaIngresoACentroAcopio && agenciaRegistraAdmision.HasValue)
            {
                ADRepositorioPos.Instancia.IngresarGuiaManualCentroAcopio(guia, conexion, transaccion);

            }

            if (!string.IsNullOrEmpty(guia.NumeroBolsaSeguridad))
            {
                //GuardarConsumoBolsaSeguridad(guia);
            }

            // Se insertan las formas de pago
            ADRepositorioPos.Instancia.AdicionarGuiaFormasPago(idAdmisionMensajeria, guia.FormasPago, ControllerContext.Current.Usuario, conexion, transaccion);

            // Se insertan los valores adicionales
            ADRepositorioPos.Instancia.AdicionarValoresAdicionales(idAdmisionMensajeria, guia.ValoresAdicionales, ControllerContext.Current.Usuario, conexion, transaccion);

            // Con base en el tipo de cliente se inserta en las tablas relacionadas
            switch (guia.TipoCliente)
            {
                case ADEnumTipoCliente.CCO:

                    // La factura se le debe cargar al remitente
                    remitenteDestinatario.FacturaRemitente = true;
                    ADRepositorioPos.Instancia.AdicionarConvenioConvenio(idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, guia.IdCliente, conexion, transaccion);
                    break;

                case ADEnumTipoCliente.CPE:
                    ADRepositorioPos.Instancia.AdicionarConvenioPeaton(idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, guia.IdCliente, conexion, transaccion);
                    break;

                case ADEnumTipoCliente.PCO:
                    ADRepositorioPos.Instancia.AdicionarPeatonConvenio(idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, conexion, transaccion);
                    break;

                case ADEnumTipoCliente.PPE:
                    ADRepositorioPos.Instancia.AdicionarPeatonPeaton(idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, guia.IdCentroServicioOrigen, guia.NombreCentroServicioOrigen, conexion, transaccion);
                    break;
            }
            return idAdmisionMensajeria;
        }

        /// <summary>
        /// Guarda la guia de mensajería como consumida
        /// </summary>
        /// <param name="guia"></param>
        private void GuardarConsumoGuiaAutomatica(ADGuia guia, SqlConnection conexion, SqlTransaction transaccion)
        {
            SUConsumoSuministroDC consumo = new SUConsumoSuministroDC()
            {
                Cantidad = 1,
                EstadoConsumo = SUEnumEstadoConsumo.CON,
                IdServicioAsociado = guia.IdServicio,
                NumeroSuministro = 0,
                Suministro = SUEnumSuministro.FACTURA_VENTA_MENSAJERIA_AUTOMATICA
            };

            if (guia.IdMensajero > 0)
            {
                consumo.GrupoSuministro = SUEnumGrupoSuministroDC.MEN;
                consumo.IdDuenoSuministro = guia.IdMensajero;
            }
            else
            {
                PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(guia.IdCentroServicioOrigen);
                SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), centroServ.Tipo);

                consumo.GrupoSuministro = grupo;
                consumo.IdDuenoSuministro = guia.IdCentroServicioOrigen;
            }
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            fachadaSuministros.GuardarConsumoSuministro(consumo, conexion, transaccion);
        }

        public DateTime ObtenerHorasDigitalizacionParaGuia(DateTime fechaEntrega, ref ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino, double tiempo, int numeroHoras)
        {
            int numeroHorasNuevo = 0;
            double numHabilesDigitalizacion = 0;
            DateTime FechaDigitalizacion;
            FechaDigitalizacion = PAAdministrador.Instancia.ObtenerFechaFinalHabil(fechaEntrega, tiempo, PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            TimeSpan diferenciaDeFechas = FechaDigitalizacion - fechaEntrega;
            numHabilesDigitalizacion = (diferenciaDeFechas.Days * 24) + diferenciaDeFechas.Hours;
            numeroHorasNuevo = Convert.ToInt32((numHabilesDigitalizacion) + numeroHoras);
            validacionServicioTrayectoDestino.NumeroHorasDigitalizacion = numeroHorasNuevo;
            return FechaDigitalizacion;
        }

        public void ObtenerHorasArchivioParaGuia(DateTime fechaEntrega, ref ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino, double tiempo, int numeroHoras)
        {
            int numeroHorasNuevo = 0;
            int numeroDeSabados = 0;
            double numHabilesDigitalizacion = 0;
            DateTime FechaArchivo;
            FechaArchivo = PAAdministrador.Instancia.ObtenerFechaFinalHabil(fechaEntrega, tiempo, PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            TimeSpan diferenciaDeFechas = FechaArchivo - fechaEntrega;
            numHabilesDigitalizacion = (diferenciaDeFechas.Days * 24) + diferenciaDeFechas.Hours;
            numeroDeSabados = ContadorSabados(fechaEntrega, FechaArchivo);
            numHabilesDigitalizacion = FechaArchivo.DayOfWeek == DayOfWeek.Saturday ? numHabilesDigitalizacion - 0.25 : numHabilesDigitalizacion + (0.5 * numeroDeSabados);           
            numeroHorasNuevo = Convert.ToInt32((numHabilesDigitalizacion) + numeroHoras);
            validacionServicioTrayectoDestino.NumeroHorasArchivo = numeroHorasNuevo;
        }


        /// <summary>
        /// Obtiene los sabados entre una fecha y otra
        /// </summary>
        /// <returns></returns>
        public int ContadorSabados(DateTime fechaInicio, DateTime fechaFin)
        {
            int cuentaSabados = 0;
            fechaInicio = new DateTime(fechaInicio.Year, fechaInicio.Month, fechaInicio.Day);
            fechaFin = new DateTime(fechaFin.Year, fechaFin.Month, fechaFin.Day);
            while (fechaInicio <= fechaFin)
            {
                if (fechaInicio.DayOfWeek == 0)
                {
                    cuentaSabados++;
                }
                fechaInicio = fechaInicio.AddDays(1);

            }
            return cuentaSabados;
        }

        /// <summary>
        /// Audita todas las admisiones automaticas generadas
        /// </summary>
        public void GuardarAuditoriaGrabacionAdmisionMensajeria(int idCaja, string metodoEjecutado, List<ADRetornoAdmision> retorno, List<ADRegistroAdmisiones> guiaEntrada, ADMensajeriaTipoCliente tipoCliente, object objetoAdicional = null)
        {
            string usuario = ControllerContext.Current != null ? ControllerContext.Current.Usuario : "NoUsuario";

            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
                {

                    try
                    {

                        string retornoSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(retorno);

                        string guiaEntradaSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(guiaEntrada);

                        string tipoClienteSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(tipoCliente);
                        string objetoAdicionalSerializado = null;

                        if (objetoAdicional != null)
                        {

                            switch (objetoAdicional.GetType().ToString())
                            {
                                case "CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.ADNotificacion":

                                    objetoAdicionalSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(objetoAdicional as ADNotificacion);
                                    break;

                                case "CO.Servidor.Servicios.ContratoDatos.Tarifas.TATipoEmpaque":
                                    objetoAdicionalSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(objetoAdicional as TATipoEmpaque);
                                    break;

                                case "CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.ADRapiEnvioContraPagoDC":
                                    objetoAdicionalSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(objetoAdicional as ADRapiEnvioContraPagoDC);
                                    break;

                                case "CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.ADRapiRadicado":
                                    objetoAdicionalSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(objetoAdicional as ADRapiRadicado);
                                    break;
                            }

                        }

                        ADRepositorioPos.Instancia.GuardarAuditoriaGrabacionAdmisionMensajeria(idCaja, metodoEjecutado, retornoSerializado, guiaEntradaSerializado, tipoClienteSerializado, usuario, objetoAdicionalSerializado);
                    }
                    catch
                    {
                        //se deja en blanco ya que la auditoria no debe interferir con el flujo de la admision
                    }
                });
            t.Start();
        }


        public List<ADResultadoAdmision> RegistrarGuiasMedioPago(List<ADRegistroAdmisiones> listaGuia, int idCaja, List<ADRegistroMediosPagoDC> mediosPago)
        {
            /// registrar la informacionde totales de caja 
            /// Registrar la informacion medios de pago
            /// procesar cada una de las guias
            /// 

            List<ADResultadoAdmision> resultados = null;

            using (TransactionScope transaccionLista = new TransactionScope())
            {

                // La primera validación que se debe hacer, es verificar que la caja esté abierta
                ICAFachadaCajasPos fachadaCajasPos = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajasPos>();
                CARegistroTransacCajaDC registroOrigen = null;
                SqlConnection conexionLista = new SqlConnection(conexionStringController);
                conexionLista.Open();

                // Registrar RTC                
                long idTransaccionCaja = AdicionarRegistroTransaccionCajaConMediosPago(listaGuia, idCaja, fachadaCajasPos, mediosPago, conexionLista, out registroOrigen);

                //Auditar transaccion
                ADRepositorioPos.Instancia.AuditarGuiaGeneradaArchivoTextoMediosPago(
                                        null, idCaja, listaGuia.FirstOrDefault().DestinatarioRemitente, idTransaccionCaja, mediosPago);

                // Registrar MediosPago
                AdicionarMediosdePago(idCaja, fachadaCajasPos, idTransaccionCaja, mediosPago, conexionLista);

                foreach (ADRegistroAdmisiones guia in listaGuia)
                {

                    SUNumeradorPrefijo numeroSuministro = ObtenerConsecutivoFacturaVenta();
                    guia.Admision.NumeroGuia = numeroSuministro.ValorActual;
                    guia.Admision.PrefijoNumeroGuia = numeroSuministro.Prefijo;

                    ADRepositorioPos.Instancia.AuditarGuiaGeneradaArchivoTextoMediosPago(
                                        guia.Admision, idCaja, guia.DestinatarioRemitente, idTransaccionCaja, guia.Notificacion);

                    SqlConnection conexion = new SqlConnection(conexionStringController);
                    SqlTransaction transaccion = null;
                    ADResultadoAdmision resultado;

                    try
                    {
                        conexion.Open();
                        transaccion = conexion.BeginTransaction();
                        // Se adiciona la admisión
                        guia.Admision.EstadoGuia = ADEnumEstadoGuia.Admitida;
                        guia.Admision.FechaAdmision = DateTime.Now;
                        // INCIO registro transacciones segun el servicio


                        switch (guia.IdServicio)
                        {
                            case ConstantesServicios.SERVICIO_NOTIFICACIONES:
                                resultado = RegistrarGuiaAutomaticaNotificacionMediosPago(
                                            guia.Admision,
                                            idCaja,
                                            guia.DestinatarioRemitente,
                                            guia.Notificacion,
                                            conexion,
                                            transaccion);
                                //guiaAdmitida = proxyMensajeria.RegistrarGuiaAutomaticaNotificacionMedioPago(ListaGuias, AplicacionInfo.Caja, destinatarioRemitente, notifica);
                                //guiaAdmitida = proxyMensajeria.RegistrarGuiaAutomaticaNotificacion(guia, AplicacionInfo.Caja, destinatarioRemitente, notifica);
                                break;

                            case ConstantesServicios.SERVICIO_INTERNACIONAL:
                                resultado = RegistrarGuiaAutomaticaInternacional(
                                                guia.Admision,
                                                idCaja,
                                                guia.DestinatarioRemitente,
                                                guia.TipoEmpaque,
                                                conexion,
                                                transaccion);
                                break;

                            case ConstantesServicios.SERVICIO_RAPIRADICADO:

                                resultado = RegistrarGuiaAutomaticaRapiRadicado(
                                                    guia.Admision,
                                                    idCaja,
                                                    guia.DestinatarioRemitente,
                                                    guia.RapiRadicado,
                                                    conexion,
                                                    transaccion);
                                break;

                            default:
                                resultado = RegistrarGuiaAutomatica(guia.Admision, idCaja, guia.DestinatarioRemitente,
                                                    conexion,
                                                    transaccion);
                                break;
                        }
                        resultado.GuidDeChequeo = guia.Admision.GuidDeChequeo;
                        transaccion.Commit();
                        conexion.Close();

                        if (resultados == null)
                        {
                            resultados = new List<ADResultadoAdmision>();
                        }

                        resultados.Add(resultado);

                    }
                    catch
                    {
                        if (transaccion != null)
                            transaccion.Rollback();
                        ADRepositorioPos.Instancia.GuardarNumeroFacturaFallido(guia.Admision.NumeroGuia, guia.Admision.PrefijoNumeroGuia);
                        throw;
                    }
                    finally
                    {
                        if (conexion.State != ConnectionState.Closed)
                            conexion.Close();
                        conexion.Dispose();
                    }
                }

                conexionLista.Close();
                transaccionLista.Complete();
            }

            return resultados;

        }


        /// <summary>
        /// Registra guia cuyo servicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        internal ADResultadoAdmision RegistrarGuiaAutomaticaNotificacionMediosPago(
                                            ADGuia guia,
                                            int idCaja,
                                            ADMensajeriaTipoCliente remitenteDestinatario,
                                            ADNotificacion notificacion,
                                            SqlConnection conexion,
                                            SqlTransaction transaccion)
        {


            try
            {

                // Se adiciona la admisión                
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, conexion, transaccion);
                ADAdmisionNotificacionPos.Instancia.AdicionarNotificacion(idAdmisionMensajeria, notificacion, conexion, transaccion);
                GuardarConsumoGuiaAutomatica(guia, conexion, transaccion);
                //IntegrarConMensajero(guia, remitenteDestinatario, notificacion);
                ADResultadoAdmision resultado = new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia, IdAdmision = guia.IdAdmision, };

                // Obtener información agencia de ciudad de origen y ciudad de destino, esto es informativo
                // TODO: RON Implementar
                try
                {
                    var agenciaOrigen = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadOrigen);
                    if (agenciaOrigen != null && agenciaOrigen.IdMunicipio == guia.IdCiudadOrigen)
                    {
                        resultado.DireccionAgenciaCiudadOrigen = "Oficina " + guia.NombreCiudadOrigen.Split('\\')[0] + ": " + agenciaOrigen.Direccion;
                    }
                    var agenciaDestino = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadDestino);
                    if (agenciaDestino != null && agenciaDestino.IdMunicipio == guia.IdCiudadDestino)
                    {
                        resultado.DireccionAgenciaCiudadDestino = "Oficina " + guia.NombreCiudadDestino.Split('\\')[0] + ": " + agenciaDestino.Direccion;
                    }

                }
                catch
                {
                    resultado.DireccionAgenciaCiudadOrigen = "R";
                    resultado.DireccionAgenciaCiudadDestino = "C";
                }

                return resultado;

            }
            catch
            {
                if (transaccion != null)
                    transaccion.Rollback();
                ADRepositorioPos.Instancia.GuardarNumeroFacturaFallido(guia.NumeroGuia, guia.PrefijoNumeroGuia);
                throw;
            }
        }

        /// <summary>
        /// Guarda la guia de mensajería como consumida
        /// </summary>
        /// <param name="guia"></param>
        private void GuardarConsumoGuiaAutomatica(ADGuia guia)
        {
            SUConsumoSuministroDC consumo = new SUConsumoSuministroDC()
            {
                Cantidad = 1,
                EstadoConsumo = SUEnumEstadoConsumo.CON,
                IdServicioAsociado = guia.IdServicio,
                NumeroSuministro = 0,
                Suministro = SUEnumSuministro.FACTURA_VENTA_MENSAJERIA_AUTOMATICA
            };

            if (guia.IdMensajero > 0)
            {
                consumo.GrupoSuministro = SUEnumGrupoSuministroDC.MEN;
                consumo.IdDuenoSuministro = guia.IdMensajero;
            }
            else
            {
                PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(guia.IdCentroServicioOrigen);
                SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), centroServ.Tipo);

                consumo.GrupoSuministro = grupo;
                consumo.IdDuenoSuministro = guia.IdCentroServicioOrigen;
            }
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            fachadaSuministros.GuardarConsumoSuministro(consumo);
        }

        /// <summary>
        /// Registra admisión internacional automática
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="tipoEmpaque"></param>
        /// <returns></returns>
        internal ADResultadoAdmision RegistrarGuiaAutomaticaInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, SqlConnection conexion, SqlTransaction transaccion)
        {
            // Se adiciona la admisión
            long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, conexion, transaccion);
            ADAdmisionInternacionalPos.Instancia.AdicionarAdmisionTipoEmpaque(idAdmisionMensajeria, tipoEmpaque, ControllerContext.Current.Usuario);
            GuardarConsumoGuiaAutomatica(guia);
            IntegrarConMensajero(guia, remitenteDestinatario);

            return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
        }

        /// <summary>
        /// Verifica si se debe hacer integración y si es el caso realiza la integración
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="remitenteDestinatario"></param>
        private void IntegrarConMensajero(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            bool integraConMensajero = false;

            // Se obtiene el parámetro que indica si se debe hacer integración con mensajero
            if (Cache.Instancia.ContainsKey(ConstantesFramework.CACHE_INTEGRA_CON_MENSAJERO))
            {
                bool.TryParse(Cache.Instancia[ConstantesFramework.CACHE_INTEGRA_CON_MENSAJERO].ToString(), out integraConMensajero);
            }
            else
            {
                // Si no existe en caché, consultarlo en la base de datos y agregarlo al caché
                integraConMensajero = ADRepositorioPos.Instancia.ObtenerParametroIntegraConMensajero();
                Cache.Instancia.Add(ConstantesFramework.CACHE_INTEGRA_CON_MENSAJERO, integraConMensajero);
            }

            if (integraConMensajero)
            {
                Integraciones.Mensajero.Entidades.Guia guiaMensajero = new Integraciones.Mensajero.Entidades.Guia()
                {
                    Alto = (short)guia.Alto,
                    Ancho = (short)guia.Ancho,
                    CiudadDestinoId = guia.IdCiudadDestino,
                    CiudadOrigenId = guia.IdCiudadOrigen,
                    CodigoAgenciaGuias = guia.IdCentroServicioOrigen.ToString(), // El poseedor de la guía es el centro de servicio de origen porque es automática y para cliente contado
                    DiceContener = guia.DiceContener,
                    EsPesoVolumetrico = guia.EsPesoVolumetrico,
                    FechaEnvio = guia.FechaAdmision,
                    FechaImpresion = guia.FechaAdmision,
                    FormaPago = (byte)guia.FormasPago.First().IdFormaPago,
                    IdTipoEntrega = guia.IdTipoEntrega,
                    Largo = (short)guia.Largo,
                    NumeroGuia = guia.NumeroGuia.ToString(),
                    Observaciones = guia.Observaciones,
                    PesoLiqMasa = (int)guia.PesoLiqMasa,
                    PesoLiqVolumetrico = (int)guia.PesoLiqVolumetrico,
                    TipoEnvio = guia.IdTipoEnvio,
                    TipoServicio = guia.IdServicio,
                    TipoTarifa = Integraciones.Mensajero.Entidades.EnumTipoTarifa.Automatica,
                    TotalPiezas = guia.TotalPiezas,
                    Usuario = ControllerContext.Current.Usuario,
                    ValorDeclarado = guia.ValorDeclarado,
                    ValorEmpaque = (int)guia.ValorEmpaque,
                    ValorPrimaSeguro = (int)guia.ValorPrimaSeguro,
                    ValorTransporte = (int)guia.ValorAdmision,
                    ValorTotal = (int)guia.ValorTotal
                };
                ADGuiaFormaPago prepago = guia.FormasPago.FirstOrDefault(g => g.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_PREPAGO);
                if (prepago != null)
                {
                    guiaMensajero.NumeroPrepago = prepago.NumeroAsociadoFormaPago;
                }
                switch (guia.TipoCliente)
                {
                    case ADEnumTipoCliente.PCO:
                        if (remitenteDestinatario.PeatonRemitente != null)
                        {
                            guiaMensajero.RemitenteIdentificacion = remitenteDestinatario.PeatonRemitente.Identificacion;
                            guiaMensajero.RemitenteNombre = string.Join(" ", remitenteDestinatario.PeatonRemitente.Nombre, remitenteDestinatario.PeatonRemitente.Apellido1, remitenteDestinatario.PeatonRemitente.Apellido2);
                            guiaMensajero.RemitenteTelefono = remitenteDestinatario.PeatonRemitente.Telefono;
                            guiaMensajero.RemitenteDireccion = remitenteDestinatario.PeatonRemitente.Direccion;
                        }

                        if (remitenteDestinatario.ConvenioDestinatario != null)
                        {
                            guiaMensajero.DestinatarioNombre = remitenteDestinatario.ConvenioDestinatario.RazonSocial;
                            guiaMensajero.DestinatarioTelefono = remitenteDestinatario.ConvenioDestinatario.Telefono;
                            guiaMensajero.DestinatarioDireccion = remitenteDestinatario.ConvenioDestinatario.Direccion;
                        }
                        break;

                    case ADEnumTipoCliente.PPE:
                        if (remitenteDestinatario.PeatonRemitente != null)
                        {
                            guiaMensajero.RemitenteIdentificacion = remitenteDestinatario.PeatonRemitente.Identificacion;
                            guiaMensajero.RemitenteNombre = string.Join(" ", remitenteDestinatario.PeatonRemitente.Nombre, remitenteDestinatario.PeatonRemitente.Apellido1, remitenteDestinatario.PeatonRemitente.Apellido2);
                            guiaMensajero.RemitenteTelefono = remitenteDestinatario.PeatonRemitente.Telefono;
                            guiaMensajero.RemitenteDireccion = remitenteDestinatario.PeatonRemitente.Direccion;
                        }
                        if (remitenteDestinatario.PeatonDestinatario != null)
                        {
                            guiaMensajero.DestinatarioNombre = string.Join(" ", remitenteDestinatario.PeatonDestinatario.Nombre, remitenteDestinatario.PeatonDestinatario.Apellido1, remitenteDestinatario.PeatonDestinatario.Apellido2);
                            guiaMensajero.DestinatarioDireccion = remitenteDestinatario.PeatonDestinatario.Direccion;
                            guiaMensajero.DestinatarioTelefono = remitenteDestinatario.PeatonDestinatario.Telefono;
                        }
                        break;
                }

                try
                {
                    if (!Integraciones.Mensajero.IntegracionMensajero.Instancia.GrabarAdmision(guiaMensajero))
                    {
                        // No se pudo admiitr la guía en mensajero, por tanto se debe cancelar la transacción
                        throw new FaultException<ControllerException>(
                            new ControllerException
                            (
                                COConstantesModulos.MENSAJERIA,
                                ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO.ToString(),
                                ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO))
                            );
                    }
                }
                catch (Exception exc)
                {
                    throw new FaultException<ControllerException>(
                            new ControllerException
                                (
                                    COConstantesModulos.MENSAJERIA,
                                    ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO.ToString(), exc.Message)
                                );
                }
            }
        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        internal ADResultadoAdmision RegistrarGuiaAutomaticaRapiRadicado(
            ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado,
            SqlConnection conexion, SqlTransaction transaccion)
        {
            try
            {
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, conexion, transaccion);
                GuardarConsumoGuiaAutomatica(guia, conexion, transaccion);

                // Se adiciona el rapiradicado
                ADRepositorioPos.Instancia.AdicionarRapiRadicado(idAdmisionMensajeria, rapiRadicado, ControllerContext.Current.Usuario, conexion, transaccion);

                //IntegrarConMensajero(guia, remitenteDestinatario);
                ADResultadoAdmision resultado = new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia, IdAdmision = guia.IdAdmision, };

                // Obtener información agencia de ciudad de origen y ciudad de destino, esto es informativo
                // TODO: RON Implementar
                try
                {
                    var agenciaOrigen = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadOrigen);
                    if (agenciaOrigen != null && agenciaOrigen.IdMunicipio == guia.IdCiudadOrigen)
                    {
                        resultado.DireccionAgenciaCiudadOrigen = "Oficina " + guia.NombreCiudadOrigen.Split('\\')[0] + ": " + agenciaOrigen.Direccion;
                    }
                    var agenciaDestino = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadDestino);
                    if (agenciaDestino != null && agenciaDestino.IdMunicipio == guia.IdCiudadDestino)
                    {
                        resultado.DireccionAgenciaCiudadDestino = "Oficina " + guia.NombreCiudadDestino.Split('\\')[0] + ": " + agenciaDestino.Direccion;
                    }
                }
                catch
                {
                    resultado.DireccionAgenciaCiudadOrigen = "R";
                    resultado.DireccionAgenciaCiudadDestino = "C";
                }

                return resultado;
            }
            catch
            {
                if (transaccion != null)
                    transaccion.Rollback();
                ADRepositorioPos.Instancia.GuardarNumeroFacturaFallido(guia.NumeroGuia, guia.PrefijoNumeroGuia);
                throw;
            }

        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        internal ADResultadoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario,
                                                        SqlConnection conexion, SqlTransaction transaccion)
        {
            ADResultadoAdmision resultado = null;

            try
            {

                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, conexion, transaccion, false, null);

                guia.IdAdmision = idAdmisionMensajeria;
                GuardarConsumoGuiaAutomatica(guia, conexion, transaccion);
                //IntegrarConMensajero(guia, remitenteDestinatario);
                resultado = new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia, IdAdmision = guia.IdAdmision, };

                // Obtener información agencia de ciudad de origen y ciudad de destino, esto es informativo
                // TODO: RON Implementar
                try
                {
                    var agenciaOrigen = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadOrigen);
                    if (agenciaOrigen != null && agenciaOrigen.IdMunicipio == guia.IdCiudadOrigen)
                    {
                        resultado.DireccionAgenciaCiudadOrigen = "Oficina " + guia.NombreCiudadOrigen.Split('\\')[0] + ": " + agenciaOrigen.Direccion;
                    }
                    var agenciaDestino = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadDestino);
                    if (agenciaDestino != null && agenciaDestino.IdMunicipio == guia.IdCiudadDestino)
                    {
                        resultado.DireccionAgenciaCiudadDestino = "Oficina " + guia.NombreCiudadDestino.Split('\\')[0] + ": " + agenciaDestino.Direccion;
                    }
                }
                catch
                {
                    resultado.DireccionAgenciaCiudadOrigen = "R";
                    resultado.DireccionAgenciaCiudadDestino = "C";
                }

            }
            catch
            {
                if (transaccion != null)
                    transaccion.Rollback();
                ADRepositorioPos.Instancia.GuardarNumeroFacturaFallido(guia.NumeroGuia, guia.PrefijoNumeroGuia);
                throw;
            }
            //TODO:CED solo se debe utilizar para las pruebas de carga
            ADRepositorioPos.Instancia.ConfirmarAuditoriaNumeroGuiaGenerado(guia.NumeroGuia);

            return resultado;
        }

        #endregion
    }
}
