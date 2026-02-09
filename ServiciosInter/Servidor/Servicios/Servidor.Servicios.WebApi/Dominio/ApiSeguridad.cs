using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.ModelosRequest.Seguridad;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiSeguridad : ApiDominioBase
    {
        private static readonly ApiSeguridad instancia = (ApiSeguridad)FabricaInterceptorApi.GetProxy(new ApiSeguridad(), COConstantesModulos.SEGURIDAD);

        public static ApiSeguridad Instancia
        {
            get { return ApiSeguridad.instancia; }
        }

        private ApiSeguridad()
        {

        }

        /// <summary>
        /// Autentica el usuario
        /// </summary>
        /// <param name="credencial">Credencial con el nombre de usuario</param>
        /// <returns>Credencial con la informacion basica del usuario y los menus y modulos a los que tiene acceso</returns>       
        public SECredencialUsuario AutenticarUsuario(CredencialRequest credencial)
        {
           
                SECredencialUsuario credeRequest = new SECredencialUsuario() { Usuario = credencial.Usuario, Password = credencial.Password };
                SECredencialUsuario crede = FabricaServicios.ServiciosAutenticacion.AutenticarUsuario(credeRequest);
                return crede;           

        }

    }

}

