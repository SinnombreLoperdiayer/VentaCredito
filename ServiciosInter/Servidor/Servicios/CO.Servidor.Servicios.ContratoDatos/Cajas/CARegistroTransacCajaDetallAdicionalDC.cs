using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Clase que contiene los campos del detalle
    /// de la caja Adicionales generalmente para las transacciones
    /// de las cajas principales
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CARegistroTransacCajaDetallAdicionalDC : DataContractBase
    {
        /// <summary>
        /// es el id de la transaccion enlazada al detalle de la
        /// operacion de caja
        /// </summary>
        [DataMember]
        public long IdRegistroTransDetalle { get; set; }

        /// <summary>
        /// en caso de una transaccion con una sucursal
        /// se guarda el id de la sucursal
        /// </summary>
        [DataMember]
        public int? IdSucursal { get; set; }

        /// <summary>
        /// valor comodin puede ser el numero de documento de un cliente
        /// o nombre del mismo
        /// </summary>
        [DataMember]
        public string Adicional01 { get; set; }

        [DataMember]
        public string Adicional02 { get; set; }

        [DataMember]
        public string Adicional03 { get; set; }
    }
}