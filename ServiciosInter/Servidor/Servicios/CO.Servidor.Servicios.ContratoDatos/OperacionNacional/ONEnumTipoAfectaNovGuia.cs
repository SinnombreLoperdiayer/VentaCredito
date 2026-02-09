using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum ONEnumTipoAfectaNovGuia
    {

        [EnumMember]
        EnCOL = 0,

        [EnumMember]
        EnCiudad = 1,

        [EnumMember]
        EnPunto = 2,

        [EnumMember]
        EnMensajero = 3,

        [EnumMember]
        EnClienteCredito = 4,


        [EnumMember]
        EnManifiesto = 5
    }

}
