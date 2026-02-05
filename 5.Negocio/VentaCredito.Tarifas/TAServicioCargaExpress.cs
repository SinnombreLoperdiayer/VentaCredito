using CO.Servidor.Dominio.Comun.Tarifas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Tarifas.Precios;

namespace VentaCredito.Tarifas
{

    public class TAServicioCargaExpress
    {
        private static readonly TAServicioCargaExpress instancia = new TAServicioCargaExpress();
        public static TAServicioCargaExpress Instancia
        {
            get { return instancia; }
        }
        /// <summary>
        /// Calcula precio
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <param name="idListaPrecio">Identificador id lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador ciudad de origen</param>
        /// <param name="idLocalidadDestino">Identificador ciudad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Objeto valor</returns>
        public TAPrecioMensajeriaDC CalcularPrecio(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            TAPrecioMensajeriaDC valorPeso = TAServicioMensajeria.Instancia.ObtenerPrecioMensajeriaCredito(TAConstantesServicios.SERVICIO_CARGA_EXPRESS, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);

            TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC()
            {
                //Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(TAConstantesServicios.SERVICIO_CARGA_EXPRESS).ToList(),
                ValorKiloInicial = valorPeso.ValorKiloInicial,
                ValorKiloAdicional = valorPeso.ValorKiloAdicional,
                Valor = valorPeso.Valor,
                ValorPrimaSeguro = valorPeso.ValorPrimaSeguro
            };

            return precio;
        }
    }
}
