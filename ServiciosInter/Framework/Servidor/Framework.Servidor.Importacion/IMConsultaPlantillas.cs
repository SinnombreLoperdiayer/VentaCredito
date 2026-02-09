using System;
using System.Collections.Generic;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Importacion.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Importacion;

namespace Framework.Servidor.Importacion
{
  public class IMConsultaPlantillas : MarshalByRefObject
  {
    #region Instancia Singleton

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    private static readonly IMConsultaPlantillas instancia = (IMConsultaPlantillas)FabricaInterceptores.GetProxy(new IMConsultaPlantillas(), Framework.Servidor.Comun.ConstantesFramework.MODULO_FW_PLANTILLAS);

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    public static IMConsultaPlantillas Instancia
    {
      get { return IMConsultaPlantillas.instancia; }
    }

    #endregion Instancia Singleton

    /// <summary>
    /// Consulta la lista de plantillas disponibles
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IMPlantillaImportacion> ObtenerPlantillasDisponibles()
    {
      return IMRepositorio.Instancia.ObtenerPlantillasDisponibles();
    }

    /// <summary>
    /// Retorna la primera plantilla uqe encuentre que tenga el nombre pasado como parámetro
    /// </summary>
    /// <param name="nombre">Nombre de la plantilla a consultar</param>
    /// <returns></returns>
    public IMPlantillaImportacion ObtenerPlantilla(string nombre)
    {
      return IMRepositorio.Instancia.ObtenerPlantilla(nombre);
    }
  }
}