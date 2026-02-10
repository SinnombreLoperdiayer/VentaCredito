using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Suministros;

namespace CO.Servidor.Suministros.PreImpresion
{
  public abstract class SUImpresionBase  
  {
    /// <summary>
    /// Obtener las provisiones de suministros de un centro de servicio
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta</param>
    /// <param name="fechaFinal">fecha final de la consulta</param>
    /// <param name="idCentroServicio">Id centro de servicio</param>
    /// <param name="idSuministro">id del suministro a consultar</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">Tamaño de la pagina</param>
    /// <returns></returns>
    public abstract SUImpresionSuministrosDC ObtenerProvisionesSuministros(DateTime? fechaInicial, DateTime? fechaFinal, long idCentroServicio, int idSuministro, int pageIndex, int pageSize);


    /// <summary>
    /// Obtener las provisiones de suministros de un centro de servicio por Ciudad Destino
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta</param>
    /// <param name="fechaFinal">fecha final de la consulta</param>
    /// <param name="idCentroServicio">Id centro de servicio</param>
    /// <param name="idSuministro">id del suministro a consultar</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">Tamaño de la pagina</param>
    /// <returns></returns>
    public abstract SUImpresionSuministrosDC ObtenerProvisionesSuministrosCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize);


    /// <summary>
    /// Obtiene las provisiones de suministros en los rangos ingresados por parametros
    /// en un rango de fechas
    /// </summary>
    /// <param name="filtroPorRango">filtro por rango</param>
    /// <returns>Lista con la informacion para impresión los suministro</returns>
    public abstract SUImpresionSuministrosDC ObtenerProvisionSuministroPorRango(SUFiltroSuministroPorRangoDC filtroPorRango);


    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro por el numero de remision
    /// </summary>
    /// <param param name="filtroPorRemision">Filtro para la busqueda</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>    
    public abstract SUImpresionSuministrosDC ObtenerProvisionSuministroPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision);
    
    /// <summary>
    /// Valida que los suministros se encuentren Provisionado en la tabla de suministros provision referencia 
    /// </summary>
    public abstract void ValidarSuministrosEstenProvisionado();

    /// <summary>
    /// Obtiene todos los rangos de los suministros asignados a todos los mensajeros
    /// </summary>
    /// <returns></returns>
    public abstract List<SURango> ObtenerTodosLosRangos();

    /// <summary>
    /// Obtiene las provisiones de suministros realizadas por un Usuario
    /// en un rango de fechas
    /// </summary>
    /// <param name="filtroPorUsuario">filtro por Usuario</param>
    /// <returns>Lista con la informacion para impresión los suministro</returns>
    public abstract SUImpresionSuministrosDC ObtenerProvisionSuministroPorUsuario(SUFiltroSuministroPorUsuarioDC filtroPorUsuario);

  }
}
