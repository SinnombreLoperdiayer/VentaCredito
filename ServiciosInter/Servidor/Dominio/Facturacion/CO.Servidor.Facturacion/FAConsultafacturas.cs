using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Facturacion.Comun;
using CO.Servidor.Facturacion.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Servicios.ContratoDatos.Comun;

namespace CO.Servidor.Facturacion
{
    public class FAConsultafacturas : ControllerBase
    {
        #region CrearInstancia

        private static readonly FAConsultafacturas instancia = (FAConsultafacturas)FabricaInterceptores.GetProxy(new FAConsultafacturas(), COConstantesModulos.MODULO_FACTURACION);

        /// <summary>
        /// Retorna una instancia de programación de facturas
        /// /// </summary>
        public static FAConsultafacturas Instancia
        {
            get { return FAConsultafacturas.instancia; }
        }

        #endregion CrearInstancia

        private const string NotaCredito = "CRE";
        private const string Anulada = "ANULADA";
        private const string NotaDebito = "DEB";
        private const string DescripcionNotaDebito = "DEBITO";
        private IADFachadaAdmisionesMensajeria fachadaAdmision = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        #region Métodos públicos

        /// <summary>
        /// Metodo que consulta las facturas existentes en el sistema utilizando unos parámetros de búsqueda
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IEnumerable<FAFacturaClienteAutDC> ConsultaFacturasFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return FARepositorioConsFactura.Instancia.ConsultaFacturasFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Consulta la lista de guias asociadas a una factura de cliente crédito
        /// </summary>
        /// <param name="numeroFactura"></param>
        /// <returns></returns>
        public List<FAOperacionFacturadaDC> ConsultarOperacionesFactura(long numeroFactura)
        {
            return FARepositorioConsFactura.Instancia.ConsultarOperacionesFactura(numeroFactura);
        }

        /// <summary>
        /// Asocia un movimento (guía crédito) a una factura específica de un cliente crédito
        /// </summary>
        /// <param name="operacionFac"></param>
        public void AsociarOperacionAFactura(FAAsociacionGuiasFacturaDC datosAsociacion)
        {
            FARepositorioConsFactura.Instancia.AsociarOperacionAFactura(datosAsociacion);
        }

        /// <summary>
        /// Deasociar un movimiento (guía crédito) de una factura de un cliente crédito
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <param name="numeroFactura"></param>
        public void DesasociarOperacionAFactura(long numeroOperacion, long numeroFactura)
        {
            FARepositorioConsFactura.Instancia.DesasociarOperacionAFactura(numeroOperacion, numeroFactura);
        }

        /// <summary>
        /// Consulta la informacion basica de una factura por el numero de Operacion( Numero de giro)
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <returns></returns>
        public FAOperacionFacturadaDC ConsultarFacturaPorNumeroOperacion(long numeroOperacion)
        {
            return FARepositorioConsFactura.Instancia.ConsultarFacturaPorNumeroOperacion(numeroOperacion);
        }

