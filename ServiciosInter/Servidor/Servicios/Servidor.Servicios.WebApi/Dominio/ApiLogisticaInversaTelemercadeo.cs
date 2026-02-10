using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.ModelosRequest.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiLogisticaInversaTelemercadeo : ApiDominioBase
    {

        private static readonly ApiLogisticaInversaTelemercadeo instancia = (ApiLogisticaInversaTelemercadeo)FabricaInterceptorApi.GetProxy(new ApiLogisticaInversaTelemercadeo(), COConstantesModulos.LOGISTICA_INVERSA);
        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        public static ApiLogisticaInversaTelemercadeo Instancia
        {
            get { return ApiLogisticaInversaTelemercadeo.instancia; }
        }


        /// <summary>
        /// Ingresa una guia a custodia
        /// </summary>
        /// <param name="custodia"></param>
        public void IngresarGuiaCustodia(PUCustodia custodia)
        {
            FabricaServicios.ServicioLogisticaInversaTelemercadeo.IngresoCustodia(custodia);
        }

        public int ObtenerNumeroDeEnviosEnUbicacion(int tipoUbicacion, int ubicacion)
        {
            return FabricaServicios.ServicioLogisticaInversaTelemercadeo.ObtenerNumeroDeEnviosEnUbicacion(tipoUbicacion, ubicacion);

        }

        /// <summary>
        /// permite sacar un envio de custodia
        /// </summary>
        /// <param name="custodia"></param>
        internal void SalidaCustodia(PUCustodia custodia)
        {
            FabricaServicios.ServicioLogisticaInversaTelemercadeo.SalidaCustodia(custodia);
        }

        /// <summary>
        /// permite obtener las planillas que continene guiasinternas devueltas
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="tipoImpreso"></param>
        /// <returns></returns>
        public IList<LIPlanillaDC> ObtenerPlanillas(int indicePagina, int registrosPorPagina, ADEnumTipoImpreso tipoImpreso, string idCentroServicioSeleccionado, string numeroPlanilla, DateTime? fechaInicial, DateTime? fechaFinal)
        {
            if (fechaInicial == null)
            {
                fechaInicial = DateTime.Now.AddDays(-8);
            }
            if (fechaFinal == null)
            {
                fechaFinal = DateTime.Now;
            }
            IDictionary<string, string> filtro = new Dictionary<string, string>();
            filtro.Add("PGG_NumeroPlanilla", numeroPlanilla);
            filtro.Add("centroServicio", idCentroServicioSeleccionado);
            filtro.Add("tipoCliente", null);
            filtro.Add("fechaInicial", fechaInicial.ToString());
            filtro.Add("fechaFinal", fechaFinal.ToString());
            return FabricaServicios.ServicioLogisticaInversaTelemercadeo.ObtenerPlanillas(filtro, indicePagina, registrosPorPagina, tipoImpreso);
        }

        /// <summary>
        /// adiciona la planilla para el ingreso de la guias de devolucion ratificada
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public LIPlanillaDC AdicionarPlanilla(PlanillaRequest planilla)
        {
            LIPlanillaDC pla = new LIPlanillaDC()
            {
                TipoPlanilla = planilla.TipoPlanilla,
                TipoCliente = planilla.TipoCliente,
                ClienteCredito = new ContratoDatos.Clientes.CLClientesDC()
                {
                    Nit = planilla.NitClienteCredito
                },
                EsConsolidado = planilla.EsConsolidado,
                CentroServicios = new PUCentroServiciosDC()
                {
                    IdCentroServicio = planilla.idCentroServicio,
                    Nombre = planilla.nombreCentroServicios
                },
                CreadoPor = planilla.CreadoPor

            };

            return FabricaServicios.ServicioLogisticaInversaTelemercadeo.AdicionarPlanilla(pla);
        }

        public long GuardarCambioEstado(LICambioEstadoCustodia ceCustodia)
        {
            return FabricaServicios.ServicioLogisticaInversaTelemercadeo.GuardarCambioEstadoWeb(ceCustodia);
        }

        public LIPlanillaDetalleDC AdicionarGuiaPlanillaContado(PlanillaDetalleRequest guiaSeleccionada)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                LIPlanillaDetalleDC guiaPla = new LIPlanillaDetalleDC()
                {
                    AdmisionMensajeria = new ADGuia { NumeroGuia = guiaSeleccionada.AdmisionNumeroGuia },
                    NumeroPlanilla = guiaSeleccionada.NumeroPlanilla,
                    GuiaInterna = new ADGuiaInternaDC
                    {
                        IdCentroServicioOrigen = guiaSeleccionada.GuiaInternaIdCentroServicioOrigen,
                        NombreCentroServicioOrigen = guiaSeleccionada.GuiaInternaNombreCentroServicioOrigen,
                        PaisDefault = new PALocalidadDC()
                        {
                            IdLocalidad = guiaSeleccionada.PaisDefaultIdLocalidad,
                            Nombre = guiaSeleccionada.PaisDefaultNombre
                        },
                        LocalidadOrigen = new PALocalidadDC()
                        {
                            IdLocalidad = guiaSeleccionada.LocalidadOrigenIdLocalidad,
                            Nombre = guiaSeleccionada.LocalidadOrigenNombre
                        },
                        EsOrigenGestion = false
                    }
                };
                ADGuia guia = new ADGuia();
                guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guiaSeleccionada.AdmisionNumeroGuia);
                ADTrazaGuia estadoGuia = new ADTrazaGuia
                {
                    Ciudad = ControllerContext.Current.NombreCentroServicio,
                    IdCiudad = ControllerContext.Current.IdCentroServicio.ToString(),
                    IdAdmision = guia.IdAdmision,
                    IdEstadoGuia = (short)ADEnumEstadoGuia.Custodia,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRatificada,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guiaSeleccionada.AdmisionNumeroGuia,
                    Observaciones = string.Empty,
                    FechaGrabacion = DateTime.Now
                };
                estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                LIPlanillaDetalleDC planillaDetalle = FabricaServicios.ServicioLogisticaInversaTelemercadeo.AdicionarGuiaPlanillaContado(guiaPla);
                trans.Complete();
                return planillaDetalle;
            }
        }

        /// <summary>
        /// metodo que permite registrar las guias internas a cliente peaton
        /// </summary>
        public LIPlanillaDetalleDC AdicionarGuiaPlanillaCredito(PlanillaDetalleRequest guiaSeleccionada)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                LIPlanillaDetalleDC guiaPla = new LIPlanillaDetalleDC()
                {
                    AdmisionMensajeria = new ADGuia { NumeroGuia = guiaSeleccionada.AdmisionNumeroGuia },
                    NumeroPlanilla = guiaSeleccionada.NumeroPlanilla,
                    GuiaInterna = new ADGuiaInternaDC
                    {
                        IdCentroServicioOrigen = guiaSeleccionada.GuiaInternaIdCentroServicioOrigen,
                        NombreCentroServicioOrigen = guiaSeleccionada.GuiaInternaNombreCentroServicioOrigen,
                        PaisDefault = new PALocalidadDC()
                        {
                            IdLocalidad = guiaSeleccionada.PaisDefaultIdLocalidad,
                            Nombre = guiaSeleccionada.PaisDefaultNombre
                        },
                        LocalidadOrigen = new PALocalidadDC()
                        {
                            IdLocalidad = guiaSeleccionada.LocalidadOrigenIdLocalidad,
                            Nombre = guiaSeleccionada.LocalidadOrigenNombre
                        },
                        EsOrigenGestion = false
                    }
                };
                ADGuia guia = new ADGuia();
                guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guiaSeleccionada.AdmisionNumeroGuia);
                ADTrazaGuia estadoGuia = new ADTrazaGuia
                {
                    Ciudad = ControllerContext.Current.NombreCentroServicio,
                    IdCiudad = ControllerContext.Current.IdCentroServicio.ToString(),
                    IdAdmision = guia.IdAdmision,
                    IdEstadoGuia = (short)ADEnumEstadoGuia.Custodia,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRatificada,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guiaSeleccionada.AdmisionNumeroGuia,
                    Observaciones = string.Empty,
                    FechaGrabacion = DateTime.Now
                };
                estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                LIPlanillaDetalleDC planillaDetalle = FabricaServicios.ServicioLogisticaInversaTelemercadeo.AdicionarGuiaPlanillaCredito(guiaPla);
                trans.Complete();
                return planillaDetalle;
            }

        }

        public LIPlanillaDetalleDC AdicionarGuiaPlanilla(PlanillaDetalleRequest guiaSeleccionada)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                LIPlanillaDetalleDC guiaPla = new LIPlanillaDetalleDC()
                {
                    AdmisionMensajeria = new ADGuia
                    {
                        NumeroGuia = guiaSeleccionada.AdmisionNumeroGuia,
                        IdCiudadDestino = guiaSeleccionada.idCiudadNuevoDestinatario,
                        NombreCiudadDestino = guiaSeleccionada.nombreCiudadNuevoDestinatario,
                        IdCiudadOrigen = guiaSeleccionada.LocalidadOrigenIdLocalidad,
                        NombreCiudadOrigen = guiaSeleccionada.LocalidadOrigenNombre,
                        ValorTotal = guiaSeleccionada.valorTotal,
                        Destinatario = new CLClienteContadoDC
                        {
                            TipoId = guiaSeleccionada.tipoIdNuevoDestinatario,
                            Identificacion = guiaSeleccionada.numeroIdNuevodestinatario,
                            Direccion = guiaSeleccionada.direccionNuevoDestinatario,
                            Nombre = guiaSeleccionada.nombreNuevoDestinatario,
                            Telefono = guiaSeleccionada.telefonoNuevoDestinatario,
                            Email = guiaSeleccionada.emailNuevoDestinatario
                        }
                    },
                    NumeroPlanilla = guiaSeleccionada.NumeroPlanilla,
                    caja = guiaSeleccionada.caja,
                    GuiaInterna = new ADGuiaInternaDC
                    {
                        IdCentroServicioOrigen = guiaSeleccionada.GuiaInternaIdCentroServicioOrigen,
                        NombreCentroServicioOrigen = guiaSeleccionada.GuiaInternaNombreCentroServicioOrigen,
                        TelefonoRemitente = guiaSeleccionada.GuiaInternaTelefonoRemitente,
                        DireccionRemitente = guiaSeleccionada.GuiaInternaNombreCentroServicioOrigen,
                        PaisDefault = new PALocalidadDC()
                        {
                            IdLocalidad = guiaSeleccionada.PaisDefaultIdLocalidad,
                            Nombre = guiaSeleccionada.PaisDefaultNombre
                        },
                        LocalidadOrigen = new PALocalidadDC()
                        {
                            IdLocalidad = guiaSeleccionada.LocalidadOrigenIdLocalidad,
                            Nombre = guiaSeleccionada.LocalidadOrigenNombre
                        },
                        EsOrigenGestion = false
                    },
                    DestinatarioModificado = guiaSeleccionada.modificoDestinatario

                };
                ADGuia guia = new ADGuia();
                guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guiaSeleccionada.AdmisionNumeroGuia);
                ADTrazaGuia estadoGuia = new ADTrazaGuia
                {
                    Ciudad = ControllerContext.Current.NombreCentroServicio,
                    IdCiudad = ControllerContext.Current.IdCentroServicio.ToString(),
                    IdAdmision = guia.IdAdmision,
                    IdEstadoGuia = (short)ADEnumEstadoGuia.Custodia,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRatificada,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guiaSeleccionada.AdmisionNumeroGuia,
                    Observaciones = string.Empty,
                    FechaGrabacion = DateTime.Now
                };
                estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                LIPlanillaDetalleDC planillaDetalle = FabricaServicios.ServicioLogisticaInversaTelemercadeo.AdicionarGuiaPlanilla(guiaPla);
                trans.Complete();
                return planillaDetalle;
            }
        }

        public IList<LIPlanillaDetalleDC> ObtenerGuiasPlanilla(long numeroPlanilla)
        {
            LIPlanillaDC plaGui = new LIPlanillaDC()
            {
                NumeroPlanilla = numeroPlanilla
            };
            return FabricaServicios.ServicioLogisticaInversaTelemercadeo.ObtenerGuiasPlanilla(plaGui);
        }

        public IList<LISalidaCustodia> ObtenerSalidasCustodiaPorDia(long idCentroServicio, DateTime fechaConsulta)
        {
            return FabricaServicios.ServicioLogisticaInversaTelemercadeo.ObtenerSalidasCustodiaPorDia(idCentroServicio, fechaConsulta);
        }

        public void EliminarGuiaPLanilla(string idPlanillaGuia)
        {
            LIPlanillaDetalleDC guiaSeleccionada = new LIPlanillaDetalleDC()
            {
                IdPlanillaGuia = Convert.ToInt64(idPlanillaGuia)
            };
            FabricaServicios.ServicioLogisticaInversaTelemercadeo.EliminarGuiaPLanilla(guiaSeleccionada);
        }
    }
}