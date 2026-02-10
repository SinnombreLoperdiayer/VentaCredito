using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    /// <summary>
    /// Clase que contien la informacion de la
    /// persona interna
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class SEPersonaInternaDC
    {
        /// <summary>
        /// es el id de la perona interna
        /// </summary>
        [DataMember]
        public long IdPersonaInterna { get; set; }

        /// <summary>
        /// tipo de indentificacion
        /// </summary>
        [DataMember]
        public string IdTipoIdentificacion { get; set; }

        /// <summary>
        /// numero de identificacion
        /// </summary>
        [DataMember]
        public string Identificacion { get; set; }

        /// <summary>
        /// es le id del cargo de la persona interna
        /// </summary>
        [DataMember]
        public int IdCargo { get; set; }

        /// <summary>
        /// nombre de la perosna Interna
        /// </summary>
        [DataMember]
        public string Nombre { get; set; }

        /// <summary>
        /// Apellidos del Usuario
        /// </summary>
        [DataMember]
        public string PrimerApellido { get; set; }

        /// <summary>
        /// Apellidos del Usuario
        /// </summary>
        [DataMember]
        public string SegundoApellido { get; set; }

        /// <summary>
        /// Es el Nombre Con Apellidos
        /// </summary>
        [DataMember]
        public string NombreCompleto { get; set; }

        /// <summary>
        /// direccion de la persona interna
        /// </summary>
        [DataMember]
        public string Direccion { get; set; }

        /// <summary>
        /// minicipio de la persona interna
        /// </summary>
        [DataMember]
        public string Municipio { get; set; }

        /// <summary>
        /// numero telefonico de la persona interna
        /// </summary>
        [DataMember]
        public string Telefono { get; set; }

        /// <summary>
        /// direccionde correo de la persona interna
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// regiona a la que pertenece la persona interna
        /// </summary>
        [DataMember]
        public long? IdRegional { get; set; }

        /// <summary>
        /// comentarioa a lugar
        /// </summary>
        [DataMember]
        public string Comentarios { get; set; }
    }
}