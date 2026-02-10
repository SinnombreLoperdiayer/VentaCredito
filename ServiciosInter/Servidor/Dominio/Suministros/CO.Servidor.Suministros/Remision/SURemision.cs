using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Comun;
using CO.Servidor.Suministros.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Integraciones.Novasoft.Proxies;

namespace CO.Servidor.Suministros.Remision
{
  public class SURemision
  {
    /// <summary>
    /// Retorna instancia de la provision del mensajero
    /// </summary>
    internal CO.Servidor.Suministros.Remision.SUProvisionMensajero provisionMensajero
    {
      get
      {
        return new SUProvisionMensajero();
      }
    }

    /// <summary>
    /// Retorna instancia de la provision del cliente
    /// </summary>
    internal CO.Servidor.Suministros.Remision.SUProvisionCliente provisionCliente
    {
      get
      {
        return new SUProvisionCliente();
      }
    }

    /// <summary>
    /// Retorna instancia de la provision del canal de ventas
    /// </summary>
    internal CO.Servidor.Suministros.Remision.SUProvisionCanalVenta provisionCanalVenta
    {
      get
      {
        return new SUProvisionCanalVenta();
      }
    }

    /// <summary>
    /// Retorna instancia de la provision del canal de ventas
    /// </summary>
    internal CO.Servidor.Suministros.Remision.SUProvisionProceso provisionProceso
    {
      get
      {
        return new SUProvisionProceso();
      }
    }

    /// <summary>
    /// Valida la asignacion del suministro
    /// </summary>
    /// <param name="suministro"></param>
    public SUSuministro ValidaSuministroAsignacion(SUSuministro suministro)
    {
      ///ToDo:Johanna Quitar al integrar con novaSoft Valida las existencias del suministro en novasoft
      // IIntegracionNovasoft integracion = new ImpIntegracionNovasoft();
      // long cantidadExistente = integracion.ObtenerCantidadExistenteSuministro(suministro.Id);

      //if (cantidadExistente < suministro.CantidadAsignada)
      //  throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_EXISTENCIA_INSUFICIENTE_SUMINISTRO.ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_EXISTENCIA_INSUFICIENTE_SUMINISTRO), suministro.Descripcion)));

        if (suministro.AplicaResolucion && suministro.Id != Convert.ToInt16(SUEnumSuministro.GUIA_CORRESPONDENCIA_INTERNA) && suministro.Id != Convert.ToInt16(SUEnumSuministro.GUIA_TRANSPORTE_MANUAL_OFFLINE))
      {
        suministro = ValidaRangoConfiguradoSuministros(suministro);
        ValidaDisponibilidadRangosSuministroAsignado(suministro);
      }

      return suministro;
    }

