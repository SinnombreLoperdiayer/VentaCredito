using System.ComponentModel;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum LIEnumTipoGestionTelemercadeo
    {

        [Description(" ")]
        [EnumMember]
        None = 0,

        [Description("NUEVA DIRECCIÓN")]
        [EnumMember]
        NuevaDireccion = 21,

        [Description("NO HUBO COMUNICACIÓN DESTINATARIO")]
        [EnumMember]
        Nohubocomunicaciondestinatario = 22,

        [Description("NO HUBO COMUNICACIÓN REMITENTE")]
        [EnumMember]
        Nohubocomunicacionremitente = 23,

        [Description("SOLICITUD DE DEVOLUCIÓN")]
        [EnumMember]
        Solicituddedevolucion = 24,

        [Description("CLIENTE NO ACEPTA DEVOLUCIÓN DEL ALCOBRO")]
        [EnumMember]
        ClienteNoAceptaDevolucionDelAlcobro = 25,

        [Description("REMITENTE SOLICITA ULTIMO REENVÍO")]
        [EnumMember]
        RemitenteSolicitaUltimoReenvio = 26,

        [Description("FALSO MOTIVO DEVOLUCIÓN")]
        [EnumMember]
        FalsoMotivoDevolución = 27,

        [Description("REINTENTAR LLAMADA")]
        [EnumMember]
        ReintentarLlamada = 28,

        [Description("DEVOLUCIÓN RATIFICADA")]
        [EnumMember]
        DevolucionRatificada = 29,

        [Description("ENVÍO PARA CUSTODIA")]
        [EnumMember]
        EnvioParaCustodia = 30,

        [Description("RECLAMO EN OFICINA")]
        [EnumMember]
        ReclamoEnOficina = 31,

        [Description("MODIFICACIÓN CAMBIO DE DESTINO")]
        [EnumMember]
        ModificacionCambioDeDestino = 32,

        
        [Description("EN ESPERA CONFIRMACION DEL CLIENTE")]
        [EnumMember]
        EnEsperaConfirmacionCliente = 33,

        [Description("DEVOLUCION REGIONAL")]
        [EnumMember]
        DevolucionRegional = 34
    }
}
