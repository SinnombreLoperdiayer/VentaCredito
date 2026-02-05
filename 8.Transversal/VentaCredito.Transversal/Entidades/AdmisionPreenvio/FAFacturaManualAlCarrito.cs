using Servicio.Entidades.Facturacion;
using Servicio.Entidades.Tarifas.Precios;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class FAFacturaManualAlCarrito
    {
        public int IdCliente { get; set; }
        [DataMember]
        public int IdContrato { get; set; }
        [DataMember]
        public int IdSucursalContrato { get; set; }
        [DataMember]
        public string IdLocalidad { get; set; }
        [DataMember]
        public string Usuario { get; set; }
        [DataMember]
        public TAEnumFormaPago FormaPago { get; set; }
        [DataMember]
        public EPlazoPago PlazoPago { get; set; }
        [DataMember]
        public decimal ValorDescuentos { get; set; }
        [DataMember]
        public ObservableCollection<FAConceptoFacturaDC> ConceptosFactura { get; set; }
    }
}
