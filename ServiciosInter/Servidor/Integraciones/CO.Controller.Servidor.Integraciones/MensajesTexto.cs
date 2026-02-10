using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CO.Controller.Servidor.Integraciones.AccesoDatos;
using Framework.Servidor.Comun;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using System.IO;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.ParametrosFW.Datos;
using Framework.Servidor.Excepciones;

namespace CO.Controller.Servidor.Integraciones
{
    public class MensajesTexto: ControllerBase
    {
        public static readonly MensajesTexto Instancia = (MensajesTexto)FabricaInterceptores.GetProxy(new MensajesTexto(), COConstantesModulos.MENSAJERIA);

        private string usuario = string.Empty;
        private string uri = string.Empty;
        private string pass = string.Empty;
        public MensajesTexto()
        {
            usuario = DALMensajesTexto.Instancia.Parametros["UserMensajesTexto"];
            uri = DALMensajesTexto.Instancia.Parametros["UriMensajesTexto"];
            pass = DALMensajesTexto.Instancia.Parametros["PassMensajesTexto"];
        }

        public bool ValidarEjecucionMotor()
        {
            bool validacion = false;
            if (DALMensajesTexto.Instancia.ObtenerFechaUltimaEjecucionMotorMensajesTexto().Date < DateTime.Now.Date)
            {
                validacion = true;
            }
            return validacion;
        }

        public void EnviarMensajesPendientes()
        {
            List<MensajeTextoDC> lstMensajes = DALMensajesTexto.Instancia.ObtenerMensajesPendientes();
            lstMensajes.ForEach(m =>
            {
                EnviarMensajeTexto(m.NumeroCelular, m.Mensaje, m.IdMensajeNoEnviado, true);                
            });
        }

        /// <summary>
        /// obtiene mensaje a enviar por idmensaje
        /// </summary>
        /// <param name="idMensaje"></param>
        /// <returns></returns>
        public string ObtenerMensajeTexto(string idMensaje)
        {
            return DALMensajesTexto.Instancia.ObtenerMensajeTexto(idMensaje);
        }

        public void EnviarMensajeNoCliente(long idAdmision, long numeroGuia, string Mensaje, string numeroCelular, decimal valor)
        {
            int valorEntero = Convert.ToInt32(valor);
            string mensaje = string.Empty;
            bool enviado = DALMensajesTexto.Instancia.HayMensajeEnviadoAdmision(idAdmision);
            mensaje = string.Format(DALMensajesTexto.Instancia.ObtenerMensajeTexto(Mensaje), idAdmision, numeroGuia, valorEntero);

            if (ValidarHorarioEnvioMensajes(numeroCelular, mensaje))
            {
                if (mensaje == string.Empty || mensaje == null)
                {
                    return;
                }

                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(string.Format(uri, usuario, pass, numeroCelular, mensaje));

                try
                {
                    if (NumeroCelValido(numeroCelular))
                    {
                        WebResponse response = myReq.GetResponse();
                        string respuesta = ((HttpWebResponse)response).StatusDescription;
                        DALAuditoria.Instancia.AuditarIntegracion("MsjTexto", string.Format(uri, usuario, pass, numeroCelular, mensaje), respuesta);
                        response.Close();

                        if (respuesta == "OK")
                        {
                            DALMensajesTexto.Instancia.InsertarMensajeEnviado(idAdmision, numeroGuia, numeroCelular, mensaje, 0);
                        }
                    }
                }
                catch (Exception exc)
                {
                    DALAuditoria.Instancia.AuditarIntegracion("MsjTexto", string.Format(uri, usuario, pass, numeroCelular, mensaje), exc.ToString());
                }
            }
        }

        public void EnviarMensajeCliente(long idAdmision, long numeroGuia, int Idcliente, string numeroPedido, string numeroCelular, int idServicio)
        {
            string mensaje = string.Empty;
            mensaje = DALMensajesTexto.Instancia.ObtenerMensajeTextoCliente(Idcliente, idServicio);
            mensaje = string.Format(mensaje, numeroGuia, numeroPedido);
            if (mensaje == string.Empty || mensaje == null)
            {
                return;
            }

            if (ValidarHorarioEnvioMensajes(numeroCelular, mensaje))
            {
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(string.Format(uri, usuario, pass, numeroCelular, mensaje));

                try
                {
                    if (NumeroCelValido(numeroCelular))
                    {
                        WebResponse response = myReq.GetResponse();
                        string respuesta = ((HttpWebResponse)response).StatusDescription;
                        DALAuditoria.Instancia.AuditarIntegracion("MsjTexto", string.Format(uri, usuario, pass, numeroCelular, mensaje), respuesta);
                        response.Close();

                        if (respuesta == "OK")
                        {
                            DALMensajesTexto.Instancia.InsertarMensajeEnviado(idAdmision, numeroGuia, numeroCelular, numeroPedido, Idcliente);
                        }
                    }
                }
                catch (Exception exc)
                {
                    DALAuditoria.Instancia.AuditarIntegracion("MsjTexto", string.Format(uri, usuario, pass, numeroCelular, mensaje), exc.ToString());
                }
            }
        }

