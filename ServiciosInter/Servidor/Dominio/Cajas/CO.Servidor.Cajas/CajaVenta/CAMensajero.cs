using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CO.Servidor.Cajas.Apertura;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Servicios.ContratoDatos.Area;

namespace CO.Servidor.Cajas.CajaVenta
{
    /// <summary>
    /// Clase que contiene los metodos de consulta adicion y transacciones que un mensajero ejerce sobre la caja
    /// </summary>
    internal class CAMensajero : ControllerBase
    {
        #region Atributos

        private static readonly CAMensajero instancia = (CAMensajero)FabricaInterceptores.GetProxy(new CAMensajero(), COConstantesModulos.CAJA);

        #endregion Atributos

        #region Instancia

        public static CAMensajero Instancia
        {
            get { return CAMensajero.instancia; }
        }

        #endregion Instancia

        /// <summary>
        /// Adiciona las Transacciones de un Mensajero.
        /// </summary>
        /// <param name="registroMensajero">Clase Cuenta Mensajero.</param>
        public long AdicionarTransaccMensajero(CACuentaMensajeroDC registroMensajero)
        {
            return  CARepositorioCaja.Instancia.AdicionarTransaccMensajero(registroMensajero);
        }

        /// <summary>
        /// Adicionar Reporte del Mensajero.
        /// </summary>
        /// <param name="reportMensajero">Clase report mensajero.</param>
        public void AdicionarReporteMensajero(CAReporteMensajeroCajaDC reportMensajero)
        {
            CARepositorioCaja.Instancia.AdicionarReporteMensajero(reportMensajero);
        }

        
      /// <summary>
      /// Obtiene todos los reportes de caja de un mensajero
      /// </summary>
      /// <param name="idMensajero"></param>
      /// <returns></returns>
        public List<CAReporteMensajeroCajaDC> ObtenerReportesMensajeros(long idMensajero)
        {
            return CARepositorioCaja.Instancia.ObtenerReportesMensajeros(idMensajero);
        }


         /// <summary>
    /// Obtiene todos los reportes de caja de un mensajero por comprobante para imprimir
    /// </summary>
    /// <param name="idMensajero"></param>
    /// <param name="numComprobante"></param>
    /// <returns></returns>
        public CADatosImpCompMensajeroDC ObtenerReportesMensajerosImprimir(long idMensajero, string numComprobante)
        {
           CADatosImpCompMensajeroDC impre = CARepositorioCaja.Instancia.ObtenerReportesMensajerosImprimir(idMensajero, numComprobante);


           IARFachadaAreas fachadaEmpresa = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>();
           AREmpresaDC datosEmpresa = fachadaEmpresa.ObtenerDatosEmpresa();
         
           impre.ConsecutivoComprobante = long.Parse(numComprobante);
           impre.NitEmpresa = datosEmpresa.Nit;
           impre.NombreEmpresa = datosEmpresa.NombreEmpresa;

           return impre;
        }


