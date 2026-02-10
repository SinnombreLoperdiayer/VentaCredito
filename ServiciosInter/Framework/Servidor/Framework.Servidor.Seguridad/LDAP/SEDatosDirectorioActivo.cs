using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Seguridad.LDAP
{
    public class SEDatosDirectorioActivo
    {
        public string NombreProvider { get; set; }

        public string NombreCadenaConexion { get; set; }

        public string CadenaConexion { get; set; }

        public string UsuarioAdministrador { get; set; }

        public string ClaveAdministrador { get; set; }

        public string ParametroDeBusqueda { get; set; }
    }
}
