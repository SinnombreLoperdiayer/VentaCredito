using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;

namespace Framework.Servidor.Seguridad
{
    /// <summary>
    /// Clase que controla el proveedor para la autenticacion
    /// </summary>
    public class SEProveedor : ControllerBase
    {
        private static readonly SEProveedor instancia = (SEProveedor)FabricaInterceptores.GetProxy(new SEProveedor(), ConstantesFramework.MODULO_FW_SEGURIDAD);

        /// <summary>
        /// Retorna la instancia de la clase de seguridad
        /// </summary>
        public static SEProveedor Instancia
        {
            get { return SEProveedor.instancia; }
        }

        private SEProveedor()
        { }

        /// <summary>
        /// Autentica el usuario
        /// </summary>
        /// <param name="credencial">Credencial con el nombre de usuario</param>
        /// <returns>Credencial con la informacion basica del usuario y los menus y modulos a los que tiene acceso</returns>
        public SECredencialUsuario AutenticarUsuario(SECredencialUsuario credencial)
        {
            credencial.Password = armarPassword(credencial.Usuario, credencial.Password);

            string idMaq = credencial.IdentificadorMaquina;
            int idAppOrigen = credencial.IdAplicativoOrigen;
            credencial= ValidarCredencialesUsuario(credencial);

            credencial.IdentificadorMaquina = idMaq;
            credencial.IdAplicativoOrigen = idAppOrigen;
            SESesionUsuario.Instancia.CrearSesionUsuario(credencial);

            return SEProvedorSql.Instancia.ObtenerMenusModulosXUsuario(credencial);
        }

