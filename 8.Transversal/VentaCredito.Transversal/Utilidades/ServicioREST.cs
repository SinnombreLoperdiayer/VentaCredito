using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Transversal.Entidades;

namespace VentaCredito.Transversal.Utilidades
{
    public class ServicioREST
    {
        private static ServicioREST instancia = new ServicioREST();
        private string urlApiServicioNotificaciones;

        public static ServicioREST Instancia
        {
            get
            {
                return instancia;
            }
        }

        public ServicioREST()
        {
            urlApiServicioNotificaciones = ConfigurationManager.AppSettings["urlApiServicioNotificaciones"];
        }


        public void NotificarClienteCredito(ADTransmisionNotificacion transmisionNotificacion, string usuarioConexion, string idClienteConexion)
        {   
            var restClient = new RestClient(urlApiServicioNotificaciones);
            var restRequest = new RestRequest(string.Format("api/Notificador/NotificarClienteCredito/"), Method.POST);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("Usuario", usuarioConexion);
            restRequest.AddHeader("IdCliente", idClienteConexion);
            restRequest.AddJsonBody(transmisionNotificacion);
            RestResponse restResponse = new RestResponse();
            try
            {
                restResponse = restClient.Execute(restRequest) as RestResponse;
                if (restResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {                    
                    throw new Exception(restResponse.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                var parametros = JsonConvert.SerializeObject(transmisionNotificacion);
                AdministradorArchivos.Instancia.CrearArchivoLogGeneral(ex, "NotificarClienteCredito", "ServicioREST", parametros);
            }
        }
    }
}
