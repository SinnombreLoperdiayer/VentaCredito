using System;
using System.Collections.Generic;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.LogisticaInversa.Reglas.Telemercadeo
{
  /// <summary>
  /// Regla para manejo de lógica de negocio para peticion del remitente
  /// </summary>
  public class LOIReglaPeticionRemitente : IReglaNegocio
  {
    ADTrazaGuia trazaGuia;

    /// <summary>
    /// Ejecutar regla especifica.
    /// </summary>
    /// <param name="parametrosRegla">Parámetros de entrada y salida de la regla.</param>

    public void Ejecutar(IDictionary<string, object> parametrosRegla)
    {
      if (parametrosRegla.ContainsKey(LOIConstantesLogisticaInversa.ESTADO_GUIA))
        trazaGuia = (ADTrazaGuia)(parametrosRegla[LOIConstantesLogisticaInversa.ESTADO_GUIA]);

      if ((trazaGuia.IdNuevoEstadoGuia != (short)(ADEnumEstadoGuia.DevolucionRatificada)))
      {
        ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO,
             LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CAMBIO_ESTADO.ToString(),
             LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CAMBIO_ESTADO));
        parametrosRegla.Add(ClavesReglasFramework.EXCEPCION, excepcion);
        parametrosRegla.Add(ClavesReglasFramework.HUBO_ERROR, true);
      }
    }
  }
}