        private string armarPassword(string usuario,string pass)
        {
            string archivo = "jkh978y3njdbfasd6CXXC % &% 8klj76jhkjhgk &&)/ KHJBKbn89jhoh/" + usuario;
            byte[] encryptedBytes = Convert.FromBase64String(pass);
            byte[] saltBytes = Encoding.UTF8.GetBytes(archivo);
            string decryptedString = string.Empty;
            using (var aes = new AesManaged())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(archivo, saltBytes);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform decryptTransform = aes.CreateDecryptor())
                {
                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        CryptoStream decryptor =
                            new CryptoStream(decryptedStream, decryptTransform, CryptoStreamMode.Write);
                        decryptor.Write(encryptedBytes, 0, encryptedBytes.Length);
                        decryptor.Flush();
                        decryptor.Close();

                        byte[] decryptBytes = decryptedStream.ToArray();
                        decryptedString =
                            UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                    }
                }
            }
            string passDecryp;
            decryptedString = decryptedString.Replace("/SLASH", "©");
            var arr = decryptedString.Split('©');

            if (arr.Length == 4)
            {
                passDecryp = arr[3];
            }
            else
            {
                throw new Exception("error validando credenciales");
            }


            return passDecryp;
        }

        // todo:id
        /// <summary>
        /// Autentica el usuario desde otra aplicacion Externa a Controller sin traer Menus
        /// </summary>
        /// <param name="credencial">Credencial con el nombre de usuario</param>
        /// <returns>Credencial con la informacion basica del usuario</returns>
        public SECredencialUsuario AutenticarUsuarioDesdeFueradeController(SECredencialUsuario credencial)
        {
            credencial = ValidarCredencialesUsuarioDesdeFueradeController(credencial);
            return credencial;
        }
        private SECredencialUsuario ValidarCredencialesUsuarioDesdeFueradeController(SECredencialUsuario credencial)
        {
            SEEnumMensajesSeguridad Mensaje;
            Mensaje = SEProvedorSql.Instancia.AutenticarUsuario(credencial);
            switch (Mensaje)
            {
                case SEEnumMensajesSeguridad.USUARIONOEXISTE:
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO)));

                case SEEnumMensajesSeguridad.CLAVEINVALIDA:
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO)));

                case SEEnumMensajesSeguridad.BLOQUEADO:
                    {
                        credencial.CambiarClave = true;
                        throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_BLOQUEADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_BLOQUEADO)));
                    }
            }

            credencial = SEProvedorSql.Instancia.ObtieneInformacionBasicaUsuarioXUsuario(credencial.Usuario);  ///////

            return credencial;
        }

        public SECredencialUsuario ColsultarUsuarioDesdeFueradeController(string nomUsuario)
        {
            return SEProvedorSql.Instancia.ObtieneInformacionBasicaUsuarioXUsuario(nomUsuario); 
        }

        public string ConsultarNombreUsuarioPorCedula(int numCedula)
        {
          return SEProvedorSql.Instancia.ConsultarNombreUsuarioPorCedula(numCedula);
        }

        public SECredencialUsuario ObtenerMenusModulosXUsuarioYSInformacion(SECredencialUsuario credencial,SEEnumSistemaInformacion sistemaInformacion)
        {
            credencial= ValidarCredencialesUsuario(credencial);
            return SEProvedorSql.Instancia.ObtenerMenusModulosXUsuarioYSInformacion(credencial,sistemaInformacion);
        }

        private SECredencialUsuario ValidarCredencialesUsuario(SECredencialUsuario credencial)
        {
            SEEnumMensajesSeguridad Mensaje;
            Mensaje = SEProvedorSql.Instancia.AutenticarUsuario(credencial); ////////////
            switch (Mensaje)
            {
                case SEEnumMensajesSeguridad.USUARIONOEXISTE:
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO)));

                case SEEnumMensajesSeguridad.CLAVEINVALIDA:
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO)));

                case SEEnumMensajesSeguridad.BLOQUEADO:
                    {
                        credencial.CambiarClave = true;
                        throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_BLOQUEADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_BLOQUEADO)));
                    }
            }

            credencial = SEProvedorSql.Instancia.ObtieneInformacionBasicaUsuario(credencial);  ///////

            credencial.UserServicios = AlgoritmoEncripcion.Instance.DesencriptarCadena(SEProvedorSql.Instancia.ObtieneParametrosFremework("UserWCF"));
            credencial.PassServicios = AlgoritmoEncripcion.Instance.DesencriptarCadena(SEProvedorSql.Instancia.ObtieneParametrosFremework("PassWCF"));

            if (DiccionarioCredenciales.instancia.ContainsKey(credencial.UserServicios + "-" + credencial.PassServicios))
            {
                DiccionarioCredenciales.instancia.Remove(credencial.UserServicios + "-" + credencial.PassServicios);
            }
            DiccionarioCredenciales.instancia.Add(credencial.UserServicios + "-" + credencial.PassServicios, DateTime.Now);

            if (Mensaje == SEEnumMensajesSeguridad.DEBECAMBIARCLAVE)
                credencial.CambiarClave = true;
            else
                credencial.CambiarClave = false;

            return credencial;
        }

        /// <summary>
        /// Autentica las credenciales de los servicios
        /// </summary>
        /// <param name="user">usuario</param>
        /// <param name="password">password</param>
        /// <returns></returns>
        public bool ValidarCredencialesWCF(string user, string password)
        {
            lock (this)
            {
                if (DiccionarioCredenciales.instancia.Count <= 0)
                {
                    string usuario = AlgoritmoEncripcion.Instance.DesencriptarCadena(SEProvedorSql.Instancia.ObtieneParametrosFremework("UserWCF"));
                    string clave = AlgoritmoEncripcion.Instance.DesencriptarCadena(SEProvedorSql.Instancia.ObtieneParametrosFremework("PassWCF"));

                    DiccionarioCredenciales.instancia.Add(usuario + "-" + clave, DateTime.Now);
                }

                return DiccionarioCredenciales.instancia.ContainsKey(user + "-" + password);
            }
        }

        /// <summary>
        /// Obtiene los parametros generales del fremework
        /// </summary>
        /// <param name="parametro">nombre del parametro</param>
        /// <returns>valor del parametro</returns>
        public string ObtieneParametrosFremework(string parametro)
        {
            return SEProvedorSql.Instancia.ObtieneParametrosFremework(parametro);
        }

        /// <summary>
        /// Guarda y actualiza el valor del dolar
        /// por defecto el cual se utiliza en caso de no tener
        /// acceso a internet
        /// </summary>
        /// <param name="valorDolar">valor del dolar a guardar</param>
        public void ActualizarValorDolarPorDefecto(string valorDolar)
        {
            SEProvedorSql.Instancia.ActualizarValorDolarPorDefecto(valorDolar);
        }

        public List<string> ConsultarUsuariosRacol(int idRacol)
        {
          return SEProvedorSql.Instancia.ConsultarUsuariosRacol(idRacol);
        }
    }
}