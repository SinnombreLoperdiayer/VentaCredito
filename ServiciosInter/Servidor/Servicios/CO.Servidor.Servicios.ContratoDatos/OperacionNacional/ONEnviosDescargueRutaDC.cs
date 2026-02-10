using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Clase que contiene la informacion de los envios del consolidado
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONEnviosDescargueRutaDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "NumeroGuia")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public long? NumeroGuia { get; set; }
    
    [DataMember]
    public long IdIngresoGuia { get; set; }
      
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EstadoEmpaque", Description = "EstadoEmpaque")]
    public PAEstadoEmpaqueDC EstadoEmpaque { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Peso", Description = "Peso")]
    public decimal PesoGuiaIngreso { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Peso", Description = "Peso")]
    public decimal PesoGuiaSistema { get; set; }

    [DataMember]
    public long IdAdmisionMensajeria { get; set; }

    [DataMember]
    public bool EstaManifestado { get; set; }

    [DataMember]
    public bool PerteneceConsolidado { get; set; }

    [DataMember]
    public string IdLocalidadDestino { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDestino", Description = "CiudadDestino")]
    public string NombreCiudadDestino { get; set; }

    [DataMember]
    public string IdCiudadOrigen { get; set; }

    [DataMember]
    public string NombreCiudadOrigen { get; set; }

    [DataMember]
    public short IdEstadoGuia { get; set; }

    /// <summary>
    /// Retorna o asigna el id del centro de servicio origen
    /// </summary>
    [DataMember]
    public long IdCentroServicioOrigen { get; set; }

    /// <summary>
    /// retorna o asigna el nombre del centro de servicios origen
    /// </summary>
    [DataMember]
    public string NombreCentroServicioOrigen { get; set; }

    /// <summary>
    /// Retorna o asigna el id del centro de servicio destino
    /// </summary>
    [DataMember]
    public long IdCentroServicioDestino { get; set; }

    /// <summary>
    /// Retorna o asigna el nombre del centro de servicio destino
    /// </summary>
    [DataMember]
    public string NombreCentroServicioDestino { get; set; }

      [DataMember]
     public int  TotalPiezasRotulo {get;set;}
      [DataMember]
      public int PiezaActualRotulo { get; set; }
      /// <summary>
      /// Id del centro de servicio que está ingresando la guia al centro de acopio
      /// </summary>
      [DataMember]
      public long IdCentroServicioIngresa { get; set; }

  }
}