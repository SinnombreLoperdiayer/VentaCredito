using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGAsignarRecogidaDC
    {
        [DataMember]
        public long IdSolicitudRecogida { get; set; }

        [DataMember]
        public string LocalidadCambio { get; set; }

        [DataMember]
        public string Longitud { get; set; }

        [DataMember]
        public string Latitud { get; set; }

        [DataMember]
        public long IdMotivo { get; set; }

        [DataMember]
        public string DescripcionMotivo { get; set; }

        [DataMember]
        public string DocPersonaResponsable { get; set; }

        [DataMember]
        public string PlacaVehiculo { get; set; }

        [DataMember]
        public COEnumIdentificadorAplicacion IdAplicacion { get; set; }

        [DataMember]
        public EnumEstadoSolicitudRecogida EstadoRecogida { get; set; }

        [DataMember]
        public int NumeroPiezas { get; set; }
        [DataMember]
        public List<String> FotografiasRecogida { get; set; }
        [DataMember]
        public bool TieneCodigoQR { get; set; }

        [DataMember]
        public RGEnumTipoRecogidaDC TipoRecogida { get; set; }

        [DataMember]
        public DateTime FechaEscaneoQR { get; set; }

        [DataMember]
        public DateTime FechaProgramacionRecogida { get; set; }

        [DataMember]
        public DateTime FechaEjecucionRecogida { get; set; }

        [DataMember]
        public string IdCliente { get; set; }

        [DataMember]
        public string NombreCliente { get; set; }

        [DataMember]
        public string DireccionCliente { get; set; }     
        
        [DataMember]
        public string Observacion { get; set; }

        [DataMember]
        public string IdCiudad { get; set; }

        public OUDatosMensajeroDC Mensajero { get; set; }


    }
}
