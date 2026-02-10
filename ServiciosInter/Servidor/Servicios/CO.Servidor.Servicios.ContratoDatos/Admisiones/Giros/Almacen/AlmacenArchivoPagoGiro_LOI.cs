using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Almacen
{
    public class AlmacenArchivoPagoGiro_LOI
    {
        public long APG_IdArchivo { get; set; }
        public long APG_IdComprobantePago { get; set; }
        public string APG_NombreAdjunto { get; set; }
        public string APG_RutaAdjunto { get; set; }
        public Guid APG_IdAdjunto { get; set; }
        public long APG_IdGiro { get; set; }
        public long APG_IdAdmision { get; set; }
        public bool APG_Manual { get; set; }
        public DateTime APG_FechaGrabacion { get; set; }
        public string APG_CreadoPor { get; set; }
        public bool APG_Decodificada { get; set; }
    }
}
