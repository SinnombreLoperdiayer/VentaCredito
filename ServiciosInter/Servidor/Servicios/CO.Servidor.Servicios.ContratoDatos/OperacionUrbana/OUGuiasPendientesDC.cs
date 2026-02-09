using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class OUGuiasPendientesDC : DataContractBase
  {
    [DataMember]
    [CamposOrdenamiento("PVG_NumeroGuia")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "NumeroGuia")]
    public long NumeroGuia { get; set; }

    [DataMember]
    public long IdPuntoServicio { get; set; }

    [DataMember]
    [CamposOrdenamiento("PLA_NombrePuntoServicio")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PuntoCentroServicio", Description = "PuntoCentroServicio")]
    public string NombrePuntoServicio { get; set; }

    [DataMember]
    public long IdMensajero { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Planilla", Description = "Planilla")]
    public long IdPlanillaVenta { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaPlanilla", Description = "FechaPlanilla")]
    public DateTime FechaPlanillaVenta { get; set; }
  }
}