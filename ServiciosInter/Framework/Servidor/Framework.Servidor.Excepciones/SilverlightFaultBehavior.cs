using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using Framework.Cliente.Comun;
using Framework.Servidor.Excepciones.Session;

namespace Framework.Servidor.Excepciones
{
    public class SilverlightFaultBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            SilverlightFaultMessageInspector inspector = new SilverlightFaultMessageInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        public class SilverlightFaultMessageInspector : IDispatchMessageInspector
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
                    string usuariosesion = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("Usuario", "http://controller.com");
                    string Idusuariosesion = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("IdUsuario", "http://controller.com");
                    string IdCentroServicioSesion = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("IdCentroServicio", "http://controller.com");
                    string nombreCentroServicioSesion = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("NombreCentroServicio", "http://controller.com");
                    string idAplicativoOrigen = "0";
                    if (OperationContext.Current.IncomingMessageHeaders.FindHeader("IdAplicativoOrigen", "http://controller.com") > 0)
                    {
                        idAplicativoOrigen = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("IdAplicativoOrigen", "http://controller.com");
                    }


                    OperationContext.Current.Extensions.Add(new ControllerContext()
                    {
                        Usuario = usuariosesion,
                        CodigoUsuario = long.Parse(Idusuariosesion),
                        NombreCentroServicio = nombreCentroServicioSesion,
                        IdCentroServicio = long.Parse(IdCentroServicioSesion),
                        IdAplicativoOrigen = int.Parse(idAplicativoOrigen)
                    });
                }
                catch
                {
                    OperationContext.Current.Extensions.Add(new ControllerContext() { Usuario = "", CodigoUsuario = 0, IdCentroServicio = 0, NombreCentroServicio = "", IdAplicativoOrigen = 0 });
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
            get { return typeof(SilverlightFaultBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SilverlightFaultBehavior();
        }
    }


    public class FaultBehaviorSesion : BehaviorExtensionElement, IEndpointBehavior
    {
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            FaultMessageInspectorSesion inspector = new FaultMessageInspectorSesion();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        public class FaultMessageInspectorSesion : IDispatchMessageInspector
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
                
                    string usuariosesion = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("Usuario", "http://controller.com");
                    string Idusuariosesion = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("IdUsuario", "http://controller.com");
                    string IdCentroServicioSesion = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("IdCentroServicio", "http://controller.com");
                    string nombreCentroServicioSesion = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("NombreCentroServicio", "http://controller.com");
                    string idAplicativoOrigen = "0";
                    if (OperationContext.Current.IncomingMessageHeaders.FindHeader("IdAplicativoOrigen", "http://controller.com") > 0)
                    {
                        idAplicativoOrigen = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("IdAplicativoOrigen", "http://controller.com");
                    }
                    string idMaquina = "";
                    if (OperationContext.Current.IncomingMessageHeaders.FindHeader("IdMaquina", "http://controller.com") > 0)
                    {
                        idMaquina = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("IdMaquina", "http://controller.com");
                    }

                    string idCaja = "0";
                    if (OperationContext.Current.IncomingMessageHeaders.FindHeader("IdCaja", "http://controller.com") > 0)
                    {
                        idCaja = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("IdCaja", "http://controller.com");
                    }

                    string tokenSesion = "";
                    if (OperationContext.Current.IncomingMessageHeaders.FindHeader("TokenSesion", "http://controller.com") > 0)
                    {
                        tokenSesion = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("TokenSesion", "http://controller.com");
                    }
                    else
                    {
                        throw new FaultException("Error con token de sesión");
                    }
                    if (string.IsNullOrWhiteSpace(tokenSesion))
                    {
                        throw new FaultException("Error con token de sesión");
                    }

                    

                    OperationContext.Current.Extensions.Add(new ControllerContext()
                    {
                        Usuario = usuariosesion,
                        CodigoUsuario = long.Parse(Idusuariosesion),
                        NombreCentroServicio = nombreCentroServicioSesion,
                        IdCentroServicio = long.Parse(IdCentroServicioSesion),
                        IdAplicativoOrigen = int.Parse(idAplicativoOrigen),
                        IdCaja = int.Parse(idCaja),
                        IdentificadorMaquina = idMaquina,
                        TokenSesion = tokenSesion
                    });

                    ValidarSesionUsuario(tokenSesion);
               

                return request.Headers.MessageId;
            }
            /// <summary>
            /// Valida la sesion
            /// </summary>
            /// <param name="token"></param>
            private void ValidarSesionUsuario(string token)
            {
                SESesionUsuario.Instancia.ValidaSesionUsuario(token);                
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
            get { return typeof(FaultBehaviorSesion); }
        }

        protected override object CreateBehavior()
        {
            return new FaultBehaviorSesion();
        }
    }



    public class ControllerContext : IExtension<OperationContext>
    {
        //The "current" custom context
        public static ControllerContext Current
        {
            get
            {
                return OperationContext.Current == null ? null : OperationContext.Current.Extensions.Find<ControllerContext>();
            }
        }

        #region IExtension<OperationContext> Members

        public void Attach(OperationContext owner)
        {
            //no-op
        }

        public void Detach(OperationContext owner)
        {
            //no-op
        }

        #endregion IExtension<OperationContext> Members

        string usuario;

        long codigoUsuario;

        long idCentroServicio;

        string nombreCentroServicio;

        int idAplicativoOrigen;

        public int IdAplicativoOrigen
        {
            get { return idAplicativoOrigen; }
            set { idAplicativoOrigen = value; }
        }

        public string NombreCentroServicio
        {
            get { return nombreCentroServicio; }
            set { nombreCentroServicio = value; }
        }

        public long IdCentroServicio
        {
            get { return idCentroServicio; }
            set { idCentroServicio = value; }
        }

        public string Usuario
        {
            get { return this.usuario; }
            set { this.usuario = value; }
        }

        public long CodigoUsuario
        {
            get { return this.codigoUsuario; }
            set { this.codigoUsuario = value; }
        }

        public long Identificacion { get; set; }

        private string identificadorMaquina = "";

        public string IdentificadorMaquina
        {
            get { return identificadorMaquina; }
            set { identificadorMaquina = value; }
        }

        public int IdCaja { get; set; }

        public string TokenSesion { get; set; }

    }
}