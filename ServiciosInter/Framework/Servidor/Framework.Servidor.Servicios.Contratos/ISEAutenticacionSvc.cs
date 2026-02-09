using System.Collections.Generic;
using System.ServiceModel;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace Framework.Servidor.Servicios.Contratos
{
  [ServiceContract(Namespace = "http://contrologis.com")]
  public interface ISEAutenticacionSvc
  {
    /// <summary>
    /// Autentica el usuario
    /// </summary>
    /// <param name="credencial">Credencial con el nombre de usuario</param>
    /// <returns>Credencial con la informacion basica del usuario y los menus y modulos a los que tiene acceso</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    SECredencialUsuario AutenticarUsuario(SECredencialUsuario credencial);

    /// <summary>
    /// Autentica el usuario según el sistema de información al cual esté ingresando
    /// </summary>
    /// <param name="credencial">Credencial con el nombre de usuario</param>
    /// <returns>Credencial con la informacion basica del usuario y los menus y modulos a los que tiene acceso</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    ContratoDatos.Seguridad.SECredencialUsuario AutenticarUsuarioXSInformacion(ContratoDatos.Seguridad.SECredencialUsuario credencial, SEEnumSistemaInformacion sistemaInformacion);

    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    /// <summary>
    /// Retorna los tipos de autenticación
    /// </summary>
    /// <returns></returns>
    IEnumerable<SETipoAutenticacion> ObtenerTiposAutenticacion();
  }
}