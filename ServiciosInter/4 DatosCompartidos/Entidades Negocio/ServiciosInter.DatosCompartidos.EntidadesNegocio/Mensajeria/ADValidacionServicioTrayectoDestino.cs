using System.Collections.Generic;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas;
using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADValidacionServicioTrayectoDestino
    {
        public int DuracionTrayectoEnHoras { get; set; }
        public ADRangoTrayecto InfoCasillero { get; set; }
        public int NumeroHorasDigitalizacion { get; set; }
        public int NumeroHorasArchivo { get; set; }

        public decimal PesoMaximoTrayectoOrigen { get; set; }
        public decimal VolumenMaximoOrigen { get; set; }
        public decimal VolumenMaximoDestino { get; set; }
        public decimal PesoMaximoTrayectoDestino { get; set; }
        public long IdCentroServiciosDestino { get; set; }
        public string DireccionCentroServiciosDestino { get; set; }
        public string TelefonoCentroServiciosDestino { get; set; }
        public string NombreCentroServiciosDestino { get; set; }
        public IEnumerable<TAValorAdicional> ValoresAdicionales { get; set; }
        public long IdCentroServiciosOrigen { get; set; }
        public string NombreCentroServiciosOrigen { get; set; }
        public bool DestinoAdmiteFormaPagoAlCobro { get; set; }
        public string CodigoPostalDestino { get; set; }
        public int? IdOperadorPostalDestino { get; set; }
        public string IdZonaOperadorPostalDestino { get; set; }

        public DateTime fechaEntrega { get; set; }
        //public string NombreCiudadDestino { get; set; }
        //public string NombreRepLegal { get; set; }
        //public string IdentificacionRepLegal { get; set; }
    }
}