using System;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;

namespace CO.Servidor.Comisiones
{
  /// <summary>
  /// Fachada para la administración de comisiones
  /// </summary>
  public class CMAdministradorComisiones
  {
    private static readonly CMAdministradorComisiones instancia = new CMAdministradorComisiones();

    public static CMAdministradorComisiones Instancia
    {
      get { return CMAdministradorComisiones.instancia; }
    }

    #region Comisiones por centros de servicio administrados

    /// <summary>
    /// Obtiene los puntos de una agencia
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista puntos de la agencia</returns>
    public IList<CMCentroServicioAdministrado> ObtenerPuntosDeAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdAgencia)
    {
      return CMConfiguradorComisiones.Instancia.ObtenerPuntosDeAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdAgencia);
    }

    /// <summary>
    /// Obtiene las agencias de un col
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista agencias de un col</returns>
    public IList<CMCentroServicioAdministrado> ObtenerAgenciasDeCol(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCol)
    {
      return CMConfiguradorComisiones.Instancia.ObtenerAgenciasDeCol(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCol);
    }

    /// <summary>
    /// Obtiene los col de un racol
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista col de un racol</returns>
    public IList<CMCentroServicioAdministrado> ObtenerColDeRacol(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdRacol)
    {
      return CMConfiguradorComisiones.Instancia.ObtenerColDeRacol(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdRacol);
    }

    /// <summary>
    /// Obtiene  las comisiones de los servicios asignados a un centro de servicios administrado
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista  de comisiones por servicios de los centros de servicio</returns>
    public IList<CMComisionesServiciosCentroServiciosAdmin> ObtenerComisionesServiciosCentroServiciosAdmin(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServicios)
    {
      return CMConfiguradorComisiones.Instancia.ObtenerComisionesServiciosCentroServiciosAdmin(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios);
    }

    /// <summary>
    /// Obtiene todos los tipos de comision
    /// </summary>
    /// <returns>Lista con los tipos de comision</returns>
    public IList<PUTiposComision> ObtenerTiposComision()
    {
      return CMConfiguradorComisiones.Instancia.ObtenerTiposComision();
    }

    /// <summary>
    /// Guarda o edita comisiones de los servicios por un centro de servicios
    /// </summary>
    /// <param name="comisionesServicios"></param>
    public void ActualizarComisionesServiciosAsignadosCentrosServiciosAdmin(CMComisionesServiciosCentroServiciosAdmin comisionesServicios)
    {
      CMConfiguradorComisiones.Instancia.ActualizarComisionesServiciosAsignadosCentrosServiciosAdmin(comisionesServicios);
    }

    /// <summary>
    /// Obtiene todos los tipos de comision fija
    /// </summary>
    /// <returns>Lista con los tipos de comision fija</returns>
    public IList<PUTipoComisionFija> ObtenerTiposComisionFija()
    {
      return CMConfiguradorComisiones.Instancia.ObtenerTiposComisionFija();
    }

    /// <summary>
    /// Obtiene  los conceptos adicionales (comisiones fijas) de un centro de servicio
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista  de los conceptos adicionales (comisiones fijas) de un centro de servicio</returns>
    public IList<CMComisionesConceptosAdicionales> ObtenerComisionesConceptosAdicionalesCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServicios)
    {
      return CMConfiguradorComisiones.Instancia.ObtenerComisionesConceptosAdicionalesCentroServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios);
    }

    /// <summary>
    /// Inserta, Modifica o Elimina una comision por concepto adicional (comision fija) a un centro de servicio sin un contrato
    /// </summary>
    /// <param name="comisionFija"></param>
    public void ActualizarComisionesConceptosAdicionalesCentroServicio(CMComisionesConceptosAdicionales comisionFija)
    {
      CMConfiguradorComisiones.Instancia.ActualizarComisionesConceptosAdicionalesCentroServicio(comisionFija);
    }

    #endregion Comisiones por centros de servicio administrados

    #region Servicios de Centro servicios

    /// <summary>
    /// Obtiene  los servicios asignados a un centro de servicios junto con sus comisiones y sus descuentos
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista  de comisiones por servicios de los centros de servicio</returns>
    public IList<CMServiciosCentroServicios> ObtenerServiciosCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServicios)
    {
      return CMConfiguradorComisiones.Instancia.ObtenerServiciosCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios);
    }

    /// <summary>
    /// Obtiene  las comisiones y sus descuentos de un servicio asignado a un centro de servicios
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServiciosServicios">Id del servicio asignado a un  centro de servicio</param>
    /// <returns>Objeto servicioCentroServicios con la lista de comisiones y descuentos</returns>
    public CMServiciosCentroServicios ObtenerComiDescServiciosCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServiciosServicios, int idServicio)
    {
      return CMConfiguradorComisiones.Instancia.ObtenerComiDescServiciosCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServiciosServicios, idServicio);
    }

    /// <summary>
    /// Adiciona modifica o elimina un servicios a un centro de servicios junto con los descuentos, comisiones y horarios
    /// </summary>
    /// <param name="servicios">Objeto con la informacion de los servicios</param>
    public void ActualizarServiciosCentroServicios(CMServiciosCentroServicios servicios)
    {
      CMConfiguradorComisiones.Instancia.ActualizarServiciosCentroServicios(servicios);
    }

    #endregion Servicios de Centro servicios
  }
}