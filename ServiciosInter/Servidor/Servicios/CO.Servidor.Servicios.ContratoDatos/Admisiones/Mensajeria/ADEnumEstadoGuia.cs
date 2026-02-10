using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum ADEnumEstadoGuia : short
    {
        [EnumMember]
        SinEstado = 0,

        [EnumMember]
        Admitida = 1,

        [EnumMember]
        CentroAcopio = 2,

        [EnumMember]
        TransitoNacional = 3,

        [EnumMember]
        TransitoRegional = 4,

        [EnumMember]
        ReclameEnOficina = 5,

        [EnumMember]
        EnReparto = 6,

        [EnumMember]
        IntentoEntrega = 7,

        [EnumMember]
        Telemercadeo = 8,

        [EnumMember]
        Custodia = 9,

        [EnumMember]
        DevolucionRatificada = 10,

        [EnumMember]
        Entregada = 11,

        [EnumMember]
        Reenvio = 12,

        [EnumMember]
        Digitalizada = 13,

        [EnumMember]
        Indemnizacion = 14,

        [EnumMember]
        Anulada = 15,

        [EnumMember]
        Archivada = 16,

        [EnumMember]
        DisposicionFinal = 17,

        [EnumMember]
        TransitoUrbano = 18,

        [EnumMember]
        Incautado = 21,

        [EnumMember]
        PendienteIngresoaCustodia = 22,

        [EnumMember]
        FisicoFaltante = 23,

        [EnumMember]
        CasoFortuito = 24,

        [EnumMember]
        NotaCredito = 26,

        [EnumMember]
        IngresoABodega = 27,

        [EnumMember]
        SalidadeBodega = 28,

        [EnumMember]
        Auditoria = 29,

        [EnumMember]
        DevolucionEsperaConfirmacionCliente = 30,

        [EnumMember]
        Distribucion = 31,

        [EnumMember]
        DevolucionRegional = 32,

        [EnumMember]
        DevolverALaRacol = 33

    }
}