using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADRastreoGuiaClienteRespuesta
    {
        /// <summary>
        /// retorna o asigna el objeto con la traza de la guia.
        /// </summary>
        public ADTrazaGuiaClienteRespuesta TrazaGuia { get; set; }

        /// <summary>
        /// retorna o asigna una lista con los estado de la guia.
        /// </summary>
        public List<ADEstadoGuiaMotivoClienteRespuesta> EstadosGuia { get; set; }

        /// <summary>
        /// retorna o asigna el objeto con la información de la guia.
        /// </summary>
        public ADGuiaClienteRespuesta Guia { get; set; }

        /// <summary>
        /// retorna o asinga si es sispostal.
        /// </summary>
        public bool EsSispostal { get; set; }

        /// <summary>
        /// retorna o asigna la imagen de la guía en base 64.
        /// </summary>
        public string ImagenGuia { get; set; }
        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool? Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado de la operación
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Código de error específico para manejo programático
        /// </summary>
        public string ErrorCode { get; set; }
    }
}
