using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using VentaCredito.AdmisionPreenvio.Datos;
using VentaCredito.Clientes.Datos.Repositorio;
using VentaCredito.Negocio;
using VentaCredito.Seguridad;
using VentaCredito.Tarifas.Datos.ContratosDatos;
using VentaCredito.Transversal.Entidades;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using VentaCredito.Transversal.Entidades.Parametros;

namespace VentaCredito.AdmisionPreenvio
{
    public class AdmisionPreenvioNegocio
    {
        private static AdmisionPreenvioNegocio instancia = new AdmisionPreenvioNegocio();

        private readonly AdmisionPreenvioDatos instanciaAdmisionDatos = AdmisionPreenvioDatos.Instancia;


        public static AdmisionPreenvioNegocio Instancia
        {
            get
            {
                return instancia;
            }
        }

        /// <summary>
        /// Id Servicio Notificaciones
        /// </summary>
        public const int SERVICIO_NOTIFICACIONES = 15;

        /// <summary>
        /// Id formato guía tamaño normal
        /// </summary>
        public const int ID_FORMATO_GUIA_NOR = 1;

        /// <summary>
        /// Id formato guía tamaño pequeño
        /// </summary>
        public const long ID_FORMATO_GUIA_PEQUE = 2;

        /// <summary>
        /// Id Servicio Rapi Radicado
        /// </summary>
        public const long SERVICIO_RAPIRADICADO = 16;

