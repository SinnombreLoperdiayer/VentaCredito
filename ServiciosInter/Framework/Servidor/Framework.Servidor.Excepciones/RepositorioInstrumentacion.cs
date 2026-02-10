using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones.Modelo;

namespace Framework.Servidor.Excepciones
{
  public class RepositorioInstrumentacion
  {
    private static readonly RepositorioInstrumentacion instancia = new RepositorioInstrumentacion();
    private const string NombreModelo = "ModeloExcepciones";
    public List<string> ModulosInstrumentados;
    private readonly object locker;

    /// <summary>
    /// Retorna la instancia de la clase ARRepositorio
    /// </summary>
    public static RepositorioInstrumentacion Instancia
    {
      get { return RepositorioInstrumentacion.instancia; }
    }

    #region ctor

    private RepositorioInstrumentacion()
    {
      ConsultarModulosInstrumentacion();

      //System.Timers.Timer tm = new System.Timers.Timer();
      //tm.Interval = 900000;//cada 15 minutos
      //tm.Elapsed += new System.Timers.ElapsedEventHandler(this.ReiniciarModulos_Tick);
      //tm.Start();
    }

    #endregion ctor

    private void ReiniciarModulos_Tick(object sender, EventArgs e)
    {
        string strUsuario = "";
        if (ControllerContext.Current != null)
            strUsuario = ControllerContext.Current.Usuario;
        else
            strUsuario = "NoUsuario";
      try
      {
        lock (locker)
        {
          ConsultarModulosInstrumentacion();
        }
      }
      catch (Exception ex)
      {
          AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, ex.Message, "AUD", ex, strUsuario);
      }
    }

    /// <summary>
    /// Consulta el listado de módulos que deben ser instrumentados
    /// </summary>
    /// <returns></returns>
    private void ConsultarModulosInstrumentacion()
    {
      using (ModeloExcepciones contexto = new ModeloExcepciones(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString("ModeloExcepciones")))
      {
        ModulosInstrumentados = contexto.InstrumentacionModulo_AUD.ToList().ConvertAll(m => m.IdModulo);
      }
    }

    /// <summary>
    /// Almacena la instrumentacion de un método especifico
    /// </summary>
    /// <param name="instrumentacion"></param>
    public void InstrumentarMetodo(Instrumentacion_AUD instrumentacion)
    {
      using (ModeloExcepciones contexto = new ModeloExcepciones(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString("ModeloExcepciones")))
      {
        contexto.Instrumentacion_AUD.Add(instrumentacion);
        contexto.SaveChanges();
      }
    }
  }
}