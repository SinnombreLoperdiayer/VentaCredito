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
    public class ServicioPesoResponse
    {

        public int SER_IdServicio { get; set; }

        public decimal SME_PesoMínimo { get; set; }

        public decimal SME_PesoMaximo { get; set; }

    }
}