        /// <summary>
        /// Recibir el dinero del mensajero.
        /// </summary>
        /// <param name="transaccionMensajero">Lista de Transacciones para el registro del dinero.</param>
        public void RecibirDineroMensajero(List<CARecibirDineroMensajeroDC> transaccionMensajero)
        {
            
            using (TransactionScope trans = new TransactionScope())
            {
                long idMensajero = 0;
                transaccionMensajero.ForEach(transaccion =>
                {
                    //Si los al cobros no han sido descargados en caja
                    if (transaccion.AlcobrosDescargados.Where(a => a.AfectadoEnCaja == false).Count() > 0 || transaccion.AlcobrosDescargados.Count==0)
                    {
                        long idTrasaccionCaja = CAApertura.Instancia.RegistrarVenta(transaccion.RegistroEnCajaMensajero).IdTransaccionCajaDtll.FirstOrDefault();                        
                        transaccion.ReporteMensajero.IdRegistroTransDetalleCaja = idTrasaccionCaja;                                                
                    }

                    transaccion.ReporteMensajero.NumeroComprobanteTransDetCaja = transaccion.RegistroEnCajaMensajero.RegistrosTransacDetallesCaja.FirstOrDefault().NumeroComprobante;

                    AdicionarReporteMensajero(transaccion.ReporteMensajero);
                    idMensajero = transaccion.ReporteMensajero.Mensajero.IdPersonaInterna;

                    AdicionarTransaccMensajero(transaccion.CuentaMensajero);
                    ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

                    //if (transaccion.CuentaMensajero.NoPlanillaAlCobros > 0)
                    //{
                    //  fachadaSuministros.ObtenerPropietarioSuministro(transaccion.CuentaMensajero.NoPlanillaAlCobros, SUEnumSuministro.PLANILLA_DIARIA_ALCOBROS, long.Parse(transaccion.CuentaMensajero.Mensajero.Identificacion));
                    //  fachadaSuministros.GuardarConsumoSuministro(new Servicios.ContratoDatos.Suministros.SUConsumoSuministroDC()
                    //  {
                    //    Cantidad = 1,
                    //    EstadoConsumo = SUEnumEstadoConsumo.CON,
                    //    GrupoSuministro = SUEnumGrupoSuministroDC.MEN,
                    //    IdDuenoSuministro = long.Parse(transaccion.CuentaMensajero.Mensajero.Identificacion),
                    //    IdServicioAsociado = 0,
                    //    NumeroSuministro = transaccion.CuentaMensajero.NoPlanillaAlCobros,
                    //    Suministro = SUEnumSuministro.PLANILLA_DIARIA_ALCOBROS
                    //  });
                    //}

                    if (transaccion.CuentaMensajero.NoPlanillaVentas > 0)
                    {
                        fachadaSuministros.ObtenerPropietarioSuministro(transaccion.CuentaMensajero.NoPlanillaVentas, SUEnumSuministro.TALONARIO_REPORTE_DIARIO_VENTAS, long.Parse(transaccion.CuentaMensajero.Mensajero.Identificacion));
                        fachadaSuministros.GuardarConsumoSuministro(new Servicios.ContratoDatos.Suministros.SUConsumoSuministroDC()
                        {
                            Cantidad = 1,
                            EstadoConsumo = SUEnumEstadoConsumo.CON,
                            GrupoSuministro = SUEnumGrupoSuministroDC.MEN,
                            IdDuenoSuministro = long.Parse(transaccion.CuentaMensajero.Mensajero.Identificacion),
                            IdServicioAsociado = 0,
                            NumeroSuministro = transaccion.CuentaMensajero.NoPlanillaVentas,
                            Suministro = SUEnumSuministro.TALONARIO_REPORTE_DIARIO_VENTAS
                        });
                    }
                });

                IOUFachadaOperacionUrbana fachadaOperUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
                fachadaOperUrbana.ActualizarAlCobrosEntregaMensajero(idMensajero, transaccionMensajero.FirstOrDefault().CuentaMensajero.NumeroDocumento, transaccionMensajero.FirstOrDefault().AlcobrosDescargados.Where(a=>a.ValorTotalGuia>0).ToList());
                trans.Complete();
            }
        }

        /// <summary>
        /// Obtiene las entregas del mensajero.
        /// </summary>
        /// <param name="idMensajero">The id mensajero.</param>
        /// <returns>Lista de Guias entregadas de alcobro  por mensajero</returns>
        public List<OUEnviosPendMensajerosDC> ObtenerEnviosPendMensajero(long idMensajero, long idComprobante)
        {
            IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();

            return fachadaOperacionUrbana.ObtenerEnviosPendMensajero(idMensajero, idComprobante);
        }

        /// <summary>
        /// Estados de la Cta mensajero.
        /// </summary>
        /// <param name="idMensajero">The id mensajero.</param>
        /// <param name="fechaConsulta">The fecha consulta.</param>
        /// <returns></returns>
        public List<CACuentaMensajeroDC> ObtenerEstadoCtaMensajero(long idMensajero, DateTime fechaConsulta)
        {
            return CARepositorioCaja.Instancia.ObtenerEstadoCtaMensajero(idMensajero, fechaConsulta);
        }

        /// <summary>
        /// Actualiza la Observacion de trans.
        /// </summary>
        /// <param name="CuentaMensajero">The cuenta mensajero.</param>
        public void ActualizarObservacionEstadoCta(CACuentaMensajeroDC cuentaMensajero)
        {
            CARepositorioCaja.Instancia.ActualizarObservacionEstadoCta(cuentaMensajero);
        }
    }
}