using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Excepciones
{
    /// <summary>
    /// Clase que contiene la informacion de los errores
    /// </summary>
    public class SEAuditoriaErrores
    {
        public string Modulo { get; set; }
        public string Tipo { get; set; }
        public string Usuario { get; set; }
        public string Mensaje { get; set; }
        public string StackTrace { get; set; }
        public DateTime FechaError { get; set; }
        public string InnerException { get; set; }
        public string NombreMetodo { get; set; }
        public string NombreAssembly { get; set; }
        public string Parametros { get; set; }
    }
}
