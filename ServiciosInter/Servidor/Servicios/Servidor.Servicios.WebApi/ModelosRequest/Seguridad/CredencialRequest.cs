using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.Seguridad
{
    public class CredencialRequest
    {
        public string Usuario { get; set; }

        public string Password { get; set; }
    }
}
