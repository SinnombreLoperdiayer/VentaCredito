using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Controller.Servidor.Integracion.SatrackControlTrafico.entidades
{
    public class RequestProgramarItinerarioXML
    {
        public ProgramarItinerarioXML programacion { get; set; }
    }

    public class ProgramarItinerarioXML
    {
        public List<ItinerarioProgramarItinerarioXML> itinerario { get; set; }
    }

    public class ItinerarioProgramarItinerarioXML
    {
        public string placa { get; set; }
        public string ruta { get; set; }

        public ParametrosProgramarItinerarioXML parametros { get; set; }
    }

    public class ParametrosProgramarItinerarioXML
    {
        public TraficoProgramarItinerarioXML trafico { get; set; }
        public CanbusProgramarItinerarioXML canbus { get; set; }
        public string disponible { get; set; }
        public string fechadespacho { get; set; }
        public RegCargueProgramarItinerarioXML regionescargue { get; set; }
        public string placatemporal { get; set; }
    }

    public class TraficoProgramarItinerarioXML
    {
        public string agencia { get; set; }
        public string planviaje { get; set; }
        public string campo1 { get; set; }
        public string nombreconductor { get; set; }
        public string telefonoconductor { get; set; }
    }

    public class CanbusProgramarItinerarioXML
    {
        public long? canpeso { get; set; }
    }

    public class RegCargueProgramarItinerarioXML
    {
        public List<string> codigoregion { get; set; }
    }

}
