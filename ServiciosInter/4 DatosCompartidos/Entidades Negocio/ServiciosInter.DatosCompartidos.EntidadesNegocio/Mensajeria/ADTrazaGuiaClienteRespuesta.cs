using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADTrazaGuiaClienteRespuesta
    {
        /// <summary>
        /// retorna o asigna el id de la guia
        /// </summary>
        public short IdEstadoGuia { get; set; }

        /// <summary>
        /// retorna o asigna la descripción de el estado de la guia
        /// </summary>
        public string DescripcionEstadoGuia { get; set; }

        /// <summary>
        /// retorna o asigna la fecha de grabación del registro
        /// </summary>
        public DateTime FechaGrabacion { get; set; }
    }
}
