using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.OperacionNacional.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.OperacionNacional
{
  internal class ONAdministradorEstadoGuia : ControllerBase
  {
    private static readonly ONAdministradorEstadoGuia instancia = (ONAdministradorEstadoGuia)FabricaInterceptores.GetProxy(new ONAdministradorEstadoGuia(), COConstantesModulos.MODULO_OPERACION_URBANA);

    /// <summary>
    /// Retorna una instancia de OUAdministradorEstadosGuia
    /// /// </summary>
    public static ONAdministradorEstadoGuia Instancia
    {
      get { return ONAdministradorEstadoGuia.instancia; }
    }



  }
}