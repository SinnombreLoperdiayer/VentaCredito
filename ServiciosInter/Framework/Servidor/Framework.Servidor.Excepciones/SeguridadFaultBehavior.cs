using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Framework.Servidor.Excepciones
{
    public class SeguridadFaultBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            SeguridadFaultMessageInspector inspector = new SeguridadFaultMessageInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        public class SeguridadFaultMessageInspector : IDispatchMessageInspector
        {
            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                if (reply.IsFault)
                {
                    HttpResponseMessageProperty property = new HttpResponseMessageProperty();

                    // Here the response code is changed to 200.
                    property.StatusCode = System.Net.HttpStatusCode.OK;

                    reply.Properties[HttpResponseMessageProperty.Name] = property;

                    try
                    {
                        OperationContext.Current.Extensions.Remove(ControllerContext.Current);
                    }
                    catch { }
                }
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                try
                {
                    string version = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("UriRelativa", "http://controller.com");
                    string usuario = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("Usuario", "http://controller.com");
                    if (!ArmarURL(version, usuario))
                    {
                        throw new Exception("error validando url relativa");
                    }
                }
                catch
                {
                    throw new Exception("error no url relativa");
                }

                return request.Headers.MessageId;
            }
        }

        // The following methods are stubs and not relevant.

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public override System.Type BehaviorType
        {
            get { return typeof(SeguridadFaultBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SeguridadFaultBehavior();
        }

        /// <summary> 
        /// Decrypt a string 
        /// </summary> 
        /// <param name="entrada">Input string in base 64 format</param> 
        /// <returns>Decrypted string</returns> 
        public static bool ArmarURL(string entrada, string usuario)
        {
            string archivo = "jkh978y3njdbfasd6CXXC % &% 8klj76jhkjhgk &&)/ KHJBKbn89jhoh";
            byte[] encryptedBytes = Convert.FromBase64String(entrada);
            byte[] saltBytes = Encoding.UTF8.GetBytes(archivo);
            string decryptedString = string.Empty;
            using (var aes = new AesManaged())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(archivo, saltBytes);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform decryptTransform = aes.CreateDecryptor())
                {
                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        CryptoStream decryptor =
                            new CryptoStream(decryptedStream, decryptTransform, CryptoStreamMode.Write);
                        decryptor.Write(encryptedBytes, 0, encryptedBytes.Length);
                        decryptor.Flush();
                        decryptor.Close();

                        byte[] decryptBytes = decryptedStream.ToArray();
                        decryptedString =
                            UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                    }
                }
            }

            if (string.Equals(decryptedString, string.Concat(ConfigurationManager.AppSettings["uriRelativa"],"/",usuario)))
            { return true; }
            else
            { return false; }


        }
    }
}
