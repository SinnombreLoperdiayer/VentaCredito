
namespace CO.Servidor.Servicios.WebApi.ModeloResponse.Raps
{
    public class NombresParametrizacionRaps
    {
        public long IdParametrizacionRap { get; set; }
        public string Nombre { get; set; }
        public bool Estado { get; set; }
        public long IdParametrizacionPadre { get; set; }
    }
}