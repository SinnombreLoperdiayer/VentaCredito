using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using VentaCredito.AdmisionPreenvio;
using VentaCredito.Recogidas;
using System.Collections.Generic;

namespace UnitTestVentaCredito
{
    [TestClass]
    public class UnitTestAdmisionPreenvio
    {
        [TestMethod]
        public void TestMethodInsertarAdmision_OK()
        {
            // Arrange
            RequestPreAdmisionWrapperCV request = new RequestPreAdmisionWrapperCV
            {
                IdTipoEntrega = "1",
                AplicaContrapago = true,
                RequiereEmpaque = false,
                ValorACobrar = 0,
                IdServicio = 3,
                DiceContener = "Prueba copidrogas FFVIII",
                ValorDeclarado = 15200,
                Peso = 2,
                IdTipoEnvio = 1,
                IdFormaPago = 2,
                NumeroPieza = 1,
                Destinatario = new DestinatarioVC()
                {
                    NumeroDocumento = "876876543",
                    Nombre = "Andrea",
                    PrimerApellido = "Perez", //Si se debe enviar si es un cliente peaton, es obligatorio
                    SegundoApellido = null,
                    Telefono = "2775535",
                    Direccion = "Cra 3 #1b-56", //Calle 59#13-55 //Calle 12#15-22
                    IdDestinatario = 0,
                    IdRemitente = 0,
                    TipoDocumento = "CC",//cc
                    IdLocalidad = "11001000",
                    CodigoConvenio = 0, //Enviar valor 0 si no es cliente convenio
                    ConvenioDestinatario = 0, //Enviar valor 0 si no es cliente convenio
                    Correo = "eve@eve.com" //Obligatorio si es cliente convenio
                },
                DescripcionTipoEntrega = "",
                Largo = 10,
                Ancho = 10,
                Alto = 10,
                NombreTipoEnvio = "SOBRE CARTA",
                CodigoConvenio = 0, //Enviar valor 0 si no es cliente convenio
                IdSucursal = 0, //Enviar valor 0 si no es cliente convenio
                IdCliente = 0, //Enviar valor 0 si no es cliente convenio
                Notificacion = null,
                Rapiradicado = null, //Enviar solo si el servicio es id 16
                IdClienteCredito = 1057,
                CodigoConvenioRemitente = 2980,
                Observaciones = "Observacion"
            };

            // Act
            ResponsePreAdmisionWrapper response = AdmisionPreenvioNegocio.Instancia.InsertarAdmision(request);

            //Assert
            Assert.IsNotNull(response);
        }


        [TestMethod]
        public void TestMethodInsertarAdmision_PesoVolumetrico()
        {
            // Arrange
            Exception ex = null;
            RequestPreAdmisionWrapperCV request = new RequestPreAdmisionWrapperCV
            {
                IdTipoEntrega = "1",
                AplicaContrapago = false,
                RequiereEmpaque = false,
                ValorACobrar = 0,
                IdServicio = 3,
                DiceContener = "Prueba API Insertar Admision",
                ValorDeclarado = 100000,
                Peso = 3,
                IdTipoEnvio = 1,
                IdFormaPago = 2,
                NumeroPieza = 1,
                Destinatario = new DestinatarioVC()
                {
                    NumeroDocumento = "876876543",
                    Nombre = "Andrea",
                    PrimerApellido = "Perez",
                    SegundoApellido = null,
                    Telefono = "2775535",
                    Direccion = "Calle 59#13-55",
                    IdDestinatario = 0,
                    IdRemitente = 0,
                    TipoDocumento = "CC",
                    IdLocalidad = "11001000",
                    CodigoConvenio = 0,
                    ConvenioDestinatario = 0,
                    Correo = "eve@eve.com"
                },
                DescripcionTipoEntrega = "",
                Largo = 16,
                Ancho = 47,
                Alto = 40,
                NombreTipoEnvio = "SOBRE CARTA",
                CodigoConvenio = 0,
                IdSucursal = 0,
                IdCliente = 0,
                Notificacion = null,
                Rapiradicado = null,
                Observaciones = "Observacion",
                IdClienteCredito = 1058,
                CodigoConvenioRemitente = 2982
            };

            // Act
            try
            {
                AdmisionPreenvioNegocio.Instancia.InsertarAdmision(request);
            }
            catch (Exception e)
            {
                ex = e;
            }

            //Assert
            Assert.AreEqual("El servicio seleccionado no se encuentra asociado. Favor validar con la Transportadora.", ex.Message);
        }


