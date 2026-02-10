using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios
{
    /// <summary>
    /// Clase que contiene la información del servicio notificaciones
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class TATarifaNotificacionesDC : DataContractBase
    {
        [DataMember]
        public ObservableCollection<TAFormaPago> FormasPago { get; set; }

        [DataMember]
        public TAServicioPesoDC ServicioPeso { get; set; }

        [DataMember]
        public TAListaPrecioServicioParametrosDC ListaPrecioParametros { get; set; }

        [DataMember]
        public ObservableCollection<TAImpuestosDC> Impuestos { get; set; }
    }
}