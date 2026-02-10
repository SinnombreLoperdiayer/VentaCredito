using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.OperacionUrbana
{
    public class SolicitudRecogidaRequest
    {

        public int AnioRecogida { get; set; }
        public int MesRecogida { get; set; }
        public int DiaRecogida { get; set; }
        public int HoraRecogida { get; set; }
        public int MinutoRecogida { get; set; }

        public string NombrePersona { get; set; }
        public short CantidadEnvios { get; set; }
        public decimal PesoAproximado { get; set; }
        public string Observaciones { get; set; }
        public string IdLocalidad { get; set; }
        public string Longitud { get; set; }
        public string Latitud { get; set; }
        public string TipoOrigen { get; set; }
        public List<string> Fotografias { get; set; }

        public string TokenDispositivo { get; set; }
        public PAEnumOsDispositivo SistemaOperativo { get; set; }

        public SolicitudRecogidaPeaton RecogidaPeaton { get; set; }


    }

    public class SolicitudRecogidaPeaton
    {
        public string TipoIdentificacion { get; set; }
        public string DocumentoCliente { get; set; }
        public string NombreCliente { get; set; }
        public string DireccionCliente { get; set; }
        public string ComplementoDireccionCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }

        public List<TipoEnvioPeaton> TiposEnvio { get; set; }
    }


    public class TipoEnvioPeaton
    {
        public short IdTipoEnvio { get; set; }
        public string Descripcion { get; set; }
        public short CantidadEnvios { get; set; }
        public decimal PesoAproximado { get; set; }

    }

}
