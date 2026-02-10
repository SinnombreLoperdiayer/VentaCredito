using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW.Datos.Modelo;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace Framework.Servidor.ParametrosFW.Datos
{
  internal class PARepositorioAudit
  {
    /// <summary>
    /// Guarda la Auditoria de las Condiciones del Operador Postal
    /// </summary>
    /// <param name="contexto">Es el contexto de la Bd</param>
    internal static void MapeoAuditCondOperadorPostal(ModeloParametrosConn contexto)
    {
      contexto.Audit<CondicionOperadorPostal_PAR, CondicionOperadorPostaHist_PAR>((record, action) => new CondicionOperadorPostaHist_PAR
      {
        COP_IdOperadorPostal = record.Field<CondicionOperadorPostal_PAR, Int32>(f => f.COP_IdOperadorPostal),
        COP_IdCondicionOperadorPostal = record.Field<CondicionOperadorPostal_PAR, Int16>(f => f.COP_IdCondicionOperadorPostal),
        COP_Descripcion = record.Field<CondicionOperadorPostal_PAR, string>(f => f.COP_Descripcion),
        COP_FechaGrabacion = record.Field<CondicionOperadorPostal_PAR, DateTime>(f => f.COP_FechaGrabacion),
        COP_CreadoPor = record.Field<CondicionOperadorPostal_PAR, string>(f => f.COP_CreadoPor),
        COP_CambiadoPor = ControllerContext.Current.Usuario,
        COP_FechaCambio = DateTime.Now,
        COP_TipoCambio = action.ToString(),
      }, (ph) => contexto.CondicionOperadorPostaHist_PAR.Add(ph));
    }

    /// <summary>
    /// Guarda la Auditoria del Operador Postal de Zona
    /// </summary>
    /// <param name="contexto">Es el contexto de la Bd</param>
    internal static void MapeoAuditOperadorPostalZona(ModeloParametrosConn contexto)
    {
      contexto.Audit<OperadorPostalZona_PAR, OperadorPostalZonaHist_PAR>((record, action) => new OperadorPostalZonaHist_PAR
      {
        OPZ_IdZona = record.Field<OperadorPostalZona_PAR, string>(f => f.OPZ_IdZona),
        OPZ_IdOperadorPostal = record.Field<OperadorPostalZona_PAR, Int32>(f => f.OPZ_IdOperadorPostal),
        OPZ_TiempoEntrega = record.Field<OperadorPostalZona_PAR, Int32>(f => f.OPZ_TiempoEntrega),
        OPZ_FechaGrabacion = record.Field<OperadorPostalZona_PAR, DateTime>(f => f.OPZ_FechaGrabacion),
        OPZ_CreadoPor = record.Field<OperadorPostalZona_PAR, string>(f => f.OPZ_CreadoPor),
        OPZ_CambiadoPor = ControllerContext.Current.Usuario,
        OPZ_FechaCambio = DateTime.Now,
        OPZ_TipoCambio = action.ToString(),
      }, (ph) => contexto.OperadorPostalZonaHist_PAR.Add(ph));
    }
  }
}