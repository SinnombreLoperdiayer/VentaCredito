using System.Collections.Generic;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADFormatoImpresion
    {
        public string NameFile { get; set; }
        public string Template { get; set; }
        public string BarCode { get; set; }
        public int Print { get; set; }
        public Dictionary<string, object> ItemsFormat { get; set; }
    }
}
