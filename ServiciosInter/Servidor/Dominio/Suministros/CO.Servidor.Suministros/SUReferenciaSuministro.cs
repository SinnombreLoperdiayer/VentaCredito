using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Suministros
{
  public class SUReferenciaSuministro
  {
    private static readonly SUReferenciaSuministro instancia = new SUReferenciaSuministro();

    /// <summary>
    /// Retorna una instancia de referencia suministros
    /// /// </summary>
    public static SUReferenciaSuministro Instancia
    {
      get { return SUReferenciaSuministro.instancia; }
    }

    internal void GuardaSuministroProvisionReferencia(SURemisionSuministroDC remision, SUTrasladoSuministroDC traslado)
    {
      SURepositorio.Instancia.GuardaSuministroProvisionReferencia(remision, traslado);
    }

    /// <summary>
    /// Guarda la referencia del suministro provisionado
    /// </summary>
    /// <param name="remision"></param>
    /// <param name="centroServiciosAprovisionado"></param>
    public void GuardaSuministroProvisionReferenciaProceso(SURemisionSuministroDC remision, SUTrasladoSuministroDC traslado)
    {
      SURepositorio.Instancia.GuardaSuministroProvisionReferenciaProceso(remision, traslado);
    }
  }
}