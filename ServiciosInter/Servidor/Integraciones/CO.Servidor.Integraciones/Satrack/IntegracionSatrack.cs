using CO.Controller.Servidor.Integraciones.AccesoDatos;
using CO.Servidor.Integraciones.ServicioTraficoSatrack;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CO.Servidor.Integraciones.Satrack
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public class IntegracionSatrack
    {

        #region  Instancia

        private static readonly IntegracionSatrack instancia = new IntegracionSatrack();
        /// <summary>
        /// Retorna una instancia de administracion de produccion
        /// /// </summary>
        public static IntegracionSatrack Instancia
        {
            get { return instancia; }
        }
        #endregion  Instancia

        /// <summary>
        /// Programa un conjunto de Itinerarios en Satrack
        /// </summary>
        /// <param name="Programacion"></param>
        /// <returns></returns>
        public string ProgramarItinerario(List<INItinerarioDC> Programacion)
        {
            using (WSControlTraficoSoapClient ServicioTrafico = new WSControlTraficoSoapClient())
                try
                {
                    string requestProgramacion = CreaRequestProgramacion(Programacion).InnerXml;

                    var usuarioSatrack = PAAdministrador.Instancia.ConsultarParametrosFramework(Properties.Resources.UsuSatrack);
                    var passwordSatrack = PAAdministrador.Instancia.ConsultarParametrosFramework(Properties.Resources.PasSatrack);
                    string resultado = ServicioTrafico.ProgramarItinerarioXML(usuarioSatrack, passwordSatrack, requestProgramacion);
                    DALAuditoria.Instancia.AuditarIntegracion(Properties.Resources.IdAuditaSatrack, requestProgramacion.ToString(), resultado);

                    return ObtenerMensajeResultado(resultado);
                }
                catch (Exception ex)
                {
                    ServicioTrafico.Abort();
                    throw new Exception(ex.Message);
                }

        }

        private string ObtenerMensajeResultado(string resultado)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(resultado);
            XmlNodeList descripcion = xDoc.GetElementsByTagName("descripcion");
            if (descripcion.Count > 0)
            {
                return descripcion[0].InnerText;
            }

            return string.Empty;
        }

        /// <summary>
        /// Crea xml para programar el itinerario
        /// </summary>
        /// <param name="programacion"></param>
        /// <returns></returns>
        private XmlDocument CreaRequestProgramacion(List<INItinerarioDC> programacion)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.DocumentElement;

            XmlElement elemProgramacion = doc.CreateElement(string.Empty, "programacion", string.Empty);
            doc.AppendChild(elemProgramacion);

            foreach (var prog in programacion)
            {
                XmlElement elemItinerario = doc.CreateElement(string.Empty, "itinerario", string.Empty);
                elemProgramacion.AppendChild(elemItinerario);

                AsignaElementoAPadre(doc, "placa", prog.Placa, ref elemItinerario);
                AsignaElementoAPadre(doc, "ruta", prog.Ruta, ref elemItinerario);

                if (prog.Parametros != null)
                {
                    XmlElement elemParametros = doc.CreateElement(string.Empty, "parametros", string.Empty);
                    elemItinerario.AppendChild(elemParametros);

                    XmlElement elemTrafico = doc.CreateElement(string.Empty, "trafico", string.Empty);
                    elemParametros.AppendChild(elemTrafico);

                    if (prog.Parametros.Trafico != null)
                    {
                        AsignaElementoAPadre(doc, "agencia", prog.Parametros.Trafico.Agencia, ref elemTrafico);
                        AsignaElementoAPadre(doc, "planviaje", prog.Parametros.Trafico.Planviaje, ref elemTrafico);
                        AsignaElementoAPadre(doc, "campo1", prog.Parametros.Trafico.Campo1, ref elemTrafico);
                        AsignaElementoAPadre(doc, "nombreconductor", prog.Parametros.Trafico.Nombreconductor, ref elemTrafico);
                        AsignaElementoAPadre(doc, "telefonoconductor", prog.Parametros.Trafico.Telefonoconductor, ref elemTrafico);
                    }

                    if (prog.Parametros.Canbus != null)
                    {
                        XmlElement elemCanbus = doc.CreateElement(string.Empty, "canbus", string.Empty);
                        elemParametros.AppendChild(elemCanbus);

                        AsignaElementoAPadre(doc, "canpeso", prog.Parametros.Canbus.Canpeso, ref elemCanbus);
                    }

                    AsignaElementoAPadre(doc, "disponible", prog.Parametros.Disponible, ref elemParametros);
                    AsignaElementoAPadre(doc, "fechadespacho", prog.Parametros.Fechadespacho, ref elemParametros);

                    if (prog.Parametros.Regionescargue != null)
                    {
                        XmlElement elemRegionesCargue = doc.CreateElement(string.Empty, "regionescargue", string.Empty);
                        elemParametros.AppendChild(elemRegionesCargue);

                        foreach (var codRegion in prog.Parametros.Regionescargue.CodigoRegion)
                        {
                            AsignaElementoAPadre(doc, "codigoregion", codRegion, ref elemRegionesCargue);
                        }
                    }

                    AsignaElementoAPadre(doc, "placatemporal", prog.Parametros.Placatemporal, ref elemParametros);
                }
            }

            return doc;
        }

        /// <summary>
        /// creaun nuevo elemento y lo asigna al elemento padre
        /// </summary>
        /// <param name="documento"> Documento xml</param>
        /// <param name="nombreElemento">Nombre del elemento a crear</param>
        /// <param name="valor">valor del elemento a crear</param>
        /// <param name="elementoPadre">elemento padre donde se asina</param>
        private void AsignaElementoAPadre(XmlDocument documento, string nombreElemento, string valor, ref XmlElement elementoPadre)
        {

            if (valor != null)
            {
                XmlElement elementoNuevo = documento.CreateElement(string.Empty, nombreElemento, string.Empty);
                XmlText text = documento.CreateTextNode(valor);
                elementoNuevo.AppendChild(text);
                elementoPadre.AppendChild(elementoNuevo);
            }
        }

    }
}