        [TestMethod]
        public void TestMethodInsertarAdmision_Fail()
        {
            // Arrange
            Exception excep = null;
            RequestPreAdmisionWrapperCV request = new RequestPreAdmisionWrapperCV
            {
                IdTipoEntrega = "1",
                AplicaContrapago = false,
                RequiereEmpaque = false,
                ValorACobrar = 0,
                IdServicio = 17,
                DiceContener = "Prueba copidrogas FFVIII",
                ValorDeclarado = 10000, //El valor minimo actualmente para este cliente es de 25.000 pesos
                Peso = 7,
                IdTipoEnvio = 1,
                IdFormaPago = 2,
                NumeroPieza = 1,
                Destinatario = new DestinatarioVC()
                {
                    NumeroDocumento = "900460915",
                    Nombre = "Andrea",
                    PrimerApellido = "Perez", //Si se debe enviar si es un cliente peaton, es obligatorio
                    SegundoApellido = null,
                    Telefono = "2775535",
                    Direccion = "Calle pollito", //Calle 59#13-55 //Calle 12#15-22
                    IdDestinatario = 0,
                    IdRemitente = 0,
                    TipoDocumento = "CC",//cc
                    IdLocalidad = "05001000",
                    CodigoConvenio = 0, //Enviar valor 0 si no es cliente convenio
                    ConvenioDestinatario = 0, //Enviar valor 0 si no es cliente convenio
                    Correo = "eve@eve.com" //Obligatorio si es cliente convenio
                },
                DescripcionTipoEntrega = "",
                Largo = 10,
                Ancho = 10,
                Alto = 10,
                NombreTipoEnvio = "SOBRE CARTA",
                CodigoConvenio = 0, //Enviar valor 0 si no es cliente convenio
                IdSucursal = 0, //Enviar valor 0 si no es cliente convenio
                IdCliente = 0, //Enviar valor 0 si no es cliente convenio
                Notificacion = null,
                Rapiradicado = new RapiradicadoPE()
                {
                    NumerodeFolios = 7, //valor ejemplo
                    CodigoRapiRadicado = "25541545" //valor ejemplo
                }, //Enviar solo si el servicio es id 16
                IdClienteCredito = 1057,
                CodigoConvenioRemitente = 2980,
                Observaciones = "Observacion"
            };

            // Act
            try
            {
                AdmisionPreenvioNegocio.Instancia.InsertarAdmision(request);
            }
            catch (Exception ex)
            {
                excep = ex;
            }

            //Assert
            Assert.IsNotNull(excep);
        }

        [TestMethod]
        public void AsignarPreenvioARecogida_OK()
        {
            // Arrange
            string usuario = "userCopidrogaspruebasPRUE"; //Ambiente: Pruebas
            string token = "bearer 1aaCeHNgpuCU-3sQ8oaNDL7frGBxd7lh6IyJh-eGS9HfN48coZVueh9gLh9ffAUx7GrBvuC823tma2rZlTPKpzUtj9rZBeOCWbTEadRENduskbUzieNDy-sMzPoWih_jnkX1Tl0XFT8ZLPdT8KLBgH96dQsIPl2mIyodA4V72JHBNqhHCplePmptMQ9heG4LBHNq0AoCZKV2Wmc5gHuC_der75zJzTmOS6xAiMk2l4TziRWqwZkYRt2JKU3dqLGd";  //Ambiente: Pruebas
            RequestRecogidas request = new RequestRecogidas()
            {
                IdClienteCredito = 996,
                IdSucursalCliente = 10553,
                ListaNumPreenvios = new List<long>() { 240000021764 }, //Este Preenvio tiene que haber sido generado y estar en estado 11-Creado, tambien que se haya generado con una sucursal igual a la aquí asignada
                FechaRecogida = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 18, 00, 00) //Hora máxima válida por día: 6pm
            };

            // Act
            ResponseRecogidas response = RecogidasNegocio.Instancia.InsertarRecogidaCliente(request, usuario, token);

            //Assert
            Assert.AreEqual("La recogida se generó Exitosamente.", response.MensajePreenviosAsociados);
        }

        [TestMethod]
        public void AsignarPreenvioARecogida_Fail()
        {
            // Arrange
            string usuario = "userCopidrogaspruebasPRUE"; //Ambiente: Pruebas
            string token = "bearer 1aaCeHNgpuCU-3sQ8oaNDL7frGBxd7lh6IyJh-eGS9HfN48coZVueh9gLh9ffAUx7GrBvuC823tma2rZlTPKpzUtj9rZBeOCWbTEadRENduskbUzieNDy-sMzPoWih_jnkX1Tl0XFT8ZLPdT8KLBgH96dQsIPl2mIyodA4V72JHBNqhHCplePmptMQ9heG4LBHNq0AoCZKV2Wmc5gHuC_der75zJzTmOS6xAiMk2l4TziRWqwZkYRt2JKU3dqLGd";  //Ambiente: Pruebas
            Exception excepcion = null;
            RequestRecogidas request = new RequestRecogidas
            {
                IdClienteCredito = 996,
                IdSucursalCliente = 10553,
                ListaNumPreenvios = new List<long>() { 240000021764 }, //Este Preenvio debe estar en estado diferente a 11-Creado
                FechaRecogida = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 18, 00, 00) //Hora máxima válida por día: 6pm
            };

            // Act
            try
            {
                RecogidasNegocio.Instancia.InsertarRecogidaCliente(request, usuario, token);
            }
            catch (Exception e)
            {
                excepcion = e;
            }

