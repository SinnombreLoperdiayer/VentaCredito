using CO.Controller.Servidor.Integracion.SatrackControlTrafico.entidades;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun.Util;
using Newtonsoft.Json;
using System.Data;
using System.Web.Script.Serialization;
using System.Xml;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Controller.Servidor.Integracion.SatrackControlTrafico
{
    public class ControlTraficoSatrack
    {
        private static readonly ControlTraficoSatrack instancia = new ControlTraficoSatrack();

        public static ControlTraficoSatrack Instancia
        {
            get { return ControlTraficoSatrack.instancia; }
        }

        private ControlTraficoSatrack() { }

        /// <summary>
        /// Crea itinerario para transporte(retorna un XML dando constancia)
        /// </summary>
        /// <param name="DatosXml"></param>
        /// <returns></returns>
        /// 
        public string programarItinerarioXML()
        {
            List<string> listaCod = new List<string>();
            string cod1 = "Region1";
            listaCod.Add(cod1);
            cod1 = "Region2";
            listaCod.Add(cod1);
            credencialesTrafico credencial = new credencialesTrafico();

            List<ItinerarioProgramarItinerarioXML> Temp = new List<ItinerarioProgramarItinerarioXML>();

            ItinerarioProgramarItinerarioXML Param = new ItinerarioProgramarItinerarioXML()
            {
                placa = "placaEjemplo1",
                ruta = "rutaEjemplo1",
                parametros = new ParametrosProgramarItinerarioXML()
                {
                    trafico = new TraficoProgramarItinerarioXML()
                    {
                        agencia = "123",
                        planviaje = "1234567",
                        campo1 = "001457-0051;C02311245",
                        nombreconductor = "Javier Gonzales",
                        telefonoconductor = "30156987415"
                    },
                    canbus = new CanbusProgramarItinerarioXML()
                    {
                        canpeso = 8
                    },
                    disponible = "1",
                    fechadespacho = "20120520 22:30",
                    regionescargue = new RegCargueProgramarItinerarioXML()
                    {
                        codigoregion = listaCod
                    },
                    placatemporal = "PlacaTemporal1"
                }
            };

            Temp.Add(Param);

            Param = new ItinerarioProgramarItinerarioXML()
            {
                placa = "placaEjemplo1",
                ruta = "rutaEjemplo1"
            };

            Temp.Add(Param);

            Param = new ItinerarioProgramarItinerarioXML()
            {
                placa = "placaEjemplo1",
                ruta = "rutaEjemplo1",
                parametros = new ParametrosProgramarItinerarioXML()
                {
                    trafico = new TraficoProgramarItinerarioXML()
                    {
                        campo1 = "001457-0051;C02311245",
                    },
                    canbus = new CanbusProgramarItinerarioXML()
                    {
                        canpeso = 8
                    },
                    disponible = "0",
                    fechadespacho = "20120520 22:30",
                }
            };

            Temp.Add(Param);


            RequestProgramarItinerarioXML ProgItXml = new RequestProgramarItinerarioXML()
            {
                programacion = new ProgramarItinerarioXML
                {
                    itinerario = Temp
                }
            };

            var json = new JavaScriptSerializer().Serialize(ProgItXml);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            SatrackControlTrafico.WSControlTraficoSoapClient CTweb = new SatrackControlTrafico.WSControlTraficoSoapClient();

            string retorno = CTweb.ProgramarItinerarioXML(credencial.Usuario, credencial.Password, xmlRequest);

            return retorno;
        }

        /// <summary>
        /// Crea itinerario para transporte(retorna un DataTable dando constancia)
        /// </summary>
        /// <param name="DatosXml"></param>
        /// <returns></returns>
        /// 
        public DataTable programarItinerarioDataTable() 
        {
            List<string> listaCod = new List<string>();
            string cod1 = "Region1";
            listaCod.Add(cod1);
            cod1 = "Region2";
            listaCod.Add(cod1);
            credencialesTrafico credencial = new credencialesTrafico();

            List<ItinerarioProgramarItinerarioXML> Temp = new List<ItinerarioProgramarItinerarioXML>();

            ItinerarioProgramarItinerarioXML Param = new ItinerarioProgramarItinerarioXML()
            {
                placa = "placaEjemplo1",
                ruta = "rutaEjemplo1",
                parametros = new ParametrosProgramarItinerarioXML()
                {
                    trafico = new TraficoProgramarItinerarioXML()
                    {
                        agencia = "123",
                        planviaje = "1234567",
                        campo1 = "001457-0051;C02311245",
                        nombreconductor = "Javier Gonzales",
                        telefonoconductor = "30156987415"
                    },
                    canbus = new CanbusProgramarItinerarioXML()
                    {
                        canpeso = 8
                    },
                    disponible = "1",
                    fechadespacho = "20120520 22:30",
                    regionescargue = new RegCargueProgramarItinerarioXML()
                    {
                        codigoregion = listaCod
                    },
                    placatemporal = "PlacaTemporal1"
                }
            };

            Temp.Add(Param);

            Param = new ItinerarioProgramarItinerarioXML()
            {
                placa = "placaEjemplo1",
                ruta = "rutaEjemplo1"
            };

            Temp.Add(Param);

            Param = new ItinerarioProgramarItinerarioXML()
            {
                placa = "placaEjemplo1",
                ruta = "rutaEjemplo1",
                parametros = new ParametrosProgramarItinerarioXML()
                {
                    trafico = new TraficoProgramarItinerarioXML()
                    {
                        campo1 = "001457-0051;C02311245",
                    },
                    canbus = new CanbusProgramarItinerarioXML()
                    {
                        canpeso = 8
                    },
                    disponible = "0",
                    fechadespacho = "20120520 22:30",
                }
            };

            Temp.Add(Param);


            RequestProgramarItinerarioXML ProgItXml = new RequestProgramarItinerarioXML()
            {
                programacion = new ProgramarItinerarioXML
                {
                    itinerario = Temp
                }
            };

            var json = new JavaScriptSerializer().Serialize(ProgItXml);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            SatrackControlTrafico.WSControlTraficoSoapClient CTweb = new SatrackControlTrafico.WSControlTraficoSoapClient();

            DataTable retorno = CTweb.ProgramarItinerarioDataTable(credencial.Usuario, credencial.Password, xmlRequest);

            return retorno;
        }

        /// <summary>
        /// Crea un XML de consulta de placas por fecha
        /// </summary>
        /// <param name="DatosConsulta"></param>
        /// <returns></returns>
        /// 
        public string ConsultarPlacasPorFechaXml()
        {
            credencialesTrafico credencial = new credencialesTrafico();
            SatrackControlTrafico.WSControlTraficoSoapClient CTweb = new SatrackControlTrafico.WSControlTraficoSoapClient();
            string retorno = CTweb.ConsultarPlacasFechaXml(credencial.Usuario, credencial.Password, "Inter1", 2011, 10, 1, 7,
                                                           0, 0, 2016, 1, 1, 0, 0, 0);
            return retorno;
        }

        /// <summary>
        /// Crea un DataTable de consulta de placas por fecha
        /// </summary>
        /// <param name="DatosConsulta"></param>
        /// <returns></returns>
        /// 
        public DataTable ConsultarPlacasPorFechaDataTable()
        {
            credencialesTrafico credencial = new credencialesTrafico();
            SatrackControlTrafico.WSControlTraficoSoapClient CTweb = new SatrackControlTrafico.WSControlTraficoSoapClient();
            DataTable retorno = CTweb.ConsultarPlacasFechaDataTable(credencial.Usuario, credencial.Password, "Inter1", 2011, 10, 1, 7,
                                                           0, 0, 2016, 1, 1, 0, 0, 0);
            return retorno;
        }

        /// <summary>
        /// Crea un XML de consulta de placas por identificador
        /// </summary>
        /// <param name="DatosConsulta"></param>
        /// <returns></returns>
        /// 
        public string ConsultarPlacasPorIdentificadorXml()
        {
            credencialesTrafico credencial = new credencialesTrafico(); 
            SatrackControlTrafico.WSControlTraficoSoapClient CTweb = new SatrackControlTrafico.WSControlTraficoSoapClient();
            string retorno = CTweb.ConsultarPlacasIdentificadorXml(credencial.Usuario, credencial.Password, 1, "*",
                                                                    200);
            return retorno;
        }

        /// <summary>
        /// Crea un DataTable de consulta de placas por identificador
        /// </summary>
        /// <param name="DatosConsulta"></param>
        /// <returns></returns>
        /// 
        public DataTable ConsultarPlacasPorIdentificadorDataTable()
        {
            credencialesTrafico credencial = new credencialesTrafico();
            SatrackControlTrafico.WSControlTraficoSoapClient CTweb = new SatrackControlTrafico.WSControlTraficoSoapClient();
            DataTable retorno = CTweb.ConsultarPlacasIdentificadorDataTable(credencial.Usuario, credencial.Password, 1, 
                                                                            "*", 200);
            return retorno;
        }
    }
}
