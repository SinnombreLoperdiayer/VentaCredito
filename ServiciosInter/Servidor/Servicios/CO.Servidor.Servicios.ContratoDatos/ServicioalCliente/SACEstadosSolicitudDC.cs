using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ServicioalCliente
{
     [DataContract(Namespace = "http://contrologis.com")]
    public class SACEstadosSolicitudDC : DataContractBase
    {
        [DataMember]
         [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
         [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "Estado")]
        public int IdEstado { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "Estado")]
        public string Descripcion { get; set; }

        [DataMember]
        public short Orden { get; set; }


        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

    }
}
