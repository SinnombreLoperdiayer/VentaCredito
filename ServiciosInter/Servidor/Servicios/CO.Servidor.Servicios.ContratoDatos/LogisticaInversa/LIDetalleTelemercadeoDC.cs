using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    /// <summary>
    /// Detalle de Telemercadeo al observar el flujo de la guia
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIDetalleTelemercadeoDC: DataContractBase
    {
        [DataMember]
        public long IdGestionGuiaTelemercadeo { get; set; }

        [DataMember]
        public string DescripcionTelemercadeo { get; set; }

        [DataMember]
        public string TelefonoMarcado { get; set; }

        [DataMember]
        public string PersonaContesta { get; set; }

        [DataMember]
        public int IdResultadoTelemercadeo { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string Observacion { get; set; }

        [DataMember]
        public string NuevaDireccionEnvio { get; set; }

        [DataMember]
        public int IdParentestoConDestinatari { get; set; }

        [DataMember]
        public string Parentesco { get; set; }

        [DataMember]
        public string NuevoTelefono { get; set; }

        [DataMember]
        public string NuevoContaco { get; set; }

        [DataMember]
        public long IdEstadoGuiaLog { get; set; }

        [DataMember]
        public string DescripcionEstado { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }


    }
}
