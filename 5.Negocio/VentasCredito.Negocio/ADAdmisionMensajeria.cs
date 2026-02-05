using System;


namespace VentaCredito.Negocio
{
    public class ADAdmisionMensajeria
    {
        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        internal ADResultadoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            int idBelcorp = -1;

            if (guia.TokenClienteCredito != Guid.Empty)
            {
                idBelcorp = Convert.ToInt32(PAParametros.Instancia.ConsultarParametrosFramework("idClienteBelcorp"));

                if (guia.IdCliente == idBelcorp)
                {
                    if (!ApiIntegracion.Instancia.VerificarTransaccionInventarioDevolucion(guia.TokenClienteCredito))
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_TRANSACCION_INVENT_DEVOLUCION_CLIENTE_CREDITO.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_TRANSACCION_INVENT_DEVOLUCION_CLIENTE_CREDITO)));
                    }
                }
            }


            // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo
            ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
            CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
            if (sucursal != null)
            {
                if (guia.TipoCliente != ADEnumTipoCliente.PCO)
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
                }

                // Se obtiene el número de guía
                ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
                SUNumeradorPrefijo numeroSuministro;
                if (guia.NumeroGuia == 0)
                {

                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.GUIA__TRANSPORTE_AUTOMATICA);
                        transaccion.Complete();
                    }

                    guia.NumeroGuia = numeroSuministro.ValorActual;
                    guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;
                }

                using (TransactionScope transaccion = new TransactionScope())
                {
                    bool avisoPorcentajeMinimoAviso = fachadaClientes.ValidarCupoCliente(guia.IdContrato, guia.ValorTotal);
                    fachadaClientes.ModificarAcumuladoContrato(guia.IdContrato, guia.ValorTotal);

                    // La primera validación que se debe hacer, es verificar que la caja esté abierta
                    ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

                    // Se deben calcular las comisiones de ventas
                    ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();
                    CMComisionXVentaCalculadaDC comision = CalcularComisionesPorVentas(guia, fachadaComisiones);
                    fachadaComisiones.GuardarComision(comision);

                    // Se adiciona el movimiento de caja
                    AdicionarMovimientoCaja(idCaja, fachadaCajas, comision, guia);

                    // Se adiciona la admisión
                    guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                    guia.FechaAdmision = DateTime.Now;
                    long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);
                    guia.IdAdmision = idAdmisionMensajeria;
                    GuardarConsumoGuiaAutomatica(guia);

                    if (guia.IdCliente == idBelcorp)
                    {
                        //asocia token de cliente credito con numero de guia
                        ApiIntegracion.Instancia.ActualizarTransaccionInventario(guia.TokenClienteCredito, guia.NumeroGuia);
                    }

                    transaccion.Complete();
                    return new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        IdAdmision = guia.IdAdmision,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = avisoPorcentajeMinimoAviso
                    };
                }
            }
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));
        }
    }
}
