using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using Framework.Servidor.Agenda.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;

namespace Framework.Servidor.Agenda
{
  /// <summary>
  /// Clase que contiene las operaciones de asignación de tareas
  /// </summary>
  public class ASAsignadorTarea : MarshalByRefObject, ITarea
  {
    #region Instancia Singleton

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    private static readonly ASAsignadorTarea instancia = (ASAsignadorTarea)FabricaInterceptores.GetProxy(new ASAsignadorTarea(), Framework.Servidor.Comun.ConstantesFramework.MODULO_FW_AGENDA);

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    public static ASAsignadorTarea Instancia
    {
      get { return ASAsignadorTarea.instancia; }
    }

    #endregion Instancia Singleton

    #region Métodos

    /// <summary>
    /// Hace asignación manual de una tarea manual
    /// </summary>
    /// <param name="tarea">Tarea a asignar</param>
    /// <param name="comentarios">Comentarios relacionados con la asignación</param>
    /// <param name="eventoAsignacion"></param>
    /// <param name="usuarioAplicacion">Usuario de la aplicación</param>
    /// <param name="archivos">Archivos a asignar a la tarea</param>
    /// <returns></returns>
    public void AsignarTareaManual(List<ASTarea> tareas, string comentarios, ASEEventoAsignacion eventoAsignacion, string usuarioAplicacion, List<string> archivos)
    {
      using (TransactionScope scope = new TransactionScope())
      {
        tareas.ForEach(t =>
          {
            ASRepositorio.Instancia.AsignarTareaManual(t, comentarios, eventoAsignacion, usuarioAplicacion, archivos);
          });
        scope.Complete();
      }
    }

    ///// <summary>
    ///// Hace asignación manual de muchas tareas asocidas a la misma falla. Este proceso aplica solo para fallas que no son editables, es decir que son automáticas NO manuales. Para el caso de uso de auto asignación de tareas
    ///// </summary>
    ///// <param name="idFalla">Identificador de la falla</param>
    ///// <param name="usuario">Usuario que hace la asignación</param>
    ///// <param name="comentarios">Comentarios relacionados con la asignación de la tarea</param>
    //public void AsignarTareasHilo(int idFalla, string usuario, string comentarios)
    //{
    //  try
    //  {
    //    //Debug.WriteLine("entro  a asignar tareas");
    //    if (comentarios == string.Empty)
    //    {
    //      comentarios = COUsuariosSistema.USUARIO_SISTEMA;
    //    }
    //    //Debug.WriteLine("ANTEES D OBTENER FALLA");
    //    ASFalla falla = ASRepositorio.Instancia.ObtenerFalla(idFalla);

    //    //falla = ASRepositorio.Instancia.ObtenerFalla(idFalla);
    //    Debug.WriteLine("DESPUES D OBTENER FALLA");
    //    if (falla != null && !falla.EsEditable)
    //    {
    //      if (falla.Tareas != null && falla.Tareas.Count() > 0)
    //      {
    //        // La Verificación de que el usuario esté configurado correctamente, que esté activo, exista y que tenga el cargo correspondiente a la tarea se hace dentro del método asignar tarea manual
    //        ASRepositorio.Instancia.AsignarTareas(falla, falla.Tareas, comentarios, ASEEventoAsignacion.Creacion, usuario);
    //        Debug.WriteLine("falla.tarea <> null");
    //      }

    //      else
    //      {
    //        Debug.WriteLine("falla.tarea === null");
    //        ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA));
    //        throw new FaultException<ControllerException>(excepcion);
    //      }
    //    }
    //    else
    //    {
    //      Debug.WriteLine("FALLA == NULL");
    //      ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA));
    //      throw new FaultException<ControllerException>(excepcion);
    //    }

    //    Debug.WriteLine("Termino regooood");
    //  }

    //  catch (Exception ex)
    //  {
    //    Debug.WriteLine("Error: " + ex);
    //  }
    //}

