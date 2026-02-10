using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Importacion.Datos.Modelo;
using Framework.Servidor.Servicios.ContratoDatos.Importacion;

namespace Framework.Servidor.Importacion.Datos
{
  /// <summary>
  /// Contiene los métodos de acceso a datos del módulo de Importación
  /// </summary>
  public class IMRepositorio
  {
    #region Instancia Singleton de la clase

    /// <summary>
    /// Instancia de la clase ASRepositorio
    /// </summary>
    private static readonly IMRepositorio instancia = new IMRepositorio();

    /// <summary>
    /// Propiedad de la instancia de la clase
    /// </summary>
    public static IMRepositorio Instancia
    {
      get { return IMRepositorio.instancia; }
    }

    #endregion Instancia Singleton de la clase

    #region Atributos

    private const string nombreModelo = "ImportacionEntities";

    #endregion Atributos

    #region Métodos de consulta

    /// <summary>
    /// Consulta la lista de plantillas disponibles
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IMPlantillaImportacion> ObtenerPlantillasDisponibles()
    {
      using (ImportacionEntities contexto = new ImportacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        // Retorno la lista de plantillas, la plantilla la dejo vacia porque no voy a utilizar la plantilla en el cliente
        return contexto.PlantillaImportacion_PAR
                .Where(p => p.PLI_Estado == ConstantesFramework.ESTADO_ACTIVO)
                .ToList()
                .ConvertAll(p => new IMPlantillaImportacion { Nombre = p.PLI_Nombre, Plantilla = string.Empty });
      }
    }

    /// <summary>
    /// Retorna la primera plantilla uqe encuentre que tenga el nombre pasado como parámetro
    /// </summary>
    /// <param name="nombre">Nombre de la plantilla a consultar</param>
    /// <returns></returns>
    public IMPlantillaImportacion ObtenerPlantilla(string nombre)
    {
      using (ImportacionEntities contexto = new ImportacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        PlantillaImportacion_PAR plantilla = contexto.PlantillaImportacion_PAR
                                                .First(p => p.PLI_Nombre.Trim().ToUpper() == nombre.Trim().ToUpper());
        return new IMPlantillaImportacion
        {
          Nombre = plantilla.PLI_Nombre,
          Plantilla = plantilla.PLI_Plantilla
        };
      }
    }

    #endregion Métodos de consulta

    #region Métodos de inserción

    /// <summary>
    /// Registra plantilla en el sistema
    /// </summary>
    /// <param name="plantilla"></param>
    /// <param name="usuario"></param>
    public void RegistrarPlantilla(IMPlantillaImportacion plantilla, string usuario)
    {
      using (ImportacionEntities contexto = new ImportacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        PlantillaImportacion_PAR p = new PlantillaImportacion_PAR()
            {
              PLI_CreadoPor = usuario,
              PLI_Estado = ConstantesFramework.ESTADO_ACTIVO,
              PLI_FechaGrabacion = DateTime.Now,
              PLI_Nombre = plantilla.Nombre,
              PLI_Plantilla = plantilla.Plantilla
            };
        contexto.PlantillaImportacion_PAR.AddObject(p);
        contexto.SaveChanges();
      }
    }

    #endregion Métodos de inserción
  }
}