using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Comun.ReglasFallasRaps
{
    public class ReglaObtenerNombreResponsable : IEjecucionConsulta
    {
        public EnumReglasFallas IdentificadorFalla
        {
            get
            {
                return EnumReglasFallas.OBTENER_NOMBRE_RESPONSABLE;
            }
        }
        public string EjecucionRegla(ContratoDatos.OperacionUrbana.OUDatosMensajeroDC datosMensajero, ContratoDatos.Admisiones.Mensajeria.ADGuia guia)
        {
            return string.Format("{0} {0}", datosMensajero.NombreMensajero, datosMensajero.PrimerApellido);
        }
    }
}