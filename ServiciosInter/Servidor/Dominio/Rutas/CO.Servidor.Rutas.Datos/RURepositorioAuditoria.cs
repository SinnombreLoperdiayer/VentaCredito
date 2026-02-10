using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Rutas.Datos.Modelo;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Rutas.Datos
{
  internal class RURepositorioAuditoria
  {
    internal static void MapeoAuditEmpresaTransportadora(ModeloRutas contexto)
    {
      contexto.Audit<EmpresaTransportadora_RUT, EmpresaTransportadoraHist_RUT>((record, action) => new EmpresaTransportadoraHist_RUT()
      {
        ETR_IdEmpresaTransportadora = record.Field<EmpresaTransportadora_RUT, Int16>(f => f.ETR_IdEmpresaTransportadora),
        ETR_Nombre = record.Field<EmpresaTransportadora_RUT, string>(f => f.ETR_Nombre),
        ETR_IdTipoTransporte = record.Field<EmpresaTransportadora_RUT, Int16>(f => f.ETR_IdTipoTransporte),
        ERT_Estado = record.Field<EmpresaTransportadora_RUT, string>(f => f.ERT_Estado),
        ETR_CreadoPor = record.Field<EmpresaTransportadora_RUT, string>(f => f.ETR_CreadoPor),
        ETR_FechaGrabacion = record.Field<EmpresaTransportadora_RUT, DateTime>(f => f.ETR_FechaGrabacion),
        ERT_CambiadoPor = ControllerContext.Current.Usuario,
        ERT_FechaCambio = DateTime.Now,

        ERT_TipoCambio = action.ToString(),
      }, (ph) => contexto.EmpresaTransportadoraHist_RUT.Add(ph));
    }
  }
}