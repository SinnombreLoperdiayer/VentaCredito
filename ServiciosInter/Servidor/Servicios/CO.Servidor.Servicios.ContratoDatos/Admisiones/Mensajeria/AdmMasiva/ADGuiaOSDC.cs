using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADGuiaOSDC: DataContractBase
    {
        [DataMember]
        public ADGuia Guia { get; set; }

        [DataMember]
        public bool SonGuiasInternas { get; set; }

        [DataMember]
        public ADMensajeriaTipoCliente DatosRemitenteDestinatario { get; set; }

        /// <summary>
        /// Es el numero de la
        /// orden de servicio
        /// </summary>
        [DataMember]
        public long NoOrdenServicio { get; set; }

        [DataMember]
        public int IdCaja { get; set; }

        [DataMember]
        public int NoFila { get; set; }

        /// <summary>
        /// Es la fecha en la que se crea
        /// la orden de servicio
        /// </summary>
        [DataMember]
        public DateTime FechaOrdenServicio { get; set; }

        /// <summary>
        /// Es el estado de la Orden de Servicio
        /// ACT (Activa) - INA (Inactiva)
        /// </summary>
        [DataMember]
        public string EstadoOrdenServicio { get; set; }
    }
}