using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using CO.Controller.Servidor.Integraciones.CuatroSieteDos;

namespace CO.Controller.ServicioWin.CuatroSieteDos
{
  public partial class ServicioIntegracionCuatroSieteDos : ServiceBase
  {
    System.Timers.Timer temporizador = new System.Timers.Timer();

    public ServicioIntegracionCuatroSieteDos()
    {
      InitializeComponent();

      Debug.WriteLine("construyendo servicio");
      try
      {
        if (!System.Diagnostics.EventLog.SourceExists("WSIntegra472"))
        {
          System.Diagnostics.EventLog.CreateEventSource(
              "WSIntegra472", "WSIntegra472Log");
        }
        eventLog1.Source = "WSIntegra472";
        eventLog1.Log = "WSIntegra472Log";
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Error contructor: " + ex.Message);
      }
    }

    protected override void OnStart(string[] args)
    {
      Debug.WriteLine("iniciando servicio");
      eventLog1.WriteEntry("Servicio integración 472 iniciado");
      temporizador.Interval = 60000;
      temporizador.Elapsed += temporizador_Elapsed;

      temporizador.Enabled = true;
    }

    private void temporizador_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      temporizador.Enabled = false;
      integrar472();
      temporizador.Enabled = true;
    }

    private void integrar472()
    {
      try
      {
        Debug.WriteLine("iniciando integracion");
        //Integracion472.Instancia.ReTransmitirTransaccionesFallidas();
        Debug.WriteLine("integracion ok");
        // TODO: RON Prueba de deshabilitación integración
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Error integrando: " + ex.Message);
        eventLog1.WriteEntry(ex.Message);
        eventLog1.WriteEntry(ex.StackTrace);
      }
    }

    protected override void OnStop()
    {
      eventLog1.WriteEntry("Servicio integración 472 Detenido");
    }
  }
}