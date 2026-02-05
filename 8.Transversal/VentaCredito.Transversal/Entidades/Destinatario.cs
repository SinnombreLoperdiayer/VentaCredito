using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades
{
    public class Destinatario
    {
        public int TipoIdentificacion { get; set; }
        public string Identificacion { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public bool EsPersonaJuridica { get; set; }
        public string RazonSocial { get; set; }
        public string Email { get; set; }        
        public string Telefono { get; set; }                                
        public string Direccion { get; set; }
    }
}
