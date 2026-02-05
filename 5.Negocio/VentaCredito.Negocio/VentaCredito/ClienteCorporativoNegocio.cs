using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Clientes.Comun;
using VentaCredito.Clientes.Datos.Repositorio;
using VentaCredito.Negocio.Interface;

namespace VentaCredito.Negocio.VentaCredito
{
    public class ClienteCorporativoNegocio : IClienteCorporativoNegocio
    {
        #region Intancia
        private static volatile ClienteCorporativoNegocio instancia;

        /// <summary>
        /// Atributo utilizado para evitar problemas con multithreading en el singleton.
        /// </summary>
        private static object syncRoot = new Object();

        public static ClienteCorporativoNegocio Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (syncRoot)
                    {
                        if (instancia == null)
                        {
                            instancia = new ClienteCorporativoNegocio();
                        }
                    }
                }
                return instancia;
            }
        }
        #endregion
        #region  Metodos

        public void EnviarCorreoCancelacionGuia(CancelacionGuia_Wrapper Cancelacion)
        {
            string UrlNegocio = ConfigurationManager.AppSettings.Get("URL_API_NEGOCIO");
            var client = new RestClient(UrlNegocio);
            string PBodyCancelacion = ConfigurationManager.AppSettings.Get("BODY_CORREO_CANCELACION");
            var request = new RestRequest("Parametros/" + PBodyCancelacion, Method.GET);         
            IRestResponse response = client.Execute(request);
            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(response.ErrorMessage);
            }
            var Body = response.Content;
            if (String.IsNullOrEmpty(Body))
            {
                throw new Exception("Error en parametro body");
            }
            Body = String.Format(Body, Cancelacion.IdGuia, Cancelacion.Observacion);
            CLClienteCreditoRepositorio.Instancia.EnviarCorreoSolicitudCancelacion(Body); 

        }

        #endregion

    }
}
