using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Comun.ReglasFallasRaps
{
    public class ReglaObtenerFechaAdmision : IEjecucionConsulta
    {
        public string EjecucionRegla(ContratoDatos.OperacionUrbana.OUDatosMensajeroDC datosMensajero, ContratoDatos.Admisiones.Mensajeria.ADGuia datosGuia)
        {
            return datosGuia.FechaAdmision.ToString();
        }
    }
}