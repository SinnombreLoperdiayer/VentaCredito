using CO.Servidor.Servicios.ContratoDatos.ReversionEstados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.ReversionEstadosGuia.ReglasCambioEstado
{
    public interface IReglasCambioEstado
    {
        void EjecucionRegla(ReversionEstado reversionEstado);
    }
}
