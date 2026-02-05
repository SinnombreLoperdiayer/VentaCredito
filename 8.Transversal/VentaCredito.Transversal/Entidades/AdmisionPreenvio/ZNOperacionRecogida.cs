using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class ZNOperacionRecogida
    {
        public long IdOperacion { get; set; }
        public int IdCentroServicio { get; set; }
        public string NombreCentroServicio { get; set; }
        public string IdLocalidad { get; set; }
        public bool PermiteRecogida { get; set; }
        public string NombreLocalidad { get; set; }
        public int TiempoInicio { get; set; }
        public int TiempoFinal { get; set; }
        public bool HabilitaEsp { get; set; }
        public int TiempoNotEsp { get; set; }
        public int TiempoMinSol { get; set; }
        public bool HabilitaTpoForzarEsp { get; set; }
        public int TiempoForzarEsp { get; set; }
        public bool HabilitaDiasFuturos { get; set; }
        public int DiasFuturosHabilitados { get; set; }
        public bool AplicaFestivos { get; set; }
        public bool RecogidaZona { get; set; }
        public bool HabilitaRadio { get; set; }
        public int RadioRecogida { get; set; }
        public int TiempoNotFija { get; set; }
        public bool HabilitaTpoForzarFija { get; set; }
        public int TiempoForzarFija { get; set; }
        public bool SeGeoreferencia { get; set; }
        public bool HabilitaPreenviosPunto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string CreadoPor { get; set; }
        public int Pagina { get; set; }
        public int Cantidad { get; set; }
        public int NotDiaAnterior { get; set; }
        public bool NotDiaAnteriorAM { get; set; }
        public bool NotDiaAnteriorPM { get; set; }
        public int NotMismoDia { get; set; }
        public bool NotMismoDiaAM { get; set; }
        public bool NotMismoDiaPM { get; set; }
    }
}
