using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CO.Servidor.Servicios.ContratoDatos.Comun;

namespace CO.Servidor.Servicios.WebApi.NotificacionesSignalR
{
    public class ParametrosSignalR
    {
        /// <summary>
        /// Identificador único de la conexíon
        /// </summary>
        public string IdConexion { get; set; }
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string NombreUsuario { get; set; }
        /// <summary>
        /// Tipo documento
        /// </summary>
        public int TipoDocumento { get; set; }
        /// <summary>
        /// Numero documento
        /// </summary>
        public long Documento { get; set; }
        /// <summary>
        /// Fecha Ingreso
        /// </summary>
        public DateTime FechaIngreso { get; set; }
        /// <summary>
        /// Contenido del mensaje
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// localidad del mensaje
        /// </summary>
        public string IdLocalidad { get; set; }

        /// <summary>
        /// identificador de la solicitud
        /// </summary>
        public long IdSolicitud { get; set; }

        /// <summary>
        /// Numero del nuevo estado
        /// </summary>
        public int NuevoEstado { get; set; }

        public COEnumIdentificadorAplicacion IdAplicacion{ get; set; }

        public bool InsertarNotificacion { get; set; }

    }
}
