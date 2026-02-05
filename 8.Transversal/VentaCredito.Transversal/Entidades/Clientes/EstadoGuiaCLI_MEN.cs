using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VentaCredito.Transversal.Entidades.Clientes
{
    public class EstadoGuiaCLI_MEN
    {
        public long IdEstadoGuia { get; set; }
        public string NombreEstado { get; set; }
        public string IdLocalidadOrigen { get; set; }
        public string IdLocalidadDestino { get; set; }
        public string NombreCiudadOrigen { get; set; }
        public string NombreCiudadDestino { get; set; }
        public int IdClienteCredito { get; set; }
        public DateTime FechaConsulta { get; set; }
        public DateTime FechaEstado { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string IdLocalidadenCurso { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DescripcionAsociada { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string NombreCiudadenCurso { get; set; }
    }
}
