using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Clase que contiene la informacion de las guias de un manifiesto
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONManifiestoGuia : DataContractBase
  {
    public event EventHandler OnCambioPropManifiGuia;

    [DataMember]
    public long IdManifiestoGuia { get; set; }

    [DataMember]
    public long IdManifiestoOperacionNacional { get; set; }

    [DataMember]
    public long IdAdmisionMensajeria { get; set; }

    private long numeroGuia;

    [DataMember]
    public long NumeroGuia
    {
      get { return numeroGuia; }
      set
      {
        numeroGuia = value;
        if (OnCambioPropManifiGuia != null)
          OnCambioPropManifiGuia(null, null);
      }
    }

    [DataMember]
    public string IdLocalidadManifestada { get; set; }

    [DataMember]
    public string IdLocalidadDespacho { get; set; }

    [DataMember]
    public string NombreLocalidadDespacho { get; set; }

    [DataMember]
    public string NombreLocalidadManifestada { get; set; }

    [DataMember]
    public bool EstaDescargada { get; set; }

    [DataMember]
    public decimal PesoEnIngreso { get; set; }

    [DataMember]
    public int IdRuta { get; set; }

    [DataMember]
    public int TipoRuta { get; set; }

    [DataMember]
    [Required]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]
    public string NumeroGuiaBinding { get; set; }

    [DataMember]
    public int IdTipoEnvio { get; set; }

    [DataMember]
    public string NombreTipoEnvio { get; set; }

    [DataMember]
    public long IdCentroServicioOrigen { get; set; }

    [DataMember]
    public string NombreCentroServicioOrigen { get; set; }

    [DataMember]
    public long IdCentroServicioDestino { get; set; }

    [DataMember]
    public string NombreCentroServicioDestino { get; set; }

    [DataMember]
    public string IdCiudadDestino { get; set; }

    [DataMember]
    public string NombreCiudadDestino { get; set; }

    [DataMember]
    public string IdCiudadOrigen { get; set; }

    [DataMember]
    public string NombreCiudadOrigen { get; set; }
    
    [DataMember]
    public bool GuiaSuelta { get; set; }
    [DataMember]
    public long IdManifiestoConsolidado { get; set; }

    [DataMember]
    public int PiezaActual { get; set; }
    [DataMember]
    public int TotalPiezas { get; set; }
    [DataMember]
    public string GuiaRotulo { get; set; }
  }
}