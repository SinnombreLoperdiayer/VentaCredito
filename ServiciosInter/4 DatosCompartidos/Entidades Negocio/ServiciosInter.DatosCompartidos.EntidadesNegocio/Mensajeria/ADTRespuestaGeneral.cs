namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    /// <summary>
    /// Entidad de respuesta general
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ADTRespuestaGeneral<T>
    {
        /// <summary>
        /// retorna o asigna el valor del error
        /// </summary>
        public bool error { get; set; }
        /// <summary>
        /// retorna o asigna el mensaje de la petición
        /// </summary>
        public string mensaje { get; set; }
        /// <summary>
        /// retorna o asigna el objeto a devolver
        /// </summary>
        public T resultado { get; set; }
    }
}
