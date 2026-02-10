using Framework.Servidor.ConsultasExternas;
using Framework.Servidor.Servicios.ContratoDatos.ConsultasExternas;
using Framework.Servidor.Servicios.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace Framework.Servidor.Servicios.Implementacion.ConsultasExternas
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class COConsultasComcelSvc : ICOConsultasComcelSvc
    {

        public List<COConsultaComcelResponse> ConsultarGuiaCliente(COConsultaComcelRequest numeroCuenta)
        {           
//		return  new List<COConsultaComcelResponse>();
            return COConsultasComcel.Instancia.ConsultarGuiaCliente(numeroCuenta);
        }
    }
}
