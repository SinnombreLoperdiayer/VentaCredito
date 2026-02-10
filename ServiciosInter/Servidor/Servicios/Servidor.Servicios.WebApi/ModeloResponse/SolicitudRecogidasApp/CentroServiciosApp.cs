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
    public class CentroServiciosApp
    {

        public string CES_Tipo { get; set; }

        public string CES_Estado { get; set; }

        public long ID { get; set; }

        public decimal latitud { get; set; }

        public decimal longitud { get; set; }

        public string direccion { get; set; }

        public string telefono1 { get; set; }

        public string telefono2 { get; set; }

        public string nombre { get; set; }

        public int tipoPropiedad { get; set; }

        public string idMunicipio { get; set; }

    }
}
