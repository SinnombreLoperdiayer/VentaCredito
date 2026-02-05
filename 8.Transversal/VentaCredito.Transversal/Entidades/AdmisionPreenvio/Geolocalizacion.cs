using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class Geolocalizacion
    {
        public string mensaje { get; set; }
        public DataGeo data { get; set; }
        public bool success { get; set; }
    }

    public class DataGeo
    {
        public string nivsocio { get; set; }
        public string zona3 { get; set; }
        public string zona2 { get; set; }
        public string zona1 { get; set; }
        public string coddireccion { get; set; }
        public string zonapostal { get; set; }
        public string validacionPlaca { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string dirtrad { get; set; }
        public string localidad { get; set; }
        public string validacion { get; set; }
        public string estado { get; set; }
        public string barrio { get; set; }
    }
}
