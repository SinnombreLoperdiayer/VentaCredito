using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene los valores
  /// de la apertura de la Caja
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAAperturaCajaDC : DataContractBase
  {
    /// <summary>
    /// Es el Id de la apertura de la Caja.
    /// </summary>
    [DataMember]
    public long IdAperturaCaja { get; set; }

    /// <summary>
    /// Es el id de la caja asignado a un Usuario
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CajaNumero", Description = "TooltipCaja")]
    public int IdCaja { get; set; }

    /// <summary>
    /// Es el id del usuario que realiza la Transaccion
    /// </summary>
    [DataMember]
    public long IdCodigoUsuario { get; set; }

    /// <summary>
    /// Es el usuario que abre la caja.
    /// </summary>
    [DataMember]
    public string CreadoPor { get; set; }

    /// <summary>
    /// Estado de apertura de la caja.
    /// </summary>
    /// <value>
    ///   <c>true</c> si [esta abierta]; si no esta cerrada, <c>false</c>.
    /// </value>
    [DataMember]
    public bool EstaAbierta { get; set; }

    /// </summary>
    /// <value>
    /// es el numero del documento del usuario.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Documento", Description = "TooltipDocumento")]
    public string DocumentoUsuario { get; set; }

    /// <summary>
    /// Es el nombre del Usuario.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
    public string NombresUsuario { get; set; }

    /// <summary>
    /// Es la base inicial de la caja del Centro de Servicio.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "ValorBase", Description = "TooltipValorBase")]
    public decimal BaseInicialApertura { get; set; }
  }
}