using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.Raps
{
    public class GestionSolicitud
    {
        public RAGestionDC Gestion { get; set; }
        public InformacionGestion informacionGestion { get; set; }
    }
}