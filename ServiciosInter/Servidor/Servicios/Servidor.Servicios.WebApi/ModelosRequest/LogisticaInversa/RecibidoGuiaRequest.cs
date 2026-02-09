using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.LogisticaInversa
{
    public class RecibidoGuiaRequest
    {
        public string RecibidoPor { get; set; }
        public string Identificacion { get; set; }
        public string FechaEntrega { get; set; }
        public long NumeroGuia { get; set; }
        public string Telefono { get; set; }
        public string Otros { get; set; }
        public LIEnumOrigenAplicacion IdAplicacionOrigen { get; set; }
        public EnumEstadoRegistro EstadoRegistro { get; set; }

    }
}