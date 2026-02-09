using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADTrazaGuiaImpresoDC
  {
    /// <summary>
    /// retorna o asigna el id de admision mensajeria
    /// </summary>
    [DataMember]
    public long? IdTrazaGuia { get; set; }

    [DataMember]
    public ADEnumTipoImpreso TipoImpreso { get; set; }

    [DataMember]
    public long NumeroImpreso { get; set; }


    [DataMember]
    public long? IdTercero { get; set; }


    /// <summary>
    /// Fecha de grabación del registro
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaDescargue")]
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// retorna o asigna el usuario que produjo el cambio de estado
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
    public string Usuario { get; set; }
  }
}