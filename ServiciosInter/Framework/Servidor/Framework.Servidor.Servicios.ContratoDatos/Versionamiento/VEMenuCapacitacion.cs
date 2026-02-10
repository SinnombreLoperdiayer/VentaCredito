using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Versionamiento
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class VEMenuCapacitacion
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        //[Display(ResourceType = typeof(Etiquetas), Name = "IdAncestro")]
        //[Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        //[StringLength(4, MinimumLength = 1, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "LongitudCadena")]
        public int IdAncestro { get; set; }

        [DataMember]
        public VEEnumProcesos IdProceso { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Etiquetas), Name = "Descripción")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Filtrable("MEC_Descripcion", "Descripción: ", COEnumTipoControlFiltro.TextBox)]
        public string Descripcion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Etiquetas), Name = "Boton")]
        public string Target { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Etiquetas), Name = "URL")]
        //[Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string URL { get; set; }

        [DataMember]
        public bool Activo { get; set; }

        [DataMember]
        public bool AplicaUsuario { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }
    }
}