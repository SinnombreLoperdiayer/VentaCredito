using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestCreacionTercerosMin
    {
        public CreacionTercerosMin root { get; set; }
    }

    public class CreacionTercerosMin
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesCreacionTercerosMin variables { get; set; }
    }

    public class VariablesCreacionTercerosMin
    {
        public long? NUMNITEMPRESATRANSPORTE { get; set; }

        public string CODTIPOIDTERCERO { get; set; }

        public string NUMIDTERCERO { get; set; }

        public string NOMIDTERCERO { get; set; }

        public string PRIMERAPELLIDOIDTERCERO { get; set; }

        public string SEGUNDOAPELLIDOIDTERCERO { get; set; }

        public long? NUMTELEFONOCONTACTO { get; set; }

        public string NOMENCLATURADIRECCION { get; set; }

        public string CODMUNICIPIORNDC { get; set; }

        public string CODSEDETERCERO { get; set; }

        public string NOMSEDETERCERO { get; set; }

        public long? CODCATEGORIALICENCIACONDUCCION { get; set; }

        public long? NUMLICENCIACONDUCCION { get; set; }

        public string FECHAVENCIMIENTOLICENCIA { get; set; }
    }
}
