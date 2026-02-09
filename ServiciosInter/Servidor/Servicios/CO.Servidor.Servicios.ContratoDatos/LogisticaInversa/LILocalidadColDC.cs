using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  /// <summary>
  /// Clase de datacontract para manejo de localidades por col
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class LILocalidadColDC : DataContractBase
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string IdLocalidad { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string NombreLocalidad { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public long IdCentroServicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "AgenciaOrigen")]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string NombreCentroServicio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Telefono { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Direccion { get; set; }
  }
}