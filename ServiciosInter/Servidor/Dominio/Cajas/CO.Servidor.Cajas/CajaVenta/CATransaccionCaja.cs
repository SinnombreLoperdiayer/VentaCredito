using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CO.Servidor.Cajas.Comun;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using CO.Servidor.Cajas.CajaFinanciera;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using System;
using System.Transactions;
using CO.Servidor.Cajas.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using System.Data.SqlClient;

namespace CO.Servidor.Cajas.CajaVenta
{
  /// <summary>
  /// Clase que contiene el metodo de adicion de
  /// movimientos de caja
  /// </summary>
  internal class CATransaccionCaja
  {
    #region Atributos

    //private static readonly CATransaccionCaja instancia = (CATransaccionCaja)FabricaInterceptores.GetProxy(new CATransaccionCaja(), COConstantesModulos.CAJA);

    #endregion Atributos

    #region Instancia

    //public static CATransaccionCaja Instancia
    //{
    //  get { return CATransaccionCaja.instancia; }
    //}

    #endregion Instancia

    /// <summary>
    /// Adicionar el movimiento caja.
    /// </summary>
    /// <param name="movimientoCaja">The movimiento caja.</param>
    /// <param name="anulacionGuia">indica si se esta anulando una guia</param>
    public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja,bool anulacionGuia=false)
    {
        CAIdTransaccionesCajaDC idOperCs;
        using (TransactionScope scope = new TransactionScope())
        {
            //Inserto la transaccion de Caja del centro de servicio
            idOperCs = CARepositorioCaja.Instancia.AdicionarMovimientoCaja(movimientoCaja, anulacionGuia);            
            CAConceptoCajaDC concepto = CARepositorioCaja.Instancia.ListaConceptos.Where(c => c.IdConceptoCaja==movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.IdConceptoCaja).FirstOrDefault();

            if ((movimientoCaja.RegistrosTransacDetallesCaja.Count>0 &&                 
                movimientoCaja.RegistroVentaFormaPago.FirstOrDefault().IdFormaPago==(short)TAEnumFormaPago.EFECTIVO) &&
                concepto.ContraPartidaCasaMatriz && !concepto.ContraPartidaCS)
            {
                string nombreConcepto = concepto.Nombre;
                string descConcepto = concepto.Descripcion;

                //Afectar caja de casa matriz
                CAConceptoCajaDC conceptoCasaMatriz = new CAConceptoCajaDC()
                {
                    IdConceptoCaja = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.IdConceptoCaja,
                    Nombre = nombreConcepto,
                    Descripcion = descConcepto,
                    EsEgreso = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoEsIngreso,
                    EsIngreso = !movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoEsIngreso
                };

                CACajaCasaMatrizDC operCajaCasaMatriz = new CACajaCasaMatrizDC()
                {
                    ConceptoCaja = conceptoCasaMatriz,
                    CreadoPor = ControllerContext.Current.Usuario,
                    Descripcion = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().Descripcion,
                    IdCentroServicioRegistra = movimientoCaja.IdCentroServiciosVenta,
                    IdCodigoUsuario = ControllerContext.Current.CodigoUsuario,
                    MovHechoPor = "CES",
                    FechaGrabacion = DateTime.Now,
                    FechaMov = DateTime.Now,
                    NombreCentroServicioRegistra = movimientoCaja.NombreCentroServiciosVenta,
                    NumeroDocumento = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().Numero.ToString(),
                    Observacion = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().Observacion,
                    Valor = movimientoCaja.ValorTotal
                };
                long idOperCasaMatriz = CACajaCasaMatriz.Instancia.RegistrarOperacion(movimientoCaja.InfoAperturaCaja.IdAperturaCaja, operCajaCasaMatriz);

                CajaCasaMatrizCentroSvcMov_CAJ transCSCasaMatriz = new CajaCasaMatrizCentroSvcMov_CAJ()
                {
                    CEM_CreadoPor = ControllerContext.Current.Usuario,
                    CEM_FechaGrabacion = DateTime.Now,
                    CEM_IdCentroServiciosMov = movimientoCaja.IdCentroServiciosVenta,
                    CEM_NombreCentroServiciosMov = movimientoCaja.NombreCentroServiciosVenta,
                    CEM_IdOperacionCajaCasaMatriz = idOperCasaMatriz,
                    CEM_IdRegistroTranscaccion = idOperCs.IdTransaccionCaja,
                };
                CARepositorioGestionCajas.Instancia.AdicionarMovCentroSrvCajaCasaMatriz(transCSCasaMatriz);
            }
            scope.Complete();            
        }
        return idOperCs;
    }


