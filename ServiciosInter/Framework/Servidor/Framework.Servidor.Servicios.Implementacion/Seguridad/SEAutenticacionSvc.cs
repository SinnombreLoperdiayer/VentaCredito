using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Servicios.Contratos;

namespace Framework.Servidor.Servicios.Implementacion
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class SEAutenticacionSvc : ISEAutenticacionSvc
  {
    public SEAutenticacionSvc()
    {
      Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura")); 
    }

    /// <summary>
    /// Autentica el usuario
    /// </summary>
    /// <param name="credencial">Credencial con el nombre de usuario</param>
    /// <returns>Credencial con la informacion basica del usuario y los menus y modulos a los que tiene acceso</returns>
    public ContratoDatos.Seguridad.SECredencialUsuario AutenticarUsuario(ContratoDatos.Seguridad.SECredencialUsuario credencial)
    {
      return SEProveedor.Instancia.AutenticarUsuario(credencial);
    }

    /// <summary>
    /// Autentica el usuario según el sistema de información al cual esté ingresando
    /// </summary>
    /// <param name="credencial">Credencial con el nombre de usuario</param>
    /// <returns>Credencial con la informacion basica del usuario y los menus y modulos a los que tiene acceso</returns>
    public ContratoDatos.Seguridad.SECredencialUsuario AutenticarUsuarioXSInformacion(ContratoDatos.Seguridad.SECredencialUsuario credencial, SEEnumSistemaInformacion sistemaInformacion)
    {
        return SEProveedor.Instancia.ObtenerMenusModulosXUsuarioYSInformacion(credencial,sistemaInformacion);
    }

    /// <summary>
    /// Retorna los tipos de autenticación
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SETipoAutenticacion> ObtenerTiposAutenticacion()
    {
      return SEAdministradorSeguridad.Instancia.ObtenerTiposAutenticacion();
    }
  }
}