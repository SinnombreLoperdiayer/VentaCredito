using System;
using System.Runtime.Serialization;


namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGRecogidasDC
    {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public string NumeroDocumento { get; set; }

        [DataMember]
        public string Nombre { get; set; }
         
        [DataMember]
        public string Direccion { get; set; }

        [DataMember]
        public string Ciudad { get; set; }

        [DataMember]
        public string NombreCiudad { get; set; }

        [DataMember]
        public int? idCliente { get; set; }

        [DataMember]
        public int? IdCentroServicio { get; set; }

        [DataMember]
        public int? IdSucursal { get; set; }

        [DataMember]
        public string NombreSucursal { get; set; }

        [DataMember]
        public string NombreCentroServicio { get; set; }

        [DataMember]
        public string PreguntarPor { get; set; }

        [DataMember]
        public string Observaciones { get; set; }

        [DataMember]
        public DateTime FechaRecogida { get; set; }

        [DataMember]
        public RGEnumTipoRecogidaDC TipoRecogida { get; set; }

        [DataMember]
        public string CodigoVerificacion { get; set; }

        [DataMember]
        public long IdAsignacion { get; set; }

        [DataMember]
        public long IdProgramacion { get; set; }

        [DataMember]
        public string Correo { get; set; }

        [DataMember]
        public string Longitud { get; set; }

        [DataMember]
        public string Latitud { get; set; }

        [DataMember]
        public string TipoDocumento { get; set; }

        [DataMember]
        public string NumeroTelefono { get; set; }


        [DataMember]
        public string DescripcionEnvios { get; set; }

        [DataMember]
        public int TotalPiezas { get; set; }

        [DataMember]
        public int PesoAproximado { get; set; }

        [DataMember]
        public bool EstaForzada { get; set; }

        [DataMember]
        public bool EsEsporadicaCliente { get; set; }

        [DataMember]
        public decimal Cantidad { get; set; }

        [DataMember]
        public long IdHorarioCentroServicio { get; set; }
        [DataMember]
        public long IdHorarioSucursal { get; set; }

        [DataMember]
        public string DescripcionHorario { get; set; }

        [DataMember]
        public DateTime Hora { get; set; }

        [DataMember]
        public int DiaSemana { get; set; }

        [DataMember]
        public long Paginas { get; set; }
    }
}
