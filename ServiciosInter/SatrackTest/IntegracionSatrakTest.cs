using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Integraciones.Satrack;
using CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack;

namespace SatrackTest
{
    [TestClass]
    public class IntegracionSatrakTest
    {
        public string Esperado { get; set; }
        public string EsperadoParametroVacio { get; private set; }
        public List<INItinerarioDC> Programacion { get; private set; }
        public List<INItinerarioDC> ProgramacionParametroVacio { get; private set; }
        public string EsperadoConParametros { get; private set; }
        public List<INItinerarioDC> ProgramacionParametroConDosValores { get; private set; }

        /// <summary>
        /// ProgramarItinerario se envia unico ItinerarioCompleto
        /// se espera respuesta OK
        /// </summary>
        [TestMethod]
        public void ProgramarItinerario_ItinerarioCompleto_RespuestaOK()
        {
            //Arrange
            CreaItinerairoCompleto();
            CreaEsperadoSencillo();
            //Act
            string respuesta = IntegracionSatrack.Instancia.ProgramarItinerario(this.Programacion);
            //Assert
            Assert.AreEqual(this.Esperado, respuesta);
        }


        /// <summary>
        /// ProgramarItinerario se envia unico Itinerario Completo Con ParametroVacio
        /// se espera respuesta OK
        /// </summary>
        [TestMethod]
        public void ProgramarItinerario_ItinerarioConParametroVacio_RespuestaOK()
        {
            //Arrange
            CreaItinerairoCompleto();
            CreaEsperadoSencillo();
            //Act
            string respuesta = IntegracionSatrack.Instancia.ProgramarItinerario(this.ProgramacionParametroVacio);
            //Assert
            Assert.AreEqual(this.EsperadoParametroVacio, respuesta);
        }

        [TestMethod]
        public void ProgramarItinerario_ItinerarioConDosParametros_ResultadoOk()
        {
            //Arrange
            CreaItinerairoCompleto();
            CreaEsperadoSencillo();
            //Act
            string respuesta = IntegracionSatrack.Instancia.ProgramarItinerario(this.ProgramacionParametroConDosValores);
            //Assert
            Assert.AreEqual(this.EsperadoConParametros, respuesta);

        }

        private void CreaEsperadoSencillo()
        {
            this.Esperado = @"<nodeppal><item><placa>SZX770</placa><ruta>426</ruta><codigo>1</codigo><descripcion>Ok</descripcion><campo1></campo1></item></nodeppal>";
            this.EsperadoParametroVacio = @"<nodeppal><item><placa>SZX770</placa><ruta>426</ruta><codigo>1</codigo><descripcion>Ok</descripcion><campo1></campo1></item></nodeppal>";
            this.EsperadoConParametros = @"<nodeppal><item><placa>SZX770</placa><ruta>426</ruta><codigo>1</codigo><descripcion>Ok</descripcion><campo1></campo1></item></nodeppal>";
        }

        private void CreaItinerairoCompleto()
        {
            this.Programacion = new List<INItinerarioDC>
            {
                new INItinerarioDC
                {
                    Placa="SZX770",
                    Ruta="426"
                }
            };

            this.ProgramacionParametroVacio = new List<INItinerarioDC>
            {
                new INItinerarioDC
                {
                    Placa="SZX770",
                    Ruta="426",
                    Parametros = new INParametrosDC
                    {

                    }
                }
            };
            this.ProgramacionParametroConDosValores = new List<INItinerarioDC>
            {
                new INItinerarioDC
                {
                    Placa="SZX770",
                    Ruta="426",
                    Parametros = new INParametrosDC
                    {
                        Canbus = new INCanbusDC
                        {
                            Canpeso = "Uno"
                        },
                        Trafico = new INTraficoDC
                        {
                            Agencia = "001",
                            Nombreconductor = "Nepomuceno Cartagena",
                            Telefonoconductor = "0573109876543"
                        }                        
                    }
                }
            };
        }


    }
}
