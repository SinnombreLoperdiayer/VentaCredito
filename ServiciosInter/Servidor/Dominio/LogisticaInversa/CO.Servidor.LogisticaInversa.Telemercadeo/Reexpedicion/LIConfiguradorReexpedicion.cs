using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.LogisticaInversa.Telemercadeo.Reexpedicion
{
  internal class LIConfiguradorReexpedicion : ControllerBase
  {
    private static readonly LIConfiguradorReexpedicion instancia = (LIConfiguradorReexpedicion)FabricaInterceptores.GetProxy(new LIConfiguradorReexpedicion(), COConstantesModulos.TELEMERCADEO);

    public static LIConfiguradorReexpedicion Instancia
    {
      get { return LIConfiguradorReexpedicion.instancia; }
    }

    /// <summary>
    /// Consulta la informacion de la guia del envio inicial
    /// </summary>
    /// <param name="numeroGuia"></param>
    public ADGuia ObtenerInformacionGuia(ADGuia guiaInicialEnvio)
    {
      IADFachadaAdmisionesMensajeria fachadaAdmision = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
      return fachadaAdmision.ObtenerGuiaXNumeroGuia(guiaInicialEnvio.NumeroGuia);
    }

    /// <summary>
    /// Realiza las validaciones de la guia para hacer la reexpedicion
    /// </summary>
    public LIReexpedicionEnvioDC ValidaGuiaParaReexpedicion(LIReexpedicionEnvioDC reexpedicion)
    {
      ///Consulta la guia ingresada para realizar la reexpedicion
      reexpedicion.GuiaEnvioInicial = ObtenerInformacionGuia(reexpedicion.GuiaEnvioInicial);

      ///Valida si la guia es alcobro y esta pagaga
      if (reexpedicion.GuiaEnvioInicial.EsAlCobro && !reexpedicion.GuiaEnvioInicial.EstaPagada)
      {
        ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO,
             LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ALCOBRO_NOPAGO.ToString(),
              LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ALCOBRO_NOPAGO));
        throw new FaultException<ControllerException>(excepcion);
      }

      ///Obtiene el ultimo estado de la guia
      reexpedicion.EstadoGuiaEnvio = EstadosGuia.ObtenerTrazaUltimoEstadoGuia(reexpedicion.GuiaEnvioInicial.IdAdmision);

      if (reexpedicion.EstadoGuiaEnvio.IdEstadoGuia != (int)ADEnumEstadoGuia.CentroAcopio)
      {
        ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO,
             LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DIF_CENTROACOPIO.ToString(),
              string.Format(LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DIF_CENTROACOPIO), reexpedicion.GuiaEnvioInicial.NumeroGuia));
        throw new FaultException<ControllerException>(excepcion);
      }

      reexpedicion.GuiaReexpedicion = reexpedicion.GuiaEnvioInicial;
      return reexpedicion;
    }

    /// <summary>
    /// Registra la reexpedicion del envio
    /// </summary>
    /// <param name="reexpedicion"></param>
    public ADGuia GuardaReexpedicionEnvio(LIReexpedicionEnvioDC reexpedicion)
    {
           

      IADFachadaAdmisionesMensajeria fachadaAdmision = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
      ADRapiRadicado rapiRadicado = new ADRapiRadicado();
      ADRapiEnvioContraPagoDC rapiEnvioContraPago = null;
      ADGuia guiaReexpedicion = new ADGuia();
      ADNotificacion notificaciones = new ADNotificacion();

      reexpedicion.GuiaReexpedicion.FormasPago = new List<ADGuiaFormaPago>();

      ///Obtiene el centro de servicio donde esta el envio al momento de realizar la reexpedicion
      PUCentroServiciosDC nuevoCentroServicioOrigen = ObtenerCentroServicioLocalidad(reexpedicion.EstadoGuiaEnvio.IdCiudad);

      TAServicioDC servicioInfo = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>().ObtenerDatosServicio(reexpedicion.GuiaEnvioInicial.IdServicio);
      reexpedicion.GuiaReexpedicion.IdConceptoCaja = servicioInfo.IdConceptoCaja;

      reexpedicion.GuiaReexpedicion.IdCentroServicioOrigen = nuevoCentroServicioOrigen.IdCentroServicio;
      reexpedicion.GuiaReexpedicion.NombreCentroServicioOrigen = nuevoCentroServicioOrigen.Nombre;
      reexpedicion.GuiaReexpedicion.IdCiudadOrigen = nuevoCentroServicioOrigen.IdMunicipio;
      reexpedicion.GuiaReexpedicion.NombreCiudadOrigen = nuevoCentroServicioOrigen.NombreMunicipio;

      ///Obtiene el centro de servicios destino del envio
      PUCentroServiciosDC nuevoCentroServicioDestino = ObtenerCentroServicioLocalidad(reexpedicion.CiudadDestino.IdLocalidad);

      TAServicioDC servicio = new TAServicioDC()
      {
        IdServicio = reexpedicion.GuiaEnvioInicial.IdServicio
      };

      PALocalidadDC localidadOrigen = new PALocalidadDC()
      {
        IdLocalidad = reexpedicion.EstadoGuiaEnvio.IdCiudad
      };

      ///Valida el trayecto y obtiene la duracion en horas del envio
      ADValidacionServicioTrayectoDestino val = fachadaAdmision.ValidarServicioTrayectoDestino(localidadOrigen, reexpedicion.CiudadDestino, servicio, nuevoCentroServicioOrigen.IdCentroServicio, reexpedicion.GuiaEnvioInicial.Peso);
      reexpedicion.GuiaReexpedicion.FechaEstimadaEntrega = DateTime.Now.AddHours(val.DuracionTrayectoEnHoras);
      reexpedicion.GuiaReexpedicion.DiasDeEntrega = (short)val.DuracionTrayectoEnHoras;
      reexpedicion.GuiaReexpedicion.EsAutomatico = true;

      if (nuevoCentroServicioDestino == null)
        throw new NotImplementedException();

      reexpedicion.GuiaReexpedicion.IdCentroServicioDestino = nuevoCentroServicioOrigen.IdCentroServicio;
      reexpedicion.GuiaReexpedicion.NombreCentroServicioDestino = nuevoCentroServicioOrigen.Nombre;
      reexpedicion.GuiaReexpedicion.IdCiudadDestino = reexpedicion.CiudadDestino.IdLocalidad;
      reexpedicion.GuiaReexpedicion.NombreCiudadDestino = reexpedicion.CiudadDestino.Nombre;

      if (reexpedicion.GuiaEnvioInicial.IdServicio == TAConstantesServicios.SERVICIO_RAPIRADICADO)
        rapiRadicado = fachadaAdmision.ObtenerRapiradicadosGuia(reexpedicion.GuiaEnvioInicial.NumeroGuia).FirstOrDefault();
      else if (reexpedicion.GuiaEnvioInicial.IdServicio == TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO)
        rapiEnvioContraPago = fachadaAdmision.ObtenerRapiEnvioContraPago(reexpedicion.GuiaEnvioInicial.IdAdmision);
      else if (reexpedicion.GuiaEnvioInicial.IdServicio == TAConstantesServicios.SERVICIO_NOTIFICACIONES)
        notificaciones = fachadaAdmision.ObtenerNotificacionGuia(reexpedicion.GuiaEnvioInicial.IdAdmision);


      reexpedicion.GuiaReexpedicion = ObtenerPrecioServicio(reexpedicion, rapiEnvioContraPago);

      ///Consulta la informacion del remitente y destinatario
      switch (reexpedicion.GuiaEnvioInicial.TipoCliente)
      {
        case ADEnumTipoCliente.CCO:
          reexpedicion.TipoClienteEnvioInicial = fachadaAdmision.ObtenerAdmisionMensajeriaConvenioConvenio(reexpedicion.GuiaEnvioInicial.IdAdmision);
          reexpedicion.GuiaReexpedicion.Remitente.Nombre = reexpedicion.TipoClienteEnvioInicial.ConvenioRemitente.RazonSocial;
          break;

        case ADEnumTipoCliente.PPE:
          reexpedicion.TipoClienteEnvioInicial = fachadaAdmision.ObtenerAdmisionMensajeriaPeatonPeaton(reexpedicion.GuiaEnvioInicial.IdAdmision);
          reexpedicion.GuiaReexpedicion.Remitente.Nombre = reexpedicion.TipoClienteEnvioInicial.PeatonRemitente.Nombre;
          reexpedicion.GuiaReexpedicion.Remitente.Apellido1 = reexpedicion.TipoClienteEnvioInicial.PeatonRemitente.Apellido1;
          reexpedicion.GuiaReexpedicion.Remitente.Apellido2 = reexpedicion.TipoClienteEnvioInicial.PeatonRemitente.Apellido2;
          break;

        case ADEnumTipoCliente.CPE:
          reexpedicion.TipoClienteEnvioInicial = fachadaAdmision.ObtenerAdmisionMensajeriaConvenioPeaton(reexpedicion.GuiaEnvioInicial.IdAdmision);
          reexpedicion.GuiaReexpedicion.Remitente.Nombre = reexpedicion.TipoClienteEnvioInicial.ConvenioRemitente.RazonSocial;
          break;

        case ADEnumTipoCliente.PCO:
          reexpedicion.TipoClienteEnvioInicial = fachadaAdmision.ObtenerAdmisionMensajeriaPeatonConvenio(reexpedicion.GuiaEnvioInicial.IdAdmision);
          reexpedicion.GuiaReexpedicion.Remitente.Nombre = reexpedicion.TipoClienteEnvioInicial.PeatonRemitente.Nombre;
          reexpedicion.GuiaReexpedicion.Remitente.Apellido1 = reexpedicion.TipoClienteEnvioInicial.PeatonRemitente.Apellido1;
          reexpedicion.GuiaReexpedicion.Remitente.Apellido2 = reexpedicion.TipoClienteEnvioInicial.PeatonRemitente.Apellido2;
          break;
      }

      if (reexpedicion.GuiaEnvioInicial.TipoCliente == ADEnumTipoCliente.CCO || reexpedicion.GuiaEnvioInicial.TipoCliente == ADEnumTipoCliente.PCO)
      {
        reexpedicion.TipoClienteEnvioInicial.ConvenioRemitente.Telefono = reexpedicion.GuiaEnvioInicial.Remitente.Telefono;
        reexpedicion.TipoClienteEnvioInicial.ConvenioRemitente.Direccion = reexpedicion.GuiaEnvioInicial.Remitente.Direccion;
        reexpedicion.TipoClienteEnvioInicial.ConvenioDestinatario.RazonSocial = reexpedicion.GuiaReexpedicion.Destinatario.Nombre;
        reexpedicion.TipoClienteEnvioInicial.ConvenioDestinatario.Nit = reexpedicion.GuiaReexpedicion.Destinatario.Identificacion;
        reexpedicion.TipoClienteEnvioInicial.ConvenioDestinatario.Direccion = reexpedicion.GuiaReexpedicion.Destinatario.Direccion;
        reexpedicion.TipoClienteEnvioInicial.ConvenioDestinatario.Telefono = reexpedicion.GuiaEnvioInicial.TelefonoDestinatario;
      }
      else if (reexpedicion.GuiaEnvioInicial.TipoCliente == ADEnumTipoCliente.PPE || reexpedicion.GuiaEnvioInicial.TipoCliente == ADEnumTipoCliente.CPE)
      {
        reexpedicion.TipoClienteEnvioInicial.PeatonDestinatario.Nombre = reexpedicion.GuiaReexpedicion.Destinatario.Nombre;
        reexpedicion.TipoClienteEnvioInicial.PeatonDestinatario.Apellido1 = reexpedicion.GuiaReexpedicion.Destinatario.Apellido1;
        reexpedicion.TipoClienteEnvioInicial.PeatonDestinatario.Apellido2 = reexpedicion.GuiaReexpedicion.Destinatario.Apellido2 == null ? string.Empty : reexpedicion.GuiaReexpedicion.Destinatario.Apellido2;
        reexpedicion.TipoClienteEnvioInicial.PeatonDestinatario.Identificacion = reexpedicion.GuiaReexpedicion.Destinatario.Identificacion;
        reexpedicion.TipoClienteEnvioInicial.PeatonDestinatario.TipoIdentificacion = reexpedicion.GuiaReexpedicion.Destinatario.TipoId;
        reexpedicion.TipoClienteEnvioInicial.PeatonDestinatario.Direccion = reexpedicion.GuiaReexpedicion.Destinatario.Direccion;
        reexpedicion.TipoClienteEnvioInicial.PeatonDestinatario.Telefono = reexpedicion.GuiaReexpedicion.Destinatario.Telefono;
      }
      reexpedicion.GuiaReexpedicion.GuidDeChequeo = System.Guid.NewGuid().ToString();

      reexpedicion.GuiaReexpedicion.TelefonoDestinatario = reexpedicion.GuiaReexpedicion.Destinatario.Telefono;
      reexpedicion.GuiaReexpedicion.DireccionDestinatario = reexpedicion.GuiaReexpedicion.Destinatario.Direccion;
      long numeroGuiaNueva;

      using (TransactionScope scope = new TransactionScope())
      {
        if (reexpedicion.GuiaEnvioInicial.IdServicio == TAConstantesServicios.SERVICIO_RAPIRADICADO)
          numeroGuiaNueva = fachadaAdmision.RegistrarGuiaAutomaticaRapiRadicado(reexpedicion.GuiaReexpedicion, 0, reexpedicion.TipoClienteEnvioInicial, rapiRadicado).NumeroGuia;
        else if (reexpedicion.GuiaEnvioInicial.IdServicio == TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO)
          numeroGuiaNueva = fachadaAdmision.RegistrarGuiaAutomaticaRapiEnvioContraPago(reexpedicion.GuiaReexpedicion, 0, reexpedicion.TipoClienteEnvioInicial, rapiEnvioContraPago).NumeroGuia;
        else if (reexpedicion.GuiaEnvioInicial.IdServicio == TAConstantesServicios.SERVICIO_NOTIFICACIONES)
          numeroGuiaNueva = fachadaAdmision.RegistrarGuiaAutomaticaNotificacion(reexpedicion.GuiaReexpedicion, 0, reexpedicion.TipoClienteEnvioInicial, notificaciones).NumeroGuia;
        else
          numeroGuiaNueva = fachadaAdmision.RegistrarGuiaAutomatica(reexpedicion.GuiaReexpedicion, 0, reexpedicion.TipoClienteEnvioInicial).NumeroGuia;
        guiaReexpedicion = fachadaAdmision.ObtenerGuiaXNumeroGuia(numeroGuiaNueva);
        fachadaAdmision.GuadarRelacionReexpedicionEnvio(reexpedicion.GuiaEnvioInicial.IdAdmision, guiaReexpedicion.IdAdmision);
        scope.Complete();
      }

      reexpedicion.GuiaReexpedicion.Destinatario.Apellido2 = reexpedicion.GuiaReexpedicion.Destinatario.Apellido2 == null ? string.Empty : reexpedicion.GuiaReexpedicion.Destinatario.Apellido2;
      reexpedicion.GuiaReexpedicion.Destinatario.Email = reexpedicion.GuiaReexpedicion.Destinatario.Email == null ? string.Empty : reexpedicion.GuiaReexpedicion.Destinatario.Email;
      reexpedicion.GuiaReexpedicion.Remitente.Email = reexpedicion.GuiaReexpedicion.Remitente.Email == null ? string.Empty : reexpedicion.GuiaReexpedicion.Remitente.Email;
      guiaReexpedicion.Destinatario = reexpedicion.GuiaReexpedicion.Destinatario;
      guiaReexpedicion.Remitente = reexpedicion.GuiaReexpedicion.Remitente;

      return guiaReexpedicion;
    }

    /// <summary>
    /// Obtiene el centro de servicios
    /// </summary>
    /// <param name="idLocalidad"></param>
    /// <returns></returns>
    public PUCentroServiciosDC ObtenerCentroServicioLocalidad(string idLocalidad)
    {
      return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(idLocalidad);
    }

    /// <summary>
    /// Obiene el precio del servicio
    /// </summary>
    /// <param name="reexpedicion"></param>
    /// <param name="rapiEnvio"></param>
    /// <returns></returns>
    public ADGuia ObtenerPrecioServicio(LIReexpedicionEnvioDC reexpedicion, ADRapiEnvioContraPagoDC rapiEnvio = null)
    {
      decimal valor = 0;
      IADFachadaAdmisionesMensajeria fachadaAdmision = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
      TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC();
      TAPrecioCargaDC precioCarga = new TAPrecioCargaDC();
      TAPrecioServicioDC precioServicio = new TAPrecioServicioDC();
      TAPrecioTramiteDC precioTramites = new TAPrecioTramiteDC();
      TAPrecioCentroCorrespondenciaDC precioCentroCorrespondencia = new TAPrecioCentroCorrespondenciaDC();

      ///Obtiene los valores adicionales del envio
      reexpedicion.GuiaReexpedicion.ValoresAdicionales = fachadaAdmision.ObtenerValoresAdicionales(reexpedicion.GuiaEnvioInicial.IdAdmision);

      ///Obtiene el id de la lista de precio vigente
      int idListaPrecio = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>().ObtenerIdListaPrecioVigente();

      ///Calcula el precio segun el tipo de servicio
      switch (reexpedicion.GuiaEnvioInicial.IdServicio)
      {
        case TAConstantesServicios.SERVICIO_CENTRO_CORRESPONDENCIA:
          precioCentroCorrespondencia = fachadaAdmision.CalcularPrecioCentroCorrespondencia(idListaPrecio);
          break;

        case TAConstantesServicios.SERVICIO_MENSAJERIA:
          precio = fachadaAdmision.CalcularPrecioMensajeria(reexpedicion.GuiaEnvioInicial.IdServicio, idListaPrecio, reexpedicion.EstadoGuiaEnvio.IdCiudad, reexpedicion.GuiaReexpedicion.IdCiudadDestino, (decimal)Math.Ceiling((double)reexpedicion.GuiaEnvioInicial.Peso), reexpedicion.GuiaEnvioInicial.ValorDeclarado);
          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precio.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precio.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorPrimaSeguro = precio.ValorPrimaSeguro;
          reexpedicion.GuiaReexpedicion.ValorServicio = precio.Valor;
          valor = precio.Valor + reexpedicion.GuiaReexpedicion.ValorAdicionales + precio.ValorPrimaSeguro + reexpedicion.GuiaReexpedicion.ValorTotalImpuestos;
          break;

        case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
          precio = fachadaAdmision.CalcularPrecioCargaExpress(reexpedicion.GuiaEnvioInicial.IdServicio, idListaPrecio, reexpedicion.EstadoGuiaEnvio.IdCiudad, reexpedicion.GuiaReexpedicion.IdCiudadDestino, (decimal)Math.Ceiling((double)reexpedicion.GuiaEnvioInicial.Peso), reexpedicion.GuiaEnvioInicial.ValorDeclarado);

          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precio.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precio.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorPrimaSeguro = precio.ValorPrimaSeguro;
          reexpedicion.GuiaReexpedicion.ValorServicio = precio.Valor;
          valor = precio.Valor + reexpedicion.GuiaReexpedicion.ValorAdicionales + precio.ValorPrimaSeguro + reexpedicion.GuiaReexpedicion.ValorTotalImpuestos;
          break;

        case TAConstantesServicios.SERVICIO_NOTIFICACIONES:
          precio = fachadaAdmision.CalcularPrecioNotificaciones(reexpedicion.GuiaEnvioInicial.IdServicio, idListaPrecio, reexpedicion.EstadoGuiaEnvio.IdCiudad, reexpedicion.GuiaReexpedicion.IdCiudadDestino, (decimal)Math.Ceiling((double)reexpedicion.GuiaEnvioInicial.Peso), reexpedicion.GuiaEnvioInicial.ValorDeclarado);

          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precio.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precio.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorPrimaSeguro = precio.ValorPrimaSeguro;
          reexpedicion.GuiaReexpedicion.ValorServicio = precio.Valor;
          valor = precio.Valor + reexpedicion.GuiaReexpedicion.ValorAdicionales + precio.ValorPrimaSeguro + reexpedicion.GuiaReexpedicion.ValorTotalImpuestos;
          break;

        case TAConstantesServicios.SERVICIO_RAPI_AM:
          precio = fachadaAdmision.CalcularPrecioRapiAm(reexpedicion.GuiaEnvioInicial.IdServicio, idListaPrecio, reexpedicion.EstadoGuiaEnvio.IdCiudad, reexpedicion.GuiaReexpedicion.IdCiudadDestino, (decimal)Math.Ceiling((double)reexpedicion.GuiaEnvioInicial.Peso), reexpedicion.GuiaEnvioInicial.ValorDeclarado);
          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precio.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precio.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorPrimaSeguro = precio.ValorPrimaSeguro;
          reexpedicion.GuiaReexpedicion.ValorServicio = precio.Valor;
          valor = precio.Valor + reexpedicion.GuiaReexpedicion.ValorAdicionales + precio.ValorPrimaSeguro + reexpedicion.GuiaReexpedicion.ValorTotalImpuestos;
          break;

        case TAConstantesServicios.SERVICIO_RAPI_CARGA:
          precioCarga = fachadaAdmision.CalcularPrecioRapiCarga(reexpedicion.GuiaEnvioInicial.IdServicio, idListaPrecio, reexpedicion.EstadoGuiaEnvio.IdCiudad, reexpedicion.GuiaReexpedicion.IdCiudadDestino, (decimal)Math.Ceiling((double)reexpedicion.GuiaEnvioInicial.Peso), reexpedicion.GuiaEnvioInicial.ValorDeclarado);

          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precioCarga.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precioCarga.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorPrimaSeguro = precioCarga.ValorPrimaSeguro;
          reexpedicion.GuiaReexpedicion.ValorServicio = precioCarga.Valor;
          valor = precioCarga.Valor + reexpedicion.GuiaReexpedicion.ValorAdicionales + precioCarga.ValorPrimaSeguro + reexpedicion.GuiaReexpedicion.ValorTotalImpuestos;
          break;

        case TAConstantesServicios.SERVICIO_RAPI_CARGA_CONTRA_PAGO:
          precioCarga = fachadaAdmision.CalcularPrecioRapiCargaContraPago(reexpedicion.GuiaEnvioInicial.IdServicio, idListaPrecio, reexpedicion.EstadoGuiaEnvio.IdCiudad, reexpedicion.GuiaReexpedicion.IdCiudadDestino, (decimal)Math.Ceiling((double)reexpedicion.GuiaEnvioInicial.Peso), 0, reexpedicion.GuiaEnvioInicial.ValorDeclarado);

          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precioCarga.Valor + precioCarga.ValorContraPago + reexpedicion.GuiaReexpedicion.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precioCarga.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorPrimaSeguro = precioCarga.ValorPrimaSeguro;
          reexpedicion.GuiaReexpedicion.ValorServicio = precioCarga.Valor;

          valor = precioCarga.Valor + reexpedicion.GuiaReexpedicion.ValorAdicionales + precioCarga.ValorPrimaSeguro + reexpedicion.GuiaReexpedicion.ValorTotalImpuestos;
          break;

        case TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO:

          precio = fachadaAdmision.CalcularPrecioRapiEnvioContraPago(reexpedicion.GuiaEnvioInicial.IdServicio, idListaPrecio, reexpedicion.EstadoGuiaEnvio.IdCiudad, reexpedicion.GuiaReexpedicion.IdCiudadDestino, (decimal)Math.Ceiling((double)reexpedicion.GuiaEnvioInicial.Peso), rapiEnvio.ValorARecaudar, reexpedicion.GuiaEnvioInicial.ValorDeclarado);
          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precio.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precio.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorPrimaSeguro = precio.ValorPrimaSeguro;
          reexpedicion.GuiaReexpedicion.ValorServicio = precio.Valor;

          break;

        case TAConstantesServicios.SERVICIO_RAPI_HOY:
          precio = fachadaAdmision.CalcularPrecioRapiHoy(reexpedicion.GuiaEnvioInicial.IdServicio, idListaPrecio, reexpedicion.EstadoGuiaEnvio.IdCiudad, reexpedicion.GuiaReexpedicion.IdCiudadDestino, (decimal)Math.Ceiling((double)reexpedicion.GuiaEnvioInicial.Peso), reexpedicion.GuiaEnvioInicial.ValorDeclarado);

          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precio.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precio.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorPrimaSeguro = precio.ValorPrimaSeguro;
          reexpedicion.GuiaReexpedicion.ValorServicio = precio.Valor;

          valor = valor = precio.Valor + reexpedicion.GuiaReexpedicion.ValorAdicionales + precio.ValorPrimaSeguro + reexpedicion.GuiaReexpedicion.ValorTotalImpuestos;
          break;

        case TAConstantesServicios.SERVICIO_RAPI_MASIVOS:
          throw new NotImplementedException();
        case TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO:
          precio = fachadaAdmision.CalcularPrecioRapiPersonalizado(reexpedicion.GuiaEnvioInicial.IdServicio, idListaPrecio, reexpedicion.EstadoGuiaEnvio.IdCiudad, reexpedicion.GuiaReexpedicion.IdCiudadDestino, (decimal)Math.Ceiling((double)reexpedicion.GuiaEnvioInicial.Peso), reexpedicion.GuiaEnvioInicial.ValorDeclarado);

          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precio.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precio.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorPrimaSeguro = precio.ValorPrimaSeguro;
          reexpedicion.GuiaReexpedicion.ValorServicio = precio.Valor;
          valor = precio.Valor + reexpedicion.GuiaReexpedicion.ValorAdicionales + precio.ValorPrimaSeguro + reexpedicion.GuiaReexpedicion.ValorTotalImpuestos;

          break;

        case TAConstantesServicios.SERVICIO_RAPI_PROMOCIONAL:
          precioServicio = fachadaAdmision.CalcularPrecioRapiPromocional(idListaPrecio, 0);

          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precioServicio.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precioServicio.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorServicio = precioServicio.Valor;
          valor = valor = precioServicio.Valor + reexpedicion.GuiaReexpedicion.ValorAdicionales + reexpedicion.GuiaReexpedicion.ValorTotalImpuestos;

          break;

        case TAConstantesServicios.SERVICIO_RAPIRADICADO:
          precio = fachadaAdmision.CalcularPrecioRapiradicado(idListaPrecio, reexpedicion.EstadoGuiaEnvio.IdCiudad, reexpedicion.GuiaReexpedicion.IdCiudadDestino, (decimal)Math.Ceiling((double)reexpedicion.GuiaEnvioInicial.Peso), reexpedicion.GuiaEnvioInicial.ValorDeclarado);

          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precio.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precio.Impuestos.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorServicio = precio.Valor;
          reexpedicion.GuiaReexpedicion.ValorPrimaSeguro = precio.ValorPrimaSeguro;
          break;

        case TAConstantesServicios.SERVICIO_TRAMITES:
          precioTramites = fachadaAdmision.CalcularPrecioTramites(idListaPrecio, 0);

          reexpedicion.GuiaReexpedicion.ValorAdicionales = reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorAdmision = precioTramites.Valor + reexpedicion.GuiaEnvioInicial.ValorAdicionales;
          reexpedicion.GuiaReexpedicion.ValorTotalImpuestos = precioTramites.ImpuestosTramite.Sum(r => r.Valor);
          reexpedicion.GuiaReexpedicion.ValorServicio = precioTramites.Valor;
          valor = precioTramites.Valor + reexpedicion.GuiaReexpedicion.ValorAdicionales + reexpedicion.GuiaReexpedicion.ValorTotalImpuestos;

          break;

        default:
          ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO,
             LOIEnumTipoErrorLogisticaInversa.EX_ERROR_SERVICIO_NO_CONFIGURADO_REEXPEDICION.ToString(),
              LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_SERVICIO_NO_CONFIGURADO_REEXPEDICION));
          throw new FaultException<ControllerException>(excepcion);
      }

      ADGuiaFormaPago formaPago = new ADGuiaFormaPago()
            {
              IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO,
              Valor = valor,
              Descripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_AL_COBRO
            };
      reexpedicion.GuiaReexpedicion.FormasPago.Add(formaPago);
      reexpedicion.GuiaReexpedicion.ValorTotal = valor;
      return reexpedicion.GuiaReexpedicion;
    }
  }
}