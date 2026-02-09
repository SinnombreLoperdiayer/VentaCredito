using CO.Controller.Servidor.Integracion.MinTransporte.Entidades;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.IO;

namespace CO.Controller.Servidor.Integracion.MinTransporte
{
    public class MinisterioTransporte
    {

        private static readonly MinisterioTransporte instancia = new MinisterioTransporte();

        public static MinisterioTransporte Instancia
        {
            get { return MinisterioTransporte.instancia; }
        }

        private MinisterioTransporte()
        {
        }


        /// <summary>
        /// Consume el servicio expuesto por el ministerio de transporte 
        /// </summary>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        private string ConsumirServicioAtenderMensajeRNDC(string mensaje)
        {
            MinTransporte.IBPMServicesservice proxy = new MinTransporte.IBPMServicesservice();
            string retorno = proxy.AtenderMensajeRNDC(mensaje);
            return retorno;
        }

        /// <summary>
        /// Crea un tercero sin licencia (Titular manifiesto)
        /// </summary>
        /// <param name="TitularManifiesto"></param>
        /// 

        public void CrearTerceroTitularManifiesto(POPropietarioVehiculo TitularManifiesto)
        {
            RequestCreacionTercerosMin titularManifiesto = new RequestCreacionTercerosMin()
            {
                root = new CreacionTercerosMin()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 11,
                        tipo = 1
                    },
                    variables = new VariablesCreacionTercerosMin() 
                    {
                        CODMUNICIPIORNDC = "11001000",
                        CODTIPOIDTERCERO = "C",
                        NOMENCLATURADIRECCION = "CALLE 156 # 9-50",
                        NOMIDTERCERO = "Linda",
                        NUMIDTERCERO = "51760125",
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMTELEFONOCONTACTO = 6753733,
                        PRIMERAPELLIDOIDTERCERO = "BARRETO",
                        SEGUNDOAPELLIDOIDTERCERO = "AREVALO",
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(titularManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<CODSEDETERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NOMSEDETERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODCATEGORIALICENCIACONDUCCION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMLICENCIACONDUCCION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHAVENCIMIENTOLICENCIA />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);


        }

        /// <summary>
        /// Crea un tercero sin licencia (propietario)
        /// </summary>
        /// <param name="propietario"></param>
        /// 

        public void CrearTerceroPropietario(POPropietarioVehiculo propietario)
        {
            RequestCreacionTercerosMin titularManifiesto = new RequestCreacionTercerosMin()
            {
                root = new CreacionTercerosMin()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 11,
                        tipo = 1
                    },
                    variables = new VariablesCreacionTercerosMin()
                    {
                        CODMUNICIPIORNDC = "11001000",
                        CODTIPOIDTERCERO = "C",
                        NOMENCLATURADIRECCION = "CALLE 156 # 9-50",
                        NOMIDTERCERO = "Linda",
                        NUMIDTERCERO = "51760125",
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMTELEFONOCONTACTO = 6753733,
                        PRIMERAPELLIDOIDTERCERO = "BARRETO",
                        SEGUNDOAPELLIDOIDTERCERO = "AREVALO"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(titularManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<CODSEDETERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NOMSEDETERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODCATEGORIALICENCIACONDUCCION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMLICENCIACONDUCCION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHAVENCIMIENTOLICENCIA />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest); 

        }

        /// <summary>
        /// Crea un tercero sin licencia (tenedor)
        /// </summary>
        /// <param name="tenedor"></param>
        /// 

        public void CrearTerceroTenedor(POTenedorVehiculo tenedor)
        {
            RequestCreacionTercerosMin titularManifiesto = new RequestCreacionTercerosMin()
            {
                root = new CreacionTercerosMin()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 11,
                        tipo = 1
                    },
                    variables = new VariablesCreacionTercerosMin()
                    {
                        CODMUNICIPIORNDC = "11001000",
                        CODTIPOIDTERCERO = "C",
                        NOMENCLATURADIRECCION = "CALLE 156 # 9-50",
                        NOMIDTERCERO = "Paola",
                        NUMIDTERCERO = "51760125",
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMTELEFONOCONTACTO = 6753733,
                        PRIMERAPELLIDOIDTERCERO = "BARRETO",
                        SEGUNDOAPELLIDOIDTERCERO = "AREVALO"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(titularManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<CODSEDETERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NOMSEDETERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODCATEGORIALICENCIACONDUCCION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMLICENCIACONDUCCION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHAVENCIMIENTOLICENCIA />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest); 
        }

        /// <summary>
        /// Crea un tercero con licencia (conductor)
        /// </summary>
        /// <param name="conductor"></param>
        /// 

        public void CrearTerceroConductor(POConductores conductor)
        {
            RequestCreacionTercerosMin titularManifiesto = new RequestCreacionTercerosMin()
            {
                root = new CreacionTercerosMin()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 11,
                        tipo = 1
                    },
                    variables = new VariablesCreacionTercerosMin()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CODTIPOIDTERCERO = "C",
                        NUMIDTERCERO = "51760125",
                        NOMIDTERCERO = "Paola",
                        PRIMERAPELLIDOIDTERCERO = "BARRETO",
                        SEGUNDOAPELLIDOIDTERCERO = "AREVALO",
                        NUMTELEFONOCONTACTO = 6753733,
                        NOMENCLATURADIRECCION = "CALLE 156 # 9-50",
                        CODMUNICIPIORNDC = "11001000",
                        CODCATEGORIALICENCIACONDUCCION = 6,
                        NUMLICENCIACONDUCCION = 110013973659,
                        FECHAVENCIMIENTOLICENCIA = "31/12/2012"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(titularManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;


            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<CODSEDETERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NOMSEDETERCERO />", "");
            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest); 

            
        }

        /// <summary>
        /// Crea un tercero sin licencia (destinatario)
        /// </summary>
        /// <param name="destinatario"></param>
        /// 

        public void CrearTerceroDestinatario(POPropietarioVehiculo destinatario)
        {
            RequestCreacionTercerosMin titularManifiesto = new RequestCreacionTercerosMin()
            {
                root = new CreacionTercerosMin()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 11,
                        tipo = 1
                    },
                    variables = new VariablesCreacionTercerosMin()
                    {
                        CODMUNICIPIORNDC = "11001000",
                        CODTIPOIDTERCERO = "C",
                        NOMENCLATURADIRECCION = "CALLE 156 # 9-50",
                        NOMIDTERCERO = "Paola",
                        NUMIDTERCERO = "51760125",
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMTELEFONOCONTACTO = 6753733,
                        PRIMERAPELLIDOIDTERCERO = "BARRETO",
                        SEGUNDOAPELLIDOIDTERCERO = "AREVALO"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(titularManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<CODSEDETERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NOMSEDETERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODCATEGORIALICENCIACONDUCCION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMLICENCIACONDUCCION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHAVENCIMIENTOLICENCIA />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest); 

        }

        /// <summary>
        /// Crea un tercero con NIT
        /// </summary>
        /// <param name="terceroNIT"></param>
        ///

        public void CrearTerceroNit(POPropietarioVehiculo terceroNIT)
        {
            RequestCreacionTercerosMin titularManifiesto = new RequestCreacionTercerosMin()
            {
                root = new CreacionTercerosMin()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 11,
                        tipo = 1
                    },
                    variables = new VariablesCreacionTercerosMin()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CODTIPOIDTERCERO = "N",
                        NUMIDTERCERO = "830047668-1",
                        NOMIDTERCERO = "TRANSFORMER LTDA",
                        CODSEDETERCERO = "1",
                        NOMSEDETERCERO = "PRINCIPAL",
                        NUMTELEFONOCONTACTO = 2567126,
                        NOMENCLATURADIRECCION = "CARRERA 17 # 88-23",
                        CODMUNICIPIORNDC = "11001000"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(titularManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<PRIMERAPELLIDOIDTERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<SEGUNDOAPELLIDOIDTERCERO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODCATEGORIALICENCIACONDUCCION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMLICENCIACONDUCCION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHAVENCIMIENTOLICENCIA />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest); 
        }

        /// <summary>
        /// Crea un vehiculo
        /// </summary>
        /// <param name="vehiculo"></param>
        ///
        public void CrearVehiculo(POVehiculo vehiculo)
        {
            RequestVehiculo miVehiculo = new RequestVehiculo()
            {
                root = new Vehiculo()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"

                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 12,
                        tipo = 1
                    },
                    variables = new VariablesVehiculo()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMPLACA = "WZH111",
                        CODCONFIGURACIONUNIDADCARGA = 50,
                        CODMARCAVEHICULOCARGA = 1,
                        CODLINEAVEHICULOCARGA = 373,
                        ANOFABRICACIONVEHICULOCARGA = 2010,
                        CODTIPOCOMBUSTIBLE = 1,
                        PESOVEHICULOVACIO = 8000,
                        CODCOLORVEHICULOCARGA = "9439",
                        CODTIPOCARROCERIA = 0,
                        CODTIPOIDPROPIETARIO = "C",
                        NUMIDPROPIETARIO = 51760125,
                        CODTIPOIDTENEDOR = "C",
                        NUMIDTENEDOR = 51760125,
                        NUMSEGUROSOAT = "AT131811151729",
                        FECHAVENCIMIENTOSOAT = "4/10/2016",
                        NUMNITASEGURADORASOAT = 8110191907,
                        CAPACIDADUNIDADCARGA = 1000,
                        UNIDADMEDIDACAPACIDAD = 1
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(miVehiculo);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

        /// <summary>
        /// Crea informacion de carga
        /// </summary>
        /// <param name="infoCarga"></param>
        ///

        public void CrearInfoDeCarga(POPropietarioVehiculo infoCarga)
        {
            RequestInformacionCarga infoDeCarga = new RequestInformacionCarga()
            {
                root = new InformacionDeCarga()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 1,
                        tipo = 1
                    },

                    variables = new VariablesInformacionDeCarga()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOINFORMACIONCARGA = "0001",
                        CODOPERACIONTRANSPORTE = "G",
                        CODTIPOEMPAQUE = 6,
                        CODNATURALEZACARGA = "1",
                        DESCRIPCIONCORTAPRODUCTO = "ACEITE DE BACALAO",
                        MERCANCIAINFORMACIONCARGA = "001504",
                        CANTIDADINFORMACIONCARGA = 20000,
                        UNIDADMEDIDACAPACIDAD = 1,
                        CODTIPOIDREMITENTE = "N",
                        NUMIDREMITENTE = 9300476681,
                        CODSEDEREMITENTE = 2,
                        CODTIPOIDDESTINATARIO = "C",
                        NUMIDDESTINATARIO = 53468994,
                        CODSEDEDESTINATARIO = "01A",
                        PACTOTIEMPOCARGUE = "SI",
                        HORASPACTOCARGA = 2,
                        MINUTOSPACTOCARGA = 10,
                        PACTOTIEMPODESCARGUE = "SI",
                        HORASPACTODESCARGUE = 2,
                        MINUTOSPACTODESCARGUE = 10
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(infoDeCarga);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<PESOCONTENEDORVACIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHACITAPACTADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORACITAPACTADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHACITAPACTADADESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORACITAPACTADADESCARGUEREMESA />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

        /// <summary>
        /// Crea informacion de viaje 
        /// </summary>
        /// <param name="infoViaje"></param>
        ///

        public void CrearInfoViaje(POPropietarioVehiculo infoViaje)
        {
            List<ManPreRemesa> lstManPreRemesa = new List<ManPreRemesa>();
            ManPreRemesa manPreRemesa = new ManPreRemesa();

            manPreRemesa.CONSECUTIVOINFORMACIONCARGA = "0001";
            lstManPreRemesa.Add(manPreRemesa);
            manPreRemesa.CONSECUTIVOINFORMACIONCARGA = "0020";
            lstManPreRemesa.Add(manPreRemesa);
            manPreRemesa.CONSECUTIVOINFORMACIONCARGA = "0035";
            lstManPreRemesa.Add(manPreRemesa);

            RequestInfoDeViaje infoDeViaje = new RequestInfoDeViaje()
            {
                root = new InformacionDeViaje()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 2,
                        tipo = 1
                    },

                    variables = new VariablesInformacionDeViaje()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOINFORMACIONVIAJE = "0001",
                        CODIDCONDUCTOR = "C",
                        NUMIDCONDUCTOR = 79616565,
                        NUMPLACA = "WZH111",
                        CODMUNICIPIOORIGENINFOVIAJE = "76001000",
                        CODMUNICIPIODESTINOINFOVIAJE = "11001000",
                        PREREMESAS = new PreRemesa()
                        {
                            MANPREREMESA = lstManPreRemesa
                        },
                        VALORFLETEPACTADOVIAJE = 3200000
                    }
                }
            };
            //var json2 = Newtonsoft.Json.JsonConvert.SerializeObject(infoDeViaje);
            var json = new JavaScriptSerializer().Serialize(infoDeViaje);
            
            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<PREREMESAS>","<PREREMESAS procesoid='44'>");

            xmlRequest = temp.InnerXml.ToString();
            

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
            
        
        }

        /// <summary>
        /// Crea informacion de Remesa terrestre
        /// </summary>
        /// <param name="infoRemesaTerrestre"></param>
        ///

        public void CrearRemesaTerrestre(POPropietarioVehiculo infoRemesaTerrestre) 
        {
            RequestRemesaTerrestre remesaTerrestre = new RequestRemesaTerrestre()
            {
                root = new RemesaTerrestre()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 3,
                        tipo = 1
                    },

                    variables = new VariablesRemesaTerrestre() 
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOREMESA = "0001",
                        CONSECUTIVOINFORMACIONCARGA = "0001",
                        CANTIDADCARGADA = 22000,
                        DUENOPOLIZA = "E",
                        NUMPOLIZATRANSPORTE = 5987462357,
                        FECHAVENCIMIENTOPOLIZACARGA = "08/08/2012",
                        COMPANIASEGURO = 8600025057,
                        FECHALLEGADACARGUE = "10/08/2011",
                        HORALLEGADACARGUEREMESA = "8:00",
                        FECHAENTRADACARGUE = "10/08/2011",
                        HORAENTRADACARGUEREMESA = "8:15",
                        FECHASALIDACARGUE = "10/08/2011",
                        HORASALIDACARGUEREMESA = "10:00"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(remesaTerrestre);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<CONSECUTIVOCARGADIVIDIDA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODOPERACIONTRANSPORTE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODNATURALEZACARGA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<UNIDADMEDIDACAPACIDAD />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODTIPOEMPAQUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<PESOCONTENEDORVACIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MERCANCIAREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<DESCRIPCIONCORTAPRODUCTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODTIPOIDREMITENTE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMIDREMITENTE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODSEDEREMITENTE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODTIPOIDDESTINATARIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMIDDESTINATARIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODSEDEDESTINATARIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORASPACTOCARGA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MINUTOSPACTOCARGA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORASPACTODESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MINUTOSPACTODESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODTIPOIDPROPIETARIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMIDPROPIETARIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODSEDEPROPIETARIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHACITAPACTADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORACITAPACTADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHACITAPACTADADESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORACITAPACTADADESCARGUEREMESA />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

        /// <summary>
        /// Crea Manifiesto de Carga
        /// </summary>
        /// <param name="infoManifiestoCargue"></param>
        ///

        public void CrearManifiestoDeCarga(POPropietarioVehiculo infoManifiestoCargue)
        {
            List<Remesa> listaRemesa = new List<Remesa>();
            Remesa elementoRemesa = new Remesa();

            elementoRemesa.CONSECUTIVOREMESA = "0001";
            listaRemesa.Add(elementoRemesa);
            elementoRemesa.CONSECUTIVOREMESA = "0020";
            listaRemesa.Add(elementoRemesa);

            RequestExpedirmanifiesto expidoManifiesto = new RequestExpedirmanifiesto()
            {
                root = new ExpedirManifiesto
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 4,
                        tipo = 1
                    },

                    variables = new VariablesExpedirManifiesto()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA = "0001",
                        CONSECUTIVOINFORMACIONVIAJE = "0001",
                        CODOPERACIONTRANSPORTE = "G",
                        FECHAEXPEDICIONMANIFIESTO = "29/07/2011",
                        CODMUNICIPIOORIGENMANIFIESTO = "76001000",
                        CODMUNICIPIODESTINOMANIFIESTO = "11001000",
                        CODIDTITULARMANIFIESTO = "C",
                        NUMIDTITULARMANIFIESTO = 79616565,
                        RETENCIONICAMANIFIESTOCARGA = "3",
                        VALORANTICIPOMANIFIESTO = 1000000,
                        CODMUNICIPIOPAGOSALDO = "11001000",
                        FECHAPAGOSALDOMANIFIESTO = "29/07/2011",
                        CODRESPONSABLEPAGOCARGUE = "E",
                        CODRESPONSABLEPAGODESCARGUE = "E",
                        OBSERVACIONES = "Se recomienda que en caso de accidente el conductor debe comunicarse al número celular 3102569871",
                        REMESASMAN = new RemesaMan() 
                        {
                            REMESA = listaRemesa
                        }
                    }

                }
            };

            var json = new JavaScriptSerializer().Serialize(expidoManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<REMESASMAN>", "<REMESASMAN procesoid='43'>");
            temp.InnerXml = temp.InnerXml.Replace("<NUMPLACA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODIDCONDUCTOR />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMIDCONDUCTOR />", "");
            temp.InnerXml = temp.InnerXml.Replace("<RETENCIONFUENTEMANIFIESTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MANNROMANIFIESTOTRANSBORDO />", "");


            xmlRequest = temp.InnerXml.ToString();


            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

        /// <summary>
        /// Crea Cumplir manifiesto de carga
        /// </summary>
        /// <param name="infoCumplirManifiesto"></param>
        ///

        public void CrearCumplirManifiestoCarga(POPropietarioVehiculo infoCumplirManifiesto)
        {
            RequestCumplirManifiesto cumplirManifiesto = new RequestCumplirManifiesto()
            {
                root = new CumplirManifiesto()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 6,
                        tipo = 1
                    },

                    variables = new VariablesCumplirManifiesto() 
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA ="0001",
                        TIPOCUMPLIDOMANIFIESTO = "C",
                        FECHAENTREGADOCUMENTOS = "10/08/2011",
                        VALORADICIONALHORASCARGUE = 1000000,
                        VALORDESCUENTOFLETE = 300000,
                        MOTIVOVALORDESCUENTOMANIFIESTO = "F",
                        VALORSOBREANTICIPO = 200000
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumplirManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOSUSPENSIONMANIFIESTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CONSECUENCIASUSPENSION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASDESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOVALORADICIONAL />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

        /// <summary>
        /// Crea Cumplir Remesa Terrestre 
        /// </summary>
        /// <param name="infoCumplirRemesaTerrestre"></param>
        ///

        public void CrearCumplirRemesaTerrestre(POPropietarioVehiculo infoCumplirRemesaTerrestre)
        {

            RequestCumplirRemesa cumploRemesa = new RequestCumplirRemesa()
            {
                root = new CumplirRemesa()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 5,
                        tipo = 1
                    },
                    variables = new VariablesCumplirRemesa() 
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOREMESA = "0001",
                        TIPOCUMPLIDOREMESA = "C",
                        FECHALLEGADADESCARGUE = "15/08/2011",
                        HORALLEGADADESCARGUECUMPLIDO = "14:10",
                        FECHAENTRADADESCARGUE = "15/08/2011",
                        HORAENTRADADESCARGUECUMPLIDO = "14:20",
                        FECHASALIDADESCARGUE = "15/08/2011",
                        HORASALIDADESCARGUECUMPLIDO = "16:20",
                        CANTIDADENTREGADA = 22000
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumploRemesa);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOSUSPENSIONREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CANTIDADCARGADA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<UNIDADMEDIDACAPACIDAD />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORALLEGADACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHAENTRADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORAENTRADACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHASALIDACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORASALIDACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }



        /// <summary>
        /// Crea Consulta Tercero
        /// </summary>
        /// <param name="infoConsultaTercero"></param>
        ///
        public void ConsultaTercero(POPropietarioVehiculo infoConsultaTercero)
        {
            RequestConsultaTerceros consulta = new RequestConsultaTerceros()
            {
                root = new ConsultaTerceros()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid =11,
                        tipo = 2
                    },
                    variables = "NOMIDTERCERO, NUMTELEFONOCONTACTO, NOMENCLATURADIRECCION, MUNICIPIORNDC",

                    documento = new DocumentoConsultaTercero() 
                    {
                        NUMNITEMPRESATRANSPORTE = "'830047668'",
                        CODTIPOIDTERCERO = "'N'",
                        NUMIDTERCERO = "'8300476681'"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(consulta);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);

        }

         /// <summary>
        /// Crea Consulta Vehiculo
        /// </summary>
        /// <param name="infoConsultaVehiculo"></param>
        ///
        public void ConsultaVehiculo(POPropietarioVehiculo infoConsultaVehiculo) 
        {
            RequestConsultaVehiculo consulta = new RequestConsultaVehiculo()
            {
                root = new ConsultaVehiculo
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 12,
                        tipo = 3
                    },
                    variables = "CODCONFIGURACIONUNIDADCARGA, ANOFABRICACIONVEHICULOCARGA, NUMIDPROPIETARIO, CODTIPOIDTENEDOR, NUMIDTENEDOR",
                    documento = new DocumentoConsultaVehiculo() 
                    {
                        NUMNITEMPRESATRANSPORTE = "'830047668'",
                        NUMPLACA = "'WZH111'"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(consulta);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

         /// <summary>
        /// Crea Consulta del cumplido de una remesa
        /// </summary>
        /// <param name="infoConsultaCumplRemesa"></param>
        ///
        public void ConsultaCumplimientoRemesa(POPropietarioVehiculo infoConsultaCumplRemesa)
        {
            RequestConsultaCumplidoRemesa consulta = new RequestConsultaCumplidoRemesa()
            {
                root = new ConsultaCumplidoRemesa() 
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 5,
                        tipo = 3
                    },
                    variables = "CONSECUTIVOREMESA,TIPOCUMPLIDOREMESA, MOTIVOSUSPENSIONREMESA, CANTIDADCARGADA, CANTIDADENTREGADA, UNIDADMEDIDACAPACIDAD, HORALLEGADACARGUEREMESA, FECHAENTRADACARGUE, FECHASALIDACARGUE, HORASALIDACARGUEREMESA, FECHALLEGADADESCARGUE, OBSERVACIONES",
                    documento = new DocumentoConsultaCumplidoRemesa()
                    {
                        NUMNITEMPRESATRANSPORTE = "'830047668'",
                        INGRESOID = "'332'"
                    }
                }
            };
            var json = new JavaScriptSerializer().Serialize(consulta);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

        /// <summary>
        /// Anula informacion de carga
        /// </summary>
        /// <param name="anularInfoCarga"></param>
        ///
        public void AnularInfoCarga(POPropietarioVehiculo anularInfoCarga) 
        {
            RequestAnularInfoCarga anularCarga = new RequestAnularInfoCarga()
            {
                root = new AnularInfoCarga()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 7,
                        tipo = 1
                    },

                    variables = new VariablesAnularInfoCarga() 
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOINFORMACIONCARGA = "WSCARGA2",
                        MOTIVOANULACIONINFOCARGA = "S"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(anularCarga);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

         /// <summary>
        /// Anula informacion de Viaje
        /// </summary>
        /// <param name="anularInfoViaje"></param>
        ///
        public void AnularInfoViaje(POPropietarioVehiculo anularInfoViaje) 
        {
            RequestAnularInfoViaje anularViaje = new RequestAnularInfoViaje()
            {
                root = new AnularInfoViaje()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 8,
                        tipo = 1
                    },
                    variables = new VariablesAnularInfoViaje() 
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOINFORMACIONVIAJE = "WSVIAJE1",
                        MOTIVOANULACIONINFOVIAJE = "S"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(anularViaje);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

         /// <summary>
        /// Anula informacion de remesa terrestre de cargue(se debe revisar)
        /// </summary>
        /// <param name="anularInfoRemesaTerrestre"></param>
        ///
        public void AnularRemesaTerrestreCarga(POPropietarioVehiculo anularInfoRemesaTerrestre) 
        {
            RequestAnularRemesaTerrestreCarga anularCargue = new RequestAnularRemesaTerrestreCarga()
            {
                root = new AnularRemesaTerrestreCarga()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 9,
                        tipo = 1
                    },
                    variables = new VariablesAnularRemesaTerrestreCarga() 
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOREMESA = "WSREMESA1",
                        MOTIVOREVERSAREMESA = "A",
                        MOTIVOANULACIONINFOVIAJE = "S"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(anularCargue);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest); 

        }


        /// <summary>
        /// Anula informacion de manifiesto de carga
        /// </summary>
        /// <param name="anularInfoMinifiestoCarga"></param>
        ///

        public void AnularManifiestoCarga(POPropietarioVehiculo anularInfoMinifiestoCarga)
        {
            RequestAnularManifiestoCarga anularManifiesto = new RequestAnularManifiestoCarga()
            {
                root = new AnularManifiestoCarga()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 32,
                        tipo = 1
                    },
                    variables = new VariablesAnularManifiestoCarga() 
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA = "WSMANIFIESTO1",
                        MOTIVOANULACIONMANIFIESTO = "S"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(anularManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest); 

        }

        /// <summary>
        /// Libera remesa terrestre de carga(se debe revisar)
        /// </summary>
        /// <param name="liberarRemesaTerrestreCarga"></param>
        ///

        public void LiberarRemesaTerrestreDeCarga(POPropietarioVehiculo liberarRemesaTerrestreCarga)
        {
            RequestLiberarRemesa infoRemesaTerrestre = new RequestLiberarRemesa()
            {
                root = new liberarRemesa()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 9,
                        tipo = 1
                    },

                    variables = new VariablesLiberarRemesa()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOREMESA = "TRATOTALR1",
                        NUMMANIFIESTOCARGA = "TRATOTALM1",
                        CODOPERACIONTRANSPORTEREVERSA = "L",
                        CODMUNICIPIOTRANSBORDO = "73001000",
                        MOTIVOTRANSBORDOREMESA = "A"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(infoRemesaTerrestre);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

        /// <summary>
        /// suspende manifiesto de carga
        /// </summary>
        /// <param name="infoSuspenderManifiesto"></param>
        ///
        public void SuspenderManifiestoDeCarga(POPropietarioVehiculo infoSuspenderManifiesto)
        {
            RequestCumplirManifiesto cumplirManifiesto = new RequestCumplirManifiesto()
            {
                root = new CumplirManifiesto()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 6,
                        tipo = 1
                    },

                    variables = new VariablesCumplirManifiesto()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA = "TRATOTALM1",
                        TIPOCUMPLIDOMANIFIESTO = "S",
                        MOTIVOSUSPENSIONMANIFIESTO = "A",
                        CONSECUENCIASUSPENSION = "T",
                        FECHAENTREGADOCUMENTOS = "29/09/2011",
                        VALORDESCUENTOFLETE = 1500000,
                        MOTIVOVALORDESCUENTOMANIFIESTO = "V"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumplirManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASDESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOVALORADICIONAL />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORSOBREANTICIPO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);

        }

        /// <summary>
        /// Registra manifiesto de carga transbordo
        /// </summary>
        /// <param name="infomanifiestoTransbordo"></param>
        ///
        public void RegistrarManifiestoDeCargaTransbordo(POPropietarioVehiculo infomanifiestoTransbordo)
        {
            List<Remesa> listaRemesa = new List<Remesa>();
            Remesa elementoRemesa = new Remesa();

            elementoRemesa.CONSECUTIVOREMESA = "TRATOTALR1";
            listaRemesa.Add(elementoRemesa);
            elementoRemesa.CONSECUTIVOREMESA = "TRATOTALR2";
            listaRemesa.Add(elementoRemesa);

            RequestExpedirmanifiesto expidoManifiesto = new RequestExpedirmanifiesto()
            {
                root = new ExpedirManifiesto
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 4,
                        tipo = 1
                    },

                    variables = new VariablesExpedirManifiesto()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA = "TRATOTALM2",
                        CODOPERACIONTRANSPORTE = "G",
                        FECHAEXPEDICIONMANIFIESTO = "29/09/2011",
                        CODMUNICIPIOORIGENMANIFIESTO = "73001000",
                        CODMUNICIPIODESTINOMANIFIESTO = "76109000",
                        CODIDTITULARMANIFIESTO = "C",
                        NUMIDTITULARMANIFIESTO = 53160000,
                        NUMPLACA = "SPO115",
                        CODIDCONDUCTOR = "C",
                        NUMIDCONDUCTOR = 12345654,
                        VALORFLETEPACTADOVIAJE = 1500000,
                        RETENCIONICAMANIFIESTOCARGA = "4.12",
                        VALORANTICIPOMANIFIESTO = 500000,
                        CODMUNICIPIOPAGOSALDO = "11001000",
                        CODRESPONSABLEPAGOCARGUE = "E",
                        CODRESPONSABLEPAGODESCARGUE = "D",
                        FECHAPAGOSALDOMANIFIESTO = "10/10/2011",
                        OBSERVACIONES = "MANIFIESTO GENERADO POR TRANSBORDO",
                        REMESASMAN = new RemesaMan()
                        {
                            REMESA = listaRemesa
                        }
                    }

                }
            };

            var json = new JavaScriptSerializer().Serialize(expidoManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<REMESASMAN>", "<REMESASMAN procesoid='43'>");
            temp.InnerXml = temp.InnerXml.Replace("<CONSECUTIVOINFORMACIONVIAJE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MANNROMANIFIESTOTRANSBORDO/>", "");
            temp.InnerXml = temp.InnerXml.Replace("<RETENCIONFUENTEMANIFIESTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MANNROMANIFIESTOTRANSBORDO />", "");


            xmlRequest = temp.InnerXml.ToString();


            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

        /// <summary>
        /// Registra el Cumplido de la Remesa Terrestre transbordo 
        /// </summary>
        /// <param name="CmplRemesaTerrestreTransbordo"></param>
        ///

        public void ResgistroCmplRemesaTerrestreTransbordoT(POPropietarioVehiculo CmplRemesaTerrestreTransbordo)
        {
            RequestCumplirRemesa cumploRemesa = new RequestCumplirRemesa()
            {
                root = new CumplirRemesa()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 5,
                        tipo = 1
                    },
                    variables = new VariablesCumplirRemesa()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOREMESA = "TRATOTALR1",
                        CANTIDADENTREGADA = 10000,
                        FECHALLEGADADESCARGUE = "30/09/2011",
                        HORALLEGADADESCARGUECUMPLIDO = "07:00",
                        FECHAENTRADADESCARGUE = "30/09/2011",
                        HORAENTRADADESCARGUECUMPLIDO = "07:10",
                        FECHASALIDADESCARGUE = "30/09/2011",
                        HORASALIDADESCARGUECUMPLIDO = "08:00"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumploRemesa);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<TIPOCUMPLIDOREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOSUSPENSIONREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CANTIDADCARGADA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<UNIDADMEDIDACAPACIDAD />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORALLEGADACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHAENTRADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORAENTRADACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHASALIDACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORASALIDACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);

        }

        /// <summary>
        /// Registra el Cumplido del manifiesto Terrestre de transbordo
        /// </summary>
        /// <param name="CmplManifiestoTransbordoT"></param>
        ///

        public void ResgistroCmplManifiestoTransbordoT(POPropietarioVehiculo CmplManifiestoTransbordoT)
        {
            RequestCumplirManifiesto cumplirManifiesto = new RequestCumplirManifiesto()
            {
                root = new CumplirManifiesto()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 6,
                        tipo = 1
                    },

                    variables = new VariablesCumplirManifiesto()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA = "TRATOTALM2",
                        TIPOCUMPLIDOMANIFIESTO = "C",
                        FECHAENTREGADOCUMENTOS = "30/09/2011",
                        VALORSOBREANTICIPO = 200000
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumplirManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOSUSPENSIONMANIFIESTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CONSECUENCIASUSPENSION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASDESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOVALORADICIONAL />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORDESCUENTOFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOVALORDESCUENTOMANIFIESTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);

        }

         /// <summary>
        /// Registra el manifiesto de carga parcial(se debe revisar)
        /// </summary>
        /// <param name="ManifiestoTransbordoDeCargaParc"></param>
        ///

        public void ResgistroManifiestoTransbordoDeCargaParc(POPropietarioVehiculo ManifiestoTransbordoDeCargaParc)
        {

            List<Remesa> listaRemesa = new List<Remesa>();
            Remesa elementoRemesa = new Remesa();

            elementoRemesa.CONSECUTIVOREMESA = "TRATOTALR1";
            listaRemesa.Add(elementoRemesa);
            elementoRemesa.CONSECUTIVOREMESA = "TRATOTALR2";
            listaRemesa.Add(elementoRemesa);

            RequestExpedirmanifiesto expidoManifiesto = new RequestExpedirmanifiesto()
            {
                root = new ExpedirManifiesto
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 4,
                        tipo = 1
                    },

                    variables = new VariablesExpedirManifiesto()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA = "1TRAPARM2",
                        CODOPERACIONTRANSPORTE = "G",
                        FECHAEXPEDICIONMANIFIESTO = "29/09/2011",
                        CODMUNICIPIOORIGENMANIFIESTO = "73001000",
                        CODMUNICIPIODESTINOMANIFIESTO = "76109000",
                        CODIDTITULARMANIFIESTO = "C",
                        NUMIDTITULARMANIFIESTO = 53160000,
                        NUMPLACA = "SPO115",
                        CODIDCONDUCTOR = "C",
                        NUMIDCONDUCTOR = 1234565,
                        VALORFLETEPACTADOVIAJE = 1500000,
                        RETENCIONICAMANIFIESTOCARGA = "4",
                        VALORANTICIPOMANIFIESTO = 200000,
                        CODMUNICIPIOPAGOSALDO = "11001000",
                        CODRESPONSABLEPAGOCARGUE = "E",
                        CODRESPONSABLEPAGODESCARGUE = "D",
                        FECHAPAGOSALDOMANIFIESTO = "15/10/2011",
                        OBSERVACIONES = "MANIFIESTO GENERADO POR TRANSBORDO",
                        REMESASMAN = new RemesaMan()
                        {
                            REMESA = listaRemesa
                        }
                    }

                }
            };

