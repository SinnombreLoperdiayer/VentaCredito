using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Seguridad.Datos.Modelo;

namespace Framework.Servidor.Seguridad.Datos
{
  /// <summary>
  /// Clase para guardar las Auditorias de la DB
  /// </summary>
  internal class SERepositorioAudit
  {
    /// <summary>
    /// Guarda Auditoria de Usuarios
    /// </summary>
    /// <param name="contexto"></param>
    internal static void MapeoAuditUsuario(EntidadesSeguridad contexto)
    {
      contexto.Audit<Usuario_SEG, UsuarioHistorico_SEG>((record, action) => new UsuarioHistorico_SEG()
      {
        USU_IdUsuario = record.Field<Usuario_SEG, string>(f => f.USU_IdUsuario),
        USU_TipoUsuario = record.Field<Usuario_SEG, string>(f => f.USU_TipoUsuario),
        USU_RequiereIdMaquina = record.Field<Usuario_SEG, bool>(f => f.USU_RequiereIdMaquina),
        USU_Comentarios = record.Field<Usuario_SEG, string>(f => f.USU_Comentarios),
        USU_FechaGrabacion = record.Field<Usuario_SEG, DateTime>(f => f.USU_FechaGrabacion),
        USU_CreadoPor = record.Field<Usuario_SEG, string>(f => f.USU_CreadoPor),
        USU_AutorizaCargaMasiva = record.Field<Usuario_SEG, bool>(f => f.USU_AutorizaCargaMasiva),
        USU_EsCajeroPpal = record.Field<Usuario_SEG, bool>(f => f.USU_EsCajeroPpal),
        USU_EsUsuarioInterno = record.Field<Usuario_SEG, bool>(f => f.USU_EsUsuarioInterno),
        USU_FechaCambio = DateTime.Now,
        USU_CambiadoPor = ControllerContext.Current.Usuario,
        USU_TipoCambio = action.ToString(),
        USU_Estado = record.Field<Usuario_SEG, string>(f => f.USU_Estado),
      }, (ph) => contexto.UsuarioHistorico_SEG.Add(ph));
    }

    /// <summary>
    /// Guarda Auditoria de Usuarios
    /// </summary>
    /// <param name="contexto"></param>
    internal static void MapeoAuditPersonaInterna(EntidadesSeguridad contexto)
    {
      contexto.Audit<PersonaInterna_PAR, PersonaInternaHistorico_PAR>((record, action) => new PersonaInternaHistorico_PAR()
      {
        PEI_IdPersonaInterna = record.Field<PersonaInterna_PAR, long>(f => f.PEI_IdPersonaInterna),
        PEI_Direccion = record.Field<PersonaInterna_PAR, string>(f => f.PEI_Direccion),
        PEI_Email = record.Field<PersonaInterna_PAR, string>(f => f.PEI_Email),
        PEI_Comentarios = record.Field<PersonaInterna_PAR, string>(f => f.PEI_Comentarios),
        PEI_Nombre = record.Field<PersonaInterna_PAR, string>(f => f.PEI_Nombre),
        PEI_PrimerApellido = record.Field<PersonaInterna_PAR, string>(f => f.PEI_PrimerApellido),
        PEI_SegundoApellido = record.Field<PersonaInterna_PAR, string>(f => f.PEI_SegundoApellido),
        PEI_CreadoPor = record.Field<PersonaInterna_PAR, string>(f => f.PEI_CreadoPor),
        PEI_FechaGrabacion = record.Field<PersonaInterna_PAR, DateTime>(f => f.PEI_FechaGrabacion),
        PEI_IdCargo = record.Field<PersonaInterna_PAR, int>(f => f.PEI_IdCargo),
        PEI_Identificacion = record.Field<PersonaInterna_PAR, string>(f => f.PEI_Identificacion),
        PEI_IdTipoIdentificacion = record.Field<PersonaInterna_PAR, string>(f => f.PEI_IdTipoIdentificacion),
        PEI_IdRegionalAdm = record.Field<PersonaInterna_PAR, long?>(f => f.PEI_IdRegionalAdm),
        PEI_Municipio = record.Field<PersonaInterna_PAR, string>(f => f.PEI_Municipio),
        PEI_Telefono = record.Field<PersonaInterna_PAR, string>(f => f.PEI_Telefono),
        PEI_FechaCambio = DateTime.Now,
        PEI_CambiadoPor = ControllerContext.Current.Usuario,
        PEI_TipoCambio = action.ToString(),
      }, (ph) => contexto.PersonaInternaHistorico_PAR.Add(ph));
    }

