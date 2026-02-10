using System;
using System.DirectoryServices;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace Framework.Servidor.Seguridad.LDAP
{
  /// <summary>
  /// Clase para Administrar Usuarios del Directorio Activo
  /// </summary>
  internal class SEAdminUsuarioLDAP
  {
    #region Campos

    private static SEAdminUsuarioLDAP instancia = new SEAdminUsuarioLDAP();

    private string connectionPrefix;
    private string usuarioAdministrador;
    private string claveAdministrador;
    private string dominio;

    #endregion Campos

    #region Contructor

    /// <summary>
    /// Crea una instancia del administrador de usuarios LDAP
    /// </summary>
    private SEAdminUsuarioLDAP()
    {
    }

    #endregion Contructor

    #region Miembros Privados

    /// <summary>
    /// Obtiene la cadena de conexión del Directorio Activo
    /// </summary>
    public void ObtenerConfiguracionLDAP()
    {
      //Conexion Directorio Activo
      connectionPrefix = WebConfigurationManager.ConnectionStrings["LDAPConnection"].ConnectionString;

      if (String.IsNullOrEmpty(connectionPrefix))
      {
        ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, ETipoErrorFramework.EX_CADENA_CONEXION_LDAP_NULA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CADENA_CONEXION_LDAP_NULA));
        throw new FaultException<ControllerException>(excepcion);
      }

      usuarioAdministrador = WebConfigurationManager.AppSettings["Controller.LDAP.ConnectionUsername"];
      claveAdministrador = WebConfigurationManager.AppSettings["Controller.LDAP.ConnectionPassword"];

      if (String.IsNullOrEmpty(usuarioAdministrador))
      {
        ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, ETipoErrorFramework.EX_USUARIO_LDAP_NULO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_LDAP_NULO));
        throw new FaultException<ControllerException>(excepcion);
      }

      if (String.IsNullOrEmpty(claveAdministrador))
      {
        ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, ETipoErrorFramework.EX_PASSWORD_USUARIO_LDAP_NULO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_PASSWORD_USUARIO_LDAP_NULO));
        throw new FaultException<ControllerException>(excepcion);
      }

      if (String.IsNullOrEmpty(dominio))
      {
        Regex regex = new Regex("LDAP://([a-zA-Z.]*)/[a-zA-Z=,]*", RegexOptions.Compiled);
        Match dominioMatch = regex.Match(connectionPrefix);

        if (dominioMatch != null && dominioMatch.Success && dominioMatch.Groups != null && dominioMatch.Groups.Count > 0)
        {
          dominio = "@" + dominioMatch.Groups[1].Value;
        }
        else
        {
          ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, ETipoErrorFramework.EX_ERROR_OBTENER_DOMINIO_CADENA_CONEXION_LDAP.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_OBTENER_DOMINIO_CADENA_CONEXION_LDAP));
          throw new FaultException<ControllerException>(excepcion);
        }
      }
    }

    private static void GuardarDatosBasicos(SECredencialUsuario credencial, string apellido, string nombreCompleto, DirectoryEntry newUser)
    {
      newUser.Properties["givenName"].Value = credencial.Nombre;
      newUser.Properties["SN"].Value = apellido;
      newUser.Properties["displayName"].Value = nombreCompleto;
      newUser.Properties["telephoneNumber"].Value = credencial.Telefono;
      newUser.Properties["mail"].Value = credencial.Email;
      newUser.Properties["description"].Value = credencial.Comentarios;
    }

    /// <summary>
    /// Habilita la cuenta del usuario de directorio activo
    /// </summary>
    /// <param name="credencial"></param>
    /// <param name="newUser"></param>
    private static void HabilitarCuenta(SECredencialUsuario credencial, DirectoryEntry newUser)
    {
      //La contrasena nunca expira
      //int exp = (int)newUser.Properties["userAccountControl"].Value;
      //newUser.Properties["userAccountControl"].Value = exp | 0x10000;

      //Habilitar Usuario en el Directorio Activo
      int val = (int)newUser.Properties["userAccountControl"].Value;
      newUser.Properties["userAccountControl"].Value = val & ~0x2;
    }

    #endregion Miembros Privados

    #region Miembros Públicos

    /// <summary>
    /// Retornar la instancia del administrador de usuarios de directorio activo
    /// </summary>
    public static SEAdminUsuarioLDAP Instancia
    {
      get { return SEAdminUsuarioLDAP.instancia; }
    }

    /// <summary>
    /// Crear un usuario en el directorio activo
    /// </summary>
    /// <param name="credencial"></param>
    public void CrearUsuario(SECredencialUsuario credencial)
    {
      //Concatenar Nombre y Apellidos
      string apellido = string.Concat(credencial.Apellido1 + " " + credencial.Apellido2);
      string nombreCompleto = string.Concat(credencial.Nombre + " " + apellido);

      ObtenerConfiguracionLDAP();
      DirectoryEntry dirEntry = new DirectoryEntry(connectionPrefix, usuarioAdministrador, claveAdministrador);

      DirectoryEntry newUser = dirEntry.Children.Add("CN=" + nombreCompleto, "user");
      newUser.Properties["samAccountName"].Value = credencial.Usuario;
      //Nombre de inicio de sesión
      newUser.Properties["userPrincipalName"].Value = credencial.Usuario + dominio;

      //Metodo para guardar los datos basicos del usuario
      GuardarDatosBasicos(credencial, apellido, nombreCompleto, newUser);

      newUser.CommitChanges();
      //Habilita el usuario
      if (credencial.Estado == ConstantesFramework.ESTADO_ACTIVO)
      {
        HabilitarCuenta(credencial, newUser);
      }
      //Crea el password
      newUser.Invoke("SetPassword", new object[] { credencial.PasswordNuevo });
      //Crea fecha de expiracion de acuerdo al parametro en el framework
      DateTime tiempo = DateTime.UtcNow;
      tiempo = tiempo.AddDays(double.Parse(ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework("DiasVencimiento")));
      newUser.Properties["accountExpires"].Value = tiempo.ToFileTime().ToString();
      //Guarda nuevo usuario
      newUser.CommitChanges();
      dirEntry.Close();
      newUser.Close();
    }

    /// <summary>
    /// Editar un usuario en el directorio activo
    /// </summary>
    /// <param name="credencial"></param>
    public void EditarUsuario(SECredencialUsuario credencial)
    {
      //Concatenar Nombre y Apellidos
      string apellido = string.Concat(credencial.Apellido1 + " " + credencial.Apellido2);
      string nombreCompleto = string.Concat(credencial.Nombre + " " + apellido);

      ObtenerConfiguracionLDAP();
      DirectoryEntry dirEntry = new DirectoryEntry(connectionPrefix, usuarioAdministrador, claveAdministrador);

      DirectorySearcher ds = new DirectorySearcher(dirEntry);
      ds.Filter = "(samAccountName=" + credencial.Usuario + ")";
      ds.SearchScope = SearchScope.Subtree;
      SearchResult results = ds.FindOne();
      if (results != null)
      {
        DirectoryEntry updateEntry = results.GetDirectoryEntry();
        //Metodo para guardar los datos basicos del usuario
        if (!String.IsNullOrWhiteSpace(apellido) && !String.IsNullOrWhiteSpace(nombreCompleto))
          GuardarDatosBasicos(credencial, apellido, nombreCompleto, updateEntry);
        //Habilita contrasena nunca expira y habilita cuenta en el dominio
        if (credencial.Estado == ConstantesFramework.ESTADO_ACTIVO)
          HabilitarCuenta(credencial, updateEntry);
        //si hubo cambio de clave, cambiarla en el directorio activo
        if (!String.IsNullOrWhiteSpace(credencial.PasswordNuevo))
          updateEntry.Invoke("SetPassword", new object[] { credencial.PasswordNuevo });
        //cambio en la fecha de expiracion de acuerdo al parametro en el framework
        DateTime tiempo = DateTime.UtcNow;
        tiempo = tiempo.AddDays(double.Parse(ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework("DiasVencimiento")));
        updateEntry.Properties["accountExpires"].Value = tiempo.ToFileTime().ToString();
        //Guarda cambios en el directorio activo
        updateEntry.CommitChanges();
        updateEntry.Close();
      }
      dirEntry.Close();
    }

    #endregion Miembros Públicos
  }
}