using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.Versionamiento
{
    /// <summary>
    /// Datos con los que un usuario ingresó al sistema
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class VEDatosIngresoUsuario : DataContractBase
    {

        [DataMember]
        public string Usuario { get; set; }

        [DataMember]
        public SEUbicacionAutorizada LocacionIngreso { get; set; }
    }
}