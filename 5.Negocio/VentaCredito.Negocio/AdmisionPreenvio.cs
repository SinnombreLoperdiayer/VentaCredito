using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;

namespace VentaCredito.Negocio
{
    public class AdmisionPreenvio
    {
        private static AdmisionPreenvio instancia = new AdmisionPreenvio();

        public static AdmisionPreenvio Instancia { get { return instancia; } }


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

        public ResponsePreAdmisionWrapper RegistrarPreenvio(RequestPreAdmisionWrapperCV admision)
        {
            string urlPreenvios = conexionApi("urlApiPreenvios", "admision");
            RestClient clientGuia = new RestClient(urlPreenvios);
            string uri = "api/Admision/InsertarPreenvioCL";

            RestRequest requestAdmision = new RestRequest(uri, Method.POST);
            requestAdmision.AddHeader("Content-Type", "application/json");
            requestAdmision.AddJsonBody(admision);

            IRestResponse responseMessage = clientGuia.Execute(requestAdmision);

            if (responseMessage.StatusCode == HttpStatusCode.OK && responseMessage.Content != null)
            {
                ResponsePreAdmisionWrapper respuesta = JsonConvert.DeserializeObject<ResponsePreAdmisionWrapper>(responseMessage.Content);
                return respuesta;
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent($"Error al registrar preenvío: {responseMessage.Content}")
                });
            }
        }
    }
}
