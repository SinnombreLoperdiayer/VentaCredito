using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModeloResponse.OperacionUrbana
{
    public class OURecogidaResponse
    {

        public long IdRecogida { get; set; }
        public short CantidadEnvios { get; set; }
        public PALocalidadDC LocalidadRecogida { get; set; }
        public DateTime FechaRecogida { get; set; }
        public string Direccion { get; set; }
        public string ComplementoDireccion { get; set; }
        public OUEstadosSolicitudRecogidaDC EstadoRecogida { get; set; }
        public OURecogidaPeatonDC RecogidaPeaton { get; set; }
        public string PersonaSolicita { get; set; }
        public string Contacto { get; set; }
        public decimal PesoAproximado { get; set; }
        public string NombreCliente { get; set; }
        public string LongitudRecogida { get; set; }
        public string LatitudRecogida { get; set; }
        public long MinutosTranscurridos { get; set; }
        public OUEnumTipoOrigenRecogida TipoOrigenRecogida { get; set; }
        public PADispositivoMovil DispositivoMovil { get; set; }
        public List<string> Fotografias { get; set; }
        public OUAsignacionRecogidaMensajeroDC AsignacionMensajero { get; set; }
    }
}
