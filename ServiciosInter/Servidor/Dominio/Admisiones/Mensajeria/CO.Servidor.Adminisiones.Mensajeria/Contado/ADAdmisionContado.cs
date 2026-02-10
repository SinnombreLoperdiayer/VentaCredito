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
using CO.Servidor.Dominio.Comun.ParametrosOperacion;
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
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Net;
using System.Xml;
using System.IO;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;

namespace CO.Servidor.Adminisiones.Mensajeria.Contado
{
    internal class ADAdmisionContado : ControllerBase
    {
        private static readonly ADAdmisionContado instancia = (ADAdmisionContado)FabricaInterceptores.GetProxy(new ADAdmisionContado(), COConstantesModulos.MENSAJERIA);

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static ADAdmisionContado Instancia
        {
            get { return ADAdmisionContado.instancia; }
        }


        private IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
        private IONFachadaOperacionNacional fachadaOperacionNacional = COFabricaDominio.Instancia.CrearInstancia<IONFachadaOperacionNacional>();
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        
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
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino = new ADValidacionServicioTrayectoDestino();
            int numeroDias = 0;
            int numeroHoras = 0;

            IPUFachadaCentroServicios centroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

            if (servicio.IdServicio != TAConstantesServicios.SERVICIO_INTERNACIONAL)
            {
                List<DateTime> listaFechasRecogida = new List<DateTime>();
                DateTime fec = DateTime.Now.Date;
                DateTime fechaRecogida = DateTime.Now;
                DateTime fechaDigitalizacion;
                DateTime fechaEntregaSegunHorario;
                int horasRecogida = 0;
                bool fechaValida = false, fechaValidaEntrega = false;
                List<TAHorarioRecogidaCsvDC> horarioCsv = tarifas.ObtenerHorarioRecogidaDeCsv(centroServiciosOrigen);
                PUCentroServiciosDC centroSerDestino = fachadaCentroServicio.ObtenerAgenciaLocalidad(municipioDestino.IdLocalidad);
                long IdCentroServicioDestino = centroSerDestino.IdCentroServicio;
                List<TAHorarioRecogidaCsvDC> horarioCsvDestino = tarifas.ObtenerHorarioRecogidaDeCsv(IdCentroServicioDestino);
                TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
                if (municipioOrigen.IdLocalidad == "99999")
                    municipioOrigen.IdLocalidad = "11001000";
                if (municipioDestino.IdLocalidad == "99999")
                    municipioDestino.IdLocalidad = "11001000";

                //Validacion para el tipo de servicio 3 cuando es igual o mayor a 3 Kg el envio, se valida si es costa con el fin de asignar el servicio 17 en caso contrario se asigna el 6 o 3 segun corresponda
                servicio.IdServicio = ValidarServicioSegunPesoMensajeria(municipioOrigen, municipioDestino, servicio, pesoGuia, tarifas);

                tiempos = tarifas.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio);

                if (horarioCsv.Any())
                {
                    for (int dias = 0; dias < 10; dias++)
                    {
                        if (fechaValida)
                            break;
                        fec = DateTime.Now.Date;
                        fec = fec.AddDays(dias);
                        for (int i = 0; i < horarioCsv.Count(); i++)
                        {
                            int diaSemana = ((int)fec.DayOfWeek == 0) ? 7 : (int)fec.DayOfWeek;
                            if (diaSemana == horarioCsv[i].DiaDeLaSemana)
                            {
                                fechaRecogida = new DateTime(fec.Year, fec.Month, fec.Day, horarioCsv[i].HoraRecogida.Hour, horarioCsv[i].HoraRecogida.Minute, horarioCsv[i].HoraRecogida.Second);
                                if ((fechaRecogida > DateTime.Now) && (PAAdministrador.Instancia.ConsultarDiasLaborales(fechaRecogida.AddDays(-1), fechaRecogida) > 0))
                                {
                                    fechaValida = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!fechaValida)
                        fechaRecogida = DateTime.Now.Date.AddHours(18);
                }

                fechaEntregaSegunHorario = fechaRecogida;

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
                                fechaRecogida = fechaEntregaSegunHorario;
                                fechaValidaEntrega = true;
                            }
                        }
                    }
                }

                horasRecogida = ((fechaRecogida.Date.AddHours(18) - DateTime.Now.Date.AddHours(DateTime.Now.Hour)).Days * 24) + (fechaRecogida.Date.AddHours(18) - DateTime.Now.Date.AddHours(DateTime.Now.Hour)).Hours;

