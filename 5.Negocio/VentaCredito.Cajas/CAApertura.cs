using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Cajas;
using VentaCredito.Cajas.CajaVenta;

namespace VentaCredito.Cajas
{
    public class CAApertura
    {

        private static CAApertura instancia = new CAApertura();


        public static CAApertura Instancia
        {
            get
            {
                return CAApertura.instancia;
            }
        }
    }
}
