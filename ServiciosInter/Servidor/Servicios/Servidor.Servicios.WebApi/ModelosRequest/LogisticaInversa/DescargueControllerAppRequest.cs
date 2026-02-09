using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.LogisticaInversa
{
    public class DescargueControllerAppRequest
    {
        public short IdServicio { get; set; }
        public long IdMensajero { get; set; }
        public string IdCiudad { get; set; }
        public string NombreCiudad { get; set; }
        public string NombreQuienRecibe { get; set; }
        public MotivoDescargueRequest MotivoGuia { get; set; }
        public string Observaciones { get; set; }
        public long NumeroGuia { get; set; }
        public long IdentificacionQuienRecibe { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public DateTime FechaGrabacion { get; set; }
        public List<LITipoEvidenciaControllerAppDC> TipoEvidencia { get; set; }
        public long NumeroIntentoFallidoEntrega { get; set; }
        public RecibidoGuiaRequest RecibidoGuia { get; set; }
        public long IdPlanilla { get; set; }
        public DateTime FechaEntrega { get; set; }
        public short IdEstado { get; set; }
        public string Usuario { get; set; }
        public int TipoContador { get; set; }
        public string NumeroContador { get; set; }
        public int TipoPredio { get; set; }
        public string DescripcionPredio { get; set; }

        // Gestion Devoluciones falsas de mensajero 
        public bool TieneIntentoEntrega { get; set; }
        public int IdSistema { get; set; }
        public int TipoNovedad { get; set; }
        public Dictionary<string, object> Parametros { get; set; }

    }
}