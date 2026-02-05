using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class DestinatarioFrecuente_CLI
    {
        public long CLC_IdClienteContado { get; set; }
        public string CLC_Identificacion { get; set; }
        public string CLC_ZonaPostal { get; set; }
        public string CLC_Direccion { get; set; }
        public string CLC_MicroZona { get; set; }
        public string CLC_MacroZona { get; set; }
        public string CLC_Latitude { get; set; }
        public string CLC_Longitude { get; set; }
        public string CLC_IdLocalidad { get; set; }
        public string CLC_DirNormalizada { get; set; }
        public string CLC_Zona1 { get; set; }
        public string CLC_EstadoGeo { get; set; }
        public string CLC_Localidad { get; set; }
        public string CLC_Barrio { get; set; }
    }
}
