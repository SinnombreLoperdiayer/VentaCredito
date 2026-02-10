using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.CentroAcopio
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAAsignacionGuiaDC : DataContractBase
    {

        [DataMember]
        public PUMovimientoInventario MovimientoInventario { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }
        [DataMember]
        public long IdAdmision { get; set; }

        [DataMember]
        public long IdCentroServicioCOL { get; set; }
        [DataMember]
        public string NombreCentroServicioCOL { get; set; }
        [DataMember]
        public string IdMunicipioCOL { get; set; }

        [DataMember]
        public long IdCentroServicioDestino { get; set; }
        [DataMember]
        public string NombreCentroServicioDestino { get; set; }

        [DataMember]
        public decimal Peso { get; set; }
        [DataMember]
        public string TipoEnvio { get; set; }
        [DataMember]
        public string DiceContener { get; set; }
        [DataMember]
        public string DireccionDestino { get; set; }

        [DataMember]
        public DateTime? FechaAsignacion { get; set; }
        /// <summary>
        /// Bandera si Esta asignada al CentroServicio
        /// </summary>
        [DataMember]
        public bool EstaAsignada { get; set; }

        [DataMember]
        public OUEnumValidacionDescargue Respuesta { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public ADTrazaGuia Estado { get; set; }

        [DataMember]
        public PALocalidadDC LocalidadDestino { get; set; }

        [DataMember]
        public string BolsaSeguridad { get; set;}

        [DataMember]
        public string CiudadDestino { get; set; }

        [DataMember]
        public string TipoCliente { get; set; }


    }

}
