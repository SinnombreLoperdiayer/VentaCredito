using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Mensajeria.Datos;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Mensajeria;

namespace Framework.Servidor.Mensajeria
{
  public class MEManejadorMensajeria : ControllerBase
  {
    #region Instancia Singleton

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    private static readonly MEManejadorMensajeria instancia = (MEManejadorMensajeria)FabricaInterceptores.GetProxy(new MEManejadorMensajeria(), Framework.Servidor.Comun.ConstantesFramework.MODULO_FW_MENSAJERIA);

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    public static MEManejadorMensajeria Instancia
    {
      get { return MEManejadorMensajeria.instancia; }
    }

    #endregion Instancia Singleton

    #region Metodos Publicos

    /// <summary>
    /// Consulta los mensajes dirigidos a una agencia
    /// </summary>
    /// <param name="agencia">Agencia a la cual se le quiere hacer la consulta</param>
    /// <returns></returns>
    public GenericoConsultasFramework<MEMensajeEnviado> ConsultarMensajesAgencia(long Agencia, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return MERepositorio.Instancia.ConsultarMensajesAgencia(Agencia, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
    }

    /// <summary>
    /// Consulta los mensajes creados x un usuario
    /// </summary>
    /// <param name="usuario">Usuario al cual se le quiere hacer la consulta</param>
    /// <returns></returns>
    public GenericoConsultasFramework<MEMensajeEnviado> ConsultarMensajesEnviadosXUsuario(string usuario, IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return MERepositorio.Instancia.ConsultarMensajesEnviadosXUsuario(usuario, filtro, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
    }

    /// <summary>
    /// Consulta el numero de mensajes que la agencia no ha leido
    /// </summary>
    /// <param name="idCentroServicio">Centro de servicio</param>
    /// <returns>Numero de mensajes sin leer</returns>
    public int ConsultarMensajesSinLeer(string idCentroServicio)
    {
      return MERepositorio.Instancia.ConsultarMensajesSinLeer(idCentroServicio);
    }

    /// <summary>
    /// Crea un mensaje nuevo dirigido a una agencia
    /// </summary>
    /// <param name="mensaje">Datos del mensaje que se está creando</param>
    public void CrearMensajeNuevo(MEMensajeEnviado mensaje, string usuario)
    {
      MERepositorio.Instancia.CrearMensajeNuevo(mensaje, usuario);
    }


    /// <summary>
    /// Crea un mensaje nuevo dirigido a varias agencias
    /// </summary>
    /// <param name="mensaje">Datos del mensaje que se está creando</param>
    public void CrearMensajeNuevoMasivo(MEMensajeEnviado mensaje, string usuario, long[] centrosServicioDestinatario)
    {
        MERepositorio.Instancia.CrearMensajeNuevoMasivo(mensaje, usuario,centrosServicioDestinatario);
    }

    /// <summary>
    /// Crea un mensaje nuevo dirigido a una agencia
    /// </summary>
    /// <param name="respuesta">Datos de la respuesta del mensaje</param>
    /// <param name="idMensaje">Mensaje asociado a la respuesta</param>
    public void CrearRespuestaMensaje(MERespuestaMensaje respuesta, string usuario)
    {
      MERepositorio.Instancia.CrearRespuestaMensaje(respuesta, usuario);
    }

    /// <summary>
    /// Actualiza el estado de un mensaje especifico a leido.
    /// </summary>
    /// <param name="idMensaje">Id del mensaje que se está actualizando</param>
    public void NotificarMensajeLeido(int idMensaje, string usuario)
    {
      MERepositorio.Instancia.NotificarMensajeLeido(idMensaje, usuario);
    }

    /// <summary>
    /// Consulta las regionales administrativas existentes y activas
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MERacol> ConsultarRegionalesAdministrativas()
    {
      return MERepositorio.Instancia.ConsultarRegionalesAdministrativas();
    }

    /// <summary>
    /// Consulta las agencias y puntos pertenecientes a un racol
    /// </summary>
    /// <param name="idRacol">Racol por el cual se desea hacer la consulta</param>
    /// <returns></returns>
    public IEnumerable<MEAgencia> ConsultarAgenciasYPuntosXRacol(long idRacol)
    {
      return MERepositorio.Instancia.ConsultarAgenciasYPuntosXRacol(idRacol);
    }

    /// <summary>
    /// Consulta las categorias de mensajes existentes
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MECategoriaMensaje> ConsultarCategoriasMensaje()
    {
      return MERepositorio.Instancia.ConsultarCategoriasMensaje();
    }

    #endregion Metodos Publicos
  }
}