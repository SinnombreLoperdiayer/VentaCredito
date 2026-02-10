using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Comun
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum COEnumTipoNovedad : short
    {
        [EnumMember]
        CONTROL_DE_CUENTAS = 1,
        [EnumMember]
        RUTA = 2,
        [EnumMember]
        INGRESO_DEVOLUCIONES = 3,
        [EnumMember]
        INGRESO_DEVOLUCIONES_AGENCIA = 4,
        [EnumMember]
        INGRESO_SALIDA_TULAS_Y_CONTENEDORES = 5,
    }
}
