using System;

namespace ServiciosInter.DatosCompartidos.Wrappers.Preenvios
{
    public class NotificacionWrapper
    {
        public long IdNotificacion { get; set; }

        public long IdAdmisionPreenvio { get; set; }

        public long IdDestinatario { get; set; }

        public string IdTipoDestino { get; set; }

        public string IdCiudadDestino { get; set; }

        public DateTime FechaGrabacion { get; set; }

        public string DireccionDestinatario { get; set; }

        public string NombreCiudadDestino { get; set; }

        public string TipoDestino { get; set; }

        public bool EntregarDireccionRemitente { get; set; }
    }
}
