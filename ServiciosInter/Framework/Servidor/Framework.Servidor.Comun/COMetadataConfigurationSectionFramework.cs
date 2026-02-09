using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun
{
  /// <summary>
  /// Descripcion:  Clase para manejo de la sección de archivo de configuración
  /// de la aplicación para resolver cadenas de conexión para Entity Framework
  /// Autor: Diego Toro
  /// Fecha: 29/09/2011
  /// Version: 1.0
  /// Modificado por:
  /// Fecha Modificación:
  /// </summary>
  public class COMetadataConfigurationSectionFramework : ConfigurationSection
  {
    #region Static Fields

    private static ConfigurationPropertyCollection sectionProperties;
    private static ConfigurationProperty cpNameSection;
    //private static ConfigurationProperty cpProviderSection;
    private static ConfigurationProperty cpNameMetadataCollection;

    #endregion Static Fields

    #region ctro

    static COMetadataConfigurationSectionFramework()
    {
      cpNameSection = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
      //cpProviderSection = new ConfigurationProperty("provider", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
      cpNameMetadataCollection = new ConfigurationProperty("", typeof(COMetadataCollectionFramework), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsDefaultCollection);

      sectionProperties = new ConfigurationPropertyCollection();
      sectionProperties.Add(cpNameSection);
      //sectionProperties.Add(cpProviderSection);
      sectionProperties.Add(cpNameMetadataCollection);
    }

    #endregion ctro

    #region Properties

    /// <summary>
    /// Gets the StringValue setting.
    /// </summary>
    [ConfigurationProperty("name", IsRequired = true)]
    public string Name
    {
      get { return (string)base[cpNameSection]; }
    }

    /// <summary>
    /// Gets the StringValue setting.
    /// </summary>
    //[ConfigurationProperty("provider", IsRequired = true)]
    //public string Provider
    //{
    //    get { return (string)base[cpProviderSection]; }
    //}

    /// <summary>
    /// Gets the StringValue setting.
    /// </summary>
    public COMetadataCollectionFramework MetadataCollection
    {
      get { return (COMetadataCollectionFramework)base[cpNameMetadataCollection]; }
    }

    /// <summary>
    /// Override the Properties collection and return our custom one.
    /// </summary>
    protected override ConfigurationPropertyCollection Properties
    {
      get { return sectionProperties; }
    }

    #endregion Properties
  }
}