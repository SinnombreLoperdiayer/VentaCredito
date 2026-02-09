using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Suministros.Datos;

namespace CO.Servidor.Suministros
{
    public class SUAdministradorConsolidados : ControllerBase
    {
        private static readonly SUAdministradorConsolidados instancia = (SUAdministradorConsolidados)FabricaInterceptores.GetProxy(new SUAdministradorConsolidados(), COConstantesModulos.MODULO_SUMINISTROS);

        /// <summary>
        /// Retorna una instancia de administrador de suministros
        /// /// </summary>
        public static SUAdministradorConsolidados Instancia
        {
            get { return SUAdministradorConsolidados.instancia; }
        }

        #region Consultas
        /// <summary>
        /// Indica si un consolidado dado está activo o inactivo
        /// </summary>
        /// <returns></returns>
        /// <param name="codigo">Código del consolidado</param>
        public string ObtenerEstadoActivoConsolidado(string codigo)
        {
            return SUSuministros.Instancia.ObtenerEstadoActivoConsolidado(codigo);
        }
        /// <summary>
        /// Retorna los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public List<OUTipoConsolidadoDC> ObtenerTiposConsolidado()
        {
            return SUSuministros.Instancia.ObtenerTiposConsolidado();
        }

        /// <summary>
        /// Retorna los tamaños de la tula
        /// </summary>
        /// <returns></returns>
        public List<SUTamanoTulaDC> ObtenerTamanosTula()
        {
            return SUSuministros.Instancia.ObtenerTamanosTula();
        }

        /// <summary>
        /// Retorna los motivos de cambios de un contenedor
        /// </summary>
        /// <returns></returns>
        public List<SUMotivoCambioDC> ObtenerMotivosCambioContenedor()
        {
            return SUSuministros.Instancia.ObtenerMotivosCambioContenedor();
        }
        #endregion



        /// <summary>
        /// Registra un nuevo contenedor en la base de datos
        /// </summary>
        /// <param name="contenedor"></param>
        public void RegistrarNuevoContenedor(SUConsolidadoDC contenedor)
        {
            SUSuministros.Instancia.RegistrarNuevoContenedor(contenedor);
        }

        /// <summary>
        /// Registra una modificación de un contendor
        /// </summary>
        /// <param name="consolidado"></param>
        public void RegistrarModificacionContenedor(SUModificacionConsolidadoDC consolidado)
        {
            SUSuministros.Instancia.RegistrarModificacionContenedor(consolidado);
        }


        public IList<SUConsolidadoDC> ObtenerListaConsolidados(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return SURepositorioConsolidados.Instancia.ObtenerListaConsolidados(filtro, indicePagina, registrosPorPagina);
        }


    }
}
