using CO.Controller.Servidor.Integracion.EventosSatrack.Entidades;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun.Util;
using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Controller.Servidor.Integracion.EventosSatrack
{
    public class EventosSatrack
    {
        private static readonly EventosSatrack instancia = new EventosSatrack();

        public static EventosSatrack Instancia
        {
            get { return EventosSatrack.instancia; }
        }

        private EventosSatrack()
        {

        }
        /// <summary>
        /// Devuelve un Dataset con los eventos especificados para una o todas las placas 
        /// de la flota de un usuario en un rango de fechas específico
        /// </summary>
        /// <param name="DatosVahiculo"></param>
        /// <returns></returns>
        /// 
        public DataSet ConsultarEventosPlacasDeFlotaUsuario()
        {

            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient miEvento = new misEventosSatrack.getEventsSoapClient();
            DataSet datos = miEvento.retrieveEventsV3(credencial.Usuario, credencial.Password, "*", "0", 2011,
                                                    2, 3, 10, 0, 0, 2011, 2, 6, 17, 0, 0);
            return datos;
        }

        /// <summary>
        /// Devuelve un string con los eventos especificados para una o todas las placas 
        /// de la flota de un usuario en un rango de fechas específico
        /// </summary>
        /// <param name="DatosVahiculo"></param>
        /// <returns></returns>
        /// 

        public string ConsultarEventosPlacasDeFlotaUsuarioString()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            string datos = ultimo.retrieveEventStringV3(credencial.Usuario, 
                                                        credencial.Password, 
                                                        "*", 
                                                        "0", 
                                                        2011,
                                                        2, 
                                                        3, 
                                                        10, 
                                                        0, 
                                                        0, 
                                                        2016, 
                                                        1, 
                                                        6, 
                                                        17, 
                                                        0, 
                                                        0);
            return datos;
        }

        /// <summary>
        /// Devuelve un Dataset con los eventos especificados para una o todas las placas 
        /// de la flota de un usuario en un rango de fechas específico
        /// </summary>
        /// <param name="DatosVahiculo"></param>
        /// <returns></returns>
        /// 

        public DataSet ConsultarEventosPlacasDeFlotaUsuarioStringIDV3()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            DataSet datos = ultimo.retrieveEventsByIDV3(credencial.Usuario, credencial.Password, "*", "21", 4557455484, 150);
            return datos;
        }

        /// <summary>
        /// Devuelve un dataset el ultimo evento especificado de una o todas las placas 
        /// </summary>
        /// <param name="DatosVahiculo"></param>
        /// <returns></returns>
        /// 

        public DataSet TomarUltimoEvento()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            DataSet datos = ultimo.getLastEvent(credencial.Usuario, credencial.Password, "XYZ123");
            return datos;
        }

        /// <summary>
        /// Devuelve un string con el ultimo evento especificado de una o todas las placas 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        public string TomarUltimoEventoString()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            string datos = ultimo.getLastEventString(credencial.Usuario, credencial.Password, "XYZ123");
            return datos;
        }

        /// <summary>
        /// Devuelve un dataset con los puntos de control del usuario 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 

        public DataSet ObtenerPuntosDeControlUsuario()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            DataSet datos = ultimo.getVirtualPointByUser(credencial.Usuario, credencial.Password);
            return datos;
        }

        /// <summary>
        /// Devuelve un dataset con los eventos de los puntos de control del usuario 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        public DataSet ObtenerEventosPuntosDeControlUsuario()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            DataSet datos = ultimo.getVirtualPointEvents(credencial.Usuario, 
                                                         credencial.Password, 
                                                         "*",
                                                         2011, 
                                                         2, 
                                                         3, 
                                                         10, 
                                                         0, 
                                                         0, 
                                                         2016, 
                                                         1, 
                                                         1, 
                                                         10, 
                                                         0, 
                                                         0);
            return datos;
        }


        /// Devuelve un dataset con el kilometraje de los vehiculos o de uno en particular 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        public DataSet obtenerKilometraje()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            DataSet datos = ultimo.GetKilometer(credencial.Usuario, 
                                                credencial.Password, 
                                                "*", 
                                                2011, 
                                                10, 
                                                10, 
                                                10, 
                                                0, 
                                                0, 
                                                2016, 
                                                1, 
                                                1, 
                                                0, 
                                                0, 
                                                0);
            return datos;
        }

        /// <summary>
        /// Devuelve un string con el kilometraje de los vehiculos o de uno en particular 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        public string obtenerKilometrajeString()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            string datos = ultimo.GetKilometerString(credencial.Usuario, 
                                                     credencial.Password, 
                                                     "*", 
                                                     2011, 
                                                     10, 
                                                     10, 
                                                     10, 
                                                     0,
                                                     0,
                                                     2016, 
                                                     1, 
                                                     1, 
                                                     0, 
                                                     0, 
                                                     0);
            return datos;
        }

        /// <summary>
        /// Devuelve un dataset con las regiones correspondientes a un usuario 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        public DataSet ObtenerRegionesPorUsuario()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            DataSet datos = ultimo.GetRegionesByUser(credencial.Usuario, credencial.Password);
            return datos;
        }

        /// <summary>
        /// Devuelve un string con las regiones correspondientes a un usuario 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        public string ObtenerRegionesPorUsuarioString()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            string datos = ultimo.GetRegionesByUserString(credencial.Usuario, credencial.Password);
            return datos;
        }

        /// <summary>
        /// Devuelve un dataset con los vehiculos correspondientes a una region 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        public DataSet ObtenerVehiculosPorRegiones()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            DataSet datos = ultimo.GetVehiclesByRegiones(credencial.Usuario, credencial.Password, "Bogota");
            return datos;
        }

        /// <summary>
        /// Devuelve un string con los vehiculos correspondientes a una region 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        public string ObtenerVehiculosPorRegionesString()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            string datos = ultimo.GetVehiclesByRegionesString(credencial.Usuario, credencial.Password, "Bogota");
            return datos;
        }

        /// <summary>
        /// Devuelve un dataset con las alarmas generadas por los vehículos de un usuario
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        public DataSet ObtenerAlarmas()
        {
            CredencialesEventos credencial = new CredencialesEventos();
            misEventosSatrack.getEventsSoapClient ultimo = new misEventosSatrack.getEventsSoapClient();
            DataSet datos = ultimo.GetAlarms(credencial.Usuario, credencial.Password, "1");
            return datos;
        }
    }
}
