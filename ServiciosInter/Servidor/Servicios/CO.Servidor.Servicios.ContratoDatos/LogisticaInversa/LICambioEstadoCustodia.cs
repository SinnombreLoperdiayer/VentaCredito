using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LICambioEstadoCustodia : DataContractBase
    {
        [DataMember]
        public long? IdTrazaGuia { get; set; }
        [DataMember]
        public short? IdNuevoEstadoGuia { get; set; }
        [DataMember]
        public string IdCiudad { get; set; }
        [DataMember]
        public string Modulo { get; set; }
        [DataMember]
        public DateTime FechaGrabacion { get; set; }
        [DataMember]
        public string Usuario { get; set; }
        [DataMember]
        public long IdCentroServicioEstado { get; set; }
        [DataMember]
        public short IdMotivoGuia { get; set; }
        [DataMember]
        public string DescripcionGuia { get; set; }
        [DataMember]
        public long? IdTercero { get; set; }
        [DataMember]
        public List<string> Evidencias { get; set; }
        [DataMember]
        public int IdBodega { get; set; }
    }

}
