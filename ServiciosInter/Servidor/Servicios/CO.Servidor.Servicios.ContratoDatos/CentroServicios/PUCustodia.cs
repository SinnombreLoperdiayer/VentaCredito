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
    public class PUCustodia : DataContractBase
    {
        [DataMember]
        public long IdCustodia { get; set; }
        [DataMember]
        public string ContenidoGuia { get; set; }
        [DataMember]
        public CCEnumTipoNovedadGuia TipoNovedad { get; set; }
        [DataMember]
        public string Observacion { get; set; }
        [DataMember]
        public ADGuiaInternaDC GuiaInterna { get; set; }
        [DataMember]
        public int Ubicacion { get; set; }
        [DataMember]
        public int DiasCustodia { get; set; }
        [DataMember]
        public PUEnumTipoUbicacion TipoUbicacion { get; set; }
        [DataMember]
        public string UbicacionDetalle { get; set; }
        [DataMember]
        public PUMovimientoInventario MovimientoInventario { get; set; }
        [DataMember]
        public List<PUAdjuntoMovimientoInventario> ListaAdjuntos { get; set; }
        [DataMember]
        public decimal Peso { get; set; }
        [DataMember]
        public string UsuarioAsignacion { get; set; }
        [DataMember]
        public string TipoEnvio { get; set; }
        [DataMember]
        public string RacolDestino { get; set; }
        [DataMember]
        public string NombreDestinatario { get; set; }
        [DataMember]
        public string RacolOrigen { get; set; }
        [DataMember]
        public string DiceContener { get; set; }
        [DataMember]
        public PUEnumAccionSalidaCustodia AccionSalida { get; set; }

        [DataMember]
        public List<string> AdjuntosPrecargue { get; set; }
    }
}
