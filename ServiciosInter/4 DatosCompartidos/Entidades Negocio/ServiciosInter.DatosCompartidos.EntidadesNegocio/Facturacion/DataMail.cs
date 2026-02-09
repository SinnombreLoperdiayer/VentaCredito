using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Facturacion
{
    public class DataMail
    {
        public string NombreRemitente { get; set; }
        public string NombreDestinatario { get; set; }
        public string Servicio { get; set; }
        public string FacturaVenta { get; set; }
        public string FechaAdmision { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Numero { get; set; }
    }
}
