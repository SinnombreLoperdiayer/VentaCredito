using System;
using System.ServiceModel;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Comun;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Suministros.Consumo
{
  /// <summary>
  /// Clase encargada de manejar el consumo de suministros
  /// </summary>
  internal class SUConsumo : ControllerBase
  {
    private static readonly SUConsumo instancia = (SUConsumo)FabricaInterceptores.GetProxy(new SUConsumo(), COConstantesModulos.MODULO_SUMINISTROS);

    /// <summary>
    /// Retorna la instancia singleton de la clase 
    /// </summary>
    internal static SUConsumo Instancia
    {
      get { return SUConsumo.instancia; }
    } 

    /// <summary>
    /// Almacena el consumo de la bolsa de seguridad 
    /// </summary>
    /// <param name="numeroBolsaSeguridad">Numero de la bolsa de seguridad, puede contener con su prefijo</param>
    /// <param name="grupoConsume">Grupo al cual pertener quien consume la bolsa de seguridad</param>
    /// <param name="idServiciosAsociado">Identificador del servicio asocial</param>
    public void GuardarConsumoBolsaSeguridad(string numeroBolsaSeguridad, int idServiciosAsociado , long idPropietario)
    {
      int i = 0;
      string prefijo = "";
      long numeroBolsa = 0;
      while (true)
      {
        prefijo = numeroBolsaSeguridad.Substring(0, i);
        if (long.TryParse(numeroBolsaSeguridad.Substring(prefijo.Length), out numeroBolsa))
          break;
        i++;
        if (prefijo == numeroBolsaSeguridad)
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
            EnumTipoErrorSuministros.EX_ERROR_BOLSA_SEG_NO_VALIDA.ToString(),
            MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_BOLSA_SEG_NO_VALIDA)));
      }

      SUSuministro suministroAsociado = null;
      if (prefijo != "")
        suministroAsociado = SUSuministros.Instancia.ConsultarSuministroxPrefijo(prefijo);

      if (suministroAsociado != null)
      {
        SUPropietarioGuia propietario = SUSuministros.Instancia.ObtenerPropietarioSuministro(numeroBolsa, (SUEnumSuministro)Enum.ToObject(typeof(SUEnumSuministro), suministroAsociado.Id), idPropietario);

        //SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), propietario.Propietario.to.CentroServicios.Tipo);
        SUEnumGrupoSuministroDC grupo = propietario.Propietario;

        SUConsumoSuministroDC consumo = null;

        consumo = new SUConsumoSuministroDC()
          {
            Cantidad = 1,
            EstadoConsumo = SUEnumEstadoConsumo.CON,
            IdServicioAsociado = idServiciosAsociado,
            GrupoSuministro = grupo,
            IdDuenoSuministro = propietario.Id,
            NumeroSuministro = numeroBolsa,
            Suministro = (SUEnumSuministro)Enum.ToObject(typeof(SUEnumSuministro), suministroAsociado.Id)
          };

        SUSuministros.Instancia.GuardarConsumoSuministro(consumo);
      }
      else
      {
        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
          EnumTipoErrorSuministros.EX_ERROR_BOLSA_SEG_NO_VALIDA.ToString(),
          MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_BOLSA_SEG_NO_VALIDA)));
      }
    }
  }

}
