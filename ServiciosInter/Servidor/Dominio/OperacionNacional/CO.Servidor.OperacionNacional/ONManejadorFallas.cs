using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.OperacionNacional.Comun;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using Framework.Servidor.Agenda;

namespace CO.Servidor.OperacionNacional
{
  /// <summary>
  /// Clase para el manejo de fallas de operación urbana
  /// </summary>
  internal class ONManejadorFallas : MarshalByRefObject
  {
    /// <summary>
    /// Asignar tareas por falla de peso
    /// </summary>
    /// <param name="guiaIngresada">Guía con los datos para gennear la falla</param>
    /// <remarks>La falla se crea en una hebra del pool de .net</remarks>
    public static void DespacharFallaPorNovedadConsolidado(ONConsolidado consolidado, string novedad, string usuario)
    {
      ThreadPool.QueueUserWorkItem((a) =>
      {
        try
        {
          string comentarios = String.Format(MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_FALLA_NOVEDAD_PRECINTO_CONSOLIDADO), consolidado.LocalidadManifestada.IdLocalidad, consolidado.NumeroPrecintoRetorno, consolidado.NumeroPrecintoIngreso, novedad);
          ASAsignadorTarea.Instancia.AsignarTareas(OPConstantesOperacionNacional.ID_FALLA_NOVEDAD_CONSOLIDADO, usuario, comentarios);
        }
        catch (Exception ex)
        {
          Trace.Write(ex);
        }
      });
    }

    /// <summary>
    /// Asignar tarea si el envio no tiene bolsa de seguridad
    /// </summary>
    /// <param name="guiaIngresada"></param>
    public static void DespacharFallaPorBolsaDeSeguridad(ONEnviosDescargueRutaDC envio, string usuario)
    {
      ThreadPool.QueueUserWorkItem((a) =>
      {
        try
        {
          string comentarios = String.Format(MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_FALLA_SIN_BOLSA_SEGURIDAD), envio.NumeroGuia, envio.EstadoEmpaque.DescripcionEstado, envio.NombreCiudadOrigen);
          ASAsignadorTarea.Instancia.AsignarTareas(OPConstantesOperacionNacional.ID_FALLA_ESTADO_EMPAQUE, usuario, comentarios);
        }
        catch (Exception ex)
        {
          Trace.Write(ex);
        }
      });
    }

    /// <summary>
    /// Asignar tarea si el envio no tiene bolsa de seguridad
    /// </summary>
    /// <param name="guiaIngresada"></param>
    public static void DespacharFallaPorBolsaDeSeguridadOrigen(ONEnviosDescargueRutaDC envio, string usuario)
    {
      ThreadPool.QueueUserWorkItem((a) =>
      {
        try
        {
          long idCentroLogistico;
          IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
          idCentroLogistico = fachadaCentroServicios.ObtenerCentroLogisticoApoyo(envio.IdCentroServicioOrigen);

          string comentarios = String.Format(MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_FALLA_SIN_BOLSA_SEGURIDAD), envio.NumeroGuia, envio.EstadoEmpaque.DescripcionEstado, envio.NombreCiudadOrigen);
          ASAsignadorTarea.Instancia.AsignarTareasOrigen(OPConstantesOperacionNacional.ID_FALLA_ESTADO_EMPAQUE, usuario, comentarios, idCentroLogistico);
        }
        catch (Exception ex)
        {
          Trace.Write(ex);
        }
      });
    }

    /// <summary>
    /// Asignar tareas por falla de peso
    /// </summary>
    /// <param name="guiaIngresada">Guía con los datos para gennear la falla</param>
    /// <remarks>La falla se crea en una hebra del pool de .net</remarks>
    public static void DespacharFallaPorDiferenciaPeso(ONEnviosDescargueRutaDC envio, string usuario)
    {
      ThreadPool.QueueUserWorkItem((a) =>
      {
        try
        {
          string comentarios = String.Format(MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_FALLA_DIFERENCIA_PESO),
            envio.NumeroGuia, envio.PesoGuiaSistema, envio.PesoGuiaIngreso, envio.IdCentroServicioOrigen + "-" + envio.NombreCentroServicioOrigen,
            envio.IdCentroServicioDestino + "-" + envio.NombreCentroServicioDestino);

          ASAsignadorTarea.Instancia.AsignarTareas(OPConstantesOperacionNacional.ID_FALLA_DIFERENCIA_PESO, usuario, comentarios);
        }
        catch (Exception ex)
        {
          Trace.Write(ex);
        }
      }, envio);
    }
  }
}