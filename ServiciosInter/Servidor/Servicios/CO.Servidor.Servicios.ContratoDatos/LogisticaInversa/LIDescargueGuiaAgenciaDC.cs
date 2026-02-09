using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIDescargueGuiaAgenciaDC : DataContractBase
    {
        [DataMember]
        public LIEnumPendientesAgencia Resultado { get; set; }

        [DataMember]
        public OUGuiaIngresadaDC Guia { get; set; }


        [DataMember]
        public ADTrazaGuia Estado { get; set; }


        [DataMember]
        public string Mensaje { get; set; }

    }
}
