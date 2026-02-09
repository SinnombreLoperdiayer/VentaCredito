using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModeloResponse.SolicitudRecogidasApp
{
    /// <summary>
    /// Esta clase es para la migracion del servidor nodeJS a WebApi, los nombres de las columnas se manejan de esa forma ya que la aplicacion los espera así. 
    /// </summary>
    public class UbicacionGuiaResponse
    {
        public long EGT_NumeroGuia { get; set; }
        public DateTime ADM_FechaEntrega { get; set; }
        public string EGT_DescripcionEstado { get; set; }
        public string MOG_Descripcion { get; set; }
        public string ADM_NombreCiudadOrigen { get; set; }
        public string ADM_NombreCiudadDestino { get; set; }
        public string EGT_NombreLocalidad { get; set; }
        public DateTime ADM_FechaAdmision { get; set; }
        public int ADM_IdServicio { get; set; }
        public long ADI_NumeroGuiaDHL { get; set; }
        public string ADM_NombreRemitente { get; set; }
        public string ADM_NombreDestinatario { get; set; }
        public string ADM_DireccionRemitente { get; set; }
        public string ADM_DireccionDestinatario { get; set; }
        public string ADM_TelefonoRemitente { get; set; }
        public string ADM_TelefonoDestinatario { get; set; }
        public string ADM_NombreTipoEnvio { get; set; }
        public string ADM_NombreServicio { get; set; }
        public decimal ADM_Peso { get; set; }
        public decimal ADM_ValorAdmision { get; set; }
        public decimal ADM_ValorPrimaSeguro { get; set; }
        public decimal ADM_ValorAdicionales { get; set; }
        public decimal ADM_ValorTotal { get; set; }
        public string ADM_DiceContener { get; set; }
        public string FOP_Descripcion { get; set; }

    }
}
