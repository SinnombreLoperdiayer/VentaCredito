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
  public class SUImpresionSumCentroServicio : SUImpresionTipo
  {
    /// <summary>
    /// Objecto impresion Base este se va construyendo recursivamente
    /// </summary>
    SUImpresionBase impresionBase;

    List<SUImpresionSumCentroServicioDC> ImpresionSumCentroServicio;

    public SUImpresionSumCentroServicio(SUImpresionBase impresion)
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
      suministroPadre.ImpresionSumCentroServicio = SURepositorioAdministracion.Instancia.ObtenerProvisionesSuministroCentroServicio(fechaInicial, fechaFinal, idCentroServicio, idSuministro, pageIndex, pageSize);
      ImpresionSumCentroServicio = suministroPadre.ImpresionSumCentroServicio;
      return suministroPadre;
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
    
    public override SUImpresionSuministrosDC ObtenerProvisionesSuministrosCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize)
    {
      SUImpresionSuministrosDC suministroPadre = impresionBase.ObtenerProvisionesSuministrosCiudadDestino(fechaInicial, fechaFinal, idCiudadDestino, idSuministro, pageIndex, pageSize);
      suministroPadre.ImpresionSumCentroServicio = SURepositorioAdministracion.Instancia.ObtenerProvisionesSuministroCentroServicioCiudadDestino(fechaInicial, fechaFinal, idCiudadDestino, idSuministro, pageIndex, pageSize);
      ImpresionSumCentroServicio = suministroPadre.ImpresionSumCentroServicio;
      return suministroPadre;
    }
    


    /// <summary>
    /// Obtiene las provisiones de suministros de un centro de servicio en los rangos ingresados por parametros
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
      suministroPadre.ImpresionSumCentroServicio = SURepositorioAdministracion.Instancia.ObtenerProvisionSuministroCentroServicioPorRango(filtroPorRango);
      ImpresionSumCentroServicio = suministroPadre.ImpresionSumCentroServicio;
      return suministroPadre;
    }


    /// <summary>
    /// Obtiene las provisiones de suministros de un centro de servicio realizado por un usuario
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
      suministroPadre.ImpresionSumCentroServicio = SURepositorioAdministracion.Instancia.ObtenerProvisionSuministroCentroServicioPorUsuario(filtroPorUsuario);
      ImpresionSumCentroServicio = suministroPadre.ImpresionSumCentroServicio;
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
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
    public override SUImpresionSuministrosDC ObtenerProvisionSuministroPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {
      SUImpresionSuministrosDC suministroPadre = impresionBase.ObtenerProvisionSuministroPorRemision(filtroPorRemision);
      suministroPadre.ImpresionSumCentroServicio = SURepositorioAdministracion.Instancia.ObtenerProvisionesSumCentroServicioPorRemision(filtroPorRemision);
      ImpresionSumCentroServicio = suministroPadre.ImpresionSumCentroServicio;
      return suministroPadre;
    }

    /// <summary>
    /// Valida que los suministros se encuentren Provisionado en la tabla de suministros provision referencia
    /// </summary>
    public override void ValidarSuministrosEstenProvisionado()
    {
      if (ImpresionSumCentroServicio == null)
        return;

      foreach (SUImpresionSumCentroServicioDC impresionSumCentroServicio in ImpresionSumCentroServicio)
      {
        int cantidadSumCentroServicio = SURepositorioAdministracion.Instancia.ObtenerCantidadSuministroProvisionReferencia(impresionSumCentroServicio.Rango.Inicio, impresionSumCentroServicio.Rango.Fin);

        if (cantidadSumCentroServicio != (impresionSumCentroServicio.Rango.Fin - impresionSumCentroServicio.Rango.Inicio) + 1)
        {
          throw new FaultException<ControllerException>(
                 new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
                   EnumTipoErrorSuministros.EX_ERROR_SUMINISTROS_NO_APROVISIONADO_CENTRO_SERVICIO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_SUMINISTROS_NO_APROVISIONADO_CENTRO_SERVICIO)));
        }
      }
    }

    /// <summary>
    /// Obtiene todos los rangos de los suministros asignados a todos al centro de servicio mas los que se asignaron recursivamente
    /// </summary>
    /// <returns></returns>
    public override List<SURango> ObtenerTodosLosRangos()
    {
      if (ImpresionSumCentroServicio == null)
        return null;

      //adiciona los rangos
      List<SURango> lstRangosPadre = impresionBase.ObtenerTodosLosRangos();
      List<SURango> lstRangosActual = ImpresionSumCentroServicio.Select(impMen => impMen.Rango).ToList();

      if (lstRangosPadre == null)
        return lstRangosActual;

      if (lstRangosActual != null)
        lstRangosPadre.AddRange(lstRangosActual);

      return lstRangosPadre;
    }
  }
}