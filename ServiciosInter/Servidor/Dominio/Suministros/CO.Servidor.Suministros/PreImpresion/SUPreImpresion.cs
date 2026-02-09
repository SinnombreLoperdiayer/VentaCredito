using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Comun;
using CO.Servidor.Suministros.Datos;
using CO.Servidor.Suministros.PreImpresion.ImplementacionBusqueda;
using CO.Servidor.Suministros.PreImpresion.ImplementacionTipo;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Suministros.PreImpresion
{
  /// <summary>
  /// Clase encargada del manejo de las consultas de la informacion a preimprimir en suministros
  /// </summary>
  internal class SUPreImpresion
  {
    /// <summary>
    /// Obtener las provisiones de un racol si estas tienen rangos consecutivos
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta</param>
    /// <param name="fechaFinal">fecha final de la consulta</param>
    /// <param name="idRacol">Id del RACOL</param>
    /// <param name="idSuministro">id del suministro a consultar</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">Tamaño de la pagina</param>
    /// <returns>Lista con las provisiones de suministro de todos los centros de servicio, mensajeros y agencias</returns>
    internal List<SUImpresionSuministrosDC> ObtenerProvisionesSuministrosPorRACOL(DateTime? fechaInicial, DateTime? fechaFinal, long idRacol, int idSuministro, int pageIndex, int pageSize, bool consultaIncluyeFecha)
    {
      if (!consultaIncluyeFecha && fechaInicial != null && fechaFinal!=null)
      {
        fechaInicial = DateTime.Now.AddYears(-1);
        fechaFinal = DateTime.Now;
      }

      List<SUImpresionSuministrosDC> lstImpresionSuministros = new List<SUImpresionSuministrosDC>();
      SUImpresionSuministrosDC impresionSuministros;
      List<SURango> rangosTodos = new List<SURango>();

      List<PUCentroServiciosDC> lstCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentrosServicios(idRacol);

      lstCentroServicio.ForEach(
        centroServicio =>
        {
          SUImpresionBase impresion = new SUBusqueda();
          impresion = new SUImpresionSumCentroServicio(impresion);
          impresion = new SUImpresionSumMensajero(impresion);
          impresion = new SUImpresionSumSucursal(impresion);
          impresionSuministros = null;        
          impresionSuministros = impresion.ObtenerProvisionesSuministros(fechaInicial, fechaFinal, idRacol, idSuministro, pageIndex, pageSize);          
          impresion.ValidarSuministrosEstenProvisionado();
         
          if (impresionSuministros.ImpresionSumCentroServicio.Count != 0 || impresionSuministros.ImpresionSumMensajero.Count != 0 || impresionSuministros.ImpresionSumSucursal.Count != 0)
            lstImpresionSuministros.Add(impresionSuministros);
          List<SURango> rangoCentroServicio = impresion.ObtenerTodosLosRangos();

          if (rangoCentroServicio != null)
            rangosTodos.AddRange(rangoCentroServicio);
        });

      this.ValidarConsecutivoRangos(rangosTodos);

      return lstImpresionSuministros;
    }


    /// <summary>
    /// Obtener las provisiones de una ciudad destino si estas tienen rangos consecutivos
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta</param>
    /// <param name="fechaFinal">fecha final de la consulta</param>
    /// <param name="idRacol">Id del RACOL</param>
    /// <param name="idSuministro">id del suministro a consultar</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">Tamaño de la pagina</param>
    /// <returns>Lista con las provisiones de suministro de la ciudad destino</returns>
    internal List<SUImpresionSuministrosDC> ObtenerProvisionesSuministrosCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize, bool consultaIncluyeFecha)
    {
      if (!consultaIncluyeFecha && fechaInicial != null && fechaFinal != null)
      {
        fechaInicial = DateTime.Now.AddYears(-1);
        fechaFinal = DateTime.Now;
      }

      List<SUImpresionSuministrosDC> lstImpresionSuministros = new List<SUImpresionSuministrosDC>();
      SUImpresionSuministrosDC impresionSuministros;
      List<SURango> rangosTodos = new List<SURango>();

    
          SUImpresionBase impresion = new SUBusqueda();
          impresion = new SUImpresionSumCentroServicio(impresion);
          impresion = new SUImpresionSumMensajero(impresion);
          impresion = new SUImpresionSumSucursal(impresion);
          impresionSuministros = null;
          impresionSuministros = impresion.ObtenerProvisionesSuministrosCiudadDestino(fechaInicial, fechaFinal, idCiudadDestino, idSuministro, pageIndex, pageSize);
          impresion.ValidarSuministrosEstenProvisionado();

          if (impresionSuministros.ImpresionSumCentroServicio.Count != 0 || impresionSuministros.ImpresionSumMensajero.Count != 0 || impresionSuministros.ImpresionSumSucursal.Count != 0)
            lstImpresionSuministros.Add(impresionSuministros);
          List<SURango> rangoCentroServicio = impresion.ObtenerTodosLosRangos();

          if (rangoCentroServicio != null)
            rangosTodos.AddRange(rangoCentroServicio);
    

      this.ValidarConsecutivoRangos(rangosTodos);

      return lstImpresionSuministros;
    }




    /// <summary>
    /// Obtener las provisiones de un Centro de servicio si estas tienen rangos consecutivos
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta</param>
    /// <param name="fechaFinal">fecha final de la consulta</param>
    /// <param name="idCentroServicio">Id Centro Servicio</param>
    /// <param name="idSuministro">id del suministro a consultar</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">Tamaño de la pagina</param>
    /// <returns>Lista con las provisiones de suministro de todos los centros de servicio, mensajeros y agencias</returns>
    internal SUImpresionSuministrosDC ObtenerProvisionesSuministrosPorCentroServicio(DateTime? fechaInicial, DateTime? fechaFinal, long idCentroServicio, int idSuministro, int pageIndex, int pageSize)
    {
      SUImpresionSuministrosDC impresionSuministros;
      List<SURango> rangosTodos = new List<SURango>();
      SUImpresionBase impresion = new SUBusqueda();
      impresion = new SUImpresionSumCentroServicio(impresion);
      impresion = new SUImpresionSumMensajero(impresion);
      impresion = new SUImpresionSumSucursal(impresion);
      impresionSuministros = impresion.ObtenerProvisionesSuministros(fechaInicial, fechaFinal, idCentroServicio, idSuministro, pageIndex, pageSize);
      impresion.ValidarSuministrosEstenProvisionado();
      List<SURango> rangoCentroServicio = impresion.ObtenerTodosLosRangos();
      this.ValidarConsecutivoRangos(rangoCentroServicio);
      return impresionSuministros;
    }

    /// <summary>
    /// Obtiene las provisiones de suministros en los rangos ingresados por parametros
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresión los suministro</returns>
    internal SUImpresionSuministrosDC ObtenerProvisionSuministroPorRango(SUFiltroSuministroPorRangoDC filtroPorRango)
    {

      if(!filtroPorRango.ConsultaIncluyeFecha)
      {
        filtroPorRango.FechaInicial = DateTime.Now.AddYears(-1);
        filtroPorRango.FechaFinal = DateTime.Now;
      }

      SUImpresionSuministrosDC impresionSuministros;
      List<SURango> rangosTodos = new List<SURango>();
      SUImpresionBase impresion = new SUBusqueda();
      impresion = new SUImpresionSumCentroServicio(impresion);
      impresion = new SUImpresionSumMensajero(impresion);
      impresion = new SUImpresionSumSucursal(impresion);
      impresion = new SUImpresionSumGestion(impresion);
      impresionSuministros = impresion.ObtenerProvisionSuministroPorRango(filtroPorRango);
      impresion.ValidarSuministrosEstenProvisionado();
      List<SURango> rangoCentroServicio = impresion.ObtenerTodosLosRangos();
      this.ValidarConsecutivoRangos(rangoCentroServicio);
      return impresionSuministros;
    }




    /// <summary>
    /// Obtiene las provisiones de suministros realizados por un usuario
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresión los suministro</returns>
    internal SUImpresionSuministrosDC ObtenerProvisionSuministroPorUsuario(SUFiltroSuministroPorUsuarioDC filtroPorUsuario)
    {

      if (!filtroPorUsuario.ConsultaIncluyeFecha)
      {
        filtroPorUsuario.FechaInicial = DateTime.Now.AddYears(-1);
        filtroPorUsuario.FechaFinal = DateTime.Now;
      }

  

      SUImpresionSuministrosDC impresionSuministros;
      List<SURango> rangosTodos = new List<SURango>();
      SUImpresionBase impresion = new SUBusqueda();
      impresion = new SUImpresionSumCentroServicio(impresion);
      impresion = new SUImpresionSumMensajero(impresion);
      impresion = new SUImpresionSumSucursal(impresion);
      impresion = new SUImpresionSumGestion(impresion);
      impresionSuministros = impresion.ObtenerProvisionSuministroPorUsuario(filtroPorUsuario);
      impresion.ValidarSuministrosEstenProvisionado();
      List<SURango> rangoCentroServicio = impresion.ObtenerTodosLosRangos();
      this.ValidarConsecutivoRangos(rangoCentroServicio);
      return impresionSuministros;
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
    internal SUImpresionSuministrosDC ObtenerProvisionSuministroPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {

      if (!filtroPorRemision.ConsultaIncluyeFecha)
      {
        filtroPorRemision.FechaInicial = DateTime.Now.AddYears(-1);
        filtroPorRemision.FechaFinal = DateTime.Now;
      }


  
      SUImpresionSuministrosDC impresionSuministros;
      List<SURango> rangosTodos = new List<SURango>();
      SUImpresionBase impresion = new SUBusqueda();
      impresion = new SUImpresionSumCentroServicio(impresion);
      impresion = new SUImpresionSumMensajero(impresion);
      impresion = new SUImpresionSumSucursal(impresion);
      impresion = new SUImpresionSumGestion(impresion);
      impresionSuministros = impresion.ObtenerProvisionSuministroPorRemision(filtroPorRemision);
      impresion.ValidarSuministrosEstenProvisionado();
      List<SURango> rangoCentroServicio = impresion.ObtenerTodosLosRangos();
      this.ValidarConsecutivoRangos(rangoCentroServicio);
      return impresionSuministros;
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
    public List<SURemisionSuministroDC> ObtenerRemisionesGuiasInternas(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {
      return SURepositorio.Instancia.ObtenerRemisionesGuiasInternas(filtroPorRemision);
    }

    /// <summary>
    /// Valida que sean consecutivos los rangos cuando se unan entre  los suministros eje: (mensajeria -centro servicio - sucursal)
    /// </summary>
    /// <param name="rangos"> rangos a evaluar</param>
    /// <returns></returns>
    private void ValidarConsecutivoRangos(List<SURango> rangos)
    {
      rangos = rangos.OrderBy(r => r.Inicio).ToList();

      for (int i = 0; i + 1 < rangos.Count; i++)
      {
        if (rangos[i].Fin + 1 != rangos[i + 1].Inicio)
        {
          throw new FaultException<ControllerException>(
           new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
             EnumTipoErrorSuministros.EX_ERROR_RANGOS_NO_CONSECUTIVO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_RANGOS_NO_CONSECUTIVO)));
        }
      }
    }
  }
}