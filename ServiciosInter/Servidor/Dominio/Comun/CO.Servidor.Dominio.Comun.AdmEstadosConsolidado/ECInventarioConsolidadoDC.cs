using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun.AdmEstadosConsolidado
{
    
    public class ECInventarioConsolidadoDC
    {
        public long InventarioConsolidado { get; set; }

        public int IdTipoConsolidado { get; set; }

        public int TipoConsolidadoDetalle { get; set; }

        public long IdCentroServicios { get; set; }

        public string Trayecto { get; set; }

        public string Codigo { get; set; }

        public string Estado { get; set; }

        public string IdLocalidad { get; set; }

        public string NombreLocalidad { get; set; }
        
    }
}
