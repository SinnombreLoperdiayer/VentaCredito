using System;

namespace VentaCredito.Transversal.Entidades
{
    public class Tracking
    {
        public string FechaNotificacion { get; set; }
        public string FechaEstado { get; set; }
        public long NumeroGuia { get; set; }
        public string DescripcionEstado { get; set; }
        public string CodigoEstado { get; set; }
        public string DescripcionMotivoEst { get; set; }
        public long CodigoMotivoEst { get; set; }
        public string CodigoCiudad { get; set; }
    }
}
