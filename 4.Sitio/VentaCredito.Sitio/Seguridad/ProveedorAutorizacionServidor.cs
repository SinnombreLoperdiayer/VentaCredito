using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Channels;
using VentaCredito.Seguridad;
using VentaCredito.Transversal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Owin.Security;
using CustomException;
using VentaCredito.Clientes.Datos.Repositorio;

namespace VentaCredito.Sitio.Seguridad
{
    public class ProveedorAutorizacionServidor : OAuthAuthorizationServerProvider
    {
 
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        /// <summary>
        /// Metodo que obtiene el tipo y el token que se necesita para actualizar el token asociado a un usuario, cada vez que lo genere.
        /// Hevelin Dayana Diaz - 28/07/2021
        /// /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            var token = context.AccessToken;
            var tipoToken = context.Options.AuthenticationType;
            var tokenCompleto = tipoToken + " " + token;
            var usuario = context.Request.Headers.GetValues("x-app-signature")?.ElementAt(0);
            CLClienteCreditoRepositorio.Instancia.InsertarTokenUsuarioIntegracion(tokenCompleto, usuario);
            return ;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var remoteIpAddresss = context.Request.RemoteIpAddress;

            //Valida si es un request a urls especificas del servicio
            var seccionUrl = context.Request.Uri.AbsolutePath.Split('/');
            var metodo = seccionUrl.Count() >= 5 ? seccionUrl[4] : string.Empty;
            string Usuario = null;
            string password = null;


            var autorizacion = new Autenticacion();
            string HAutorizaciontmp = context.Request.Headers.GetValues("Authorization")?.ElementAt(0);
            if (String.IsNullOrEmpty(HAutorizaciontmp))
            {
                throw new Exception("Usted no se encuentra autorizado.");
            }

            if (!HAutorizaciontmp.Contains("Basic"))
            {
                throw new Exception("Invalid Authorization.");
            }


            string Base64AUT = HAutorizaciontmp.Replace("Basic ", "");

            string Credential = Base64Decode(Base64AUT);
            IList<string> Credenciales = Credential.Split(':');
            if (Credenciales.Count > 1)
            {
                Usuario = Credenciales.ElementAt(0);
                password = Credenciales.ElementAt(1);
            }
            else
            {
                throw new Exception("Credenciales Invalidas.");
            }
            string appSignature = context.Request.Headers.GetValues("x-app-signature")?.ElementAt(0);
            if (appSignature != Usuario)
            {
                throw new Exception("Credenciales Invalidas.");
            }
            string HAutorizacion = password;

            if (String.IsNullOrEmpty(HAutorizacion))
            {
                throw new Exception("Usted no se encuentra autorizado para consumir este metodo.");
            }
            if (string.IsNullOrEmpty(metodo))
            {
                metodo = seccionUrl.Count() >= 3 ? seccionUrl[3] : string.Empty;

                //var token = context.OwinContext.Response;



                CrearContexto(appSignature, HAutorizacion, metodo, "");
                if (metodo.ToLower().Equals("token"))
                {

                    autorizacion = SeguridadServicio.Instancia.ValidarUsuarioServicio();


                    if (autorizacion.EstaAutorizado)
                    {
                        var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                        identity.AddClaim(new Claim("sub", appSignature));
                        identity.AddClaim(new Claim("role", "user"));
                        context.Validated(identity);
                    }

                }
                else
                {
                    context.SetError(autorizacion.ErrorAutenticacion);
                    return;
                }


            }

            else
            {
                CrearContexto(appSignature, HAutorizacion, metodo, "");
                autorizacion = SeguridadServicio.Instancia.ValidarAutorizacionServicio();

                if (!autorizacion.EstaAutorizado)
                {
                    context.SetError("Error_autenticacion", autorizacion.ErrorAutenticacion);
                    return;
                }
            }
        }

        public static void CrearContexto(string usuario, string password, string metodo, string token)
        {
            IContextChannel mockedChannel = (IContextChannel)ChannelFactory<IMockedService>.CreateChannel(
               new CustomBinding(new BinaryMessageEncodingBindingElement(),
                 new HttpTransportBindingElement
                 {
                     MaxBufferSize = 2147483647,
                     MaxReceivedMessageSize = 2147483647,
                 }),
                 new EndpointAddress(new Uri(@"http://localhost/CO.Servidor.Servicios.Web/PAParametrosFW.svc")));


            OperationContext.Current = new OperationContext(mockedChannel);

            var contextoSitio = new ContextoSitio()
            {
                Usuario = usuario,
                Password = password,
                Metodo = metodo,
                Token = token
            };

            OperationContext.Current.Extensions.Add(contextoSitio);
        }
        
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}