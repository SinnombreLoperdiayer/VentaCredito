using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLClienteCreditoSucursalContrato : DataContractBase
  {
    [DataMember]
    public int IdCliente { get; set; }

    [DataMember]
    public string Nit { get; set; }

    [DataMember]
    public string RazonSocial { get; set; }

    [DataMember]
    public string Direccion { get; set; }

    [DataMember]
    public string Telefono { get; set; }

    [DataMember]
    public int IdSucursal { get; set; }

    [DataMember]
    public string NombreSucursal { get; set; }

    [DataMember]
    public int IdContrato { get; set; }

    [DataMember]
    public string NumeroContrato { get; set; }

    [DataMember]
    public string NombreCliente { get; set; }

    [DataMember]
    public string DigitoVerificacion { get; set; }

    [DataMember]
    public int IdListaPrecios { get; set; }

    [DataMember]
    public PALocalidadDC Localidad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Sucursal", Description = "TooltipSucursal")]
    public List<CLSucursalDC> SucursalesCliente { get; set; }
  }
}