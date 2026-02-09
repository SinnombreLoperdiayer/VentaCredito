using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
 

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAListarCargoEscalonamientoRapsDC
    {
        [DataMember]
        public string IdCargo { get; set; }
        
        [DataMember]
        public string Descripcion { get; set; }

        //[DataMember]
        //public string CodigoSucursal { get; set; }

        //[DataMember]
        //public bool EnteControl { get; set; }

        //[DataMember]
        //public bool Regional { get; set; }

        
        ////[DataMember]
        ////public string IdProcedimiento { get; set; }

        ////[DataMember]
        ////public string NombreProcedimiento { get; set; }

        ////[DataMember]
        ////public string Idproceso { get; set; }

        ////[DataMember]
        ////public string NombreProceso { get; set; }


        //[DataMember]
        //public string CargoNovasoft { get; set; }
    }
}
