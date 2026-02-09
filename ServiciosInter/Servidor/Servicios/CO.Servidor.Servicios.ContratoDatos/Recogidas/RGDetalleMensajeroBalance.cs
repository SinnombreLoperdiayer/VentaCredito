using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    public class RGDetalleMensajeroBalance
    {
        public class RGDetalleInfoMensajero {
            public long IdMensajero { get; set; }
            public string IdLocalidad { get; set; }
            public string Nombre { get; set; }
            public string Recogidas { get; set; }
            public string Entregas { get; set; }
            public string Telefono { get; set; }
            public string Email { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public DateTime FechaGrabacion { get; set; }
            public string Vehiculo { get; set; }
            public string Placa { get; set; }
            public string TipoContrato { get; set; }
            public string TipoVehiculo { get; set; }
            public string ColorVehiculo { get; set; }
            public string Foto { get; set; }
        }

        public class RGDetalleRutaMensajero {
            public string IdMensajero { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public DateTime FechaGrabacion { get; set; }
            public bool EsEntrega { get; set; }
            public bool EsRecogida { get; set; }
            public bool EsDevolucion { get; set; }
            public string Guia { get; set; }
        }

        public class RGRutasPorFechaMensajero {
            public List<RGDetalleRutaMensajero> RutaPorFecha { get; set; }
        }

        public RGDetalleInfoMensajero infoMensajero{ get; set; }
        public List<RGRutasPorFechaMensajero> rutaMensajero{ get; set; }
    }
}
