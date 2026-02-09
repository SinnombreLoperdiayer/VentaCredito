using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
    /// <summary>
    /// Clase que contiene la información de un ajuste de caja de un banco
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAAjusteCajaBancoDC
    {
        [DataMember]
        public CAConceptoCajaDC ConceptoCaja { get; set; }

        [DataMember]
        public ARCuentaCasaMatrizDC CuentaOrigen { get; set; }

        [DataMember]
        public ARCuentaCasaMatrizDC CuentaDestino { get; set; }

        [DataMember]
        public decimal Valor{get;set;}

        [DataMember]
        public string NoConsignacion{get;set;}

        [DataMember]
        public string Observacion{get;set;}

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        /// <summary>
        /// Número de comprobante asociado cuando es un ajuste x 
        /// </summary>
        [DataMember]
        public string NumeroComprobante { get; set; }
    }
}
