using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Seguridad
{
    /// <summary>
    /// Clase para contener la informacion del usuario consultado 
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class SEGUsuarioDC
    {
        [DataMember]
        public int Registro { get; set; }

        [DataMember]
        public string IdUsuario { get; set; }

        [DataMember]
        public string LoginUsuario { get; set; }

        [DataMember]
        public bool Estado { get; set; }

        [DataMember]
        public DateTime FechaCreacion { get; set; }

        [DataMember]
        public string IdDocumento { get; set; }

        [DataMember]
        public string NumeroIdentificacion { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public DateTime Ingreso { get; set; }

        [DataMember]
        public DateTime Egreso { get; set; }

        [DataMember]
        public String Correo { get; set; }

        [DataMember]
        public String Celular { get; set; }

    }
}
