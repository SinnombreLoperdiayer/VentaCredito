using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIEstadoDescargueControllerAppDC : DataContractBase
    {
        [DataMember]
        public OUEnumValidacionDescargue Resultado { get; set; }


        [DataMember]
        public string Mensaje { get; set; }
    }
}
