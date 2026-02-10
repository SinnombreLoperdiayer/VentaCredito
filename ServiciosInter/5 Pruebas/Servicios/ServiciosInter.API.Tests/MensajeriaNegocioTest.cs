using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria;
using ServiciosInter.Negocio.Mensajeria;
using Newtonsoft.Json;
using ServiciosInter.Infraestructura.AccesoDatos.Repository.Mensajeria;
using System.Linq;

namespace ServiciosInter.API.Tests.Controllers
{
    /// <summary>
    /// Clase que contiene las pruebas del nuevo metodo UnificarEstadosDos implementado en ObtenerRastreoGuiasPortalClientesPost 
    /// </summary>
    [TestClass]
    public class MensajeriaNegocioTest
    {   
        /// <summary>
        /// Metodo que contiene la data que se espera para el caso de prueba 1
        /// </summary>
        private List<ADEstadoGuiaMotivoClienteRespuesta> DataEsperadaCaso1()
        {
            List<ADEstadoGuiaMotivoClienteRespuesta> lstEstados = new List<ADEstadoGuiaMotivoClienteRespuesta>
            {
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 1,
                        DescripcionEstadoGuia = "Recibimos tú envío",
                        Ciudad = "VILLAVICENCIO",
                        FechaGrabacion = Convert.ToDateTime("2017-02-11T05:26:33.493")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 2,
                        DescripcionEstadoGuia = "En Centro Logístico Origen",
                        Ciudad = "VILLAVICENCIO",
                        FechaGrabacion = Convert.ToDateTime("2017-02-13T20:23:39.99")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 4,
                        DescripcionEstadoGuia = "Viajando a tu destino",
                        Ciudad = "VILLAVICENCIO",
                        FechaGrabacion = Convert.ToDateTime("2017-02-14T00:32:46.46")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 2,
                        DescripcionEstadoGuia = "En Centro Logístico Origen",
                        Ciudad = "PUERTO LOPEZ",
                        FechaGrabacion = Convert.ToDateTime("2017-02-14T08:31:05.743")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 2,
                        DescripcionEstadoGuia = "En Centro Logístico Destino",
                        Ciudad = "PUERTO LOPEZ",
                        FechaGrabacion = Convert.ToDateTime("2017-02-14T08:34:16.527")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 6,
                        DescripcionEstadoGuia = "En camino hacia ti",
                        Ciudad = "PUERTO LOPEZ",
                        FechaGrabacion = Convert.ToDateTime("2017-02-14T08:37:38.467")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 7,
                        DescripcionEstadoGuia = "No logramos hacer la entrega",
                        Ciudad = "PUERTO LOPEZ",
                        FechaGrabacion = Convert.ToDateTime("2017-03-11T09:46:08.267")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 2,
                        DescripcionEstadoGuia = "En Centro Logístico Origen",
                        Ciudad = "VILLAVICENCIO",
                        FechaGrabacion = Convert.ToDateTime("2017-03-13T16:57:05.89")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 2,
                        DescripcionEstadoGuia = "En Centro Logístico Destino",
                        Ciudad = "BOGOTA",
                        FechaGrabacion = Convert.ToDateTime("2017-03-14T12:24:19.99")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 9,
                        DescripcionEstadoGuia = "Tu envío presenta una novedad",
                        Ciudad = "BOGOTA",
                        FechaGrabacion = Convert.ToDateTime("2017-03-14T14:17:39.203")
                    }
                }
            };
            return lstEstados;
        }
        /// <summary>
        /// Metodo que contiene la data que se espera para el caso de prueba 2
        /// </summary>
        private List<ADEstadoGuiaMotivoClienteRespuesta> DataEsperadaCaso2()
        {
            List<ADEstadoGuiaMotivoClienteRespuesta> lstEstados = new List<ADEstadoGuiaMotivoClienteRespuesta>
            {
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 1,
                        DescripcionEstadoGuia = "Recibimos tú envío",
                        Ciudad = "BOGOTA\\CUND\\COL",
                        FechaGrabacion = Convert.ToDateTime("2016-11-08T20:48:59.947")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 15,
                        DescripcionEstadoGuia = "Envío cancelado ",
                        Ciudad = "BOGOTA",
                        FechaGrabacion = Convert.ToDateTime("2016-11-10T09:49:01.797")
                    }
                }
            };
            return lstEstados;
        }
        /// <summary>
        /// Metodo que contiene la data que se espera para el caso de prueba 3
        /// </summary>
        private List<ADEstadoGuiaMotivoClienteRespuesta> DataEsperadaCaso3()
        {
            List<ADEstadoGuiaMotivoClienteRespuesta> lstEstados = new List<ADEstadoGuiaMotivoClienteRespuesta>
            {
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 1,
                        DescripcionEstadoGuia = "Recibimos tú envío",
                        Ciudad = "BOGOTA\\CUND\\COL",
                        FechaGrabacion = Convert.ToDateTime("2020-02-26T10:55:12.863")
                    }
                }
            };
            return lstEstados;
        }
        /// <summary>
        /// Metodo que contiene la data que se espera para el caso de prueba 4
        /// </summary>
        private List<ADEstadoGuiaMotivoClienteRespuesta> DataEsperadaCaso4()
        {
            List<ADEstadoGuiaMotivoClienteRespuesta> lstEstados = new List<ADEstadoGuiaMotivoClienteRespuesta>
            {
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 1,
                        DescripcionEstadoGuia = "Recibimos tú envío",
                        Ciudad = "TENJO\\CUND\\COL",
                        FechaGrabacion = Convert.ToDateTime("2016-11-01T16:40:19.387")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 2,
                        DescripcionEstadoGuia = "En Centro Logístico Origen",
                        Ciudad = "BOGOTA",
                        FechaGrabacion = Convert.ToDateTime("2017-03-22T13:06:09.413")
                    }
                },
                new ADEstadoGuiaMotivoClienteRespuesta
                {
                    EstadoGuia = new ADTrazaGuiaEstadoGuia()
                    {
                        IdEstadoGuia = 14,
                        DescripcionEstadoGuia = "Tu envío presenta una novedad",
                        Ciudad = "BOGOTA",
                        FechaGrabacion = Convert.ToDateTime("2017-03-22T15:18:15.013")
                    }
                }
            };
            return lstEstados;
        }
        /// <summary>
        /// Metodo que contempla el caso de prueba cuando se reciben varios estados 2 y presenta una novedad
        /// </summary>
        [TestMethod]
        public void TestUnificarEstadosDos_MultipleEstadosNovedad()
        {   
            List<ADEstadoGuiaMotivoClienteRespuesta> lstEstadosOrigen = MensajeriaRepository.Instancia.ObtenerEstadosMotivosGuiaPorPortal("3000202835540");
            List<ADEstadoGuiaClienteRespuesta> lstEstadosConvertidos = ConvertirAEstadosClienteRespuesta(lstEstadosOrigen);
            var jResultado = JsonConvert.SerializeObject(MensajeriaNegocio.Instancia.UnificarEstadosDos(lstEstadosConvertidos));
            var jsonEsperado = JsonConvert.SerializeObject(DataEsperadaCaso1());
            Assert.AreEqual(jResultado, jsonEsperado);
        }
        /// <summary>
        /// Metodo que contempla el caso de prueba cuando se reciben dos estados 2
        /// </summary>
        [TestMethod]
        public void TestUnificarEstadosDos_DosEstados()
        {
            List<ADEstadoGuiaMotivoClienteRespuesta> lstEstadosOrigen = MensajeriaRepository.Instancia.ObtenerEstadosMotivosGuiaPorPortal("700010650158");
            List<ADEstadoGuiaClienteRespuesta> lstEstadosConvertidos = ConvertirAEstadosClienteRespuesta(lstEstadosOrigen);
            var jResultado = JsonConvert.SerializeObject(MensajeriaNegocio.Instancia.UnificarEstadosDos(lstEstadosConvertidos));
            var jsonEsperado = JsonConvert.SerializeObject(DataEsperadaCaso2());
            Assert.AreEqual(jResultado, jsonEsperado);
        }
        /// <summary>
        /// Metodo que contempla el caso de prueba cuando se recibe un estados 2
        /// </summary>
        [TestMethod]
        public void TestUnificarEstadosDos_UnEstados()
        {
            List<ADEstadoGuiaMotivoClienteRespuesta> lstEstadosOrigen = MensajeriaRepository.Instancia.ObtenerEstadosMotivosGuiaPorPortal("230002076927");
            List<ADEstadoGuiaClienteRespuesta> lstEstadosConvertidos = ConvertirAEstadosClienteRespuesta(lstEstadosOrigen);
            var jResultado = JsonConvert.SerializeObject(MensajeriaNegocio.Instancia.UnificarEstadosDos(lstEstadosConvertidos));
            var jsonEsperado = JsonConvert.SerializeObject(DataEsperadaCaso3());
            Assert.AreEqual(jResultado, jsonEsperado);
        }
        /// <summary>
        /// Metodo que contempla el caso de prueba cuando se recibe un estados de envento especial
        /// </summary>
        [TestMethod]
        public void TestUnificarEstadosDos_EventoEspecial()
        {
            List<ADEstadoGuiaMotivoClienteRespuesta> lstEstadosOrigen = MensajeriaRepository.Instancia.ObtenerEstadosMotivosGuiaPorPortal("700010561910");
            List<ADEstadoGuiaClienteRespuesta> lstEstadosConvertidos = ConvertirAEstadosClienteRespuesta(lstEstadosOrigen);
            var jResultado = JsonConvert.SerializeObject(MensajeriaNegocio.Instancia.UnificarEstadosDos(lstEstadosConvertidos));
            var jsonEsperado = JsonConvert.SerializeObject(DataEsperadaCaso4());
            Assert.AreEqual(jResultado, jsonEsperado);
        }

        public static List<ADEstadoGuiaClienteRespuesta> ConvertirAEstadosClienteRespuesta(List<ADEstadoGuiaMotivoClienteRespuesta> lstEstadosOrigen)
        {
            return lstEstadosOrigen.Select(item => new ADEstadoGuiaClienteRespuesta
            {
                EstadoGuia =
            {
                IdEstadoGuia = item.EstadoGuia.IdEstadoGuia,
                DescripcionEstadoGuia = item.EstadoGuia.DescripcionEstadoGuia,
                Ciudad = item.EstadoGuia.Ciudad,
                FechaGrabacion = item.EstadoGuia.FechaGrabacion
            }
            }).ToList();
        }
    }
}
