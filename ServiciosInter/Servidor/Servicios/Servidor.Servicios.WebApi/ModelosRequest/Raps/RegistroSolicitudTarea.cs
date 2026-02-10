using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System.Collections.Generic;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.Raps
{
    public class RegistroSolicitudTarea
    {
        public RASolicitudDC Solicitud { get; set; }
        public List<RAAdjuntoDC> Adjuntos { get; set; }
        public InformacionGestion informacionGestion { get; set; }
        public Dictionary<string, object> parametrosParametrizacion { get; set; }
        public string idCiudad { get; set; }
        //public bool esAgrupamiento { get; set; }
        public int idSistema { get; set; }
        public int idTipoNovedad { get; set; }
        public RAParametrizacionRapsDC ParametrizacionRaps { get; set; }
        public List<RACargoDC> ListarCargos { get; set; }
        public List<RAEscalonamientoDC> lstEscalonamiento { get; set; }
        public List<RATiempoEjecucionRapsDC> lstTiempoEjecucion { get; set; }
        public List<RAParametrosParametrizacionDC> lstParametros { get; set; }
        public List<RAPersonaDC> LstPersonas { get; set; }
    }
}