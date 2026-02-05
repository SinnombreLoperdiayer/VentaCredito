using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class PUCentroServiciosDC
    {

        public string Direccion { get; set; }
        public long IdCentroServicio { get; set; }
        public string IdMunicipio { get; set; }
        public string Nombre { get; set; }
        public string NombreMunicipio { get; set; }
        public decimal PesoMaximo { get; set; }
        public string Tipo { get; set; }
        public string TipoSubtipo { get; set; }
    }
}
