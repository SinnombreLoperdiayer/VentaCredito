using System;
using System.Collections.Generic;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using VentaCredito.Clientes.Datos.Repositorio;
using RestSharp;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq;
using System.Globalization;
using VentaCredito.Seguridad;
using VentaCredito.Transversal;

namespace VentaCredito.Recogidas
{
    public class RecogidasNegocio
    {
        private static RecogidasNegocio instancia = new RecogidasNegocio();
        private enum SUEnumSuministro : int { SUMINISTRO_CONSECUTIVO_PREGUIA_CORPORATIVA = 24 }
        public static RecogidasNegocio Instancia
        {
            get
            {
                return instancia;
            }
        }


        /// <summary>
        /// Id Origen Api Insersión recogidas
        /// </summary>
        public const int IdOrigenSolRecogida = 24;

        /// <summary>
        /// Mensajes asociados una recogida
        /// /// </summary>
        public const string MensajeRecogidasAsociadas = "Algunos envíos ya tienen una orden de Recogida. Favor verificar";
        public const string MensajeRecogidasNoExistentes = "Algunos envíos asociados no existen. Favor verificar";
        public const string MensajeRecogidasNoSucursales = "Algunos envíos no pertenecen a la sucursal. Favor verificar.";
        public const string MensajeRecogidasGenerico = "Algunos envíos incluidos en la lista ya han sido procesados. Favor verificar.";
        /// <summary>
        /// Consumo servicios externos
        /// </summary>
        public String conexionApi(string conexion, string nombreApi)
        {
            String urlServicios = ConfigurationManager.AppSettings[conexion];

            if (String.IsNullOrEmpty(urlServicios))
            {
                throw new Exception("Url servidor " + nombreApi + " no encontrado en configuración");
            }
            else
            {
                return urlServicios;
            }
        }


