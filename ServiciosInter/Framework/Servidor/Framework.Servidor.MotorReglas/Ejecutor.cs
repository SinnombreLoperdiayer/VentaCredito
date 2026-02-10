using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Remoting;
using System.ServiceModel;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;

namespace Framework.Servidor.MotorReglas
{
  /// <summary>
  /// Ejecutar para una regla de negocio
  /// </summary>
  public class Ejecutor
  {
    private static int tiempoCaducaInstancia = 0;

    /// <summary>
    /// Ejecutar uan regla de negocio.
    /// </summary>
    /// <param name="nombreAssembly">Nombre del assembly que contiene la regla de negocio.</param>
    /// <param name="namespace">Namespace de la regla de negocio.</param>
    /// <param name="nombreClase">Nombre de la clase dentro del namespace que contiene la lógica de la regla.</param>
    /// <param name="parametrosRegla">Parametros de entrada y salida de la regla.</param>
    /// <returns>Respuesta del estado de la ejecución de la regla y referencia a los parámetros de salida de la regla</returns>
    public static RespuestaEjecutorReglas EjecutarRegla(string nombreAssembly, string @namespace, string nombreClase, IDictionary<string, object> parametrosRegla)
    {
      if (String.IsNullOrEmpty(nombreAssembly) || String.IsNullOrEmpty(@namespace) || String.IsNullOrEmpty(nombreClase))
      {
        ControllerException excepcion = new ControllerException(ConstantesFramework.PARAMETROS_FRAMEWORK, ETipoErrorFramework.
                                                                EX_ERROR_PARAMETROS_NULOS_EJECUCION_REGLA.ToString(),
                                                                MensajesFramework.CargarMensaje(ETipoErrorFramework.
                                                                EX_ERROR_PARAMETROS_NULOS_EJECUCION_REGLA));

        throw new FaultException<ControllerException>(excepcion);
      }

      if (parametrosRegla == null)
      {
        parametrosRegla = new Dictionary<string, object>();
        parametrosRegla.Add(ClavesReglasFramework.HUBO_ERROR, false);
        parametrosRegla.Add(ClavesReglasFramework.EXCEPCION, null);
      }

      RespuestaEjecutorReglas resultado = new RespuestaEjecutorReglas(parametrosRegla);

      try
      {
        IReglaNegocio regla;

        string tipoRegla = @namespace + "." + nombreClase;

        if (Cache.Instancia.ContainsKey(tipoRegla))
        {
          // Cargar la instancia desde el cache del servidor
          regla = Cache.Instancia[tipoRegla] as IReglaNegocio;
        }
        else
        {
          tiempoCaducaInstancia = ObtenerTiempoCaducaCache();

          regla = CrearInstancia(nombreAssembly, tipoRegla);
          Cache.Instancia.Add(tipoRegla, regla, tiempoCaducaInstancia);
        }

        // ejecutar la regla
        regla.Ejecutar(parametrosRegla);

        //validar si hubo error en la ejecución de la regla
        object error;

        if (parametrosRegla.TryGetValue(ClavesReglasFramework.HUBO_ERROR, out error))
        {
          if (Convert.ToBoolean(error))
          {
            resultado.HuboError = true;
            if (parametrosRegla.TryGetValue(ClavesReglasFramework.EXCEPCION, out error))
            {
              if (error is Exception && error != null)
              {
                resultado.Excepcion = (Exception)error;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        resultado.Excepcion = ex;
        AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.ToString(), ConstantesFramework.MODULO_FRAMEWORK, ex);
      }

      //retornar resultado de la ejecución de la regla
      return resultado;
    }

    private static IReglaNegocio CrearInstancia(string nombreAssembly, string tipoRegla)
    {
      ObjectHandle instancia;
      IReglaNegocio regla;

      // Se instancia la clase
      instancia = Activator.CreateInstance(nombreAssembly, tipoRegla);
      regla = instancia.Unwrap() as IReglaNegocio;

      if (regla == null)
      {
        ControllerException excepcion = new ControllerException(ConstantesFramework.PARAMETROS_FRAMEWORK, ETipoErrorFramework.
                                                              EX_ERROR_REGLA_NO_IMPLEMENTADA.ToString(),
                                                              MensajesFramework.CargarMensaje(ETipoErrorFramework.
                                                              EX_ERROR_REGLA_NO_IMPLEMENTADA));
        throw new FaultException<ControllerException>(excepcion);
      }

      return regla;
    }

    private static int ObtenerTiempoCaducaCache()
    {
      if (tiempoCaducaInstancia == 0)
      {
        tiempoCaducaInstancia = Convert.ToInt32(PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.TIEMPO_CADUCA_REGLA_INSTANCIA_NEGOCIO));
      }

      return tiempoCaducaInstancia;
    }
  }
}