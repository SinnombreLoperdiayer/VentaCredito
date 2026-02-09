using System;
using Framework.Servidor.Agenda.Datos.Modelo;
using Framework.Servidor.Excepciones;

namespace Framework.Servidor.Agenda.Datos
{
  /// <summary>
  /// Posee los métodos para realizar las auditorías necesarias para el módulo de Agenda
  /// </summary>
  internal class ASRepositorioAuditoria
  {
    /// <summary>
    /// Hace el mapeo para auditoría entre la tabla de Falla y su histórico
    /// </summary>
    /// <param name="contexto">Contexto en que se están verificando los cambios</param>
    /// <param name="usuario">Usuario que realiza la transacción</param>
    internal static void MapeoAuditoriaFalla(EntidadesAgenda contexto)
    {
      contexto.Audit<Falla_ASG, FallaHistorico_ASG>((record, action) => new FallaHistorico_ASG
      {
        FAL_CambiadoPor = ControllerContext.Current.Usuario,
        FAL_CreadoPor = record.Field<Falla_ASG, string>(campo => campo.FAL_CreadoPor),
        FAL_Descripcion = record.Field<Falla_ASG, string>(campo => campo.FAL_Descripcion),
        FAL_EsEditable = record.Field<Falla_ASG, bool>(campo => campo.FAL_EsEditable),
        FAL_Estado = record.Field<Falla_ASG, string>(campo => campo.FAL_Estado),
        FAL_FechaCambio = DateTime.Now,
        FAL_FechaGrabacion = record.Field<Falla_ASG, DateTime>(campo => campo.FAL_FechaGrabacion),
        FAL_IdFalla = record.Field<Falla_ASG, int>(campo => campo.FAL_IdFalla),
        FAL_IdModulo = record.Field<Falla_ASG, string>(campo => campo.FAL_IdModulo),
        FAL_Tipo = record.Field<Falla_ASG, string>(campo => campo.FAL_Tipo),
        FAL_TipoCambio = action.ToString()
      }, (registro) => contexto.FallaHistorico_ASG.Add(registro));
    }

    /// <summary>
    /// Hace el mapeo para auditoría entre la tabla de Tarea y su histórico
    /// </summary>
    /// <param name="contexto">Contexto en que se están verificando los cambios</param>
    /// <param name="usuario">Usuario que realiza la transacción</param>
    internal static void MapeoAuditoriaTarea(EntidadesAgenda contexto)
    {
      contexto.Audit<Tarea_ASG, TareaHistorico_ASG>((record, action) => new TareaHistorico_ASG
      {
        TAR_CambiadoPor = ControllerContext.Current.Usuario,
        TAR_CargoResponsable = record.Field<Tarea_ASG, int>(campo => campo.TAR_CargoResponsable),
        TAR_CreadoPor = record.Field<Tarea_ASG, string>(campo => campo.TAR_UsuarioInsercion),
        TAR_Descripcion = record.Field<Tarea_ASG, string>(campo => campo.TAR_Descripcion),
        TAR_Estado = record.Field<Tarea_ASG, string>(campo => campo.TAR_Estado),
        TAR_FechaCambio = DateTime.Now,
        TAR_FechaGrabacion = record.Field<Tarea_ASG, DateTime>(campo => campo.TAR_FechaGrabacion),
        TAR_IdFalla = record.Field<Tarea_ASG, int>(campo => campo.TAR_IdFalla),
        TAR_IdTarea = record.Field<Tarea_ASG, int>(campo => campo.TAR_IdTarea),
        TAR_TiempoEscalamiento = record.Field<Tarea_ASG, int>(campo => campo.TAR_TiempoEscalamiento),
        TAR_TipoCambio = action.ToString()
      }, (registro) => contexto.TareaHistorico_ASG.Add(registro));
    }
  }
}