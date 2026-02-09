using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.OperacionUrbana.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.OperacionUrbana.Datos
{
  /// <summary>
  /// Clase para guardar la Auditoria del Módulo de Operacion urbana
  /// </summary>
  internal class OURepositorioAudit
  {
    ///// <summary>
    /////
    ///// </summary>
    ///// <param name="contexto"></param>
    //internal static void MapeoAuditParametrosOperacion(ModeloOperacionUrbana contexto)
    //{
    //  contexto.Audit<ParametrosOperacionUrbana_OPU, ParametrosOperacionUrbanaHist_OPU>((record, action) => new ParametrosOperacionUrbanaHist_OPU
    //  {
    //    POU_ValorParametro = record.Field<ParametrosOperacionUrbana_OPU, string>(f => f.POU_ValorParametro),
    //    POU_Descripcion = record.Field<ParametrosOperacionUrbana_OPU, string>(f => f.POU_Descripcion),
    //    POU_FechaCambio = DateTime.Now,
    //    POU_FechaGrabacion = record.Field<ParametrosOperacionUrbana_OPU, DateTime>(f => f.POU_FechaGrabacion),
    //    POU_IdParametro = record.Field<ParametrosOperacionUrbana_OPU, string>(f => f.POU_IdParametro),
    //    POU_CreadoPor = record.Field<ParametrosOperacionUrbana_OPU, string>(f => f.POU_CreadoPor),
    //    POU_TipoCambio = action.ToString(),
    //    POU_CambiadoPor = ControllerContext.Current.Usuario,
    //  }, (ph) => contexto.ParametrosOperacionUrbanaHist_OPU.Add(ph));
    //}

      /// <summary>
      /// Método para auditar los cambios de asignacion de tulas
      /// </summary>
      /// <param name="contexto"></param>
      internal static void MapeoAuditAsignacionTulas(ModeloOperacionUrbana contexto)
      {
          contexto.Audit<AsignacionTulaPuntoServicio_OPU, AsignaTulaPuntoServicioHist_OPU>((record, action) => new AsignaTulaPuntoServicioHist_OPU
          {
              ATP_Estado = record.Field<AsignacionTulaPuntoServicio_OPU, string>(f => f.ATP_Estado),
              ATP_IdAsignacionTula = record.Field<AsignacionTulaPuntoServicio_OPU, long>(f => f.ATP_IdAsignacionTula),
              ATP_FechaCambio = DateTime.Now,
              ATP_FechaGrabacion = record.Field<AsignacionTulaPuntoServicio_OPU, DateTime>(f => f.ATP_FechaGrabacion),
              ATP_IdCentroServicioDestino = record.Field<AsignacionTulaPuntoServicio_OPU, long>(f => f.ATP_IdCentroServicioDestino),
              ATP_IdCentroServicioOrigen = record.Field<AsignacionTulaPuntoServicio_OPU, long>(f => f.ATP_IdCentroServicioOrigen),
              ATP_IdTipoAsignacion = record.Field<AsignacionTulaPuntoServicio_OPU, short>(f => f.ATP_IdTipoAsignacion),
              ATP_NoPrecinto = record.Field<AsignacionTulaPuntoServicio_OPU, long>(f => f.ATP_NoPrecinto),
              ATP_NoTula = record.Field<AsignacionTulaPuntoServicio_OPU, string>(f => f.ATP_NoTula),
              ATP_NumContTransDespacho = record.Field<AsignacionTulaPuntoServicio_OPU, long>(f => f.ATP_NumContTransDespacho),
              ATP_NumContTransRetorno = record.Field<AsignacionTulaPuntoServicio_OPU, long?>(f => f.ATP_NumContTransRetorno),
              ATP_CreadoPor = record.Field<AsignacionTulaPuntoServicio_OPU, string>(f => f.ATP_CreadoPor),
              ATP_TipoCambio = action.ToString(),
              ATP_CambiadoPor = ControllerContext.Current.Usuario,
          }, (ph) => contexto.AsignaTulaPuntoServicioHist_OPU.Add(ph));
      }
  }
}