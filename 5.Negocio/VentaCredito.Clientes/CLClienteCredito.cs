using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Transversal.Entidades.Clientes;
using VentaCredito.Clientes.Datos.Repositorio;
using System.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Servicio.Entidades.Clientes;
using VentaCredito.Datos.Repositorio;
using Servicio.Entidades.Admisiones.Mensajeria;
using VentaCredito.Transversal.Enumerables;

namespace VentaCredito.Clientes
{
    public class CLClienteCredito
    {
        private static CLClienteCredito instancia = new CLClienteCredito();
        public Dictionary<int, string> mensajesEstadoDos = new Dictionary<int, string>();

        public static CLClienteCredito Instancia
        {
            get
            {
                return instancia;
            }
        }

        /// <summary>
        /// Propiedad con el listado de mensajes homologados para el estado 2
        /// </summary>
        public Dictionary<int, string> MensajesEstadoDos
        {
            get
            {
                if (mensajesEstadoDos.Count == 0)
                {
                    mensajesEstadoDos = ADCentroAcopioRepositorio.Instancia
                       .ObtenerMensajesEstadoDos()
                       .ToDictionary(m => m.IdMensaje, m => m.Mensaje);
                }
                return mensajesEstadoDos;
            }
        }

        /// <summary>
        /// Nombre parametro admision
        /// </summary>
        public const string NOMBRE_PARAMETRO = "MaxGuiasaConsultar";


        /// <summary>
        /// Validar el cupo de cliente
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="valorTransaccion"></param>
        /// <returns>"True" si se superó el porcentaje mínimo de aviso.</returns>
        public bool ValidarCupoCliente(int idContrato, decimal valorTransaccion)
        {
            return CLContrato.Instancia.ValidarCupoContrato(idContrato, valorTransaccion);
        }

