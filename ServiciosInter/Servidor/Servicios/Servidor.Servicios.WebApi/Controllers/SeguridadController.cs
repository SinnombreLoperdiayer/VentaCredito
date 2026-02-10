using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using CO.Servidor.Servicios.WebApi.ModelosRequest.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
    [RoutePrefix("api/Seguridad")]
    public class SeguridadController : ApiController
    {

       
        
        /// <summary>
        /// Autentica el usuario
        /// </summary>
        /// <param name="credencial">Credencial con el nombre de usuario</param>
        /// <returns>Credencial con la informacion basica del usuario y los menus y modulos a los que tiene acceso</returns>
        [HttpPost]       
        //[ActionName("AutenticarUsuario")]        
        [Route("AutenticarUsuario")]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]     
        [SeguridadWebApi]
        public SECredencialUsuario AutenticarUsuario([FromBody]CredencialRequest credencial)
        {
            return ApiSeguridad.Instancia.AutenticarUsuario(credencial);
        }      

    }
   
   

 
}
