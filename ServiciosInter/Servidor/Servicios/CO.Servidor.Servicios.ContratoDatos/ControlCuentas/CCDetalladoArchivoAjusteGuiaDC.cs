using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CCDetalladoArchivoAjusteGuiaDC : DataContractBase
    {
        [DataMember]
        public long IdArchivo { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public decimal? ValorComercial { get; set; }

        [DataMember]
        public decimal? ValorTransporte { get; set; }

        [DataMember]
        public decimal? ValorPrima { get; set; }

        [DataMember]
        public decimal? ValorTotal { get; set; }

        [DataMember]
        public int? Peso { get; set; }

        [DataMember]
        public int? Servicio { get; set; }

        [DataMember]
        public string Usuario { get; set; }

        [DataMember]
        public short? FormaPago { get; set; }

        [DataMember]
        public string TipoEntrega { get; set; }
    }
}
