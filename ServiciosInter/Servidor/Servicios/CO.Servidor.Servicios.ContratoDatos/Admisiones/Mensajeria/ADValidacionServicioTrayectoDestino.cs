using System.Collections.Generic;
using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    /// <summary>
    /// Contiene información resultado de la validación de servicio dados el trayecto y el destino
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADValidacionServicioTrayectoDestino : DataContractBase
    {
        [DataMember]
        public int DuracionTrayectoEnHoras { get; set; }

        [DataMember]
        public decimal PesoMaximoTrayectoOrigen { get; set; }

        [DataMember]
        public decimal VolumenMaximoOrigen { get; set; }

        [DataMember]
        public decimal VolumenMaximoDestino { get; set; }

        [DataMember]
        public decimal PesoMaximoTrayectoDestino { get; set; }

        [DataMember]
        public long IdCentroServiciosDestino { get; set; }

        [DataMember]
        public string DireccionCentroServiciosDestino { get; set; }

        [DataMember]
        public string TelefonoCentroServiciosDestino { get; set; }

        [DataMember]
        public string NombreCentroServiciosDestino { get; set; }

        [DataMember]
        public IEnumerable<TAValorAdicional> ValoresAdicionales { get; set; }

        [DataMember]
        public long IdCentroServiciosOrigen { get; set; }

        [DataMember]
        public string NombreCentroServiciosOrigen { get; set; }

        [DataMember]
        public bool DestinoAdmiteFormaPagoAlCobro { get; set; }

        [DataMember]
        public string CodigoPostalDestino { get; set; }

        [DataMember]
        public int? IdOperadorPostalDestino { get; set; }

        [DataMember]
        public string IdZonaOperadorPostalDestino { get; set; }

        [DataMember]
        public string NombreCiudadDestino { get; set; }

        [DataMember]
        public ADRangoTrayecto InfoCasillero { get; set; }

        [DataMember]
        public int NumeroHorasDigitalizacion { get; set; }

        [DataMember]
        public int NumeroHorasArchivo { get; set; }

        [DataMember]
        public string NombreRepLegal { get; set; }

        [DataMember]
        public string IdentificacionRepLegal { get; set; }
    }

}