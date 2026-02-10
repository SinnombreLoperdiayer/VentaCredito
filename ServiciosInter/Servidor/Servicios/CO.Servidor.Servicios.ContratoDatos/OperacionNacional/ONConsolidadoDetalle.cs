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
  /// Clase que contiene la informacion de los envios del consolidado
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONConsolidadoDetalle : DataContractBase
  {
    [DataMember]
    public long IdManifiestoConsolidadoDetalle { get; set; }

    [DataMember]
    public long IdManfiestoConsolidado { get; set; }

    [DataMember]
    public long IdAdminisionMensajeria { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "NumeroGuia")]
    public long? NumeroGuia { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descargada", Description = "Descargada")]
    public bool EstaDescargada { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Peso", Description = "Peso")]
    public decimal PesoEnIngreso { get; set; }

    [DataMember]
    public string IdLocalidadManifiesta { get; set; }

    [DataMember]
    public int TipoRuta { get; set; }

    [DataMember]
    public long IdManifiestoOperacionNacional { get; set; }

    [DataMember]
    public string IdLocalidadDespacho { get; set; }

    [DataMember]
    public string NombreLocalidadDespacho { get; set; }

    [DataMember]
    public int IdRuta { get; set; }

    [DataMember]
    public int IdTipoEnvio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoEnvio", Description = "TipoEnvio")]
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
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDestino", Description = "CiudadDestino")]
    public string NombreCiudadDestino { get; set; }

    [DataMember]
    public string IdCiudadOrigen { get; set; }

    [DataMember]
    public string NombreCiudadOrigen { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descargada", Description = "Descargada")]
    public string EstaDescargadaDetalle { get; set; }
      [DataMember]
    public int PiezaActual { get; set; }
      [DataMember]
    public int TotalPiezas { get; set; }
      [DataMember]
      public string GuiaRotulo { get; set; }

  }
}