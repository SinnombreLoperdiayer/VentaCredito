using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class TATarifasContraPago
    {
        public decimal ValorServicioContraPago { get; set; }

        public decimal PorcentajeServicioContraPago { get; set; }

        public decimal porcentajeimpuesto { get; set; }

        public decimal ValorPrima { get; set; }

        public int IdconceptoCaja { get; set; }

        public string IdunidadNegocio { get; set; }

        public TAFormaPago FormasPagoHabilitadasContraPago { get; set; }


        public TAServicioDC ServiciosContraPago { get; set; }


        public decimal ValorKiloInicial { get; set; }


        public decimal ValorKiloAdicional { get; set; }
    }
}
