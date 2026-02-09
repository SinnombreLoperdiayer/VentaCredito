using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CO.Servidor.Dominio.Comun.Cajas
{
  /// <summary>
  /// Interfaz para fachada de cajas
  /// </summary>
    public interface ICAFachadaCajas
    {
        /// <summary>
        /// Obtiene la dupla del concepto.
        /// </summary>
        /// <param name="idConcepto">The id concepto.</param>
        /// <returns>dupla del concepto enviado</returns>
        CAConceptoCajaDC ObtenerDuplaConcepto(int idConcepto);

        /// <summary>
        /// Obtiene el concepto de Caja por id.
        /// </summary>
        /// <param name="idConcepto">The id concepto.</param>
        /// <returns></returns>
        CAConceptoCajaDC ObtenerConceptoPorId(int idConcepto);

        /// <summary>
        /// Obtiene la transaccion completa dependiendo del id del concepto de la caja y del numero de la guia
        /// </summary>
        CARegistroTransacCajaDC ObtenerTransaccionCajaAnulacionGuia(long numeroGuia, int idConceptoCaja);

        /// <summary>
        /// Obtiene la transaccion completa dependiendo del id
        /// del concepto de la caja y del numero del Giro
        /// </summary>
        CARegistroTransacCajaDC ObtenerTransaccionCajaAnulacionGiro(long numeroGiro, int idConceptoCaja);

        /// <summary>
        /// Adicionar el movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja);

         /// <summary>
        /// Adicionar el movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja, SqlConnection conexion, SqlTransaction transaccion);

        /// <summary>
        /// Adicionar el movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        CAIdTransaccionesCajaDC AdicionarMovimientoCajaAnulacionGuia(CARegistroTransacCajaDC movimientoCaja, bool FormaPagoAlCobro);

        /// <summary>
        /// Adicionar el movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja, PAEnumConsecutivos? tipoComprobante);

        /// <summary>
        /// Registro de transacciones entre Centro de Servicio y Centro de servicio
        /// </summary>
        /// <param name="infoTransaccion">The info transaccion.</param>
        /// <param name="idRegistroTransCentroOrigen">The id registro trans centro origen.</param>
        /// <param name="idRegistroTransCentroDestino">The id registro trans centro destino.</param>
        void RegistroTransaccionCentroSvcCentroSvc(CAOperaRacolBancoEmpresaDC infoTransaccion, long idRegistroTransCentroOrigen, out long idRegistroTransCentroDestino);

        /// <summary>
        /// Adiciona las Transacciones de un Mensajero.
        /// </summary>
        /// <param name="registroMensajero">Clase Cuenta Mensajero.</param>
        void AdicionarTransaccMensajero(CACuentaMensajeroDC registroMensajero);

        /// <summary>
        /// Inserta in registro del cliente credito y cambia el estado de la facturacion y el tipo de dato
        /// Este metodo se utiliza para el cambio de forma de pago de AlCobro -Credito
        /// </summary>
        void AdicionarRegistroTransClienteCredito(ADNovedadGuiaDC novedadGuia);

        /// <summary>
        /// Consultar el id del cliente propietario de una operación
        /// </summary>
        /// <param name="idOperacion">Número de la operación de caja que se quiere consultar</param>
        /// <returns>Id del cliente dueño de la operación, si no pertenece a un cliente retorna null</returns>
        List<CAOperacionDeClienteDC> ConsultarClientePropDeOperacion(long idOperacionDesde, long idOperacionHasta);

        /// <summary>
        /// Obtiene el saldo en caja de un centro de servicios
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        decimal ObtenerSaldoCajaCentroServicios(long idCentroServicios);

        /// <summary>
        /// Obtiene el saldo acumulado en caja
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        decimal ObtenerSaldoActualCaja(long idCentroServicios);

        /// <summary>
        /// Modifica una transacción de caja dado el número de guía, el concepto de caja y el centro de servicios al que pertenece agregando o modificando el número de comprobante de acuerdo al tipo de consecutivo
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="idConceptoCaja"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="tipoConsecutivo"></param>
        /// <example>Agregar número de comprobante de ingreso a una transacción al cobro (que fué ingresada previamente pero no tenía número de comprobante de ingreso asociado) </example>
        long AgregarNumeroComprobanteAMovimientoCaja(long numeroGuia, int idConceptoCaja, long idCentroServicios, PAEnumConsecutivos tipoConsecutivo);

        /// <summary>
        /// Ejecuta la busqueda del detalle de la
        /// transaccion
        /// </summary>
        /// <param name="numeroGiro">numero del giro</param>
        /// <param name="idConceptoCaja">concepto de caja de giro</param>
        void AnularGiroCaja(long numeroGiro, int idConceptoCaja);

        /// <summary>
        /// Proceso de Devolucion en Caja cobrando
        /// el servicio del giro
        /// </summary>
        /// <param name="idGiro"></param>
        /// <param name="idConcepto"></param>
        void DevolucionSinPorteGiro(GIAdmisionGirosDC infoGiro, int idConcepto, long idCentroServicios);

        /// <summary>
        /// Proceso de Devolucion total en Caja
        /// por el giro
        /// </summary>
        /// <param name="idGiro">id del giro</param>
        /// <param name="idConcepto">id concepto</param>
        void DevolucionConPorteGiro(GIAdmisionGirosDC infoGiro, int idConcepto, long idCentroServicios);

        /// <summary>
        /// Metodo para realizar el ajuste del valor del giro en Caja
        /// </summary>
        /// <param name="solicitudAtendida">info de la Solicitud</param>
        void AjustarValorGiroCaja(GISolicitudGiroDC solicitudAtendida);

        /// <summary>
        /// Obtener el registro de transacción para una númeor de operación
        /// </summary>
        /// <param name="numeroOperacion">Número de operación: Número de guía, Número de giro, etc</param>
        /// <returns>Información del registro detallado de la transacción</returns>
        CARegistroTransacCajaDetalleDC ObtenerRegistroVentaPorNumeroOperacion(long numeroOperacion);

        /// <summary>
        /// Actualizar el Saldo del Pin Prepago.
        /// </summary>
        /// <param name="idPinPrepago">Es el id del pin prepago.</param>
        /// <param name="valorCompraPinPrepago">Es el valor de la compra del pin prepago.</param>
        void ActualizarSaldoPinPrepago(long idPinPrepago, decimal valorCompraPinPrepago);

        /// <summary>
        /// Consulta el centro de servicios que recibió el pago de un al cobro
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        PUCentroServiciosDC ConsultarCentroDeServiciosPagoAlCobro(long numeroGuia, out decimal valorCargado);

        /// <summary>
        /// Consulta el Centro de servicio cuya caja se afectó por concepto de pago de al cobro
        /// </summary>
        /// <param name="numeroAlCobro"></param>
        /// <returns></returns>
        PUCentroServiciosDC ConsultarCajaAfectadaPorPagoDeAlCobro(long numeroAlCobro, out decimal valorTotal);

        /// <summary>
        /// metodo que inserta un movimiento para que se descuente un movimiento al cobro por guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="observacion"></param>
        /// <param name="descripcion"></param>
        /// <param name="idPersonaInterna"></param>
        /// <param name="idCentroServicios"></param>
        bool InsertarDescuentoAlCobroDevuelto(long numeroGuia, long idCentroServicios, long idAdmisionMensajeria);

        /// <summary>
        /// Inserta los medios de para un registro de transaccion de caja
        /// </summary>
        /// <param name="idCaja"></param>
        /// <param name="idTransaccionCaja"></param>
        /// <param name="mediosPago"></param>
        //void AdicionarRegistroMediosPagoCaja(List<ADRegistroMediosPagoDC> mediosPago);

        /// <summary>
        /// Inserta el detalle de caja de la transaccion caja del parametro
        /// </summary>
        /// <param name="registroDetalle"></param>
        /// <param name="conexion"></param>
        /// <param name="transaccion"></param>
        /// <param name="IdCentroServiciosVenta"></param>
        /// <param name="usuario"></param>
        //void AdicionarDetalleMovimientoCaja(CARegistroTransacCajaDetalleDC registroDetalle, SqlConnection conexion, SqlTransaction transaccion, long IdCentroServiciosVenta, string usuario);
        
    }
}