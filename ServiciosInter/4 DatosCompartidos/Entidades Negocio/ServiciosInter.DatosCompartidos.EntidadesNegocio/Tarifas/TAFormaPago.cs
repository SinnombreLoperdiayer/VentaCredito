namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas
{
    public class TAFormaPago
    {
        public short IdFormaPago { get; set; }

        public string Descripcion { get; set; }

        /// <summary>
        /// Propiedad establecida para binding en Parametrizacion de Servicios Novasoft
        /// </summary>
        //public int IdFormaPagoInt { get; set; }

        //public bool Asignada { get; set; }

        //public bool Actual { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        //public EnumEstadoRegistro EstadoRegistro { get; set; }

        /// <summary>
        /// Indica si la forma de pago acepta mixto
        /// </summary>
        //public bool AceptaMixto { get; set; }

        //public decimal Valor { get; set; }

        /// <summary>
        /// Indica si la forma aplica para cliente
        /// </summary>
        //public bool AplicaFactura { get; set; }

        /// <summary>
        /// Es el numero asociado a la forma de pago.
        /// Pricipalmente para el PinPrepago y
        /// Cheque.
        /// </summary>
        /// <value>
        /// Es el numero asociado a la forma de pago.
        /// Pricipalmente para el PinPrepago y
        /// Cheque.
        /// </value>
        //public string NumeroAsociadoFormaPago { get; set; }

        //  public System.Collections.Generic.List<TAServicioDC> ServiciosAsociados { get; set; }
    }
}