        /// <summary>
        /// Modifica el acumulado de un contrato a partir de una valor de transaccion
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="valorTransaccion"></param>
        public void ModificarAcumuladoContrato(int idContrato, decimal valorTransaccion)
        {
            //CLRepositorio.Instancia.ModificarAcumuladoContrato(idContrato, valorTransaccion);
        }
        /// <summary>
        /// Servicio que inserta un cliente credito y le genera un usuario
        /// Hevelin Dayana Diaz Susa - 12/07/2021
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns>Retorna el id del cliente si se guarda</returns>
        public UsuarioIntegracion InsertarClienteCredito(RequestCLCredito cliente)
        {
            UsuarioIntegracion usuario = CLClienteCreditoRepositorio.Instancia.InsertarClienteCredito(cliente);

            if (!string.IsNullOrEmpty(usuario.UsuarioCliente) && !string.IsNullOrEmpty(usuario.PasswordCliente))
            {
                if (cliente.ServiciosCliente.Count > 0)
                {
                    for (int i = 0; i < cliente.ServiciosCliente.Count; i++)
                    {
                        InsertarServicioUsuario(usuario.UsuarioCliente, cliente.ServiciosCliente[i]);
                    }
                }

                if (cliente.EstadosCliente != null)
                {
                    string idEstados = string.Join(",", cliente.EstadosCliente);
                    if (cliente.EstadosCliente.Count > 0)
                    {
                        InsertarEstadosGuiaClientes(cliente.IdCliente, idEstados);
                    }
                    CLClienteCreditoRepositorio.Instancia.ActivarPushClienteCredito(cliente);
                }
                else
                {
                    CLClienteCreditoRepositorio.Instancia.DesactivarPushClienteCredito(cliente);
                }

                string usuarioBase64 = "Basic" + " " + ConvertUserToBase64(usuario.UsuarioCliente + ":" + usuario.PasswordCliente);
                ResponseToken token = consumoTokenSeguridad(usuario.UsuarioCliente, usuarioBase64);
                UsuarioIntegracion usuarioNew = new UsuarioIntegracion();
                usuarioNew.UsuarioCliente = usuario.UsuarioCliente;
                usuarioNew.Token = token.token_type + " " + token.access_token;
                CLClienteCreditoRepositorio.Instancia.InsertarTokenUsuarioIntegracion(usuarioNew.Token, usuario.UsuarioCliente);
                return usuarioNew;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// metodo que inserta los servicios seleccionados por un determinado cliente
        /// Hevelin Dayana Diaz Susa - 12/07/2021
        /// </summary>
        /// <param name="usuario">el usuario creado en usuario integracion</param>
        /// <param name="idServicio">id servicio seleccionado por el cliente</param>
        public void InsertarServicioUsuario(string usuario, int idServicio)
        {
            CLClienteCreditoRepositorio.Instancia.InsertarServicioUsuario(usuario, idServicio);
        }

        /// <summary>
        /// metodo que obtiene el listado de todos los servicios creados en base de datos iseguridad
        /// Hevelin Dayana Diaz Susa - 12/07/2021
        /// </summary>
        /// <returns>Lista todos los servicios creados en tabla SERVICIOS_SEG ISEGURIDAD</returns>
        public List<Servicios_SEG> ObtenerServiciosSeguridad()
        {
            return CLClienteCreditoRepositorio.Instancia.ObtenerServiciosSeguridad();
        }

        /// <summary>
        /// Metodo que encripta un string(usuario:password)
        /// </summary>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        public string ConvertUserToBase64(string userPassword)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(userPassword);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Metodo que genera token para un usuario ya creado, para que pueda consumir los servicios de venta credito
        ///Hevelin Dayana Diaz - 15/07/2021
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="tokenUsuario"></param>
        /// <returns></returns>
        public ResponseToken consumoTokenSeguridad(string usuario, string tokenUsuario)
        {
            var urlVentaCredito = ConfigurationManager.AppSettings["urlVentaCredito"];
            if (String.IsNullOrEmpty(urlVentaCredito))
            {
                throw new Exception("Url servidor venta credito no encontrado en configuración");
            }
            var token = new RestClient(urlVentaCredito);
            var uri = "Autorizacion/token";
            var request = new RestRequest(uri, Method.POST);
            request.AddHeader("x-app-signature", usuario);
            request.AddHeader("Authorization", tokenUsuario);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=password", ParameterType.RequestBody);
            IRestResponse response = token.Execute(request);
            if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK) && response.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<ResponseToken>(response.Content);
                return respuesta;
            }
            else
            {
                throw new Exception("No es posible conectarse con servicios venta credito.");
            }
        }

        /// <summary>
        /// metodo que inserta los estados seleccionados por un determinado cliente para proceso push
        /// Hevelin Dayana Diaz Susa - 06/10/2021
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idEstado"></param>
        public void InsertarEstadosGuiaClientes(Int64 idCliente, string idEstados)
        {
            CLClienteCreditoRepositorio.Instancia.InsertarEstadoGuiaClientes(idCliente, idEstados);
        }

