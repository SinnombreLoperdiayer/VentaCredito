using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
    /// <summary>
    /// Clase que contiene la informacion de los tipos de combustible para los carros
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class POTipoCombustibleDC : DataContractBase
    {
        [DataMember]
        public int IdTipoCombustible { get; set; }

        [DataMember]
        public string Descripcion { get; set; }
    }

}
