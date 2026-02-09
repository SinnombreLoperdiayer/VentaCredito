using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PRLiquidacionProduccionDC : DataContractBase
    {
        [DataMember]
        public long IdLiquidacionProduccion { get; set; }

        [DataMember]
        public string IdLiquNumMostrar { get; set; } 

        [DataMember]
        public long IdCentroServicios { get; set; }

        [DataMember]
        public string NombreCentroServicios { get; set; }        

        [DataMember]
        public string IdEstadoLiquidacionProduccion { get; set; }

        [DataMember]
        public string EstadoLiquidacionProduccion { get; set; }

        [DataMember]
        public long NumeroGuiaInterna { get; set; }
                
        [DataMember]
        public int Mes { get; set; }

        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public DateTime FechaAprobacion { get; set; }

        [DataMember]
        public string UsuarioAprueba { get; set; }

        [DataMember]
        public decimal TotalPagos { get; set; }

        [DataMember]
        public decimal TotalDeducciones { get; set; }

        public decimal TotalAFavor { get { return TotalPagos - TotalDeducciones; } }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public long IdTransaccionCaja { get; set; }

        [DataMember]
        public Dictionary<PREnumTipoConcepRetencionDC, int> BasesRetencion { get; set; }
    }
}
