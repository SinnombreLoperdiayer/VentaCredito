using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Seguridad.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Framework.Servidor.Seguridad
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
        /// Crea sesion de usuario 
        /// </summary>
        /// <param name="credencial"></param>
        public void CrearSesionUsuario(SECredencialUsuario credencial)
        {
            SERepositorioSesion.Instancia.CrearSesionUsuario(credencial);
        }
       
        /// <summary>
        /// Acualiza los datos de centro servicio y caja para el usuario
        /// </summary>
        /// <param name="credencia"></param>
        public void ActualizaSesion(SECredencialUsuario credencial)
        {
            SERepositorioSesion.Instancia.ActualizaSesion(credencial);
        }


    }
}