    /// <summary>
    /// Valida que el rango ingresado para el suministro este configurado en una resolucion vigente
    /// </summary>
    /// <param name="suministro"></param>
    public SUSuministro ValidaRangoConfiguradoSuministros(SUSuministro suministro)
    {
      long rangoInicialIngresado = suministro.RangoInicial;
      bool rangoInicialconf = false;
      bool rangoFinalConfigurado = false;
      bool fechaVigenteSuministro = true;

      ///Obtiene los numeradores vigentes configurados para el suministro ingresado
      List<SUNumeradorPrefijo> numerador = SURepositorioAdministracion.Instancia.ObtenerNumeradorSuministroVigentes(suministro.Id, suministro.Descripcion);

      ///si no existe resoluciones configuradas para el suministro lanza una excepcion
      if (numerador.Count > 0)
      {
        numerador.ForEach(num =>
        {
          ///Valida que el rango ingresado este en una configuracion para el suministro
          ///Se adiciona validación de numeración que este activa la resolución
          if (rangoInicialIngresado >= num.RangoInicial && rangoInicialIngresado <= num.RangoFinal && num.EstaActivo)
          {
            rangoInicialconf = true;
            rangoInicialIngresado = num.RangoFinal;
            if (suministro.RangoFinal <= num.RangoFinal)
              rangoFinalConfigurado = true;

            if (num.FechaFinal < DateTime.Now.Date)
            {
              fechaVigenteSuministro = false;
            }
            suministro.FechaInicialResolucion = num.FechaInicial;
            suministro.FechaFinalResolucion = num.FechaFinal;

            suministro.IdResolucion = num.IdNumerador;
          }
        
        });

        ///Si no existe una configuracion para el rango muestra una excepcion
        if (!rangoInicialconf && !rangoFinalConfigurado)
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_ERROR_RANGO_NO_CONFIGURADO_RESOLUCIONES.ToString(),
            string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_RANGO_NO_CONFIGURADO_RESOLUCIONES), suministro.Descripcion)));

        //Error cuando la fecha final de vigencia del suministro es inferior a la fecha actual
        if (!fechaVigenteSuministro)
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_ERROR_FECHA_RANGO_VIGENCIA_RESOLUCION.ToString(),
            string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_FECHA_RANGO_VIGENCIA_RESOLUCION), suministro.Descripcion)));
      }
      else
        throw new FaultException<ControllerException>
          (new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
           EnumTipoErrorSuministros.EX_ERROR_NUMERACION_NO_CONFIGURADA.ToString(),
           string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_NUMERACION_NO_CONFIGURADA), suministro.Descripcion)));

      return suministro;
    }

    /// <summary>
    /// VAlida que el rango ingresado no este asignado
    /// </summary>
    /// <param name="suministro"></param>
    public void ValidaDisponibilidadRangosSuministroAsignado(SUSuministro suministro)
    {
      SURepositorioAdministracion.Instancia.ValidaRangoSuministrosAsignados(suministro.Id, suministro.Descripcion, suministro.RangoInicial, suministro.RangoFinal);
    }

    /// <summary>
    /// Crea la guia interna de la remision
    /// </summary>
    /// <param name="guiaInterna"></param>
    /// <returns>NUmero de guia interna</returns>
    public ADGuiaInternaDC CrearGuiaInternaRemision(ADGuiaInternaDC guiaInterna)
    {
      return COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarGuiaInterna(guiaInterna);
    }

    /// <summary>
    /// Obtiene los grupos de suministros
    /// </summary>
    /// <returns></returns>
    public List<SUGrupoSuministrosDC> ObtenerGruposSuministros()
    {
      return SURepositorioAdministracion.Instancia.ObtenerGruposSuministros();
    }

    /// <summary>
    /// Retorna los suministros que esten en el rango de fecha seleccionado
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <returns></returns>
    public List<SURemisionSuministroDC> ObtenerSuministrosRemisionXRangoFecha(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, string idGrupo)
    {
      List<SURemisionSuministroDC> listSuministro = new List<SURemisionSuministroDC>();

      if (idGrupo != null)
      {
        if (idGrupo.CompareTo(SUEnumGrupoSuministroDC.CLI.ToString()) == 0)
          listSuministro = SURepositorioAdministracion.Instancia.ObtenerSuministroAsignadoSucursalXRangoFechas(filtro, indicePagina, registrosPorPagina);
        else if (idGrupo.CompareTo(SUEnumGrupoSuministroDC.MEN.ToString()) == 0)
          listSuministro = SURepositorioAdministracion.Instancia.ObtenerSuministroAsignadoMensajeroXRangoFechas(filtro, indicePagina, registrosPorPagina);
        else if (idGrupo.CompareTo(SUEnumGrupoSuministroDC.AGE.ToString()) == 0
            || idGrupo.CompareTo(SUEnumGrupoSuministroDC.PTO.ToString()) == 0
            || idGrupo.CompareTo(SUEnumGrupoSuministroDC.RAC.ToString()) == 0)
          listSuministro = SURepositorioAdministracion.Instancia.ObtenerSuministroAsignadoCentroSvcXRangoFechas(filtro, indicePagina, registrosPorPagina);
        else if (idGrupo.CompareTo(SUEnumGrupoSuministroDC.PRO.ToString()) == 0)
          listSuministro = SURepositorioAdministracion.Instancia.ObtenerSuministroAsignadoGestionXRangoFechas(filtro, indicePagina, registrosPorPagina);
      }
      return listSuministro;
    }

    /// <summary>
    /// Realiza la desasignacion de suministros de un mensajero
    /// </summary>
    /// <param name="remision">remision</param>
    /// <param name="EsModificacion">Indica si la desasignacion es una modificacion o de una desasignacion</param>
    public void DesasignarSuministrosRemision(SURemisionSuministroDC remision, bool esModificacion)
    {
      if (remision.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.MEN.ToString()) == 0)
      {
        if (!esModificacion)
          DesasignarSuministrosRemisionMensajero(remision);
        else
        {
          SURepositorioAdministracion.Instancia.DesasignarRangoSuministrosRemisionMen(remision.GrupoSuministros.SuministroGrupo.IdProvisionSuministroSerial, remision.MensajeroAsignacion.IdMensajero, remision.GrupoSuministros.SuministroGrupo.Id);
        }
      }
      else if (remision.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.CLI.ToString()) == 0)
      {
        if (!esModificacion)
          DesasignarSuministrosRemisionSucursal(remision);
        else
          SURepositorioAdministracion.Instancia.DesasignarRangoSuministrosRemisionSuc(remision.GrupoSuministros.SuministroGrupo.IdProvisionSuministro, remision.Sucursal.IdSucursal, remision.GrupoSuministros.SuministroGrupo.Id);
      }
      else if (remision.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.AGE.ToString()) == 0
              || remision.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.PTO.ToString()) == 0
              || remision.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.RAC.ToString()) == 0)
      {
        if (!esModificacion)
          DesasignarSuministrosRemisionCentroSvc(remision);
        else
          SURepositorioAdministracion.Instancia.DesasignarRangoSuministrosRemisionCentroSvc(remision.GrupoSuministros.SuministroGrupo.IdProvisionSuministro, remision.CentroServicioAsignacion.IdCentroServicio, remision.GrupoSuministros.SuministroGrupo.Id);
      }
      else if (remision.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.PRO.ToString()) == 0)
      {
        if (!esModificacion)
          DesasignarSuministrosRemisionProceso(remision);
        else
          SURepositorioAdministracion.Instancia.DesasignarRangoSuministrosRemisionProceso(remision.GrupoSuministros.SuministroGrupo.IdProvisionSuministro, Convert.ToInt32(remision.GrupoSuministros.SuministroGrupo.IdPropietario), remision.GrupoSuministros.SuministroGrupo.Id);
      }
    }

    /// <summary>
    /// Realiza la desasignacion de suministros de un mensajero
    /// </summary>
    public void DesasignarSuministrosRemisionMensajero(SURemisionSuministroDC remision)
    {
        ///Valida que el suministro no haya sido consumido y realiza la desasignacion
        if (!SURepositorioAdministracion.Instancia.ValidaConsumoSuministroRango(remision.GrupoSuministros.SuministroGrupo.Id, remision.GrupoSuministros.SuministroGrupo.RangoInicial, remision.GrupoSuministros.SuministroGrupo.RangoFinal))
        {
            using (TransactionScope scope = new TransactionScope())
            {
                SURepositorioAdministracion.Instancia.DesasignaSuministroProvisionReferencia(remision.GrupoSuministros.SuministroGrupo.RangoInicial, remision.GrupoSuministros.SuministroGrupo.RangoFinal, remision.GrupoSuministros.SuministroGrupo.Id);
                SURepositorioAdministracion.Instancia.DesasignarRangoSuministrosRemisionMen(remision.GrupoSuministros.SuministroGrupo.IdProvisionSuministro, remision.MensajeroAsignacion.IdMensajero, remision.GrupoSuministros.SuministroGrupo.Id); 
                scope.Complete();
            }
        }
        else
            throw new FaultException<ControllerException>
           (new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
            EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO.ToString(),
            string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO), remision.GrupoSuministros.SuministroGrupo.Descripcion)));


    }

    /// <summary>
    /// Realiza la desasignacion de suministros de un mensajero
    /// </summary>
    public void DesasignarSuministrosRemisionSucursal(SURemisionSuministroDC remision)
    {
        ///Valida que el suministro no haya sido consumido y realiza la desasignacion
        if (!SURepositorioAdministracion.Instancia.ValidaConsumoSuministroRango(remision.GrupoSuministros.SuministroGrupo.Id, remision.GrupoSuministros.SuministroGrupo.RangoInicial, remision.GrupoSuministros.SuministroGrupo.RangoFinal))
        {
            using (TransactionScope scope = new TransactionScope())
            {
                SURepositorioAdministracion.Instancia.DesasignaSuministroProvisionReferencia(remision.GrupoSuministros.SuministroGrupo.RangoInicial, remision.GrupoSuministros.SuministroGrupo.RangoFinal, remision.GrupoSuministros.SuministroGrupo.Id);
								SURepositorioAdministracion.Instancia.DesasignarRangoSuministrosRemisionSuc(remision.GrupoSuministros.SuministroGrupo.IdProvisionSuministro, Convert.ToInt32(remision.GrupoSuministros.SuministroGrupo.IdPropietario), remision.GrupoSuministros.SuministroGrupo.Id);
                scope.Complete();
            }
        }
        else
            throw new FaultException<ControllerException>
           (new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
            EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO.ToString(),
            string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO), remision.GrupoSuministros.SuministroGrupo.Descripcion)));

    }

    /// <summary>
    /// Realiza la desasignacion de centro de servicios
    /// </summary>
    public void DesasignarSuministrosRemisionProceso(SURemisionSuministroDC remision)
    {
        ///Valida que el suministro no haya sido consumido y realiza la desasignacion
        if (!SURepositorioAdministracion.Instancia.ValidaConsumoSuministroRango(remision.GrupoSuministros.SuministroGrupo.Id, remision.GrupoSuministros.SuministroGrupo.RangoInicial, remision.GrupoSuministros.SuministroGrupo.RangoFinal))
        {
            using (TransactionScope scope = new TransactionScope())
            {
                SURepositorioAdministracion.Instancia.DesasignaSuministroProvisionReferencia(remision.GrupoSuministros.SuministroGrupo.RangoInicial, remision.GrupoSuministros.SuministroGrupo.RangoFinal, remision.GrupoSuministros.SuministroGrupo.Id);
                SURepositorioAdministracion.Instancia.DesasignarRangoSuministrosRemisionProceso(remision.GrupoSuministros.SuministroGrupo.IdProvisionSuministroSerial, Convert.ToInt32(remision.GrupoSuministros.SuministroGrupo.IdPropietario), remision.GrupoSuministros.SuministroGrupo.Id);
                scope.Complete();
            }
        }
        else
            throw new FaultException<ControllerException>
           (new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
            EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO.ToString(),
            string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO), remision.GrupoSuministros.SuministroGrupo.Descripcion)));
    }

    /// <summary>
    /// Realiza la desasignacion de centro de servicios
    /// </summary>
    public void DesasignarSuministrosRemisionCentroSvc(SURemisionSuministroDC remision)
    {
			//Valida que el suministro no haya sido consumido y realiza la desasignacion
			if (!SURepositorioAdministracion.Instancia.ValidaConsumoSuministroRango(remision.GrupoSuministros.SuministroGrupo.Id, remision.GrupoSuministros.SuministroGrupo.RangoInicial, remision.GrupoSuministros.SuministroGrupo.RangoFinal))
			{
				using (TransactionScope scope = new TransactionScope())
				{
					SURepositorioAdministracion.Instancia.DesasignarRangoSuministrosRemisionCentroSvc(remision.GrupoSuministros.SuministroGrupo.IdProvisionSuministro, remision.CentroServicioAsignacion.IdCentroServicio, remision.GrupoSuministros.SuministroGrupo.Id);
					scope.Complete();
				}
			}
			else
				throw new FaultException<ControllerException>
			 (new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
				EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO.ToString(),
				string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO), remision.GrupoSuministros.SuministroGrupo.Descripcion)));
    }

    /// <summary>
    /// Método para cambiar el estado de una remision a anulada
    /// </summary>
    /// <param name="remision"></param>
    public void AnularRemision(SURemisionSuministroDC remision)
    {
      SURepositorioAdministracion.Instancia.AnularRemision(remision);
    }

    public long AdministrarModificacionSuministro(SURemisionSuministroDC remisionDestino, SURemisionSuministroDC remisionOrigen)
    {
      long idRemision = 0;
      using (TransactionScope scope = new TransactionScope())
      {
        ///Realiza la desasignacion y guarda los historicos correspondientes
        remisionDestino.GrupoSuministros.SuministroGrupo = remisionOrigen.GrupoSuministros.SuministroGrupo;
        //Si el nuevo rango de suministros es para un grupo de suministros de mensajero
        if (remisionDestino.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.MEN.ToString()) == 0)
        {
          idRemision = provisionMensajero.ModificarProvisionSuministroMensajero(remisionDestino, remisionOrigen);
        }
        else if (remisionDestino.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.CLI.ToString()) == 0)
        {
          idRemision = provisionCliente.ModificarProvisionSuministroSucursal(remisionDestino, remisionOrigen);
        }
        else if (remisionDestino.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.AGE.ToString()) == 0
          || remisionDestino.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.PTO.ToString()) == 0
          || remisionDestino.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.RAC.ToString()) == 0)
        {
          idRemision = provisionCanalVenta.ModificarProvisionSuministroCanalVenta(remisionDestino, remisionOrigen);
        }
        else if (remisionDestino.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.PRO.ToString()) == 0)
        {
          if (remisionDestino.CentroServicioAsignacion == null)
            remisionDestino.CentroServicioAsignacion = new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC();
          remisionDestino.CentroServicioAsignacion.IdCentroCostos = remisionDestino.ProcesoAsignacion.CentroCostos;
          remisionDestino.CentroServicioAsignacion.CodigoBodega = remisionDestino.ProcesoAsignacion.CodBodegaErp;
          idRemision = provisionProceso.ModificarProvisionSuministroProceso(remisionDestino, remisionOrigen);
        }

        scope.Complete();
      }
      return idRemision;
    }

    /// <summary>
    /// Guarda la provision de suministros restantes al grupo de suministros original
    /// </summary>
    /// <param name="remisionOrigen"></param>
    /// <param name="rangoInicial"></param>
    /// <param name="rangoFinal"></param>
    public void GuardarProvisionRestanteModificacionSuministro(SURemisionSuministroDC remisionOrigen, long rangoInicial, long rangoFinal)
    {
      long idProvisionSum = 0;
      if (remisionOrigen.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.MEN.ToString()) == 0)
      {
        remisionOrigen.GrupoSuministros.SuministroGrupo.CantidadAsignada = (int)(rangoFinal - rangoInicial);
        ///Guarda la provision de suministros
        idProvisionSum = SURepositorioAdministracion.Instancia.GuardarProvisionSuministrosMensajero(remisionOrigen.GrupoSuministros.SuministroGrupo, remisionOrigen.IdRemision, (int)remisionOrigen.GrupoSuministros.SuministroGrupo.IdAsignacionSuministro);

        ///Guarda el serial de la provision para los suministros del mensajero
        SURepositorioAdministracion.Instancia.GuardarProvisionSuminsitroSerialMensajero(remisionOrigen.GrupoSuministros.SuministroGrupo, idProvisionSum, rangoInicial, rangoFinal - 1, true);
      }
      else if (remisionOrigen.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.CLI.ToString()) == 0)
      {
        remisionOrigen.GrupoSuministros.SuministroGrupo.CantidadAsignada = (int)(rangoFinal - rangoInicial);

        ///Guarda la provision de suministros
        idProvisionSum = SURepositorioAdministracion.Instancia.GuardarProvisionSuministrosSucursal(remisionOrigen.GrupoSuministros.SuministroGrupo, remisionOrigen.IdRemision, (int)remisionOrigen.GrupoSuministros.SuministroGrupo.IdAsignacionSuministro);

        ///Guarda el serial de la provision para los suministros de la sucursal
        SURepositorioAdministracion.Instancia.GuardarProvisionSuminsitroSerialSucursal(remisionOrigen.GrupoSuministros.SuministroGrupo, idProvisionSum, rangoInicial, rangoFinal - 1, true);
      }
      else if (remisionOrigen.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.AGE.ToString()) == 0
        || remisionOrigen.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.PTO.ToString()) == 0
        || remisionOrigen.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.RAC.ToString()) == 0)
      {
        remisionOrigen.GrupoSuministros.SuministroGrupo.CantidadAsignada = (int)(rangoFinal - rangoInicial);
        ///Guarda la provision de suministros
        idProvisionSum = SURepositorioAdministracion.Instancia.GuardarProvisionSuministrosCanalVenta(remisionOrigen.GrupoSuministros.SuministroGrupo, remisionOrigen.IdRemision, (int)remisionOrigen.GrupoSuministros.SuministroGrupo.IdAsignacionSuministro);
        ///Guarda el serial de la provision para los suministros del Canal de venta
        SURepositorioAdministracion.Instancia.GuardarProvisionSuminsitroSerialCanalVenta(remisionOrigen.GrupoSuministros.SuministroGrupo, idProvisionSum, rangoInicial, rangoFinal - 1, true);
      }
      else if (remisionOrigen.GrupoSuministros.IdGrupoSuministro.CompareTo(SUEnumGrupoSuministroDC.PRO.ToString()) == 0)
      {
        remisionOrigen.GrupoSuministros.SuministroGrupo.CantidadAsignada = (int)(rangoFinal - rangoInicial);
        ///Guarda la provision de suministros
        idProvisionSum = SURepositorio.Instancia.GuardarProvisionSuministrosProceso(remisionOrigen.GrupoSuministros.SuministroGrupo, remisionOrigen.IdRemision, (int)remisionOrigen.GrupoSuministros.SuministroGrupo.IdAsignacionSuministro);

        ///Guarda el serial de la provision para los suministros del proceso
        SURepositorio.Instancia.GuardarProvisionSuministroSerialProceso(remisionOrigen.GrupoSuministros.SuministroGrupo, idProvisionSum, rangoInicial, rangoFinal - 1, true);
      }
    }
  }
}