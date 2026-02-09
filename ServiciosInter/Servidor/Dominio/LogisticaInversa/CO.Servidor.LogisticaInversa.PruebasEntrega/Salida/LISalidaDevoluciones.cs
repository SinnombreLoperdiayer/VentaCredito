using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using System.Linq;
using CO.Servidor.Dominio.Comun.CentroAcopio;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.LogisticaInversa.PruebasEntrega.Descargue;
using CO.Servidor.Servicios.ContratoDatos.Suministros;


namespace CO.Servidor.LogisticaInversa.PruebasEntrega.Salida
{
    public class LISalidaDevoluciones : ControllerBase
    {
        #region Instancia

        private static readonly LISalidaDevoluciones instancia = (LISalidaDevoluciones)FabricaInterceptores.GetProxy(new LISalidaDevoluciones(), COConstantesModulos.PRUEBAS_DE_ENTREGA);

        public static LISalidaDevoluciones Instancia
        {
            get { return LISalidaDevoluciones.instancia; }
        }

        public LISalidaDevoluciones()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        private IPUFachadaCentroServicios fachadaCes = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        private IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
        #endregion Instancia


        #region Métodos

        /// <summary>
        /// Obtiene las guias para asignar que se encuentran en bodega
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC AsignarGuiaCentroAcopio(long numeroGuia, long idCol)
        {
            OUGuiaIngresadaDC guiaIngresada = null;
            try
            {
                guiaIngresada = LIRepositorioCentroDevoluciones.Instancia.ObtenerGuiaBodegaPorAsignar(numeroGuia);
                if (guiaIngresada != null)
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        PUMovimientoInventario salidaMovimiento = new Servicios.ContratoDatos.CentroServicios.PUMovimientoInventario
                        {
                            TipoMovimiento = PUEnumTipoMovimientoInventario.Salida,
                            IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio,
                            Bodega = new PUCentroServiciosDC
                            {
                                IdCentroServicio = ControllerContext.Current.IdCentroServicio,
                            },
                            NumeroGuia = numeroGuia,
                            FechaGrabacion = DateTime.Now,
                            FechaEstimadaIngreso = DateTime.Now,
                            CreadoPor = ControllerContext.Current.Usuario,
                        };
                        fachadaCes.AdicionarMovimientoInventario(salidaMovimiento);

                        PUMovimientoInventario asignacionMovimiento = new Servicios.ContratoDatos.CentroServicios.PUMovimientoInventario
                        {
                            TipoMovimiento = PUEnumTipoMovimientoInventario.Asignacion,
                            IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio,
                            Bodega = new PUCentroServiciosDC
                            {
                                IdCentroServicio = idCol,
                            },
                            NumeroGuia = numeroGuia,
                            FechaGrabacion = DateTime.Now,
                            FechaEstimadaIngreso = DateTime.Now,
                            CreadoPor = ControllerContext.Current.Usuario,
                        };
                        fachadaCes.AdicionarMovimientoInventario(asignacionMovimiento);
                        transaccion.Complete();
                        return guiaIngresada;
                    }
                }
                else
                {
                    return guiaIngresada;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Metodo para obtener guia para asignar a planilla auditor
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerGuiaBodegaPorAsignarAuditor(long numeroGuia)
        {
            return LIRepositorioCentroDevoluciones.Instancia.ObtenerGuiaBodegaPorAsignarAuditor(numeroGuia);
        }

        /// <summary>
        /// Metodo para guardar planilla asignacion
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public OUPlanillaAsignacionDC GestionarPlanillaAuditor(OUPlanillaAsignacionDC planilla)
        {
            SUNumeradorPrefijo numeroSuministro = new SUNumeradorPrefijo();
            OUGuiaIngresadaDC guia = ObtenerGuiaBodegaPorAsignarAuditor(planilla.Guias.NumeroGuia.Value);
            if (guia != null)
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    planilla.Guias.NumeroGuia = guia.NumeroGuia;
                    planilla.Guias.IdAdmision = guia.IdAdmision;
                    planilla.Guias.IdCiudad = guia.IdCiudad;
                    planilla.Guias.Ciudad = guia.Ciudad;


                    planilla = fachadaOperacionUrbana.GuardarPlanillaAsignacionEnvio(planilla);
                    planilla.Guias.DiceContener = guia.DiceContener;
                    planilla.Guias.NombreTipoEnvio = guia.NombreTipoEnvio;
                    planilla.Guias.FechaAsignacion = guia.FechaAsignacion;

                    //Consultar suministro
                    numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.SUMINISTRO_CONSECUTIVO_TAPA_AUDITORIA);

                    //Adicionar Tapa Logistica Inversa
                    LITapasLogisticaInversa.Instancia.AdicionarTapaLogistica(new LITapaLogisticaDC()
                    {
                        NumeroGuia = planilla.Guias.NumeroGuia.Value,
                        Impresa = false,
                        NumeroTapaLogistica = numeroSuministro.ValorActual,
                        Tipo = LIEnumTipoTapaLogisticaDC.Auditoria,
                        FechaGrabacion = DateTime.Now,
                        CreadoPor = ControllerContext.Current.Usuario
                    });

                    transaction.Complete();
                };

                return planilla;
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.LOGISTICA_INVERSA, "0", "La guía no se encuentra para auditoria"));
            }
        }

        /// <summary>
        /// Asignar mensajero a planilla
        /// </summary>
        /// <param name="planilla"></param>
        public void AsignarPlanillaMensajero(OUPlanillaAsignacionDC planilla)
        {
            fachadaOperacionUrbana.AsignaMensajeroPlanilla(planilla);
        }

        /// <summary>
        /// Obtener planilla asignacion centro logistico
        /// </summary>
        /// <returns></returns>
        public List<OUPlanillaAsignacionDC> ObtenerPlanillasAsignacionCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, bool incluyeFecha)
        {
            return fachadaOperacionUrbana.ObtenerPlanillasAsignacionCentroLogistico(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCentroServicios, incluyeFecha);
        }


        /// <summary>
        /// Metodo para obtener mensajeros auditores
        /// </summary>
        /// <param name="puntoServicio"></param>
        /// <param name="esAgencia"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajerosAuditores(long puntoServicio, bool esAgencia)
        {
            return fachadaOperacionUrbana.ObtenerMensajerosAuditores(puntoServicio, esAgencia);
        }
        #endregion


    }
}
