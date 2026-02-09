using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integraciones.CuatroSieteDos
{
    public enum EnumTipoIdentificacion472: int
    {
        TarjetaIdentidad=12,
        CedulaCiudadania=13,
        TarjetaExtranjeria = 21,
        CedulaExtranjeria = 22,
        Nit=31,
        Pasaporte=41,
        TipoDocumentoExtranjero=42,
        OtroTipoIdentificacion=0
    }

    public class TipoIdentificacion472
    {

        public static string ObtenerTipoIdentificacion472(string tipoIdentificaController)
        {

            switch (tipoIdentificaController.Trim())
            {
                case "CC":
                    return ((int)EnumTipoIdentificacion472.CedulaCiudadania).ToString();
                    

                case "CE":
                    return ((int)EnumTipoIdentificacion472.CedulaExtranjeria).ToString();
                    

                case "NI":
                    return ((int)EnumTipoIdentificacion472.Nit).ToString();
                    

                case "TI":
                    return ((int)EnumTipoIdentificacion472.TarjetaIdentidad).ToString();
                    
                default:
                    return ((int)EnumTipoIdentificacion472.OtroTipoIdentificacion).ToString();
                    
            }
        }
    }

}
