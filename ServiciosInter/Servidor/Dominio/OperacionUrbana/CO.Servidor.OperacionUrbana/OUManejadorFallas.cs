using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Web;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Agenda;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.OperacionUrbana
{
  /// <summary>
  /// Clase para el manejo de fallas de operación urbana
  /// </summary>
  internal class OUManejadorFallas : MarshalByRefObject
  {
    /// <summary>
    /// Asignar tareas por falla de peso
    /// </summary>
    /// <param name="guiaIngresada">Guía con los datos para gennear la falla</param>
    /// <remarks>La falla se crea en una hebra del pool de .net</remarks>
    public static void DespacharFallaPorDiferenciaPeso(OUGuiaIngresadaDC guiaIngresada, decimal valorDesfase, string usuario)
    {
      if (guiaIngresada.PesoSistema < (guiaIngresada.Peso - valorDesfase) || guiaIngresada.PesoSistema < (guiaIngresada.Peso + valorDesfase))
      {
        ThreadPool.QueueUserWorkItem((a) =>
          {
            try
            {
              string comentarios = String.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_DIFERENCIAS_PESO),
                guiaIngresada.NumeroGuia,
                guiaIngresada.PesoSistema,
                guiaIngresada.Peso,
                guiaIngresada.IdCentroServicioDestino + "-" + guiaIngresada.NombreCentroServicioDestino,
                guiaIngresada.IdCentroServicioOrigen + "-" + guiaIngresada.NombreCentroServicioOrigen);
              ASAsignadorTarea.Instancia.AsignarTareas(OUConstantesOperacionUrbana.FALLA_DIFERENCIA_PESO, usuario, comentarios);
            }
            catch (Exception ex)
            {
              Trace.Write(ex);
            }
          }, guiaIngresada);
      }
    }

    /// <summary>
    /// Asignar tareas por guías no provisionadas
    /// </summary>
    /// <param name="guiaIngresada">Guía con los datos para validar y generar la falla</param>
    public static void DespacharFallaPorGuiaNoProvisionada(OUGuiaIngresadaDC guiaIngresada)
    {
      if (!COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ValidarSuministroProvisionado(OUConstantesOperacionUrbana.SUMINISTRO_BOLSA_SEGURIDAD, guiaIngresada.IdSucursal))
      {
        ThreadPool.QueueUserWorkItem((a) =>
        {
          try
          {
            string comentarios = String.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_NO_PROVISIONADA), guiaIngresada.NumeroGuia, guiaIngresada.IdCentroLogistico);
            ASAsignadorTarea.Instancia.AsignarTareas(OUConstantesOperacionUrbana.FALLA_GUIA_PROVISIONADA, ControllerContext.Current.Usuario, comentarios);
          }
          catch (Exception ex)
          {
            Trace.Write(ex);
          }
        }, guiaIngresada);
      }
    }

    /// <summary>
    /// Asignar tarea por cliente con presupuesto vencido
    /// </summary>
    /// <param name="guiaIngresada">Guía con los datos para validar y generar la falla</param>
    public static void DespacharFallaPorClienteConPresupestoVencido(OUGuiaIngresadaDC guiaIngresada)
    {
      if (!COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ValidaCupoPresupuestoMensual(guiaIngresada.IdContratoRemitente, guiaIngresada.ValorTotal))
      {
        ThreadPool.QueueUserWorkItem((a) =>
        {
          try
          {
            string comentarios = String.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CLIENTE_CON_PRESUPUESO_VENCIDO), guiaIngresada.IdContratoRemitente, guiaIngresada.IdCentroLogistico);
            ASAsignadorTarea.Instancia.AsignarTareas(OUConstantesOperacionUrbana.FALLA_PRESUPUESTO_VENCIDO, ControllerContext.Current.Usuario, comentarios);
          }
          catch (Exception ex)
          {
            Trace.Write(ex);
          }
        });
      }
    }

    /// <summary>
    /// Asignar tarea si el envio no tiene bolsa de seguridad y el cliente tiene suministro de bolsa
    /// </summary>
    /// <param name="guiaIngresada"></param>
    public static void DespacharFallaPorBolsaDeSeguridad(OUGuiaIngresadaDC guiaIngresada, string usuario, string novedad)
    {
      ThreadPool.QueueUserWorkItem((a) =>
      {
        try
        {
          string comentarios = String.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_SIN_BOLSA_DE_SEGURIDAD), guiaIngresada.NumeroGuia, novedad, guiaIngresada.IdContratoRemitente);
          ASAsignadorTarea.Instancia.AsignarTareas(OUConstantesOperacionUrbana.FALLA_SIN_BOLSA_DE_SEGURIDAD, usuario, comentarios);
        }
        catch (Exception ex)
        {
          Trace.Write(ex);
        }
      });
    }

    /// <summary>
    /// Asigna tarea por las guias no planilladas
    /// </summary>
    /// <param name="idCentroServicios"></param>
    /// <param name="nombreCentroServicios"></param>
    /// <param name="direccionCentroServicios"></param>
    /// <param name="totalGuias"></param>
    public static void DespacharFallaPorGuiasNoPlanilladas(long idCentroServicios, string nombreCentroServicios, string direccionCentroServicios, int totalGuias, string usuario)
    {
      ThreadPool.QueueUserWorkItem((a) =>
      {
        try
        {
          string comentarios = String.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIAS_SIN_PLANILLAR), idCentroServicios, nombreCentroServicios, direccionCentroServicios, totalGuias);
          ASAsignadorTarea.Instancia.AsignarTareas(OUConstantesOperacionUrbana.FALLA_GUIAS_SIN_PLANILLAR, ControllerContext.Current.Usuario, comentarios);
        }
        catch (Exception ex)
        {
          Trace.Write(ex);
        }
      });
    }

    /// <summary>
    /// Asigna tarea por reabrir una recogida esporadica
    /// </summary>
    /// <param name="idCentroServicios"></param>
    /// <param name="nombreCentroServicios"></param>
    /// <param name="direccionCentroServicios"></param>
    /// <param name="totalGuias"></param>
    public static void DespacharFallaPorReabrirRecogida(long idRecogida, string motivoApertura, string usuario)
    {
      ThreadPool.QueueUserWorkItem((a) =>
      {
        try
        {
          string comentarios = String.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_RECOGIDA_ABIERTA), idRecogida, motivoApertura);
          ASAsignadorTarea.Instancia.AsignarTareas(OUConstantesOperacionUrbana.FALLA_REABRIR_RECOGIDA, usuario, comentarios);
        }
        catch (Exception ex)
        {
          Trace.Write(ex);
        }
      });
    }
  }
}