        /// <summary>
        /// Inserta recogidas preenvios asociados a un cliente.
        /// Hevelin Dayana Diaz - 17/06/2021
        /// </summary>
        /// <param name="recogidas"></param>
        /// <returns>Objeto que contiene numero de recogida, numeros de preenvios a recoger y la fecha de la solicitud de recogida</returns>
        public ResponseRecogidas InsertarRecogidaCliente(RequestRecogidas recogidas, string usuario, string token)
        {
            Autenticacion autenticacion = SeguridadServicio.Instancia.ValidarCredencialesClienteCredito(usuario, token, recogidas.IdClienteCredito);

            if (autenticacion.EstaAutorizado)
            {
                ClienteCreditoVC cliente = ObtenerClienteCreditoActivo(recogidas.IdClienteCredito, recogidas.IdSucursalCliente);
                bool validarFestivo = false;
                bool validarHorario = false;
                if (cliente.IdCliente != 0)
                {
                    ZNOperacionRecogida localidad = ConsultarOperacionLocalidad(cliente.IdLocalidad);

                    if (localidad.PermiteRecogida)
                    {
                        if (recogidas.FechaRecogida != null)
                        {
                            if (recogidas.FechaRecogida < DateTime.Now)
                                throw new Exception("La fecha de la recogida no puede ser anterior a la fecha actual");
                            int diaFechaRecogida = (int)recogidas.FechaRecogida.DayOfWeek;
                            if (diaFechaRecogida == 0)
                            {
                                diaFechaRecogida = 7;
                            }
                            List<ZNTurnoOperacion> horarioOperacion = ConsultaOperRecogidaLocalidadDia(cliente.IdLocalidad, diaFechaRecogida);

                            //Valida si la ciudad opera el día seleccionado
                            if (horarioOperacion.Count > 0)
                            {
                                //recorre los turnos de recogidas para validar el con la hora enviada en la fecha de recogida
                                for (int x = 0; x < horarioOperacion.Count; x++)
                                {
                                    ZNTurnoOperacion turno = horarioOperacion[x];
                                    DateTime horaInicio = Convert.ToDateTime(turno.HorarioInicio);
                                    DateTime horaFin = Convert.ToDateTime(turno.HorarioFin);

                                    horaInicio = new DateTime(recogidas.FechaRecogida.Year, recogidas.FechaRecogida.Month, recogidas.FechaRecogida.Day, horaInicio.Hour, horaInicio.Minute, horaInicio.Second);
                                    horaFin = new DateTime(recogidas.FechaRecogida.Year, recogidas.FechaRecogida.Month, recogidas.FechaRecogida.Day, horaFin.Hour, horaFin.Minute, horaFin.Second);

                                    if ((recogidas.FechaRecogida >= horaInicio) && (recogidas.FechaRecogida <= horaFin))
                                    {
                                        validarHorario = true;
                                        break;
                                    }
                                    else
                                    {
                                        validarHorario = false;
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("El día seleccionado se encuentra fuera de la operación. Favor rectificar e intente nuevamente.");
                            }

                            //Valida hora de recogida
                            if (validarHorario)
                            {
                                //Valida aplica festivo administrador logistico
                                if (!localidad.AplicaFestivos)
                                {
                                    string fechaDesde = recogidas.FechaRecogida.Year + "-" + recogidas.FechaRecogida.Month + "-" + recogidas.FechaRecogida.Day;
                                    string fechaHasta = recogidas.FechaRecogida.Year + "-" + recogidas.FechaRecogida.Month + "-" + recogidas.FechaRecogida.Day;
                                    List<DateTime> listaDiasFestivos = ObtenerFestivos(fechaDesde, fechaHasta);

                                    if (listaDiasFestivos.Count <= 0)
                                    {
                                        validarFestivo = false;
                                    }
                                    else
                                    {
                                        validarFestivo = true;
                                    }
                                }
                                else
                                {
                                    validarFestivo = false;
                                }

                                if (!validarFestivo)
                                {
                                    int totalPiezasPreenvios = 0;
                                    decimal totalPesoEnvios = 0;
                                    List<long> numerosPreenvios = new List<long>();
                                    List<long> numerosPreenviosAsociados = new List<long>();
                                    List<long> numerosPreenviosNoExistentes = new List<long>();
                                    List<long> numerosPreenviosNoIncluidos = new List<long>();
                                    List<long> numerosPreenviosNoSucursales = new List<long>();
                                    List<int> entrada = new List<int>();
                                    string mensajePreenviosAsociados = string.Empty;
                                    string mensajePreenviosNoExistentes = string.Empty;
                                    string mensajePreenviosNoSucursales = string.Empty;

                                    if (recogidas.ListaNumPreenvios.Count > 0)
                                    {
                                        for (int i = 0; i < recogidas.ListaNumPreenvios.Count; i++)
                                        {
                                            int numPreenvio = int.Parse(recogidas.ListaNumPreenvios[i].ToString().Substring(0, 2));
                                            if (numPreenvio == (int)SUEnumSuministro.SUMINISTRO_CONSECUTIVO_PREGUIA_CORPORATIVA)
                                            {
                                                PreenvioAdmisionCL preenvio = ObtenerPreenvioPorNumero(recogidas.ListaNumPreenvios[i]);
                                                if (preenvio != null)
                                                {
                                                    if (preenvio.IdEstadoPreenvio == 11)
                                                    {
                                                        if (preenvio.IdClienteCredito == recogidas.IdClienteCredito && preenvio.CodigoConvenioRemitente == recogidas.IdSucursalCliente)
                                                        {
                                                            numerosPreenvios.Add(preenvio.NumeroPreenvio);
                                                            totalPesoEnvios = totalPesoEnvios + preenvio.Peso;
                                                            totalPiezasPreenvios = totalPiezasPreenvios + preenvio.NumeroPieza;
                                                        }
                                                        else
                                                        {
                                                            numerosPreenviosNoIncluidos.Add(preenvio.NumeroPreenvio);
                                                            numerosPreenviosNoSucursales.Add(preenvio.NumeroPreenvio);
                                                            mensajePreenviosNoSucursales = MensajeRecogidasNoSucursales;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (string.IsNullOrEmpty(mensajePreenviosAsociados))
                                                        {
                                                            mensajePreenviosAsociados = MensajeRecogidasAsociadas;
                                                        }
                                                        if (preenvio.IdEstadoPreenvio == 12)
                                                        {
                                                            numerosPreenviosAsociados.Add(preenvio.NumeroPreenvio);
                                                            numerosPreenviosNoIncluidos.Add(preenvio.NumeroPreenvio);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    numerosPreenviosNoIncluidos.Add(recogidas.ListaNumPreenvios[i]);
                                                    numerosPreenviosNoExistentes.Add(recogidas.ListaNumPreenvios[i]);
                                                    mensajePreenviosNoExistentes = MensajeRecogidasNoExistentes;
                                                }
                                            }
                                        }

                                        RGRecogidasCL recogida = new RGRecogidasCL();

                                        recogida.IdRemitente = cliente.IdCliente;
                                        recogida.TipoDocumento = "NI";
                                        recogida.NumeroDocumento = cliente.Nit;
                                        recogida.Nombre = cliente.RazonSocial;
                                        recogida.NumeroTelefono = cliente.Telefono;
                                        recogida.Direccion = cliente.Direccion;
                                        recogida.Correo = cliente.Email;
                                        recogida.Ciudad = cliente.IdLocalidad;
                                        recogida.NombreCiudad = cliente.NombreLocalidad;
                                        recogida.TipoRecogida = 2;
                                        recogida.FechaRecogida = Convert.ToDateTime(recogidas.FechaRecogida);
                                        recogida.TotalPiezas = totalPiezasPreenvios;
                                        recogida.PesoAproximado = totalPesoEnvios;
                                        recogida.DescripcionEnvios = "La recogida contiene " + totalPiezasPreenvios + " preenvios.";
                                        recogida.PreguntarPor = "";
                                        recogida.NumeroPreenvios = numerosPreenvios;
                                        recogida.IdClienteCredito = cliente.IdCliente;
                                        recogida.IdSucursalCliente = recogidas.IdSucursalCliente;
                                        recogida.IdOrigenSolRecogida = IdOrigenSolRecogida;

                                        var localidadDestino = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerInformacionLocalidad(cliente.IdLocalidad);
                                        //Consulta geolacalizacion direccion destinatario
                                        RequestGeomultizonaVC requestGeo = new RequestGeomultizonaVC();
                                        requestGeo.address = recogida.Direccion;
                                        requestGeo.city = localidadDestino.Nombre;
                                        Geolocalizacion geolocalizacion = GeolocalizacionNegocio.GeolocalizacionNegocio.Instancia.GeolocalizacionDireccion(requestGeo, "Recogida");

                                        if (geolocalizacion != null)
                                        {
                                            if (!string.IsNullOrEmpty(geolocalizacion.data.localidad))
                                            {
                                                var geo = geolocalizacion.data;
                                                recogida.Latitud = geo.latitude;
                                                recogida.Longitud = geo.longitude;
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(cliente.Latitud) & !string.IsNullOrEmpty(cliente.Longitud))
                                                {
                                                    recogida.Latitud = cliente.Latitud;
                                                    recogida.Longitud = cliente.Longitud;
                                                }
                                                else
                                                {
                                                    recogida.Latitud = "";
                                                    recogida.Longitud = "";
                                                }
                                            }
                                        }

                                        entrada.Add(numerosPreenvios.Count);
                                        entrada.Add(numerosPreenviosAsociados.Count);
                                        entrada.Add(numerosPreenviosNoExistentes.Count);
                                        entrada.Add(numerosPreenviosNoSucursales.Count);

                                        switch (entrada)
                                        {
                                            case List<int> x when x[0] <= 0 && x[1] > 0 && x[2] <= 0 && x[3] <= 0:
                                                throw new Exception("Todos los preenvios incluidos en la lista ya tienen una recogida. Favor verificar.");
                                            case List<int> x when x[0] <= 0 && x[1] <= 0 && x[2] > 0 && x[3] <= 0:
                                                throw new Exception("Los preenvios incluidos en la lista no existen o han sido procesados. Favor verificar.");
                                            case List<int> x when x[0] <= 0 && x[1] <= 0 && x[2] <= 0 && x[3] > 0:
                                                throw new Exception("Todos los preenvíos incluidos no pertenecen a la sucursal. Favor verificar.");
                                            case List<int> x when x[0] <= 0 && x[1] > 0 && x[2] > 0 && x[3] <= 0:
                                                throw new Exception("Los preenvios incluidos en la recogida no existen o ya están asociados a una recogida anterior. Favor Verificar.");
                                            case List<int> x when x[0] <= 0 && x[1] > 0 && x[2] <= 0 && x[3] > 0:
                                                throw new Exception("Algunos preenvios no pertenecen a la sucursal o están incluidos en una recogida previa. Favor verificar.");
                                            case List<int> x when x[0] > 0 && x[1] >= 0 && x[2] >= 0 && x[3] >= 0:
                                                ResponseRecogidas recogidaInsertada = InsertarRecogidaEsporadicaClienteCredito(recogida);
                                                recogidaInsertada.FechaSolicitud = recogida.FechaRecogida;
                                                recogidaInsertada.PreenviosAsociados = numerosPreenvios;

                                                if (recogidaInsertada.IdRecogida > 0 && numerosPreenvios.Count > 0 && numerosPreenviosAsociados.Count <= 0 && numerosPreenviosNoExistentes.Count <= 0 && numerosPreenviosNoSucursales.Count <= 0)
                                                {
                                                    recogidaInsertada.MensajePreenviosAsociados = "La recogida se generó Exitosamente.";
                                                }

                                                recogidaInsertada.MensajePreenviosNoIncluidos = ValidarMensajeRecogidaPreenviosNoIncluidos(mensajePreenviosAsociados, mensajePreenviosNoExistentes, mensajePreenviosNoSucursales, recogidaInsertada.MensajePreenviosAsociados);
                                                recogidaInsertada.PreenviosNoIncluidos = numerosPreenviosNoIncluidos;
                                                
                                                return recogidaInsertada;
                                            default:
                                                throw new Exception("Los preenvios incluidos en la lista ya han sido procesados. Favor verificar.");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("No hay preenvios asociados.");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Fecha corresponde a día festivo. Por favor, intente nuevamente.");
                                }
                            }
                            else
                            {
                                throw new Exception("El horario se encuentra fuera de la operación. Favor rectificar e intente nuevamente.");
                            }
                        }
                        else
                        {
                            throw new Exception("La fecha de solicitud de recogida es obligatoria.");
                        }
                    }
                    else
                    {
                        throw new Exception("No se encuentran habilitadas estas recogidas para la ciudad.");
                    }
                }
                else
                {
                    throw new Exception("Cliente no válido.");
                }
            }
            else
            {
                throw new Exception("Las credenciales no coinciden con el cliente crédito. Consulte con el administrador.");
            }
        }

        /// <summary>
        /// Metodo que se consume para insertar la recogida desde servicios recogidas 
        /// Hevelin Dayana Diaz - 17/06/2021 
        /// </summary>
        /// <param name="recogidaCL">Objeto recogida asociada a un cliente</param>
        /// <returns></returns>
        public ResponseRecogidas InsertarRecogidaEsporadicaClienteCredito(RGRecogidasCL recogidaCL)
        {
            String urlServiciosRecogidas = conexionApi("urlRecogidas", "recogidas");
            var client = new RestClient(urlServiciosRecogidas);
            string uri = "api/Recogidas/InsertarRecogidaEsporadicaCL";
            var requestRecogidas = new RestRequest(uri, Method.POST);
            requestRecogidas.Timeout = 10000000;
            requestRecogidas.AddHeader("Content-Type", "application/json");
            requestRecogidas.AddHeader("usuario", "usuario");
            requestRecogidas.AddJsonBody(recogidaCL);
            IRestResponse responseMessage = client.Execute(requestRecogidas);
            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<ResponseRecogidas>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new Exception(responseMessage.Content);
            }
        }

        /// <summary>
        /// Obtiene clientes creditos activos.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSucursal"></param>
        /// <returns>Retorna un cliente credito activo, con su respectiva sucursal y contrato activo y vegente.</returns>
        public ClienteCreditoVC ObtenerClienteCreditoActivo(long idCliente, long idSucursal)
        {
            return CLClienteCreditoRepositorio.Instancia.ObtenerClienteCreditoActivo(idCliente, idSucursal);
        }

        public PreenvioAdmisionCL ObtenerPreenvioPorNumero(long numero)
        {

            String urlServiciosPreenvios = conexionApi("urlApiPreenvios", "controller");
            var client = new RestClient(urlServiciosPreenvios);
            string uri = "api/Admision/ObtenerPreenvioCliente";
            var urlServiciosPreenviosC = new RestRequest(uri, Method.GET);
            urlServiciosPreenviosC.AddHeader("Content-Type", "application/json");
            urlServiciosPreenviosC.AddParameter("PreEnvioCliente", numero);
            IRestResponse responseMessage = client.Execute(urlServiciosPreenviosC);
            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<PreenvioAdmisionCL>(responseMessage.Content);
                return respuesta;
            }
            else if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.NoContent) && responseMessage.Content == "")
            {
                return null;
            }
            else
            {
                throw new Exception(responseMessage.Content);
            }
        }

        /// <summary>
        /// Obtiene los dias festivos desde base de datos
        /// Hevelin Dayana Diaz
        /// </summary>
        /// <param name="fechaDesde">fecha de inicio a consultar</param>
        /// <param name="fechaHasta">fecha fin a consultar</param>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivos(string fechaDesde, string fechaHasta)
        {
            String urlServiciosController = conexionApi("urlServiciosController", "controller");
            var client = new RestClient(urlServiciosController);
            string uri = "api/ParametrosFramework/ObtenerFestivosSinCache";
            var requestController = new RestRequest(uri, Method.GET);
            requestController.AddHeader("Content-Type", "application/json");
            requestController.AddHeader("usuario", "usuario");
            requestController.AddParameter("fechaDesde", fechaDesde);
            requestController.AddParameter("fechaHasta", fechaHasta);
            IRestResponse responseMessage = client.Execute(requestController);
            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<List<DateTime>>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new Exception(responseMessage.Content);
            }
        }

        /// <summary>
        /// Método que consulta las condiciones de la ciudad de recogida en el administrador logistico
        /// Hevelin Dayana Diaz Susa - 02/07/2021
        /// </summary>
        /// <param name="idLocalidad">Id de localidad donde se ejecutará la recogida</param>
        /// <returns></returns>
        public ZNOperacionRecogida ConsultarOperacionLocalidad(string idLocalidad)
        {
            String urlServiciosRecogidas = conexionApi("urlRecogidas", "recogidas");
            var client = new RestClient(urlServiciosRecogidas);
            string uri = "api/AdminZona/ConsultaOperacionRecogidaLocalidad";
            var requestRecogidas = new RestRequest(uri, Method.GET);
            requestRecogidas.AddHeader("Content-Type", "application/json");
            requestRecogidas.AddHeader("usuario", "usuario");
            requestRecogidas.AddParameter("idLocalidad", idLocalidad);
            IRestResponse responseMessage = client.Execute(requestRecogidas);
            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<ZNOperacionRecogida>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new Exception(responseMessage.Content);
            }
        }


