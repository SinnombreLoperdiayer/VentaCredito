namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Parametros
{
    /// <summary>
    /// Esta clase es para la migracion del servidor nodeJS a WebApi, los nombres de las columnas se manejan de esa forma ya que la aplicacion los espera así.
    /// </summary>
    public class ResponseGenericoApp
    {
        public string value { get; set; }

        public string label { get; set; }
    }
}