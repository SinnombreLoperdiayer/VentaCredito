using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Comun;
using CO.Servidor.Suministros.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Suministros.PreImpresion.ImplementacionTipo
{
  public class SUImpresionSumMensajero : SUImpresionTipo
  {
    /// <summary>
    /// Objecto impresion Base este se va construyendo recursivamente
    /// </summary>
    SUImpresionBase impresionBase;

    List<SUImpresionSumMensajeroDC> ImpresionSumMensajero;

    public SUImpresionSumMensajero(SUImpresionBase impresion)
    {
      this.impresionBase = impresion;
    }

    /// <summary>
    /// Obtener las provisiones de un mensajero si estas tienen rangos consecutivos
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta</param>
    /// <param name="fechaFinal">fecha final de la consulta</param>
    /// <param name="idCentroServicio">Id centro de servicio</param>
    /// <param name="idSuministro">id del suministro a consultar</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">Tamaño de la pagina</param>
    /// <returns></returns>
    public override SUImpresionSuministrosDC ObtenerProvisionesSuministros(DateTime? fechaInicial, DateTime? fechaFinal, long idCentroServicio, int idSuministro, int pageIndex, int pageSize)
    {
      SUImpresionSuministrosDC suministroPadre = impresionBase.ObtenerProvisionesSuministros(fechaInicial, fechaFinal, idCentroServicio, idSuministro, pageIndex, pageSize);
      suministroPadre.ImpresionSumMensajero = SURepositorioAdministracion.Instancia.ObtenerProvisionesSuministroMensajero(fechaInicial, fechaFinal, idCentroServicio, idSuministro, pageIndex, pageSize);
      ImpresionSumMensajero = suministroPadre.ImpresionSumMensajero;
      return suministroPadre;
    }

    /// <summary>
    /// Obtener las provisiones de un mensajero si estas tienen rangos consecutivos por ciudad de destino
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta</param>
    /// <param name="fechaFinal">fecha final de la consulta</param>
    /// <param name="idCentroServicio">Id centro de servicio</param>
    /// <param name="idSuministro">id del suministro a consultar</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">Tamaño de la pagina</param>
    /// <returns></returns>
    public override SUImpresionSuministrosDC ObtenerProvisionesSuministrosCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize)
    {
      SUImpresionSuministrosDC suministroPadre = impresionBase.ObtenerProvisionesSuministrosCiudadDestino(fechaInicial, fechaFinal, idCiudadDestino, idSuministro, pageIndex, pageSize);
      suministroPadre.ImpresionSumMensajero = SURepositorioAdministracion.Instancia.ObtenerProvisionesSuministroMensajeroCiudadDestino(fechaInicial, fechaFinal, idCiudadDestino, idSuministro, pageIndex, pageSize);
      ImpresionSumMensajero = suministroPadre.ImpresionSumMensajero;
      return suministroPadre;
    }

    

    /// <summary>
    /// Obtiene las provisiones de suministros de los mensajero en los rangos ingresados por parametros
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
    public override SUImpresionSuministrosDC ObtenerProvisionSuministroPorRango(SUFiltroSuministroPorRangoDC filtroPorRango)
    {
      SUImpresionSuministrosDC suministroPadre = impresionBase.ObtenerProvisionSuministroPorRango(filtroPorRango);
      suministroPadre.ImpresionSumMensajero = SURepositorioAdministracion.Instancia.ObtenerProvisionSumMensajeroPorRango(filtroPorRango);
      ImpresionSumMensajero = suministroPadre.ImpresionSumMensajero;
      return suministroPadre;
    }

    /// <summary>
    /// Obtiene las provisiones de suministros de los mensajero realizado por un usuario
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
    public override SUImpresionSuministrosDC ObtenerProvisionSuministroPorUsuario(SUFiltroSuministroPorUsuarioDC filtroPorUsuario)
    {
      SUImpresionSuministrosDC suministroPadre = impresionBase.ObtenerProvisionSuministroPorUsuario(filtroPorUsuario);
      suministroPadre.ImpresionSumMensajero = SURepositorioAdministracion.Instancia.ObtenerProvisionSumMensajeroPorUsuario(filtroPorUsuario);
      ImpresionSumMensajero = suministroPadre.ImpresionSumMensajero;
      return suministroPadre;
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro por el numero de remision
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="usuario">Usuario que realizo la remision</param>
    /// <param name="remisionInicial">numero de la remision inicial</param>
    /// <param name="remisionFinal">numero de la remision final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de los mensajeros</returns>
    public override SUImpresionSuministrosDC ObtenerProvisionSuministroPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {
      SUImpresionSuministrosDC suministroPadre = impresionBase.ObtenerProvisionSuministroPorRemision(filtroPorRemision);
      suministroPadre.ImpresionSumMensajero = SURepositorioAdministracion.Instancia.ObtenerProvisionesSumMensajeroPorRemision(filtroPorRemision);
      ImpresionSumMensajero = suministroPadre.ImpresionSumMensajero;
      return suministroPadre;
    }

    /// <summary>
    /// Valida que los suministros se encuentren Provisionado en la tabla de suministros provision referencia
    /// </summary>
    public override void ValidarSuministrosEstenProvisionado()
    {
      if (ImpresionSumMensajero == null)
        return;

      foreach (SUImpresionSumMensajeroDC impresionSumMensajero in ImpresionSumMensajero)
      {
        int cantidadSumMensajero = SURepositorioAdministracion.Instancia.ObtenerCantidadSuministroProvisionReferencia(impresionSumMensajero.Rango.Inicio, impresionSumMensajero.Rango.Fin);

        if (cantidadSumMensajero != (impresionSumMensajero.Rango.Fin - impresionSumMensajero.Rango.Inicio) + 1)
        {
          throw new FaultException<ControllerException>(
            new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
              EnumTipoErrorSuministros.EX_ERROR_SUMINISTROS_NO_APROVISIONADO_MENSAJERO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_SUMINISTROS_NO_APROVISIONADO_MENSAJERO)));
        }
      }
    }

    /// <summary>
    /// Obtiene todos los rangos de los suministros asignados a todos los mensajeros mas los que se asignaron recursivamente
    /// </summary>
    /// <returns></returns>
    public override List<SURango> ObtenerTodosLosRangos()
    {
      if (ImpresionSumMensajero == null)
        return null;

      //adiciona los rangos
      List<SURango> lstRangosPadre = impresionBase.ObtenerTodosLosRangos();
      List<SURango> lstRangosActual = ImpresionSumMensajero.Select(impMen => impMen.Rango).ToList();

      if (lstRangosPadre == null)
        return lstRangosActual;

      if (lstRangosActual != null)
        lstRangosPadre.AddRange(lstRangosActual);

      return lstRangosPadre;
    }
  }
}