using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.WebApi.ModelosRequest.Raps;
using System.Collections.Generic;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
    public class RegistroSolicitud
    {
        public RASolicitudDC Solicitud { get; set; }
        public List<RAAdjuntoDC> Adjuntos { get; set; }
        public InformacionGestion informacionGestion { get; set; }
        public Dictionary<string, object> parametrosParametrizacion { get; set; }
        public string idCiudad { get; set; }
        //public bool esAgrupamiento { get; set; }
        public int idSistema {get; set;}
        public int idTipoNovedad { get; set; }
    }
}
