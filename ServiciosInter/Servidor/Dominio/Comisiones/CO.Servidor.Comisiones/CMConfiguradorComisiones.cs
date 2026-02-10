using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Comisiones.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Comisiones
{
  /// <summary>
  /// Clase de dominio para la configuracion de comisiones
  /// </summary>
  public class CMConfiguradorComisiones : ControllerBase
  {
    private static readonly CMConfiguradorComisiones instancia = (CMConfiguradorComisiones)FabricaInterceptores.GetProxy(new CMConfiguradorComisiones(), COConstantesModulos.COMISIONES);

    public static CMConfiguradorComisiones Instancia
    {
      get { return CMConfiguradorComisiones.instancia; }
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
      return CMRepositorio.Instancia.ObtenerPuntosDeAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdAgencia);
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
      return CMRepositorio.Instancia.ObtenerAgenciasDeCol(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCol);
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
      return CMRepositorio.Instancia.ObtenerColDeRacol(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdRacol);
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
      return CMRepositorio.Instancia.ObtenerComisionesServiciosCentroServiciosAdmin(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios);
    }

    /// <summary>
    /// Obtiene todos los tipos de comision
    /// </summary>
    /// <returns>Lista con los tipos de comision</returns>
    public IList<PUTiposComision> ObtenerTiposComision()
    {
      return CMRepositorio.Instancia.ObtenerTiposComision();
    }

    /// <summary>
    /// Obtiene todos los tipos de comision fija
    /// </summary>
    /// <returns>Lista con los tipos de comision fija</returns>
    public IList<PUTipoComisionFija> ObtenerTiposComisionFija()
    {
      return CMRepositorio.Instancia.ObtenerTiposComisionFija();
    }

    /// <summary>
    /// Guarda o edita comisiones de los servicios por un centro de servicios
    /// </summary>
    /// <param name="comisionesServicios"></param>
    public void ActualizarComisionesServiciosAsignadosCentrosServiciosAdmin(CMComisionesServiciosCentroServiciosAdmin comisionesServicios)
    {
      CMRepositorio.Instancia.ActualizarComisionesServiciosAsignadosCentrosServiciosAdmin(comisionesServicios);
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
      IList<CMComisionesConceptosAdicionales> ComSinContrato = null;

      if (filtro.Count <= 0 || filtro.ContainsKey("CSD_Descripcion"))
        ComSinContrato = CMRepositorio.Instancia.ObtenerConceptosAdicionalesCentroServicioNoContrato(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios);

      ///Cambia el filtro para adicionarle el nombre de la columna correcto para la tabla centroSrvComiFijaContrato
      if (filtro.ContainsKey("CSD_Descripcion"))
      {
        string f = filtro["CSD_Descripcion"];
        filtro.Remove("CSD_Descripcion");
        filtro.Add("CCC_Descripcion", f);
      }

      IList<CMComisionesConceptosAdicionales> ComConContrato = CMRepositorio.Instancia.ObtenerConceptosAdicionalesCentroServicioConContrato(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios);
      if (ComSinContrato != null)
      {
        IList<CMComisionesConceptosAdicionales> ComFijas = ComConContrato.Union<CMComisionesConceptosAdicionales>(ComSinContrato).ToList();
        return ComFijas;
      }
      else
        return ComConContrato;
    }

    /// <summary>
    /// Inserta, Modifica o Elimina una comision por concepto adicional (comision fija) a un centro de servicio sin un contrato
    /// </summary>
    /// <param name="comisionFija"></param>
    public void ActualizarComisionesConceptosAdicionalesCentroServicio(CMComisionesConceptosAdicionales comisionFija)
    {
      ///verificar cuando se modifica, que si tiene contrato revisar en la tabla sin contrato si existe se elimina

      switch (comisionFija.EstadoRegistro)
      {
        case EnumEstadoRegistro.ADICIONADO:
          if (comisionFija.ConContrato)
            CMRepositorio.Instancia.AdicionarConceptoAdicionalConContrato(comisionFija);

          else
            CMRepositorio.Instancia.AdicionarConceptoAdicionalSinContrato(comisionFija);

          break;
        case EnumEstadoRegistro.MODIFICADO:
          if (CMRepositorio.Instancia.VerificarConceptoAdicionalConContrato(comisionFija))
          {
            if (comisionFija.IdContrato > 0)
              CMRepositorio.Instancia.EditarConceptoAdicionalConContrato(comisionFija);
            else
            {
              using (TransactionScope transaccion = new TransactionScope())
              {
                CMRepositorio.Instancia.EliminarConceptoAdicionalConContrato(comisionFija);
                CMRepositorio.Instancia.AdicionarConceptoAdicionalSinContrato(comisionFija);
                transaccion.Complete();
              };
            }
          }
          else if (CMRepositorio.Instancia.VerificarConceptoAdicionalSinContrato(comisionFija))
          {
            if (comisionFija.IdContrato <= 0)
              CMRepositorio.Instancia.EditarConceptoAdicionalSinContrato(comisionFija);
            else
            {
              using (TransactionScope transaccion = new TransactionScope())
              {
                CMRepositorio.Instancia.EliminarConceptoAdicionalSinContrato(comisionFija);
                CMRepositorio.Instancia.AdicionarConceptoAdicionalConContrato(comisionFija);
                transaccion.Complete();
              };
            }
          }
          else
          {
            ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
            throw new FaultException<ControllerException>(excepcion);
          }

          break;
        case EnumEstadoRegistro.BORRADO:
          if (comisionFija.ConContrato)
            CMRepositorio.Instancia.EliminarConceptoAdicionalConContrato(comisionFija);
          else
            CMRepositorio.Instancia.EliminarConceptoAdicionalSinContrato(comisionFija);

          break;
      }
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
      return CMRepositorio.Instancia.ObtenerServiciosCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios);
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
      CMServiciosCentroServicios centroSer = new CMServiciosCentroServicios()
      {
        ComisionesServicios = CMRepositorio.Instancia.ObtenerComisionesServiciosCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServiciosServicios, idServicio),
        DescuentosServicios = CMRepositorio.Instancia.ObtenerDescuentosServiciosCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServiciosServicios, idServicio),
        IdCentroServiciosServicio = IdCentroServiciosServicios
      };
      return centroSer;
    }

    /// <summary>
    /// Adiciona modifica o elimina un servicios a un centro de servicios junto con los descuentos, comisiones y horarios
    /// </summary>
    /// <param name="servicios">Objeto con la informacion de los servicios</param>
    public void ActualizarServiciosCentroServicios(CMServiciosCentroServicios servicios)
    {
      switch (servicios.EstadoRegistro)
      {
        case EnumEstadoRegistro.ADICIONADO:
          CMRepositorio.Instancia.AdicionarServiciosCentroServicios(servicios);
          break;
        case EnumEstadoRegistro.MODIFICADO:
          CMRepositorio.Instancia.EditarServiciosCentroServicios(servicios);
          break;
        case EnumEstadoRegistro.BORRADO:
          CMRepositorio.Instancia.DesactivarServiciosCentroServicios(servicios);
          break;
      }
    }

    /// <summary>
    /// Obtener las comisiones fijas de un centro servicios activas
    /// </summary>
    /// <param name="fechaCorte"></param>
    /// <param name="idCentroServicios"></param>
    /// <returns></returns>
    public List<CMComisionesConceptosAdicionales> ObtenerComisionesFijasCentroSvcContrato(long idCentroServicios)
    {
      ///Obtiene las comisiones fijas de un contrato
      List<CMComisionesConceptosAdicionales> comisionesContrato = CMRepositorio.Instancia.ObtenerComisionesFijasCentroSvcContrato(idCentroServicios);

      ////Obtiene las comisiones fijas de un centro servicios
      List<CMComisionesConceptosAdicionales> comisiones = CMRepositorio.Instancia.ObtenerComisionesFijasCentroSvc(idCentroServicios);
      foreach (CMComisionesConceptosAdicionales comision in comisiones)
      {
        comisionesContrato.Add(comision);
      }

      return comisionesContrato;
    }

    #endregion Servicios de Centro servicios
  }
}