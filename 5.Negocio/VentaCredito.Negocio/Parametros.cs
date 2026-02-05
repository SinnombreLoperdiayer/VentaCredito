
using Servicio.Entidades.Admisiones.Mensajeria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Negocio
{
    public class Parametros
    {
        private static Parametros instancia = new Parametros();

        public static Parametros Instancia
        {
            get
            {
                return instancia;
            }
        }
        public IEnumerable<ADTipoEntrega> ObtenerTiposEntrega()
        {
            return Datos.Repositorio.Parametros.Instancia.ObtenerTiposEntrega();
        }
    }
}