    /// <summary>
    /// Hace asignación manual de muchas tareas asocidas a la misma falla. Este proceso aplica solo para fallas que no son editables, es decir que son automáticas NO manuales. Para el caso de uso de auto asignación de tareas
    /// </summary>
    /// <param name="idFalla">Identificador de la falla</param>
    /// <param name="usuario">Usuario que hace la asignación</param>
    /// <param name="comentarios">Comentarios relacionados con la asignación de la tarea</param>
    public bool AsignarTareas(int idFalla, string usuario, string comentarios)
    {
      if (comentarios == string.Empty)
      {
        comentarios = COUsuariosSistema.USUARIO_SISTEMA;
      }
      ASFalla falla = ASRepositorio.Instancia.ObtenerFalla(idFalla);
      if (falla != null && !falla.EsEditable)
      {
        if (falla.Tareas != null && falla.Tareas.Count() > 0 && !string.IsNullOrEmpty(usuario))
        {
          if (comentarios.Length > 200)
            comentarios = comentarios.Substring(0, 200);
          // La Verificación de que el usuario esté configurado correctamente, que esté activo, exista y que tenga el cargo correspondiente a la tarea se hace dentro del método asignar tarea manual
          return ASRepositorio.Instancia.AsignarTareas(falla, falla.Tareas, comentarios, ASEEventoAsignacion.Creacion, usuario);
        }

        else
        {
          ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA));
          throw new FaultException<ControllerException>(excepcion);
        }
      }
      else
      {
        ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA));
        throw new FaultException<ControllerException>(excepcion);
      }
    }

    /// <summary>
    /// Hace asignación manual de muchas tareas asocidas a la misma falla. Este proceso aplica solo para fallas que no son editables, es decir que son automáticas NO manuales. Para el caso de uso de auto asignación de tareas
    /// </summary>
    /// <param name="idFalla">Identificador de la falla</param>
    /// <param name="usuario">Usuario que hace la asignación</param>
    /// <param name="comentarios">Comentarios relacionados con la asignación de la tarea</param>
    public bool AsignarTareasOrigen(int idFalla, string usuario, string comentarios, long idCentroServicio)
    {
      if (comentarios == string.Empty)
      {
        comentarios = COUsuariosSistema.USUARIO_SISTEMA;
      }
      ASFalla falla = ASRepositorio.Instancia.ObtenerFalla(idFalla);
      if (falla != null && !falla.EsEditable)
      {
        if (falla.Tareas != null && falla.Tareas.Count() > 0 && !string.IsNullOrEmpty(usuario))
        {
          if (comentarios.Length > 200)
            comentarios = comentarios.Substring(0, 200);

          // La Verificación de que el usuario esté configurado correctamente, que esté activo, exista y que tenga el cargo correspondiente a la tarea se hace dentro del método asignar tarea manual
          return ASRepositorio.Instancia.AsignarTareasOrigen(falla, falla.Tareas, comentarios, ASEEventoAsignacion.Creacion, usuario, idCentroServicio);
        }

        else
        {
          ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA));
          throw new FaultException<ControllerException>(excepcion);
        }
      }
      else
      {
        ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA));
        throw new FaultException<ControllerException>(excepcion);
      }
    }

    /// <summary>
    /// Hace asignación manual de muchas tareas asocidas a la misma falla. Este proceso aplica solo para fallas que no son editables, es decir que son automáticas NO manuales. Para el caso de uso de auto asignación de tareas
    /// </summary>
    /// <param name="idFalla"></param>
    /// <param name="usuario"></param>
    public bool AsignarTareas(int idFalla, string usuario)
    {
      return this.AsignarTareas(idFalla, usuario, string.Empty);
    }

    /// <summary>
    /// Hace escalamiento automático de tareas cuando el tiempo límite de resolución ha sido superado
    /// </summary>
    public void EscalarTareas()
    {
      ASRepositorio.Instancia.AutoEscalarTareas();
    }

    /// <summary>
    /// Reasignar tareas
    /// </summary>
    /// <param name="tareaAsignada"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public void ReasignarTarea(ASTareaAsignada tareaAsignada, string user)
    {
      ASRepositorio.Instancia.ReasignarTarea(tareaAsignada, user);
    }

    /// <summary>
    /// Asigna tarea manual por agenda
    /// </summary>
    /// <param name="tarea">Información de la tarea asignada</param>
    /// <param name="usuarioAplicacion">Usuario que hace la solicitud</param>
    /// <returns></returns>
    public void AsignarTareaManualPorAgenda(ASTareaPorAgenda tarea, string usuarioAplicacion)
    {
      ASUsuario usuarioResponsable = ASRepositorio.Instancia.ObtenerUsuario(tarea.UsuarioResponsable);
      if (usuarioResponsable != null)
      {
        bool usuarioActivo = true;
        if (usuarioResponsable.TipoUsuario == COConstantesModulos.USUARIO_LDAP)
        {
          usuarioActivo = Framework.Servidor.Seguridad.SEAdministradorSeguridad.Instancia.ValidarUsuario(new Servicios.ContratoDatos.Seguridad.SECredencialUsuario()
          {
            Usuario = usuarioResponsable.IdUsuario
          });
        }
        if (usuarioActivo)
        {
          ASRepositorio.Instancia.AsignarTareaManualPorAgenda(tarea, usuarioAplicacion, usuarioResponsable);
        }
        else
        {
          ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_USUARIO_NO_ACTIVO_O_NO_VALIDO_ATENDER_TAREA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_NO_ACTIVO_O_NO_VALIDO_ATENDER_TAREA));
          throw new FaultException<ControllerException>(excepcion);
        }
      }
      else
      {
        ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_USUARIO_NO_ACTIVO_O_NO_VALIDO_ATENDER_TAREA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_NO_ACTIVO_O_NO_VALIDO_ATENDER_TAREA));
        throw new FaultException<ControllerException>(excepcion);
      }
    }

    #endregion Métodos
  }
}