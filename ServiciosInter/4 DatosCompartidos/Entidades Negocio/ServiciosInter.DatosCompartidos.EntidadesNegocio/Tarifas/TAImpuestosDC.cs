namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas
{
    public class TAImpuestosDC
    {
        /// <summary>
        /// Retorna o asigna el identificador del impuesto
        /// </summary>

        public short Identificador { get; set; }

        /// <summary>
        /// Retorna a asigna la descripción del impuesto
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Retorna o asigna el valor del impuesto asignado al servicio
        /// </summary>
        public decimal Valor { get; set; }
    }
}