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
using CO.Controller.Servidor.Integraciones.BelCorp;

namespace CO.Servidor.Adminisiones.Mensajeria.Credito
{
    /// <summary>
    /// Contiene la lógica de negocio de admisión para cliente crédito
    /// </summary>
    internal class ADAdmisionCredito : ControllerBase
    {
        private static readonly ADAdmisionCredito instancia = (ADAdmisionCredito)FabricaInterceptores.GetProxy(new ADAdmisionCredito(), COConstantesModulos.MENSAJERIA);
        private ITAFachadaTarifas fachadaTarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static ADAdmisionCredito Instancia
        {
            get { return ADAdmisionCredito.instancia; }
        }

        /// <summary>
        /// Valida si el servicio está habilitado para el trayecto dado por el municipio de origen y el de destino, debe retornar el peso máximo soportado para
        /// dicho trayecto, la duración en días y la prima de seguro acordaba con el cliente en el contrato
        /// </summary>
        /// <param name="municipioOrigen">Municipio de origen</param>
        /// <param name="municipioDestino">Municipio de destino</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <param name="idCliente">Identificador del cliente crédito</param>
        /// <param name="idListaPrecios">Identificador de la lista de precios asociada al contrato</param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestinoCliente(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, int idSucursal, int idCliente, int idListaPrecios, decimal pesoGuia, DateTime? fechadmisionEnvio = null)
        {
            // Validar trayecto, calcular duración en días
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            DateTime fechaDigitalizacion;
            int idservicio = servicio.IdServicio;
            ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino = new ADValidacionServicioTrayectoDestino();
            bool fechaValidaEntrega = false;
            PUCentroServiciosDC centroSerDestino;
            int numeroDias = 0;
            int numeroHoras = 0;
            TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
            if (municipioOrigen.IdLocalidad == "99999")
                municipioOrigen.IdLocalidad = "11001000";
            if (municipioDestino.IdLocalidad == "99999")
                municipioDestino.IdLocalidad = "11001000";

            //Validacion para el tipo de servicio 3 cuando es igual o mayor a 3 Kg el envio, se valida si es costa con el fin de asignar el servicio 17 en caso contrario se asigna el 6 o 3 segun corresponda
            servicio.IdServicio = ADAdmisionContado.Instancia.ValidarServicioSegunPesoMensajeria(municipioOrigen, municipioDestino, servicio, pesoGuia, tarifas);

            tiempos = tarifas.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, new TAServicioDC { IdServicio = TAConstantesServicios.SERVICIO_MENSAJERIA });
            IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            if (servicio.IdServicio != TAConstantesServicios.SERVICIO_INTERNACIONAL)
            {
                centroSerDestino = fachadaCentroServicio.ObtenerAgenciaLocalidad(municipioDestino.IdLocalidad);
                List<TAHorarioRecogidaCsvDC> horarioCsvDestino = tarifas.ObtenerHorarioRecogidaDeCsv(centroSerDestino.IdCentroServicio);

                PUCentroServiciosDC re = fachadaCentroServicio.ObtenerRepresentanteLegalPunto(centroSerDestino.IdCentroServicio);

                validacionServicioTrayectoDestino = tarifas.ValidarServicioTrayectoDestinoCliente(municipioOrigen, municipioDestino, servicio, idListaPrecios, idservicio, idSucursal);
                validacionServicioTrayectoDestino.IdentificacionRepLegal = re.IdentificacionPropietario;
                validacionServicioTrayectoDestino.NombreRepLegal = re.NombrePropietario;
                // numeroDias = validacionServicioTrayectoDestino.DuracionTrayectoEnHoras;

                DateTime fechaEntregaSegunHorario = DateTime.Now.Date;
                DateTime fechaEntregaDefinitiva = DateTime.Now;
                int horasEntrega = 0;

                fechaEntregaSegunHorario = fechaEntregaDefinitiva;

                if (horarioCsvDestino.Any())
                {
                    for (int dias = 0; dias < 10; dias++)
                    {
                        if (fechaValidaEntrega)
                            break;
                        fechaEntregaSegunHorario = fechaEntregaSegunHorario.AddDays(dias);
                        for (int i = 0; i < horarioCsvDestino.Count(); i++)
                        {
                            int diaSemana = ((int)fechaEntregaSegunHorario.DayOfWeek == 0) ? 7 : (int)fechaEntregaSegunHorario.DayOfWeek;
                            if (diaSemana == horarioCsvDestino[i].DiaDeLaSemana)
                            {
                                fechaEntregaDefinitiva = fechaEntregaSegunHorario;
                                fechaValidaEntrega = true;
                            }
                        }
                    }
                }

                horasEntrega = ((fechaEntregaDefinitiva.Date.AddHours(18) - DateTime.Now.Date.AddHours(DateTime.Now.Hour)).Days * 24) + (fechaEntregaDefinitiva.Date.AddHours(18) - DateTime.Now.Date.AddHours(DateTime.Now.Hour)).Hours;

                numeroDias = tiempos.numeroDiasEntrega;

                if (tarifas.ValidarServicioTrayectoCasilleroAereo(municipioOrigen.IdLocalidad, municipioDestino.IdLocalidad, servicio.IdServicio))
                {
                    ADRangoTrayecto newTrayecto = new ADRangoTrayecto() { IdLocalidadOrigen = municipioOrigen.IdLocalidad, IdLocalidadDestino = municipioDestino.IdLocalidad };
                    newTrayecto.Rangos = new List<ADRangoCasillero>();
                    newTrayecto.Rangos.Add(new ADRangoCasillero() { RangoInicial = 0, RangoFinal = 999, Casillero = "AEREO" });
                    validacionServicioTrayectoDestino.InfoCasillero = newTrayecto;
                }
                else
                    validacionServicioTrayectoDestino.InfoCasillero = ADAdmisionMensajeria.Instancia.ConsultarCasilleroTrayecto(municipioOrigen.IdLocalidad, municipioDestino.IdLocalidad);

                //según indicaciones de Walter si el origen es el mismo destino no se debe tener en cuenta el horario de recogida
                if (municipioOrigen.IdLocalidad == municipioDestino.IdLocalidad)
                {
                    fechaEntregaDefinitiva = DateTime.Now;
                }


                if (servicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_AM)
                {
                    if (fechadmisionEnvio.HasValue)
                    {
                        double numHabiles = 0;
                        DateTime fechaEntrega;
                        fechaEntrega = PAAdministrador.Instancia.ObtenerFechaFinalHabilSinSabados(fechaEntregaDefinitiva, 1, PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
                        // Rapi AM se  en horas al siguiente día antes de 12 M.
                        TimeSpan diferenciaDeFechasAdm = fechaEntrega - fechadmisionEnvio.Value;
                        numHabiles = diferenciaDeFechasAdm.TotalDays * 24;
                        int numeroHorasRapiAM = Convert.ToInt32(((numHabiles) - DateTime.Now.Hour) + 12);
                        //numeroHorasRapiAM = numeroHorasRapiAM + (horasRecogida >= 20 ? horasRecogida : 0);
                        validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHorasRapiAM;
                        fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(fechaEntrega, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, numeroHorasRapiAM);
                        ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                    }
                    else
                    {
                        double numDias = 0;
                        DateTime horaEntregaRapiAM = DateTime.Now;
                        numeroDias += 1;
                        horaEntregaRapiAM = PAAdministrador.Instancia.ObtenerFechaFinalHabilSinSabados(DateTime.Now, Convert.ToDouble(numeroDias), PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
                        TimeSpan diferenciaDeFechas;
                        diferenciaDeFechas = horaEntregaRapiAM - DateTime.Now;
                        numDias = diferenciaDeFechas.TotalDays * 24;
                        if (numDias > 0)
                        {
                            numeroHoras = Convert.ToInt32(((numDias) - horaEntregaRapiAM.Hour) + 12);
                            numeroHoras = numeroHoras + (horasEntrega >= 20 ? horasEntrega : 0);
                            validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHoras;
                        }
                        else
                        {
                            numeroHoras = (24 - DateTime.Now.Hour) + 12;
                            numeroHoras = numeroHoras + (horasEntrega >= 20 ? horasEntrega : 0);
                            validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHoras;
                        }
                        fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(horaEntregaRapiAM, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, numeroHoras);
                        ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                    }


                }
                else if (servicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_HOY)
                {
                    // Rapi HOY se calcula en horas el mismo día antes de las 6 PM.
                    validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = 18 - DateTime.Now.Hour;
                    fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(DateTime.Now, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, (18 - DateTime.Now.Hour));
                    ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                }
                else
                {
                    // Si en el rango de fechas hay fines de semana y/o festivos, deben ser omitidos
                    double numHabiles = 0;
                    DateTime fechaEntrega;
                    fechaEntrega = PAAdministrador.Instancia.ObtenerFechaFinalHabilSinSabados(fechaEntregaDefinitiva.Date.AddHours(18), Convert.ToDouble(numeroDias), PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
                    //fechaRecogida.Date.AddHours(18);                    
                    TimeSpan diferenciaDeFechas = fechaEntrega - DateTime.Now.Date.AddHours(18);
                    numHabiles = diferenciaDeFechas.Days * 24;
                    //+ (horasRecogida >= 24 ? horasRecogida : 0)
                    numeroHoras = Convert.ToInt32((numHabiles));
                    validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHoras;
                    fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(fechaEntrega, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, numeroHoras);
                    ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                }

                validacionServicioTrayectoDestino.CodigoPostalDestino = municipioDestino.IdLocalidad;
                fachadaCentroServicio.ObtenerInformacionValidacionTrayectoOrigen(municipioOrigen, validacionServicioTrayectoDestino);                
                validacionServicioTrayectoDestino.IdCentroServiciosDestino = centroSerDestino.IdCentroServicio;
                validacionServicioTrayectoDestino.NombreCentroServiciosDestino = centroSerDestino.Nombre;
                validacionServicioTrayectoDestino.DireccionCentroServiciosDestino = centroSerDestino.Direccion;
                
            }
            else
            {
                // Se obtiene Operador postal del destino (aplica para internacional)
                PAOperadorPostal operadorPostal = PAAdministrador.Instancia.ObtenerOperadorPostalLocalidad(municipioDestino.IdLocalidad);
                if (operadorPostal != null)
                {
                    validacionServicioTrayectoDestino.IdOperadorPostalDestino = operadorPostal.Id;
                    validacionServicioTrayectoDestino.IdZonaOperadorPostalDestino = operadorPostal.IdZona;
                    validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = operadorPostal.TiempoEntrega * 24; // Porque el tiempo de entrega por zona se da en días
                }

                //Cuando es internacional se valida con la ciudad de destino Bogota
                validacionServicioTrayectoDestino.CodigoPostalDestino = "11001000";
                fachadaCentroServicio.ObtenerInformacionValidacionTrayectoOrigen(municipioOrigen, validacionServicioTrayectoDestino);
                validacionServicioTrayectoDestino.ValoresAdicionales = new List<TAValorAdicional>();
            }

            return validacionServicioTrayectoDestino;
        }

        public DateTime ObtenerHorasDigitalizacionParaGuia(DateTime fechaEntrega, ref ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino, double tiempo, int numeroHoras)
        {
            int numeroHorasNuevo = 0;
            double numHabilesDigitalizacion = 0;
            DateTime fechaDigitalizacion;
            fechaDigitalizacion = PAAdministrador.Instancia.ObtenerFechaFinalHabil(fechaEntrega, tiempo, PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            TimeSpan diferenciaDeFechas = fechaDigitalizacion - fechaEntrega;
            numHabilesDigitalizacion = (diferenciaDeFechas.Days * 24) + diferenciaDeFechas.Hours;
            numeroHorasNuevo = Convert.ToInt32((numHabilesDigitalizacion) + numeroHoras);
            validacionServicioTrayectoDestino.NumeroHorasDigitalizacion = numeroHorasNuevo;
            return fechaDigitalizacion;

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
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        internal ADResultadoAdmision RegistrarGuiaManual(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, bool esAdmisionCOL, long? idAgenciaRegistraAdmision = null)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo
                ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
                if (sucursal != null)
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
                    bool avisoPorcentajeMinimoAviso = fachadaClientes.ValidarCupoCliente(guia.IdContrato, guia.ValorTotal);
                    fachadaClientes.ModificarAcumuladoContrato(guia.IdContrato, guia.ValorTotal);
                    //CLSucursalDC suc = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
                    guia.NombreSucursal = sucursal.Nombre;
                    CLClientesDC cliente = fachadaClientes.ObtenerCliente(guia.IdCliente);
                    guia.NombreCliente = cliente.RazonSocial;
                    guia.NitCliente = cliente.Nit;

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

                    if (!esAdmisionCOL)
                    {
                        RegistrarGuia(guia, remitenteDestinatario);
                    }
                    else
                    {
                        RegistrarGuia(guia, remitenteDestinatario, true, idAgenciaRegistraAdmision);
                    }

                    GuardarConsumoGuia(guia);

                    transaccion.Complete();
                    return new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = avisoPorcentajeMinimoAviso
                    };
                }
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));
            }
        }

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual internacional
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        internal ADResultadoAdmision RegistrarGuiaManualInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, bool esAdmisionCOL, long? idAgenciaRegistraAdmision = null)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo
                ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
                if (sucursal != null)
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
                    bool avisoPorcentajeMinimoAviso = fachadaClientes.ValidarCupoCliente(guia.IdContrato, guia.ValorTotal);
                    fachadaClientes.ModificarAcumuladoContrato(guia.IdContrato, guia.ValorTotal);
                    guia.NombreSucursal = sucursal.Nombre;
                    CLClientesDC cliente = fachadaClientes.ObtenerCliente(guia.IdCliente);
                    guia.NombreCliente = cliente.RazonSocial;
                    guia.NitCliente = cliente.Nit;

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

                    long idAdmisionMensajeria;

                    if (!esAdmisionCOL)
                    {
                        idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);
                    }
                    else
                    {
                        idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, true, idAgenciaRegistraAdmision);
                    }

                    GuardarConsumoGuia(guia);

                    ADAdmisionInternacional.Instancia.AdicionarAdmisionTipoEmpaque(idAdmisionMensajeria, tipoEmpaque);

                    transaccion.Complete();
                    return new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = avisoPorcentajeMinimoAviso
                    };
                }
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));
            }
        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        internal ADResultadoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            if (guia.TokenClienteCredito != Guid.Empty)
            {
                int idBelcorp = Convert.ToInt32(PAParametros.Instancia.ConsultarParametrosFramework("idClienteBelcorp"));

                if (guia.IdCliente == idBelcorp)
                {
                    if (!IntegracionBelCorp.Instancia.VerificarTransaccionInventarioDevolucion(guia.TokenClienteCredito))
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

                    //asocia token de cliente credito con numero de guia
                    IntegracionBelCorp.Instancia.ActualizarTransaccionInventario(guia.TokenClienteCredito, guia.NumeroGuia);

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

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        internal ADResultadoAdmision RegistrarGuiaManualRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado, bool esAdmisionCOL, long? idAgenciaRegistraAdmision = null)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo
                ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
                if (sucursal != null)
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
                    bool avisoPorcentajeMinimoAviso = fachadaClientes.ValidarCupoCliente(guia.IdContrato, guia.ValorTotal);
                    fachadaClientes.ModificarAcumuladoContrato(guia.IdContrato, guia.ValorTotal);
                    guia.NombreSucursal = sucursal.Nombre;
                    CLClientesDC cliente = fachadaClientes.ObtenerCliente(guia.IdCliente);
                    guia.NombreCliente = cliente.RazonSocial;
                    guia.NitCliente = cliente.Nit;
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
                    long idAdmisionMensajeria;
                    if (!esAdmisionCOL)
                    {
                        idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);
                    }
                    else
                    {
                        idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, true, idAgenciaRegistraAdmision);
                    }

                    GuardarConsumoGuia(guia);

                    // Se adiciona el rapiradicado
                    ADRepositorio.Instancia.AdicionarRapiRadicado(idAdmisionMensajeria, rapiRadicado, ControllerContext.Current.Usuario);

                    transaccion.Complete();
                    return new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = avisoPorcentajeMinimoAviso
                    };
                }
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));
            }
        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        internal ADResultadoAdmision RegistrarGuiaAutomaticaRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado)
        {
            // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo
            ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
            CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
            if (sucursal != null)
            {
                // Se obtiene el número de guía
                ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
                SUNumeradorPrefijo numeroSuministro;

                using (TransactionScope transaccion = new TransactionScope())
                {
                    numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.GUIA__TRANSPORTE_AUTOMATICA);
                    transaccion.Complete();
                }

                guia.NumeroGuia = numeroSuministro.ValorActual;
                guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;
                using (TransactionScope transaccion = new TransactionScope())
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
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
                    GuardarConsumoGuiaAutomatica(guia);

                    // Se adiciona el rapiradicado
                    ADRepositorio.Instancia.AdicionarRapiRadicado(idAdmisionMensajeria, rapiRadicado, ControllerContext.Current.Usuario);

                    transaccion.Complete();
                    return new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = avisoPorcentajeMinimoAviso
                    };
                }
            }
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));
        }

        /// <summary>
        /// Registra guia cuyo servicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        internal ADResultadoAdmision RegistrarGuiaManualNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion, bool esAdmisionCOL, long? idAgenciaRegistraAdmision = null)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo
                ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
                if (sucursal != null)
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
                    bool avisoPorcentajeMinimoAviso = fachadaClientes.ValidarCupoCliente(guia.IdContrato, guia.ValorTotal);
                    fachadaClientes.ModificarAcumuladoContrato(guia.IdContrato, guia.ValorTotal);
                    guia.NombreSucursal = sucursal.Nombre;
                    CLClientesDC cliente = fachadaClientes.ObtenerCliente(guia.IdCliente);
                    guia.NombreCliente = cliente.RazonSocial;
                    guia.NitCliente = cliente.Nit;
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

                    long idAdmisionMensajeria;
                    if (!esAdmisionCOL)
                    {
                        idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);
                    }
                    else
                    {
                        idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, true, idAgenciaRegistraAdmision);
                    }

                    GuardarConsumoGuia(guia);

                    ADAdmisionNotificacion.Instancia.AdicionarNotificacion(idAdmisionMensajeria, notificacion);

                    transaccion.Complete();
                    return new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = avisoPorcentajeMinimoAviso
                    };
                }
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));
            }
        }

        /// <summary>
        /// Registra guia cuyo servicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        internal ADResultadoAdmision RegistrarGuiaAutomaticaNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion)
        {
            // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo
            ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
            CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
            if (sucursal != null)
            {
                // Se obtiene el número de guía
                ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
                SUNumeradorPrefijo numeroSuministro;
                using (TransactionScope tx = new TransactionScope())
                {
                    numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.GUIA__TRANSPORTE_AUTOMATICA);
                    tx.Complete();
                }

                guia.NumeroGuia = numeroSuministro.ValorActual;
                guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;

                using (TransactionScope transaccion = new TransactionScope())
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
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
                    ADAdmisionNotificacion.Instancia.AdicionarNotificacion(idAdmisionMensajeria, notificacion);
                    GuardarConsumoGuiaAutomatica(guia);
                    transaccion.Complete();
                    return new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = avisoPorcentajeMinimoAviso
                    };
                }
            }
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        internal ADResultadoAdmision RegistrarGuiaManualRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago, bool esAdmisionCOL, long? idAgenciaRegistraAdmision = null)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo
                ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
                if (sucursal != null)
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
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
                    long idAdmisionMensajeria;
                    if (!esAdmisionCOL)
                    {
                        idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);
                    }
                    else
                    {
                        idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, true, idAgenciaRegistraAdmision);
                    }

                    GuardarConsumoGuia(guia);

                    ADAdmisionRapiEnvioContraPago.Instancia.AdicionarRapiEnvioContraPago(idAdmisionMensajeria, rapiEnvioContraPago);

                    transaccion.Complete();
                    return new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = avisoPorcentajeMinimoAviso
                    };
                }
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));
            }
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        internal ADResultadoAdmision RegistrarGuiaAutomaticaRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago)
        {
            // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo
            ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
            CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });
            if (sucursal != null)
            {
                // Se obtiene el número de guía
                ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
                SUNumeradorPrefijo numeroSuministro;
                using (TransactionScope tx = new TransactionScope())
                {
                    numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.GUIA__TRANSPORTE_AUTOMATICA);
                    tx.Complete();
                }

                guia.NumeroGuia = numeroSuministro.ValorActual;
                guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;

                using (TransactionScope transaccion = new TransactionScope())
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
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
                    ADAdmisionRapiEnvioContraPago.Instancia.AdicionarRapiEnvioContraPago(idAdmisionMensajeria, rapiEnvioContraPago);
                    GuardarConsumoGuiaAutomatica(guia);
                    transaccion.Complete();
                    return new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = avisoPorcentajeMinimoAviso
                    };
                }
            }
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));
        }

        /// <summary>
        /// Registra una guía automática en el sistema
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        private long RegistrarGuia(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, bool validaIngresoACentroAcopio = false, long? agenciaRegistraAdmision = null)
        {
            string usuario = ControllerContext.Current.Usuario;
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

            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            DateTime fechaActual = DateTime.Now;
            ADValidacionServicioTrayectoDestino SegundaValidacion = new ADValidacionServicioTrayectoDestino();
            //if (guia.FechaEstimadaEntrega.Date <= DateTime.Now.Date)
            //{
            PALocalidadDC municipioOrigen = new PALocalidadDC { IdLocalidad = guia.IdCiudadOrigen, Nombre = guia.NombreCiudadOrigen };
            PALocalidadDC municipioDestino = new PALocalidadDC { IdLocalidad = guia.IdCiudadDestino, Nombre = guia.NombreCiudadDestino };
            TAServicioDC servicio = new TAServicioDC { IdServicio = guia.IdServicio, Nombre = guia.NombreServicio };
            SegundaValidacion = ADAdmisionContado.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, guia.IdCentroServicioOrigen,guia.Peso);
            guia.FechaEstimadaEntrega = DateTime.Now.AddHours(SegundaValidacion.DuracionTrayectoEnHoras).Date.AddHours(18);
            guia.FechaEstimadaDigitalizacion = fechaActual.AddHours(SegundaValidacion.NumeroHorasDigitalizacion);
            guia.FechaEstimadaDigitalizacion = new DateTime(guia.FechaEstimadaDigitalizacion.Year, guia.FechaEstimadaDigitalizacion.Month, guia.FechaEstimadaDigitalizacion.Day, guia.FechaEstimadaDigitalizacion.Hour, 0, 0);
            guia.FechaEstimadaArchivo = fechaActual.AddHours(SegundaValidacion.NumeroHorasArchivo);
            guia.FechaEstimadaArchivo = new DateTime(guia.FechaEstimadaArchivo.Year, guia.FechaEstimadaArchivo.Month, guia.FechaEstimadaArchivo.Day, guia.FechaEstimadaArchivo.Hour, 0, 0);
            //}


            // se crea validacion para registrar el tiempo de digitalizacion y archivo de la guia
            //if (guia.FechaEstimadaDigitalizacion.Year <= 1 && guia.FechaEstimadaArchivo.Year <= 1)
            //{                
            //    fechaActual = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, 18, 0, 0);                
            //    TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
            //    tiempos = tarifas.ObtenerTiempoDigitalizacionArchivo(guia.IdCiudadOrigen, guia.IdCiudadDestino);
            //    ADValidacionServicioTrayectoDestino validacionTiempoDigitalizacionArchivo = new ADValidacionServicioTrayectoDestino();
            //    DateTime fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(guia.FechaEstimadaEntrega, ref validacionTiempoDigitalizacionArchivo, tiempos.numeroDiasDigitalizacion, (guia.DiasDeEntrega * 24));
            //    guia.FechaEstimadaDigitalizacion = fechaActual.AddHours(validacionTiempoDigitalizacionArchivo.NumeroHorasDigitalizacion);
            //    ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionTiempoDigitalizacionArchivo, tiempos.numeroDiasArchivo, validacionTiempoDigitalizacionArchivo.NumeroHorasDigitalizacion);
            //    guia.FechaEstimadaArchivo = fechaActual.AddHours(validacionTiempoDigitalizacionArchivo.NumeroHorasArchivo);
            //}

            long idAdmisionMensajeria = ADRepositorio.Instancia.AdicionarAdmision(guia, remitenteDestinatario);
            guia.IdAdmision = idAdmisionMensajeria;
            // Agregar bit que indica si el destinatario autoriza envío de mensaje de texto
            if (guia.TipoCliente == ADEnumTipoCliente.CPE)
            {
                ADRepositorio.Instancia.ValidarSiDestinatarioAutorizaEnvioMensajeTexto(guia.IdCliente,
                  remitenteDestinatario.PeatonDestinatario.TipoIdentificacion, remitenteDestinatario.PeatonDestinatario.Identificacion,
                  idAdmisionMensajeria, guia.NumeroGuia, guia.NoPedido
                  );
            }


            // TODO ID, Se quita la insersion en la tabla AdmisionRotulos_MEN
            //if (guia.Peso >= ADRepositorio.Instancia.PesoMinimoRotulo)
            //{
            //  ADRepositorio.Instancia.AdicionarRotulosAdmision(guia.TotalPiezas, guia.NumeroGuia, idAdmisionMensajeria);
            //}

            // Acá se debe validar si se debe generar ingreso a centro de acopio, esto debe aplicar solo para admisión manual COL
            if (validaIngresoACentroAcopio && agenciaRegistraAdmision.HasValue)
            {
                ADRepositorio.Instancia.IngresarGuiaManualCentroAcopio(guia);
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
            ADRepositorio.Instancia.AdicionarGuiaFormasPago(idAdmisionMensajeria, guia.FormasPago, ControllerContext.Current.Usuario);

            // Se insertan los valores adicionales
            ADRepositorio.Instancia.AdicionarValoresAdicionales(idAdmisionMensajeria, guia.ValoresAdicionales, ControllerContext.Current.Usuario);

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
        /// Calcula las comisiones de venta para la admisión registrada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="fachadaComisiones"></param>
        /// <returns></returns>
        private CMComisionXVentaCalculadaDC CalcularComisionesPorVentas(ADGuia guia, ICMFachadaComisiones fachadaComisiones)
        {
            CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentas(
              new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
              {
                  IdCentroServicios = (int)guia.IdCentroServicioOrigen,
                  IdServicio = guia.IdServicio,
                  TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Vender,
                  ValorBaseComision = guia.ValorServicio,
                  NumeroOperacion = guia.NumeroGuia,
              });
            return comision;
        }

        /// <summary>
        /// Calcula las comisiones de entrega para la admisión registrada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="fachadaComisiones"></param>
        /// <returns></returns>
        private CMComisionXVentaCalculadaDC CalcularComisionesPorEntrega(ADGuia guia, ICMFachadaComisiones fachadaComisiones)
        {
            CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentas(
              new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
              {
                  IdCentroServicios = (int)guia.IdCentroServicioOrigen,
                  IdServicio = guia.IdServicio,
                  TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Entregar,
                  ValorBaseComision = guia.ValorServicio,
                  NumeroOperacion = guia.NumeroGuia,
              });
            return comision;
        }

        /// <summary>
        /// Metodo para ingresar un movimiento en caja de una guia de cliente contado
        /// </summary>
        /// <param name="guia">información de la guía</param>
        /// <param name="idCaja">caja desde la que se genera la transacción</param>
        public void TransaccionCajaCredito(ADGuia guia, int idCaja)
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

            // Se deben calcular las comisiones de ventas
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();
            CMComisionXVentaCalculadaDC comision = CalcularComisionesPorEntrega(guia, fachadaComisiones);

            //Se guarda la comisión
            fachadaComisiones.GuardarComision(comision);

            // Se adiciona el movimiento de caja
            AdicionarMovimientoCaja(idCaja, fachadaCajas, comision, guia);
            //fachadaComisiones.GuardarComision(comision);
        }

        /// <summary>
        /// Adiciona un movimiento de caja para la transacción de guia creada
        /// </summary>
        /// <param name="idCaja"></param>
        /// <param name="fachadaCajas"></param>
        /// <param name="comision"></param>
        private void AdicionarMovimientoCaja(int idCaja, ICAFachadaCajas fachadaCajas, CMComisionXVentaCalculadaDC comision, ADGuia guia)
        {
            fachadaCajas.AdicionarMovimientoCaja(new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = idCaja,
                    IdCodigoUsuario = guia.IdCodigoUsuario
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.CRE,
                IdCentroResponsable = comision.IdCentroServicioResponsable,
                IdCentroServiciosVenta = comision.IdCentroServicioVenta,
                NombreCentroResponsable = comision.NombreCentroServicioResponsable,
                NombreCentroServiciosVenta = comision.NombreCentroServicioVenta,
                RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
        {
          new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC() {
             Cantidad = 1,
             ConceptoCaja = new CAConceptoCajaDC() { IdConceptoCaja = guia.IdConceptoCaja },
             ConceptoEsIngreso = true,
             EstadoFacturacion = CAEnumEstadoFacturacion.PED,
             FechaFacturacion = DateTime.Now,
             Numero = guia.NumeroGuia,
             NumeroFactura = string.Empty,
             Observacion = guia.Observaciones,
             ValorDeclarado = guia.ValorDeclarado,
             ValoresAdicionales = guia.ValorAdicionales,
             ValorImpuestos = guia.ValorTotalImpuestos,
             ValorPrimaSeguros = guia.ValorPrimaSeguro,
             ValorRetenciones = guia.ValorTotalRetenciones,
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
                }),
                RegistroVentaClienteCredito = new CARegistroTransClienteCreditoDC()
                {
                    IdCliente = guia.IdCliente,
                    IdContrato = guia.IdContrato,
                    IdSucursal = guia.IdSucursal,
                    NitCliente = guia.NitCliente,
                    NombreCliente = guia.NombreCliente,
                    NombreSucursal = guia.NombreSucursal,
                    NumeroContrato = guia.IdContrato.ToString()
                }
            });
        }

        /// <summary>
        /// Guarda la guia de mensajería como consumida
        /// </summary>
        /// <param name="guia"></param>
        private void GuardarConsumoGuia(ADGuia guia)
        {
            PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(guia.IdCentroServicioOrigen);
            SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), centroServ.Tipo);


            SUConsumoSuministroDC consumo = new SUConsumoSuministroDC()
            {
                Cantidad = 1,
                EstadoConsumo = SUEnumEstadoConsumo.CON,
                GrupoSuministro = grupo,
                IdDuenoSuministro = guia.IdCentroServicioOrigen,
                IdServicioAsociado = guia.IdServicio,
                NumeroSuministro = guia.NumeroGuia,
                Suministro = SUEnumSuministro.GUIA_TRANSPORTE_MANUAL
            };
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            fachadaSuministros.GuardarConsumoSuministro(consumo);
        }

        /// <summary>
        /// Guarda la guia de mensajería como consumida
        /// </summary>
        /// <param name="guia"></param>
        private void GuardarConsumoGuiaAutomatica(ADGuia guia)
        {
            PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(guia.IdCentroServicioOrigen);
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
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            fachadaSuministros.GuardarConsumoSuministro(consumo);
        }

        /// <summary>
        /// Registra guía automática internacional
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="tipoEmpaque">Tipo de empaque</param>
        /// <returns></returns>
        internal ADResultadoAdmision RegistrarGuiaAutomaticaInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque)
        {
            // Para cliente crédito se debe hacer validación de si tiene cupo y descontarlo
            ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
            CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal(guia.IdSucursal, new CLClientesDC() { IdCliente = guia.IdCliente });

            if (sucursal != null)
            {
                // Se obtiene el número de guía
                ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
                SUNumeradorPrefijo numeroSuministro;
                using (TransactionScope transaccion = new TransactionScope())
                {
                    numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.GUIA__TRANSPORTE_AUTOMATICA);
                    transaccion.Complete();
                }

                guia.NumeroGuia = numeroSuministro.ValorActual;
                guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;

                using (TransactionScope transaccion = new TransactionScope())
                {
                    guia.IdCentroServicioOrigen = sucursal.Agencia;
                    bool superoPorcentajeMinimoAviso = fachadaClientes.ValidarCupoCliente(guia.IdContrato, guia.ValorTotal);
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
                    ADAdmisionInternacional.Instancia.AdicionarAdmisionTipoEmpaque(idAdmisionMensajeria, tipoEmpaque);
                    GuardarConsumoGuiaAutomatica(guia);
                    transaccion.Complete();
                    return new ADResultadoAdmision
                    {
                        NumeroGuia = guia.NumeroGuia,
                        AdvertenciaPorcentajeCupoSuperadoClienteCredito = superoPorcentajeMinimoAviso
                    };
                }
            }
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE)));
        }
    }
}