using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas
{
    public class TAPreciosAgrupadosDC
    {
        public int IdServicio { get; set; }

        public TAPrecioMensajeriaDC Precio { get; set; }

        public TAPrecioCargaDC PrecioCarga { get; set; }

        public string Mensaje { get; set; }

        public string NombreServicio { get; set; }

        public string TiempoEntrega { get; set; }

        public TAFormaPagoServicio FormaPagoServicio { get; set; }

        public DateTime fechaEntrega { get; set; }

        public int idListaPrecios { get; set; } = 0;

        public decimal nuevoValorComercial { get; set; } = 0;

        public string FranjaServicio { get; set; }
    }
}