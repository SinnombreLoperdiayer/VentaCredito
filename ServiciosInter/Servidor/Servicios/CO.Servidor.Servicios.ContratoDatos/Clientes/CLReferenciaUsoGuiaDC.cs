using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Almacena la configuración de referencia para uso de las guías de un cliente según su condición de uso:
  //Origen Cerrado-Destino Abierto: OC-DA
  //Origen Cerrado-Destino Cerrado: OC-DC
  //Origen Abierto-Destino Cerrado: OA-DC
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLReferenciaUsoGuiaDC
  {
    /// <summary>
    /// Condición de uso de la guía:
    ///Es origen abierto: 1 (true)
    ///No es origen abierto: 0 (false)
    /// </summary>
    [DataMember]
    public bool EsOrigenAbierto { get; set; }

    /// <summary>
    /// País de origen
    /// </summary>
    [DataMember]
    public string PaisOrigen { get; set; }

    /// <summary>
    /// Pais Origen de tipo PALocalidadDC
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Pais", Description = "TooltipPais")]
    public PALocalidadDC PaisOrigenLoc { get; set; }

    /// <summary>
    /// Identificador de la ciudad de origen
    /// </summary>
    [DataMember]
    public string CiudadOrigen { get; set; }

    /// <summary>
    /// Ciudad origen de tipo PALocalidadDC
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudad")]
    public PALocalidadDC CiudadOrigenLoc { get; set; }

    /// <summary>
    /// Identificación del tipo de identificación
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion", Description = "TooltipTipoIdentificacion")]
    public string TipoIdentificacionOrigen { get; set; }

    /// <summary>
    /// Identificación del origen
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    public string IdentificacionOrigen { get; set; }

    /// <summary>
    /// Teléfono del origen
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
    [StringLength(20, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [RegularExpression(ConstantesFramework.REGEX_VALIDACION_TELEFONO, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "TelefonoNoValido")]
    public string TelefonoOrigen { get; set; }

    /// <summary>
    /// Nombre completo del origen
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
    public string NombreOrigen { get; set; }

    /// <summary>
    /// Dirección del origen
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
    public string DireccionOrigen { get; set; }

    /// <summary>
    /// Código postal de la localidad
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoPostal", Description = "ToolTipCodigoPostal")]
    public string CodigoPostalOrigen { get; set; }

    /// <summary>
    /// Condición de uso de la guía:
    /// Es origen abierto: 1 (true)
    /// No es origen abierto: 0 (false)
    /// </summary>
    [DataMember]
    public bool EsDestionoAbierto { get; set; }

    /// <summary>
    /// País de destino
    /// </summary>
    [DataMember]
    public string PaisDestino { get; set; }

    /// <summary>
    /// Pais destino de tipo PALocalidadDC
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Pais", Description = "TooltipPais")]
    public PALocalidadDC PaisDestinoLoc { get; set; }

    /// <summary>
    /// Identificador de la ciudad de destino
    /// </summary>
    [DataMember]
    public string CiudadDestino { get; set; }

    /// <summary>
    /// Ciudad destino de tipo PALocalidadDC
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudad")]
    public PALocalidadDC CiudadDestinoLoc { get; set; }

    /// <summary>
    /// Identificación del tipo de identificación
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion", Description = "TooltipTipoIdentificacion")]
    public string TipoIdentificacionDestino { get; set; }

    /// <summary>
    /// Identificación del destinatario
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    public string IdentificacionDestino { get; set; }

    /// <summary>
    /// Teléfono del destinatario
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
    [StringLength(20, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [RegularExpression(ConstantesFramework.REGEX_VALIDACION_TELEFONO, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "TelefonoNoValido")]
    public string TelefonoDestino { get; set; }

    /// <summary>
    /// Nombre completo del destinatario
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
    public string NombreDestino { get; set; }

    /// <summary>
    /// Dirección del destinatario
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
    public string DireccionDestino { get; set; }

    /// <summary>
    /// Código postal de la localidad
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoPostal", Description = "ToolTipCodigoPostal")]
    public string CodigoPostalDestino { get; set; }

    /// <summary>
    /// Codigo de la sucursal
    /// </summary>
    [DataMember]
    public int IdSucursal { get; set; }

    /// <summary>
    /// Indica el estado de un registro
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}