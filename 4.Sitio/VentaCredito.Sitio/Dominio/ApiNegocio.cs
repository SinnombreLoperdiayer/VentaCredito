using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using RestSharp;
using Servicio.Entidades.Admisiones.Mensajeria;
using Servicio.Entidades.Clientes;
using VentaCredito.Sitio.Comun;
//using VentaCredito.CentroServicios;
using VentaCredito.Sitio.Dominio.Comun;
using VentaCredito.Transversal.Entidades;

namespace VentaCredito.Sitio.Dominio
{
    public class ApiNegocio : ApiDominioBase
    {
        private static readonly ApiNegocio instancia = (ApiNegocio)FabricaInterceptorApi.GetProxy(new ApiNegocio(), Properties.Resources.VentaCredito);

        public static ApiNegocio Instancia
        {
            get
            {
                return instancia;
            }
        }


        internal ADResultadoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            return FabricaServicios.ServicioVentaCredito.RegistrarGuiaAutomatica(guia,idCaja,remitenteDestinatario);
        }

        internal AdmisionEnvioResponse RegistrarEnvioAutomatico(AdmisionEnvioRequest admisionEnvio, bool imprimeGuia = false)
        {
            VentaCredito.Negocio.Parametros.Instancia.ValidarRestriccionTipoEntregaServicioAgil(admisionEnvio.IdServicio, admisionEnvio.IdTipoEntrega.ToString());

            admisionEnvio.IdCliente = admisionEnvio.IdCliente; //Convert.ToInt32(Resources.IdClienteDirectTv);
            var guia = new ADGuia();

            guia = CrearGuia(admisionEnvio);

            var idCaja = 0;
            var remitenteDestinatario = new ADMensajeriaTipoCliente { 
                ConvenioDestinatario = new ADConvenio
                {
                    Nit = admisionEnvio.Destinatario.Identificacion,
                    RazonSocial = admisionEnvio.Destinatario.RazonSocial,
                    IdSucursalRecogida =  admisionEnvio.IdSucursal,
                    
                },
                PeatonDestinatario = new ADPeaton
                {
                    TipoIdentificacion = "CC" ,//admisionEnvio.Destinatario.TipoIdentificacion == 1 ? 'CC',
                    Identificacion =   admisionEnvio.Destinatario.Identificacion,
                    Nombre =   string.Format(" {0} {1}",admisionEnvio.Destinatario.PrimerNombre, admisionEnvio.Destinatario.SegundoNombre),
                    Apellido1   = admisionEnvio.Destinatario.PrimerApellido,
                    Apellido2 = admisionEnvio.Destinatario.SegundoApellido,
                    Telefono   = admisionEnvio.Destinatario.Telefono,
                    Direccion = admisionEnvio.Destinatario.Direccion,
                    Email = admisionEnvio.Destinatario.Email
                },

                //PeatonRemitente = new ADPeaton
                //{
                //    TipoIdentificacion = "NI",
                //    Nombre = guia.Remitente.Nombre,
                //    Apellido1 = guia.Remitente.Apellido1,
                //    Apellido2 = guia.Remitente.Apellido2,
                //    Direccion = guia.Remitente.Direccion,
                //    Email = guia.Remitente.Email == null ? string.Empty  : guia.Remitente.Email,
                //    Identificacion = guia.Remitente.Identificacion,
                //    Telefono = guia.Remitente.Telefono
                //},
                ConvenioRemitente = new ADConvenio
                {
                    Nit = guia.Remitente.Identificacion,
                    EMail = guia.Remitente.Email == null ? string.Empty : guia.Remitente.Email,
                    IdSucursalRecogida = guia.IdSucursal,
                    Contrato = guia.IdContrato,
                    IdListaPrecios = guia.IdListaPrecios,
                    RazonSocial = guia.Remitente.NombreYApellidos,
                    Direccion = guia.Remitente.Direccion,
                    Telefono = guia.Remitente.Telefono,
                    Id = guia.IdCliente
                },
            };

            var admisionRetorno = FabricaServicios.ServicioVentaCredito.RegistrarGuiaAutomatica(guia, idCaja, remitenteDestinatario);

            var response = new AdmisionEnvioResponse();

            response.EstadoGuia = (int)ADEnumEstadoGuia.Admitida;
            response.FechaAdmision = DateTime.Now;
            response.NumeroGuia = admisionRetorno.NumeroGuia;
            response.pdfBytes = imprimeGuia ? ObtenerPdfGuia(guia.NumeroGuia) : null;

            return response;
        }
       
