using System;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using VentaCredito.Transversal.Utilidades;
using System.Configuration;
using RestSharp;
using Newtonsoft.Json;

namespace VentaCredito.GeolocalizacionNegocio
{
    public class GeolocalizacionNegocio
    {
        private static GeolocalizacionNegocio instancia = new GeolocalizacionNegocio();
        public static GeolocalizacionNegocio Instancia
        {
            get
            {
                return instancia;
            }
        }

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


        public Geolocalizacion GeolocalizacionDireccion(RequestGeomultizonaVC geomultizona, String admision)
        {
            String urlServiInformacion = conexionApi("urlServiInformacion", "servInformación");

            var clientGuia = new RestClient(urlServiInformacion);
            string uri = "api/ServiInformacion/GeoMultiZona";
            var requestGeolocalizacion = new RestRequest(uri, Method.POST);
            requestGeolocalizacion.AddHeader("Content-Type", "application/json");
            requestGeolocalizacion.AddJsonBody(geomultizona);
            IRestResponse responseMessage = clientGuia.Execute(requestGeolocalizacion);

            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                var respuesta = JsonConvert.DeserializeObject<Geolocalizacion>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                if (admision == "Preenvio")
                {
                    LogTraza.EscribirLog("Falla en servicio de georreferenciación", responseMessage.ErrorException);
                }
                return null;
            }
        }


    }

    
}
