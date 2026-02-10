using CO.Servidor.Dominio.Comun.Util;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Raps.Datos;
using CO.Servidor.RAPS.Reglas.ResponsablesManuales;
using CO.Servidor.Servicios.ContratoDatos.Raps.Citas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CO.Servidor.Raps
{
    public class RAMotorRaps : ControllerBase
    {
        private static readonly RAMotorRaps instancia = (RAMotorRaps)FabricaInterceptores.GetProxy(new RAMotorRaps(), COConstantesModulos.MODULO_RAPS);
        #region singleton

        public static RAMotorRaps Instancia
        {
            get { return RAMotorRaps.instancia; }
        }

        public RAMotorRaps() { }

        #endregion

        #region Escalonamiento

        /// <summary>
        /// Obtiene los horarios del empleado para el cual se realizara el escalamiento de un rap
        /// </summary>
        /// <param name="idCargo"></param>
        /// <param name="idSucursal"></param>
        public RACargoEscalarDC ObtenerHorariosEmpleadoEscalarPorCargoSucursal(RACargoEscalarDC cargoEscalar)
        {
            return RARepositorio.Instancia.ObtenerHorariosEmpleadoEscalarPorCargoSucursal(cargoEscalar);

        }

        /// <summary>
        /// Obtiene todas las Solicitudes vencidas junto con el escalonamiento
        /// </summary>
        /// <returns></returns>
        public List<RASolicitudDC> ObtenerSolicitudesVencidasEscalonamiento()
        {

            return RARepositorio.Instancia.ObtenerSolicitudesVencidasEscalonamiento();
        }

        /// <summary>
        /// Realiza el proceso de escalonamiento de las solicitudes vencidas, solo se utiliza en el motorRaps
        /// </summary>
        public void EscalarSolicitudesVencidas()
        {

            //Obtiene las solicitudes vencidas, junto con el escalonamiento.
            //la lista de escalonamiento que no haya tenido gestiones notificadas por el motor, para asegurar que para la lista de escalonamiento obtenida, nunca se le haya realizado una gestion
            List<RASolicitudDC> lstSolicitudesVencidas = RARepositorio.Instancia.ObtenerSolicitudesVencidasEscalonamiento();

            lstSolicitudesVencidas.ForEach(solicitud =>
            {
                var escalonamiento = solicitud.Escalonamiento.FirstOrDefault();
                if (escalonamiento != null)
                {
                    DateTime nuevaFechaVencimiento = DateTime.Now;
                    if (escalonamiento.IdTipoHora == 1)
                    {
                        nuevaFechaVencimiento = CalcularFechaVencimientoHorarioLaboral(escalonamiento.CargoEscalar, escalonamiento.HorasEscalar);
                    }
                    else
                    {
                        nuevaFechaVencimiento = nuevaFechaVencimiento.AddHours(escalonamiento.HorasEscalar);
                    }


                    using (TransactionScope trans = new TransactionScope())
                    {

                        //Inserta gestion de vencimiento
                        RAGestionDC gestionVencida = new RAGestionDC()
                        {
                            IdSolicitud = solicitud.IdSolicitud,
                            Comentario = "Solicitud Vencida",
                            IdCargoGestiona = "",
                            CorreoEnvia = "MotorRaps",
                            //IdAccion = RAEnumAccion.Vencer,
                            IdCargoDestino = solicitud.IdCargoResponsable,
                            CorreoDestino = solicitud.Cargo.CorreoCorporativo,
                            IdResponsable = solicitud.IdCargoResponsable,
                            IdEstado = RAEnumEstados.Vencido,
                            IdUsuario = "MotorRaps",
                            FechaVencimiento = solicitud.FechaVencimiento,
                            DocumentoSolicita = solicitud.DocumentoSolicita,
                            DocumentoResponsable = solicitud.DocumentoResponsable,
                        };
                        RARepositorioSolicitudes.Instancia.CrearGestion(gestionVencida);


                        //Inserta gestion de escalamiento
                        RAGestionDC gestionAsignada = new RAGestionDC()
                        {
                            IdSolicitud = solicitud.IdSolicitud,
                            Comentario = "Solicitud Escalada",
                            IdCargoGestiona = "",
                            CorreoEnvia = "MotorRaps",
                            //IdAccion = RAEnumAccion.Escalar,
                            IdCargoDestino = escalonamiento.CargoEscalar.IdCargoController,
                            CorreoDestino = escalonamiento.CargoEscalar.Correo,
                            IdResponsable = escalonamiento.CargoEscalar.IdCargoController,
                            IdEstado = RAEnumEstados.Escalado,
                            IdUsuario = "MotorRaps",
                            FechaVencimiento = nuevaFechaVencimiento,
                            DocumentoSolicita = solicitud.DocumentoSolicita,
                            DocumentoResponsable = escalonamiento.CargoEscalar.DocumentoEmpleado,
                        };
                        long idGestion = RARepositorioSolicitudes.Instancia.CrearGestion(gestionAsignada);


                        RASolicitudDC soli = new RASolicitudDC()
                        {
                            IdSolicitud = solicitud.IdSolicitud,
                            FechaVencimiento = nuevaFechaVencimiento,
                            IdCargoResponsable = escalonamiento.CargoEscalar.IdCargoController,
                            DocumentoResponsable = escalonamiento.CargoEscalar.DocumentoEmpleado
                        };



                        RARepositorioSolicitudes.Instancia.ReasignarSolicitudEscaldaVencimiento(soli);
                        RARepositorioSolicitudes.Instancia.InsertarNotificacionGestionMotor(escalonamiento, escalonamiento.CargoEscalar, idGestion, solicitud.IdSolicitud);
                        trans.Complete();
                    }
                    try
                    {
                        StringBuilder sbCorreo = ValidarAmbientePruebasEnviarCorreo();


                        sbCorreo.AppendLine("<br>");
                        sbCorreo.AppendLine("<b>SE HA ASIGNADO UN NUEVO RAP (Escalado)</b>");
                        sbCorreo.AppendLine("<br>");
                        sbCorreo.AppendLine("<br>");
                        sbCorreo.AppendLine("<big><b>" + solicitud.Descripcion + "</b></big>");
                        sbCorreo.AppendLine("<br>");
                        sbCorreo.AppendLine("<br>");
                        sbCorreo.AppendLine("Número de la solicitud: " + solicitud.IdSolicitud);

                        sbCorreo.AppendLine("<br>");
                        sbCorreo.AppendLine("<br>");
                        sbCorreo.AppendLine("Fecha Vencimiento: <b>" + nuevaFechaVencimiento.ToString("dd/MM/yyyy hh:mm tt") + "</b>");
                        sbCorreo.AppendLine("<br>");
                        sbCorreo.AppendLine("<br>");

                        sbCorreo.AppendLine();
                        sbCorreo.AppendLine();

                        StringBuilder sbCorreoVencido = ValidarAmbientePruebasEnviarCorreo();


                        sbCorreoVencido.AppendLine("<br>");
                        sbCorreoVencido.AppendLine("<b> VENCIMIENTO DE RAP </b>");
                        sbCorreoVencido.AppendLine("<br>");
                        sbCorreoVencido.AppendLine("<br>");
                        sbCorreoVencido.AppendLine("<big><b>" + solicitud.Descripcion + "</b></big>");
                        sbCorreoVencido.AppendLine("<br>");
                        sbCorreoVencido.AppendLine("<br>");
                        sbCorreoVencido.AppendLine("Número de la solicitud: " + solicitud.IdSolicitud);

                        sbCorreoVencido.AppendLine();
                        sbCorreoVencido.AppendLine();

                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                //CorreoElectronico.Instancia.Enviar("arquitecto.desarrollo@interrapidisimo.com", "Nuevo RAP Asignado", sbCorreo.ToString());
                                CorreoElectronico.Instancia.Enviar(escalonamiento.CargoEscalar.Correo, "Nuevo RAP Asignado (Escalado)", sbCorreo.ToString());
                                CorreoElectronico.Instancia.Enviar(solicitud.Cargo.CorreoCorporativo, "RAP Vencido", sbCorreoVencido.ToString());
                            }
                            catch (Exception ex)
                            {
                                Utilidades.AuditarExcepcion(ex, true);
                            }
                        });


                        NotificarClienteConectado(solicitud.IdSolicitud, escalonamiento.CargoEscalar.DocumentoEmpleado, "Nuevo RAP Asignado (Escalado)", true);

                    }
                    catch
                    {
                        //validar porque se revienta por transacciones distribuidas
                    }


                }


            });

        }

        /// <summary>
        /// Calcula la fecha de vencimiento de la nueva asignacion
        /// </summary>
        /// <returns></returns>
        public DateTime CalcularFechaVencimientoHorarioLaboral(RACargoEscalarDC cargoEscalar, int horasEscalar, DateTime? fechaEjecucion = null)
        {


            //PAParametros

            DateTime fechaActual = DateTime.Now;
            DateTime FechaInicialEscalamiento = DateTime.Now;

            if (fechaEjecucion != null && ((DateTime)fechaEjecucion) > fechaActual)
            {
                fechaActual = fechaActual.Add(((DateTime)fechaEjecucion) - fechaActual);
                FechaInicialEscalamiento = fechaActual.Add(((DateTime)fechaEjecucion) - fechaActual);
            }


            double minutosAdicionar = horasEscalar * 60, minutosAddOrg = horasEscalar * 60;

            double minutosEscalar = horasEscalar * 60;

            //calculo de la fecha laboral real
            if (cargoEscalar.HorarioEmpleado.Count > 2)
            {
                if (cargoEscalar.HorarioEmpleado[0].HoraEntrada.Day == cargoEscalar.HorarioEmpleado[1].HoraEntrada.Day)
                {
                    for (int i = 0; i <= cargoEscalar.HorarioEmpleado.Count - 1; i++)
                    {
                        cargoEscalar.HorarioEmpleado[i].HoraEntrada = cargoEscalar.HorarioEmpleado[i].HoraEntrada.AddDays(i);
                        cargoEscalar.HorarioEmpleado[i].HoraSalida = cargoEscalar.HorarioEmpleado[i].HoraSalida.AddDays(i);
                    }
                }
            }


            int inicioCont = 0;

            //calcula la fecha desde la cual inicia el escalonamiento
            if (fechaActual < cargoEscalar.HorarioEmpleado[0].HoraEntrada)
            {
                FechaInicialEscalamiento = cargoEscalar.HorarioEmpleado[0].HoraEntrada;
            }
            else
            {
                if (fechaActual < cargoEscalar.HorarioEmpleado[0].HoraSalida)
                {
                    FechaInicialEscalamiento = fechaActual;
                }
                else
                {
                    FechaInicialEscalamiento = cargoEscalar.HorarioEmpleado[1].HoraEntrada;
                    inicioCont = 1;
                }
            }

            //si faltan dos horas para terminar el dia laboral del empleado, el inicio de la tarea se debe contar a partir del siguiente dia
            if (FechaInicialEscalamiento.Day == cargoEscalar.HorarioEmpleado[0].HoraSalida.Day && cargoEscalar.HorarioEmpleado[0].HoraSalida.AddHours(-2) < FechaInicialEscalamiento)
            {
                FechaInicialEscalamiento = cargoEscalar.HorarioEmpleado[1].HoraEntrada;
                inicioCont = 1;
            }


            //La FechaInicialEscalamiento se tranforma en la fecha de vencimiento

            for (int i = inicioCont; i < cargoEscalar.HorarioEmpleado.Count - 1; i++)
            {
                if (FechaInicialEscalamiento >= cargoEscalar.HorarioEmpleado[i].HoraEntrada)
                {
                    if (FechaInicialEscalamiento.AddMinutes(minutosAdicionar) > cargoEscalar.HorarioEmpleado[i].HoraSalida)
                    {
                        if (minutosAdicionar != minutosAddOrg)
                        {
                            if (minutosAdicionar - (cargoEscalar.HorarioEmpleado[i].HoraSalida - cargoEscalar.HorarioEmpleado[i].HoraEntrada).TotalMinutes > 0)
                            {
                                minutosAdicionar = minutosAdicionar - (cargoEscalar.HorarioEmpleado[i].HoraSalida - cargoEscalar.HorarioEmpleado[i].HoraEntrada).TotalMinutes;
                            }
                            else
                            {
                                FechaInicialEscalamiento = FechaInicialEscalamiento.AddMinutes(minutosAdicionar);
                                break;
                            }
                        }
                        else
                        {

                            if (minutosAdicionar - (cargoEscalar.HorarioEmpleado[i].HoraSalida - cargoEscalar.HorarioEmpleado[i].HoraEntrada.AddMinutes((FechaInicialEscalamiento - cargoEscalar.HorarioEmpleado[i].HoraEntrada).TotalMinutes)).TotalMinutes > 0)
                            {
                                minutosAdicionar = minutosAdicionar - (cargoEscalar.HorarioEmpleado[i].HoraSalida - cargoEscalar.HorarioEmpleado[i].HoraEntrada.AddMinutes((FechaInicialEscalamiento - cargoEscalar.HorarioEmpleado[i].HoraEntrada).TotalMinutes)).TotalMinutes;
                            }
                            else
                            {
                                FechaInicialEscalamiento = FechaInicialEscalamiento.AddMinutes(minutosAdicionar);
                                break;
                            }


                        }
                        FechaInicialEscalamiento = FechaInicialEscalamiento.AddDays(1);
                        var f = cargoEscalar.HorarioEmpleado.Where(h => h.HoraEntrada.Date.Day == FechaInicialEscalamiento.Date.Day).First();
                        FechaInicialEscalamiento = new DateTime(FechaInicialEscalamiento.Year, FechaInicialEscalamiento.Month, FechaInicialEscalamiento.Day, f.HoraEntrada.Hour, f.HoraEntrada.Minute, 0);


                    }
                    else
                    {
                        FechaInicialEscalamiento = FechaInicialEscalamiento.AddMinutes(minutosAdicionar);
                        break;
                    }

                }
            }


            List<DateTime> lstFestivos = RARepositorio.Instancia.ObtenerFestivos(FechaInicialEscalamiento.Date, FechaInicialEscalamiento.AddDays(1).Date, RARepositorio.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));

            if (lstFestivos.Where(f => f.Date == FechaInicialEscalamiento.Date).Count() > 0)
            {
                FechaInicialEscalamiento = FechaInicialEscalamiento.AddDays(1);

                if (lstFestivos.Where(f => f.Date == FechaInicialEscalamiento.Date).Count() > 0)
                {
                    FechaInicialEscalamiento = FechaInicialEscalamiento.AddDays(1);
                }
            }

            return FechaInicialEscalamiento;


        }

        private string urlApi = "";

        /// <summary>
        /// Notifica mediante signal al cliente conectado en la aplicacion web de RAPS
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <param name="documentoCliente"></param>
        public void NotificarClienteConectado(long idSolicitud, string documentoCliente, string mensaje = "Nuevo RAP Asignado. Id Solicitud: ", bool insertarNotificacion = false)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(urlApi))
                        urlApi = PAParametros.Instancia.ConsultarParametrosFramework("UrlControllerApi");

                    var objSignal = new { Documento = documentoCliente, Mensaje = mensaje + idSolicitud, IdSolicitud = Convert.ToInt64(idSolicitud), InsertarNotificacion = insertarNotificacion };
                    var restClient = new RestClient(urlApi);
                    var restRequest = new RestRequest("api/NotificacionesController/NotificarSolicitudUsuario", Method.POST);
                    restRequest.AddJsonBody(objSignal);
                    restRequest.AddHeader("usuario", "admin");
                    restClient.Execute(restRequest);
                }
                catch (Exception ex)
                {
                    Utilidades.AuditarExcepcion(ex, true);
                }
            });
        }

        /// <summary>
        /// Notifica mediante signal al cliente conectado en la aplicacion web de RAPS
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <param name="documentoCliente"></param>
        public void NotificarCitaClienteConectado(string descripcionCita, DateTime fechaInicio, string documentoCliente)
        {
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(urlApi))
                    urlApi = PAParametros.Instancia.ConsultarParametrosFramework("UrlControllerApi");

                var objSignal = new { Documento = documentoCliente, Mensaje = descripcionCita + ". Inicia: " + fechaInicio.ToString("dd/MM/yyyy HH:mm") };
                var restClient = new RestClient(urlApi);
                var restRequest = new RestRequest("api/NotificacionesController/NotificarSolicitudUsuario", Method.POST);
                restRequest.AddJsonBody(objSignal);
                restRequest.AddHeader("usuario", "admin");
                restClient.Execute(restRequest);
            });
        }


        /// <summary>
        /// Notifica mediante signal al cliente conectado en la aplicacion web de RAPS
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <param name="documentoCliente"></param>
        public void NotificarClienteConectadoSinTask(long idSolicitud, string documentoCliente, bool insertarNotificacion = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(urlApi))
                    urlApi = PAParametros.Instancia.ConsultarParametrosFramework("UrlControllerApi");

                var objSignal = new { Documento = documentoCliente, Mensaje = "Nuevo RAP Asignado. Id Solicitud: " + idSolicitud, IdSolicitud = Convert.ToInt64(idSolicitud), InsertarNotificacion = insertarNotificacion };
                var restClient = new RestClient(urlApi);
                var restRequest = new RestRequest("api/NotificacionesController/NotificarSolicitudUsuario", Method.POST);
                restRequest.AddJsonBody(objSignal);
                restRequest.AddHeader("usuario", "admin");
                restClient.Execute(restRequest);
            }
            catch (Exception ex)
            {
                Utilidades.AuditarExcepcion(ex, true);
            }

        }

        /// <summary>
        /// Notifica mediante signal al cliente conectado en la aplicacion web de RAPS
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <param name="documentoCliente"></param>
        public void NotificarTodosClientesConectados()
        {
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(urlApi))
                    urlApi = PAParametros.Instancia.ConsultarParametrosFramework("UrlControllerApi");

                //var objSignal = new { Documento = documentoCliente, Mensaje = "Nuevo RAP Asignado. Id Solicitud: " + idSolicitud };
                var restClient = new RestClient(urlApi);
                var restRequest = new RestRequest("api/NotificacionesController/NotificarSolicitudTodosUsuarios", Method.POST);
                //restRequest.AddJsonBody(objSignal);
                restRequest.AddHeader("usuario", "admin");
                restClient.Execute(restRequest);
            });
        }

        /// <summary>
        /// Crea las solicitudes automaticas que no tienen sistema fuente
        /// </summary>
        public void CrearSolicitudesAutomaticasSinSistemaFuente()
        {

            List<RASolicitudDC> solicitudes = RARepositorio.Instancia.ObtenerRapsAutomaticosGenerarSolicitudes();
            solicitudes.ForEach(solicitud =>
            {

                solicitud.Escalonamiento = solicitud.Escalonamiento.OrderBy(s => s.Orden).ToList();

                ///busca los datos de la persona asociada al cargo en la base de datos novasoft.
                ///y se escala a la primer persona que encuentre con datos.
                foreach (var escalonamiento in solicitud.Escalonamiento)
                {
                    RACargoEscalarDC cargoEscalar = ObtenerHorariosEmpleadoEscalarPorCargoSucursal(
                    new RACargoEscalarDC()
                    {
                        IdCargoController = escalonamiento.idCargo,
                        Sucursal = escalonamiento.IdSucursalEscalar,
                        Correo = escalonamiento.CorreoEscalar
                    });

                    if (!string.IsNullOrWhiteSpace(cargoEscalar.IdCargoController))
                    {
                        DateTime nuevaFechaVencimiento = DateTime.Now;
                        if (escalonamiento.IdTipoHora == 1)
                        {
                            nuevaFechaVencimiento = CalcularFechaVencimientoHorarioLaboral(cargoEscalar, escalonamiento.HorasEscalar);
                        }
                        else
                        {
                            nuevaFechaVencimiento = nuevaFechaVencimiento.AddHours(escalonamiento.HorasEscalar);
                        }


                        using (TransactionScope trans = new TransactionScope())
                        {

                            solicitud.IdCargoSolicita = "";
                            solicitud.IdEstado = RAEnumEstados.Asignado;
                            solicitud.FechaVencimiento = nuevaFechaVencimiento;
                            solicitud.FechaCreacion = DateTime.Now;
                            solicitud.IdCargoResponsable = cargoEscalar.IdCargoController;
                            solicitud.DocumentoSolicita = "";
                            solicitud.DocumentoResponsable = cargoEscalar.DocumentoEmpleado;
                            solicitud.IdSolicitudPadre = 0;
                            solicitud.idSucursal = cargoEscalar.Sucursal.Trim();
                            //crea la solicitud                            
                            solicitud.IdSolicitud = RARepositorio.Instancia.InsertarSolicitud(solicitud);

                            //Inserta gestion de creacion
                            RAGestionDC gestion = new RAGestionDC()
                            {
                                IdSolicitud = solicitud.IdSolicitud,
                                Comentario = "Solicitud Creada",
                                IdCargoGestiona = "",
                                CorreoEnvia = "MotorRaps",
                                //IdAccion = RAEnumAccion.Crear,
                                IdCargoDestino = solicitud.IdCargoResponsable,
                                CorreoDestino = cargoEscalar.Correo,
                                IdResponsable = solicitud.IdCargoResponsable,
                                IdEstado = RAEnumEstados.Asignado,
                                IdUsuario = "MotorRaps",
                                FechaVencimiento = nuevaFechaVencimiento,
                                DocumentoSolicita = solicitud.DocumentoSolicita,
                                DocumentoResponsable = solicitud.DocumentoResponsable,
                            };
                            long idGestion = RARepositorioSolicitudes.Instancia.CrearGestion(gestion);

                            RARepositorioSolicitudes.Instancia.InsertarNotificacionGestionMotor(escalonamiento, cargoEscalar, idGestion, solicitud.IdSolicitud);

                            RARepositorio.Instancia.InsertarUltimaEjecucionRapAutomatico(solicitud.IdParametrizacionRap, "MotorRaps");


                            trans.Complete();
                        }
                        try
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("<br>");
                            sb.AppendLine("<b>SE HA ASIGNADO UN NUEVO RAP</b>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("<big><b> Número de la solicitud: " + solicitud.IdSolicitud + "</b></big>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("<big><b>" + solicitud.Descripcion + "</b></big>");

                            sb.AppendLine("<br>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("Fecha Vencimiento: <b>" + nuevaFechaVencimiento.ToString("dd/MM/yyyy hh:mm tt") + "</b>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("<br>");

                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    CorreoElectronico.Instancia.Enviar(cargoEscalar.Correo, "Nuevo RAP Asignado", sb.ToString());
                                }
                                catch (Exception ex)
                                {
                                    Utilidades.AuditarExcepcion(ex, true);
                                }
                            });

                            NotificarClienteConectado(solicitud.IdSolicitud, cargoEscalar.DocumentoEmpleado);

                        }
                        catch
                        {
                            //validar porque se revienta por transacciones distribuidas
                        }


                        break;


                    }
                    else
                    {
                        //definir que se hace cuando no encuentra el responsable responsable
                    }

                }
            });
        }

        /// <summary>
        /// Crea las solicitudes automaticas que tienen un sistema fuente ej:  Controller
        /// </summary>
        public void CrearSolicitudesAutomaticasConSistemaFuente()
        {


            List<RASolicitudDC> solicitudes = RARepositorio.Instancia.ObtenerRapsAutomaticosConSistemaFuenteGenerarSolicitudes();

            if (solicitudes != null && solicitudes.Count > 0)
            {
                List<RASolicitudDC> solicitudesConPadre = solicitudes.Where(s => s.IdParametrizacionRapPadre > 0).ToList();
                if (solicitudesConPadre.Count() > 0)
                {
                    CrearSolicitudAutConSistemaFuenteConParamPadre(solicitudesConPadre);
                }
                List<RASolicitudDC> solicitudesSinPadre = solicitudes.Where(s => s.IdParametrizacionRapPadre <= 0).ToList();
                if (solicitudesSinPadre.Count() > 0)
                {
                    CrearSolicitudAutConSistemaFuenteSinParamPadre(solicitudesSinPadre);
                }
            }

        }


        /// <summary>
        /// Crea las solicitudes automaticasConSistema fuente que tengan una parametrizacion de raps padre
        /// </summary>
        /// <param name="solicitudes"></param>
        /// <summary>
        /// Crea las solicitudes automaticasConSistema fuente que tengan una parametrizacion de raps padre
        /// </summary>
        /// <param name="solicitudes"></param>
        private void CrearSolicitudAutConSistemaFuenteConParamPadre(List<RASolicitudDC> solicitudes)
        {
            //Para cada solicitud automatica se debe consultar los parametros de parametrizacion 
            //y agrupar por parametro de agrupamiento para asi crear solo un rap con todos sus datos agrupados


            //agrupa por las parametrizaciones padre y por la sucursal
            List<RASolicitudDC> lstSolicitudesAcum = solicitudes.GroupBy(s => new { s.IdParametrizacionRapPadre, s.idSucursal }).Select(s => s.First()).ToList();

            string valorAgrupamiento = String.Empty;

            lstSolicitudesAcum.ForEach(solicitudAcumulativa =>
            {

                solicitudAcumulativa.Escalonamiento = solicitudAcumulativa.Escalonamiento.OrderBy(s => s.Orden).ToList();

                //agrupa por los parametros de agrupamiento
                var solicitudesHermanas = solicitudes.Where(p => p.IdParametrizacionRapPadre == solicitudAcumulativa.IdParametrizacionRapPadre
                && p.idSucursal == solicitudAcumulativa.idSucursal).ToList();



                //realiza el agrupamiento por el valor del parametro marcado como es agrupamiento, por cada grupo se debe crear una solicitud 
                //de rap con el id de la parametrizacion padre
                Dictionary<string, List<RAParametrosParametrizacionDC>> dicParamAgrupamiento = new Dictionary<string, List<RAParametrosParametrizacionDC>>();
                solicitudesHermanas.ForEach(soliHermana =>
                {
                    foreach (var paSoliHermana in soliHermana.ParametrosSolicitud)
                    {
                        if (paSoliHermana.EsAgrupamiento)
                        {
                            valorAgrupamiento = paSoliHermana.Valor.ToString();
                            if (!dicParamAgrupamiento.ContainsKey(paSoliHermana.Valor))
                            {
                                dicParamAgrupamiento.Add(paSoliHermana.Valor, new List<RAParametrosParametrizacionDC>());
                            }

                            soliHermana.ParametrosSolicitud.Where(param => param.IdSolicitud == paSoliHermana.IdSolicitud).ToList().ForEach(p =>
                            {
                                dicParamAgrupamiento[paSoliHermana.Valor].Add(p);

                            });
                            break;
                        }
                    }
                });

                foreach (var dicParam in dicParamAgrupamiento)
                {
                    StringBuilder sbConHtml = new StringBuilder();
                    StringBuilder sbSinHtml = new StringBuilder();
                    StringBuilder sbDesSolicitud = new StringBuilder();
                    Dictionary<long, StringBuilder> dicDescripcion = new Dictionary<long, StringBuilder>();
                    StringBuilder sbDescripcionDiccionario = new StringBuilder();

                    sbDesSolicitud.AppendLine(solicitudAcumulativa.NombreParametrizacionRapPadre);

                    // sbDesSolicitud.AppendLine("|" + p.DescripcionSolicitud);


                    sbConHtml.AppendLine();
                    sbSinHtml.AppendLine();

                    var ss = dicParam.Value.GroupBy(p => p.IdSolicitud).Select(s => s.First()).ToList();

                    var soliHer = solicitudesHermanas.Where(s => s.IdSolicitud == dicParam.Value.First().IdSolicitud).GroupBy(p => p.IdSolicitud).Select(s => s.First()).ToList();

                    //Arma encabezado de la descripcion del rap, solo muestra los parametros marcados como EsEncabezadoDescripcion
                    //y hace conteo de fallas agrupadas en la solicitud
                    soliHer.Where(sol => sol.ParametrosSolicitud.Where(p => p.EsEncabezadoDescripcion).Count() >= 1)
                    .ToList()
                    .ForEach(soli =>
                    {
                        if (!dicDescripcion.ContainsKey(soli.IdSolicitud))
                        {
                            sbDescripcionDiccionario.AppendLine(" | " + soli.Descripcion + "(" + ss.Count() + ")");

                            var solHerm = solicitudesHermanas.Where(s => s.IdSolicitud == soli.IdSolicitud).FirstOrDefault();
                            if (solHerm != null)
                            {
                                solHerm.ParametrosSolicitud.Where(par => par.EsEncabezadoDescripcion).ToList().ForEach(solParam =>
                                {
                                    sbDescripcionDiccionario.AppendLine(" | " + solParam.descripcionParametro + ":" + solParam.Valor);
                                });
                            }

                            dicDescripcion.Add(soli.IdSolicitud, sbDescripcionDiccionario);
                        }
                        //else
                        //{                                
                        //    dicDescripcion[p.IdSolicitud].AppendLine(" => " + p.descripcionParametro + "(" + ss.Where(sol => sol.idParametro == p.idParametro).Count() + ")");
                        //}

                    });
                    //genera la descripcion de la solicitud con la descripcion generada en el paso anterior
                    foreach (var dic in dicDescripcion)
                    {
                        sbDesSolicitud.AppendLine(dic.Value.ToString());
                    }

                    //Solo tiene en cuenta los parametros que no estÃ¡n marcados como EsEncabezadoDescripcion
                    //genera la descripcion de la gestion con la informacion de los parametros
                    ss
                    .ToList().ForEach(p =>
                    {



                        sbSinHtml.AppendLine("------------------------");
                        sbSinHtml.AppendLine(p.DescripcionSolicitud);
                        sbSinHtml.AppendLine("------------------------");
                        sbSinHtml.AppendLine();


                        sbConHtml.AppendLine("<br>");
                        sbConHtml.AppendLine("------------------------");
                        sbConHtml.AppendLine("<big><b>" + p.DescripcionSolicitud + "</b></big>");
                        sbConHtml.AppendLine("------------------------");
                        sbConHtml.AppendLine("<br>");

                        sbConHtml.AppendLine("<p>");

                        var pp = dicParam.Value.Where(pa => pa.IdSolicitud == p.IdSolicitud).ToList();
                        pp.ForEach(para =>
                        {
                            if (!para.EsEncabezadoDescripcion)
                            {
                                sbConHtml.AppendLine("<b>" + para.descripcionParametro + "</b>  :  <b>" + para.Valor + "</b>");
                                sbConHtml.AppendLine("<br>");

                                sbSinHtml.AppendLine(para.descripcionParametro + " : " + para.Valor);
                            }
                        });

                        sbConHtml.AppendLine("</p>");

                        sbSinHtml.AppendLine();
                    });

                    solicitudAcumulativa.Descripcion = sbDesSolicitud.ToString();

                    RAGestionDC gestion = null;

                    RAEscalonamientoDC escalona = new RAEscalonamientoDC();
                    //le asigna la solicitud a la persona del escalonamiento       modi                                 
                    foreach (var escalonamiento in solicitudAcumulativa.Escalonamiento)
                    {
                        RAEscalonamientoDC escalonamientoValidaResponsable = new RAEscalonamientoDC(escalonamiento) ;
                                                
                        DateTime nuevaFechaVencimiento = DateTime.Now;

                        if (dicParam.Key == escalonamiento.CargoEscalar.DocumentoEmpleado && solicitudAcumulativa.Escalonamiento.Count > 1)
                        {
                            continue;
                        }                        
                       
                        if (escalonamiento.IdTipoEscalonamiento == (int)RAEnumTipoEscalonamiento.ESCALONAMIENTO_RESPONSABLE_CENTRO_SERVICIO)
                        {
                            escalonamientoValidaResponsable = ObtenerResponsableEscalonamientoespecial(escalonamientoValidaResponsable, dicParam.Key);
                        }

                        if (escalonamiento.IdTipoHora == 1)
                        {
                            nuevaFechaVencimiento = CalcularFechaVencimientoHorarioLaboral(escalonamientoValidaResponsable.CargoEscalar, escalonamiento.HorasEscalar);
                        }
                        else
                        {
                            nuevaFechaVencimiento = nuevaFechaVencimiento.AddHours(escalonamiento.HorasEscalar);
                        }

                        escalona = escalonamiento;

                        solicitudAcumulativa.IdCargoSolicita = "0";
                        solicitudAcumulativa.IdEstado = RAEnumEstados.Asignado;
                        solicitudAcumulativa.FechaVencimiento = nuevaFechaVencimiento;
                        solicitudAcumulativa.FechaCreacion = DateTime.Now;
                        solicitudAcumulativa.IdCargoResponsable = escalonamientoValidaResponsable.CargoEscalar.IdCargoController;
                        solicitudAcumulativa.DocumentoSolicita = "";
                        solicitudAcumulativa.DocumentoResponsable = escalonamientoValidaResponsable.CargoEscalar.DocumentoEmpleado;
                        solicitudAcumulativa.idSucursal = escalonamientoValidaResponsable.CargoEscalar.Sucursal;
                        solicitudAcumulativa.IdSolicitudPadre = 0;


                        gestion = new RAGestionDC()
                        {
                            IdSolicitud = solicitudAcumulativa.IdSolicitud,
                            Comentario = sbSinHtml.ToString(),
                            IdCargoGestiona = solicitudAcumulativa.IdCargoResponsable,
                            CorreoEnvia = "MotorRaps",
                            //IdAccion = RAEnumAccion.Crear,
                            IdCargoDestino = solicitudAcumulativa.IdCargoResponsable,
                            CorreoDestino = escalonamientoValidaResponsable.CargoEscalar.Correo,
                            IdResponsable = solicitudAcumulativa.IdCargoResponsable,
                            IdEstado = RAEnumEstados.Asignado,
                            IdUsuario = "MotorRaps",
                            FechaVencimiento = nuevaFechaVencimiento,
                            DocumentoSolicita = solicitudAcumulativa.DocumentoSolicita,
                            DocumentoResponsable = solicitudAcumulativa.DocumentoResponsable,
                        };

                        break;
                    }


                    if (gestion != null)
                    {
                        Dictionary<string, object> parametros = new Dictionary<string, object>();

                        solicitudAcumulativa.ParametrosSolicitud.ForEach(pAcumula =>
                        {
                            parametros.Add(pAcumula.idParametro.ToString(), pAcumula.Valor);
                        });

                        bool solCreada = false;

                        using (TransactionScope trans = new TransactionScope())
                        {
                            //Las solicitudes quedan asignadas a la padre (ejemplo: noAlcanzo y FalsoMotivo => FallasMensajero)
                            solicitudAcumulativa.IdParametrizacionRap = solicitudAcumulativa.IdParametrizacionRapPadre;// solicitudes.Where(solOrg => solOrg.IdSolicitud == solicitudAcumulativa.IdSolicitud).First().IdParametrizacionRap;
                            solicitudAcumulativa.EsAcumulativa = true;
                            solicitudAcumulativa.FechaInicio = DateTime.Now;
                            long idSolicitud = RARepositorioSolicitudes.Instancia.CrearSolicitud(solicitudAcumulativa);
                            gestion.IdSolicitud = idSolicitud;
                            long idGestion = RARepositorioSolicitudes.Instancia.CrearGestion(gestion);

                            RARepositorioSolicitudes.Instancia.InsertarNotificacionGestionMotor(escalona, escalona.CargoEscalar, idGestion, idSolicitud);

                            //Actualiza las solicitudes hermana con el numero de solicitud generado                                
                            ss.ForEach(solHerm =>
                            {
                                RARepositorio.Instancia.ActualizarSolicitudAcumulativaConSolicitud(solHerm.IdSolicitud, idSolicitud);
                            });

                            RARepositorio.Instancia.InsertarUltimaEjecucionRapAutomatico(solicitudAcumulativa.IdParametrizacionRap, "MotorRaps");

                            trans.Complete();
                            solCreada = true;


                        }

                        try
                        {
                            if (solCreada)
                            {
                                StringBuilder sbCorreo = ValidarAmbientePruebasEnviarCorreo();

                                sbCorreo.AppendLine("<br>");
                                sbCorreo.AppendLine("<b>SE HA ASIGNADO UN NUEVO RAP</b>");
                                sbCorreo.AppendLine("<br>");
                                sbCorreo.AppendLine("<br>");
                                sbCorreo.AppendLine("<big><b>" + solicitudAcumulativa.NombreParametrizacionRapPadre + "</b></big>");
                                sbCorreo.AppendLine("<br>");
                                sbCorreo.AppendLine("<br>");
                                sbCorreo.AppendLine("Número de la solicitud: " + gestion.IdSolicitud);

                                sbCorreo.AppendLine("<br>");
                                sbCorreo.AppendLine("<br>");
                                sbCorreo.AppendLine("Fecha Vencimiento: <b>" + solicitudAcumulativa.FechaVencimiento.ToString("dd/MM/yyyy hh:mm tt") + "</b>");
                                sbCorreo.AppendLine("<br>");
                                sbCorreo.AppendLine("<br>");

                                sbCorreo.AppendLine("Tipo de solicitud:  " + sbConHtml.ToString());
                                sbCorreo.AppendLine();
                                sbCorreo.AppendLine();

                                //solicitudAcumulativa.ParametrosSolicitud.ForEach(pAcumula =>
                                //{
                                //    sbCorreo.AppendLine(pAcumula.descripcionParametro + "  :" + pAcumula.Valor);
                                //});


                                Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        CorreoElectronico.Instancia.Enviar(gestion.CorreoDestino, "Nuevo RAP Asignado", sbCorreo.ToString());
                                        // CorreoElectronico.Instancia.Enviar("arquitecto.desarrollo@interrapidisimo.com", "Nuevo RAP Asignado", sbCorreo.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Utilidades.AuditarExcepcion(ex, true);
                                    }
                                });

                                NotificarClienteConectado(gestion.IdSolicitud, solicitudAcumulativa.DocumentoResponsable);
                            }

                        }
                        catch
                        {
                            //validar porque se revienta por transacciones distribuidas
                        }

                    }
                }
            });
        }


        public RAEscalonamientoDC ObtenerResponsableEscalonamientoespecial(RAEscalonamientoDC escalonamiento, string valorAgrupamiento)
        {           
            IDictionary<string, object> parametrosRegla = new Dictionary<string, object>();
            parametrosRegla.Add("responsableFalla", valorAgrupamiento);

            RAReglasIngrecionesManualDC regla = RARepositorioSolicitudes.Instancia.ObtenerReglasIntegracionesTipoEscalonamiento(RAEnumTipoEscalonamiento.ESCALONAMIENTO_RESPONSABLE_CENTRO_SERVICIO);
            RAResponsableDC resultado = AdministradorReglasRaps.EjecutarRegla(regla.Assembly, regla.NameSpace, regla.Clase, parametrosRegla);
            if (!String.IsNullOrEmpty(resultado.CargoEscalona.DocumentoEmpleado))
            {
                escalonamiento.CargoEscalar = resultado.CargoEscalona;
                escalonamiento.Orden = 0;
            }           
            return escalonamiento;
        }
        


        /// <summary>
        /// Crea las solicitudes automaticasConSistema fuente que no tengan una parametrizacion de raps padre
        /// </summary>
        /// <param name="solicitudes"></param>
        private void CrearSolicitudAutConSistemaFuenteSinParamPadre(List<RASolicitudDC> solicitudes)
        {
            solicitudes.ForEach(solicitudAcumulativa =>
            {

                solicitudAcumulativa.Escalonamiento = solicitudAcumulativa.Escalonamiento.OrderBy(s => s.Orden).ToList();



                RAGestionDC gestion = null;



                //le asigna la solicitud a la persona del escalonamiento                                        
                foreach (var escalonamiento in solicitudAcumulativa.Escalonamiento)
                {
                    RACargoEscalarDC cargoEscalar = escalonamiento.CargoEscalar;
                    DateTime nuevaFechaVencimiento = DateTime.Now;
                    if (escalonamiento.IdTipoHora == 1)
                    {
                        nuevaFechaVencimiento = CalcularFechaVencimientoHorarioLaboral(cargoEscalar, escalonamiento.HorasEscalar);
                    }
                    else
                    {
                        nuevaFechaVencimiento = nuevaFechaVencimiento.AddHours(escalonamiento.HorasEscalar);
                    }

                    solicitudAcumulativa.IdCargoSolicita = "";
                    solicitudAcumulativa.IdEstado = RAEnumEstados.Asignado;
                    solicitudAcumulativa.FechaVencimiento = nuevaFechaVencimiento;
                    solicitudAcumulativa.FechaCreacion = DateTime.Now;
                    solicitudAcumulativa.IdCargoResponsable = cargoEscalar.IdCargoController;
                    solicitudAcumulativa.DocumentoSolicita = "";
                    solicitudAcumulativa.DocumentoResponsable = cargoEscalar.DocumentoEmpleado;
                    solicitudAcumulativa.idSucursal = cargoEscalar.Sucursal;
                    solicitudAcumulativa.IdSolicitudPadre = 0;


                    gestion = new RAGestionDC()
                    {
                        IdSolicitud = solicitudAcumulativa.IdSolicitud,
                        Comentario = "Solicitud Asignada",
                        IdCargoGestiona = "",
                        CorreoEnvia = "MotorRaps",
                        //IdAccion = RAEnumAccion.Crear,
                        IdCargoDestino = solicitudAcumulativa.IdCargoResponsable,
                        CorreoDestino = cargoEscalar.Correo,
                        IdResponsable = solicitudAcumulativa.IdCargoResponsable,
                        IdEstado = RAEnumEstados.Asignado,
                        IdUsuario = "MotorRaps",
                        FechaVencimiento = nuevaFechaVencimiento,
                        DocumentoSolicita = solicitudAcumulativa.DocumentoSolicita,
                        DocumentoResponsable = solicitudAcumulativa.DocumentoResponsable,
                    };

                    break;
                }

                if (gestion != null)
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    solicitudAcumulativa.ParametrosSolicitud.ForEach(p =>
                    {
                        parametros.Add(p.idParametro.ToString(), p.Valor);
                    });

                    bool solCreada = false;

                    using (TransactionScope trans = new TransactionScope())
                    {

                        long idSolicitud = RARepositorioSolicitudes.Instancia.CrearSolicitud(solicitudAcumulativa);
                        gestion.IdSolicitud = idSolicitud;
                        RARepositorioSolicitudes.Instancia.CrearGestion(gestion);
                        RARepositorio.Instancia.ActualizarSolicitudAcumulativaConSolicitud(solicitudAcumulativa.IdSolicitud, idSolicitud);
                        RARepositorio.Instancia.InsertarUltimaEjecucionRapAutomatico(solicitudAcumulativa.IdParametrizacionRap, "MotorRaps");

                        trans.Complete();
                        solCreada = true;


                    }

                    try
                    {
                        if (solCreada)
                        {

                            StringBuilder sb = ValidarAmbientePruebasEnviarCorreo();



                            sb.AppendLine("<br>");
                            sb.AppendLine("<b>SE HA ASIGNADO UN NUEVO RAP</b>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("Descripción: " + solicitudAcumulativa.Descripcion);
                            sb.AppendLine("<br>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("Número de la solicitud: " + gestion.IdSolicitud);
                            sb.AppendLine("<br>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("Fecha Vencimiento: <b>" + solicitudAcumulativa.FechaVencimiento.ToString("dd/MM/yyyy hh:mm tt") + "</b>");
                            sb.AppendLine("<br>");
                            sb.AppendLine("<br>");

                            sb.AppendLine("Tipo de solicitud:  <b>" + solicitudAcumulativa.Descripcion + "</b>");

                            solicitudAcumulativa.ParametrosSolicitud.ForEach(p =>
                            {
                                sb.AppendLine("<b>" + p.descripcionParametro + "</b>  :" + p.Valor);
                            });


                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    //CorreoElectronico.Instancia.Enviar(gestion.CorreoDestino, "Nuevo RAP Asignado", sb.ToString());
                                }
                                catch (Exception ex)
                                {
                                    Utilidades.AuditarExcepcion(ex, true);
                                }
                            });

                            NotificarClienteConectado(gestion.IdSolicitud, solicitudAcumulativa.DocumentoResponsable);
                        }

                    }
                    catch
                    {
                        //validar porque se revienta por transacciones distribuidas
                    }

                }

            });
        }

        /// <summary>
        /// Valida si es un ambiente de pruebas y retorna un StringBuilder con el encabezado de pruebas
        /// </summary>
        /// <returns></returns>
        private static StringBuilder ValidarAmbientePruebasEnviarCorreo()
        {
            StringBuilder sb = new StringBuilder();
            if (Convert.ToBoolean(PAParametros.Instancia.ConsultarParametrosFramework("EsAmbientePruebas")))
            {
                sb.AppendLine("<br>");
                sb.AppendLine("<big><b>------------------------------------------</b></big>");
                sb.AppendLine("<br>");
                sb.AppendLine("<big><b>***************** CORREO DE PRUEBA ! IGNORAR ! *****************</b></big>");
                sb.AppendLine("<br>");
                sb.AppendLine("<big><b>------------------------------------------</b></big>");
                sb.AppendLine("<br>");
            }
            return sb;
        }

        #endregion

        #region MotorCitas
        /// <summary>
        /// Envia los recordatorios de las citas a los integrantes
        /// </summary>
        public void EnviarRecordatoriosCitasMotor()
        {
            List<RANotificacionCitaMotorDC> lstCitasNotificar = RACitas.Instancia.ObtenerRecordatoriosCitasMotor();

            lstCitasNotificar.ForEach(cita =>
            {

                StringBuilder sb = ValidarAmbientePruebasEnviarCorreo();

                sb.AppendLine("<br>");
                sb.AppendLine("<b>RECORDATORIO DE CITA</b>");
                sb.AppendLine("<br>");
                sb.AppendLine("<br>");
                sb.AppendLine("<b>" + cita.DescripcionCita.ToUpper() + "</b>");
                sb.AppendLine("<br>");
                sb.AppendLine("<br>");
                sb.AppendLine("FECHA CITA: " + cita.FechaInicioCita.ToString("dd/MM/yyyy HH:mm"));
                sb.AppendLine("<br>");
                sb.AppendLine("<br>");
                sb.AppendLine("SITIO CITA: " + cita.LugarCita);

                string usuario = ControllerContext.Current.Usuario;
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        //CorreoElectronico.Instancia.Enviar("arquitecto.desarrollo@interrapidisimo.com", "Recordatorio de cita", sb.ToString());
                        CorreoElectronico.Instancia.Enviar(cita.CorreoNotificar, "Recordatorio de cita", sb.ToString());
                        RACitas.Instancia.InsertarEjecucionRecordatorio(cita, usuario);
                    }
                    catch (Exception ex)
                    {
                        Utilidades.AuditarExcepcion(ex, true);
                    }
                });

                NotificarCitaClienteConectado(cita.DescripcionCita, cita.FechaInicioCita, cita.DocumentoIntegrante.ToString());


            });


        }

        #endregion
    }
}
