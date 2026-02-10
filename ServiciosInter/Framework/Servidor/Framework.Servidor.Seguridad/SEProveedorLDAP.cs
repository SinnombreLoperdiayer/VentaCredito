using Framework.Servidor.Seguridad.LDAP;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace Framework.Servidor.Seguridad
{
  /// <summary>
  /// Clase que intereactua con el proveedor LDAP para hacer la autenticacion del usuario
  /// </summary>
  internal class SEProveedorLDAP
  {
    public static readonly SEProveedorLDAP Instancia = new SEProveedorLDAP();

    private SEProveedorLDAP() { }

    /// <summary>
    /// Velida las credenciales de un usuario contra el directorio activo
    /// </summary>
    /// <param name="credencial">Credencial del usuario</param>
    /// <returns>Mensaje con el resultado de la creacion del usuario</returns>
    public SEEnumMensajesSeguridad AutenticarUsuario(SECredencialUsuario credencial)
    {
      return SEAutenticacionLDAP.Instancia.ValidarCredenciales(credencial);
    }

    /// <summary>
    /// Valida si un usuario existe en el directorio activo
    /// </summary>
    /// <param name="credencial"></param>
    /// <returns></returns>
    public bool ValidarUsuario(SECredencialUsuario credencial)
    {
      return SEAutenticacionLDAP.Instancia.ValidarCuentaUsuario(credencial) == SEEnumMensajesSeguridad.EXITOSO;
    }

    /// <summary>
    /// Cambia la contraseña de un usuario
    /// </summary>
    /// <param name="credencial">Credencial del usuario</param>
    /// <returns>Mensaje con el resultado de la creacion del usuario</returns>
    public SEEnumMensajesSeguridad CambiarPassword(SECredencialUsuario credencial)
    {
      return SEAutenticacionLDAP.Instancia.CambiarContrasena(credencial);
    }

    /// <summary>
    /// Crea un usuario en el directorio activo
    /// </summary>
    /// <param name="credencial">Credencial de usuario</param>
    /// <returns>Mensaje con el resultado de la creacion del usuario</returns>
    public SEEnumMensajesSeguridad CrearUsuario(SECredencialUsuario credencial)
    {
      return SEAutenticacionLDAP.Instancia.CrearUsuario(credencial);
    }

    /// <summary>
    /// Desactiva una cuenta de usuario dentro del directorio activo
    /// </summary>
    /// <param name="credencial">Credencial con la cuenta del usuario</param>
    /// <returns>Mensaje con el resultado de la desactivacion del usuario</returns>
    public SEEnumMensajesSeguridad DesactivarUsuario(SECredencialUsuario credencial)
    {
      return SEAutenticacionLDAP.Instancia.DesactivarUsuario(credencial);
    }

    /// <summary>
    /// Resetea la contraseña del usuario
    /// </summary>
    /// <param name="credencial">Credencial con la informacion del usuario</param>
    /// <returns>Mensaje con la respuesta del reseteo</returns>
    public SEEnumMensajesSeguridad ResetearPassword(SECredencialUsuario credencial)
    {
      return SEAutenticacionLDAP.Instancia.ResetearPassword(credencial);
    }
  }
}