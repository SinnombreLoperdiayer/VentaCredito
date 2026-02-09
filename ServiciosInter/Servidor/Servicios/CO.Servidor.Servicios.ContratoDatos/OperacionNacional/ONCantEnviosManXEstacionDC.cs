using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Clase que contiene la informacion la cantidad de los envíos que fueron
  /// manifestados x estación en un ruta y manifiesto determinado
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONCantEnviosManXEstacionDC
  {
    [DataMember]
    [Display(Name = "Ruta")]
    public int IdRuta { get; set; }

    [DataMember]
    [Display(Name = "Manifiesto")]
    public long IdManifiesto { get; set; }

    [DataMember]
    [Display(Name = "Id Ciudad")]
    public string IdLocalidad { get; set; }

    [DataMember]
    [Display(Name = "Nombre Ciudad")]
    public string NombreLocalidad { get; set; }

    [DataMember]
    [Display(Name = "Cant. Envios")]
    public int CantidadEnvios { get; set; }

    [DataMember]
    [Display(Name = "Seleccionar")]
    public bool Seleccionada { get; set; }
  }
}