            var json = new JavaScriptSerializer().Serialize(expidoManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<REMESASMAN>", "<REMESASMAN procesoid='43'>");
            temp.InnerXml = temp.InnerXml.Replace("<CONSECUTIVOINFORMACIONVIAJE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MANNROMANIFIESTOTRANSBORDO/>", "");
            temp.InnerXml = temp.InnerXml.Replace("<RETENCIONFUENTEMANIFIESTO />", "");


            xmlRequest = temp.InnerXml.ToString();


            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);

            

           
        }

         /// <summary>
        /// Cumplir la Remesa liberada y transbordada
        /// </summary>
        /// <param name="CumplirRemesaLiberadaTrans"></param>
        ///

        public void ResgistroCumplirRemesaLibTransbordadaParc(POPropietarioVehiculo CumplirRemesaLiberadaTrans)
        {
            RequestCumplirRemesa cumploRemesa = new RequestCumplirRemesa()
            {
                root = new CumplirRemesa()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 5,
                        tipo = 1
                    },
                    variables = new VariablesCumplirRemesa()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOREMESA = "1TRAPARR1",
                        CANTIDADENTREGADA = 9000,
                        FECHALLEGADADESCARGUE = "29/09/2011",
                        HORALLEGADADESCARGUECUMPLIDO = "02:00",
                        FECHAENTRADADESCARGUE = "29/09/2011",
                        HORAENTRADADESCARGUECUMPLIDO = "02:15",
                        FECHASALIDADESCARGUE = "29/09/2011",
                        HORASALIDADESCARGUECUMPLIDO = "03:00"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumploRemesa);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<TIPOCUMPLIDOREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOSUSPENSIONREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CANTIDADCARGADA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<UNIDADMEDIDACAPACIDAD />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORALLEGADACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHAENTRADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORAENTRADACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHASALIDACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORASALIDACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
            
        }

        /// <summary>
        /// Cumplir el Manifiesto de Carga de transbordo parcial
        /// </summary>
        /// <param name="CumplirManifiestoLiberadoTransParc"></param>
        ///

        public void ResgistroCumplirManifiestoLiberadoTransParc(POPropietarioVehiculo CumplirManifiestoLiberadoTransParc)
        {
            RequestCumplirManifiesto cumplirManifiesto = new RequestCumplirManifiesto()
            {
                root = new CumplirManifiesto()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 6,
                        tipo = 1
                    },

                    variables = new VariablesCumplirManifiesto()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA = "1TRAPARM2",
                        TIPOCUMPLIDOMANIFIESTO = "C",
                        FECHAENTREGADOCUMENTOS = "29/09/2011"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumplirManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOSUSPENSIONMANIFIESTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CONSECUENCIASUSPENSION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASDESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOVALORADICIONAL />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORDESCUENTOFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOVALORDESCUENTOMANIFIESTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORSOBREANTICIPO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);

        }

        /// <summary>
        /// Cumplir la Remesa de carga que no fue liberada
        /// </summary>
        /// <param name="CumplirRemesaNoLiberada"></param>
        ///

        public void ResgistroCumplirRemesaNoLiberadaTransParc(POPropietarioVehiculo CumplirRemesaNoLiberada)
        {
            RequestCumplirRemesa cumploRemesa = new RequestCumplirRemesa()
            {
                root = new CumplirRemesa()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 5,
                        tipo = 1
                    },
                    variables = new VariablesCumplirRemesa()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOREMESA = "1TRAPARR2",
                        CANTIDADENTREGADA = 10000,
                        FECHALLEGADADESCARGUE = "29/09/2011",
                        HORALLEGADADESCARGUECUMPLIDO = "03:00",
                        FECHAENTRADADESCARGUE = "29/09/2011",
                        HORAENTRADADESCARGUECUMPLIDO = "04:00",
                        FECHASALIDADESCARGUE = "29/09/2011",
                        HORASALIDADESCARGUECUMPLIDO = "05:00"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumploRemesa);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<TIPOCUMPLIDOREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOSUSPENSIONREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CANTIDADCARGADA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<UNIDADMEDIDACAPACIDAD />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORALLEGADACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHAENTRADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORAENTRADACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHASALIDACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORASALIDACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

         /// <summary>
        /// Cumplir Manifiesto de Carga desde donde se transbordó
        /// </summary>
        /// <param name="CumplirManifiestoCargaTransbordo"></param>
        ///

        public void ResgistroCumplirManifiestoDesdeTransbordo(POPropietarioVehiculo CumplirManifiestoCargaTransbordo)
        {
            RequestCumplirManifiesto cumplirManifiesto = new RequestCumplirManifiesto()
            {
                root = new CumplirManifiesto()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 6,
                        tipo = 1
                    },

                    variables = new VariablesCumplirManifiesto()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA = "1TRAPARM2",
                        TIPOCUMPLIDOMANIFIESTO = "C",
                        FECHAENTREGADOCUMENTOS = "29/09/2011",
                        VALORDESCUENTOFLETE = 85000,
                        MOTIVOVALORDESCUENTOMANIFIESTO = "F"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumplirManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOSUSPENSIONMANIFIESTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CONSECUENCIASUSPENSION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASDESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOVALORADICIONAL />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORDESCUENTOFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORSOBREANTICIPO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }

         /// <summary>
        /// Expedir Remesa Terrestre de Carga para multiples destinos
        /// </summary>
        /// <param name="InfoRemesaMultDest"></param>
        ///

        public void RegistroRemesaMultiplesDestinos(POPropietarioVehiculo InfoRemesaMultDest) 
        {
            RequestRemesaTerrestre remesaTerrestre = new RequestRemesaTerrestre()
            {
                root = new RemesaTerrestre()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },
                    solicitud = new Solicitud()
                    {
                        procesoid = 3,
                        tipo = 1
                    },

                    variables = new VariablesRemesaTerrestre()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOREMESA = "DIVIDIRCARGAR1",
                        CONSECUTIVOINFORMACIONCARGA = "DIVIDIRCARGAIC1",
                        CANTIDADCARGADA = 15000,
                        CODTIPOIDDESTINATARIO = "N",
                        NUMIDDESTINATARIO = "1000000000",
                        CODSEDEDESTINATARIO = "01A",
                        DUENOPOLIZA = "E",
                        NUMPOLIZATRANSPORTE = 98765432,
                        COMPANIASEGURO = 8600265186,
                        FECHAVENCIMIENTOPOLIZACARGA = "15/10/2011",
                        HORASPACTODESCARGUE = 1,
                        MINUTOSPACTODESCARGUE = 30,
                        FECHALLEGADACARGUE = "28/09/2011",
                        HORALLEGADACARGUEREMESA = "07:00",
                        FECHAENTRADACARGUE = "28/09/2011",
                        HORAENTRADACARGUEREMESA = "08:00",
                        FECHASALIDACARGUE = "28/09/2011",
                        HORASALIDACARGUEREMESA = "10:00"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(remesaTerrestre);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<CONSECUTIVOCARGADIVIDIDA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODOPERACIONTRANSPORTE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODNATURALEZACARGA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<UNIDADMEDIDACAPACIDAD />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODTIPOEMPAQUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<PESOCONTENEDORVACIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MERCANCIAREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<DESCRIPCIONCORTAPRODUCTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODTIPOIDREMITENTE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMIDREMITENTE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODSEDEREMITENTE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORASPACTOCARGA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MINUTOSPACTOCARGA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODTIPOIDPROPIETARIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<NUMIDPROPIETARIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CODSEDEPROPIETARIO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHACITAPACTADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORACITAPACTADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHACITAPACTADADESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORACITAPACTADADESCARGUEREMESA />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);

            
        }

         /// <summary>
        /// Cumplir Remesa Terrestre de Carga para multiples destinos  
        /// </summary>
        /// <param name="InfocmplRemesaMultiDes"></param>
        ///

        public void RegistroCumplirRemesaMultiplesDestinos(POPropietarioVehiculo InfocmplRemesaMultiDes) 
        {
            RequestCumplirRemesa cumploRemesa = new RequestCumplirRemesa()
            {
                root = new CumplirRemesa()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 5,
                        tipo = 1
                    },
                    variables = new VariablesCumplirRemesa()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOREMESA = "DIVIDIRCARGAR2",
                        CANTIDADENTREGADA = 15000,
                        FECHALLEGADADESCARGUE = "30/09/2011",
                        HORALLEGADADESCARGUECUMPLIDO = "07:00",
                        FECHAENTRADADESCARGUE = "30/09/2011",
                        HORAENTRADADESCARGUECUMPLIDO = "07:10",
                        FECHASALIDADESCARGUE = "30/09/2011",
                        HORASALIDADESCARGUECUMPLIDO = "08:00"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumploRemesa);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<TIPOCUMPLIDOREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOSUSPENSIONREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CANTIDADCARGADA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<UNIDADMEDIDACAPACIDAD />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORALLEGADACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHAENTRADACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORAENTRADACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<FECHASALIDACARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<HORASALIDACARGUEREMESA />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }
        
        /// <summary>
        /// Cumplir manifiesto para multiples destinos
        /// </summary>
        /// <param name="InfoManifiestoMultDest"></param>
        ///

        public void RegistroCumplirManifiestoMultiplesDestinos(POPropietarioVehiculo InfoManifiestoMultDest)
        {
            RequestCumplirManifiesto cumplirManifiesto = new RequestCumplirManifiesto()
            {
                root = new CumplirManifiesto()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 6,
                        tipo = 1
                    },

                    variables = new VariablesCumplirManifiesto()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA = "1TRAPARM2",
                        TIPOCUMPLIDOMANIFIESTO = "C",
                        FECHAENTREGADOCUMENTOS = "29/09/2011",
                        VALORSOBREANTICIPO = 500000
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumplirManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOSUSPENSIONMANIFIESTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<CONSECUENCIASUSPENSION />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASDESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOVALORADICIONAL />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORDESCUENTOFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOVALORDESCUENTOMANIFIESTO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORSOBREANTICIPO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);

        }
        
        /// <summary>
        /// Suspensión de la Remesa Terrestre de Carga
        /// </summary>
        /// <param name="SuspenderRemesa"></param>
        ///

        public void SuspenderRemesaCarga(POPropietarioVehiculo SuspenderRemesa)
        {


            RequestSuspenderRemesa suspRemesa = new RequestSuspenderRemesa()
            {
                root = new SuspenderRemesa()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 5,
                        tipo = 1
                    },
                    variables = new VarSuspenderRemesa() 
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        CONSECUTIVOREMESA = "PERTOTALR1",
                        NUMMANIFIESTOCARGA = "PERTOTALM1",
                        CODOPERACIONTRANSPORTECUMPLIDO = "S",
                        MOTIVOSUSPENSIONREMESA = "A"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(suspRemesa);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }
        
        /// <summary>
        /// Suspensión de manifiesto de Carga para perdida total
        /// </summary>
        /// <param name="SuspenderManPT"></param>
        ///

        public void SuspenderManifiestoPerdTotal(POPropietarioVehiculo SuspenderManPT) 
        {
            RequestCumplirManifiesto cumplirManifiesto = new RequestCumplirManifiesto()
            {
                root = new CumplirManifiesto()
                {
                    acceso = new Acceso()
                    {
                        password = "RNDC9999",
                        username = "APARRA@9999"
                    },

                    solicitud = new Solicitud()
                    {
                        procesoid = 6,
                        tipo = 1
                    },

                    variables = new VariablesCumplirManifiesto()
                    {
                        NUMNITEMPRESATRANSPORTE = 830047668,
                        NUMMANIFIESTOCARGA = "TRATOTALM1",
                        TIPOCUMPLIDOMANIFIESTO = "S",
                        MOTIVOSUSPENSIONMANIFIESTO = "A",
                        CONSECUENCIASUSPENSION = "T",
                        FECHAENTREGADOCUMENTOS = "29/09/2011",
                        MOTIVOVALORDESCUENTOMANIFIESTO = "V"
                    }
                }
            };

            var json = new JavaScriptSerializer().Serialize(cumplirManifiesto);

            var xmlRequest = JsonConvert.DeserializeXmlNode(json).InnerXml;

            System.Xml.XmlDocument temp = new XmlDocument();

            temp.LoadXml(xmlRequest);
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALHORASDESCARGUE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORADICIONALFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<MOTIVOVALORADICIONAL />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORDESCUENTOFLETE />", "");
            temp.InnerXml = temp.InnerXml.Replace("<VALORSOBREANTICIPO />", "");
            temp.InnerXml = temp.InnerXml.Replace("<OBSERVACIONES />", "");
            temp.InnerXml = temp.InnerXml.Replace("<INGRESOID />", "");

            xmlRequest = temp.InnerXml.ToString();

            string retorno = ConsumirServicioAtenderMensajeRNDC(xmlRequest);
        }
    }
}
