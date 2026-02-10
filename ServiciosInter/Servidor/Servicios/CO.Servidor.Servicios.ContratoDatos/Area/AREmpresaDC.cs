using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Area
{
  /// <summary>
  /// Clase que contiene la informacion de la empresa (Interrapidisimo)
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class AREmpresaDC
  {
    [DataMember]
    public int IdEmpresa { get; set; }

    [DataMember]
    public string NombreEmpresa { get; set; }

    [DataMember]
    public string SiglaEmpresa { get; set; }

    [DataMember]
    public string Nit { get; set; }

    [DataMember]
    public string DigitoVerificacion { get; set; }

    [DataMember]
    public string Direccion { get; set; }

    [DataMember]
    public string Telefono { get; set; }

    [DataMember]
    public string IdLocalidad { get; set; }

    [DataMember]
    public string NombreLocalidad { get; set; }

    [DataMember]
    public long? IdentificacionPersonaInterna { get; set; }

    [DataMember]
    public long? IdentificacionPersonaExterna { get; set; }

    [DataMember]
    public int CodigoMinisterio { get; set; }
  }
}