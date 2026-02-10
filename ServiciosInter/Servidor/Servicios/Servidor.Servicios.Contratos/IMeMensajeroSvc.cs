using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CO.Servidor.Servicios.Contratos
{
    public interface IMeMensajeroSvc
    {
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        MEMensajero ConsultarMensajero(int idDocumento);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearMensajero(MEMensajero mensajero);

    }
}
