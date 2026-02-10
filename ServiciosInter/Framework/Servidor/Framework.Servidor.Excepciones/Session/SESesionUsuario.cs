using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Framework.Servidor.Excepciones.Session
{
    public class SESesionUsuario : ControllerBase
    {
        private static readonly SESesionUsuario instancia = (SESesionUsuario)FabricaInterceptores.GetProxy(new SESesionUsuario(), ConstantesFramework.MODULO_FW_SEGURIDAD);

        /// <summary>
        /// Retorna la instancia de la clase de seguridad
        /// </summary>
        public static SESesionUsuario Instancia
        {
            get { return SESesionUsuario.instancia; }
        }

        private SESesionUsuario()
        { }


        
        /// <summary>
        /// Valida la sesion sel usuario
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        public bool ValidaSesionUsuario(string token)
        {
            string rst = SERepositorioSesion.Instancia.ValidaSesionUsuario(token);

            switch (rst)
            {
                case ConstantesRespuestaValidacionSesion.FALLO_VALIDAR:
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.SEGURIDAD, "", "Error al validar sesión."));
                    break;

                case ConstantesRespuestaValidacionSesion.MULTIPLES_SESIONES:
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.SEGURIDAD, "", "Error al validar sesión."));
                    break;

                case ConstantesRespuestaValidacionSesion.PERMITIDO:
                    return true;
                    break;

                default:
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.SEGURIDAD, "", "Error no manejado validando sesión"));
                    break;
            }

        }

        /// <summary>
        /// Acualiza los datos de centro servicio y caja para el usuario
        /// </summary>
        /// <param name="credencia"></param>
        public void ActualizaSesion()
        {
            SERepositorioSesion.Instancia.ActualizaSesion();
        }

    }
}
