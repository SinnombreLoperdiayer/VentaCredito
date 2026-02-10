using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Seguridad.Datos;
using Framework.Servidor.Seguridad.LDAP;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace Framework.Servidor.Seguridad
{
    internal class SEProvedorSql : MarshalByRefObject
    {
        public static readonly SEProvedorSql Instancia = (SEProvedorSql)FabricaInterceptores.GetProxy(new SEProvedorSql(), "Seguridad");

        private SEProvedorSql() { }

        private const string FormatoClaveHashed = "hashed";
        private const string FormatoClavePlana = "plana";

        /// <summary>
        /// Metodo para autenticar el usuario
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        /// <returns>Resultado de la autenticacion</returns>
        public SEEnumMensajesSeguridad AutenticarUsuario(SECredencialUsuario credencial)
        {

            SEEnumMensajesSeguridad Mensaje = new SEEnumMensajesSeguridad();
            string tipoUsuario = SERepositorio.Instancia.ObtenerTipoUsuario(credencial);



            if (tipoUsuario == null)
                return SEEnumMensajesSeguridad.USUARIONOEXISTE;


            if (tipoUsuario == COConstantesModulos.USUARIO_APLICACION)
            {
                SECredencialUsuario usuarioDB = SERepositorio.Instancia.AutenticarUsuario(credencial);

                if (usuarioDB.ClaveBloqueada)
                    return SEEnumMensajesSeguridad.BLOQUEADO;


                string clave = string.Empty;
                credencial.IdCodigoUsuario = usuarioDB.IdCodigoUsuario;
                credencial.EsUsuarioInterno = usuarioDB.EsUsuarioInterno;

                if (usuarioDB.FormatoClave.Trim() == FormatoClaveHashed)
                    clave = COEncripcion.ObtieneHash(credencial.Password);
                else
                    clave = credencial.Password;

                if (!usuarioDB.Password.Equals(clave))
                {
                    IncrementarIntentosFallidos(usuarioDB);
                    Mensaje = SEEnumMensajesSeguridad.CLAVEINVALIDA;
                }
                else
                {
                    //Valida si el usuario debe cambiar la clave
                    if (usuarioDB.DiasVencimiento < (DateTime.Now - usuarioDB.FechaUltimoCambioClave.Date).TotalDays)
                        Mensaje = SEEnumMensajesSeguridad.DEBECAMBIARCLAVE;
                    else
                        Mensaje = SEEnumMensajesSeguridad.EXITOSO;

                    //reinicia los intentos de autenticacion
                    SERepositorio.Instancia.ReiniciarIntentosAutenticacion(credencial);
                }
            }
            else if (tipoUsuario == COConstantesModulos.USUARIO_LDAP)
            {
                Mensaje = SEProveedorLDAP.Instancia.AutenticarUsuario(credencial);
            }

            return Mensaje;

        }

        /// <summary>
        /// Incrementa el numero de intentos fallidos y bloquea al usuario si es requerido
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario.</param>
        private void IncrementarIntentosFallidos(SECredencialUsuario credencial)
        {
            if (credencial.CantidadIntentosFallidos == 0)
            {
                SERepositorio.Instancia.IniciarTiempoIntentosFallidos(credencial);
                SERepositorio.Instancia.IncrementarIntentosFallidos(credencial);
            }
            else//Bloquea al usuario
                if (credencial.CantidadIntentosFallidos >= int.Parse(SERepositorio.Instancia.ConsultarParametrosSeguridad("IntentoAutenticacion")))
                {
                    //Solo se bloquea la cuenta cuando se superan los intetnos de autenticacion dentro del tiempo de intentos de autenticacion
                    TimeSpan DiferenciaFecha = DateTime.Now - credencial.InicioIntentosFallidos.Value;
                    if (DiferenciaFecha.TotalSeconds <= double.Parse(SERepositorio.Instancia.ConsultarParametrosSeguridad("TiempoIntentosAutent")))
                    {
                        SERepositorio.Instancia.IniciarTiempoIntentosFallidos(credencial);
                        SERepositorio.Instancia.BloquearUsuario(credencial);
                        throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_BLOQUEADO_INTENTOS_AUTENTICA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_BLOQUEADO_INTENTOS_AUTENTICA)));
                    }
                    else
                        SERepositorio.Instancia.ReiniciarIntentosAutenticacion(credencial);
                }
                else
                    SERepositorio.Instancia.IncrementarIntentosFallidos(credencial);
        }

        ///// <summary>
        ///// Cambia la contraseña de un usuario
        ///// </summary>
        ///// <param name="credencial">Credencial con la informacion de un usuario</param>
        //public void CambiarPassword(SECredencialUsuario credencial)
        //{
        //  SECredencialUsuario usuarioDB = SERepositorio.Instancia.AutenticarUsuario(credencial);
        //  string clave = "";
        //  if (usuarioDB.FormatoClave == FormatoClaveHashed)
        //    clave = COEncripcion.ObtieneHash(credencial.PasswordNuevo);
        //  else
        //    clave = credencial.PasswordNuevo;

        //  if (usuarioDB.PasswordAnterior == clave)
        //    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_PASSWORD_DIFERENTE.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_PASSWORD_DIFERENTE)));

        //  SERepositorio.Instancia.CambiarPassword(credencial);
        //}

        ///// <summary>
        ///// Resetea el password de una cuenta de usuario
        ///// </summary>
        ///// <param name="credencial">Credencial con la informacion de un usuario.</param>
        //public void ResetearPassword(SECredencialUsuario credencial)
        //{
        //  SERepositorio.Instancia.CambiarPassword(credencial);
        //}

        /// <summary>
        /// Desactivar una una cuanta de usuario
        /// </summary>
        /// <param name="credencial"></param>
        public void DesactivarUsuario(SECredencialUsuario credencial)
        {
            SERepositorio.Instancia.DesactivarUsuario(credencial);
        }

        /// <summary>
        /// Activa una cuenta de usuario
        /// </summary>
        /// <param name="credencial"></param>
        public void ActivarUsuario(SECredencialUsuario credencial)
        {
            SERepositorio.Instancia.ActivarUsuario(credencial);
        }

        /// <summary>
        /// Retorna los menus dependiendo del usuario
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        /// <returns>Credencial con la informacion del usuario</returns>
        public SECredencialUsuario ObtenerMenusModulosXUsuario(SECredencialUsuario credencial)
        {
            return SERepositorio.Instancia.ObtenerMenusModulosXUsuario(credencial);
        }

        /// <summary>
        /// Retorna los menus dependiendo del usuario
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        /// <returns>Credencial con la informacion del usuario</returns>
        public SECredencialUsuario ObtenerMenusModulosXUsuarioYSInformacion(SECredencialUsuario credencial, SEEnumSistemaInformacion sistemaInformacion)
        {
            return SERepositorio.Instancia.ObtenerMenusModulosXUsuarioYSInformacion(credencial, sistemaInformacion);
        }

        /// <summary>
        /// Retorna la informacion basica del usuario
        /// </summary>
        /// <param name="credencial">credencial con el nombre de usuario de la cuenta</param>
        /// <returns>Credencial con la informacion basica del usuario</returns>
        public SECredencialUsuario ObtieneInformacionBasicaUsuario(SECredencialUsuario credencial)
        {
            return SERepositorio.Instancia.ObtieneInformacionBasicaUsuario(credencial);
        }

        /// <summary>
        /// Retorna la informacion basica del usuario x Usuario (Se utiliza desde Afuera de Controller)
        /// </summary>
        /// <param name="credencial">credencial con el nombre de usuario de la cuenta</param>
        /// <returns>Credencial con la informacion basica del usuario</returns>
        public SECredencialUsuario ObtieneInformacionBasicaUsuarioXUsuario(string usuario)
        {
            return SERepositorio.Instancia.ObtieneInformacionBasicaUsuarioXUsuario(usuario);
        }


        /// <summary>
        /// Obtiene los parametros generales del fremework
        /// </summary>
        /// <param name="parametro">nombre del parametro</param>
        /// <returns>valor del parametro</returns>
        public string ObtieneParametrosFremework(string parametro)
        {
            return SERepositorio.Instancia.ConsultarParametrosSeguridad(parametro);
        }

        /// <summary>
        /// Guarda y actualiza el valor del dolar
        /// por defecto el cual se utiliza en caso de no tener
        /// acceso a internet
        /// </summary>
        /// <param name="valorDolar"></param>
        public void ActualizarValorDolarPorDefecto(string valorDolar)
        {
            SERepositorio.Instancia.ActualizarValorDolarPorDefecto(valorDolar);
        }

        public string ConsultarNombreUsuarioPorCedula(int numCedula)
        {
          return SERepositorio.Instancia.ConsultarNombreUsuarioPorCedula(numCedula);
        }

        internal List<string> ConsultarUsuariosRacol(int idRacol)
        {
          return SERepositorio.Instancia.COnsultarUsuariosRacol(idRacol);
        }
    }
}