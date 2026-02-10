using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
    /// <summary>
    /// Clase que contiene la informacion los mensajeros
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class POPersonaExterna : DataContractBase
    {
        [DataMember]
        public long IdPersona { get; set; }
        [DataMember]
        public string IdTipoIdentificacion { get; set; }
	    [DataMember]
        public string Identificacion { get; set; }
	    [DataMember]
        public string DigitoVerificacion { get; set; }
	    [DataMember]
	    public Nullable<DateTime> FechaExpedicionDocumento { get; set; }
        [DataMember]
        public string PrimerNombre { get; set; }
	    [DataMember]
        public string SegundoNombre { get; set; }
	    [DataMember]
        public string PrimerApellido { get; set; }
	    [DataMember]
        public string SegundoApellido { get; set; }
	    [DataMember]
        public string Direccion { get; set; }
	    [DataMember]
        public string Municipio { get; set; }
	    [DataMember]
        public string Telefono { get; set; }
	    [DataMember]
        public string NumeroCelular { get; set; }
	    [DataMember]
        public Nullable<DateTime> FechaGrabacion { get; set; }
	    [DataMember]
        public string CreadoPor { get; set; }
        [DataMember]
        public string Email { get; set; }
    }

}
