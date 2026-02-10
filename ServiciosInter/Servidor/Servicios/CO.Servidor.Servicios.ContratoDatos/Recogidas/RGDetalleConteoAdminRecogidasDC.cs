using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Comun;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGDetalleConteoAdminRecogidasDC
    {

        [DataMember]
        public string FiltroConteo { get; set; }

        [DataMember]
        public long IdSolRecogida { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public DateTime FechaHoraRecogida { get; set; }

        [DataMember]
        public string IdLocalidadRecogida { get; set; }

        [DataMember]
        public string IdOrigenSolRecogida { get; set; }

        [DataMember]
        public int IdAgencia { get; set; }

        [DataMember]
        public COEnumIdentificadorAplicacion IdEstadoSolicitud { get; set; }

        [DataMember]
        public long PesoAproximado { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public string DireccionRecogida { get; set; }

        [DataMember]
        public string NumeroDocumento { get; set; }

        [DataMember]
        public long? IdClienteContado { get; set; }

        [DataMember]
        public long? IdSucursal { get; set; }

        [DataMember]
        public long? IdCentroServicios { get; set; }

        [DataMember]
        public bool EsEsporadicaCliente { get; set; }

        [DataMember]
        public RGEnumTipoRecogidaDC TipoSolRecogida { get; set; }


        [DataMember]
        public RGEnumClaseSolicitud ClaseSolicitud { get; set; }

        [DataMember]
        public decimal Longitud { get; set; }

        [DataMember]
        public decimal Latitud { get; set; }

        [DataMember]
        public string DescripcionMotivo { get; set; }

        [DataMember]
        public string LongitudRecogida { get; set; }

        [DataMember]
        public string LatitudRecogida { get; set; }

        [DataMember]
        public string IdentificacionCliente { get; set; }
    }
}
