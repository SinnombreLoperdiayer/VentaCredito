using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADGuiaInternacionalDC : DataContractBase
    {

        [DataMember]
        public long IdAdmision { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public long NumeroGuiaDHL { get; set; }

        [DataMember]
        public DateTime FechaEstimadaEntregaDHL { get; set; }

        [DataMember]
        public string IdPaisDestino { get; set; }

        [DataMember]
        public string NombrePaisDestino { get; set; }

        [DataMember]
        public string IdDivPolitica { get; set; }

        [DataMember]
        public string NombreDivPolitica { get; set; }

        [DataMember]
        public string IdCiudadDestino { get; set; }

        [DataMember]
        public string NombreCiudadDestino { get; set; }

        [DataMember]
        public string CodigoPostalDestino { get; set; }

        [DataMember]
        public string DireccionDestinatario { get; set; }


        [DataMember]
        public byte[] arrByteXMLresponse { get; set; }


        [DataMember]
        public string RequestTarifa { get; set; }
        [DataMember]
        public string ResponseTarifa { get; set; }

        [DataMember]
        public string RequestGuia { get; set; }
        [DataMember]
        public string ResponseGuia { get; set; }


        [DataMember]
        public int IdTipoEmpaque { get; set; }
        [DataMember]
        public string TipoEmpaqueNombre { get; set; }

    }
}
