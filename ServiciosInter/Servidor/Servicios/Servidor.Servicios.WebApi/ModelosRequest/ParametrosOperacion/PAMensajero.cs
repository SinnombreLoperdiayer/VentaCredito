using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.ParametrosOperacion
{
    public class PAMensajero
    {
        public string EstadoRegistro { get; set; }

        public long Agencia { get; set; }
        public int IdCargo { get; set; }
        public string DescripcionCargo { get; set; }
        public string TipoDocumento { get; set; }
        public string IdDocumento { get; set; }
        public string Nombre { get; set; }
        public string Telefono2 { get; set; }
        public PAPersonaInterna PersonaInterna { get; set; }

        public string IdLocalidad { get; set; }
        public int IDCol { get; set; }
        public string Col { get; set; }
        public int IdVehiculo { get; set; }
        public string Placa { get; set; }
        public string CiudadResidencia { get; set; }
        public string IdAgencia { get; set; }
        public long IdCentroServicio { get; set; }

        public string NombreCentroServicio { get; set; }
        public short IdTipoMensajero { get; set; }
        public string TipoMensajero { get; set; }
        public DateTime FechaVenciminetoPase { get; set; }
        public string NumeroPase { get; set; }
        public string IdEstado { get; set; }
        public bool EsContratista { get; set; }
        public int IdTipoContrato { get; set; }

        public string Descripcion { get; set; }
        public bool EsMensajeroUrbano { get; set; }
        public string Celular { get; set; }
        public string TipoVehiculo { get; set; }

        public string CodigoContrato { get; set; }

    }
}