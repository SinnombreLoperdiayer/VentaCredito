using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal
{
    [ServiceContract]
    public interface IMockedService
    {
        [OperationContract]        
        void DoWork();
    }
}
