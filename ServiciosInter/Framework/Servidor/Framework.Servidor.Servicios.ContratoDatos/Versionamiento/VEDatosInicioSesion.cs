using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class VEDatosInicioSesion : DataContractBase
    {
        [DataMember]
        public VEDatosCentroServicio DatosCentroServicio { get; set; }

        [DataMember]
        public long CodigoGestion { get; set; }

        [DataMember]
        public string NombreGestion { get; set; }

        /// <summary>
        /// Cargo del usuario autenticado
        /// </summary>
        [DataMember]
        public Agenda.ASCargo Cargo { get; set; }

        /// <summary>
        /// Nombres del usuario que se autenticó
        /// </summary>
        [DataMember]
        public string NombresUsuario { get; set; }

        /// <summary>
        /// Apellidos del usuario que se autenticó
        /// </summary>
        [DataMember]
        public string ApellidosUsuario { get; set; }

        [DataMember]
        public long IdUsuario { get; set; }

        /// <summary>
        /// Nombres del usuario que se autenticó
        /// </summary>
        [DataMember]
        public string DocumentoUsuario { get; set; }

        /// <summary>
        /// Indica si el usuario que se está autenticando es un cajero ppal.
        /// </summary>
        [DataMember]
        public bool EsCajeroPpal { get; set; }

        /// <summary>
        /// Fecha del servidor
        /// </summary>
        [DataMember]
        public System.DateTime FechaServidor { get; set; }

        /// <summary>
        /// Nombre corto del País que debe ser cargado por defecto
        /// </summary>
        [DataMember]
        public string NomCortoPaisPorDefecto { get; set; }

        /// <summary>
        /// País por defecto
        /// </summary>
        [DataMember]
        public string IdPaisPorDefecto { get; set; }

        [DataMember]
        public string DescPaisPorDefecto { get; set; }

        [DataMember]
        public VEDatosClienteCredito DatosClienteCredito { get; set; }

        [DataMember]
        public int IdCasaMatriz { get; set; }


        [DataMember]
        public string URLKompadreStereo { get; set; }
    }
}