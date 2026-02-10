using CO.Controller.Servidor.Integraciones;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiMotorMensajesTexto : ApiDominioBase
    {

        #region Singleton

        private static readonly ApiMotorMensajesTexto instancia = (ApiMotorMensajesTexto)FabricaInterceptorApi.GetProxy(new ApiMotorMensajesTexto(), COConstantesModulos.MENSAJERIA);

        public static ApiMotorMensajesTexto Instancia
        {
            get { return ApiMotorMensajesTexto.instancia; }
        }

        #endregion

        #region Constructor
        public ApiMotorMensajesTexto()
        {
        }
        #endregion

        #region Metodos
        public void EnviarMensajesPendientes()
        {
            if (MensajesTexto.Instancia.ValidarEjecucionMotor())
            {
                MensajesTexto.Instancia.EnviarMensajesPendientes();
            }
        }
        #endregion

    }
}