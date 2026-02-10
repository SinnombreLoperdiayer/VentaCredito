using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Mensajero
{
    public class MEGrupoRodamiento
    {
        public int Id { get; set; }

        public string RodamientoMensajero { get; set; }
        public string IdCiudad { get; set; }

        public int IdZona { get; set; }

     
        public string NombreTipoCiudad { get; set; }

        public string NombreZona { get; set; } 
        
        public List<METipoRodamiento> TipoRodamiento { get; set; }

        public DateTime FechaInicial { get; set; }

        public DateTime FechaFinal { get; set; }

        public bool Estado { get; set; }

        public int IdTipoCiudad { get; set; }

        public int TotalPaginas { get; set; }

    }
}
