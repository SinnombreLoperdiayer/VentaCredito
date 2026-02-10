using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Comun.ReglasFallasRaps
{
    public class ReglaObtenerFechaDescarga : IEjecucionConsulta
    {
        public string EjecucionRegla(ContratoDatos.OperacionUrbana.OUDatosMensajeroDC datosMensajero, ContratoDatos.Admisiones.Mensajeria.ADGuia datosGuia)
        {
            return DateTime.Now.ToString();
        }
    }
}