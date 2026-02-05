using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.CentroServicios;

namespace VentaCredito.CentroServicios
{
    public class PUAdministradorCentroServicios
    {
        private static PUAdministradorCentroServicios instancia = new PUAdministradorCentroServicios();

        public static PUAdministradorCentroServicios Instancia
        {
            get
            {
                return instancia;
            }
        }

        /// <summary>
        /// Obtener la agencia a partir de la localida
        /// </summary>
        /// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciaLocalidad(localidad);
        }

    }
}
