using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUGuiaIngresadaAppDC : DataContractBase
    {
        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public int IdEstadoGuia { get; set; }

        [DataMember]
        public string NombreEstadoGuia { get; set; }

        [DataMember]
        public string DireccionDestinatario { get; set; }

        [DataMember]
        public TAServicioDC Servicio;

        [DataMember]
        public long Planilla { get; set; }

        [DataMember]
        public DateTime FechaAuditoria { get; set; }

        [DataMember]
        public DateTime FechaAsignacion { get; set; }

        [DataMember]
        public bool EsAlCobro { get; set; }

        [DataMember]
        public string Ciudad { get; set; }

        [DataMember]
        public string IdCiudad { get; set; }

        [DataMember]
        public string TipoCliente { get; set; }

    }
}
