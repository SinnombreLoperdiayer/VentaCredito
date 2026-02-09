using CO.Servidor.Adminisiones.Mensajeria.Comun;
using CO.Servidor.Adminisiones.Mensajeria.Contado;
using CO.Servidor.Adminisiones.Mensajeria.Datos;
using CO.Servidor.Adminisiones.Mensajeria.Servicios;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionNacional;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;

namespace CO.Servidor.Adminisiones.Mensajeria.GuiaInterna
{
    /// <summary>
    /// Clase para manejar operaciones de CRUD sobre la guía interna
    /// </summary>
    internal class ADAdmisionGuiaInterna : ADAdmisionServicio
    {
        private static readonly ADAdmisionGuiaInterna instancia = (ADAdmisionGuiaInterna)FabricaInterceptores.GetProxy(new ADAdmisionGuiaInterna(), COConstantesModulos.MENSAJERIA);

        private IADFachadaAdmisionesMensajeria fachada = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        private ITAFachadaTarifas fachadaTarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();

        /// <summary>
        /// Retorna una instancia de Consultas de admision guía interna
        /// /// </summary>
        public static ADAdmisionGuiaInterna Instancia
        {
            get { return ADAdmisionGuiaInterna.instancia; }
        }

        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

        #region Inserciones

        /// <summary>
        /// Adiciona una guia interna en las tablas de admisiones y guía interna
        /// </summary>
        /// <returns>Identificador de la admisión de la guía interna</returns>
        public ADGuiaInternaDC AdicionarGuiaInterna(ADGuiaInternaDC guiaInterna)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            if (guiaInterna != null)
            {
                SUNumeradorPrefijo numeroSuministro = new SUNumeradorPrefijo();
                SUPropietarioGuia propietarioGuia = new SUPropietarioGuia();
                if (!guiaInterna.EsManual)
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        // Se obtiene el número de guía
                        numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.GUIA_CORRESPONDENCIA_INTERNA);
                        transaccion.Complete();
                    }
                }
                else
                {
                    //TODO: SE MODIFICA A ADO
                    propietarioGuia = fachadaSuministros.ObtenerPropietarioSuministro(guiaInterna.NumeroGuia, SUEnumSuministro.GUIA_CORRESPONDENCIA_INTERNA);
                    if (propietarioGuia.Id == 0)
                        throw new FaultException<ControllerException>
                          (new ControllerException(COConstantesModulos.MENSAJERIA,
                           ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(),
                            ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
                }

                using (TransactionScope transaccion = new TransactionScope())
                {
                    long idDueñoGuia = 0;
                    if (guiaInterna.EsOrigenGestion && guiaInterna.GestionOrigen.IdGestion != 0)
                    {
                        idDueñoGuia = guiaInterna.GestionOrigen.IdGestion;
                        //TODO: MODIFICAR A ADO
                        ARCasaMatrizDC casaMatrizOrigen = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>().ObtenerCasaMatriz(guiaInterna.GestionOrigen.IdCasaMatriz);
                        PUCentroServiciosDC centroServicioOrigen = fachadaCentroServicios.ObtenerAgenciaLocalidad(casaMatrizOrigen.IdLocalidad);
                        guiaInterna.IdCentroServicioOrigen = centroServicioOrigen.IdCentroServicio;
                        guiaInterna.NombreCentroServicioOrigen = centroServicioOrigen.Nombre;
                        guiaInterna.NombreRemitente = centroServicioOrigen.Nombre;
                        guiaInterna.TelefonoRemitente = centroServicioOrigen.Telefono1;
                        guiaInterna.DireccionRemitente = centroServicioOrigen.Direccion;
                        guiaInterna.LocalidadOrigen = new PALocalidadDC()
                        {
                            IdLocalidad = casaMatrizOrigen.IdLocalidad,
                            Nombre = casaMatrizOrigen.NombreLocalidad,
                        };
                    }

                    if (guiaInterna.IdCentroServicioDestino == 0)
                    {
                        //PUCentroServiciosDC centroServicioorigen = fachadaCentroServicios.ObtenerCentroServicio(guiaInterna.IdCentroServicioOrigen);
                        //if (centroServicioorigen.Tipo == "COL")
                        //{
                        //    if (fachadaCentroServicios.ObtenerPuntosAgenciasDependientes(guiaInterna.IdCentroServicioOrigen).FirstOrDefault(ces=>ces.IdCentroServicio == propietarioGuia.Id).IdCentroServicio!=null)
                        //    idDueñoGuia = propietarioGuia.Id;
                        //    else
                        //    idDueñoGuia = guiaInterna.IdCentroServicioOrigen;

                        //}
                        //else
                        idDueñoGuia = guiaInterna.IdCentroServicioOrigen;


                        PUCentroServiciosDC centroServicioDestino = fachadaCentroServicios.ObtenerAgenciaLocalidad(guiaInterna.LocalidadDestino.IdLocalidad);
                        guiaInterna.IdCentroServicioDestino = centroServicioDestino.IdCentroServicio;
                        guiaInterna.NombreCentroServicioDestino = centroServicioDestino.Nombre;
                        guiaInterna.LocalidadDestino.Nombre = centroServicioDestino.NombreMunicipio;
                    }

                    if (guiaInterna.TipoEntrega == null)
                    {
                        guiaInterna.TipoEntrega = new ADTipoEntrega
                        {
                            Id = ValoresDefaultGuia.IdTipoEntrega,
                            Descripcion = ValoresDefaultGuia.DescripcionTipoEntrega,
                        };
                    }


                    ADGuia guia = ArmarGuia(guiaInterna);
                    ADMensajeriaTipoCliente remitenteDestinatario = new ADMensajeriaTipoCliente();
                    if (!guiaInterna.EsManual)
                    {
                        guia.NumeroGuia = numeroSuministro.ValorActual;
                        guiaInterna.NumeroGuia = guia.NumeroGuia;
                        guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;
                    }
                    //else
                    //{

                    //    if (propietarioGuia.Id != idDueñoGuia)
                    //        throw new FaultException<ControllerException>
                    //       (new ControllerException(COConstantesModulos.MENSAJERIA,
                    //        ADEnumTipoErrorMensajeria.EX_ERROR_PROPIETARIO_GUIA.ToString(),
                    //         ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_PROPIETARIO_GUIA)));
                    //    else
                    //        GuardarConsumoGuia(guiaInterna, propietarioGuia);
                    //}

                    TAServicioDC servicio = new TAServicioDC
                    {
                        IdServicio = TAConstantesServicios.SERVICIO_MENSAJERIA,
                        UnidadNegocio = TAConstantesServicios.UNIDAD_MENSAJERIA,
                    };

                    guia.IdCentroServicioDestino = guiaInterna.IdCentroServicioDestino;
                    guia.NombreCentroServicioDestino = guiaInterna.NombreCentroServicioDestino;

                    if (guiaInterna.GestionOrigen == null)
                        guiaInterna.GestionOrigen = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty };
                    else if (string.IsNullOrEmpty(guiaInterna.GestionOrigen.Descripcion))
                        guiaInterna.GestionOrigen.Descripcion = String.Empty;

                    if (guiaInterna.GestionDestino == null)
                        guiaInterna.GestionDestino = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty };
                    else if (string.IsNullOrEmpty(guiaInterna.GestionDestino.Descripcion))
                        guiaInterna.GestionDestino.Descripcion = String.Empty;

                    guia.GuidDeChequeo = guiaInterna.GuidDeChequeo == null ? string.Empty : guiaInterna.GuidDeChequeo;

                    guia.FechaEstimadaDigitalizacion = guia.FechaEstimadaDigitalizacion.Year == 1 ? guia.FechaEstimadaEntrega : guia.FechaEstimadaDigitalizacion;
                    guia.FechaEstimadaArchivo = guia.FechaEstimadaArchivo.Year == 1 ? guia.FechaEstimadaEntrega : guia.FechaEstimadaArchivo;
                    PALocalidadDC municipioDestino = new PALocalidadDC() { IdLocalidad = guia.IdCiudadDestino, Nombre = guia.NombreCiudadDestino };
                    ADValidacionServicioTrayectoDestino validacionTrayectoDestino = new ADValidacionServicioTrayectoDestino();
                    /// Se envia por defecto 1 en el peso de la guia ya que no cuenta con este dato el objeto de guia interna
                    validacionTrayectoDestino = fachada.ValidarServicioTrayectoDestino(guiaInterna.LocalidadOrigen, municipioDestino, servicio, guiaInterna.IdCentroServicioOrigen, 1);
                    guia.MotivoNoUsoBolsaSeguriDesc = ObtenerInformacionDeCasillero(validacionTrayectoDestino.InfoCasillero, guia.Peso, guia.IdTipoEnvio);

                    //guia.FechaEstimadaEntrega = DateTime.Now.AddHours(validacionTrayectoDestino.DuracionTrayectoEnHoras).Date.AddHours(18);

                    /************************************************************/
                    /*************** FECHA ESTIMADA DE ENTREGA ******************/

                    ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
                    DateTime fechaActual = DateTime.Now;
                    ADValidacionServicioTrayectoDestino SegundaValidacion = new ADValidacionServicioTrayectoDestino();
                    if (guia.FechaEstimadaEntrega.Date <= DateTime.Now.Date)
                    {
                        PALocalidadDC municipioOrigen = new PALocalidadDC { IdLocalidad = guia.IdCiudadOrigen, Nombre = guia.NombreCiudadOrigen };
                        PALocalidadDC datosMunicipioDestino = new PALocalidadDC { IdLocalidad = guia.IdCiudadDestino, Nombre = guia.NombreCiudadDestino };
                        TAServicioDC servicioTarifas = new TAServicioDC { IdServicio = guia.IdServicio, Nombre = guia.NombreServicio };
                        SegundaValidacion = ValidarServicioTrayectoDestino(municipioOrigen, datosMunicipioDestino, servicioTarifas, guia.IdCentroServicioOrigen, guia.Peso);
                        guia.FechaEstimadaEntrega = DateTime.Now.AddHours(SegundaValidacion.DuracionTrayectoEnHoras).Date.AddHours(18);
                        guia.FechaEstimadaDigitalizacion = fechaActual.AddHours(SegundaValidacion.NumeroHorasDigitalizacion);
                        guia.FechaEstimadaDigitalizacion = new DateTime(guia.FechaEstimadaDigitalizacion.Year, guia.FechaEstimadaDigitalizacion.Month, guia.FechaEstimadaDigitalizacion.Day, guia.FechaEstimadaDigitalizacion.Hour, 0, 0);
                        guia.FechaEstimadaArchivo = fechaActual.AddHours(SegundaValidacion.NumeroHorasArchivo);
                        guia.FechaEstimadaArchivo = new DateTime(guia.FechaEstimadaArchivo.Year, guia.FechaEstimadaArchivo.Month, guia.FechaEstimadaArchivo.Day, guia.FechaEstimadaArchivo.Hour, 0, 0);
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


                    /************************************************************/
                    /************************************************************/
                    
                    //guia.FechaEstimadaEntrega = new DateTime(guia.FechaEstimadaEntrega.Year, guia.FechaEstimadaEntrega.Month, guia.FechaEstimadaEntrega.Day, 18, 0,0);
                    guia.FechaEstimadaDigitalizacion = DateTime.Now.AddHours(validacionTrayectoDestino.NumeroHorasDigitalizacion);
                    guia.FechaEstimadaDigitalizacion = new DateTime(guia.FechaEstimadaDigitalizacion.Year, guia.FechaEstimadaDigitalizacion.Month, guia.FechaEstimadaDigitalizacion.Day, guia.FechaEstimadaDigitalizacion.Hour, 0, 0);
                    guia.FechaEstimadaArchivo = DateTime.Now.AddHours(validacionTrayectoDestino.NumeroHorasArchivo);
                    guia.FechaEstimadaArchivo = new DateTime(guia.FechaEstimadaArchivo.Year, guia.FechaEstimadaArchivo.Month, guia.FechaEstimadaArchivo.Day, guia.FechaEstimadaArchivo.Hour, 0, 0);
                    // guia.FechaEstimadaEntrega = 
                    guiaInterna.InfoCasillero = guia.MotivoNoUsoBolsaSeguriDesc == null ? "" : guia.MotivoNoUsoBolsaSeguriDesc;
                    guiaInterna.IdAdmisionGuia = ADRepositorio.Instancia.AdicionarAdmision(guia, remitenteDestinatario);


                    if (guia.FechaEstimadaEntrega.Date <= DateTime.Now.Date && guia.IdServicio != TAConstantesServicios.SERVICIO_RAPI_HOY)
                    {
                       ADAdmisionContado.AuditaFechaEstimadaEntregaMalCalculada(guia);
                       throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, "Error calculando fecha estimada de entrega", "Error calculando la fecha estimada de entrega, favor volver a intentar."));
                    }

                    // Acá se debe validar si se debe generar ingreso a centro de acopio, esto debe aplicar solo para admisión manual COL
                    if (guiaInterna.EsManual)
                    {
                        long idCs = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(guia.NumeroGuia);
                        if (idCs > 0)
                        {
                            idCs = COFabricaDominio.Instancia.CrearInstancia<IONFachadaOperacionNacional>().GuiaYaFueIngresadaACentroDeAcopioRetornaCS(guia.NumeroGuia);
                            if (idCs > 0)
                            {
                                PUCentroServiciosDC cs = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(idCs);
                                ADTrazaGuia trazaGuia = new ADTrazaGuia
                                {
                                    NumeroGuia = guia.NumeroGuia,
                                    IdAdmision = guiaInterna.IdAdmisionGuia,
                                    Observaciones = guia.Observaciones,
                                    IdCiudad = cs.CiudadUbicacion.IdLocalidad,
                                    Ciudad = cs.CiudadUbicacion.Nombre,
                                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio,
                                    IdEstadoGuia = (short)EstadosGuia.ObtenerUltimoEstadoxNumero(guia.NumeroGuia)
                                };
                                EstadosGuia.InsertaEstadoGuia(trazaGuia);

                            }
                        }
                    }

                    guiaInterna.NumeroGuia = guia.NumeroGuia;
                    guiaInterna.DiceContener = guiaInterna.DiceContener == null ? string.Empty : guiaInterna.DiceContener;
                    ADRepositorio.Instancia.AdicionarGuiaInterna(guiaInterna);

                    transaccion.Complete();
                }
                return guiaInterna;
            }
            else
                return null;
        }


        /************************************ METODOS USADO EN FECHA ESTIMADA ENTREGA *******************************************************/
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
                servicio.IdServicio = ADAdmisionContado.Instancia.ValidarServicioSegunPesoMensajeria(municipioOrigen, municipioDestino, servicio, pesoGuia, tarifas);

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
                centroServicios.ObtenerInformacionValidacionTrayectoOrigen(municipioOrigen, validacionServicioTrayectoDestino);
                validacionServicioTrayectoDestino.ValoresAdicionales = new List<TAValorAdicional>();
            }
            return validacionServicioTrayectoDestino;
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

        /*************************************************************************************************************************/

        public string ObtenerInformacionDeCasillero(ADRangoTrayecto RangoCasillero, decimal peso, int idTipoEnvio)
        {
            string casillero = "";
            if (RangoCasillero != null)
            {
                if (RangoCasillero.Rangos != null && RangoCasillero.Rangos.Count == 1) // AEREO
                {
                    casillero = RangoCasillero.Rangos.FirstOrDefault().Casillero;
                }
                else
                {
                    decimal _pesoAUsar = peso;
                    // Si tipo de envío es sobre carta o sobre manila debe aplicar la regla, de lo contrario: NO
                    if (idTipoEnvio > 2)
                    {
                        var rangoMayor = RangoCasillero.Rangos.OrderByDescending(r => r.RangoInicial).FirstOrDefault();
                        if (rangoMayor != null)
                        {
                            _pesoAUsar = rangoMayor.RangoInicial;
                        }
                    }
                    var rango = RangoCasillero.Rangos.FirstOrDefault(r => _pesoAUsar <= r.RangoFinal && _pesoAUsar >= r.RangoInicial);
                    if (rango != null)
                        casillero = rango.Casillero;
                }
            }
            return casillero;
        }

        /// <summary>
        /// Método para llenar la guía con valores por defecto
        /// </summary>
        /// <param name="guiaInterna">Guía interna con su identificador de base de datos</param>
        /// <returns>Guía de admisión con valores por defecto</returns>
        private ADGuia ArmarGuia(ADGuiaInternaDC guiaInterna)
        {
            return new ADGuia
            {
                AdmisionSistemaMensajero = ValoresDefaultGuia.AdmisionSistemaMensajero,
                Alto = ValoresDefaultGuia.Alto,
                Ancho = ValoresDefaultGuia.Ancho,
                CodigoPostalDestino = ValoresDefaultGuia.CodigoPostalDestino,
                CodigoPostalOrigen = ValoresDefaultGuia.CodigoPostalOrigen,
                DescripcionTipoEntrega = guiaInterna.TipoEntrega.Descripcion,
                DiasDeEntrega = ValoresDefaultGuia.DiasDeEntrega,
                DiceContener = guiaInterna.DiceContener,
                DigitoVerificacion = ValoresDefaultGuia.DigitoVerificacion,
                DireccionDestinatario = guiaInterna.DireccionDestinatario,
                EsAlCobro = ValoresDefaultGuia.EsAlCobro,
                EsAutomatico = !guiaInterna.EsManual,
                EsPesoVolumetrico = ValoresDefaultGuia.EsPesoVolumetrico,
                EsRecomendado = ValoresDefaultGuia.EsRecomendado,
                FechaAdmision = DateTime.Now,
                FechaEstimadaEntrega = DateTime.Now,
                GuidDeChequeo = ValoresDefaultGuia.GuidDeChequeo,
                IdCentroServicioDestino = guiaInterna.IdCentroServicioDestino,
                IdCentroServicioOrigen = guiaInterna.IdCentroServicioOrigen,
                IdCiudadDestino = guiaInterna.LocalidadDestino.IdLocalidad,
                IdCiudadOrigen = guiaInterna.LocalidadOrigen.IdLocalidad,
                IdMotivoNoUsoBolsaSegurida = ValoresDefaultGuia.IdMotivoNoUsoBolsaSegurida,
                IdPaisDestino = guiaInterna.PaisDefault.IdLocalidad,
                IdPaisOrigen = guiaInterna.PaisDefault.IdLocalidad,
                IdServicio = ValoresDefaultGuia.IdServicio,
                IdTipoEntrega = guiaInterna.TipoEntrega.Id,
                IdTipoEnvio = ValoresDefaultGuia.IdTipoEnvio,
                IdUnidadMedida = ValoresDefaultGuia.IdUnidadMedida,
                IdUnidadNegocio = ValoresDefaultGuia.IdUnidadNegocio,
                Largo = ValoresDefaultGuia.Largo,
                MotivoNoUsoBolsaSeguriDesc = ValoresDefaultGuia.MotivoNoUsoBolsaSeguriDesc,
                NombreCentroServicioDestino = guiaInterna.NombreCentroServicioDestino,
                NombreCentroServicioOrigen = guiaInterna.NombreCentroServicioOrigen,
                NombreCiudadDestino = guiaInterna.LocalidadDestino.Nombre,
                NombreCiudadOrigen = guiaInterna.LocalidadOrigen.Nombre,
                NombrePaisDestino = guiaInterna.PaisDefault.Nombre,
                NombrePaisOrigen = guiaInterna.PaisDefault.Nombre,
                NombreServicio = ValoresDefaultGuia.NombreServicio,
                NombreTipoEnvio = ValoresDefaultGuia.NombreTipoEnvio,
                NoUsoaBolsaSeguridadObserv = ValoresDefaultGuia.NoUsoaBolsaSeguridadObserv,
                NumeroBolsaSeguridad = ValoresDefaultGuia.NumeroBolsaSeguridad,
                NumeroGuia = guiaInterna.NumeroGuia,
                NumeroPieza = ValoresDefaultGuia.NumeroPieza,
                Observaciones = ValoresDefaultGuia.Observaciones,
                ObservacionEstadoGuia = ValoresDefaultGuia.ObservacionEstadoGuia,
                Peso = ValoresDefaultGuia.Peso,
                TelefonoDestinatario = guiaInterna.TelefonoDestinatario,
                TipoCliente = ValoresDefaultGuia.TipoCliente,
                TotalPiezas = ValoresDefaultGuia.TotalPiezas,
                ValorAdicionales = ValoresDefaultGuia.ValorAdicionales,
                ValorAdmision = ValoresDefaultGuia.ValorAdmision,
                ValorDeclarado = ValoresDefaultGuia.ValorDeclarado,
                ValorEmpaque = ValoresDefaultGuia.ValorEmpaque,
                ValorPrimaSeguro = ValoresDefaultGuia.ValorPrimaSeguro,
                ValorTotal = ValoresDefaultGuia.ValorTotal,
                ValorTotalImpuestos = ValoresDefaultGuia.ValorTotalImpuestos,
                ValorTotalRetenciones = ValoresDefaultGuia.ValorTotalRetenciones,
                EstadoGuia = ValoresDefaultGuia.EstadoGuia,
                ValoresAdicionales = ValoresDefaultGuia.ValoresAdicionales,
                FormasPago = ValoresDefaultGuia.FormasPago,
                IdMensajero = ValoresDefaultGuia.IdMensajero,
                NombreMensajero = ValoresDefaultGuia.NombreMensajero,
                EstaPagada = ValoresDefaultGuia.EstaPagada,
                FechaPago = DateTime.Now,
                Remitente = new Servidor.Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                {
                    Nombre = guiaInterna.NombreRemitente,
                    Direccion = guiaInterna.DireccionRemitente,
                    Telefono = guiaInterna.TelefonoRemitente,
                    Email = string.Empty,
                    Identificacion = guiaInterna.IdentificacionRemitente == null ? string.Empty : guiaInterna.IdentificacionRemitente
                },
                Destinatario = new Servidor.Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                {
                    Nombre = guiaInterna.NombreDestinatario,
                    Direccion = guiaInterna.DireccionDestinatario,
                    Telefono = guiaInterna.TelefonoDestinatario,
                    Email = string.Empty,
                    Identificacion = guiaInterna.IdentificacionDestinatario == null ? string.Empty : guiaInterna.IdentificacionDestinatario
                },
                NumeroContrato = ValoresDefaultGuia.NumeroContrato,
                NombreCliente = ValoresDefaultGuia.NombreCliente,
                NombreSucursal = ValoresDefaultGuia.NombreSucursal,
            };
        }

        /// <summary>
        /// Clase anidada para almacenar los valores por defecto para crear una guía interna
        /// </summary>
        public static class ValoresDefaultGuia
        {
            public const bool AdmisionSistemaMensajero = false;

            public const decimal Alto = 0;

            public const decimal Ancho = 0;

            public const string CodigoPostalDestino = "";

            public const string CodigoPostalOrigen = "";

            public const string DescripcionTipoEntrega = "ENTREGA EN DIRECCION";

            public const short DiasDeEntrega = 0;

            public const string DiceContener = "";

            public const string DigitoVerificacion = "";

            public const bool EsAlCobro = false;

            public const bool EsAutomatico = false;

            public const bool EsPesoVolumetrico = false;

            public const bool EsRecomendado = false;

            public const string GuidDeChequeo = "";

            public const short IdMotivoNoUsoBolsaSegurida = 0;

            public const int IdServicio = 3;

            public const string IdTipoEntrega = "1";

            public const short IdTipoEnvio = 6;

            public const string IdUnidadMedida = "";

            public const string IdUnidadNegocio = "";

            public const decimal Largo = 0;

            public const string MotivoNoUsoBolsaSeguriDesc = "";

            public const string NombreServicio = "MENSAJERIA";

            public const string NombreTipoEnvio = "";

            public const string NoUsoaBolsaSeguridadObserv = "";

            public const string NumeroBolsaSeguridad = "";

            public const short NumeroPieza = 0;

            public const string Observaciones = "";

            public const decimal Peso = 0;

            public const decimal PesoVolumetrico = 0;

            public const ADEnumTipoCliente TipoCliente = ADEnumTipoCliente.INT;

            public const short TotalPiezas = 0;

            public const decimal ValorAdicionales = 0;

            public const decimal ValorAdmision = 0;

            public const decimal ValorDeclarado = 0;

            public const decimal ValorEmpaque = 0;

            public const decimal ValorPeso = 0;

            public const decimal ValorPrimaSeguro = 0;

            public const decimal ValorTotal = 0;

            public const decimal ValorTotalImpuestos = 0;

            public const decimal ValorTotalRetenciones = 0;

            public const decimal ValorVolumen = 0;

            public const string ObservacionEstadoGuia = "";

            public const ADEnumEstadoGuia EstadoGuia = ADEnumEstadoGuia.Admitida;

            public static List<TAValorAdicional> ValoresAdicionales = new List<TAValorAdicional>();

            public static List<ADGuiaFormaPago> FormasPago = new List<ADGuiaFormaPago>();

            public static long IdMensajero = 0;

            public static string NombreMensajero = "";

            public static bool EstaPagada = true;

            public static string NumeroContrato = "";

            public static string NombreCliente = "";

            public static string NombreSucursal = "";
        }

        /// <summary>
        /// Guarda la guia de mensajería como consumida
        /// </summary>
        /// <param name="guia"></param>
        private void GuardarConsumoGuia(ADGuiaInternaDC guia, SUPropietarioGuia duenoGuia)
        {
            SUConsumoSuministroDC consumo = new SUConsumoSuministroDC()
            {
                Cantidad = 1,
                EstadoConsumo = SUEnumEstadoConsumo.CON,
                GrupoSuministro = SUEnumGrupoSuministroDC.PRO,
                IdDuenoSuministro = (long)(duenoGuia.Propietario),
                IdServicioAsociado = TAConstantesServicios.SERVICIO_MENSAJERIA,
                NumeroSuministro = guia.NumeroGuia,
                Suministro = SUEnumSuministro.GUIA_CORRESPONDENCIA_INTERNA
            };
            fachadaSuministros.GuardarConsumoSuministro(consumo);
        }

        #endregion Inserciones

        #region Eliminación

        /// <summary>
        /// Metodo para eliminar una admisión con auditoria
        /// </summary>
        /// <param name="idAdmision"></param>
        public void EliminarAdmision(long idAdmision)
        {
            ADRepositorio.Instancia.EliminarAdmision(idAdmision);
        }

        public bool ValidarCredencialSispostal(credencialDTO credencial)
        {
            var usuario = Encoding.UTF8.GetString(Convert.FromBase64String(credencial.usuario));
            var password = Encoding.UTF8.GetString(Convert.FromBase64String(credencial.clave));
            var UsuarioReg = PAAdministrador.Instancia.ConsultarParametrosFramework("AdUserSispostal");
            var PasswordReg = PAAdministrador.Instancia.ConsultarParametrosFramework("AdPassSispostal");

            return usuario == UsuarioReg && password == PasswordReg;

        }

        #endregion Eliminación
    }
}