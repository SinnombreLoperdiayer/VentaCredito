using CO.Cliente.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class FAEstadoNotaDC
    {
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstado")]
        public string Id { get; set; }

        [DataMember]
        public string Descripcion { get; set; }
    }
}
