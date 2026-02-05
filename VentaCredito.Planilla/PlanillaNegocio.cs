using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Newtonsoft.Json;
using RestSharp;
using Servicio.Entidades.Clientes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using VentaCredito.Clientes;
using VentaCredito.Clientes.Datos.Repositorio;
using VentaCredito.Planilla.Datos;
using VentaCredito.Planilla.Datos.Interfaces;
using VentaCredito.Planilla.Interfaces;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using VentaCredito.Transversal;
using VentaCredito.Transversal.Entidades.Planilla;
using VentaCredito.Transversal.Enumerables;
using NovedadConsolidado = CO.Servidor.Servicios.ContratoDatos.CentroAcopio.NovedadConsolidado;

namespace VentaCredito.Planilla
{
    public class PlanillaNegocio : IPlanillaNegocio
    {
        private readonly IPlanillaDatos datos = PlanillaDatos.Instancia;
        #region Intancia
        private static volatile PlanillaNegocio instancia;

        /// <summary>
        /// Atributo utilizado para evitar problemas con multithreading en el singleton.
        /// </summary>
        private static object syncRoot = new Object();

        public static PlanillaNegocio Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (syncRoot)
                    {
                        if (instancia == null)
                        {
                            instancia = new PlanillaNegocio();
                        }
                    }
                }
                return instancia;
            }
        }
        #endregion

        #region Metodos
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
        /// Realiza validaciones del consolidado y cambia el estado
        /// </summary>
        /// <param name="reserva"></param>
        /// <returns></returns>
        public string ReservarConsolidadoPrecinto(CAReservaPrecintoConsolidado reserva)
        {
            bool consolidadoValido;
            bool precintoValido;
            int estadoXValidarConsolidado;
            int estadoXValidarPrecinto;
            string mensajeRetorno;

            var datosTulaContenedorPrecinto = ObtenerDatosTulaContenedorPrecinto(reserva);
            CAMovimientoConsolidadoDCIntegra consolidado = datosTulaContenedorPrecinto.Where(r => r.IdTipoConsolidado!=3).SingleOrDefault();
            CAMovimientoConsolidadoDCIntegra precinto = datosTulaContenedorPrecinto.Where(r => r.IdTipoConsolidado == 3).SingleOrDefault();

            if (consolidado != null)
            {
                CAMovimientoConsolidadoDCIntegra movimientoConsolidado = new CAMovimientoConsolidadoDCIntegra
                {
                    NumeroPrecinto = precinto == null ? 0 : precinto.NumeroPrecinto,
                    IdCentroServicioDestino = reserva.IdCentroServicio,
                    NumeroConsolidado = consolidado.NumeroConsolidado,
                    IdTipoMovimiento = reserva.EsReservada ? (int)EnumTipoMovimientoConsolidado.Salida : (int)EnumTipoMovimientoConsolidado.Ingresado,
                    IdTipoConsolidado = consolidado.IdTipoConsolidado
                };
                estadoXValidarConsolidado = movimientoConsolidado.IdTipoMovimiento == (int)EnumTipoMovimientoConsolidado.Salida ? (int)EnumTipoMovimientoConsolidado.Ingresado : (int)EnumTipoMovimientoConsolidado.Salida;

                mensajeRetorno = reserva.EsReservada ? "Reserva exitosa" : "Liberación Exitosa";

                if (precinto != null)
                {
                    LogMovimientoPrecintoPUAIntegra movimientoPrecinto = new LogMovimientoPrecintoPUAIntegra
                    {
                        IdTipoMovimientoPrecinto = reserva.EsReservada ? (int)EnumTipoMovimientoPrecinto.CONSUMIDO : (int)EnumTipoMovimientoPrecinto.CREADO,
                        NumeroPrecinto = precinto.NumeroPrecinto.ToString(),
                        Fecha = DateTime.Now.ToShortDateString(),
                        Usuario = "Admin"
                    };
                    estadoXValidarPrecinto = movimientoPrecinto.IdTipoMovimientoPrecinto == (int)EnumTipoMovimientoPrecinto.CONSUMIDO ? (int)EnumTipoMovimientoPrecinto.CREADO : (int)EnumTipoMovimientoPrecinto.CONSUMIDO;

                    consolidadoValido = datos.ValidarInsertarMovimientoConsolidadoUrbano(movimientoConsolidado, estadoXValidarConsolidado);
                    precintoValido = datos.ValidarInsertarMovimientoPrecinto(movimientoPrecinto, estadoXValidarPrecinto, reserva.IdCentroServicio);

                    if (consolidadoValido && precintoValido)
                    {
                        CambioEstadoConsolidado(movimientoConsolidado);
                        CambioEstadoPrecinto(movimientoPrecinto);
                    }
                    else
                    {
                        throw new Exception("No existe en el inventario o se encuentra en uso. Favor validar");
                    }
                }
                else
                {
                    if (reserva.NumeroPrecinto == "0")
                    {
                        CambioEstadoConsolidado(movimientoConsolidado);
                    }
                    else
                    {
                        throw new Exception("El precinto no existe en el inventario. Favor validar");
                    }
                }
            }
            else
            {
                throw new Exception("El consolidado no existe en el inventario.Favor validar");
            }
            return mensajeRetorno;
        }

        /// <summary>
        /// Obtiene la información del consolidado (Tula o Contenedor) y el precinto
        /// </summary>
        /// <param name="reserva"></param>
        /// <returns></returns>
        public List<CAMovimientoConsolidadoDCIntegra> ObtenerDatosTulaContenedorPrecinto(CAReservaPrecintoConsolidado reserva)
        {
            String URIAdmisionServer = ConfigurationManager.AppSettings["urlServiciosController"];

            if (String.IsNullOrEmpty(URIAdmisionServer))
            {
                throw new Exception("Url servidor controller no encontrado en configuración");
            }

            string hostAdmision = URIAdmisionServer;

            var clientGuia = new RestClient(hostAdmision);
            string uri = "api/CentroAcopio/ObtenerDatosTulaContenedorPrecinto/" + reserva.IdTipo + "/" + reserva.NumeroConsolidado + "/" + reserva.NumeroPrecinto;
            var request = new RestRequest(uri, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Usuario", "Usuario");
            IRestResponse responseMessage = clientGuia.Execute(request);

            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                if (responseMessage.Content != null)
                {
                    var respuesta = JsonConvert.DeserializeObject<List<CAMovimientoConsolidadoDCIntegra>>(responseMessage.Content);
                    return respuesta;
                }
                else
                {
                    throw new Exception("No existe en el inventario. Favor validar");
                }
            }
            else
            {
                throw new Exception("No es posible conectarse con servicios controller");
            }
        }

        /// <summary>
        /// cambia el estado del consolidado
        /// </summary>
        /// <param name="movimientoConsolidado"></param>
        /// <param name="idEstado"></param>
        public int CambioEstadoConsolidado(CAMovimientoConsolidadoDCIntegra movimientoConsolidado)
        {
            var respuesta = datos.InsertarMovimientoConsolidado(movimientoConsolidado);
            return respuesta;
        }

        /// <summary>
        /// Valida y cambia el estado del precinto
        /// </summary>
        /// <param name="logMovimiento"></param>
        /// <param name="idEstado"></param>
        public string CambioEstadoPrecinto(LogMovimientoPrecintoPUAIntegra logMovimiento)
        {
            var respuesta = datos.InsertarLogMovimientoPrecinto(logMovimiento);
            return respuesta;
        }
        /// <summary>
        /// Obtiene los horarios de recogidas por el id del centroServicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns>Lista de objeto de tipo RGDatosRecogidaCentroServicioDC </returns>
        public List<RGDatosRecogidaCentroServicioDC> ObtenerHorariosRecogidas(long idCentroServicio)
        {
            List<RGDatosRecogidaCentroServicioDC> horariosRecogidas = new List<RGDatosRecogidaCentroServicioDC>();
            var urlController = ConfigurationManager.AppSettings["urlServiciosController"];
            if (String.IsNullOrEmpty(urlController))
            {
                throw new Exception("Url servidor controller no encontrado en configuración");
            }
            var clienteReg = new RestClient(urlController);
            var uri = "api/Recogidas/ObtenerDatosRecogidasCentroServicio/" + idCentroServicio;
            var request = new RestRequest(uri, Method.GET);
            request.AddHeader("usuario", "usuario");
            IRestResponse response = clienteReg.Execute(request);
            if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                if (!response.Content.Equals("[]"))
                {
                    horariosRecogidas = JsonConvert.DeserializeObject<List<RGDatosRecogidaCentroServicioDC>>(response.Content);
                }
                else
                {
                    throw new Exception("No se encontraron horarios de recogidas configurados para el centro de servicio");
                }
            }
            else
            {
                throw new Exception("No es posible conectarse con servicios controller");
            }
            return horariosRecogidas;
        }
        /// <summary>
        /// Obtiene la información del mensajero de acuerdo a horario de recogida y cedula
        /// </summary>
        /// <param name="consultaMensajero"></param>
        /// <returns>objeto de tipo MensajeroRecogida</returns>
        public MensajeroRecogida ObtenerDatosMensajero(ConsultaMensajero consultaMensajero)
        {
            validarCamposConsultaMensajero(consultaMensajero);
            RGDatosRecogidaCentroServicioDC recogidaMensajero = null;
            Persona_Huella_AUT huellaMensajero = new Persona_Huella_AUT();
            MensajeroRecogida mensajeroRecogida = new MensajeroRecogida();

            var horariosRecogidas = ObtenerHorariosRecogidas(consultaMensajero.IdCentroServicio);

            //Busca en los horarios de recogida la hora que concuerde con la enviada desde el cliente
            foreach (var element in horariosRecogidas)
            {
                string horaRecStr = element.FechaHoraRecogida.ToString("H:mm");
                if (horaRecStr == consultaMensajero.HorarioRecogida)
                {
                    recogidaMensajero = element;
                }
            }

            if (recogidaMensajero != null)
            {
                if (recogidaMensajero.IdentificacionResponsable == consultaMensajero.NumeroIdentificacion)
                {
                    huellaMensajero = ObtenerHuellaMensajero(consultaMensajero.NumeroIdentificacion);
                    mensajeroRecogida.TipoIdentificación = consultaMensajero.TipoIdentificacion;
                    mensajeroRecogida.NumeroIdentificacion = recogidaMensajero.IdentificacionResponsable;
                    mensajeroRecogida.Nombre = recogidaMensajero.NombreResponsable;
                    mensajeroRecogida.FotoMensajero = recogidaMensajero.FotoMensajero;
                    mensajeroRecogida.HuellaDactilar = huellaMensajero.SerializadoHuella;
                    mensajeroRecogida.EsManoDerecha = huellaMensajero.ManoDerecha;
                    mensajeroRecogida.Dedo = huellaMensajero.Dedo;
                    mensajeroRecogida.IdSolicitudRecogida =  recogidaMensajero.IdSolicitudRecogida;
                    mensajeroRecogida.TelefonoMensajero = recogidaMensajero.TelefonoMensajero;
                }
                else
                {
                    throw new Exception("No se encontro mensajero, para la hora de recogida ingresada");
                }
            }
            else
            {
                throw new Exception("No se encontraron recogidas para la hora ingresada");
            }
            return mensajeroRecogida;
        }

        public void validarCamposConsultaMensajero(ConsultaMensajero consultaMensajero)
        {
            bool camposInValidos = (string.IsNullOrEmpty(consultaMensajero.HorarioRecogida) ||
                string.IsNullOrEmpty(consultaMensajero.NumeroIdentificacion) ||
                string.IsNullOrEmpty(consultaMensajero.TipoIdentificacion) || 
                consultaMensajero.IdCentroServicio == 0);
            if (camposInValidos)
            {
                throw new Exception("Faltan datos de ingreso, por favor valide");
            }

            if (consultaMensajero.TipoIdentificacion.ToLower() != "cc")
            {
                throw new Exception("Datos de entrada erróneos, favor rectificar");
            }
        }
        /// <summary>
        /// Obtiene la huella de un mensajero de aceurdo a la identificación
        /// </summary>
        /// <param name="numeroDocumento"></param>
        /// <returns>Un objeto de tipo Persona_Huella_AUT</returns>
        public Persona_Huella_AUT ObtenerHuellaMensajero(string numeroDocumento)
        {
            var objetoHuellaMensajero = datos.ObtenerHuellaMensajero(numeroDocumento);
            return objetoHuellaMensajero;
        }
        /// <summary>
        /// Obtiene clientes activos.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSucursal"></param>
        /// <returns>Retorna un cliente activo, con su respectiva sucursal y contrato activo y vegente.</returns>
        public ClienteCreditoVC ObtenerClienteCreditoActivo(long idCliente, long idSucursal)
        {
            return CLClienteCreditoRepositorio.Instancia.ObtenerClienteCreditoActivo(idCliente, idSucursal);
        }
        /// <summary>
        /// /Se obtiene la información del Preenvio
        /// </summary>
        /// <param name="numero"></param>
        /// <returns></returns>
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
        /// Genera la planilla de recolección de preenvios.
        /// Alejandro Cardenas 16/07/2021
        /// </summary>
        /// <param name="planillaRecoleccion"></param>
        /// <returns>planillaRecoleccionPreenviosResponse</returns>
        public PlanillaRecoleccionPreenviosResponse GenerarPlanillaRecoleccionPreenvios(PlanillaRecoleccionPreenviosRequest planillaRecoleccion)
        {
            PlanillaRecoleccionPreenviosResponse planillaRecoleccionPreenviosResponse = new PlanillaRecoleccionPreenviosResponse();
            PlanillaRecoleccionImpresionRequest planillaRecoleccionImpresionRequest = new PlanillaRecoleccionImpresionRequest();
            ClienteCreditoVC cliente = ObtenerClienteCreditoActivo(planillaRecoleccion.IdCliente, planillaRecoleccion.IdSucursal);
            if (cliente.IdCliente != 0)
            {
                CLSucursalDC sucursal = CLConsultas.Instancia.ObtenerSucursalCliente(planillaRecoleccion.IdSucursal, new CLClientesDC() { IdCliente = cliente.IdCliente });
                if (planillaRecoleccion.ListaNumPreenvios.Count > 0)
                {
                    planillaRecoleccionImpresionRequest = InsertarPlanillaPreenvio(planillaRecoleccion);
                }
                else
                {
                    throw new Exception("Verifique los datos de entrada.");
                }
                if (planillaRecoleccionImpresionRequest.NumerosPreenviosValidos.Count != 0)
                {
                    planillaRecoleccionImpresionRequest.IdCliente = planillaRecoleccion.IdCliente;
                    planillaRecoleccionImpresionRequest.IdSucursal = planillaRecoleccion.IdSucursal;
                    planillaRecoleccionImpresionRequest.NombreSucursal = sucursal.Nombre;
                    planillaRecoleccionImpresionRequest.Cliente = cliente;
                    planillaRecoleccionImpresionRequest.FechaRegistro = planillaRecoleccionImpresionRequest.FechaCreacion.ToString("dd/MM/yyyy");
                    planillaRecoleccionImpresionRequest.HoraCreacion = planillaRecoleccionImpresionRequest.FechaCreacion.ToString("HH:mm:ss");
                    planillaRecoleccionPreenviosResponse.CantidadPreenvios = planillaRecoleccionImpresionRequest.CantidadPreenvios;
                    planillaRecoleccionPreenviosResponse.FechaCreacion = planillaRecoleccionImpresionRequest.FechaCreacion;
                    planillaRecoleccionPreenviosResponse.NumeroPlanilla = planillaRecoleccionImpresionRequest.NumeroPlanilla;
                    planillaRecoleccionPreenviosResponse.MensajeCantidaMaximaPreenvios = planillaRecoleccionImpresionRequest.MensajeCantidaMaximaPreenvios;
                    planillaRecoleccionPreenviosResponse.NumerosPreenviosNoIncluidos = planillaRecoleccionImpresionRequest.NumerosPreenviosNoIncluidos;
                    planillaRecoleccionPreenviosResponse.MensajePreenviosInvalidos = planillaRecoleccionImpresionRequest.mensajePreenviosInvalidos;
                    planillaRecoleccionPreenviosResponse.NumerosPreenviosInvalidos = planillaRecoleccionImpresionRequest.numerosPreenviosInvalidos;
                    planillaRecoleccionPreenviosResponse.ArregloBytesPlanilla = ObtenerPdfPlanilla(planillaRecoleccionImpresionRequest);
                }
                else
                {
                    throw new Exception("Verifique los datos de entrada.");
                }
                return planillaRecoleccionPreenviosResponse;
            }
            else
            {
                throw new Exception("La identificación del cliente o sucursal es errada. Por favor verifique los datos e intente nuevamente.");
            }
        }
        /// <summary>
        /// Se inserta el registro de la planilla que se esta generando.
        /// Alejandro Cardenas 16/07/2021
        /// </summary>
        /// <param name="planillaRecoleccion"></param>
        /// <returns></returns>
        public PlanillaRecoleccionImpresionRequest InsertarPlanillaPreenvio(PlanillaRecoleccionPreenviosRequest planillaRecoleccion)
        {
            String urlServiciosRecogidas = conexionApi("urlApiPreenvios", "controller");
            var client = new RestClient(urlServiciosRecogidas);
            string uri = "api/Planilla/InsertarPlanillaPreenvio";
            var requestPlanilla = new RestRequest(uri, Method.POST);
            requestPlanilla.Timeout = 10000000;
            requestPlanilla.AddHeader("Content-Type", "application/json");
            requestPlanilla.AddJsonBody(planillaRecoleccion);
            IRestResponse responseMessage = client.Execute(requestPlanilla);
            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<PlanillaRecoleccionImpresionRequest>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new Exception(responseMessage.Content);
            }
        }
        /// <summary>
        /// Obtiene los bytes del pdf que se genero.
        /// Alejandro Cardenas 16/07/2021
        /// </summary>
        /// <param name="planillaRecoleccion"></param>
        /// <returns></returns>
        public byte[] ObtenerPdfPlanilla(PlanillaRecoleccionImpresionRequest planillaRecoleccion)
        {
            //Consumo de servicio para obtener pdf de la planilla
            String urlAdmisionServer = ConfigurationManager.AppSettings["urlApiImpresionesMS"];

            if (String.IsNullOrEmpty(urlAdmisionServer))
            {
                throw new Exception("Url servidor impresion no encontrado en configuración");
            }
            var client = new RestClient(urlAdmisionServer);
            string uri = "api/Preenvios/GenerarPlanillaPreenvios";
            var requestPdf = new RestRequest(uri, Method.POST);
            requestPdf.Timeout = 10000000;
            requestPdf.AddHeader("Content-Type", "application/json");
            requestPdf.AddJsonBody(planillaRecoleccion);
            IRestResponse responseMessage = client.Execute(requestPdf);
            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<byte[]>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new Exception("No es posible conectarse con el servicio de impresion");
            }
        }


        /// <summary>
        /// Inserta planilla con envíos asociados, admitidos por el centro de servicio
        /// </summary>
        /// <param name="requestPlanilla"></param>
        /// <returns></returns>
        public ResponsePlanilla CrearPlanillaCentroServicio(RequestPlanilla requestPlanilla)
        {
            bool camposVacios = (requestPlanilla.IdCentroServicio == 0 || requestPlanilla.IdSolicitudRecogida == 0 || 
                string.IsNullOrEmpty(requestPlanilla.DocumentoMensajero) || string.IsNullOrEmpty(requestPlanilla.Usuario));
            if (camposVacios)
            {
                throw new Exception("Faltan datos de entrada, Favor verificar e Intente nuevamente");
            }

            string usuarioQueCrea = requestPlanilla.Usuario;
            int totalEnviosSueltos = requestPlanilla.ListaGuiasSueltas == null ? 0 : requestPlanilla.ListaGuiasSueltas.Count();
            int totalEnviosConsolidados = requestPlanilla.ListaGuiasConsolidadas == null ? 0 : requestPlanilla.ListaGuiasConsolidadas.Count();
            short totalEnvios = Convert.ToInt16 (totalEnviosSueltos + totalEnviosConsolidados);

            if (totalEnvios == 0)
            {
                throw new Exception("Debe ingresar almenos un envío");
            }
            int totalConsolidados = requestPlanilla.ListaConsolidados == null ? 0 : requestPlanilla.ListaConsolidados.Count();
            
            if(!requestPlanilla.NoTieneTula)
            {
                //Se grantiza que si viene una lista de guias consolidadas, debe venir una lista de consolidados(Tulas o Contenedores).
                if (totalEnviosConsolidados > 0 && totalConsolidados == 0 || totalConsolidados > 0 && totalEnviosConsolidados == 0)
                {
                    throw new Exception("Debe ingresar un listado de consolidados y un listado de guías");
                }
            }
            else
            {
                if(totalConsolidados > 0 && totalEnviosConsolidados == 0)
                {
                    throw new Exception("Debe ingresar un listado de consolidados y un listado de guías");
                }
            }
            

            OUPlanillaVentaDC planillaVenta = new OUPlanillaVentaDC();
            //Consultar info mensajero por documento
            var mensajeroPlanilla = ObtenerDatosMensajeroXDocumento(requestPlanilla.DocumentoMensajero);


          
            List<NovedadConsolidado> listaNovedades = new List<NovedadConsolidado>();
            
            if (requestPlanilla.NoTieneTula)
            {
                NovedadConsolidado novedad = new NovedadConsolidado();
                novedad.IdNovedad = (int)EnumTipoNovedades.NoTieneTula;
                novedad.Descripcion = "No Tiene Tula";
                listaNovedades.Add(novedad);
            }

            if (requestPlanilla.NoTienePrecinto)
            {
                NovedadConsolidado novedad = new NovedadConsolidado();
                novedad.IdNovedad = (int)EnumTipoNovedades.NoTienePrecinto;
                novedad.Descripcion = "No Tiene Precinto";
                listaNovedades.Add(novedad);
            }



            //Consultar info centro servicio por id
            var centroServicioPlanilla = ObtenerInfoCentroServicioXId(requestPlanilla.IdCentroServicio);

            //Mapear campos en el objeto planilla
            planillaVenta.IdPuntoServicio = centroServicioPlanilla.IdCentroServicio;
            planillaVenta.NombreCentroServicios = centroServicioPlanilla.NombreCentroServicio;
            planillaVenta.DireccionPuntoServicio = centroServicioPlanilla.DireccionCentroServicio;
            planillaVenta.IdMensajero = mensajeroPlanilla.IdMensajero;
            planillaVenta.NombreCompleto = mensajeroPlanilla.NombreMensajero;
            planillaVenta.TotalEnvios = totalEnvios;
            planillaVenta.IdSolicitudRecogida = requestPlanilla.IdSolicitudRecogida;
            planillaVenta.ListaNovedad = listaNovedades;             

            List <CAMovimientoConsolidadoDC> ltsMovimientosConsolidados = new List<CAMovimientoConsolidadoDC>();
            var consolidadoListas = new List<OUPlanillaVentaGuiasDC>();
            if (requestPlanilla.ListaConsolidados != null && requestPlanilla.ListaGuiasConsolidadas != null)
            {
                foreach (var element in requestPlanilla.ListaConsolidados)
                {
                    CAMovimientoConsolidadoDC movimientoConsolidado = new CAMovimientoConsolidadoDC();
                    movimientoConsolidado.NumeroConsolidado = element;
                    ltsMovimientosConsolidados.Add(movimientoConsolidado);
                }
                planillaVenta.MovimientoConsolidado = ltsMovimientosConsolidados;

                foreach (var element in requestPlanilla.ListaGuiasConsolidadas)
                {
                    var consolidadoGuias = new OUPlanillaVentaGuiasDC();
                    consolidadoGuias.NumeroGuia = element.NumeroGuia;
                    consolidadoGuias.NumeroTulaContenedor = element.NumeroConsolidado;
                    consolidadoGuias.Suelta = false;
                    consolidadoListas.Add(consolidadoGuias);
                }
            }

            if (requestPlanilla.ListaGuiasSueltas !=null)
            {
                foreach (var element in requestPlanilla.ListaGuiasSueltas)
                {
                    var consolidadoGuias = new OUPlanillaVentaGuiasDC();
                    consolidadoGuias.NumeroGuia = element;
                    consolidadoGuias.Suelta = true;
                    consolidadoListas.Add(consolidadoGuias);
                }
            }

            planillaVenta.ConsolidadoListas = consolidadoListas;

            //Consumo servicio de controller que inserta la planilla
            var idPlanilla = CrearPlanillaVentasIntegra(planillaVenta, usuarioQueCrea);

            //Obtengo la fecha de la planilla
            var fechaPlanilla = datos.ObtenerFechaPlanilla(idPlanilla);

            ResponsePlanilla responsePlanilla = new ResponsePlanilla();
            responsePlanilla.NumeroPlanilla = idPlanilla;
            responsePlanilla.FechaHoraCreacion = fechaPlanilla;
            responsePlanilla.PuntoDeVenta = centroServicioPlanilla.IdCentroServicio;
            responsePlanilla.NombrePuntoVenta = centroServicioPlanilla.NombreCentroServicio;
            responsePlanilla.CantidadEnviosSueltos = totalEnviosSueltos;
            responsePlanilla.CantidadEnviosConsolidados = totalEnviosConsolidados;
            responsePlanilla.CantidadEnviosSinPlanillar = datos.ObtenerCantidadEnviosSinPlanillarXIdCentroSrvicio(centroServicioPlanilla.IdCentroServicio);
            responsePlanilla.UsuarioQueGenera = usuarioQueCrea;
            responsePlanilla.IdentificacionMenajero = mensajeroPlanilla.IdentificacionMensajero;
            return responsePlanilla;
        }


        /// <summary>
        /// Obtiene los datos de un mensajero creado en controller 
        /// </summary>
        /// <param name="numeroDocumento"></param>
        /// <returns></returns>
        public MensajeroPlanilla ObtenerDatosMensajeroXDocumento(string numeroDocumento)
        {
            var mensajero = datos.ObtenerDatosMensajeroXDocumento(numeroDocumento);
            if (mensajero.IdMensajero == 0)
            {
                throw new Exception("No se encontro mensajero con la identificación ingresada ");
            }
            return mensajero;
        }

        /// <summary>
        /// Obtiene información de un centro de servicio creado en controller
        /// </summary>
        /// <param name="IdCentroServicio"></param>
        /// <returns></returns>
        public CentroServicioPlanilla ObtenerInfoCentroServicioXId(long IdCentroServicio)
        {
            var centroServicio = datos.ObtenerInfoCentroServicioXId(IdCentroServicio);
            if (centroServicio.IdCentroServicio == 0)
            {
                throw new Exception("No se encontro información del centro de servicio con el id ingresado");
            }
            return centroServicio;
        }

        /// <summary>
        /// Metodo que inserta una planilla en servicios controller
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public long CrearPlanillaVentasIntegra(OUPlanillaVentaDC planilla, string usuario)
        {
            long idPlanilla=0;
            string NombreOrigen = ConfigurationManager.AppSettings["NombreOrigen"];
            var urlController = conexionApi("urlServiciosController","ServiciosController");
            var restClient = new RestClient(urlController);
            var uri = "api/OperacionUrbanaController/CrearPlanillaCentroServicioIntegracion";
            var request = new RestRequest(uri, Method.POST);
            request.AddHeader("usuario", usuario);
            request.AddHeader("NombreOrigen",  NombreOrigen);
            request.AddJsonBody(planilla);
            IRestResponse response = restClient.Execute(request);
            if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                idPlanilla = Convert.ToInt32(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }
            return idPlanilla;
        }

        #endregion
    }
}
