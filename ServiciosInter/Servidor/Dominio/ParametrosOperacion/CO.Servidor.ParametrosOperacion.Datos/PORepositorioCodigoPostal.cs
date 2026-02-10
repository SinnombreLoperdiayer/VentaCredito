using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CO.Servidor.ParametrosOperacion.Datos
{
    public class PORepositorioCodigoPostal
    {
        private static readonly PORepositorioCodigoPostal instancia = new PORepositorioCodigoPostal();
       
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static PORepositorioCodigoPostal Instancia
        {
            get { return PORepositorioCodigoPostal.instancia; }
        }

    }
}
