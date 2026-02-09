using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;


namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
      [DataContract(Namespace = "http://contrologis.com")]
    public class FATipoNotaDC
    {
        /// <summary>
        /// Impuestos asociados al concepto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Tipo", Description = "Tipo")]
        public string IdTipoNota
        { get; set; }

        /// <summary>
        /// Impuestos asociados al concepto
        /// </summary>
        [DataMember]
        public string DescripcionTipoNota
        { get; set; }
    }
}
