using System;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Tarifas.Servicios;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Tarifas
{
    /// <summary>
    /// Clase para la publicación de lógica para configuración de Tarifas Internacional
    /// </summary>
    public class TAAdministradorInternacional
    {
        private static TAAdministradorInternacional instancia = new TAAdministradorInternacional();

        private TAAdministradorInternacional()
        {
        }

        /// <summary>
        /// Retorna una instancia del administrador de servicio internacional
        /// </summary>
        public static TAAdministradorInternacional Instancia
        {
            get { return TAAdministradorInternacional.instancia; }
        }

        private TAServicioInternacional tarifaInternacional;

        /// <summary>
        /// Valida que tarifaInternacional no sea null
        /// </summary>
        private void ValidarInstanciaServicio()
        {
            if (tarifaInternacional == null)
            {
                tarifaInternacional = TAServicioInternacional.Instancia;
            }
        }

        /// <summary>
        /// Obtiene precio internacional
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de precio internacional</returns>
        public IEnumerable<TAServicioInternacionalPrecioDC> ObtenerZonasPorOperadorPostal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            ValidarInstanciaServicio();

            return tarifaInternacional.ObtenerZonasPorOperadorPostal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio);
        }

        /// <summary>
        /// Guarda los cambios realizados en el Usercontrol de Tarifa Internacional
        /// </summary>
        /// <param name="consolidadoTarifaInternacional">Objeto con los cambios realizados</param>
        public void GuardarTarifaInternacional(TATarifaInternacionalDC consolidadoTarifaInternacional)
        {
            ValidarInstanciaServicio();

            tarifaInternacional.GuardarTarifaInternacional(consolidadoTarifaInternacional);
        }

        /// <summary>
        /// Obtiene el porcentaje de Recargo
        /// </summary>
        /// <returns>double PorcentajedeRecargo</returns>
        public double ObtenerPorcentajeRecargo()
        {
            return TAServicioInternacional.Instancia.ObtenerPorcentajeRecargo();
        }

        /// <summary>
        /// Obtiene el Valor regiatrado por defecto
        /// del dolar cuando no alla red para consultarlo
        /// </summary>
        /// <returns>valor del dolar decimal</returns>
        public decimal ObtenerValorDolarSinRed()
        {
            return TAServicioInternacional.Instancia.ObtenerValorDolarSinRed();
        }
    }
}