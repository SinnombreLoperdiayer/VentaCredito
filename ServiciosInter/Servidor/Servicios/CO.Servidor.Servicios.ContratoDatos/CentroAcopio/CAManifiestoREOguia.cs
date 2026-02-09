using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.CentroAcopio
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAManifiestoREOguia : DataContractBase
    {

        [DataMember]
        public long IdManifiesto { get; set; }

        [DataMember]
        public long IdManifiestoREOGuia { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public long IdAdmision { get; set; }

        //[DataMember]
        //public long IdCentroServicioCOL { get; set; }
        //[DataMember]
        //public string NombreCentroServicioCOL { get; set; }
        //[DataMember]
        //public string IdMunicipioCOL { get; set; }

        //[DataMember]
        //public long IdCentroServicioDestino { get; set; }
        //[DataMember]
        //public string NombreCentroServicioDestino { get; set; }

        //[DataMember]
        //public decimal Peso { get; set; }
        //[DataMember]
        //public string TipoEnvio { get; set; }
        //[DataMember]
        //public string DiceContener { get; set; }
        //[DataMember]
        //public string DireccionDestino { get; set; }

        //[DataMember]
        //public DateTime? FechaAsignacion { get; set; }
        //[DataMember]
        //public bool EstaAsignada { get; set; }

        //[DataMember]
        //public OUEnumValidacionDescargue Respuesta { get; set; }

        //[DataMember]
        //public string Mensaje { get; set; }

        //[DataMember]
        //public ADTrazaGuia Estado { get; set; }

        //[DataMember]
        //public PALocalidadDC LocalidadDestino { get; set; }



    }

}
