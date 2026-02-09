using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class TATarifaRapiPromocionalDC : DataContractBase
    {
        [DataMember]
        public int IdListaPrecio { get; set; }

        [DataMember]
        public int IdServicio { get; set; }

        [DataMember]
        public ObservableCollection<TAServicioRapiPromocionalDC> ServicioRapiPromocional { get; set; }

        [DataMember]
        public ObservableCollection<TAFormaPago> FormasPago { get; set; }

        [DataMember]
        public TAServicioPesoDC ServicioPeso { get; set; }

        [DataMember]
        public ObservableCollection<TAImpuestosDC> Impuestos { get; set; }
    }
}