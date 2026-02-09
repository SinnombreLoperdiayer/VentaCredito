using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Solicitudes.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Solicitudes.Giros.Motivos
{
  internal class GIMotivo : ControllerBase
  {
    /// <summary>
    /// Se crea la instancia
    /// </summary>
    private static readonly GIMotivo instancia = (GIMotivo)FabricaInterceptores.GetProxy(new GIMotivo(), COConstantesModulos.GIROS);

    public static GIMotivo Instancia
    {
      get { return GIMotivo.instancia; }
    }

    /// <summary>
    /// Obtiene los Motivos de Solicitudes Rafael Ramirez 05-01-2012
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public List<GIMotivoSolicitudDC> ObtenerMotivosSolicitud(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return GIRepositorioSolicitudes.Instancia.ObtenerMotivosSolicitud(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtiene los Tipos Solicitudes y Motivos
    /// </summary>
    /// <returns></returns>
    public List<GITipoSolicitudDC> ObtenerTiposSolicitud()
    {
      return GIRepositorioSolicitudes.Instancia.ObtenerTiposSolicitud();
    }

    /// <summary>
    /// Adiciona - Modifica - Elimina el Motivo de la Solicitud Rafael Ramirez 05-01-2012
    /// </summary>
    /// <param name="motivoSolicitud"></param>
    public void ActuzalizarMotivoSolicitudes(GIMotivoSolicitudDC motivoSolicitud)
    {
      if (motivoSolicitud.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
      {
        GIRepositorioSolicitudes.Instancia.AdicionarMotivoSolicitud(motivoSolicitud);
      }

      if (motivoSolicitud.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
      {
        GIRepositorioSolicitudes.Instancia.ModificarMotivoSolicitud(motivoSolicitud);
      }

      if (motivoSolicitud.EstadoRegistro == EnumEstadoRegistro.BORRADO)
      {
        GIRepositorioSolicitudes.Instancia.EliminarMotivoSolicitud(motivoSolicitud);
      }
    }
  }
}