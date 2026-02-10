using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Controller.Servidor.Integracion.SatrackControlTrafico.entidades
{
    class credencialesTrafico
    {
        public string Usuario { get; set; }
        public string Password { get; set; }


        public credencialesTrafico() 
        {
            this.Usuario = "Interrapidisimo";
            this.Password = "Samuel0207";
        }
    }
}
