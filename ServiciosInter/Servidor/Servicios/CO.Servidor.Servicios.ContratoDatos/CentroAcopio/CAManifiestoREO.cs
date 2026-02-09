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
    public class CAManifiestoREO : DataContractBase
    {

        [DataMember]
        public long IdManifiestoREO { get; set; }

        [DataMember]
        public long IdCenSerManifiesta { get; set; }
        [DataMember]
        public string NombreCenSerManifiesta { get; set; }
        
        [DataMember]
        public long IdCenSerDestino { get; set; }
        [DataMember]
        public string NombreCenSerDestino { get; set; }


        [DataMember]
        public int IdVehiculo { get; set; }
        [DataMember]
        public string PlacaVehiculo { get; set; }

        [DataMember]
        public long IdMensajero { get; set; }
        [DataMember]
        public string CedulaMensajero { get; set; }
        [DataMember]
        public string NombreMensajero { get; set; }


        [DataMember]
        public int CantidadGuias { get; set; }


        [DataMember]
        public DateTime? FechaCreacion { get; set; }
        

    }

}
