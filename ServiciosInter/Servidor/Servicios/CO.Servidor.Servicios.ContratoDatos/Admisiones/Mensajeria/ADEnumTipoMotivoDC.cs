using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum ADEnumTipoMotivoDC : short
  {
    [EnumMember]
    DevolucionesMensajero = 1,

    [EnumMember]
    DevolucionesSupervisor = 2,

    [EnumMember]
    DevolucionesAgencia = 3,

    [EnumMember]
    DisposicionFinal = 4,

    [EnumMember]
    Devolucion = 5,

    [EnumMember]
    DevolucionWPFMensajero = 6,

    [EnumMember]
        DevolucionWPFAgencia = 7,

        [EnumMember]
        DevolucionWPFAgenciaTelemercadeo = 9,
       
        [EnumMember]
        DescargueEntregaMaestraAuditor = 10

  }
}