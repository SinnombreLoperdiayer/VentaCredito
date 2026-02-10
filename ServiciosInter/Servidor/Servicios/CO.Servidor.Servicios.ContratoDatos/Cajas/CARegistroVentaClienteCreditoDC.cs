using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene los campos
  /// del registro de Cliente Credito
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CARegistroTransClienteCreditoDC : DataContractBase
  {
    /// <summary>
    /// Gets or sets the id cliente.
    /// </summary>
    /// <value>
    /// Es el auto numero del id del cliente credito.
    /// </value>
    [DataMember]
    public int IdCliente { get; set; }

    /// <summary>
    /// Gets or sets the nombre cliente.
    /// </summary>
    /// <value>
    /// Es el nombre del Cliente Credito.
    /// </value>
    [DataMember]
    public string NombreCliente { get; set; }

    /// <summary>
    /// Gets or sets the nit cliente.
    /// </summary>
    /// <value>
    /// Es el nit del cliente Credito.
    /// </value>
    [DataMember]
    public string NitCliente { get; set; }

    /// <summary>
    /// Gets or sets the id contrato.
    /// </summary>
    /// <value>
    /// es el id del contrato del cliente credito.
    /// </value>
    [DataMember]
    public int IdContrato { get; set; }

    /// <summary>
    /// Gets or sets the numero contrato.
    /// </summary>
    /// <value>
    /// Es el numero del contrato del
    /// cliente credito
    /// </value>
    [DataMember]
    public string NumeroContrato { get; set; }

    /// <summary>
    /// Gets or sets the id sucursal.
    /// </summary>
    /// <value>
    /// Es el id del centro de servicio
    /// </value>
    [DataMember]
    public int IdSucursal { get; set; }

    /// <summary>
    /// Gets or sets the nombre sucursal.
    /// </summary>
    /// <value>
    /// Es el nombre del centro o punto de servicio.
    /// </value>
    [DataMember]
    public string NombreSucursal { get; set; }
  }
}