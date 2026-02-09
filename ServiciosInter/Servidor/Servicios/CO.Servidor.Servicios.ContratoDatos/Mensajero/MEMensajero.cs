using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Mensajero
{
    public class MEMensajero
    {
        public int IdTipoCiudad { get; set; }
        public OUTipoMensajeroDC TipMensajeros { get; set; }

        public EnumEstadoRegistro EstadoRegistro { get; set; }

        public PALocalidadDC LocalidadMensajero { get; set; }

        public SECargo CargoMensajero { get; set; }
        public OUEstadosMensajeroDC Estado { get; set; }


        public string TipoDocumento { get; set; }
        public string IdDocumento { get; set; }
        public string Nombre { get; set; }
        public string Telefono2 { get; set; }
        public OUPersonaInternaDC PersonaInterna { get; set; }
        public int IDCol { get; set; }
        public string Col { get; set; }
        public int IdVehiculo { get; set; }
        public string Placa { get; set; }
        public string CiudadResidencia { get; set; }
        public string IdAgencia { get; set; }
        public PUAgenciaDeRacolDC Agencia { get; set; }
        public short IdTipoMensajero { get; set; }
        public string TipoMensajero { get; set; }
        public DateTime FechaVenciminetoPase { get; set; }
        public string NumeroPase { get; set; }
        public string IdEstado { get; set; }
        public bool EsContratista { get; set; }
        public POTipoContrato TipoContrato { get; set; }
        public bool EsMensajeroUrbano { get; set; }
        public string Celular { get; set; }
        public string TipoVehiculo { get; set; }

        public string CodigoContrato { get; set; }

        public byte[] Foto { get; set; }

        public string TipoPersona { get; set; }

        public POVehiculo Vehiculo { get; set; }

      
    }
}
