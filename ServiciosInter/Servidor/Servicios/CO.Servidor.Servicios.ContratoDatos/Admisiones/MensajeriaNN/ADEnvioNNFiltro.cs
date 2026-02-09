using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class AdEnvioNNFiltro: DataContractBase
    {
        [DataMember]
        public int Indicador { get; set; }
        [DataMember]
        public long NumeroMensajeriaNN { get; set; }
        [DataMember]
        public DateTime FechaInicio { get; set; }
        [DataMember]
        public DateTime FechaFin { get; set; }
        [DataMember]
        public string CiudadOrigen { get; set; }
        [DataMember]
        public string CiudadDestino { get; set; }
        [DataMember]
        public string LocalidadCaptura { get; set; }

        
        [DataMember]
        public string NombreRemitente { get; set; }
        [DataMember]
        public string NombreDestinatario { get; set; }
        [DataMember]
        public string IdentificacionRemitente { get; set; }
        [DataMember]
        public string IdentificacionDestinatario { get; set; }
        [DataMember]

        public string DireccionRemitente { get; set; }
        [DataMember]
        public string DireccionDestinatario { get; set; }
        [DataMember]
        public string DiceContener { get; set; }
        [DataMember]
        public string TelefonoDestinatario { get; set; }
        [DataMember]
        public string TelefonoRemitente { get; set; }
    
        [DataMember]
        public string CorreoRemitente { get; set; }
        [DataMember]
        public string CorreoDestinatario { get; set; }
        [DataMember]
        public string NumeroBolsaSeguridad { get; set; }
         
        [DataMember]
        public PUCentroServiciosDC CentroServicio { get; set; }
        [DataMember]
        public int IdOperativo { get; set; }
        [DataMember]
        public int CantidadPiezas { get; set; }
        [DataMember]
        public string DescripcionEmpaque { get; set; }
        [DataMember]
        public ClasificacionEnvioNN Clasificacion { get; set; }
        [DataMember]
        public TAFormaPago FormaPago { get; set; }
        [DataMember]
        public RURutaDC Operativo { get; set; }

        [DataMember]
        public int Pagina { get; set; }

        [DataMember]
        public int NRegistros { get; set; }

        [DataMember]
        public string Filtro { get; set; }
    }

}
