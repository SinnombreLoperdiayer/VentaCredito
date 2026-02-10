using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SEMaquinaVersion : DataContractBase
  {
    [DataMember]
    [CamposOrdenamiento("MAV_MaquinaId")]
    [Display(Name = "Id Maquina")]
    public string MaquinaId { get; set; }

    [DataMember]
    [CamposOrdenamiento("MAV_MaquinaVersionId")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdMaquina")]
    public int MaquinaVersionId { get; set; }

    [DataMember]
    [CamposOrdenamiento("Mav_Estado")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]

    public string Estado { get; set; }

    [CamposOrdenamiento("Estado_Desc")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
    public string EstadoDesc
    {
      get
      {
        switch (Estado)
        {
          case "ACT":
            return "ACTIVO";
          case "INA":
            return "INACTIVO";
          case "PAC":
            return "PENDIENTE";
          default: return "PENDIENTE";
        }
      }
    }

    [DataMember]
    public List<SEEstadoUsuario> EstadoColeccion { get; set; }

    [DataMember]
    [CamposOrdenamiento("MAV_CreadoPor")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
    public string Usuario { get; set; }

    [DataMember]
    [CamposOrdenamiento("MAV_FechaGrabacion")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCreacion")]
    public DateTime Fecha { get; set; }
  }
}