            //Assert
            Assert.IsNotNull(excepcion);
        }

        [TestMethod]
        public void TestInsertarAdmisionPortalCli()
        {
            // Arrange
            Exception excep = null;
            RequestPreAdmisionPortalCli request = new RequestPreAdmisionPortalCli
            {
                IdTipoEntrega = "2",
                AplicaContrapago = false,
                RequiereEmpaque = false,
                ValorACobrar = 0,
                IdServicio = 3,
                IdRecogida = 123,
                FechaRecogida = DateTime.Now,
                DiceContener = "Prueba unitaria copidrogas FFVIII",
                ValorDeclarado = 100000, //El valor minimo actualmente para este cliente es de 25.000 pesos
                Peso = 3,
                IdTipoEnvio = 1,
                IdFormaPago = 2,
                NumeroPieza = 1,
                Destinatario = new DestinatarioVC()
                {
                    NumeroDocumento = "900460915",
                    Nombre = "Andrea",
                    PrimerApellido = "Perez", //Si se debe enviar si es un cliente peaton, es obligatorio
                    SegundoApellido = null,
                    Telefono = "2775535",
                    Direccion = "Calle pollito", //Calle 59#13-55 //Calle 12#15-22
                    IdDestinatario = 0,
                    IdRemitente = 0,
                    TipoDocumento = "CC",//cc
                    IdLocalidad = "11001000",
                    CodigoConvenio = 0, //Enviar valor 0 si no es cliente convenio
                    ConvenioDestinatario = 0, //Enviar valor 0 si no es cliente convenio
                    Correo = "eve@eve.com" //Obligatorio si es cliente convenio
                },
                DescripcionTipoEntrega = "",
                Largo = 10,
                Ancho = 10,
                Alto = 10,
                NombreTipoEnvio = "SOBRE CARTA",
                CodigoConvenio = 0, //Enviar valor 0 si no es cliente convenio
                IdSucursal = 0, //Enviar valor 0 si no es cliente convenio
                IdCliente = 0, //Enviar valor 0 si no es cliente convenio
                Rapiradicado = new RapiradicadoPE()
                {
                    NumerodeFolios = 7, //valor ejemplo
                    CodigoRapiRadicado = "25541545" //valor ejemplo
                }, //Enviar solo si el servicio es id 16
                IdClienteCredito = 996,
                CodigoConvenioRemitente = 2869,
                Observaciones = "Observacion"
            };

            // Act
            try
            {
                AdmisionPreenvioNegocio.Instancia.InsertarAdmisionPortalCli(request);
            }
            catch (Exception ex)
            {
                excep = ex;
            }
            //Assert
            Assert.IsNull(excep);
        }

        [TestMethod]
        public void TestInsertarAdmisionPortalCliFail()
        {
            // Arrange
            Exception excep = null;
            RequestPreAdmisionPortalCli request = new RequestPreAdmisionPortalCli
            {
                IdTipoEntrega = "2",
                AplicaContrapago = false,
                RequiereEmpaque = false,
                ValorACobrar = 0,
                IdServicio = 3,
                IdRecogida = 0,///Se envía en cero para que retorn Excepción
                FechaRecogida = DateTime.Now,
                DiceContener = "Prueba unitaria copidrogas FFVIII",
                ValorDeclarado = 100000, //El valor minimo actualmente para este cliente es de 25.000 pesos
                Peso = 3,
                IdTipoEnvio = 1,
                IdFormaPago = 2,
                NumeroPieza = 1,
                Destinatario = new DestinatarioVC()
                {
                    NumeroDocumento = "900460915",
                    Nombre = "Andrea",
                    PrimerApellido = "Perez", //Si se debe enviar si es un cliente peaton, es obligatorio
                    SegundoApellido = null,
                    Telefono = "2775535",
                    Direccion = "Calle pollito", //Calle 59#13-55 //Calle 12#15-22
                    IdDestinatario = 0,
                    IdRemitente = 0,
                    TipoDocumento = "CC",//cc
                    IdLocalidad = "05001000",
                    CodigoConvenio = 0, //Enviar valor 0 si no es cliente convenio
                    ConvenioDestinatario = 0, //Enviar valor 0 si no es cliente convenio
                    Correo = "eve@eve.com" //Obligatorio si es cliente convenio
                },
                DescripcionTipoEntrega = "",
                Largo = 10,
                Ancho = 10,
                Alto = 10,
                NombreTipoEnvio = "SOBRE CARTA",
                CodigoConvenio = 0, //Enviar valor 0 si no es cliente convenio
                IdSucursal = 0, //Enviar valor 0 si no es cliente convenio
                IdCliente = 0, //Enviar valor 0 si no es cliente convenio
                Rapiradicado = new RapiradicadoPE()
                {
                    NumerodeFolios = 7, //valor ejemplo
                    CodigoRapiRadicado = "25541545" //valor ejemplo
                }, //Enviar solo si el servicio es id 16
                IdClienteCredito = 996,
                CodigoConvenioRemitente = 2869,
                Observaciones = "Observacion"
            };

            // Act
            try
            {
                AdmisionPreenvioNegocio.Instancia.InsertarAdmisionPortalCli(request);
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
