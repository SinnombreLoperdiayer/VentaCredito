using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class LIGuiaCustodiaDC : DataContractBase
  {
    [DataMember]
    public ADTrazaGuia TrazaGuia { get; set; }

    [DataMember]
    public long idAdmision { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]
    public long NumeroGuia { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Motivo")]
    public ADMotivoGuiaDC Motivo { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Destinatario")]
    public string NombreDestinatario { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Telefono")]
    public string TelefonoDestinatario { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Remitente")]
    public string NombreRemitente { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Telefono")]
    public string TelefonoRemitente { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "DiasCustodia")]
    public int DiasCustodia { get; set; }
  }
}