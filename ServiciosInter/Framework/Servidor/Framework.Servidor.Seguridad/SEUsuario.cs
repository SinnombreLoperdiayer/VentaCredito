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
    internal class SEUsuario : ControllerBase
    {
        #region Instancia Singleton

        private static readonly SEUsuario instancia = (SEUsuario)FabricaInterceptores.GetProxy(new SEUsuario(), ConstantesFramework.MODULO_FW_SEGURIDAD);

        /// <summary>
        /// Retorna la instancia de la clase de seguridad
        /// </summary>
        public static SEUsuario Instancia
        {
            get { return SEUsuario.instancia; }
        }

        private SEUsuario()
        { }

        #endregion Instancia Singleton

        /// <summary>
        /// Metodo que Gestiona las Adiciones y Modificaciones
        /// de los Usuarios, estos No se Borran Solo se Inactivan
        /// </summary>
        /// <param name="credencial"></param>
        public void GestionarAdminUsuarios1(SEAdminUsuario credencial)
        {
            SEUsuarioDC infoUsuario;

            using (TransactionScope trans = new TransactionScope())
            {
                infoUsuario = SERepositorio.Instancia.ObtenerUsuario(credencial.Usuario);

                if (credencial.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                {
                    if (SERepositorio.Instancia.ValidadUsuarioExiste(credencial.Usuario))
                    {
                        throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_YA_EXISTE.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_YA_EXISTE)));
                    }
                    if (credencial.EsUsuarioInterno)
                    {
                        if (credencial.TipoUsuario == COConstantesModulos.USUARIO_LDAP)
                        {

                            if (!ValidaUsuarioExisteEnDominio(credencial.Usuario))
                            {
                                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_LDAP_NULO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_LDAP_NULO)));
                            }
                        }
                        else
                        {
                            if (infoUsuario == null)
                            {
                                credencial.IdCodigoUsuario = SERepositorio.Instancia.AdicionarUsuarioSeg(credencial);
                                ValidarPersonaInternaParaUsuario(credencial);
                                SERepositorio.Instancia.AdicionarUsuarioPersonaInterna(credencial.IdCodigoUsuario, credencial.IdPersonaInterna);
                                if (credencial.AplicaPAM)
                                {
                                    SERepositorio.Instancia.InsertarMensajeroPam(credencial);
                                }
                            }
                            else
                            {
                                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_YA_EXISTE.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_YA_EXISTE)));
                            }
                        }
                    }
                    else
                    {
                        if (infoUsuario == null)
                        {
                            credencial.IdCodigoUsuario = SERepositorio.Instancia.AdicionarUsuarioSeg(credencial);
                            ValidarPersonaExternaParaUsuario(credencial);
                            SERepositorio.Instancia.AdicionarUsuarioPersonaExterna(credencial.IdCodigoUsuario, credencial.IdPersonaInterna);
                        }
                        else
                        {
                            throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_USUARIO_YA_EXISTE.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_YA_EXISTE)));
                        }
                        SERepositorio.Instancia.VersionarCliente(credencial);
                    }
                    SERepositorio.Instancia.GestionarCredencialUsuario(credencial);
                }

                if (credencial.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                {
                    SERepositorio.Instancia.GestionarCredencialUsuario(credencial);
                    SERepositorio.Instancia.ResetearPassword(credencial);
                    SERepositorio.Instancia.EditarUsuario(credencial);
                }
                CrearUsuarioGestionesSucursales(credencial);
                trans.Complete();
            }
        }


        public void GestionarAdminUsuarios(SEAdminUsuario credencial)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                //1 gestionar si es adicion o modificacion
                if (credencial.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                {
                    //2 validar que no exista el usuario
                    if (SERepositorio.Instancia.ValidadUsuarioExiste(credencial.Usuario))
                    {
                        throw new FaultException<ControllerException>
                            (new ControllerException(ConstantesFramework.MODULO_FRAMEWORK,
                            ETipoErrorFramework.EX_USUARIO_YA_EXISTE.ToString(),
                            MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_YA_EXISTE)));
                    }

                    //3 verificar si es usuario de dominio
                    if (credencial.TipoUsuario == COConstantesModulos.USUARIO_LDAP)
                    {
                        //4 verificar que exista el usuario en el dominio
                        if (!ValidaUsuarioExisteEnDominio(credencial.Usuario))
                        {
                            throw new FaultException<ControllerException>
                                (new ControllerException
                                (ConstantesFramework.MODULO_FRAMEWORK,
                                ETipoErrorFramework.EX_USUARIO_LDAP_NULO.ToString(),
                                MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_LDAP_NULO)));
                        }
                    }

                    if (credencial.EsUsuarioInterno)
                    {
                        //5 crea el usuario
                        credencial.IdCodigoUsuario = SERepositorio.Instancia.AdicionarUsuarioSeg(credencial);
                        //6 gestiona persona interna
                        ValidarPersonaInternaParaUsuario(credencial);
                        //7 adiciona usuario persona interna
                        SERepositorio.Instancia.AdicionarUsuarioPersonaInterna(credencial.IdCodigoUsuario, credencial.IdPersonaInterna);
                        if (credencial.AplicaPAM)
                        {
                            SERepositorio.Instancia.InsertarMensajeroPam(credencial);
                        }
                    }
                    else
                    {
                        //8 adiciona usuario externo
                        credencial.IdCodigoUsuario = SERepositorio.Instancia.AdicionarUsuarioSeg(credencial);
                        //9 gestiona persona externa
                        ValidarPersonaExternaParaUsuario(credencial);
                        //10 adiciona usuario persona externa
                        SERepositorio.Instancia.AdicionarUsuarioPersonaExterna(credencial.IdCodigoUsuario, credencial.IdPersonaInterna);
                        //11 versiona cliente 
                        SERepositorio.Instancia.VersionarCliente(credencial);
                    }
                }
                else if (credencial.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                {
                    //12 edita usuario
                    SERepositorio.Instancia.EditarUsuario(credencial);
                    SERepositorio.Instancia.ResetearPassword(credencial);
                }

                //13 gestiona credencial 
                SERepositorio.Instancia.GestionarCredencialUsuario(credencial);
                //14 gestionan localizaciones
                CrearUsuarioGestionesSucursales(credencial);
                //15 cierra transaccion
                trans.Complete();

            }
        }

        /// <summary>
        /// Valida si existe la Persona Interna,
        /// si existe Actualiza los Datos con los enviados
        /// desde el cliente, si no existe crea a la persona Externa
        /// </summary>
        /// <param name="credencial"></param>
        private void ValidarPersonaInternaParaUsuario(SEAdminUsuario credencial)
        {
            if (SERepositorio.Instancia.ValidarPersonaInternaExiste(credencial.TipoIdentificacion, credencial.Identificacion))
            {
                //Actualizo los datos de la Persona InternaUsuario
                SERepositorio.Instancia.EditarPersonaInternaPorUsuario(credencial);
                SERepositorio.Instancia.BorrarUsuarioPersonaInternaXIdPersona(credencial.IdPersonaInterna);
            }
            else
            {
                //Creo la Persona Interna
                credencial.IdPersonaInterna = SERepositorio.Instancia.AdicionarPersonaInterna(credencial);
            }
        }

        /// <summary>
        /// Valida si existe la Persona externa,
        /// si existe Actualiza los Datos con los enviados
        /// desde el cliente, si no existe crea a la persona Externa
        /// </summary>
        /// <param name="credencial"></param>
        private void ValidarPersonaExternaParaUsuario(SEAdminUsuario credencial)
        {
            if (SERepositorio.Instancia.ValidarPersonaExternaExiste(credencial.TipoIdentificacion, credencial.Identificacion))
            {
                //Actualizo los datos de la Persona Interna
                SERepositorio.Instancia.EditarPersonaExternaPorUsuario(credencial);
            }
            else
            {
                //Creo la Persona Interna
                credencial.IdPersonaInterna = SERepositorio.Instancia.AdicionarPersonaExterna(credencial);
            }
        }

        /// <summary>
        /// Creacion del Usuario con todos los
        /// permisos y asociaciones
        /// </summary>
        /// <param name="credencial"></param>
        private long AdicionarUsuario(SEAdminUsuario credencial)
        {
            //creo el usuario en tabla
            return credencial.IdCodigoUsuario = SERepositorio.Instancia.AdicionarUsuarioSeg(credencial);
        }

        /// <summary>
        /// Crea el usurio con las
        /// gestiones - Sucursales - Centros Servicios
        /// </summary>
        /// <param name="credencial"></param>
        private void CrearUsuarioGestionesSucursales(SEAdminUsuario credencial)
        {
            SERepositorio.Instancia.DesautorizarCentrosServicio(credencial.IdCodigoUsuario);
            if (credencial.CentrosDeServicioAutorizados != null)
            {
                foreach (SECentroServicio centroServicio in credencial.CentrosDeServicioAutorizados)
                {
                    SERepositorio.Instancia.AutorizarCentroServicio(centroServicio, credencial.IdCodigoUsuario);
                }
            }

            SERepositorio.Instancia.DesautorizarGestiones(credencial.IdCodigoUsuario);
            if (credencial.GestionesAutorizadas != null)
            {
                foreach (SEGestion gestion in credencial.GestionesAutorizadas)
                {
                    SERepositorio.Instancia.AutorizarGestion(gestion, credencial.IdCodigoUsuario);
                }
            }

            SERepositorio.Instancia.DesautorizarSucursales(credencial.IdCodigoUsuario);
            if (credencial.SucursalesAutorizadas != null)
            {
                foreach (SESucursal sucursal in credencial.SucursalesAutorizadas)
                {
                    SERepositorio.Instancia.AutorizarSucursal(sucursal, credencial.IdCodigoUsuario);
                }
                if (credencial.SucursalesAutorizadas.Count > 0)
                {
                    credencial.EsUsuarioInterno = false;
                }
                else
                {
                    credencial.EsUsuarioInterno = true;
                }
            }
            else { credencial.EsUsuarioInterno = true; }
        }

        /// <summary>
        /// Edicion de Borrado de Registros Relacionados
        /// por usuario y persona interna
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <param name="personaInterna"></param>
        private void EditarUsuarioPersonaInterna(long idCodigoUsuario, long personaInterna)
        {
            //Elimino registro por el usuario de la Relacion con la anterior persona Interna
            SERepositorio.Instancia.BorrarUsuarioPersonaInternaXUsuario(idCodigoUsuario);

            //Elimino Registro por PersonaInterna de la Relacion por si tenia un usuario INA relacionado
            SERepositorio.Instancia.BorrarUsuarioPersonaInternaXIdPersona(personaInterna);

            //Adiciono Nueva Relacion
            SERepositorio.Instancia.AdicionarUsuarioPersonaInterna(idCodigoUsuario, personaInterna);

            //si existe usuario persona Externa la Borro
            SERepositorio.Instancia.BorrarUsuarioPersonaExternaXIdPersona(personaInterna);
        }

        /// <summary>
        /// Edicion de Borrado de Registros Relacionados
        /// por usuario y persona interna
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <param name="personaInterna"></param>
        private void EditarUsuarioPersonaExterna(long idCodigoUsuario, long personaInterna)
        {
            //Elimino registro por el usuario de la Relacion con la anterior persona Interna
            SERepositorio.Instancia.BorrarUsuarioPersonaExternaXUsuario(idCodigoUsuario);

            //Elimino Registro por PersonaInterna de la Relacion por si tenia un usuario INA relacionado
            SERepositorio.Instancia.BorrarUsuarioPersonaExternaXIdPersona(personaInterna);

            //si existe usuario persona Interna la Borro
            SERepositorio.Instancia.BorrarUsuarioPersonaInternaXUsuario(idCodigoUsuario);

            //si existe usuario persona Interna la Borro
            SERepositorio.Instancia.BorrarUsuarioPersonaInternaXIdPersona(personaInterna);

            //Adiciono Nueva Relacion
            SERepositorio.Instancia.AdicionarUsuarioPersonaExterna(idCodigoUsuario, personaInterna);
        }

        public bool ValidaUsuarioExisteEnDominio(string credencial)
        {
            string cadenaConexion = WebConfigurationManager.ConnectionStrings["LDAPConnection"].ConnectionString;
            cadenaConexion = cadenaConexion.Substring(7, cadenaConexion.Length - 7);
            string servidor = string.Empty;
            string oufolder = string.Empty;

            try
            {
                servidor = cadenaConexion.Split('/')[0];
                oufolder = cadenaConexion.Split('/')[1].Split(',')[1];
            }
            catch (Exception)
            {
                throw new Exception("Cadena de conexión LDAP no está formada correctamente.");
            }

            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, servidor))
            {
                UserPrincipal up = UserPrincipal.FindByIdentity(
                    pc,
                    IdentityType.SamAccountName,
                    credencial);

                return (up != null);
            }

        }

    }
}