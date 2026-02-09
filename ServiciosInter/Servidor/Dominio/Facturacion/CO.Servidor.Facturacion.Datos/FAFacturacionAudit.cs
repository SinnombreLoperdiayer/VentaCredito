using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Facturacion.Datos.Modelo;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Facturacion.Datos
{
  /// <summary>
  ///  Clase para guardar la Auditoria del Módulo de facturacion
  /// </summary>
  internal class FAFacturacionAudit
  {
    /// <summary>
    /// Metodo de auditoria de programaciones de facturacion
    /// </summary>
    /// <param name="cliente"></param>
    internal static void MapearAuditModificarProgramacion(FacturacionEntities contexto)
    {
      contexto.Audit<ProgramacionFactura_FAC, ProgramacionFacturaHist_FAC>((record, action) => new ProgramacionFacturaHist_FAC()
      {
        PRF_CambiadoPor = ControllerContext.Current.Usuario,
        PRF_CreadoPor = record.Field<ProgramacionFacturaHist_FAC, string>(f => f.PRF_CreadoPor),
        PRF_DescAgrupamiento = record.Field<ProgramacionFacturaHist_FAC, string>(f => f.PRF_DescAgrupamiento),
        PRF_DescContrato = record.Field<ProgramacionFacturaHist_FAC, string>(f => f.PRF_DescContrato),
        PRF_DiaCorte = record.Field<ProgramacionFacturaHist_FAC, short>(f => f.PRF_DiaCorte),
        PRF_FechaEjecucion = record.Field<ProgramacionFacturaHist_FAC, DateTime>(f => f.PRF_FechaEjecucion),
        PRF_Ejecutado = record.Field<ProgramacionFacturaHist_FAC, bool>(f => f.PRF_Ejecutado),
        PRF_FechaGrabacion = record.Field<ProgramacionFacturaHist_FAC, DateTime>(f => f.PRF_FechaGrabacion),
        PRF_FechaProgramacion = record.Field<ProgramacionFacturaHist_FAC, DateTime>(f => f.PRF_FechaProgramacion),
        PRF_IdAgrupamiento = record.Field<ProgramacionFacturaHist_FAC, int>(f => f.PRF_IdAgrupamiento),
        PRF_IdCliente = record.Field<ProgramacionFacturaHist_FAC, int>(f => f.PRF_IdCliente),
        PRF_IdContrato = record.Field<ProgramacionFacturaHist_FAC, int>(f => f.PRF_IdContrato),
        PRF_IdProgramacion = record.Field<ProgramacionFacturaHist_FAC, long>(f => f.PRF_IdProgramacion),
        PRF_IdRacol = record.Field<ProgramacionFacturaHist_FAC, long>(f => f.PRF_IdRacol),
        PRF_NombreRacol = record.Field<ProgramacionFacturaHist_FAC, string>(f => f.PRF_NombreRacol),
        PRF_RazonSocialCliente = record.Field<ProgramacionFacturaHist_FAC, string>(f => f.PRF_RazonSocialCliente),
        PRF_FechaCambio = DateTime.Now,
        PRF_TipoCambio = action.ToString(),
      }, (ph) => contexto.ProgramacionFacturaHist_FAC.Add(ph));
    }

    /// <summary>
    /// Metodo de auditoria de programaciones de facturacion
    /// </summary>
    /// <param name="cliente"></param>
    internal static void MapearAuditBorrarNota(FacturacionEntities contexto)
    {
      contexto.Audit<NotasFactura_FAC, NotasFacturaHist_FAC>((record, action) => new NotasFacturaHist_FAC()
      {
        NOF_CambiadoPor = ControllerContext.Current.Usuario,
        NOF_CreadoPor = record.Field<NotasFacturaHist_FAC, string>(f => f.NOF_CreadoPor),
        NOF_FechaCambio = DateTime.Now,
        NOF_FechaGrabacion = record.Field<NotasFacturaHist_FAC, DateTime>(f => f.NOF_FechaGrabacion),
        NOF_IdNota = record.Field<NotasFacturaHist_FAC, long>(f => f.NOF_IdNota),
        NOF_NumeroFactura = record.Field<NotasFacturaHist_FAC, long>(f => f.NOF_NumeroFactura),
        NOF_Observaciones = record.Field<NotasFacturaHist_FAC, string>(f => f.NOF_Observaciones),
        NOF_TipoCambio = action.ToString(),
        NOF_EstadoNota = record.Field<NotasFacturaHist_FAC, string>(f => f.NOF_EstadoNota),
        NOF_IdDescripcion = record.Field<NotasFacturaHist_FAC, short>(f => f.NOF_IdDescripcion),
        NOF_IdResponsable = record.Field<NotasFacturaHist_FAC, short>(f => f.NOF_IdResponsable),
        NOF_TipoNota = record.Field<NotasFacturaHist_FAC, string>(f => f.NOF_TipoNota),
        NOF_ValorNota = record.Field<NotasFacturaHist_FAC, decimal>(f => f.NOF_ValorNota),
      }, (ph) => contexto.NotasFacturaHist_FAC.Add(ph));
    }
  }
}