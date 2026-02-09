using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLServiciosSucursal : DataContractBase
  {
    [DataMember]
    public TAServicioDC Servicio { get; set; }

    [DataMember]
    public CLContratosDC Contrato { get; set; }

    [DataMember]
    public TAListaPrecioDC ListaPrecio { get; set; }

    [DataMember]
    public int IdSucursal { get; set; }
  }
}