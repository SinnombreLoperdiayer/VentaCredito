using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    public class ADUbicacionGuia
    {
        public long NumeroGuia { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string DescripcionEstadoGuiaTraza { get; set; }
        public string DescripcionMotivoGuia { get; set; }
        public string NombreCiudadOrigen { get; set; }
        public string NombreCiudadDestino { get; set; }
        public string NombreLocalidadGuiatraza { get; set; }
        public DateTime FechaAdmision { get; set; }
        public int IdServicio { get; set; }
        public long NumeroGuiaDHL { get; set; }
        public string NombreRemitente { get; set; }
        public string NombreDestinatario { get; set; }
        public string DireccionRemitente { get; set; }
        public string DireccionDestinatario { get; set; }
        public string TelefonoRemitente { get; set; }
        public string TelefonoDestinatario { get; set; }
        public string NombreTipoEnvio { get; set; }
        public string NombreServicio { get; set; }
        public decimal Peso { get; set; }
        public decimal ValorAdmision { get; set; }
        public decimal ValorPrimaSeguro { get; set; }
        public decimal ValorAdicionales { get; set; }
        public decimal ValorTotal { get; set; }
        public string DiceContener { get; set; }
        public string DescripcionFormaPago { get; set; }

    }
}
