namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    /// <summary>
    /// Entidad de petición de solicitud
    /// </summary>
    public class ADRastreoGuiaIVRSolicitud
    {
        /// <summary>
        /// retorna o asigna el numero de guia.
        /// </summary>
        public string NumeroGuia { get; set; }

        /// <summary>
        /// retorna o asigna el numero de telefono.
        /// </summary>
        public string NumeroTelefono { get; set; }
    }
}
