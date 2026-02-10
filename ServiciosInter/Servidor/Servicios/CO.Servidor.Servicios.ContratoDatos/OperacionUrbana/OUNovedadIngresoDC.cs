using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUNovedadIngresoDC : DataContractBase
    {
        /// <summary>
        /// retorna o asigna el id del estado del empaque
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Novedad", Description = "Novedad")]
        public short IdNovedad{ get; set; }

        /// <summary>
        /// retorna o asigna el id del estado del empaque
        /// </summary>
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Novedad", Description = "Novedad")]
        [DataMember]
        public string DescripcionNovedad { get; set; }


        /// <summary>
        /// retorna o asigna el id del estado del empaque
        /// </summary>
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Asignado", Description = "Asignado")]
        [DataMember]
        public bool Asignado { get; set; }
    }
}
