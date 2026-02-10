using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Tarifas.Comun;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Tarifas.Servicios
{
    /// <summary>
    /// Clase para manejo de Tarifa Internacional
    /// </summary>
    internal class TAServicioInternacional : TAServicio
    {
        #region Campos

        private static readonly TAServicioInternacional instancia = (TAServicioInternacional)FabricaInterceptores.GetProxy(new TAServicioInternacional(), COConstantesModulos.TARIFAS);

        private int idServicio = TAConstantesServicios.SERVICIO_INTERNACIONAL;

        #endregion Campos

        #region Propiedades

        public static TAServicioInternacional Instancia
        {
            get { return TAServicioInternacional.instancia; }
        }

        /// <summary>
        /// Retorna el identificador del servicio a partir de su identificador interno
        /// </summary>
        public int IdServicio
        {
            get
            {
                return this.idServicio;
            }
            set
            {
                this.idServicio = value;
            }
        }

        #endregion Propiedades

        #region Métodos Públicos

        /// <summary>
        /// Calcular el precio de la tarifa internacional
        /// </summary>
        /// <param name="idListaPrecios">Identificador de la lista de precios</param>
        /// <returns>Información del precio del servicio y los impuestos</returns>
        public TAPrecioServicioDC CalcularPrecio(int idListaPrecios, int tipoEmpaque, string idLocalidadDestino, decimal peso, string idZona, decimal valorDeclarado)
        {
            string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(IdServicio, idListaPrecios);
            int idLp = int.Parse(idListaPrecioServicio);

            TAPrecioServicioDC precio = new TAPrecioServicioDC()
            {
                Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(IdServicio),
                ValoresAdicionales = TARepositorio.Instancia.ObtenerValorValoresAdicionalesServicio(IdServicio),
                Valor = TARepositorio.Instancia.ObtenerPrecioInternacional(tipoEmpaque, idLp, idLocalidadDestino, peso, idZona),
                PrimaSeguro = TARepositorio.Instancia.ObtenerPrimaSeguro(idListaPrecios, valorDeclarado, idServicio)
            };

            double? valorDolar = 0;

            // Si el precio es diferente de null entonces se calcula la el precio en pesos (dado que el valor viene en dólares)
            if (precio != null)
            {
                if (Cache.Instancia.ContainsKey(ConstantesFramework.CACHE_DOLAR_EN_PESOS))
                {
                    valorDolar = (double)Cache.Instancia[ConstantesFramework.CACHE_DOLAR_EN_PESOS];
                }
                else
                {
                    valorDolar = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerValorDolarEnPesos();
                }
            }

            decimal porceRecargoCombus = 0, valorRecargo;

            ///Obtiene el porcentaje de recargo de combustible para el operador Postal
            porceRecargoCombus = PAAdministrador.Instancia.ObtenerPorcentajeRecargoCombustibleOPxZona(idZona);

            ///Calcula el valor con la TRM
            precio.Valor *= (decimal)valorDolar;

            ///Calcula el valor del recargo con base en el valor de la tarifa en pesos
            valorRecargo = precio.Valor * porceRecargoCombus / 100;

            ///Le adiciona al valor el recargo del combustible
            precio.Valor += valorRecargo;
            precio.TRM = (decimal)valorDolar;
            return precio;
        }

        public void ObtenerPreciosServicoInternacional(int idListaPrecios)
        {
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
            IEnumerable<TAServicioInternacionalPrecioDC> coleccionServicioInternacional = TARepositorioInternacional.Instancia.ObtenerZonasPorOperadorPostal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdServicio, idListaPrecio);
            return coleccionServicioInternacional;
        }

        /// <summary>
        /// Guarda los cambios realizados en el Usercontrol de Tarifa Internacional
        /// </summary>
        /// <param name="consolidadoTarifaInternacional">Objeto con los cambios realizados</param>
        public void GuardarTarifaInternacional(TATarifaInternacionalDC consolidadoTarifaInternacional)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                ActualizarPrecioInternacional(consolidadoTarifaInternacional.ServicioInternacional);
                TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(consolidadoTarifaInternacional.FormasPago, IdServicio);
                consolidadoTarifaInternacional.ServicioPeso.IdServicio = IdServicio;
                TARepositorio.Instancia.EditarServicioPeso(consolidadoTarifaInternacional.ServicioPeso);
                consolidadoTarifaInternacional.ListaPrecioParametros.IdServicio = IdServicio;
                TARepositorio.Instancia.EditarParametrosListaPrecioServicio(consolidadoTarifaInternacional.ListaPrecioParametros);
                TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(consolidadoTarifaInternacional.Impuestos, IdServicio);

                //Guarda el porcentaje de Recargo sea mod o nuevo
                ActualizarParametroPorcentajeRecargo(consolidadoTarifaInternacional.PorcentajeRecargo);

                //Guarda el Valor del Dolar sea mod ó nuevo
                ActualizarValorDolarPorDefecto(consolidadoTarifaInternacional.ValorDolarSinSistema);
                transaccion.Complete();
            }
        }

        /// <summary>
        /// Obtiene el porcentaje de Recargo
        /// </summary>
        /// <returns>double PorcentajedeRecargo</returns>
        public double ObtenerPorcentajeRecargo()
        {
            IADFachadaAdmisionesMensajeria fachadaAdmisionesM = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
            return fachadaAdmisionesM.ObtenerParametrosAdmisiones().PorcentajeRecargo;
        }

        /// <summary>
        /// Obtiene el Valor regiatrado por defecto
        /// del dolar cuando no alla red para consultarlo
        /// </summary>
        /// <returns>valor del dolar decimal</returns>
        public decimal ObtenerValorDolarSinRed()
        {
            decimal valorDolarDecimal = 0;

            string valorDolarString = SEProveedor.Instancia.ObtieneParametrosFremework(TAConstantesTarifas.PARAMETRO_CONSULTA_VALOR_DOLAR);

            if (!string.IsNullOrEmpty(valorDolarString))
            {
                valorDolarDecimal = Convert.ToDecimal(valorDolarString);
            }
            return valorDolarDecimal;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        /// <summary>
        /// Adiciona, Edita o elimina los cambios realizados a precio internacional
        /// </summary>
        /// <param name="consolidadoCambios">Colección con los cambios realizados</param>
        private void ActualizarPrecioInternacional(IEnumerable<TAServicioInternacionalPrecioDC> consolidadoCambios)
        {
            consolidadoCambios.ToList().ForEach(tarifaInternacional =>
              {
                  if (tarifaInternacional.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                  {
                      tarifaInternacional.IdServicio = IdServicio;
                      TARepositorioInternacional.Instancia.AdicionarPrecioInternacional(tarifaInternacional);
                  }

                  if (tarifaInternacional.EstadoRegistro == EnumEstadoRegistro.MODIFICADO || tarifaInternacional.TipoEmpaque.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                      TARepositorioInternacional.Instancia.EditarPrecioInternacional(tarifaInternacional);

                  if (tarifaInternacional.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                      TARepositorioInternacional.Instancia.EliminarPrecioInternacional(tarifaInternacional);
              });
        }

        /// <summary>
        /// Actualiza el valor del porcentaje de
        /// recargo
        /// </summary>
        /// <param name="valor">el valor a actualizar</param>
        private void ActualizarParametroPorcentajeRecargo(double porcentaje)
        {
            IADFachadaAdmisionesMensajeria fachadaAdmisionesM = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
            fachadaAdmisionesM.ActualizarParametroPorcentajeRecargo(porcentaje);
        }

        /// <summary>
        /// Guarda y actualiza el valor del dolar
        /// por defecto el cual se utiliza en caso de no tener
        /// acceso a internet
        /// </summary>
        /// <param name="valorDolar">valor del dolar a guardar</param>
        private void ActualizarValorDolarPorDefecto(decimal valorDolar)
        {
            SEProveedor.Instancia.ActualizarValorDolarPorDefecto(valorDolar.ToString());
        }

        #endregion Métodos Privados
    }
}