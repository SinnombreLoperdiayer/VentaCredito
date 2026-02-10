using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Dominio.Comun.OperacionUrbana
{
  /// <summary>
  /// Interfaz para comunicación de datos con un sistema externo
  /// </summary>
  [ServiceContract(Namespace = "http://contrologis.com")]
  public interface IInterfazSistemaExterno
  {
    #region Mensajeros

    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    OUPersonaInternaDC ConsultaMensajero(string documento, bool contratista);

    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    bool ValidaVinculacionMensajero(string documento);

    #endregion Mensajeros
  }
}