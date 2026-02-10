using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios
{

    [DataContract(Namespace = "http://contrologis.com")]
    public class TAPrecioRangoImpuestosDC : DataContractBase
    {
        [DataMember]
        public IEnumerable<TAPrecioRangoDC> LstPrecioRango { get; set; }

        [DataMember]
        public TAServicioImpuestosDC Impuestos { get; set; }
    }
}