using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Tarifas;

namespace VentaCredito.Datos.Repositorio
{
    public class AdmisionValorAdicionalRepositorio
    {
        private static AdmisionValorAdicionalRepositorio instancia = new AdmisionValorAdicionalRepositorio();
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        public static AdmisionValorAdicionalRepositorio Instancia { get { return instancia; } }

        /// <summary>
        /// Inserta los valores adicionales pasados
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="valoresAdicionales"></param>
        /// <param name="usuario"></param>
        public void AdicionarValoresAdicionales(long idAdmisionMensajeria, List<TAValorAdicional> valoresAdicionales, string usuario)
        {
            //using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    if (valoresAdicionales != null && valoresAdicionales.Any())
            //    {
            //        valoresAdicionales.ForEach(valorAdicional =>
            //          contexto.paCrearAdmiValorAdicional_MEN(idAdmisionMensajeria, valorAdicional.IdTipoValorAdicional, valorAdicional.Descripcion, valorAdicional.PrecioValorAdicional, usuario));
            //    }
            //}
        }

    }
}
