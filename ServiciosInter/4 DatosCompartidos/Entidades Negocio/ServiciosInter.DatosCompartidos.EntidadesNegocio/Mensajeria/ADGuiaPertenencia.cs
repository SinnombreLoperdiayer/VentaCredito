using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADGuiaPertenencia
    {
        /// <summary>
        /// Asigna o retorna el numero de guia.
        /// </summary>
        public string NumeroGuia { get; set; }

        /// <summary>
        /// retorna o asigna el telefono del remitente.
        /// </summary>
        public string TelefonoRemitente { get; set; }

        /// <summary>
        /// retorna o asigna el identificacion del remitente.
        /// </summary>
        public string IdentificacionRemitente { get; set; }

        /// <summary>
        /// retorna o asigna el telefono del destinatario.
        /// </summary>
        public string TelefonoDestinatario { get; set; }

        /// <summary>
        /// retorna o asigna el identificacion del destinatario.
        /// </summary>
        public string IdentificacionDestinatario { get; set; }
    }
}
