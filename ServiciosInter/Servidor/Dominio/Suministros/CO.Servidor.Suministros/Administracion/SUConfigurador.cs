using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Comun;
using CO.Servidor.Suministros.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;

namespace CO.Servidor.Suministros.Administracion
{
  /// <summary>
  /// Clase para configurar suministros
  /// </summary>
  internal class SUConfigurador
  {
    #region suministros

    /// <summary>
    /// Obtiene todos los suministros configurados
    /// </summary>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      if (string.IsNullOrWhiteSpace(campoOrdenamiento))
      {
        campoOrdenamiento = "SUM_Descripcion";
        ordenamientoAscendente = true;
      }
      return SURepositorioAdministracion.Instancia.ObtenerTodosSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Realiza la administracion del suministro
    /// </summary>
    /// <param name="suministro"></param>
    public void AdministrarSuministro(SUSuministro suministro)
    {
      if (suministro.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
      {
        ///Valida que no exista un suministro con el mismo codigo erp o interno
        if (SURepositorioAdministracion.Instancia.ObtenerSuministroXCodigoERP(suministro.CodigoERP) == null)
        {
          if (SURepositorioAdministracion.Instancia.ObtenerSuministroXCodigoAlterno(suministro.CodigoAlterno) == null)
            SURepositorioAdministracion.Instancia.GuardarSuministro(suministro);
          else
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_ERROR_SUMINISTRO_EXISTE_CODIGO_ALTERNO.ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_SUMINISTRO_EXISTE_CODIGO_ALTERNO), suministro.CodigoAlterno)));
        }
        else
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_ERROR_SUMINISTRO_EXISTE_CODIGO_NOVASOFT.ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_SUMINISTRO_EXISTE_CODIGO_NOVASOFT), suministro.CodigoERP)));
      }
      else if (suministro.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
      {
        ///Valida que no exista un suministro con el mismo codigo erp o interno
        if (SURepositorioAdministracion.Instancia.ValidarSuministroXCodigoERP(suministro.CodigoERP, suministro.Id))
        {
          if (SURepositorioAdministracion.Instancia.ValidarSuministroXCodigoAlterno(suministro.CodigoAlterno, suministro.Id))
            EditarSuministro(suministro);
          else
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_ERROR_SUMINISTRO_EXISTE_CODIGO_ALTERNO.ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_SUMINISTRO_EXISTE_CODIGO_ALTERNO), suministro.CodigoAlterno)));
        }
        else
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_ERROR_SUMINISTRO_EXISTE_CODIGO_NOVASOFT.ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_SUMINISTRO_EXISTE_CODIGO_NOVASOFT), suministro.CodigoERP)));
      }
    }

    /// <summary>
    /// Obtiene todos los suministros
    /// </summary>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministros()
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministros();
    }

    /// <summary>
    /// Edita el suministro
    /// y el estado del mismo en las tablas de SuministrosCentroServicios_SUM-
    ///SuministrosMensajero_SUM-SuministrosProceso_SUM-SuministrosSucursal_SUM
    /// </summary>
    /// <param name="suministro"></param>
    public void EditarSuministro(SUSuministro suministro)
    {
      //valido si el estado del suministro cambio
      //para actualizar las demas tablas que utilizan el
      //mismo suministroRafram 20/02/2013
      bool suministroActivo = ObtenerSuministro(suministro.Id).EstaActivo;

      if (suministroActivo != suministro.EstaActivo)
      {
        string estadoSuministro = suministro.EstaActivo == true ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO;

        SURepositorioAdministracion.Instancia.ActualizarEstadoSuministroCentroServicio(suministro.Id, estadoSuministro);
        SURepositorioAdministracion.Instancia.ActualizarEstadoSuministroMensajero(suministro.Id, estadoSuministro);
        SURepositorioAdministracion.Instancia.ActualizarEstadoSuministroProceso(suministro.Id, estadoSuministro);
        SURepositorioAdministracion.Instancia.ActualizarEstadoSuministroSucursal(suministro.Id, estadoSuministro);
      }

      SURepositorioAdministracion.Instancia.EditarSuministro(suministro);
    }

    /// <summary>
    /// trae la info de un suministro por su
    /// id
    /// </summary>
    /// <param name="idSuministro"></param>
    /// <returns>info suministro</returns>
    public SUSuministro ObtenerSuministro(int idSuministro)
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministro(idSuministro);
    }

    /// <summary>
    /// Obtener los suministros activos y que sean para preimprimir
    /// </summary>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosPreImpresion()
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministrosPreImpresion();
    }

    /// <summary>
    /// Retorna las categorias de los suministros
    /// </summary>
    /// <returns></returns>
    public List<SUCategoriaSuministro> ObtenerCategoriasSuministros()
    {
      return SURepositorioAdministracion.Instancia.ObtenerCategoriasSuministros();
    }

    #endregion suministros

    #region ResolucionSuministro

    /// <summary>
    /// Retorna las resoluciones de suministros
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public List<SUNumeradorPrefijo> ObtenerResolucionesSuministro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      if (string.IsNullOrWhiteSpace(campoOrdenamiento))
      {
        campoOrdenamiento = "SUM_Descripcion";
        ordenamientoAscendente = true;
      }
      return SURepositorioAdministracion.Instancia.ObtenerResolucionesSuministro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Administra la resulucion del suministro
    /// </summary>
    /// <param name="numerador"></param>
    public void AdministrarResolucion(SUNumeradorPrefijo numerador)
    {
      if (numerador.EstadoRegistro.CompareTo(EnumEstadoRegistro.ADICIONADO) == 0)
      {
        ValidaRangosNumerador(numerador);
        numerador.IdNumerador = ObtenerConsecutivoNumerador(PAEnumConsecutivos.Resolucion_de_suministros).ToString();
        SURepositorioAdministracion.Instancia.GuardarResolucionSuministro(numerador);
      }
      else if (numerador.EstadoRegistro.CompareTo(EnumEstadoRegistro.MODIFICADO) == 0)
      {
        ValidaRangosNumerador(numerador);
        SURepositorioAdministracion.Instancia.EditarResolucionSuministro(numerador);
      }
    }

    /// <summary>
    /// Metodo para Obtener un consecutivo
    /// de una solicitud
    /// </summary>
    /// <param name="tipoConsecutivo">Valor del tipo de consecutivo</param>
    /// <returns>El consecutivo de la solicitud</returns>
    public long ObtenerConsecutivoNumerador(PAEnumConsecutivos tipoConsecutivo)
    {
      using (TransactionScope scope = new TransactionScope())
      {
        long consecutivo = PAAdministrador.Instancia.ObtenerConsecutivo(tipoConsecutivo);
        scope.Complete();
        return consecutivo;
      };
    }

    /// <summary>
    /// Valida que el rango ingresado no este configurado en otro suministro
    /// </summary>
    /// <param name="numerador"></param>
    public void ValidaRangosNumerador(SUNumeradorPrefijo numerador)
    {
      List<SUNumeradorPrefijo> lstNumerador = SURepositorioAdministracion.Instancia.ObtenerNumeradorSuministroVigentes(numerador.Suministro.Id, numerador.Suministro.Descripcion);

      if (lstNumerador.Any())
      {
        lstNumerador.Where(nu => nu.IdNumerador != numerador.IdNumerador).ToList().ForEach(r =>
        {
          if ((numerador.RangoInicial >= r.RangoInicial && numerador.RangoInicial <= r.RangoFinal) || (numerador.RangoFinal >= r.RangoInicial && numerador.RangoFinal <= r.RangoFinal))
          {
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_RANGO_NO_VALIDO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_RANGO_NO_VALIDO)));
          }
          if ((numerador.FechaInicial <= r.FechaFinal) || (numerador.FechaFinal <= r.FechaInicial))
          {
            if (numerador.RangoActual == r.RangoActual)
              throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_ERROR_FECHA_RANGO_RESOLUCION.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_FECHA_RANGO_RESOLUCION)));
          }
        });
      }
    }

        public SUDatosResponsableSuministroDC ObtenerResponsableSuministro(long numeroGuia)
        {
            return SURepositorioAdministracion.Instancia.ObtenerResponsableSuministro(numeroGuia);
        }

        /// <summary>
        /// Obtiene los suministros q aplican resolucion
        /// </summary>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosResolucion()
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministrosResolucion();
    }

    #endregion ResolucionSuministro

    #region Grupo de suministros

    /// <summary>
    /// Obtiene los suministros de un grupo
    /// </summary>
    /// <param name="idGrupo"></param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosAsignadosGrupo(string idGrupo)
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministrosAsignadosGrupo(idGrupo);
    }

    /// <summary>
    /// Obtiene los grupos de suministros configurados
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public List<SUGrupoSuministrosDC> ObtenerGrupoSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return SURepositorioAdministracion.Instancia.ObtenerGrupoSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtiene los grupos de suministros configurados incluye lista con suministros grupo
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public List<SUGrupoSuministrosDC> ObtenerGrupoSuministrosConSuminGrupo(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return SURepositorioAdministracion.Instancia.ObtenerGrupoSuministrosConSuminGrupo(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Administra el grupo de suministros
    /// </summary>
    /// <param name="grupo"></param>
    public void AdministrarGrupoSuministros(SUGrupoSuministrosDC grupo)
    {
        if (grupo.SuministroGrupo.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
        {
            SURepositorioAdministracion.Instancia.AdicionarSuministroGrupo(grupo);
            AdicionarSuministro(grupo);
        }

        else if (grupo.SuministroGrupo.EstadoRegistro == EnumEstadoRegistro.BORRADO)
        {
            SURepositorioAdministracion.Instancia.EliminarSuministroGrupo(grupo);
            //EliminarSuministro(grupo);
        }
    }

    //private void EliminarSuministro(SUGrupoSuministrosDC grupo)
    //{
    //    switch (grupo.IdGrupoSuministro)
    //    {
    //        case  "CLI":
    //            SURepositorioAdministracion.Instancia.EliminarSuministroSucursal(grupo);
    //            break;
    //        case  "AGE":
    //            SURepositorioAdministracion.Instancia.EliminarSuministroCentroServicio(grupo);
    //          break;
    //    }
    //}

    private void AdicionarSuministro(SUGrupoSuministrosDC grupo)
    {
        switch (grupo.IdGrupoSuministro)
        {
            case "CLI":
                SURepositorioAdministracion.Instancia.AdicionarSuministroSucursal(grupo);
                break;
            case "AGE":
                SURepositorioAdministracion.Instancia.AdicionarSuministroCentroServicio(grupo);
                break;
        } 
    }

    /// <summary>
    /// Obtener suministros de grupo
    /// </summary>
    /// <param name="idGrupo"></param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosGrupo(string idGrupo, long idMensajero)
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministrosGrupo(idGrupo, idMensajero);
    }

    /// <summary>
    /// Obtiene los suministros que no estan incluidos en ningun grupo, ni en en el grupo seleccionado
    /// </summary>
    /// <param name="idGrupo"></param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosNoIncluidosEnGrupo(IDictionary<string, string> filtro, string idGrupo, long idMensajero)
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministrosNoIncluidosEnGrupo(filtro, idGrupo, idMensajero);
    }

    /// <summary>
    /// Obtiene los mensajeros y conductores configurados
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public IEnumerable<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                          int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      IPOFachadaParametrosOperacion fachadaParametros = COFabricaDominio.Instancia.CrearInstancia<IPOFachadaParametrosOperacion>();
      return fachadaParametros.ObtenerMensajerosConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    #endregion Grupo de suministros

    #region Suministros mensajero

    /// <summary>
    /// Administra la informacion del mensajero
    /// </summary>
    /// <param name="grupoMensajero"></param>
    public void AdministrarSuministroMensajero(SUGrupoSuministrosDC grupoMensajero)
    {
      grupoMensajero.SuministrosGrupo.ForEach(r =>
      {
        grupoMensajero.SuministroGrupo = r;
        SURepositorioAdministracion.Instancia.GuardarSuministrosMensajero(grupoMensajero);
      });
    }

    #endregion Suministros mensajero

    #region Aprovisinamiento de suministros

    /// <summary>
    /// Obtiene las ultimas remisiones realizadas
    /// </summary>
    public List<SURemisionSuministroDC> ObtenerUltimasRemisiones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return SURepositorioAdministracion.Instancia.ObtenerUltimasRemisiones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtiene los suministros aprabados para realizar la remision al mensajero
    /// </summary>
    /// <param name="idMensajero"></param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosAsignadosMensajero(long idMensajero)
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministrosAsignadosMensajero(idMensajero);
    }

    /// <summary>
    /// Consulta los mensajeros de la agencia
    /// </summary>
    /// <param name="idAgencia"></param>
    /// <returns></returns>
    public IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia)
    {
      return COFabricaDominio.Instancia.CrearInstancia<IPOFachadaParametrosOperacion>().ObtenerMensajerosAgencia(idAgencia);
    }

    #endregion Aprovisinamiento de suministros

    #region Aprovisionar suministros de un Centro de servicio

    /// <summary>
    /// Guardar los suministros que posee un centro de servicio
    /// o los actualiza si se cambiado la autorizacion
    /// </summary>
    /// <param name="suministroCentroServicio"></param>
    public void GuardarSuministroCentroServicio(SUSuministroCentroServicioDC suministroCentroServicio)
    {
      SURepositorioAdministracion.Instancia.GuardarSuministroCentroServicio(suministroCentroServicio);
    }

    /// <summary>
    /// Obtener suministros de grupo de un centro de servicio
    /// </summary>
    /// <param name="idGrupo">CSV -PUA -racol</param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosGrupoCentroServicio(string idGrupo, long idCentroServicio)
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministrosGrupoCentroServicio(idGrupo, idCentroServicio);
    }

    /// <summary>
    /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados en el centro de servicio
    /// </summary>
    /// <param name="filtro">codigo erp(novasoft) y descripcion del suministro</param>
    /// <param name="idGrupo">id del grupo</param>
    /// <param name="idCentroServicio">id del centro de servicio</param>
    /// <returns>Lista de suministro</returns>
    public List<SUSuministro> ObtenerSuministrosCentroServicioNoIncluidosEnGrupo(IDictionary<string, string> filtro, string idGrupo, long idCentroServicio)
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministrosCentroServicioNoIncluidosEnGrupo(filtro, idGrupo, idCentroServicio);
    }

    #endregion Aprovisionar suministros de un Centro de servicio
  }
}