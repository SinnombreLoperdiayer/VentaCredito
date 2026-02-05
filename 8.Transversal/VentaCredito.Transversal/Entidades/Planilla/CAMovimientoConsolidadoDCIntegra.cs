using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class CAMovimientoConsolidadoDCIntegra
    {
        public int IdMovimiento { get; set; }


        public long NumeroPrecinto { get; set; }


        public long IdCentroServicioDestino { get; set; }


        public int IdTipoMovimiento { get; set; }


        public string FechaMovimiento { get; set; }


        public string CreadoPor { get; set; }


        public string NumeroConsolidado { get; set; }


        public int IdTipoConsolidado { get; set; }


        public int IdMovimientoLog { get; set; }


        public string IdLocalidadOrigen { get; set; }


        public string IdLocalidadDestino { get; set; }


        public string IdLocalidadMovimiento { get; set; }


        //public NovedadConsolidado Novedad { get; set; }


        //public List<NovedadConsolidado> ListaNovedades { get; set; }


        public string DescripcionTipoConsolidado { get; set; }


        public long? IdAdmisionMensajeria { get; set; }


        public int IdEstado { get; set; }


        public string Estado { get; set; }
    }
}
