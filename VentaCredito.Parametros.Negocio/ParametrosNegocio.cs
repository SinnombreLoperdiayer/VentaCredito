using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Transversal.Entidades.Parametros;
using VentaCredito.Transversal.Enumerables;
using VentaCredito.ParametrosC.Datos;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using System.Runtime;
using System.Runtime.Caching;
using VentaCredito.Transversal.Const;
using VentaCredito.LocalidadesNegocio;

namespace VentaCredito.Parametros.Negocio
{
    public class ParametrosNegocio
    {
        private static ParametrosNegocio instancia = new ParametrosNegocio();
        public static ParametrosNegocio Instancia
        {
            get
            {
                return instancia;
            }
        }

        /// <summary>
        /// Obtiene el valor minimo declarado por id lista de precios del cliente credito filtrado por peso.
        /// Hevelin Dayana Diaz - 14/02/2022
        /// </summary>
        /// <param name="idListaPrecios"></param>
        /// <param name="peso"></param>
        /// <returns>Valor minimo declarado segun el peso</returns>
        public TAValorPesoDeclaradoDC ObtenerValorPesoDeclaradoClienteCredito(decimal peso, int idListaPrecios)
        {
            TAValorPesoDeclaradoDC valorMinimoDeclarado = new TAValorPesoDeclaradoDC();
            TAValorPesoDeclaradoDC listaPrecios = ParametrosDatos.Instancia.ObtenerValorPesoDeclarado(idListaPrecios, peso);
            if (listaPrecios.IdListaPrecio != 0)
            {
                valorMinimoDeclarado = listaPrecios;
            }
            return valorMinimoDeclarado;
        }

        public List<PAZonaDificilAcceso> ObtenerZonasDesdeCache()
        {
            var cache = MemoryCache.Default;
            if (cache.Contains(CacheConst.CacheKey))
            {
                return (List<PAZonaDificilAcceso>)cache.Get(CacheConst.CacheKey);
            }

            List<PAZonaDificilAcceso> zonas = LocalidadesNegocio.LocalidadesNegocio.Instancia.ConlsutarZonaDificilAcceso();
            cache.Set(CacheConst.CacheKey, zonas, DateTimeOffset.Now.AddMinutes(30));
            return zonas;
        }
    }
}
