using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Seguridad.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace Framework.Servidor.Seguridad
{
    /// <summary>
    /// Clase que hace consultas de seguridad a la base de datos
    /// </summary>
    internal class SEConsultas : ControllerBase
    {
        #region Instancia Singleton

        private static readonly SEConsultas instancia = (SEConsultas)FabricaInterceptores.GetProxy(new SEConsultas(), ConstantesFramework.MODULO_FW_SEGURIDAD);

        /// <summary>
        /// Retorna la instancia de la clase de seguridad
        /// </summary>
        public static SEConsultas Instancia
        {
            get { return SEConsultas.instancia; }
        }

        private SEConsultas()
        { }

        #endregion Instancia Singleton

        #region Métodos

        /// <summary>
        /// Retorna los tipos de autenticación
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SETipoAutenticacion> ObtenerTiposAutenticacion()
        {
            return SERepositorio.Instancia.ObtenerTiposAutenticacion();
        }

        /// <summary>
        /// Obtiene a los Cajeros Auxiliares de un Centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<SEUsuarioCentroServicioDC> ObtenerCajeroCentroServicio(long idCentroServicio, string idRol)
        {
            return SERepositorio.Instancia.ObtenerCajeroCentroServicio(idCentroServicio, idRol);
        }

        /// <summary>
        /// Consulta las cajas con los usuarios de un punto trayendo tambien
        ///	las cajas no utilizadas
        /// </summary>
        /// <param name="idCentroSvc">id punto centro servicio</param>
        /// <returns>lista de cajas del punto centro servicio</returns>
        public List<SEUsuarioCentroServicioDC> ObtenerCajerosCajaPorPunto(long idCentroSvc)
        {
            return SERepositorio.Instancia.ObtenerCajerosCajaPorPunto(idCentroSvc);
        }

        /// <summary>
        /// Obtiene los usuarios internos activos
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SEAdminUsuario> ObtenerUsuariosInternosActivos()
        {
            return SERepositorio.Instancia.ObtenerUsuariosInternosActivos();
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el codigo del
        /// usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario)
        {
            return SERepositorio.Instancia.ObtenerUsuarioPorCodigo(idCodigoUsuario);
        }

        /// <summary>
        /// Valida la persona interna si
        /// tiene usuario asignado y si no existe
        /// consulto el usuario si esta activo
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns>Mensajes de Error</returns>
        public SEUsuarioPorCodigoDC ValidarUsuario(string idUsuario, string identificacion)
        {
            SEPersonaInternaDC personaInterna = ObtenerPersonaInternaPorIdentificacion(identificacion);
            SEUsuarioPorCodigoDC validoUsuario = SERepositorio.Instancia.ObtenerUsuarioPorIdUsuarioData(idUsuario);

            //Consulto si existe la persona Interna por Identificacion
            if (personaInterna != null && !string.IsNullOrWhiteSpace(personaInterna.Identificacion))
            {
                //consulto si existe un usuario Activo asignado a la persona Interna
                SEUsuarioPorCodigoDC usuario = SERepositorio.Instancia.ObtenerUsuarioPersonaInterna(personaInterna.IdPersonaInterna);

                //valido si es diferente el usuario consultado a de la persona Digitada
                if (usuario != null && usuario.Usuario != idUsuario)
                {
                    if (usuario.IdCodigoUsuario > 1 && usuario.EstadoUsuario == ConstantesFramework.ESTADO_ACTIVO)
                    {
                        //Mensaje de error porque la persona interna tiene un Usuario Activo
                        ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD,
                            ETipoErrorFramework.EX_USUARIO_ACTIVO_DE_PERSONA_INTERNA.ToString(),
                            string.Format(MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_ACTIVO_DE_PERSONA_INTERNA),
                            personaInterna.NombreCompleto, personaInterna.Identificacion, usuario.Usuario));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
            }
            else
            {
                //Consulto la Persona Externa
                SEPersonaInternaDC personaExterna = ObtenerPersonaExternaPorIdentificacion(identificacion);

                //Valido si existe Persona Externa
                if (personaExterna != null && !string.IsNullOrWhiteSpace(personaExterna.Identificacion))
                {
                    //consulto si existe un usuario Activo asignado a la persona Externa
                    SEUsuarioPorCodigoDC usuario = SERepositorio.Instancia.ObtenerUsuarioPersonaExterna(personaExterna.IdPersonaInterna);

                    //valido si es diferente el usuario consultado a de la persona Digitada
                    if (usuario != null && usuario.Usuario != idUsuario)
                    {
                        if (usuario.IdCodigoUsuario > 1 && usuario.EstadoUsuario == ConstantesFramework.ESTADO_ACTIVO)
                        {
                            //Mensaje de error porque la persona interna tiene un Usuario Activo
                            ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD,
                                ETipoErrorFramework.EX_USUARIO_ACTIVO_DE_PERSONA_INTERNA.ToString(),
                                string.Format(MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_ACTIVO_DE_PERSONA_INTERNA),
                                personaExterna.NombreCompleto, personaExterna.Identificacion, usuario.Usuario));
                            throw new FaultException<ControllerException>(excepcion);
                        }
                    }
                }
            }

        

            if (validoUsuario != null)
            {
                if (validoUsuario.Documento != identificacion)
                {
                    if (validoUsuario.EstadoUsuario == ConstantesFramework.ESTADO_ACTIVO)
                    {
                        //Mensaje de error porque el usuario digitado esta activo
                        ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD,
                        ETipoErrorFramework.EX_USUARIO_ACTIVO_PARA_UNA_PERSONA_INTERNA.ToString(),
                        string.Format(MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_ACTIVO_PARA_UNA_PERSONA_INTERNA),
                        validoUsuario.Usuario, validoUsuario.NombreCompleto, validoUsuario.Documento));

                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
            }
            else
            {
                SEUsuarioDC usuarioCreado = SERepositorio.Instancia.ObtenerUsuario(idUsuario);
                if (usuarioCreado != null && usuarioCreado.EstadoUsuario == ConstantesFramework.ESTADO_ACTIVO)
                {
                    //Mensaje de error porque el usuario digitado esta activo pero sin Relacionar
                    ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD,
                    ETipoErrorFramework.EX_USUARIO_ACTIVO_SIN_ASIGNAR.ToString(),
                    string.Format(MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_ACTIVO_SIN_ASIGNAR),
                    usuarioCreado.EstadoUsuario));

                    throw new FaultException<ControllerException>(excepcion);
                }
            }
            return validoUsuario;
        }

        /// <summary>
        /// se consulta la persona por la identificacion
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns>datos del usuario encontrado</returns>
        public SEPersonaInternaDC ObtenerPersonaInternaPorIdentificacion(string identificacion)
        {
            return SERepositorio.Instancia.ObtenerPersonaInternaPorIdentificacion(identificacion);
        }

        /// <summary>
        /// se consulta la persona por la identificacion
        /// </summary>
        /// <param name="identificacion">doc de Identificacion</param>
        /// <returns>datos del Usuario Encontrado</returns>
        public SEPersonaInternaDC ObtenerPersonaExternaPorIdentificacion(string identificacion)
        {
            return SERepositorio.Instancia.ObtenerPersonaExternaPorIdentificacion(identificacion);
        }

        #endregion Métodos
    }
}