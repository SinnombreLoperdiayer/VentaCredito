using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using VentaCredito.AdmisionPreenvio;

namespace UnitTestVentaCredito
{
    [TestClass]
    public class UnitTestImpresionesPreGuias
    {
        [TestMethod]
        public void TestMethodConsultarFormatoGuiaSimpliLocDestSuccess()
        {
            //Arange
            string IdLocalidadDes = "11001000";

            //Act
            Int32 esFormatoSimpli = AdmisionPreenvioNegocio.Instancia.ConsultarFormatoGuiaSimpliLocDest(IdLocalidadDes);

            //Assert
            Assert.IsNotNull(esFormatoSimpli);
        }

        [TestMethod]
        public void TestMethodConsultarFormatoGuiaSimpliLocDestFail()
        {
            //Arange
            string IdLocalidadDes = null;
            Exception excep = null;

            // Act
            try
            {
                Int32 esFormatoSimpli = AdmisionPreenvioNegocio.Instancia.ConsultarFormatoGuiaSimpliLocDest(IdLocalidadDes);
            }
            catch(Exception  ex)
            {
                excep = ex;
            }
            //Assert
            Assert.IsNotNull(excep);
        }

        [TestMethod]
        public void TestMethodObtenerBase64PdfGuiaSimplificadaSuccess()
        {
            //Arrange
            //Data ambiente de pruebas
            long numeroPreGuia = 240000018834;

            //Act
            Byte[] preguia = AdmisionPreenvioNegocio.Instancia.ObtenerPdfGuia(numeroPreGuia);

            //Assert
            Assert.IsNotNull(preguia);
        }

        [TestMethod]
        public void TestMethodObtenerBase64PdfGuiaSimplificadaFail()
        {
            //Arange
            //Data ambiente de pruebas
            long numeroPreGuia = 0;
            Exception excep = null;

            // Act
            try
            {
                Byte[] preguia = AdmisionPreenvioNegocio.Instancia.ObtenerPdfGuia(numeroPreGuia);
            }
            catch (Exception ex)
            {
                excep = ex;
            }
            //Assert
            Assert.IsNotNull(excep);
        }
            

    }
}
