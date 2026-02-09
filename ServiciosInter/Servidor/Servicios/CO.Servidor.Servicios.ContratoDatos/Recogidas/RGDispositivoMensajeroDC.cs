using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGDispositivoMensajeroDC
    {
        [DataMember]
        public long IdDispositivo { get; set; }
        [DataMember]
        public decimal Longitud { get; set; }
        [DataMember]
        public decimal Latitud { get; set; }
        [DataMember]
        public string IdLocalidad { get; set; }
        
        [DataMember]
        public long IdAgencia { get; set; }
        [DataMember]
        public string Telefono2 { get; set; }
        [DataMember]
        public string NumeroPase { get; set; }
        [DataMember]
        public DateTime FechaVencimientoPase { get; set; }
        [DataMember]
        public bool EsContratista { get; set; }
        [DataMember]
        public short TipoContrato { get; set; }
        [DataMember]
        public long IdPersonaInterna { get; set; }
        [DataMember]
        public bool EsMensajeroUrbano { get; set; }
        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public long IdMensajero { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string PrimerApellido { get; set; }
        [DataMember]
        public string SegundoApellido { get; set; }
        [DataMember]
        public string Telefono { get; set; }
        [DataMember]
        public int IdTipoMensajero { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public long? IdVehiculo { get; set; }
        [DataMember]
        public string placa { get; set; }
        [DataMember]
        public int? IdTipoContrato { get; set; }
        [DataMember]
        public string NombreTipoContrato { get; set; }
        [DataMember]
        public string Propiedad { get; set; }

        [DataMember]
        public string NombreCompleto { get; set; }
        [DataMember]
        public long IdPosicionMensajero { get; set; }
    }
}