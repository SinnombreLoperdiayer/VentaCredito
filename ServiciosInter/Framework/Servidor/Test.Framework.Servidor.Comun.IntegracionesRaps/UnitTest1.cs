using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Framework.Servidor.Comun.Integraciones;
using System.Collections.Generic;

namespace Test.Framework.Servidor.Comun.IntegracionesRaps
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCrearSolicitudAutomatica()
        {

             Dictionary<string, object> parametrosParametrizacion = new Dictionary<string,object> ();
            parametrosParametrizacion.Add("146",700002658988);
            parametrosParametrizacion.Add("147", DateTime.Now);
            parametrosParametrizacion.Add("148", "Carmelo Perez");
            parametrosParametrizacion.Add("149", 878978);
            
            //IntegracionRaps.CrearSolicitudAcumulativaRaps(EnumTipoNovedadRaps.NoAlcanzo, parametrosParametrizacion, "11001000");
        }
    }
}
