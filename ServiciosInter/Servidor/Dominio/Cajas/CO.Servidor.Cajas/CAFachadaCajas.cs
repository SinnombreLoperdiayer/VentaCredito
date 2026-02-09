using CO.Servidor.Cajas.Apertura;
using CO.Servidor.Cajas.CajaFinanciera;
using CO.Servidor.Cajas.CajaVenta;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CO.Servidor.Cajas
{
    /// <summary>
    /// Fachada para acceso a la lógica de cajas
    /// </summary>
    public class CAFachadaCajas : ICAFachadaCajas
    {
        /// <summary>
        /// Obtiene el concepto de Caja por id.
        /// </summary>
        /// <param name="idConcepto">The id concepto.</param>
        /// <returns></returns>
        public CAConceptoCajaDC ObtenerConceptoPorId(int idConcepto)
        {
            return CACaja.Instancia.ObtenerConceptoPorId(idConcepto);
        }

        /// <summary>
        /// Obtiene la transaccion completa dependiendo del id del concepto de la caja y del numero de la guia
        /// </summary>
        public CARegistroTransacCajaDC ObtenerTransaccionCajaAnulacionGuia(long numeroGuia, int idConceptoCaja)
        {
            return CACaja.Instancia.ObtenerTransaccionCajaAnulacionGuia(numeroGuia, idConceptoCaja);
        }

        /// <summary>
        /// Obtiene la transaccion completa dependiendo del id
        /// del concepto de la caja y del numero del Giro
        /// </summary>
        public CARegistroTransacCajaDC ObtenerTransaccionCajaAnulacionGiro(long numeroGiro, int idConceptoCaja)
        {
            return CACaja.Instancia.ObtenerTransaccionCajaAnulacionGiro(numeroGiro, idConceptoCaja);
        }

        /// <summary>
        /// Obtiene la dupla del concepto.
        /// </summary>
        /// <param name="idConcepto">The id concepto.</param>
        /// <returns>dupla del concepto enviado</returns>
        public CAConceptoCajaDC ObtenerDuplaConcepto(int idConcepto)
        {
            return CAAdministradorCajas.Instancia.ObtenerDuplaConcepto(idConcepto);
        }

        /// <summary>
        /// Adicionar el movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja)
        {
            return CAApertura.Instancia.RegistrarVenta(movimientoCaja);
        }

        /// <summary>
        /// Adicionar el movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja,SqlConnection conexion, SqlTransaction transaccion)
        {
            return CAApertura.Instancia.RegistrarVenta(movimientoCaja,conexion,transaccion);
        }
        /// <summary>
        /// Adicionar el movimiento caja, para anular una guia
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        public CAIdTransaccionesCajaDC AdicionarMovimientoCajaAnulacionGuia(CARegistroTransacCajaDC movimientoCaja, bool FormaPagoAlCobro)
        {
            return CAApertura.Instancia.RegistrarAnulacionGuia(movimientoCaja, FormaPagoAlCobro);
        }

        /// <summary>
        /// Adicionar el movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja, PAEnumConsecutivos? idConsecutivoComprobante)
        {
            return CAApertura.Instancia.RegistrarVentaRequiereTipoComprobante(movimientoCaja, idConsecutivoComprobante);
        }

        /// <summary>
        /// Registro de transacciones entre Centro de Servicio y Centro de servicio
        /// </summary>
        /// <param name="infoTransaccion">The info transaccion.</param>
        /// <param name="idRegistroTransCentroOrigen">The id registro trans centro origen.</param>
        /// <param name="idRegistroTransCentroDestino">The id registro trans centro destino.</param>
        public void RegistroTransaccionCentroSvcCentroSvc(CAOperaRacolBancoEmpresaDC infoTransaccion, long idRegistroTransCentroOrigen, out long idRegistroTransCentroDestino)
        {
            CACajaFinanciera.Instancia.RegistroTransaccionCentroSvcCentroSvc(infoTransaccion, idRegistroTransCentroOrigen, out idRegistroTransCentroDestino);
        }

        /// <summary>
        /// Adiciona las Transacciones de un Mensajero.
        /// </summary>
        /// <param name="registroMensajero">Clase Cuenta Mensajero.</param>
        public void AdicionarTransaccMensajero(CACuentaMensajeroDC registroMensajero)
        {
            CAMensajero.Instancia.AdicionarTransaccMensajero(registroMensajero);
        }

        /// <summary>
        /// Inserta un registro del cliente credito y cambia el estado de la facturacion y el tipo de dato
        /// Este metodo se utiliza para el cambio de forma de pago de AlCobro -Credito
        /// </summary>
        public void AdicionarRegistroTransClienteCredito(ADNovedadGuiaDC novedadGuia)
        {
            CAApertura.Instancia.AdicionarRegistroTransClienteCredito(novedadGuia);
        }

        /// <summary>
        /// Consultar el id del cliente propietario de una operación
        /// </summary>
        /// <param name="idOperacion">Número de la operación de caja que se quiere consultar</param>
        /// <returns>Id del cliente dueño de la operación, si no pertenece a un cliente retorna null</returns>
        public List<CAOperacionDeClienteDC> ConsultarClientePropDeOperacion(long idOperacionDesde, long idOperacionHasta)
        {
            return CACaja.Instancia.ConsultarClientePropDeOperacion(idOperacionDesde, idOperacionHasta);
        }

        /// <summary>
        /// Obtiene el saldo en caja de un centro de servicios
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public decimal ObtenerSaldoCajaCentroServicios(long idCentroServicios)
        {
            return CACajaPrincipal.Instancia.ObtenerUltimoCierrePunto(idCentroServicios).SaldoAnteriorEfectivo;
        }

        /// <summary>
        /// Obtiene el saldo acumulado en caja
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public decimal ObtenerSaldoActualCaja(long idCentroServicios)
        {
            return CACaja.Instancia.ObtenerSaldoActualCaja(idCentroServicios);
        }

        /// <summary>
        /// Modifica una transacción de caja dado el número de guía, el concepto de caja y el centro de servicios al que pertenece agregando o modificando el número de comprobante de acuerdo al tipo de consecutivo
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="idConceptoCaja"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="tipoConsecutivo"></param>
        public long AgregarNumeroComprobanteAMovimientoCaja(long numeroGuia, int idConceptoCaja, long idCentroServicios, PAEnumConsecutivos tipoConsecutivo)
        {
            return CACaja.Instancia.AgregarNumeroComprobanteAMovimientoCaja(numeroGuia, idConceptoCaja, idCentroServicios, tipoConsecutivo);
        }

        /// <summary>
        /// Ejecuta la busqueda del detalle de la
        /// transaccion
        /// </summary>
        /// <param name="numeroGiro">numero del giro</param>
        /// <param name="idConceptoCaja">concepto de caja de giro</param>
        public void AnularGiroCaja(long numeroGiro, int idConceptoCaja)
        {
            CACaja.Instancia.AnularGiroCaja(numeroGiro, idConceptoCaja);
        }

        /// <summary>
        /// Proceso de Devolucion en Caja cobrando
        /// el servicio del giro
        /// </summary>
        /// <param name="idGiro"></param>
        /// <param name="idConcepto"></param>
        public void DevolucionSinPorteGiro(GIAdmisionGirosDC infoGiro, int idConcepto, long idCentroServicios)
        {
            CACaja.Instancia.DevolucionSinPorteGiro(infoGiro, idConcepto, idCentroServicios);
        }

        /// <summary>
        /// Proceso de Devolucion total en Caja
        /// por el giro
        /// </summary>
        /// <param name="idGiro">id del giro</param>
        /// <param name="idConcepto">id concepto</param>
        public void DevolucionConPorteGiro(GIAdmisionGirosDC infoGiro, int idConcepto, long idCentroServicios)
        {
            CACaja.Instancia.DevolucionConPorteGiro(infoGiro, idConcepto, idCentroServicios);
        }

        /// <summary>
        /// Metodo para realizar el ajuste del valor del giro en Caja
        /// </summary>
        /// <param name="solicitudAtendida">info de la Solicitud</param>
        public void AjustarValorGiroCaja(GISolicitudGiroDC solicitudAtendida)
        {
            CACaja.Instancia.AjustarValorGiroCaja(solicitudAtendida);
        }

        /// <summary>
        /// Obtener el registro de transacción para una númeor de operación
        /// </summary>
        /// <param name="numeroOperacion">Número de operación: Número de guía, Número de giro, etc</param>
        /// <returns>Información del registro detallado de la transacción</returns>
        public CARegistroTransacCajaDetalleDC ObtenerRegistroVentaPorNumeroOperacion(long numeroOperacion)
        {
            return CACaja.Instancia.ObtenerRegistroVentaPorNumeroOperacion(numeroOperacion);
        }

        /// <summary>
        /// Actualizar el Saldo del Pin Prepago.
        /// </summary>
        /// <param name="idPinPrepago">Es el id del pin prepago.</param>
        /// <param name="valorCompraPinPrepago">Es el valor de la compra del pin prepago.</param>
        public void ActualizarSaldoPinPrepago(long idPinPrepago, decimal valorCompraPinPrepago)
        {
            CAPinPrepago.Instancia.ActualizarSaldoPinPrepago(idPinPrepago, valorCompraPinPrepago);
        }

        /// <summary>
        /// Consulta el centro de servicios que recibió el pago de un al cobro
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ConsultarCentroDeServiciosPagoAlCobro(long numeroGuia, out decimal valorCargado)
        {
            return CACaja.Instancia.ConsultarCentroDeServiciosPagoAlCobro(numeroGuia, out valorCargado);
        }

        /// <summary>
        /// Consulta el Centro de servicio cuya caja se afectó por concepto de pago de al cobro
        /// </summary>
        /// <param name="numeroAlCobro"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ConsultarCajaAfectadaPorPagoDeAlCobro(long numeroAlCobro, out decimal valorTotal)
        {
            return CACaja.Instancia.ConsultarCajaAfectadaPorPagoDeAlCobro(numeroAlCobro, out valorTotal);
        }

        /// <summary>
        /// metodo que inserta un movimiento para que se descuente un movimiento al cobro por guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="observacion"></param>
        /// <param name="descripcion"></param>
        /// <param name="idPersonaInterna"></param>
        /// <param name="idCentroServicios"></param>
        public bool InsertarDescuentoAlCobroDevuelto(long numeroGuia, long idCentroServicios, long idAdmisionMensajeria)
        {
            return CACaja.Instancia.InsertarDescuentoAlCobroDevuelto(numeroGuia, idCentroServicios, idAdmisionMensajeria);
        }

    }
}