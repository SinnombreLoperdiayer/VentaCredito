using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAAjustesCajaNovedadesDC
    {

        [DataMember]
        public PUCentroServiciosDC CentroServicio{get;set;}

        [DataMember]
        public PUCentroServiciosDC CentroServicioContraPartida { get; set; }

        [DataMember]
        public CAConceptoCajaDC ConceptoCaja { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public string Observaciones { get; set; }

        [DataMember]
        public string NumeroComprobante { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string Usuario { get; set; }

        [DataMember]
        public long NumeroDocumento { get; set; }
    
    }
}