        /// <summary>
        /// Método que consulta los turnos por ciudad de operacion de recogidas en el administrador logistico
        /// Hevelin Dayana Diaz Susa - 02/07/2021
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<ZNTurnoOperacion> ConsultaOperRecogidaLocalidadDia(string idLocalidad, int dia)
        {
            String urlServiciosRecogidas = conexionApi("urlRecogidas", "recogidas");
            var client = new RestClient(urlServiciosRecogidas);
            string uri = "api/AdminZona/ConsultaOperRecogidaLocalidadDia";
            var requestRecogidas = new RestRequest(uri, Method.GET);
            requestRecogidas.AddHeader("Content-Type", "application/json");
            requestRecogidas.AddHeader("usuario", "usuario");
            requestRecogidas.AddParameter("idLocalidad", idLocalidad);
            requestRecogidas.AddParameter("dia", dia);
            IRestResponse responseMessage = client.Execute(requestRecogidas);
            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<List<ZNTurnoOperacion>>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new Exception(responseMessage.Content);
            }
        }

        public static string ValidarMensajeRecogidaPreenviosNoIncluidos(string mensajePreenviosAsociados, string mensajePreenviosNoExistentes, string mensajePreenviosNoSucursales, string mensajeExitoso)
        {
            string mensaje = string.Empty;

            if (!string.IsNullOrEmpty(mensajePreenviosAsociados) && !string.IsNullOrEmpty(mensajePreenviosNoExistentes) && string.IsNullOrEmpty(mensajePreenviosNoSucursales))
            {
                mensaje = "Algunos preenvios ya tienen una orden de recogida o no existen. Favor verificar";
            }
            else if (!string.IsNullOrEmpty(mensajePreenviosAsociados) && string.IsNullOrEmpty(mensajePreenviosNoExistentes) && string.IsNullOrEmpty(mensajePreenviosNoSucursales))
            {
                mensaje = mensajePreenviosAsociados;
            }
            else if (!string.IsNullOrEmpty(mensajePreenviosNoExistentes) && string.IsNullOrEmpty(mensajePreenviosAsociados) && string.IsNullOrEmpty(mensajePreenviosNoSucursales))
            {
                mensaje = mensajePreenviosNoExistentes;
            }
            else if (string.IsNullOrEmpty(mensajePreenviosNoExistentes) && string.IsNullOrEmpty(mensajePreenviosAsociados) && !string.IsNullOrEmpty(mensajePreenviosNoSucursales))
            {
                mensaje = mensajePreenviosNoSucursales;
            }
            else if (string.IsNullOrEmpty(mensajeExitoso))
            {
                mensaje = MensajeRecogidasGenerico;
            }

            return mensaje;
        }

    }
}
