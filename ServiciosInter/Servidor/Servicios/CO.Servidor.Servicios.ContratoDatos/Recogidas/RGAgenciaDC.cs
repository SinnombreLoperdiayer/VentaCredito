using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGAgenciaDC
    {
        [DataMember]
        public long IdCentroLogistico { get; set; }
        [DataMember]
        public long IdAgencia { get; set; }
        [DataMember]
        public int IdPropietario { get; set; }
        [DataMember]
        public string NOMBRE { get; set; }
        [DataMember]
        public string Direccion { get; set; }
        [DataMember]
        public string IdMunicipio { get; set; }
        [DataMember]
        public long IdPersonaResponsable { get; set; }
        [DataMember]
        public decimal Latitud { get; set; }
        [DataMember]
        public decimal Longitud { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string IdCentroCostos { get; set; }
        [DataMember]
        public decimal PesoMaximo { get; set; }
        [DataMember]
        public decimal VolumenMaximo { get; set; }
        [DataMember]
        public Boolean AdmiteFormaPagoAlCobro { get; set; }
        [DataMember]
        public string CodigoBodega { get; set; }
        [DataMember]
        public short IdClasificadorCanalVenta { get; set; }
    }
}
