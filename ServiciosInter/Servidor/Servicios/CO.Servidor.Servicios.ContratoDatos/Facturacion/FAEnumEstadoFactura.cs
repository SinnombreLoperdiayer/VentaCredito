using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
    /// <summary>
    /// Contiene los estados de
    /// la Factura
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public enum FAEnumEstadoFactura
    {
        /// <summary>
        /// Factura Anulada
        /// </summary>
        [EnumMember]
        ANU,

        /// <summary>
        /// Factura Anulada
        /// </summary>
        [EnumMember]
        Anulada,

        /// <summary>
        /// Factura Automatica
        /// </summary>
        [EnumMember]
        AUT,

        /// <summary>
        /// Factura Atomatica
        /// </summary>
        [EnumMember]
        Automatica,

        /// <summary>
        /// Factura Aprobada
        /// </summary>
        [EnumMember]
        APR,

        /// <summary>
        /// Factura Aprobada
        /// </summary>
        [EnumMember]
        Aprobada,

        /// <summary>
        /// Factura Creada
        /// </summary>
        [EnumMember]
        CRE,

        /// <summary>
        /// Factura Anulada
        /// </summary>
        [EnumMember]
        Creada,
    }
}