using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIReclameEnOficinaDC : DataContractBase
    {
        [DataMember]
        public long IdReclameEnOficina { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public PUMovimientoInventario MovimientoInventario { get; set; }
        [DataMember]
        public int Ubicacion { get; set; }
        [DataMember]
        public PUEnumTipoUbicacion TipoUbicacion { get; set; }
        [DataMember]
        public string UbicacionDetalle { get; set; }
        [DataMember]
        public int DiasTranscurridos { get; set; }
        [DataMember]
        public bool EstadoDevolucion { get; set; }
        [DataMember]
        public CLClienteContadoDC Destinatario { get; set; }
        [DataMember]
        public bool EsAlCobro { get; set; }
        [DataMember]
        public decimal ValorTotal { get; set; }
        [DataMember]
        public decimal Peso { get; set; }
        [DataMember]
        public string DiceContener { get; set; }
        [DataMember]
        public string DocumentoDestinatario { get; set; }
        [DataMember]
        public string NombreDestinatario { get; set; }
        [DataMember]
        public string TelefonoDestinatario { get; set; }
        [DataMember]
        public string DocumentoRemitente { get; set; }
        [DataMember]
        public string NombreRemitente { get; set; }
        [DataMember]
        public string DireccionRemitente { get; set; }
        [DataMember]
        public ADGuiaFormaPago FormaPago { get; set; }
        [DataMember]
        public bool EsContado { get; set; }
        [DataMember]
        public bool Accion { get; set; }

        /// <summary>
        /// Usuario que genera la Asignacion
        /// </summary>
        [DataMember]
        public DateTime FechaAsignacion { get; set; }
        [DataMember]
        public string UsuarioAsigna { get; set; }
        [DataMember]
        public DateTime? FechaINGRESO { get; set; }

        [DataMember]
        public bool EstaEntregada { get; set; }

        /// <summary>
        /// Informativo (Asignado, Ingresado)
        /// </summary>
        [DataMember]
        public PUEnumTipoMovimientoInventario EstadoGuiaenPRO { get; set; }

        [DataMember]
        public OUEnumValidacionDescargue Respuesta { get; set; }
        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public string ImagenEvidenciaEntrega { get; set; }


    }
}
