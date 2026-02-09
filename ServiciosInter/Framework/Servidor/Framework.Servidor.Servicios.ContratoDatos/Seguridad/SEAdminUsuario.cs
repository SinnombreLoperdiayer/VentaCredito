using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    /// <summary>
    /// Clase que contiene la informacion del usuario
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class SEAdminUsuario : SECredencialUsuario
    {
        [DataMember]
        public List<SETipoIdentificacion> TipoIdentificacionColeccion { get; set; }

        [DataMember]
        public IList<SECargo> CargoColleccion { get; set; }

        [DataMember]
        public List<SERegional> RegionalColeccion { get; set; }

        [DataMember]
        public List<SETipoUsuario> TipoUsuarioColeccion { get; set; }

        [DataMember]
        public List<SEEstadoUsuario> EstadoUsuarioColeccion { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }
    }
}