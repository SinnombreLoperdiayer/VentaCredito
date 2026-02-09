using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun.Mensajero
{
    public interface IMEFachadaMensajero
    {

        MEMensajero ConsultarMensajero(int idDocumento);

    }
}
