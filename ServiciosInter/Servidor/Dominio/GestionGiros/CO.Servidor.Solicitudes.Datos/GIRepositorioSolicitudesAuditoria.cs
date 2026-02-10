using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Solicitudes.Datos.Modelo;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Solicitudes.Datos
{
  internal class GIRepositorioSolicitudesAuditoria
  {
    /// <summary>
    /// Guardar Auditoria Lista de Motivos Rafael Ramirez 05-01-2012
    /// </summary>
    /// <param name="contexto"></param>
    internal static void MapeoAuditMotivoSolicitud(ModeloSolicitudes contexto)
    {
      contexto.Audit<MotivoSolicitud_GIR, MotivoSolicitudHist_GIR>((record, action) => new MotivoSolicitudHist_GIR()
      {
        MOS_IdMotivoSolicitud = record.Field<MotivoSolicitud_GIR, Int16>(f => f.MOS_IdMotivoSolicitud),
        MOS_IdTipoSolicitud = record.Field<MotivoSolicitud_GIR, Int16>(f => f.MOS_IdTipoSolicitud),
        MOS_Descripcion = record.Field<MotivoSolicitud_GIR, string>(f => f.MOS_Descripcion),
        MOS_CreadoPor = record.Field<MotivoSolicitud_GIR, string>(f => f.MOS_CreadoPor),
        MOS_FechaGrabacion = record.Field<MotivoSolicitud_GIR, DateTime>(f => f.MOS_FechaGrabacion),
        MOS_EsEditable = record.Field<MotivoSolicitud_GIR, bool>(f => f.MOS_EsEditable),
        MOS_CambiadoPor = ControllerContext.Current.Usuario,
        MOS_FechaCambio = DateTime.Now,
        MOS_TipoCambio = action.ToString(),
      }, (ph) => contexto.MotivoSolicitudHist_GIR.Add(ph));
    }
  }
}