using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using Framework.Servidor.Agenda;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.LogisticaInversa.PruebasEntrega
{
  public class LIManejadorFallasPruebasEntrega
  {
    private static readonly LIManejadorFallasPruebasEntrega instancia = (LIManejadorFallasPruebasEntrega)FabricaInterceptores.GetProxy(new LIManejadorFallasPruebasEntrega(), COConstantesModulos.PRUEBAS_DE_ENTREGA);

    public static LIManejadorFallasPruebasEntrega Instancia
    {
      get { return LIManejadorFallasPruebasEntrega.instancia; }
    }

    /// <summary>
    /// Asignar tareas por guías no descargadas de un manifiesto
    /// </summary>
    public List<long> GenerarFallaGuiasNoDescargadas()
    {
      return LIRepositorioPruebasEntrega.Instancia.ObtenerGuiasNoDescargadas();
    }

    /// <summary>
    /// Asignar tareas por guías mal diligenciadas
    /// </summary>
    public List<long> GenerarFallaGuiasMalDiligenciadas()
    {
      return LIRepositorioPruebasEntrega.Instancia.ObtenerGuiasMalDiligenciadas();
    }
  }
}