        /// <summary>
        /// Servicio que consulta los estados de una cantidad parametrizada de guias asociadas a un cliente
        /// Según sea el caso, devuelve los estados por los que haya pasado la Guia, el Preenvio y la Recogida
        /// Hevelin Dayana Diaz - 11/10/2021
        /// </summary>
        /// <param name="request">Objeto que contiene id de cliente y número de guias a consultar</param>
        /// <returns></returns>
        public ResponseEstadosGuia_CLI ConsultarEstadosGuiaPorCliente(RequestEstadosGuia_CLI request)
        {
            bool guiasNoClientePreenvio = false, guiasNoClienteGuia = false, guiasNoClienteRecogida = false;

            List<EstadoGuiaCLI_MEN> estadosGuia = new List<EstadoGuiaCLI_MEN>();
            List<EstadoGuiaCLI_MEN> estadosPreenvio = new List<EstadoGuiaCLI_MEN>();
            List<EstadoGuiaCLI_MEN> estadosRecogida = new List<EstadoGuiaCLI_MEN>();

            ResponseEstadosGuia_CLI responseEstadosGuia_CLI = new ResponseEstadosGuia_CLI();
            List<GuiasEstados> guias = new List<GuiasEstados>();
            GuiasEstados guia = new GuiasEstados();

            List<long> guiasNoEncontradas = new List<long>();
            List<long> guiasNoClientes = new List<long>();
            string numeroMaxGuias = ConsultarNumeroMaximoGuias(NOMBRE_PARAMETRO);
            if (request.IdCliente != 0)
            {
                if (request.NumeroGuias.Count > 0)
                {
                    if (request.NumeroGuias.Count <= Convert.ToInt16(numeroMaxGuias))
                    {
                        for (int i = 0; i < request.NumeroGuias.Count; i++)
                        {
                            long nroGuia = request.NumeroGuias[i];
                            guia = new GuiasEstados();

                            if (nroGuia.ToString().StartsWith("3000"))
                            {
                                estadosGuia = AdmisionMensajeriaRepositorio.Instancia.ConsultarEstadosGuiaDevolucionPorGuia(nroGuia);

                                if (estadosGuia != null && estadosGuia.Count > 0)
                                {
                                    guia = new GuiasEstados()
                                    {
                                        NumeroGuia = nroGuia,
                                        DetalleMotivoDevolucion = AdmisionMensajeriaRepositorio.Instancia.ConsultarMotivoDevolucionGuia(nroGuia).MotivoDevolucion,
                                        EstadosGuia = estadosGuia
                                    };

                                    guias.Add(guia);
                                }
                                else
                                {
                                    guiasNoEncontradas.Add(nroGuia);
                                }
                            }
                            else
                            {
                                estadosGuia = ConsultarEstadosGuiaConMensajesActualizados(request.IdCliente, nroGuia);
                                estadosPreenvio = AdmisionMensajeriaRepositorio.Instancia.ConsultarEstadosPreenvioPorGuia(request.IdCliente, nroGuia);
                                estadosRecogida = AdmisionMensajeriaRepositorio.Instancia.ConsultarEstadosRecogidaPorGuia(request.IdCliente, nroGuia);

                                if (estadosPreenvio != null || estadosGuia != null || estadosRecogida != null)
                                {
                                    //Estados del Preenvio
                                    if (estadosPreenvio != null && estadosPreenvio.Count > 0)
                                    {
                                        var guiasNoCliente = estadosPreenvio.Where(x => x.IdClienteCredito == request.IdCliente).ToList();
                                        if (guiasNoCliente.Count > 0)
                                        {
                                            guia.NumeroGuia = nroGuia;
                                            guia.EstadosPreenvio = estadosPreenvio;
                                            guiasNoClientePreenvio = true;
                                        }
                                        else
                                        {
                                            if (!guiasNoClientes.Exists(x => x == nroGuia))
                                            {
                                                guiasNoClientes.Add(nroGuia);
                                            }
                                        }
                                    }

                                    //Estados de la Guia
                                    if (estadosGuia != null && estadosGuia.Count > 0)
                                    {
                                        var guiasNoCliente = estadosGuia.Where(x => x.IdClienteCredito == request.IdCliente).ToList();
                                        if (guiasNoCliente.Count > 0)
                                        {
                                            guia.NumeroGuia = nroGuia;
                                            guia.EstadosGuia = estadosGuia;
                                            guiasNoClienteGuia = true;
                                        }
                                        else
                                        {
                                            if (!guiasNoClientes.Exists(x => x == nroGuia))
                                            {
                                                guiasNoClientes.Add(nroGuia);
                                            }
                                        }
                                    }

                                    //Estados de la Recogida
                                    if (estadosRecogida != null && estadosRecogida.Count > 0)
                                    {
                                        var guiasNoCliente = estadosRecogida.Where(x => x.IdClienteCredito == request.IdCliente).ToList();
                                        if (guiasNoCliente.Count > 0)
                                        {
                                            guia.NumeroGuia = nroGuia;
                                            guia.EstadosRecogida = estadosRecogida;
                                            guiasNoClienteRecogida = true;
                                        }
                                        else
                                        {
                                            if (!guiasNoClientes.Exists(x => x == nroGuia))
                                            {
                                                guiasNoClientes.Add(nroGuia);
                                            }
                                        }
                                    }


                                    if (estadosPreenvio != null || estadosGuia != null || estadosRecogida != null)
                                    {
                                        if ((estadosPreenvio != null && estadosPreenvio.Count > 0) || (estadosGuia != null && estadosGuia.Count > 0) || (estadosRecogida != null && estadosRecogida.Count > 0))
                                        {
                                            if (guiasNoClientePreenvio == true || guiasNoClienteGuia == true || guiasNoClienteRecogida == true)
                                            {
                                                guias.Add(guia);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    guiasNoEncontradas.Add(nroGuia);
                                }
                            }
                        }

                        if (guiasNoEncontradas.Count > 0)
                        {
                            var NoGuias = "Algunas guías no existen o no se encuentran admitidas ( " + string.Join(",", guiasNoEncontradas) + " ).";
                            throw new Exception(NoGuias);
                        }
                        else
                        {
                            if (guias.Count > 0)
                            {
                                if (guiasNoClientes.Count > 0)
                                {
                                    var msjGuiaCliente = "Algunas guías no corresponden con el cliente.";
                                    responseEstadosGuia_CLI.ListadoGuiasNoCliente = guiasNoClientes;
                                    responseEstadosGuia_CLI.MensajeGuiasNoCliente = msjGuiaCliente;
                                }
                                responseEstadosGuia_CLI.ListadoGuias = guias;
                                return responseEstadosGuia_CLI;
                            }
                        }
                    }
                    else
                    {
                        string mensaje = "La entrada sobrepasa el límite máximo de " + numeroMaxGuias + " guías a consultar.";
                        throw new Exception(mensaje);
                    }
                }
                else
                {
                    throw new Exception("Debe incluir al menos un número de guía para su consulta.");
                }
            }
            else
            {
                throw new Exception("Debe incluir el id del cliente para su consulta.");
            }
            return null;
        }

        /// <summary>
        /// Servicio que consulta los estados de la guia de un cliente y actualiza los mensajes para estado 2 (Centro Acopio).
        /// Brayan Baez - 06/11/2024
        /// </summary>
        /// <param name="idCliente">Id cliente credito</param>
        /// <param name="numeroGuia">numero Guia</param>
        /// <returns>Lista de estados guia asociada a un cliente credito</returns>
        public List<EstadoGuiaCLI_MEN> ConsultarEstadosGuiaConMensajesActualizados(long idCliente, long numeroGuia)
        {
            List<EstadoGuiaCLI_MEN> estadosGuia = new List<EstadoGuiaCLI_MEN>();

            estadosGuia = AdmisionMensajeriaRepositorio.Instancia.ConsultarEstadosGuiaPorCliente(idCliente, numeroGuia);

            if (estadosGuia == null)
            {
                return null;
            }

            Dictionary<int, string> mensajes = MensajesEstadoDos;

            List<EstadoGuiaCLI_MEN> estadosDos = estadosGuia
                .Where(e => e.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio)
                .OrderBy(e => e.FechaEstado)
                .ToList();

            string ObtenerMensaje(int mensajeInicial, int mensajeAlterno, ref int contador)
            {
                return contador++ == 0 ? mensajes[mensajeInicial] : mensajes[mensajeAlterno];
            }

            int contadorIndiferente = 0, contadorOrigen = 0, contadorDestino = 0;

            estadosDos.ForEach(estadoActual => {

                if (estadoActual.IdLocalidadOrigen == estadoActual.IdLocalidadDestino)
                {
                    estadoActual.DescripcionAsociada = ObtenerMensaje((int)EnumMensajeCentroAcopio.EnCentroLogistico, (int)EnumMensajeCentroAcopio.RetornoCentroLogistico, ref contadorIndiferente);
                }
                else if (estadoActual.IdLocalidadenCurso == estadoActual.IdLocalidadOrigen)
                {
                    estadoActual.DescripcionAsociada = ObtenerMensaje((int)EnumMensajeCentroAcopio.EnCentroLogisticoOrigen, (int)EnumMensajeCentroAcopio.RetornoCentroLogisticoOrigen, ref contadorOrigen);
                }
                else if (estadoActual.IdLocalidadenCurso == estadoActual.IdLocalidadDestino)
                {
                    estadoActual.DescripcionAsociada = ObtenerMensaje((int)EnumMensajeCentroAcopio.EnCentroLogisticoDestino, (int)EnumMensajeCentroAcopio.RetornoCentroLogisticoDestino, ref contadorDestino);
                }
                else
                {
                    estadoActual.DescripcionAsociada = mensajes[(int)EnumMensajeCentroAcopio.EnCentroLogisticoTransito];
                }
            });

            return estadosGuia;
        }

        public string ConsultarNumeroMaximoGuias(string NombreParametro)
        {
            return CLClienteCreditoRepositorio.Instancia.ConsultarNumeroMaximoGuias(NombreParametro);
        }

        /// <summary>
        /// Servicio que consulta las sucursales activas asociadas  a un cliente, teniendo en cuenta el contrato actual.
        /// Hevelin Dayana Diaz - 28/04/2023
        /// </summary>
        /// <param name="idCliente">Id cliente credito</param>
        /// <returns>Lista de sucursales asociadas a un cliente credito</returns>
        public List<SucursalCliente_CLI> ObtenerSucursalesActivasPorCliente(int idCliente)
        {
            return CLClienteCreditoRepositorio.Instancia.ObtenerSucursalesActivasPorCliente(idCliente);
        }

        /// <summary>
        /// Actualiza la información de recogida de un cliente crédito
        /// Mauricio Hernandez Cabrera - 18/08/2023 - HU 51407
        /// </summary>
        /// <returns>Mensaje Satisfactorio o Fallido del proceso de actualización de la información de recogida del cliente crédito</returns>
        public string ActualizarInfoRecogidaClienteCredito(RequestCLCredito reqCliente)
        {
            if (string.IsNullOrEmpty(reqCliente.DireccionCliente.Trim()) || string.IsNullOrEmpty(reqCliente.TelefonoCliente.Trim()) ||
                string.IsNullOrEmpty(reqCliente.LocalidadCliente.Trim()) || reqCliente.IdCliente.Equals(0) || reqCliente.IdSucursal.Equals(0))
            {
                return "Proceso FALLIDO. Todos los datos son obligatorios. Por favor validar.";
            }

            CLClientesDC cliente = CLClienteCreditoRepositorio.Instancia.ObtenerCliente(reqCliente.IdCliente);
            if (cliente == null || cliente.IdCliente.Equals(0))
            {
                return "Proceso FALLIDO. Cliente no existe.";
            }

            CLSucursalDC sucursal = CLConsultas.Instancia.ObtenerSucursalCliente(reqCliente.IdSucursal, new CLClientesDC() { IdCliente = Convert.ToInt32(reqCliente.IdCliente) });
            if (sucursal == null || sucursal.IdSucursal.Equals(0))
            {
                return "Proceso FALLIDO. Sucursal no existe.";
            }
            else if (cliente.IdCliente != sucursal.IdCliente)
            {
                return "Proceso FALLIDO. Sucursal no se encuentra asociada al cliente.";
            }

            bool existeLocalidad = LocalidadesNegocio.LocalidadesNegocio.Instancia.ConsultarExistenciaLocalidad(reqCliente.LocalidadCliente);
            if (!existeLocalidad)
            {
                return "Proceso FALLIDO. Código DANE no existe.";
            }

            reqCliente.IdZona = LocalidadesNegocio.LocalidadesNegocio.Instancia.ConsultarZonaHabilitadaParaNuevaLocalidad(reqCliente.LocalidadCliente);

            return CLClienteCreditoRepositorio.Instancia.ActualizarInfoRecogidaClienteCredito(reqCliente);
        }

    }
}