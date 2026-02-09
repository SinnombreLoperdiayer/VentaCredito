using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModeloResponse.SolicitudRecogidasApp
{
    public class SolicitudRecogidaResponse
    {
        public string NombreCompleto { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string IdMunicipio { get; set; }
        public string NombreMunicipio { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string ComplementoDireccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }


    }
}