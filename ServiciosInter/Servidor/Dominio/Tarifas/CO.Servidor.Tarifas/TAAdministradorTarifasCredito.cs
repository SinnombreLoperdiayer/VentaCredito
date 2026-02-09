using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Tarifas
{
   public  class TAAdministradorTarifasCredito : ControllerBase
    {


        #region Propiedades

        private static readonly TAAdministradorTarifasCredito instancia = (TAAdministradorTarifasCredito)FabricaInterceptores.GetProxy(new TAAdministradorTarifasCredito(), COConstantesModulos.TARIFAS);


 
        /// <summary>
        /// Retorna una instancia del administrador de tarifas
        /// </summary>
        public static TAAdministradorTarifasCredito Instancia
        {
            get { return TAAdministradorTarifasCredito.instancia; }
        }

        #endregion Propiedades

        #region Calculo precio credito

        /// <summary>
        /// Retorna el valor de mensajeria credito
        /// </summary>
        /// <returns></returns>
        public TAPrecioMensajeriaDC ObtenerPrecioMensajeriaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            TAPrecioMensajeriaDC valor = TARepositorioCredito.Instancia.ObtenerPrecioMensajeriaCredito(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
            valor.Impuestos = TARepositorioCredito.Instancia.ObtenerValorImpuestosServicioCredito(idServicio).ToList();
            return valor;
        }

        /// <summary>
        /// Calcula precio rapicarga, el calculo del precio se realiza de acuerdo al peso ingresado y los rangos configurados
        /// Si el peso ingresado esta en un valor intermedio se aplica la siguiente formula
        /// valor=(valorRango * pesoRangoFinal) +(kilosAdicionales * valorRango)
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Objeto precio</returns>
        public TAPrecioCargaDC ObtenerPrecioCargaCredito(int idServicio, int idListaPrecio,  string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);
            int idLp = int.Parse(idListaPrecioServicio);

            TAPrecioCargaDC valor = TARepositorioCredito.Instancia.ObtenerPrecioCargaCredito(idServicio, idListaPrecio, idLp, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
            valor.Impuestos = TARepositorioCredito.Instancia.ObtenerValorImpuestosServicioCredito(idServicio).ToList();
            return valor;
        }
        #endregion
    }
}
