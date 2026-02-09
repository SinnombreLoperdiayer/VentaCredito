using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.WebApi.NotificacionesSignalR;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApiHub
{
    public class HubPrincipal : Hub
    {
        #region singleton
        //private readonly HubPrincipal instancia;
        public void Sumar() { }
        #endregion

        #region Lista Usuarios
        public sealed class ManejadorUsuarios
        {
            static readonly HashSet<ParametrosSignalR> listaUsuarios = new HashSet<ParametrosSignalR>();

            public static HashSet<ParametrosSignalR> ListaUsuarios
            {
                get { return ManejadorUsuarios.listaUsuarios; }
            }
        }


        public static HashSet<ParametrosSignalR> ListaUsuarios
        {
            get
            {
                return ManejadorUsuarios.ListaUsuarios;
            }
        }


        #endregion

        #region Override Task
        /// <summary>
        /// SOBRE-ESCRITURA EVENTO ONCONNECTED
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            //ADICION DE CLIENTES A LA LISTA (EN EJECUCIÓN PERMANENTE)
            ManejadorUsuarios.ListaUsuarios.Add(new ParametrosSignalR()
            {
                IdConexion = Context.ConnectionId,
                NombreUsuario = Context.QueryString["nombreUsuario"].Trim(),
                Documento = Convert.ToInt64(Context.QueryString["numeroDocumento"].Trim()),
                TipoDocumento = Convert.ToInt16(Context.QueryString["tipoDocumento"].Trim()),
                FechaIngreso = DateTime.Now,
                IdAplicacion = (COEnumIdentificadorAplicacion)Enum.Parse(typeof(COEnumIdentificadorAplicacion), Convert.ToString(Context.QueryString["idAplicacion"].Trim()))
            });
            var grupo = Convert.ToString(Context.QueryString["idAplicacion"].Trim());
            Groups.Add(Context.ConnectionId, grupo);

            // Clients.Group(grupo).enviarNotificacionUsuario(new ParametrosSignalR { Mensaje = "Bienvenido al grupo" });
            return base.OnConnected();
        }

        /// <summary>
        /// SOBREESCRITURA DE LA TAREA ONDISCONNECTED
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            //ELIMINACIÓN DEL USUARIO 
            ParametrosSignalR usuario = ManejadorUsuarios.ListaUsuarios.Where(e => e.IdConexion == Context.ConnectionId).FirstOrDefault();
            if (usuario != null)
            {
                ManejadorUsuarios.ListaUsuarios.Remove(usuario);
                var grupo = Convert.ToString(Context.QueryString["idAplicacion"].Trim());
                Groups.Remove(Context.ConnectionId, grupo);
            }

            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Sobreescritura del metodo reconexión
        /// </summary>
        /// <returns></returns>
        public override Task OnReconnected()
        {
            //RECONEXIÓN DEL USUARIO 
            ParametrosSignalR usuario = ManejadorUsuarios.ListaUsuarios.Where(e => e.IdConexion == Context.ConnectionId).FirstOrDefault();
            if (usuario != null)
            {
                ManejadorUsuarios.ListaUsuarios.Remove(usuario);
                var grupo = Convert.ToString(Context.QueryString["idAplicacion"].Trim());
                Groups.Add(Context.ConnectionId, grupo);
            }
            return base.OnReconnected();
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Metodo para forzar el cierre de conexión del cliente
        /// </summary>
        /// <param name="parametro"></param>
        public void CerrarConexionCliente(ParametrosSignalR parametro)
        {
            //RECONEXIÓN DEL USUARIO 
            ParametrosSignalR usuario = ManejadorUsuarios.ListaUsuarios.Where(e => e.IdConexion == Context.ConnectionId).FirstOrDefault();
            if (usuario != null)
            {
                ManejadorUsuarios.ListaUsuarios.Remove(usuario);
                Clients.Client(usuario.IdConexion).pararConexion();
            }
        }

        /// <summary>
        /// ENVIAR NOTIFICACION A TODOS LOS USUARIOS
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        public void EnviarMensajeTodoDestino(ParametrosSignalR mensaje)
        {
            Clients.All.enviarNotificacionTodoUsuario(mensaje);
        }

        /// <summary>
        /// ENVIAR NOTIFICACION A DETERMINADO USUARIO
        /// </summary>
        /// <param name="mensaje"></param>
        public void EnviarMensajeDeterminadoUsuario(ParametrosSignalR mensaje)
        {
            //BUSQUEDA POR NUMERO DE DOCUMENTO
            ParametrosSignalR usuario = ManejadorUsuarios.ListaUsuarios.Where(e => e.Documento == mensaje.Documento).FirstOrDefault();
            if (usuario != null)
            {
                Clients.Client(usuario.IdConexion).enviarNotificacionUsuario(mensaje);
            }
        }

        /// <summary>
        /// RETORNA LA CANTIDAD DE USUARIOS EN LINEA
        /// </summary>
        /// <returns></returns>
        public void CalcularCantidadUsuarios(ParametrosSignalR mensaje)
        {
            Clients.All.consultarCantidad(ManejadorUsuarios.ListaUsuarios.Count.ToString());
        }

        public void ActualizarUsuario(ParametrosSignalR mensaje)
        {
            ParametrosSignalR parametro = new ParametrosSignalR()
            {
                IdConexion = Context.ConnectionId,
                NombreUsuario = Context.QueryString["nombreUsuario"].Trim(),
                Documento = Convert.ToInt64(Context.QueryString["numeroDocumento"].Trim()),
                TipoDocumento = Convert.ToInt16(Context.QueryString["tipoDocumento"].Trim()),
                FechaIngreso = DateTime.Now
            };
            ManejadorUsuarios.ListaUsuarios.Add(parametro);


        }
        #endregion

        #region Recogidas

        /// <summary>
        /// ENVIAR NOTIFICACION A administradores
        /// </summary>
        /// <param name="mensaje"></param>
        public void NotificaAdministradoresRecogidas(ParametrosSignalR mensaje)
        {
            Clients.All.enviarNotificacionAdministradorRecogida(mensaje);
        }
        #endregion
    }
}