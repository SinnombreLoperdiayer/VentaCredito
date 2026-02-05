using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VentaCredito.Transversal.Entidades.Clientes;
using VentaCredito.Clientes;

namespace UnitTestVentaCredito
{
    [TestClass]
    public class UnitTestClienteCredito
    {
        [TestMethod]
        public void TestMethodConsultarSucursales_OK()
        {
            // Arrange

            Int32 idClienteCredito = 1057;

            // Act
            List<SucursalCliente_CLI> response = CLClienteCredito.Instancia.ObtenerSucursalesActivasPorCliente(idClienteCredito);

            //Assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void TestMethodConsultarSucursales_FAIL()
        {
            // Arrange
            Int32 idClienteCredito = 0;
            Exception excepcion = null;

            // Act
            try
            {
                List<SucursalCliente_CLI> response = CLClienteCredito.Instancia.ObtenerSucursalesActivasPorCliente(idClienteCredito);
            }
            catch (Exception e)
            {
                excepcion = e;
            }

            //Assert
            Assert.IsNotNull(excepcion);
        }

        [TestMethod]
        public void TestMethodConsultarEstadosGuiaPorCliente_OK()
        {
            // Arrange

            RequestEstadosGuia_CLI request = new RequestEstadosGuia_CLI
            {
                IdCliente = 1611,
                NumeroGuias = new List<long> { 230001896999, 230001896999, 230001896999 }
            };
            // Act
            ResponseEstadosGuia_CLI response = CLClienteCredito.Instancia.ConsultarEstadosGuiaPorCliente(request);

            //Assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void TestMethodConsultarEstadosGuiaPorCliente_FAIL()
        {
            // Arrange
            RequestEstadosGuia_CLI request = new RequestEstadosGuia_CLI
            {
                IdCliente = 1611,
                NumeroGuias = new List<long> { 230001898879, 230001896999, 230001896999 }
            };
            Exception excepcion = null;

            // Act
            try
            {
                ResponseEstadosGuia_CLI response = CLClienteCredito.Instancia.ConsultarEstadosGuiaPorCliente(request);
            }
            catch (Exception e)
            {
                excepcion = e;
            }

            //Assert
            Assert.IsNotNull(excepcion);
        }
    }
}
