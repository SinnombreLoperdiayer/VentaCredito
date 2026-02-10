using System;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.Web.Configuration;
using System.Web.Security;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System.DirectoryServices.AccountManagement;
using Framework.Servidor.Excepciones;
using System.ServiceModel;

namespace Framework.Servidor.Seguridad.LDAP
{
    /// <summary>
    /// clase para el manejo de la autenticacion al directorio activo.
    /// </summary>
    internal class SEAutenticacionLDAP : ActiveDirectoryMembershipProvider
    {
        //private static volatile SEAutenticacionLDAP instancia = new SEAutenticacionLDAP();
        private static SEAutenticacionLDAP instancia = new SEAutenticacionLDAP();

        const int UF_DONT_EXPIRE_PASSWD = 0x10000;
        private SEDatosDirectorioActivo datosInicializacion;
        private string userToValidate = string.Empty;

        private SEAutenticacionLDAP()
          : base()
        {
            SEDatosDirectorioActivo datosDirectorio = ObtenerDatosDominio();
            this.Inicializar(datosDirectorio);
        }

        /// <summary>
        /// Retorna la instancia para operaciones sobre el directorio activo
        /// </summary>
        public static SEAutenticacionLDAP Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new SEAutenticacionLDAP();
                }

                return instancia;
            }
        }

        #region METODOS PRIVADOS

        /// <summary>
        /// Obtiene los datos del dominio desde el config
        /// </summary>
        /// <returns></returns>
        private SEDatosDirectorioActivo ObtenerDatosDominio()
        {
            return new SEDatosDirectorioActivo()
            {
                NombreCadenaConexion = "LDAPConnection",
                NombreProvider = "ProveedorAutenticacion",
                CadenaConexion = WebConfigurationManager.ConnectionStrings["LDAPConnection"].ConnectionString,
                ClaveAdministrador = WebConfigurationManager.AppSettings["controller.LDAP.ConnectionPassword"],
                ParametroDeBusqueda = WebConfigurationManager.AppSettings["controller.LDAP.AttributeMapUsername"],
                UsuarioAdministrador = WebConfigurationManager.AppSettings["controller.LDAP.ConnectionUsername"]
            };
        }

        /// <summary>
        /// Inicializa el ActiveDirectoryMembershipProvider con los datos del directorio activo
        /// </summary>
        /// <param name="datosInicializacion">Datos del directorio activo</param>
        private void Inicializar(SEDatosDirectorioActivo datosInicializacion)
        {
            this.datosInicializacion = datosInicializacion;
            var config = new NameValueCollection();
            config.Add("connectionStringName", datosInicializacion.NombreCadenaConexion);
            config.Add("connectionUsername", datosInicializacion.UsuarioAdministrador);
            config.Add("connectionPassword", datosInicializacion.ClaveAdministrador);
            config.Add("attributeMapUsername", datosInicializacion.ParametroDeBusqueda);
            base.Initialize(datosInicializacion.NombreProvider, config);
        }

        /// <summary>
        /// Metodo privado para la validacion de las credenciales de un usuario
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        /// <param name="datosDominio">Informacion del dominio</param>
        /// <returns></returns>
        private SEEnumMensajesSeguridad ValidarCuentaUsuario(SECredencialUsuario credencial, SEDatosDirectorioActivo datosDominio)
        {
            SEEnumMensajesSeguridad mensaje = SEEnumMensajesSeguridad.USUARIONOEXISTE;
            DateTime fechaExpiracion = new DateTime();

            using (DirectoryEntry entradaRaiz = new DirectoryEntry(datosDominio.CadenaConexion, datosDominio.UsuarioAdministrador, datosDominio.ClaveAdministrador))
            using (DirectorySearcher buscadorRaiz = new DirectorySearcher(entradaRaiz))
            {
                TimeSpan timespanMaxPwdAge = new TimeSpan();
                buscadorRaiz.SearchScope = SearchScope.Subtree;
                buscadorRaiz.PropertiesToLoad.Add("maxPwdAge");
                //buscadorRaiz.Filter = "(objectClass=domainDNS)";
                buscadorRaiz.Filter = "(objectClass=person)";
                try
                {
                    System.DirectoryServices.SearchResult resultadoRaiz = buscadorRaiz.FindOne();
                    using (DirectoryEntry nodoHijoRaiz = resultadoRaiz.GetDirectoryEntry())
                    {
                        string nombreRaiz = nodoHijoRaiz.Name;
                        string valor = "maxPwdAge";
                        if (resultadoRaiz.Properties.Contains(valor))
                        {
                            long ticks = ValorAbsoluto(resultadoRaiz.Properties[valor][0]);
                            timespanMaxPwdAge = TimeSpan.FromTicks(ticks);
                        }
                    }
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    return SEEnumMensajesSeguridad.DIRECTORIOACTIVONODISPONIBLE;
                }
                catch (NullReferenceException)
                {
                    return SEEnumMensajesSeguridad.SERVIDORDOMINIOSINDNS;
                }

                using (DirectorySearcher buscadorUsuario = new DirectorySearcher(entradaRaiz))
                {
                    buscadorUsuario.SearchScope = SearchScope.Subtree;
                    buscadorUsuario.PropertiesToLoad.Add("userCertificate");
                    buscadorUsuario.Filter = "(sAMAccountName=" + credencial.Usuario + ")";
                    System.DirectoryServices.SearchResult resultadoNodo = buscadorUsuario.FindOne();

                    if (resultadoNodo == null)
                        return SEEnumMensajesSeguridad.USUARIONOEXISTE;

                    using (DirectoryEntry nodoUsuario = resultadoNodo.GetDirectoryEntry())
                    {
                        string nombreDom = nodoUsuario.Name;
                        //Chequear si el usuario está deshabilitado
                        const int ACCOUNTDISABLE = 0x0002;
                        int flags = (int)nodoUsuario.Properties["userAccountControl"].Value;
                        bool deshabilitada = Convert.ToBoolean(flags & ACCOUNTDISABLE);
                        if (deshabilitada)
                            return SEEnumMensajesSeguridad.BLOQUEADO;
                        //Vencimiento password
                        fechaExpiracion = ObtenerVencimiento(nodoUsuario, timespanMaxPwdAge);
                    }
                }
                TimeSpan tiempoRestante = new TimeSpan();

                //fechaExpiracion = DateTime.Now.AddDays(1);

                if (fechaExpiracion == DateTime.MinValue)
                {
                    tiempoRestante = TimeSpan.MinValue;
                }
                else if (fechaExpiracion.CompareTo(DateTime.Now) > 0)
                {
                    tiempoRestante = fechaExpiracion.Subtract(DateTime.Now);
                    mensaje = SEEnumMensajesSeguridad.EXITOSO;
                }
                else
                {
                    tiempoRestante = TimeSpan.MinValue;
                }
                if (tiempoRestante.TotalDays < 1)
                    mensaje = SEEnumMensajesSeguridad.DEBECAMBIARCLAVE;
                else if (tiempoRestante.TotalDays > 1 && tiempoRestante.TotalDays < 13)
                    mensaje = SEEnumMensajesSeguridad.CLAVEPORVENCER;
            }

            return mensaje;
        }

        /// <summary>
        /// Obtiene la fecha de vencimiento de la contraseña del usuario
        /// </summary>
        /// <param name="usuario">Usiario a validar</param>
        /// <param name="timespanMaxPwdAge"></param>
        /// <returns>Fecha de vencimiento de la contraseña</returns>
        private DateTime ObtenerVencimiento(DirectoryEntry usuario, TimeSpan timespanMaxPwdAge)
        {
            int flags = (int)usuario.Properties["userAccountControl"][0];

            //Si nunca expira:
            if (Convert.ToBoolean(flags & UF_DONT_EXPIRE_PASSWD))
                return DateTime.MaxValue;
            //Obtiene ultimo cambio de password
            long ticks = ObtenerInt64(usuario, "pwdLastSet");
            //Obtiene fecha de vencimiento de password
            long time = ObtenerInt64(usuario, "accountExpires");
            //Si debe cambiar el password la próxima vez
            if (ticks == 0)
                return DateTime.MinValue;
            //Si no tiene password
            if (ticks == -1)
                throw new Exception("Usuario sin clave");
            //Conversion a fecha de la ultima vez que cambió clave
            DateTime pwdLastSet = DateTime.FromFileTime(ticks);
            //Conversion de fecha de vencimiento
            DateTime pwdTime = new DateTime();

            try
            {
                pwdTime = new DateTime().AddDays(time);
            }
            catch (Exception)
            {
                pwdTime = DateTime.Now.AddDays(30);
            }

            //retorna fecha de vencimiento.
            return (pwdTime);
        }

        /// <summary>
        /// Metodo privad que actualiza la fecha de vencimiento de la contrasena, de acuerdo a los parametros del Framewrok
        /// </summary>
        /// <param name="credencial"></param>
        /// <param name="datosDominio"></param>
        /// <returns></returns>
        private bool CambiarVencimientoContrasena(SECredencialUsuario credencial, SEDatosDirectorioActivo datosDominio)
        {
            DateTime tiempo = DateTime.UtcNow;
            tiempo = tiempo.AddDays(double.Parse(ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework("DiasVencimiento")));

            using (DirectoryEntry entradaRaiz = new DirectoryEntry(datosDominio.CadenaConexion, datosDominio.UsuarioAdministrador, datosDominio.ClaveAdministrador))
            using (DirectorySearcher buscadorRaiz = new DirectorySearcher(entradaRaiz))
            {
                using (DirectorySearcher buscadorUsuario = new DirectorySearcher(entradaRaiz))
                {
                    buscadorUsuario.SearchScope = SearchScope.Subtree;
                    buscadorUsuario.PropertiesToLoad.Add("userCertificate");
                    buscadorUsuario.Filter = "(sAMAccountName=" + credencial.Usuario + ")";
                    System.DirectoryServices.SearchResult resultadoNodo = buscadorUsuario.FindOne();
                    if (resultadoNodo == null)
                        return false;
                    using (DirectoryEntry nodoUsuario = resultadoNodo.GetDirectoryEntry())
                    {
                        nodoUsuario.Properties["accountExpires"].Value = tiempo.ToFileTime().ToString();
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el valor absoluto de un numero
        /// </summary>
        /// <param name="longInt"></param>
        /// <returns></returns>
        private long ValorAbsoluto(object longInt)
        {
            return Math.Abs((long)longInt);
        }

        private Int64 ObtenerInt64(DirectoryEntry entrada, string attr)
        {
            DirectorySearcher ds = new DirectorySearcher(
                  entrada,
                  String.Format("({0}=*)", attr),
                  new string[] { attr },
                  SearchScope.Base
                  );
            SearchResult sr = ds.FindOne();
            if (sr != null)
                if (sr.Properties.Contains(attr))
                    return (Int64)sr.Properties[attr][0];
            return -1;
        }

        /// <summary>
        /// Desactiva la cuenta del usuario en el directorio activo
        /// </summary>
        /// <param name="credencial">Credencial con la cuenta del usuario</param>
        /// <param name="datosDominio">Datos del directorio activo</param>
        /// <returns>Mensaje con el resultado de la desactivacion del usuario</returns>
        private SEEnumMensajesSeguridad DesactivarUsuario(SECredencialUsuario credencial, SEDatosDirectorioActivo datosDominio)
        {
            using (DirectoryEntry entradaRaiz = new DirectoryEntry(datosDominio.CadenaConexion, datosDominio.UsuarioAdministrador, datosDominio.ClaveAdministrador))
            using (DirectorySearcher buscadorRaiz = new DirectorySearcher(entradaRaiz))
            {
                using (DirectorySearcher buscadorUsuario = new DirectorySearcher(entradaRaiz))
                {
                    buscadorUsuario.SearchScope = SearchScope.Subtree;
                    buscadorUsuario.PropertiesToLoad.Add("userCertificate");
                    buscadorUsuario.Filter = "(sAMAccountName=" + credencial.Usuario + ")";
                    System.DirectoryServices.SearchResult resultadoNodo = buscadorUsuario.FindOne();
                    if (resultadoNodo == null)
                        return SEEnumMensajesSeguridad.USUARIONOEXISTE;

                    using (DirectoryEntry nodoUsuario = resultadoNodo.GetDirectoryEntry())
                    {
                        nodoUsuario.Properties["userAccountControl"].Value = 0x0002;
                        return SEEnumMensajesSeguridad.EXITOSO;
                    }
                }
            }
        }

        #endregion METODOS PRIVADOS

        /// <summary>
        /// Desactiva una cuenta de usuario dentro del directorio activo
        /// </summary>
        /// <param name="credencial">Credencial con la cuenta del usuario</param>
        /// <returns>Mensaje con el resultado de la desactivacion del usuario</returns>
        public SEEnumMensajesSeguridad DesactivarUsuario(SECredencialUsuario credencial)
        {
            return DesactivarUsuario(credencial, this.datosInicializacion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool ValidaUsuarioExisteEnDominio(string username)
        {
            string cadenaConexion = WebConfigurationManager.ConnectionStrings["LDAPConnection"].ConnectionString;
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, cadenaConexion))
            {
                UserPrincipal up = UserPrincipal.FindByIdentity(
                    pc,
                    IdentityType.SamAccountName,
                    userToValidate);

                return (up != null);
            }
        }

        /// <summary>
        /// Valida si un usuario existe en el directorio activo
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        internal SEEnumMensajesSeguridad ValidarCuentaUsuario(SECredencialUsuario credencial)
        {
            return ValidarCuentaUsuario(credencial, this.datosInicializacion);
        }

        /// <summary>
        /// Valida las credenciales de un usuario
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        /// <returns>Mensaje con el resultado de la validacion de la credencial</returns>
        internal SEEnumMensajesSeguridad ValidarCredenciales(SECredencialUsuario credencial)
        {
            System.Diagnostics.Debug.Print("AutenticacionLDAP, ini AutenticarClave, antes de validar cuenta:" + System.DateTime.Now.ToShortTimeString());
            SEEnumMensajesSeguridad resultado = ValidarCuentaUsuario(credencial, this.datosInicializacion);
            System.Diagnostics.Debug.Print("AutenticacionLDAP, AutenticarClave, despues de validar cuenta:" + System.DateTime.Now.ToShortTimeString());
            if ((SEEnumMensajesSeguridad.EXITOSO.CompareTo(resultado) == 0) || (SEEnumMensajesSeguridad.CLAVEPORVENCER.CompareTo(resultado) == 0) || (SEEnumMensajesSeguridad.DEBECAMBIARCLAVE.CompareTo(resultado) == 0))
            {
                try
                {
                    System.Diagnostics.Debug.Print("AutenticacionLDAP, AutenticarClave, antes de validateUser, despues de inicializar:" + System.DateTime.Now.ToShortTimeString());
                    if (!base.ValidateUser(credencial.Usuario, credencial.Password))
                        resultado = SEEnumMensajesSeguridad.CLAVEINVALIDA;
                    System.Diagnostics.Debug.Print("AutenticacionLDAP, AutenticarClave, desp de validateUser:" + System.DateTime.Now.ToShortTimeString());
                }

                catch (InvalidOperationException)
                {
                    this.Inicializar(this.datosInicializacion);
                    resultado = this.ValidarCredenciales(credencial);
                }
            }
            System.Diagnostics.Debug.Print("AutenticacionLDAP, fin AutenticarClave:" + System.DateTime.Now.ToShortTimeString());
            return resultado;
        }

        /// <summary>
        /// Crea un usuario en el directorio activo
        /// </summary>
        /// <param name="credencial">Credencial del usuario</param>
        /// <returns>Mensaje con el resultado de la creacion del usuario</returns>
        internal SEEnumMensajesSeguridad CrearUsuario(SECredencialUsuario credencial)
        {
            SEEnumMensajesSeguridad resultado = SEEnumMensajesSeguridad.USUARIONOSEPUDOCREAR;
            MembershipCreateStatus estado;
            MembershipUser usuario = null;
            bool x = true;
            try
            {
                //Verifica si el usuario existe, si es asi se activa y se le cambia la informacion basica
                MembershipUser usuarioBloqueado = base.GetUser(credencial.Usuario, false);
                if (usuarioBloqueado != null)
                {
                    base.UnlockUser(credencial.Usuario);
                    base.ResetPassword(credencial.Usuario, credencial.Password);
                    usuarioBloqueado.Email = credencial.Email;
                    base.UpdateUser(usuarioBloqueado);
                    return SEEnumMensajesSeguridad.EXITOSO;
                }
                else
                {
                    usuario = base.CreateUser(credencial.Usuario, credencial.Password, credencial.Email, "", "", x, null, out estado);
                    switch (estado)
                    {
                        case MembershipCreateStatus.DuplicateEmail:
                            resultado = SEEnumMensajesSeguridad.EMAILDUPLICADO;
                            break;
                        case MembershipCreateStatus.DuplicateProviderUserKey:

                            break;
                        case MembershipCreateStatus.DuplicateUserName:
                            resultado = SEEnumMensajesSeguridad.NOMBREDEUSUARIODUPLICADO;

                            break;
                        case MembershipCreateStatus.InvalidAnswer:
                            resultado = SEEnumMensajesSeguridad.RESPUESTAINVALIDA;

                            break;
                        case MembershipCreateStatus.InvalidEmail:
                            resultado = SEEnumMensajesSeguridad.EMAILINVALIDO;

                            break;
                        case MembershipCreateStatus.InvalidPassword:
                            resultado = SEEnumMensajesSeguridad.PASSWORDINVALIDO;

                            break;
                        case MembershipCreateStatus.InvalidProviderUserKey:

                            break;

                        case MembershipCreateStatus.InvalidQuestion:
                            resultado = SEEnumMensajesSeguridad.PREGUNTAINVALIDA;
                            break;
                        case MembershipCreateStatus.InvalidUserName:
                            resultado = SEEnumMensajesSeguridad.NOMBREDEUSUARIOINVALIDO;
                            break;
                        case MembershipCreateStatus.ProviderError:
                            resultado = SEEnumMensajesSeguridad.ERRORDEPROVEEDORLDAP;
                            break;
                        case MembershipCreateStatus.Success:
                            resultado = SEEnumMensajesSeguridad.EXITOSO;
                            break;
                        case MembershipCreateStatus.UserRejected:
                            resultado = SEEnumMensajesSeguridad.USUARIORECHAZADO;
                            break;
                        default:
                            resultado = SEEnumMensajesSeguridad.NOSECREOELUSUARIO;
                            break;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                this.Inicializar(this.datosInicializacion);
                resultado = this.CrearUsuario(credencial);
            }
            return resultado;
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="credencial">Credenciales del usuario</param>
        /// <returns>Mensaje con el resultado del cambio de la constraseña</returns>
        internal SEEnumMensajesSeguridad CambiarContrasena(SECredencialUsuario credencial)
        {
            SEEnumMensajesSeguridad resultado = SEEnumMensajesSeguridad.ERRORCAMBIANDOPASSWORD;
            //DirectoryEntry(datosDominio.CadenaConexion, datosDominio.UsuarioAdministrador, datosDominio.ClaveAdministrador))
            try
            {

                var cadenaCon =  this.datosInicializacion.CadenaConexion.Split('/');


                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, cadenaCon[2], this.datosInicializacion.UsuarioAdministrador, this.datosInicializacion.ClaveAdministrador))
                {
                    UserPrincipal up = UserPrincipal.FindByIdentity(pc, credencial.Usuario);
                    //up.ChangePassword(credencial.PasswordAnterior, credencial.PasswordNuevo);
                    if (!base.ValidateUser(credencial.Usuario, credencial.PasswordAnterior))
                    {
                        throw new FaultException<ControllerException>(new ControllerException("Seguridad", "CambioPassLdap","Error validando la contraseña."));                       
                    }
                    else
                    {
                        up.SetPassword(credencial.PasswordNuevo);
                        up.Save();
                        resultado = SEEnumMensajesSeguridad.EXITOSO;
                    }

                }
            }
            catch(PasswordException pEx)
            {
                throw new FaultException<ControllerException>(new ControllerException("Seguridad", "CambioPassLdap", pEx.Message));
            }



            /*  bool cambioRealizado = false;
              SEEnumMensajesSeguridad resultado = SEEnumMensajesSeguridad.ERRORCAMBIANDOPASSWORD;

              try
              {
                  cambioRealizado = base.ChangePassword(credencial.Usuario, credencial.PasswordAnterior, credencial.PasswordNuevo);
              }
              catch (InvalidOperationException)
              {
                  this.Inicializar(this.datosInicializacion);
                  resultado = this.CambiarContrasena(credencial);
              }
              if (cambioRealizado)
              {
                  CambiarVencimientoContrasena(credencial, this.datosInicializacion);
                  resultado = SEEnumMensajesSeguridad.EXITOSO;
              }*/
            return resultado;
        }

        /// <summary>
        /// Resetea la contraseña del usuario
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        /// <returns>Mensaje con la respuesta del reseteo</returns>
        ///
        internal SEEnumMensajesSeguridad ResetearPassword(SECredencialUsuario credencial)
        {
            string rsta = "";

            SEEnumMensajesSeguridad resultado = SEEnumMensajesSeguridad.ERRORCAMBIANDOPASSWORD;
            try
            {
                rsta = base.ResetPassword(credencial.Usuario, credencial.Password);
            }
            catch (InvalidOperationException)
            {
                this.Inicializar(this.datosInicializacion);
                resultado = this.ResetearPassword(credencial);
            }
            if (!string.IsNullOrWhiteSpace(rsta))
                resultado = SEEnumMensajesSeguridad.EXITOSO;
            return resultado;
        }
    }
}