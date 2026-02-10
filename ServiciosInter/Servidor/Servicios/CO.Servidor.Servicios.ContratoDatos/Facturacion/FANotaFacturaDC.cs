using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class FANotaFacturaDC
    {
        [DataMember]
        [Display(Name = "Id")]
        public long IdNota { get; set; }

        [DataMember]
        public long NumeroFactura { get; set; }

        [DataMember]
        [Display(Name = "Tipo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public FATipoNotaDC TipoNota { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "Valor")]
        public decimal ValorNota { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "Observaciones")]
        public string Observaciones { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public FADescripcionNotaDC Descripcion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public FAResponsableDC Responsable { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public FAEstadoNotaDC EstadoNota { get; set; }

        [DataMember]
        [Display(Name = "Fecha")]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        [Display(Name = "Usuario")]
        public string CreadoPor { get; set; }

        [DataMember]
        public long idDetalleServicioFact { get; set; }
    }
}