using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUDatosMensajeroDC : DataContractBase
    {
        [DataMember]
        public long IdMensajero { get; set; }

        [DataMember]
        public int IdTipoMensajero { get; set; }

        [DataMember]
        public long IdAgencia { get; set; }

        [DataMember]
        public string Telefono2 { get; set; }

        [DataMember]
        public DateTime FechaIngreso { get; set; }

        [DataMember]
        public DateTime FechaTerminacionContrato { get; set; }

        [DataMember]
        public string NumeroPase { get; set; }

        [DataMember]
        public DateTime FechaVencimientoPase { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public bool EsContratista { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool EsVehicular { get; set; }

        [DataMember]
        public long IdPersonaInterna { get; set; }

        [DataMember]
        public string IdTipoIdentificacion { get; set; }

        [DataMember]
        public string Identificacion { get; set; }

        [DataMember]
        public int IdCargo { get; set; }

        [DataMember]
        public string NombreMensajero { get; set; }

        [DataMember]
        public string PrimerApellido { get; set; }

        [DataMember]
        public string SegundoApellido { get; set; }

        [DataMember]
        public string DireccionMensajero { get; set; }

        [DataMember]
        public string Municipio { get; set; }

        [DataMember]
        public string Telefono { get; set; }

        [DataMember]
        public string EmailMensajero { get; set; }

        [DataMember]
        public long IdRegionalAdm { get; set; }

        [DataMember]
        public string Comentarios { get; set; }

        [DataMember]
        public long IdCentroServicios { get; set; }

        [DataMember]
        public string NombreCentroServicio { get; set; }

        [DataMember]
        public string Telefono1 { get; set; }

        [DataMember]
        public string DireccionCentroServicio { get; set; }

        [DataMember]
        public string IdMunicipio { get; set; }

        [DataMember]
        public string NombreLocalidad { get; set; }

        [DataMember]
        public int TipoContrato { get; set; }

    }
}
