using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Clase que contiene los datos que se deben imprimir cuando un mensajero reporta el dinero
    /// de sus ventas y recaudo de al cobros.
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CADatosImpCompMensajeroDC
    {
        [DataMember]
        public string NombreEmpresa { get; set; }

        [DataMember]
        public string NitEmpresa { get; set; }

        [DataMember]
        public long ConsecutivoComprobante { get; set; }

        [DataMember]
        public string NombrMensajero { get; set; }

        [DataMember]
        public string CedulaMensajero { get; set; }

        [DataMember]
        public List<CADatosMovimientoDC> MovimientosAgencia { get; set; }

        [DataMember]
        public List<CADatosMovimientoDC> MovmientosMensajero { get; set; }

        [DataMember]
        public DateTime FechaActual { get; set; }

        /// <summary>
        /// Datos del Mensajero que se realiza el descargue
        /// </summary>
        [DataMember]
        public OUNombresMensajeroDC Mensajero { get; set; }

        /// <summary>
        /// Es la lista de las entregas alcobro del mensajero
        /// </summary>
        [DataMember]
        public List<OUEnviosPendMensajerosDC> ListEntregasAlCobroMensajero { get; set; }
    }

    [DataContract(Namespace = "http://contrologis.com")]
    public class CADatosMovimientoDC
    {
        [DataMember]
        public string NombreConceptoCaja { get; set; }

        [DataMember]
        public int IdConceptoCaja { get; set; }

        [DataMember]
        public decimal ValorOperacion { get; set; }

        [DataMember]
        public string NumeroOperacion { get; set; }
    }
}