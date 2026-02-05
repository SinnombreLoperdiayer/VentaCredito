using CO.Servidor.Dominio.Comun.Tarifas;
using Servicio.Entidades.Tarifas.Precios;
using System;
using VentaCredito.Tarifas.Datos.Repositorio;

namespace VentaCredito.Tarifas
{
    public class TAServicioMensajeria
    {
        private static TAServicioMensajeria instancia = new TAServicioMensajeria();

        public static TAServicioMensajeria Instancia
        {
            get {   return instancia;   }
        }
        
        public decimal ObtenerValorMinimoDeclarado(int idListaPrecio, decimal peso)
        {
            return PrecioMensajeriaCredito.Instancia.ObtenerValorMinimoDeclarado(idListaPrecio, peso);
        }

        public Servicio.Entidades.Tarifas.Precios.TAPrecioMensajeriaDC ObtenerPrecioMensajeriaCredito(int servicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega, bool aplicaContraPago = false)
        {
            return PrecioMensajeriaCredito.Instancia.ObtenerPrecioMensajeriaCredito(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega, aplicaContraPago);
        }

        public CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios.TAPrecioMensajeriaDC CalcularPrecioRapiRadicado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega)
        {
            int idServicio = TAConstantesServicios.SERVICIO_RAPIRADICADO;
            return PrecioMensajeriaCredito.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }
    }
}