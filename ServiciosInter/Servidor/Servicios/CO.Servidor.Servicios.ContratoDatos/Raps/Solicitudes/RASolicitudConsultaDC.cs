using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RASolicitudConsultaDC
    {
        [DataMember]
        public long IdSolicitud { get; set; }

        [DataMember]
        public long IdParametrizacionRap { get; set; }

        [DataMember]
        public string IdCargoSolicita { get; set; }

        [DataMember]
        public string IdCargoResponsable { get; set; }

        [DataMember]
        public DateTime FechaCreacion { get; set; }

        [DataMember]
        public DateTime FechaVencimiento { get; set; }

        [DataMember]
        public int IdEstado { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public long IdSolicitudPadre { get; set; }

        [DataMember]
        public string DocumentoSolicita { get; set; }

        [DataMember]
        public string DocumentoResponsable { get; set; }

        [DataMember]
        public string NombreSolicita { get; set; }

        [DataMember]
        public string NombreResponsable { get; set; }

        [DataMember]
        public string NombreParametrizacion { get; set; }

        [DataMember]
        public string CodigoSucursal { get; set; }

        [DataMember]
        public string SucursalSolicita { get; set; }

        [DataMember]
        public string ProcesoSolicita { get; set; }

        [DataMember]
        public string ProcedimientoSolicita { get; set; }

        [DataMember]
        public string Anchor { get; set; }

    }
}
