using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace Framework.Servidor.ParametrosFW
{
  internal class PAZona : ControllerBase
  {
    internal static readonly Framework.Servidor.ParametrosFW.PAZona Instancia = (Framework.Servidor.ParametrosFW.PAZona)FabricaInterceptores.GetProxy(new Framework.Servidor.ParametrosFW.PAZona(), ConstantesFramework.PARAMETROS_FRAMEWORK);

    private PAZona()
    { }

    /// <summary>
    /// Consulta todas las zonas
    /// </summary>
    /// <returns></returns>
    public List<Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC> ObtenerZonas(System.Collections.Generic.IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return PARepositorio.Instancia.ConsultarZonas(filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
    }

    /// <summary>
    /// Consulta las localidades en zona por el id de zona
    /// </summary>
    /// <param name="IdZona"></param>
    /// <returns></returns>
    public List<Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC> ConsultarLocalidadEnZonaXZona(string idZona, IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return PARepositorio.Instancia.ConsultarLocalidadEnZonaXZona(idZona, filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
    }

    /// <summary>
    /// Inserta una nueva zona
    /// </summary>
    /// <param name="zona"></param>
    public void InsertarZona(Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC zona)
    {
      PARepositorio.Instancia.InsertarZona(zona);
    }

    /// <summary>
    /// Inserta una nueva localidad perteneciente a una zona
    /// </summary>
    /// <param name="localidadEnZona"></param>
    public void GuardarCambiosLocalidadEnZona(List<PAZonaLocalidad> localidadesEnZona)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        localidadesEnZona.Where(obj => obj.Localidad.EstadoRegistro == EnumEstadoRegistro.BORRADO).ToList().ForEach(obj => PARepositorio.Instancia.EliminarLocalidadEnZona(obj.Localidad.IdLocalidad));
        localidadesEnZona.Where(obj => obj.Localidad.EstadoRegistro == EnumEstadoRegistro.ADICIONADO).ToList().ForEach(obj =>
         {
           if (PARepositorio.Instancia.VerificarDisponibilidadLocalidadEnZona(obj.Localidad.IdLocalidad))
             PARepositorio.Instancia.InsertaLocalidadEnZona(obj);
         });

        transaccion.Complete();
      }
    }

    /// <summary>
    /// Modifica una zona
    /// </summary>
    /// <param name="zona"></param>
    public void ModificarZona(Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC zona)
    {
      PARepositorio.Instancia.ModificarZona(zona);
    }

    /// <summary>
    /// Consulta los tipos de zona
    /// </summary>
    /// <returns>Lista con los tipos de zona</returns>
    public List<PATipoZona> ConsultarTipoZona()
    {
      return PARepositorio.Instancia.ConsultarTipoZona();
    }

    /// <summary>
    /// Elimina la informacion de una zona y sus relaciones
    /// </summary>
    /// <param name="IdZona"></param>
    public void EliminarZona(string idZona)
    {
      PARepositorio.Instancia.EliminarZona(idZona);
    }

    /// <summary>
    /// Consulta las zonas de una localidad
    /// </summary>
    /// <param name="idLocalidad"></param>
    /// <returns>lista de zonas</returns>
    public IList<PAZonaDC> ConsultarZonasDeLocalidadXLocalidad(string idLocalidad)
    {
      return PARepositorio.Instancia.ConsultarZonasDeLocalidadXLocalidad(idLocalidad);
    }

    /// <summary>
    /// Consulta la zona a la que está asociada una localidad
    /// </summary>
    /// <param name="idLocalidad"></param>
    /// <returns></returns>
    public PAZonaDC ConsultarZonaDeLocalidad(string idLocalidad)
    {
      return PARepositorio.Instancia.ConsultarZonaDeLocalidad(idLocalidad);
    }

    /// <summary>
    /// Obtiene la Zona y el tipo de Zona
    /// correspondiente
    /// </summary>
    /// <returns>lista de zonas y su tipo</returns>
    public List<PAZonaDC> ObtenerListadoZonas()
    {
      return PARepositorio.Instancia.ObtenerListadoZonas();
    }
  }
}