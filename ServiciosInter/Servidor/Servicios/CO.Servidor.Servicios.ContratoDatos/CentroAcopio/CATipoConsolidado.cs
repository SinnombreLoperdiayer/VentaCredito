using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroAcopio
{

    [DataContract(Namespace = "http://contrologis.com")]
    public class CATipoConsolidado : DataContractBase
    {
        [DataMember]
        public int IdTipoConsolidado { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public string NumeroConsolidado { get; set; }

        [DataMember]
        public long IdCentroServicioPropietario { get; set; }

        [DataMember]
        public string NombreCentroServicioPropietario { get; set; }

        [DataMember]
        public string NombreRacolPropietario { get; set; }

        [DataMember]
        public long IdCentroServicioDestino { get; set; }

        [DataMember]
        public string NombreCentroServicioDestino { get; set; }


        [DataMember]
        public string NombreRacolDestino { get; set; }


        [DataMember]
        public long IdRacolDestino { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public bool Estado { get; set; }

        [DataMember]
        public PUCentroServiciosDC CentroServicioDestinoSeleccionado { get; set; }
       
    }
}
