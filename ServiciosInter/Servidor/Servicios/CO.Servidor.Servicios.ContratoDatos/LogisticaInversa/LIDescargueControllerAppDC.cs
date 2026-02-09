using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIDescargueControllerAppDC : DataContractBase
    {
        [DataMember]
        public long IdMensajero { get; set; }
        [DataMember]
        public string IdCiudad { get; set; }
        [DataMember]
        public string NombreCiudad { get; set; }
        [DataMember]
        public string NombreQuienRecibe { get; set; }
        [DataMember]
        public ADMotivoGuiaDC MotivoGuia { get; set; }
        [DataMember]
        public string Observaciones { get; set; }
        [DataMember]
        public long NumeroGuia { get; set; }
        [DataMember]
        public short IdServicio { get; set; }
        [DataMember]
        public long IdentificacionQuienRecibe { get; set; }
        [DataMember]
        public string Telefono { get; set; }
        [DataMember]
        public string Latitud { get; set; }
        [DataMember]
        public string Longitud { get; set; }
        [DataMember]
        public DateTime FechaGrabacion { get; set; }
        [DataMember]
        public List<LITipoEvidenciaControllerAppDC> TipoEvidencia { get; set; }
        [DataMember]
        public long NumeroIntentoFallidoEntrega { get; set; }
        [DataMember]
        public LIRecibidoGuia RecibidoGuia { get; set; }
        [DataMember]
        public long IdPlanilla { get; set; }
        public DateTime FechaEntrega { get; set; }
        public int TipoPredio { get; set; }
        public string DescripcionPredio { get; set; }
        public short IdEstado { get; set; }
        public string Usuario { get; set; }
        public int TipoContador { get; set; }
        public string NumeroContador { get; set; }
    }
}
