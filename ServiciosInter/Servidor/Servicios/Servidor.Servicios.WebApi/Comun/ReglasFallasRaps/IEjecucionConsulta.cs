using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.Comun.ReglasFallasRaps
{
    public interface IEjecucionConsulta
    {
        string EjecucionRegla(ContratoDatos.OperacionUrbana.OUDatosMensajeroDC datosMensajero, ContratoDatos.Admisiones.Mensajeria.ADGuia datosGuia);
    }
}
