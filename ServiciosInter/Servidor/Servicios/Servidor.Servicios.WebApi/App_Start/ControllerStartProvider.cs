using CO.Servidor.Servicios.WebApi.ProcesosAutomaticos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace CO.Servidor.Servicios.WebApi.App_Start
{
    public class ControllerStartProvider : IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            //MotorRaps.Instancia.IniciarMotorPruebaEjecucionElimarMetodo();
        }
    }
}