using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using System;
using System.Collections.Generic;
using System.Text;
using CO.Servidor.Dominio.Comun.GestionGiros;
using Framework.Servidor.Comun.Util;
using CO.Controller.Servidor.Integraciones.AccesoDatos.DALIntegraciones;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System.Xml;
using System.Net;
using System.IO;
using System.Web;
using System.Resources;
using System.Configuration;
using CO.Controller.Servidor.Integraciones.AccesoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.ServiceModel;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.LogisticaInversa;

namespace CO.Servidor.Integraciones
{
    public class IntegracionesController : ControllerBase
    {
        private IGIFachadaGestionGiros fachadaGiros = COFabricaDominio.Instancia.CrearInstancia<IGIFachadaGestionGiros>();

        private IADFachadaAdmisionesMensajeria fachadaAdmisiones = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private ILIFachadaLogisticaInversaPruebasEntrega fachadaLogisticaInversaPruebaEntrega = COFabricaDominio.Instancia.CrearInstancia<ILIFachadaLogisticaInversaPruebasEntrega>();

        #region  Instancia

        private static readonly IntegracionesController instancia = (IntegracionesController)FabricaInterceptores.GetProxy(new IntegracionesController(), COConstantesModulos.INT_YANBAL); 
        /// <summary>
        /// Retorna una instancia de administracion de produccion
        /// /// </summary>
        public static IntegracionesController Instancia
        {
            get { return IntegracionesController.instancia; }
        }

        #endregion  Instancia

        #region Métodos

        #region Leonisa

        public string ConsultarGuiaLeonisa(long numeroGuia)
        {
            ADTrazaGuia trazaGuia = EstadosGuia.ObtenerEstadoGestion(numeroGuia);
            if (trazaGuia != null)
            {
                if (trazaGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada)
                    return string.Concat(trazaGuia.DescripcionEstadoGuia, " - ", EstadosGuia.ObtenerMotivoGestion(trazaGuia.IdTrazaGuia.Value).Descripcion) + " - " + trazaGuia.FechaGrabacion;
                else
                    return trazaGuia.DescripcionEstadoGuia + " - " + trazaGuia.FechaGrabacion;
            }
            else
            {
                trazaGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(numeroGuia);
                return trazaGuia.DescripcionEstadoGuia + " - " + trazaGuia.FechaGrabacion;
            }

        }



        #endregion

        #region 472
        public respuestaWSRiesgoLiquidezDTO consultaIngresosEgresosPuntosDeAtencion(credencialDTO credencial, string codigoRed, DateTime fecha, string horaInicial, string horaFinal)
        {
            respuestaWSRiesgoLiquidezDTO respuesta = new respuestaWSRiesgoLiquidezDTO();
            string fechaInicial = fecha.AddHours(DateTime.Parse(horaInicial).Hour).AddMinutes(DateTime.Parse(horaInicial).Minute).ToString();
            string fechaFinal = fecha.AddHours(DateTime.Parse(horaFinal).Hour).AddMinutes(DateTime.Parse(horaFinal).Minute).ToString();
            if (ValidarPassword472(credencial))
            {
                respuesta.fecha = fecha;
                respuesta.codigoRed = codigoRed;
                respuesta.estado = "COMPLETADO";
                respuesta.descripcionEstado = "Se realizo la consulta de forma exitosa";
                respuesta.listaPuntosDeAtencion = fachadaGiros.consultaIngresosEgresosPuntosDeAtencion(fechaInicial, fechaFinal);
            }
            else
            {
                respuesta.fecha = fecha;
                respuesta.codigoRed = codigoRed;
                respuesta.estado = "FALLIDO";
                respuesta.descripcionEstado = "Usuario y/o contraseña no corresponden";
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    StringBuilder cadenaElementosPeticion = new StringBuilder();
                    cadenaElementosPeticion.Append(codigoRed + " - ");
                    cadenaElementosPeticion.Append(fecha.ToString() + " - ");
                    cadenaElementosPeticion.Append(horaInicial + " - ");
                    cadenaElementosPeticion.Append(horaFinal);
                    string peticionSerializada = Serializacion.Serialize<credencialDTO>(credencial) + Serializacion.Serialize<string>(cadenaElementosPeticion.ToString());
                    string respuestaSerializada = Serializacion.Serialize<respuestaWSRiesgoLiquidezDTO>(respuesta);
                    string tipoIntegracion = "472";
                    fachadaGiros.AuditarIntegracion472(tipoIntegracion, peticionSerializada, respuestaSerializada);
                }
                catch (Exception ex)
                { }
            });

