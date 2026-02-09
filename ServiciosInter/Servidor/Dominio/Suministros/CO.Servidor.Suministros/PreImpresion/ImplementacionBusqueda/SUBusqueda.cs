using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Suministros;

namespace CO.Servidor.Suministros.PreImpresion.ImplementacionBusqueda
{
  class SUBusqueda : SUImpresionBase
  {

    /// <summary>
    /// Crear el objeto donde se almacenaran todos los suministros
    /// </summary>
    /// <param name="fechaInicial"></param>
    /// <param name="fechaFinal"></param>
    /// <param name="idCentroServicio"></param>
    /// <param name="idSuministro"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public override SUImpresionSuministrosDC ObtenerProvisionesSuministros(DateTime? fechaInicial, DateTime? fechaFinal, long idCentroServicio, int idSuministro, int pageIndex, int pageSize)
    {
      return new SUImpresionSuministrosDC();
    }

    public override List<Servicios.ContratoDatos.Suministros.SURango> ObtenerTodosLosRangos()
    {
      return null;
    }

    public override void ValidarSuministrosEstenProvisionado() { }
    
    /// <summary>
    /// Crea el objeto donde se almacenaran todos los suministros filtrados por rango
    /// </summary>
    /// <param name="fechaInicial"></param>
    /// <param name="fechaFinal"></param>
    /// <param name="rangoInicial"></param>
    /// <param name="rangoFinal"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public override SUImpresionSuministrosDC ObtenerProvisionSuministroPorRango(SUFiltroSuministroPorRangoDC filtroPorRango)
    {
      return new SUImpresionSuministrosDC();
    }

    /// <summary>
    /// Crea el objeto donde se almacenaran todos los suministros filtrados por rango
    /// </summary>
    /// <param name="fechaInicial"></param>
    /// <param name="fechaFinal"></param>
    /// <param name="rangoInicial"></param>
    /// <param name="rangoFinal"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public override SUImpresionSuministrosDC ObtenerProvisionSuministroPorUsuario(SUFiltroSuministroPorUsuarioDC filtroPorUsuario)
    {
      return new SUImpresionSuministrosDC();
    }

    /// <summary>
    /// Crea el objeto donde se almacenaran todos los suministros filtrados por remision
    /// </summary>
    /// <param name="fechaInicial"></param>
    /// <param name="fechaFinal"></param>
    /// <param name="usuario"></param>
    /// <param name="remisionInicial"></param>
    /// <param name="remisionFinal"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public override SUImpresionSuministrosDC ObtenerProvisionSuministroPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {
      return new SUImpresionSuministrosDC();
    }

    /// <summary>
    /// Crea el objeto donde se almacenaran todos los suministros filtrados por ciudad Destino
    /// </summary>
    /// <param name="fechaInicial"></param>
    /// <param name="fechaFinal"></param>
    /// <param name="usuario"></param>
    /// <param name="remisionInicial"></param>
    /// <param name="remisionFinal"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public override SUImpresionSuministrosDC ObtenerProvisionesSuministrosCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize)
    {
      return new SUImpresionSuministrosDC();
    }
  }
}
