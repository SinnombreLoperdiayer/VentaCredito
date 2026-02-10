using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Mensajeria;

namespace Framework.Servidor.Servicios.Contratos
{
  [ServiceContract(Namespace = "http://contrologis.com")]
  public interface IMEMensajeriaSvc
  {
   
    /// <summary>
    /// Consulta los mensajes dirigidos a una agencia
    /// </summary>
    /// <param name="agencia">Agencia a la cual se le quiere hacer la consulta</param>
    /// <returns></returns>
      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      GenericoConsultasFramework<MEMensajeEnviado> ConsultarMensajesAgencia(long Agencia, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente);

    /// <summary>
    /// Consulta los mensajes creados x un usuario
    /// </summary>
    /// <param name="usuario">Usuario al cual se le quiere hacer la consulta</param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<MEMensajeEnviado> ConsultarMensajesEnviadosXUsuario(string usuario, IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente);

    
    /// <summary>
    /// Consulta el numero de mensajes que la agencia no ha leido
    /// </summary>
    /// <param name="idCentroServicio">Centro de servicio</param>
    /// <returns>Numero de mensajes sin leer</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
      int ConsultarMensajesSinLeer(string idCentroServicio);

    
    /// <summary>
    /// Crea un mensaje nuevo dirigido a una agencia
    /// </summary>
    /// <param name="mensaje">Datos del mensaje que se está creando</param>    
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
      void CrearMensajeNuevo(MEMensajeEnviado mensaje);

    


   
    /// <summary>
    /// Crea un mensaje nuevo dirigido a una agencia
    /// </summary>
    /// <param name="respuesta">Datos de la respuesta del mensaje</param>
    /// <param name="idMensaje">Mensaje asociado a la respuesta</param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
      void CrearRespuestaMensaje(MERespuestaMensaje respuesta);

    
    /// <summary>
    /// Actualiza el estado de un mensaje especifico a leido.
    /// </summary>
    /// <param name="idMensaje">Id del mensaje que se está actualizando</param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
        void NotificarMensajeLeido(int idMensaje);

    
    /// <summary>
    /// Consulta las regionales administrativas existentes y activas
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
      IEnumerable<MERacol> ConsultarRegionalesAdministrativas();

    
    /// <summary>
    /// Consulta las agencias y puntos pertenecientes a un racol
    /// </summary>
    /// <param name="idRacol">Racol por el cual se desea hacer la consulta</param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
      IEnumerable<MEAgencia> ConsultarAgenciasYPuntosXRacol(long idRacol);

    
    /// <summary>
    /// Consulta las categorias de mensajes existentes
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
        IEnumerable<MECategoriaMensaje> ConsultarCategoriasMensaje();


      /// <summary>
    /// Crea un mensaje nuevo dirigido a varias agencias
    /// </summary>
    /// <param name="mensaje">Datos del mensaje que se está creando</param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void CrearMensajeNuevoMasivoWPF(MEMensajeEnviado mensaje, long[] centrosServicioDestinatario, string usuario);
  }
}