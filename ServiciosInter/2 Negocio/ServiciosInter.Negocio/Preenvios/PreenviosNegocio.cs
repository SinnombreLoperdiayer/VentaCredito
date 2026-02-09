using ServiciosInter.DatosCompartidos.Wrappers.Preenvios;
using ServiciosInter.Negocio.Interfaces;
using System;
using System.Configuration;

namespace ServiciosInter.Negocio.Preenvios
{
    public class PreenviosNegocio : IPreenviosNegocio
    {
        public PreenvioAdmisionWrapper obtenerPreenvioPorNumero(long PreEnvio)
        {
            String URIPreenvios = ConfigurationManager.AppSettings.Get("UrlServicioPreEnvio");
            throw new NotImplementedException();
        }
    }
}
