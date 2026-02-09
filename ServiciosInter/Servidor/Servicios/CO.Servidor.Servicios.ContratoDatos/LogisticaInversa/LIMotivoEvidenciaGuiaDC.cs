using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Cliente.Servicios.ContratoDatos;


namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIMotivoEvidenciaGuiaDC : DataContractBase
    {

        [DataMember]
        public long IdEstadoGuialog { get; set; }

        [DataMember]
        public DateTime? FechaMotivo { get; set; }

        [DataMember]
        public short TipoEvidencia { get; set; }

        [DataMember]
        public long? NumeroEvidencia { get; set; }

        [DataMember]
        public  LIDetalleMotivoGuiaDC Motivo { get; set; }
    }
}
