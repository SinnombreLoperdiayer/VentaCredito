using System;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Contrato para transporte de la información complementaria de cierre de caja
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAInfoComplementariaCierreCajaDC : DataContractBase
  {
    [DataMember]
    public int IdCAja { get; set; }

    [DataMember]
    public long IdCierreCaja { get; set; }

    [DataMember]
    public string NombreCompletoUsuario { get; set; }

    [DataMember]
    public DateTime FechaApertura { get; set; }

    [DataMember]
    public DateTime FechaCierre { get; set; }

    [DataMember]
    public decimal TotalIngresosEfectivo { get; set; }

    [DataMember]
    public decimal TotalEgresosEfectivo { get; set; }

    [DataMember]
    public decimal TotalIngresosOtrasFormas { get; set; }
  }
}