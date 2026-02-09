using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADMotivoGuiaDC : DataContractBase
    {
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Motivo", Description = "TooltipMotivo")]
        public short IdMotivoGuia { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Motivo", Description = "TooltipMotivo")]
        public string Descripcion { get; set; }

        [DataMember]
        public ADEnumTipoMotivoDC Tipo { get; set; }
        
        [DataMember]
        public bool EsVisible { get; set; }
        
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Adjuntos", Description = "TooltipAdjuntos")]
        public bool EsEscaneo { get; set; }
        
        [DataMember]
        public int MotivoCRC { get; set; }
        
        [DataMember]
        public bool SeReporta { get; set; }
        
        [DataMember]
        public bool CausaSupervision { get; set; }
        
        [DataMember]
        public string nombreAssembly { get; set; }
        
        [DataMember]
        public string @namespace { get; set; }
        
        [DataMember]
        public string nombreClase { get; set; }
        
        [DataMember]
        public int TiempoAfectacion { get; set; }
        
        [DataMember]
        public bool IntentoEntrega { get; set; }
        
        [DataMember]
        public long? IdTercero { get; set; }
        
        [DataMember]
        public string ObservacionMotivo { get; set; }

        [DataMember]
        public bool CapturaPredio { get; set; }

        [DataMember]
        public bool CapturaContador { get; set; }

        [DataMember]
        public bool CapturaObservacion { get; set; }
    }
}