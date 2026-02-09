using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CO.Servidor.Servicios.Contratos
{
    public interface IMEConfiguracionMensajeroSvc
    {
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<MECargo> ObtenerCargos();
    }
}
