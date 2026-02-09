using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADRastreoGuiaClienteSolicitud
    {
        /// <summary>
        /// retorna o asigna el numero de guia.
        /// </summary>
        public string NumeroGuia { get; set; }

        /// <summary>
        /// retorna o asigna el numero de identificacion.
        /// </summary>
        public string NumeroIdentificacion { get; set; }

        /// <summary>
        /// retorna o asigna el numero de telefono.
        /// </summary>
        public string NumeroTelefono { get; set; }

        /// <summary>
        /// retorna o asigna el id opcions
        /// </summary>
        public int? IdOpcion { get; set; }

        public bool EncriptaAes { get; set; } = false;
    }
}