        /// <summary>
        /// Anular la factura indicada
        /// </summary>
        /// <param name="numeroFactura"></param>
        public void AnularFactura(FAEstadoFacturaDC estadoFactura)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                FARepositorioConsFactura.Instancia.AnularFactura(estadoFactura);
                scope.Complete();
            }
        }

        /// <summary>
        /// Permite reprogramar una factura anulada para que nuevamente sea generada
        /// </summary>
        /// <param name="FechaNueva"></param>
        public void ReprogramarFactura(DateTime FechaNueva, FAFacturaClienteAutDC factura)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                FARepositorioConsFactura.Instancia.ReprogramarFactura(FechaNueva, factura);
                transaccion.Complete();
            }
        }

        /// <summary>
        /// permite aprobar una factura
        /// </summary>
        /// <param name="estadoFactura"></param>
        public void AprobarFactura(FAEstadoFacturaDC estadoFactura)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                FARepositorioConsFactura.Instancia.AprobarFactura(estadoFactura);
                transaccion.Complete();
            }
        }

        /// <summary>
        /// Agrega una nueva nota crédito o débito a una factura
        /// </summary>
        /// <param name="nota"></param>
        public FANotaFacturaDC AgregarNotaFactura(FANotaFacturaDC nota)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                FANotaFacturaDC notaRetorna;
                if (nota.TipoNota.IdTipoNota == NotaCredito)
                {
                    FAFacturaClienteAutDC factura = FARepositorioConsFactura.Instancia.ConsultarFactura(nota.NumeroFactura);
                    if ((factura.ValorTotalFactura + factura.TotalNotasDebito) < (factura.TotalNotasCredito + nota.ValorNota))
                    {
                        throw new FaultException<ControllerException>
                            (new ControllerException(COConstantesModulos.MODULO_FACTURACION,
                                FAEnumTipoErrorFacturacion.EX_ERROR_VALOR_NOTA.ToString(),
                                FAMensajesFacturacion.CargarMensaje(FAEnumTipoErrorFacturacion.EX_ERROR_VALOR_NOTA)));
                    }
                    else
                        notaRetorna = FARepositorioConsFactura.Instancia.AgregarNotaFactura(nota);
                }
                else
                    notaRetorna = FARepositorioConsFactura.Instancia.AgregarNotaFactura(nota);

                transaccion.Complete();
                return notaRetorna;
            }
        }

        /// <summary>
        /// Crea las Nvs Notas asociadas a cada Detalle de la Factura
        /// </summary>
        /// <param name="lstConceptosDtll">Lista del detalle que lleva Notas Credito o Debito</param>
        /// <returns>la lista del detalle con las inserciones Realizadas</returns>
        public ObservableCollection<FAConceptoFacturaDC> AgregarNotasAFactura(ObservableCollection<FAConceptoFacturaDC> lstConceptosDtll)
        {
            if (lstConceptosDtll != null && lstConceptosDtll.Count > 0)
            {
                lstConceptosDtll.ToList().ForEach(con =>
                {
                    if (con.LstNotasAsociadasAlConcepto != null)
                    {
                        con.LstNotasAsociadasAlConcepto.ToList().ForEach(not =>
                        {
                            not = AgregarNotaFactura(not);
                        });
                    }
                });
            }
            return lstConceptosDtll;
        }

        /// <summary>
        /// Elimina una nota crédito o débito a una factura
        /// </summary>
        /// <param name="nota"></param>
        public void EliminarNotaFactura(FANotaFacturaDC nota)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                FAFacturaClienteAutDC factura = FARepositorioConsFactura.Instancia.ConsultarFactura(nota.NumeroFactura);
                if (nota.TipoNota.IdTipoNota == NotaDebito)
                {
                    if ((factura.ValorTotalFactura + factura.TotalNotasDebito - factura.TotalNotasCredito - nota.ValorNota) > 0)
                        FARepositorioConsFactura.Instancia.EliminarNotaFactura(nota);
                    else
                        throw new FaultException<ControllerException>
                         (new ControllerException(COConstantesModulos.MODULO_FACTURACION,
                             FAEnumTipoErrorFacturacion.EX_VALOR_NEGATIVO_EN_FACTURA_POR_NOTA.ToString(),
                             FAMensajesFacturacion.CargarMensaje(FAEnumTipoErrorFacturacion.EX_VALOR_NEGATIVO_EN_FACTURA_POR_NOTA)));
                }
                else
                    FARepositorioConsFactura.Instancia.EliminarNotaFactura(nota);
                transaccion.Complete();
            }
        }

        /// <summary>
        /// Método para obtener los tipos de nota
        /// </summary>
        /// <returns></returns>
        public IList<FATipoNotaDC> ObtenerTiposNota()
        {
            return FARepositorioConsFactura.Instancia.ObtenerTiposNota();
        }

        #endregion Métodos públicos

        #region Consultas

        /// <summary>
        /// Consultar los motivos causa de una anualción de una factura
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FAMotivoAnulacionDC> ConsultarMotivosAnulacion()
        {
         return  FARepositorioConsFactura.Instancia.ConsultarMotivosAnulacion();
        }

        /// <summary>
        /// Método para consultar responsables de facturacion
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FAResponsableDC> ConsultarResponsables()
        {
            return FARepositorioConsFactura.Instancia.ConsultarResponsables();
        }

        /// <summary>
        /// Método de consulta de descripciones de nota credito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FADescripcionNotaDC> ConsultarDescripcionesNota()
        {
            return FARepositorioConsFactura.Instancia.ConsultarDescripcionesNota();
        }

        /// <summary>
        /// Método de consulta de estados de nota
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FAEstadoNotaDC> ConsultarEstadosNota()
        {
            return FARepositorioConsFactura.Instancia.ConsultarEstadosNota();
        }

        #endregion Consultas

        #region Impresion

        /// <summary>
        /// Método para obtener facturas para impresión con filtro
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IEnumerable<FAFacturaClienteAutDC> ConsultaFacturasImpresion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return FARepositorioImpresion.Instancia.ConsultaFacturasImpresion(filtro, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Método para generar las guías internas
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public IEnumerable<FAFacturaClienteAutDC> GenerarGuiasFacturas(IDictionary<string, string> filtro, List<long> listaNumFacturas, ARGestionDC gestionOrigen)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                IEnumerable<FAFacturaClienteAutDC> listaFacturas;

                if (listaNumFacturas != null && listaNumFacturas.Count() != 0)
                    listaFacturas = FARepositorioImpresion.Instancia.ConsultaFacturasListas(listaNumFacturas);
                else
                    listaFacturas = FARepositorioImpresion.Instancia.ConsultaFacturasImpresionCompletas(filtro);

                foreach (FAFacturaClienteAutDC fac in listaFacturas)
                {
                    if (fac.GuiaInterna.NumeroGuia == 0)
                    {
                        fac.GuiaInterna = new ADGuiaInternaDC
                        {
                            EsOrigenGestion = true,
                            GestionDestino = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty },
                            GestionOrigen = gestionOrigen,
                            DiceContener = FAConstantesFacturacion.DESCRIPCION_FACTURA + fac.NumeroFactura,
                            DireccionDestinatario = fac.DireccionRadicacion,
                            EsManual = false,
                            IdAdmisionGuia = 0,
                            LocalidadDestino = fac.CiudadRadicacion,
                            NombreDestinatario = fac.RazonSocial + "-" + fac.IdCliente,
                            NumeroGuia = 0,
                            PaisDefault = new PALocalidadDC { IdLocalidad = ConstantesFramework.ID_LOCALIDAD_COLOMBIA, Nombre = ConstantesFramework.DESC_LOCALIDAD_COLOMBIA },
                            TelefonoDestinatario = fac.TelefonoRadicacion,
                        };
                        fac.GuiaInterna = fachadaAdmision.AdicionarGuiaInterna(fac.GuiaInterna);
                        FARepositorioImpresion.Instancia.ActualizarGuiaFactura(fac);
                    }
                    else
                        fac.GuiaInterna = fachadaAdmision.ObtenerGuiaInterna(fac.GuiaInterna.NumeroGuia);
                }
                transaccion.Complete();
                return listaFacturas;
            }
        }

        #endregion Impresion
    }
}