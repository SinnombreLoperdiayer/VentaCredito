using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
    /// <summary>
    /// Clase que contiene la información de tarifa internacional
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class TATarifaInternacionalDC : DataContractBase
    {
        [DataMember]
        public ObservableCollection<TAServicioInternacionalPrecioDC> ServicioInternacional { get; set; }

        [DataMember]
        public ObservableCollection<TAImpuestosDC> Impuestos { get; set; }

        [DataMember]
        public ObservableCollection<TAFormaPago> FormasPago { get; set; }

        [DataMember]
        public TAServicioPesoDC ServicioPeso { get; set; }

        [DataMember]
        public TAListaPrecioServicioParametrosDC ListaPrecioParametros { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PorcentajeRecargo", Description = "ToolTipPorcentajeRecargo")]
        public double PorcentajeRecargo { get; set; }

        [DataMember]
        public decimal ValorDolarSinSistema { get; set; }
    }
}