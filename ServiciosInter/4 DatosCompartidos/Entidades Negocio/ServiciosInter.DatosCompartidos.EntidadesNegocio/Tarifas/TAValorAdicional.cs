namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas
{
    public class TAValorAdicional
    {
        public string IdTipoValorAdicional { get; set; }

        public string Descripcion { get; set; }

        public TAServicioDC Servicio { get; set; }

        public int IdServicio { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>

        // public EnumEstadoRegistro EstadoRegistro { get; set; }

        /// <summary>
        /// Lista con los valores adicionales que puede utilizar el servicio de giros
        /// </summary>

        // public IList<TACampoTipoValorAdicionalDC> CamposTipoValorAdicionalDC { get; set; }

        /// <summary>
        /// Almacena los precios por valor adicional para un servicio
        /// </summary>

        public decimal PrecioValorAdicional { get; set; }

        public bool EsEmbalaje { get; set; }
    }
}