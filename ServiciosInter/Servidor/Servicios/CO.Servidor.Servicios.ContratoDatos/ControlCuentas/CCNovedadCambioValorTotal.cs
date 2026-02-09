using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CCNovedadCambioValorTotal : ADNovedadGuiaDC
    {
        public decimal NuevoValorTotal { get { return NuevoValorTransporte + NuevoValorPrima; } }

        [DataMember]
        public decimal NuevoValorTransporte { get; set; }

        [DataMember]
        public decimal NuevoValorPrima { get; set; }

        [DataMember]
        public decimal NuevoValorComercial { get; set; }
    }
}
