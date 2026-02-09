using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum SUEnumCategoria : short
    {
        [EnumMember]
        NoAplica = 0,
        [EnumMember]
        BolsaSeguridadAgencia = 1,
        [EnumMember]
        BolsaSeguridadCliente = 2,
        [EnumMember]
        SuministrosAgenciaPunto = 3,
        [EnumMember]
        FacturacionClienteCredito = 4,
        [EnumMember]
        ProduccionAgenciasPuntos = 5
    }
}