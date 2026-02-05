
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Cajas.Comun;
using VentaCredito.Cajas.Datos.Repositorio;
using Framework.Servidor.Comun;

namespace VentaCredito.Cajas.CajaVenta
{
    public class CACaja
    {

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static CACaja instancia = new CACaja();

        public static CACaja Instancia
        {
            get { return instancia ; }
            
        }



        #region Metodos

        /// <summary>
        /// Obtiene el valor del parametro configurado en la tabla de parametrso caja.
        /// </summary>
        /// <param name="idParametro">The id parametro.</param>
        /// <returns></returns>
        public string ObtenerParametroCajas(string idParametro)
        {
            string parametroCaja = string.Empty;

            parametroCaja = CACajasRepositorio.Instancia.ObtenerParametroCajas(idParametro);

            if (parametroCaja == null)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA,
                      CAEnumTipoErrorCaja.ERROR_PARAMETRO_CAJA_NO_ENCONTRADO.ToString(),
                      String.Format(CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_PARAMETRO_CAJA_NO_ENCONTRADO), idParametro));
                throw new FaultException<ControllerException>(excepcion);
            }

            return parametroCaja;
        }

        #endregion

    }
}