    /// <summary>
    /// Adicionar el movimiento caja.
    /// </summary>
    /// <param name="movimientoCaja">The movimiento caja.</param>
    /// <param name="anulacionGuia">indica si se esta anulando una guia</param>
    public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja,SqlConnection conexion,SqlTransaction transaccion, bool anulacionGuia = false)
    {
        CAIdTransaccionesCajaDC idOperCs;
       
        
        //Inserto la transaccion de Caja del centro de servicio
        idOperCs = CARepositorioCaja.Instancia.AdicionarMovimientoCaja(movimientoCaja, conexion, transaccion, anulacionGuia);
            CAConceptoCajaDC concepto = CARepositorioCaja.Instancia.ListaConceptos.Where(c => c.IdConceptoCaja == movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.IdConceptoCaja).FirstOrDefault();

            if ((movimientoCaja.RegistrosTransacDetallesCaja.Count > 0 &&
                movimientoCaja.RegistroVentaFormaPago.FirstOrDefault().IdFormaPago == (short)TAEnumFormaPago.EFECTIVO) &&
                concepto.ContraPartidaCasaMatriz && !concepto.ContraPartidaCS)
            {
                string nombreConcepto = concepto.Nombre;
                string descConcepto = concepto.Descripcion;

                //Afectar caja de casa matriz
                CAConceptoCajaDC conceptoCasaMatriz = new CAConceptoCajaDC()
                {
                    IdConceptoCaja = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.IdConceptoCaja,
                    Nombre = nombreConcepto,
                    Descripcion = descConcepto,
                    EsEgreso = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoEsIngreso,
                    EsIngreso = !movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoEsIngreso
                };

                CACajaCasaMatrizDC operCajaCasaMatriz = new CACajaCasaMatrizDC()
                {
                    ConceptoCaja = conceptoCasaMatriz,
                    CreadoPor = ControllerContext.Current.Usuario,
                    Descripcion = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().Descripcion,
                    IdCentroServicioRegistra = movimientoCaja.IdCentroServiciosVenta,
                    IdCodigoUsuario = ControllerContext.Current.CodigoUsuario,
                    MovHechoPor = "CES",
                    FechaGrabacion = DateTime.Now,
                    FechaMov = DateTime.Now,
                    NombreCentroServicioRegistra = movimientoCaja.NombreCentroServiciosVenta,
                    NumeroDocumento = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().Numero.ToString(),
                    Observacion = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().Observacion,
                    Valor = movimientoCaja.ValorTotal
                };
                long idOperCasaMatriz = CACajaCasaMatriz.Instancia.RegistrarOperacion(movimientoCaja.InfoAperturaCaja.IdAperturaCaja, operCajaCasaMatriz,conexion,transaccion);

                CajaCasaMatrizCentroSvcMov_CAJ transCSCasaMatriz = new CajaCasaMatrizCentroSvcMov_CAJ()
                {
                    CEM_CreadoPor = ControllerContext.Current.Usuario,
                    CEM_FechaGrabacion = DateTime.Now,
                    CEM_IdCentroServiciosMov = movimientoCaja.IdCentroServiciosVenta,
                    CEM_NombreCentroServiciosMov = movimientoCaja.NombreCentroServiciosVenta,
                    CEM_IdOperacionCajaCasaMatriz = idOperCasaMatriz,
                    CEM_IdRegistroTranscaccion = idOperCs.IdTransaccionCaja,
                };
                CARepositorioGestionCajas.Instancia.AdicionarMovCentroSrvCajaCasaMatriz(transCSCasaMatriz,conexion,transaccion);
            }
           
        return idOperCs;
    }

    /// <summary>
    /// Adicionar el movimiento caja y obtiene el número de consecutivo de acuerdo al tipo de movimiento a registrar.
    /// </summary>
    /// <param name="movimientoCaja">Movimiento caja.</param>
    /// <param name="tipoConsecutivo">Tipo de consecutivo asociado al movimiento</param>
    public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja, PAEnumConsecutivos idConsecutivoComprobante)
    {
      long consecutivo = 0;
      // Obtengo consecuttivo de acuerdo al tipo
      if (idConsecutivoComprobante.Equals(PAEnumConsecutivos.Comprobante_Egreso))
        consecutivo = CACaja.Instancia.ObtenerConsecutivoComprobateCajaEgreso();
      else if (idConsecutivoComprobante.Equals(PAEnumConsecutivos.Comprobante_Ingreso))
        consecutivo = CACaja.Instancia.ObtenerConsecutivoComprobateCajaIngreso();

      movimientoCaja.RegistrosTransacDetallesCaja.First().NumeroComprobante = consecutivo.ToString();

      //Inserto la transaccion de Caja
      return AdicionarMovimientoCaja(movimientoCaja);
    }

  

    /// <summary>
    /// Inserta in registro del cliente credito y cambia el estado de la facturacion y el tipo de dato
    /// Este metodo se utiliza para el cambio de forma de pago de AlCobro -Credito
    /// </summary>
    public void AdicionarRegistroTransClienteCredito(ADNovedadGuiaDC novedadGuia)
    {
      CARepositorioCaja.Instancia.AdicionarRegistroTransClienteCredito(novedadGuia);
    }
  }
}