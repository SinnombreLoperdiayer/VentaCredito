using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CCResultadoAnulacionGuia
  {
    /// <summary>
    /// Número de guía que ha solicitado anulación
    /// </summary>
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia")]
    [DataMember]
    public long NumeroGuia { get; set; }
    
    /// <summary>
    /// Indica si la guía existe en el sistema
    /// </summary>
    [DataMember]
    public bool GuiaExiste { get; set; }

    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FueAnulada")]
    [DataMember]
    public bool FueAnulada { get; set; }

    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "MensajeAnulacion")]
    [DataMember]
    public string Mensaje { get; set; }

    /// <summary>
    /// Indica si se debe permitir crear una nueva guía
    /// </summary>
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CreacionNuevaGuia")]
    [IgnoreDataMember]
    public bool CrearNuevaGuia { get; set; }

    /// <summary>
    /// Contenido básico de la guía, solo aplica cuando "GuiaExiste" es true y "FueAnulada" es false, se usa para crear la guía en estado anulado
    /// </summary>
    [DataMember]
    public ADGuia Guia { get; set; }

    [IgnoreDataMember]
    public bool CrearNuevaGuiaVisible
    {
      get
      {
          //return (!FueAnulada && Guia != null);
          return !GuiaExiste;
      }
    }
  }
}
