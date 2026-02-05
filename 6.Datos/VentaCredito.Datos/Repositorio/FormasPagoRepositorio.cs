using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Admisiones.Mensajeria;

namespace VentaCredito.Datos.Repositorio
{
    public class FormasPagoRepositorio
    {
        private static FormasPagoRepositorio instancia = new FormasPagoRepositorio();
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;


        public static FormasPagoRepositorio Instancia { get { return instancia; } }

        /// <summary>
        /// Inserta en base de datos las formas de pago de mensajería
        /// </summary>
        /// <param name="formasPagoGuia"></param>
        /// <param name="usuario"></param>
        public void AdicionarGuiaFormasPago(long idAdmisionMensajeria, List<ADGuiaFormaPago> formasPagoGuia, string usuario)
        {
            //using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    formasPagoGuia.ForEach(formaPago =>
            //        contexto.paCrearAdmiGuiaFormaPago_MEN(idAdmisionMensajeria, formaPago.IdFormaPago, formaPago.Valor, usuario, formaPago.NumeroAsociadoFormaPago)
            //      );
            //}
        }
    }
}