                //if (validarTrayecto)
                ////cambiar por el nuevo objeto
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
                    fechaRecogida = DateTime.Now;
                }

                if (servicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_AM)
                {
                    if (fechadmisionEnvio.HasValue)
                    {
                        double numHabiles = 0;
                        DateTime fechaEntrega;
                        fechaEntrega = PAAdministrador.Instancia.ObtenerFechaFinalHabilSinSabados(fechaRecogida, 1, PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
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
                            numeroHoras = numeroHoras + (horasRecogida >= 20 ? horasRecogida : 0);
                            validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHoras;
                        }
                        else
                        {
                            numeroHoras = (24 - DateTime.Now.Hour) + 12;
                            numeroHoras = numeroHoras + (horasRecogida >= 20 ? horasRecogida : 0);
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
                    fechaEntrega = PAAdministrador.Instancia.ObtenerFechaFinalHabilSinSabados(fechaRecogida.Date.AddHours(18), Convert.ToDouble(numeroDias), PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
                    //fechaRecogida.Date.AddHours(18);                    
                    TimeSpan diferenciaDeFechas = fechaEntrega - DateTime.Now.Date.AddHours(18);
                    numHabiles = diferenciaDeFechas.Days * 24;
                    //+ (horasRecogida >= 24 ? horasRecogida : 0)
                    numeroHoras = Convert.ToInt32((numHabiles));
                    validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHoras;
                    fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(fechaEntrega, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, numeroHoras);
                    ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                }
                centroServicios.ObtenerInformacionValidacionTrayecto(municipioDestino, validacionServicioTrayectoDestino, centroServiciosOrigen, municipioOrigen);
                validacionServicioTrayectoDestino.ValoresAdicionales = new List<TAValorAdicional>();
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
                centroServicios.ObtenerInformacionValidacionTrayectoOrigen(municipioOrigen, validacionServicioTrayectoDestino);
                validacionServicioTrayectoDestino.ValoresAdicionales = new List<TAValorAdicional>();
            }
            return validacionServicioTrayectoDestino;
        }

        public int ValidarServicioSegunPesoMensajeria(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, decimal pesoGuia, ITAFachadaTarifas tarifas)
        {
            int idServicio = servicio.IdServicio;

            if (servicio.IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA && pesoGuia >= 3)
            {
                List<int> lstIdServicios = tarifas.ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(municipioOrigen.IdLocalidad.ToString(), municipioDestino.IdLocalidad.ToString());
                if (lstIdServicios != null && lstIdServicios.Count > 0 && lstIdServicios.FirstOrDefault() != 0)
                {
                    bool esCosta = tarifas.ValidarServicioTrayectoCasilleroAereo(municipioOrigen.IdLocalidad.ToString(), municipioDestino.IdLocalidad.ToString(), servicio.IdServicio);

                    if (esCosta)
                    {
                        idServicio = lstIdServicios.FirstOrDefault();
                    }
                    else if (lstIdServicios.Count == 1)
                    {
                        idServicio = lstIdServicios.FirstOrDefault();
                    }
                    else
                    {
                        idServicio = lstIdServicios.FirstOrDefault(s => s != 17);
                    }
                }
            }
            return idServicio;
        }

        public DateTime ObtenerHorasDigitalizacionParaGuia(DateTime fechaEntrega, ref ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino, double tiempo, int numeroHoras)
        {
            int numeroHorasNuevo = 0;
            int numeroDeSabados = 0;
            double numHabilesDigitalizacion = 0;
            DateTime FechaDigitalizacion;
            FechaDigitalizacion = PAAdministrador.Instancia.ObtenerFechaFinalHabil(fechaEntrega, tiempo, PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            TimeSpan diferenciaDeFechas = FechaDigitalizacion - fechaEntrega;
            numHabilesDigitalizacion = (diferenciaDeFechas.Days * 24) + diferenciaDeFechas.Hours;
            numeroDeSabados = ContadorSabados(fechaEntrega, FechaDigitalizacion);
            numHabilesDigitalizacion = numHabilesDigitalizacion + (0.5 * numeroDeSabados);
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
            numeroDeSabados = ContadorSabados(fechaEntrega, FechaArchivo);
            numHabilesDigitalizacion = (diferenciaDeFechas.Days * 24) + diferenciaDeFechas.Hours;
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
            ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino = new ADValidacionServicioTrayectoDestino();

            IPUFachadaCentroServicios centroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            centroServicios.ObtenerInformacionValidacionTrayecto(municipioDestino, validacionServicioTrayectoDestino, centroServiciosOrigen, municipioOrigen);

            return validacionServicioTrayectoDestino;
        }


        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        internal ADResultadoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {

            if (string.IsNullOrWhiteSpace(guia.NombreCentroServicioDestino))
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO.ToString(), "El campo NombreCentroServicioDestino no puede estar vacio."));
            }

            if (guia.NumeroGuia == 0)
            {
                SUNumeradorPrefijo numeroSuministro = ObtenerConsecutivoFacturaVenta();

                guia.NumeroGuia = numeroSuministro.ValorActual;
                guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;


                ADRepositorio.Instancia.AuditarGuiaGeneradaArchivoTexto(guia, idCaja, remitenteDestinatario);

                //TODO:CED solo se debe utilizar para las pruebas de carga
                //ADRepositorio.Instancia.AuditarNumeroGuiaGenerado(guia.NumeroGuia);
            }

            ADResultadoAdmision resultado = null;

            SqlConnection conexion = new SqlConnection(conexionStringController);
            SqlTransaction transaccion = null;
            try
            {

                conexion.Open();
                transaccion = conexion.BeginTransaction();

                RegistrarComisionesYMovimientosCaja(guia, idCaja, conexion, transaccion);

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                guia.FechaAdmision = DateTime.Now;
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




                transaccion.Commit();
                conexion.Close();

            }
            catch
            {
                if (transaccion != null)
                    transaccion.Rollback();
                ADRepositorio.Instancia.GuardarNumeroFacturaFallido(guia.NumeroGuia, guia.PrefijoNumeroGuia);
                throw;
            }
            finally
            {
                if (conexion.State != ConnectionState.Closed)
                    conexion.Close();
                conexion.Dispose();
            }
            //TODO:CED solo se debe utilizar para las pruebas de carga
            ADRepositorio.Instancia.ConfirmarAuditoriaNumeroGuiaGenerado(guia.NumeroGuia);

            return resultado;
        }

        /// <summary>
        /// Verifica si se debe hacer integración y si es el caso realiza la integración
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="remitenteDestinatario"></param>
        private void IntegrarConMensajero(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion)
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
                integraConMensajero = ADRepositorio.Instancia.ObtenerParametroIntegraConMensajero();
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
                Integraciones.Mensajero.Entidades.Notificacion notificacionMensajero = new Integraciones.Mensajero.Entidades.Notificacion()
                {
                    CiudadOrigen = notificacion.CiudadDestino.IdLocalidad,
                    DireccionDeQuienRecibeo = notificacion.DireccionDestinatario,
                    NombreDeQuienRecibe = string.Join(" ", notificacion.NombreDestinatario != null ? notificacion.NombreDestinatario.Trim() : string.Empty, notificacion.Apellido1Destinatario != null ? notificacion.Apellido1Destinatario.Trim() : string.Empty, notificacion.Apellido2Destinatario != null ? notificacion.Apellido2Destinatario.Trim() : string.Empty),
                    NumeroGuia = guia.NumeroGuia.ToString(),
                    TelefonoAQuienNotificar = guia.TelefonoDestinatario,
                    Usuario = ControllerContext.Current.Usuario,
                    TipoDestino = notificacion.TipoDestino.Id
                };

                try
                {
                    if (!Integraciones.Mensajero.IntegracionMensajero.Instancia.GrabarNotificacion(guiaMensajero, notificacionMensajero))
                    {
                        // No se pudo admiitr la guía en mensajero, por tanto se debe cancelar la transacción
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO)));
                    }
                }
                catch (Exception exc)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO.ToString(), exc.Message));
                }
            }
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
                integraConMensajero = ADRepositorio.Instancia.ObtenerParametroIntegraConMensajero();
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
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO)));
                    }
                }
                catch (Exception exc)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO.ToString(), exc.Message));
                }
            }
        }

        /// <summary>
        /// Registra una guía en el sistema
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        private long RegistrarGuia(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, bool validaIngresoACentroAcopio = false, long? agenciaRegistraAdmision = null)
        {
            string usuario = ControllerContext.Current.Usuario;
            if (remitenteDestinatario.PeatonRemitente != null)
            {
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
            }

            if (remitenteDestinatario.PeatonDestinatario != null)
            {
                //Destinatario

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
            if (guia.DiceContener != null)
            {
                guia.DiceContener = guia.DiceContener.Replace("<", "-");
                guia.DiceContener = guia.DiceContener.Replace(">", "-");
                guia.DiceContener = guia.DiceContener.Replace('"', '-');
            }

            if (guia.Observaciones != null)
            {
                guia.Observaciones = guia.Observaciones.Replace("<", "-");
                guia.Observaciones = guia.Observaciones.Replace(">", "-");
                guia.Observaciones = guia.Observaciones.Replace('"', '-');
            }
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
            guia.FechaPago = guia.EsAlCobro ? ConstantesFramework.MinDateTimeController : DateTime.Now;
            
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            DateTime fechaActual = DateTime.Now;
            ADValidacionServicioTrayectoDestino SegundaValidacion = new ADValidacionServicioTrayectoDestino();
            if (guia.FechaEstimadaEntrega.Date <= DateTime.Now.Date && guia.IdServicio != TAConstantesServicios.SERVICIO_RAPI_HOY)
            {
                PALocalidadDC municipioOrigen = new PALocalidadDC { IdLocalidad = guia.IdCiudadOrigen, Nombre= guia.NombreCiudadOrigen };
                PALocalidadDC municipioDestino = new PALocalidadDC { IdLocalidad = guia.IdCiudadDestino, Nombre = guia.NombreCiudadDestino };
                TAServicioDC servicio = new TAServicioDC { IdServicio = guia.IdServicio, Nombre = guia.NombreServicio };
                SegundaValidacion = ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, guia.IdCentroServicioOrigen, guia.Peso);
                guia.FechaEstimadaEntrega = DateTime.Now.AddHours(SegundaValidacion.DuracionTrayectoEnHoras).Date.AddHours(18);
                guia.FechaEstimadaDigitalizacion = fechaActual.AddHours(SegundaValidacion.NumeroHorasDigitalizacion);
                guia.FechaEstimadaDigitalizacion = new DateTime(guia.FechaEstimadaDigitalizacion.Year, guia.FechaEstimadaDigitalizacion.Month, guia.FechaEstimadaDigitalizacion.Day, guia.FechaEstimadaDigitalizacion.Hour, 0, 0);
                guia.FechaEstimadaArchivo = fechaActual.AddHours(SegundaValidacion.NumeroHorasArchivo);
                guia.FechaEstimadaArchivo = new DateTime(guia.FechaEstimadaArchivo.Year, guia.FechaEstimadaArchivo.Month, guia.FechaEstimadaArchivo.Day, guia.FechaEstimadaArchivo.Hour, 0, 0);
            }

            if (guia.FechaEstimadaEntrega.Date <= DateTime.Now.Date && guia.IdServicio != TAConstantesServicios.SERVICIO_RAPI_HOY)
            {
                AuditaFechaEstimadaEntregaMalCalculada(guia);
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, "Error calculando fecha estimada de entrega", "Error calculando la fecha estimada de entrega, favor volver a intentar."));
            }


            // se crea validacion para registrar el tiempo de digitalizacion y archivo de la guia
            if (guia.FechaEstimadaDigitalizacion.Year <= 1 && guia.FechaEstimadaArchivo.Year <= 1)
            {
                TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
                tiempos = tarifas.ObtenerTiempoDigitalizacionArchivo(guia.IdCiudadOrigen, guia.IdCiudadDestino);
                fechaActual = fechaActual.Date.AddHours(18);               
                ADValidacionServicioTrayectoDestino validacionTiempoDigitalizacionArchivo = new ADValidacionServicioTrayectoDestino();
                DateTime fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(guia.FechaEstimadaEntrega, ref validacionTiempoDigitalizacionArchivo, tiempos.numeroDiasDigitalizacion, (guia.DiasDeEntrega * 24));
                guia.FechaEstimadaDigitalizacion = fechaActual.AddHours(validacionTiempoDigitalizacionArchivo.NumeroHorasDigitalizacion);
                ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionTiempoDigitalizacionArchivo, tiempos.numeroDiasArchivo, validacionTiempoDigitalizacionArchivo.NumeroHorasDigitalizacion);
                guia.FechaEstimadaArchivo = fechaActual.AddHours(validacionTiempoDigitalizacionArchivo.NumeroHorasArchivo);
            }

            long idAdmisionMensajeria = ADRepositorio.Instancia.AdicionarAdmision(guia, remitenteDestinatario);

            guia.IdAdmision = idAdmisionMensajeria;
            // TODO ID, Se quita la insersion en la tabla AdmisionRotulos_MEN
            //if (guia.Peso >= ADRepositorio.Instancia.PesoMinimoRotulo)
            //{
            //  ADRepositorio.Instancia.AdicionarRotulosAdmision(guia.TotalPiezas, guia.NumeroGuia, idAdmisionMensajeria);
            //}

            // Acá se debe validar si se debe generar ingreso a centro de acopio, esto debe aplicar solo para admisión manual COL

            if (validaIngresoACentroAcopio && agenciaRegistraAdmision.HasValue)
            {
                ADRepositorio.Instancia.IngresarGuiaManualCentroAcopio(guia);

                //    if (fachadaOperacionUrbana.GuiaYaFueIngresadaACentroDeAcopio(guia.NumeroGuia, agenciaRegistraAdmision.Value)
                //      ||fachadaOperacionNacional.GuiaYaFueIngresadaACentroDeAcopio(guia.NumeroGuia, agenciaRegistraAdmision.Value))
                //    {
                //        PUCentroServiciosDC cs = fachadaCentroServicio.ObtenerCentroServicio(agenciaRegistraAdmision.Value);
                //        fachadaOperacionUrbana.CambiarEstado(new Servidor.Servicios.ContratoDatos.OperacionUrbana.OUGuiaIngresadaDC
                //        {
                //            NumeroGuia = guia.NumeroGuia,
                //            IdAdmision = idAdmisionMensajeria,
                //            Observaciones = guia.Observaciones,
                //            IdCiudad = cs.CiudadUbicacion.IdLocalidad,
                //            Ciudad = cs.CiudadUbicacion.Nombre
                //        }, ADEnumEstadoGuia.EnCentroAcopio);
                //    }

            }

            if (!string.IsNullOrEmpty(guia.NumeroBolsaSeguridad))
            {
                //GuardarConsumoBolsaSeguridad(guia);
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

            guia.FechaEstimadaEntrega = guia.FechaEstimadaEntrega.Date.AddHours((guia.IdServicio == TAConstantesServicios.SERVICIO_RAPI_AM ? 12 : 18));

            // El campo de la guía "EsAlCobro" se llena a partir de las formas de pago seleccionadas
            ADGuiaFormaPago formaPagoAlcobro = guia.FormasPago.FirstOrDefault(formaPago => formaPago.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO && formaPago.Valor > 0);
            guia.EsAlCobro = (formaPagoAlcobro != null);
            guia.EstaPagada = !guia.EsAlCobro;
            guia.FechaPago = guia.EsAlCobro ? ConstantesFramework.MinDateTimeController : DateTime.Now;

            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            DateTime fechaActual = DateTime.Now;
            ADValidacionServicioTrayectoDestino SegundaValidacion = new ADValidacionServicioTrayectoDestino();
            if (guia.FechaEstimadaEntrega.Date <= DateTime.Now.Date && guia.IdServicio != TAConstantesServicios.SERVICIO_RAPI_HOY)
            {
                PALocalidadDC municipioOrigen = new PALocalidadDC { IdLocalidad = guia.IdCiudadOrigen, Nombre = guia.NombreCiudadOrigen };
                PALocalidadDC municipioDestino = new PALocalidadDC { IdLocalidad = guia.IdCiudadDestino, Nombre = guia.NombreCiudadDestino };
                TAServicioDC servicio = new TAServicioDC { IdServicio = guia.IdServicio, Nombre = guia.NombreServicio };
                SegundaValidacion = ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, guia.IdCentroServicioOrigen, guia.Peso);
                guia.FechaEstimadaEntrega = DateTime.Now.AddHours(SegundaValidacion.DuracionTrayectoEnHoras).Date.AddHours((guia.IdServicio == TAConstantesServicios.SERVICIO_RAPI_AM ? 12 : 18));
                guia.FechaEstimadaDigitalizacion = fechaActual.AddHours(SegundaValidacion.NumeroHorasDigitalizacion);
                guia.FechaEstimadaDigitalizacion = new DateTime(guia.FechaEstimadaDigitalizacion.Year, guia.FechaEstimadaDigitalizacion.Month, guia.FechaEstimadaDigitalizacion.Day, guia.FechaEstimadaDigitalizacion.Hour, 0, 0);
                guia.FechaEstimadaArchivo = fechaActual.AddHours(SegundaValidacion.NumeroHorasArchivo);
                guia.FechaEstimadaArchivo = new DateTime(guia.FechaEstimadaArchivo.Year, guia.FechaEstimadaArchivo.Month, guia.FechaEstimadaArchivo.Day, guia.FechaEstimadaArchivo.Hour, 0, 0);
            }


            if (guia.FechaEstimadaEntrega.Date <= DateTime.Now.Date && guia.IdServicio != TAConstantesServicios.SERVICIO_RAPI_HOY)
            {

                AuditaFechaEstimadaEntregaMalCalculada(guia);

                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, "Error calculando fecha estimada de entrega", "Error calculando la fecha estimada de entrega, favor volver a intentar."));
            }


            // se crea validacion para registrar el tiempo de digitalizacion y archivo de la guia
            if (guia.FechaEstimadaDigitalizacion.Year <= 1 && guia.FechaEstimadaArchivo.Year <= 1)
            {               
                fechaActual = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, 18, 0, 0);                
                TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
                tiempos = tarifas.ObtenerTiempoDigitalizacionArchivo(guia.IdCiudadOrigen, guia.IdCiudadDestino);
                ADValidacionServicioTrayectoDestino validacionTiempoDigitalizacionArchivo = new ADValidacionServicioTrayectoDestino();
                DateTime fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(guia.FechaEstimadaEntrega, ref validacionTiempoDigitalizacionArchivo, tiempos.numeroDiasDigitalizacion, (guia.DiasDeEntrega * 24));
                guia.FechaEstimadaDigitalizacion = fechaActual.AddHours(validacionTiempoDigitalizacionArchivo.NumeroHorasDigitalizacion);
                ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionTiempoDigitalizacionArchivo, tiempos.numeroDiasArchivo, validacionTiempoDigitalizacionArchivo.NumeroHorasDigitalizacion);
                guia.FechaEstimadaArchivo = fechaActual.AddHours(validacionTiempoDigitalizacionArchivo.NumeroHorasArchivo);
            }
            long idAdmisionMensajeria = ADRepositorio.Instancia.AdicionarAdmision(guia, remitenteDestinatario, conexion, transaccion);
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
                    ADRepositorio.Instancia.AdicionarMovimientoInventario(newMovInv, conexion, transaccion);
                    break;
                default:
                    break;
            }



            // TODO ID, Se quita la insersion en la tabla AdmisionRotulos_MEN
            if (guia.TotalPiezas > 1)
            {
                ADRepositorio.Instancia.AdicionarRotulosAdmision(guia.TotalPiezas, guia.NumeroGuia, idAdmisionMensajeria, conexion, transaccion);
            }

            // Acá se debe validar si se debe generar ingreso a centro de acopio, esto debe aplicar solo para admisión manual COL

            if (validaIngresoACentroAcopio && agenciaRegistraAdmision.HasValue)
            {
                ADRepositorio.Instancia.IngresarGuiaManualCentroAcopio(guia, conexion, transaccion);

                //    if (fachadaOperacionUrbana.GuiaYaFueIngresadaACentroDeAcopio(guia.NumeroGuia, agenciaRegistraAdmision.Value)
                //      ||fachadaOperacionNacional.GuiaYaFueIngresadaACentroDeAcopio(guia.NumeroGuia, agenciaRegistraAdmision.Value))
                //    {
                //        PUCentroServiciosDC cs = fachadaCentroServicio.ObtenerCentroServicio(agenciaRegistraAdmision.Value);
                //        fachadaOperacionUrbana.CambiarEstado(new Servidor.Servicios.ContratoDatos.OperacionUrbana.OUGuiaIngresadaDC
                //        {
                //            NumeroGuia = guia.NumeroGuia,
                //            IdAdmision = idAdmisionMensajeria,
                //            Observaciones = guia.Observaciones,
                //            IdCiudad = cs.CiudadUbicacion.IdLocalidad,
                //            Ciudad = cs.CiudadUbicacion.Nombre
                //        }, ADEnumEstadoGuia.EnCentroAcopio);
                //    }

            }

            if (!string.IsNullOrEmpty(guia.NumeroBolsaSeguridad))
            {
                //GuardarConsumoBolsaSeguridad(guia);
            }

            // Se insertan las formas de pago
            ADRepositorio.Instancia.AdicionarGuiaFormasPago(idAdmisionMensajeria, guia.FormasPago, ControllerContext.Current.Usuario, conexion, transaccion);

            // Se insertan los valores adicionales
            ADRepositorio.Instancia.AdicionarValoresAdicionales(idAdmisionMensajeria, guia.ValoresAdicionales, ControllerContext.Current.Usuario, conexion, transaccion);

            // Con base en el tipo de cliente se inserta en las tablas relacionadas
            switch (guia.TipoCliente)
            {
                case ADEnumTipoCliente.CCO:

                    // La factura se le debe cargar al remitente
                    remitenteDestinatario.FacturaRemitente = true;
                    ADRepositorio.Instancia.AdicionarConvenioConvenio(idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, guia.IdCliente, conexion, transaccion);
                    break;

                case ADEnumTipoCliente.CPE:
                    ADRepositorio.Instancia.AdicionarConvenioPeaton(idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, guia.IdCliente, conexion, transaccion);
                    break;

                case ADEnumTipoCliente.PCO:
                    ADRepositorio.Instancia.AdicionarPeatonConvenio(guia.IdCliente, idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, conexion, transaccion);
                    break;

                case ADEnumTipoCliente.PPE:
                    ADRepositorio.Instancia.AdicionarPeatonPeaton(idAdmisionMensajeria, remitenteDestinatario, ControllerContext.Current.Usuario, guia.IdCentroServicioOrigen, guia.NombreCentroServicioOrigen, conexion, transaccion);
                    break;
            }
            return idAdmisionMensajeria;
        }
        /// <summary>
        /// Audita la guia cuando la fecha estimada de entrega está mal calculada
        /// </summary>
        /// <param name="guia"></param>
        internal static void AuditaFechaEstimadaEntregaMalCalculada(ADGuia guia)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("/******************************************************************/");

                        sb.AppendLine("Fecha Estimada Entrega mal calculada.   FechaAuditoria: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " NumeroGuia: " + guia.NumeroGuia);

                        sb.AppendLine("--Ciudad Origen---");
                        sb.AppendLine(guia.IdCiudadOrigen + " -- " + guia.NombreCiudadOrigen);

                        sb.AppendLine("--Ciudad Destino---");
                        sb.AppendLine(guia.IdCiudadDestino + " -- " + guia.NombreCiudadDestino);

                        string guiaEntradaSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(guia);

                        sb.AppendLine(guiaEntradaSerializado);

                        sb.AppendLine("/******************************************************************/");

                        string pathAuditoriaAdmisionesController = @"C:\ControllerAuditoriaFechaEstEntregaAdmision\";
                        string file = pathAuditoriaAdmisionesController + "auditoria-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                        File.AppendAllText(file, sb.ToString());
                    }
                    catch { }
                });
        }


        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        internal ADResultadoAdmision RegistrarGuiaManualRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {


                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);

                // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
                if (guia.IdMensajero == 0)
                {
                    RegistrarComisionesYMovimientosCaja(guia, idCaja);
                }
                else
                {
                    AdicionarMovimientoMensajero(guia);
                }

                GuardarConsumoGuia(guia);

                // Se adiciona el rapiradicado
                ADRepositorio.Instancia.AdicionarRapiRadicado(idAdmisionMensajeria, rapiRadicado, ControllerContext.Current.Usuario);

                IntegrarConMensajero(guia, remitenteDestinatario);

                transaccion.Complete();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
            }
        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        internal ADResultadoAdmision RegistrarGuiaManualRapiRadicadoCOL(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado, long idAgenciaRegistraAdmision)
        {
            int idCaja = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, true, idAgenciaRegistraAdmision);

                // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
                if (guia.IdMensajero == 0)
                {
                    RegistrarComisionesYMovimientosCaja(guia, idCaja);
                }
                else
                {
                    AdicionarMovimientoMensajero(guia);
                }

                GuardarConsumoGuia(guia);

                // Se adiciona el rapiradicado
                ADRepositorio.Instancia.AdicionarRapiRadicado(idAdmisionMensajeria, rapiRadicado, ControllerContext.Current.Usuario);

                IntegrarConMensajero(guia, remitenteDestinatario);

                transaccion.Complete();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
            }
        }

        /// <summary>
        /// Registra el movimiento cuando la guía pertenece a un mensajero
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="fachadaCajas"></param>
        public void AdicionarMovimientoMensajero(ADGuia guia)
        {
            // Se deben calcular las comisiones de ventas
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();
            CMComisionXVentaCalculadaDC comision = CalcularComisionesPorVentas(guia, fachadaComisiones);
            fachadaComisiones.GuardarComision(comision);

            if (!guia.FormasPago.Exists(f => f.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO))
            {
                ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

                fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC()
                {
                    ConceptoEsIngreso = true,
                    FechaGrabacion = DateTime.Now,
                    ConceptoCajaMensajero = new CAConceptoCajaDC()
                    {
                        IdConceptoCaja = guia.IdConceptoCaja
                    },
                    Mensajero = new Servidor.Servicios.ContratoDatos.OperacionUrbana.OUNombresMensajeroDC()
                    {
                        IdPersonaInterna = guia.IdMensajero,
                        NombreApellido = guia.NombreMensajero
                    },
                    NumeroDocumento = guia.NumeroGuia,
                    Valor = guia.ValorTotal,
                    UsuarioRegistro = ControllerContext.Current.Usuario
                });
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
            SUNumeradorPrefijo numeroSuministro = ObtenerConsecutivoFacturaVenta();
            guia.NumeroGuia = numeroSuministro.ValorActual;
            guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;

            ADRepositorio.Instancia.AuditarGuiaGeneradaArchivoTexto(guia, idCaja, remitenteDestinatario, rapiRadicado);

            SqlConnection conexion = new SqlConnection(conexionStringController);
            SqlTransaction transaccion = null;
            try
            {

                conexion.Open();
                transaccion = conexion.BeginTransaction();

                RegistrarComisionesYMovimientosCaja(guia, idCaja, conexion, transaccion);

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                guia.FechaAdmision = DateTime.Now;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, conexion, transaccion);
                GuardarConsumoGuiaAutomatica(guia, conexion, transaccion);

                // Se adiciona el rapiradicado
                ADRepositorio.Instancia.AdicionarRapiRadicado(idAdmisionMensajeria, rapiRadicado, ControllerContext.Current.Usuario, conexion, transaccion);

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

                transaccion.Commit();
                return resultado;
            }
            catch
            {
                if (transaccion != null)
                    transaccion.Rollback();
                ADRepositorio.Instancia.GuardarNumeroFacturaFallido(guia.NumeroGuia, guia.PrefijoNumeroGuia);
                throw;
            }
            finally
            {
                if (conexion.State != ConnectionState.Closed)
                    conexion.Close();
                conexion.Dispose();
            }

        }

        /// <summary>
        /// Retorna el consecutivo del número de factura de venta
        /// </summary>
        /// <returns></returns>
        public SUNumeradorPrefijo ObtenerConsecutivoFacturaVenta()
        {
            // Se obtiene el número de guía

            SUNumeradorPrefijo numeroSuministro = ADRepositorio.Instancia.ObtenerNumeroFacturaAutomatica();

            /*ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            SUNumeradorPrefijo numeroSuministro;
            using (TransactionScope tx = new TransactionScope())
            {

                numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.FACTURA_VENTA_MENSAJERIA_AUTOMATICA);
                tx.Complete();
            }*/
            return numeroSuministro;
        }

        /// <summary>
        /// Registra las comisiones y movimientos de caja respectivos
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        private void RegistrarComisionesYMovimientosCaja(ADGuia guia, int idCaja)
        {
            // Se deben calcular las comisiones de ventas
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();
            CMComisionXVentaCalculadaDC comision = CalcularComisionesPorVentas(guia, fachadaComisiones);
            fachadaComisiones.GuardarComision(comision);

            // La primera validación que se debe hacer, es verificar que la caja esté abierta
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
            CARegistroTransacCajaDC registroOrigen = null;

            // Se adiciona el movimiento de caja
            long idMovimientoCaja = AdicionarMovimientoCaja(idCaja, fachadaCajas, comision, guia, out registroOrigen);

            // Si la forma de pago es "Al Cobro" se debe registrar la comisión para el centro de servicio de destino
            //if (guia.FormasPago.Count == 1 && guia.FormasPago.First().IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO)
            //{
            //  Servidor.Servicios.ContratoDatos.GestionCajas.CAOperaRacolBancoEmpresaDC infoTransaccion = new Servidor.Servicios.ContratoDatos.GestionCajas.CAOperaRacolBancoEmpresaDC()
            //  {
            //    RegistroCentroServicio = registroOrigen,
            //    RegistroCentrSvcMenor = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            //    {
            //      InfoAperturaCaja = new CAAperturaCajaDC()
            //      {
            //        IdCaja = 0,
            //        IdCodigoUsuario = guia.IdCodigoUsuario,
            //      },
            //      TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
            //      IdCentroResponsable = comisionDestino.IdCentroServicioResponsable,
            //      IdCentroServiciosVenta = comisionDestino.IdCentroServicioVenta,
            //      NombreCentroResponsable = comisionDestino.NombreCentroServicioResponsable,
            //      NombreCentroServiciosVenta = comisionDestino.NombreCentroServicioVenta,
            //      RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
            //      {
            //        new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC()
            //        {
            //           Cantidad = 1,
            //           ConceptoEsIngreso = true,
            //           EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
            //           FechaFacturacion = DateTime.Now,
            //           Numero = guia.NumeroGuia,
            //           NumeroFactura = guia.NumeroGuia.ToString(),
            //           Observacion = guia.Observaciones,
            //           ValorDeclarado = guia.ValorDeclarado,
            //           ValoresAdicionales = guia.ValorAdicionales,
            //           ValorImpuestos = guia.ValorTotalImpuestos,
            //           ValorPrimaSeguros = guia.ValorPrimaSeguro,
            //           ValorRetenciones = guia.ValorTotalRetenciones,
            //           ValorServicio = guia.ValorServicio,
            //           ValorTercero = 0
            //        }
            //      },
            //      ValorTotal = guia.ValorAdmision + guia.ValorPrimaSeguro + guia.ValorAdicionales,
            //      TotalImpuestos = guia.ValorTotalImpuestos,
            //      TotalRetenciones = guia.ValorTotalRetenciones,
            //      Usuario = ControllerContext.Current.Usuario,
            //      RegistroVentaFormaPago = guia.FormasPago.ConvertAll(formaPago => new CARegistroVentaFormaPagoDC
            //      {
            //        Valor = formaPago.Valor,
            //        IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
            //        Descripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
            //        NumeroAsociado = formaPago.NumeroAsociadoFormaPago
            //      })
            //    }
            //  };
            //  long idTransaccionDestino = 0;
            //  fachadaCajas.RegistroTransaccionCentroSvcCentroSvc(infoTransaccion, idMovimientoCaja, out idTransaccionDestino);
            //}
            //else
            //if (guia.FormasPago.Count(f => f.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO) > 0)
            //{
            //    CMComisionXVentaCalculadaDC comisionDestino = fachadaComisiones.CalcularComisionesxVentas(
            //    new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
            //    {
            //        IdCentroServicios = guia.IdCentroServicioDestino,
            //        IdServicio = guia.IdServicio,
            //        TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Entregar,
            //        ValorBaseComision = guia.ValorServicio,
            //        NumeroOperacion = guia.NumeroGuia
            //    });

            //    comisionDestino.EsRegistroValido = false;

            //  Servidor.Servicios.ContratoDatos.GestionCajas.CAOperaRacolBancoEmpresaDC infoTransaccion = new Servidor.Servicios.ContratoDatos.GestionCajas.CAOperaRacolBancoEmpresaDC()
            //  {
            //    RegistroCentroServicio = registroOrigen,
            //    RegistroCentrSvcMenor = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            //    {
            //      InfoAperturaCaja = new CAAperturaCajaDC()
            //      {
            //        IdCaja = 0,
            //        IdCodigoUsuario = guia.IdCodigoUsuario,
            //      },
            //      TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
            //      IdCentroResponsable = comisionDestino.IdCentroServicioResponsable,
            //      IdCentroServiciosVenta = comisionDestino.IdCentroServicioVenta,
            //      NombreCentroResponsable = comisionDestino.NombreCentroServicioResponsable,
            //      NombreCentroServiciosVenta = comisionDestino.NombreCentroServicioVenta,
            //      RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
            //      {
            //        new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC()
            //        {
            //           Cantidad = 1,
            //           ConceptoEsIngreso = true,
            //           EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
            //           FechaFacturacion = DateTime.Now,
            //           Numero = guia.NumeroGuia,
            //           NumeroFactura = guia.NumeroGuia.ToString(),
            //           Observacion = guia.Observaciones,
            //           ValorDeclarado = 0,
            //           ValoresAdicionales = 0,
            //           ValorImpuestos = 0,
            //           ValorPrimaSeguros = 0,
            //           ValorRetenciones = 0,
            //           ValorServicio = guia.FormasPago.First(f=>f.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO).Valor,
            //           ValorTercero = 0
            //        }
            //      },
            //      ValorTotal = guia.FormasPago.First(f => f.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO).Valor,
            //      TotalImpuestos = 0,
            //      TotalRetenciones = 0,
            //      Usuario = ControllerContext.Current.Usuario,
            //      RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>()
            //      {
            //        new CARegistroVentaFormaPagoDC
            //        {
            //          Valor = guia.FormasPago.First(f => f.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO).Valor,
            //          IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
            //          Descripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
            //          NumeroAsociado = guia.FormasPago.First(f => f.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO).NumeroAsociadoFormaPago
            //        }
            //      }
            //    }
            //  };
            //  long idTransaccionDestino = 0;
            //  fachadaCajas.RegistroTransaccionCentroSvcCentroSvc(infoTransaccion, idMovimientoCaja, out idTransaccionDestino);
            //}
        }

        /// <summary>
        /// Registra las comisiones y movimientos de caja respectivos
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        private void RegistrarComisionesYMovimientosCaja(ADGuia guia, int idCaja, SqlConnection conexion, SqlTransaction transaccion)
        {
            // Se deben calcular las comisiones de ventas
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();
            CMComisionXVentaCalculadaDC comision = CalcularComisionesPorVentas(guia, fachadaComisiones, conexion, transaccion);
            fachadaComisiones.GuardarComision(comision, conexion, transaccion);

            // La primera validación que se debe hacer, es verificar que la caja esté abierta
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
            CARegistroTransacCajaDC registroOrigen = null;

            // Se adiciona el movimiento de caja
            long idMovimientoCaja = AdicionarMovimientoCaja(idCaja, fachadaCajas, comision, guia, out registroOrigen, conexion, transaccion);

        }


        /// <summary>
        /// Calcula las comisiones de venta para la admisión registrada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="fachadaComisiones"></param>
        /// <returns></returns>
        private CMComisionXVentaCalculadaDC CalcularComisionesPorVentas(ADGuia guia, ICMFachadaComisiones fachadaComisiones, SqlConnection conexion, SqlTransaction transaccion)
        {
            CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentas(
              new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
              {
                  IdCentroServicios = (int)guia.IdCentroServicioOrigen,
                  IdServicio = guia.IdServicio,
                  TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Vender,
                  ValorBaseComision = guia.ValorServicio,
                  NumeroOperacion = guia.NumeroGuia,
              }, conexion, transaccion);

            return comision;
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
        public void TransaccionCajaContado(ADGuia guia, int idCaja)
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

            // Se deben calcular las comisiones de ventas
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();
            CMComisionXVentaCalculadaDC comision = CalcularComisionesPorVentas(guia, fachadaComisiones);

            // Se adiciona el movimiento de caja
            CARegistroTransacCajaDC registroCaja = null;
            AdicionarMovimientoCaja(idCaja, fachadaCajas, comision, guia, out registroCaja);
            fachadaComisiones.GuardarComision(comision);
        }

        /// <summary>
        /// Adiciona un movimiento de caja para la transacción de guia creada
        /// </summary>
        /// <param name="idCaja"></param>
        /// <param name="fachadaCajas"></param>
        /// <param name="comision"></param>
        private long AdicionarMovimientoCaja(int idCaja, ICAFachadaCajas fachadaCajas, CMComisionXVentaCalculadaDC comision, ADGuia guia, out CARegistroTransacCajaDC registro)
        {
            registro = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                  {
                      IdCaja = idCaja,
                      IdCodigoUsuario = guia.IdCodigoUsuario,
                  },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
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
            return fachadaCajas.AdicionarMovimientoCaja(registro).IdTransaccionCaja;
        }
        /// <summary>
        /// Adiciona un movimiento de caja para la transacción de guia creada
        /// </summary>
        /// <param name="idCaja"></param>
        /// <param name="fachadaCajas"></param>
        /// <param name="comision"></param>
        private long AdicionarMovimientoCaja(int idCaja, ICAFachadaCajas fachadaCajas, CMComisionXVentaCalculadaDC comision, ADGuia guia, out CARegistroTransacCajaDC registro, SqlConnection conexion, SqlTransaction transaccion)
        {
            registro = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = idCaja,
                    IdCodigoUsuario = guia.IdCodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
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
            return fachadaCajas.AdicionarMovimientoCaja(registro, conexion, transaccion).IdTransaccionCaja;
        }

        /// <summary>
        /// Registra guia cuyo servicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        internal ADResultadoAdmision RegistrarGuiaManualNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
                if (guia.IdMensajero == 0)
                {
                    RegistrarComisionesYMovimientosCaja(guia, idCaja);
                }
                else
                {
                    AdicionarMovimientoMensajero(guia);
                }

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);

                GuardarConsumoGuia(guia);

                ADAdmisionNotificacion.Instancia.AdicionarNotificacion(idAdmisionMensajeria, notificacion);

                IntegrarConMensajero(guia, remitenteDestinatario, notificacion);

                transaccion.Complete();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
            }
        }

        /// <summary>
        /// Registra guia cuyo servicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        internal ADResultadoAdmision RegistrarGuiaManualNotificacionCOL(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion, long idAgenciaRegistraAdmision)
        {
            int idCaja = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
                if (guia.IdMensajero == 0)
                {
                    RegistrarComisionesYMovimientosCaja(guia, idCaja);
                }
                else
                {
                    AdicionarMovimientoMensajero(guia);
                }

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, true, idAgenciaRegistraAdmision);

                GuardarConsumoGuia(guia);

                ADAdmisionNotificacion.Instancia.AdicionarNotificacion(idAdmisionMensajeria, notificacion);

                IntegrarConMensajero(guia, remitenteDestinatario, notificacion);

                transaccion.Complete();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
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
            SUNumeradorPrefijo numeroSuministro = ObtenerConsecutivoFacturaVenta();
            guia.NumeroGuia = numeroSuministro.ValorActual;
            guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;

            ADRepositorio.Instancia.AuditarGuiaGeneradaArchivoTexto(guia, idCaja, remitenteDestinatario, notificacion);

            SqlConnection conexion = new SqlConnection(conexionStringController);
            SqlTransaction transaccion = null;

            try
            {
                conexion.Open();
                transaccion = conexion.BeginTransaction();

                RegistrarComisionesYMovimientosCaja(guia, idCaja, conexion, transaccion);

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                guia.FechaAdmision = DateTime.Now;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, conexion, transaccion);
                ADAdmisionNotificacion.Instancia.AdicionarNotificacion(idAdmisionMensajeria, notificacion, conexion, transaccion);
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


                transaccion.Commit();
                conexion.Close();

                return resultado;

            }
            catch
            {
                if (transaccion != null)
                    transaccion.Rollback();
                ADRepositorio.Instancia.GuardarNumeroFacturaFallido(guia.NumeroGuia, guia.PrefijoNumeroGuia);
                throw;
            }
            finally
            {
                if (conexion.State != ConnectionState.Closed)
                    conexion.Close();
                conexion.Dispose();
            }
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        internal ADResultadoAdmision RegistrarGuiaManualRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
                if (guia.IdMensajero == 0)
                {
                    RegistrarComisionesYMovimientosCaja(guia, idCaja);
                }
                else
                {
                    AdicionarMovimientoMensajero(guia);
                }

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);

                GuardarConsumoGuia(guia);

                ADAdmisionRapiEnvioContraPago.Instancia.AdicionarRapiEnvioContraPago(idAdmisionMensajeria, rapiEnvioContraPago);

                IntegrarConMensajero(guia, remitenteDestinatario);

                transaccion.Complete();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
            }
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        internal ADResultadoAdmision RegistrarGuiaManualRapiEnvioContraPagoCOL(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago, long idAgenciaRegistraAdmision)
        {
            int idCaja = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
                if (guia.IdMensajero == 0)
                {
                    RegistrarComisionesYMovimientosCaja(guia, idCaja);
                }
                else
                {
                    AdicionarMovimientoMensajero(guia);
                }

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, true, idAgenciaRegistraAdmision);

                GuardarConsumoGuia(guia);

                ADAdmisionRapiEnvioContraPago.Instancia.AdicionarRapiEnvioContraPago(idAdmisionMensajeria, rapiEnvioContraPago);

                IntegrarConMensajero(guia, remitenteDestinatario);

                transaccion.Complete();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
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
            SUNumeradorPrefijo numeroSuministro = ObtenerConsecutivoFacturaVenta();
            guia.NumeroGuia = numeroSuministro.ValorActual;
            guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;

            ADRepositorio.Instancia.AuditarGuiaGeneradaArchivoTexto(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago);


            SqlConnection conexion = new SqlConnection(conexionStringController);
            SqlTransaction transaccion = null;

            try
            {
                conexion.Open();
                transaccion = conexion.BeginTransaction();


                RegistrarComisionesYMovimientosCaja(guia, idCaja, conexion, transaccion);

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                guia.FechaAdmision = DateTime.Now;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, conexion, transaccion);
                ADAdmisionRapiEnvioContraPago.Instancia.AdicionarRapiEnvioContraPago(idAdmisionMensajeria, rapiEnvioContraPago, conexion, transaccion);
                GuardarConsumoGuiaAutomatica(guia, conexion, transaccion);
                // IntegrarConMensajero(guia, remitenteDestinatario);

                transaccion.Commit();

                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
            }

            catch
            {
                if (transaccion != null)
                    transaccion.Rollback();
                ADRepositorio.Instancia.GuardarNumeroFacturaFallido(guia.NumeroGuia, guia.PrefijoNumeroGuia);
                throw;
            }
            finally
            {
                if (conexion.State != ConnectionState.Closed)
                    conexion.Close();
                conexion.Dispose();
            }

        }

        /// <summary>
        /// Registra guía manual de col
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        //internal ADResultadoAdmision RegistrarGuiaManualCOL(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, long idAgenciaRegistraAdmision)
        //{
        //    // El id de la caja debe ser 0 para registro de guía manual por un COL
        //    int idCaja = 0;
        //    SqlConnection conexion = new SqlConnection(conexionStringController);
        //    SqlTransaction transaccion = null;

        //    try
        //    {
        //        conexion.Open();
        //        transaccion = conexion.BeginTransaction();


        //        // Se adiciona la admisión
        //        guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
        //        long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario,conexion, transaccion, true, idAgenciaRegistraAdmision);
        //        guia.IdAdmision = idAdmisionMensajeria;

        //        // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
        //        if (guia.IdMensajero == 0)
        //        {
        //            RegistrarComisionesYMovimientosCaja(guia, idCaja);
        //        }
        //        else
        //        {
        //            AdicionarMovimientoMensajero(guia);
        //        }

        //        GuardarConsumoGuia(guia);

        //        IntegrarConMensajero(guia, remitenteDestinatario);
        //        transaccion.Commit();
        //        return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
        //        }
        //    catch
        //    {
        //        if (transaccion != null)
        //            transaccion.Rollback();
        //        ADRepositorio.Instancia.GuardarNumeroFacturaFallido(guia.NumeroGuia, guia.PrefijoNumeroGuia);
        //        throw;
        //    }
        //    finally
        //    {
        //        if (conexion.State != ConnectionState.Closed)
        //            conexion.Close();
        //        conexion.Dispose();
        //    }
        //}


        internal ADResultadoAdmision RegistrarGuiaManualCOL(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, long idAgenciaRegistraAdmision)
        {
            // El id de la caja debe ser 0 para registro de guía manual por un COL
            int idCaja = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {


                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, true, idAgenciaRegistraAdmision);
                guia.IdAdmision = idAdmisionMensajeria;

                // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
                if (guia.IdMensajero == 0)
                {
                    RegistrarComisionesYMovimientosCaja(guia, idCaja);
                }
                else
                {
                    AdicionarMovimientoMensajero(guia);
                }

                GuardarConsumoGuia(guia);

                IntegrarConMensajero(guia, remitenteDestinatario);

                transaccion.Complete();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
            }
        }

        /// <summary>
        /// Registra guía manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        internal ADResultadoAdmision RegistrarGuiaManual(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            if (string.IsNullOrWhiteSpace(guia.NombreCentroServicioDestino))
            {

                try
                {
                    if (guia.IdCentroServicioDestino > 0)
                    {
                        PUCentroServiciosDC cenSrvDestino = fachadaCentroServicio.ObtenerCentroServicio(guia.IdCentroServicioDestino);
                        guia.NombreCentroServicioDestino = cenSrvDestino.Nombre;
                    }
                    else
                    {
                        PUCentroServiciosDC centroSerDestino = fachadaCentroServicio.ObtenerAgenciaLocalidad(guia.IdCiudadDestino);
                        guia.IdCentroServicioDestino = centroSerDestino.IdCentroServicio;
                        guia.NombreCentroServicioDestino = centroSerDestino.Nombre;
                    }
                }
                catch (Exception ex)
                {
                    ///si ocurre un error no debe afectar el proceso de admision, la insercion saca excepcion si no se envia el nombre del centro de servicio de destino
                }
                //throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO.ToString(), "El campo NombreCentroServicioDestino no puede estar vacio."));
            }

            //TODO CED: habilitar transaccion
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);
                guia.IdAdmision = idAdmisionMensajeria;

                // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
                if (guia.IdMensajero == 0)
                {
                    RegistrarComisionesYMovimientosCaja(guia, idCaja);
                }
                else
                {

                    //buscartipomenssajero
                    //if(Mesajeropam consume  RegistrarComisionesYMovimientosCaja(guia, idCaja);
                    //si no AdicionarMovimientoMensajero(guia);

                    AdicionarMovimientoMensajero(guia);
                }

                GuardarConsumoGuia(guia);

                IntegrarConMensajero(guia, remitenteDestinatario);

                transaccion.Complete();
                return new ADResultadoAdmision
                {
                    NumeroGuia = guia.NumeroGuia,
                    IdAdmision = guia.IdAdmision
                };
            }
        }

        /// <summary>
        /// Registra guía manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        internal ADResultadoAdmision RegistrarGuiaManualInternacionalCOL(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, long idAgenciaRegistraAdmision)
        {
            int idCaja = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {
                // La primera validación que se debe hacer, es verificar que la caja esté abierta
                ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

                // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
                if (guia.IdMensajero == 0)
                {
                    RegistrarComisionesYMovimientosCaja(guia, idCaja);
                }
                else
                {
                    AdicionarMovimientoMensajero(guia);
                }

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, true, idAgenciaRegistraAdmision);

                GuardarConsumoGuia(guia);

                ADAdmisionInternacional.Instancia.AdicionarAdmisionTipoEmpaque(idAdmisionMensajeria, tipoEmpaque);

                IntegrarConMensajero(guia, remitenteDestinatario);

                transaccion.Complete();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
            }
        }

        /// <summary>
        /// Registra guía manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        internal ADResultadoAdmision RegistrarGuiaManualInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Si es un centro de servicio el dueño de la operación debe adicionar el movimiento de caja y calcular las comisiones por venta
                if (guia.IdMensajero == 0)
                {
                    RegistrarComisionesYMovimientosCaja(guia, idCaja);
                }
                else
                {
                    AdicionarMovimientoMensajero(guia);
                }

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);

                GuardarConsumoGuia(guia);

                ADAdmisionInternacional.Instancia.AdicionarAdmisionTipoEmpaque(idAdmisionMensajeria, tipoEmpaque);

                IntegrarConMensajero(guia, remitenteDestinatario);

                transaccion.Complete();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
            }
        }

        /// <summary>
        /// Registra admisión internacional automática
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="tipoEmpaque"></param>
        /// <returns></returns>
        internal ADResultadoAdmision RegistrarGuiaAutomaticaInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque)
        {
            SUNumeradorPrefijo numeroSuministro = ObtenerConsecutivoFacturaVenta();

            guia.NumeroGuia = numeroSuministro.ValorActual;
            guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;

            using (TransactionScope transaccion = new TransactionScope())
            {
                RegistrarComisionesYMovimientosCaja(guia, idCaja);

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                guia.FechaAdmision = DateTime.Now;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario);
                ADAdmisionInternacional.Instancia.AdicionarAdmisionTipoEmpaque(idAdmisionMensajeria, tipoEmpaque);
                GuardarConsumoGuiaAutomatica(guia);
                IntegrarConMensajero(guia, remitenteDestinatario);
                transaccion.Complete();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
            }
        }

        //todo:id Graba Guia Internacional DHL
        /// <summary>
        /// Registra admisión internacional automática con DHL
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="tipoEmpaque"></param>
        /// <returns></returns>
        internal ADResultadoAdmision RegistrarGuiaAutomaticaInternacional_DHL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, ADGuiaInternacionalDC guiaInternacional)
        {
            SUNumeradorPrefijo numeroSuministro = ObtenerConsecutivoFacturaVenta();
            guia.NumeroGuia = numeroSuministro.ValorActual;
            guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;


            ADRepositorio.Instancia.AuditarGuiaGeneradaArchivoTexto(guia, idCaja, remitenteDestinatario, tipoEmpaque);

            SqlConnection conexion = new SqlConnection(conexionStringController);
            SqlTransaction transaccion = null;

            try
            {
                conexion.Open();
                transaccion = conexion.BeginTransaction();

                RegistrarComisionesYMovimientosCaja(guia, idCaja, conexion, transaccion);

                // Se adiciona la admisión
                guia.EstadoGuia = ADEnumEstadoGuia.Admitida;
                guia.FechaAdmision = DateTime.Now;
                long idAdmisionMensajeria = RegistrarGuia(guia, remitenteDestinatario, conexion, transaccion);

                //ADAdmisionInternacional.Instancia.AdicionarAdmisionTipoEmpaque(idAdmisionMensajeria, tipoEmpaque);

                guiaInternacional.IdAdmision = idAdmisionMensajeria;
                guiaInternacional.NumeroGuia = guia.NumeroGuia;
                guiaInternacional.NumeroGuiaDHL = guia.NumeroGuiaDHL;
                guiaInternacional.IdTipoEmpaque = tipoEmpaque.IdTipoEmpaque;
                guiaInternacional.TipoEmpaqueNombre = tipoEmpaque.Descripcion;

                ADAdmisionInternacional.Instancia.AdicionarAdmisionInternacional(guiaInternacional, conexion, transaccion);

                GuardarConsumoGuiaAutomatica(guia, conexion, transaccion);


                transaccion.Commit();
                return new ADResultadoAdmision { NumeroGuia = guia.NumeroGuia };
            }
            catch
            {
                if (transaccion != null)
                    transaccion.Rollback();
                ADRepositorio.Instancia.GuardarNumeroFacturaFallido(guia.NumeroGuia, guia.PrefijoNumeroGuia);
                throw;
            }
            finally
            {
                if (conexion.State != ConnectionState.Closed)
                    conexion.Close();
                conexion.Dispose();
            }

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
                PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(guia.IdCentroServicioOrigen);
                SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), centroServ.Tipo);

                consumo.GrupoSuministro = grupo;
                consumo.IdDuenoSuministro = guia.IdCentroServicioOrigen;
            }
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            fachadaSuministros.GuardarConsumoSuministro(consumo);
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

        /// <summary>
        /// Almacena el consumo de la bolsa de seguridad asociada a la guía
        /// </summary>
        /// <param name="guia"></param>
        public void GuardarConsumoBolsaSeguridad(ADGuia guia)
        {
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

            int i = 0;
            string prefijo = "";
            long NumeroBolsaSeguridad = 0;
            while (true)
            {
                prefijo = guia.NumeroBolsaSeguridad.Substring(0, i);
                if (long.TryParse(guia.NumeroBolsaSeguridad.Substring(prefijo.Length), out NumeroBolsaSeguridad))
                    break;
                i++;
                if (prefijo == guia.NumeroBolsaSeguridad)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_BOLSA_NOVALIDA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_BOLSA_NOVALIDA)));
            }

            SUSuministro suministroAsociado = null;
            if (prefijo != "")
                suministroAsociado = fachadaSuministros.ConsultarSuministroxPrefijo(prefijo);

            if (suministroAsociado != null)
            {
                SUPropietarioGuia propietario = fachadaSuministros.ObtenerPropietarioSuministro(NumeroBolsaSeguridad, (SUEnumSuministro)Enum.ToObject(typeof(SUEnumSuministro), suministroAsociado.Id), guia.IdCentroServicioOrigen);

                SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), propietario.CentroServicios.Tipo);
                SUConsumoSuministroDC consumo = null;
                if (guia.IdMensajero <= 0)
                {
                    consumo = new SUConsumoSuministroDC()
                    {
                        Cantidad = 1,
                        EstadoConsumo = SUEnumEstadoConsumo.CON,
                        IdServicioAsociado = guia.IdServicio,
                        GrupoSuministro = grupo,
                        IdDuenoSuministro = guia.IdCentroServicioOrigen,
                        NumeroSuministro = NumeroBolsaSeguridad,
                        Suministro = (SUEnumSuministro)Enum.ToObject(typeof(SUEnumSuministro), suministroAsociado.Id)
                    };
                }
                else
                {
                    consumo = new SUConsumoSuministroDC()
                    {
                        Cantidad = 1,
                        EstadoConsumo = SUEnumEstadoConsumo.CON,
                        IdServicioAsociado = guia.IdServicio,
                        GrupoSuministro = SUEnumGrupoSuministroDC.MEN,
                        IdDuenoSuministro = guia.IdMensajero,
                        NumeroSuministro = NumeroBolsaSeguridad,
                        Suministro = (SUEnumSuministro)Enum.ToObject(typeof(SUEnumSuministro), suministroAsociado.Id)
                    };
                }
                fachadaSuministros.GuardarConsumoSuministro(consumo);
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_BOLSA_NOVALIDA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_BOLSA_NOVALIDA)));
            }
        }

        /// <summary>
        /// Adiciona una guía anulada. Se usa para la parte de anulación de una guía. Se espera uqe se pase el id del centro de servicio de origen y el número de la guía.
        /// </summary>
        /// <param name="guia"></param>
        public long AdicionarAdmisionAnulada(ADGuia guia)
        {
            guia.IdAdmision = ADRepositorio.Instancia.AdicionarAdmisionAnulada(guia);
            GuardarConsumoGuia(guia);
            return guia.IdAdmision;
        }

        /// <summary>
        /// Audita todas las admisiones automaticas generadas
        /// </summary>
        public void GuardarAuditoriaGrabacionAdmisionMensajeria(int idCaja, string metodoEjecutado, ADRetornoAdmision retorno, ADGuia guiaEntrada, ADMensajeriaTipoCliente tipoCliente, object objetoAdicional = null)
        {
            string usuario = ControllerContext.Current != null ? ControllerContext.Current.Usuario : "NoUsuario";



            System.Threading.Tasks.Task tRetornoAdm = new System.Threading.Tasks.Task(() =>
            {
                try
                {

                    StringBuilder sb = new StringBuilder();

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


                    sb.AppendLine("/******************************************************************/");

                    sb.AppendLine("Retorno Admisión.   FechaAuditoria: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "   Caja: " + idCaja.ToString() + "   NumeroGuia: " + retorno.NumeroGuia);
                    sb.AppendLine("--Objeto RetornoAdmision response ---");
                    sb.AppendLine(retornoSerializado);
                    sb.AppendLine("--Objeto admision request---");
                    sb.AppendLine(guiaEntradaSerializado);
                    sb.AppendLine("--Objeto tipoCliente---");
                    sb.AppendLine(tipoClienteSerializado);
                    sb.AppendLine("--Objeto adicional---");
                    sb.AppendLine(objetoAdicionalSerializado);
                    sb.AppendLine("/******************************************************************/");

                    string pathAuditoriaAdmisionesController = @"C:\ControllerAuditoriaRetornoAdmision\";
                    string file = pathAuditoriaAdmisionesController + "auditoria-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                    File.AppendAllText(file, sb.ToString());

                }
                catch
                {
                }
            });
            tRetornoAdm.Start();



            //guarda auditoria en base de datos
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

                        ADRepositorio.Instancia.GuardarAuditoriaGrabacionAdmisionMensajeria(idCaja, metodoEjecutado, retornoSerializado, guiaEntradaSerializado, tipoClienteSerializado, usuario, objetoAdicionalSerializado);
                    }
                    catch
                    {
                        //se deja en blanco ya que la auditoria no debe interferir con el flujo de la admision
                    }
                });
            t.Start();
        }

        #region Generacion Fallas

        /// <summary>
        /// Genera falla de cálculo del valor de la guía manual
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="valorCobrado"></param>
        /// <param name="valorCalculado"></param>
        public void GenerarFallaCalculoValorGuiaManual(long numeroGuia, decimal valorCobrado, decimal valorCalculado)
        {
            string usuario = ControllerContext.Current.Usuario;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    string comentarios = String.Format(ADMensajesAdmisiones.MensajeFallaDiferenciaValorCobrado, numeroGuia, valorCobrado, valorCalculado);

                    Framework.Servidor.Agenda.ASAsignadorTarea.Instancia.AsignarTareas(ADConstantes.ID_FALLA_DIFERENCIA_VALOR_COBRADO, usuario, comentarios);
                }
                catch (Exception ex)
                {
                    AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.ToString(), COConstantesModulos.MENSAJERIA);
                }
            });
        }

        #endregion Generacion Fallas
    }
}