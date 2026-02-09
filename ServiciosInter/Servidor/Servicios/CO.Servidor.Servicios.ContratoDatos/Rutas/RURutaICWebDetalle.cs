using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{
    //public class RURutaICWebDetalle 
    //{
    //    public List<RURutaICWeb> _RutaDetalle { get; set; }
    //    public List<RURutaCWebDetalleCentrosServicios> _RutaPtosControl { get; set; }
    //}

    [DataContract(Namespace="http://contrologis.com") ]
    public class RURutaICWeb : DataContractBase
    {
        [DataMember]
        public int IdRuta { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string IdLocalidadOrigen { get; set; }
        [DataMember]
        public string NombreLocalidadOrigen { get; set; }
        [DataMember]
        public string PosOrigen { get; set; }
        [DataMember]
        public string IdLocalidadDestino { get; set; }
        [DataMember]
        public string NombreLocalidadDestino { get; set; }
        [DataMember]
        public string PosDestino { get; set; }
        [DataMember]
        public int GeneraManifiesto { get; set; }
        [DataMember]
        public string CreadoPor { get; set; }
        [DataMember]
        public double CostoMensualTotal { get; set; }
        [DataMember]
        public bool RutaMasivos { get; set; }
        [DataMember]
        public int IdTipoRuta { get; set; }
        [DataMember]
        public string NombreTipoRuta { get; set; }
        [DataMember]
        public short IdMedioTransporte { get; set; }
        [DataMember]
        public string DescMedioTransporte { get; set; }
        [DataMember]
        public short IdTipoVehiculo { get; set; }
        [DataMember]
        public string DescTipoVehiculo { get; set; }
    }

    
   
   
}
