using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.OperacionNacional.Comun;
using CO.Servidor.OperacionNacional.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;

namespace CO.Servidor.OperacionNacional
{
  internal class ONManejadorIngresoRuta : ControllerBase
  {
    private static readonly ONManejadorIngresoRuta instancia = (ONManejadorIngresoRuta)FabricaInterceptores.GetProxy(new ONManejadorIngresoRuta(), COConstantesModulos.MODULO_OPERACION_NACIONAL);

    /// <summary>
    /// Retorna una instancia de ONManejadorIngresoRuta
    /// </summary>
    public static ONManejadorIngresoRuta Instancia
    {
      get { return ONManejadorIngresoRuta.instancia; }
    }

    /// <summary>
    /// Valida el vehiculo seleccionado para poder hacer el ingreso a col
    /// </summary>
    public bool ValidacionVehiculoIngreso(ONIngresoOperativoDC ingreso)
    {
      //Obtiene el ultimo ingreso del vehiculo a la agencia, si es ingreso permite hacer el proceso
      //de lo contrario muestra una excepción
      if (!ONRepositorio.Instancia.ObtenerTipoIngresoVehiculo(ingreso.Vehiculo.IdVehiculo, ingreso.IdAgencia))
        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_FALTA_INGRESO_VEHICULO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_FALTA_INGRESO_VEHICULO)));

      ///Valida que la ciudad del ingreso pertenezca a la ruta o la estacion de la ruta
      if (!ONRepositorio.Instancia.ValidarCiudadEnRutaEstacionesRuta(ingreso.CiudadDescargue.IdLocalidad, ingreso.Vehiculo.IdVehiculo, ingreso.IdAgencia))
        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_CIUDAD_INGRESO_NO_RUTA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_CIUDAD_INGRESO_NO_RUTA)));

      return true;
    }

    /// <summary>
    /// Obtiene los envios consolidados de los manifiestos asociados al vehiculo
    /// </summary>
    /// <param name="idVehiculo"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    public List<ONManifiestoOperacionNacional> ObtenerEnviosConsolidadosManifiestoVehiculo(int idVehiculo, int idRuta, int indicePagina, int registrosPorPagina)
    {
      return ONRepositorio.Instancia.ObtenerEnviosConsolidadosManifiestoVehiculo(idVehiculo, idRuta, indicePagina, registrosPorPagina);
    }

    /// <summary>
    /// Obtiene los consolidados de los manifiestos abiertos asociados al vehiculo
    /// </summary>
    /// <param name="idVehiculo">id del vehiculo</param>
    /// <returns></returns>
    public List<ONConsolidado> ObtenerConsolidadosManifiestoVehiculo(int idVehiculo, int idRuta, int indicePagina, int registrosPorPagina)
    {
      bool estaDescargado = false;
      return ONRepositorio.Instancia.ObtenerConsolidadosManifiestoVehiculo(idVehiculo, idRuta, estaDescargado, indicePagina, registrosPorPagina);
    }

    /// <summary>
    /// Obtiene los estados del empaque para el parametro de peso especificado
    /// </summary>
    /// <param name="idParametro"></param>
    /// <returns></returns>
    public List<PAEstadoEmpaqueDC> ObtenerEstadosEmpaqueParametroPeso(string idParametro)
    {
      string valorPesoMaximo;
      decimal valorPeso;
      valorPesoMaximo = ONRepositorio.Instancia.ObtenerParametroOperacionNacional(idParametro);

      if (decimal.TryParse(valorPesoMaximo, out valorPeso))
        return PAAdministrador.Instancia.ObtenerEstadosEmpaque(valorPeso);
      else
        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_ESTADOS_EMPAQUE.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_ESTADOS_EMPAQUE)));
    }

    /// <summary>
    /// Guarda el ingreso a la agencia del vehiculo del operativo
    /// </summary>
    /// <param name="ingresoOperativo"></param>
    /// <returns></returns>
    public long GuardarIngresoAgenciaRuta(ONIngresoOperativoDC ingresoOperativo)
    {
      long idOperativo = 0;
      if (ingresoOperativo.IdIngresoOperativo == 0)
      {
        using (TransactionScope scope = new TransactionScope())
        {
          ///Consulta el ultimo ingreso del vehiculo a la agencia, si el ingreso esta abierto
          ///retorna el id del ingreso, sino realiza el ingreso
          idOperativo = ONRepositorio.Instancia.ObtenerUltimoIngresoOperativoAgenciaRuta(ingresoOperativo.Vehiculo.IdVehiculo, ingresoOperativo.IdAgencia, ingresoOperativo.Ruta.IdRuta);

          if (idOperativo == 0)
            idOperativo = ONRepositorio.Instancia.GuardarIngresoOperativoAgencia(ingresoOperativo, OPConstantesOperacionNacional.ID_TIPO_OPERATIVO_RUTA);

          scope.Complete();
        }
      }

      return idOperativo;
    }

    /// <summary>
    /// Guarda la novedad del consolidado
    /// </summary>
    /// <param name="idConsolidado">id del consolidado </param>
    /// <param name="descripcion">Descripcion de la novedad</param>
    /// <param name="numeroPrecintoIngreso">numero del precinto de ingreso</param>
    /// <param name="numeroTulaContenedor">número de tula o contendor de ingreso</param>
    public void GuardarNovedadConsolidado(ONConsolidado consolidado, string descripcion)
    {
      using (TransactionScope scope = new TransactionScope())
      {
        ONRepositorio.Instancia.GuardarNovedadConsolidado(consolidado.IdManfiestoConsolidado, descripcion, consolidado.NumeroPrecintoIngreso, consolidado.NumeroContenedorTulaLlegada);
        ONManejadorFallas.DespacharFallaPorNovedadConsolidado(consolidado, descripcion, ControllerContext.Current.Usuario);

        scope.Complete();
      }
    }

    /// <summary>
    /// Consulta todos los envios de un consolidado
    /// </summary>
    /// <param name="idVehiculo"></param>
    /// <param name="idIngresoOperativo"></param>
    /// <returns></returns>
    public List<ONConsolidadoDetalle> ObtenerTodosEnviosSueltosVehiculo(int idVehiculo, int idRuta)
    {
      return ONRepositorio.Instancia.ObtenerTodosEnviosSueltosVehiculo(idVehiculo, idRuta);
    }

    /// <summary>
    /// Consulta todos los envios consolidados del manifiesto
    /// </summary>
    /// <param name="idVehiculo"></param>
    /// <returns></returns>
    public List<ONConsolidadoDetalle> ObtenerTodosEnviosConsolidadoManifiesto(int idVehiculo, int idRuta)
    {
      return ONRepositorio.Instancia.ObtenerTodosEnviosConsolidadoManifiesto(idVehiculo, idRuta);
    }

    /// <summary>
    /// Guarda el envio ingresado
    /// </summary>
    /// <param name="ingreso"></param>
    public ONIngresoOperativoDC GuardaIngresoEnvioAgencia(ONIngresoOperativoDC ingreso)
    {
      bool estaDescargada;
      ///Si el envio no tiene id de mensajeria consulta el id  
      if (ingreso.EnvioIngreso.IdAdmisionMensajeria == 0)
      {
        ///Valida que la guia ingresada este capturada en sistema
        ADGuia guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerInfoGuiaXNumeroGuia(ingreso.EnvioIngreso.NumeroGuia.Value);
        if (guia.IdAdmision == 0)
          ingreso.EnvioIngreso.IdAdmisionMensajeria = guia.IdAdmision;
        else
        {
          ingreso.EnvioIngreso.IdAdmisionMensajeria = guia.IdAdmision;
          ingreso.EnvioIngreso.IdLocalidadDestino = guia.IdCiudadDestino;
          ingreso.EnvioIngreso.PesoGuiaSistema = guia.Peso;
          ingreso.EnvioIngreso.NombreCiudadDestino = guia.NombreCiudadDestino;
          ingreso.EnvioIngreso.IdCiudadOrigen = guia.IdCiudadOrigen;
          ingreso.EnvioIngreso.NombreCiudadOrigen = guia.NombreCiudadOrigen;
          ingreso.EnvioIngreso.IdCentroServicioDestino = guia.IdCentroServicioDestino;
          ingreso.EnvioIngreso.NombreCentroServicioDestino = guia.NombreCentroServicioDestino;
          ingreso.EnvioIngreso.IdCentroServicioOrigen = guia.IdCentroServicioOrigen;
          ingreso.EnvioIngreso.NombreCentroServicioOrigen = guia.NombreCentroServicioOrigen;
        }
      }

      ///Si el estado del empaque del envio no tiene bolsa de seguridad envia una falla
      if (ingreso.EnvioIngreso.EstadoEmpaque.IdEstadoEmpaque == OPConstantesOperacionNacional.ID_ESTADO_EMPAQUE_SIN_BOLSA_SEGURIDAD
        || ingreso.EnvioIngreso.EstadoEmpaque.IdEstadoEmpaque == OPConstantesOperacionNacional.ID_ESTADO_EMPAQUE_MAL_EMBALADO)
      {
        if (ingreso.EnvioIngreso.IdAdmisionMensajeria == 0)
        {
          ONManejadorFallas.DespacharFallaPorBolsaDeSeguridad(ingreso.EnvioIngreso, ControllerContext.Current.Usuario);
        }
        else
        {
          ONManejadorFallas.DespacharFallaPorBolsaDeSeguridadOrigen(ingreso.EnvioIngreso, ControllerContext.Current.Usuario);
        }
      }

      ///Si el envio es descargado por la seccion del envios sueltos, es decir los estados del empaque es bien embalado o
      ///mal embalado, envia una falla por diferencia de peso
      if (ingreso.EnvioIngreso.EstadoEmpaque.IdEstadoEmpaque == OPConstantesOperacionNacional.ID_ESTADO_EMPAQUE_BIEN_EMBALADO
        || ingreso.EnvioIngreso.EstadoEmpaque.IdEstadoEmpaque == OPConstantesOperacionNacional.ID_ESTADO_EMPAQUE_MAL_EMBALADO)
      {
        ///Si el envio tiene valor de peso de ingreso valida la diferencia teniendo encuenta el desfase del peso
        if (ingreso.EnvioIngreso.PesoGuiaSistema > 0)
        {
          ///Obtiene el valor de desfase para el peso
          string valor = ONRepositorio.Instancia.ObtenerParametroOperacionNacional(OPConstantesOperacionNacional.ID_PARAMETRO_DESFASE_PESO);
          decimal valorDesfase = Convert.ToDecimal(valor);
          ///VAlida que el peso ingresado sea menor que el peso registrado en la admision mas o menos el valor del desfase
          ///si es menor envia una falla
          if (ingreso.EnvioIngreso.PesoGuiaIngreso < (ingreso.EnvioIngreso.PesoGuiaSistema - valorDesfase) ||
            ingreso.EnvioIngreso.PesoGuiaIngreso < (ingreso.EnvioIngreso.PesoGuiaSistema + valorDesfase))
          { 
            ONManejadorFallas.DespacharFallaPorDiferenciaPeso(ingreso.EnvioIngreso, ControllerContext.Current.Usuario);
          }
        }
      }

      using (TransactionScope scope = new TransactionScope())
      {
        if (ingreso.IdIngresoOperativo <= 0)
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_OPERATIVO_NO_INICIADO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_OPERATIVO_NO_INICIADO)));
        ///La guia no esta capturada en sistema
        if (ingreso.EnvioIngreso.IdAdmisionMensajeria == 0)
        {
          ///Si ya se ingreso el envio para el id del operativo muestra una excepcion, de lo contrario ingresa el envio
          if (!ONRepositorio.Instancia.ObtenerIngresoAgenciaEnvioNoRegistrado(ingreso.IdIngresoOperativo, ingreso.EnvioIngreso.NumeroGuia.Value))
          {
            if (ingreso.ManifiestoIngreso.ConsolidadoManifiesto == null)
              ingreso.ManifiestoIngreso.ConsolidadoManifiesto = new ONConsolidado();

            ONRepositorio.Instancia.GuardarIngresoEnvioNoRegistrado(ingreso);
          }
          else
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ENVIO_YA_INGRESO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ENVIO_YA_INGRESO)));
        }
        /// La guia esta capturada en sistema
        else
        {
          ///consulta el envio, si el envio ya fue ingresado genera una excepcion, sino ingresa el envio
          if (!ONRepositorio.Instancia.ObtenerIngresoAgenciaEnvio(ingreso.IdIngresoOperativo, ingreso.EnvioIngreso.IdAdmisionMensajeria))
            ONRepositorio.Instancia.GuardarIngresoEnvioRegistrado(ingreso);
          else
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ENVIO_YA_INGRESO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ENVIO_YA_INGRESO)));

         ADTrazaGuia trazaGuia = new ADTrazaGuia()
          {
              IdAdmision = ingreso.EnvioIngreso.IdAdmisionMensajeria,
              NumeroGuia = ingreso.EnvioIngreso.NumeroGuia,
              Observaciones = string.Empty,
              IdCiudad = ingreso.CiudadDescargue.IdLocalidad,
              Ciudad = ingreso.CiudadDescargue.Nombre,
              Modulo = COConstantesModulos.MODULO_OPERACION_NACIONAL,
              IdEstadoGuia = (short)EstadosGuia.ObtenerUltimoEstadoxNumero(ingreso.EnvioIngreso.NumeroGuia.Value),
              IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio
          };

         EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia);
         if (trazaGuia.IdTrazaGuia == 0)
         {
             string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();
             ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL
                             , EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                             , MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO));
             excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
             throw new FaultException<ControllerException>(excepcion);
         }

          if (ingreso.CiudadDescargue.IdLocalidad == ingreso.EnvioIngreso.IdLocalidadDestino)
          {
            estaDescargada = true;
            if (ingreso.ManifiestoIngreso.ConsolidadoManifiesto != null)
            {
              ONRepositorio.Instancia.ActualizarDescargueEnvioConsolidado(ingreso.ManifiestoIngreso.ConsolidadoManifiesto.IdManfiestoConsolidado,
                ingreso.EnvioIngreso.IdAdmisionMensajeria, estaDescargada);
            }
          }

        

          //Validar si el envio pertenece al consolidado
          if(ingreso.ManifiestoIngreso.ConsolidadoManifiesto!=null && ONRepositorio.Instancia.ValidarEnvioDentroConsolidado(ingreso.ManifiestoIngreso.ConsolidadoManifiesto.IdManfiestoConsolidado,ingreso.EnvioIngreso.NumeroGuia.Value))
            ingreso.EnvioIngreso.PerteneceConsolidado = true;
          else
            ingreso.EnvioIngreso.PerteneceConsolidado = false;

          //Validar si el envio esta manifestado
          if (ONRepositorio.Instancia.ValidarEnvioDentroManifiesto(ingreso.ManifiestoIngreso.IdManifiestoOperacionNacional, ingreso.EnvioIngreso.NumeroGuia.Value))
            ingreso.EnvioIngreso.EstaManifestado = true;
          else
            ingreso.EnvioIngreso.EstaManifestado = false;

        }
        scope.Complete();
      }

      return ingreso;
    }

    /// <summary>
    /// Guarda y valida los envios transito
    /// </summary>
    /// <param name="idGuiaInterna"></param>
    /// <param name="ingreso"></param>
    public void GuardaIngresoEnviosTransito(ONIngresoOperativoDC ingreso)
    {
         


      bool estaDescargado = false;
      string valor;
      int idEstadoEmpaque;
      ///Consulta que el numero de guia ingresada corresponda a un envio transito
      ONConsolidado consolidado = ONRepositorio.Instancia.ObtenerConsolidadoEnvioTransito(ingreso.Vehiculo.IdVehiculo, estaDescargado, ingreso.ManifiestoIngreso.ConsolidadoManifiesto.NumeroGuiaInterna.Value);

      ///Si el consolidado esta manifestado para el vehiculo y no esta descargado
      if (consolidado.IdGuiaInterna > 0)
      {
        ///Si la localidad destino del consolidado es diferente a la ciudad destino donde se esta haciendo el descargue
        ///realiza el ingreso a bodega
        if (consolidado.LocalidadManifestada.IdLocalidad != ingreso.CiudadDescargue.IdLocalidad)
        {
          valor = ONRepositorio.Instancia.ObtenerParametroOperacionNacional(OPConstantesOperacionNacional.ID_PARAMETRO_ESTADO_EMPAQUE_ENVIO_TRANSITO);

          idEstadoEmpaque = Convert.ToInt16(valor);
          ONRepositorio.Instancia.GuardarIngresoEnviosTransito(ingreso.IdIngresoOperativo, idEstadoEmpaque, ingreso.ManifiestoIngreso.ConsolidadoManifiesto.NumeroGuiaInterna.Value);
        }
        else
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ENVIO_NO_ES_TRANSITO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ENVIO_NO_ES_TRANSITO)));
      }
      else
        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ENVIO_TRANSITO_NO_MANIFESTADO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ENVIO_TRANSITO_NO_MANIFESTADO)));
    }

    /// <summary>
    /// Obtiene los envios sueltos del manifiesto
    /// </summary>
    /// <param name="idVehiculo">id del vehiculo</param>
    /// <param name="idIngresoOperativo">id del ingreso a la agencia del vehiculo</param>
    /// <param name="estaDescargada">bit para saber si esta descargado o no el manifiesto</param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize">tamaño de la pagina</param>
    /// <returns></returns>
    public List<ONConsolidadoDetalle> ObtenerEnviosSueltosManifiestoVehiculo(int idVehiculo, int idRuta, long idIngresoOperativo, int pageIndex, int pageSize)
    {
      bool estaDescargada = false;
      return ONRepositorio.Instancia.ObtenerEnviosSueltosManifiestoVehiculo(idVehiculo, idRuta, idIngresoOperativo, estaDescargada, pageIndex, pageSize);
    }

    /// <summary>
    /// Retorna los envios del consolidado seleccionado
    /// </summary>
    /// <param name="idConsolidado"></param>
    /// <returns></returns>
    public List<ONConsolidadoDetalle> ObtenerEnviosConsolidado(long idConsolidado)
    {
      return ONRepositorio.Instancia.ObtenerEnviosConsolidado(idConsolidado);
    }

    /// <summary>
    /// Obtiene los manifiestos sin descargar de un vehiculo
    /// </summary>
    /// <param name="idVehiculo"></param>
    /// <returns></returns>
    public List<ONManifiestoOperacionNacional> ObtenerManifiestosVehiculo(int idVehiculo)
    {
      return ONRepositorio.Instancia.ObtenerManifiestosVehiculo(idVehiculo);
    }

    /// <summary>
    /// Obtiene el total de los envios manifestados
    /// </summary>
    /// <param name="idVehiculo"></param>
    /// <param name="idLocalidadDestino"></param>
    /// <returns></returns>
    public int ObtenerTotalEnviosManifestadosVehiculoLocalidad(int idVehiculo, string idLocalidadDestino)
    {
      return ONRepositorio.Instancia.ObtenerTotalEnviosManifestadosVehiculoLocalidad(idVehiculo, idLocalidadDestino);
    }

    /// <summary>
    /// Obtiene el total de los envios sobrantes
    /// </summary>
    /// <param name="idVehiculo"></param>
    /// <param name="idLocalidadDestino"></param>
    /// <returns></returns>
    public int ObtenerTotalEnviosSobrantesVehiculoLocalidad(long idOperativo, string idLocalidadDestino)
    {
      return ONRepositorio.Instancia.ObtenerTotalEnviosSobrantesVehiculoLocalidad(idOperativo, idLocalidadDestino);
    }

    /// <summary>
    /// Obtiene el total de los envios descargados
    /// </summary>
    /// <param name="idVehiculo"></param>
    /// <param name="idLocalidadDestino"></param>
    /// <returns></returns>
    public int ObtenerTotalEnviosDescargadosVehiculoLocalidad(long idOperativo, string idLocalidadDestino)
    {
      return ONRepositorio.Instancia.ObtenerTotalEnviosDescargadosVehiculoLocalidad(idOperativo, idLocalidadDestino);
    }

    /// <summary>
    /// Cierra el ingreso del operativo para el vehiculo
    /// </summary>
    public void CerrarIngresoOperativoAgencia(ONIngresoOperativoDC ingreso)
    {
      bool estaCerrado = true;

      if (!ONRepositorio.Instancia.VerificarEstadoIngresoOperativoAgencia(ingreso.IdIngresoOperativo))
      {

          using (TransactionScope scope = new TransactionScope())
          {

              ///Cierra el ingreso del operativo
              ONRepositorio.Instancia.ActualizarCierreOperativoAgencia(ingreso.IdIngresoOperativo, estaCerrado);

              ///guarda los sobrantes y faltes del operativo
              ONRepositorio.Instancia.GuardarInconsistenciasSobrantesOperativoIngreso(ingreso.IdIngresoOperativo, ingreso.ManifiestoIngreso.IdManifiestoOperacionNacional);
              ONRepositorio.Instancia.GuardarInconsistenciasFaltantesOperativoIngreso(ingreso.IdIngresoOperativo, ingreso.ManifiestoIngreso.IdManifiestoOperacionNacional);

              ///Valida que la ciudad de descargue sea igual a la ciudad destino de la ruta
              ///Si es igual, descarga el manifiesto
              if (ingreso.Ruta.IdLocalidadDestino == ingreso.CiudadDescargue.IdLocalidad)
              {
                  ONRepositorio.Instancia.ActualizarDescargueManifiestoRuta(ingreso.Ruta.IdRuta, ingreso.Vehiculo.IdVehiculo);
              }

              scope.Complete();
          }
      }
      else
      {
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_INGRESO_A_AGENCIA_CERRADO.ToString(),MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_INGRESO_A_AGENCIA_CERRADO)));
      }

    }
  }
}