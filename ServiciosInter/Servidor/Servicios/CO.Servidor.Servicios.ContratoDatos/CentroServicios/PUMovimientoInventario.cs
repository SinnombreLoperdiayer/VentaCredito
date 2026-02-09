using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PUMovimientoInventario : DataContractBase
    {
        [DataMember]
        public long IdMovimientoInventario { get; set; }

        [DataMember]
        public long IdCentroServicioOrigen { get; set; }

        [DataMember]
        public long IdBodega { get; set; }

        [DataMember]
        public PUCentroServiciosDC Bodega { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public DateTime FechaEstimadaIngreso { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public PUEnumTipoMovimientoInventario TipoMovimiento { get; set; }


    }
}
