using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Suministros;
using VentaCredito.Suministros.Datos.Repositorio;
using System.ServiceModel;
using VentaCredito.Suministros.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Comun;

namespace VentaCredito.Suministros
{
    public class Suministro
    {


        #region CrearInstancia

        private static readonly Suministro instancia = new Suministro();

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static Suministro Instancia
        {
            get { return Suministro.instancia; }
        }

        #endregion CrearInstancia
        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        public void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro)
        {
            
            SUSuministros.Instancia.GuardarConsumoSuministro(consumoSuministro);
        }

        /// <summary>
        /// Obtiene el numero  prefijo + valorActual
        /// </summary>
        /// <param name="idSuministro">id del suministro</param>
        /// <returns>numero del giro</returns>
        public SUNumeradorPrefijo ObtenerNumeroPrefijoValor(SUEnumSuministro Suministro)
        {
            var numerador  = SUSuministros.Instancia.ObtenerNumeroPrefijoValor(Suministro);

            if (numerador == null)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_NO_EXISTE.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_NO_EXISTE)));
            }
            else if (numerador.ValorActual == 0)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_INVALIDO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_INVALIDO)));
            }

            return numerador;
        }
        
    }
}
