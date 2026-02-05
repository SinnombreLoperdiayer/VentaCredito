using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Transversal.Entidades;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using VentraCredito.Localidades.Datos;

namespace VentaCredito.LocalidadesNegocio
{
    public class LocalidadesNegocio
    {
        private static LocalidadesNegocio instancia = new LocalidadesNegocio();
        public static LocalidadesNegocio Instancia
        {
            get
            {
                return instancia;
            }
        }

        /// <summary>
        /// Retorna la existencia de una localidad (0 = No existe / 1 = Si existe)
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns>Existencia de la localidad</returns>
        public bool ConsultarExistenciaLocalidad(string idLocalidad)
        {
            return LocalidadesVentaCreditoDatos.Instancia.ConsultarExistenciaLocalidad(idLocalidad);
        }

        /// <summary>
        /// Obtiene las localidades de destino asociadas a una sucursal.
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns>Lista con nombre y id de las localidades de destino asociadas a un cliente credito, filtrado por sucursal.</returns>
        public List<LocalidadesCLI> ObtenerlocalidadesVentaCredito(int idSucursal)
        {
            return LocalidadesVentaCreditoDatos.Instancia.ObtenerlocalidadesVentaCredito(idSucursal);
        }

        /// <summary>
        /// Retorna el Estado (0 = Inactivo / 1 = Activo) de una localidad para saber si es viable o no para que permita crear preenvios
        /// </summary>
        /// <param name="IdLocalidadDestino"></param>
        /// <returns>Estado de validez de la localidad destino</returns>
        public bool ConsultarValidezDestinoGeneracionGuias(string IdLocalidadDestino)
        {
            return LocalidadesVentaCreditoDatos.Instancia.ConsultarValidezDestinoGeneracionGuias(IdLocalidadDestino);
        }

        /// <summary>
        /// Consulta el SubTipo de un Centro de Servicios asociado a una localidad
        /// </summary>
        /// <param name="IdLocalidadCS"></param>
        /// <returns>Retorna el SubTipo de un Centro de Servicios asociado a una localidad</returns>
        public bool ConsultarSubtipoCentroServiciosPorLocalidad(string IdLocalidadCS, int idTipoEntrega)
        {
            return LocalidadesVentaCreditoDatos.Instancia.ConsultarSubtipoCentroServiciosPorLocalidad(IdLocalidadCS, idTipoEntrega);
        }

        /// <summary>
        /// Retorna el Id de la Zona más idónea para la nueva localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns>Devuelve "-1" si la Localidad la maneja, sino, entonces la primera del listado en orden ascendente</returns>
        public string ConsultarZonaHabilitadaParaNuevaLocalidad(string idLocalidad)
        {
            return LocalidadesVentaCreditoDatos.Instancia.ConsultarZonaHabilitadaParaNuevaLocalidad(idLocalidad);
        }

        /// <summary>
        /// Retorna Lista de Zonas Dificil Acceso
        /// </summary>
        /// <returns>Retorna Lista de Zonas Dificil Acceso</returns>
        public List<PAZonaDificilAcceso> ConlsutarZonaDificilAcceso()
        {
            return LocalidadesVentaCreditoDatos.Instancia.ConlsutarZonaDificilAcceso();
        }
    }
}
