using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Mensajero
{
    public class MEUnidadNegocioFormaPago
    {
        public METiposLiquidacion TipoLiquidacion { get; set; }

        public int IdUnidadForma { get;set;}
        public string IdUnidad { get; set; }
        public string UnidadNegocio { get; set; }
        public string Descripcion { get; set; }
        public short IdFormaPago { get; set; }
        public string FormaPago { get; set; }
        public int IdTipoAccion { get; set; }
        public string TipoAccion { get; set; }
        public int TotalPaginas { get; set; }
    }
}
