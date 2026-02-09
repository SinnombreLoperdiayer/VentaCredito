using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Cajas.Apertura;
using CO.Servidor.Cajas.Comun;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Cajas.CajaVenta
{
  /// <summary>
  /// Clase que contiene las ventas, validaciones
  /// y consultas de un Pin Prepago
  /// </summary>
  internal class CAPinPrepago : ControllerBase
  {
    #region Atributos

    private static readonly CAPinPrepago instancia = (CAPinPrepago)FabricaInterceptores.GetProxy(new CAPinPrepago(), COConstantesModulos.CAJA);

    #endregion Atributos

    #region Instancia

    public static CAPinPrepago Instancia
    {
      get { return CAPinPrepago.instancia; }
    }

    #endregion Instancia

    /// <summary>
    /// Obtiene el detalle del pin prepago.
    /// </summary>
    /// <param name="pinPrepago">The pin prepago.</param>
    /// <returns>Lista de las compras realizadas con un pin prepago</returns>
    public List<CAPinPrepagoDtllCompraDC> ObtenerDtllCompraPinPrepago(long pinPrepago)
    {
      return CARepositorioCaja.Instancia.ObtenerDtllCompraPinPrepago(pinPrepago);
    }

    /// <summary>
    /// Adiciona Pin Prepago a la tbla Pin Prepago.
    /// </summary>
    /// <param name="ventaPinPrepago">The venta pin prepago.</param>
    public void AdicionarPinPrepago(CAPinPrepagoDC pinPrepago)
    {
      CARepositorioCaja.Instancia.AdicionarPinPrepago(pinPrepago);
    }

    /// <summary>
    /// Venta del Pin prepago.
    /// </summary>
    /// <param name="ventaPinPrepago">The venta pin prepago.</param>
    public void VenderPinPrepago(CAVenderPinPrepagoDC ventaPinPrepago)
    {
      using (TransactionScope trans = new TransactionScope())
      {
        ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

        SUPropietarioGuia propietarioPin = fachadaSuministros.ObtenerPropietarioSuministro(ventaPinPrepago.AddPinPrepago.Pin, SUEnumSuministro.PIN_PREPAGO, ventaPinPrepago.AddPinPrepago.IdCentroServicioVende);
        if (propietarioPin != null && (propietarioPin.Propietario == SUEnumGrupoSuministroDC.AGE || propietarioPin.Propietario == SUEnumGrupoSuministroDC.PTO || propietarioPin.Propietario == SUEnumGrupoSuministroDC.RAC) && propietarioPin.Id == ventaPinPrepago.RegistroEnCajaPinPrepago.IdCentroServiciosVenta)
        {
          SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), propietarioPin.CentroServicios.Tipo);

          CAApertura.Instancia.RegistrarVenta(ventaPinPrepago.RegistroEnCajaPinPrepago);
          AdicionarPinPrepago(ventaPinPrepago.AddPinPrepago);

          SUConsumoSuministroDC consumo = new SUConsumoSuministroDC()
          {
            Cantidad = 1,
            EstadoConsumo = SUEnumEstadoConsumo.CON,
            GrupoSuministro = grupo,
            IdDuenoSuministro = ventaPinPrepago.RegistroEnCajaPinPrepago.IdCentroServiciosVenta,
            IdServicioAsociado = 0,
            NumeroSuministro = ventaPinPrepago.AddPinPrepago.Pin,
            Suministro = SUEnumSuministro.PIN_PREPAGO
          };
          fachadaSuministros.GuardarConsumoSuministro(consumo);

          trans.Complete();
        }
        else
        {
          //El pin prepago no pertenece al punto de servicio que lo quiere utilizar
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_PINPREPAGO_ERRADO.ToString(), CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_PINPREPAGO_ERRADO)));
        }
      }
    }

    /// <summary>
    /// Obtiene los Prepagos vendidos por un Centro de Servicio.
    /// </summary>
    /// <param name="idCentroServicio">The id centro servicio.</param>
    /// <returns>Lista de Prepagos vendidos</returns>
    public List<CAPinPrepagoDC> ObtenerPrepagosCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                              int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                              long idCentroServicio)
    {
      return CARepositorioCaja.Instancia.ObtenerPrepagosCentroServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                                    ordenamientoAscendente, out totalRegistros, idCentroServicio);
    }

    /// <summary>
    /// Validar el saldo del Prepago para descontar del
    /// saldo.
    /// </summary>
    /// <param name="idPinPrepago">el id del pin prepago.</param>
    /// <param name="valorCompraPinPrepago">es el valor de la compra con el pin prepago.</param>
    /// <returns></returns>
    public void ValidarSaldoPrepago(long pinPrepago, decimal valorCompraPinPrepago)
    {
      CARepositorioCaja.Instancia.ValidarSaldoPrepago(pinPrepago, valorCompraPinPrepago);
    }

    /// <summary>
    /// Actualizar el Saldo del Pin Prepago.
    /// </summary>
    /// <param name="idPinPrepago">Es el id del pin prepago.</param>
    /// <param name="valorCompraPinPrepago">Es el valor de la compra del pin prepago.</param>
    public void ActualizarSaldoPinPrepago(long idPinPrepago, decimal valorCompraPinPrepago)
    {
      CARepositorioCaja.Instancia.ActualizarSaldoPinPrepago(idPinPrepago, valorCompraPinPrepago);
    }
  }
}