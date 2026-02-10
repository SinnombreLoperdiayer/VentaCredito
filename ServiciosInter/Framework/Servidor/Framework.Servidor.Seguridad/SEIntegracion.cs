using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Seguridad.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using System.Web.Configuration;
namespace Framework.Servidor.Seguridad
{
    internal class SEIntegracion : ControllerBase
    {
        #region Instancia Singleton

        private static readonly SEIntegracion instancia = (SEIntegracion)FabricaInterceptores.GetProxy(new SEIntegracion(), ConstantesFramework.MODULO_FW_SEGURIDAD);

        /// <summary>
        /// Retorna la instancia de la clase de seguridad
        /// </summary>
        public static SEIntegracion Instancia
        {
            get { return SEIntegracion.instancia; }
        }

        private SEIntegracion()
        { }

        #endregion Instancia Singleton


        #region CRUD USUARIO INTEGRACION
        /// <summary>
        /// Metodo que Inserta un usuario Integracion validando si el usuario que se desea asignar existe
        /// </summary>
        /// <param name="Usuario"></param>
        /// <returns></returns>
        public SERespuestaProceso CrearUsuarioIntegracion(SEUsuarioIntegracionDC Usuario)
        {
            SERespuestaProceso respuestaProceso = new SERespuestaProceso();
            try
            {
                Usuario.IdCliente = Usuario.idClienteInt.ToString();
                if (string.IsNullOrEmpty(Usuario.Usuario) || Usuario.IdCliente == "0" || string.IsNullOrEmpty(Usuario.IdCliente))
                {
                    respuestaProceso.valor = 0;
                    respuestaProceso.Mensaje = "Falta Informacion Relacionada al usuario";
                }
                else
                {
                    int idUsuarioExistente = SERepositorio.Instancia.ConsultarUsuarioExisteIntegracion(Usuario.Usuario);
                    if (idUsuarioExistente != 0)
                    {
                        respuestaProceso.valor = 0;
                        respuestaProceso.Mensaje = "El usuario que desea crear ya existe";
                    }
                    else
                    {
                        Usuario.Contrasena = COEncripcion.ObtieneHash(Usuario.Contrasena);
                        SERepositorio.Instancia.InsertarUsuarioIntegracion(Usuario);
                        respuestaProceso.Mensaje = "usuario creado con Exito";
                        respuestaProceso.valor = Usuario.IdUsuario;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaProceso.valor = 0;
                respuestaProceso.Mensaje = "Error al insertar el usuario" + ex;
            }
            return respuestaProceso;
        }
        /// <summary>
        /// Edita un usuario de Integracion validando si el usuario que desea asignar existe y si la contraseña a cambiado 
        /// </summary>
        /// <param name="Usuario"></param>
        /// <returns></returns>
        public SERespuestaProceso EditarUsuarioIntegracion(SEUsuarioIntegracionDC Usuario)
        {
            SERespuestaProceso respuestaProceso = new SERespuestaProceso();
            try
            {
                if (string.IsNullOrEmpty(Usuario.Usuario) || Usuario.IdCliente == "0" || string.IsNullOrEmpty(Usuario.IdCliente))
                {
                    respuestaProceso.valor = 0;
                    respuestaProceso.Mensaje = "Falta Informacion Relacionada al usuario";
                }
                else
                {
                    int idUsuarioExistente = SERepositorio.Instancia.ConsultarUsuarioExisteIntegracion(Usuario.Usuario);
                    if (idUsuarioExistente != 0 && Usuario.IdUsuario != idUsuarioExistente)
                    {
                        respuestaProceso.valor = 0;
                        respuestaProceso.Mensaje = "El usuario que desea asignar ya existe";
                    }
                    else
                    {
                        SEUsuarioIntegracionDC UsuarioConsulta = (from d in ObtenerUsuariosIntegracion(0) where Usuario.IdUsuario == d.IdUsuario select d).First();
                        if (Usuario.Contrasena != UsuarioConsulta.Contrasena)
                        {
                            Usuario.Contrasena = COEncripcion.ObtieneHash(Usuario.Contrasena);
                        }
                        SERepositorio.Instancia.EditarUsuarioIntegracion(Usuario);
                        respuestaProceso.Mensaje = "Cambio Realizado con Exito";
                        respuestaProceso.valor = Usuario.IdUsuario;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaProceso.valor = 0;
                respuestaProceso.Mensaje = "Error al insertar el usuario" + ex;
            }
            return respuestaProceso;
        }
        /// <summary>
        /// Cambia el estado de un usuario dejandolo deshabilitado 
        /// </summary>
        /// <param name="Usuario"></param>
        /// <returns></returns>
        public SERespuestaProceso DeshabilitarUsuarioIntegracion(SEUsuarioIntegracionDC Usuario)
        {
            SERespuestaProceso respuestaProceso = new SERespuestaProceso();
            try
            {
                respuestaProceso.valor = SERepositorio.Instancia.EliminarUsuarioIntegracion(Usuario);
                respuestaProceso.Mensaje = "Usuario deshabilitado con exito";
            }
            catch (Exception ex)
            {

                respuestaProceso.valor = 0;
                respuestaProceso.Mensaje = "Ocurrio un error al deshabilitar el usuario" + ex;
            }
            return respuestaProceso;
        }
        /// <summary>
        /// Consulta un usuario integracion validando el usuario y la contraseña
        /// </summary>
        /// <param name="Usuario"></param>
        /// <returns></returns>
        public SERespuestaProceso ConsultarUsuarioIntegracion(SEUsuarioIntegracionDC Usuario)
        {
            SERespuestaProceso respuestaProceso = new SERespuestaProceso();
            try
            {
                respuestaProceso.valor = SERepositorio.Instancia.ConsultarUsuarioIntegracion(Usuario);
                if (respuestaProceso.valor != 0)
                {
                    respuestaProceso.Mensaje = "Usuario con datos correctos";

                }
                else
                {
                    respuestaProceso.Mensaje = "Usuario con datos incorrectos";
                }
            }
            catch (Exception ex)
            {

                respuestaProceso.valor = 0;
                respuestaProceso.Mensaje = "Ocurrio un error al consultar el usuario" + ex;
            }
            return respuestaProceso;
        }
        /// <summary>
        /// Obtiene el listado de usuarios activos de integracion si se desea filtar por un cliente se hace el filtro correspondiente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public List<SEUsuarioIntegracionDC> ObtenerUsuariosIntegracion(int idCliente)
        {
            if (idCliente != 0)
            {
                List<SEUsuarioIntegracionDC> Usuarios = SERepositorio.Instancia.ConsultarUsuariosActivosIntegracion();
                Usuarios = (from d in Usuarios where Convert.ToInt32(d.IdCliente) == idCliente select d).ToList();
                return Usuarios;
            }
            else
            {
                List<SEUsuarioIntegracionDC> Usuarios = SERepositorio.Instancia.ConsultarUsuariosActivosIntegracion();
                return Usuarios;
            }

        }

        #endregion
    }
}

