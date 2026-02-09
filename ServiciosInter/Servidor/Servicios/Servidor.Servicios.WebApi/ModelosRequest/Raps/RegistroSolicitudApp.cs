using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.Raps
{
    [DataContract]
    public class RegistroSolicitudApp
    {
        [DataMember]
        public string IdCiudad { get; set; }

        [DataMember]
        public int IdSistema { get; set; }

        [DataMember]
        public int IdTipoNovedad { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public string NombreCompleto { get; set; }

        [DataMember]
        public DateTime FechaRegistro { get; set; }

        [DataMember]
        public int Motivo { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string Foto { get; set; }

        [DataMember]
        public long IdCentroServicio { get; set; }

        [DataMember]
        public string Observacion { get; set; }

        [DataMember]
        public string Adjunto { get; set; }

        [DataMember]
        public bool EsPersonalizado { get; set; }

        [DataMember]
        public Dictionary<string, object> Parametros;

    }
}