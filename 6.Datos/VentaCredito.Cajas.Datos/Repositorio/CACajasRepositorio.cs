using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Framework.Servidor.Excepciones;
using Servicio.Entidades.Cajas;
using VentaCredito.Cajas.Comun;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Framework.Servidor.Comun;
using VentaCredito.Transversal;

namespace VentaCredito.Cajas.Datos.Repositorio
{
    public class CACajasRepositorio
    {

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;


        public static CACajasRepositorio instancia = new CACajasRepositorio();
        public static CACajasRepositorio Instancia
        {
            get
            {
                return instancia;
            }
        }

        public List<CAConceptoCajaDC> ListaConceptos { get; set; }

        public string ObtenerParametroCajas(string idParametro)
        {
            string rst = string.Empty;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametroCajasPorId_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParametro", idParametro);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    rst = reader["PAC_ValorParametro"].ToString();
                }

            }
            return rst;
        }


        /// <summary>
        /// Insercion de Registro Transaccion caja y de Dtlle
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="idAperturaCaja">The id apertura caja.</param>
        /// <param name="IdRegistros">The id registros.</param>
        /// <param name="contexto">The contexto.</param>
        /// <returns></returns>
        public long InsertarRegistroTransaccion(CARegistroTransacCajaDC movimientoCaja, long idAperturaCaja, CAIdTransaccionesCajaDC IdRegistros)
        {
            long idRegistroCaja;
            long? idRegistroDet;

            //se hace la insercion en el registro transaccional de la Caja y se toma el Id

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paCrearRegistroTransCaja_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RTC_Usuario", movimientoCaja.Usuario);
                cmd.Parameters.AddWithValue("@RTC_IdAperturaCaja", idAperturaCaja);
                cmd.Parameters.AddWithValue("@RTC_IdCentroServiciosVenta", movimientoCaja.IdCentroServiciosVenta);
                cmd.Parameters.AddWithValue("@RTC_NombreCentroServiciosVenta", movimientoCaja.NombreCentroServiciosVenta);
                cmd.Parameters.AddWithValue("@RTC_IdCentroServiciosResponsable", movimientoCaja.IdCentroResponsable);
                cmd.Parameters.AddWithValue("@RTC_NombreCtroSvcsResponsable", movimientoCaja.NombreCentroResponsable);
                cmd.Parameters.AddWithValue("@RTC_ValorTotal", movimientoCaja.ValorTotal);
                cmd.Parameters.AddWithValue("@RTC_TotalImpuestos", movimientoCaja.TotalImpuestos);
                cmd.Parameters.AddWithValue("@RTC_TotalRetenciones", movimientoCaja.TotalRetenciones);
                cmd.Parameters.AddWithValue("@RTC_TipoDatosAdicionales", movimientoCaja.TipoDatosAdicionales.ToString());
                cmd.Parameters.AddWithValue("@RTC_CreadoPor", movimientoCaja.Usuario);
                cmd.Parameters.AddWithValue("@RTC_FechaGrabacion", DateTime.Now);
                conn.Open();
                idRegistroCaja = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();
            }

            //clase para el retorno
            IdRegistros.IdTransaccionCaja = idRegistroCaja;

            movimientoCaja.RegistrosTransacDetallesCaja
              .ToList()
              .ForEach(AddDtll =>
              {
                  //Dependiendo del tipo de Cliente se llenan unos campos
                  if (movimientoCaja.TipoDatosAdicionales == CAEnumTipoDatosAdicionales.CRE)
                  {
                      //AddDtll.NumeroFactura = CAConstantesCaja.VALOR_VACIO;
                      AddDtll.EstadoFacturacion = CAEnumEstadoFacturacion.PED;
                      AddDtll.FechaFacturacion = ConstantesFramework.MinDateTimeController;
                  }
                  else
                  {
                      //Rafram 20121231 se deja el valor que traiga por defecto el campo numero Factura
                      //AddDtll.NumeroFactura = AddDtll.Numero.ToString();
                      AddDtll.EstadoFacturacion = CAEnumEstadoFacturacion.FAC;
                      AddDtll.FechaFacturacion = DateTime.Now;
                  }
                  decimal valorEfectivo = movimientoCaja.RegistroVentaFormaPago.Where(fp => fp.IdFormaPago == CAConstantesCaja.FORMA_PAGO_EFECTIVO).Sum(val => val.Valor);

                  using (SqlConnection conn = new SqlConnection(conexionStringController))
                  {
                      SqlCommand cmd = new SqlCommand("paCrearRegTransCajaDetalle_CAJ", conn);
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.AddWithValue("@idRegistroTranscaccion", idRegistroCaja);
                      cmd.Parameters.AddWithValue("@idConceptoCaja", AddDtll.ConceptoCaja.IdConceptoCaja);
                      cmd.Parameters.AddWithValue("@nombreConcepto", AddDtll.ConceptoCaja.Nombre);
                      cmd.Parameters.AddWithValue("@conceptoEsIngreso", AddDtll.ConceptoEsIngreso);
                      cmd.Parameters.AddWithValue("@cantidad", AddDtll.Cantidad);
                      cmd.Parameters.AddWithValue("@valorServicio", AddDtll.ValorServicio);
                      cmd.Parameters.AddWithValue("@valoresAdicionales", AddDtll.ValoresAdicionales);
                      cmd.Parameters.AddWithValue("@valorTercero", AddDtll.ValorTercero);
                      cmd.Parameters.AddWithValue("@valorImpuestos", AddDtll.ValorImpuestos);
                      cmd.Parameters.AddWithValue("@valorRetenciones", AddDtll.ValorRetenciones);
                      cmd.Parameters.AddWithValue("@valorPrimaSeguros", AddDtll.ValorPrimaSeguros);
                      cmd.Parameters.AddWithValue("@valorDeclarado", AddDtll.ValorDeclarado);
                      cmd.Parameters.AddWithValue("@numero", AddDtll.Numero);
                      cmd.Parameters.AddWithValue("@idCuentaExterna", Convert.ToInt16(CAConstantesCaja.VALOR_VACIO));
                      cmd.Parameters.AddWithValue("@numeroFactura", AddDtll.NumeroFactura);
                      cmd.Parameters.AddWithValue("@estadoFacturacion", AddDtll.EstadoFacturacion.ToString());
                      cmd.Parameters.AddWithValue("@fechaFacturacion", AddDtll.FechaFacturacion);
                      cmd.Parameters.AddWithValue("@observacion", AddDtll.Observacion == null ? string.Empty : AddDtll.Observacion);
                      cmd.Parameters.AddWithValue("@descripcion", AddDtll.Descripcion == null ? string.Empty : AddDtll.Descripcion);
                      cmd.Parameters.AddWithValue("@numeroComprobante", AddDtll.NumeroComprobante == null ? string.Empty : AddDtll.NumeroComprobante);
                      cmd.Parameters.AddWithValue("@creadoPor", movimientoCaja.Usuario);
                      cmd.Parameters.AddWithValue("@minDate", ConstantesFramework.MinDateTimeController);
                      cmd.Parameters.AddWithValue("@CentroServicios", movimientoCaja.IdCentroServiciosVenta);
                      cmd.Parameters.AddWithValue("@ValorEfectivo", valorEfectivo);
                      cmd.Parameters.AddWithValue("@RTC_FechaGrabacion", DateTime.Now);
                      conn.Open();
                      idRegistroDet = Convert.ToInt64(cmd.ExecuteScalar());
                      conn.Close();
                  }

                  //Inserto registro de Impuestos
                  InsertarRegistroImpuesto(movimientoCaja, AddDtll, idRegistroDet);

                  //Inserto las Transacciones Adicionales de Operaciones Caja
                  AdicionarDetallesAdicionales(idRegistroDet.Value, AddDtll.DetalleAdicional);

                  //Agrego los datos de id transaccion detalle transaccion
                  if (idRegistroDet != null)
                  {
                      IdRegistros.IdTransaccionCajaDtll = new List<long>();
                      IdRegistros.IdTransaccionCajaDtll.Add(idRegistroDet.HasValue ? idRegistroDet.Value : 0);
                  }
              });
            return idRegistroCaja;
        }





        /// <summary>
        ///Se registra la transaccion adicional por una operacion de caja ppal
        /// </summary>
        /// <param name="idDetalle">id de la tranasaccion del detalle</param>
        /// <param name="detalle">detalle de la transaccion</param>
        public void AdicionarDetallesAdicionales(long idDetalle, CARegistroTransacCajaDetallAdicionalDC detalle)
        {
            if (detalle != null && detalle.Adicional01 != null)
            {
                using (SqlConnection conn = new SqlConnection(conexionStringController))
                {
                    SqlCommand cmd = new SqlCommand("paCrearRegTransCajaDetlleAdicional_CAJ", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idRegistroDetalleTranscaccion", idDetalle);
                    cmd.Parameters.AddWithValue("@idSucursal", detalle.IdSucursal.Value);
                    cmd.Parameters.AddWithValue("@Adicional1", detalle.Adicional01);
                    cmd.Parameters.AddWithValue("@Adicional2", detalle.Adicional02);
                    cmd.Parameters.AddWithValue("@Adicional3", detalle.Adicional03);
                    cmd.Parameters.AddWithValue("@creadoPor", ContextoSitio.Current.Usuario);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Inserta los registros de los Impuestos.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="contexto">The contexto.</param>
        /// <param name="AddDtll">The add DTLL.</param>
        /// <param name="idRegistroDet">The id registro det.</param>
        public void InsertarRegistroImpuesto(CARegistroTransacCajaDC movimientoCaja, CARegistroTransacCajaDetalleDC AddDtll, long? idRegistroDet)
        {
            if (AddDtll.LtsImpuestos != null)
            {
                AddDtll.LtsImpuestos.ForEach(Imp =>
                {
                    using (SqlConnection conn = new SqlConnection(conexionStringController))
                    {
                        SqlCommand cmd = new SqlCommand("paCrearRegTransImpuestos_CAJ", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RTI_IdRegTransDtllCaja", idRegistroDet);
                        cmd.Parameters.AddWithValue("@RTI_IdImpuesto", Imp.InfoImpuesto.IdImpuesto);
                        cmd.Parameters.AddWithValue("@RTI_DescripcionImpuesto", Imp.InfoImpuesto.DescripcionImpuesto);
                        cmd.Parameters.AddWithValue("@RTI_ValorConfigurado", Imp.InfoImpuesto.ValorImpuesto);
                        cmd.Parameters.AddWithValue("@RTI_IdCuentaExterna", ObtenerConceptoPorId(AddDtll.ConceptoCaja.IdConceptoCaja).IdCuentaExterna);
                        cmd.Parameters.AddWithValue("@RTI_Valor", Imp.ValorImpuestoLiquidado);
                        cmd.Parameters.AddWithValue("@RTI_CreadoPor", movimientoCaja.Usuario);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                });
            }
        }

        /// <summary>
        /// Obtiene el concepto de Caja por id.
        /// </summary>
        /// <param name="idConcepto">Identifcador del concepto de caja.</param>
        /// <returns>Objeto con la información del concepto de caja</returns>
        public CAConceptoCajaDC ObtenerConceptoPorId(int idConcepto)
        {
            CAConceptoCajaDC conceptoCaja = null;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerConceptoCaja_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idConcepto", idConcepto);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                var concepto = dt.AsEnumerable().FirstOrDefault();
                if (concepto != null)
                {
                    conceptoCaja = new CAConceptoCajaDC()
                    {
                        IdConceptoCaja = concepto.Field<int>("COC_IdConceptoCaja"),
                        Nombre = concepto.Field<string>("COC_Nombre"),
                        EsIngreso = concepto.Field<bool>("COC_EsIngreso"),
                        IdCuentaExterna = concepto.Field<short?>("COC_IdCuentaExterna")
                    };
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA,
                         CAEnumTipoErrorCaja.ERROR_NO_CONCEPTO_CAJA.ToString(),
                         String.Format(CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_NO_CONCEPTO_CAJA), idConcepto));

                    throw new FaultException<ControllerException>(excepcion);
                }
                return conceptoCaja;
            }
        }

        /// <summary>
        /// Se Registran las Formas de Pago
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="idRegistroCaja">The id registro caja.</param>
        /// <param name="contexto">The contexto.</param>
        public void InsertarFormasDePago(CARegistroTransacCajaDC movimientoCaja, long idRegistroCaja, bool anulacionGuia)
        {
            //se recorren las formas de pago y se insertan las que tienen valor>0
            movimientoCaja.RegistroVentaFormaPago.ToList().ForEach(AddFormaPag =>
            {
                if (AddFormaPag.Valor > 0)
                {
                    if (!anulacionGuia)
                    {
                        //Actualiza el Valor de pin Prepago
                        if (AddFormaPag.IdFormaPago == CAConstantesCaja.FORMA_PAGO_PREPAGO)
                        {
                            if (!String.IsNullOrWhiteSpace(AddFormaPag.NumeroAsociado))
                            {
                                ActualizarSaldoPinPrepago(Convert.ToInt64(AddFormaPag.NumeroAsociado), AddFormaPag.Valor);
                            }
                        }
                    }

                    //Diego informa: que incluso siendo una anulacion de guia, se debe insertar en la tabla de formas de pago
                    //Se ejecuta el Procedimiento Almacenado de las formas de Pago
                    using (SqlConnection conn = new SqlConnection(conexionStringController))
                    {
                        SqlCommand cmd = new SqlCommand("paCrearRegTransFormasPago_CAJ", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idRegistroVenta", idRegistroCaja);
                        cmd.Parameters.AddWithValue("@idFormaPago", AddFormaPag.IdFormaPago);
                        cmd.Parameters.AddWithValue("@descripcionFormaPago", AddFormaPag.Descripcion);
                        cmd.Parameters.AddWithValue("@valor", AddFormaPag.Valor);
                        cmd.Parameters.AddWithValue("@numeroAsociado", AddFormaPag.NumeroAsociado);
                        cmd.Parameters.AddWithValue("@campo1", AddFormaPag.Campo01);
                        cmd.Parameters.AddWithValue("@campo2", AddFormaPag.Campo02);
                        cmd.Parameters.AddWithValue("@creadoPor", movimientoCaja.Usuario);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        /// <summary>
        /// Retorna el valor de una anulacion a un prin prepago
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        public void RetornarSaldoPinPrepago(CARegistroTransacCajaDC movimientoCaja)
        {
            movimientoCaja.RegistroVentaFormaPago.ToList().ForEach(AddFormaPag =>
            {
                if (AddFormaPag.IdFormaPago == CAConstantesCaja.FORMA_PAGO_PREPAGO && AddFormaPag.Valor != 0)
                {
                    if (!String.IsNullOrWhiteSpace(AddFormaPag.NumeroAsociado))
                    {
                        long numeroPin = Convert.ToInt64(AddFormaPag.NumeroAsociado);

                        using (SqlConnection conn = new SqlConnection(conexionStringController))
                        {
                            SqlCommand cmd = new SqlCommand("paObtenerPinPrePago_CAJ", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@PinPrePago", numeroPin);
                            SqlDataAdapter ta = new SqlDataAdapter();
                            DataTable dt = new DataTable();
                            ta.Fill(dt);

                            var pinPrepagoRes = dt.AsEnumerable().FirstOrDefault();

                            if (pinPrepagoRes != null)
                            {
                                decimal pinValorFinal = pinPrepagoRes.Field<decimal>("PIP_Saldo");
                                pinValorFinal = pinPrepagoRes.Field<decimal>("PIP_Saldo") + AddFormaPag.Valor;
                            }
                            else
                            {
                                ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                      .ERROR_PINPREPAGO_SIN_NUMERO_ASOCIADO.ToString(), CACajaServerMensajes
                                                                      .CargarMensaje(CAEnumTipoErrorCaja.ERROR_PINPREPAGO_SIN_NUMERO_ASOCIADO));
                                throw new FaultException<ControllerException>(excepcion);
                            }

                        }
                    }
                }
            });
        }



        /// <summary>
        /// Actualizar el Saldo del Pin Prepago.
        /// </summary>
        /// <param name="idPinPrepago">Es el id del pin prepago.</param>
        /// <param name="valorCompraPinPrepago">Es el valor de la compra del pin prepago.</param>
        public void ActualizarSaldoPinPrepago(long idPinPrepago, decimal valorCompraPinPrepago)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paActualizarPinPrePago_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPinPrePago", idPinPrepago);
                cmd.Parameters.AddWithValue("@ValorCompraPinPrepago", valorCompraPinPrepago);
                cmd.ExecuteNonQuery();
            }
        }



        /// <summary>
        /// Validar el saldo del Prepago para descontar del
        /// saldo.
        /// </summary>
        /// <param name="idPinPrepago">el id del pin prepago.</param>
        /// <param name="valorCompraPinPrepago">es el valor de la compra con el pin prepago.</param>
        /// <returns></returns>
        public bool ValidarSaldoPrepago(long pinPrepago, decimal valorCompraPinPrepago)
        {
            bool rst = false;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPinPrePago_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PinPrePago", pinPrepago);
                SqlDataAdapter ta = new SqlDataAdapter();
                DataTable dt = new DataTable();
                ta.Fill(dt);

                var pinPrepagoRes = dt.AsEnumerable().FirstOrDefault();

                if (pinPrepagoRes != null)
                {
                    if ((pinPrepagoRes.Field<decimal>("PIP_Saldo") - valorCompraPinPrepago) < 0)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                              .ERROR_VALOR_PINPREPAGO_SUPERA_SALDO.ToString(), CACajaServerMensajes
                                                                              .CargarMensaje(CAEnumTipoErrorCaja.ERROR_VALOR_PINPREPAGO_SUPERA_SALDO));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    rst = true;
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                            .ERROR_PINPREPAGO_NO_ENCONTRADO.ToString(), CACajaServerMensajes
                                                                            .CargarMensaje(CAEnumTipoErrorCaja.ERROR_PINPREPAGO_NO_ENCONTRADO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
            return rst;
        }



        /// <summary>
        ///Insercion de Tipos de Datos Adicionales
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="idRegistroCaja">The id registro caja.</param>
        /// <param name="contexto">The contexto.</param>
        public void InsertarTiposDatosAdicionales(CARegistroTransacCajaDC movimientoCaja, long idRegistroCaja)
        {
            if (movimientoCaja.TipoDatosAdicionales == CAEnumTipoDatosAdicionales.CRE)
            {
                if (movimientoCaja.RegistroVentaClienteCredito != null)
                {
                    // se ejecuta el Procedimiento Almacenado de los datos del Cliente Credito

                    using (SqlConnection conn = new SqlConnection(conexionStringController))
                    {
                        SqlCommand cmd = new SqlCommand("paCrearRegClienteCredito_CAJ", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idRegistroVenta", idRegistroCaja);
                        cmd.Parameters.AddWithValue("@idCliente", movimientoCaja.RegistroVentaClienteCredito.IdCliente);
                        cmd.Parameters.AddWithValue("@nombreCliente", movimientoCaja.RegistroVentaClienteCredito.NombreCliente);
                        cmd.Parameters.AddWithValue("@nit",movimientoCaja.RegistroVentaClienteCredito.NitCliente);
                        cmd.Parameters.AddWithValue("@idContrato", movimientoCaja.RegistroVentaClienteCredito.IdContrato);
                        cmd.Parameters.AddWithValue("@numeroContrato", movimientoCaja.RegistroVentaClienteCredito.NumeroContrato);
                        cmd.Parameters.AddWithValue("@idSucursal", movimientoCaja.RegistroVentaClienteCredito.IdSucursal);
                        cmd.Parameters.AddWithValue("@nombreSucursal", movimientoCaja.RegistroVentaClienteCredito.NombreSucursal);
                        cmd.Parameters.AddWithValue("@creadoPor", movimientoCaja.Usuario);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                      .ERROR_CLIENTE_CREDITO_SIN_DATOS.ToString(), CACajaServerMensajes
                                                                      .CargarMensaje(CAEnumTipoErrorCaja.ERROR_CLIENTE_CREDITO_SIN_DATOS));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }
    }
}
