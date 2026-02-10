using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CO.Servidor.LogisticaInversa.Comun;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.ParametrosFW.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.LogisticaInversa.Reglas.CambioEstado
{
  public class LOIReglaCorreoDevolucion : IReglaNegocio
  {
    private object correoElectronico;

    private object nombreDestinatario;

    /// <summary>
    /// Ejecutar regla especifica.
    /// </summary>
    /// <param name="parametrosRegla">Parámetros de entrada y salida de la regla.</param>
    public void Ejecutar(IDictionary<string, object> parametrosRegla)
    {
      if (parametrosRegla.ContainsKey(PAConstantesParametros.PARAMETRO_CORREO_ELECTRONICO))
        parametrosRegla.TryGetValue(PAConstantesParametros.PARAMETRO_CORREO_ELECTRONICO, out correoElectronico);

      if (parametrosRegla.ContainsKey(PAConstantesParametros.PARAMETRO_NOMBRE_DESTINATARIO))
        parametrosRegla.TryGetValue(PAConstantesParametros.PARAMETRO_NOMBRE_DESTINATARIO, out nombreDestinatario);

      Task.Factory.StartNew(() =>
      {
        try
        {
          InformacionAlerta informacionAlerta = PAParametros.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_DESCARGUE_DEVOLUCION);
          string mensaje = String.Format(informacionAlerta.Mensaje, nombreDestinatario.ToString(), Environment.NewLine);
          PAAdministrador.Instancia.EnviarCorreologisticaInversa(correoElectronico.ToString(), informacionAlerta.Asunto, mensaje);
        }
        catch (Exception ex)
        {
          AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.ToString(), COConstantesModulos.TELEMERCADEO);
        }
      }, TaskCreationOptions.PreferFairness);
    }
  }
}