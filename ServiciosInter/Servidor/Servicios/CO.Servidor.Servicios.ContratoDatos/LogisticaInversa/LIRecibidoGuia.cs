using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
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
    public class LIRecibidoGuia : DataContractBase
    {
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CertificacionRecibidoPor", Description = "TooltipCertificacionRecibidoPor")]
        public string RecibidoPor { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CertificacionIdentificacion", Description = "TooltipCertificacionIdentificacion")]
        public string Identificacion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CertificacionTelefono", Description = "TooltipCertificacionTelefono")]
        public string Telefono { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CertificacionOtros", Description = "TooltipCertificacionOtros")]
        public string Otros { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaEntrega", Description = "TooltipCertificacionFechaEntrega")]
        public DateTime FechaEntrega { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "HoraEntrega", Description = "TooltipCertificacionFechaEntrega")]
        public DateTime HoraEntrega { get; set; }

        [DataMember]
        public long IdGuia { get; set; }

        [DataMember]
        [Range(1, 999999999999999, ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public long NumeroGuia { get; set; }


        [DataMember]
        public string RutaImagen { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        /// <summary>
        /// Adición atributo Fecha Admision
        /// </summary>
        [DataMember]
        public DateTime FechaAdmision { get; set; }


        [DataMember]
        public DateTime? FechaVerificacion { get; set; }

        [DataMember]
        public bool Verificado { get; set; }

        /// <summary>
        /// Adicion origenes aplicacion
        /// </summary>
        [DataMember]
        public LIEnumOrigenAplicacion IdAplicacionOrigen { get; set; }
    }
}