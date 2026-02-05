using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Clientes.Comun;

namespace VentaCredito.Negocio.Interface
{
    public interface IClienteCorporativoNegocio
    {
        void EnviarCorreoCancelacionGuia(CancelacionGuia_Wrapper Cancelacion);
    }
}
