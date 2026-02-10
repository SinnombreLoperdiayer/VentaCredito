namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADGuiaFormaPago
    {
        public short IdFormaPago { get; set; }
        public decimal Valor { get; set; }
        public string Descripcion { get; set; }

        /// <summary>
        /// Es el numero asociado a la forma de pago.
        /// Pricipalmente para el PinPrepago y
        /// Cheque.
        /// </summary>
        /// <value>
        /// numero asociado
        /// </value>
        public string NumeroAsociadoFormaPago { get; set; }
    }
}