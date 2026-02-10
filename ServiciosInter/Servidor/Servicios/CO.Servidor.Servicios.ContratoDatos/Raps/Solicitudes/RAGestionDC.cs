using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAGestionDC
    {
        [DataMember]
        public long IdGestion { get; set; }

        [DataMember]
        public long IdSolicitud { get; set; }

        [DataMember]
        public DateTime Fecha { get; set; }

        [DataMember]
        public string Comentario { get; set; }

        [DataMember]
        public string IdCargoGestiona { get; set; }

        [DataMember]
        public string CorreoEnvia { get; set; }

        //[DataMember]
        // public RAEnumAccion IdAccion { get; set; }

        [DataMember]
        public string IdCargoDestino { get; set; }

        [DataMember]
        public string CorreoDestino { get; set; }

        [DataMember]
        public string IdResponsable { get; set; }

        [DataMember]
        public RAEnumEstados IdEstado { get; set; }

        [DataMember]
        public string IdUsuario { get; set; }

        [DataMember]
        public DateTime FechaVencimiento { get; set; }

        [DataMember]
        public string DocumentoSolicita { get; set; }

        [DataMember]
        public string DocumentoResponsable { get; set; }

        [DataMember]
        public string DescripcionCargoGestiona { get; set; }

        [DataMember]
        public string DescripcionCargoDestino { get; set; }

        [DataMember]
        public string NombreResponsable { get; set; }

        [DataMember]
        public string NombreSolicita { get; set; }

        [DataMember]
        public List<RAAdjuntoDC> ListaAdjuntos { get; set; }

        [DataMember]
        public string SucursalSolicita { get; set; }

        [DataMember]
        public string ProcesoSolicita { get; set; }

        [DataMember]
        public string ProcedimientoSolicita { get; set; }

        [DataMember]
        public string SucursalResponsable { get; set; }

        [DataMember]
        public string ProcesoResponsable { get; set; }

        [DataMember]
        public string ProcedimientoResponsable { get; set; }

        [DataMember]
        public string Anchor { get; set; }

        [DataMember]
        public string CodigoPlantaSolicita { get; set; }

        [DataMember]
        public string CodigoPlantaResponsable { get; set; }

        [DataMember]
        public string MessageError { get; set; }
    }
}
