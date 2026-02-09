using Framework.Servidor.Servicios.ContratoDatos.ConsultasExternas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Framework.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ICOConsultasComcelSvc
    {
        [OperationContract]
        List<COConsultaComcelResponse> ConsultarGuiaCliente(COConsultaComcelRequest numeroCuenta);
    }
}
