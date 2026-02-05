using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.CentroServicios;
using VentaCredito.Tarifas;
using VentaCredito.Tarifas.Datos.Repositorio;
using Servicio.Entidades.Admisiones.Mensajeria;
using Servicio.Entidades.CentroServicios;
using Servicio.Entidades.Tarifas;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Dominio.Comun.Tarifas;
using System.Configuration;
using System.Threading;
using System.Globalization;

namespace VentaCredito.Negocio.Contado
{
    public class ADAdmisionContado
    {
        private string formatoFechaIso = ConfigurationManager.AppSettings["FormatoFechaISO"];
        private static readonly ADAdmisionContado instancia = new ADAdmisionContado();
        private PUAdministradorCentroServicios fachadaCentroServicio = PUAdministradorCentroServicios.Instancia;

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static ADAdmisionContado Instancia
        {
            get { return ADAdmisionContado.instancia; }
        }


        public ADAdmisionContado()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
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
            //ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            TATrayecto tarifas = TATrayecto.Instancia;
            
            ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino = new ADValidacionServicioTrayectoDestino();
            int numeroDias = 0;
            int numeroHoras = 0;

            PUCentroServicios centroServicios = PUCentroServicios.Instancia;
            //COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

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
                PUCentroServiciosDC centroSerOrigen = fachadaCentroServicio.ObtenerAgenciaLocalidad(municipioOrigen.IdLocalidad);
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
                        fechaEntrega = PAAdministrador.Instancia.ObtenerFechaFinalHabilSinSabados(
                                                    DateTime.ParseExact(fechaRecogida.ToString(formatoFechaIso), formatoFechaIso, null), 
                                                    1, 
                                                    PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
                        // Rapi AM se  en horas al siguiente día antes de 12 M.
                        TimeSpan diferenciaDeFechasAdm = fechaEntrega - fechadmisionEnvio.Value;
                        numHabiles = diferenciaDeFechasAdm.TotalDays * 24;
                        int numeroHorasRapiAM = Convert.ToInt32(((numHabiles) - DateTime.Now.Hour) + 12);
                        //numeroHorasRapiAM = numeroHorasRapiAM + (horasRecogida >= 20 ? horasRecogida : 0);
                        validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHorasRapiAM;
                        fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(
                                                        fechaEntrega, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, numeroHorasRapiAM);
                        ObtenerHorasArchivioParaGuia(
                                            fechaDigitalizacion, 
                                            ref validacionServicioTrayectoDestino, 
                                            tiempos.numeroDiasArchivo, 
                                            validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                    }
                    else
                    {
                        double numDias = 0;
                        DateTime horaEntregaRapiAM = DateTime.Now;
                        numeroDias += 1;
                        horaEntregaRapiAM = PAAdministrador.Instancia.ObtenerFechaFinalHabilSinSabados(DateTime.ParseExact(DateTime.Now.ToString(formatoFechaIso), formatoFechaIso, null) , Convert.ToDouble(numeroDias), PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
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
                    fechaEntrega = PAAdministrador.Instancia.ObtenerFechaFinalHabilSinSabados(DateTime.ParseExact(fechaRecogida.Date.AddHours(18).ToString(formatoFechaIso), formatoFechaIso, null) , Convert.ToDouble(numeroDias), PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
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
                //PAOperadorPostal operadorPostal = PAAdministrador.Instancia.ObtenerOperadorPostalLocalidad(municipioDestino.IdLocalidad);
                //if (operadorPostal != null)
                //{
                //    validacionServicioTrayectoDestino.IdOperadorPostalDestino = operadorPostal.Id;
                //    validacionServicioTrayectoDestino.IdZonaOperadorPostalDestino = operadorPostal.IdZona;
                //    validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = operadorPostal.TiempoEntrega * 24; // Porque el tiempo de entrega por zona se da en días
                //}
                ////Cuando es internacional se valida con la ciudad de destino Bogota
                //validacionServicioTrayectoDestino.CodigoPostalDestino = "11001000";
                //centroServicios.ObtenerInformacionValidacionTrayectoOrigen(municipioOrigen, validacionServicioTrayectoDestino);
                //validacionServicioTrayectoDestino.ValoresAdicionales = new List<TAValorAdicional>();
            }
            return validacionServicioTrayectoDestino;
        }

        public int ValidarServicioSegunPesoMensajeria(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, decimal pesoGuia, TATrayecto tarifas)
        {
            int idServicio = servicio.IdServicio;

            if (servicio.IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA && pesoGuia >= 3)
            {
                List<int> lstIdServicios = TrayectoRepositorio.Instancia.ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(municipioOrigen.IdLocalidad.ToString(), municipioDestino.IdLocalidad.ToString());
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
    }
}