        /// <summary>
        /// Enviar mensaje de texto con base en el número de guía y el número de celular
        /// </summary>
        /// <param name="numGuia"></param>
        /// <param name="numCel"></param>
        public void EnviarMensajeTexto(long numGuia)
        {
            string usuario = DALMensajesTexto.Instancia.Parametros["UserMensajesTexto"];
            string uri = DALMensajesTexto.Instancia.Parametros["UriMensajesTexto"];
            string pass = DALMensajesTexto.Instancia.Parametros["PassMensajesTexto"];

            // Si hay clientes parametrizados para recibir mensajes de texto
            if (DALMensajesTexto.Instancia.Parametros.Count > 0)
            {
                // Consultar cliente propietario guía
                string numPedido = string.Empty;
                string fechaEntrega = string.Empty;
                string numCel = string.Empty;
                int? idCliente = DALMensajesTexto.Instancia.ConsultarInfoClienteRemitenteGuiaYTiempoEntrega(numGuia, ref numPedido, ref fechaEntrega, ref numCel);
                if (idCliente.HasValue)
                {
                    string mensaje = DALMensajesTexto.Instancia.ObtenerMensajeTextoCliente(idCliente.Value, 0);
                    mensaje = string.Format(mensaje, numGuia, numPedido);

                    if (ValidarHorarioEnvioMensajes(numCel, mensaje))
                    {
                        HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(string.Format(this.uri, this.usuario, this.pass, numCel, mensaje));

                        try
                        {
                            if (NumeroCelValido(numCel))
                            {
                                WebResponse response = myReq.GetResponse();
                                string respuesta = ((HttpWebResponse)response).StatusDescription;
                                DALAuditoria.Instancia.AuditarIntegracion("MsjTexto", string.Format(uri, usuario, pass, numCel, mensaje), respuesta);
                                response.Close();

                                if (respuesta == "OK")
                                {
                                    DALMensajesTexto.Instancia.ActualizarMensajeEnviado(numGuia);
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                            DALAuditoria.Instancia.AuditarIntegracion("MsjTexto", string.Format(uri, usuario, pass, numCel, mensaje), exc.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// enviar mensaje de texto
        /// </summary>
        /// <param name="pNumCel"></param>
        /// <param name="pMensaje"></param>
        public void EnviarMensajeTexto(string pNumCel, string pMensaje, int idMensajeNoEnviado = 0, bool esMotor = false)
        {
            if (ValidarHorarioEnvioMensajes(pNumCel, pMensaje, esMotor))
            {
                string usuario = DALMensajesTexto.Instancia.Parametros["UserMensajesTexto"];
                string uri = DALMensajesTexto.Instancia.Parametros["UriMensajesTexto"];
                string pass = DALMensajesTexto.Instancia.Parametros["PassMensajesTexto"];

                // Si hay clientes parametrizados para recibir mensajes de texto
                if (DALMensajesTexto.Instancia.Parametros.Count > 0)
                {

                    HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(string.Format(uri, usuario, pass, pNumCel, pMensaje));                    

                    try
                    {
                        if (NumeroCelValido(pNumCel))
                        {
                            WebResponse response = myReq.GetResponse();
                            string respuesta = ((HttpWebResponse)response).StatusDescription;
                            DALAuditoria.Instancia.AuditarIntegracion("MsjTexto", string.Format(uri, usuario, pass, pNumCel, pMensaje), respuesta);
                            response.Close();
                            if (esMotor)
                            {
                                DALMensajesTexto.Instancia.ActualizarEstadoMensajeTexto(idMensajeNoEnviado);
                                DALMensajesTexto.Instancia.InsertarFechaEjecucion();
                            }
                        }

                    }
                    catch (Exception exc)
                    {
                        DALAuditoria.Instancia.AuditarIntegracion("MsjTexto", string.Format(uri, usuario, pass, pNumCel, pMensaje), exc.ToString());
                    }

                }
            }
        }

        /// <summary>
        /// valida numero de celular
        /// </summary>
        /// <param name="pNumCel"></param>
        /// <returns></returns>
        public bool NumeroCelValido(string pNumCel)
        {
            if (pNumCel.Length != 10) return false;

            long Valor;
            if (!long.TryParse(pNumCel, out Valor)) return false;

            if (pNumCel.Substring(0, 1) != "3") return false;

            return true;
        }

        /// <summary>
        /// Valida el horario para enviar mensajes de texto
        /// </summary>
        /// <param name="numeroCelular"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public bool ValidarHorarioEnvioMensajes(string numeroCelular, string mensaje, bool esMotor = false)
        {
            bool validacion = false;

            HorarioMensajesTextoDC horario = new HorarioMensajesTextoDC();
            List<DateTime> lstFestivos = PARepositorio.Instancia.ObtenerFestivos(new DateTime(DateTime.Now.Year, 1, 1), new DateTime((DateTime.Now.Year + 1), 1, 1), PARepositorio.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday && (lstFestivos.Where(f => f.Date == DateTime.Now.Date).Count() <= 0))
            {
                horario = DALAuditoria.Instancia.ObtenerHorarioParaMensajes(Convert.ToInt32(DateTime.Now.DayOfWeek));
                if (DateTime.Now.TimeOfDay > horario.HoraInicio.TimeOfDay && DateTime.Now.TimeOfDay < horario.HoraFin.TimeOfDay)
                {
                    validacion = true;
                }
            }

            if (!validacion && !esMotor)
            {
                DALAuditoria.Instancia.InsertarMensajesNoEnviados(numeroCelular, mensaje);
            }

            return validacion;
        }
    }
}
