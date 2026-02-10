using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun
{
    /// <summary>
    /// Descripcion: Elemento de la sección personalizda de configuración para menejo de metadata y
    /// cadena de conexión para Entity Framework
    /// Autor: Diego Toro
    /// Fecha: 29/09/2011
    /// Version: 1.0
    /// Modificado por:
    /// Fecha Modificación:
    /// </summary>   
    public class COMetadataConfigurationElementFramework : ConfigurationElement
    {
        #region Static Fields

        private static ConfigurationProperty cpName;
        private static ConfigurationProperty cpMetadata;
        private static ConfigurationProperty cpStringConnection;

        private static ConfigurationPropertyCollection sectionProperties;

        #endregion Static Fields

        #region ctor

        static COMetadataConfigurationElementFramework()
        {
            cpName = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsKey);

            cpMetadata = new ConfigurationProperty("metadata", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
            cpStringConnection = new ConfigurationProperty("connectionString", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

            sectionProperties = new ConfigurationPropertyCollection();

            sectionProperties.Add(cpName);
            sectionProperties.Add(cpMetadata);
            sectionProperties.Add(cpStringConnection);
        }

        #endregion ctor

        #region Properties

        /// <summary>
        /// Gets the StringValue setting.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base[cpName]; }
        }


        /// <summary>
        /// Gets the StringValue setting.
        /// </summary>
        [ConfigurationProperty("metadata", IsRequired = true)]
        public string Metadata
        {
            get { return (string)base[cpMetadata]; }
        }

        /// <summary>
        /// Gets the StringValue setting.
        /// </summary>
        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)base[cpStringConnection]; }
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