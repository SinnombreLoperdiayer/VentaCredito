using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum LIEnumSaleParaPendientesAgencia
    {
        [DataMember]
        [Description("")]
        None=0,
        [DataMember]
        [Description("Devolucion A Espera Confimacion Racol")]
        DevolucionAEsperaConfimacionRacol = 1,
        [DataMember]
        [Description("Para Nuevo Intento de Entrega")]
        ParaNuevoIntentoDeEntrega = 2,
        [DataMember]
        [Description("Devolver A La Racol")]
        DevolverALaRacol = 3
    }
}
