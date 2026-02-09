using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.SincronizacionDatos
{
    public class SincronizacionOffLine
    {
        public List<GuiaOffline> LstGuiaOffline { get; set; }
        public int CantidadSuministrosSolicitar { get; set; }
        public List<SuministrosOffLine> LstSuministrosOffLine { get; set; }
        public string Usuario { get; set; }
        public long IdPuntoServicio { get; set; }
        public List<TablaAnchor> TablaMaxAnchor { get; set; }

        public List<long> LstGuiasSincronizadas { get; set; }

    }

    public class SuministrosOffLine
    {
        public long NumeroGuia { get; set; }
        public DateTime FechaSincronizacion { get; set; }
    }

    public class GuiaOffline
    {
        public int IdServicio { get; set; }
        public int IdCaja { get; set; }
        public string ObjetoGuia { get; set; }
        public string ObjetoParametroAdicional { get; set; }
        public bool EstaSincronizado { get; set; }
        public string DestinatarioRemitente { get; set; }
        public DateTime FechaSincronizacion { get; set; }
    }

    public class TablaAnchor
    {
        public string NombreTabla { get; set; }
        public string Anchor { get; set; }
    }
}
