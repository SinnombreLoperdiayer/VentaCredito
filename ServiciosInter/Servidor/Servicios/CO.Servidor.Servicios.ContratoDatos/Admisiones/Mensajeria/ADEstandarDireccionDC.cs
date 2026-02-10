using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract]
    public class ADEstandarDireccionDC
    {
        [DataMember]
        public int? IdAbreviaturaDireccion { get; set; }

        [DataMember]
        public string VariacionDireccion { get; set; }

        [DataMember]
        public string AbreviacionDireccion { get; set; }

    }
}
