using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Comun
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum COEnumMotivoGuiaDevolucionAgencias : short
    {
        [EnumMember]
        SinMotivo = 0,
        [EnumMember]
        FueraDeZona = 168,
        [EnumMember]
        Accidente = 169,
        [EnumMember]
        BloqueoDeCalles_CrudoInvierno_Manifestacio = 170,
        [EnumMember]
        Huelga = 171,
        [EnumMember]
        Vacaciones = 172,
        [EnumMember]
        ResidenteAusente = 173,
        [EnumMember]
        ResidenteAusente_NoAlcanzoElMensajero = 174,
        [EnumMember]
        NoResideInmuebleDeshabitado = 175,
        [EnumMember]
        NoResideCambioDeDomicilio = 176,
        [EnumMember]
        NoResideFallecido = 177,
        [EnumMember]
        DireccionErradaDireccionIncompleta = 178,
        [EnumMember]
        DireccionErradaDireccionNoExiste = 179,
        [EnumMember]
        DireccionErradaNoCorrespondeLaCiudadDestino = 180,
        [EnumMember]
        DesconocidoDestinatarioDesconocido = 181,
        [EnumMember]
        RehusadoSeNegoARecibir = 182,
        [EnumMember]
        RehusadoAveridado = 183,
        [EnumMember]
        RehusadoContenidoIncompleto_Hurto = 184,
        [EnumMember]
        RehusadoNoPagaronAlCobro = 185,
        [EnumMember]
        RehusadoNoPagaronProductoContrapago = 186,
        [EnumMember]
        NoReclamadoEnOficina = 187,
        [EnumMember]
        CambiarNuevaDireccionDeEntrega = 188,
        [EnumMember]
        APeticionDelRemitente = 189,
        [EnumMember]
        Incautado = 190,
        [EnumMember]
        Hurto = 191,
        [EnumMember]
        EnvioCruzadoTrocado = 192,
        [EnumMember]
        NoMarcadoNoIdentificado_Nn = 193,
    }
}
