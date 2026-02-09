using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Facturacion
{
    public class MailRequest
    {
        public string destinatario { get; set; }
        public byte[] adjunto { get; set; }
        public Dictionary<string, object> parametrosReplace { get; set; }
        public string nombreArchivo { get; set; }
    }
}
