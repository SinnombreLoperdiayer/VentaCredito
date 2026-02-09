using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace Framework.Servidor.ParametrosFW
{
  /// <summary>
  /// Clase de negocio para las localidades
  /// </summary>
  public class PALocalidad : ControllerBase
  {
    /// <summary>
    /// Singleton
    /// </summary>
    public static readonly PALocalidad Instancia = (PALocalidad)FabricaInterceptores.GetProxy(new PALocalidad(), ConstantesFramework.PARAMETROS_FRAMEWORK);

    /// <summary>
    /// Constructor
    /// </summary>
    private PALocalidad()
    { }

    /// <summary>
    /// Consulta las localidades aplicando el filtro y la paginacion
    /// </summary>
    /// <param name="IdTipoLocalidad"></param>
    /// <returns></returns>
    public List<PALocalidadDC> ConsultarLocalidades(IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return PARepositorio.Instancia.ConsultarLocalidades(filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
    }

    /// <summary>
    /// Elimina la informacion de una localidad y sus relaciones
    /// </summary>
    /// <param name="IdLocalidad"></param>
    public void EliminarLocalidad(string idLocalidad)
    {
      PARepositorio.Instancia.EliminarLocalidad(idLocalidad);
    }

    /// <summary>
    /// Consulta los tipos de localidad
    /// </summary>
    /// <returns>Lista con los tipos de localidad</returns>
    public List<PATipoLocalidad> ConsultarTipoLocalidad()
    {
      return PARepositorio.Instancia.ConsultarTipoLocalidad();
    }

    /// <summary>
    /// Inserta una nueva localidad
    /// </summary>
    /// <param name="localidad">Objeto con la informacion de la localidad</param>
    public void InsertarLocalidad(PALocalidadDC localidad)
    {
      if (PARepositorio.Instancia.VerificarDisponibilidadLocalidad(localidad.IdLocalidad))
        PARepositorio.Instancia.InsertarLocalidad(localidad);
      else
        throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_REGISTRO_YA_EXISTE.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_REGISTRO_YA_EXISTE)));
    }

    /// <summary>
    /// Modifica una localidad
    /// </summary>
    /// <param name="localidad"></param>
    public void ModificarLocalidad(PALocalidadDC localidad)
    {
      PARepositorio.Instancia.ModificarLocalidad(localidad);
    }

    /// <summary>
    /// Consulta las zonas de localidad por el id de la localidad
    /// </summary>
    /// <param name="IdLocalidad"></param>
    /// <returns></returns>
    public List<PAZonaDC> ConsultarZonaDeLocalidadXLocalidad(string idLocalidad, IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return PARepositorio.Instancia.ConsultarZonaDeLocalidadXLocalidad(idLocalidad, filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
    }

    /// <summary>
    /// Inserta una nueva localidad perteneciente a una zona
    /// </summary>
    /// <param name="localidadEnZona"></param>
    public void GuardarCambiosZonaDeLocalidad(List<PAZonaLocalidad> zonaDelocalidades)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        zonaDelocalidades.Where(obj => obj.Zona.EstadoRegistro == EnumEstadoRegistro.BORRADO).ToList().ForEach(obj => PARepositorio.Instancia.EliminarZonaDeLocalidad(obj.Zona.IdZona, obj.Localidad.IdLocalidad));
        zonaDelocalidades.Where(obj => obj.Zona.EstadoRegistro == EnumEstadoRegistro.ADICIONADO).ToList().ForEach(obj =>
        {
          PARepositorio.Instancia.InsertaZonaDeLocalidad(obj);
        });

        transaccion.Complete();
      }
    }

    /// <summary>
    /// Consulta las localidades por tipo de localidad
    /// </summary>
    /// <param name="IdTipoLocalidad"></param>
    /// <returns></returns>
    public List<PALocalidadDC> ConsultarLocalidadesXTipoLocalidad(PAEnumTipoLocalidad tipoLocalidad)
    {
      return PARepositorio.Instancia.ConsultarLocalidadesXTipoLocalidad(((short)tipoLocalidad).ToString());
    }

    /// <summary>
    /// Retorna la lista de localidad que no son países ni departamentos.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamento()
    {
      return PARepositorio.Instancia.ObtenerLocalidadesNoPaisNoDepartamento((int)PAEnumTipoLocalidad.PAIS, (int)PAEnumTipoLocalidad.DEPARTAMENTO);
    }

    /// <summary>
    /// Retorna la lista de localidad que no son países ni departamentos para Colombia
    /// </summary>
    /// <returns></returns>
    public IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamentoColombia()
    {
      return PARepositorio.Instancia.ObtenerMunicipiosCorregimientoInspeccionCaserioPais((int)PAEnumTipoLocalidad.PAIS, (int)PAEnumTipoLocalidad.DEPARTAMENTO, ConstantesFramework.ID_LOCALIDAD_COLOMBIA);
    }

    /// <summary>
    /// Retorna la lista de localidades que no son países ni departamentos para un país dado
    /// </summary>
    /// <param name="idPais"></param>
    /// <returns></returns>
    internal IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamentoPorPais(string idPais)
    {
      return PARepositorio.Instancia.ObtenerMunicipiosPorPais((int)PAEnumTipoLocalidad.PAIS, (int)PAEnumTipoLocalidad.DEPARTAMENTO, idPais);
    }

    /// <summary>
    /// Retorna la informacion de una localidad por el id de ella misma
    /// </summary>
    /// <returns></returns>
    public PALocalidadDC ObtenerInformacionLocalidad(string idLocalidad)
    {
      return PARepositorio.Instancia.ObtenerInformacionLocalidad(idLocalidad);
    }

    /// <summary>
    /// Retorna la lista de países
    /// </summary>
    /// <returns></returns>
    public IEnumerable<PALocalidadDC> ObtenerPaises()
    {
      return PARepositorio.Instancia.ObtenerPaises((int)PAEnumTipoLocalidad.PAIS);
    }

    /// <summary>
    /// Consulta las localidades por el padre y por el tipo de localidad
    /// </summary>
    /// <param name="IdTipoLocalidad"></param>
    /// <returns></returns>
    public List<PALocalidadDC> ConsultarLocalidadesXidPadreXidTipo(string idPadre, PAEnumTipoLocalidad tipoLocalidad)
    {
      return PARepositorio.Instancia.ConsultarLocalidadesXidPadreXidTipo(idPadre, ((short)tipoLocalidad).ToString());
    }

    /// <summary>
    /// Consulta las ciudades por departamento
    /// </summary>
    /// <param name="idDepto">Id del departamento</param>
    /// <param name="SoloMunicipios">Indica si solo se selecciona municipios o municipios + corregimientos inspecciones  caserios...</param>
    /// <returns>Lista de localidades</returns>
    public List<PALocalidadDC> ConsultarLocalidadesXDepartamento(string idDepto, bool SoloMunicipios)
    {
      return PARepositorio.Instancia.ConsultarLocalidadesXDepartamento(idDepto, SoloMunicipios);
    }

    /// <summary>
    /// Obtener la localidad por su identificación
    /// </summary>
    /// <param name="idLocalidad"></param>
    /// <returns></returns>
    public PALocalidadDC ObtenerLocalidadPorId(string idLocalidad)
    {
      return PARepositorio.Instancia.ObtenerLocalidadPorId(idLocalidad);
    }

       /// <summary>
        /// Obtiene el radio de busqueda de las reqcogidas en una localidad
        /// </summary>
        /// <param name="Idlocalidad"></param>
        /// <returns></returns>
    public int ObtenerRadioBusquedaRecogidaLocalidad(string Idlocalidad)
    {
        return PARepositorio.Instancia.ObtenerRadioBusquedaRecogidaLocalidad(Idlocalidad);
    }

  }
}