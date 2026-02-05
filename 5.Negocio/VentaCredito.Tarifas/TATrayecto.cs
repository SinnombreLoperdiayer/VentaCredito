using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CO.Servidor.Dominio.Comun.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Servicio.Entidades.Tarifas;
using VentaCredito.Tarifas.Datos.Repositorio;
using Servicio.Entidades.Tarifas.Precios;

namespace VentaCredito.Tarifas
{
    public class TATrayecto
    {
        private static TATrayecto instancia = new TATrayecto();

        public static TATrayecto Instancia
        {
            get {   return instancia;   }
        }

        /// <summary>
        /// Retorna la lista del horario de determinado centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeCsv(long idCentroServicio)
        {
            return HorarioRecogidaCsv.Instancia.ObtenerHorarioRecogidaDeCsv(idCentroServicio);
        }

        /// <summary>
        /// Retorna el número de dias del trayecto si existe
        /// </summary>
        /// <param name="municipioOrigen">Municipio destino</param>
        /// <param name="municipioDestino">Municipio de destino</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <returns></returns>
        public TATiempoDigitalizacionArchivo ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, decimal peso = 0)
        {
            if (servicio.IdServicio != TAConstantesServicios.SERVICIO_INTERNACIONAL)
            {
                return DigitalizacionArchivoRepositorio.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, peso);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna Validacion si el Servicio-Origen-Destino, debe etiquetarse como AEREO en el campo del casillero de la Guia
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public bool ValidarServicioTrayectoCasilleroAereo(string municipioOrigen, string municipioDestino, int idServicio)
        {
            return TrayectoRepositorio.Instancia.ValidarServicioTrayectoCasilleroAereo(municipioOrigen, municipioDestino, idServicio);
        }

        /// <summary>
        /// Obtiene los servicios de rapicarga, Rapi Carga Terrestre y mensajeria por municipio origen y destino 
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <returns></returns>
        public List<int> ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(string municipioOrigen, string municipioDestino)
        {
            return TrayectoRepositorio.Instancia.ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(municipioOrigen, municipioDestino);
        }

        /// <summary>
        /// Obtener concepto de caja a partir del numero del servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public int ObtenerConceptoCaja(int idServicio)
        {
            return TrayectoRepositorio.Instancia.ObtenerConceptoCaja(idServicio);
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
        public TAPrecioMensajeriaDC CalcularPrecioCargaExpress(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            //throw new NotImplementedException();
             return TAServicioCargaExpress.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioCargaAerea(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega)
        {
            throw new NotImplementedException();
            //return TAServicioCargaExpressAerea.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }
    }
}
