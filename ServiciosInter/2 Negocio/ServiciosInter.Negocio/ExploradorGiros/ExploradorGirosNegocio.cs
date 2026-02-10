using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Giros.SharedTypes.BE;
using Giros.SharedTypes.BE.Giros;
using Newtonsoft.Json;
using RestSharp;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Giros;
using ServiciosInter.DatosCompartidos.Wrappers;
using ServiciosInter.Infraestructura.AccesoDatos;
using ServiciosInter.Infraestructura.AccesoDatos.Interfaces;
using ServiciosInter.Negocio.Interfaces;

namespace ServiciosInter.Negocio.ExploradorGiros
{
    public enum DocumentosIdentidad : int
    {
        CEDULA_CIUDADANIA = 1,
        CEDULA_EXTRANJERIA = 2,
        NIT = 3,
        TARJETA_IDENTIDAD = 4
    };

    /// <summary>
    /// Clase que expone la fachada del explorador de giros
    /// </summary>
    public class ExploradorGirosNegocio : IExploradorGirosNegocio
    {
        private static volatile ExploradorGirosNegocio instancia;
        private readonly IExploradorGirosRepository exploradorGirosRepository = ExploradorGirosRepository.Instancia;

        /// <summary>
        /// Atributo utilizado para evitar problemas con multithreading en el singleton.
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// Instancia de acceso ( Singleton con multithreading )
        /// </summary>
        public static ExploradorGirosNegocio Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (syncRoot)
                    {
                        if (instancia == null)
                        {
                            instancia = new ExploradorGirosNegocio();
                        }
                    }
                }
                return instancia;
            }
        }

        #region Métodos

        /// <summary>
        /// Obtiene la informacíón de un giro
        /// </summary>
        /// <param name="informacionGiro">Informacíon del giro</param>
        public ExploradorGirosWrapper ObtenerDatosGiros(ExploradorGirosWrapper informacionGiro)
        {
            if (informacionGiro == null)
            {
                throw new ArgumentNullException(nameof(informacionGiro));
            }

            ////Conexión Server Giros
            String URIGirosServer = ConfigurationManager.AppSettings.Get("URIGirosServer");
            if (String.IsNullOrEmpty(URIGirosServer))
            {
                throw new Exception("Url servidor giros no encontrado en configuración");
            }

            //if (String.IsNullOrEmpty(informacionGiro.DigitoVerificacion))
            //{
            //    throw new ArgumentNullException(nameof(informacionGiro.DigitoVerificacion), "Digito de Verificacion no valido.");
            //}

            string hostGiros = URIGirosServer;
            var client = new RestClient(hostGiros);
            string uri = "giros/{NumeroGiro}/DV/{DigitoVerificacion}";
            var request = new RestRequest(uri, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddUrlSegment("DigitoVerificacion", informacionGiro.DigitoVerificacion);
            request.AddUrlSegment("NumeroGiro", informacionGiro.NumeroGiro.ToString());
            IRestResponse<AdmisionGiro_GIR> response = client.Execute<AdmisionGiro_GIR>(request);
            AdmisionGiro_GIR giro = null;
            if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK) && response.Content!=null)
            {
                giro = JsonConvert.DeserializeObject<AdmisionGiro_GIR>(response.Content);
                return MapGiroEntity(giro);
            }

            return exploradorGirosRepository.ObtenerDatosGiros(informacionGiro);

        }

        private ExploradorGirosWrapper MapGiroEntity(AdmisionGiro_GIR giro)
        {
            ExploradorGirosWrapper explorador = new ExploradorGirosWrapper();
            explorador.CreadoPor = giro.CreadoPor.ToString();
            explorador.DigitoVerificacion = giro.DigitoVerificacion;
            explorador.NombreRemitente = giro.NombreRemitente;
            explorador.NombreDestinatario = giro.NombreRemitente;
            explorador.NumeroGiro = (long)giro.IdFacturaGiro;
            explorador.TelefonoDestinatario = giro.NumeroCelularDestinatario;
            explorador.TelefonoRemitente = giro.NumeroCelularRemitente;
            explorador.DigitoVerificacion = giro.DigitoVerificacion;
            explorador.ValorGiro = giro.ValorGiro;
            explorador.ValorTotal = giro.ValorTotal.Value;
            explorador.FechaGrabacion = giro.FechaGrabacion;
            explorador.EstadosGiro = MapEstadosTrazabilidadGiroEntity(giro.EstadosGiro);

            return explorador;
        }

        private IList<EstadosGiro_GIR> MapEstadosTrazabilidadGiroEntity(ICollection<TrazabilidadEstadoGiro_GIR> estadosGiro)
        {
            IList<EstadosGiro_GIR> lEstadosGiro = new List<EstadosGiro_GIR>();
            EstadosGiro_GIR estado = null;
            foreach (TrazabilidadEstadoGiro_GIR trazabilidad in estadosGiro)
            {
                estado = new EstadosGiro_GIR();
                estado.EstadoGiro = trazabilidad.NombreEstado;
                estado.FechaCambioEstado = trazabilidad.FechaGrabacion;
                estado.idGiro = trazabilidad.IdFacturaGiro;
                lEstadosGiro.Add(estado);
            }
            return lEstadosGiro;
        }

        #endregion Métodos
    }
}