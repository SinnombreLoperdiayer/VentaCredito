using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    /// <summary>
    /// Indica el tipo de entrega que se debe hacer sobre el envío
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADTipoEntrega
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }
    }
}