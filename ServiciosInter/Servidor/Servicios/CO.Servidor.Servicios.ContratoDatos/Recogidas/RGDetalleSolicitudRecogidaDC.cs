using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGDetalleSolicitudRecogidaDC
    {
        [DataMember]
        public long IdSolRecogida { get; set; }
        [DataMember]
        public DateTime FechaGrabacion { get; set; }
        [DataMember]
        public DateTime FechaHoraRecogida { get; set; }
        [DataMember]
        public long IdOrigenSolRecogida { get; set; }
        [DataMember]
        public string NombreAplicacion { get; set; }
        [DataMember]
        public string Documento { get; set; }
        [DataMember]
        public string NombreCompleto { get; set; }
        [DataMember]
        public string IdLocalidadRecogida { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Direccion { get; set; }
        [DataMember]
        public string Telefono { get; set; }
        [DataMember]
        public string Entrega { get; set; }
        [DataMember]
        public decimal Peso { get; set; }
        [DataMember]
        public decimal Cantidad { get; set; }

        [DataMember]
        public string DescripcionEnvios { get; set; }

        [DataMember]
        public Decimal Longitud { get; set; }
        [DataMember]
        public Decimal Latitud { get; set; }
        [DataMember]
        public Decimal LongitudCiudad { get; set; }
        [DataMember]
        public Decimal LatitudCiudad { get; set; }

        [DataMember]
        public string DescripcionMotivo { get; set; }

        [DataMember]
        public string DocPersonaResponsable { get; set; }

        [DataMember]
        public string NombrePersonaResponsable { get; set; }

        [DataMember]
        public string FechaEjecucion { get; set; }

        [DataMember]
        public string IdEstadoSolicitud { get; set; }

        [DataMember]
        public long TipoRecogida { get; set; }

        [DataMember]
        public bool EsEsporadicaCliente { get; set; }

        [DataMember]
        public List<string> ImagenesEvidencia { get; set; }
    }
}