    /// <summary>
    /// Guarda la Auditoria de los cambios de contraseña en la DB
    /// </summary>
    /// <param name="contexto"></param>
    internal static void MapeoAuditCambiarPassword(EntidadesSeguridad contexto, string usuario)
    {
      contexto.Audit<CredencialUsuario_SEG, CredencialUsuarioHistorico_SEG>((record, action) => new CredencialUsuarioHistorico_SEG()
        {
          CRU_CambiadoPor = usuario,
          CRU_CantIntentosFallidosClave = record.Field<CredencialUsuario_SEG, Int16>(f => f.CRU_CantIntentosFallidosClave),
          CRU_Clave = record.Field<CredencialUsuario_SEG, string>(f => f.CRU_Clave),
          CRU_ClaveAnterior = record.Field<CredencialUsuario_SEG, string>(f => f.CRU_ClaveAnterior),
          CRU_ClaveBloqueada = record.Field<CredencialUsuario_SEG, bool>(f => f.CRU_ClaveBloqueada),
          CRU_InicioIntentosFallidos = record.Field<CredencialUsuario_SEG, DateTime?>(f => f.CRU_InicioIntentosFallidos),
          CRU_DiasVencimiento = record.Field<CredencialUsuario_SEG, int>(f => f.CRU_DiasVencimiento),
          CRU_CreadoPor = record.Field<CredencialUsuario_SEG, string>(f => f.CUR_CreadoPor),
          CRU_FechaUltimoBloqueoClave = record.Field<CredencialUsuario_SEG, DateTime?>(f => f.CRU_FechaUltimoBloqueoClave),
          CRU_FechaUltimoCambioClave = record.Field<CredencialUsuario_SEG, DateTime>(f => f.CRU_FechaUltimoCambioClave),
          CRU_FechaGrabacion = record.Field<CredencialUsuario_SEG, DateTime>(f => f.CRU_FechaGrabacion),
          CRU_FormatoClave = record.Field<CredencialUsuario_SEG, string>(f => f.CRU_FormatoClave),
          CRU_FechaCambio = DateTime.Now,
          CRU_IdCodigoUsuario = record.Field<CredencialUsuario_SEG, long>(f => f.CRU_IdCodigoUsuario),
          CRU_TipoCambio = action.ToString()
        }, (ph) => contexto.CredencialUsuarioHistorico_SEG.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de los cambios de centro logístico por usuario
    /// </summary>
    /// <param name="contexto"></param>
    internal static void MapeoAuditUsuarioCentroLogistico(EntidadesSeguridad contexto)
    {
      //contexto.Audit<PersonaIntCentroLogistico_SEG, PersonaIntCentroLogisHist_SEG>((record, action) => new PersonaIntCentroLogisHist_SEG()
      //  {
      //    PIC_IdPersonaInterna = record.Field<PersonaIntCentroLogistico_SEG, long>(f => f.PIC_IdPersonaInterna),
      //    PIC_IdCentroLogistico = record.Field<PersonaIntCentroLogistico_SEG, long>(f => f.PIC_IdCentroLogistico),
      //    PIC_FechaGrabacion = record.Field<PersonaIntCentroLogistico_SEG, DateTime>(f => f.PIC_FechaGrabacion),
      //    PIC_CreadoPor = record.Field<PersonaIntCentroLogistico_SEG, string>(f => f.PIC_CreadoPor),
      //    PIC_FechaCambio = DateTime.Now,
      //    PIC_CambiadoPor = ControllerContext.Current.Usuario,
      //    PIC_TipoCambio = action.ToString()
      //  }, (ph) => contexto.PersonaIntCentroLogisHist_SEG.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de la eliminación de roles por usuario
    /// </summary>
    /// <param name="contexto"></param>
    internal static void MapeoAuditRolUsuario(EntidadesSeguridad contexto)
    {
      contexto.Audit<UsuarioRol_SEG, UsuarioRolHistorico_SEG>((record, action) => new UsuarioRolHistorico_SEG()
      {
        USR_IdCodigoUsuario = record.Field<UsuarioRol_SEG, long>(f => f.USR_IdCodigoUsuario),
        USR_IdRol = record.Field<UsuarioRol_SEG, string>(f => f.USR_IdRol),
        USR_FechaGrabacion = record.Field<UsuarioRol_SEG, DateTime>(f => f.USR_FechaGrabacion),
        USR_CreadoPor = record.Field<UsuarioRol_SEG, string>(f => f.USR_CreadoPor),
        USR_FechaCambio = DateTime.Now,
        USR_CambiadoPor = ControllerContext.Current.Usuario,
        USR_TipoCambio = action.ToString()
      }, (ph) => contexto.UsuarioRolHistorico_SEG.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de los cambios de estado de maquina
    /// </summary>
    /// <param name="contexto"></param>
    internal static void MapeoAuditMaquinaEstado(EntidadesSeguridad contexto)
    {
      contexto.Audit<MaqVersion_VER, MaqVersionHistorico_VER>((record, action) => new MaqVersionHistorico_VER()
      {
        MAV_CambiadoPor = ControllerContext.Current.Usuario,
        MAV_Estado = record.Field<MaqVersion_VER, string>(f => f.MAV_Estado),
        MAV_CreadoPor = record.Field<MaqVersion_VER, string>(f => f.MAV_CreadoPor),
        MAV_FechaCambio = DateTime.Now,
        MAV_FechaGrabacion = record.Field<MaqVersion_VER, DateTime>(f => f.MAV_FechaGrabacion),
        MAV_IdVersion = record.Field<MaqVersion_VER, string>(f => f.MAV_IdVersion),
        MAV_MaquinaId = record.Field<MaqVersion_VER, string>(f => f.MAV_MaquinaId),
        MAV_MaquinaVersionId = record.Field<MaqVersion_VER, int>(f => f.MAV_MaquinaVersionId),
        MAV_TipoCambio = action.ToString()
      }, (ph) => contexto.MaqVersionHistorico_VER.Add(ph));
    }

    internal static void MapeoAuditUsuarioPersonaInterna(EntidadesSeguridad contexto)
    {
      contexto.Audit<UsuarioPersonaInterna_SEG, UsuarioPersonaInternaHist_SEG>((record, action) => new UsuarioPersonaInternaHist_SEG()
      {
        USP_CambiadoPor = ControllerContext.Current.Usuario,
        USP_CreadoPor = record.Field<UsuarioPersonaInterna_SEG, string>(f => f.USP_CreadoPor),
        USP_FechaGrabacion = record.Field<UsuarioPersonaInterna_SEG, DateTime>(f => f.USP_FechaGrabacion),
        USP_TipoCambio = action.ToString(),
        USP_IdCodigoUsuario = record.Field<UsuarioPersonaInterna_SEG, long>(f => f.USP_IdCodigoUsuario),
        USP_IdPersonaInterna = record.Field<UsuarioPersonaInterna_SEG, long>(f => f.USP_IdPersonaInterna)
      }, (ph) => contexto.UsuarioPersonaInternaHist_SEG.Add(ph));
    }
  }
}