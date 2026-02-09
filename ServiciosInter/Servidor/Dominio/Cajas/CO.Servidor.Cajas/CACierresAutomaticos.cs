using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using CO.Servidor.Cajas.CACierreCaja;
using CO.Servidor.Cajas.CajaFinanciera;
using CO.Servidor.Cajas.CajaVenta;
using CO.Servidor.Cajas.Comun;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Cajas
{
    /// <summary>
    /// Clase que realiza la opracion del
    /// cierre automatico de las cajas.
    /// </summary>
    public class CACierresAutomaticos : ControllerBase
    {
        #region Atributos

        private static readonly CACierresAutomaticos instancia = (CACierresAutomaticos)FabricaInterceptores.GetProxy(new CACierresAutomaticos(), COConstantesModulos.CAJA);

        #endregion Atributos

        #region Instancia

        public static CACierresAutomaticos Instancia
        {
            get { return instancia; }
        }

        #endregion Instancia

        /// <summary>
        /// Proceso de Cierre automatico de Puntos
        /// Centros de Servicio
        /// </summary>
        public void CierreAutomaticoCajasAgencias()
        {
            //Se debe de realizar primero el Cierre de Puntos y centros de Servicio para luego
            //Realizar el Cierre de Racol - Operacion Nacional - Banco y Casa Matriz
            //Proceso de Cierre automatico de Puntos
            //y Centros de Servicio

            CACierreCaja.CACierreCaja.Instancia.CrearAperturasEnCero(DateTime.Now.AddDays(-1));
            List<CACierreAutomaticoDC> cajasAbiertas = CACierreCaja.CACierreCaja.Instancia.ObtenerCentrosSvcConCajasAbiertas();

            if (cajasAbiertas != null && cajasAbiertas.Count > 0)

            {
                cajasAbiertas.ForEach(CentroSvc =>
                {
                    try
                    {
                        //crear transaccion para cerrar las Cajas auxiliares
                        CARegistroTransacCajaDC movimientoCaja = CrearTransaccionCierreCaja(CentroSvc.IdCentroServicios, CentroSvc.NombreCentroServicio, CentroSvc.TipoCentroSvc);

                        using (TransactionScope scope = new TransactionScope())
                        {
                            //Cierro y reporto las Cajas de los Auxiliares
                            CACierreCaja.CACierreCaja.Instancia.CerrarCajasAutomaticamentePorCentroSvc(movimientoCaja);

                            //Lleno la Data necesaria para realizar el cierre del punto
                            CACierreCentroServicioDC infoCierreCentroSvc = CACajaPrincipal.Instancia
                                                                            .LlenarInfoCierrePuntoAutomatico(CACajaPrincipal.Instancia.ObtenerResumenCierrePunto(CentroSvc.IdCentroServicios, 0, true),
                                                                                                              CentroSvc.IdCentroServicios, ConstantesFramework.USUARIO_SISTEMA);

                            //Cierro la Caja del Centro de Servicio
                            CACierreCaja.CACierreCaja.Instancia.CerrarCajaPuntoCentroServcio(infoCierreCentroSvc, true);

                            scope.Complete();
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.Write("Fallo por " + exc.ToString());
                        AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.ToString(), COConstantesModulos.CAJA,exc);
                    }
                });
            }

            //Proceso de cierre de Cajas de Gestion
            List<CAAperturaCajaCasaMatrizDC> cajasGestionAbiertas = CACajaFinanciera.Instancia.ObtenerAperturasCajaGestion().ToList();

            using (TransactionScope scope = new TransactionScope())
            {
                if (cajasGestionAbiertas != null && cajasGestionAbiertas.Count > 0)
                {
                    cajasGestionAbiertas.ForEach(caj =>
                    {
                        CACierreCajaGestion.Instancia.CerrarCajaGestion(caj.IdCasaMatriz, ConstantesFramework.ID_USUARIO_SISTEMA, 0);
                    });
                }

                scope.Complete();
            }
        }

        /// <summary>
        /// Se crea la Transaccion para el cierre del cajas y punto
        /// </summary>
        public CARegistroTransacCajaDC CrearTransaccionCierreCaja(long idCentroSrv, string nombreCentroSrv, string tipoCentroSrv)
        {
           // PUAgenciaDeRacolDC centroSrv = CACaja.Instancia.ObtenerResponsableCentroSrvSegunTipo(idCentroSrv, tipoCentroSrv);

            CARegistroTransacCajaDC OperacionPunto = new CARegistroTransacCajaDC()
            {
                IdCentroServiciosVenta = idCentroSrv,
                NombreCentroServiciosVenta = nombreCentroSrv,
                IdCentroResponsable = idCentroSrv,
                NombreCentroResponsable = nombreCentroSrv,
                ValorTotal = CAConstantesCaja.VALOR_CERO_DECIMAL,
                TotalImpuestos = CAConstantesCaja.VALOR_CERO_DECIMAL,
                TotalRetenciones = CAConstantesCaja.VALOR_CERO_DECIMAL,
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                Usuario = ConstantesFramework.USUARIO_SISTEMA,
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = 0,
                    IdCodigoUsuario = ConstantesFramework.ID_USUARIO_SISTEMA,
                    CreadoPor = ConstantesFramework.USUARIO_SISTEMA,
                },
                RegistrosTransacDetallesCaja = new List<CARegistroTransacCajaDetalleDC>()
          {
            new CARegistroTransacCajaDetalleDC()
            {
              ConceptoCaja= new CAConceptoCajaDC()
              {
                IdConceptoCaja=(int)CAEnumConceptosCaja.TRANS_DINERO_ENTRE_CAJAS,
                EsIngreso=true
              },
              Cantidad=1,
              EstadoFacturacion= CAEnumEstadoFacturacion.FAC,
              FechaFacturacion= DateTime.Now,
              ValorTercero=CAConstantesCaja.VALOR_CERO_DECIMAL,
              ValorImpuestos=CAConstantesCaja.VALOR_CERO_DECIMAL,
              ValorPrimaSeguros=CAConstantesCaja.VALOR_CERO_DECIMAL,
              ValorRetenciones=CAConstantesCaja.VALOR_CERO_DECIMAL,
              Numero=Convert.ToInt64(CAConstantesCaja.VALOR_CERO_DECIMAL),
              NumeroComprobante=string.Empty,
              NumeroFactura=CAConstantesCaja.VALOR_CERO,
              ValorDeclarado=CAConstantesCaja.VALOR_CERO_DECIMAL,
              ValoresAdicionales=CAConstantesCaja.VALOR_CERO_DECIMAL,
              ValorServicio=CAConstantesCaja.VALOR_CERO_DECIMAL,
              Observacion=string.Empty,
              Descripcion=CAConstantesCaja.TRANS_DINERO_CAJAPPAL_CAJAAUX,
              ConceptoEsIngreso=true
            }
          },
                RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>()
          {
            new CARegistroVentaFormaPagoDC()
            {
              IdFormaPago= TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
              Descripcion= TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
              Valor=CAConstantesCaja.VALOR_CERO_DECIMAL,
              NumeroAsociado=string.Empty,
              Campo01=string.Empty,
              Campo02=string.Empty,
            }
          }
            };

            return OperacionPunto;
        }
    }
}