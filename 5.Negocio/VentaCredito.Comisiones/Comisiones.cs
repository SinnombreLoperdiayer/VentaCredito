using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Comisiones;

namespace VentaCredito.Comisiones
{
    public class Comision
    {
        private static Comision instancia = new Comision();

        public static Comision Instancia
        {
            get
            {
                return instancia;
            }
        }

        public CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta)
        {
            CMComisionXVentaCalculadaDC comision = CMLiquidadorComisiones.Instancia.CalcularComisionesxVentas(consulta);            
            return comision;
        }        
    }
}

