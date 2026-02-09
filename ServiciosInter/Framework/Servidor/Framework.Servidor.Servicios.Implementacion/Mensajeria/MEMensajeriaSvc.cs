using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Mensajeria;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Mensajeria;
using Framework.Servidor.Servicios.Contratos;
using System.ServiceModel;

namespace Framework.Servidor.Servicios.Implementacion.Mensajeria
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class MEMensajeriaSvc : IMEMensajeriaSvc
  {
    public MEMensajeriaSvc()
    {
      Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
    }

    /// <summary>
    /// Consulta los mensajes dirigidos a una agencia
    /// </summary>
    /// <param name="agencia">Agencia a la cual se le quiere hacer la consulta</param>
    /// <returns></returns>
    public GenericoConsultasFramework<MEMensajeEnviado> ConsultarMensajesAgencia(long Agencia, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return MEManejadorMensajeria.Instancia.ConsultarMensajesAgencia(Agencia, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
    }

    /// <summary>
    /// Consulta los mensajes creados x un usuario
    /// </summary>
    /// <param name="usuario">Usuario al cual se le quiere hacer la consulta</param>
    /// <returns></returns>
    public GenericoConsultasFramework<MEMensajeEnviado> ConsultarMensajesEnviadosXUsuario(string usuario, IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return MEManejadorMensajeria.Instancia.ConsultarMensajesEnviadosXUsuario(usuario, filtro, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
    }

    /// <summary>
    /// Consulta el numero de mensajes que la agencia no ha leido
    /// </summary>
    /// <param name="idCentroServicio">Centro de servicio</param>
    /// <returns>Numero de mensajes sin leer</returns>
    public int ConsultarMensajesSinLeer(string idCentroServicio)
    {
      return MEManejadorMensajeria.Instancia.ConsultarMensajesSinLeer(idCentroServicio);
    }

    /// <summary>
    /// Crea un mensaje nuevo dirigido a una agencia
    /// </summary>
    /// <param name="mensaje">Datos del mensaje que se está creando</param>
    public void CrearMensajeNuevo(MEMensajeEnviado mensaje)
    {
      MEManejadorMensajeria.Instancia.CrearMensajeNuevo(mensaje, ControllerContext.Current.Usuario);
    }    

    /// <summary>
    /// Crea un mensaje nuevo dirigido a una agencia
    /// </summary>
    /// <param name="respuesta">Datos de la respuesta del mensaje</param>
    /// <param name="idMensaje">Mensaje asociado a la respuesta</param>
    public void CrearRespuestaMensaje(MERespuestaMensaje respuesta)
    {
      MEManejadorMensajeria.Instancia.CrearRespuestaMensaje(respuesta, ControllerContext.Current.Usuario);
    }

    /// <summary>
    /// Actualiza el estado de un mensaje especifico a leido.
    /// </summary>
    /// <param name="idMensaje">Id del mensaje que se está actualizando</param>
    public void NotificarMensajeLeido(int idMensaje)
    {
      MEManejadorMensajeria.Instancia.NotificarMensajeLeido(idMensaje, ControllerContext.Current.Usuario);
    }

    /// <summary>
    /// Consulta las regionales administrativas existentes y activas
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MERacol> ConsultarRegionalesAdministrativas()
    {
      return MEManejadorMensajeria.Instancia.ConsultarRegionalesAdministrativas();
    }

    /// <summary>
    /// Consulta las agencias y puntos pertenecientes a un racol
    /// </summary>
    /// <param name="idRacol">Racol por el cual se desea hacer la consulta</param>
    /// <returns></returns>
    public IEnumerable<MEAgencia> ConsultarAgenciasYPuntosXRacol(long idRacol)
    {
      return MEManejadorMensajeria.Instancia.ConsultarAgenciasYPuntosXRacol(idRacol);
    }

    /// <summary>
    /// Consulta las categorias de mensajes existentes
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MECategoriaMensaje> ConsultarCategoriasMensaje()
    {
      return MEManejadorMensajeria.Instancia.ConsultarCategoriasMensaje();
    }

    /// <summary>
    /// Crea un mensaje nuevo dirigido a varias agencias
    /// </summary>
    /// <param name="mensaje">Datos del mensaje que se está creando</param>
    public void CrearMensajeNuevoMasivoWPF(MEMensajeEnviado mensaje, long[] centrosServicioDestinatario,string usuario)
    {
        MEManejadorMensajeria.Instancia.CrearMensajeNuevoMasivo(mensaje, usuario, centrosServicioDestinatario);
    }

  }
}