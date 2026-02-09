using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUImpresionPlanillaVentasDC
    {

        [DataMember]
        public long NumConTransReotorno { get; set; }
        [DataMember]
        public OUPlanillaVentaDC Planilla { get; set; }

        [DataMember]
        public List<OUAsignacionDC> Asignaciones { get; set; }
        [DataMember]
        public string NombreCentroServiciosOrigen { get; set; }
        [DataMember]
        public string NombreCentroServiciosDestino { get; set; }
        [DataMember]
        public string NombreEmpresaTransportadora { get; set; }
        [DataMember]
        public string NombreCiudadDestino { get; set; }
        [DataMember]
        public List<string> GuiasRotulosSueltos { get; set; }
    }
}
