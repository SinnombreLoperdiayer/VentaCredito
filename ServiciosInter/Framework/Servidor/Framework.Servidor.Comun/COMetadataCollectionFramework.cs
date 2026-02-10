using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun
{
  
  /// <summary>
  /// Descripcion: Clase para manejo de sección personalizada de configuración en archvio .config de la aplicación
  /// Autor: Diego Toro
  /// Fecha: 29/09/2011
  /// Version: 1.0
  /// Modificado por:
  /// Fecha Modificación:
  /// </summary>   
  [ConfigurationCollection(typeof(COMetadataConfigurationElementFramework), CollectionType = ConfigurationElementCollectionType.BasicMap)]
  public class COMetadataCollectionFramework : ConfigurationElementCollection
  {
    #region Constructors

    static COMetadataCollectionFramework()
    {
    }

    #endregion Constructors

    #region Properties

    protected override ConfigurationPropertyCollection Properties
    {
      get { return new ConfigurationPropertyCollection(); }
    }

    public override ConfigurationElementCollectionType CollectionType
    {
      get { return ConfigurationElementCollectionType.BasicMap; }
    }

    protected override string ElementName
    {
      get { return "metadatamodel"; }
    }

    #endregion Properties

    #region Indexers

    public COMetadataConfigurationElementFramework this[int index]
    {
      get { return (COMetadataConfigurationElementFramework)base.BaseGet(index); }
      set
      {
        if (base.BaseGet(index) != null)
        {
          base.BaseRemoveAt(index);
        }
        base.BaseAdd(index, value);
      }
    }

    new public COMetadataConfigurationElementFramework this[string name]
    {
      get { return (COMetadataConfigurationElementFramework)base.BaseGet(name); }
    }

    #endregion Indexers

    #region Methods

    public void Add(COMetadataConfigurationElementFramework item)
    {
      base.BaseAdd(item);
    }

    public void Remove(COMetadataConfigurationElementFramework item)
    {
      base.BaseRemove(item);
    }

    public void RemoveAt(int index)
    {
      base.BaseRemoveAt(index);
    }

    #endregion Methods

    #region Overrides

    protected override ConfigurationElement CreateNewElement()
    {
      return new COMetadataConfigurationElementFramework();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return (element as COMetadataConfigurationElementFramework).Name;
    }

    #endregion Overrides
  }
}