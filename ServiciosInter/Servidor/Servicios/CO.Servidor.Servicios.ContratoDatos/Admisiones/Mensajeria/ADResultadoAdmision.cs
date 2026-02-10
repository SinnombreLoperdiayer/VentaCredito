using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract]
    public class ADResultadoAdmision
    {
        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public long IdAdmision { get; set; }

        /// <summary>
        /// Indica si se debe generar advertencia de porcentaje de cupo superado. Aplica para cliente crédito
        /// </summary>
        [DataMember]
        public bool? AdvertenciaPorcentajeCupoSuperadoClienteCredito { get; set; }

        [DataMember]
        public string DireccionAgenciaCiudadOrigen { get; set; }

        [DataMember]
        public string DireccionAgenciaCiudadDestino { get; set; }

        // todo:id Retorna aqui el pdf de la Guia
        [DataMember]
        public byte[] pdfBytes{ get; set; }

        [DataMember]
        public string MensajeRta { get; set; }
    }

}
