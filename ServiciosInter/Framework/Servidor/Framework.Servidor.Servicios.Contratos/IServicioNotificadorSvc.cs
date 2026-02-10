using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Framework.Servidor.Servicios.ContratoDatos.Mensajeria;

namespace Framework.Servidor.Servicios.Contratos
{
    [ServiceContract(CallbackContract = typeof(IServicioNotificadorCallBack))]
    public interface IServicioNotificadorSvc
    {
        [OperationContract]
        bool ConectarNotificador(string idCliente);
        
        [OperationContract]
        bool VerificarConexionNotificador(string idCliente);

        [OperationContract]
        List<string> ObtenerClientesConectados();

        [OperationContract]
        bool EnviarNotificacionCliente(string idCliente);

         /// <summary>
        /// Obtiene los centros de servicios que deberian estar en linea
        /// </summary>        
        /// <returns></returns>
        [OperationContract]
        List<Framework.Servidor.Servicios.ContratoDatos.Notificador.CentrosServiciosLinea> ConsultarCentrosServiciosDeberianEstarLinea();
        
        [OperationContract]
        bool BloquearClienteOnLine(string idCliente);
        
        [OperationContract]
        bool DesBloquearClienteOnLine(string idCliente);
        
        [OperationContract]
        void EnviarNoticiasClientes(MEMensajeEnviado mensaje, List<string> clientes);

         /// <summary>
        /// Notifica a todos los clientes de una ciudad, que se creó una nueva recogida
        /// </summary>
        /// <param name="idLocalidadNotificacion"></param>
        [OperationContract]
        void NotificarRecogidaNodeJS(string idRecogida);
      
    }

    public interface IServicioNotificadorCallBack
    {

        [OperationContract(IsOneWay = true)]
        void NotificarClienteController(string str);

        [OperationContract()]
        bool testConexion();
        [OperationContract()]
        void BloquearCliente();

        [OperationContract()]
        void DesBloquearCliente();

        [OperationContract()]
        void ServicioNoticias(MEMensajeEnviado mensaje);

        [OperationContract(IsOneWay = true)]
        void NotificarRecogidaClientes(string idLocalidadRecogida, string direccionRecogida, string nombreCompletoPersona);
    }
}
