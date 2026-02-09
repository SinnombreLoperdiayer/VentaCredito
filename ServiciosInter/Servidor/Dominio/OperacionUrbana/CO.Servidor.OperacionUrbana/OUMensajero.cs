using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.OperacionUrbana.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.OperacionUrbana
{
  /// <summary>
  /// Clase que representa el mensajero de operación urbana
  /// </summary>
  internal class OUMensajero : ControllerBase
  {
    private static readonly OUMensajero instancia = (OUMensajero)FabricaInterceptores.GetProxy(new OUMensajero(), COConstantesModulos.MODULO_OPERACION_URBANA);

    /// <summary>
    /// Retorna una instancia de Mensajero
    /// /// </summary>
    public static OUMensajero Instancia
    {
      get { return OUMensajero.instancia; }
    }

    private OUMensajero()
    { }

    /// <summary>
    /// Obtiene los mensajeros dependientes de un centro de servicio, es decir, no solo trae los pertenecientes al centro de servicio dado, sino también 
    /// de aquellos de quienes el centro de servicio pasado como parámetro es responsable
    /// </summary>
    /// <param name="idCentroServicio">Id del centro de servicio de quien se desean obtener los mensajeros</param>
    /// <returns></returns>
    public List<OUNombresMensajeroDC> ObtenerMensajerosDependientesCentroServicio(long idCentroServicio)
    {
      List<OUNombresMensajeroDC> mensajeros = ObtenerNombreMensajeroAgencia(idCentroServicio).ToList();
      List<PUCentroServiciosDC> centrosServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerPuntosAgenciasDependientes(idCentroServicio);
      // Ahora obtener los clientes con sus respectivos contratos por cada centro de servicio
      foreach (PUCentroServiciosDC cs in centrosServicio)
      {
        IEnumerable<OUNombresMensajeroDC> mensajerosDependientes = ObtenerNombreMensajeroAgencia(cs.IdCentroServicio);
        mensajeros.AddRange(mensajerosDependientes);
      }
      return mensajeros;
    }
    /// <summary>
    /// Obtene los datos del mensajero de la agencia.
    /// </summary>
    /// <param name="idAgencia">Es el id agencia.</param>
    /// <returns>la lista de mensajeros de una agencia</returns>
    public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgencia(long idAgencia)
    {
      return OURepositorio.Instancia.ObtenerNombreMensajeroAgencia(idAgencia);
    }
    #region Mensajero

    ///////// <summary>
    ///////// Obtiene los mensajeros
    ///////// </summary>
    ///////// <returns></returns>
    //////public IList<OUMensajeroDC> ObtenerMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    //////{
    //////  return OURepositorio.Instancia.ObtenerMensajero(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    //////}

    /// <summary>
    /// Adiciona, edita o elimina un mensajero
    /// </summary>
    /// <param name="mensajero"></param>
    public void ActualizarMensajero(OUMensajeroDC mensajero)
    {
      ////bool validaContratista;
      //if (mensajero.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
      //{
      //  OURepositorio.Instancia.AdicionarMensajero(mensajero);
      //}

      //else if (mensajero.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
      //{
      //  OURepositorio.Instancia.EditarMensajero(mensajero);
      //}
    }

    /// <summary>
    /// Obtiene la lista de tipos de mensajero
    /// </summary>
    /// <returns></returns>
    public IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero()
    {
      return OURepositorio.Instancia.ObtenerTiposMensajero();
    }

    public IList<OUEstadosMensajeroDC> ObtenerEstadosMensajero()
    {
      IList<OUEstadosMensajeroDC> lestados = new List<OUEstadosMensajeroDC>();
      OUEstadosMensajeroDC estadoAdicionar;

      foreach (string estados in Enum.GetNames(typeof(OUEnumEstadosMensajero)))
      {
        estadoAdicionar = new OUEstadosMensajeroDC() { IdEstado = estados };

        if (OUEnumEstadosMensajero.ACT.ToString().CompareTo(estados) == 0)
          estadoAdicionar.Descripcion = OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.IN_ACTIVO);

        else if (OUEnumEstadosMensajero.INA.ToString().CompareTo(estados) == 0)
          estadoAdicionar.Descripcion = OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.IN_INACTIVO);

        else if (OUEnumEstadosMensajero.SUS.ToString().CompareTo(estados) == 0)
          estadoAdicionar.Descripcion = OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.IN_SUSPENDIDO);

        lestados.Add(estadoAdicionar);
      }

      return lestados;
    }

    /// <summary>
    /// Consulta si existe la persona en la bd de nova soft y retorna la informacion de la persona
    /// </summary>
    /// <param name="identificacion">Documento de identificacion</param>
    /// <param name="contratista">Tipo de vinculacion (Contratista o tercero)</param>
    /// <returns></returns>
    public OUMensajeroDC ConsultaExisteMensajero(string identificacion, bool contratista)
    {
      OUMensajeroDC mensajero = new OUMensajeroDC();

      ////////if (OURepositorio.Instancia.ConsultaExisteMensajero(identificacion))
      ////////{
      ////////  mensajero.PersonaInterna = InterfazSistemaExterno.Instancia.ConsultaMensajero(identificacion, contratista);
      ////////}
      ////////else
      ////////{
      ////////  throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_NO_CUMPLE_TIPO_VINCULACION.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_NO_CUMPLE_TIPO_VINCULACION)));
      ////////}

      ////////mensajero.Apellidos = mensajero.PersonaInterna.PrimerApellido + " " + mensajero.PersonaInterna.SegundoApellido;
      ////////mensajero.EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.ADICIONADO;
      ////////mensajero.Habilitado = true;
      ////////mensajero.EsContratista = contratista;

      ////////mensajero.FechaVencimientoPase = DateTime.Now;

      return mensajero;
    }

    /// <summary>
    /// Retorna el último mensajero que tuvo asignada una guía dada
    /// </summary>
    /// <param name="idGuia"></param>
    /// <returns></returns>
    public OUNombresMensajeroDC ConsultarUltimoMensajeroGuia(long idGuia)
    {
      return OURepositorio.Instancia.ConsultarUltimoMensajeroGuia(idGuia);
    }

    #endregion Mensajero
  }
}