        /// <summary>
        /// Consumo servicios externos
        /// </summary>
        public String conexionApi(string conexion, string nombreApi)
        {
            String urlServicios = ConfigurationManager.AppSettings[conexion];

            if (String.IsNullOrEmpty(urlServicios))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent($"Url servidor {nombreApi} no encontrado en configuración")
                });
            }
            else
            {
                return urlServicios;
            }
        }

        /// <summary>
        /// Inserta admision preenvio.
        /// </summary>
        /// <param name="admision"></param>
        /// <returns>Objeto que contiene  número de pre envío (pre guía) generado, con la fecha y hora de creación y de vencimiento (el vencimiento solo será informativo).</returns>
        public ResponsePreAdmisionWrapper InsertarAdmision(RequestPreAdmisionWrapperCV admision)
        {
            bool VerificacionContenidoCliente = false;

            ClienteCreditoVC cliente = ObtenerClienteCreditoActivo(admision.IdClienteCredito, admision.CodigoConvenioRemitente);
            if (cliente.IdCliente == 0)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Cliente no válido.")
                });
            }

            if (!Regex.IsMatch(admision.Destinatario.NumeroDocumento ?? "", @"^\d+$"))
            {
                string error = string.IsNullOrEmpty(admision.Destinatario.NumeroDocumento) ? "El campo destinatario.numeroDocumento es obligatorio."
                    : "El campo destinatario.numeroDocumento debe contener únicamente caracteres numéricos.";
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(error)
                });
            }

            if (admision.Destinatario != null && (string.IsNullOrEmpty(admision.Destinatario.IdLocalidad) || string.IsNullOrEmpty(admision.Destinatario.Direccion)))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Faltan datos importantes. La dirección y la localidad de destinatario son obligatorios. Por favor, validar.")
                });
            }

            //Valida contrapago y si el destino admite realizar preenvios con contrapago y pago en casa
            if (admision.AplicaContrapago && LocalidadesNegocio.LocalidadesNegocio.Instancia.ConsultarValidezDestinoGeneracionGuias(admision.Destinatario.IdLocalidad))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("El destino no es válido para envíos con contra pago y pago en casa.")
                });
            }

            if(!Negocio.Parametros.Instancia.ObtenerTiposEntrega().Any(e => e.Id.Equals(admision.IdTipoEntrega)))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("El parámetro de Tipo Entrega no es válido.")
                });
            }

            if (!LocalidadesNegocio.LocalidadesNegocio.Instancia.ConsultarSubtipoCentroServiciosPorLocalidad(admision.Destinatario.IdLocalidad, Convert.ToInt32(admision.IdTipoEntrega)))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent("No se encontró el SubTipo de ningún Centro de Servicios asociado a la localidad.")
                });
            }

            admision.Remitente = RemitenteEncontradoXIdCliente(cliente);

            DestinatarioFrecuente_CLI destinatario = AdmisionPreenvioDatos.Instancia.ConsultarClienteFrecuentePorIdentificacion_CLI(admision.Destinatario.NumeroDocumento, admision.Destinatario.TipoDocumento);
            admision.Destinatario.IdDestinatario = destinatario.CLC_IdClienteContado;
            admision.IdCiudadOrigen = cliente.IdLocalidad;
            admision.IdCiudadDestino = admision.Destinatario.IdLocalidad;
            admision.IdUnidadMedida = "kg";
            admision.EsPesoVolumetrico = false;
            admision.IdContrato = cliente.IdContrato;
            admision.IdListaPrecios = cliente.IdListaPrecios;

            admision.Largo = Math.Max(admision.Largo, 0);
            admision.Ancho = Math.Max(admision.Ancho, 0);
            admision.Alto = Math.Max(admision.Alto, 0);

            //liquidacion
            admision.PesoLiqMasa = 0;
            admision.PesoLiqVolumetrico = 0;
            admision.IdMotivoNoUsoBolsaSegurida = 0;
            admision.ValorAdicionales = 0;
            admision.ValorEmpaque = 0;

            admision.EsAlCobro = admision.IdFormaPago.Equals(Servicio.Entidades.Tarifas.Precios.TAEnumFormaPago.AL_COBRO);
            admision.NumeroPieza = 1;
            admision.IdUnidadNegocio = admision.Peso > 5 ? "CAR" : "MEN";


            admision.CodigoConvenio = admision.IdSucursal;
            admision.EmailDestinatario = admision.Destinatario.Correo;
            admision.EmailRemitente = admision.Remitente.Correo;


            //Validación campo primer apellido para cliente credito, en el servicio de la admision es obligatorio, por eso se deja un espacio
            admision.Destinatario.PrimerApellido = admision.IdSucursal != 0 ? " " : admision.Destinatario.PrimerApellido;
            admision.Destinatario.SegundoApellido = String.IsNullOrEmpty(admision.Destinatario.SegundoApellido) ? " " : admision.Destinatario.SegundoApellido;
            ValidarDatosObligatorios(admision);


            PALocalidadDC localidadDestino = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerInformacionLocalidad(admision.IdCiudadDestino);
            admision.NombreCiudadDestino = localidadDestino.NombreCompleto;
            admision.NombreCiudadOrigen = cliente.NombreLocalidad;

            DataGeo geo = ObtenerGeolocalizacion(admision, localidadDestino, destinatario);
            ValidarZonaDificilAcceso(admision, geo);

            admision.Latitude = geo.latitude;
            admision.Longitude = geo.longitude;
            admision.Zona1 = geo.zona1;
            admision.Zona2 = geo.zona2;
            admision.Zona3 = geo.zona3;
            admision.Barrio = geo.barrio;
            admision.ZonaPostal = geo.zonapostal;
            admision.EstadoGeoDesti = geo.estado;
            admision.DirDestiNormalizada = geo.dirtrad;
            admision.LocalidadDestiGeo = geo.localidad;
            admision.CodigoPostalDestino = geo.zonapostal;

            //Validación Peso Volumetrico
            decimal resultadoPesoVol = ((admision.Largo) * (admision.Ancho) * (admision.Alto)) / 6000;
            decimal pesoVolumetricoRedondeado = Math.Ceiling(resultadoPesoVol);
            admision.Peso = Math.Max(pesoVolumetricoRedondeado, Math.Ceiling(admision.Peso));

            TAValorPesoDeclaradoDC valoresDeclaradosCliente = Parametros.Negocio.ParametrosNegocio.Instancia.ObtenerValorPesoDeclaradoClienteCredito(admision.Peso, cliente.IdListaPrecios);
            decimal valorMinimo = valoresDeclaradosCliente.ValorMinimoDeclarado;
            decimal valorMaximo = valoresDeclaradosCliente.ValorMaximoDeclarado;

            if (admision.ValorDeclarado < valorMinimo || admision.ValorDeclarado > valorMaximo)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent($"El valor declarado no se encuentra dentro del rango permitido ({Convert.ToInt32(valorMinimo)} - {Convert.ToInt32(valorMaximo)}). Favor rectificar los datos ingresados.")
                });
            }

            //Consulta centro de servicio por idLocalidadOrigen
            PUCentroServiciosDC centroServicio = ObtenerColResponsablePorCiudad(admision.IdCiudadOrigen);
            if (centroServicio == null || admision.Peso > centroServicio.PesoMaximo)
            {
                if (centroServicio == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent("No es posible admitir el preenvío, no existe un centro servicio en la ciudad de origen.")
                    });
                }

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Excede el peso máximo para la ciudad. Favor rectificar los datos ingresados.")
                });
            }
            admision.IdCentroServicioOrigen = centroServicio.IdCentroServicio;
            admision.NombreCentroServicioOrigen = centroServicio.Nombre;

            TAPreciosAgrupadosDC servicio = ObtenerCotizacion(admision, cliente, centroServicio);
            if (admision.AplicaContrapago)
            {
                VerificacionContenidoCliente = SeguridadServicio.Instancia.ConsultarVeriContenidoClienteCredito(cliente.IdCliente);
                MapearValoresContrapago(admision, servicio);
            }

            MapearPorTipoDeServicio(admision, servicio);

            if (admision.ValorTotal > cliente.ValorPresupuesto)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Sin presupuesto asignado. favor validar con la Transportadora.")
                });
            }


            // Invocar al método externo para registrar el preenvío
            return RegistrarAdmisionPreenvio(admision, cliente, VerificacionContenidoCliente);
        }

        private static void MapearPorTipoDeServicio(RequestPreAdmisionWrapperCV admision, TAPreciosAgrupadosDC servicio)
        {
            if (servicio == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("El servicio seleccionado no se encuentra asociado. Favor validar con la Transportadora.")
                });
            }
            if (!admision.AplicaContrapago)
            {
                admision.ValorPrimaSeguro = servicio.Precio.ValorPrimaSeguro;
                admision.ValorAdmision = servicio.Precio.Valor;
                admision.ValorTotal = (servicio.Precio.Valor + servicio.Precio.ValorPrimaSeguro);
                admision.FechaEstimadaEntrega = servicio.fechaEntrega;
                admision.DiasDeEntrega = Int16.Parse(servicio.TiempoEntrega);
                admision.NombreServicio = servicio.NombreServicio;
                admision.ValorTotalImpuestos = 0;
                admision.ValorTotalRetenciones = 0;
            }
            if (servicio.IdServicio == SERVICIO_NOTIFICACIONES)
            {
                NotificacionPE notificacion = new NotificacionPE();
                notificacion.DireccionDestinatario = "";
                notificacion.EntregarDireccionRemitente = true;
                notificacion.IdCiudadDestino = admision.Destinatario.IdLocalidad;
                admision.Notificacion = notificacion;
            }

            if (servicio.IdServicio == SERVICIO_RAPIRADICADO)
            {
                RapiradicadoPE rapi = new RapiradicadoPE();
                rapi.DireccionDestinatario = "";
                rapi.EntregarDireccionRemitente = true;
                rapi.IdCiudadDestino = admision.Destinatario.IdLocalidad;
                rapi.ConsecutivoRadicado = 0;
                rapi.NumerodeFolios = admision.Rapiradicado.NumerodeFolios;
                rapi.CodigoRapiRadicado = admision.Rapiradicado.CodigoRapiRadicado;
                admision.Rapiradicado = rapi;
            }
        }

        private DataGeo ObtenerGeolocalizacion(RequestPreAdmisionWrapperCV admision, PALocalidadDC localidadDestino, DestinatarioFrecuente_CLI destinatario)
        {
            if (!string.IsNullOrEmpty(destinatario.CLC_Localidad) &&
                !string.IsNullOrEmpty(destinatario.CLC_Latitude) &&
                !string.IsNullOrEmpty(destinatario.CLC_Longitude) &&
                ((destinatario.CLC_IdLocalidad == admision.Destinatario.IdLocalidad)
                && (destinatario.CLC_Direccion == admision.Destinatario.Direccion)))
            {
                return new DataGeo
                {
                    latitude = destinatario.CLC_Latitude,
                    longitude = destinatario.CLC_Longitude,
                    zona1 = destinatario.CLC_Zona1,
                    zona2 = destinatario.CLC_MacroZona,
                    zona3 = destinatario.CLC_MicroZona,
                    barrio = destinatario.CLC_Barrio,
                    zonapostal = destinatario.CLC_ZonaPostal,
                    estado = destinatario.CLC_EstadoGeo,
                    dirtrad = destinatario.CLC_DirNormalizada,
                    localidad = destinatario.CLC_Localidad
                };
            }

            // Realizar consulta si los datos no están disponibles
            RequestGeomultizonaVC requestGeo = new RequestGeomultizonaVC
            {
                address = admision.Destinatario.Direccion,
                city = localidadDestino.Nombre
            };

            Geolocalizacion geolocalizacion = GeolocalizacionNegocio.GeolocalizacionNegocio.Instancia.GeolocalizacionDireccion(requestGeo, "Preenvio");
            return geolocalizacion?.data;
        }

        private void ValidarZonaDificilAcceso(RequestPreAdmisionWrapperCV admision, DataGeo geo)
        {
            PAZonaDificilAcceso zona = new PAZonaDificilAcceso
            {
                IdLocalidad = Convert.ToInt64(admision.Destinatario.IdLocalidad),
                ZonaDescripcion = geo?.zona2
            };

            // Tipo entrega 2 = "Reclamo en oficina"
            bool esZonaDificilAcceso = Convert.ToInt32(admision.IdTipoEntrega) != 2
                ? ConsultarSiZonaEsDificilAccesoDesdeCache(zona)
                : false;

            if (esZonaDificilAcceso)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent("Algunos envíos tienen como destino zonas de difícil acceso, seguridad y movilidad; por favor remítalos con tipo de entrega reclamo en oficina.")
                });
            }
        }

        private bool ConsultarSiZonaEsDificilAccesoDesdeCache(PAZonaDificilAcceso zona)
        {
            // Obtén las zonas de difícil acceso desde la caché
            List<PAZonaDificilAcceso> zonasDificilesAcceso = Parametros.Negocio.ParametrosNegocio.Instancia.ObtenerZonasDesdeCache();

            // Verifica si la zona está marcada como de difícil acceso
            return zonasDificilesAcceso.Any(z =>
                z.IdLocalidad == zona.IdLocalidad &&
                z.ZonaDescripcion.Equals(zona.ZonaDescripcion, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///  Se arma objeto para realizar la consulta al servicio de ResultadoLitaCotizarClienteCredito
        /// </summary>
        /// <param name="admision"></param>
        /// <param name="cliente"></param>
        /// <param name="centroServicio"></param>
        /// <returns>Retorna los precios parametrizados por servicio del cliente credito segun contrato y lista de precios</returns>
        private TAPreciosAgrupadosDC ObtenerCotizacion(RequestPreAdmisionWrapperCV admision, ClienteCreditoVC cliente, PUCentroServiciosDC centroServicio)
        {
            string asStringFecha = DateTime.Now.ToString("dd-MM-yyyy");

            PrecioServicioDto precioServicioDto = new PrecioServicioDto()
            {
                IdCliente = admision.IdClienteCredito,
                IdLocalidadOrigen = admision.IdCiudadOrigen,
                IdLocalidadDestino = admision.IdCiudadDestino,
                Peso = admision.Peso,
                ValorDeclarado = admision.ValorDeclarado,
                IdTipoEntrega = admision.IdTipoEntrega,
                Fecha = asStringFecha,
                IdServicio = admision.IdServicio,
                IdContrato = cliente.IdContrato,
                IdListaPrecios = cliente.IdListaPrecios,
                IdCentroServicioOrigen = centroServicio.IdCentroServicio,
                EsMarketPlace = admision.EsMarketplace.HasValue ? admision.EsMarketplace.Value : false
            };
            return ResultadoListaCotizarClienteCredito(precioServicioDto);
        }

        public ResponsePreAdmisionWrapper RegistrarAdmisionPreenvio(RequestPreAdmisionWrapperCV admision, ClienteCreditoVC cliente, bool VerificacionContenidoCliente)
        {
            ResponsePreAdmisionWrapper respuesta = Negocio.AdmisionPreenvio.Instancia.RegistrarPreenvio(admision);
            if (admision.AplicaContrapago)
            {
                VerificacionContenidoPreenvio(respuesta.NumeroPreenvio, VerificacionContenidoCliente);
            }
            if (admision.EsMarketplace.HasValue)
            {

                ADAdmisionMensajeria.Instancia.RegistrarVentaParaFacturaManual(admision.IdClienteCredito, respuesta.NumeroPreenvio, admision.IdContrato, admision.CodigoConvenioRemitente, admision.IdCiudadDestino, admision.ValorTotal, admision.IdFormaPago);
            }
            return respuesta;
        }

        /// <summary>
        /// Validación de datos obligatorios del Insertar Preenvio
        /// </summary>
        /// <param name="admision"></param>
        public void ValidarDatosObligatorios(RequestPreAdmisionWrapperCV admision)
        {

            Dictionary<string, Func<bool>> validaciones = new Dictionary<string, Func<bool>>()
            {

                { "NumeroDocumentoDestinatario", () => !EsInvalidoString(admision.Destinatario.NumeroDocumento) },
                { "TelefonoDestinatario", () => !EsInvalidoString(admision.Destinatario.Telefono) },
                { "NombreDestinatario", () => !EsInvalidoString(admision.Destinatario.Nombre) },
                { "PrimerApellidoDestinatario", () => !EsInvalidoString(admision.Destinatario.PrimerApellido) },

                { "NumeroDocumentoRemitente", () => !EsInvalidoString(admision.Remitente.NumeroDocumento) },
                { "TelefonoRemitente", () => !EsInvalidoString(admision.Remitente.Telefono) },
                { "NombreRemitente", () => !EsInvalidoString(admision.Remitente.Nombre) },
                { "PrimerApellidoRemitente", () => !EsInvalidoString(admision.Remitente.PrimerApellido) },
                { "DireccionRemitente", () => !EsInvalidoString(admision.Remitente.Direccion) },

                { "IdTipoEntrega", () => !string.IsNullOrEmpty(admision.IdTipoEntrega) },
                { "ValorDeclarado", () => admision.ValorDeclarado > 0 },
                { "IdFormaPago", () => admision.IdFormaPago > 0 },
                { "IdTipoEnvio", () => admision.IdTipoEnvio > 0 },
                { "IdServicio", () => admision.IdServicio > 0 }
            };

            foreach (var validacion in validaciones)
            {
                if (!validacion.Value())
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent($"El campo {validacion.Key} es obligatorio o tiene un valor incorrecto.")
                    });
                }
            }
        }

        /// <summary>
        /// Valida si el valor es nulo
        /// </summary>
        /// <param name="valor"></param>
        /// <returns>falso o verdadero en caso de que el valor sea nulo</returns>
        private bool EsInvalidoString(string valor)
        {
            return string.IsNullOrEmpty(valor);
        }

        /// <summary>
        /// Inserta un preenvio realizado desde el portal de clientes Express
        /// </summary>
        /// <param name="admisionPortalCli"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ResponsePreAdmisionWrapper InsertarAdmisionPortalCli(RequestPreAdmisionPortalCli admisionPortalCli)
        {
            if (admisionPortalCli.IdRecogida == 0 || admisionPortalCli.FechaRecogida.Equals(DateTime.MinValue))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("La FechaRecogida y el IdRecogida son necesarios.")
                });
            }

            ClienteCreditoVC cliente = ObtenerClienteCreditoActivo(admisionPortalCli.IdClienteCredito, admisionPortalCli.CodigoConvenioRemitente);

            if (cliente.IdCliente != 0)
            {
                admisionPortalCli.Remitente = RemitenteEncontradoXIdCliente(cliente);
                DestinatarioFrecuente_CLI destinatario = AdmisionPreenvioDatos.Instancia.ConsultarClienteFrecuentePorIdentificacion_CLI(admisionPortalCli.Destinatario.NumeroDocumento, admisionPortalCli.Destinatario.TipoDocumento);
                admisionPortalCli.Destinatario.IdDestinatario = destinatario != null && destinatario.CLC_IdClienteContado != 0 ? destinatario.CLC_IdClienteContado : 0;

                admisionPortalCli.IdCiudadOrigen = cliente.IdLocalidad;
                admisionPortalCli.IdCiudadDestino = admisionPortalCli.Destinatario.IdLocalidad;
                admisionPortalCli.IdUnidadMedida = "kg";
                admisionPortalCli.EsPesoVolumetrico = false;
                admisionPortalCli.IdContrato = cliente.IdContrato;
                admisionPortalCli.IdListaPrecios = cliente.IdListaPrecios;


                admisionPortalCli.Largo = admisionPortalCli.Largo != 0 ? admisionPortalCli.Largo : 0;
                admisionPortalCli.Ancho = admisionPortalCli.Ancho != 0 ? admisionPortalCli.Ancho : 0;
                admisionPortalCli.Alto = admisionPortalCli.Alto != 0 ? admisionPortalCli.Alto : 0;

                //liquidacion
                admisionPortalCli.PesoLiqMasa = 0;
                admisionPortalCli.PesoLiqVolumetrico = 0;
                admisionPortalCli.IdMotivoNoUsoBolsaSegurida = 0;
                admisionPortalCli.ValorAdicionales = 0;
                admisionPortalCli.ValorEmpaque = 0;

                admisionPortalCli.EsAlCobro = admisionPortalCli.IdFormaPago == 3 ? true : false;
                admisionPortalCli.NumeroPieza = 1;
                admisionPortalCli.IdUnidadNegocio = admisionPortalCli.Peso > 5 ? "CAR" : "MEN";

                admisionPortalCli.IdCliente = admisionPortalCli.IdCliente != 0 ? admisionPortalCli.IdCliente : 0;
                admisionPortalCli.CodigoConvenio = admisionPortalCli.IdSucursal != 0 ? admisionPortalCli.IdSucursal : 0;
                //Validación campo primer apellido para cliente credito, en el servicio de la admisionPortalCli es obligatorio, por eso se deja un espacio
                admisionPortalCli.Destinatario.PrimerApellido = admisionPortalCli.IdSucursal != 0 ? " " : admisionPortalCli.Destinatario.PrimerApellido;
                admisionPortalCli.Destinatario.SegundoApellido = String.IsNullOrEmpty(admisionPortalCli.Destinatario.SegundoApellido) ? " " : admisionPortalCli.Destinatario.SegundoApellido;
                PALocalidadDC localidadDestino = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerInformacionLocalidad(admisionPortalCli.IdCiudadDestino);
                admisionPortalCli.NombreCiudadDestino = localidadDestino.NombreCompleto;
                admisionPortalCli.NombreCiudadOrigen = cliente.NombreLocalidad;
                admisionPortalCli.EmailDestinatario = admisionPortalCli.Destinatario.Correo;
                admisionPortalCli.EmailRemitente = admisionPortalCli.Remitente.Correo;

                //Validación peso volumetrico
                decimal formulaPeso = ((admisionPortalCli.Largo) * (admisionPortalCli.Ancho) * (admisionPortalCli.Alto)) / 6000;
                decimal pesoVolumetrico = Math.Round(formulaPeso);
                decimal peso = pesoVolumetrico > admisionPortalCli.Peso ? pesoVolumetrico : admisionPortalCli.Peso;
                admisionPortalCli.Peso = peso;
                TAValorPesoDeclaradoDC valoresDeclaradosCliente = Parametros.Negocio.ParametrosNegocio.Instancia.ObtenerValorPesoDeclaradoClienteCredito(admisionPortalCli.Peso, cliente.IdListaPrecios);
                decimal valorMinimo = valoresDeclaradosCliente.ValorMinimoDeclarado;
                decimal valorMaximo = valoresDeclaradosCliente.ValorMaximoDeclarado;

                //Geolacalizacion direccion destinatario

                //Consulta si la localidad destino se georreferencia
                Localidad_PAR localidad = AdmisionPreenvioDatos.Instancia.ConsultarZonayGeoPorIdLacalidad_CLI(admisionPortalCli.Destinatario.IdLocalidad);

                if (localidad.LOC_SeGeorreferencia)
                {
                    DataGeo geo = ValidarDestinatarioGeorreferenciacion(admisionPortalCli, localidadDestino, destinatario);

                    admisionPortalCli.Latitude = geo.latitude;
                    admisionPortalCli.Longitude = geo.longitude;
                    admisionPortalCli.Zona1 = geo.zona1;
                    admisionPortalCli.Zona2 = geo.zona2;
                    admisionPortalCli.Zona3 = geo.zona3;
                    admisionPortalCli.Barrio = geo.barrio;
                    admisionPortalCli.ZonaPostal = geo.zonapostal;
                    admisionPortalCli.EstadoGeoDesti = geo.estado;
                    admisionPortalCli.DirDestiNormalizada = geo.dirtrad;
                    admisionPortalCli.LocalidadDestiGeo = geo.localidad;
                    admisionPortalCli.CodigoPostalDestino = geo.zonapostal;
                }
                else
                {
                    admisionPortalCli.ZonaPostal = localidad.LOC_CodigoPostal;
                }

                //Consulta centro de servicio por idLocalidadOrigen
                PUCentroServiciosDC centroServicio = ObtenerColResponsablePorCiudad(admisionPortalCli.IdCiudadOrigen);
                if (centroServicio != null)
                {
                    if (admisionPortalCli.Peso <= centroServicio.PesoMaximo) // Se valida que el peso sea menor al peso permitido por el centro de servicio asociado a la ciudad de origen
                    {

                        if (admisionPortalCli.ValorDeclarado >= valorMinimo && admisionPortalCli.ValorDeclarado <= valorMaximo)
                        {
                            admisionPortalCli.IdCentroServicioOrigen = centroServicio.IdCentroServicio;
                            admisionPortalCli.NombreCentroServicioOrigen = centroServicio.Nombre;

                            DateTime now = DateTime.Now;
                            string asStringFecha = DateTime.Now.ToString("dd-MM-yyyy");

                            TAPreciosAgrupadosDC servicio = ResultadoListaCotizarCliente(admisionPortalCli.IdClienteCredito, admisionPortalCli.IdCiudadOrigen, admisionPortalCli.IdCiudadDestino, admisionPortalCli.Peso, admisionPortalCli.ValorDeclarado, admisionPortalCli.IdTipoEntrega, asStringFecha, admisionPortalCli.IdServicio);

                            if (servicio != null)
                            {
                                if (admisionPortalCli.AplicaContrapago)
                                {
                                    MapearValoresContrapago(admisionPortalCli, servicio);
                                }
                                else
                                {
                                    admisionPortalCli.ValorPrimaSeguro = servicio.Precio.ValorPrimaSeguro;
                                    admisionPortalCli.ValorAdmision = servicio.Precio.Valor;
                                    admisionPortalCli.ValorTotal = (servicio.Precio.Valor + servicio.Precio.ValorPrimaSeguro);
                                    admisionPortalCli.FechaEstimadaEntrega = servicio.fechaEntrega;
                                    admisionPortalCli.DiasDeEntrega = Int16.Parse(servicio.TiempoEntrega);
                                    admisionPortalCli.NombreServicio = servicio.NombreServicio;
                                    admisionPortalCli.ValorTotalImpuestos = 0;
                                    admisionPortalCli.ValorTotalRetenciones = 0;
                                }
                                if (servicio.IdServicio == SERVICIO_NOTIFICACIONES)
                                {
                                    NotificacionPE notificacion = new NotificacionPE();
                                    notificacion.DireccionDestinatario = "";
                                    notificacion.EntregarDireccionRemitente = true;
                                    notificacion.IdCiudadDestino = admisionPortalCli.Destinatario.IdLocalidad;
                                    admisionPortalCli.Notificacion = notificacion;
                                }

                                if (servicio.IdServicio == SERVICIO_RAPIRADICADO)
                                {
                                    RapiradicadoPE rapi = new RapiradicadoPE();
                                    rapi.DireccionDestinatario = "";
                                    rapi.EntregarDireccionRemitente = true;
                                    rapi.IdCiudadDestino = admisionPortalCli.Destinatario.IdLocalidad;
                                    rapi.ConsecutivoRadicado = 0;
                                    rapi.NumerodeFolios = admisionPortalCli.Rapiradicado.NumerodeFolios;
                                    rapi.CodigoRapiRadicado = admisionPortalCli.Rapiradicado.CodigoRapiRadicado;
                                    admisionPortalCli.Rapiradicado = rapi;
                                }
                            }
                            else
                            {
                                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                                {
                                    Content = new StringContent("El servicio seleccionado no se encuentra asociado. Favor validar con la Transportadora.")
                                });
                            }

                            if (admisionPortalCli.ValorTotal < cliente.ValorPresupuesto)
                            {
                                //Consumo de servicio para insertar admision preenvio cliente credito.No 
                                String urlPreenvios = conexionApi("urlApiPreenvios", "admision");

                                RestClient clientGuia = new RestClient(urlPreenvios);
                                string uri = "api/Admision/InsertarPreenvioPortalCL";
                                RestRequest requestadmisionPortalCli = new RestRequest(uri, Method.POST);
                                requestadmisionPortalCli.AddHeader("Content-Type", "application/json");
                                requestadmisionPortalCli.AddJsonBody(admisionPortalCli);
                                IRestResponse responseMessage = clientGuia.Execute(requestadmisionPortalCli);

                                if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
                                {

                                    ResponsePreAdmisionWrapper respuesta = JsonConvert.DeserializeObject<ResponsePreAdmisionWrapper>(responseMessage.Content);
                                    return respuesta;
                                }
                                else
                                {
                                    throw new Exception(responseMessage.Content);
                                }
                            }
                            else
                            {
                                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                                {
                                    Content = new StringContent("Sin presupuesto asignado. favor validar con la Transportadora.")
                                });
                            }

                        }
                        else
                        {
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                            {
                                Content = new StringContent($"El valor declarado no se encuentra dentro del rango permitido ({Convert.ToInt32(valorMinimo) - Convert.ToInt32(valorMaximo)}). Favor rectificar los datos ingresados.")
                            });
                        }

                    }
                    else
                    {
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                        {
                            Content = new StringContent("Excede el peso máximo para la ciudad. Favor rectificar los datos ingresados.")
                        });
                    }

                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent("No es posible admitir el prenvío, no existe un centro servicio a la ciudad de origen.")
                    });
                }
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Cliente no válido.")
                });
            }
        }

        /// <summary>
        /// Hevelin Dayana Diaz Susa - 18/08/2022
        /// Método que georreferencia la dirección de un destinatario y valida si se georreferenció anteriormente
        /// </summary>
        /// <param name="admision"></param>
        /// <param name="localidadDestino"></param>
        /// <returns></returns>
        public DataGeo ValidarDestinatarioGeorreferenciacion(RequestPreAdmisionWrapperCV admision, PALocalidadDC localidadDestino, DestinatarioFrecuente_CLI destinatario)
        {
            DataGeo geo = new DataGeo();
            if ((destinatario.CLC_IdLocalidad != admision.Destinatario.IdLocalidad) || (destinatario.CLC_Direccion != admision.Destinatario.Direccion))
            {
                RequestGeomultizonaVC requestGeo = new RequestGeomultizonaVC();
                requestGeo.address = admision.Destinatario.Direccion;
                requestGeo.city = localidadDestino.Nombre;
                Geolocalizacion geolocalizacion = GeolocalizacionNegocio.GeolocalizacionNegocio.Instancia.GeolocalizacionDireccion(requestGeo, "Preenvio");
                if (geolocalizacion != null && !string.IsNullOrEmpty(geolocalizacion.data.localidad))
                {
                    geo = geolocalizacion.data;
                }
            }
            else
            {
                geo.latitude = destinatario.CLC_Latitude;
                geo.longitude = destinatario.CLC_Longitude;
                geo.zona1 = destinatario.CLC_Zona1;
                geo.zona2 = destinatario.CLC_MacroZona;
                geo.zona3 = destinatario.CLC_MicroZona;
                geo.barrio = destinatario.CLC_Barrio;
                geo.zonapostal = destinatario.CLC_ZonaPostal;
                geo.estado = destinatario.CLC_EstadoGeo;
                geo.dirtrad = destinatario.CLC_DirNormalizada;
                geo.localidad = destinatario.CLC_Localidad;
            }

            return geo;
        }
        public void MapearValoresContrapago(RequestPreAdmisionWrapperCV admision, TAPreciosAgrupadosDC servicio)
        {
            //Consumo servicio contizador
            var tarifasContrapago = ObtenerServicioCotizador(admision.ValorDeclarado, admision.IdCiudadOrigen, admision.IdCiudadDestino, admision.IdListaPrecios, admision.IdCentroServicioOrigen, admision.IdFormaPago, admision.Peso);
            TATarifasContraPago servicioContrapago = null;
            foreach (var elemet in tarifasContrapago)
            {
                if (elemet.ServiciosContraPago.IdServicio == admision.IdServicio)
                {
                    servicioContrapago = elemet;
                }
            }
            if (servicioContrapago != null && servicioContrapago.ServiciosContraPago.AplicaContraPago)
            {
                if (servicioContrapago.ValorServicioContraPago != 0 || servicioContrapago.PorcentajeServicioContraPago != 0)
                {
                    decimal valorServicioContrapago = servicioContrapago.ValorServicioContraPago == 0 ? (servicioContrapago.PorcentajeServicioContraPago * admision.ValorDeclarado) / 100 : servicioContrapago.ValorServicioContraPago;
                    //Se mapean los campos de contrapago en los campos de la admisión
                    admision.ValorPrimaSeguro = servicio.Precio.ValorPrimaSeguro;
                    admision.ValorAdmision = servicio.Precio.Valor;
                    admision.FechaEstimadaEntrega = servicio.fechaEntrega;
                    admision.DiasDeEntrega = Int16.Parse(servicio.TiempoEntrega);
                    admision.NombreServicio = servicio.NombreServicio;
                    admision.ValorTotalImpuestos = (valorServicioContrapago * servicioContrapago.porcentajeimpuesto) / 100; //Valor porcentaje iva del contrapago
                    admision.ValorTotalRetenciones = 0;
                    admision.ValorAdicionales = servicioContrapago.ValorServicioContraPago;//Valor flete contrapago
                    admision.ValorTotal = (servicio.Precio.Valor + servicio.Precio.ValorPrimaSeguro + admision.ValorAdicionales + admision.ValorTotalImpuestos);
                    admision.ValorContrapago = admision.ValorDeclarado;
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent("Los valores del contrapago no se encuentran parametrizados.")
                    });
                }
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("El servicio seleccionado no permite Contrapago. Por favor validad con la entidad.")
                });
            }
        }

        /// <summary>
        /// Método que consulta realizado llenado objeto para insertar la verificacion de contenido de un preenvio
        /// Hevelin Dayanana Diaz - 14/10/2022
        /// </summary>
        public void MapearValoresMarcacionVerificacionContenido(long NumeroGuia, bool VerificacionContenido)
        {
            VerificacionContenido_ADM verificacion = new VerificacionContenido_ADM();
            verificacion.ADM_NumeroGuia = NumeroGuia;
            verificacion.ADM_VerificacionContenido = VerificacionContenido;
            InsertarVerificacionContenidoNumeroGuia(verificacion);
        }

        /// <summary>
        /// Metodo que inserta la marca de verificacion de contenido del preenvio
        /// Jonathan Contreras - 04/01/2023
        /// </summary>
        public void VerificacionContenidoPreenvio(long NumeroGuia, bool VerificacionContenido)
        {
            VerificacionContenido_ADM verificacion = new VerificacionContenido_ADM();
            verificacion.ADM_NumeroGuia = NumeroGuia;
            verificacion.ADM_VerificacionContenido = VerificacionContenido;
            InsertarVerificacionContenidoNumeroGuiaPreenvio(verificacion);
        }

        /// <summary>
        /// Método que insertar la verificacion de contenido asociado al preenvio creado
        /// Hevelin Dayanana Diaz - 14/10/2022
        /// </summary>
        public void InsertarVerificacionContenidoNumeroGuia(VerificacionContenido_ADM verificacion)
        {
            AdmisionPreenvioDatos.Instancia.InsertarVerificacionContenidoNumeroGuia(verificacion);
        }

        /// <summary>
        /// Método que insertar la verificacion de contenido asociado al preenvio creado en la base de datos de preenvios
        /// Jonathan Contreras - 04/01/2023
        /// </summary>
        public void InsertarVerificacionContenidoNumeroGuiaPreenvio(VerificacionContenido_ADM verificacion)
        {
            AdmisionPreenvioDatos.Instancia.InsertarVerificacionContenidoNumeroGuiaPreenvio(verificacion);
        }

        public List<TATarifasContraPago> ObtenerServicioCotizador(decimal valorContraPago, string idLocaLidadOrigen, string idLocalidadDestino, int listaPrecios, long idCentroServicio, int idFormaPago, decimal peso)
        {
            List<TATarifasContraPago> tarifas = new List<TATarifasContraPago>();
            var UriController = ConfigurationManager.AppSettings["urlServiciosController"];
            var client = new RestClient(UriController);
            var request = new RestRequest($"api/Tarifas/ObtenerListaPreciosServiciosContraPago/{valorContraPago}/{idLocaLidadOrigen}/{idLocalidadDestino}/{listaPrecios}/{idCentroServicio}/{idFormaPago}/{peso}/", Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var respuesta = JsonConvert.DeserializeObject<List<TATarifasContraPago>>(response.Content);

                if (respuesta.Count > 0)
                {
                    foreach (var element in respuesta)
                    {
                        tarifas.Add(element);
                    }
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent("El cliente no cuenta con servicios contrapago asociados.")
                    });
                }
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Error consumiendo el servicio de cotización cotrapago.")
                });
            }
            return tarifas;
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


        public RemitenteVC RemitenteEncontradoXIdCliente(ClienteCreditoVC cliente)
        {
            RemitenteVC remitente = new RemitenteVC();
            remitente.IdRemitente = cliente.IdCliente;
            remitente.TipoDocumento = "NI";
            remitente.NumeroDocumento = cliente.Nit;
            remitente.Nombre = cliente.RazonSocial;
            remitente.PrimerApellido = " ";
            remitente.SegundoApellido = " ";
            remitente.Telefono = cliente.Telefono;
            remitente.Direccion = cliente.Direccion;
            remitente.Correo = cliente.Email;
            remitente.FechaGrabacion = DateTime.Now;
            remitente.IdLocalidad = cliente.IdLocalidad;
            remitente.NombreLocalidad = cliente.NombreLocalidad;

            return remitente;
        }

        public TAPreciosAgrupadosDC ResultadoListaCotizarCliente(long idCliente, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, string fecha, int idServicio)
        {
            string uri = "api/CotizadorCliente/ResultadoListaCotizar/" + idCliente + "/" + idLocalidadOrigen + "/" + idLocalidadDestino + "/" + peso + "/" + valorDeclarado + "/" + idTipoEntrega + "/" + fecha;
            TAPreciosAgrupadosDC tAPreciosAgrupadosDC = ResultadoCotizacion(uri, idServicio);
            return tAPreciosAgrupadosDC;
        }

        public TAPreciosAgrupadosDC ResultadoListaCotizarCliente(long idCliente, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, string fecha, int idServicio, bool? esMarketplace)
        {
            string uri = "api/CotizadorCliente/ResultadoListaCotizar/" + idCliente + "/" + idLocalidadOrigen + "/" + idLocalidadDestino + "/" + peso + "/" + valorDeclarado + "/" + idTipoEntrega + "/" + fecha + "/" + esMarketplace;
            TAPreciosAgrupadosDC tAPreciosAgrupadosDC = ResultadoCotizacion(uri, idServicio);
            return tAPreciosAgrupadosDC;
        }

        /// <summary>
        /// Realiza la cotización de cliente crédito por servicio
        /// </summary>
        /// <param name="precioServicioDto">Dto con los atributos necesarios para realizar la cotización</param>
        /// <returns>Lista de tarifas asignada por el servicio cotizado</returns>
        public TAPreciosAgrupadosDC ResultadoListaCotizarClienteCredito(PrecioServicioDto precioServicioDto)
        {
            if (precioServicioDto == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Dto no valido.")
                });
            }

            String urlSeriviciosInter = conexionApi("urlSeriviciosInter", "cotizar");
            RestClient client = new RestClient(urlSeriviciosInter);
            string uri = "api/CotizadorCliente/ResultadoListaCotizarCredito";
            RestRequest requestCotizacion = new RestRequest(uri, Method.POST);
            requestCotizacion.AddHeader("Content-Type", "application/json");
            requestCotizacion.AddJsonBody(precioServicioDto);
            IRestResponse responseMessage = client.Execute(requestCotizacion);

            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                TAPreciosAgrupadosDC servicio = JsonConvert.DeserializeObject<TAPreciosAgrupadosDC>(responseMessage.Content);
                return servicio;
            }
            else
            {
                throw new Exception(responseMessage.Content);
            }
        }

        public TAPreciosAgrupadosDC ResultadoCotizacion(string uri, int idServicio)
        {
            //Consumo de servicio para cotizar un cliente credito.
            String urlSeriviciosInter = conexionApi("urlSeriviciosInter", "cotizar");

            var client = new RestClient(urlSeriviciosInter);
            var requestCotizador = new RestRequest(uri, Method.GET);
            requestCotizador.AddHeader("Content-Type", "application/json");
            IRestResponse responseMessage = client.Execute(requestCotizador);
            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                TAPreciosAgrupadosDC servicio = new TAPreciosAgrupadosDC();
                var respuesta = JsonConvert.DeserializeObject<List<TAPreciosAgrupadosDC>>(responseMessage.Content);
                if (respuesta.Count > 0)
                {
                    for (int i = 0; i < respuesta.Count; i++)
                    {
                        if (respuesta[i].IdServicio == idServicio)
                        {
                            servicio = respuesta[i];
                            break;
                        }
                        else
                        {
                            servicio = null;
                        }
                    }
                    return servicio;
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent("No hay servicios asociados que cumpla con los datos ingresados.")
                    });
                }
            }
            else
            {
                throw new Exception(responseMessage.Content);
            }
        }

        public PUCentroServiciosDC ObtenerColResponsablePorCiudad(string idLocalidadOrigen)
        {
            string urlServiciosController = conexionApi("urlServiciosController", "controller");
            var client = new RestClient(urlServiciosController);
            string uri = "api/CentrosServicio/ObtenerAgenciaCiudadCredito/" + idLocalidadOrigen;
            var requestController = new RestRequest(uri, Method.GET);
            requestController.AddHeader("Content-Type", "application/json");
            requestController.AddHeader("usuario", "usuario");
            IRestResponse responseMessage = client.Execute(requestController);
            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<PUCentroServiciosDC>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new Exception(responseMessage.Content);
            }
        }

        public bool ConsultarSiZonaEsDificilAcceso(PAZonaDificilAcceso zona)
        {
            string urlServiciosController = conexionApi("urlServiciosController", "Controller");
            var client = new RestClient(urlServiciosController);
            string uri = "api/ParametrosFramework/GIZonaDificilAcceso";
            var requestController = new RestRequest(uri, Method.POST);
            requestController.AddHeader("Content-Type", "application/json");
            requestController.AddHeader("usuario", "usuario");
            requestController.AddJsonBody(zona);

            IRestResponse responseMessage = client.Execute(requestController);

            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<bool>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new Exception(responseMessage.Content);
            }
        }

        /// <summary>
        /// Validación Destino para formato preenvio Simplificado Integraciones 
        /// 1- Formato simplificado , 0 Formato normal mediano
        /// Hevelin Dayana Diaz - 08/06/2022
        /// </summary>
        /// <param name="requestImpresionPreguiaSim"></param>
        /// <returns></returns>
        public byte[] ObtenerPdfGuia(long numeroGuia)
        {
            if (numeroGuia != 0)
            {
                //PreenvioAdmisionCL preenvio = ObtenerPreenvioPorNumero(numeroGuia);
                PreenvioAdmisionCL preenvio = AdmisionPreenvioDatos.Instancia.ObtenerPreenvioClienteCredito(numeroGuia);
                if (preenvio != null)
                {
                    int EsFormatoSimplificado = ConsultarFormatoGuiaSimpliLocDest(preenvio.IdCiudadDestino);

                    if (EsFormatoSimplificado == 1)
                    {
                        return ObtenerPdfFormatoPdfSimplificado(numeroGuia);
                    }
                    else
                    {
                        return ObtenerPdfFormatoPdfPregiaNormal(numeroGuia);
                    }
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent("Número de guía no existe. Favor verifique los datos.")
                    });
                }
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("El número de guía es obligatorio.")
                });
            }
        }

        public byte[] ObtenerPdfGuiaFormatoPeq(long numeroGuia)
        {
            if (numeroGuia != 0)
            {
                PreenvioAdmisionCL preenvio = ObtenerPreenvioPorNumero(numeroGuia);
                if (preenvio != null)
                {
                    if (preenvio.NumeroPreenvio == numeroGuia)
                    {
                        //Consumo de servicio para obtener pdf de la guia de transporte
                        String URIAdmisionServer = ConfigurationManager.AppSettings["urlApiImpresionesMS"];

                        if (String.IsNullOrEmpty(URIAdmisionServer))
                        {
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                            {
                                Content = new StringContent("Url servidor impresiones (urlApiImpresionesMS) no encontrado en configuración.")
                            });
                        }

                        string hostAdmision = URIAdmisionServer;

                        List<long> listaNumeroGuia = new List<long>();
                        listaNumeroGuia.Add(numeroGuia);

                        var clientGuia = new RestClient(hostAdmision);
                        string uri = "api/Preenvios/ObtenerArrByteGuiaPequena";
                        var requestPdf = new RestRequest(uri, Method.POST);
                        requestPdf.Timeout = 10000000;
                        requestPdf.AddHeader("Content-Type", "application/json");
                        requestPdf.AddJsonBody(listaNumeroGuia);
                        IRestResponse responseMessage = clientGuia.Execute(requestPdf);

                        if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
                        {

                            var respuesta = JsonConvert.DeserializeObject<byte[]>(responseMessage.Content);
                            return respuesta;
                        }
                        else
                        {
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                            {
                                Content = new StringContent("Servicio no disponible en este momento.")
                            });
                        }
                    }
                    else
                    {
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                        {
                            Content = new StringContent("Número de guía no existe. Favor verifique los datos.")
                        });
                    }
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent("Número de guía no existe. Favor verifique los datos.")
                    });
                }

            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("El número de guía es obligatorio.")
                });
            }
        }

        /// <summary>
        /// Obtiene el arreglo de bytes de los formatos de guía de una lista de preenvios
        /// </summary>
        /// <param name="requestImpresionPreguia"></param>
        /// <returns></returns>
        public ResponseImpresionPreguia ObtenerByArrGuias(RequestImpresionPreguia requestImpresionPreguia)
        {
            ValidarCampos(requestImpresionPreguia);
            ResponseImpresionPreguia resposeImpPreguias = new ResponseImpresionPreguia();
            resposeImpPreguias.LtsPreenviosNoIncluidos = new List<long>();
            List<long> ltsGuiasExistentes = new List<long>();
            var cliente = ObtenerClienteCreditoActivo(requestImpresionPreguia.IdCliente, requestImpresionPreguia.IdSucursal);
            if (cliente.IdCliente == 0)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Cliente o sucursal invalida, por favor valide.")
                });
            }
            if (requestImpresionPreguia.PorRangoFecha)
            {
                //Consulte las guías admitidas por el cliente y sucursal, segun rangos de fecha
                ltsGuiasExistentes = ConsultarPreGuiasPorFechas(requestImpresionPreguia);
            }
            else
            {
                //Consulte cada guia que viene en la lista de guías, validando que cada una pertenezca al cliente y sucursal enviada
                foreach (var element in requestImpresionPreguia.LtsPreguias)
                {
                    bool existePreenvio = ValidarExisteciaPreenvio(element, requestImpresionPreguia.IdCliente, requestImpresionPreguia.IdSucursal);
                    if (existePreenvio)
                    {
                        //Llenar lista de guías que si existen
                        ltsGuiasExistentes.Add(element);
                    }
                    else
                    {
                        //Llenar lista de guías que no existen
                        resposeImpPreguias.LtsPreenviosNoIncluidos.Add(element);
                    }
                }
            }
            if (ltsGuiasExistentes.Count > 0)
            {
                if (requestImpresionPreguia.Formato == ID_FORMATO_GUIA_NOR || requestImpresionPreguia.Formato == 0)
                {
                    //Consuma servicio de impresiones
                    resposeImpPreguias.PdfGuias = ObtenerBase64Guias(ltsGuiasExistentes);
                }
                else if (requestImpresionPreguia.Formato == ID_FORMATO_GUIA_PEQUE)
                {
                    //Consumo servicio de impresiones formato guía pequeña
                    resposeImpPreguias.PdfGuias = ObtenerBase64GuiasFormatoPequeño(ltsGuiasExistentes);
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent("El formato indicado no es el correcto. Favor verifique e intente nuevamente.")
                    });
                }

            }
            if (resposeImpPreguias.LtsPreenviosNoIncluidos.Count > 0)
            {
                resposeImpPreguias.MsjError = "Algunos de estos preenvíos no corresponden al cliente y sucursal ingresados";
            }
            resposeImpPreguias.FechaEjcucion = DateTime.Now;
            return resposeImpPreguias;
        }

        /// <summary>
        /// Valida los campos del objeto RequestImpresionPreguia no sen nulos
        /// </summary>
        /// <param name="impresionPreguia"></param>
        public void ValidarCampos(RequestImpresionPreguia impresionPreguia)
        {
            const string msjCamposInvalidos = "Algunos campos obligatorios no fueron diligenciados. Favor verifique e intente nuevamente";
            bool camposInvalidos;
            if (impresionPreguia.PorRangoFecha)
            {
                bool fechaValida = ValidarFechas(impresionPreguia.FechaInicio, impresionPreguia.FechaFinal);

                camposInvalidos = (impresionPreguia.IdCliente == 0
                    || impresionPreguia.IdSucursal == 0
                    || !fechaValida);

                if (camposInvalidos)
                {
                    throw new Exception(msjCamposInvalidos);
                }
            }
            else
            {
                camposInvalidos = (impresionPreguia.IdCliente == 0
                    || impresionPreguia.IdSucursal == 0
                    || impresionPreguia.LtsPreguias.Count == 0);
                if (camposInvalidos)
                {
                    throw new Exception(msjCamposInvalidos);
                }
            }
        }
        /// <summary>
        /// Valida que las fechas ingresadas sean validas
        /// </summary>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        public bool ValidarFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            bool EsFechaValida = false;
            string diasImpresionPreevios = ConfigurationManager.AppSettings["DiasImpresionPreenvios"];
            if (string.IsNullOrEmpty(diasImpresionPreevios))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("El parametro DiasImpresionPreenvios no se encuentra configurado.")
                });
            }

            if (fechaInicio.Ticks > 0 && fechaFin.Ticks > 0)
            {
                EsFechaValida = true;

                int result = DateTime.Compare(fechaInicio, fechaFin);
                int resultFechaActual = DateTime.Compare(fechaFin, DateTime.Now);
                if (result > 0)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent("La fecha y hora inicial debe ser menor a la final. Verifique los datos e intente nuevamente.")
                    });
                }
                if (resultFechaActual > 0)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent("La fecha fin no puede ser mayor a la fecha actual.")
                    });
                }

                TimeSpan diferenciaHoraria = fechaFin - fechaInicio;
                double maximoDiasHabilitados = Convert.ToDouble(diasImpresionPreevios);
                if (diferenciaHoraria.TotalDays > maximoDiasHabilitados)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent($"El rango de fecha sobrepasa la cantidad máxima de días. Favor limitar los parámetros a un máximo de {Convert.ToInt16(diasImpresionPreevios)} días.")
                    });
                }
            }
            return EsFechaValida;
        }

        /// <summary>
        /// Valida que el preenvio exista y pertenezca a el cliente credito especifico
        /// </summary>
        /// <param name="numeroPreenvio"></param>
        /// <param name="idCliente"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public bool ValidarExisteciaPreenvio(long numeroPreenvio, long idCliente, long idSucursal)
        {
            return instanciaAdmisionDatos.ValidarExistenciaPreenvio(numeroPreenvio, idCliente, idSucursal);
        }

        /// <summary>
        /// Consulta las preguias por rango de fecha, cliente y sucursal
        /// </summary>
        /// <param name="requestPreeGuias"></param>
        /// <returns></returns>
        public List<long> ConsultarPreGuiasPorFechas(RequestImpresionPreguia requestPreeGuias)
        {
            List<long> listaPreeGuias = new List<long>();
            listaPreeGuias = instanciaAdmisionDatos.ConsultarPreGuiasPorFechas(requestPreeGuias);
            if (listaPreeGuias.Count == 0)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("No se encontraron guías con los datos ingresados, por favor valide.")
                });
            }
            return listaPreeGuias;
        }

        /// <summary>
        /// Consume el servicio de impresioneMs que obtiene el arreglo de bytes 
        /// </summary>
        /// <param name="lstPreGuias"></param>
        /// <returns></returns>
        public byte[] ObtenerBase64Guias(List<long> lstPreGuias)
        {
            //Consumo de servicio para obtener pdf de la guia de transporte
            String URIAdmisionServer = ConfigurationManager.AppSettings["urlApiImpresionesMS"];

            if (String.IsNullOrEmpty(URIAdmisionServer))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Url servidor admision no encontrado en configuración.")
                });
            }

            string hostAdmision = URIAdmisionServer;

            var clientGuia = new RestClient(hostAdmision);
            string uri = "api/Preenvios/ObtenerArrByteImpresionGuias";
            var requestPdf = new RestRequest(uri, Method.POST);
            requestPdf.Timeout = 10000000;
            requestPdf.AddHeader("Content-Type", "application/json");
            requestPdf.AddJsonBody(lstPreGuias);
            IRestResponse responseMessage = clientGuia.Execute(requestPdf);

            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<byte[]>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("No es posible conectarse con el servicio de impresion.")
                });
            }
        }

        /// <summary>
        /// Consume el servicio de impresioneMs que obtiene el arreglo de bytes 
        /// </summary>
        /// <param name="lstPreGuias"></param>
        /// <returns></returns>
        public byte[] ObtenerBase64GuiasFormatoPequeño(List<long> lstPreGuias)
        {
            //Consumo de servicio para obtener pdf de la guia de transporte formato pequeña
            String URIAdmisionServer = ConfigurationManager.AppSettings["urlApiImpresionesMS"];

            if (String.IsNullOrEmpty(URIAdmisionServer))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Url servidor admision no encontrado en configuración.")
                });
            }

            string hostAdmision = URIAdmisionServer;

            var clientGuia = new RestClient(hostAdmision);
            string uri = "api/Preenvios/ObtenerArrByteGuiaPequena";
            var requestPdf = new RestRequest(uri, Method.POST);
            requestPdf.Timeout = 10000000;
            requestPdf.AddHeader("Content-Type", "application/json");
            requestPdf.AddJsonBody(lstPreGuias);
            IRestResponse responseMessage = clientGuia.Execute(requestPdf);

            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<byte[]>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("No es posible conectarse con el servicio de impresion.")
                });
            }
        }

        /// <summary>
        /// Obtiene todos los estados que puede tener una guia
        /// Hevelin Dayana Diaz - 05/10/2021
        /// </summary>
        /// <returns>Lista de estados mensajeria</returns>
        public List<EstadoGuia_MEN> ObtenerEstadosLogisticosGuias()
        {
            return AdmisionPreenvioDatos.Instancia.ObtenerEstadosLogisticosGuias();
        }

        /// <summary>
        /// Obtiene un preenvio por número
        /// Hevelin Dayana Diaz - 05/01/2022
        /// </summary>
        /// <returns>Lista de estados mensajeria</returns>
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
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent($"No fue posible obtener el preenvio del cliente. {responseMessage.Content}")
                });
            }
        }


        /// <summary>
        /// Método que consulta por localidad de destino si retorna formato en base 64 normal o base 64 simplificado
        /// Hevelin Dayana Diaz - 08/06/2022
        /// </summary>
        /// <param name="IdLocalidadDes"></param>
        /// <returns>1 si es base 64 simplificado, 2 si es base 64 normal</returns>
        public int ConsultarFormatoGuiaSimpliLocDest(string IdLocalidadDes)
        {
            return AdmisionPreenvioDatos.Instancia.ConsultarFormatoGuiaSimpliLocDest(IdLocalidadDes);
        }

        /// <summary>
        /// Método que retorna base 64 de formato guia simplificado filtrado por numero de preguia
        /// Hevelin Dayana Diaz - 08/06/2022
        /// </summary>
        /// <param name="NumeroPreguia"></param>
        /// <returns>base 64 simplificado</returns>
        public byte[] ObtenerPdfFormatoPdfSimplificado(long NumeroPreguia)
        {
            //Consumo de servicio para obtener pdf de la guia de transporte
            string URIAdmisionServer = ConfigurationManager.AppSettings["urlApiImpresionesMS"];

            if (string.IsNullOrEmpty(URIAdmisionServer))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Url servidor impresione no encontrado en configuración.")
                });
            }

            string hostAdmision = URIAdmisionServer;

            RestClient clientGuia = new RestClient(hostAdmision);
            string uri = "api/Preenvios/ObtenerBase64ImpresionSimplificada/" + NumeroPreguia.ToString();
            RestRequest requestPdf = new RestRequest(uri, Method.GET);
            requestPdf.Timeout = 10000000;
            requestPdf.AddHeader("Content-Type", "application/json");
            IRestResponse responseMessage = clientGuia.Execute(requestPdf);

            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                byte[] respuesta = JsonConvert.DeserializeObject<byte[]>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Servicio no disponible en este momento.")
                });
            }
        }

        public byte[] ObtenerPdfFormatoPdfPregiaNormal(long numeroGuia)
        {
            //Consumo de servicio para obtener pdf de la guia de transporte
            string URIAdmisionServer = ConfigurationManager.AppSettings["urlApiImpresionesMS"];

            if (string.IsNullOrEmpty(URIAdmisionServer))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Url servidor admision no encontrado en configuración.")
                });
            }

            string hostAdmision = URIAdmisionServer;

            RestClient clientGuia = new RestClient(hostAdmision);
            string uri = "api/Preenvios/ObtenerBase64Impresion/" + numeroGuia.ToString();
            RestRequest requestPdf = new RestRequest(uri, Method.GET);
            requestPdf.AddHeader("Content-Type", "application/json");
            IRestResponse responseMessage = clientGuia.Execute(requestPdf);

            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {

                byte[] respuesta = JsonConvert.DeserializeObject<byte[]>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("No es posible conectarse con el servicio de impresion.")
                });
            }
        }
    }
}
