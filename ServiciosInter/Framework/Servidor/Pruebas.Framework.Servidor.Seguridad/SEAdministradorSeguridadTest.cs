using System;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas.Framework.Servidor.Seguridad
{
  /// <summary>
  ///This is a test class for SEAdministradorSeguridadTest and is intended
  ///to contain all SEAdministradorSeguridadTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SEAdministradorSeguridadTest
  {
    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes

    //
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //

    #endregion Additional test attributes

    /// <summary>
    ///A test for ActualizarAdminUsuarios
    ///</summary>
    [TestMethod()]
    public void ActualizarAdminUsuariosTest()
    {
      SEAdminUsuario adminUsuario = new SEAdminUsuario()
      {
        Usuario = "carolina.amariles",
        Nombre = "Carolinaaa",
        Apellido1 = "Amariles",
        Apellido2 = "Gomez",
        Comentarios = "Unit Test",
        Direccion = "dir nueva",
        Email = "caroline@hotmail.com",
        Estado = "ACT",
        EstadoRegistro = global::Framework.Servidor.Comun.EnumEstadoRegistro.MODIFICADO,
        TipoUsuario = "LDAP",
        PasswordNuevo = "#123abcdeef098$888",
        TipoIdentificacion = "CC",
        Telefono = "7865434",
        RequiereIdentificadorMaquina = false,
        Identificacion = "2323232",
        IdRegional = 0011,
        IdCargo = 2,
      };

      SEAdministradorSeguridad.Instancia.ActualizarAdminUsuarios(adminUsuario);
    }
  }
}