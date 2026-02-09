using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using CO.Servidor.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.Implementacion.Comisiones
{
  /// <summary>
  /// Clase para los servicios de administración de Clientes
  /// </summary>
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class CMComisionesSvc : ICMComisionesSvc
  {
    /// <summary>
    /// Obtiene los puntos de una agencia
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista puntos de la agencia</returns>
    public GenericoConsultasFramework<CMCentroServicioAdministrado> ObtenerPuntosDeAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdAgencia)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CMCentroServicioAdministrado>()
      {
        Lista = CMAdministradorComisiones.Instancia.ObtenerPuntosDeAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdAgencia),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Obtiene las agencias de un col
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista agencias de un col</returns>
    public GenericoConsultasFramework<CMCentroServicioAdministrado> ObtenerAgenciasDeCol(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdCol)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CMCentroServicioAdministrado>()
      {
        Lista = CMAdministradorComisiones.Instancia.ObtenerAgenciasDeCol(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCol),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Obtiene los col de un racol
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista col de un racol</returns>
    public GenericoConsultasFramework<CMCentroServicioAdministrado> ObtenerColDeRacol(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdRacol)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CMCentroServicioAdministrado>()
      {
        Lista = CMAdministradorComisiones.Instancia.ObtenerColDeRacol(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdRacol),
        TotalRegistros = totalRegistros
      };
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
    public GenericoConsultasFramework<CMComisionesServiciosCentroServiciosAdmin> ObtenerComisionesServiciosCentroServiciosAdmin(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdCentroServicios)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CMComisionesServiciosCentroServiciosAdmin>()
      {
        Lista = CMAdministradorComisiones.Instancia.ObtenerComisionesServiciosCentroServiciosAdmin(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Obtiene todos los tipos de comision
    /// </summary>
    /// <returns>Lista con los tipos de comision</returns>
    public IList<PUTiposComision> ObtenerTiposComision()
    {
      return CMAdministradorComisiones.Instancia.ObtenerTiposComision();
    }

    /// <summary>
    /// Guarda o edita comisiones de los servicios por un centro de servicios Administrados
    /// </summary>
    /// <param name="comisionesServicios"></param>
    public void ActualizarComisionesServiciosAsignadosCentrosServiciosAdmin(CMComisionesServiciosCentroServiciosAdmin comisionesServicios)
    {
      CMAdministradorComisiones.Instancia.ActualizarComisionesServiciosAsignadosCentrosServiciosAdmin(comisionesServicios);
    }

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
    public GenericoConsultasFramework<CMServiciosCentroServicios> ObtenerServiciosCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdCentroServicios)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CMServiciosCentroServicios>()
      {
        Lista = CMAdministradorComisiones.Instancia.ObtenerServiciosCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Obtiene todos los tipos de comision fija
    /// </summary>
    /// <returns>Lista con los tipos de comision fija</returns>
    public IList<PUTipoComisionFija> ObtenerTiposComisionFija()
    {
      return CMAdministradorComisiones.Instancia.ObtenerTiposComisionFija();
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
    public GenericoConsultasFramework<CMComisionesConceptosAdicionales> ObtenerComisionesConceptosAdicionalesCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdCentroServicios)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CMComisionesConceptosAdicionales>()
      {
        Lista = CMAdministradorComisiones.Instancia.ObtenerComisionesConceptosAdicionalesCentroServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Inserta, Modifica o Elimina una comision por concepto adicional (comision fija) a un centro de servicio sin un contrato
    /// </summary>
    /// <param name="comisionFija"></param>
    public void ActualizarComisionesConceptosAdicionalesCentroServicio(CMComisionesConceptosAdicionales comisionFija)
    {
      CMAdministradorComisiones.Instancia.ActualizarComisionesConceptosAdicionalesCentroServicio(comisionFija);
    }

    /// <summary>
    /// Adiciona modifica o elimina un servicios a un centro de servicios junto con los descuentos, comisiones y horarios
    /// </summary>
    /// <param name="servicios">Objeto con la informacion de los servicios</param>
    public void ActualizarServiciosCentroServicios(CMServiciosCentroServicios servicios)
    {
      CMAdministradorComisiones.Instancia.ActualizarServiciosCentroServicios(servicios);
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
      return CMAdministradorComisiones.Instancia.ObtenerComiDescServiciosCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServiciosServicios, idServicio);
    }
  }
}