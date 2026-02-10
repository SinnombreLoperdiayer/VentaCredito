using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIGestionesGuiaDC : DataContractBase
    {
        [DataMember]
        public long idTrazaGuia { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]
        public ADGuia GuiaAdmision { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Motivo")]
        public ADMotivoGuiaDC Motivo { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Gestiones")]
        public int Gestiones { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Dias")]
        public int DiasTelemercadeo { get; set; }

        [DataMember]
        public List<LIEvidenciaDevolucionDC> ArchivosAdjuntos { get; set; }
    }
}