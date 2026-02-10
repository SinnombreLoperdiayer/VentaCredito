using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas
{
    public class TAValorPesoDeclaradoDC
    {
        public int IdValorPesoDeclarado { get; set; }
        public int IdListaPrecio { get; set; }
        public decimal PesoInicial { get; set; }
        public decimal PesoFinal { get; set; }
        public decimal ValorMinimoDeclarado { get; set; }
        public decimal ValorMaximoDeclarado { get; set; }
    }
}
