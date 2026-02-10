using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum LIEnumTipoNovedadGuia
    {
        [EnumMember]
        SIN_NOVEDAD = 0,
        [EnumMember]
        ENVIO_MAL_ESTADO = 26,
        [EnumMember]
        BOLSA_VIOLENTADA = 27,
        [EnumMember]
        EMPAQUE_EN_MAL_ESTADO = 28,
        [EnumMember]
        SIN_BOLSA_SEGURIDAD = 29,
        [EnumMember]
        El_envío_es_para_reclamar_en_oficina = 32,
        [EnumMember]
        El_envío_no_llegó = 33,
        [EnumMember]
        El_envío_llegó_averiado = 34,
        [EnumMember]
        El_envío_llegó_saqueado = 35,
    }
}
