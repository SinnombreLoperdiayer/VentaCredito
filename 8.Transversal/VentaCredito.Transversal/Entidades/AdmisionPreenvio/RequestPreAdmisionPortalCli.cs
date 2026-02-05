using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class RequestPreAdmisionPortalCli : RequestPreAdmisionWrapperCV
    {
        public long IdRecogida { get; set; }
        public DateTime FechaRecogida { get; set; } 
    }
}