            return respuesta;
        }

        public bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega)
        {
            return fachadaLogisticaInversaPruebaEntrega.InsertarLecturaEcaptureArchivoPruebaEntrega(archivoPruebaEntrega);
        }

        public bool ValidarRecepcionHistoricoEcapture(long numeroGuia, string codigoProceso)
        {
            return fachadaLogisticaInversaPruebaEntrega.ValidarRecepcionHistoricoEcapture(numeroGuia, codigoProceso);
        }

        public int ConsultarOrigenGuia(long numeroGuia)
        {
            return fachadaLogisticaInversaPruebaEntrega.ConsultarOrigenGuia(numeroGuia);
        }

        public void ActualizarArchivoVolanteSincronizado(ArchivoVolante archivoVolante)
        {
            fachadaLogisticaInversaPruebaEntrega.ActualizarArchivoVolanteSincronizado(archivoVolante);
        }

        public List<ArchivoVolante> VerificarVolante(string numeroVolante)
        {
            return fachadaLogisticaInversaPruebaEntrega.VerificarVolante(numeroVolante);
        }

        public void InsertaHistoricoArchivoGuiaDigitalizada(ArchivoGuia archivoGuia)
        {
            fachadaLogisticaInversaPruebaEntrega.InsertaHistoricoArchivoGuiaDigitalizada(archivoGuia);
        }

        public void ActualizarArchivoGuiaDigitalizada(ArchivoGuia archivoGuia)
        {
            fachadaLogisticaInversaPruebaEntrega.ActualizarArchivoGuiaDigitalizada(archivoGuia);
        }

        public List<ArchivoGuia> VerificarGuia(string numeroGuia)
        {
            return fachadaLogisticaInversaPruebaEntrega.VerificarGuia(numeroGuia);
        }

        /// <summary>
        /// obtiene el valor real de la caja en el punto
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public respuestaWSRiesgoLiquidezDTO consultaValorRealPorPuntosDeAtencion(credencialDTO credencial, string codigoRed)
        {
            respuestaWSRiesgoLiquidezDTO respuesta = new respuestaWSRiesgoLiquidezDTO();
            if (ValidarPassword472(credencial))
            {
                respuesta.codigoRed = codigoRed;
                respuesta.fecha = DateTime.Now;
                respuesta.estado = "COMPLETADO";
                respuesta.descripcionEstado = "Se realizo la consulta de forma exitosa";
                respuesta.listaPuntosDeAtencion = fachadaGiros.consultaValorRealPorPuntosDeAtencion();
            }
            else
            {
                respuesta.fecha = DateTime.Now;
                respuesta.codigoRed = codigoRed;
                respuesta.estado = "FALLIDO";
                respuesta.descripcionEstado = "Usuario y/o contraseña no corresponden";
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    StringBuilder cadenaElementosPeticion = new StringBuilder();
                    cadenaElementosPeticion.Append(codigoRed);
                    string peticionSerializada = Serializacion.Serialize<credencialDTO>(credencial) + Serializacion.Serialize<string>(cadenaElementosPeticion.ToString());
                    string respuestaSerializada = Serializacion.Serialize<respuestaWSRiesgoLiquidezDTO>(respuesta);
                    string tipoIntegracion = "472";
                    fachadaGiros.AuditarIntegracion472(tipoIntegracion, peticionSerializada, respuestaSerializada);
                }
                catch (Exception ex)
                { }
            });

            return respuesta;
        }

        /// <summary>
        /// consulta Valor Real Punto De Atencion
        /// </summary>
        /// <param name="credencial"></param>
        /// <param name="codigoRed"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public respuestaWSRiesgoLiquidezDTO consultaValorRealPuntoDeAtencion(credencialDTO credencial, string codigoRed, string idCentroServicio)
        {
            respuestaWSRiesgoLiquidezDTO respuesta = new respuestaWSRiesgoLiquidezDTO();
            if (ValidarPassword472(credencial))
            {
                respuesta.codigoRed = codigoRed;
                respuesta.fecha = DateTime.Now;
                respuesta.estado = "COMPLETADO";
                respuesta.descripcionEstado = "Se realizo la consulta de forma exitosa";
                respuesta.listaPuntosDeAtencion = fachadaGiros.consultaValorRealPorPuntoDeAtencion(idCentroServicio);
            }
            else
            {
                respuesta.fecha = DateTime.Now;
                respuesta.codigoRed = codigoRed;
                respuesta.estado = "FALLIDO";
                respuesta.descripcionEstado = "Usuario y/o contraseña no corresponden";
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    StringBuilder cadenaElementosPeticion = new StringBuilder();
                    cadenaElementosPeticion.Append(codigoRed);
                    string peticionSerializada = Serializacion.Serialize<credencialDTO>(credencial) + Serializacion.Serialize<string>(cadenaElementosPeticion.ToString());
                    string respuestaSerializada = Serializacion.Serialize<respuestaWSRiesgoLiquidezDTO>(respuesta);
                    string tipoIntegracion = "472";
                    fachadaGiros.AuditarIntegracion472(tipoIntegracion, peticionSerializada, respuestaSerializada);
                }
                catch (Exception ex)
                { }
            });

            return respuesta;
        }

        /// <summary>
        /// valida el usuario y contraseña para consumir el servicio por 472
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        public bool ValidarPassword472(credencialDTO credencial)
        {
            return fachadaGiros.ValidarPassword472(credencial);
        }

        #endregion

        #region Tracking

        public INTrackingGuiaDC ConsultarTrackingGuia(credencialDTO credencial, long numeroGuia)
        {
            int idCliente = INRepositorioIntegraciones.Instancia.ValidarCredencial(credencial);
            if (idCliente != 0)
            {
                ADGuia GuiaConsultada = fachadaAdmisiones.ConsultarGuia(idCliente, numeroGuia);
                if (GuiaConsultada.IdAdmision != 0)
                {
                    return new INTrackingGuiaDC
                    {
                        respuestaAcceso = "CORRECTO",
                        descripcionRespuestaAcceso = "Acceso valido",
                        observacionConsultaGuia = "Guia consultada con exito",
                        guiaConsultada = GuiaConsultada
                    };
                }
                else
                {
                    return new INTrackingGuiaDC
                    {
                        observacionConsultaGuia = "Guia no encontrada",
                        respuestaAcceso = "CORRECTO",
                        descripcionRespuestaAcceso = "Acceso valido"
                    };
                }

            }
            else
            {
                return new INTrackingGuiaDC
                {
                    respuestaAcceso = "FALLIDO",
                    descripcionRespuestaAcceso = "Usuario y/o contraseña no corresponden"
                };

            }
        }

        #endregion

        #region Yanbal
        /// <summary>
        /// Proceso que consume el servicio de seguridad de yanbal para solicitar token o renovarlo
        /// </summary>
        /// <returns>Token y key de seguridad</returns>
        public INTRespuestaProceso SolicitarToken()
        {

            ServicioSeguridadYanbal.IntegracionWSReq obj = new ServicioSeguridadYanbal.IntegracionWSReq();
            ServicioSeguridadYanbal.Cabecera cabeceraServ = new ServicioSeguridadYanbal.Cabecera();
            ServicioSeguridadYanbal.Detalle detalleServ = new ServicioSeguridadYanbal.Detalle();
            ServicioSeguridadYanbal.SolicitarTokenEnvType user = new ServicioSeguridadYanbal.SolicitarTokenEnvType();
            ServicioSeguridadYanbal.CodigoPaisODType[] codPais = { new ServicioSeguridadYanbal.CodigoPaisODType() };
            cabeceraServ.codigoAplicacion = Properties.Resources.CodigoAplicacionXml;
            cabeceraServ.codigoInterfaz = Properties.Resources.CodigoInterfazTokenXml;
            cabeceraServ.codigoPais = Properties.Resources.CodigoPaisXml;
            cabeceraServ.usuarioAplicacion = ConfigurationManager.AppSettings.Get("Controller.Integracion.Yanbal.Usuario"); 
            obj.cabecera = cabeceraServ;
            codPais[0].valor = Properties.Resources.CodigoPaisODXml;
            obj.cabecera.codigosPaisOD = codPais;
            user.keySeguridad = ConfigurationManager.AppSettings.Get("Controller.Integracion.Yanbal.KeySeguridadXml");
            user.password = ConfigurationManager.AppSettings.Get("Controller.Integracion.Yanbal.Password");
            obj.detalle = detalleServ;
            obj.detalle.parametros = user;
            ServicioSeguridadYanbal.WSSeguridadTokenImplClient Servicio = new ServicioSeguridadYanbal.WSSeguridadTokenImplClient();
            ServicioSeguridadYanbal.IntegracionWSResp token = Servicio.solicitarToken(obj);

            if (token.detalle.datos == null)
            {
                throw new FaultException<ControllerException>(new ControllerException("YAN",token.detalle.respuesta.codigoRespuesta, token.detalle.respuesta.mensajeRespuesta));
            }

            return new INTRespuestaProceso()
            {
                idtransaccion = Convert.ToInt32(token.cabecera.idTransaccion),
                Mensaje = token.detalle.datos.keySeguridad,
                Token = token.detalle.datos.idToken
            };


        }
        /// <summary>
        /// Consume el servicio eventos expuesto por yanbal
        /// </summary>
        /// <param name="eventos"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private INTRespuestaProceso ConsumirServicioEventos(ADEstadoHomologacionYanbal[] eventos, INTRespuestaProceso token)
        {
            INTRespuestaProceso Respuesta = new INTRespuestaProceso();
            
                XmlDocument xmlDoc = new XmlDocument();
                XmlDocument xmlResponse = new XmlDocument();
                
                string rutaArchivo = ConfigurationManager.AppSettings.Get("Controller.Integracion.Yanbal.Ruta.Eventos");

                if (File.Exists(rutaArchivo))
                {
                    string DatosXml;
                    StreamReader srFile = new StreamReader(rutaArchivo);
                    using (srFile) { DatosXml = srFile.ReadToEnd(); }

                    DatosXml = HttpUtility.UrlDecode(DatosXml);
                    xmlDoc.LoadXml(DatosXml + Environment.NewLine);

                    AsignarDatos_QUOTE_aXML(ref xmlDoc, eventos, token);

                    XmlTextWriter Request;
                    XmlTextWriter Response;
                    //Graba el Response.XML del Request en disco
                    string pathFile = ConfigurationManager.AppSettings.Get("Controller.Integracion.Yanbal.Ruta.Request");

                    Request = new XmlTextWriter(pathFile, new UTF8Encoding(false));
                    xmlDoc.Save(Request);
                    Request.Close();
                    xmlResponse = PostXMLTransaction(ConfigurationManager.AppSettings.Get("Controller.Integracion.Yanbal.Ruta.EndPoint"), xmlDoc);

                    //Graba el Response.XML del Response en disco
                    string pathFileResponse = ConfigurationManager.AppSettings.Get("Controller.Integracion.Yanbal.Ruta.Response");
                    Response = new XmlTextWriter(pathFileResponse, new UTF8Encoding(false));
                    xmlResponse.Save(Response);
                    Response.Close();

                    
                    DALAuditoria.Instancia.AuditarIntegracion("IntYanbal", xmlDoc.InnerXml,xmlResponse.InnerXml);
                    //Lee el response.XML
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(pathFileResponse);

                    XmlNodeList detalle = xDoc.GetElementsByTagName("detalle");
                    XmlNodeList xLista = ((XmlElement)detalle[0]).GetElementsByTagName("respuesta");

                    foreach (XmlElement nodo in xLista)
                    {
                        Respuesta.Codigo = Convert.ToInt32(nodo["codigoRespuesta"].InnerText);
                        Respuesta.Mensaje = nodo["mensajeRespuesta"].InnerText;
                    }
                }
                else
                {
                    Respuesta.Codigo = 999;
                    Respuesta.Mensaje = string.Concat("Archivo plantillaQuote o Ruta no Existe: ", rutaArchivo);
                }
           
            return Respuesta;
        }
        public static XmlDocument PostXMLTransaction(string v_strURL, XmlDocument v_objXMLDoc)
        {
            //Declare XMLResponse document
            XmlDocument XMLResponse = null;

            //Declare an HTTP-specific implementation of the WebRequest class.
            HttpWebRequest objHttpWebRequest;

            //Declare an HTTP-specific implementation of the WebResponse class
            HttpWebResponse objHttpWebResponse = null;

            //Declare a generic view of a sequence of bytes
            Stream objRequestStream = null;
            Stream objResponseStream = null;

            //Declare XMLReader
            XmlTextReader objXMLReader;

            //Creates an HttpWebRequest for the specified URL.
            objHttpWebRequest = (HttpWebRequest)WebRequest.Create(v_strURL);

            objHttpWebRequest.Timeout = (15*60) * 1000;
                //---------- Start HttpRequest 

            //Set HttpWebRequest properties
            byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(v_objXMLDoc.InnerXml);
                objHttpWebRequest.Method = "POST";
                objHttpWebRequest.ContentLength = bytes.Length;
                objHttpWebRequest.ContentType = "text/xml; encoding='utf-8'";                
                //Get Stream object 
                objRequestStream = objHttpWebRequest.GetRequestStream();

                //Writes a sequence of bytes to the current stream 
                objRequestStream.Write(bytes, 0, bytes.Length);

                //Close stream
                objRequestStream.Close();

                //---------- End HttpRequest

                //Sends the HttpWebRequest, and waits for a response.
                objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();

                //---------- Start HttpResponse
                if (objHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    //Get response stream 
                    objResponseStream = objHttpWebResponse.GetResponseStream();

                    //Load response stream into XMLReader
                    objXMLReader = new XmlTextReader(objResponseStream);

                    //Declare XMLDocument
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(objXMLReader);

                    //Set XMLResponse object returned from XMLReader
                    XMLResponse = xmldoc;

                    //Close XMLReader
                    objXMLReader.Close();
                }

                //Close HttpWebResponse
                objHttpWebResponse.Close();
           
                //Close connections
                objRequestStream.Close();
                objResponseStream.Close();
                objHttpWebResponse.Close();

                //Release objects
                objXMLReader = null;
                objRequestStream = null;
                objResponseStream = null;
                objHttpWebResponse = null;
                objHttpWebRequest = null;
           

            //Return
            return XMLResponse;
        }
        /// <summary>
        /// Asigna datos al xml
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="eventosEnviar"></param>
        /// <param name="token"></param>
        private void AsignarDatos_QUOTE_aXML(ref XmlDocument xmlDoc, ADEstadoHomologacionYanbal[] eventosEnviar, INTRespuestaProceso token)
        {

            Asignar_Datos_QUOTE(ref xmlDoc, eventosEnviar, token);
        }
        /// <summary>           
        /// Crea el xml para consumir el servicio
        /// </summary>
        /// <param name="xmlDoc"></param>
        private void Asignar_Datos_QUOTE(ref XmlDocument xmlDoc, ADEstadoHomologacionYanbal[] eventosEnviar, INTRespuestaProceso token)
        {
            XmlElement integracionWSReq = (XmlElement)xmlDoc.GetElementsByTagName("integracionWSReq").Item(0);
            XmlNode cabecera = integracionWSReq.GetElementsByTagName("cabecera").Item(0);
            cabecera["codigoInterfaz"].InnerText = Properties.Resources.CodigoInterfazEventosXml;
            cabecera["usuarioAplicacion"].InnerText = Properties.Resources.UsuarioAplicacionXml;
            cabecera["codigoAplicacion"].InnerText = Properties.Resources.CodigoAplicacionXml;
            cabecera["codigoPais"].InnerText = Properties.Resources.CodigoPaisXml;
            cabecera["codigoPaisOD"].InnerText = Properties.Resources.CodigoPaisODXml;
            XmlNode seguridad = integracionWSReq.GetElementsByTagName("seguridad").Item(0);
            seguridad["keySeguridad"].InnerText = token.Mensaje;
            seguridad["token"].InnerText = token.Token;
            XmlNode detalle = (XmlElement)xmlDoc.GetElementsByTagName("detalle").Item(0);
            XmlNode parametros = (XmlElement)xmlDoc.GetElementsByTagName("parametros").Item(0);
            XmlNode eventos = (XmlElement)xmlDoc.GetElementsByTagName("eventos").Item(0);
            parametros["tipEnvio"].InnerText = Properties.Resources.TipEnvioXML;
            eventos.RemoveChild(eventos["evento"]);
            foreach (var item in eventosEnviar)
            {
                XmlElement evento = (XmlElement)xmlDoc.CreateElement("evento");

                AppendTextElement(evento, "codEvento", item.ADM_DescripcionEstado);
                AppendTextElement(evento, "fecHorCarga", item.ADM_FechaAdmision.ToString("yyyy-MM-dd hh:mm:ss"));
                AppendTextElement(evento, "codOrden", item.ADM_NoPedido);
                AppendTextElement(evento, "tipOrden", "BD");
                AppendTextElement(evento, "numGuiRemision", item.ADM_NumeroGuia);
                AppendTextElement(evento, "codDirectora",Properties.Resources.CodigoDirectoraXml);
                AppendTextElement(evento, "codConsultora", item.ADM_IdDestinatario);
                AppendTextElement(evento, "fecHorEvento", item.EGT_FechaGrabacion.ToString("yyyy-MM-dd hh:mm:ss"));
                AppendTextElement(evento, "tipEvento", item.HY_TipoEventoYanbal);
                AppendTextElement(evento, "cooEvento", item.HY_CodEventoYanbal);
                AppendTextElement(evento, "desEvento", item.HY_DescripcionYanbal);
                AppendTextElement(evento, "usuEvento", "INTERRAPID");
                AppendTextElement(evento, "tipParentesco", "");
                AppendTextElement(evento, "receptorNombre", "");
                AppendTextElement(evento, "receptorID", "");
                AppendTextElement(evento, "ubiRetorno", item.CES_Nombre);
                AppendTextElement(evento, "observacion", "");
                AppendTextElement(evento, "motivo", item.HY_MotivoYanbal);
                AppendTextElement(evento, "motivoDesc", item.HY_DescripcionMotivo);
                AppendTextElement(evento, "tipBulto", item.ADM_NumeroBolsaSeguridad);
                eventos.AppendChild(evento);
            }
        }
        /// <summary>
        /// Asigna atributos a los nodos en el XML
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private XmlElement AppendTextElement(XmlElement parent, string name, string value)
        {
            var elem = parent.OwnerDocument.CreateElement(name);
            parent.AppendChild(elem);
            elem.InnerText = value;
            return elem;
        }
        /// <summary>
        /// Metodo que consume el servicio de yanbal (Envia los estados y motivos)
        /// </summary>
        /// <param name="EventoRegistra">Lista de eventos y motivos</param>
        /// <param name="token">Se recibe el key y el token para armar la cabecera</param>
        /// <returns>Respuesta del proceso</returns>
        public INTRespuestaProceso RegistrarEventos(ADEstadoHomologacionYanbal[ ] EventoRegistra, INTRespuestaProceso token)
        {
            INTRespuestaProceso Respuesta = new INTRespuestaProceso();
           
             Respuesta = ConsumirServicioEventos(EventoRegistra, token);

             if (Respuesta.Codigo == 0000)
              {
                  INRepositorioIntegraciones.Instancia.ActualizarParametroIntegracionYanbal();
              }
           return Respuesta;

        }
        /// <summary>
        /// Consulta los cambios realizados sobre las guias de yanbal en el intervalo de tiempo de la ultima ejecucion del proceso y el momento actual
        /// </summary>
        /// <returns>Lista con los estados y motivos homologados </returns>
        public List<ADEstadoHomologacionYanbal> ConsultarTrazaEventos()
        {
            return INRepositorioIntegraciones.Instancia.ConsultarMovimientosYanbal();

        }
        /// <summary>
        /// Consulta en parametros para conocer la frecuencia de ejecucion
        /// </summary>
        /// <returns></returns>
        public int ConsultarFrecuenciaEjecucion()
        {
            return INRepositorioIntegraciones.Instancia.FrecuenciaEjecucion();
        }


        #endregion
        #endregion

    }
}
