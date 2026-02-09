using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public class ReglaMapeoNombreCliente : IEstadoRAPS
    {
        public string EjecutarReglaParametros(RADatosFallaDC datos)
        {
            return datos.NombreCliente;
        }
    }
}
