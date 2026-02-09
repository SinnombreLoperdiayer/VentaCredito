using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun
{
  /// <summary>
  /// Descripcion: Clase para el manejo de las conexiones con la base de datos usando configuración en archivo .config
  /// Autor: Diego Toro
  /// Fecha: 29/09/2011
  /// Version: 1.0
  /// Modificado por:
  /// Fecha Modificación:
  /// </summary>
  public class COAdministradorConexionesDbFramework : ICOAdministradorDbConexion
  {
    #region Campos

    private static readonly COAdministradorConexionesDbFramework instancia = new COAdministradorConexionesDbFramework();

    private Dictionary<string, string> cadenasConexion;

    #endregion Campos

    #region Constructor

    private COAdministradorConexionesDbFramework()
    {
      cadenasConexion = new Dictionary<string, string>();
    }

    #endregion Constructor

    #region Indices

    public string this[string index]
    {
      get
      {
        return ObtenerConnectionString(index);
      }
    }

    #endregion Indices

    #region Propiedades

    /// <summary>
    /// Retorna una instancia de la clase para el manejo de las conexiones con la base de datos
    /// </summary>
    public static COAdministradorConexionesDbFramework Instancia
    {
      get { return instancia; }
    }

    #endregion Propiedades

    #region Publicos

    /// <summary>
    /// Obtener una cadena de conexión para Entity Framework a patir del nombre del modelo
    /// </summary>
    /// <param name="nombreModelo">Nombre del modelo cuya metadata y cadena de conexión asociada esta en el archivo de configuración de la aplicación</param>
    /// <returns>Cadena de conexión conforme lo requiere el contexto de EntityFramework</returns>
    /// <exception cref="Exception">En caso de que no se encuentre la cadnea de conexión asociada al modelo</exception>
    public string ObtenerConnectionString(string nombreModelo)
    {
      string resultado = null;

      if (!cadenasConexion.TryGetValue(nombreModelo, out resultado))
      {
        COMetadataConfigurationSectionFramework confiSeccion = ConfigurationManager.GetSection("efMetadataSection") as COMetadataConfigurationSectionFramework;

        if (confiSeccion == null)
        {
          throw new Exception("Error en el archivo de configuración, no existe sesión de metadata para las entidades");
        }

        COMetadataCollectionFramework configuracionMetadataCollecion = confiSeccion.MetadataCollection;

        if (configuracionMetadataCollecion == null)
        {
          throw new Exception("Error en el archivo de configuración, no existe sesión de metadata para las entidades");
        }

        foreach (COMetadataConfigurationElementFramework item in confiSeccion.MetadataCollection)
        {
          if (item.Name.Equals(nombreModelo, StringComparison.OrdinalIgnoreCase))
          {
            if (ConfigurationManager.ConnectionStrings[item.ConnectionString] == null)
            {
              throw new Exception(String.Format("Error en el archivo de configuración, no existe cadena de conexión para modelo {0}", nombreModelo));
            }

            lock (this)
            {
              SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[item.ConnectionString].ConnectionString);
              EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
              entityBuilder.ProviderConnectionString = builder.ConnectionString;
              entityBuilder.Provider = ConfigurationManager.ConnectionStrings[item.ConnectionString].ProviderName;
              entityBuilder.Metadata = item.Metadata;
              resultado = entityBuilder.ToString();

              if (!String.IsNullOrEmpty(resultado) && !cadenasConexion.ContainsKey(nombreModelo))
              {
                cadenasConexion.Add(nombreModelo, resultado);
              }
            }
          }
        }

        if (String.IsNullOrEmpty(resultado))
        {
          throw new Exception(String.Format("Error en el archivo de configuración, no se encontró cadena de conexión para el modelo {0}", nombreModelo));
        }
      }

      return resultado;
    }

    /// <summary>
    /// Obtener una cadena de conexión para ADO.NET a patir del nombre del modelo
    /// </summary>
    /// <param name="nombreModelo">Nombre del modelo</param>
    /// <returns>Cadena de conexión conforme lo requiere el contexto de ADO.NET</returns>
    /// <exception cref="Exception">En caso de que no se encuentre la cadnea de conexión asociada al modelo</exception>
    public string ObtenerConnectionStringNoEntity(string nombreModelo)
    {
      string resultado = null;
      COMetadataConfigurationSectionFramework confiSeccion = ConfigurationManager.GetSection("efMetadataSection") as COMetadataConfigurationSectionFramework;

      if (confiSeccion == null)
      {
        throw new Exception("Error en el archivo de configuración, no existe sesión de metadata para las entidades");
      }

      COMetadataCollectionFramework configuracionMetadataCollecion = confiSeccion.MetadataCollection;

      if (configuracionMetadataCollecion == null)
      {
        throw new Exception("Error en el archivo de configuración, no existe sesión de metadata para las entidades");
      }

      foreach (COMetadataConfigurationElementFramework item in confiSeccion.MetadataCollection)
      {
        if (item.Name.Equals(nombreModelo, StringComparison.OrdinalIgnoreCase))
        {
          if (ConfigurationManager.ConnectionStrings[item.ConnectionString] == null)
          {
            throw new Exception(String.Format("Error en el archivo de configuración, no existe cadena de conexión para modelo {0}", nombreModelo));
          }

          SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[item.ConnectionString].ConnectionString);
          resultado = builder.ConnectionString;

          break;
        }
      }

      return resultado;
    }

    #endregion Publicos
  }
}