        /// <summary>
        /// Crea el objeto guia requerido para la admision
        /// </summary>
        /// <param name="admisionEnvio">objeto enviado por el cliente credito</param>
        /// <returns></returns>
        private ADGuia CrearGuia(AdmisionEnvioRequest admisionEnvio)
        {
            var resultado = new ADGuia();            

            resultado.NumeroPieza = 1;
            resultado.IdServicio = admisionEnvio.IdServicio;
            resultado.IdTipoEntrega = admisionEnvio.IdTipoEntrega.ToString();           
            resultado.IdCentroServicioOrigen = 1295;
            resultado.IdPaisOrigen = admisionEnvio.IdPaisOrigen;
            resultado.IdCiudadOrigen = admisionEnvio.IdCiudadOrigen;
            resultado.NombreCiudadDestino = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerInformacionLocalidad(admisionEnvio.IdCiudadDestino).NombreCompleto;            
            resultado.CodigoPostalOrigen = admisionEnvio.CodigoPostalOrigen;
            resultado.IdPaisDestino = admisionEnvio.IdPaisDestino;
            resultado.IdCiudadDestino = admisionEnvio.IdCiudadDestino;
            resultado.NombreCiudadOrigen = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerInformacionLocalidad(admisionEnvio.IdCiudadOrigen).NombreCompleto;
            resultado.CodigoPostalDestino = admisionEnvio.CodigoPostalDestino;
            resultado.ValorDeclarado = admisionEnvio.ValorDeclarado;
            resultado.DiceContener = admisionEnvio.DiceContener;
            resultado.Observaciones = admisionEnvio.Observaciones;
            resultado.IdUnidadMedida = admisionEnvio.IdUnidadMedidaPeso == 1 ? "KG" : "GR";
            resultado.Peso = admisionEnvio.Peso;
            resultado.Largo = admisionEnvio.Largo;
            resultado.Ancho = admisionEnvio.Ancho;
            resultado.Alto = admisionEnvio.Alto;
            resultado.PesoLiqVolumetrico = (admisionEnvio.Largo * admisionEnvio.Ancho * admisionEnvio.Alto) / 6000;
            resultado.PesoLiqMasa = admisionEnvio.Peso;
            resultado.IdTipoEnvio = admisionEnvio.IdTipoEnvio;
            resultado.Destinatario = new CLClienteContadoDC
            {
                Nombre = string.Format("{0} {1}", admisionEnvio.Destinatario.PrimerNombre, admisionEnvio.Destinatario.SegundoNombre),
                Apellido1 = admisionEnvio.Destinatario.PrimerApellido,
                Apellido2 = admisionEnvio.Destinatario.SegundoApellido,
                TipoIdentificacion = admisionEnvio.Destinatario.TipoIdentificacion,
                Identificacion = admisionEnvio.Destinatario.Identificacion,
                Email = admisionEnvio.Destinatario.Email,
                Direccion = admisionEnvio.Destinatario.Direccion,
                Telefono = admisionEnvio.Destinatario.Telefono,                                
            };
            resultado.GuidDeChequeo = Guid.NewGuid().ToString();
            resultado.DigitoVerificacion = string.Empty;
            resultado.Remitente = ConsultarDatosCliente(admisionEnvio.IdCliente);
            resultado.NotificarEntregaPorEmail = admisionEnvio.NotificarEntregaPorEmail;
            resultado.NoPedido = admisionEnvio.NoPedido;
            resultado.IdSucursal = admisionEnvio.IdSucursal;
            resultado.FormasPago = new List<ADGuiaFormaPago>();
            resultado.FormasPago.Add(new ADGuiaFormaPago { IdFormaPago = 2, Descripcion = "Crédito" });
            resultado.IdUnidadNegocio = CO.Servidor.Dominio.Comun.Tarifas.TAConstantesServicios.UNIDAD_MENSAJERIA.ToString();
            resultado.TelefonoDestinatario = admisionEnvio.Destinatario.Telefono;
            resultado.DireccionDestinatario = admisionEnvio.Destinatario.Direccion;            
            
            resultado.ValoresAdicionales = new List<Servicio.Entidades.Tarifas.TAValorAdicional>();
            resultado.TipoCliente = ADEnumTipoCliente.CPE;
            resultado.IdCliente = admisionEnvio.IdCliente;
            
            return resultado;
        }

        private CLClienteContadoDC ConsultarDatosCliente(int idCliente)
        {
            var remitente = new CLClienteContadoDC();

            var datosCliente = FabricaServicios.ServicioClientes.ObtenerDatosCliente(idCliente);

            remitente.Nombre = datosCliente.RazonSocial;
            remitente.Apellido1 = "-";
            remitente.Apellido2 = datosCliente.RazonSocial;
            remitente.Direccion = datosCliente.Direccion;
            remitente.Telefono = datosCliente.Telefono;
            remitente.TipoIdentificacion = 2;
            remitente.Identificacion = datosCliente.Nit;             
            return remitente;
        }
        public byte[] ObtenerPdfGuia(long numeroGuia)
        {
            //Consumo de servicio para obtener pdf de la guia de transporte
            String URIAdmisionServer = ConfigurationManager.AppSettings["urlApiImpresionesMS"];

            if (String.IsNullOrEmpty(URIAdmisionServer))
            {
                throw new Exception("Url servidor admision no encontrado en configuración");
            }

            string hostAdmision = URIAdmisionServer;

            var clientGuia = new RestClient(hostAdmision);
            string uri = "api/Mensajeria/ObtenerBase64Impresion/" + numeroGuia.ToString();
            var requestPdf = new RestRequest(uri, Method.GET);
            requestPdf.AddHeader("Content-Type", "application/json");
            IRestResponse responseMessage = clientGuia.Execute(requestPdf);

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
    }
}


