using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADTrazaGuiaEstadoGuiaDetallado
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
        /// Retorna o asigna el nombre de la ciudad del centro logistico
        /// </summary>
        public string Ciudad { get; set; }

        /// <summary>
        /// retorna o asigna la fecha de grabación del registro
        /// </summary>
        public DateTime FechaGrabacion { get; set; }

        /// <summary>
        /// retorna o asigna la localidad actual de la guia
        /// </summary>
        public string IdLocalidadEnCurso { get; set; }

        /// <summary>
        /// retorna o asigna la localidad de origen de la guia
        /// </summary>
        public string IdLocalidadOrigen { get; set; }

        /// <summary>
        /// retorna o asigna la localidad destino de la guia
        /// </summary>
        public string IdLocalidadDestino { get; set; }
    }
}
