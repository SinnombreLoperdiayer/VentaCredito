using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using CO.Servidor.Servicios.Contratos;

namespace CO.Servidor.Servicios.Implementacion.Digitalizacion
{
  /// <summary>
  ///Implementacion de Solicitudes Giros
  /// </summary>
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class DIDigitalizacionSvc : IDIDigitalizacionSvc
  {
  }
}