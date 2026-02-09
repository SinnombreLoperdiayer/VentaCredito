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
    public class TipoEnvioApp
    {
        public int TEN_IdTipoEnvio { get; set; }

        public string TEN_Descripcion { get; set; }

        public decimal TEN_PesoMinimo { get; set; }

        public decimal TEN_PesoMaximo { get; set; }

    }
}
