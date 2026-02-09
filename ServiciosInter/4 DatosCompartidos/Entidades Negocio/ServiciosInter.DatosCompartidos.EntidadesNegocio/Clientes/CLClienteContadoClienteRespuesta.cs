using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Clientes
{
    public class CLClienteContadoClienteRespuesta
    {
        /// <summary>
        /// retorna o asigna el nombre del remitente o destinatario
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// retorna o asigna el telefono del remitente o destinatario
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// retorna o asigna el identificacion del remitente o destinatario
        /// </summary>
        public string Identificacion { get; set; }

        /// <summary>
        /// retorna o asigna el direccion del remitente o destinatario
        /// </summary>
        public string Direccion { get; set; }

        /// <summary>
        /// retorna o asigna el id tipo del remitente o destinatario
        /// </summary>
        public string TipoId { get; set; }

        /// <summary>
        /// retorna o asigna la direccion de correo.
        /// </summary>
        public string Email { get; set; }

    }
}
