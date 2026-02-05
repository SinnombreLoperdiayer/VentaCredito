using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades
{
    public class ADTransmisionNotificacion
    {
        public long IdAdmisionMensajeria { get; set; }
        public EnumTipoNotificacion TipoNotificacion { get; set; }
        public long NumeroGuia { get; set; }
        public int IdEstadoNovMotivoGuia { get; set; }
        public string Usuario { get; set; }
        public long idCliente { get; set; }

    }
}
