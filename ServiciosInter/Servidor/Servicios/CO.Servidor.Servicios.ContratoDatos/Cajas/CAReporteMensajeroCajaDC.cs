using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Clase que contiene los Valores
    /// de la Tabla Reporte Mensajero Caja
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAReporteMensajeroCajaDC : DataContractBase
    {
        /// <summary>
        /// Es el Id unico del Registro reporte mensajero.
        /// </summary>
        [DataMember]
        public long IdRegistroTransDetalleCaja { get; set; }

        /// <summary>
        /// Los Datos del mensajero.
        /// </summary>
        [DataMember]
        public OUNombresMensajeroDC Mensajero { get; set; }

        /// <summary>
        /// La fecha en la que se graba el registro.
        /// </summary>
        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        /// <summary>
        /// Es el usuario quer realiza el registro.
        /// </summary>
        [DataMember]
        public string UsuarioRegistro { get; set; }

        /// <summary>
        /// Numero de comprobante de la transaccion detalle caja
        /// </summary>
        [DataMember]
        public string NumeroComprobanteTransDetCaja { get; set; }
        
    }
}