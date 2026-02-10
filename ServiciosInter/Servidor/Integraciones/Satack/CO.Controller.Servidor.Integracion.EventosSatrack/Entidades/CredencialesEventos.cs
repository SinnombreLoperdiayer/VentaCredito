using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Controller.Servidor.Integracion.EventosSatrack.Entidades
{
    class CredencialesEventos
    {
        public string Usuario { get; set; }
        public string Password { get; set; }


        public CredencialesEventos() 
        {
            this.Usuario = "Interrapidisimo";
            this.Password = "Samuel0207";
        }

    }
}
